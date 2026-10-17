# PMSHub — kế hoạch giảm CPU, RAM, I/O và điểm nghẽn

Ngày lập: 17/10/2026. Phạm vi: phiếu cân HQ online, backup HQ, CheckInOut và ca chung qua nhiều cân.

## 1. Mục tiêu và phạm vi bất biến

Giảm tài nguyên tiêu thụ cho cùng một khối lượng công việc, giữ nguyên kết quả và các bước xử lý nghiệp vụ. Đây là kế hoạch, chưa triển khai tối ưu lên mã chạy hoặc database.

Mốc quan sát đầu lượt: HEAD `c0844df` cùng các thay đổi tích hợp ca chung trong worktree. Source tiếp tục có chỉnh sửa song song trong khi lập tài liệu, nên đây không phải xác nhận nghiệm thu trạng thái worktree mới nhất. Trước khi triển khai phải chốt một bản chuẩn đầy đủ, bao gồm thay đổi đang làm dở; không dùng kết quả build trước đó để nghiệm thu bản chuẩn mới này.

Các điều kiện bắt buộc:

- Giữ nguyên điều kiện nhận/từ chối phiếu, Id, chống trùng, trạng thái và cách gửi lại.
- Giữ nguyên quy tắc thẻ cá nhân/thẻ nhóm, Vào/Ra, ca thường/đêm, hết hạn, chuyển máy, ca chung và snapshot thành phẩm/xưởng.
- Giữ nguyên công thức, phạm vi dữ liệu và thứ tự thành phẩm của tổng rổ/kg. Không gộp tổng gross của phiếu online với tổng net của tổng kết ca.
- Giữ nguyên các mốc kiểm tra trước/sau khi cân ổn định, thứ tự đọc/ghi phục vụ quyết định nghiệp vụ, thứ tự khóa và ranh giới transaction. Không bỏ lần kiểm tra trong khóa.
- Giữ nguyên thời điểm xác nhận sau lưu thành công, nhánh lỗi, cấu trúc/kiểu dữ liệu JSON, thông báo firmware, số lượng và thứ tự thông báo có thể quan sát được.
- Giữ nguyên endpoint, public API, giao diện, export, tần suất tác vụ nền và các giới hạn dữ liệu hiện có.

Thay đổi có thể làm khác các điều kiện này không thuộc đợt tối ưu. Nếu cần sửa một lỗi nghiệp vụ sẵn có, tách thành công việc khác; không sửa ngầm trong patch hiệu năng.

## 2. Điểm nghẽn và tài nguyên cần đo

Đây là các điểm nghi ngờ dựa trên source. Chưa có số đo SQL/tải thực để khẳng định thứ hạng ảnh hưởng.

| Điểm nghi ngờ | File / phương thức | CPU | RAM | I/O và thời gian chờ |
| --- | --- | --- | --- | --- |
| Chốt ca xét toàn bộ lịch sử mỗi phút, có LEAD theo thẻ/máy | `ScaleAttendanceDayCloseService.CloseSql` | Tính thứ tự và lọc lịch sử | Memory grant, nguy cơ sort spill | Đọc lịch sử lớn, cạnh tranh với ghi mới |
| Tổng ngày COUNT/SUM được tính lại sau mỗi phiếu và gửi lại | `HQ_PhieuCan.GetOnlineTotals`, `CommitService.HqResponse` | Tổng hợp lại cùng tập dữ liệu | Bộ nhớ/cache của SQL | Reads tăng theo số phiếu trong ngày |
| Tổng kết ca chung dùng EXISTS liên kết lượt tham gia | `ScaleSharedAttendancePolicy.ShiftWeights`, `ScaleAttendanceProcessor.Success` | EXISTS/GROUP BY; có đổi mã máy số sang chuỗi | Memory grant cho tổng hợp | Có thể đọc dư nếu index/plan không phù hợp |
| Tìm ca đang mở có NOT EXISTS và nhánh OR | `ScaleAttendanceTransferService.OpenShifts` | Tìm sự kiện kế tiếp | Materialize entity đầy đủ | Nhiều dòng lịch sử, lookup/scan tùy plan |
| Backup duyệt và sắp xếp lại tập scan cho từng phiếu | `HqWeightBackupProcessor.Process` | Lặp tìm kiếm theo N phiếu và M scan | Cấp phát lặp, tải dữ liệu rộng | Năm preload, các query fallback |
| Backup lấy khóa riêng cho từng Id | `HqWeightBackupProcessor.Process` | Chi phí xử lý command | Giữ dữ liệu lô khi chờ | N lần gọi khóa SQL và thời gian giữ khóa thẻ |
| Khóa chung HQ chứa SELECT, INSERT và callback giao diện | `CommitService.CommitHqLite`, `MayCansService.AddItem` | Subscriber/logic trong vùng khóa | Các request và transaction đang chờ | Máy khác phải đợi vùng khóa chung |
| Đọc lại dữ liệu ca/phiếu đã có ở một số nhánh | `ScaleSharedAttendanceService.Process`, `ScaleAttendanceProcessor.Success`, `ScaleProductionAttendancePolicy.GetError` | Dựng query/materialize lặp | Allocation cho entity và collection | Nhiều command trong request |

