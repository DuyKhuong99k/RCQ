-- CHỈ ĐỌC. Chạy trước và sau khi ĐỔI XƯỞNG TRÊN HUB DEMO.
-- Chọn cùng DB test đã insert mẫu. Không UPDATE/DELETE/ghi dữ liệu.
SET NOCOUNT ON;
DECLARE @Tag nvarchar(100)=N'TEST_XUONG_LICHSU_20261005_V1';
SELECT c.MaChamCong AS MaThe,t.MaNhanVien,n.Name AS TenNhanVien,
    c.MaSoMay,c.ThoiGian,c.TrangThai,c.MaXuong AS XuongDaLuu,x.Ten AS TenXuongLichSu,
    m.MaXuong AS XuongCuaCanHienTai,c.VaoId
FROM dbo.CheckInOut c
LEFT JOIN dbo.TheTu t ON t.MaTheTu=c.MaChamCong
LEFT JOIN dbo.NhanVienDaiThanh n ON n.MaNhanVien=t.MaNhanVien
LEFT JOIN dbo.XiNghiep x ON x.Ma=c.MaXuong
LEFT JOIN dbo.MayCan m ON m.Id=CONVERT(varchar(100),c.MaSoMay)
WHERE c.GhiChu=@Tag ORDER BY c.ThoiGian;
SELECT p.MaXuong AS XuongPhieuCan,x.Ten,COUNT(*) AS SoPhieu,SUM(p.TrongLuong-p.TrongLuongTare) AS TongKg
FROM dbo.HQ_PhieuCan p LEFT JOIN dbo.XiNghiep x ON x.Ma=p.MaXuong
WHERE p.GhiChu=@Tag GROUP BY p.MaXuong,x.Ten ORDER BY p.MaXuong;
SELECT w.Xuong,x.Ten,COUNT(*) AS SoLuotChamCongXuong
FROM dbo.ChamCongTaiXuong w LEFT JOIN dbo.XiNghiep x ON x.Ma=w.Xuong
WHERE w.ThietBi=N'TEST_LS_20261005' GROUP BY w.Xuong,x.Ten ORDER BY w.Xuong;
-- Sau đổi cân: chỉ XuongCuaCanHienTai đổi. XuongDaLuu, tên xưởng lịch sử và tổng kg phải giữ nguyên.
