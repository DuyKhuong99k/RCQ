# PMSHub: đánh giá hiệu năng HQ_PhieuCan và CheckInOut — 17/10/2026

## 1. Mục tiêu và kết luận

Rà soát luồng cân HQ online, khôi phục phiếu cân HQ và chấm công Vào/Ra; đề xuất tối ưu tiếp theo với thay đổi nhỏ, giữ nghiệp vụ hiện có.

**Ràng buộc theo yêu cầu người dùng:** không thay đổi nghiệp vụ, chức năng, công thức tổng hợp, chống trùng, quy tắc ca/chuyển máy, định dạng export hoặc giao thức firmware. Chỉ nhận tối ưu khi cùng dữ liệu và thời điểm nghiệp vụ cho cùng kết quả trước/sau. Không tự thay isolation, transaction hoặc nới khóa để đổi lấy tốc độ.

**Có cải thiện trong mã nguồn, rõ nhất ở backup theo lô. Chưa đủ bằng chứng để kết luận hiệu năng insert HQ_PhieuCan và check-in/out trên hệ thống đang vận hành đã tốt.** Luồng online còn nhiều lần gọi SQL, khóa chung giữa các máy và truy vấn tổng hợp tăng chi phí theo lượng dữ liệu. Bộ benchmark hiện tại chưa đo SQL thực.

Phạm vi kiểm tra: source tại commit `ce8a0f7`, đối chiếu các commit tối ưu `04d320a`, `22ef2af`, `2e42191`, `316f586`, chạy build Release và các kiểm thử liên quan. Chưa xác nhận bản DLL đã cài, index/RCSI thực tế, execution plan, độ trễ mạng SQL hoặc tải từ cân thật. Không khởi chạy PMSHub và các hosted service, không kết nối/ghi database sản xuất, không gửi lệnh tới thiết bị.

### Những cải thiện có thật trong source

| Hạng mục | Bằng chứng | Đánh giá |
| --- | --- | --- |
| Tái sử dụng nhân viên đã đọc trong luồng cân HQ | `CommitService.cs:7519,7565`; `ScaleProductionAttendancePolicy.cs:17` | Khi có nhân viên hợp lệ, bỏ được hai SELECT danh mục trong mỗi lần gọi `GetError`. Đây là giảm số truy vấn, chưa phải số đo độ trễ. |
| Backup tải trước dữ liệu theo lô | `HqWeightBackupProcessor.cs:122–149` | Năm nhóm dữ liệu được đọc trước vòng lặp: phiếu đã lưu, thẻ, nhân viên, lịch sử xưởng, lịch sử chấm công. Giảm rõ kiểu truy vấn từng phiếu trong trường hợp dữ liệu đầy đủ. |
| Backup chuyển Serializable sang ReadCommitted | `HqWeightBackupProcessor.cs:109` | Giảm phạm vi bảo vệ của isolation; vẫn có khóa ứng dụng theo thẻ và Id. Phải kiểm thử đồng thời với mọi đầu ghi, không mặc nhiên coi việc giảm isolation là đã an toàn trên dữ liệu thật. |
| OpenShifts bổ sung điều kiện thẻ trong truy vấn con | `ScaleAttendanceTransferService.cs:38` | Có thể giảm phạm vi truy cập khi tra một thẻ; lợi ích thực phải đọc plan. Khi `card == null`, bộ lọc mới không thu hẹp tập dữ liệu. |
| Có script index và RCSI | `sql/20261017-ToiUuHieuNangHQPhieuCan.sql` | Có file không chứng minh đã chạy trên DB. Index tổng ngày có ích cho đúng truy vấn đó; chưa bao phủ hết đường check-out. |

## 2. Nguyên nhân / vấn đề còn lại

Các mức ưu tiên dưới đây xếp theo ảnh hưởng dự kiến và sự rõ ràng trong source; chưa phải xếp hạng theo số đo production.

### P1 — Tác vụ chốt ca xét lại toàn bộ quá khứ mỗi phút

