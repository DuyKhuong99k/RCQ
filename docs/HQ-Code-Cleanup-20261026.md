# Clean code Hub — 26/10/2026

Phạm vi: `D:\RCQ\RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`. Không sửa checkout nhánh 123, không publish/restart/deploy.

Các mục đầu ghi lại lượt cleanup trước. Lượt review tiếp theo, sau khi nhóm được yêu cầu theo ca như nhân viên, nằm ở phần cuối tài liệu; số test và dấu kiểm tra của từng lượt được ghi riêng.

## Đã dọn

- `ChatHub.SendData`: gom ba đoạn tạo phản hồi lỗi vào `CreateSendDataError`, giữ đúng envelope LITE và lỗi legacy của các lệnh khác. Bỏ đoạn đo/gửi HQ không thể chạy tới sau khi đã đi qua bounded delivery; bỏ các dòng debug/delay đã comment ở chính luồng này.
- Gom thao tác abort kết nối cũ và lấy Id để audit thành helper private. Chỉ parse metadata audit khi gửi thất bại, không parse lại mọi phản hồi thành công.
- Tách `IHqFirmwareIdentitySender` thành file riêng, vẫn cùng namespace/assembly và chữ ký; không đổi contract `IMayCansService` hay tham số firmware.
- `HqFirmwareSend`, `HqOnlineUi`: làm rõ tên biến, bố cục và xử lý lỗi có chủ ý; tách khởi chạy worker UI khỏi thao tác đưa vào queue. Giữ nguyên khóa, thứ tự row/notify, queue 100 dòng và giới hạn gửi 2/3 giây.
- Đổi tên timer refresh thành `ScheduleNotifyFlush`, `_notifyFlushTimer`, `_notifyIntervalMilliseconds` cho đúng cơ chế cửa sổ refresh cố định, không phải debounce reset liên tục. Giữ nguyên 100 ms.
- Bỏ **1.778 dòng bản class MayCansService cũ đã comment hoàn toàn** ở cuối file. File còn 2.691 dòng. Không xóa code đang chạy; bản comment cũ vẫn khôi phục được từ Git.

## Giữ nguyên

Không thay SQL ghi phiếu/khóa/chống cùng Id, kiểm tra ca theo thành phẩm + nguyên liệu + xưởng, tính đủ tổng rồi phản hồi, offline backup, thông báo nghiệp vụ, ST/US/0 kg hoặc firmware. Hash trước/sau xác nhận hai file cấu hình người dùng, writer online, service ca online, policy chấm công và danh sách thông báo không đổi.

## Xác minh

- Build Release: **0 errors**. Full build đầu lượt 388 warnings, incremental cuối lượt 181 warnings từ code phụ thuộc hiện có; đợt này không xử lý toàn bộ nullable/legacy warnings.
- Thêm 40 trường hợp so sánh **chính xác chuỗi JSON** cho HQ/legacy, LITE/CHAMCONG/chữ thường/lệnh khác, kết nối chưa xác định/không phải cân/lỗi router/thành công. Kiểm tra này chạy đạt trên DLL trước cleanup, sau đó chạy lại trên DLL đã refactor.
- HqStableSwipe: **400 PASS**, HqAttendanceDiagnosis: **53 PASS**, HqOnlineWeight: **112 PASS**, HqHttpBackup: **111 PASS**, ScaleOnlineWorkContext: **71 PASS**. Tổng **747 assertions**.
- 30 client SignalR WebSocket loopback: 11 đợt, **330/330 phản hồi, 330 phiếu SQL, 330 SETNHANVIEN**, đúng thứ tự identity trước phản hồi cuối và đúng tổng rổ/kg. Cả đợt 30 cân: lượt đầu 1,970 giây; nóng ít dữ liệu 49–74 ms; thêm 100.000 phiếu mẫu 624–661 ms.
- Đây là máy test/LocalDB GUID riêng, không phải số đo hay cam kết trên server xưởng. Không có browser, firmware thật, TLS/LAN hoặc tải báo cáo/background jobs đầy đủ. Database test đã được xóa sau test.
- `git diff --check` đạt. Chưa commit/push/publish, không cần SQL mới hoặc đổi firmware riêng cho cleanup này. Khi publish, dùng đầy đủ project đã build, bao gồm file interface mới; không trộn DLL lẻ với release cũ.

## Review và cleanup tiếp theo — nhóm theo ca

### Những điểm đã xử lý

