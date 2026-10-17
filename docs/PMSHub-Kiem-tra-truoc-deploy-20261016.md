# PMSHub — kiểm tra trước triển khai 16/10/2026

## Kết luận

**Chưa chấp thuận triển khai đại trà.** Build Release và kiểm thử hồi quy qua, nhưng còn các vấn đề bảo mật/tính toàn vẹn dữ liệu dưới đây.
Đây là rà soát source tại `D:/RCQ/RCQ`, không xác nhận byte-for-byte bản DLL đang cài ở xưởng.
Không sửa code, không deploy, không gửi lệnh cân thật, không ghi dữ liệu SQL sản xuất trong lượt kiểm tra này.
Không thực hiện thử đăng nhập vượt quyền hay gửi dữ liệu giả vào Hub dưới xưởng.

## Cần xử lý trước khi dùng chính thức

### P0 — Luồng đăng nhập URL bỏ qua kiểm tra token

- `PMSHub/Components/Pages/Login.razor`: nhận hai tham số và gọi `loginService.Login`.
- `Services/LoginService.cs:27`: overload nhận username/token nhưng không kiểm tra token; gọi đăng nhập theo username rồi trả true.
- Hàm đăng nhập theo username tạo session nếu tìm được tài khoản.
- Rủi ro: tạo phiên mà không chứng minh quyền sở hữu tài khoản. Việc bảo vệ các trang sau đăng nhập không khắc phục đường tạo phiên này.
- Cần tắt đường cũ nếu không dùng, hoặc xác minh token có chữ ký/hạn sử dụng và trạng thái tài khoản. Không sửa bằng cách chỉ ẩn liên kết.
- Chưa khai thác HTTP trên xưởng; phát hiện dựa trên luồng code đầy đủ.

### P1 — Đồng bộ chấm công offline có thể ACK nhầm bản ghi xung đột

- `PMSHub/Controllers/ChamCongController.cs:89`: chỉ kiểm tra tồn tại Id rồi trả OK; nhánh bắt lỗi PK cũng chỉ kiểm tra tồn tại.
- Id được chuẩn hóa theo máy + giây. Hai lượt khác thẻ/trạng thái nhưng cùng Id không được kiểm tra sự đồng nhất trước ACK.
- Rủi ro: lượt gửi sau không được lưu nhưng firmware nhận thông báo đã tồn tại thành công.
- Cần phân biệt gửi lại đúng dữ liệu (OK/idempotent) với xung đột Id (lỗi rõ ràng, giữ dữ liệu gốc và đủ thông tin đối soát).
- Giữ giao thức firmware nếu có thể; kiểm thử request trùng giống nhau, khác thẻ, khác trạng thái và đồng thời.

### P1 — Chuyển ca mới chỉ cập nhật DB, chưa đảm bảo trạng thái firmware offline

- `Services/ScaleAttendanceTransferService.cs`: transaction tạo Ra máy cũ/Vào máy mới, có audit và giờ server; không có bước đồng bộ trạng thái nội bộ của firmware.
- Máy mới có ca trong SQL không có nghĩa firmware offline đã mở ca tương ứng. Dữ liệu offline cũ gửi lên sau có thể mâu thuẫn ca chuyển.
- Cần quy định rõ chuyển máy trong chế độ nào; chưa cho dùng thao tác này với offline khi chưa nghiệm thu đồng bộ muộn và lượt quẹt tiếp theo trên hai cân thật.
- Không tự suy ra chế độ firmware từ `MayCan.AType`: hình thức Offline của Hub và mode firmware là hai cấu hình khác nhau.

### P1 — Đầu nhận thiết bị thiếu ràng buộc danh tính/phạm vi

