# Ca HQ online theo thành phẩm / nguyên liệu / xưởng

Nền: `origin/Dev_Huy` tại `c2988f9`. Bản làm việc: `feature/hq-shift-product-material-workshop`.
Không đổi firmware, dữ liệu SQL thật hoặc cấu hình kết nối. Không commit/push/deploy tự động.

## Nghiệp vụ

- Nhân viên và thẻ nhóm đều phải Vào một ca còn hạn trước khi ghi phiếu online.
- Khóa ca là **mã thành phẩm + mã nguyên liệu + mã xưởng**, so mã, giữ số 0 đầu; không so tên.
- Cùng bộ mã: được ghi ở bất kỳ cân HQ nào cùng xưởng, không cần Vào từng cân.
- Vào lặp cùng bộ mã tại bất kỳ cân: báo đã Vào, giữ nguyên ca/giờ bắt đầu.
- Khác một mã: chưa Vào mới thì không nhận sản lượng; Vào mới đóng ca cũ/mở ca mới trong một transaction.
- Ra ở bất kỳ cân HQ nào, không yêu cầu cấu hình cân Ra trùng ca đang làm.
- Ca thường không kéo sang hôm sau; ca đêm qua ngày trong giới hạn `ScaleAttendance:MaxShiftHours` (hiện 16h).
- Cập nhật 26/10 theo yêu cầu mới: thẻ nhóm `IsNhom=1` theo ca như nhân viên, kể cả đồng thời `IsHuman=1`. Quẹt Ra rồi không được tạo phiếu online mới cho tới khi Vào lại. Ngoại lệ nhóm không cần ca chỉ còn ở đối chiếu backup offline lịch sử, không dùng cho LITE online.
- Lô/size/máy không phải khóa ca. Đổi lô hoặc size nhưng giữ bộ mã không tự đóng ca.

## Dữ liệu và tổng

`CheckInOut` tiếp tục lưu cặp Vào/Ra để báo cáo cũ ghép giờ theo máy gốc và `VaoId`.
`ScaleAttendanceScan` lưu máy quẹt thực tế và snapshot thành phẩm/nguyên liệu/xưởng; ca mới không dùng cấu hình hiện tại để đoán lịch sử.
Ra tự động có Id riêng `.AUTO.`, không chiếm Id máy.giây của lượt quẹt thật.

- Tổng màn cân: cộng trực tiếp `HQ_PhieuCan.TrongLuong` (giống offline cộng `TrongLuongNhan`), **không trừ bì lần hai**.
- Tổng nhân viên: cùng mã thành phẩm/nguyên liệu/xưởng và `MaNhanVien`, gom mọi máy/thẻ trong ngày; ca đêm qua ngày giữ từ đầu ca đêm.
- Các trường JSON `TongSoRoCan`/`TongTrongLuongCan` giữ tên cũ nhưng phạm vi là tổng bộ mã/xưởng, không còn riêng máy.
- Tổng khi Ra: số phiếu và ký trong ca đang đóng, đúng thẻ của ca và bộ mã/xưởng, gom mọi máy. Giờ công chỉ tính một lần.
- Chỉ tính phiếu `ChiSanLuong=1`, `Status=1`. Ca cũ/offline không có snapshot đủ ba mã giữ cách đối chiếu cũ; không tự backfill.

## Trước khi triển khai

Trước khi F5/chạy thử solution mới: kiểm tra cấu hình kết nối và trỏ về **bản sao database test**, không mặc định cấu hình trong Git là máy thử.

1. Sao lưu database, dừng các Hub cùng ghi PMS_HQ trong cửa sổ bảo trì.
2. Chạy `docs/sql/20261018-HqOnlineWorkContext.sql` trên **đúng PMS_HQ**. Script chỉ thêm bảng/cột/index, không sửa phiếu cũ, không đổi RCSI.
3. Không chạy đồng thời bản xưởng cũ theo máy với bản mới theo bộ mã trên cùng database.
4. Ca đang mở từ bản cũ chưa có snapshot: nhân viên phải Ra rồi Vào lại; không đoán nguyên liệu từ cân hiện tại.
5. Chuyển máy bằng quản trị không áp dụng với ca online mới: cùng bộ mã không cần chuyển; khác bộ mã/xưởng phải quẹt Vào mới.
6. Đồng bộ đúng lô/thành phẩm/nguyên liệu giữa firmware và Hub trước khi quẹt Vào. Luồng LITE lấy mã đang chọn ở Hub.
7. Web báo cáo riêng `D:\ChamCongSanXuat` đang lọc sản lượng theo máy trong `ChamCongSanXuatDataService`; cần cập nhật/kiểm thử phần tổng sản lượng theo snapshot trước khi dùng báo cáo đó đối chiếu ca mới. Không sửa web báo cáo trong nhánh RCQ này.

