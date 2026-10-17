# Ca chung theo mã thành phẩm tại cân HQ

## Quy tắc

- Vào A lần đầu: tạo một ca trong CheckInOut; lưu mã thành phẩm của lần Vào trong ScaleAttendanceScan.
- Vào B cùng mã: chỉ lưu lượt tham gia ca trong ScaleAttendanceScan. Giữ giờ Vào, loại ca và giới hạn giờ của A; không tạo thêm ca tính giờ.
- Vào cân khác mã: ghi Ra liên kết VaoId của ca cũ và Vào ca mới trong cùng transaction. Giữ loại ca thường/đêm. Cũng xử lý khi trở lại máy gốc đã đổi mã.
- Ra tại A hoặc máy đã tham gia: đóng ca chung. CheckInOut.MaSoMay của phiếu Ra giữ theo máy gốc để chốt ngày và ghép giờ hiện có không cộng hai lần; ScaleAttendanceScan lưu máy quẹt thực tế.
- Máy tham gia chỉ được ghi sản lượng sau lần Vào được chấp nhận, đúng mã thành phẩm của lần đó. Máy gốc giữ cách ghi phiếu hiện có. Muốn chuyển thành phẩm tại máy tham gia thì quẹt Vào để mở ca mới.
- Tổng khi Ra cộng phiếu hợp lệ (Status=1, ChiSanLuong=true, trừ tare) của đúng thẻ trong ca: máy gốc và các máy tham gia với đúng mã/thời điểm. Khi Ra và Vào ca mới cùng giây, phiếu khớp máy/mã mới tính cho ca mới; phiếu ở máy/mã cũ vẫn thuộc ca cũ, không cộng hai lần.
- Vào lặp tại máy gốc cùng mã vẫn trả ALREADY_CHECKED_IN. Gửi lại đúng Id đã nhận trả Duplicated=true; không mở/đóng thêm ca. Các lượt tiếp tục ca cũng có Id được lưu bền vững.
- Id của Ra tự động có dạng `<IdVao>.AUTO.<yyyyMMddHHmmss>`, tránh trùng với Id lượt quẹt thật khi đóng/mở cùng giây. Firmware không phải tạo hoặc gửi Id này.

Không tự đóng ca hết hạn, ca bị đánh dấu lỗi, nhiều ca đang mở thật hoặc ca chưa xác định được mã thành phẩm. Dữ liệu cũ chưa có snapshot chỉ được nhận mã từ sản lượng đúng máy/thẻ/ca có đúng một mã duy nhất; không lấy cấu hình hiện tại để đoán lịch sử. Không sửa GhiChu, không xóa ca/phiếu cũ.

## Đồng bộ

Route HTTP và các khóa JSON cũ được giữ. POST `/api/chamcong/post` nhận thêm trường tùy chọn `MaThanhPham`: **mã tại thời điểm quẹt**, không phải tên. Khi dữ liệu đã được xử lý online, gửi lại không cần mã này vẫn nhận lại kết quả theo Id đã lưu.

Lượt Vào offline mới sang cân khác cần đủ mã để quyết định tiếp tục hoặc chuyển ca. Firmware cũ không gửi mã thì không được tạo thêm một ca khác để bỏ qua kiểm tra. HTTP trả 409 với message/code rõ ràng khi không thể xác định an toàn. Lượt chấm công mới cũ hơn lịch sử đã chấp nhận cũng trả OUT_OF_ORDER, không dùng giờ nhận lại để đóng ca đang làm; cần đối chiếu hoặc đồng bộ chấm công theo thứ tự trước khi tiếp tục online.

PHIEUCANADD/PHIEUCANADDS kiểm tra tại ngày giờ cân gốc. Phiếu tại máy tham gia được nhận nếu ca và lượt tham gia đã đồng bộ, đúng mã và thời điểm; không dùng ca hiện tại để thay ca lịch sử. Thẻ nhóm sản xuất vẫn giữ quy tắc không bắt buộc chấm công.

