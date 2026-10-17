# Xưởng lịch sử của ca cân

## Triển khai

1. Backup PMS_HQ, chọn thời gian bảo trì. Tạm dừng nhận dữ liệu và thao tác đổi xưởng trên tất cả Hub cùng DB.
2. Kiểm tra các Hub và web trỏ đúng cùng PMS_HQ. Chạy `sql/20261016-XuongLichSuChamCong.sql` bằng SSMS trên DB đó.
3. Publish/cài bản Hub và web mới; nâng cấp **tất cả** bản Hub cùng ghi DB (kể cả demo nếu dùng DB thật). Không chạy lẫn Hub cũ cho thao tác chuyển xưởng.
4. Khởi động lại, thử Vào/Ra, chuyển xưởng, đồng bộ và báo cáo trước khi mở lại sản xuất.

Script thêm `CheckInOut.MaXuong` (nullable), bảng `LichSuXuongMayCan` và hàm `fn_XuongMayCanTaiThoiDiem`.
Ứng dụng không tự chạy DDL/migration. Triển khai web/code mới mà chưa có schema sẽ lỗi: phải chạy SQL trước.
Script có thể chạy lại; không reset mốc đã có, không UPDATE/xóa phiếu lịch sử.

## Quy tắc

- Mốc thời gian dùng UTC+7, độ chính xác giây như firmware. Hai lần chuyển khác xưởng trong cùng một giây bị từ chối để tránh mốc trùng.
- Nút Đổi xưởng lưu cấu hình cân và mốc chuyển trong cùng transaction, có khóa SQL theo cân.
- Online Vào lưu snapshot theo giờ phát sinh; Ra kế thừa xưởng lượt Vào. Ca chuyển máy lưu xưởng máy cũ cho Ra, tra lịch sử máy mới cho Vào.
- Offline POST tra lịch sử bằng **thời gian phát sinh**, không thời điểm đồng bộ. Không yêu cầu đổi firmware.
- Web thêm/sửa ca dùng cùng lịch sử. Chỉ sửa thẻ/ghi chú mà không thay máy/giờ thì giữ nguyên snapshot; đổi máy/giờ phải tra lại.
- Báo cáo ưu tiên snapshot; chỉ khi snapshot trống mới tra lịch sử. Không fallback về MayCan.MaXuong hiện tại.
- Phiếu cân HQ mới dùng lịch sử tại giờ cân; backup có mã xưởng trái mốc lịch sử sẽ yêu cầu đối chiếu, không ghi đè. Backup thiếu thông tin mà không có căn cứ duy nhất vẫn bị từ chối như trước.

## Dữ liệu trước triển khai và giới hạn

Không thể khẳng định xưởng cũ từ cấu hình hiện tại. Ca trước mốc ban đầu có thể hiện **Chưa xác định xưởng lịch sử**.
Quản lý cần đối chiếu phiếu cân/chứng từ trước khi bổ sung mã xưởng cho những ca này. Không chạy UPDATE hàng loạt lấy MayCan hiện tại.
Các lượt offline mới nhận nhưng phát sinh trước mốc ban đầu cũng giữ trạng thái chưa xác định.

Chỉ chuyển xưởng bằng Hub đã nâng cấp. Thay MayCan.MaXuong trực tiếp trong SQL hoặc phần mềm cũ sẽ không ghi mốc lịch sử: phải kiểm soát quy trình này.
Không mang cân đi nơi khác khi còn ca mở; quẹt Ra, chuyển xưởng rồi quẹt Vào lại. Nếu cần chuyển giữa ca, dùng quy trình quản lý và đối chiếu riêng.
Không sửa/xóa lịch sử chuyển xưởng tùy tiện: các phiếu cũ chưa có snapshot vẫn đang dùng lịch sử đó.
Không tự sửa dữ liệu xưởng lịch sử của phiếu đã được lưu đúng.

## Thử nghiệm chấp nhận

- Cân tại xưởng 2 lúc 08:00, Ra; chuyển xưởng 1 lúc 10:00, Vào/Ra tiếp. Báo cáo ca trước vẫn ở xưởng 2, ca sau ở xưởng 1.
- Lượt offline lúc 09:00 gửi sau 10:00 vẫn thuộc xưởng 2; lượt 10:01 thuộc xưởng 1.
- Chuyển lại hôm sau, tải báo cáo hôm trước: dữ liệu không đổi xưởng theo cân.
- Ca không có mốc phù hợp hiện cảnh báo, không tự ghép vào xưởng hiện tại.
- Sửa ghi chú/giờ/máy trên web, chạy lại Excel và đối chiếu; kiểm tra snapshot theo đúng quy tắc trên.

Kiểm thử tự động hiện có chạy bộ nhớ/HTTP cục bộ; chưa thay thế thử trên bản sao SQL và cân thật.
