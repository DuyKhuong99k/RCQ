USE [PMS_HQ];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Chi chay tren DB TEST. Mac dinh ROLLBACK: xem ket qua nhung khong giu du lieu.
-- Doi @KeepData = 1 neu muon luu tren DB TEST de xem web bao cao.
-- Khong phai goi WSS that: ca lich su khong dung de gia lap quet hom nay.
IF @@TRANCOUNT <> 0
    THROW 51000, N'Hãy kết thúc transaction đang mở trước khi chạy script thử.', 1;

DECLARE @KeepData bit = 0;
DECLARE @Day date = '20261002'; -- 02/10/2026; doi neu ngay nay da co du lieu.
DECLARE @Machine varchar(50) = '7779520';
DECLARE @Card varchar(50) = '0005610611';
DECLARE @Employee varchar(50), @Workshop varchar(50), @MachineName nvarchar(200);
DECLARE @Lot varchar(50) = 'L01', @Size varchar(50) = '20X20', @Material varchar(50) = 'DuaLUOI';
DECLARE @Tag nvarchar(100) = N'TEST_TENCONGVIEC_THEOCA';
DECLARE @Base datetime2(7) = CONVERT(datetime2(7), @Day);
DECLARE @Cases TABLE (Ca int PRIMARY KEY, Vao datetime2(7), Ra datetime2(7),
    Expected nvarchar(200), VaoId varchar(100), RaId varchar(100));
DECLARE @Weights TABLE (Ca int, AtTime datetime2(7), Product varchar(50), Kg decimal(18,3));

INSERT INTO @Cases (Ca, Vao, Ra, Expected)
VALUES (1, DATEADD(minute,480,@Base), DATEADD(minute,510,@Base), N'LỘT VỎ'),
       (2, DATEADD(minute,540,@Base), DATEADD(minute,570,@Base), N'BÀO VỎ'),
       (3, DATEADD(minute,600,@Base), DATEADD(minute,630,@Base), N'LỘT VỎ, BÀO VỎ');
UPDATE @Cases SET
    VaoId = @Machine + '.' + CONVERT(char(8),Vao,112) + REPLACE(CONVERT(char(8),Vao,108),':',''),
    RaId = @Machine + '.' + CONVERT(char(8),Ra,112) + REPLACE(CONVERT(char(8),Ra,108),':','');
INSERT INTO @Weights (Ca, AtTime, Product, Kg)
VALUES (1, DATEADD(minute,485,@Base), 'LV', 7.750),
       (1, DATEADD(minute,495,@Base), 'LV', 6.950),
       (2, DATEADD(minute,545,@Base), 'BV', 6.250),
       (2, DATEADD(minute,555,@Base), 'BV', 8.350),
       (3, DATEADD(minute,605,@Base), 'LV', 5.250),
       (3, DATEADD(minute,615,@Base), 'BV', 4.750);

