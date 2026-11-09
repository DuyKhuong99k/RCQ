# Nạp thẻ nhân viên với cân đặt loại Offline trên Hub

## Phạm vi

- Trang `/TheNhanVien` không loại `AppType._type5` khỏi danh sách máy cân nữa.
- Danh sách lấy từ `DeviceViewService` của phiên đăng nhập: đúng quyền khu vực và xưởng đã chọn. Trang nạp thẻ tự nạp phạm vi này, không cần mở trang Thiết bị trước.
- Chỉ cho chọn cân có `IsConnected`, `IsActive` và `ConnectionId` không rỗng. Kiểm tra lại khi Bắt đầu.
- Bỏ việc tự gán `IsConnected=true` cho cân Offline khi tải cấu hình. Trạng thái kết nối do WSS quản lý, không suy ra từ loại hình cân.
- Cập nhật UID qua sự kiện thiết bị; không điền lại UID cũ khi một cân khác cập nhật. Dừng nạp khi đóng hộp thoại/rời trang, mất kết nối hoặc cân chuyển ra khỏi phạm vi. Không tắt chế độ nạp do phiên khác mở.
- Không sửa firmware, đổi chế độ cân, luồng ghi phiếu cân/chấm công hay tạo hồ sơ nhân viên mới. Không yêu cầu SQL bổ sung.

## Điều kiện đọc thẻ

`Offline` trong cấu hình Hub khác với việc mất kết nối WSS. Cân vẫn cần kết nối và gửi UID lên WSS. Hub không tự đọc được UID nếu firmware không gửi gói chứa mã thẻ.

Khi `IsNapThe=true`, nhánh LITE hiện có nhận `TheId`, trả `IsDataAction=0` và thông báo `Nạp Thẻ`, không chạy nghiệp vụ ghi phiếu cân. Thao tác gán/xóa thẻ ở trang Thẻ Nhân Viên vẫn theo cơ chế hiện có.

## Kiểm tra tại cân thật sau rebuild/publish

1. Giữ loại cân `Offline` trên Hub, cho cân kết nối WSS. Mở trực tiếp trang Thẻ Nhân Viên, chọn nhân viên: cân đúng xưởng phải hiện trong danh sách.
2. Bấm Bắt đầu, quẹt thẻ. Kiểm tra ô Thẻ Id nhận đúng UID và không sinh phiếu cân do lượt nạp thẻ.
3. Bấm Dừng hoặc đóng hộp thoại. Cân phải hết trạng thái nạp thẻ trên Hub; quy trình cân thường không đổi.
4. Ngắt WSS: cân không còn được chọn để bắt đầu. Cân Offline khác xưởng/ngoài quyền vẫn bị ẩn.
5. Nếu không nhận UID, kiểm tra log gói gửi lên từ firmware, không đổi chế độ cân một cách tự động để thử.

## Kiểm thử tự động

`tests/WorkshopDeviceFilter/CardEnrollmentChecks.cs` dùng dữ liệu giả và gọi bộ lọc, Start/Stop, cập nhật UID của component thật, cùng nhánh LITE thật trong CommitService. Không kết nối SQL hoặc thiết bị xưởng.

Kiểm tra: mọi AppType (kể cả Offline), quyền/xưởng, kết nối giả/đứt, tranh nạp thẻ, gửi thông báo thất bại, cleanup, UID cũ và LITE âm/0/dương chỉ trả UID khi nạp thẻ.

## Làm mới danh sách để cân bấm Nạp khi cần