- File/lớp: `Services/ScaleAttendanceDayCloseService.cs`, `CloseSql`, `ExecuteAsync`.
- Vị trí: dòng 26–50, 79–80.
- Nguyên nhân: CTE nhận mọi sự kiện `ThoiGian < now`, dùng hai phép `LEAD` theo thẻ/máy, rồi mới lọc ca cần đánh dấu ở UPDATE. `Isforget <> 1` ở cuối không thu hẹp tập đầu vào cho phép tính thứ tự. Source không có cận dưới lịch sử. Mức quét/sắp xếp thực tế cần xác nhận bằng plan và logical reads.
- Ảnh hưởng dự kiến: chi phí tăng theo lịch sử, cạnh tranh CPU/I/O với các lượt quẹt và insert; mỗi instance Hub lại chạy dịch vụ này.
- Cách xác minh: đo riêng command chốt ca, CPU, logical reads, số dòng đọc/cập nhật và wait khi bật/tắt dịch vụ trên DB thử nghiệm. So sánh DB có lịch sử ngắn và dài với cùng tải phát sinh mới.
- Hướng sửa: chọn các ca Vào chưa xử lý cần đối soát làm tập ứng viên, sau đó tìm sự kiện kế tiếp tương đương quy tắc hiện tại. Tách quét định kỳ khỏi lượt đối soát dữ liệu offline gửi muộn; bổ sung cơ chế chỉ một instance thực hiện nếu đang chạy nhiều Hub.
- Rủi ro: không được chỉ thêm `WHERE ThoiGian >= hôm_nay` hoặc lọc mất bản ghi Ra trước khi tính `LEAD`. Phải giữ ca đêm, ca thiếu Ra lâu ngày và khả năng phát hiện dữ liệu gửi muộn.

### P1 — Check-out cần truy vấn HQ theo thẻ và khoảng thời gian, khác với index tổng ngày

- File/phương thức: `Services/ScaleAttendanceProcessor.cs`, `Success`, dòng 212–229.
- Nguyên nhân: lúc Ra, truy vấn lọc `MayCan`, `TheIdNhanVien`, `NgayGio` trong ca, `ChiSanLuong = 1`, `Status = 1`, rồi GROUP BY thành phẩm và tính `SUM(TrongLuong - TrongLuongTare)`.
- Script hiện tại tạo index HQ trên `(Ngay, MayCan, TrongLuong) INCLUDE (MaNhanVien)`. Truy vấn checkout không có điều kiện `Ngay` và cần các cột khác; index này chưa phù hợp để phục vụ toàn bộ truy vấn checkout.
- Cách xác minh: lấy actual execution plan và `STATISTICS IO/TIME` của truy vấn Ra trên DB thử nghiệm, với ca có nhiều phiếu, nhiều thành phẩm và ca đêm. Kiểm tra scan, sort, key lookup và chuyển kiểu ngầm.
- Hướng sửa: thử một index theo `(MayCan, TheIdNhanVien, NgayGio)`, include `MaThanhPham`, `TrongLuong`, `TrongLuongTare`; cân nhắc filtered index cho `ChiSanLuong = 1 AND Status = 1`. Chỉ chốt thiết kế sau khi kiểm tra index sẵn có và plan.
- Rủi ro: mỗi index tăng chi phí ghi và dung lượng. Không tạo đồng loạt các index suy đoán. Giữ nguyên tổng net, tập phiếu và thứ tự tên thành phẩm của checkout.

### P1 — Khóa chung trong singleton khiến nhiều máy phải chờ nhau khi lưu HQ