RAM cao riêng lẻ chưa chứng minh rò rỉ. Theo dõi allocation, live heap, GC và xu hướng tăng sau tải. SQL giữ RAM để cache có thể giúp giảm I/O; không ép giảm cache hoặc gọi GC cưỡng bức để làm số RAM đẹp hơn.

## 3. File thay đổi trong lượt lập kế hoạch

Chỉ thêm tài liệu này. Giữ nguyên các file C#/SQL đang được chỉnh sửa ngoài lượt lập kế hoạch.

Tài liệu đối chiếu: [Báo cáo rà soát trước đó](PMSHub-Danh-gia-hieu-nang-va-ke-hoach-20261017.md). Báo cáo cũ mô tả snapshot trước khi có phần tích hợp ca chung hiện tại; số lần query và kết quả build cũ không đại diện cho source đang thay đổi.

## 4. Kế hoạch thực hiện theo thứ tự

### Bước 0 — Chốt bản chuẩn và đo đường cơ sở

Thực hiện trước khi sửa mã:

1. Chốt đầy đủ source, DLL, cấu hình và bộ tình huống của bản chuẩn. Chạy trên bản sao DB tương ứng; không tự dùng Hub thử nghiệm trỏ DB sản xuất.
2. Ghi lại đầu vào, phản hồi, dữ liệu DB sau xử lý và thứ tự sự kiện của từng nhánh. Dữ liệu giả lập cần có cả trường hợp khớp/không khớp, không lặp lại lỗi benchmark dùng lookup rỗng.
3. Đo riêng Hub và SQL. Một mã tương quan cho request nối được thời gian nhận, chờ khóa, SQL, commit, callback và gửi phản hồi; không ghi bí mật hoặc payload đầy đủ không cần thiết.
4. Đếm đủ command EF, Dapper, applock và begin/commit. Chỉ đo EF sẽ bỏ sót Dapper và không thể kết luận số lượt SQL toàn request.
5. Dùng metadata/DMV/Query Store sẵn có để kiểm tra index, plan và wait; không tự bật RCSI, xóa plan cache, rebuild toàn bộ index hoặc thay cấu hình SQL.

Đo p50/p95/p99, số lượt hoàn tất/giây, CPU ms/lượt ở Hub và SQL, logical reads/lượt, physical I/O, log bytes/flush, allocation/lượt, live heap, GC pause, thread pool, thời gian chờ/giữ khóa, timeout và deadlock. Lấy mẫu có giới hạn để công cụ đo không tự tạo tải đáng kể.

Kết quả đầu ra: bảng đường cơ sở theo từng luồng và danh sách query/vùng xử lý có chi phí cao nhất. Xếp lại thứ tự bước 1–4 theo số đo này. Đối chiếu các tối ưu đã có trong bản chuẩn mới để chỉ đo/kiểm chứng phần đã làm, không tạo patch trùng với thay đổi song song.

### Bước 1 — Giảm I/O SQL bằng index phù hợp, giữ nguyên câu truy vấn nghiệp vụ

