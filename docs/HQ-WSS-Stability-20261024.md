# Tối ưu đường WSS HQ — 24/10/2026

> Đây là ghi nhận lịch sử tại ngày 24/10. Không dùng mô tả cũ về miễn ca cho thẻ nhóm làm quy trình hiện tại: online nhóm cũng phải theo ca; dữ liệu backup lịch sử giữ tương thích riêng. Xem [rà soát 28/10](HQ-Hub-Review-Cleanup-20261028.md) và tài liệu Attendance Mode/Employee Management để triển khai bản mới.

Giữ nguyên firmware: cân chờ response WSS, không tự cho cân tiếp sau 5s. Không thiết kế
server dựa trên việc firmware giữ/gửi lại phiếu online đang chờ. Không sửa nhánh 123.

## Đã thay đổi

- `CommitHqLiteCore` bỏ lookup Id DAO ở ngoài transaction. `SaveAsync` chỉ tra Id một lần
  dưới khóa SQL; dùng callback chuẩn bị phiếu chỉ cho Id mới. Không đọc lại cấu hình/thẻ
  hiện tại khi xác nhận replay; không dùng DAO để INSERT hoặc tạo đối tượng phiếu.
- Ghi online chuyển sang EF async: mở transaction, lấy applock, đọc Id/lịch sử, kiểm tra ca,
  INSERT và COMMIT. Mỗi context xử lý tuần tự, không chạy song song nhiều query trên cùng context.
- Giữ thứ tự khóa thẻ -> máy -> Id. Gộp hai lệnh khóa máy/Id vào một SQL batch, không bỏ khóa.
  Cùng resource Id với HTTP/Sync; READ COMMITTED giữ nguyên, không thêm NOLOCK.
- Kiểm tra ca online giữ cùng quy tắc nhưng gộp MAX sự kiện/scan thành một query UNION ALL + MAX;
  bỏ query snapshot khi danh sách ca rỗng. Không tra lại nhân viên SQL khi đã có cache cho request.
- Nhóm IsNhom vẫn không cần Vào/Ra, kể cả đồng thời IsHuman. Nhân viên thường vẫn cần ca đúng
  thành phẩm/nguyên liệu/xưởng; ca ngày/đêm, scan đảo thứ tự và lịch sử xưởng không đổi.
- Không cache số tổng hoặc ACK sớm. Ghi COMMIT xong mới tính đầy đủ tổng, vẫn retry query tổng
  tối đa 3 lần trong ngân sách 4s riêng cho tổng. Lỗi tổng không INSERT lại hay yêu cầu đổi Id.

## Chờ và hủy

- Hàng đợi C# từng máy: tối đa 15s chờ được vào lượt xử lý. Hết hạn: không chạy phần ghi,
  trả `Cân đang bận. Chưa lưu phiếu`/NOT_SAVED. Không xóa trạng thái của request owner đang chạy.
  Thời gian này không phải timeout 5s của firmware và không phải delay chủ động.
- Writer async: ngân sách 30s cho kết nối/khóa/chuẩn bị/kiểm tra/lưu; SQL command timeout 15s,
  applock timeout 5s từng khóa. Lệnh xong sớm tiếp tục ngay, không chờ đủ các mốc này.
- Hết hạn/hủy/lỗi COMMIT chưa xác minh: báo UNKNOWN theo quy tắc cũ, không khẳng định chưa lưu,
  không tự retry INSERT. Transaction dispose/rollback giải phóng khóa; không tác vụ ghi nền sau timeout.
- Không dùng ConnectionAborted để hủy giữa chừng bước ghi SQL. Kết nối cân mất không được dùng
  làm bằng chứng phiếu chưa lưu. Khi gửi ReceiveData, dùng token kết nối để không cố gửi tới kết nối đã mất;
  nếu đã ngắt kết nối thì không thử gửi thêm error lặp lại.
- Không hứa toàn lượt dưới 5s hoặc luôn dưới 30s: chờ queue, rollback/driver phản hồi hủy,
  tính tổng, cập nhật UI và gửi SETNHANVIEN/ReceiveData là các bước khác nhau.
  Không có giải pháp WSS đơn lẻ bảo đảm firmware đã nhận response khi mạng đứt sau COMMIT.

## Đo tại server

