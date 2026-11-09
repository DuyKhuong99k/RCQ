# Sửa đồng bộ cấu hình cân và startup SQL — 25/10/2026

## Phạm vi

Mã nguồn: `D:\RCQ\RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`.

Đã sửa code và build Release. Chưa publish, chưa dừng/restart Hub đang chạy, chưa triển khai xuống xưởng. Không sửa firmware, không đổi dữ liệu thật, không thay hai file cấu hình đang có thay đổi của người dùng. **Không có bảng/index/script SQL mới cho lượt sửa này**; các yêu cầu schema của phiên bản ca theo bộ mã trước đó vẫn cần được đáp ứng.

## 1. Không dùng mã cũ trong khoảng Set → xác nhận

`HqScaleSelection` giữ trạng thái riêng cho từng đối tượng máy đang đăng ký:

- Set từ web cho thành phẩm/nguyên liệu/lô/size đánh dấu trường đó đang chờ xác nhận, trước khi gửi lệnh.
- Giữ mã cũ là giá trị đã biết, nhưng **không cho dùng nó để mở ca mới/ghi phiếu mới khi đang chờ**. Không tự gán mã mới rồi giả định cân đã nhận.
- Xác nhận dùng `SendSelectedData` hoặc `SendSelectedData2` hiện có của firmware. Chỉ nhận từ kết nối hiện tại của đúng máy và đúng giá trị đang chờ; phản hồi cũ khác giá trị không xóa trạng thái chờ.
- Khi nhiều trường đang đổi, phải xác nhận đủ tất cả mới sẵn sàng. Xác nhận và đọc bộ mã được bảo vệ đồng bộ, không đọc nửa bộ cũ/nửa bộ mới trong cùng batch.
- Sau 10 giây chưa xác nhận chỉ đổi thông báo thành **“Chưa xác nhận mã. Bấm Set lại”**; không tự mở khóa, không có timer hay vòng lặp SQL. Operator có thể Set lại/đổi lựa chọn để khôi phục; phải có xác nhận phù hợp mới sẵn sàng.
- Gửi Set bị lỗi cũng không tự trở về Ready vì gói có thể đã đến cân trước lúc kết nối báo lỗi.
- Mỗi request giữ một snapshot bất biến. Nếu snapshot thay đổi trong lúc xếp hàng, trả **“Mã cân vừa đổi. Quẹt lại”** thay vì lưu phiếu bằng mã đã lỗi thời.

Không thêm round trip SQL để theo dõi xác nhận; trạng thái nằm trong RAM. Khóa theo từng cân, không khóa toàn bộ 30 máy. Set, xác nhận, CHAMCONG và LITE dùng cùng cơ chế xếp hàng theo cân; SQL vẫn dùng khóa thẻ để đồng bộ giữa các cân/process.

## 2. Các hành vi giữ nguyên

- Quẹt Ra vẫn dùng được khi mã mới đang chờ xác nhận; giờ Ra/ca gốc vẫn do nghiệp vụ hiện tại kiểm tra.
- Cùng thành phẩm + nguyên liệu + xưởng được cân máy khác; khác một mã phải Vào ca mới, đóng ca cũ trong transaction.
- Quy tắc nhóm ở thời điểm sửa selection là không bắt Vào/Ra. **Đã được thay thế ngày 26/10 theo yêu cầu mới:** online nhóm phải theo ca như nhân viên; xem `HQ-Firmware-Responses.md`. Ca thường/ca đêm và số kg net vẫn giữ nguyên.
- Id đã lưu, payload đúng vẫn trả xác nhận và tổng đầy đủ, ngay cả đang có Set mới chờ xác nhận. Không INSERT/cộng thêm, không phụ thuộc cấu hình mới để khẳng định phiếu cũ.
- US/thiếu trạng thái online vẫn không lưu; ST/0 vẫn chỉ nhận diện nhân viên; không thêm chặn 30 giây online.
- Offline HTTP/backup giữ cách xét payload lịch sử riêng, không bị trạng thái Set online can thiệp.
- Không đổi tên lệnh/thứ tự tham số firmware: BOARD nhận 2 tham số cho thành phẩm/nguyên liệu/size, 1 tham số cho lô. `SendSelectedData2` trả lệnh xuống cân nhưng không được hiểu thành một Set mới và gây chờ xác nhận vô hạn.

Đã đọc nguồn firmware cục bộ `PhanBach_CanThongKe-TheAn/.../FW/V3.0/ESP32/src/pms_wss.cpp`: nhánh `SETTHANHPHAM` cập nhật mã/tên và gửi lại `SendSelectedData`. Đây là kiểm tra mã nguồn hiện có, không phải kiểm chứng binary/thiết bị vật lý tại xưởng.