- File/phương thức: `Services/CommitService.cs`, `CommitHqLite`, dòng 7563–7587; `_lockItems` ở dòng 18. `PMSHub/Program.cs` đăng ký `ICommitService` dạng singleton.
- Nguyên nhân: `_lockItems` được dùng chung giữa nhiều luồng máy. Phần được khóa chứa SELECT Id lần hai, INSERT đồng bộ qua Dapper và `MayCansService.AddItem`.
- `AddItem` gọi `ItemUpdated` ngay trong luồng xử lý (`MayCansService.cs:101`), trong khi `NotifyItemChanged` mới là đường có gom thông báo. Subscriber chậm làm kéo dài vùng khóa.
- Khóa SQL theo thẻ đang được giữ bằng một DbContext, còn Dapper ghi phiếu qua kết nối riêng. Khi nhiều máy chờ khóa chung, thời gian giữ khóa thẻ cũng tăng. INSERT đã commit ở kết nối Dapper không được rollback bởi transaction của DbContext.
- Cách xác minh: đo thời gian chờ/giữ `_lockItems`, thời gian chờ applock, thời gian INSERT và callback UI riêng. Tải nhiều máy khác nhau quan trọng hơn chỉ lặp trên một máy.
- Hướng xử lý: trước tiên đo và giảm phần việc thừa trong vùng khóa hiện có. Đưa UI ra sau commit, chuyển sang khóa theo máy hoặc thống nhất kết nối/transaction chỉ là các phương án nghiên cứu sau khi chứng minh tương đương cả lúc lỗi và đồng thời; chưa đưa các thay đổi này vào đợt tối ưu ít rủi ro.
- Rủi ro: không bỏ SELECT Id lần hai hoặc bỏ khóa tùy tiện; phải giữ gửi lại không nhân đôi, chuyển máy và kiểm tra ca. Cần kiểm thử hai instance Hub, callback UI lỗi và tình huống commit thành công nhưng mất phản hồi.

### P1 — Backup vẫn gọi SQL cho từng Id và giữ khóa lâu

- File/phương thức: `Services/HqWeightBackupProcessor.cs`, `Process`, dòng 109–119, 172–193.
- Nguyên nhân: mỗi thẻ có một lần `sp_getapplock`; mỗi Id có thêm một lần `ExecuteSqlInterpolated` gọi `sp_getapplock`. Khóa được giữ trong suốt preload, kiểm tra và ghi lô.
- Với N phiếu/C thẻ, phần đã biết gồm: 1 SELECT máy + C lệnh khóa thẻ + N lệnh khóa Id + 5 SELECT preload + truy vấn bổ sung nếu thiếu metadata/scan + các command SaveChanges + điều khiển transaction. Không thể gọi toàn bộ request là “chỉ còn 5 truy vấn”.
- Ví dụ 100 phiếu mới/1 thẻ: riêng SELECT máy, 5 preload và 101 lệnh khóa đã là **107 lần gọi command**, chưa tính SaveChanges, truy vấn bổ sung và begin/commit. Đây là đếm đường code, không phải số đo network.
- Hướng sửa: tránh preload danh mục/lịch sử không cần thiết khi toàn bộ Id đã tồn tại và payload khớp. Nghiên cứu gom các lệnh lấy khóa Id thành ít command hơn, nhưng phải giữ nguyên thứ tự khóa, điều kiện lỗi/timeout và mã trả về. Không tự thêm giới hạn lô làm từ chối payload hiện đang được nhận; kích thước lô chỉ dùng làm tham số bài đo trước mắt.
- Rủi ro: không dùng khóa riêng của backup theo máy rồi cho rằng đã đồng bộ được với online. Mọi đường ghi cạnh tranh cần dùng quy ước tương thích. Chia lô không được tự đổi hợp đồng hiện tại từ “cả lô” sang ACK một phần.

### P2 — Backup còn lọc/sắp xếp trong RAM cho từng phiếu

