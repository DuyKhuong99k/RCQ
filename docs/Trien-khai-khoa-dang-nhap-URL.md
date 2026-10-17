# Khóa đăng nhập bằng URL/token chưa xác thực

## Thay đổi

- `/api/login/...` chỉ chuyển về `/api/loginlocal/`, không tạo hoặc đổi phiên. Middleware xử lý trước session; trang Blazor cũ cũng không còn gọi đăng nhập.
- Xóa hai API `Login(username)` và `Login(username, token)` khỏi service/interface để không còn đường tạo phiên chỉ bằng tên tài khoản.
- Nút PMS Hub trên web chấm công mở trang đăng nhập tài khoản/mật khẩu của Hub, không gửi username hoặc token giả qua URL.
- Theo yêu cầu bổ sung, đăng nhập mật khẩu chỉ tạo phiên khi `NguoiDung.IsActive = 1`. `0` hoặc `NULL` bị từ chối. Không tự thay cờ trong SQL; chưa tự thu hồi phiên đã có khi tài khoản bị khóa sau đăng nhập.
- Truy vấn tìm tài khoản lúc đăng nhập dùng tham số SQL thay vì nối tên tài khoản vào câu lệnh. Mật khẩu sai/rỗng không tạo phiên; giữ định dạng mật khẩu đã lưu hiện tại.
- Không thay đổi firmware, bảng SQL, phiếu cân, chấm công, cờ nhóm hoặc phiên người dùng đang tồn tại trong code xử lý URL.

## Triển khai

1. Sao lưu bản publish và cấu hình hiện tại. Chọn thời điểm bảo trì: restart ứng dụng có thể làm gián đoạn kết nối cân/phiên, không thực hiện giữa lúc đang cân nếu chưa có phương án vận hành.
2. Publish và cập nhật toàn bộ bộ file Hub mới, không chỉ chép riêng `Services.dll` vì interface và component cũng đã đổi. Giữ cấu hình database/port và thư mục DataProtection-Keys của từng site.
3. Cập nhật web chấm công để nút Hub dùng URL mới. Nếu web chưa cập nhật, Hub mới vẫn chuyển URL cũ về trang mật khẩu.
4. Kiểm tra từng Hub đang hoạt động (không chỉ Hub demo): URL cũ không đăng nhập được, đăng nhập mật khẩu/chọn xưởng vẫn đúng, đăng xuất hoạt động, thẻ nhóm và nhân viên cân bình thường.
5. Người dùng mở Hub từ web phải xác thực riêng nếu chưa có phiên Hub. Không cần sửa tài khoản, mật khẩu hoặc SQL để dùng thay đổi này.

Kiểm thử cục bộ không chứng nhận DLL đã được cài trên server xưởng hoặc xác nhận toàn bộ các rủi ro trong báo cáo audit đã được xử lý. Các vấn đề audit ngoài đăng nhập URL không thuộc phạm vi bản sửa này.
