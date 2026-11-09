# Đo 30 cân gửi đồng thời — 25/10/2026

## Cập nhật mới nhất: tách UI và giới hạn chờ gửi firmware

Sau hai tối ưu mô tả trong `HQ-UI-Firmware-Send-20261025.md`, đã chạy lại hai tiến trình độc lập: **660/660 phản hồi, 660 phiếu SQL, 660 SETNHANVIEN**, không timeout/lỗi ở các burst đã đo. Lần 2 còn kiểm tra mỗi `ReceiveData` phải có đúng một `SETNHANVIEN` đến trước nó. Toàn bộ bài hồi quy riêng đạt 627 assertions.

Giữ cùng phạm vi loopback, máy test i7-10750H/32 GB, LocalDB/RCSI/index như phần cấu hình bên dưới; không có browser/firmware thật hoặc SQL xưởng. Thời gian là cả đợt 30 cân:

| Lần | Đợt | Cả đợt (ms) | P95 client (ms) |
| --- | --- | ---: | ---: |
| 1 | Đầu tiên | 1740,35 | 1729,59 |
| 1 | Nóng ít dữ liệu 1 | 61,20 | 60,76 |
| 1 | Nóng ít dữ liệu 2 | 54,63 | 50,91 |
| 1 | Nóng ít dữ liệu 3 | 53,73 | 51,95 |
| 1 | Nóng ít dữ liệu 4 | 57,54 | 56,15 |
| 1 | Nóng ít dữ liệu 5 | 52,33 | 51,43 |
| 1 | 100k dòng 1 | 608,56 | 606,73 |
| 1 | 100k dòng 2 | 598,96 | 596,64 |
| 1 | 100k dòng 3 | 637,32 | 636,62 |
| 1 | 100k dòng 4 | 591,37 | 589,15 |
| 1 | 100k dòng 5 | 609,85 | 609,21 |
| 2 | Đầu tiên | 1839,82 | 1829,02 |
| 2 | Nóng ít dữ liệu 1 | 60,64 | 60,10 |
| 2 | Nóng ít dữ liệu 2 | 51,81 | 50,95 |
| 2 | Nóng ít dữ liệu 3 | 50,52 | 49,78 |
| 2 | Nóng ít dữ liệu 4 | 68,53 | 66,23 |
| 2 | Nóng ít dữ liệu 5 | 47,61 | 47,10 |
| 2 | 100k dòng 1 | 728,43 | 723,48 |
| 2 | 100k dòng 2 | 647,43 | 646,86 |
| 2 | 100k dòng 3 | 657,70 | 655,70 |
| 2 | 100k dòng 4 | 629,25 | 628,42 |
| 2 | 100k dòng 5 | 640,85 | 638,98 |

Tóm tắt: lần đầu 1,740–1,840 giây; nóng ít dữ liệu 47,61–68,53 ms; có 100k dòng 591,37–728,43 ms. Không suy ra cam kết độ trễ ở xưởng hoặc mức tăng tốc cố định từ phép đo khác nhau. Cả hai database GUID đã được xóa sau test. Các bảng cũ bên dưới giữ lại làm lịch sử.

> Các bảng ban đầu bên dưới là trước sửa đồng bộ Set/Vào. Sau sửa đã đo lại hai lần, 660/660 thành công: đợt đầu 1,730–2,016 giây; nóng ít dữ liệu 50,78–65,46 ms; 100k dòng 620,71–1.198,65 ms, có một client chậm khoảng 1,20 giây. Xem `HQ-Selection-Startup-Fix-20261025.md` để có số đo sau sửa và giới hạn phép đo.

## Kết quả

Đã chạy **2 tiến trình độc lập**, mỗi tiến trình 11 đợt × 30 kết nối SignalR WebSocket thật. Tổng **660/660 phản hồi thành công, 660 phiếu mới trong SQL và 660 SETNHANVIEN**. Không có lỗi/timeout trong các đợt đã đo.

Thời gian hoàn tất **cả đợt 30 cân**, từ lúc nhả hàng rào đồng thời đến lúc client cuối nhận `ReceiveData`:

