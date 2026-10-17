# PMSHUB treo trên IIS, đăng nhập không vào và máy cân mất kết nối

Ngày kiểm tra: 17/10/2026 (UTC+7). Bắt đầu từ `3f44650` kèm các thay đổi có sẵn; trong lúc rà soát workspace được commit sang `195ef14` ở bên ngoài lượt này. Các nhận định được đối chiếu lại với source hiện có; không nhận các commit đó là thay đổi của lượt rà soát này.

## 1. Mục tiêu

Kiểm tra nguy cơ nghẽn SQL, CPU, RAM và lỗi làm tiến trình thoát khi nhiều cân quẹt thẻ; đề xuất thứ tự xử lý với thay đổi nhỏ, giữ nghiệp vụ.

Thông tin người vận hành xác nhận: PMSHUB chạy qua **IIS**, còn truy cập được trang đăng nhập nhưng treo ở bước đăng nhập, đồng thời máy cân khó kết nối. Với triệu chứng này, ưu tiên điều tra **chờ SQL / cạn kết nối SQL / thiếu luồng xử lý**, chưa thể gọi là tiến trình đã sập. Truy cập được trang đăng nhập cũng không loại trừ tiến trình vừa được IIS khởi động lại.

Chưa có log hoặc quyền truy cập server, số đo CPU/RAM, cấu hình App Pool, execution plan hay trạng thái database lúc treo. Không có cơ sở kết luận database hỏng hoặc RAM thiếu. Các kết luận dưới đây phân biệt lỗi thấy trong source với nguyên nhân production còn cần xác minh.

## 2. Nguyên nhân / vấn đề

### A. Ưu tiên cao nhất theo triệu chứng: đăng nhập cũng chờ SQL đồng bộ

**File, lớp/phương thức nghi ngờ:**

- `PMSHub/Components/Pages/LoginLocal.razor:172–187`: `DoLogin` gọi `LoginLocal` đồng bộ trước khi có `await` của nhánh thành công.
- `Services/LoginService.cs:65–116`: `LoginLocal`, `LoginLocalUser` đọc tài khoản rồi nạp quyền trước khi tạo session thành công.
- `Dao/Repos/HQ/NguoiDung.cs:98–104`: `GetByUserName` gọi `connection.Open()` và Dapper `Query` đồng bộ. Truy vấn này đã dùng tham số.
- `Services/UserService.cs:48–62`, `Dao/Repos/HQ/UserArea.cs:78–85`: `AddUserArea` đọc quyền bằng SQL đồng bộ.

**Nguyên nhân ở code:** luồng đăng nhập phụ thuộc trực tiếp thời gian mở kết nối và chạy hai nhóm truy vấn SQL; chưa có đo thời gian từng bước để biết mắc ở đâu. Khi SQL chậm hoặc pool kết nối hết chỗ, màn hình có thể trông như đứng ngay sau khi bấm đăng nhập. Lỗi SQL hiện bị quy về đăng nhập thất bại, chi tiết chủ yếu in Console.

Đường nhận thẻ và đăng nhập cùng sử dụng tài nguyên của ứng dụng/SQL. Không có bằng chứng khóa trên bảng chấm công trực tiếp khóa bảng tài khoản; cần phân biệt khóa bảng với cạnh tranh CPU/I/O/pool kết nối.

**Yếu tố khởi tạo:** `LoginService` phụ thuộc `IUserService`, `UserService` phụ thuộc singleton `IMayCansService`. Constructor `MayCansService:45–61` gọi `Load`; `Load:2091–2223` đọc máy rồi đọc phiếu gần nhất cho các loại máy. Vì vậy lần khởi tạo dịch vụ đầu tiên sau khi App Pool khởi động lại có thể kéo theo nhiều truy vấn trước khi trang sẵn sàng. Đây là đường khởi tạo, không khẳng định mọi lần bấm đăng nhập đều gọi lại `Load`.

**Cách xác minh:** ghi thời gian bắt đầu/kết thúc riêng cho khởi tạo dịch vụ, mở kết nối tài khoản, query tài khoản, query quyền và tạo session; đối chiếu request SQL và pool tại đúng thời điểm. Log chỉ mã tương quan và thời gian, không mật khẩu, chuỗi kết nối hay thông tin tài khoản đầy đủ.

