# Rà soát và clean code Hub — 28/10/2026

## Phạm vi và thứ tự ưu tiên

`D:\RCQ\RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`.
Ưu tiên: dữ liệu đúng, sau đó ổn định, rồi tốc độ. Không đổi firmware, nhánh 123,
PMS Báo Cáo Dev_Khuong, cấu hình kết nối hoặc schema/dữ liệu SQL xưởng.
Đây là rà soát trọng điểm đường HQ đang dùng, không phải chứng nhận toàn bộ module legacy đã sạch lỗi.

## Thay đổi trong lượt này

1. **Kg net và bì:** thêm `HqWeightValues` dùng chung cho online, Sync và HTTP backup.
   SQL lưu `decimal(18,3)`; không làm tròn ngầm, không cho giá trị vượt miền SQL.
   Kg hàng đã trừ bì có thể nhỏ hơn bì: ví dụ net 0,400 kg, bì 0,800 kg là hợp lệ.
   Sửa Sync trước đây chặn trường hợp này, thống nhất với HTTP/online; không trừ bì lần hai.
   Online ST bằng 0/âm vẫn chỉ nhận diện, không tạo phiếu; US/thiếu ST vẫn từ chối.
2. **Xác nhận lại cùng Id:** `MatchesExisting` đối chiếu thêm `STT` và `TheId`,
   ngoài các trường đã kiểm tra. Không ACK nhầm gói đổi ordinal/thẻ gốc; không ghi đè phiếu cũ.
   HTTP dùng cùng comparator thay cho kiểm tra lặp. Quy tắc thiếu metadata lịch sử và so mã
   không phân biệt hoa/thường được giữ nguyên.
3. **Tải danh mục nhân viên:** một expression ánh xạ dùng chung SQL và form.
   Truy vấn chỉ lấy các cột hiển thị/chỉnh sửa, không kéo ngày sinh/địa chỉ/điện thoại dư thừa.
   Giữ lọc xưởng phía SQL, quyền tài khoản, kiểm tra MNgay, journal và transaction.
4. **Tìm nhân viên:** trim từ khóa một lần và so trực tiếp các trường; bỏ mảng tạm từng dòng.
   Không truy vấn SQL theo từng phím. Không đổi cách tìm không phân biệt hoa/thường.
5. **Callback Blazor:** chín callback gửi lệnh từ Thiết bị và hai callback Thẻ chức năng
   chuyển từ `async void` sang `Task`; thao tác chỉ đổi trạng thái chuyển về `void`.
   Blazor có thể chờ tác vụ; không đổi nội dung lệnh hay bảng dữ liệu.
6. **Mã chết:** bỏ hai hàm HTTP private `.Result` không được gọi và timer private chưa bật
   từng ngắt cân loại Offline mỗi 5 giây. Không thay timer gom thông báo UI,
   heartbeat thật hoặc giao thức kết nối.

## Các nguyên tắc đã giữ

- Online chỉ chống ghi lại cùng Id, không thêm cooldown cùng kg/30 giây.
- Khóa card và máy/Id, transaction và kiểm tra ca nằm cùng lượt ghi SQL.
- ACK hoàn thành sau commit và tính đủ tổng; lỗi tổng sau commit vẫn mang `SAVED`,
  không giả vờ chưa lưu/không cộng tổng bịa/không tự INSERT lại.
- Kết nối gửi phản hồi là kết nối gốc; không chuyển phản hồi cũ sang kết nối mới.
- Cùng thành phẩm/nguyên liệu/xưởng được làm qua cân khác; khác một mã phải mở ca mới.
- Thẻ nhóm online cũng theo ca. Backup lịch sử giữ quy tắc tương thích riêng đã chốt.
- Không thêm tải danh mục nền vào mỗi lượt cân; firmware tự nạp khi người dùng yêu cầu.
- Xóa nhân viên theo quyết định mới: chỉ hồ sơ và journal; cảnh báo ảnh hưởng tra tên/thẻ,
  không chủ động xóa phiếu cân hoặc chấm công, vẫn tuân thủ FK thực tế.

## Kiểm chứng

- Build Release `PMSHub` và 11 bộ hồi quy: HqStableSwipe, HqWeightBackup,
  ScaleSharedAttendance, ScaleAttendanceProtocol, WorkshopDeviceFilter, HqOnlineWeight,
  HqHttpBackup, ScaleOnlineWorkContext, HqWeightDuplicate, HqFunctionCards, HqAttendanceDiagnosis.
- Bổ sung case net nhỏ hơn bì; biên `decimal(18,3)`, kg/bì tràn/sai độ chính xác;
  cùng Id khác STT/TheId; projection SQL không tải cột thừa; kiểu trả Task của callback.
