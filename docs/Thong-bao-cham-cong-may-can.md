# Thông báo chấm công gửi từ PMSHub xuống máy cân

Đối chiếu mã nguồn ngày 15/10/2026. Đây là thông báo Hub gửi, không khẳng định firmware hiển thị nguyên văn hoặc đầy đủ trên màn hình. Không bao gồm thông báo do firmware tự tạo khi offline. Không thay đổi nội dung giao thức.

## 1. Khi quẹt chấm công Vào/Ra (CHAMCONG)

| Thông báo nguyên văn | Ý nghĩa / hướng dẫn |
| --- | --- |
| Đã chấm công Vào thành công. | Đã ghi nhận Vào; có thể bắt đầu làm tại cân. |
| Đã chấm công Ra thành công. | Đã ghi nhận Ra; kết thúc lượt làm tại cân. |
| Đã chấm công Vào ca đêm thành công. | Đã ghi nhận Vào ca đêm. |
| Đã chấm công Ra ca đêm thành công. | Đã ghi nhận Ra ca đêm. |
| Đã quẹt Vào rồi. | Lượt Vào đã được ghi nhận; không cần quẹt lặp. Không tạo giờ Vào mới. |
| Đã quẹt Ra rồi. | Lượt Ra đã được ghi nhận; không cần quẹt lặp. Không tạo giờ Ra mới. |
| Đã quẹt Vào ca đêm rồi. | Vào ca đêm đã ghi nhận. |
| Đã quẹt Ra ca đêm rồi. | Ra ca đêm đã ghi nhận. |
| Thẻ chưa đăng ký | Nhờ quản lý kiểm tra đăng ký thẻ và nhân viên. |
| Chưa Vào. Quẹt Vào trước. | Chưa thể ghi Ra; quẹt Vào để bắt đầu ca nếu đang bắt đầu làm. Nếu thực tế đã làm nhưng thiếu Vào, báo quản lý điều chỉnh. |
| Chưa có ca Vào đang mở. Vui lòng quẹt Vào trước. | Không tìm được lượt Vào để ghép Ra; báo quản lý nếu là dữ liệu thiếu. |
| Đã chấm Vào, chưa chấm Ra. | Kiểm tra trạng thái lượt Vào trước khi thao tác tiếp. Thông báo này có trong nhánh kiểm tra chuyển trạng thái; quẹt Vào lặp thông thường dùng “Đã quẹt Vào rồi.”. |
| Đang ở cân {mã máy}. Ra cân cũ. | Còn ca tại cân khác: quẹt Ra ở cân cũ hoặc nhờ người có quyền xác nhận chuyển máy. |
| Trùng ca nhiều cân. Báo quản lý. | Dữ liệu có nhiều ca chưa Ra đồng thời; không tự quẹt thêm để xử lý. |
| Ca đã hết hạn (ca thường chốt cuối ngày, tối đa {số giờ} giờ). Cần bổ sung giờ Ra; quẹt Vào để bắt đầu ca mới. | Ca đã quá hạn. Nhờ quản lý bổ sung giờ Ra thực tế; Vào mới nếu bắt đầu lượt làm mới. Không tự ghi giờ Ra cho ca cũ. Số giờ lấy từ cấu hình, mặc định 16. |
| Đã quẹt Vào ca mới. Ca trước thiếu giờ Ra, cần bổ sung. | Ca Vào mới đã ghi nhận; quản lý vẫn cần kiểm tra ca trước. |
| Đã quẹt Vào ca đêm mới. Ca trước thiếu giờ Ra, cần bổ sung. | Ca đêm mới đã ghi nhận; quản lý kiểm tra ca trước. |
| Lượt quẹt cũ hơn lịch sử. Kiểm tra giờ thiết bị. | Kiểm tra ngày giờ cân; báo kỹ thuật, không quẹt lặp liên tục. |
| Lượt quẹt cũ hơn ca đang mở. Kiểm tra giờ thiết bị. | Giờ gửi lên trước giờ Vào đang lưu; báo kỹ thuật. |
| Không thể xử lý chấm công. Vui lòng thử lại. | Lỗi xử lý phía Hub; thử lại một lần, nếu còn lỗi báo quản lý/kỹ thuật. |

Lưu ý: ca thường các ngày cũ không dùng để chặn ca hôm nay; ca đêm còn hợp lệ có thể được giữ qua nửa đêm. Cảnh báo ca trước thiếu Ra không xuất hiện với mọi ca cũ mà phụ thuộc nhánh xử lý. Không hướng dẫn người dùng quẹt Ra giả để bù dữ liệu lịch sử.

## 2. Khi cân sản lượng nhưng điều kiện chấm công chưa hợp lệ

Các thông báo dưới đây thuộc kiểm tra phiếu cân online, không phải phản hồi thành công khi quẹt Vào/Ra.