**Hướng sửa chính xác:** thêm đo thời gian và phân biệt lỗi hạ tầng với sai thông tin đăng nhập trước. Sau khi tìm được điểm chờ, tối ưu truy vấn/index hoặc kết nối tương ứng; chuyển I/O nội bộ sang async nếu giữ được hợp đồng hiện tại. Không bọc toàn bộ đăng nhập trong `Task.Run`, không tăng timeout hàng loạt để che lỗi.

**Rủi ro:** không bỏ kiểm tra tài khoản hoạt động hoặc quyền xưởng. Session/điều hướng Blazor cần kiểm tra riêng khi thay đổi async; không tự đổi cơ chế xác thực trong đợt hiệu năng.

### B. Khóa chung HQ và hai kết nối SQL làm tải đồng thời xếp hàng

**Vị trí:** `Services/CommitService.cs:7473–7577`, `CommitHqLite`, `HqResponse`; `PMSHub/Program.cs:105` đăng ký singleton `ICommitService`.

**Nguyên nhân:** request mở transaction EF và khóa thẻ SQL ở dòng 7544–7547 trước khi chờ `_lockItems` ở dòng 7550. Bên trong khóa chung có đọc lại phiếu, kiểm tra trùng, INSERT đồng bộ và cập nhật giao diện. Các máy/thẻ khác nhau cũng phải chờ vùng khóa này. Dapper ghi phiếu qua kết nối riêng trong khi kết nối EF vẫn giữ transaction.

`dbPMScontext` dùng `Base.Ins.ConnectionString2`; DAO HQ mặc định dùng `Base.Ins.ConnectionString`. Luồng nạp cấu hình `AppViewModel.cs:171,220` gán giá trị thứ hai từ giá trị thứ nhất. Cần xác nhận cấu hình triển khai và pool thực tế, không in giá trị bí mật ra log.

**Ảnh hưởng:** giữ kết nối và luồng xử lý trong lúc xếp hàng; nếu pool chung bị các request chờ chiếm hết, request đang giữ khóa có thể tiếp tục phải chờ kết nối Dapper để hoàn tất. Đây là nguy cơ cạn pool/timeout theo cấu trúc code, chưa phải deadlock production đã được ghi nhận. CPU có thể chưa cao mà ứng dụng vẫn treo.

`MayCansService.AddItem:101–106` gọi subscriber trực tiếp, nên callback chậm/lỗi cũng nằm trên đường lưu. Nếu INSERT đã thành công mà cập nhật giao diện hoặc lấy tổng sau đó lỗi, máy có thể không nhận ACK thành công và gửi lại, làm tăng tải. Transaction EF không rollback INSERT Dapper đã commit riêng.

**Xác minh:** đo chờ/giữ khóa C#, thời gian mở từng kết nối, applock, INSERT, callback và tổng SQL sau INSERT. Thử nhiều máy khác nhau, kèm một SQL cố tình chậm trên DB thử nghiệm.

**Hướng sửa:** giảm phần việc thừa và tách lỗi cập nhật giao diện khỏi kết quả dữ liệu đã lưu; nghiên cứu rút ngắn vùng khóa hoặc thống nhất transaction/kết nối bằng patch riêng. Giữ khóa thẻ, kiểm tra lại Id, chống trùng và thứ tự nghiệp vụ. Không bỏ khóa hoặc đổi isolation ngay.

### C. Tác vụ chốt ca đọc lại lịch sử mỗi phút

**Vị trí:** `Services/ScaleAttendanceDayCloseService.cs:18–50,58–80`, `CloseSql`, `ExecuteAsync`.

CTE lấy các sự kiện trước thời điểm hiện tại, dùng `LEAD` theo thẻ/máy rồi mới lọc dòng cần UPDATE. Không có cận dưới lịch sử. Mức đọc/sắp xếp thực phụ thuộc plan và index, nhưng phạm vi ứng viên tăng theo lịch sử. Mỗi tiến trình Hub đều đăng ký dịch vụ này. `WorkshopAttendanceDayCloseService` có phép quét lịch sử tương tự, chủ yếu khi khởi động/chốt ngày; không chạy mỗi phút trong nhánh thành công.