Đây là đợt can thiệp được ưu tiên sau khi đo vì chưa phải đổi các nhánh C# hoặc transaction. Chỉ chọn index có ích qua actual plan; không tạo toàn bộ danh sách sau theo mặc định.

| Truy vấn | Phương án cần thử trên DB thử nghiệm | Điều kiện giữ lại |
| --- | --- | --- |
| Tổng ngày HQ | Kiểm tra index hiện có `(Ngay, MayCan, TrongLuong) INCLUDE (MaNhanVien)` và độ phủ thực tế | Giảm đọc bảng/lookup; không tăng chi phí INSERT quá mức so với lợi ích |
| Tổng kết ca theo thẻ và thời gian, gồm ca chung | So sánh phương án bắt đầu bằng `(TheIdNhanVien, NgayGio)` với `(MayCan, TheIdNhanVien, NgayGio)`; include các cột tổng hợp cần thiết; xem xét filtered index đúng `Status/ChiSanLuong` | Plan phù hợp cả ca một cân lẫn ca chung; không bỏ phiếu của máy tham gia |
| Ca mở, lượt gần nhất | Đánh giá index theo thẻ/máy/thời gian/Id; chỉ thêm đường tra `VaoId` nếu cần | Giảm reads của nhánh NOT EXISTS, giữ nguyên truy vấn và kết quả |
| Lượt tham gia ca chung | Kiểm tra index đã có `(VaoId, MaSoMay, ThoiGian) INCLUDE (TrangThai, MaThanhPham)`; chỉ bổ sung nếu plan chỉ ra thiếu | Phục vụ đúng EXISTS/LatestScanTime; tránh thêm index gần trùng |
| Xưởng tại thời điểm cân | Tận dụng index/unique index `(MayCan, TuThoiGian)` trước khi thêm index phủ | Giảm lookup nếu đáng kể; không có index dư chỉ khác tên |

Kiểm tra kiểu tham số, độ dài và collation với schema thực. Riêng `scan.MaSoMay.ToString() == weight.MayCan` trong ca chung cần xem SQL dịch ra và plan; không tự đổi sang parse số vì mã có số 0 đầu, khoảng trắng hoặc mã cũ có thể đổi nghĩa so sánh.

Thực hiện từng index, đo cả đọc và ghi, dung lượng và log phát sinh. Index không thay đổi công thức nghiệp vụ nhưng vẫn có thể làm chậm insert hoặc gây chờ lúc tạo; phải có cửa sổ triển khai và cách gỡ riêng index mới nếu không đạt.

Không đổi kiểu dữ liệu, thêm cột/bảng, constraint hoặc cập nhật dữ liệu nghiệp vụ trong bước này.

### Bước 2 — Giảm CPU và allocation trong bộ nhớ, giữ nguyên các điểm đọc/ghi

Phạm vi đầu tiên: `HqWeightBackupProcessor`, các hàm thuần xử lý dữ liệu đã tải và projection chỉ đọc.

- Nhóm `preloadedScans` theo mã thẻ một lần trong request; sắp xếp mỗi nhóm một lần, sau đó tìm sự kiện gần nhất đúng thời điểm phiếu. Giữ nguyên cách so sánh mã, thứ tự thời gian/Id khi bằng nhau và kết quả fallback. Không mặc định thứ tự collation SQL giống thứ tự .NET.
- Tra cứu lịch sử xưởng trên tập đã tải bằng cấu trúc có thứ tự thay vì lọc/sắp xếp toàn tập cho mỗi phiếu; giữ chính xác điều kiện thời điểm và trường hợp không tìm thấy lịch sử.
- Tái sử dụng tập Id/mã và kết quả tính thuần trong cùng request khi đầu vào không thay đổi; bỏ các collection trung gian hoặc lần duyệt thừa sau khi kiểm tra chỗ dùng.
- Với query chỉ đọc, chọn đúng các cột cần dùng thay vì materialize entity đầy đủ, sau khi liệt kê đủ trường của mọi nhánh. Giữ số lần query và vị trí query trong bước đầu để không thay đổi góc nhìn dữ liệu đồng thời.
- Những query đã `AsNoTracking` thì giữ nguyên. Không thêm dictionary toàn cục, cache có TTL, thay số dòng giao diện hoặc cắt dữ liệu lô/lịch sử để giảm RAM.