- `Services/CommunicationService.cs:14`: xác nhận client chủ yếu dựa vào ConnectionId có trong danh sách và Id tự khai báo, chưa có chứng thực thiết bị.
- `Services/UserService.cs`: client tự khai Id cân đã đăng ký có thể được đặt trạng thái active; cần xử lý cả trùng kết nối cùng Id.
- `PMSHub/Controllers/ChamCongController.cs` và `ChamCongTaiXuongController.cs`: đầu nhận POST không có kiểm tra danh tính thiết bị đủ mạnh trong action.
- `Services/WorkshopHub.cs:82`: CheckIn nhận deviceCode/xuong từ tham số; Save ghi Xuong từ đầu vào mà chưa đối chiếu đủ thiết bị đang giữ ConnectionId.
- Endpoint sync chấm công chưa yêu cầu phiên quản trị trong action.
- Rủi ro: client không đúng thiết bị trong mạng có thể gửi/gọi thao tác với mã máy/xưởng tự khai báo.
- Cần tách endpoint quản trị (session/quyền) và thiết bị (đăng ký/IP/allowlist hoặc chứng thực phù hợp giao thức). Dùng mạng riêng/firewall làm lớp bổ sung, không coi đó là xác thực đầy đủ.

### P1 — Chưa giới hạn ngày giờ tương lai ở đầu nhận lượt quẹt

- `Services/ScaleAttendanceProcessor.cs`: kiểm tra định dạng/thứ tự nhưng không có cận trên so với giờ máy chủ trước khi ghi lượt mới.
- `PMSHub/Controllers/ChamCongController.cs`: tương tự với giờ phát sinh offline.
- Rủi ro: cân sai ngày/giờ có thể tạo sự kiện tương lai, sau đó các lượt thực tế bị báo cũ hơn lịch sử.
- Cần đặt ngưỡng lệch hợp lý cho online; offline giữ được dữ liệu quá khứ nhưng phải tách sự kiện tương lai bất thường để đối soát.
- Không được áp dụng quy tắc "chỉ nhận hôm nay" cho toàn bộ sync offline.

### P2 — Cân loại hình Offline có thể xanh dù chưa kết nối

- `Services/MayCansService.cs:2437` tự đặt `IsConnected=true` cho `_type5` lúc Load.
- Phải phân biệt hình thức cân, có kết nối WSS và có heartbeat. Không đặt xanh chỉ dựa AType.
- Gây hiểu nhầm vận hành; không kết luận đây là lỗi mất dữ liệu tự thân.

### P2 — Đăng nhập mật khẩu chưa chặn tài khoản ngừng hoạt động

- `Services/LoginService.cs:85`: kiểm tra mật khẩu nhưng không thấy điều kiện IsActive trước tạo phiên.
- Giao diện `LoginLocal.razor` gọi trực tiếp luồng này, không có bước chặn bổ sung tương đương.
- Cần kiểm tra active khi đăng nhập và tái kiểm tra quyền đối với thao tác quản trị nhạy cảm. Chuyển máy đã có kiểm tra IsAdmin/IsActive riêng.

## Rủi ro vận hành/cấu hình cần hoàn tất

- `Program.cs:43` bật DetailedErrors cả production: tắt khi triển khai thực tế, giữ log chi tiết phía server có quyền truy cập.
- Cấu hình kết nối được lưu dưới wwwroot (dạng mã hóa); không để file cấu hình nhạy cảm thành static asset công khai. Không xem mã hóa bằng khóa có sẵn trong ứng dụng là thay thế ACL.
- Ba hosted service chạy ở mỗi instance; các dịch vụ đánh dấu ca đọc bảng chung. Không để hub demo trỏ DB thật và tự chạy nền ngoài kế hoạch.
- Lịch sử xưởng chỉ có khi chạy SQL bổ sung và chuyển xưởng bằng Hub mới. Không trộn Hub cũ/SQL cập nhật trực tiếp không ghi mốc.
- Những mã thẻ chỉ khác khoảng trắng đã được phát hiện trước đó: web báo cáo đã giữ ánh xạ riêng, nhưng đầu vào Hub chuẩn hóa Trim. Phải đối soát danh mục thẻ trước dùng thật; không UPDATE Trim hàng loạt làm gộp nhân viên.
- Có DbContext dài hạn trong singleton MainViewModel và một số thao tác thiết bị; cần thử truy cập đồng thời và theo dõi lỗi concurrency. Chưa kết luận có lỗi thực tế từ kiểm thử tải vì chưa chạy tải.
- Chốt quy trình backup, phục hồi, quản lý phiên bản DLL, log và rollback trước triển khai.

