# Đối chiếu bản 123 và chuẩn bị server xưởng — 23/10/2026

> Cập nhật 24/10: lỗi HTTP offline thiếu Stated đã có bản vá, xem
> `HQ-HTTP-Backup-20261024.md`. Các nhận định guard ST dưới đây mô tả nền trước bản vá;
> HTTP mới cho thiếu Stated, online LITE vẫn bắt ST. Kiểm tra lịch sử xưởng/Sync/báo cáo vẫn cần.

> Bổ sung cùng ngày: writer WSS đã chuyển async, bỏ lookup DAO thừa, thêm queue/write deadline
> và log chậm. Firmware chờ response, không tự mở lượt sau 5s. Xem `HQ-WSS-Stability-20261024.md`.

## Kết luận

Chưa nên thay toàn bộ bản xưởng ngay. Luồng online mới đã qua test cô lập/SQL LocalDB,
nhưng phải xác minh SQL thật, Sync firmware và web báo cáo trước khi đưa toàn xưởng vào dùng.
Không chạy hai bản cũ/mới cùng ghi PMS_HQ để thử A/B.

- Nhánh `123` trên máy này: `8778eee`. Đây là đối chiếu source theo thông tin người dùng,
  chưa xác minh hash DLL của bản Release thực sự trên server.
- Bản sửa: `D:\RCQ\RCQ-ProductMaterialWorkshop`, nhánh
  `feature/hq-shift-product-material-workshop`, nền `c2988f9`.
- Nhiều sửa đổi và file mới vẫn **chưa commit**. Publish chỉ theo tên nhánh/commit sẽ bỏ sót
  trạng thái bản làm việc nếu không chốt đầy đủ source. Không apply stash lên nhánh 123.
- Không thay firmware, không chạy Hub hay lệnh trên SQL xưởng trong lượt kiểm tra này.
- Cấu hình cá nhân đang sửa được giữ nguyên, không coi là cấu hình deploy xưởng.

## Đã thay đổi gì so với 123

### 1. Phần online mới trong bản làm việc

- Khóa ca bằng **mã thành phẩm + mã nguyên liệu + mã xưởng**. Máy/lô/size không phải khóa ca.
- Cùng bộ mã: cân ở máy HQ nào cùng xưởng cũng được; Vào lặp giữ ca/giờ gốc.
- Khác một mã: phải Vào; đóng ca cũ/mở ca mới cùng transaction. Ra ở cân bất kỳ.
- Ca ngày không kéo sang hôm sau; ca đêm qua ngày trong giới hạn cấu hình (hiện mặc định 16h).
- `CheckInOut` giữ ca tính giờ; `ScaleAttendanceScan` giữ máy quẹt thực tế và snapshot bộ mã.
- Cập nhật 26/10: thẻ nhóm `IsNhom=1` phải theo ca Vào/Ra như nhân viên để ghi phiếu online, kể cả đồng thời `IsHuman=1`. Sau Ra, phiếu mới bị chặn tới khi Vào lại; backup offline lịch sử giữ cách đối chiếu cũ.
- Tổng ngày gom mọi máy cùng bộ mã/xưởng; tổng nhân viên theo MaNhanVien, tổng kết ca theo
  thẻ/ca tương ứng. Cộng TrongLuong trực tiếp, không trừ bì lần hai; chỉ phiếu sản lượng Status=1.
- LITE chỉ nhận ST/kg dương hợp lệ; US/thiếu trạng thái không ghi, không giữ thẻ chờ tự lưu.
- ST/0 kg chỉ nhận diện qua cache + SETNHANVIEN, báo không lưu; không tự tạo ca.
- Tuần tự từng máy ở WSS; khóa thẻ/máy/Id trong SQL. Kiểm tra ca và INSERT/COMMIT cùng kết nối.
- Chống ghi lại cùng Id; cùng Id khác nội dung báo lỗi, không ghi đè. Không cooldown 30s online.
  Hai Id mới có thể là hai rổ thật: khóa quẹt cùng một rổ vẫn do nhúng.
- Chỉ báo hoàn thành khi COMMIT và có đủ bốn tổng. Lỗi tổng: WSS chỉ thử lại query tổng tối đa
  3 lần trong 4 giây riêng cho phần tổng, không INSERT lại hoặc yêu cầu firmware đổi Id.
