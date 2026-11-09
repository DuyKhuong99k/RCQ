# Phản hồi HQ LITE - 23/10/2026

> Cập nhật 24/10: firmware hiện chờ response mới cho cân tiếp, không tự mở lượt sau 5s.
> Đã tối ưu writer async/lookup và thêm giới hạn chờ/log chậm phía server;
> xem `HQ-WSS-Stability-20261024.md`. Các nhắc tới timeout tự mở lượt ở phần lịch sử dưới đây
> không còn mô tả firmware hiện tại. Format response/SETNHANVIEN giữ nguyên.

Chỉ sửa RCQ, không sửa firmware. Theo lựa chọn của người dùng: **chờ tính đủ tổng rồi mới gửi hoàn thành**.
Không ACK sớm, không gửi `TotalsAvailable=false`, không thay tổng thiếu bằng 0 hoặc cache cũ.

## Giữ tương thích

- Giữ `ReceiveData` với arguments `["LITE", "<JSON DataTranfer>"]`.
- Giữ `IsError`, `Data`, `DataError` và JSON lồng dạng chuỗi như trước.
- Giữ `IsDataAction=1`, Id, TheId, NgayGio, kg và bốn trường tổng ở phản hồi thành công.
- Giữ lệnh `SETNHANVIEN` với đúng ba arguments `[MaNhanVien, MaHoSo, Name]`.
- Các trường mới là bổ sung; các DTO ở nhánh cũ không phát thêm trường null.

## Thông tin đối chiếu và trạng thái lưu

Lỗi HQ có thêm `Id`, `TheId`, `NgayGio`, `MayCan` nếu đọc được từ request/kết nối đăng ký.
Chỉ echo chuỗi có độ dài hợp lệ; JSON hỏng không được cố suy đoán Id.
ACK thành công vẫn có Id/thẻ/giờ, thêm `SaveStatus`, và mã hồ sơ/tên nhân viên nếu đã có trong cache của đường ghi mới.

`SaveStatus`:

- `NOT_SAVED`: request này bị từ chối trước bước ghi (US, ST/0 hoặc ST/kg âm nhận diện, sai ca, JSON sai...). Không có xác nhận INSERT mới. Trạng thái này không có nghĩa một Id đã tồn tại từ request trước bị xóa hoặc chưa từng được lưu.
- `SAVED`: đã COMMIT, hoặc đã đối chiếu đúng Id/nội dung với phiếu SQL đang có.
- `UNKNOWN`: lỗi hệ thống/SQL ở thời điểm chưa xác minh được kết quả; không khẳng định phiếu chưa lưu. Báo quản lý đối chiếu SQL, không yêu cầu nhân viên quẹt lại để tạo Id mới. Giao thức vẫn hỗ trợ đối chiếu cùng Id nếu một client có giữ phiếu gốc, nhưng luồng hiện tại không phụ thuộc khả năng đó của firmware.

## Các tình huống

1. US hoặc thiếu/sai trạng thái: lỗi cân chưa ổn định, không INSERT/cộng tổng/nhận diện; không giữ thẻ chờ.
2. ST/0 hoặc ST/kg âm ở lượt mới: nhận diện bằng cache, gửi SETNHANVIEN; LITE vẫn lỗi `Số kg bằng 0. Không lưu phiếu` hoặc `Số kg âm. Không lưu phiếu`, NOT_SAVED. Không tự mở ca, không tính lại/cộng/reset tổng, không đọc/ghi SQL. ST là firmware báo ổn định, không đồng nghĩa kg phải dương. Tare âm, thẻ sai, giờ/Id sai và lượt chờ cũ vẫn bị từ chối; không dùng thẻ của lượt này cho phiếu tiếp theo.
3. ST/kg dương: kiểm tra ca + INSERT/COMMIT, tính đầy đủ tổng rồi gửi ACK hoàn thành, SAVED.
4. Gửi lại Id đã lưu và khớp nội dung: không INSERT/cộng thêm; tính lại tổng đầy đủ rồi ACK cùng Id.
5. SQL đã lưu nhưng tính tổng timeout/lỗi: WSS **tự thử lại riêng query tổng**, tối đa 3 lần, nghỉ 150 ms giữa các lần trong ngân sách chờ 4 giây cho phần tổng. Mỗi lần dùng DbContext mới và CancellationToken; không gọi lại INSERT/kiểm tra ca hoặc yêu cầu firmware gửi phiếu. Tổng đọc được thì ACK đầy đủ cho Id gốc. Nếu vẫn lỗi/hết hạn: không báo lỗi INSERT, không gửi ACK hoàn thành hoặc tổng giả; gửi DataError `Đã lưu phiếu. Chưa có tổng`, `SaveStatus=SAVED`, kèm Id/thẻ/giờ/máy. Nhân viên không quẹt lại chỉ vì lỗi tổng; quản lý kiểm tra SQL/log. Query cũ hoàn tất sau hết hạn không được tự phát ACK/cập nhật UI cho lượt khác.
6. Exception SQL chưa xác minh COMMIT: lỗi ngắn, UNKNOWN; chi tiết chỉ ở log máy chủ.
7. JSON/exception/router lỗi: luôn có DataError với ErrorString, không trả DataError=null. JSON sai được đánh dấu NOT_SAVED; lỗi ngoài dự kiến UNKNOWN.
8. Lỗi UI/SETNHANVIEN không làm phiếu đã COMMIT và tổng đã đọc thành công biến thành lỗi lưu.