- Các bài SQL chỉ dùng database LocalDB tên GUID riêng và dọn sau test.
  Bài mạng chỉ dùng localhost; không gửi lệnh cân thực hoặc đọc cấu hình SQL xưởng.
- Cả 11 bộ hồi quy đạt trên bản Release cuối. Build 0 lỗi, còn 376 cảnh báo legacy
  (số lượng phụ thuộc rebuild/incremental); không coi đó là solution đã sạch warning.

### Bài tải localhost 30 cân

11 đợt x 30 kết nối = 330 request; đúng 330 ACK, 330 phiếu mới trong SQL,
330 gói SETNHANVIEN. Harness đối chiếu số phiếu và tổng context/nhân viên.
SQL LocalDB riêng; dùng index/RCSI trong fixture, không đổi SQL xưởng.

| Đợt | Hoàn tất cả 30 (ms) | Trung bình/client (ms) | P95/client (ms) | Chậm nhất/client (ms) |
| --- | ---: | ---: | ---: | ---: |
| Lượt đường xử lý đầu tiên, bảng nhỏ | 2141,53 | 2017,57 | 2127,57 | 2139,96 |
| Sau thêm 100.000 phiếu, đợt 1 | 1181,13 | 599,74 | 672,66 | 1180,37 |
| 100.000 phiếu, đợt 2 | 675,95 | 568,18 | 669,48 | 675,58 |
| 100.000 phiếu, đợt 3 | 640,74 | 555,94 | 638,97 | 640,04 |
| 100.000 phiếu, đợt 4 | 661,07 | 587,99 | 660,29 | 660,54 |
| 100.000 phiếu, đợt 5 | 647,08 | 540,77 | 646,11 | 646,98 |

P95 là tính riêng trong từng đợt 30 client, không phải P95 gộp. Các đợt bảng nhỏ
đã làm nóng hoàn tất 61,49–123,33 ms. Đây là số đo cục bộ, không có mốc before/after
để tuyên bố tăng tốc bao nhiêu phần trăm và không cam kết thời gian tương tự ở xưởng.
Tất cả fixture SQL đã dọn sau test.

## Những giới hạn còn lại / tối ưu tiếp

1. **Mất mạng và đổi Id:** server không thể phân biệt tuyệt đối một rổ gửi lại với Id mới
   và hai rổ thật cùng kg. Khi firmware không giữ Id của phiếu đang chờ, chỉ WSS không
   thể bảo đảm exactly-once qua lần mất phản hồi/chuyển Offline. Không dùng heuristic
   cùng kg để che vấn đề vì sẽ làm mất rổ thật.
2. **Luật offline 30 giây:** vẫn là chính sách cũ riêng của offline, chưa đổi trong lượt này.
   Có thể chặn hai rổ thật cùng thông tin trong khoảng đó. Cần chốt nghiệp vụ/giao thức
   trước khi thay; không âm thầm đưa luật này vào online.
3. **SQL xưởng:** tốc độ thực phụ thuộc index, RCSI, dung lượng, IO, tempdb và đồng thời báo cáo/backup.
   Các script kiểm tra chỉ đọc và index hiện có cần đối chiếu đúng server. Không đổi isolation,
   bật retry INSERT, dùng NOLOCK hoặc giảm kiểm tra ca để lấy tốc độ.
4. **Đồng hồ và lịch sử xưởng:** tiếp tục yêu cầu giờ/mã lịch sử hợp lệ; không gán xưởng hiện tại
   cho phiếu cũ thiếu lịch sử. Cần kiểm tra đồng bộ giờ và lịch sử chuyển cân khi triển khai.
5. **Legacy còn lại:** CommitService/Devices vẫn lớn; các module Cối, đặt mẫu, nhật ký khác
   còn callback cũ và SQL đồng bộ. Không refactor đại trà module chưa có fixture nghiệp vụ,
   vì thay hình thức code không tự bảo đảm dữ liệu chính xác hơn. Các cảnh báo legacy
   của solution chưa được giải quyết toàn bộ.
6. **Đo chạy dài:** test cục bộ không thay được vận hành nhiều giờ với firmware/mạng/SQL xưởng.
   Cần theo dõi log `[HQ WSS SLOW]`, `[HQ TRANSPORT UNCONFIRMED]`, lỗi SQL/tổng,
   đối chiếu số phiếu và tổng kg từng ca trước khi kết luận ổn định sản xuất.

## Triển khai

Không có migration SQL mới. Publish đầy đủ Hub sau khi kiểm thử, không chép lẻ DLL.
Giữ nguyên cấu hình SQL, DataProtection keys và App_Data/attendance-mode.json trên server.
Chưa tự publish hoặc khởi động lại Hub xưởng trong lượt này.