- Hết hạn tổng: `Đã lưu phiếu. Chưa có tổng`, SaveStatus=SAVED. Lỗi hệ thống chưa xác minh
  lưu: UNKNOWN. Không gửi tổng 0 giả hoặc yêu cầu quẹt lại vì lỗi tổng.
- Lỗi có Id/thẻ/giờ/máy khi đọc được an toàn; SETNHANVIEN vẫn đúng ba arguments,
  ReceiveData vẫn giao thức JSON cũ. Rút ngắn thông báo mới, không sửa thông báo cũ ngoài phạm vi.
- Đã gỡ delay thử 9 giây. Không có delay test đang bật.

### 2. Thay đổi kế thừa từ nền c2988f9 (cũng đi theo nếu deploy toàn bản)

Không nhầm “không sửa offline ở lần sửa WSS cuối” thành “offline giống hệt 123”.

- Có thêm ca chung/ScaleAttendanceScan và điều chỉnh đường đồng bộ chấm công HQ.
- Backup preload theo lô, transaction ReadCommitted, khóa thẻ/Id, đối chiếu lịch sử;
  chính sách chống phiếu giống nhau dưới 30 giây vẫn còn trên đường offline/Sync.
- HTTP Post/PostSilent yêu cầu Stated=ST và kg dương. Firmware offline thiếu Stated có thể bị từ chối.
- Chuyển máy quản trị có tìm thẻ/tên, phân trang, chọn nhiều người chung đích, giờ máy chủ,
  tài khoản/lý do; ca online mới đủ snapshot bị chặn chuyển kiểu cũ.
- Cache đăng nhập/thẻ/máy và sửa đồng thời danh sách quyền; tăng timeout SQL mặc định,
  tăng min threadpool 100. Đây không phải bằng chứng đã nhanh hơn trên server 8 GB.
- Tổng legacy GetOnlineTotals có NOLOCK; online mới không dùng query này, nhưng tổng hiển thị
  sau backup vẫn gọi nó và vẫn theo máy. Không xem tổng đó là đối soát chính xác của ca mới.

## Điểm còn phải xử lý/kiểm chứng

1. **Báo cáo riêng chưa theo ca mới.**
   `D:\ChamCongSanXuat\ChamCongSanXuat\ChamCongRauCuQua\Services\ChamCongSanXuatDataService.cs`
   vẫn lọc phiếu `p.MayCan = c.MaSoMay` (khoảng dòng 222), không dùng snapshot bộ mã.
   Có thể thiếu sản lượng cân trên máy khác. Chức năng sửa/xóa ca cũng cần kiểm thử liên kết
   ScaleAttendanceScan trước khi cho quản trị sửa ca mới; lượt này chưa sửa web báo cáo.
2. **Offline/Sync chưa hỗ trợ trọn quy trình mới.** Kiểm tra payload thật và nạp một lô mẫu
   trên DB sao chép: ST, Id gốc/giờ, lô/size/thành phẩm/nguyên liệu/xưởng/thẻ, Vào/Ra trước/sau đổi ca.
   Không bù dữ liệu cũ bằng cấu hình hiện tại; không gửi lại backlog thật để thử vào production.
3. **Chưa có benchmark xưởng.** Query tổng vẫn cộng theo ngày/bộ mã mỗi phiếu; còn SQL đồng bộ,
   lookup Id hai bước, chờ khóa và hàng đợi từng máy. Không bỏ bước lookup có khóa để giảm query.
   Ngân sách 4s không gồm chờ hàng đợi/khóa/ghi SQL/mạng, không bảo đảm ACK dưới 5s.
4. **Cache danh mục.** Máy/thẻ chấm công có TTL khoảng 60s, tên thành phẩm cache không có TTL;
   vừa đổi xưởng/gán thẻ/tên có thể chưa phản ánh tức thì. Cần thử đổi cấu hình lúc đang kết nối.
5. **Nhiều Hub/tác vụ nền.** Chốt ca đang xét 7 ngày mỗi phút, mỗi tiến trình đăng ký dịch vụ;
   đo I/O/wait nếu nhiều Hub dùng chung DB. RCSI không loại bỏ khóa ghi/applock hay mọi deadlock.