- Mỗi lần đăng nhập hợp lệ tại `/api/loginlocal/`, Hub chờ đọc lại hai danh sách từ SQL trước khi chuyển tới trang đích (kể cả trang Thiết bị). Không chạy khi sai mật khẩu/xưởng không hợp lệ, không có vòng lặp cập nhật nền. Nút đăng nhập được khóa trong lúc xử lý.
- Nếu tải lại lỗi sau xác thực, vẫn cho đăng nhập với bộ dữ liệu cũ và báo cảnh báo `Đăng nhập thành công. Chưa tải lại được nhân viên/thẻ; đang giữ danh sách cũ.` Không báo sai mật khẩu hoặc xóa danh sách đang phục vụ cân.
- Trang Thẻ Nhân Viên tự đọc lại `NhanVienDaiThanh` và `TheTu` khi mở trang, bấm Thêm/Sửa/Tải lại, và sau khi gán/sửa/xóa thẻ thành công.
- Hub dùng `HubEmployeeCardCatalog`: đọc SQL bất đồng bộ, chờ tối đa 15 giây tính cả xếp lượt; chỉ công bố collection mới khi đọc đủ cả hai bảng. Không xóa bộ nhớ trước khi SQL trả kết quả. Đọc lỗi giữ bộ dữ liệu trước đó, ghi log và báo trên web; đã lưu SQL thì báo rõ `Đã lưu`, không yêu cầu thêm lại.
- Các API `api/NhanVien/Gets` và `api/The/GetTheNhanViens` đọc danh sách mới trong Hub. Giữ nguyên JSON và lọc ngày mà firmware đang sử dụng; thẻ cùng nhân viên thêm thứ tự UID để kết quả phân trang xác định.
- Không tự gửi lệnh đồng bộ tới cân. Người vận hành bấm **Nạp nhân viên**, sau đó **Nạp thẻ nhân viên** trên cân khi cần. Cân Offline không tự có dữ liệu mới nếu chưa nạp xuống.
- Thay đổi này làm mới hồ sơ đã có trong SQL và ánh xạ thẻ; không bổ sung màn hình tạo hồ sơ nhân viên mới. Nếu tạo/sửa hồ sơ ở phần mềm báo cáo hoặc SQL, mở lại trang Thẻ Nhân Viên hoặc bấm Tải lại để Hub nhận dữ liệu đó.
- Không thêm truy vấn SQL vào từng lượt cân, không thay schema, firmware hoặc cấu hình SQL. API delta vẫn theo mốc ngày hiện có, không tự đổi thời gian các bản ghi cũ.

Kiểm thử bổ sung: `EmployeeCardCatalogChecks` kiểm tra cache không rỗng trong lúc tải, giữ dữ liệu khi lỗi/hủy, tải song song, API JSON và phân trang; `EmployeeCardCatalogSqlChecks` chạy câu SQL thật trên database LocalDB GUID riêng, thử thêm/sửa/xóa rồi làm mới và xóa database test khi hoàn tất. Không thay cấu hình DAO hoặc truy cập SQL xưởng.

## Cleanup — 27/10/2026

- `LoginLocal.razor` và `TheNhanVien.razor` giữ giao diện và khai báo dịch vụ; bỏ các khối giao diện cũ đã comment. Logic đăng nhập nằm trong `LoginLocal.razor.cs`; thao tác thẻ nằm trong `TheNhanVien.Cards.cs`, tách khỏi quản lý kết nối/nạp thẻ trong `TheNhanVien.razor.cs`.
- Bỏ biến và dịch vụ được inject nhưng không dùng; thay `async void` bằng `Task` cho phím đăng nhập, và hàm đồng bộ cho các handler không có await. Lỗi dùng logging có ngữ cảnh, không ghi trực tiếp Console.
- Nhánh sửa thẻ được làm phẳng bằng các bước kiểm tra sớm; vẫn dùng bản sao trước SQL, chỉ cập nhật cache sau khi lưu thành công. Gom xử lý lỗi và đóng hộp thoại dùng chung, giữ các thông báo và điều kiện nghiệp vụ.
- Tách `IHubEmployeeCardCatalog` khỏi lớp thực thi; gom SQL và thông số timeout thành hằng số có tên. Giữ nguyên truy vấn, 15 giây tổng lượt tải, 10 giây câu SQL, và cơ chế chỉ công bố cache khi đọc đủ hai bảng.
- Không thêm cập nhật nền, webhook, SQL vào từng lượt cân, hoặc lệnh firmware. Không đổi cấu hình cá nhân, schema, luồng đăng nhập/xác thực hoặc định dạng phản hồi cân.
- Kiểm thử vẫn gọi các handler thật trên DLL sau tách partial class, không chỉ kiểm tra tên file. Các kiểm tra đọc source được cập nhật để kiểm tra cả markup và code-behind.
- Xác minh sau cleanup: build Release 0 lỗi (incremental 196 cảnh báo legacy; chưa xử lý toàn bộ solution); WorkshopDeviceFilter 134, HqAttendanceDiagnosis 185, HqStableSwipe 512 kiểm tra đạt, tổng 831. SQL dùng LocalDB GUID riêng đã tự xóa; không truy cập SQL xưởng hoặc cân thật. Chưa commit/push/publish.