- `CommitService` vẫn là lớp legacy lớn. Tách nguyên khối xử lý HQ sang `CommitService.HqOnline.cs` và `CommitService.HqBackup.cs`, dùng partial class để giữ nguyên constructor, interface, phương thức override tạo DbContext, và thứ tự gọi hiện có. Đây là tách file theo trách nhiệm, chưa phải thay toàn bộ kiến trúc của class legacy.
- Gom hai lần tra thẻ/nhân viên thành `FindHqEmployee`, chỉ đọc cache. Đường gửi lại Id đã lưu vẫn không chạy lookup này hoặc phụ thuộc cấu hình/thông tin thẻ hiện tại.
- `ScaleProductionAttendancePolicy`: online đồng bộ/bất đồng bộ dùng chung query timeline, query snapshot và kiểm tra cả giờ thiết bị lẫn giờ nghiệp vụ hiện tại. Bản đồng bộ nay dùng một MAX trên UNION ALL thay vì hai MAX riêng. Không chuyển ngược sang chặn nhóm hoặc bỏ kiểm tra ca; ngoại lệ nhóm chỉ còn ở hàm có tên rõ `RequiresLegacyAttendance` của backup lịch sử.
- `ScaleOnlineAttendanceService`: gom query ca tại giờ cân, xác định ca đêm qua ngày và mốc bắt đầu tính tổng. Giữ cùng bộ mã, xưởng, mã nhân viên và cộng kg net; không đổi số query của đường async hoặc cách SUM/COUNT.
- Review phát hiện ghi log bằng Console trực tiếp có thể ném lỗi trong lúc từ chối ca, retry tổng hoặc sau COMMIT. Chuyển các vị trí trong HQ online/backup/response và catch LITE sang helper log có bảo vệ. Không để lỗi đầu ra log biến kết quả SAVED thành UNKNOWN hoặc ngắt retry tổng. Không đổi nội dung thông báo nghiệp vụ.
- Bỏ using/biến parse không dùng sau khi tách file, làm rõ comment nhận diện kg không dương, định dạng phần backup dễ đọc hơn; bỏ null-forgiving ở điều kiện nhóm legacy.

### Xác minh lượt này

- Trước refactor: thêm 12 assertions đối chiếu policy đồng bộ/bất đồng bộ cho cả nhóm IsHuman=0/1 ở các bước chưa Vào, cùng ca khác cân, sai thành phẩm/nguyên liệu/xưởng và sau Ra. Bộ HqAttendanceDiagnosis đạt **103 PASS** trên DLL trước cleanup; sau cleanup vẫn **103 PASS**, thêm gây lỗi đầu ra Console khi xử lý request thật.
- Build Release: **0 errors**. Lượt build lại Services có **388 warnings** mã nền; incremental cuối có **181 warnings**. Không phải toàn bộ solution đã sạch cảnh báo.
- HqStableSwipe **512 PASS** (gồm 40 trường hợp so sánh chuỗi JSON), HqOnlineWeight **126 PASS**, ScaleOnlineWorkContext **71 PASS**, HqWeightBackup **175 PASS**, HqHttpBackup **111 PASS**, HqFunctionCards **37 PASS**, HqAttendanceDiagnosis **103 PASS**: tổng **1.135 assertions**.
- Ba assertions mới của writer/response gây lỗi Console cùng lỗi tính tổng và UI: retry vẫn phục hồi trả đủ tổng đúng; lỗi tổng dai dẳng vẫn báo SAVED, không đổi thành UNKNOWN hoặc tạo dữ liệu giả.
- SignalR WebSocket loopback 30 client, 11 đợt: **330/330 phản hồi, 330 phiếu mới trong SQL, 330 SETNHANVIEN**, không đổi thứ tự identity trước phản hồi cuối. Cả đợt đầu **2,104 giây**; các đợt nóng ít dữ liệu **55–65 ms**; sau thêm 100.000 phiếu mẫu **573–1.163 ms**. Là số đo một lần trên máy test/LocalDB riêng, không dùng để khẳng định tăng tốc so với lần trước hoặc cam kết tốc độ xưởng.
- Dấu SHA256 xác nhận hai file cấu hình cá nhân, writer SQL, helper gửi firmware, HTTP backup writer, selection và danh sách thông báo không đổi. `HqOnlineResponse` chỉ đổi hai lệnh ghi log sang helper an toàn, không thay format JSON, tổng, số lần retry hoặc deadline.
- SQL test dùng các database GUID riêng, đã tự dọn. Không sửa firmware, schema hoặc dữ liệu xưởng; không khởi chạy/dừng Hub của người dùng, không commit/push/deploy. Khi publish cần lấy đầy đủ project chứa hai file partial mới, không ghép DLL với bản cũ.