6. Firmware phải ghép ReceiveData theo Id/thẻ khi ACK đến sau timeout. SETNHANVIEN không có Id,
   chỉ để hiển thị, không phải bằng chứng phiếu đã lưu. Parsing hai message chung frame là chuyện khác.

## SQL cần chạy

### A. Chạy NGAY được để kiểm tra, không đổi dữ liệu

Mở `docs/sql/20261023-KiemTraTrienKhaiHQ-ReadOnly.sql` trên server xưởng, xác nhận PMS_HQ,
chạy nguyên file và gửi tất cả Results + Messages. Mặc định chỉ metadata/DMV, không cộng toàn ngày.

- Có version/edition/giờ server/RCSI, cột và kiểu dữ liệu, key/INCLUDE, số dòng ước tính,
  usage index, stats, request đang chờ. Thiếu quyền DMV không đồng nghĩa thiếu schema.
- Cuối file có phần đo tổng tắt sẵn. Chỉ bật sau khi điền đúng bộ mã/nhân viên/ngày thực tế,
  giờ ít tải. Đo query này không thay thế đo WSS end-to-end hay p95/p99.
- Gửi plan thực tế nếu có quyền. Không xóa cache SQL, shrink DB, rebuild toàn bộ hoặc tạo hàng loạt index.
- Muốn lấy thống kê Query Store đã có: script `20261017-KiemTraHieuNangPMSHub-ReadOnly.sql`;
  không tự bật Query Store. Số usage reset sau restart, không xóa index chỉ vì usage=0.

### B. Chỉ chạy khi chuẩn bị nâng cấp, sau backup và dừng các Hub ghi chung DB

1. Nếu thiếu CheckInOut.MaXuong hoặc LichSuXuongMayCan: xem
   `20261016-XuongLichSuChamCong.sql`. Script này có thêm mốc lịch sử hiện tại, không phải chỉ DDL;
   không chạy lại để “đo tốc độ”, không đoán xưởng cho dữ liệu trước mốc triển khai.
2. Chạy `20261018-HqOnlineWorkContext.sql`: tạo ScaleAttendanceScan nếu chưa có, thêm
   MaLoaiNguyenLieu/MaXuong nếu thiếu, thêm các index ca/thẻ/thời gian/bộ mã. Không sửa phiếu cũ,
   không đổi RCSI. Nếu bảng tự tạo sai cấu trúc, script không tự sửa hết: kiểm tra phần A trước.
3. Không cần chạy riêng `20261017-ScaleAttendanceScan.sql` nếu đã chạy bản 20261018 đúng cấu trúc.
4. Các index cũ/RCSI người dùng cho biết đã bật: xác minh rồi giữ nguyên. Không cần chạy lại script
   `20261017-ToiUuHieuNangHQPhieuCan.sql` vì có `ROLLBACK IMMEDIATE` khi RCSI đang tắt.
5. Index cùng chức năng có thể khác tên. Đối chiếu key/INCLUDE trước khi thêm bản trùng;
   script nâng cấp chỉ kiểm tra theo tên. Tạo index tốn I/O/log và có thể khóa bảng; chạy giờ bảo trì.

Riêng các trường phản hồi SaveStatus/Id/tên và retry tổng **không cần thêm bảng/cột SQL**.

## Trình tự triển khai đề nghị

1. Nhận kết quả A, xác minh payload Sync/firmware và sửa/kiểm thử báo cáo nếu cần dùng đối soát ca mới.
2. Thử trên bản sao PMS_HQ, hai cân test; không trỏ solution đang F5 vào DB sản xuất.
3. Chốt đầy đủ source bản sửa (kể cả file mới), ghi nhận commit/package; giữ nguyên bản release 123,
   cấu hình server và backup DB. Không chỉ chép Services.dll: Models/Vars/Services/Hub đều đổi.
4. Chọn lúc giao ca/không có ca mở; xử lý giờ Ra thực tế trước bảo trì. Ca 123 thiếu snapshot sẽ
   bị báo Ra rồi Vào, không tự đổi dữ liệu lịch sử để ép qua kiểm tra.