**Ảnh hưởng dự kiến:** CPU/RAM grant/tempdb/I/O ở SQL Server, chờ khóa với giao dịch khác. Cần đo cả `sqlservr` lẫn tiến trình ứng dụng; không chỉ xem CPU của IIS.

**Xác minh:** execution plan, logical reads, CPU, thời gian, memory grant và blocking của lệnh chốt ca lúc có nhiều quẹt. Đối chiếu với chu kỳ một phút và thời điểm App Pool restart.

**Hướng sửa:** chọn tập ca chưa xử lý rồi tra sự kiện kế tiếp tương đương quy tắc hiện tại; có đường đối soát dữ liệu gửi muộn. Nếu nhiều tiến trình cùng chạy, thiết kế một đầu thực hiện chốt ca sau khi xác nhận mô hình triển khai.

**Rủi ro:** chỉ giới hạn “hôm nay” có thể bỏ ca đêm, ca thiếu Ra cũ và dữ liệu offline. Không sửa bằng cách lọc mất sự kiện Ra trước khi tính thứ tự.

### D. Backup hiện vẫn có SQL cho từng phiếu, thêm chi phí chống trùng mới

**Vị trí:** `Services/HqWeightBackupProcessor.cs:97–200`, `Process`; `Services/HqWeightDuplicatePolicy.cs:10–37`, `FindDuplicates`, `HasDuplicate`.

Lô backup giữ khóa các thẻ, lấy khóa từng Id, preload dữ liệu, rồi xử lý từng phiếu. Ngoài preload, dòng 196 gọi `HasDuplicate`: xét tập EF Local và nếu chưa thấy trùng thì chạy thêm `EXISTS` SQL cho từng phiếu mới. Online cũng gọi kiểm tra này bên trong khóa chung HQ.

Trong nhánh đủ metadata và hợp lệ, N phiếu mới/C thẻ có ít nhất `6 + C + 2N` command đã nhận diện: SELECT máy, 5 preload, C khóa thẻ, N khóa Id, N kiểm tra trùng SQL; chưa tính SaveChanges, transaction, fallback lịch sử và các truy vấn nghiệp vụ bổ sung. Ví dụ 100 phiếu mới/1 thẻ là 207 command theo đường code, **không phải số đo network/benchmark**. Không thể dùng nhận định “backup chỉ có 5 query” cho source hiện tại.

Tập EF Local tăng sau mỗi `Add`. Khi phiếu đều mới/không trùng, kiểm tra lặp tập Local có thể có tổng chi phí bậc hai theo số phiếu. Việc lọc/sắp xếp `preloadedScans` cho từng phiếu cũng lặp. Cửa sổ preload theo min/max của cả lô có thể rất rộng nếu backup trải nhiều ngày.

**Xác minh:** đo số command, thời gian transaction, số phiếu, số scan preload, allocation và thời gian kiểm tra trùng với các lô 1/10/50/100/500 trên DB bản sao, cùng lúc có quẹt online. Không gửi các lô thử vào production.

**Hướng sửa:** nhóm dữ liệu theo khóa nghiệp vụ trong một lô; thiết kế kiểm tra trùng theo tập tương đương với hiện tại; xem index phục vụ máy/thẻ/thời gian; giảm các command khóa khi chứng minh giữ được thứ tự và mã lỗi. Bắt đầu với patch nhỏ, không tự bỏ chống trùng, đổi cửa sổ 30 giây, chia atomicity của lô hoặc ACK trước commit.

### E. Tổng ngày và tổng ca làm tăng chi phí mỗi lượt phản hồi

**Vị trí:** `Dao/Repos/HQ/HQ_PhieuCan.cs:3264–3274`, `GetOnlineTotals`; `Services/ScaleAttendanceProcessor.cs:187–245`, `Success`; `Services/ScaleSharedAttendancePolicy.cs:39–61`, `ShiftWeights`.

