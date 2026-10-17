# Backup phiếu cân HQ

## Trạng thái cân HQ trực tuyến

`LITE` và API HTTP nhận phiếu HQ yêu cầu request có `Stated="ST"` và `TrongLuong > 0`;
thiếu trạng thái hoặc cân chưa ổn định bị từ chối. Điều kiện dựa trên chính request,
không dựa vào cờ `IsStated` dùng chung trên máy. HTTP `Post` và `PostSilent` áp dụng
cùng điều kiện.

Giao thức hiện có không phân biệt bản tin `ST` do cân tự gửi lại với lần quẹt RFID mới;
`Id` và `NgayGio` của bản tin tự gửi lại có thể thay đổi. Vì vậy kiểm tra này ngăn lưu
bản tin đang báo chưa ổn định, nhưng **không bảo đảm nhân viên phải quẹt lại sau khi cân
ổn định**. Muốn thực thi quy tắc đó, firmware cần gửi mã lượt quẹt riêng: giữ nguyên mã
khi tự gửi lại một lượt cân và tạo mã mới khi đọc lần quẹt RFID mới. PMSHub hiện không
nhận mã này.

Backup lịch sử không chứa trạng thái cân tại lúc quẹt; không thể dùng trạng thái cân
hiện tại để xác nhận ngược cho phiếu quá khứ.

Kiểm thử điều kiện trạng thái: sau khi build PMSHub, chạy
`dotnet run --project tests/HqStableSwipe/HqStableSwipe.csproj`.
Kiểm thử không kết nối database hoặc gửi lệnh tới cân.

## Đồng bộ phiếu đã cân

Máy HQ (`WKv=17`) dùng `PHIEUCANADD` (một phiếu) hoặc `PHIEUCANADDS` (mảng phiếu).
Sync gửi `phieucansync`; ReSync gửi `ReSyncPhieuCan`, tham số ngày `yyyyMMdd`.
Toast gửi lệnh không xác nhận rằng dữ liệu đã được lưu. Kiểm tra phản hồi backup và SQL.

Payload nhận mã máy từ kết nối; `MayCan`, nếu gửi, phải khớp. `Id`, `IdIn` hoặc `IdOut`
phải là `máy.yyyyMMddHHmmss`; các alias cùng gửi phải giống nhau. `NgayGio` nhận
`yyyyMMddHHmmss`, `yyyy-MM-dd HH:mm:ss` hoặc `yyyy-MM-ddTHH:mm:ss`, theo giờ Việt Nam.
Thẻ lấy từ `TheId`/`TheIdNhanVien`; không chấp nhận hai thẻ khác nhau.

Phải gửi mã lô, size, thành phẩm, trọng lượng và tare, không gửi tên hiển thị thay mã.
HQ backup là phiếu sản lượng: trạng thái nguồn 0/1/2 được lưu với `Status=1`;
phiếu hủy bị từ chối, không tự hồi sinh phiếu đã hủy/sửa nội dung.
`MaLoaiNguyenLieu` và `MaXuong` nên gửi đầy đủ. Với firmware cũ thiếu hai mã này,
chỉ lấy từ một tổ hợp lịch sử duy nhất trong SQL khớp ngày/máy/lô/size/thành phẩm.
Không lấy cấu hình máy hiện tại cho phiếu quá khứ. Không có hoặc nhiều tổ hợp thì trả lỗi.

Phiếu mới phải có thẻ được gán đúng nhân viên và ca Vào hợp lệ ở đúng máy tại thời điểm
cân. Không bắt ca vẫn mở lúc gửi backup, nhưng ca lỗi/hết hạn hoặc đã Ra trước lúc cân
bị từ chối. Đồng bộ/điều chỉnh chấm công trước nếu thiếu lịch sử; không tự tạo ca Vào.

Phiếu đã có cùng Id và nội dung được ACK, không insert/update lại. Id có nội dung khác
trả lỗi. Mỗi mảng lưu trong một transaction: một phiếu lỗi thì hủy đợt, không ACK một phần.
ACK giữ dạng đối tượng/mảng tương ứng request, trả Id và Status=1 sau commit SQL.
Log `[HQ BACKUP OK]` ghi Received/Inserted/Existing. `[HQ BACKUP ERROR]` ghi lỗi cụ thể.
Độ dài truyền chấp nhận số ký tự hoặc số byte UTF-8 khớp chính xác.

Phiếu khác Id nhưng cùng dữ liệu còn lại (kể cả STT, máy, thẻ, nhân viên, xưởng,
các mã sản phẩm, trọng lượng, tare và trạng thái) phải cách nhau ít nhất 30 giây
theo `NgayGio`. Không so sánh `Id`, `Ngay`, `NgayGio` để xác định cùng nội dung;
thời gian trong ghi chú tự sinh `[HQ_BACKUP] ...` cũng không làm khác nội dung.
Ghi chú nghiệp vụ vẫn được so sánh; NULL/chuỗi rỗng ở trường tùy chọn cùng nghĩa.
Kiểm tra cả hai phía thời gian để backup gửi ngược thứ tự không lọt phiếu trùng,
kể cả qua nửa đêm và các phiếu mới trong cùng mảng chưa SaveChanges.
Một phiếu trùng làm hủy cả đợt backup như các lỗi kiểm tra khác.
Cân online, backup và API HTTP dùng chung khóa SQL theo thẻ trong lúc kiểm tra/lưu.

Kiểm thử: build `PMSHub/PMSHub.csproj`, sau đó chạy
`dotnet run --project tests/HqWeightDuplicate/HqWeightDuplicate.csproj`.
Thêm `-- --sql-localdb` trên Windows để kiểm tra SQL và yêu cầu đồng thời trong
database LocalDB tạm riêng; không đọc cấu hình/kết nối database PMS đang dùng.

Triển khai lại PMSHub; không cần migration/schema mới. Sau ReSync ngày 12/10/2026
máy 7779520, kỳ vọng 44 phiếu / 322,550 kg **nếu** đủ lịch sử ca, metadata hợp lệ
và không có phiếu trùng nội dung trong vòng 30 giây.
Không xóa dữ liệu firmware trước khi đối chiếu SQL theo Id và tổng trọng lượng.
Không tự import CSV, thay đổi giờ cân hoặc vượt qua kiểm tra ca để đạt số lượng kỳ vọng.
