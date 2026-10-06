# Chấm công online tại cân

Firmware đã Auth thành công gọi SignalR:

```text
SendData("CHAMCONG", độDài, json)
```

```json
{"ChamCongId":"7779520.20261005090106","TheId":"0002344026","NgayGio":"20261005090106","TrangThai":"Vào"}
```

Gói ví dụ có 105 ký tự, 106 byte UTF-8. Hub chấp nhận cả hai cách
đếm cho CHAMCONG. Mã máy lấy từ kết nối đã Auth, không tin tiền tố Id
do thiết bị tự khai báo. ChamCongId phải khớp mã máy + "." + NgayGio.

## Phản hồi

Hub gọi `ReceiveData("CHAMCONG", envelopeJson)`.
Firmware parse envelopeJson trước, sau đó parse chuỗi JSON ở Data hoặc DataError.
Giữ tương thích DataTranfer đang dùng: Data/DataError là chuỗi, không phải object.

Thành công (Data sau khi parse):

```json
{
  "ChamCongId": "7779520.20261005090106",
  "TheId": "0002344026",
  "MaNhanVien": "NV001",
  "TenNhanVien": "Nguyễn Văn A",
  "TongSoRo": 0,
  "TongTrongLuong": 0,
  "TenCongViec": "AnGa",
  "TrangThai": "Vào",
  "ThoiGianVao": "2026-10-05 09:01:06",
  "ThoiGianRa": null,
  "TongThoiGianGiay": 0,
  "TongThoiGian": "00:00:00",
  "IsValid": true,
  "Duplicated": false,
  "MessStr": "Chấm công hợp lệ."
}
```

Envelope: IsError=false, Data=chuỗi JSON trên, DataError=null.
MaNhanVien/TenNhanVien tra TheTu.MaNhanVien -> NhanVienDaiThanh.
TenCongViec và CheckInOut.TenMay/CongViec lấy MayCan.DisplayName.
Phiếu chấm công được lưu CheckInOut, không lưu ChamCongTaiXuong.

Không hợp lệ (DataError sau khi parse):

```json
{
  "ChamCongId": "7779520.20261005090106",
  "TheId": "0002344026",
  "IsValid": false,
  "Code": "CARD_NOT_REGISTERED",
  "MessStr": "Thẻ chưa đăng ký"
}
```

Envelope: IsError=true, Data=null, DataError=chuỗi JSON trên.
Firmware hiện MessStr; không báo chấm công thành công, không xem bản này đã lưu.
Các Code: INVALID_JSON, INVALID_CARD, INVALID_TIME, INVALID_DEVICE,
INVALID_ID, INVALID_STATUS, INVALID_DEVICE_NAME, CARD_NOT_REGISTERED,
ID_CONFLICT, OUT_OF_ORDER, INVALID_TRANSITION, MISSING_CHECK_IN,
ALREADY_CHECKED_IN, ALREADY_CHECKED_OUT, SERVER_ERROR.
Ca quá hạn trả SHIFT_EXPIRED; cấu hình không hợp lệ trả INVALID_CONFIGURATION.
Lỗi kiểm tra độ dài/active từ router vẫn có ErrorString và Cmd như giao thức cũ.
SERVER_ERROR có thể thử lại cùng Id; không tạo Id mới khi gửi lại.

## Quy tắc ca

- Cùng mã thẻ tại cùng máy và cùng ngày: lần đầu Vào; Vào -> Ra -> Vào.
- Ra khi chưa Vào: không lưu, trả lỗi.
- Vào/Vào hoặc Ra/Ra với Id mới: không lưu lượt lặp; trả IsError=true,
  DataError có IsValid=false, AlreadyRecorded=true, Code=ALREADY_CHECKED_IN
  hoặc ALREADY_CHECKED_OUT và MessStr="Đã quẹt Vào rồi." / "Đã quẹt Ra rồi.".
  DataError vẫn có thông tin nhân viên, sản lượng và mốc thời gian của ca đã lưu.
  Firmware cần đọc các mốc trong DataError để hiển thị, không tự gửi lại liên tục.
- Vào lặp: ThoiGianVao giữ giờ Vào đầu, ThoiGianRa=null, tổng thời gian=0.
- Ra lặp: giữ cả giờ Vào và giờ Ra đã lưu, tổng thời gian không tăng theo giờ quẹt lặp.
- Qua 0 giờ reset trạng thái theo ngày; Vào 22:00 -> Ra 06:00 hôm sau
  không được ghép ca. Ngày mới phải quẹt Vào trước khi quẹt Ra.
