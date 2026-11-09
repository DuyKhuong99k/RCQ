/* Chay tren SQL xuong va gui lai tat ca Results + Messages.
   CHI DOC: khong tao index/bang, khong doi RCSI, khong sua phieu/ca.
   Mac dinh chi doc metadata/DMV; KHONG quet toan bang nghiep vu.
   DMV co the can VIEW DATABASE STATE / VIEW SERVER STATE (tuy phien ban).
   Thieu quyen DMV khong co nghia index bi thieu.
   KHONG chay lai script bat RCSI WITH ROLLBACK IMMEDIATE de chan doan.
*/
USE [PMS_HQ];
GO
SET NOCOUNT ON;
DECLARE @PreviousLockTimeout int = @@LOCK_TIMEOUT;
SET LOCK_TIMEOUT 3000;

SELECT N'01_DATABASE' AS Section, @@SERVERNAME AS ServerName, DB_NAME() AS DatabaseName,
    CONVERT(nvarchar(128), SERVERPROPERTY('ProductVersion')) AS ProductVersion,
    CONVERT(nvarchar(128), SERVERPROPERTY('Edition')) AS Edition,
    d.compatibility_level, d.is_read_committed_snapshot_on, d.is_auto_update_stats_on,
    SYSDATETIMEOFFSET() AS ServerTimeWithOffset
FROM sys.databases d WHERE d.database_id = DB_ID();

-- Metadata du kien. Co cot khong dong nghia kieu/do dai da dung: xem phan 03.
SELECT N'02_REQUIRED_SCHEMA' AS Section, e.TableName, e.ColumnName,
    CASE WHEN OBJECT_ID(N'dbo.' + e.TableName,N'U') IS NULL THEN N'MISSING_TABLE'
         WHEN COL_LENGTH(N'dbo.' + e.TableName,e.ColumnName) IS NULL THEN N'MISSING_COLUMN'
         ELSE N'PRESENT_CHECK_TYPE' END AS Result
FROM (VALUES
    (N'CheckInOut',N'Id'),(N'CheckInOut',N'VaoId'),(N'CheckInOut',N'MaXuong'),
    (N'CheckInOut',N'CaDem'),(N'CheckInOut',N'Isforget'),
    (N'ScaleAttendanceScan',N'Id'),(N'ScaleAttendanceScan',N'VaoId'),
    (N'ScaleAttendanceScan',N'MaSoMay'),(N'ScaleAttendanceScan',N'ThoiGian'),
    (N'ScaleAttendanceScan',N'TrangThai'),(N'ScaleAttendanceScan',N'MaThanhPham'),
    (N'ScaleAttendanceScan',N'MaLoaiNguyenLieu'),(N'ScaleAttendanceScan',N'MaXuong'),
    (N'LichSuXuongMayCan',N'MayCan'),(N'LichSuXuongMayCan',N'MaXuong'),
    (N'LichSuXuongMayCan',N'TuThoiGian'),
    (N'HQ_PhieuCan',N'Id'),(N'HQ_PhieuCan',N'Ngay'),(N'HQ_PhieuCan',N'NgayGio'),
    (N'HQ_PhieuCan',N'MaThanhPham'),(N'HQ_PhieuCan',N'MaLoaiNguyenLieu'),
    (N'HQ_PhieuCan',N'MaXuong'),(N'HQ_PhieuCan',N'MaNhanVien'),
    (N'HQ_PhieuCan',N'TheIdNhanVien'),(N'HQ_PhieuCan',N'TrongLuong'),
    (N'HQ_PhieuCan',N'TrongLuongTare'),(N'HQ_PhieuCan',N'ChiSanLuong'),(N'HQ_PhieuCan',N'Status')
) e(TableName,ColumnName) ORDER BY e.TableName,e.ColumnName;

SELECT N'03_COLUMN_TYPES' AS Section, t.name AS TableName,c.name AS ColumnName,
    ty.name AS DataType,c.max_length,c.precision,c.scale,c.is_nullable,c.collation_name
FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
JOIN sys.columns c ON c.object_id=t.object_id
JOIN sys.types ty ON ty.user_type_id=c.user_type_id
WHERE s.name=N'dbo' AND t.name IN
    (N'HQ_PhieuCan',N'CheckInOut',N'ScaleAttendanceScan',N'LichSuXuongMayCan')
