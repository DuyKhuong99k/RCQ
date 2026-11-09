# Quản lý nhân viên trên Hub — 27/10/2026

## Vị trí và phạm vi

- Mã Hub: `D:\RCQ\RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`.
- Đối chiếu form và journal với `D:\RCQ\RCQ`, PMS ở nhánh `Dev_Khuong`: `PMS/Controllers/DanhMuc/StaffController.cs`, `PMS/Views/DanhMuc/Staff/NhanVienView.cshtml`, `WebAPI/Controllers/NhanVienDaiThanhsController.cs`.
- Không checkout/merge nhánh, sửa cấu hình conflict hoặc chỉnh PMS Báo Cáo.
- Hub có menu **Nhân Viên** (`/NhanVien`) và liên kết từ trang **Thẻ Nhân Viên**. Hỗ trợ Thêm, Sửa, Xóa từng hồ sơ và tìm kiếm/phân trang.
- Thanh công cụ: **Thêm nhân viên / Sửa / Xóa / Tải lại**. Chọn một dòng trong bảng để Sửa/Xóa;
  chưa chọn hoặc đang xử lý thì khóa nút. Xóa vẫn phải xác nhận. Bỏ cột thao tác từng dòng để
  không phải kéo ngang tới nút. Tìm kiếm, đổi trang/sắp xếp, tải lại và lưu thành công đều bỏ
  lựa chọn cũ; DataSource được giữ ổn định qua render. Các cờ/cột/luồng SQL của RCQ không đổi.
- Bảng hiển thị đầy đủ cột tích: Đang làm, Phục vụ, Bàn kiểm, Chức năng, Nhân viên BT, Thẻ nhóm, Gia công như Báo Cáo; thêm Nhân viên cắt và Thông báo định mức tương ứng form Hub. Cột Tên nhóm (`DeptName0`) tách khỏi cờ Thẻ nhóm. Các ô tích chỉ hiển thị trạng thái; thay đổi bằng nút Sửa để đi qua kiểm tra/lưu hiện có. Giữ độ rộng cột và cuộn ngang trong khung bảng khi không đủ chỗ; không thêm truy vấn SQL hoặc schema.
- Theo lựa chọn của người dùng: mọi tài khoản đăng nhập còn hoạt động được thao tác, nhưng chỉ trong xưởng đang chọn. Đây là quyền của chức năng Hub mới, không sửa quyền `CustomAuthorize` của PMS Báo Cáo.

## Dữ liệu và an toàn

- Ghi vào `NhanVienDaiThanh`. Không tự tạo/gán `TheTu`; sau khi lưu hồ sơ phải dùng trang Thẻ Nhân Viên để gán UID.
- Các trường giống form Báo Cáo: mã nhân viên, mã hồ sơ, họ tên, mã chấm công, nhóm (`DeptName0`), loại sản lượng; cờ đang làm, nhân viên thường, phục vụ, gia công, bàn kiểm, chức năng, nhóm, thông báo định mức. Có thêm cờ nhân viên cắt đang có trong model.
- Mặc định: đang làm/nhân viên thường/thông báo định mức bật, loại sản lượng 1. Các cờ nhóm và nhân viên thường độc lập, cho phép cùng bật. Không đổi logic chấm công/cân theo các cờ này.
- Mã hồ sơ và họ tên bắt buộc; mã chấm công có thể để trống theo nhánh Insert của Báo Cáo. Giới hạn độ dài kiểm tra cả phía server. Loại sản lượng chỉ -1, 0, 1.
- Gợi ý mã số toàn database (ít nhất 6 chữ số), không đặt chỗ mã. SQL vẫn kiểm tra trùng để xử lý hai người tạo cùng mã. Không cho đổi khóa mã nhân viên khi Sửa.
- Xưởng được đọc từ phiên đăng nhập và kiểm tra lại cùng trạng thái tài khoản trong SQL trên mỗi thao tác; không nhận xưởng từ form. Hồ sơ khác xưởng hoặc chưa gán xưởng không xuất hiện/không được sửa qua chức năng này.
- Sửa chỉ gán các trường trên form lên entity hiện có; giữ nguyên các trường khác (ngày sinh, địa chỉ, điện thoại...) và không chạm cột ngoài model như `IsThuKho`.
- `MNgay` lấy giờ Việt Nam phía server; khi Sửa phải tăng so với phiên bản trước, kể cả đồng hồ có độ phân giải thấp/lùi giờ. Dùng mốc này cùng khóa dòng SQL để chặn ghi đè/xóa hồ sơ đã bị người khác sửa. Không thêm cột rowversion.
- Sửa: ghi `HQ_NhanVien_U` cùng transaction với hồ sơ. Xóa: ghi `HQ_NhanVien_D` cùng transaction. Không có hai lần Save độc lập khiến thiếu journal.
- Cập nhật 28/10/2026 theo yêu cầu đồng nhất với Báo Cáo: Xóa có bước xác nhận tên/mã và cảnh báo ảnh hưởng. Không còn tự chặn hồ sơ chỉ vì có tài khoản, thẻ, phiếu cân hoặc chấm công. Chỉ xóa dòng `NhanVienDaiThanh` và ghi snapshot `HQ_NhanVien_D` trong cùng transaction; không phát lệnh sửa/xóa thẻ, tài khoản, phiếu cân, ca hoặc lịch sử. Giữ kiểm tra quyền/xưởng, mốc `MNgay` và khóa dòng. Nếu SQL có FK/ràng buộc ngăn xóa, báo rõ và rollback; không tự bỏ FK hay xóa dữ liệu liên quan.
- Xóa hồ sơ có thể khiến thẻ còn gán mã đó không tra được nhân viên để cân tiếp; báo cáo đang JOIN danh mục hiện tại có thể không hiện tên/không hiện dòng đó dù dữ liệu phiếu cân gốc còn nguyên. API nhật ký xóa dùng để firmware nạp xóa nhân viên như Báo Cáo; không tự gửi lệnh xuống cân. Nếu chỉ nghỉ việc, nên sửa trạng thái thay vì xóa. Không bảo đảm hành vi các trigger/FK tùy chỉnh trên server chưa được kiểm tra; không sửa schema SQL xưởng trong lượt này.
- Cờ trạng thái làm việc là cờ danh mục hiện có; không bổ sung cơ chế khóa quẹt firmware/WSS mới trong lượt này.
- Thao tác dùng ReadCommitted, tận dụng RCSI nếu SQL xưởng đã bật. Không còn quét các bảng lịch sử để quyết định có được xóa hồ sơ không. Khóa ghi/concurrency chỉ áp dụng hồ sơ đang sửa/xóa; vẫn tuân thủ FK SQL thực tế.