## Phạm vi offline

Chỉ đổi đường online `CHAMCONG` và `LITE`. Không đổi payload firmware hoặc tự gán bộ mã cho chấm công offline cũ.
API đồng bộ offline dùng bộ xử lý cũ để giữ đối chiếu, chưa được coi là hỗ trợ quy trình mới đầy đủ.
Không chuyển online/offline giữa một ca mới để thay thế chấm công; dữ liệu thiếu snapshot cần đối chiếu riêng.
Online LITE chỉ chống ghi lại **cùng Id**: xác nhận phiếu đã lưu, không thêm/cộng lại. Cùng Id khác máy/thẻ/giờ/ký/tare hoặc phiếu đã hủy bị từ chối, không ghi đè. Không suy đoán lại lô/thành phẩm/nguyên liệu từ cấu hình hiện tại khi xác nhận lượt gửi lại.
Id mới không bị chặn theo số ký giống nhau, lệch nhẹ hoặc khoảng cách 30 giây; vẫn phải đạt ST, ca và bộ mã hợp lệ.
Chống quẹt lặp cùng một rổ do nhúng. Chính sách chống trùng 30 giây của offline/Sync được giữ nguyên, không áp dụng vào online LITE.
Lưu ý bản nền `c2988f9` có guard HTTP yêu cầu `Stated=ST`; firmware offline không gửi trường này.
Ngày 24/10 đã sửa riêng HTTP Post/PostSilent cho phép thiếu Stated, giữ online LITE bắt ST.
Xem `HQ-HTTP-Backup-20261024.md` về transaction, retry, lịch sử xưởng và phạm vi kiểm thử.
Không dùng kết quả online/HTTP để khẳng định Sync SignalR offline cũ đã tương thích đầy đủ.

## Bổ sung an toàn WSS online

- US/thiếu trạng thái/không phải ST hoặc ký không dương: không tạo phiếu, không gọi SETWAITINGSTABLE, xóa thẻ chờ trên Hub. Người dùng phải quẹt lại khi cân sẵn sàng.
- ST có `IsWaiting=true` nhưng Id chưa tồn tại cũng bị từ chối: không cho kết quả chờ cũ tự tạo phiếu mới. Đúng Id/nội dung đã COMMIT vẫn được xác nhận lại, không cộng thêm.
- ST chỉ kiểm tra từ payload của request. Không dùng `machine.IsStated` dùng chung để quyết định INSERT.
- `HqOnlineWeightWriter.ForMachine` tuần tự hóa toàn bộ lượt xử lý theo từng máy, bao gồm các await/cập nhật UI. Các máy khác vẫn xử lý độc lập.
- SQL khóa theo máy và Id, dùng khóa thẻ chung với chấm công; kiểm tra lại Id và ca trước ghi. Kiểm tra ca, INSERT và COMMIT dùng cùng kết nối/transaction EF, không còn DAO INSERT trên kết nối riêng.
- Chỉ cập nhật danh sách phiếu mới sau COMMIT. Gửi lại cùng Id không thêm vào danh sách lần hai. Lỗi cập nhật danh sách UI không biến INSERT đã COMMIT thành một phiếu chưa ghi.
- Lỗi ghi SQL/ca không hợp lệ không trả `IsDataAction=1`. Nếu một client có giữ Id gốc thì có thể đối chiếu lại cùng Id; firmware hiện không được giả định có khả năng này. Không yêu cầu nhân viên quẹt lại/cấp Id mới chỉ vì tổng lỗi; WSS tự retry riêng query tổng. Mất kết nối sau COMMIT với Id mới vẫn cần đối chiếu, không thể bảo đảm chống cùng rổ hai Id chỉ ở WSS.
- Payload ký/tare phải khớp độ chính xác lưu SQL (tối đa 3 chữ số thập phân); không âm thầm làm tròn rồi báo xung đột khi retry.
- Không sửa firmware/offline/Sync, không thêm cooldown 30 giây, không thêm migration cho riêng lớp bảo vệ WSS này.
- Đây không phải khóa vật lý một rổ: hai Id ST mới khác nhau vẫn được xét như hai phiếu. Không bảo đảm ngăn một rổ quẹt hai lần khi firmware chưa khóa.