- Tác vụ ScaleAttendanceDayCloseService chạy lúc 0 giờ UTC+7 và khi PMSHub khởi động.
  Mọi lượt Vào ngày trước không có Ra trước lượt Vào kế tiếp trong cùng ngày/máy/thẻ
  được đặt Isforget=1; không sửa GhiChu.
- Không tạo phiếu Ra giả lúc 23:59:59, không tự xóa phiếu. Dữ liệu thiếu Ra
  để nhân sự bổ sung giờ thực tế trong cùng ngày.
- Nếu server dừng lúc 0 giờ, khi chạy lại sẽ quét bù. Reset trạng thái trong API
  dựa vào phạm vi ngày của lượt quẹt, không phải đợi tác vụ chốt chạy xong.
- Danh sách nhân sự: chạy file HR_MissingScaleCheckouts.sql (chỉ đọc).
  Báo cáo CheckInOut cũng không ghép/cộng giờ sang ngày hôm sau.
- Id đã lưu, cùng thẻ và trạng thái: phản hồi hợp lệ trong Data, Duplicated=true,
  MessStr="Đã quẹt Vào rồi." hoặc "Đã quẹt Ra rồi.", không ghi thêm.
- Id cùng giây nhưng khác thẻ/trạng thái: giữ dòng đầu, trả ID_CONFLICT.
- Lượt cũ hơn lịch sử của thẻ tại máy: trả OUT_OF_ORDER, không chèn.
- Khóa giao dịch SQL theo máy ngăn hai request online làm sai trạng thái.
- Giới hạn ca mặc định 16 giờ, cấu hình ScaleAttendance:MaxShiftHours trong
  PMSHub/appsettings.json (1..24 giờ); khởi động lại PMSHub sau khi chỉnh.
  Đây là ngưỡng vận hành phát hiện quên quẹt, không phải quy định tính lương.
- Trong cùng ngày, nếu ca Vào đã quá giới hạn mà quẹt Ra: IsError=true, Code=SHIFT_EXPIRED,
  CanBoSungGioRa=true, trả ThoiGianVao cũ và ThoiGianQuet hiện tại;
  ThoiGianRa, TongThoiGianGiay, TongThoiGian đều null. Firmware hiện MessStr
  và dừng tự gửi lại; không hiển thị tổng nhiều ngày hoặc coi null là giờ công.
- Trong cùng ngày, nếu ca Vào đã quá giới hạn mà quẹt Vào: ghi ca Vào mới, tổng bằng 0,
  IsError=false, WarningCode=PREVIOUS_SHIFT_MISSING_OUT và ThoiGianVaoCaTruoc.
  Firmware vẫn hiện cảnh báo MessStr trong phản hồi thành công.
  Không lưu thêm metadata vào GhiChu hoặc tạo cột tham chiếu ca cũ.
- Không tự tạo giờ Ra hoặc chỉnh/xóa ca cũ; quản lý bổ sung giờ thực tế.
- Phiếu Ra cũ vượt giới hạn cũng bị chặn khi gửi lại, trước khi tính sản lượng.
  Giới hạn giờ áp dụng luồng CHAMCONG online tại cân. Chốt ngày đánh dấu lỗi
  tất cả nguồn phiếu trong CheckInOut; báo cáo cân bỏ ghép qua ngày.
  Luồng chấm công tại xưởng (ChamCongTaiXuong) không nằm trong thay đổi này.

## Sản lượng

Mỗi phản hồi thành công có thêm mốc thời gian và tổng thời gian của ca:

- ThoiGianVao: chuỗi yyyy-MM-dd HH:mm:ss.
- ThoiGianRa: null khi Vào; chuỗi yyyy-MM-dd HH:mm:ss khi Ra.
- TongThoiGianGiay: số nguyên tổng giây, firmware dùng để tính toán.
- TongThoiGian: chuỗi tổng giờ:phút:giây để hiển thị, ví dụ 08:00:00.
  Định dạng không cuộn vòng sau 24 giờ; ca vượt giới hạn cấu hình
  bị từ chối trước khi tạo phản hồi tổng thời gian.

Ví dụ phần thời gian trong Data khi Ra cùng ngày:

