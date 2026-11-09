/* PMS_HQ - chạy ngoài giờ sản xuất, trước khi deploy bản online mới.
   Chỉ bổ sung schema/index. Không cập nhật ca, phiếu cân hoặc dữ liệu offline cũ.
   Dừng tất cả Hub cùng ghi PMS_HQ lúc nâng cấp. Không trộn bản cũ/mới.
   Có thể chạy lại. Không bật/tắt RCSI trong script này. */
USE [PMS_HQ];
GO
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 15000;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.CheckInOut', N'U') IS NULL OR OBJECT_ID(N'dbo.HQ_PhieuCan', N'U') IS NULL
        THROW 51000, N'Thiếu bảng CheckInOut hoặc HQ_PhieuCan. Kiểm tra đúng database.', 1;
    IF OBJECT_ID(N'dbo.ScaleAttendanceScan', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ScaleAttendanceScan
        (
            Id varchar(100) NOT NULL CONSTRAINT PK_ScaleAttendanceScan PRIMARY KEY,
            VaoId varchar(100) NOT NULL,
            MaSoMay int NOT NULL,
            ThoiGian datetime2(7) NOT NULL,
            TrangThai nvarchar(20) NOT NULL,
            MaThanhPham varchar(50) NULL,
            MaLoaiNguyenLieu varchar(50) NULL,
            MaXuong varchar(50) NULL,
            CONSTRAINT CK_ScaleAttendanceScan_Status CHECK (TrangThai IN (N'Vào', N'Ra')),
            CONSTRAINT CK_ScaleAttendanceScan_Machine CHECK (MaSoMay > 0)
        );
    END;
    IF COL_LENGTH(N'dbo.ScaleAttendanceScan', N'MaLoaiNguyenLieu') IS NULL
        ALTER TABLE dbo.ScaleAttendanceScan ADD MaLoaiNguyenLieu varchar(50) NULL;
    IF COL_LENGTH(N'dbo.ScaleAttendanceScan', N'MaXuong') IS NULL
        ALTER TABLE dbo.ScaleAttendanceScan ADD MaXuong varchar(50) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ScaleAttendanceScan')
        AND name = N'IX_ScaleAttendanceScan_VaoId_MaSoMay_ThoiGian')
        EXEC(N'CREATE INDEX IX_ScaleAttendanceScan_VaoId_MaSoMay_ThoiGian
               ON dbo.ScaleAttendanceScan(VaoId,MaSoMay,ThoiGian) INCLUDE(TrangThai,MaThanhPham)');
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CheckInOut')
        AND name = N'IX_CheckInOut_Card_Status_Time_Context')
        EXEC(N'CREATE INDEX IX_CheckInOut_Card_Status_Time_Context
               ON dbo.CheckInOut(MaChamCong,TrangThai,ThoiGian DESC)
               INCLUDE(MaSoMay,VaoId,CaDem,Isforget,MaXuong)');
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CheckInOut')
        AND name = N'IX_CheckInOut_VaoId')
        EXEC(N'CREATE INDEX IX_CheckInOut_VaoId ON dbo.CheckInOut(VaoId)
               INCLUDE(TrangThai,ThoiGian) WHERE VaoId IS NOT NULL');
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.HQ_PhieuCan')
        AND name = N'IX_HQ_PhieuCan_Card_Time_WorkContext')
        EXEC(N'CREATE INDEX IX_HQ_PhieuCan_Card_Time_WorkContext
               ON dbo.HQ_PhieuCan(TheIdNhanVien,NgayGio)
               INCLUDE(MaThanhPham,MaLoaiNguyenLieu,MaXuong,TrongLuong,TrongLuongTare,ChiSanLuong,Status,MayCan)');
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.HQ_PhieuCan')
        AND name = N'IX_HQ_PhieuCan_WorkContext_Time')
        EXEC(N'CREATE INDEX IX_HQ_PhieuCan_WorkContext_Time
               ON dbo.HQ_PhieuCan(MaThanhPham,MaLoaiNguyenLieu,MaXuong,NgayGio)
               INCLUDE(TrongLuong,TrongLuongTare,TheIdNhanVien,MaNhanVien,ChiSanLuong,Status)');
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
SET LOCK_TIMEOUT -1;
SELECT DB_NAME() AS DatabaseName, COL_LENGTH(N'dbo.ScaleAttendanceScan',N'MaLoaiNguyenLieu') AS MaterialColumnBytes,
    COL_LENGTH(N'dbo.ScaleAttendanceScan',N'MaXuong') AS WorkshopColumnBytes;
GO
