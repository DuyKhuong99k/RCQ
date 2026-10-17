-- PMSHub: chẩn đoán chỉ đọc. Chọn đúng database PMS trước khi chạy.
-- Không tạo index, đổi cấu hình, cập nhật dữ liệu hoặc xuất payload/chuỗi kết nối.
-- Một số DMV/Query Store cần quyền xem trạng thái; lỗi quyền được trả theo từng phần.
-- Thống kê sử dụng index có thể bị reset sau restart và không thay thế execution plan.
SET NOCOUNT ON;
DECLARE @PreviousLockTimeout int = @@LOCK_TIMEOUT;
SET LOCK_TIMEOUT 3000;

SELECT DB_NAME() AS DatabaseName,
       CONVERT(nvarchar(128), SERVERPROPERTY('ProductVersion')) AS ProductVersion,
       compatibility_level, is_read_committed_snapshot_on,
       snapshot_isolation_state_desc, is_auto_update_stats_on
FROM sys.databases
WHERE database_id = DB_ID();

-- Ước lượng số dòng từ metadata, không COUNT trên bảng nghiệp vụ.
SELECT s.name AS SchemaName, t.name AS TableName, SUM(p.rows) AS ApproximateRows
FROM sys.tables AS t
JOIN sys.schemas AS s ON s.schema_id = t.schema_id
JOIN sys.partitions AS p ON p.object_id = t.object_id AND p.index_id IN (0, 1)
WHERE s.name = N'dbo'
  AND t.name IN (N'HQ_PhieuCan', N'CheckInOut', N'LichSuXuongMayCan',
                 N'TheTu', N'NhanVienDaiThanh')
GROUP BY s.name, t.name
ORDER BY t.name;

-- Mỗi dòng là một cột index: key_ordinal = 0/is_included_column = 1 là INCLUDE.
SELECT t.name AS TableName, i.name AS IndexName, i.type_desc,
       i.is_unique, i.is_primary_key, i.is_disabled, i.filter_definition,
       ic.key_ordinal, ic.is_included_column, ic.is_descending_key,
       c.name AS ColumnName, ty.name AS DataType, c.max_length,
       c.collation_name
FROM sys.tables AS t
JOIN sys.schemas AS s ON s.schema_id = t.schema_id
JOIN sys.indexes AS i ON i.object_id = t.object_id AND i.index_id > 0
JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
JOIN sys.types AS ty ON ty.user_type_id = c.user_type_id
WHERE s.name = N'dbo'
  AND t.name IN (N'HQ_PhieuCan', N'CheckInOut', N'LichSuXuongMayCan')
ORDER BY t.name, i.name, ic.is_included_column, ic.key_ordinal, ic.index_column_id;

BEGIN TRY
    SELECT t.name AS TableName, i.name AS IndexName,
           u.user_seeks, u.user_scans, u.user_lookups, u.user_updates,
           u.last_user_seek, u.last_user_scan, u.last_user_update
    FROM sys.tables AS t
    JOIN sys.schemas AS s ON s.schema_id = t.schema_id
    JOIN sys.indexes AS i ON i.object_id = t.object_id AND i.index_id > 0
    LEFT JOIN sys.dm_db_index_usage_stats AS u
      ON u.database_id = DB_ID() AND u.object_id = i.object_id AND u.index_id = i.index_id
    WHERE s.name = N'dbo'
      AND t.name IN (N'HQ_PhieuCan', N'CheckInOut', N'LichSuXuongMayCan')
    ORDER BY t.name, i.name;
END TRY
BEGIN CATCH
    SELECT N'IndexUsage' AS DiagnosticSection, ERROR_NUMBER() AS ErrorNumber,
           ERROR_MESSAGE() AS DiagnosticMessage;
END CATCH;

