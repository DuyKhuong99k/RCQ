-- =========================================================================================
-- Script: docs/sql/20261017-ToiUuHieuNangHQPhieuCan.sql
-- Mục đích: Tối ưu hiệu năng gửi phiếu cân online (HQ_PhieuCan), chấm công và backup HQ.
--           Triệt tiêu Table Scan, chống nghẽn bảng, deadlock và phình RAM trên PMSHub.
-- =========================================================================================

-- CHỌN DATABASE CỦA BẠN (VD: USE [PMS_HQ]; hoặc USE [Ten_Database_Cua_Ban];)
-- USE [PMS_HQ];
-- GO

SET XACT_ABORT ON;
GO

-- =========================================================================================
-- PHẦN 1: TẠO CÁC INDEX TỐI ƯU (VIỆC 1)
-- =========================================================================================

-- 1. Triệt tiêu Table Scan cho câu GetOnlineTotals trên HQ_PhieuCan
-- Truy vấn: WHERE Ngay = @day AND MayCan = @machineId AND TrongLuong > 0
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = N'IX_HQ_PhieuCan_Ngay_MayCan_TrongLuong' 
      AND object_id = OBJECT_ID(N'dbo.HQ_PhieuCan')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_HQ_PhieuCan_Ngay_MayCan_TrongLuong]
    ON [dbo].[HQ_PhieuCan] ([Ngay], [MayCan], [TrongLuong])
    INCLUDE ([MaNhanVien]);
    PRINT N'[OK] Đã tạo Index: IX_HQ_PhieuCan_Ngay_MayCan_TrongLuong';
END
ELSE
BEGIN
    PRINT N'[INFO] Index IX_HQ_PhieuCan_Ngay_MayCan_TrongLuong đã tồn tại.';
END
GO

-- 2. Triệt tiêu quét bảng chấm công OpenShifts & kiểm tra ca trên CheckInOut
-- Truy vấn: WHERE MaChamCong = @card AND (TrangThai = 'Vào' OR TrangThai = 'Ra')
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = N'IX_CheckInOut_MaChamCong_TrangThai_ThoiGian' 
      AND object_id = OBJECT_ID(N'dbo.CheckInOut')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_CheckInOut_MaChamCong_TrangThai_ThoiGian]
    ON [dbo].[CheckInOut] ([MaChamCong], [TrangThai], [ThoiGian])
    INCLUDE ([MaSoMay], [VaoId], [Id], [CaDem], [Isforget]);
    PRINT N'[OK] Đã tạo Index: IX_CheckInOut_MaChamCong_TrangThai_ThoiGian';
END
ELSE
BEGIN
    PRINT N'[INFO] Index IX_CheckInOut_MaChamCong_TrangThai_ThoiGian đã tồn tại.';
END
GO

-- 3. Hỗ trợ tra cứu lịch sử xưởng cân nhanh theo mốc thời gian (ScaleWorkshopHistory)
IF OBJECT_ID(N'dbo.LichSuXuongMayCan', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes 
        WHERE name = N'IX_LichSuXuongMayCan_Lookup' 
          AND object_id = OBJECT_ID(N'dbo.LichSuXuongMayCan')
    )
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_LichSuXuongMayCan_Lookup]
        ON [dbo].[LichSuXuongMayCan] ([MayCan], [TuThoiGian] DESC)
        INCLUDE ([MaXuong]);
        PRINT N'[OK] Đã tạo Index: IX_LichSuXuongMayCan_Lookup';
    END
    ELSE
    BEGIN
        PRINT N'[INFO] Index IX_LichSuXuongMayCan_Lookup đã tồn tại.';
    END
END
GO

-- =========================================================================================
-- PHẦN 2: BẬT READ_COMMITTED_SNAPSHOT (VIỆC 2)
-- Chạy vào giờ nghỉ hoặc trước ca để tránh ngắt các kết nối dở dang.
-- =========================================================================================

DECLARE @DbName sysname = DB_NAME();
DECLARE @IsSnapshotOn bit;

SELECT @IsSnapshotOn = is_read_committed_snapshot_on 
FROM sys.databases 
WHERE name = @DbName;

IF @IsSnapshotOn = 0
BEGIN
    PRINT N'[INFO] Đang bật READ_COMMITTED_SNAPSHOT cho database: ' + @DbName;
    DECLARE @Sql nvarchar(max) = N'ALTER DATABASE ' + QUOTENAME(@DbName) + N' SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE;';
    EXEC sp_executesql @Sql;
    PRINT N'[OK] Đã bật thành công READ_COMMITTED_SNAPSHOT ON cho database: ' + @DbName;
END
ELSE
BEGIN
    PRINT N'[INFO] READ_COMMITTED_SNAPSHOT đã được bật từ trước cho database: ' + @DbName;
END
GO

-- =========================================================================================
-- PHẦN 3: KIỂM TRA TRẠNG THÁI SAU KHI THIẾT LẬP
-- =========================================================================================

PRINT N'---------------------------------------------------------------------------------';
PRINT N'KIỂM TRA TRẠNG THÁI DATABASE & INDEX:';

-- Kiểm tra snapshot
SELECT name AS DatabaseName, 
       CASE WHEN is_read_committed_snapshot_on = 1 THEN N'ON (Đã kích hoạt chống khóa bảng)' ELSE N'OFF (Chưa bật)' END AS ReadCommittedSnapshotStatus
FROM sys.databases 
WHERE name = DB_NAME();

-- Kiểm tra Index
SELECT t.name AS TableName, i.name AS IndexName, i.type_desc AS IndexType
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IN (N'IX_HQ_PhieuCan_Ngay_MayCan_TrongLuong', N'IX_CheckInOut_MaChamCong_TrangThai_ThoiGian', N'IX_LichSuXuongMayCan_Lookup')
ORDER BY t.name, i.name;
GO
