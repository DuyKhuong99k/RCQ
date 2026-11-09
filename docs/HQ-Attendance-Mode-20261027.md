# Bật/tắt chấm công Hub — 27/10/2026

Phạm vi: `RCQ-ProductMaterialWorkshop`. Không sửa firmware, không đổi SQL xưởng, không tự deploy.

## Thao tác

- Nút dùng chung nằm ở đầu trang **Thiết bị** và **Thiết bị tại xưởng**. Cả hai điều khiển một trạng thái cho mọi cân/thiết bị xưởng trong tiến trình Hub này.
- Mặc định **Bật**, giữ hành vi đang dùng trước khi có nút.
- Mỗi lần Bật hoặc Tắt đều phải nhập lại tài khoản và mật khẩu quản trị. Tài khoản phải có `IsAdmin=true`, `IsActive=true`, đúng mật khẩu. Không dùng tên tài khoản/token trên URL để đổi chế độ.
- Xác nhận đọc trực tiếp 5 cột cần thiết của `NguoiDung` trong SQL, không dùng cache đăng nhập. Không thay phiên đang mở hoặc lưu mật khẩu. Sai xác nhận có thời gian chờ ngắn trước khi thử lại.
- Hai quản trị cùng thao tác không được ghi đè âm thầm: kiểm tra revision khi lưu, thao tác dùng phiên bản cũ phải xác nhận lại.

## Hành vi

Khi **Bật**:

- Nhân viên/nhóm cân sản lượng theo ca thành phẩm + nguyên liệu + xưởng như trước.
- Quẹt tại xưởng theo Vào/Ra trong ngày, phải đủ 15 phút từ lượt Vào mới được Ra.
- Khi bật lại, giữ và dùng ca cũ **nếu vẫn còn hợp lệ** theo quy tắc hiện có. Không tạo Vào/Ra giả, không sửa thời gian hoặc xóa lịch sử. Mốc 15 phút tại xưởng vẫn tính từ lượt Vào gốc.

Khi **Tắt**:

- LITE dương hợp lệ ghi vào `HQ_PhieuCan` không cần ca Vào, không so bộ mã với ca cũ. Bỏ riêng điều kiện chấm công, không bỏ kiểm tra thẻ sản xuất, kg/tare, mã sản xuất/xưởng, pending Set hoặc xung đột Id.
- Cả phần ghi và tính tổng online không đọc bảng chấm công. Tổng lấy theo ngày + nhân viên + thành phẩm + nguyên liệu + xưởng, gom các cân, cộng kg net như trước.
- ST/0 hoặc âm vẫn chỉ nhận diện thẻ; US/thiếu trạng thái hoặc dữ liệu sai vẫn không lưu phiếu. Cùng Id đúng nội dung chỉ xác nhận phiếu có sẵn, không INSERT/cộng lại.
- Chấm công online ở cân và xưởng trả `Chấm công đang tắt`, không ghi thêm Vào/Ra. Chuyển ca bằng quản trị cũng bị chặn. Hai dịch vụ tự đánh dấu thiếu Ra tạm dừng khi chế độ đang Tắt.
- WSS backup **phiếu cân** bỏ điều kiện ca lịch sử khi Tắt; HTTP backup phiếu cân giữ cơ chế lịch sử riêng đang có. Khi Bật lại, WSS backup khôi phục quy tắc cũ, kể cả ngoại lệ nhóm của dữ liệu offline lịch sử đã thống nhất trước đây.
- HTTP **đồng bộ chấm công** `/api/chamcong/post` trả 409, `code=ATTENDANCE_DISABLED`, không giả báo đã lưu. Không xóa lịch sử hoặc cố buộc firmware xóa backup. Nếu firmware vẫn gửi chấm công khi Tắt, có thể còn báo chưa đồng bộ chấm công; đây không phải lỗi lưu `HQ_PhieuCan`. Dữ liệu lịch sử gửi khi Bật lại được xử lý theo quy tắc hiện có, không có bộ lọc thời gian hồi tố cho các đợt Tắt.

Một lượt xử lý giữ snapshot chế độ nhất quán giữa kiểm tra và tính tổng. Gói đã được nhận/xử lý trước lúc đổi chế độ có thể hoàn tất theo chế độ cũ; không hủy ngang INSERT hoặc đổi kết quả COMMIT đã có. Gói mới theo chế độ mới.

## Triển khai và lưu thiết lập