-- Ảnh chụp tức thời; không thấy request chờ không chứng minh trước đó không bị nghẽn.
-- Không xuất SQL text hoặc thông tin đăng nhập.
BEGIN TRY
    SELECT session_id, status, command, blocking_session_id, wait_type, wait_time,
           total_elapsed_time, cpu_time, reads, logical_reads, writes,
           open_transaction_count
    FROM sys.dm_exec_requests
    WHERE database_id = DB_ID() AND session_id <> @@SPID
    ORDER BY total_elapsed_time DESC;
END TRY
BEGIN CATCH
    SELECT N'ActiveRequests' AS DiagnosticSection, ERROR_NUMBER() AS ErrorNumber,
           ERROR_MESSAGE() AS DiagnosticMessage;
END CATCH;

-- SQL Server có Query Store mới chạy phần này; không tự bật Query Store.
-- Dùng SQL động để máy chủ cũ không phải biên dịch các catalog view chưa hỗ trợ.
IF OBJECT_ID(N'sys.database_query_store_options') IS NOT NULL
BEGIN
    BEGIN TRY
        EXEC sys.sp_executesql N'
            SELECT actual_state_desc, desired_state_desc, readonly_reason,
                   current_storage_size_mb, max_storage_size_mb
            FROM sys.database_query_store_options;';

        -- Các khoảng thống kê giao với 24 giờ gần nhất, không phải percentile.
        -- Chỉ xuất Id/nhóm truy vấn, không xuất text có thể chứa dữ liệu đầu vào.
        EXEC sys.sp_executesql N'
            SELECT TOP (20) q.query_id, p.plan_id,
                CASE WHEN qt.query_sql_text LIKE N''%HQ_PhieuCan%''
                     THEN N''HQ_PhieuCan'' ELSE N''CheckInOut'' END AS QueryGroup,
                SUM(rs.count_executions) AS Executions,
                SUM(CONVERT(float, rs.count_executions) * rs.avg_duration)
                    / NULLIF(SUM(rs.count_executions), 0) / 1000.0 AS AvgDurationMs,
                MAX(rs.max_duration) / 1000.0 AS MaxDurationMs,
                SUM(CONVERT(float, rs.count_executions) * rs.avg_cpu_time)
                    / NULLIF(SUM(rs.count_executions), 0) / 1000.0 AS AvgCpuMs,
                SUM(CONVERT(float, rs.count_executions) * rs.avg_logical_io_reads)
                    / NULLIF(SUM(rs.count_executions), 0) AS AvgLogicalReads
            FROM sys.query_store_query AS q
            JOIN sys.query_store_query_text AS qt ON qt.query_text_id = q.query_text_id
            JOIN sys.query_store_plan AS p ON p.query_id = q.query_id
            JOIN sys.query_store_runtime_stats AS rs ON rs.plan_id = p.plan_id
            JOIN sys.query_store_runtime_stats_interval AS rsi
              ON rsi.runtime_stats_interval_id = rs.runtime_stats_interval_id
            WHERE rsi.end_time >= DATEADD(HOUR, -24, SYSDATETIMEOFFSET())
              AND rs.execution_type = 0
              AND (qt.query_sql_text LIKE N''%HQ_PhieuCan%''
                   OR qt.query_sql_text LIKE N''%CheckInOut%'')
              AND qt.query_sql_text NOT LIKE N''%sys.query_store%''
            GROUP BY q.query_id, p.plan_id,
                CASE WHEN qt.query_sql_text LIKE N''%HQ_PhieuCan%''
                     THEN N''HQ_PhieuCan'' ELSE N''CheckInOut'' END
            ORDER BY MaxDurationMs DESC;';
    END TRY
    BEGIN CATCH
        SELECT N'QueryStore' AS DiagnosticSection, ERROR_NUMBER() AS ErrorNumber,
               ERROR_MESSAGE() AS DiagnosticMessage;
    END CATCH;
END
ELSE
    SELECT N'QueryStore' AS DiagnosticSection,
           N'Catalog Query Store không có hoặc tài khoản không nhìn thấy.' AS DiagnosticMessage;

SET LOCK_TIMEOUT @PreviousLockTimeout;