## Những phần đã có kiểm chứng tích cực

- Chấm công online có khóa SQL theo thẻ/máy; không tự nhận Ra sai máy.
- Chuyển ca dùng transaction; giờ chuyển lấy server UTC+7; ghi đúng tài khoản và lý do, không nhận giờ từ UI.
- Ca thường/ca đêm có quy tắc giới hạn và xử lý quẹt lặp riêng.
- Backup phiếu HQ chuẩn hóa Id, đối chiếu dữ liệu đã lưu, không ghi đè lịch sử tùy tiện.
- Xưởng được snapshot theo thời điểm ca và có lịch sử chuyển cân; không gán lịch sử chỉ từ MayCan hiện tại.
- Danh sách cân phân theo xưởng và khu vực được phép; chuyển xưởng cập nhật danh sách của từng phiên.

## Kiểm thử đã chạy trong lượt kiểm tra

- Build PMSHub Release --no-restore: thành công, 0 lỗi.
- HqWeightBackup: 138 checks qua, bộ nhớ, không SQL writes/lệnh cân.
- WorkshopDeviceFilter/HTTP logout: 39 checks qua.
- ScaleAttendanceProtocol: qua các tình huống giao thức hiện có, không SQL writes.
- ScaleAttendanceTransfers.Tests.ps1, ScaleAttendanceNightShift.Tests.ps1, WorkshopAttendanceDirection.Tests.ps1: qua.

Các bộ kiểm thử này chưa chứng minh đã khắc phục các phát hiện ở trên. Chưa thử payload độc hại trên production, chưa thử tải nhiều cân, chưa thử firmware offline thật, chưa thử mất SQL hoặc mất điện giữa lượt gửi.

## Nghiệm thu bắt buộc trên bản sao DB + cân thật trước rollout

1. Đăng nhập sai token/mật khẩu/tài khoản khóa phải bị từ chối; quyền quản trị không cấp từ client.
2. POST trùng đúng dữ liệu không tăng dòng; trùng Id khác thẻ/trạng thái không được ACK thành công.
3. Vào xưởng -> Vào cân -> cân sản lượng -> Ra cân -> Ra xưởng; đối chiếu số lượt, kg và thời gian.
4. Nghỉ trưa, chuyển cân, chuyển xưởng, ca thường/đêm qua 0 giờ, quên Ra hôm trước.
5. Mất mạng, tích dữ liệu offline, nối lại, sync nhiều lần; so sánh Id/số phiếu/tổng kg với dữ liệu gốc firmware.
6. Chuyển máy rồi quẹt tiếp/Ra trên hai cân; thử cả dữ liệu offline cũ gửi muộn. Không mặc nhiên coi DB đúng là firmware đã đúng.
7. Mất SQL và restart/recycle IIS: không báo lưu thành công khi chưa commit, thử lại không nhân đôi dữ liệu.
8. Nhiều cân/nhiều người quản lý thao tác cùng lúc; theo dõi độ trễ, CPU/RAM, SQL waits/deadlocks.
9. Thử restore backup và quay lại bản cũ theo quy trình. Không tháo schema lịch sử tùy tiện khi code mới đang chạy.

Chỉ chạy pilot có giám sát sau khi xử lý các mục P0/P1 phù hợp; chưa đánh dấu sẵn sàng sản xuất đại trà.