- Vị trí: `HqWeightBackupProcessor.cs:137–149,179–190`; `ScaleWorkshopHistory.Resolve(IEnumerable<...>)`.
- Nguyên nhân: mỗi phiếu lại duyệt toàn bộ `preloadedScans` để lọc thẻ/thời gian rồi tìm mới nhất. Tập scan đã sắp xếp trước đó nhưng vòng lặp vẫn diễn đạt lại việc sắp xếp. Lịch sử xưởng cũng được tìm lại theo từng phiếu.
- Source thực chưa có lookup theo thẻ như phép benchmark RAM. EF runtime có thể tối ưu một phần phép `FirstOrDefault` sau sắp xếp, nhưng vẫn phải xét tập ứng viên lặp lại.
- Hướng sửa: nhóm scan theo thẻ một lần, giữ thứ tự `(ThoiGian DESC, Id DESC)` và chọn phần tử đầu đủ điều kiện; khi số scan/thẻ lớn mới cần tìm kiếm nhị phân. Cache kết quả metadata theo đầy đủ bộ khóa nghiệp vụ trong một lô. Giới hạn cửa sổ thời gian/lượng dữ liệu preload một cách bảo toàn dữ liệu cũ.
- Rủi ro: collation SQL có thể khác `StringComparer.Ordinal` của dictionary. Không tùy tiện Trim/đổi comparer vì có thể gộp mã thẻ khác nhau. Cần đối soát mã khác hoa/thường hoặc khoảng trắng trước khi sửa.

### P2 — Chấm công online đọc lại dữ liệu đã có

- File/phương thức: `ScaleAttendanceProcessor.Process`, dòng 69–100; `Success`, dòng 185–206.
- Nguyên nhân: `Process` đã kiểm tra Id và đọc sự kiện gần nhất; `Success` lại tìm Id, có thể tìm lần hai theo thẻ/máy/thời gian, và lúc Ra lại tra phiếu Vào. Các truy vấn và tổng kết checkout đang nằm trong transaction giữ khóa thẻ/máy.
- Hướng sửa: truyền bản ghi hiện có/phiếu Vào đã xác định vào hàm nội bộ dựng phản hồi, phân biệt request mới, gửi lại cùng Id và quẹt lặp khác Id; chỉ đọc lại khi chưa đủ dữ liệu. Có thể gộp tra thẻ/nhân viên thành một truy vấn hoặc cache danh mục có invalidation sau khi đo.
- Rủi ro: không được mặc định `latest` luôn là phiếu Vào cho mọi nhánh gửi lại. Giữ nguyên giờ gốc, `VaoId`, loại ca và thông báo firmware; không cache trạng thái ca đang mở thay cho kiểm tra trong transaction.

### P2 — Tính tổng ngày sau mỗi phiếu vẫn tăng chi phí theo số phiếu trong ngày

- File/phương thức: `Dao/Repos/HQ/HQ_PhieuCan.cs:3264`, `GetOnlineTotals`; gọi tại `CommitService.HqResponse:7592`.
- Nguyên nhân: sau mỗi phiếu và cả gửi lại, SQL lại COUNT/SUM tập phiếu `Ngay + MayCan + TrongLuong > 0`. Index hiện tại có thể phủ truy vấn, nhưng vẫn phải đọc các mục thỏa điều kiện; nó không biến SUM thành phép tra một giá trị có sẵn.
- Đếm tĩnh nhánh nhân viên bình thường/cân ổn định/Id mới, không gửi lệnh cập nhật nhân viên: khoảng 7 SELECT + 1 INSERT + 1 command applock, thêm begin/commit transaction. Chưa tính thông điệp thiết bị và callback UI. Việc bỏ hai SELECT danh mục là có lợi nhưng chưa làm cả đường lưu trở thành một INSERT ngắn.
- Hướng sửa trong phạm vi hiện tại: xác nhận index phủ, plan và logical reads; giữ truy vấn tổng đúng công thức hiện có. Tổng tích lũy hoặc cache có invalidation không thuộc đợt tối ưu này vì phải giải quyết thêm insert, backup, sửa/hủy phiếu và nhiều instance, có thể cần thay đổi cấu trúc dữ liệu.
- Rủi ro nghiệp vụ: tổng online hiện dùng **gross** `TrongLuong`, không lọc `Status/ChiSanLuong`; tổng checkout dùng **net** và chỉ phiếu hợp lệ. Đây là hai hành vi đang tồn tại. Không gộp/chuyển công thức trong một đợt tối ưu hiệu năng.

### P2 — OpenShifts và index CheckInOut cần được kiểm chứng bằng plan

