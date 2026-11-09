# Sửa HTTP backup thiếu ST — 24/10/2026

## Nguyên nhân

Firmware kiểm ST khi cân offline rồi lưu SQLite. Cuối ngày `PhieuCanSync` gửi từng phiếu
qua `POST /api/data/post` với TrongLuongNhan/TrongLuongTare và các mã gốc, không có Stated.
Bản nền c2988f9 bắt buộc Stated=ST trên HTTP, nên trả 400 ngay trước INSERT.
`Đã Backup 0/2` là số phiếu firmware nhận HTTP 200 và cập nhật được trạng thái cục bộ,
không phải thông báo từ bộ xử lý Sync qua SignalR. Firmware đang kiểm tra không in response body.

## Phạm vi sửa

- Chỉ HQ HTTP `Post`/`PostSilent` dùng `HqHttpBackupWriter`; các nhánh nghiệp vụ khác giữ writer cũ.
- HTTP thiếu Stated được nhận như dữ liệu lịch sử; không tự gán ST giả. Nếu gửi Stated rõ ràng,
  chỉ nhận chuỗi ST, không nhận US/rỗng/null/giá trị khác. Đây là tin vào kiểm ổn định của
  firmware offline, server không thể chứng minh trạng thái vật lý của một rổ trong quá khứ.
- WSS online LITE vẫn yêu cầu ST; US/thiếu ST/0kg không tạo phiếu. ST/0 nhận diện nhân viên như trước.
- Kiểm tra dữ liệu trước mở SQL: máy đăng ký HQ, các mã gốc bắt buộc, Id <=100 ký tự, giờ đúng
  có tiền tố đúng mã máy, giờ yyyyMMddHHmmss và không ở tương lai, kg dương/bì không âm/decimal(18,3), STT hợp lệ.
- Nhận chuỗi số hoặc số JSON bằng invariant culture; không âm thầm đổi bì lỗi thành 0 hoặc làm tròn kg.
- TrongLuongNhan là kg net: không trừ bì lần hai và không bắt net phải lớn hơn bì.
- Giữ nguyên Id/giờ/thẻ/mã nhân viên/lô/size/thành phẩm/nguyên liệu. Không đổi Id cũ sang giờ gửi.
  HTTP chưa ép Id lịch sử phải có timestamp giống NgayGio, để không làm hỏng dữ liệu đồng hồ cũ.

## Lưu và chống ghi lại

1. Một kết nối EF/transaction ReadCommitted; khóa thẻ trước rồi khóa Id chung với WSS/Sync.
2. Đọc Id ngay trong transaction. Id đã có phải khớp nội dung request/status sản lượng;
   Id xung đột trả 400, không ACK hoặc ghi đè. Trường xưởng không được gửi thì không so với
   cấu hình hôm nay khi xác nhận lại phiếu cũ. Không tự tra lại nhân viên hiện tại để thay mã lịch sử.
3. Phiếu mới lấy xưởng từ LichSuXuongMayCan tại NgayGio gốc. Không dùng MayCan.MaXuong hôm nay.
   Thiếu mốc lịch sử hoặc xưởng gửi lên xung đột: trả 400 rõ lý do, không tự backfill.
4. Giữ nguyên chính sách offline trùng payload dưới 30 giây. Hai phiếu ảnh người dùng lúc
   15:40:59 và 15:41:46 cách nhau 47s không chặn nhau bởi luật này.
   Luật này vẫn có thể chặn hai rổ thật cùng payload dưới 30s; lần sửa ST này không thay chính sách đó.
5. INSERT/SaveChanges/COMMIT trên cùng kết nối, không thêm kết nối DAO trong khi đang giữ khóa.
6. HTTP 200 chỉ sau COMMIT hoặc đối chiếu đúng phiếu đã có. Giữ trường message/Status cũ,
   bổ sung Id/duplicated. Retry không INSERT hoặc thêm UI lần hai.
7. AddItem/Notify lỗi sau COMMIT chỉ ghi log, không biến phiếu đã lưu thành HTTP 400.
   Backup không tự đặt trạng thái ổn định hiện tại của cân thành ST.