ORDER BY t.name,c.column_id;

-- Ten khac nhung cung key/INCLUDE co the tuong duong: KHONG tao index chi vi khac ten.
SELECT N'04_EXPECTED_INDEX_NAMES' AS Section,e.TableName,e.IndexName,
    CASE WHEN i.index_id IS NULL THEN N'NOT_FOUND_BY_NAME_CHECK_EQUIVALENT'
         WHEN i.is_disabled=1 THEN N'DISABLED' ELSE N'PRESENT_CHECK_KEYS' END AS Result
FROM (VALUES
    (N'ScaleAttendanceScan',N'IX_ScaleAttendanceScan_VaoId_MaSoMay_ThoiGian'),
    (N'CheckInOut',N'IX_CheckInOut_Card_Status_Time_Context'),
    (N'CheckInOut',N'IX_CheckInOut_VaoId'),
    (N'HQ_PhieuCan',N'IX_HQ_PhieuCan_Card_Time_WorkContext'),
    (N'HQ_PhieuCan',N'IX_HQ_PhieuCan_WorkContext_Time'),
    (N'LichSuXuongMayCan',N'IX_LichSuXuongMayCan_Lookup')
) e(TableName,IndexName)
LEFT JOIN sys.indexes i ON i.object_id=OBJECT_ID(N'dbo.'+e.TableName) AND i.name=e.IndexName
ORDER BY e.TableName,e.IndexName;

SELECT N'05_INDEX_KEYS' AS Section,t.name AS TableName,i.name AS IndexName,
    i.type_desc,i.is_unique,i.is_primary_key,i.is_disabled,i.filter_definition,
    ic.key_ordinal,ic.is_included_column,ic.is_descending_key,c.name AS ColumnName
FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
JOIN sys.indexes i ON i.object_id=t.object_id AND i.index_id>0
JOIN sys.index_columns ic ON ic.object_id=i.object_id AND ic.index_id=i.index_id
JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
WHERE s.name=N'dbo' AND t.name IN
    (N'HQ_PhieuCan',N'CheckInOut',N'ScaleAttendanceScan',N'LichSuXuongMayCan')
ORDER BY t.name,i.name,ic.is_included_column,ic.key_ordinal,ic.index_column_id;

SELECT N'06_APPROX_ROWS' AS Section,t.name AS TableName,SUM(p.rows) AS ApproximateRows
FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
JOIN sys.partitions p ON p.object_id=t.object_id AND p.index_id IN(0,1)
WHERE s.name=N'dbo' AND t.name IN
    (N'HQ_PhieuCan',N'CheckInOut',N'ScaleAttendanceScan',N'LichSuXuongMayCan')
GROUP BY t.name;

BEGIN TRY
    SELECT N'07_INDEX_USAGE' AS Section,t.name AS TableName,i.name AS IndexName,
        u.user_seeks,u.user_scans,u.user_lookups,u.user_updates,u.last_user_seek,u.last_user_scan
    FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
    JOIN sys.indexes i ON i.object_id=t.object_id AND i.index_id>0
    LEFT JOIN sys.dm_db_index_usage_stats u ON u.database_id=DB_ID()
        AND u.object_id=i.object_id AND u.index_id=i.index_id
    WHERE s.name=N'dbo' AND t.name IN
        (N'HQ_PhieuCan',N'CheckInOut',N'ScaleAttendanceScan',N'LichSuXuongMayCan')
    ORDER BY t.name,i.name;
END TRY
BEGIN CATCH
    SELECT N'07_INDEX_USAGE' AS Section,ERROR_NUMBER() AS ErrorNumber,ERROR_MESSAGE() AS Message;
END CATCH;

BEGIN TRY
    SELECT N'08_STATS' AS Section,t.name AS TableName,st.name AS StatsName,
        sp.last_updated,sp.rows,sp.rows_sampled,sp.modification_counter
    FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
    JOIN sys.stats st ON st.object_id=t.object_id
    OUTER APPLY sys.dm_db_stats_properties(st.object_id,st.stats_id) sp
    WHERE s.name=N'dbo' AND t.name IN
        (N'HQ_PhieuCan',N'CheckInOut',N'ScaleAttendanceScan',N'LichSuXuongMayCan')
    ORDER BY t.name,st.name;