5. Dừng tất cả Hub cùng ghi PMS_HQ; backup DB, chạy B theo kết quả A; xác minh lại schema/index.
6. Publish Release đầy đủ từ thư mục bản sửa sang thư mục MỚI. Giữ cấu hình kết nối/URL/port/quyền
   server; không chép cấu hình máy phát triển đè lên. PMSHub target net8.0: xác minh runtime/hosting
   phù hợp với kiểu publish trước khi đổi site. Không nâng SQL/RCSI hay tăng RAM theo phỏng đoán.
7. Khởi động duy nhất phiên bản mới trên các Hub cần dùng; kiểm tra log, kết nối, chọn đúng
   lô/size/thành phẩm/nguyên liệu/xưởng ở Hub. Đối chiếu giờ cân và giờ server UTC+7.
8. Test một lượt từng tình huống dưới đây, kiểm tra SQL và HMI, rồi tăng số cân. Không dùng hai
   bản nghiệp vụ cũ/mới trên cùng DB để canary. Mất kết nối/thử Sync trên DB sao chép trước.
9. Rollback: dừng bản mới, giữ log/dữ liệu mới và xác định ca đã mở theo snapshot. Không tự restore
   backup DB lên production làm mất phiếu phát sinh; không xóa ScaleAttendanceScan để quay lui.
   Nếu đã phát sinh ca/phiếu mới cần đối soát trước khi cho 123 ghi tiếp. Extra schema không đồng nghĩa
   nghiệp vụ quay lui an toàn. Chưa chốt xử lý ca/Sync/báo cáo thì tiếp tục giữ 123 ở xưởng.

### Test nghiệm thu

- Nhân viên Vào A, cân B cùng bộ mã; Vào B lặp không đổi giờ gốc; Ra C.
- Chỉ đổi thành phẩm, chỉ đổi nguyên liệu, chỉ đổi xưởng: chưa Vào bị chặn; Vào mới đóng/mở đúng một ca.
- Nhóm IsNhom=1, kể cả IsHuman=1: cân không bắt ca. Nhân viên thường chưa Vào vẫn bị chặn.
- US không phiếu; ST/0 hiện đúng tên nhưng không phiếu/ca; ST/kg dương lưu đúng một lần.
- Cùng Id gửi lại không tăng rổ/ký; Id khác là phiếu khác, firmware phải khóa rổ thật.
- Tổng gom nhiều máy đúng mã/nhân viên; không trừ bì hai lần. Thử ngày mới/ca đêm trên DB test.
- Response chậm >5s không làm ACK cũ kết thúc lượt mới; lỗi tổng báo đã lưu, không quẹt lại rổ đó.
- Sau nạp offline/Sync không đổi xưởng/thành phẩm lịch sử, không mất phiếu vì guard/cooldown.
- Báo cáo và chức năng sửa ca không làm mất/đứt liên kết snapshot.

## Kết quả kiểm tra lại ngày 23/10/2026

- Build Release PMSHub incremental: 0 lỗi/0 cảnh báo phát sinh; lượt compile đầy đủ trước còn
  cảnh báo mã nền (390 ở lượt xác minh trước), không tuyên bố toàn solution sạch warning.
- HqStableSwipe: 260 PASS, giả lập/cache/transport, không SQL xưởng.
- HqOnlineWeight: 98 PASS, SQL LocalDB riêng, INSERT/COMMIT/concurrency/retry tổng.
- ScaleOnlineWorkContext: 66 PASS, SQL LocalDB riêng, ca/bộ mã/tổng/migration chạy lại.
- HqWeightBackup: 175 PASS (in-memory); ScaleSharedAttendance: 59 PASS; ScaleAttendanceProtocol: PASS.
- Hai script chẩn đoán chỉ đọc chạy được trên LocalDB master (kiểm tra cả trường hợp chưa có schema).
  Script mới đã thử phần đo tổng trên DB tạm riêng: 2 phiếu/15 kg, đúng tổng và không đổi số dòng;
  database test đã dọn. Kết quả tốc độ fixture nhỏ không đại diện hiệu năng xưởng.
- Chưa benchmark server thật, thử firmware vật lý hoặc deploy; số test không phải cam kết độ trễ.
- Lượt này chỉ thêm tài liệu/checklist và script chẩn đoán chỉ đọc; sửa cú pháp khôi phục
  LOCK_TIMEOUT trong script chẩn đoán cũ. Không tối ưu tiếp mã nghiệp vụ.