Chuyển máy thủ công của quản trị vẫn là thao tác đóng/mở ca độc lập có người xác nhận và lý do. Kiểm tra giờ chuyển bao gồm cả các lượt tham gia ca chung.

## Triển khai

1. Sao lưu DB và bản publish. Dừng toàn bộ instance PMSHub phục vụ các cân liên quan.
2. Chọn đúng DB PMS_HQ của hệ thống, chạy `docs/sql/20261017-ScaleAttendanceScan.sql`. Script chỉ tạo bảng/index mới, có thể chạy lại; không thay dữ liệu và cấu trúc CheckInOut/HQ_PhieuCan cũ. Không tự chạy script từ ứng dụng.
3. Publish PMSHub đã cập nhật, khởi động đồng bộ các instance. Bản mới bắt buộc có bảng ScaleAttendanceScan; không triển khai xen kẽ bản cũ/mới.
4. Với firmware đồng bộ offline, gửi MaThanhPham của từng lượt Vào để hỗ trợ chuyển cân trước khi sản lượng được đồng bộ.
5. Nghiệm thu trên DB test và cân thật trước rollout. Bộ test tự động hiện không kết nối DB hoặc gửi lệnh đến cân.

Nếu quay về bản cũ: dừng Hub, xử lý các ca chung còn mở và đồng bộ dữ liệu thiết bị trước khi đổi bản. Giữ nguyên bảng ScaleAttendanceScan để đối chiếu; không tự xóa. Bản cũ không hiểu lượt tham gia ca chung.

## Nghiệm thu

- A 08:00 TP01, B 09:00 TP01: chỉ một Vào tính giờ; hai cân đều ghi phiếu. Ra B 10:00: một Ra nối Vào A, tổng hai giờ và đúng kg/rổ A+B.
- Sau đó Vào C TP02: ca mới bắt đầu tại C. Thử chuyển thẳng khi ca TP01 còn mở: Ra TP01 và Vào TP02 cùng thời điểm, toàn bộ cùng commit hoặc rollback.
- B chưa Vào dù cùng mã không được ghi phiếu. Đổi mã tại B rồi quẹt Vào: đóng ca cũ, mở ca mới. Trở lại A khác mã cũng chuyển được, không trùng Id.
- Gửi lại cùng Id Vào/Ra/tiếp tục và thử hai cân quẹt đồng thời: không tạo thêm ca hoặc thêm phiếu.
- Khởi động lại Hub giữa ca rồi quẹt B: giữ giờ Vào cũ, không mất liên kết máy tham gia.
- Backup trước/sau/đúng giây chuyển: ghép đúng ca, không cộng trùng. Phiếu trước khi B tham gia hoặc sai mã/thẻ bị từ chối.
- Ca thường qua 0 giờ, ca đêm còn hạn/hết hạn, Isforget=1, ca cũ nhiều mã/không có mã và dữ liệu trùng thật: không tự đoán hoặc bịa giờ Ra.
- Đối chiếu báo cáo CheckInOut hiện có và việc chốt thiếu Ra. Các lượt tham gia không xuất hiện thành ca tính giờ thứ hai.

## Kiểm tra mã nguồn

```text
dotnet build PMSHub/PMSHub.csproj --no-restore
dotnet run --project tests/ScaleSharedAttendance/ScaleSharedAttendance.csproj
dotnet run --project tests/ScaleAttendanceProtocol/ScaleAttendanceProtocol.csproj --no-restore
dotnet run --project tests/HqWeightBackup/HqWeightBackup.csproj --no-restore
```

ScaleSharedAttendance kiểm tra quy tắc, tập phiếu, ranh giới ca và khả năng dịch truy vấn sang SQL Server mà không mở kết nối SQL. Chưa thay thế kiểm thử giao dịch SQL, deadlock, firmware, báo cáo bên ngoài hoặc sự cố mất kết nối trên môi trường test.