## Phối hợp bên nhúng

- Hai thông điệp có thể tới chung một frame; firmware đã xử lý tách frame theo thông tin người dùng xác nhận.
- Firmware cần dùng Id/thẻ trong ReceiveData để ghép đúng lượt, không dùng ACK/error cũ để kết thúc lượt mới sau timeout 5 giây.
- SETNHANVIEN vẫn là lệnh hiển thị tương thích cũ, không có Id trong ba arguments và không chứng minh đã lưu phiếu. Nhận được tên nhân viên không phải ACK cân thành công.
- Firmware cần đọc SaveStatus để phân biệt đã lưu, bị từ chối và chưa xác minh. Với firmware không giữ Id gốc, WSS không yêu cầu thao tác gửi lại/cấp Id mới vì lỗi tổng; WSS tự retry phần đọc. Một request Id mới vẫn được xét là phiếu mới, không có cách chắc chắn phân biệt hai rổ thật với một rổ quẹt lại chỉ dựa kg/thẻ. Khóa rổ do nhúng, không thêm chặn thời gian làm mất phiếu thật.
- Không thể bảo đảm dưới 5 giây nếu SQL/mạng chậm: người dùng chọn chờ đủ tổng. Không thêm delay chủ động hoặc thay thuật toán tổng trong lần sửa phản hồi này.
- Offline/Sync và báo cáo không đổi. Không cần migration SQL cho các trường phản hồi mới.

## Kết quả kiểm tra trên máy phát triển

### Thẻ nhóm theo ca — 26/10/2026

- Theo yêu cầu mới, mọi thẻ sản xuất hợp lệ (nhân viên hoặc `IsNhom=1`, kể cả nhóm đồng thời `IsHuman=1`) phải có ca Vào còn hạn trước khi ghi phiếu LITE dương. Không còn ngoại lệ nhóm ở đường kiểm tra online đồng bộ hoặc bất đồng bộ.
- Chưa Vào hoặc đã Ra: `Chưa Vào ca. Quẹt Vào`, `SaveStatus=NOT_SAVED`, không INSERT phiếu mới. Cùng thành phẩm/nguyên liệu/xưởng được cân ở máy khác; đổi một mã phải quẹt Vào để chuyển ca. Ca ngày/đêm và tổng kg dùng cùng chính sách đang áp dụng cho nhân viên.
- ST/kg 0 hoặc âm vẫn chỉ nhận diện thẻ và trả thông báo đúng số kg, không mở ca hay ghi phiếu. Gửi lại đúng Id/nội dung đã lưu vẫn xác nhận `SAVED` dù sau đó đã Ra; đây không phải phiếu mới hoặc cộng tổng lần nữa.
- Tách ngoại lệ nhóm của backup offline lịch sử sang `HqWeightBackupProcessor.RequiresLegacyAttendance`. Không áp quy tắc online mới ngược vào phiếu offline cũ; HTTP backup không đổi. Muốn áp dụng bắt ca cho cả offline cần thống nhất riêng với nhúng vì dữ liệu lịch sử trước đây được tạo không bắt ca.
- Không sửa firmware, không đổi schema SQL, không xóa/sửa dữ liệu phiếu hoặc tự tạo ca lịch sử. Cần rebuild/publish toàn bộ PMSHub của bản làm việc này; chưa tự deploy xuống xưởng.
- Kiểm tra: Release build **0 errors, 388 warnings mã nền**. HqAttendanceDiagnosis **91 PASS** (thêm 38 assertions cho nhóm ở cả hai trạng thái IsHuman, chạy public router + SQL riêng), HqStableSwipe **512 PASS**, HqOnlineWeight **123 PASS**, ScaleOnlineWorkContext **71 PASS**, HqWeightBackup **175 PASS**, HqHttpBackup **111 PASS**: tổng **1.083 assertions**. Fixture online nhóm của HTTP test đã bổ sung ca Vào hiện hành, không dựa vào ngoại lệ bỏ ca cũ. Các SQL test dùng LocalDB GUID riêng, đã tự dọn; chưa thử cân vật lý hoặc server xưởng.

### Nhận diện ST/kg âm — 26/10/2026