## Cập nhật Hub và cân

- Sau commit SQL, gọi `IHubEmployeeCardCatalog.RefreshAsync` để dropdown nhân viên/thẻ và API nạp trên cân dùng dữ liệu mới. Chờ đọc đủ hai bảng rồi mới thay cache, không làm rỗng danh sách đang phục vụ cân.
- Nếu commit thành công nhưng tải cache lỗi: vẫn báo **Đã thêm/sửa/xóa**, kèm cảnh báo tải lại. Không tự gửi lại lệnh ghi SQL.
- Không thêm SQL vào từng lượt cân, không thêm cập nhật nền hay tự gửi lệnh xuống cân. Khi cần trên cân: Nạp nhân viên; nếu gán thẻ mới thì Nạp thẻ nhân viên. Sửa/xóa dùng các API journal U/D hiện có, giữ JSON firmware cũ.

## Bảng cần có trên server

Không có schema mới. Chức năng cần những bảng đang dùng bởi Báo Cáo/Hub:
`NhanVienDaiThanh`, `XiNghiep`, `NguoiDung`, `TheTu`, `HQ_NhanVien_U`, `HQ_NhanVien_D`.
Nếu thiếu bảng journal, thao tác tương ứng phải thất bại/rollback, không tự tạo bảng hoặc bỏ qua ghi đồng bộ.

## Kiểm thử và triển khai

- `tests/HqAttendanceDiagnosis/HubEmployeeManagerChecks.cs`: SQL thật trong database LocalDB GUID riêng; tài khoản thường, kiểm tra xưởng/tài khoản, validation, tạo/sửa/xóa, metadata không có trên form, IsThuKho ngoài model, journal, lỗi transaction, xung đột cập nhật, mã trùng. Xóa hồ sơ có tài khoản/thẻ/phiếu cân/chấm công được phép như Báo Cáo, so sánh dữ liệu liên quan trước/sau giữ nguyên. Có test FK thật chặn xóa, journal lỗi phải rollback, hai yêu cầu Xóa đồng thời chỉ một journal và cache lỗi sau commit phải báo đúng trạng thái.
- `tests/WorkshopDeviceFilter/HubEmployeeUiChecks.cs`: form sửa dùng bản sao, Hủy không ghi, xác nhận xóa, khóa thao tác khi bận, gợi ý mã, tìm kiếm, điều hướng và validation.
- Không truy cập SQL xưởng hoặc cân thật trong test. Chưa commit/push/publish.
- Rebuild/publish đầy đủ project Hub mới. Không chép lẻ DLL vào release cũ. Sau deploy kiểm tra bằng hồ sơ test chưa dùng trong đúng xưởng trước khi sử dụng thực tế.
