-- CHỈ CHẠY TRÊN DB TEST/BẢN SAO sau script 20261016-XuongLichSuChamCong.sql.
-- Không kết nối thiết bị. Tất cả dữ liệu thử được ROLLBACK.
SET XACT_ABORT ON;
IF DB_NAME() = N'PMS_HQ'
    THROW 51000,N'Hãy chạy trên DB test/bản sao, không chạy trên PMS_HQ sản xuất.',1;
BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @Machine varchar(100)='__TEST_XUONG_HISTORY__';
    IF EXISTS(SELECT 1 FROM dbo.LichSuXuongMayCan WHERE MayCan=@Machine)
        THROW 51000,N'Mã test đã tồn tại. Không thay đổi dữ liệu có sẵn.',1;
    INSERT dbo.LichSuXuongMayCan(MayCan,MaXuong,TuThoiGian) VALUES
        (@Machine,N'2','20261016 06:00:00'),
        (@Machine,N'1','20261016 10:00:00'),
        (@Machine,N'2','20261017 06:00:00');
    IF dbo.fn_XuongMayCanTaiThoiDiem(@Machine,'20261016 05:59:59') IS NOT NULL
        THROW 51000,N'SAI: không được đoán xưởng trước mốc đầu.',1;
    IF COALESCE(dbo.fn_XuongMayCanTaiThoiDiem(@Machine,'20261016 09:00:00'),N'?')<>N'2'
        THROW 51000,N'SAI: dữ liệu offline lúc 09 giờ phải thuộc xưởng 2.',1;
    IF COALESCE(dbo.fn_XuongMayCanTaiThoiDiem(@Machine,'20261016 10:00:00'),N'?')<>N'1'
        THROW 51000,N'SAI: đúng mốc chuyển phải thuộc xưởng 1.',1;
    IF COALESCE(dbo.fn_XuongMayCanTaiThoiDiem(@Machine,'20261016 13:00:00'),N'?')<>N'1'
        THROW 51000,N'SAI: chuyển lại hôm sau không đổi xưởng ca hôm trước.',1;
    IF COALESCE(dbo.fn_XuongMayCanTaiThoiDiem(@Machine,'20261017 09:00:00'),N'?')<>N'2'
        THROW 51000,N'SAI: chuyển về xưởng cũ không đúng.',1;
    ROLLBACK;
    SELECT N'PASS: 5 tình huống lịch sử xưởng; đã ROLLBACK toàn bộ dữ liệu thử.' AS KetQua;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK;
    THROW;
END CATCH;