- Publish **đầy đủ PMSHub** từ bản làm việc, không ghép DLL lẻ với bản cũ. Không cần bảng/cột/index SQL mới riêng cho nút này.
- Lưu tại `App_Data/attendance-mode.json` dưới content root của Hub, **ngoài wwwroot**. Có Enabled, revision, tài khoản/id và giờ thay đổi gần nhất; không có mật khẩu.
- Tài khoản chạy Hub/IIS phải có quyền ghi `App_Data`. Nếu không lưu được file thì không đổi trạng thái trong RAM, giao diện báo thất bại.
- **Giữ/backup App_Data khi chép publish**; không chọn cách deploy xóa toàn bộ file tại đích mà không bảo toàn thư mục này. Project và Git đã loại file thiết lập vận hành khỏi publish/commit để không đè chế độ xưởng bằng chế độ của máy phát triển.
- Thiếu file ở lần chạy đầu: Bật. File hiện có bị hỏng hoặc thiếu trường bắt buộc: không tự bỏ chấm công; startup báo lỗi để xử lý. Ghi file tạm, flush và thay thế trước khi công bố trạng thái mới cho luồng cân.
- Không sửa JSON bằng tay khi Hub đang chạy. Thiết kế hiện tại cache trong **một tiến trình Hub**; không dùng cùng thiết lập cho nhiều worker/replica đang chạy độc lập. Nếu triển khai web garden/farm cần store chung có đồng bộ liên tiến trình riêng.
- Nên sử dụng HTTPS cho trang nhập mật khẩu. Đợt này không tự thay cấu hình TLS, IIS hoặc firmware.

## Cấu trúc mã

- `AttendanceModeState` / `IAttendanceMode`: snapshot bất biến và cổng đọc.
- `AttendanceModeStore`: lưu trạng thái nguyên khối, kiểm tra revision, thông báo UI sau khi lưu.
- `AttendanceModeManager` / `SqlAttendanceAdministratorVerifier`: giới hạn xác nhận, kiểm tài khoản quản trị từ SQL, gọi store. Dùng chung bộ kiểm mật khẩu với đăng nhập nhưng không tạo/đổi session.
- `AttendanceModePanel`: một component giao diện dùng lại ở hai trang; xóa mật khẩu khỏi model khi thành công, thất bại, hủy hoặc đóng component.
- Các điểm nhận firmware nhận chế độ qua dependency injection; không đọc file/SQL thiết lập cho từng phiếu cân.

## Xác minh

- Release build: **0 errors, 388 warnings mã nền**; không tuyên bố toàn solution sạch cảnh báo.
- HqAttendanceDiagnosis **178 PASS**, gồm 51 assertions mới cho quyền xác nhận, tài khoản khóa/NULL/không phải admin, SQL chỉ cần cột thực sự dùng, UI render/handler, xóa mật khẩu, file hỏng/thiếu Enabled, lỗi ghi file, xác nhận đồng thời, nạp lại sau restart, cả HTTP/SignalR cân/xưởng, backup, giữ ca cũ và mốc 15 phút.
- HqStableSwipe **512**, HqOnlineWeight **126**, ScaleOnlineWorkContext **71**, HqWeightBackup **175**, HqHttpBackup **111**, HqFunctionCards **37**, WorkshopDeviceFilter **74**: cùng bộ trên tổng **1.284 assertions đạt**.
- Test thêm 30 client SignalR WebSocket: **330 phản hồi, 330 phiếu mới trong SQL, 330 SETNHANVIEN**, đúng tổng và thứ tự. Lượt đầu cả đợt khoảng 1,990 giây; nóng ít dữ liệu 54–71 ms; có 100.000 phiếu mẫu 703–1.370 ms. Số đo LocalDB/loopback, không phải cam kết trên máy xưởng.
- SQL test dùng GUID database riêng, file mode ở thư mục test tạm, đã tự dọn. Không tạo file chế độ thực trong PMSHub/App_Data, không truy cập dữ liệu xưởng hoặc gửi tới firmware thật. Chưa kiểm thử thiết bị vật lý hoặc binary firmware đang nạp tại xưởng.

## Phản hồi quẹt Vào lặp — cập nhật 27/10/2026

- Phản hồi chấm công tại xưởng khi chưa đủ 15 phút hoặc gửi Vào lặp có `success=true`, `duplicated=true`, đầy đủ mã thẻ/mã nhân viên/tên nhân viên/giờ quẹt/xưởng/thiết bị. Theo yêu cầu mới, bỏ thông báo thử nghiệm và gửi đúng **đã quẹt vào** trong cả `trangThai` và `message` để firmware đọc theo trường đang dùng.
- Thành công ở đây là xác nhận đã nhận diện nhân viên và người này đã Vào trước đó, **không phải đã ghi một lượt chấm công mới**. Không lưu thông báo vào SQL; giữ nguyên lượt Vào và mốc 15 phút, đủ 15 phút mới cho Ra.
- HTTP và SignalR dùng chung phản hồi. Gửi lại cùng Id vẫn xác nhận đúng lượt gốc như trước. Thẻ chưa đăng ký, trạng thái Tắt, lỗi giờ và các lỗi khác không đổi.
- Build 0 errors (388 warnings mã nền), HqAttendanceDiagnosis **179 PASS**, gồm kiểm tra danh tính Unicode, Vào lặp trước/sau 15 phút, HTTP/SignalR và quẹt đồng thời không thêm dòng SQL. Chưa deploy hoặc thử màn hình cân thật.