- Vị trí: `ScaleAttendanceTransferService.cs:38–46`; script index ngày 17/10, dòng 45–47.
- Nguyên nhân: truy vấn có NOT EXISTS với nhánh `VaoId == Id` OR sự kiện mới hơn cùng thẻ/máy. Index hiện tại đặt `(MaChamCong, TrangThai, ThoiGian)`, còn `MaSoMay`, `VaoId` chỉ INCLUDE. INCLUDE không cung cấp thứ tự tìm kiếm riêng theo `VaoId` hoặc máy.
- Hướng sửa: thử tách hai điều kiện phủ định thành hai NOT EXISTS tương đương, chọn projection nhỏ cho đường online; thử index theo thẻ/máy/thời gian/Id và index `VaoId` có lọc NULL nếu plan chứng minh cần. Giữ truy vấn đối soát toàn bộ lịch sử riêng về phạm vi sử dụng; không làm mất ca lỗi cũ trong trang quản lý.
- Điểm cần kiểm thử hồi quy: điều kiện thẻ mới trong truy vấn con làm thay đổi kết quả nếu tồn tại `VaoId` trỏ ca của thẻ khác. Dữ liệu chuẩn không nên có liên kết đó, nhưng bộ test hiện không chứng minh toàn bộ lịch sử thỏa điều kiện. Cần kiểm tra dữ liệu liên kết trước khi tuyên bố tương đương tuyệt đối.

### P2 — SQL đồng bộ vẫn chiếm luồng xử lý trong request async

`CommunicationService.Route` là async nhưng `ScaleAttendanceProcessor.Process`, backup, các SELECT/INSERT của HQ vẫn đồng bộ. Khi SQL chậm hoặc nhiều cân đồng thời, các luồng xử lý bị giữ để chờ I/O và khóa.

Chuyển lần lượt những hàm nội bộ liên quan sang `ToListAsync`/`SaveChangesAsync` và Dapper async sau khi giảm số command và vùng khóa; giữ chữ ký public hiện có nếu chưa được yêu cầu đổi API. Không bọc toàn bộ luồng trong `Task.Run` để coi là đã giải quyết. Đo hàng đợi thread pool và độ trễ trước/sau.

## 3. Các file thay đổi trong lượt rà soát

1. Thêm tài liệu này: kết luận, bằng chứng, các hạn chế kiểm thử và kế hoạch triển khai.
2. Thêm `docs/sql/20261017-KiemTraHieuNangPMSHub-ReadOnly.sql`: kiểm tra cấu hình DB, kích thước bảng, index, mức sử dụng index, request đang chờ và thống kê Query Store nếu có.

Không sửa C#, XAML, giao thức, public API hoặc schema. Không cập nhật package.

Trong lúc rà soát có xuất hiện các file C#/SQL và thư mục test chấm công mới từ thay đổi song song ngoài lượt làm việc này. Không chỉnh sửa hoặc nhận là thay đổi của báo cáo. Kết quả build/test dưới đây áp dụng cho trạng thái đã chạy kiểm tra; không được dùng để nghiệm thu các file mới xuất hiện sau đó.

## 4. Bản sửa / kế hoạch tối ưu cụ thể

Script chẩn đoán đi kèm chỉ đọc metadata/DMV/Query Store, không bật RCSI hoặc tạo index. Người chạy chọn đúng DB trong công cụ SQL. Một số phần cần quyền xem trạng thái; lỗi quyền được trả riêng để vẫn đọc được các phần còn lại. Không quét COUNT toàn bảng dữ liệu nghiệp vụ và không xuất chuỗi kết nối/payload.

