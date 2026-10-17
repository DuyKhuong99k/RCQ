/*
CHỈ DÙNG DB TEST/BẢN SAO, không chạy trên DB đang sản xuất.
Chọn đúng DB ở SSMS trước khi chạy. Cần schema 20261016-XuongLichSuChamCong.sql và Hub/web mới.
Đổi @ConfirmTestDatabase=1 để xác nhận DB test.
Đổi @KeepData=1 để COMMIT và xem trên web; mặc định 0 sẽ ROLLBACK.

Mẫu: cùng một cân/nhân viên ngày 05/10/2026:
  Sáng xưởng 2 (Xưởng Cũ): xưởng 4h, cân 3h, 14.70kg.
  Chiều xưởng 1 (Xưởng Mới): xưởng 4h, cân 3h, 14.60kg.
Đây là snapshot lịch sử MẪU đã xác nhận, không phải suy ra xưởng từ MayCan hiện tại.
Không sửa MayCan, không chèn/xóa mốc lịch sử thật, không gửi lệnh firmware.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @ConfirmTestDatabase bit=0;
DECLARE @KeepData bit=0;
DECLARE @Machine varchar(50)='7779520';
DECLARE @Card varchar(50)=NULL; -- NULL: tự chọn thẻ nhân viên chưa có dữ liệu ngày này.
DECLARE @Day date='20261005';
DECLARE @Tag nvarchar(100)=N'TEST_XUONG_LICHSU_20261005_V1';
IF @ConfirmTestDatabase<>1 THROW 51000,N'Chỉ dùng DB TEST. Chọn đúng DB và đặt @ConfirmTestDatabase=1.',1;
IF @@TRANCOUNT<>0 THROW 51000,N'Hãy kết thúc transaction đang mở trước.',1;
IF COL_LENGTH(N'dbo.CheckInOut',N'MaXuong') IS NULL
   OR OBJECT_ID(N'dbo.fn_XuongMayCanTaiThoiDiem') IS NULL
    THROW 51000,N'Chưa có schema xưởng lịch sử. Chạy script triển khai trước trên DB test.',1;
DECLARE @Base datetime2(7)=CONVERT(datetime2(7),@Day);
DECLARE @Employee nvarchar(50),@Name nvarchar(200),@MachineName nvarchar(200);
DECLARE @Cases TABLE (Ca int PRIMARY KEY, Xuong nvarchar(100), WorkshopIn datetime2(7), WorkshopOut datetime2(7),
    ScaleIn datetime2(7), ScaleOut datetime2(7), Product varchar(50), VaoId varchar(100), RaId varchar(100));
INSERT @Cases(Ca,Xuong,WorkshopIn,WorkshopOut,ScaleIn,ScaleOut,Product) VALUES
 (1,N'2',DATEADD(second,25217,@Base),DATEADD(second,39617,@Base),DATEADD(second,27017,@Base),DATEADD(second,37817,@Base),'LV'),
 (2,N'1',DATEADD(second,46817,@Base),DATEADD(second,61217,@Base),DATEADD(second,48617,@Base),DATEADD(second,59417,@Base),'BV');
UPDATE @Cases SET VaoId=@Machine+'.'+CONVERT(char(8),ScaleIn,112)+REPLACE(CONVERT(char(8),ScaleIn,108),':',''),
    RaId=@Machine+'.'+CONVERT(char(8),ScaleOut,112)+REPLACE(CONVERT(char(8),ScaleOut,108),':','');
DECLARE @Weights TABLE(Ca int,AtTime datetime2(7),Kg decimal(18,3),Id varchar(100));
INSERT @Weights(Ca,AtTime,Kg) VALUES
 (1,DATEADD(second,29717,@Base),7.750),(1,DATEADD(second,33317,@Base),6.950),
 (2,DATEADD(second,51317,@Base),7.250),(2,DATEADD(second,54917,@Base),7.350);
UPDATE @Weights SET Id=@Machine+'.'+CONVERT(char(8),AtTime,112)+REPLACE(CONVERT(char(8),AtTime,108),':','');
BEGIN TRY
    BEGIN TRANSACTION;
    SELECT @MachineName=COALESCE(NULLIF(DisplayName,N''),Id) FROM dbo.MayCan WHERE Id=@Machine;
    IF @MachineName IS NULL OR TRY_CONVERT(int,@Machine) IS NULL
        THROW 51000,N'Không tìm được cân. Chọn @Machine đã đăng ký.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.XiNghiep WHERE Ma='1') OR NOT EXISTS(SELECT 1 FROM dbo.XiNghiep WHERE Ma='2')
        THROW 51000,N'Cần xưởng 1 và 2 trong XiNghiep.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.HQ_ThanhPham WHERE Id='LV') OR NOT EXISTS(SELECT 1 FROM dbo.HQ_ThanhPham WHERE Id='BV')
       OR NOT EXISTS(SELECT 1 FROM dbo.HQ_LoaiNguyenLieu WHERE Id='DuaLUOI')
        THROW 51000,N'Cần thành phẩm LV/BV và nguyên liệu DuaLUOI trong DB test.',1;
    IF EXISTS(SELECT 1 FROM dbo.CheckInOut WHERE GhiChu=@Tag)
       OR EXISTS(SELECT 1 FROM dbo.ChamCongTaiXuong WHERE ThietBi=N'TEST_LS_20261005')
        THROW 51000,N'Mẫu đã có, không insert thêm. Dùng file Verify để đối chiếu.',1;
    -- Không tạo nhân viên/thẻ giả và không trộn thêm ca vào nhân viên đã có dữ liệu.
    SELECT TOP(1) @Card=t.MaTheTu,@Employee=t.MaNhanVien,@Name=COALESCE(n.Name,t.MaNhanVien)
    FROM dbo.TheTu t JOIN dbo.NhanVienDaiThanh n ON n.MaNhanVien=t.MaNhanVien
    WHERE (@Card IS NULL OR t.MaTheTu=@Card)
      AND NULLIF(t.MaTheTu,'') IS NOT NULL AND t.MaTheTu NOT LIKE '%.%'
      AND n.IsHuman=1 AND n.IsChucNang=0 AND n.IsPhucVu=0
      AND NOT EXISTS(SELECT 1 FROM dbo.CheckInOut c WHERE
          (c.MaChamCong=t.MaTheTu OR c.MaChamCong LIKE t.MaTheTu+'.%'
           OR EXISTS(SELECT 1 FROM dbo.TheTu otherCard WHERE otherCard.MaNhanVien=t.MaNhanVien
               AND otherCard.MaTheTu=CASE WHEN CHARINDEX('.',c.MaChamCong)>0
                   THEN LEFT(c.MaChamCong,CHARINDEX('.',c.MaChamCong)-1) ELSE c.MaChamCong END))
          AND c.ThoiGian>=@Base AND c.ThoiGian<DATEADD(day,1,@Base))
      AND NOT EXISTS(SELECT 1 FROM dbo.ChamCongTaiXuong c WHERE (c.MaTheTu=t.MaTheTu OR c.MaNhanVien=t.MaNhanVien)
          AND c.ThoiGian>=@Base AND c.ThoiGian<DATEADD(day,1,@Base))
      AND NOT EXISTS(SELECT 1 FROM dbo.HQ_PhieuCan p WHERE (p.TheIdNhanVien=t.MaTheTu OR p.MaNhanVien=t.MaNhanVien) AND p.Ngay=@Day)
    ORDER BY t.MaTheTu;
    IF @Employee IS NULL THROW 51000,N'Không có thẻ phù hợp chưa có dữ liệu ngày 05/10. Chọn thẻ test khác.',1;
    IF EXISTS(SELECT 1 FROM dbo.CheckInOut c JOIN @Cases x ON c.Id=x.VaoId OR c.Id=x.RaId)
       OR EXISTS(SELECT 1 FROM dbo.HQ_PhieuCan p JOIN @Weights w ON p.Id=w.Id)
        THROW 51000,N'Trùng Id tại giây mẫu với dữ liệu khác. Chọn cân test khác; không ghi đè.',1;

    INSERT dbo.ChamCongTaiXuong(Id,ThoiGian,Xuong,ThietBi,MaTheTu,MaNhanVien,TenNhanVien,TrangThai,Isforget)
    SELECT 'TEST_LS_20261005_W'+CONVERT(varchar(2),Ca)+'_IN',WorkshopIn,Xuong,N'TEST_LS_20261005',@Card,@Employee,@Name,N'Vào',0 FROM @Cases
    UNION ALL
    SELECT 'TEST_LS_20261005_W'+CONVERT(varchar(2),Ca)+'_OUT',WorkshopOut,Xuong,N'TEST_LS_20261005',@Card,@Employee,@Name,N'Ra',0 FROM @Cases;
    INSERT dbo.CheckInOut(Id,MaChamCong,ThoiGian,MaSoMay,TenMay,CongViec,TrangThai,Isforget,CaDem,VaoId,GhiChu,MaXuong)
    SELECT VaoId,@Card,ScaleIn,CONVERT(int,@Machine),@MachineName,LEFT(@MachineName,100),N'Vào',0,0,NULL,@Tag,Xuong FROM @Cases
    UNION ALL
    SELECT RaId,@Card,ScaleOut,CONVERT(int,@Machine),@MachineName,LEFT(@MachineName,100),N'Ra',0,0,VaoId,@Tag,Xuong FROM @Cases;
    INSERT dbo.HQ_PhieuCan(Id,STT,Ngay,NgayGio,MayCan,MaLo,MaSize,MaThanhPham,MaLoaiNguyenLieu,
        MaNhanVien,MaNhanVienPhucVu,MaNhanVienBanKiem,TrongLuong,TrongLuongTare,ChiSanLuong,Status,TheId,TheIdNhanVien,GhiChu,MaXuong)
    SELECT w.Id,0,@Day,w.AtTime,@Machine,'L01','20X20',c.Product,'DuaLUOI',
        @Employee,'','',w.Kg,0,1,1,@Card,@Card,@Tag,c.Xuong FROM @Weights w JOIN @Cases c ON c.Ca=w.Ca;

    SELECT DB_NAME() AS DatabaseTest,@Machine AS MaCan,@Card AS MaThe,@Employee AS MaNhanVien,@Name AS TenNhanVien,
        @KeepData AS SeLuuDuLieu;
    SELECT c.Ca,x.Ten AS XuongLichSu,c.WorkshopIn,c.WorkshopOut,c.ScaleIn,c.ScaleOut,
        '04:00:00' AS TongOXuong,'03:00:00' AS TongOCan,'01:00:00' AS ChenhLech,
        SUM(w.Kg) AS SanLuongKg,CAST(SUM(w.Kg)/3.0 AS decimal(18,2)) AS NangSuatKgGio
    FROM @Cases c JOIN @Weights w ON w.Ca=c.Ca JOIN dbo.XiNghiep x ON x.Ma=c.Xuong
    GROUP BY c.Ca,x.Ten,c.WorkshopIn,c.WorkshopOut,c.ScaleIn,c.ScaleOut ORDER BY c.Ca;
    IF @KeepData=1 BEGIN COMMIT; PRINT N'ĐÃ LƯU. Lọc web ngày 05/10/2026 theo mã nhân viên vừa in. Đổi xưởng cân trên Hub rồi chạy file Verify.'; END
    ELSE BEGIN ROLLBACK; PRINT N'ĐÃ ROLLBACK: chưa lưu mẫu. Đặt @KeepData=1 trên DB TEST để xem web.'; END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK;
    THROW;
END CATCH;
