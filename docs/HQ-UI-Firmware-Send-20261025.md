# HQ: tách cập nhật web và giới hạn chờ gửi firmware — 25/10/2026

Áp dụng tại `D:\RCQ\RCQ-ProductMaterialWorkshop`, nhánh `feature/hq-shift-product-material-workshop`. Không sửa checkout `D:\RCQ\RCQ`/nhánh 123, không deploy, không thay firmware hoặc cấu hình kết nối xưởng.

## 1. Cập nhật web không chặn phản hồi cân

- `CommitService` đưa thêm dòng HQ và thông báo refresh sang `HqOnlineUi`, sau khi SQL đã COMMIT. Không đưa thao tác INSERT, kiểm tra ca, tính tổng hay gửi firmware vào queue này.
- Mỗi đối tượng cân có một worker riêng và tối đa 100 dòng hiển thị đang chờ; nhiều thông báo refresh gộp thành thông báo mới nhất. Không kéo theo request/AsyncLocal gửi firmware vào worker.
- Khi web kẹt và hàng đợi đầy, chỉ bỏ dòng **hiển thị cũ nhất**, không xóa/mất phiếu SQL. Danh sách thiết bị vốn chỉ hiển thị 100 dòng; báo cáo/SQL vẫn là nguồn đầy đủ.
- `MayCansService.AddItem` của HQ xuất bản một collection mới, tối đa 100 dòng, loại Id trùng. Không vừa sửa collection cũ vừa để grid đọc hoặc gọi `CollectionChanged` của collection cũ trên luồng cân.
- Timer refresh dùng cửa sổ 100 ms, không bị hoãn mãi khi có lưu lượng liên tục. Không chạy chồng callback nếu browser subscriber chậm; lấy từng thông báo bằng `TryRemove`, không `ToArray()+Clear()` làm mất refresh mới giữa lượt xử lý.
- Đây là tối ưu **đường chờ firmware**; không khẳng định đã chạy thử giao diện Syncfusion thật trên browser hoặc đo server xưởng.

## 2. Gửi firmware có giới hạn chờ

Luồng bình thường giữ nguyên:

`kiểm tra → COMMIT SQL → tính đủ tổng → SETNHANVIEN → ReceiveData`

- Gửi `SETNHANVIEN`: tối đa chờ 2 giây.
- Gửi `ReceiveData` HQ online LITE/CHAMCONG: tối đa chờ 3 giây.
- Đây là **giới hạn riêng phần gửi mạng**, không phải chèn delay, không phải cam kết cả lượt cân xong trong 5 giây. Hàng đợi máy, SQL và tính tổng có giới hạn riêng; không sửa các giới hạn đó trong đợt này.
- Vẫn giữ nguyên target và tham số firmware; `SETNHANVIEN` dùng ba tham số mã nhân viên, mã hồ sơ, tên nhân viên. ST/0 vẫn chỉ nhận diện, không lưu phiếu; US vẫn không lưu. Offline HTTP không cần ST và không bị áp luật ST của online.
- Gắn identity và kết quả với **kết nối gốc**. Không đổi sang ConnectionId mới giữa chừng sau khi cân reconnect.
- Nếu gửi bị treo/lỗi: hủy phần chờ gửi, ghi log `HQ TRANSPORT UNCONFIRMED` với Id, máy, target và trạng thái lưu gốc, rồi abort kết nối cũ để tránh gói muộn chen vào lượt sau. Nếu identity thất bại thì không xếp thêm kết quả sau một write đang kẹt.
- Không gửi fallback thứ hai sau khi đã bắt đầu gửi phản hồi cuối; không INSERT lại, không rollback phiếu đã COMMIT và không đổi `SAVED` thành `NOT_SAVED` chỉ vì mạng lỗi. Lỗi logger cũng không gây gửi lại.
- `SendAsync` hoàn tất chỉ là hoàn tất bước gửi của SignalR, **không phải ACK của firmware**. Khi mạng đứt vẫn không thể bảo đảm cân đã nhận phản hồi. Firmware không giữ pending Id nên WSS không thể tự biết Id mới sau đó là cùng rổ hay rổ mới. Đợt này không thay chính sách chống trùng chỉ theo cùng Id.
- Không bỏ bước tính đủ tổng để ACK sớm hoặc trả tổng giả. Nếu bước đọc tổng lỗi sau COMMIT, phản hồi vẫn ghi `SaveStatus=SAVED` như bản sửa trước.

## Kiểm thử

Build Release: 0 errors, 388 warnings có sẵn trong toàn project.

| Bộ kiểm tra | Kết quả |
| --- | ---: |
| HqStableSwipe, gồm deadline, kết nối mới/cũ, queue UI, timer | 280 PASS |
| HqAttendanceDiagnosis, gồm đường WSS thật qua SQL rồi giả lập UI/mạng treo | 53 PASS |
| HqOnlineWeight | 112 PASS |
| HqHttpBackup | 111 PASS |
| ScaleOnlineWorkContext | 71 PASS |

Tổng 627 assertions. SQL dùng database LocalDB GUID riêng, xóa sau khi chạy; không dùng cấu hình/dữ liệu xưởng. Riêng kiểm thử deadline bao gồm transport cố tình bỏ qua cancellation và hoàn tất muộn, identity treo, lỗi logger, UI kẹt, queue vượt 100 dòng, notification đến giữa lúc flush và lưu lượng refresh liên tục.

30 kết nối SignalR WebSocket loopback, chạy hai lần: 660/660 phản hồi, 660/660 phiếu SQL, 660 SETNHANVIEN, đủ tổng rổ/kg. Lần 2 kiểm tra thêm identity phải đến trước phản hồi cuối. Đợt đầu 1,740–1,840 giây; nóng ít dữ liệu 47,61–68,53 ms; sau thêm 100.000 phiếu 591,37–728,43 ms. Đơn vị là thời gian hoàn tất **cả đợt 30 cân**, không phải số đo trên server xưởng. Máy i7-10750H/32 GB, LocalDB Express 17.0.4025.3, RCSI ON; không bao gồm TLS, LAN, browser, background jobs và tải báo cáo thực tế. Chi tiết số đo nằm ở `HQ-WSS-30-Clients-20261025.md`.

## Triển khai

Đợt tối ưu hai mục này **không cần thêm bảng/index hoặc thay firmware**. Những SQL migration cần cho toàn bộ bản ProductMaterialWorkshop vẫn phải đủ theo checklist triển khai trước đó; không có nghĩa nhánh 123 chỉ cần chép riêng Services.dll.

Rebuild/publish đầy đủ PMSHub từ checkout ProductMaterialWorkshop, giữ file kết nối SQL/URL đúng môi trường xưởng. Không chép DLL lẻ trộn với release cũ. Chưa tự thực hiện publish/restart/deploy trong lần sửa này.