## 3. Startup trước dịch vụ nền

`PMSHub/Program.cs` gọi `HqDatabaseStartup.InitializeAsync` trước đăng ký/khởi chạy hosted services:

1. Yêu cầu file `wwwroot/JsonFuFunc/DatabaseConnectSetting.json` tại ContentRoot.
2. Nạp AppViewModel/config một lần ở startup, không chờ trình duyệt/cân đầu tiên kích hoạt lazy init.
3. Kiểm tra DAO/EF đã có cấu hình thống nhất, có server/database và thông tin xác thực cần thiết.
4. Thử SQL **SELECT 1**, deadline 10 giây, không pooling cho kết nối probe dùng một lần.
5. Thiếu/sai cấu hình hoặc SQL chưa truy cập được: dừng startup trước khi nhận phiếu, thông báo không chứa password/connection string. Không dùng fallback trỏ sang database khác.

SELECT 1 không chứng minh đủ quyền INSERT/schema/index hay tính sẵn sàng lâu dài; lỗi SQL trong từng lượt cân vẫn do writer hiện tại xử lý. Đổi hành vi có chủ đích: SQL chưa sẵn sàng lúc khởi động thì Hub chưa chạy, thay vì chạy nửa chừng với tài khoản rỗng rồi nhận firmware.

## 4. Log phục vụ lỗi “chưa Vào”

- `[HQ ATTENDANCE RESULT]`: Id, hướng Vào/Ra, giờ thiết bị, thành phẩm/nguyên liệu đã chụp, revision, Success và Code.
- `[HQ LITE REJECT]`: máy, Id, giờ thiết bị, SaveStatus, lý do. Không log mỗi lượt ST thành công; bỏ log từ chối dự kiến của US và ST/0 để hạn chế nhiễu.
- `[HQ ATTENDANCE REJECT]`: khi kiểm tra ca thất bại, ghi giờ xét, root/snapshot tìm được và bộ mã đang xét. Không mở truy vấn SQL phụ chỉ để log.
- Các log request chỉ dùng 4 ký tự cuối thẻ để đối chiếu, không in password hoặc toàn bộ payload. Log mới sau COMMIT được bảo vệ để lỗi logging không biến Vào thành công thành phản hồi thất bại.

**Chưa kết luận mọi lỗi “chưa Vào” trước đó đều do cửa sổ Set/echo.** Đã sửa nguyên nhân lệch mã tái hiện được và thêm thông tin để xác định chính xác lỗi còn lại khi có log thực tế. Không tự mở ca/bỏ kiểm tra để che lỗi.

## 5. Kiểm thử

Build Release cuối: **0 errors**, còn **181 warnings** ở lượt incremental (full rebuild trước đó 388 warnings của solution). Không tuyên bố warning-free.

| Bộ kiểm thử | Kết quả |
| --- | ---: |
| HqAttendanceDiagnosis — đổi mã, timeout/echo, route Vào/LITE thật, startup, logging failure | 47 PASS |
| HqStableSwipe — US/ST, 0 kg, phản hồi/transport firmware | 257 PASS |
| HqOnlineWeight — Id, race, transaction async, deadline, tổng | 112 PASS |
| HqHttpBackup — payload offline không có ST, HTTP controller/SQL/online retry | 111 PASS |
| ScaleOnlineWorkContext — bộ mã/xưởng, ca đêm, rollback, đồng thời | 71 PASS |

Test mới đi qua **CommunicationService.Route CHAMCONG → ScaleAttendanceProcessor thật → SQL thật trong DB GUID LocalDB**, rồi Route LITE → CommitService thật. Bao gồm Vào rồi cân ngay lần đầu, Set chưa xác nhận không tạo ca/phiếu cũ, đổi thành phẩm và nguyên liệu rồi Vào/cân lần đầu, cân khác cùng bộ mã, Vào lặp, Ra cân khác và cân sau Ra bị chặn đúng. Bốn phiếu được chấp nhận tạo đúng bốn dòng, không có dòng từ request bị chặn.

Các cache/transport được test double khi không cần thiết bị vật lý. Chỉ tiến trình test đặt EF connection vào DB GUID riêng, khôi phục lại ở finally và xóa DB test. Không chạy toàn bộ PMSHub startup/background job trỏ vào SQL thật để test.

### Đo lại 30 kết nối sau sửa