Mục tiêu đo: CPU ms và bytes cấp phát trên một phiếu/lô giảm, heap không tăng dần qua nhiều vòng tải, GC ít hơn. Lookup cũng dùng thêm RAM; chỉ giữ phương án có lợi với kích thước lô thực tế, không dựa vào phép đo một trường hợp cực đoan.

### Bước 3 — Giảm truy vấn lặp tại cùng một điểm kiểm tra

Phạm vi: `ScaleAttendanceProcessor.Success`, `ScaleSharedAttendanceService.Process`, `ScaleProductionAttendancePolicy.GetError` và các helper nội bộ.

- Lập danh sách command theo từng nhánh trước. Xác định query nào đọc lại đúng bản ghi đã có và được bảo vệ tương đương trong cùng vùng khóa/transaction.
- Truyền dữ liệu đã có cho helper nội bộ nếu chứng minh vẫn đúng ở chính điểm đó; phân biệt rõ request mới, cùng Id gửi lại, quẹt lặp khác Id, ca gốc và lượt tham gia. Giữ public API.
- Không tái sử dụng kết quả đọc trước khi lấy khóa để bỏ lần đọc quyết định trong khóa. Không tái sử dụng trạng thái trước khi chờ cân cho lần kiểm tra khi cân ổn định.
- `OpenShifts` hiện có thể được gọi trước và sau khi lấy khóa máy ở luồng ca chung. Không mặc định lần sau là thừa; cần chứng minh không có đầu ghi nào làm thay đổi trạng thái giữa hai mốc đó.
- Không cache kết quả SQL xuyên request hoặc memoize tùy tiện dưới ReadCommitted. Việc cùng transaction không tự bảo đảm cùng snapshot cho mọi statement.
- Tối ưu câu truy vấn tương đương chỉ khi có bài so sánh tập kết quả, NULL, trùng thời gian, collation và số dòng. Không đổi EXISTS sang JOIN làm nhân đôi phiếu tổng hợp.

Giữ nguyên thứ tự các kiểm tra và lỗi ưu tiên. Nếu không chứng minh được tương đương khi có request cạnh tranh thì chỉ tối ưu plan/index, không loại query đó.

### Bước 4 — Giảm chi phí chốt ca, giữ nguyên lịch chạy và kết quả đánh dấu

Phạm vi: `ScaleAttendanceDayCloseService.CloseSql`.

1. Đo cost của CTE/LEAD và số dòng thật được cập nhật; thử index trước để giảm sort/lookup.
2. Nếu vẫn đắt, viết phương án SQL lấy tập ca ứng viên rồi tìm đúng sự kiện kế tiếp theo thứ tự `(ThoiGian, Id)` và cách chuẩn hóa mã thẻ hiện có.
3. So sánh chính xác tập Id được đánh dấu với SQL gốc trên nhiều bộ lịch sử và cùng mốc thời gian. Kiểm tra thứ tự cạnh tranh với ghi offline và lượt quẹt đang phát sinh.
4. Chỉ thay khi kết quả và độ an toàn ghi đồng thời tương đương. Nếu chưa chứng minh được, giữ SQL gốc và dùng tối ưu index.

Không giảm lịch chạy một phút, không chỉ đọc hôm nay, không dùng mốc xử lý làm bỏ sót dữ liệu gửi muộn, không cắt lịch sử, không tự đóng ca, không đổi chính sách nhiều instance. Các thay đổi đó có thể làm khác thời điểm hoặc kết quả nghiệp vụ.

### Bước 5 — Xử lý thời gian chờ sau khi đã giảm công việc thừa

Phạm vi đo: `_lockItems`, applock theo thẻ/máy/Id, các command đồng bộ và callback UI.