| Thứ tự | Công việc | Kết quả phải có trước bước tiếp theo |
| --- | --- | --- |
| 1 | Xác nhận DLL thực tế, index, RCSI, quy mô bảng; sửa cách đánh giá benchmark | Có số đo nền theo từng lệnh và từng truy vấn, không dùng tỷ lệ giả lập làm kết quả production. |
| 2 | Đo và xử lý tác vụ chốt ca; thử index phù hợp cho checkout/OpenShifts | Giảm reads/CPU của truy vấn đắt nhất trên DB thử nghiệm; kết quả nghiệp vụ khớp trước/sau. |
| 3 | Giảm truy vấn lặp của chấm công; nhóm scan/metadata backup trong RAM; gom command khóa Id | Đếm được command thực giảm; không thay đổi ACK, ca, tổng kg hoặc xử lý trùng. Mỗi thay đổi thành một patch nhỏ. |
| 4 | Đo chờ khóa chung HQ, giảm công việc thừa bên trong khóa hiện tại | Giữ nguyên quy ước khóa/transaction; muốn thay đổi phạm vi khóa phải có bài chứng minh tương đương riêng, chưa mặc định thực hiện. |
| 5 | Chuyển I/O nội bộ cần thiết sang async nếu giữ được thứ tự và xử lý lỗi | Giảm thời gian chiếm luồng; giữ API, trạng thái và ACK. Không đưa cache tổng/tổng tích lũy vào phạm vi này. |

Ưu tiên làm bước 1–2 trước. Không đề xuất ngay Redis, queue nền cho ACK, bulk insert, partition hoặc thay kiến trúc. Nếu SaveChanges thực sự chiếm phần lớn thời gian, mới đánh giá cách ghi lô bằng thư viện sẵn có; không đánh đồng preload theo lô với bulk insert.

### Bộ đo trên bản sao DB

- Cùng snapshot DB, cấu hình máy, index, mức isolation, mức log và dữ liệu đầu vào khi so sánh. Chạy khởi động trước; so sánh cả lịch sử lớn lẫn lượng dữ liệu hiện tại.
- Tách ba luồng: phiếu HQ online ổn định, CHAMCONG Vào/Ra, backup HQ. Tách thời gian gửi lệnh tới thiết bị, thời gian SQL và thời gian UI khỏi tổng thời gian nhận phản hồi.
- Đếm cả EF và Dapper, command khóa ứng dụng, begin/commit; ghi thời gian chờ khóa và giữ transaction. EF interceptor riêng không đo được toàn bộ Dapper và SignalR.
- Thử 1/10/30/50 máy hoặc số máy thực tế; tăng đến mức tải đỉnh và thêm kịch bản gấp đôi tải đỉnh. Thử backup 1/10/100/500 phiếu hoặc giới hạn firmware hỗ trợ, cả lúc không tải và khi đang cân online.
- Ghi p50/p95/p99, throughput hoàn tất, timeout, deadlock, retry, command/request, logical reads, CPU SQL, CPU/RAM Hub, GC, thread pool và độ trễ callback UI. Với RCSI, theo dõi thêm version store theo cấu hình SQL Server thực tế.
- Thử đủ: thẻ cá nhân/thẻ nhóm; ca thường/đêm; ca khác máy/hết hạn; nhiều thành phẩm; gửi lại đúng Id; cùng Id khác payload; hai request đồng thời; hai instance; offline gửi muộn; mất phản hồi sau commit; sửa/hủy phiếu; đổi ngày; UI callback lỗi.
- Khi kết thúc, đối soát danh sách Id, số phiếu và tổng kg trực tiếp trong DB thử nghiệm với đầu vào mong đợi. Tốc độ tăng nhưng sai dữ liệu là không đạt.

Ngưỡng thử nghiệm đề xuất, cần chốt theo thời gian chờ thực của firmware và hạ tầng: phiếu online/Vào p95 ≤ 200 ms; Ra p95 ≤ 400 ms trên mạng LAN; không có lỗi mất/trùng/ACK sai trong tập thử; không deadlock/timeout trong bài tải nghiệm thu. Đây là mục tiêu, chưa phải khả năng hiện tại. Báo cả p99 và so sánh tương đối để tránh che giấu các lượt chậm.

Sau khi có kết quả, pilot có giám sát một nhóm cân rồi mới mở rộng. Mỗi patch phải có số đo trước/sau và cách quay lại; chỉ giữ index chứng minh được lợi ích tổng thể đọc/ghi.

### Điều kiện bắt buộc để giữ nguyên chức năng