```json
{
  "ThoiGianVao": "2026-10-05 08:00:00",
  "ThoiGianRa": "2026-10-05 16:00:00",
  "TongThoiGianGiay": 28800,
  "TongThoiGian": "08:00:00"
}
```

Các mốc dùng giờ thiết bị đã lưu trong database, không tự đổi sang UTC.
Tổng thời gian là Ra trừ Vào trong cùng ngày tại máy này, chưa trừ nghỉ giữa ca.
Gửi lại cùng lượt quẹt dùng lại mốc sự kiện, không dùng giờ nhận lại ở server.
Nếu thiếu mốc Vào, trả MISSING_CHECK_IN thay vì báo tổng bằng 0.

Vào: bắt đầu ca mới, TongSoRo/TongTrongLuong bằng 0.
Ra: lấy từ thời điểm Vào gần nhất đến thời điểm Ra, cùng máy và
HQ_PhieuCan.TheIdNhanVien bằng mã thẻ; chỉ ChiSanLuong=true, Status=1.
TongSoRo là số phiếu (giả định một phiếu là một rổ).
TongTrongLuong là tổng TrongLuong - TrongLuongTare, cùng đơn vị dữ liệu phiếu cân.
Sang ngày mới tổng ca mới bằng 0; không cộng sản lượng hoặc thời gian của ngày trước.
Chỉ tính phiếu đã có trong SQL lúc xử lý; phiếu đồng bộ muộn có thể
làm tổng tăng nếu thiết bị yêu cầu lại cùng lượt Ra.
Không tính dữ liệu máy khác hoặc các bảng phiếu cân chuyên biệt khác.

## Kiểm tra / triển khai

Build PMSHub rồi build/run tests/ScaleAttendanceProtocol.
Test không truy cập SQL: độ dài UTF-8, router, JSON lỗi, Id giả,
các chuyển trạng thái và ranh giới 0 giờ UTC+7. Vẫn cần thử thực tế chốt lỗi SQL,
nhận/lưu/tổng ca với database.
Publish lại PMSHub; Id CheckInOut phải là varchar(100) như thay đổi trước.
Index tùy chọn: AddScaleAttendanceOnlineIndexes.sql trong project báo cáo.
Chính sách mới thay thế cách ghép ca qua đêm trước đây tại cân; phản hồi HTTP tại xưởng không đổi.

## Đối chiếu khi thiết bị báo hai phản hồi

Mỗi lần phương thức SendData nhận CHAMCONG có một Trace mới:
- CHAMCONG RX: lúc Hub nhận lời gọi, có ChamCongId, Connection, Host, PID,
  UTC và SHA-256 của payload.
- CHAMCONG TX: sau một lần SendAsync hoàn tất, cùng Trace và Connection;
  hash là SHA-256 của chuỗi envelope phản hồi. Đây không phải xác nhận
  firmware đã nhận hay đã hiển thị.
- REJECT/ERROR: yêu cầu không được xử lý hoặc gửi thất bại.

Hai RX cùng ChamCongId với hai Trace khác nhau chứng minh Hub đã nhận
hai lời gọi; tiếp tục đối chiếu log gửi của thiết bị, connection và các instance.
Một RX/một TX nhưng firmware in hai lần thì đối chiếu đúng phiên bản server,
mọi instance và callback/log nhận firmware. Không kết luận chỉ từ số dòng
in bên thiết bị. Thu thập cả hai đầu trong cùng một lần quẹt.
Trace chỉ nằm trong log server, không thay đổi nội dung phản hồi firmware.


## Tách ghi chú người dùng khỏi dữ liệu hệ thống
- Với bảng đã chuyển đổi trước đây: chạy database/SimplifyCheckInOutColumns.sql trong project ChamCongSanXuat.
- Dừng cả PMSHub và web báo cáo khi chuyển đổi; cập nhật cả hai ứng dụng cùng đợt.
- Chỉ thêm TrangThai để lưu Vào/Ra; Isforget có sẵn dùng đánh dấu thiếu Ra.
- Không dùng MaLoi, CaTruocThieuRaId, DuLieuNguon. Script đơn giản hóa bỏ ba cột này sau khi sao lưu.
- GhiChu: chỉ dành cho người dùng. Tính ca không phân tích GhiChu nữa.
- Script tạo CheckInOut_BackupBeforeSimplify để khôi phục ba cột đã bỏ khi cần; giữ nguyên các bản sao lưu trước đó.