- Gói LITE `Stated=ST`, `TrongLuong=-0.80`, `TrongLuongTare=0.80` đi cùng nhánh nhận diện với ST/0. Thẻ nhân viên/nhóm sản xuất hợp lệ được trả `SETNHANVIEN [MaNhanVien, MaHoSo, Name]`, rồi `ReceiveData` có `IsError=true`, `Data=null`, `DataError.ErrorString="Số kg âm. Không lưu phiếu"`, `SaveStatus=NOT_SAVED` và Id/thẻ gốc.
- ST là firmware báo ổn định; kg âm không bị gán nhầm thành US. Không suy đoán nguyên nhân kg âm là chưa đặt rổ hoặc sai tare: chỉ thông báo đúng giá trị nhận được.
- Không mở SQL cho nhánh nhận diện, không INSERT/update phiếu, không mở/đóng ca hoặc tính/cộng/reset tổng. Không giữ thẻ pending để tự ghi khi trọng lượng dương sau đó. Writer ghi phiếu riêng vẫn từ chối kg bằng 0/âm trước bất kỳ SQL command nào.
- US/thiếu trạng thái, payload không hợp lệ, tare âm, thẻ chưa đăng ký/không phải sản xuất và lượt chờ cũ vẫn không được dùng để nhận diện. Số kg âm khác với tare âm; không bỏ kiểm tra tare.
- Build Release 0 errors (388 warnings mã nền). HqStableSwipe **512 PASS**, HqOnlineWeight **123 PASS** có LocalDB test SQL, HqAttendanceDiagnosis **53 PASS**, HqHttpBackup **111 PASS**: tổng **799 assertions**. Gói mẫu đi qua ChatHub/CommunicationService/CommitService với danh tính cache giả lập, xác nhận thứ tự hai target và không mở SQL; không đoán tên thật của mã thẻ mẫu. Chưa thử firmware vật lý.
- Không sửa firmware hoặc cấu hình xưởng, không deploy, không cần SQL migration mới. Rebuild/publish đầy đủ PMSHub từ `D:\RCQ\RCQ-ProductMaterialWorkshop` để áp dụng.

Các kết quả cũ bên dưới giữ làm lịch sử:

- Build Release PMSHub thành công: 0 lỗi, 4.027 cảnh báo mã nền ở lượt rebuild khi Vars thay đổi (không phải build sạch cảnh báo).
- HqStableSwipe: **253 PASS** gồm kiểm tra chờ tổng hoàn tất, không ACK sớm, lỗi tổng không gửi số giả, metadata lỗi, DTO cũ không thêm trường null, lỗi JSON thật và SendData trên ChatHub với client/router giả.
- HqOnlineWeight: **95 PASS**, có SQL Server LocalDB riêng: gây lỗi SQL thật sau COMMIT, xác minh phiếu vẫn một dòng; retry cùng Id không INSERT, tổng phục hồi trả đầy đủ ACK chính xác.
- ScaleOnlineWorkContext: **64 PASS**, SQL LocalDB riêng, giữ luồng ca theo bộ mã.
- HqWeightBackup: **175 PASS** (in-memory); ScaleSharedAttendance: **59 PASS**; ScaleAttendanceProtocol: PASS.
- `git diff --check` sạch. Chưa thử firmware vật lý mới, chưa deploy, không truy cập database xưởng, không thay migration/index hay firmware.

### Bổ sung retry tổng ở WSS

- Bỏ yêu cầu “gửi lại cùng Id” trong thông báo lỗi tổng và lỗi xử lý chưa xác minh; không giả định firmware giữ phiếu online để retry.
- 4 giây là ngân sách chờ của phần đọc tổng sau COMMIT, không phải cam kết thời gian end-to-end; chưa gồm chờ khóa/ghi SQL/mạng. Không chờ vô hạn hoặc giữ transaction ghi khi retry tổng.
- Nếu kết nối mất hoặc người dùng tự quẹt lại với Id mới, WSS vẫn không thể tự nhận biết đó là rổ cũ. Retry query tổng không phải cơ chế chống cùng một rổ hai Id.
- Tổng đồng bộ/async dùng chung biểu thức SQL; kiểm tra parity ngày/ca đêm để không đổi cách cộng kg/rổ.

### Xác minh sau bổ sung retry tổng

- Build Release PMSHub: thành công, 0 lỗi, 390 cảnh báo mã nền.
- HqStableSwipe: **260 PASS**, gồm phục hồi sau hai lỗi đọc, deadline/cancellation, query về muộn không phát thêm ACK hoặc cập nhật UI, không còn thông báo yêu cầu gửi/quẹt lại do lỗi tổng.
- HqOnlineWeight: **98 PASS**, SQL LocalDB riêng: sau một COMMIT, hai query tổng lỗi SQL thật, query thứ ba bằng context mới đọc được tổng đúng; phiếu vẫn một dòng, không cần request firmware thứ hai. Lỗi dai dẳng dừng sau giới hạn thử.
- ScaleOnlineWorkContext: **66 PASS**, SQL LocalDB riêng: tổng async khớp tổng sync cả ngày thường và ca đêm.
- HqWeightBackup: **175 PASS**, ScaleSharedAttendance: **59 PASS**, ScaleAttendanceProtocol: PASS.
- Không deploy, không sửa firmware/SQL xưởng/cấu hình cá nhân; `git diff --check` sạch. Vẫn cần thử firmware vật lý, đặc biệt response đến sau timeout 5 giây.