Hai lần chạy, mỗi lần 11 đợt × 30 client SignalR WebSocket thật: **660/660 phản hồi thành công, 660 dòng phiếu mới, 660 SETNHANVIEN**, xác minh Id và tổng rổ/kg. Mỗi đợt nhả 30 client cùng lúc, mỗi máy một nhân viên thường, cùng bộ mã/xưởng; có ca mở sẵn.

| Trường hợp | Thời gian hoàn tất cả 30 client, hai lần đo |
| --- | ---: |
| Đợt LITE đầu tiên | 1,730–2,016 giây |
| Đã nóng, ít dữ liệu | 50,78–65,46 ms |
| Thêm 100.000 phiếu cùng nhóm tính tổng | 620,71–1.198,65 ms |

Trong 10 đợt sau khi thêm 100k dòng, 9 đợt hoàn tất trong 620,71–687,82 ms; **một đợt có một client chậm khoảng 1,20 giây**. Không bỏ số đo này và chưa quy nguyên nhân khi chưa có trace thành phần ở mức dưới 2 giây.

Chi tiết lần cuối trên DLL cuối (ms; mỗi dòng 30/30 thành công và INSERT):

| Đợt | Cả đợt | P95 từng client | Client chậm nhất |
| --- | ---: | ---: | ---: |
| Đầu tiên | 1730,35 | 1713,80 | 1726,89 |
| Nóng ít dữ liệu 1 | 58,41 | 57,80 | 57,87 |
| Nóng ít dữ liệu 2 | 50,78 | 49,70 | 49,80 |
| Nóng ít dữ liệu 3 | 65,46 | 64,37 | 65,20 |
| Nóng ít dữ liệu 4 | 52,75 | 51,01 | 51,45 |
| Nóng ít dữ liệu 5 | 54,33 | 51,51 | 53,54 |
| 100k dòng 1 | 1198,65 | 650,73 | 1198,28 |
| 100k dòng 2 | 630,83 | 627,02 | 629,10 |
| 100k dòng 3 | 620,71 | 617,11 | 620,54 |
| 100k dòng 4 | 636,92 | 631,54 | 636,55 |
| 100k dòng 5 | 640,89 | 639,36 | 640,63 |

Máy đo i7-10750H, khoảng 32 GB RAM, .NET 8.0.31, LocalDB Express 17.0.4025.3, RCSI ON và index theo migration hiện có. Kết nối WS loopback, không đo TLS, LAN/Wi-Fi, IIS, background jobs hay báo cáo chạy song song; auth/cache nạp sẵn. Không phải phần cứng xưởng i5/RAM 8 GB, không phải cam kết SLA hoặc độ ổn định cả ngày. Chi tiết phương pháp trong `HQ-WSS-30-Clients-20261025.md` vẫn áp dụng; không gửi phiếu thử vào SQL xưởng.

## 6. Publish và kiểm tra tại xưởng

1. Publish **project PMSHub trong đúng thư mục RCQ-ProductMaterialWorkshop**, cấu hình Release, ra thư mục mới; không lấy lại release nhánh 123.
2. Sao lưu bản đang chạy. Giữ cấu hình SQL/site, `DataProtection-Keys` và quyền thư mục của server; không chép cấu hình test từ máy phát triển sang xưởng.
3. Thay bản publish trong khoảng bảo trì và khởi động lại đúng site/process. Code build trên máy này không tự cập nhật vào tiến trình Hub đang chạy.
4. Xác nhận startup không còn `Login failed for user ''`/`ConnectionString ... not initialized`. Nếu startup dừng, sửa cấu hình/kết nối SQL trước, không tắt kiểm tra mới.
5. Test một cân trước: Set mã mới, đợi Hub hiện đúng mã, Vào, cân; đổi thành phẩm, xác nhận, Vào và cân; làm tương tự với nguyên liệu. Kiểm tra ca/snapshot/phiếu trong SQL đúng bộ mã.
6. Test mất xác nhận: phải báo đang đổi/chưa xác nhận và không có phiếu mới; Set lại rồi xác nhận được mới tiếp tục. Quẹt Ra vẫn hoạt động theo ca gốc.
7. Test cân thứ hai cùng bộ mã, test offline backup, rồi mới mở rộng cả xưởng. Nếu còn báo chưa Vào, lấy các dòng RESULT/REJECT cùng Id/giờ; không xóa ca/phiếu để thử.

Không bảo đảm tốc độ/sự ổn định cả ca làm việc chỉ từ test local. Còn phụ thuộc SQL/index, dữ liệu, tải báo cáo/backup, IIS, mạng và firmware thực tế.