## Thông báo ngắn trên màn cân

Chỉ rút gọn các thông báo trong phần HQ online mới. Không thay nội dung của HTTP/offline,
các nhánh chấm công cũ, thông báo thành công/quẹt lặp cũ, hay lý do đóng ca được lưu để kiểm toán.
Các kiểm tra giờ/Id/tare vốn có trong CommitHqLite giữ nguyên câu cũ.
Mã lỗi (`Code`), định dạng JSON và điều kiện nhận/từ chối giữ nguyên.

- Khác thành phẩm: `Khác thành phẩm. Quẹt Vào`.
- Khác nguyên liệu: `Khác nguyên liệu. Quẹt Vào`.
- Khác xưởng: `Khác xưởng. Quẹt Vào`.
- Thiếu trường nào báo riêng trường đó, không gom thành một câu dài.
- Chưa có ca: `Chưa Vào ca. Quẹt Vào`.
- Ca cũ thiếu snapshot: `Ca cũ thiếu mã. Quẹt Ra rồi Vào`.
- US: `Cân chưa ổn định. Không lưu phiếu` (chỉ online mới; HTTP/offline giữ câu cũ). US không INSERT, không cộng tổng, không giữ thẻ để tự ghi sau; gửi lại Id đã có cũng không trả xác nhận ST thành công cho request US.
- ST/0 kg ở lượt quẹt mới: tra thẻ/nhân viên từ cache, gửi `SETNHANVIEN [MaNhanVien, MaHoSo, Name]` theo giao thức firmware có sẵn để nhận diện; phản hồi LITE vẫn là lỗi `Số kg bằng 0. Không lưu phiếu`. Không gửi xác nhận hoàn thành cân, không tạo phiếu/Vào/Ra, không cộng/reset tổng và không giữ thẻ chờ. Lượt tiếp theo phải mang mã thẻ của chính nó. US, ký âm và callback `IsWaiting=true` không được nhận diện theo ngoại lệ này. Writer SQL vẫn từ chối trọng lượng 0; HTTP/offline không đổi.
- Trùng Id khác nội dung: `Trùng Id, khác dữ liệu`.
- Lượt chờ cũ: `Lượt chờ cũ. Quẹt lại thẻ`.

Lỗi SQL của luồng LITE mới báo `Lỗi SQL. Báo quản lý`; log máy chủ vẫn giữ chi tiết exception.
Thông báo tập trung trong `ScaleOnlineMessages`, được test tối đa 40 ký tự và 64 byte UTF-8
(chừa dung lượng cho phần bao lệnh HMI).
Giới hạn này không thay thế thử nghiệm font/độ rộng ô chữ trên HMI thực tế.

## Kiểm thử

Build Release PMSHub rồi chạy:

```powershell
dotnet run --project tests/ScaleOnlineWorkContext -c Release -- --sql-localdb
dotnet run --project tests/ScaleSharedAttendance -c Release -p:HubBuildConfiguration=Release
dotnet run --project tests/HqStableSwipe -c Release -p:HubBuildConfiguration=Release
dotnet run --project tests/HqOnlineWeight -c Release -- --sql-localdb
dotnet run --project tests/HqWeightBackup -c Release -p:HubBuildConfiguration=Release
dotnet run --project tests/ScaleAttendanceProtocol -c Release -p:HubBuildConfiguration=Release
```