- Chạy cùng tập đầu vào trên bản gốc và bản tối ưu với mốc thời gian nghiệp vụ được kiểm soát trong môi trường test; đối chiếu mọi cột nghiệp vụ được ghi và phản hồi JSON cho cân, gồm cả nhánh lỗi/gửi lại.
- Đối chiếu số phiếu, Id, giờ ca, xưởng lịch sử, thành phẩm, gross/net, tổng rổ/kg và trạng thái sau khởi động lại. Không sửa tên trường, mã lỗi hoặc thông báo chỉ vì thuận tiện cho tối ưu.
- Thử cạnh tranh cùng thẻ/cùng máy/khác máy, backup cùng online, chuyển máy cùng lượt cân, offline gửi muộn và hai instance. Không chỉ so sánh chạy tuần tự.
- Nếu khác nghiệp vụ hoặc kết quả lỗi, loại patch đó khỏi đợt tối ưu. Nếu gặp lỗi nghiệp vụ sẵn có, báo riêng và không âm thầm sửa trong patch hiệu năng.
- Không gộp tạo index với bật RCSI, không bỏ khóa/kiểm tra cuối và không ACK trước khi SQL lưu thành công. Chưa có số đo thì chỉ ghi là kỳ vọng, không cam kết phần trăm tăng tốc.

## 5. Giải thích các đề xuất và giới hạn của benchmark hiện tại

`tests/HqPerformanceComparison` chạy qua **174 kiểm tra**, nhưng không chứng minh “ổn định 100%” hoặc giảm thời gian SQL 99%:

1. `Program.cs:307–308` tự gán `oldQueryRoundtrips = batchCount * 5`, `newQueryRoundtrips = 5`. Không có interceptor, SQL trace hoặc kết nối DB để đo hai con số. Phép tính bỏ qua khóa theo từng Id, truy vấn fallback và ghi DB.
2. Dữ liệu sự kiện dùng thẻ `CARD_0001` đến `CARD_0100` (`:160`), còn phiếu benchmark dùng `CARD_HUMAN_OK` (`:250`). Lookup mới tại `:336` vì thế rỗng. Hai biến đếm kết quả cũng không được assert. Tỷ lệ tốc độ chủ yếu so sánh một lượt dò không tìm thấy trên cả danh sách với lookup rỗng.
3. Source backup thực tế vẫn dùng `preloadedScans.Where(...).OrderByDescending(...).FirstOrDefault()` cho từng phiếu. Nó chưa có `ToLookup` theo thẻ như đoạn benchmark.
4. `OldGetErrorLogic/NewGetErrorLogic` là các hàm mô phỏng dùng `ValidateHistoricalScan`, không thực thi toàn bộ `ScaleProductionAttendancePolicy.GetError` với `OpenShifts`, nhiều ca mở và kiểm tra giờ server.
5. `OldOpenShifts/NewOpenShifts` chạy trên danh sách RAM. Chúng không chứng minh SQL dùng Index Seek, giảm deadlock hoặc giảm logical reads.
6. Test JSON tự dựng một `DataLiteRes`, không thực thi toàn bộ luồng insert/ACK và dữ liệu tổng hợp từ DB.

Lượt chạy này bộ mô phỏng in 1.312 ms so với 5 ms, tức 262,4 lần. **Không dùng kết quả đó làm mức tăng hiệu năng của PMSHub.** Trước khi dùng benchmark cho quyết định triển khai, cần tạo dữ liệu có/không khớp, assert kết quả, chạy đúng code thực và đo command/SQL trên DB thử nghiệm.

## 6. Ghi chú debug, tính đúng và rủi ro