Mỗi ACK cân HQ lại tính COUNT/SUM ngày, cả khi gửi lại Id. Check-out và một số nhánh ca chung còn tổng hợp theo ca. Ca chung có nhiều truy vấn con `EXISTS` và chuyển mã máy số sang chuỗi; cần plan SQL thực để đánh giá seek/scan, không suy đoán chỉ từ LINQ. `OpenShifts` cũng được gọi trước khóa máy và trong xử lý ca chung.

**Hướng xử lý:** đo và thử index theo truy vấn thực tế, gồm `ScaleAttendanceScan`, `VaoId`, thẻ/máy/thời gian; tránh chỉ kiểm tra index tổng ngày. Không tạo đồng loạt index suy đoán. Giữ công thức tổng ngày gross và tổng ca net như hiện tại; chưa đưa cache tổng hoặc bảng tổng mới vào phạm vi.

### F. Lỗi timer có thể làm tiến trình thoát — cần sửa nhưng chưa khớp nhất với triệu chứng đăng nhập

**File/phương thức:**

- `PMSHub/Components/Pages/FilletLogView.razor:356–379,526–533`: `BatchTimerOnElapsed`, `Dispose`.
- `PMSHub/Components/Pages/DHLogView.razor:363–386,531–538`: cùng mẫu.

Callback `async void` dừng timer, `await InvokeAsync(...)`, rồi gọi `batchTimer.Start()` trong `finally`. Trong lúc chờ, người dùng rời trang/circuit bị hủy và `Dispose` hủy timer. Callback tiếp tục sẽ gọi Start trên timer đã hủy, phát sinh `ObjectDisposedException` ở ngoài phần `catch`. Vì callback là async void trên luồng timer, ngoại lệ có thể thành lỗi không xử lý và làm tiến trình thoát.

**Đã kiểm chứng cơ chế:** chạy chương trình .NET 8 cô lập chỉ dùng `System.Timers.Timer` và hai tín hiệu điều phối, ép Dispose vào giữa await và finally. Kết quả: `Unhandled exception. System.ObjectDisposedException`, stack qua `Task.ThrowAsync` / ThreadPool; tiến trình con thoát với mã `-532462766`. Không khởi chạy Hub, không kết nối SQL hoặc thiết bị. Đây là tái hiện mẫu lỗi, chưa phải tái hiện trang Razor có renderer/SignalR thực, chưa chứng minh lần treo IIS là do lỗi này.

**Cách xác minh production:** đối chiếu `.NET Runtime` / `Application Error` / ASP.NET Core Module / WAS với thời điểm mở-rời `/FL-log-view` hoặc `/DH-log-view`, stack timer và PID đổi.

**Hướng sửa chính xác:** quản lý trạng thái disposed và vòng đời callback cùng một cơ chế đồng bộ; không restart timer sau Dispose; bao phủ cả lỗi trong finally, có log lỗi bất thường. Có thể thay bằng vòng lặp `PeriodicTimer` có hủy nếu patch vẫn nhỏ và giữ nhịp cập nhật. Chỉ kiểm tra một biến disposed trước Start mà không đồng bộ vẫn còn cửa sổ đua.

**Rủi ro:** dừng nhầm timer sẽ khiến nhật ký không cập nhật; cần kiểm tra điều hướng/đóng tab trong khi đang refresh, kể cả lúc SQL chậm.

### G. Tác vụ trang nhật ký không dừng, tăng tải SQL và giữ RAM theo số lần mở trang

**Vị trí:** `FilletLogView.razor:288–353,526–533`; `DHLogView.razor:297–360,531–538`, `StartMayCanWorker`, `Dispose`.

Mỗi instance trang tạo một `Task.Run` chạy `while (true)`, đọc hai truy vấn SQL rồi chờ 10 giây. `Dispose` chỉ đặt `isRunning = false`; vòng lặp không kiểm tra biến này và không có cancellation. Khi chưa có ngoại lệ làm thoát worker, task giữ tham chiếu tới component/các dịch vụ/collection sau khi rời trang.

