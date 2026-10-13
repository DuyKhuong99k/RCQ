# Backup phiếu cân HQ

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

Triển khai lại PMSHub; không cần migration/schema mới. Sau ReSync ngày 12/10/2026
máy 7779520, kỳ vọng 44 phiếu / 322,550 kg **nếu** đủ lịch sử ca và metadata hợp lệ.
Không xóa dữ liệu firmware trước khi đối chiếu SQL theo Id và tổng trọng lượng.
Không tự import CSV, thay đổi giờ cân hoặc vượt qua kiểm tra ca để đạt số lượng kỳ vọng.
