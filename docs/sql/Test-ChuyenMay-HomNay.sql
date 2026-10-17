/*
CHỈ CHẠY TRÊN PMS_HQ DÙNG TEST/DEMO, không trên hệ thống đang sản xuất.
Thêm 4 nhân viên có thẻ hợp lệ, không có hoạt động trong 2 ngày gần đây:
  1,2: ca Vào hợp lệ, có thể chuyển.
  3: đã Ra, không xuất hiện trong danh sách ca mở.
  4: Isforget=1, nếu chọn chuyển phải bị từ chối.
Không thay tài khoản, MayCan, TheTu; không gửi WSS/HTTP, không tự chuyển máy.
@KeepData=0 mặc định ROLLBACK. Đặt cả hai biến =1 để lưu vào DB TEST xem Hub.
*/
USE [PMS_HQ];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 5000; -- Chờ khóa tối đa 5 giây; KHÔNG phải giới hạn tổng thời gian query.
DECLARE @ConfirmTestDatabase bit=0;
DECLARE @KeepData bit=0;
DECLARE @Source varchar(50)='7779520';
DECLARE @Target varchar(50)='10244140';
IF @ConfirmTestDatabase<>1 THROW 51000,N'Chọn DB TEST/DEMO, rồi đặt @ConfirmTestDatabase=1.',1;
IF @@TRANCOUNT<>0 THROW 51000,N'Hãy kết thúc transaction đang mở trước.',1;
DECLARE @Now datetime2(7)=CONVERT(datetime2(7),SWITCHOFFSET(SYSDATETIMEOFFSET(),'+07:00'));
DECLARE @Day date=CONVERT(date,@Now), @Start datetime2(7)=DATEADD(minute,-10,@Now);
SET @Start=DATEADD(second,DATEDIFF(second,CONVERT(date,@Start),@Start),CONVERT(datetime2(7),CONVERT(date,@Start)));
IF CONVERT(date,@Start)<>@Day SET @Start=DATEADD(second,10,CONVERT(datetime2(7),@Day));
IF DATEADD(second,90,@Start)>=@Now THROW 51000,N'Đầu ngày chưa đủ thời gian tạo ca mẫu. Thử lại sau 2 phút.',1;
DECLARE @Tag nvarchar(100)=N'TEST_CHUYENMAY_'+CONVERT(nvarchar(8),@Day,112);
DECLARE @MachineName nvarchar(200),@Workshop nvarchar(100);
DECLARE @Cases TABLE (Ca int PRIMARY KEY, Card varchar(50), Employee nvarchar(50), Name nvarchar(200),
    Vao datetime2(7), VaoId varchar(100), Ra datetime2(7), RaId varchar(100));