END TRY
BEGIN CATCH
    SELECT N'08_STATS' AS Section,ERROR_NUMBER() AS ErrorNumber,ERROR_MESSAGE() AS Message;
END CATCH;

-- Chi anh chup hien tai; khong co wait luc nay khong chung minh khong tung bi cham.
BEGIN TRY
    SELECT N'09_ACTIVE_REQUESTS' AS Section,session_id,status,command,blocking_session_id,
        wait_type,wait_time,total_elapsed_time,cpu_time,logical_reads,reads,writes,open_transaction_count
    FROM sys.dm_exec_requests WHERE database_id=DB_ID() AND session_id<>@@SPID
    ORDER BY total_elapsed_time DESC;
END TRY
BEGIN CATCH
    SELECT N'09_ACTIVE_REQUESTS' AS Section,ERROR_NUMBER() AS ErrorNumber,ERROR_MESSAGE() AS Message;
END CATCH;

/* TUY CHON: doi @MeasureTotals=1 va dien 4 ma + moc gio THUC TE.
   Chi chay luc it tai, 1 lan, co the ton I/O neu thieu index. Khong xoa cache SQL.
   Thuong: 00:00 -> 00:00 hom sau; ca dem: @From la gio Vao hom truoc.
   Do query tong tuong duong, KHONG phai do end-to-end WSS hay p95/p99.
   Bat Include Actual Execution Plan (Ctrl+M) neu co quyen; gui ca Messages (IO/TIME).
*/
DECLARE @MeasureTotals bit=0;
DECLARE @From datetime2(7)='2026-10-23T00:00:00', @To datetime2(7)='2026-10-24T00:00:00';
DECLARE @Product varchar(50)='',@Material varchar(50)='',@Workshop varchar(50)='',@Employee varchar(50)='';
IF @MeasureTotals=1
BEGIN
    IF @Product='' OR @Material='' OR @Workshop='' OR @Employee='' OR @To<=@From
        SELECT N'10_TOTALS' AS Section,N'Dien du ma va khoang gio truoc khi do.' AS Message;
    ELSE IF COL_LENGTH(N'dbo.HQ_PhieuCan',N'MaXuong') IS NULL
        SELECT N'10_TOTALS' AS Section,N'Thieu schema HQ_PhieuCan.' AS Message;
    ELSE
    BEGIN TRY
        SET STATISTICS IO ON;
        SET STATISTICS TIME ON;
        EXEC sys.sp_executesql N'
            SELECT COUNT(*) AS ContextCount,SUM(TrongLuong) AS ContextWeight,
                SUM(CASE WHEN MaNhanVien=@Employee THEN 1 ELSE 0 END) AS EmployeeCount,
                SUM(CASE WHEN MaNhanVien=@Employee THEN TrongLuong ELSE 0 END) AS EmployeeWeight
            FROM dbo.HQ_PhieuCan
            WHERE NgayGio>=@From AND NgayGio<@To AND MaThanhPham=@Product
                AND MaLoaiNguyenLieu=@Material AND MaXuong=@Workshop AND ChiSanLuong=1 AND Status=1;',
            N'@From datetime2(7),@To datetime2(7),@Product varchar(50),@Material varchar(50),@Workshop varchar(50),@Employee varchar(50)',
            @From,@To,@Product,@Material,@Workshop,@Employee;
        SET STATISTICS IO OFF;
        SET STATISTICS TIME OFF;
    END TRY
    BEGIN CATCH
        SET STATISTICS IO OFF;
        SET STATISTICS TIME OFF;
        SELECT N'10_TOTALS' AS Section,ERROR_NUMBER() AS ErrorNumber,ERROR_MESSAGE() AS Message;
    END CATCH;
END;
-- SET LOCK_TIMEOUT can so nguyen literal, khong nhan bien truc tiep tren SQL Server.
DECLARE @RestoreLockTimeoutSql nvarchar(80)=N'SET LOCK_TIMEOUT '+CONVERT(nvarchar(12),@PreviousLockTimeout)+N';';
EXEC sys.sp_executesql @RestoreLockTimeoutSql;