SQL test chỉ tạo/xóa database `RCQ_OnlineContextTest_<GUID>` hoặc `RCQ_OnlineWeightTest_<GUID>` trên LocalDB, không đọc cấu hình kết nối PMS.
Kiểm thử phần cứng thực tế vẫn cần: firmware phải gửi đủ CHAMCONG, ST/US, Id và giờ đúng; Hub xác nhận SQL thành công trước khi cân khóa rổ.

### Kết quả kiểm tra trên máy phát triển (18/10/2026)

- Build Release PMSHub: thành công, 0 lỗi; vẫn còn cảnh báo của mã nền (full build 4.063 cảnh báo, không phải bản build sạch cảnh báo).
- ScaleOnlineWorkContext: **57 kiểm tra PASS**, có SQL Server LocalDB thật, chạy song song bốn lượt Vào, rollback transaction,
  tổng kg qua nhiều máy, Ra bất kỳ máy, ngày/ca đêm, gửi lại Id sau đổi cấu hình và migration chạy hai lần.
- ScaleSharedAttendance: 59 PASS (giữ hành vi đối chiếu cũ).
- Kết quả HqStableSwipe trước bổ sung writer: 82 PASS. Xem kết quả bổ sung WSS phía dưới; kiểm tra này không thay thế thử firmware thật.
- HqWeightBackup: 175 PASS (kiểm tra in-memory, không phải xác nhận Sync HTTP firmware thật).
- HqWeightDuplicate: 42 PASS (kiểm tra chính sách nền offline/Sync đang được giữ).
- ScaleAttendanceProtocol: PASS, không truy cập database xưởng.
- `git diff --check`: sạch. Nhánh xưởng ban đầu và hai file cấu hình người dùng không bị sửa.
- Chưa kiểm thử firmware vật lý, chưa chạy migration trên server xưởng, chưa deploy.

### Kết quả bổ sung bảo vệ WSS online

- Build Release PMSHub thành công: 0 lỗi, 390 cảnh báo trong lượt build xác minh cuối (vẫn còn cảnh báo mã nền).
- HqOnlineWeight: **48 PASS**, gồm SQL LocalDB thật. Tám request cùng Id chỉ một INSERT; khác máy/thẻ tranh cùng Id chỉ một bên thành công; khác thẻ cùng máy được ghi tuần tự. Đã thử xung đột giờ/ký/tare, ST chờ cũ, retry sau COMMIT, SQL từ chối ghi/rollback và không kẹt khóa sau lỗi.
- HqStableSwipe: **88 PASS**, kiểm tra US/thiếu trạng thái không truy cập dữ liệu nghiệp vụ, xóa thẻ chờ cũ và không gửi SETWAITINGSTABLE.
- ScaleOnlineWorkContext: **57 PASS** gồm SQL LocalDB, giữ quy trình ca theo bộ mã.
- HqWeightBackup: **175 PASS** (in-memory). Offline/Sync không bị sửa trong lần bổ sung này.
- Cấu hình cá nhân trong worktree được giữ nguyên. Nhánh `Dev_Khuong` ở checkout ban đầu và các conflict cấu hình của người dùng không bị thay đổi.
- Không sửa firmware, không chạy SQL trên database thật, không commit/push/deploy. Cần thử cân thật trước triển khai chính thức; vẫn giữ các cảnh báo triển khai và báo cáo riêng ở trên.

### Kết quả rút gọn thông báo (22/10/2026)

- Build Release PMSHub: thành công, 0 lỗi, 390 cảnh báo mã nền.
- HqStableSwipe: **92 PASS**, gồm kiểm tra lỗi SQL trả câu ngắn, không lộ exception trong payload; HTTP cũ giữ nguyên thông báo.
- HqOnlineWeight: **80 PASS**, gồm giới hạn độ dài thông báo và SQL LocalDB riêng; giữ chống gửi lại Id, transaction và khóa máy.
- ScaleOnlineWorkContext: **64 PASS**, gồm SQL LocalDB riêng và các câu báo đúng thành phẩm/nguyên liệu/xưởng; không đổi quy trình ca.
- ScaleSharedAttendance: **59 PASS**; ScaleAttendanceProtocol: PASS, giữ hành vi luồng cũ.
- `git diff --check`: sạch. Không sửa firmware, cấu hình cá nhân hoặc SQL xưởng; chưa deploy, chưa thử font/ô chữ trên cân thật.