| Thông báo nguyên văn | Hướng dẫn |
| --- | --- |
| Chưa quẹt Vào tại cân. | Quẹt Vào tại cân này trước khi cân sản lượng. |
| Quẹt Vào lại để tiếp tục cân. | Lượt trước đã Ra; quẹt Vào cho lượt làm mới. |
| Ca đã hết hạn. Quẹt Vào lại tại cân. | Quẹt Vào để mở lượt làm mới; quản lý kiểm tra ca thiếu Ra. |
| Đang ở cân {mã máy}. Ra cân cũ. | Ra ở cân cũ hoặc nhờ xác nhận chuyển máy. |
| Trùng ca nhiều cân. Báo quản lý. | Nhờ quản lý xử lý dữ liệu ca trùng. |
| Ca đang lỗi. Nhờ quản lý kiểm tra. | Ca hiện tại đang có cờ lỗi; nhờ quản lý kiểm tra. |
| Giờ cân trước lượt chấm công. Kiểm tra giờ máy. | Kiểm tra ngày giờ cân; báo kỹ thuật. |

## 3. Thông báo kỹ thuật để quản lý / người hỗ trợ tra cứu

| Nội dung gửi xuống | Nguyên nhân cần kiểm tra |
| --- | --- |
| Giới hạn ca chấm công phải từ 1 đến 24 giờ. | Cấu hình Hub sai. |
| JSON chấm công không hợp lệ. | Gói dữ liệu chấm công sai định dạng. |
| Thiếu hoặc sai mã thẻ. | Mã thẻ trống hoặc quá dài. |
| NgayGio phải có định dạng yyyyMMddHHmmss. | Dữ liệu ngày giờ firmware gửi không hợp lệ. |
| Kết nối chưa xác định được mã máy cân. | Hub chưa xác định được mã máy phù hợp. |
| ChamCongId không khớp máy đang kết nối hoặc NgayGio. | Id lượt quẹt không khớp máy/ngày giờ. |
| TrangThai chỉ được là Vào hoặc Ra. | Giá trị trạng thái gửi lên không hợp lệ. |
| Tên máy vượt quá độ dài cho phép của CheckInOut.TenMay. | Tên máy quá dài. |
| Id đã được dùng cho lượt quẹt khác. Giữ nguyên bản ghi đã lưu. | Trùng Id với dữ liệu khác; Hub không ghi đè. |
| Id đã được dùng cho lượt quẹt khác. | Trùng Id trong nhánh ghi đồng thời; Hub không ghi đè. |
| Dữ Liệu Không Toàn Vẹn | Độ dài gói tin không khớp. |
| Client Null | Không tìm thấy thông tin client trong router. |
| Client chưa active | Client chưa được kích hoạt. |

## 4. Vị trí nội dung trong phản hồi WSS

Lệnh `CHAMCONG` được trả qua `ReceiveData`. Nội dung thông báo chấm công nằm trong trường `MessStr` của JSON bên trong `Data` khi thành công, hoặc `DataError` khi bị từ chối. JSON bên trong được đóng gói dưới dạng chuỗi. Lỗi router như “Dữ Liệu Không Toàn Vẹn” dùng `ErrorString`, không phải `MessStr`.

Quẹt lặp cùng hướng có thể trả `IsError=true`, `IsValid=false`, `AlreadyRecorded=true`: điều đó có nghĩa lượt cũ đã ghi nhận, không phải lần quẹt lặp đã tạo thêm phiếu. Khi gửi lại đúng Id của phiếu đã lưu, phản hồi có thể là thành công với `Duplicated=true`.

Nguồn đối chiếu: `Services/ScaleAttendanceProcessor.cs`, `Services/ScaleProductionAttendancePolicy.cs`, `Services/CommunicationService.cs`.

## 5. Tên công việc trong phản hồi chấm công

`TenCongViec` là **tên thành phẩm**, không phải mã.

- Khi Vào: tên thành phẩm đang được chọn trên cân, tra `HQ_ThanhPham.Ten`.
- Khi Ra (kể cả quẹt Ra lặp/gửi lại): tổng hợp tên thành phẩm trên các phiếu cân hợp lệ đã có trong SQL, đúng thẻ nhân viên, đúng cân, từ giờ Vào đến giờ Ra. Giữ thứ tự phát sinh đầu tiên, không lặp tên; không lấy thành phẩm đang chọn lúc Ra để thay lịch sử.
- Ví dụ làm `LV`, sau đó `BV`: `"TenCongViec":"LỘT VỎ, BÀO VỎ"`.
- Chỉ đổi thành phẩm trên máy nhưng chưa phát sinh phiếu cân thì không thêm tên đó vào tổng kết. Chưa có phiếu/tên khớp danh mục thì gửi chuỗi rỗng, không gán tên máy. Tên trong danh mục được giữ nguyên chữ hoa/thường và bỏ khoảng trắng thừa đầu cuối.
- Phiếu offline chưa đồng bộ lên SQL chưa thể có trong tổng kết tại thời điểm Ra. Gửi lại lượt Ra sau khi đồng bộ sẽ tính lại theo phiếu hiện có trong ca.
- Không đổi khóa/kiểu JSON, không sửa lịch sử chấm công hoặc tổng số rổ/kg và không thay đổi firmware. Firmware cần hiển thị được chuỗi nhiều tên dài hơn trước.