| Trường hợp | Khoảng đo của hai lần chạy |
| --- | ---: |
| Đợt LITE đầu tiên của tiến trình | 1,786–2,094 giây |
| Luồng đã nóng, ít dữ liệu | 44,75–64,75 ms |
| Luồng đã nóng, thêm 100.000 phiếu cùng nhóm tính tổng | 635,11–763,84 ms |

Đây **không phải** thời gian mỗi phiếu nhân với 30: các máy khác nhau được xử lý song song. `BatchMs` bao gồm độ lệch lúc bắt đầu gửi của 30 client (tối đa 3,26 ms ở bài này). P95 bên dưới được tính riêng trong mỗi đợt 30 phiếu, không phải P95 gộp toàn bộ bài.

## Cấu hình và phạm vi

- Máy đo: Intel Core i7-10750H, 6 nhân/12 luồng, RAM khoảng 32 GB. Không phải máy xưởng i5/RAM 8 GB.
- .NET 8.0.31; DLL Release hiện có của `RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`. **Không đo bản release nhánh 123.**
- SQL LocalDB Express 64-bit, `ProductVersion=17.0.4025.3`; RCSI bật, các index của `20261018-HqOnlineWorkContext.sql` áp dụng trong DB test.
- 30 kết nối đã sẵn sàng, mỗi cân một thẻ/nhân viên riêng, tất cả cùng thành phẩm LM, nguyên liệu XOAIDONG và xưởng 1. Nhân viên thường, không phải thẻ nhóm; ca mở có snapshot để chạy kiểm tra chấm công thật.
- Mỗi request: `SendData("LITE", len, json)`, ST, 1,400 kg, tare 0,710 kg, Id riêng. Chỉ một phiếu đang chờ trên mỗi cân, đúng cơ chế firmware người dùng mô tả.
- Đường chạy thật: `ChatHub.SendData → CommunicationService.Route → CommitService.CommitLite → SaveAsync → SQL COMMIT → tính đủ tổng → SETNHANVIEN → ReceiveData`.
- Đo từ client ngay trước gửi đến callback client nhận phản hồi, không chỉ đo INSERT. Kiểm tra SaveStatus=SAVED, Id, tổng rổ/kg cá nhân và tổng nhóm; sau mỗi đợt đối chiếu đủ 30 Id trong SQL. Tổng nhóm phản ánh snapshot lúc truy vấn, không bắt buộc mỗi phản hồi đều thấy đủ cả 30 phiếu đồng thời.
- Cache thẻ/nhân viên/máy được nạp giả lập sẵn; registry đăng nhập được đăng ký sẵn. Gửi SETNHANVIEN qua SignalR thật; phần thông báo giao diện/cache máy được thay bằng test double, không có trình duyệt theo dõi.
- Không chạy toàn bộ PMSHub startup/background jobs. Không đo đăng nhập, kết nối mới, TLS/WSS mã hóa, IIS, mạng LAN/Wi-Fi, độ trễ nhúng/HMI hoặc tải báo cáo/backup chạy song song. Server dùng WS loopback `127.0.0.1`.
- Đợt đầu bao gồm lần đầu thực thi luồng LITE/JIT/query, nhưng không bao gồm thời gian dựng server/DB/kết nối; không phải cold boot của máy/SQL.
- 100.000 dòng được tạo tổng hợp cùng ngày/ngữ cảnh, chia đều cho 30 người, để gây tải phép cộng tổng. Đây không phải dữ liệu/định dạng Id lịch sử firmware thực tế.

## Số đo chi tiết

Đơn vị ms; mỗi dòng đều **30/30 phản hồi và 30/30 phiếu SQL**.