Lỗi dữ liệu trả 400 + lý do ngắn. Lỗi SQL/hệ thống chưa xác minh kết quả trả 503 + UNKNOWN,
không trả exception SQL ra màn hình. Log server có tiền tố HQ HTTP BACKUP, Id/máy khi parse được.
`NOT_SAVED` là request bị từ chối, không khẳng định Id đó chưa từng có từ request trước.
Firmware offline giữ nguyên Id trong SQLite nên có thể thử đồng bộ lại chính phiếu gốc;
không được cấp Id mới để né lỗi. Firmware chỉ cập nhật đã đồng bộ khi HTTP 200 như trước.

## Không thay đổi

- Không sửa firmware, protocol LITE, ca Vào/Ra, thuật toán tổng online hoặc bảng SQL.
- Không chuyển HTTP offline sang ValidateOnline/GetError để xét ca hiện tại cho phiếu cuối ngày.
- Không sửa processor Sync/Resync SignalR. Cần tiếp tục kiểm thử riêng ca chung và payload của đường đó.
- Không cập nhật web báo cáo theo snapshot trong bản vá này.
- Không sửa nhánh 123 hoặc cấu hình cá nhân, không deploy/commit/push.

## Kiểm thử và triển khai

Build PMSHub Release rồi chạy:

```powershell
dotnet run --project tests/HqHttpBackup -c Release -- --sql-localdb
dotnet run --project tests/HqStableSwipe -c Release -p:HubBuildConfiguration=Release
dotnet run --project tests/HqOnlineWeight -c Release -- --sql-localdb
dotnet run --project tests/ScaleOnlineWorkContext -c Release -- --sql-localdb
```

HTTP test gọi action controller thật với dịch vụ giả và database SQL LocalDB riêng
RCQ_HttpBackupTest_GUID, không khởi động web/app sản xuất hoặc đọc cấu hình PMS_HQ.
Kiểm tra parse, HTTP status, dữ liệu thấy được từ kết nối khác sau ACK, retry/cạnh tranh,
SQL rollback, UI lỗi sau COMMIT, HTTP/online cùng Id. Không thay thế test firmware/HTTP qua IIS thật.

Không cần migration SQL cho bản vá này. LichSuXuongMayCan vẫn phải có mốc đúng tại giờ phiếu gốc.
Publish lại đầy đủ PMSHub từ RCQ-ProductMaterialWorkshop (bao gồm Services.dll mới), giữ cấu hình
server. Chưa xóa/đổi status các phiếu SQLite còn lỗi; thử lại backup ngày 22/10 sau cập nhật,
đối chiếu đúng hai Id trong SQL và 200/Backup 2/2. Nếu còn 400, xem lý do HQ HTTP BACKUP ở log;
không lấy dữ liệu cấu hình hiện tại chèn cưỡng bức vào lịch sử.

### Kết quả xác minh

- PMSHub Release build thành công: 0 lỗi, 181 cảnh báo ở lượt build xác minh cuối (còn warning mã nền).
- HqHttpBackup: **109 PASS**, gồm SQL LocalDB riêng. Hai Id trong ảnh đều lưu được khi xưởng lịch sử
  hợp lệ; 8 request cùng Id chỉ 1 INSERT; khác thẻ tranh Id và HTTP/online tranh Id được khóa đúng;
  lỗi UI sau COMMIT không trả 400; lỗi ghi SQL không ACK và rollback.
- HqStableSwipe: **255 PASS** sau cập nhật test HTTP thiếu ST được phép; ranh giới online vẫn giữ.
- HqOnlineWeight: **98 PASS**, ScaleOnlineWorkContext: **66 PASS**, có SQL LocalDB riêng.
- HqWeightBackup: **175 PASS** (in-memory), HqWeightDuplicate: **42 PASS**,
  ScaleSharedAttendance: **59 PASS**, ScaleAttendanceProtocol: PASS.
- Không truy cập PMS_HQ thật, chưa deploy hoặc thử firmware vật lý. Không coi số test là benchmark xưởng.