N worker còn sống gây khoảng `2N` query mỗi chu kỳ truy vấn cộng thời gian chờ 10 giây, không phải mỗi lần mở trang luôn nhân đôi cố định. Danh sách/queue hiển thị có giới hạn khoảng 100 phần tử; vấn đề chính là số worker/component còn bị giữ, không khẳng định mọi queue tăng vô hạn. `HighThroughputQueue` không có giới hạn trong định nghĩa, nhưng chưa thấy đường enqueue hoạt động ở MayCansService để coi nó là nguồn rò RAM hiện tại.

**Xác minh:** mở/rời hai trang lặp 20–50 lần trên môi trường thử, đếm worker/query và heap sau khi circuit đóng/hết thời gian giữ. Tác vụ còn chạy sau khi Dispose là lỗi vòng đời thấy rõ trong source; lượng RAM production cần heap/số đo.

**Hướng sửa:** lưu task + token hủy theo component, vòng lặp nhận token, hủy khi Dispose và không chạm giao diện sau khi rời trang. Đọc/cập nhật collection phải thống nhất trên dispatcher hoặc snapshot được bảo vệ; hiện worker đọc collection đồng thời timer có thể sửa collection.

### H. Nguy cơ phụ ở cập nhật giao diện và kết nối

- `Devices.razor:1839–1846`: event `async void` await renderer chưa có xử lý lỗi nội bộ; catch tại nơi phát sự kiện không bắt được đầy đủ lỗi sau await. Cần bảo vệ vòng đời component và log lỗi, không chỉ nuốt ngoại lệ.
- `MayCansService.cs:257–350`: debounce reset trên mọi sự kiện có thể trì hoãn giao diện khi có luồng sự kiện liên tục; snapshot rồi `Clear` có thể làm mất update vừa đến. Timer đang flush cũng có thể bị reset rồi bị dispose trong finally, để pending update chờ lần thông báo sau. Đây là vấn đề UI/trạng thái hiển thị, chưa phải bằng chứng mất dữ liệu SQL hay mất kết nối thật.
- `MainViewModel.cs:25,60` giữ DbContext trong singleton; `MayCansService.Load:2095–2097` dùng context đó trước vùng khóa cuối hàm. Các lần Load đồng thời có nguy cơ lỗi dùng chung DbContext. Nên tạo context riêng trong phạm vi Load, giữ public API, sau khi kiểm tra hành vi nạp/cập nhật.
- `Program.cs:88–92`: SignalR đặt keepalive 15 giây và client timeout 30 giây, chưa override `MaximumReceiveMessageSize` hoặc số invocation song song mỗi client. Mặc định lần lượt là 32 KB và 1. Backup lớn có thể bị ngắt kết nối nếu vượt kích thước **một thông điệp đầy đủ**, không phải tổng các gói của nhiều máy cộng lại. Mặc định 1 invocation là giới hạn từng kết nối, không giới hạn toàn server. Kiểm tra log và kích thước thực trước khi đổi; tăng song song có thể làm hỏng thứ tự quẹt.
- `DeviceHeartbeatMonitor` cập nhật trạng thái thiết bị chấm công xưởng qua SQL mỗi 2 giây. Nó không đồng nghĩa cơ chế heartbeat SignalR của cân; trạng thái offline trên UI cần phân biệt với socket đã mất thật.
- Provider log trong `Program.cs:26–28` chỉ Console/Debug. Cần kiểm tra IIS có thu log hay chưa; source không tự chứng minh có log bền vững sau recycle.

## 3. File thay đổi trong lượt này

Chỉ thêm tài liệu này. Không sửa C#, Razor, cấu hình IIS/DB, schema, giao thức hay nghiệp vụ. Các thay đổi source/test và hai tài liệu hiệu năng/SQL chẩn đoán đã có từ trước được giữ nguyên.

## 4. Kế hoạch / phạm vi patch cụ thể