BEGIN TRY
    SELECT @MachineName=COALESCE(NULLIF(DisplayName,N''),Id),@Workshop=MaXuong
    FROM dbo.MayCan WHERE Id=@Source AND WKv=17;
    IF @MachineName IS NULL OR NULLIF(@Workshop,N'') IS NULL
        THROW 51000,N'Cân nguồn phải là cân HQ đã có mã xưởng.',1;
    IF @Target=@Source OR NOT EXISTS(SELECT 1 FROM dbo.MayCan WHERE Id=@Target AND WKv=17)
        THROW 51000,N'Cần cân đích HQ khác cân nguồn.',1;
    IF EXISTS(SELECT 1 FROM dbo.CheckInOut WHERE ThoiGian>=CONVERT(datetime2(7),@Day) AND GhiChu=@Tag)
        THROW 51000,N'Đã tạo mẫu hôm nay. Không tạo lặp; xem query đối chiếu cuối file.',1;

    -- V2: gom dữ liệu MỘT LẦN, không chạy truy vấn lịch sử cho từng nhân viên.
    -- Chuẩn bị ngoài transaction ghi để không giữ transaction mở khi tìm thẻ.
    DROP TABLE IF EXISTS #CMCardGroups;
    DROP TABLE IF EXISTS #CMRecentCards;
    DROP TABLE IF EXISTS #CMWorkshopPeople;
    DROP TABLE IF EXISTS #CMWeightPeople;
    DROP TABLE IF EXISTS #CMBusyEmployees;
    SELECT LTRIM(RTRIM(MaTheTu)) AS Card,COUNT(DISTINCT MaNhanVien) AS EmployeeCount
    INTO #CMCardGroups FROM dbo.TheTu GROUP BY LTRIM(RTRIM(MaTheTu));
    CREATE CLUSTERED INDEX IX_CMCardGroups ON #CMCardGroups(Card);
    SELECT DISTINCT CASE WHEN CHARINDEX('.',c.MaChamCong)>0
        THEN LEFT(c.MaChamCong,CHARINDEX('.',c.MaChamCong)-1) ELSE c.MaChamCong END AS Card
    INTO #CMRecentCards FROM dbo.CheckInOut c
    WHERE c.ThoiGian>=DATEADD(day,-2,CONVERT(datetime2(7),@Day));
    CREATE CLUSTERED INDEX IX_CMRecentCards ON #CMRecentCards(Card);
    SELECT DISTINCT MaNhanVien,MaTheTu INTO #CMWorkshopPeople
    FROM dbo.ChamCongTaiXuong WHERE ThoiGian>=CONVERT(datetime2(7),@Day);
    SELECT DISTINCT MaNhanVien,TheIdNhanVien INTO #CMWeightPeople
    FROM dbo.HQ_PhieuCan WHERE NgayGio>=CONVERT(datetime2(7),@Day);
    SELECT t.MaNhanVien INTO #CMBusyEmployees
    FROM #CMRecentCards r JOIN dbo.TheTu t ON t.MaTheTu=r.Card
    UNION SELECT MaNhanVien FROM #CMWorkshopPeople
    UNION SELECT t.MaNhanVien FROM #CMWorkshopPeople w JOIN dbo.TheTu t ON t.MaTheTu=w.MaTheTu
    UNION SELECT MaNhanVien FROM #CMWeightPeople
    UNION SELECT t.MaNhanVien FROM #CMWeightPeople w JOIN dbo.TheTu t ON t.MaTheTu=w.TheIdNhanVien;
    CREATE CLUSTERED INDEX IX_CMBusyEmployees ON #CMBusyEmployees(MaNhanVien);

    ;WITH Eligible AS (
        SELECT t.MaTheTu,t.MaNhanVien,n.Name,
            ROW_NUMBER() OVER(PARTITION BY t.MaNhanVien ORDER BY t.MaTheTu) AS CardNo
        FROM dbo.TheTu t JOIN dbo.NhanVienDaiThanh n ON n.MaNhanVien=t.MaNhanVien
        JOIN #CMCardGroups g ON g.Card=t.MaTheTu AND g.EmployeeCount=1
        WHERE n.IsHuman=1 AND n.IsChucNang=0 AND n.IsPhucVu=0
          AND NULLIF(t.MaTheTu,N'') IS NOT NULL AND t.MaTheTu NOT LIKE '%.%'
          AND DATALENGTH(t.MaTheTu)=DATALENGTH(LTRIM(RTRIM(t.MaTheTu)))
          AND NOT EXISTS(SELECT 1 FROM #CMBusyEmployees b WHERE b.MaNhanVien=t.MaNhanVien)
    ), Chosen AS (
        SELECT TOP(4) MaTheTu,MaNhanVien,Name FROM Eligible WHERE CardNo=1 ORDER BY MaTheTu
    )
    INSERT @Cases(Ca,Card,Employee,Name)
    SELECT ROW_NUMBER() OVER(ORDER BY MaTheTu),MaTheTu,MaNhanVien,COALESCE(Name,MaNhanVien) FROM Chosen;
    IF (SELECT COUNT(*) FROM @Cases)<>4
        THROW 51000,N'Cần 4 nhân viên có thẻ không trùng ánh xạ và chưa hoạt động gần đây. Không sửa nhân viên đang làm.',1;
    UPDATE @Cases SET Vao=DATEADD(second,Ca,@Start);
    UPDATE @Cases SET VaoId=@Source+'.'+CONVERT(char(8),Vao,112)+REPLACE(CONVERT(char(8),Vao,108),':',''),
        Ra=CASE WHEN Ca=3 THEN DATEADD(second,60,Vao) END;
    UPDATE @Cases SET RaId=CASE WHEN Ra IS NOT NULL THEN @Source+'.'+CONVERT(char(8),Ra,112)+REPLACE(CONVERT(char(8),Ra,108),':','') END;
    BEGIN TRANSACTION;
    IF EXISTS(SELECT 1 FROM dbo.CheckInOut c JOIN @Cases x ON c.Id=x.VaoId OR c.Id=x.RaId)
        THROW 51000,N'Trùng Id giây tạo mẫu. Thử lại sau, không ghi đè.',1;
    INSERT dbo.CheckInOut(Id,MaChamCong,ThoiGian,MaSoMay,TenMay,CongViec,TrangThai,Isforget,CaDem,VaoId,GhiChu,MaXuong)
    SELECT VaoId,Card,Vao,CONVERT(int,@Source),@MachineName,LEFT(@MachineName,100),N'Vào',
        CASE WHEN Ca=4 THEN 1 ELSE 0 END,0,NULL,@Tag,@Workshop FROM @Cases
    UNION ALL
    SELECT RaId,Card,Ra,CONVERT(int,@Source),@MachineName,LEFT(@MachineName,100),N'Ra',0,0,VaoId,@Tag,@Workshop
    FROM @Cases WHERE Ca=3;
    SELECT @Day AS NgayTest,@Source AS CanNguon,@Target AS CanDichGoiY;
    SELECT Ca,Card AS MaThe,Employee AS MaNhanVien,Name AS TenNhanVien,Vao AS GioVao,
        CASE WHEN Ca IN(1,2) THEN N'Chuyển được; giờ chuyển tự lấy lúc bấm xác nhận'
             WHEN Ca=3 THEN N'Đã Ra; không có trong danh sách ca mở'
             ELSE N'Ca lỗi; chọn chuyển phải bị từ chối' END AS MongDoi
    FROM @Cases ORDER BY Ca;
    SELECT UserName,IsAdmin,IsActive FROM dbo.NguoiDung WHERE UserName='admin1';
    -- admin1 phải có IsAdmin=1 và IsActive không bằng 0. Script KHÔNG tự cấp quyền.
    IF @KeepData=1 BEGIN COMMIT; PRINT N'ĐÃ LƯU mẫu. Mở Hub demo /ScaleAttendanceTransfers, đăng nhập quản trị để thử.'; END
    ELSE BEGIN ROLLBACK; PRINT N'ĐÃ ROLLBACK, chưa lưu. Đặt @KeepData=1 trên DB TEST để dùng giao diện.'; END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK;
    THROW;
END CATCH;
GO

-- CHỈ ĐỌC: chạy riêng đoạn này SAU khi bấm chuyển máy trên Hub.
DECLARE @Today date=CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(),'+07:00'));
DECLARE @Tag nvarchar(100)=N'TEST_CHUYENMAY_'+CONVERT(nvarchar(8),@Today,112);
SELECT c.Id,c.MaChamCong,c.MaSoMay,c.TrangThai,c.ThoiGian,c.VaoId,c.MaXuong,c.NguoiXacNhan,c.LyDo
FROM dbo.CheckInOut c
WHERE c.MaChamCong IN(SELECT MaChamCong FROM dbo.CheckInOut WHERE GhiChu=@Tag AND ThoiGian>=CONVERT(datetime2(7),@Today))
  AND c.ThoiGian>=CONVERT(datetime2(7),@Today)
ORDER BY c.MaChamCong,c.ThoiGian,CASE WHEN c.TrangThai=N'Ra' THEN 0 ELSE 1 END;
-- Chuyển thành công: Ra máy cũ và Vào máy mới có CÙNG ThoiGian lúc bấm;
-- NguoiXacNhan đều là tài khoản thực hiện, ví dụ admin1; LyDo giữ nội dung đã nhập.
