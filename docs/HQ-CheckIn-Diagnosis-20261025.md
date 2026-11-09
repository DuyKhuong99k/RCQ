# Chẩn đoán Vào rồi cân báo chưa Vào/khác thành phẩm — 25/10/2026

> Đây là báo cáo chẩn đoán trước sửa. Sau khi người dùng yêu cầu thực hiện, code đã được sửa; xem `HQ-Selection-Startup-Fix-20261025.md`. Test HqAttendanceDiagnosis hiện là bộ hồi quy 47 PASS thay cho bản tái hiện lỗi ban đầu bên dưới.

## Kết luận đã kiểm chứng

**Tái hiện được một cửa sổ lệch trạng thái khi Set thành phẩm/nguyên liệu từ web. Chưa chứng minh đây là nguyên nhân của mọi lượt lỗi tại máy thật, và chưa tái hiện lỗi “chưa Vào” sau một lượt Vào đã commit với bộ mã không đổi.**

`MayCansService.CommandSetThanhPham` gửi `SETTHANHPHAM` xuống firmware nhưng không cập nhật `MayCan.MaThanhPham` của Hub; `CommandSetNguyenLieu` có cùng hành vi. Hub chờ firmware trả `SendSelectedData/SendSelectedData2` để cập nhật thuộc tính.

Trong thời gian chờ phản hồi cấu hình:

1. Hub giữ thành phẩm BV; web gửi Set BVT1 xuống cân.
2. CHAMCONG Vào đến trước bản trả mã mới: `CommunicationService.Route` lấy BV từ máy trong cache; ca/snapshot lưu BV và trả thành công.
3. Firmware trả mã BVT1; cache máy đổi thành BVT1.
4. LITE lấy BVT1, so với snapshot BV và trả `Khác thành phẩm. Quẹt Vào`.

Test gọi **hàm CommandSet thật trong DLL Release**, với transport giả lập để bắt lệnh gửi. Sau đó dùng **ScaleOnlineAttendanceService.Process thật và GetOnlineErrorAsync thật trên SQL LocalDB riêng**. Bước cập nhật cache sau firmware echo được mô phỏng trực tiếp bằng gán thuộc tính, đúng thao tác reflection của handler hiện tại; không giả lập firmware vật lý hay kiểm chứng thứ tự mạng tại xưởng.

Lệnh Set này không nằm trong phần sửa mới của MayCansService; quy tắc online mới so chính xác thành phẩm/nguyên liệu/xưởng khiến cửa sổ đồng bộ cũ trở nên quan trọng. Không nên khắc phục bằng cách bỏ kiểm tra bộ mã hoặc tự nhận phiếu khi chưa có Vào.

## Đối chiếu dữ liệu cục bộ từ log người dùng

Đã SELECT chỉ đọc trên `DESKTOP-2A1MPUC\SQLEXPRESS`, database `PMS_HQ`, cân 7779460, đúng ngày/giờ log. Không UPDATE/INSERT dữ liệu thật.

| Giờ | Dữ liệu đã lưu |
| --- | --- |
| 09:46:03 | Ra, liên kết ca Vào 09:31:19, bộ mã BV/XOAIDONG/1 |
| 09:46:13 | Vào, snapshot BV/XOAIDONG/1 |
| 09:46:30, 09:46:56, 09:47:18 | Phiếu cân BV, đúng thẻ của ca |
| 09:47:59 | Ra tự động ca BV và Vào BVT1/XOAIDONG/1 trong cùng thời điểm |
| 09:48:26 | Phiếu cân BVT1 đã lưu |

Tại thời điểm kiểm tra, truy vấn ca mở trả đúng một ca 09:47:59, `Isforget=0`, snapshot BVT1/XOAIDONG/1. Không thấy ca mất trong chuỗi này. Các Id RX không xuất hiện thành dòng SQL không tự chứng minh lỗi: lượt Vào/Ra lặp có thể trả AlreadyRecorded mà không tạo bản ghi mới. Log TX hiện chỉ có hash nên không suy ra được thành công/từ chối của từng request chưa có payload.

SQL cục bộ hiện báo `ProductVersion=17.0.1000.7`, RCSI=0. Đây là cấu hình của database đã đọc tại máy này, không thay thế xác nhận cấu hình server xưởng; không tự bật RCSI trong lần chẩn đoán này.

## Lỗi SQL lúc startup trong log

Log ghi `Login failed for user ''` ở DeviceHeartbeatMonitor và `ConnectionString property has not been initialized` ở hai dịch vụ chốt ngày, sau đó các vòng chốt ngày thành công. Code cho thấy cấu hình `Base.Ins` khởi tạo rỗng, được nạp khi AppViewModel/MainViewModel khởi tạo; dịch vụ nền có thể chạy trước đó. Lỗi này xác nhận thứ tự khởi tạo chưa chắc chắn, **không chứng minh** các lỗi cân lúc 09:46–09:48 do đăng nhập SQL.

## Kiểm thử

- `ScaleOnlineWorkContext`: **71 PASS**, có SQL LocalDB: mở ca, đổi mã, Ra cân khác, ca đêm, đồng thời, async/sync policy.
- `HqAttendanceDiagnosis`: **10 PASS** chẩn đoán, gồm tái hiện khoảng Set/echo, cân ngay sau Vào commit, và giữ transaction Vào chưa commit để xác nhận lượt cân chờ cùng khóa card rồi đọc được ca mới ở lần đầu. PASS ở test tái hiện nghĩa là **đã xác nhận lỗi tồn tại**, không phải đã sửa lỗi.
- Các DB GUID test đều được xóa sau chạy.
- Không thay đổi production code, cấu hình, firmware, dữ liệu thật hoặc publish trong lần chẩn đoán này. Hai file cấu hình đang có sửa của người dùng được giữ nguyên.

Chạy lại (cần DLL Release mới nhất):

```powershell
dotnet run --project tests/HqAttendanceDiagnosis/HqAttendanceDiagnosis.csproj -c Release -p:NuGetAudit=false -p:UseSharedCompilation=false -- --sql-localdb
```

## Việc còn cần xác nhận trước khi sửa

1. Đồng bộ trạng thái Set tại Hub, xử lý thứ tự Set/echo/Vào/LITE thống nhất và bảo toàn trường hợp gửi thất bại/echo cũ; không đổi giao thức firmware.
2. Nạp cấu hình SQL trước khi dịch vụ nền và request nghiệp vụ được phép chạy.
3. Với lỗi “chưa Vào”, cần cặp request CHAMCONG Vào và LITE thất bại cùng hai phản hồi ReceiveData (Id, TheId, NgayGio, Code/MessStr/ErrorString). RX/TX chỉ có hash hiện tại chưa đủ; không được kết luận do tải 30 cân hoặc tự mở ca để che lỗi.
