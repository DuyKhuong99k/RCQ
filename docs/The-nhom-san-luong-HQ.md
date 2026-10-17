# Thẻ nhóm ghi sản lượng HQ

- Nhận diện theo `TheTu.MaNhanVien` liên kết `NhanVienDaiThanh.MaNhanVien`, dùng cờ `IsNhom` trong SQL, không suy ra từ tên hoặc mã gửi lên.
- Nhóm `IsNhom = 1` được ghi phiếu sản lượng không cần chấm Vào/Ra; áp dụng cả nhóm `IsHuman = 0` và nhóm `IsHuman = 1`.
- Vẫn từ chối thẻ chức năng (`IsChucNang = 1`) và phục vụ (`IsPhucVu = 1`).
- Nhân viên thường (`IsNhom = 0, IsHuman = 1`) vẫn cần ca Vào hợp lệ tại cân. Đối tượng không phải người và không phải nhóm vẫn bị chặn.
- Áp dụng luồng cân trực tiếp và backup Sync/Resync HQ. Không tự tạo ca chấm công hoặc giờ làm cho nhóm; không thay đổi dữ liệu cũ hay định dạng firmware.
- Giữ kiểm tra phiếu trùng, mã thẻ/nhân viên, trọng lượng, lô/size/thành phẩm/nguyên liệu, xưởng lịch sử và tính nguyên tử của đợt backup.

## Kiểm tra sau triển khai

1. Nâng cấp tất cả Hub phục vụ cân HQ liên quan. Nếu mới sửa danh mục thẻ/nhân viên, nạp lại danh mục hoặc khởi động lại Hub theo quy trình vận hành để bộ nhớ nhận cờ mới.
2. Thẻ nhóm MAYCAT2 (`IsNhom=1, IsHuman=0`), không có Vào/Ra: cân ổn định với đủ cấu hình phải lưu phiếu HQ dưới đúng mã nhóm.
3. Nhóm `IsHuman=1` cũng không cần Vào/Ra.
4. Nhân viên thường chưa Vào: vẫn báo chưa quẹt Vào; sau Vào hợp lệ tại cân mới được ghi sản lượng.
5. Gửi lại cùng phiếu không tăng sản lượng; backup cùng Id nhưng khác nội dung phải bị từ chối theo kiểm tra hiện có.
6. Backup nhóm không có chấm công được lưu nếu các kiểm tra khác hợp lệ; backup nhân viên thường thiếu ca tại giờ cân vẫn bị chặn.

Kiểm thử mã không thay thế thử trên cân thật. Không cần chạy câu UPDATE hàng loạt để biến nhóm thành nhân viên.