| Thứ tự | Công việc | Điều kiện hoàn thành |
| --- | --- | --- |
| 0 — Làm ngay khi treo | Ghi giờ UTC+7, App Pool/PID và thời điểm bắt đầu tiến trình; thu Event Viewer/IIS/ANCM, trạng thái SQL và counters đồng thời trước khi recycle nếu vận hành cho phép. | Phân biệt process exit/recycle, nghẽn SQL, cạn pool, thiếu luồng hoặc sự cố mạng. |
| 1 — Patch chẩn đoán nhỏ | Đo từng bước login, Route, chờ/giữ khóa HQ, mở kết nối, query và ACK. Thu log có xoay vòng theo cơ chế vận hành hiện có. | Xác định nơi treo và truy vấn gây chờ bằng số đo; không log bí mật/payload đầy đủ. |
| 2 — Patch ổn định vòng đời | Sửa timer và cancellation của hai trang nhật ký; bảo vệ callback Devices. Mỗi nhóm sửa nhỏ có bài tái hiện riêng. | Rời trang không còn worker/query tồn dư; Dispose trong lúc await không có ngoại lệ không xử lý. |
| 3 — Patch SQL theo bằng chứng | Ưu tiên lệnh chốt ca, chống trùng và tổng ca/ngày nếu chúng đứng đầu reads/CPU/wait; thử index cần thiết trên DB bản sao. | Cùng dữ liệu cho kết quả nghiệp vụ tương đương, giảm reads/thời gian thực đo. Chỉ triển khai index sau khi được duyệt riêng. |
| 4 — Patch giảm thời gian giữ tài nguyên | Giảm command backup và phần việc trong khóa HQ, xử lý lỗi UI sau lưu; xem transaction/kết nối bằng thay đổi riêng có kiểm thử đồng thời. | Không nhân đôi, không sai ca/tổng, không ACK dữ liệu chưa lưu; giảm chờ khóa/pool. |
| 5 — Nghiệm thu và pilot | Tải nhiều cân + đăng nhập + backup + chốt ca + điều hướng nhật ký, rồi pilot một nhóm cân. | Đạt tiêu chí bên dưới trước khi mở toàn bộ cân. |

Script đọc SQL đã có: `docs/sql/20261017-KiemTraHieuNangPMSHub-ReadOnly.sql`. Chưa chạy script này trong lượt kiểm tra. Bổ sung quan sát `NguoiDung`, `UserArea`, `ScaleAttendanceScan` theo các truy vấn mới khi thu plan; script cũ chưa bao quát mọi bảng. DMV cần quyền phù hợp và ảnh chụp **lúc đang treo**; ảnh chụp sau restart không đủ kết luận.

Với IIS, cần ghi nhận hosting in-process hay out-of-process để chọn đúng `w3wp`, `dotnet` hoặc `PMSHub` cho counters/stack; không đo nhầm PID. Kiểm tra nguyên nhân recycle, Rapid-Fail Protection, idle timeout, lịch recycle, giới hạn bộ nhớ, WebSocket và số worker processes. Nếu có web garden/nhiều node/overlap recycle, phải xét trạng thái singleton và session trong RAM theo từng tiến trình. Chưa có cấu hình server để khẳng định mục nào đang sai; không tắt các bảo vệ IIS hàng loạt.

## 5. Giải thích cách đo / tiêu chí nghiệm thu

- **SQL:** phân biệt thời gian mở kết nối với thời gian query; lưu command count, elapsed time, logical reads và plan. Thu wait/blocker, transaction mở lâu, memory grant, CPU/I/O/tempdb. `LCK_M_*` gợi ý khóa; `RESOURCE_SEMAPHORE` gợi ý chờ cấp RAM cho query; `THREADPOOL` tại SQL là worker của SQL, khác ThreadPool .NET. Kiểm tra lỗi hết dung lượng data/log nếu log thật có báo, không suy ra database hỏng.
- **Ứng dụng:** CPU, private bytes/working set, GC heap/Gen2/LOH, allocation, ThreadPool queue/thread count, request đang chờ, kết nối SQL đang dùng, số circuit/worker và số reconnect. CPU thấp nhưng queue/chờ khóa tăng vẫn là nghẽn.
- **Tải thử:** cùng snapshot DB, có lịch sử dài; 1/10/50/100 kết nối hoặc số cân thực tế, cùng các mức backup nêu trên. Đây là tham số bài thử, không phải công suất đã cam kết. Tải tăng có kiểm soát trên môi trường thử.
- **Đúng nghiệp vụ:** gửi lại cùng Id; trùng khác Id; lỗi sau commit/mất ACK; cùng thẻ trên nhiều máy; ca chung, đổi thành phẩm, Ra tổng hợp, ca đêm, offline gửi muộn; backup đồng thời online; mở/rời trang nhật ký khi query chậm.
- **Chấp nhận:** không process exit/recycle ngoài dự kiến, không mất/nhân đôi dữ liệu; login và ACK có p50/p95/p99, timeout/reconnect không tăng theo thời gian; p99 ACK dưới timeout firmware thực tế đã xác nhận, không tự lấy 30 giây SignalR làm SLA. RAM sau warm-up/GC và số worker không tăng đều qua các chu kỳ tải/điều hướng. Có dư địa CPU/RAM/SQL theo tải đỉnh đo thực.

