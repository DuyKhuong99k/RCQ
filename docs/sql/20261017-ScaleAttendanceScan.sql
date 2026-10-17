/* Chạy trước khi cập nhật PMSHub. Chỉ thêm bảng liên kết, không sửa ca cũ.
   Dừng các Hub khi triển khai; không chạy bản cũ và mới đồng thời.
   Có thể chạy lại. Không tự suy đoán mã thành phẩm của dữ liệu lịch sử. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
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
        CONSTRAINT CK_ScaleAttendanceScan_Status CHECK (TrangThai IN (N'Vào', N'Ra')),
        CONSTRAINT CK_ScaleAttendanceScan_Machine CHECK (MaSoMay > 0)
    );
END;
IF NOT EXISTS (SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.ScaleAttendanceScan')
      AND name = N'IX_ScaleAttendanceScan_VaoId_MaSoMay_ThoiGian')
    CREATE INDEX IX_ScaleAttendanceScan_VaoId_MaSoMay_ThoiGian
    ON dbo.ScaleAttendanceScan(VaoId, MaSoMay, ThoiGian)
    INCLUDE (TrangThai, MaThanhPham);
COMMIT TRANSACTION;