- Giữ khóa SQL theo thẻ, quy tắc thứ tự khóa và PK. Không thay bằng khóa trong một process nếu phải hỗ trợ nhiều instance.
- Đường POST đồng bộ chấm công trong `PMSHub/Controllers/ChamCongController.cs:90–115` chưa dùng cùng khóa thẻ và kiểm tra trùng mới dựa vào tồn tại Id. Cần đưa nó vào kiểm thử khi giảm isolation/tối ưu đồng thời; chưa thể coi applock của online là bao phủ mọi đầu ghi. Không biến sửa lỗi ACK trùng thành thay đổi giao thức ngầm.
- RCSI giảm cạnh tranh đọc/ghi trong một số tình huống; không loại bỏ mọi khóa, khóa ghi/ghi, applock hoặc deadlock. Script tối ưu cũ dùng `WITH ROLLBACK IMMEDIATE`: có thể hủy transaction/kết nối đang hoạt động. Không chạy nguyên script trong giờ sản xuất chỉ để kiểm tra trạng thái.
- Lịch sử xưởng đã có unique index `(MayCan, TuThoiGian)` từ script 16/10. Index bổ sung cùng tiền tố có thể giúp phủ dữ liệu nhưng cũng có thể dư; kiểm tra index thực và chuyển kiểu tham số trước khi thêm.
- Có thể kiểm tra implicit conversion ở Dapper `Get(Id)` vì model Id là varchar, còn tham số string mặc định có thể Unicode; mức ảnh hưởng phụ thuộc schema/collation/plan. Đây là điểm cần xác minh, chưa phải nguyên nhân đã được đo.
- Không tối ưu tổng ngày bằng cách chỉ cộng biến trong RAM nếu chưa xử lý replay, backup, sửa/hủy phiếu, restart và nhiều Hub.
- Giữ kiểm tra ca hợp lệ sau khi cân ổn định và trong transaction. Không tái sử dụng kết quả trước lúc chờ cân để bỏ lần kiểm tra bảo vệ cuối.
- Chưa thay đổi source sản xuất nên lượt rà soát này không tạo tác động runtime. Rủi ro của các hướng sửa được nêu riêng để thiết kế patch và nghiệm thu tiếp theo.

## 7. Kết quả build / kiểm thử đã chạy

| Lệnh | Kết quả | Phạm vi chứng minh |
| --- | --- | --- |
| `dotnet build PMSHub/PMSHub.csproj -c Release --no-restore --nologo -v minimal` | Thành công; 0 lỗi, 4.063 cảnh báo; 32,10 giây theo build output | Biên dịch source hiện tại, không chạy Hub. Chưa xử lý cảnh báo ngoài phạm vi. |
| `dotnet run --project tests/HqPerformanceComparison/HqPerformanceComparison.csproj -c Release -p:HubBuildConfiguration=Release --no-restore` | PASS, 174 kiểm tra | Mô phỏng RAM; các giới hạn nêu ở mục 5. |
| `dotnet run --project tests/HqWeightBackup/HqWeightBackup.csproj -c Release -p:HubBuildConfiguration=Release --no-restore` | PASS, 175 kiểm tra | Chuẩn hóa/đối chiếu phiếu, lịch sử và hợp đồng dữ liệu trong RAM; không ghi SQL. |
| `dotnet run --project tests/ScaleAttendanceProtocol/ScaleAttendanceProtocol.csproj -c Release -p:HubBuildConfiguration=Release --no-restore` | PASS | Giao thức và quy tắc chấm công hiện có; không truy vấn/ghi SQL. |
| `Services/Tests/ScaleAttendanceTransfers.Tests.ps1` qua PowerShell 7 | PASS | Quy tắc ca/chuyển máy trên dữ liệu giả lập. |
| `Services/Tests/ScaleAttendanceNightShift.Tests.ps1` qua PowerShell 7 | PASS | Ca đêm, giới hạn giờ và thời gian. |
| `Services/Tests/WorkshopAttendanceDirection.Tests.ps1` qua PowerShell 7 | PASS | Hướng Vào/Ra theo lịch sử giả lập. |

Không chạy `dotnet test` toàn solution: các bộ liên quan là console harness và script, đã thực thi trực tiếp ở trên. Chưa có kiểm thử SQL integration, tải nhiều cân hoặc nghiệm thu thiết bị thật trong lượt này. Script SQL chẩn đoán mới chưa chạy trên DB; chưa xác nhận plan/index/RCSI production.