### Xác minh US không lưu phiếu (23/10/2026)

- Đổi riêng thông báo HQ online thành `Cân chưa ổn định. Không lưu phiếu`; không đổi điều kiện nhận/từ chối và thông báo HTTP/offline cũ.
- HqStableSwipe: **93 PASS**; US/thiếu trạng thái bị từ chối trước đọc/lưu phiếu, xóa thẻ chờ và không gửi lệnh chờ cân bằng.
- HqOnlineWeight: **83 PASS**, có SQL LocalDB riêng. US Id mới, US gửi lại Id ST đã lưu và US lượt chờ cũ đều không INSERT, không xác nhận thành công hoặc thay đổi phiếu đã lưu.
- Build Release PMSHub: 0 lỗi, 390 cảnh báo mã nền; `git diff --check` sạch. Delay thử nghiệm đã được gỡ; không sửa firmware hoặc database xưởng, chưa deploy.

### Nhận diện nhân viên ở ST/0 kg (23/10/2026)

- Bổ sung nhánh chỉ nhận diện bằng cache, gửi lệnh `SETNHANVIEN` sẵn có; LITE trả lỗi không lưu phiếu, không gửi `IsDataAction=1`. Không yêu cầu mở ca chỉ để xem chủ thẻ; cân có kg vẫn kiểm tra ca như trước.
- HqStableSwipe: **181 PASS**, gồm nhân viên, nhóm, nhóm đồng thời IsHuman, tên/hồ sơ null, thẻ sai, US/0, kg âm, lượt chờ cũ, giờ/Id/tare sai, lỗi gửi tên và không dùng lại thẻ nhận diện ở lượt cân tiếp theo. Test dùng cache và dịch vụ giả, không mở SQL hoặc gửi tới cân thật.
- HqOnlineWeight: **87 PASS** có SQL LocalDB riêng; writer vẫn chặn phiếu 0kg, US và giữ xử lý trùng Id/transaction.
- Build Release PMSHub: 0 lỗi, 390 cảnh báo mã nền; `git diff --check` sạch. Không đổi HTTP/offline/firmware, không SQL xưởng, chưa deploy hoặc thử HMI vật lý.

### Hoàn thiện phản hồi firmware (23/10/2026)

Theo lựa chọn người dùng, vẫn chờ đủ tổng rồi mới ACK hoàn thành. Nếu tổng lỗi sau COMMIT,
WSS tự retry riêng query tổng (tối đa 3 lần/ngân sách chờ 4 giây). Nếu vẫn lỗi, trả ngắn
`Đã lưu phiếu. Chưa có tổng` với SaveStatus=SAVED, không yêu cầu quẹt lại/cấp Id mới và không gửi tổng 0/tổng cũ.
Lỗi SQL chưa xác minh COMMIT là UNKNOWN; từ chối request trước ghi là NOT_SAVED.
Phản hồi lỗi có Id/thẻ/giờ/máy để đối chiếu; SETNHANVIEN giữ ba arguments như cũ.
Xem chi tiết giao thức và kết quả 253/95/64 kiểm tra trong `HQ-Firmware-Responses.md`.
Không thay thuật toán tổng, nghiệp vụ ca, chống trùng Id hoặc offline/Sync.

### Bổ sung WSS tự đọc lại tổng, không bắt firmware gửi phiếu (23/10/2026)

Firmware hiện tạo Id mới theo thời gian gửi và không được giả định giữ phiếu gốc.
Vì vậy WSS chỉ retry query tổng sau COMMIT cho item gốc; không retry INSERT hay yêu cầu quẹt lại vì tổng lỗi.
Tối đa ba lần trong ngân sách chờ bốn giây cho phần tổng, context mới/cancellation mỗi lần.
Nếu không lấy được tổng: báo `Đã lưu phiếu. Chưa có tổng` với SaveStatus=SAVED, không gửi số tổng giả.
Tổng async và sync dùng chung biểu thức SQL. Build 0 lỗi; kiểm tra 260/98/66 PASS,
cùng backup 175, ca chung 59 và protocol PASS. Xem `HQ-Firmware-Responses.md` về giới hạn và trách nhiệm nhúng.