- Giữ nguyên vùng khóa, thứ tự khóa, timeout và transaction trong đợt này. SQL/CPU trong vùng khóa chạy nhanh hơn sẽ giảm thời gian chờ mà chưa cần đổi luồng.
- Đo riêng `ItemUpdated` và các subscriber. Có thể giảm tra cứu/cấp phát bên trong nếu vẫn giữ nguyên số lần, thứ tự và thời điểm gọi callback; chưa chuyển thông báo sang queue hoặc đổi debounce.
- Async chỉ là phương án sau khi đã biết có nghẽn thread pool. Nếu thực hiện, phải await tuần tự ở các điểm I/O tương ứng, giữ hành vi lỗi và thứ tự; không song song hóa các query trên cùng DbContext, không dùng Task.Run/fire-and-forget để gửi ACK sớm.
- Gom các lệnh applock thành ít command chỉ được cân nhắc khi chứng minh giữ nguyên thứ tự, timeout, lỗi và hành vi transaction. Không nằm trong các patch đầu vì gộp command có thể thay đổi cách báo lỗi/đợi khóa.

Async giảm thời gian chiếm luồng chờ I/O, không tự làm SQL ít CPU/reads hơn. Nếu còn nghẽn ở khóa chung sau các bước trên, báo số đo và lập phương án riêng; không tự chuyển sang khóa theo máy hoặc nới phạm vi khóa.

## 5. Giải thích tác dụng và thứ tự ưu tiên

| Nhóm tài nguyên | Thay đổi ưu tiên | Giới hạn cần hiểu |
| --- | --- | --- |
| CPU SQL | Index phù hợp, giảm sort/lookup, tối ưu SQL chốt ca tương đương | Thêm nhiều index làm ghi tốn CPU hơn |
| CPU Hub | Nhóm scan một lần, bớt duyệt/sắp xếp/cấp phát lặp | Không loại các lần kiểm tra nghiệp vụ bắt buộc |
| RAM Hub | Projection gọn, ít collection trung gian, lookup chỉ sống trong request | Lookup có overhead; phải đo heap/GC, không chỉ nhìn Task Manager |
| RAM SQL/tempdb | Plan ít sort/hash/spill, chỉ đọc tập cần thiết theo đúng điều kiện | Không ép giảm cache SQL để đổi lấy I/O lớn hơn |
| I/O dữ liệu | Index phủ vừa đủ, ít đọc dư và ít query lặp có thể loại an toàn | Tổng ngày vẫn đọc các mục cần tổng hợp; chưa chuyển sang tổng tích lũy |
| I/O log ghi | Tránh index dư và thao tác ghi không cần thiết đã được chứng minh | Giữ nguyên mọi bản ghi/audit và ranh giới commit, không giảm độ bền dữ liệu |
| Chờ khóa/thread pool | Giảm chi phí trong vùng khóa hiện tại; async có kiểm chứng ở bước sau | Giữ khóa, không tăng timeout để che điểm nghẽn |

Thứ tự dự kiến: đo → index/plan → CPU/RAM trong request → query lặp đủ điều kiện → chốt ca tương đương → thời gian chờ. Nếu số đo cho thấy tác vụ chốt ca chiếm phần lớn tài nguyên thì đẩy phần index/SQL tương đương của nó lên trước. Mỗi patch chỉ giải quyết một nguyên nhân; không refactor hoặc đổi kiến trúc.

## 6. Xác minh không đổi nghiệp vụ và quản lý rủi ro

### Ma trận đối chiếu bản chuẩn và bản tối ưu

| Nhóm | Tình huống tối thiểu | Dữ liệu/kết quả phải đối chiếu |
| --- | --- | --- |
| Cân HQ | Chờ cân, ổn định, thẻ cá nhân/nhóm, thiếu danh mục, tare | Phiếu được nhận/từ chối, trường lưu, phản hồi, tổng gross |
| Chống trùng | Gửi lại đúng Id, khác payload, hai request cùng Id | Số dòng, dữ liệu gốc, trạng thái/ACK và lỗi như bản chuẩn |
| Ca | Vào/Ra, quẹt lặp, hết hạn, quên Ra, ca đêm qua ngày | Mốc giờ gốc, loại ca, VaoId, Isforget, thông báo |
| Chuyển máy/ca chung | Cùng/khác thành phẩm, quay lại cân, nhiều lượt tham gia | Ca gốc, membership, snapshot, phạm vi phiếu được tính và tổng net |
| Lịch sử | Chuyển xưởng, khoảng thời gian bằng nhau, mã khác case/khoảng trắng | Cách tra mã và thứ tự chọn bản ghi như bản chuẩn |
| Backup | Mới/trùng/hỗn hợp, thiếu metadata, nhiều ngày, gửi muộn | Nguyên tắc cả lô, thứ tự ACK, số phiếu và tổng kg |
| Lỗi/đồng thời | Cùng thẻ, khác máy, hai instance, SQL timeout, mất ACK sau commit, callback lỗi | Không có khác biệt mới về dữ liệu, rollback, retry và phản hồi |
| Tác vụ nền | Ca thiếu Ra lâu ngày, ca đêm, dữ liệu sai cũ, offline gửi sau | Cùng tập Id được đánh dấu ở cùng thời điểm kiểm tra |

