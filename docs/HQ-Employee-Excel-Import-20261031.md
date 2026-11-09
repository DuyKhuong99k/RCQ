# Hub: thêm nhân viên từ Excel

Áp dụng tại `RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`.
Không thay đổi Dev_Khuong, VCC, firmware, luồng cân hoặc chấm công.

## Cách dùng

Nhân viên → Nhập từ Excel → Tải file Excel mẫu hoặc chọn file có 3 tiêu đề ở dòng đầu:
Mã nhân viên, Mã hồ sơ, Tên nhân viên. Đặt mã là Text để giữ số 0 đầu.
Chỉ đọc trang tính đầu; không phụ thuộc thứ tự 3 cột. Tiêu đề không dấu cũng được chấp nhận.
Tối đa 1.000 nhân viên / file .xlsx 5 MB. Ô công thức/ô lỗi bị từ chối.
Bấm Kiểm tra danh sách, xem kết quả từng dòng rồi bấm Nhập.
Bảng xem trước có bộ lọc từng cột kiểu Excel, sắp xếp và chọn 20/50/100 dòng mỗi trang (mặc định 20).
Lọc/phân trang chỉ đổi phần hiển thị, không thay đổi toàn bộ danh sách đã kiểm tra dùng khi bấm Nhập.

File mẫu trống, không có dữ liệu nhân viên giả. Phần mở rộng .xls/.csv chưa hỗ trợ.
Ngày 2026-11-01: đổi mẫu tải xuống theo bố cục DS công nhân mới2.xlsx người dùng cung cấp:
3 cột, Times New Roman 14, tiêu đề in đậm căn giữa, nền trắng và kẻ khung tới dòng 30.
Các ô nhập mã đặt Text cho 1.000 dòng để giữ số 0 đầu; không mang theo hình/add-in trong file tham chiếu.
Chỉ thay tài nguyên mẫu tải xuống, không sửa bộ đọc Excel, luồng lưu SQL hoặc dữ liệu nhân viên.
Workbook giải nén vượt 25 MB hoặc 1.000 thành phần bị từ chối trước khi phân tích.

## Quy tắc dữ liệu

- Tái sử dụng quy tắc nhập 3 cột của báo cáo Dev_Khuong, không sao chép controller/API/cấu hình của báo cáo.
- Mọi tài khoản đang hoạt động đã đăng nhập được nhập vào đúng xưởng đang chọn, như CRUD nhân viên hiện tại.
- Xưởng do server lấy từ phiên đăng nhập. Đổi xưởng sau xem trước yêu cầu kiểm tra lại.
- Mã nhân viên tồn tại ở bất kỳ xưởng nào: bỏ qua, không ghi đè hoặc chuyển xưởng.
- Trùng mã nhân viên/mã hồ sơ trong file, thiếu dữ liệu, quá độ dài hoặc mã hồ sơ đã dùng: báo lỗi. Có lỗi thì không ghi dòng nào.
- Tối đa mã 50 ký tự, tên 100 ký tự. Giữ Unicode và số 0 đã được Excel lưu, không thể khôi phục số 0 Excel đã làm mất.
- Mặc định như báo cáo: đang làm, nhân viên bình thường, thông báo định mức, loại sản lượng 1; chưa gán thẻ/nhóm.
- Xem trước không ghi SQL. Khi lưu kiểm tra lại SQL trong giao dịch Serializable, một SaveChanges cho cả lô.
- Không tự thử ghi lại khi SQL lỗi/timeout. Người dùng kiểm tra lại; các mã đã lưu được bỏ qua ở lượt tiếp theo.
- SQL đã commit nhưng cache lỗi: vẫn báo Đã thêm, kèm cảnh báo tải danh sách. Không báo sai là chưa lưu.
- Làm mới catalog Hub một lần sau commit. Trên cân bấm Nạp nhân viên/Nạp thẻ khi cần, không tự gửi lệnh xuống cân.

## Triển khai / kiểm thử

Publish lại PMSHub, gồm thư viện ClosedXML 0.104.2 và các phụ thuộc, cùng wwwroot/templates/NhanVienImportTemplate.xlsx.
Không cần tạo bảng, đổi schema hoặc chạy script SQL mới. Giữ cấu hình kết nối server đang sử dụng.
Không chỉ chép riêng PMSHub.dll vì Services/ViewModels cũng có phần nhập mới.

Build Release 0 lỗi. Bộ HqAttendanceDiagnosis đạt 282 assertions trên LocalDB GUID riêng, đã xóa fixture:
xem trước không ghi, mặc định/Unicode/số 0, trùng mã toàn xưởng, lỗi dữ liệu, đổi phiên/xưởng,
kiểm tra lại sau preview, SQL rollback toàn lô, nhập đồng thời, cache lỗi sau commit và lô 1.000 dòng.
Kiểm thử UI/reader nằm trong WorkshopDeviceFilter: toolbar, điều kiện cho phép nhập, reset preview,
đọc file/tiêu đề/giới hạn/formula/duplicate, file mẫu và dữ liệu giữ số 0 đầu.
Lượt chạy cuối đạt 209 checks, có đối chiếu file mẫu 32 nhân viên người dùng gửi ở chat Dev_Khuong;
chỉ đọc/kiểm tra file, không nhập nhân viên thật vào SQL.
Chưa đo trên SQL xưởng hoặc thao tác trực tiếp bằng cân thật.
