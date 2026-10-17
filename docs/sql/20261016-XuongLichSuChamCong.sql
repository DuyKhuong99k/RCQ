-- Chạy thủ công trên PMS_HQ, SAU KHI BACKUP, TRƯỚC KHI cài Hub/Web mới.
-- Dừng thao tác chuyển xưởng trong lúc triển khai; nâng cấp TẤT CẢ Hub dùng DB này.
-- Không backfill ca cũ bằng MaXuong hiện tại. Không tự chạy script từ ứng dụng.
USE [PMS_HQ];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH(N'dbo.CheckInOut', N'MaXuong') IS NULL
    ALTER TABLE dbo.CheckInOut ADD MaXuong nvarchar(100) NULL;
IF OBJECT_ID(N'dbo.LichSuXuongMayCan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichSuXuongMayCan (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_LichSuXuongMayCan PRIMARY KEY,
        MayCan varchar(100) NOT NULL,
        MaXuong nvarchar(100) NULL,
        TuThoiGian datetime2(7) NOT NULL,
        CONSTRAINT UX_LichSuXuongMayCan UNIQUE (MayCan, TuThoiGian)
    );
END;
-- Mốc bắt đầu theo UTC+7, độ chính xác giây như firmware. Chạy lại không thêm mốc mới.
DECLARE @Now datetime2(7) = CONVERT(datetime2(7), SWITCHOFFSET(SYSDATETIMEOFFSET(), '+07:00'));
SET @Now = DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(date,@Now), @Now), CONVERT(datetime2(7),CONVERT(date,@Now)));
INSERT dbo.LichSuXuongMayCan (MayCan, MaXuong, TuThoiGian)
SELECT CONVERT(varchar(100),m.Id), NULLIF(LTRIM(RTRIM(m.MaXuong)),N''), @Now
FROM dbo.MayCan m
WHERE NOT EXISTS (SELECT 1 FROM dbo.LichSuXuongMayCan h WITH (UPDLOCK,HOLDLOCK) WHERE h.MayCan=CONVERT(varchar(100),m.Id));
COMMIT;
GO
CREATE OR ALTER FUNCTION dbo.fn_XuongMayCanTaiThoiDiem(@MayCan varchar(100), @ThoiGian datetime2(7))
RETURNS nvarchar(100)
AS
BEGIN
    DECLARE @MaXuong nvarchar(100);
    SELECT TOP (1) @MaXuong=MaXuong FROM dbo.LichSuXuongMayCan
    WHERE MayCan=@MayCan AND TuThoiGian<=@ThoiGian ORDER BY TuThoiGian DESC;
    RETURN NULLIF(LTRIM(RTRIM(@MaXuong)),N'');
END;
GO
SELECT COUNT_BIG(*) AS SoMocLichSu FROM dbo.LichSuXuongMayCan;
-- Ca trước mốc triển khai có thể NULL: cần quản lý xác minh, không tự đoán xưởng.
SELECT COUNT_BIG(*) AS SoLuotChuaCoXuongLichSu FROM dbo.CheckInOut WHERE MaXuong IS NULL;
GO