BEGIN TRY
    BEGIN TRANSACTION;
    SELECT @MachineName=DisplayName, @Workshop=MaXuong FROM dbo.MayCan WHERE Id=@Machine;
    SELECT @Employee=MaNhanVien FROM dbo.TheTu WHERE MaTheTu=@Card;
    IF @MachineName IS NULL OR TRY_CONVERT(int,@Machine) IS NULL OR @Workshop IS NULL
        THROW 51000, N'Không tìm được cân/xưởng hợp lệ. Kiểm tra @Machine.', 1;
    IF NULLIF(LTRIM(RTRIM(@Employee)), '') IS NULL
        THROW 51000, N'Thẻ chưa có mã nhân viên. Chọn @Card đã đăng ký trong TheTu.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.HQ_ThanhPham WHERE Id='LV' AND NULLIF(LTRIM(RTRIM(Ten)),N'') IS NOT NULL)
       OR NOT EXISTS (SELECT 1 FROM dbo.HQ_ThanhPham WHERE Id='BV' AND NULLIF(LTRIM(RTRIM(Ten)),N'') IS NOT NULL)
        THROW 51000, N'Cần thành phẩm LV và BV có tên trong HQ_ThanhPham.', 1;

    -- Uu tien ma lo/size/nguyen lieu da co cua can, tranh dung ma vi du khong ton tai.
    SELECT TOP (1) @Lot=MaLo, @Size=MaSize, @Material=MaLoaiNguyenLieu
    FROM dbo.HQ_PhieuCan WHERE MayCan=@Machine AND Status=1 AND ChiSanLuong=1
    ORDER BY NgayGio DESC, Id DESC;

    -- Khong xoa/ghi de du lieu cu. Dung ngay khac neu the/can nay da co lich su.
    IF EXISTS (SELECT 1 FROM dbo.CheckInOut
        WHERE MaChamCong=@Card AND ThoiGian>=@Base AND ThoiGian<DATEADD(day,1,@Base))
       OR EXISTS (SELECT 1 FROM dbo.HQ_PhieuCan
        WHERE MayCan=@Machine AND TheIdNhanVien=@Card AND NgayGio>=@Base AND NgayGio<DATEADD(day,1,@Base))
        THROW 51000, N'Thẻ/cân đã có dữ liệu ngày này. Đổi @Day; script không xóa dữ liệu cũ.', 1;
    IF EXISTS (SELECT 1 FROM dbo.CheckInOut p JOIN @Cases c ON p.Id=c.VaoId OR p.Id=c.RaId)
        THROW 51000, N'Trùng Id chấm công với dữ liệu khác. Đổi ngày/cân, không ghi đè.', 1;
    IF EXISTS (SELECT 1 FROM dbo.HQ_PhieuCan p JOIN @Weights w
        ON p.Id=@Machine+'.'+CONVERT(char(8),w.AtTime,112)+REPLACE(CONVERT(char(8),w.AtTime,108),':',''))
        THROW 51000, N'Trùng Id phiếu cân với dữ liệu khác. Đổi ngày/cân, không ghi đè.', 1;

    INSERT INTO dbo.CheckInOut
        (Id,MaChamCong,ThoiGian,MaSoMay,TenMay,CongViec,TrangThai,Isforget,CaDem,VaoId,GhiChu)
    SELECT VaoId,@Card,Vao,CONVERT(int,@Machine),@MachineName,LEFT(@MachineName,100),N'Vào',0,0,NULL,@Tag
    FROM @Cases;
    INSERT INTO dbo.CheckInOut
        (Id,MaChamCong,ThoiGian,MaSoMay,TenMay,CongViec,TrangThai,Isforget,CaDem,VaoId,GhiChu)
    SELECT RaId,@Card,Ra,CONVERT(int,@Machine),@MachineName,LEFT(@MachineName,100),N'Ra',0,0,VaoId,@Tag
    FROM @Cases;
    INSERT INTO dbo.HQ_PhieuCan
        (Id,STT,Ngay,NgayGio,MayCan,MaLo,MaSize,MaThanhPham,MaLoaiNguyenLieu,
         MaNhanVien,MaNhanVienPhucVu,MaNhanVienBanKiem,TrongLuong,TrongLuongTare,
         ChiSanLuong,Status,TheId,TheIdNhanVien,GhiChu,MaXuong)
    SELECT @Machine+'.'+CONVERT(char(8),AtTime,112)+REPLACE(CONVERT(char(8),AtTime,108),':',''),
        0,@Day,AtTime,@Machine,@Lot,@Size,Product,@Material,
        @Employee,'','',Kg,0,1,1,@Card,@Card,@Tag,@Workshop
    FROM @Weights;

    -- Doi chieu tung ca Ra -> VaoId, loc phieu cung may/the/khoang thoi gian nhu Hub.
    ;WITH ProductNames AS (
        SELECT c.Ca,LTRIM(RTRIM(tp.Ten)) AS Ten,MIN(p.NgayGio) AS FirstTime
        FROM @Cases c
        JOIN dbo.CheckInOut r ON r.Id=c.RaId
        JOIN dbo.CheckInOut v ON v.Id=r.VaoId
        JOIN dbo.HQ_PhieuCan p ON p.MayCan=CONVERT(varchar(50),v.MaSoMay)
            AND p.TheIdNhanVien=v.MaChamCong AND p.NgayGio>=v.ThoiGian AND p.NgayGio<=r.ThoiGian
            AND p.ChiSanLuong=1 AND p.Status=1
        JOIN dbo.HQ_ThanhPham tp ON tp.Id=p.MaThanhPham
        WHERE NULLIF(LTRIM(RTRIM(tp.Ten)),N'') IS NOT NULL
        GROUP BY c.Ca,LTRIM(RTRIM(tp.Ten))
    )
    SELECT c.Ca,c.Vao,c.Ra,c.Expected AS TenCongViecMongDoi,
        j.TenCongViec,t.SoPhieu,t.TongKg,
        CASE WHEN j.TenCongViec=c.Expected THEN N'ĐẠT' ELSE N'KIỂM TRA TÊN DANH MỤC' END AS KetQua
    FROM @Cases c
    OUTER APPLY (SELECT STRING_AGG(CONVERT(nvarchar(max),n.Ten),N', ')
        WITHIN GROUP (ORDER BY n.FirstTime,n.Ten) AS TenCongViec FROM ProductNames n WHERE n.Ca=c.Ca) j
    OUTER APPLY (SELECT COUNT(*) AS SoPhieu,SUM(p.TrongLuong-p.TrongLuongTare) AS TongKg
        FROM dbo.HQ_PhieuCan p WHERE p.MayCan=@Machine AND p.TheIdNhanVien=@Card
        AND p.NgayGio>=c.Vao AND p.NgayGio<=c.Ra AND p.ChiSanLuong=1 AND p.Status=1) t
    ORDER BY c.Ca;

    IF @KeepData=1
    BEGIN
        COMMIT TRANSACTION;
        PRINT N'Đã LƯU 6 lượt quẹt và 6 phiếu cân mẫu. Xem web theo ngày @Day.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT N'Đã ROLLBACK: chỉ xem kết quả thử, không giữ dữ liệu mẫu.';
    END;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