- Lượt xử lý HQ >=2s ghi `[HQ ONLINE SLOW]` với QueueMs, SqlMs, TotalsMs, IdentityMs, TotalMs.
  Chỉ log lượt chậm; không tạo file/ghi database cho từng lượt cân hoặc log toàn payload.
- ChatHub ghi `[HQ WSS SLOW]` khi route+send >=2s: RouteMs và SendMs. Send hoàn tất ở SignalR
  không chứng minh firmware đã xử lý; đây là thời gian phía server, không phải round trip vật lý.
- Giữ cấu hình SQL/port/URL/server và min threadpool nền, không tự tăng pool/threads/RAM theo phỏng đoán.
  Kiểm tra log ứng dụng và kết quả script SQL read-only đã giao để quyết định index/tuning tiếp.

## Phạm vi chưa thay

- HTTP backup vừa sửa vẫn cho thiếu ST, writer/transaction riêng như bản vá HTTP.
- Processor Sync/Resync và luật offline 30s không đổi. Tổng legacy sau Sync vẫn cần rà riêng
  (theo máy/NOLOCK). Không lấy kết quả WSS mới làm chứng nhận toàn bộ Sync/báo cáo.
- Tác vụ chốt ca nền, cache đăng nhập/thẻ/máy/tên thành phẩm, notifier UI và web báo cáo chưa được
  viết lại trong patch này. Không gộp các thay đổi nghiệp vụ khác vào tối ưu đường ghi online.
- Không tự cập nhật SQL xưởng, không migration mới, không thay firmware, không deploy/commit/push.

## Kiểm chứng

- Interceptor SQL LocalDB: đường writer sync 5 command, async 4 command (gộp khóa); không gồm
  begin/commit transaction. Trong writer async chỉ một SELECT Id/một connection, không command đồng bộ.
  Ngoài ra call site đã bỏ một SELECT DAO; query MAX kiểm tra thứ tự gộp từ 2 xuống 1.
  Đây là số command trên fixture, không phải đo phần trăm nhanh hơn hay p95 server xưởng.
- SQL test: 8 async writer cùng Id chỉ 1 INSERT; xung đột/replay; hủy trước INSERT rollback,
  hết hạn không giữ kẹt khóa; SQL từ chối ghi không trả thành công; US và ca sai không ghi.
- Test action thực `CommitLite`: INSERT + đủ tổng, không lookup DAO thừa, replay sau mất cache/cấu hình
  vẫn chính xác và không thêm UI. Test HTTP và async WSS tranh cùng Id chỉ 1 dòng.
- Async/sync policy so sánh trên SQL cho bốn bộ mã, gồm ca đúng hợp lệ và scan tương lai ở máy khác.
- Test transport: kết nối mất vẫn hoàn tất router rồi hủy gửi bằng token; không gửi lỗi lặp vô hạn.

Build Release đầy đủ từ `D:\RCQ\RCQ-ProductMaterialWorkshop` khi triển khai, giữ cấu hình server;
không chỉ chép một DLL, không dùng hai phiên bản nghiệp vụ cũ/mới cùng ghi DB. Chưa đo tải hoặc
nghiệm thu firmware vật lý. Xem checklist triển khai và các giới hạn Sync/báo cáo trước khi đưa toàn xưởng vào dùng.

## Kết quả xác minh cuối

- PMSHub Release: build 0 lỗi, 181 cảnh báo ở lượt build xác minh cuối (warning mã nền còn tồn tại).
- HqStableSwipe: **257 PASS**, có transport hủy gửi khi connection mất, không ngắt router giữa chừng.
- HqOnlineWeight: **112 PASS**, SQL LocalDB riêng, gồm writer async/queue deadline/SQL failure và đếm command.
- ScaleOnlineWorkContext: **71 PASS**, SQL LocalDB riêng, gồm parity async/sync chính sách ca.
- HqHttpBackup: **111 PASS**, SQL LocalDB riêng, gồm CommitLite thật/đủ tổng/replay mất cache và HTTP tranh Id với async WSS.
- HqWeightBackup: **175 PASS** (in-memory); HqWeightDuplicate: **42 PASS**;
  ScaleSharedAttendance: **59 PASS**; ScaleAttendanceProtocol: PASS.
- Không truy cập database sản xuất; database test dùng GUID và được dọn sau test. git diff --check sạch.