| Lần | Đợt | Cả đợt | TB mỗi client | P95 client | Chậm nhất client |
| --- | --- | ---: | ---: | ---: | ---: |
| 1 | Đầu tiên | 2093,78 | 2072,68 | 2075,55 | 2078,29 |
| 1 | Nóng, ít dữ liệu 1 | 62,46 | 55,33 | 61,07 | 62,19 |
| 1 | Nóng, ít dữ liệu 2 | 57,22 | 50,50 | 55,05 | 56,89 |
| 1 | Nóng, ít dữ liệu 3 | 50,15 | 43,70 | 48,58 | 49,31 |
| 1 | Nóng, ít dữ liệu 4 | 48,61 | 42,92 | 47,91 | 48,38 |
| 1 | Nóng, ít dữ liệu 5 | 44,75 | 39,74 | 43,37 | 43,53 |
| 1 | 100k dòng 1 | 643,28 | 549,49 | 641,90 | 642,36 |
| 1 | 100k dòng 2 | 635,11 | 541,23 | 632,88 | 634,95 |
| 1 | 100k dòng 3 | 680,67 | 576,50 | 679,53 | 680,45 |
| 1 | 100k dòng 4 | 650,70 | 556,04 | 648,85 | 649,00 |
| 1 | 100k dòng 5 | 686,86 | 588,42 | 686,49 | 686,64 |
| 2 | Đầu tiên | 1786,09 | 1765,58 | 1772,52 | 1782,71 |
| 2 | Nóng, ít dữ liệu 1 | 62,29 | 50,18 | 59,29 | 60,62 |
| 2 | Nóng, ít dữ liệu 2 | 53,10 | 44,53 | 52,26 | 52,95 |
| 2 | Nóng, ít dữ liệu 3 | 47,99 | 41,57 | 46,47 | 46,79 |
| 2 | Nóng, ít dữ liệu 4 | 49,75 | 42,16 | 48,68 | 49,48 |
| 2 | Nóng, ít dữ liệu 5 | 64,75 | 59,07 | 63,72 | 64,04 |
| 2 | 100k dòng 1 | 763,84 | 635,77 | 759,44 | 763,52 |
| 2 | 100k dòng 2 | 726,13 | 605,45 | 723,79 | 725,74 |
| 2 | 100k dòng 3 | 709,82 | 632,09 | 705,04 | 708,13 |
| 2 | 100k dòng 4 | 691,04 | 589,29 | 683,20 | 690,79 |
| 2 | 100k dòng 5 | 729,13 | 652,82 | 726,29 | 728,84 |

## Diễn giải

Bài này cho thấy luồng mới xử lý được 30 máy đồng thời trong điều kiện đã nêu, và chi phí tăng rõ khi phải cộng nhiều phiếu cùng ngữ cảnh. Không thể từ đó cam kết server xưởng luôn dưới một giây, hoặc suy ra thông lượng duy trì cả ngày. Chưa đo trường hợp nhiều cân cùng một thẻ (khóa thẻ sẽ tuần tự hóa), tải backup/báo cáo hỗn hợp, mạng mất/chập chờn, lỗi SQL, hoặc SQL chứa hàng triệu phiếu của xưởng.

Muốn có con số thật ở xưởng cần phép đo được cho phép trên cấu hình/dữ liệu đại diện ở server đó; bài này không tự gửi phiếu test vào SQL nhà máy.

## Chạy lại an toàn

Từ `D:\RCQ\RCQ-ProductMaterialWorkshop`:

```powershell
dotnet build PMSHub/PMSHub.csproj --no-restore -c Release -m:1 -nr:false -p:NuGetAudit=false -p:UseSharedCompilation=false
dotnet run --project tests/HqWssLoad/HqWssLoad.csproj -c Release -p:NuGetAudit=false -p:UseSharedCompilation=false --no-launch-profile -- --sql-localdb
```

Harness dùng DLL Release tại `PMSHub/bin/Release/net8.0`, nên cần build trước nếu sửa production code. Không đọc file cấu hình kết nối xưởng. Mỗi lần tạo DB `RCQ_WssLoadTest_<GUID>` riêng trên LocalDB, áp index/RCSI chỉ trong DB đó rồi xóa DB test ở `finally`. Cả hai lần đo đã báo cleanup thành công. Không thay đổi code chạy sản xuất, cấu hình, firmware hoặc dữ liệu xưởng trong lần đo này.