## 6. Ghi chú debug / clean code / rủi ro

Ưu tiên hiện tại sau phản hồi người vận hành: **bắt trạng thái SQL/pool/luồng lúc treo đăng nhập**, đồng thời chuẩn bị patch vòng đời timer đã có cơ chế tái hiện. Không nâng RAM/CPU hoặc tăng timeout trước khi biết tài nguyên nào đang chờ. Restart có thể tạm hết triệu chứng vì giải phóng worker/pool/circuit hoặc đổi truy vấn đang chạy; không phải bằng chứng đã sửa nguyên nhân.

Không đề xuất Redis, framework mới, queue ghi nền, bỏ transaction, nới khóa, tắt chốt ca vĩnh viễn, xóa lịch sử hoặc đổi isolation trong lượt này. Không chạy tải/SQL ghi lên production. Khi pilot giữ bản DLL/config gốc và cách quay lại; rollback không xóa dữ liệu nghiệp vụ.

Tham khảo chính thức: [SignalR configuration](https://learn.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-8.0) cho giới hạn/thời gian chờ; [async return types](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types) cho lỗi async void; [ThreadPool starvation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation) cho cách đọc counters và stack. Các nhận định về PMSHUB ở trên xuất phát từ source, không phải kết luận của tài liệu Microsoft.

## 7. Kết quả build / test

Đã chạy trên source hiện có, không chạy ứng dụng PMSHub hoặc hosted service. Sau khi phát hiện các commit bên ngoài, đã build và chạy lại cả 5 bộ kiểm tra; kết quả kiểm cuối tại `195ef14`:

- `dotnet build PMSHub/PMSHub.csproj --no-restore -c Release -v minimal`: thành công, **0 lỗi, 4.063 cảnh báo**, thời gian lượt cuối 29,13 giây. Cảnh báo phần lớn nullable/async/style tồn tại trong source, không tự sửa ngoài phạm vi. Build thành công không chứng minh runtime chịu tải tốt.
- Chạy bằng `dotnet run --project tests/<Tên>/<Tên>.csproj -c Release -p:HubBuildConfiguration=Release --no-restore --verbosity quiet -p:WarningLevel=0`:
  - `HqStableSwipe`: PASS, 79 kiểm tra ở source kiểm cuối (lượt đầu trước các commit bên ngoài có 115 kiểm tra).
  - `HqWeightDuplicate`: PASS, 42 kiểm tra; không truyền `--sql-localdb`.
  - `HqWeightBackup`: PASS, 175 kiểm tra.
  - `ScaleAttendanceProtocol`: PASS, exit code 0; nhánh giao thức/chính sách không truy cập SQL.
  - `ScaleSharedAttendance`: PASS, 57 kiểm tra.
- Probe timer .NET 8 cô lập: tái hiện ngoại lệ không xử lý và tiến trình con thoát như mô tả tại mục F. Chương trình probe đặt ngoài repo trong thư mục tạm, không sửa code sản phẩm.

Các kiểm tra trên dùng dữ liệu RAM/mock hoặc dịch LINQ ra SQL, **không đo SQL thực hoặc firmware thật**. Chưa chạy load test, kiểm thử SQL tích hợp hay xác nhận nguyên nhân từ log server. Chưa triển khai bản sửa nào.