Nếu bản chuẩn có lỗi đã biết, ghi riêng kết quả và phạm vi; không dùng mục tiêu “sửa lỗi luôn” để thay đổi nghiệp vụ trong patch hiệu năng.

### Bài tải và điều kiện nghiệm thu

- Cùng snapshot DB, cấu hình, phần cứng, lưu lượng, dữ liệu và điều kiện cache khi so sánh. Warm-up trước; chạy nhiều vòng và đảo thứ tự hai bản để giảm sai lệch.
- Tách bài cho online, Vào/Ra/ca chung, backup và tải hỗn hợp với tác vụ nền chạy đúng lịch. Dùng mức thấp, mức đỉnh thực tế và mức gấp đôi tải đỉnh đã ghi nhận; không tự coi 50 máy là đủ cho mọi xưởng.
- So sánh CPU ms/request, logical reads/request, allocation/request và chi phí ghi; xem đồng thời p95/p99, throughput và lỗi. CPU thấp hơn vì xử lý ít phiếu hơn hoặc bị nghẽn không được xem là đạt.
- So sánh live heap sau các chu kỳ tải và thời gian nghỉ tương đương; không gọi GC cưỡng bức trong vận hành. Đo cả tempdb spill và file log SQL, không chỉ I/O của tiến trình Hub.
- Điều kiện cứng: kết quả nghiệp vụ/luồng quan sát được khớp bản chuẩn; không có hồi quy mất/trùng dữ liệu hoặc ACK; không tăng deadlock/timeout; đạt lợi ích tài nguyên hoặc độ trễ đã đăng ký cho từng patch mà không làm xấu đáng kể phần còn lại.
- Chưa đặt phần trăm giảm CPU/RAM/I/O hoặc số ms cam kết khi chưa có đường cơ sở. Ngưỡng tài nguyên và độ trễ được chốt từ tải thực và thời gian chờ firmware trước khi chạy nghiệm thu.
- Thử trên DB bản sao, sau đó pilot có giám sát theo nhóm cân. Mỗi patch có bản DLL gốc và cách quay lại; index mới có script gỡ riêng, không đụng index cũ. Không rollback bằng cách xóa dữ liệu nghiệp vụ.

Những việc ngoài phạm vi: Redis/cache tổng có TTL, bảng tổng tích lũy, thêm queue ghi nền, ACK trước commit, đổi isolation/RCSI, chia lô làm đổi atomicity, nới khóa, giới hạn payload mới, xóa/archiving lịch sử, đổi kiểu mã máy, đổi nghiệp vụ chuyển máy và thay framework.

## 7. Kết quả kiểm tra của lượt này

Đã đọc trạng thái Git và các phương thức liên quan, gồm luồng ca chung mới. Chưa sửa C#/cấu hình/schema, chưa chạy SQL chẩn đoán hoặc triển khai tối ưu.

Không chạy lại build/test trong lượt lập kế hoạch vì chưa có patch mã nguồn của kế hoạch và source đang có thay đổi song song. Kết quả build/test của lượt rà soát trước chỉ áp dụng snapshot trước đó. Bước 0 bắt buộc build và chạy lại toàn bộ kiểm thử liên quan sau khi chốt bản chuẩn, rồi bổ sung bài SQL integration/tải và ma trận tương đương nêu trên.
