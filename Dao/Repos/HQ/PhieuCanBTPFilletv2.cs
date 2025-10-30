using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanBTPFilletv2
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanBTPFilletv2";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanBTPFilletv2]
      WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanBTPFilletv2]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaUserCan]
           ,[MaMayCan]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[MaNhanVien]
           ,[MaMayLangDa]
           ,[TrongLuong]
           ,[IsEnabled]
           ,[MaXuong]
           ,[CaTra]
           ,[GhiChu],[TrongLuongTare],[MaNhanVienPhucVu],[Id])
     VALUES
           (@STT 
           ,@Ngay 
           ,@Gio 
           ,@MaUserCan 
           ,@MaMayCan 
           ,@MaLoaiCa 
           ,@MaMau 
           ,@MaSize 
           ,@MaThanhPham 
           ,@MaLo 
           ,@MaThe 
           ,@MaNhanVien 
           ,@MaMayLangDa 
           ,@TrongLuong 
           ,@IsEnabled 
           ,@MaXuong 
           ,@CaTra 
           ,@GhiChu,@TrongLuongTare,@MaNhanVienPhucVu,@Id)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanBTPFilletv2]
   SET [Gio] = @Gio
      ,[MaUserCan] = @MaUserCan 
       
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaMau] = @MaMau 
      ,[MaSize] = @MaSize 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaLo] = @MaLo 
      ,[MaThe] = @MaThe 
      ,[MaNhanVien] = @MaNhanVien 
      ,[MaMayLangDa] = @MaMayLangDa 
      ,[TrongLuong] = @TrongLuong 
      ,[IsEnabled] = @IsEnabled 
      
      ,[CaTra] = @CaTra 
      ,[GhiChu] = @GhiChu,
[TrongLuongTare] = @TrongLuongTare,
[MaNhanVienPhucVu] = @MaNhanVienPhucVu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from PhieuCanBTPFilletv2";
        private readonly string qrUpdateIdIsEnabled = @"UPDATE [dbo].[PhieuCanBTPFilletv2]
   SET 
      [IsEnabled] = @isEnabled 
 WHERE [Id] = @id";

        public PhieuCanBTPFilletv2(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public int ChuyenSize<T>(List<T> items, string sizeId)
        {
            try
            {
                var query = $@"
UPDATE [dbo].[PhieuCanBTPFilletv2]
   SET [MaSize] = '{sizeId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, items);
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int ChuyenXuong<T>(List<T> items, string xuongId)
        {
            try
            {
                var query = $@"
UPDATE [dbo].[PhieuCanBTPFilletv2]
   SET [MaXuong] = '{xuongId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, items);
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }













        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }
        public int Delete<T>(List<T> items)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrDelete, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete(DateTime dateTime)
        {
            try
            {
                var query = @"DELETE FROM [dbo].[PhieuCanBTPFilletv2]
      WHERE [Ngay]= @Ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connectionString.Contains("PMS_HQ") == false)
                    {
                        return 0;
                    }

                    connection.Open();
                    var rows = connection.Execute(query, new { ngay = dateTime.Date });
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public T Get<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            try
            {
                var query = @"Select
    *
from
    PhieuCanBTPFilletv2
Where
    Ngay = @ngay
    and MaThe = @theId
    and IsEnabled = @isEnabled
order by
    STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date, theId, isEnabled }).FirstOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            var query = @"Select * from PhieuCanBTPFilletv2 where Ngay= @ngay and MaThe = @theId and IsEnabled = @isEnabled ";
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, theId, isEnabled }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetChiTiets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"SELECT
    p.STT,
    p.[Ngày],
    p.[Giờ],
    p.[Lô],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    n.MaHoSo as [Mã Hồ Sơ Phục Vụ],
    n.Name as [Tên Nhân Viên Phục Vụ], 
    p.[Loại Cá],
    p.[Thành Phẩm],
    p.[Size],
    p.[Màu],
    p.[Cá Trả],
    p.[Trọng Lượng],
    p.[Trọng Lượng]*p.DinhMucHaoHut as [TL Trước Hao Hụt],
    p.[Mở Khóa],
    p.[Mã Thẻ],
    p.[Máy Lạng Da],
    p.[Máy Cân],
    p.[Xưởng]
from
    (
        Select
            p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(@ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien as [Mã Nhân Viên],
            n.MaHoSo as [Mã Hồ Sơ],
            n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.TrongLuong as [Trọng Lượng],
            p.IsEnabled as [Mở Khóa],
            p.MaThe as [Mã Thẻ],
            'M1' as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.MaNhanVienPhucVu,
            tp.DinhMucHaoHut
        from
            PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            NhanVienDaiThanh n
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
    ) p
    LEFT JOIN NhanVienDaiThanh n ON p.MaNhanVienPhucVu = n.MaNhanVien
order by
    p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetChiTiets(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"SELECT p.STT,
    p.[Ngày],
    p.[Giờ],
    p.[Lô],
    p.MaNhanVien as [Mã Nhân Viên],
    nv.MaHoSo as [Mã Hồ Sơ],
    nv.name as [Tên Nhân Viên],
    n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    n.MaHoSo as [Mã Hồ Sơ Phục Vụ],
    n.Name as [Tên Nhân Viên Phục Vụ],
    p.[Loại Cá],
    p.[Thành Phẩm],
    p.[Size],
    p.[Màu],
    p.[Cá Trả],
    p.[Trọng Lượng],
    p.[Trọng Lượng] * p.DinhMucHaoHut as [TL Trước Hao Hụt],
    p.TrongLuongTare as [Trọng Lượng Tare],
    p.[Mở Khóa],
    p.[Mã Thẻ],
    p.[Máy Lạng Da],
    p.[Máy Cân],
    p.[Xưởng]
from (
        Select p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(p.Ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien,
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.TrongLuong as [Trọng Lượng],
            p.TrongLuongTare,
            p.IsEnabled as [Mở Khóa],
            p.MaThe as [Mã Thẻ],
            'M1' as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.MaNhanVienPhucVu,
            tp.DinhMucHaoHut
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau
        where p.Ngay <= @ngay
            and p.Ngay >=@fromDate
            and p.MaXuong = @xuongId
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
    ) p
    LEFT JOIN NhanVienDaiThanh n ON p.MaNhanVienPhucVu = n.MaNhanVien
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
order by p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"SELECT p.STT,
    p.[Ngày] as Ngay,
    p.[Giờ] as Gio,
    p.[Lô] as Lo,
    p.MaNhanVien as MaNhanVien,
    nv.MaHoSo as MaHoSo,
    nv.name as TenNhanVien,
    n.MaNhanVien as MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    n.Name as TenNhanVienPhucVu,
    p.[Loại Cá] as LoaiCaName,
    p.[Thành Phẩm] as ThanhPhamName,
    p.[Size] as SizeName,
    p.[Màu] as MauName,
    p.[Cá Trả] as CaTra,
    p.[Trọng Lượng] as TrongLuong,
    p.[Trọng Lượng] * p.DinhMucHaoHut as TLTruocHaoHut,
    p.TrongLuongTare as TrongLuongTare,
    p.[Mở Khóa] as MoKhoa,
    p.[Mã Thẻ] as MaThe,
    p.[Máy Lạng Da] as MayLangDa,
    p.[Máy Cân] as MayCan,
    p.[Xưởng] as Xuong
from (
        Select p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(p.Ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien,
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.TrongLuong as [Trọng Lượng],
            p.TrongLuongTare,
            p.IsEnabled as [Mở Khóa],
            p.MaThe as [Mã Thẻ],
            'M1' as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.MaNhanVienPhucVu,
            tp.DinhMucHaoHut
        from PhieuCanBTPFilletv2 p
            left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
            left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
            left join MaSizeFillet s on p.MaSize = s.Ma
            left join MaMauFillet mau on p.MaMau = mau.Ma
        where p.Ngay <= @ngay
            and p.Ngay >=@fromDate
            and p.MaXuong = @xuongId and tp.IsNguyenCon = 0
    ) p
    LEFT JOIN NhanVienDaiThanh n ON p.MaNhanVienPhucVu = n.MaNhanVien
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
order by p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"SELECT p.STT,
    p.[Ngày] as Ngay,
    p.[Giờ] as Gio,
    p.[Lô] as Lo,
    p.MaNhanVien as MaNhanVien,
    nv.MaHoSo as MaHoSo,
    nv.name as TenNhanVien,
    n.MaNhanVien as MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    n.Name as TenNhanVienPhucVu,
    p.[Loại Cá] as LoaiCaName,
    p.[Thành Phẩm] as ThanhPhamName,
    p.[Size] as SizeName,
    p.[Màu] as MauName,
    p.[Cá Trả] as CaTra,
    p.[Trọng Lượng] as TrongLuong,
    p.[Trọng Lượng] * p.DinhMucHaoHut as TLTruocHaoHut,
    p.TrongLuongTare as TrongLuongTare,
    p.[Mở Khóa] as MoKhoa,
    p.[Mã Thẻ] as MaThe,
    p.[Máy Lạng Da] as MayLangDa,
    p.[Máy Cân] as MayCan,
    p.[Xưởng] as Xuong
from (
        Select p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(p.Ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien,
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.TrongLuong as [Trọng Lượng],
            p.TrongLuongTare,
            p.IsEnabled as [Mở Khóa],
            p.MaThe as [Mã Thẻ],
            'M1' as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.MaNhanVienPhucVu,
            tp.DinhMucHaoHut
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau
        where p.Ngay <= @ngay
            and p.Ngay >=@fromDate
            and p.MaXuong = @xuongId
            and p.MaNhanVien = @maNhanVien
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
    ) p
    LEFT JOIN NhanVienDaiThanh n ON p.MaNhanVienPhucVu = n.MaNhanVien
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
order by p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maNhanVien = maNhanVien })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"select * from 

(SELECT p.STT,
    p.[Ngày] as Ngay,
    p.[Giờ] as Gio,
    p.[Lô] as Lo,
    p.MaNhanVien as MaNhanVien,
    nv.MaHoSo as MaHoSo,
    nv.name as TenNhanVien,
    n.MaNhanVien as MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    n.Name as TenNhanVienPhucVu,
    p.[Loại Cá] as LoaiCaName,
    p.[Thành Phẩm] as ThanhPhamName,
    p.[Size] as SizeName,
    p.[Màu] as MauName,
    p.[Cá Trả] as CaTra,
    p.[Trọng Lượng] as TrongLuong,
    p.[Trọng Lượng] * p.DinhMucHaoHut as TLTruocHaoHut,
    p.TrongLuongTare as TrongLuongTare,
    p.[Mở Khóa] as MoKhoa,
    p.[Mã Thẻ] as MaThe,
    p.[Máy Lạng Da] as MayLangDa,
    p.[Máy Cân] as MayCan,
    p.[Xưởng] as Xuong
from (
        Select p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(p.Ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien,
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.TrongLuong as [Trọng Lượng],
            p.TrongLuongTare,
            p.IsEnabled as [Mở Khóa],
            p.MaThe as [Mã Thẻ],
            'M1' as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.MaNhanVienPhucVu,
            tp.DinhMucHaoHut
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau
        where p.Ngay <= @toDate
            and p.Ngay >=@fromDate
            and p.MaXuong = @xuongId
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
    ) p
    LEFT JOIN NhanVienDaiThanh n ON p.MaNhanVienPhucVu = n.MaNhanVien
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
)pc
where
pc.MaHoSo = @maHoSo
order by pc.STT desc,
    pc.Lo,
    pc.MayCan
";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maHoSo = maHoSo })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"SELECT p.STT,
    p.[Ngày] as Ngay,
    p.[Giờ] as Gio,
    p.[Lô] as Lo,
    p.MaNhanVien as MaNhanVien,
    nv.MaHoSo as MaHoSo,
    nv.name as TenNhanVien,
    n.MaNhanVien as MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    n.Name as TenNhanVienPhucVu,
    p.[Loại Cá] as LoaiCaName,
    p.[Thành Phẩm] as ThanhPhamName,
    p.[Size] as SizeName,
    p.[Màu] as MauName,
    p.[Cá Trả] as CaTra,
    p.[Trọng Lượng] as TrongLuong,
    p.[Trọng Lượng] * p.DinhMucHaoHut as TLTruocHaoHut,
    p.TrongLuongTare as TrongLuongTare,
    p.[Mở Khóa] as MoKhoa,
    p.[Mã Thẻ] as MaThe,
    p.[Máy Lạng Da] as MayLangDa,
    p.[Máy Cân] as MayCan,
    p.[Xưởng] as Xuong
from (
        Select p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(p.Ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien,
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.TrongLuong as [Trọng Lượng],
            p.TrongLuongTare,
            p.IsEnabled as [Mở Khóa],
            p.MaThe as [Mã Thẻ],
            'M1' as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.MaNhanVienPhucVu,
            tp.DinhMucHaoHut,
			p.MaThe
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
			TheTu t
        where p.Ngay <= @toDate
            and p.Ngay >=@fromDate
            and p.MaXuong = @xuongId
            --and p.MaThe = t.MaTheTu
			--and t.MaTheTu = @maThe
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
    ) p
    LEFT JOIN NhanVienDaiThanh n ON p.MaNhanVienPhucVu = n.MaNhanVien
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
	Left join TheTu t on p.MaNhanVien = t.MaNhanVien
	where
	p.MaThe = @maThe
order by p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maThe = maThe })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<T> GetChiTietPhieuCanChuaSuas<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"SELECT 
    pbtp.STT,
    pbtp.Ngay,
    pbtp.MaMayCan,
    pbtp.MaXuong,
    pbtp.Gio,
	pbtp.MaUserCan,
	pbtp.MaLoaiCa,
	lc.Ten as LoaiCaName,
	pbtp.MaMau,
	ma.Ten as MauName,
	pbtp.MaSize,
	s.Ten as SizeName,
	pbtp.MaThanhPham,
	tp.Ten as ThanhPhamName,
	pbtp.MaLo,
	pbtp.MaThe,
    pbtp.TrongLuong,
	pbtp.TrongLuongTare,
    pbtp.GhiChu
FROM 
    PhieuCanBTPFilletv2 pbtp
	left join MaLoaiCaFillet lc on pbtp.MaLoaiCa = lc.Ma
	left join MaMauFillet ma on pbtp.MaMau = ma.Ma
	left join MaSizeFillet s on pbtp.MaSize = s.Ma
	left join MaThanhPhamFillet tp on pbtp.MaThanhPham = tp.Ma
WHERE 
    pbtp.Ngay >= @fromDate
    AND pbtp.Ngay <= @toDate
    AND pbtp.MaXuong = @xuongId and tp.IsNguyenCon = 0
    AND NOT EXISTS (
        SELECT 1 
        FROM PhieuCanTPFilletv2 ptp 
        WHERE 
            ptp.STTBTP = pbtp.STT 
            AND ptp.Ngay = pbtp.Ngay 
            AND ptp.MaMayCanBTP = pbtp.MaMayCan
            AND ptp.MaXuong = pbtp.MaXuong
    )
ORDER BY 
    pbtp.Ngay, 
    pbtp.Gio;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanViens<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
//                var query =
//                    @"SELECT
//    p.[Ngày] as Ngay,
//    p.[Thời Gian Làm Việc (h)] as TGLamViec,
//    n.MaHoSo as MaHoSo,
//    n.Name as TenNhanVien,
//    p.[Lô] as Lo,
//    p.[Loại Cá] as LoaiCaName,
//    p.[Thành Phẩm] as ThanhPhamName,
//    p.[Size] as SizeName,
//    p.[Màu] as Mau,
//    p.[Cá Trả] as CaTra,
//    p.[Số Rổ] as SoRo,
//    p.[Trọng Lượng] as TrongLuong,
//    p.[TL Trước Hao Hụt] as TLTruocHaoHut
//from (
//        Select p.Ngay as [Ngày],
//            cast(
//                DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
//            ) as [Thời Gian Làm Việc (h)],
//            -- n.MaHoSo as [Mã Hồ Sơ],
//            -- n.Name as [Tên Nhân Viên],
//            p.MaNhanVien,
//            p.MaLo as [Lô],
//            la.Ten as [Loại Cá],
//            tp.Ten as [Thành Phẩm],
//            s.Ten as [Size],
//            mau.Ten as [Màu],
//            p.CaTra as [Cá Trả],
//            Count(*) as [Số Rổ],
//            Sum(p.TrongLuong) as [Trọng Lượng],
//            Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
//        from PhieuCanBTPFilletv2 p,
//            MaLoaiCaFillet la,
//            MaThanhPhamFillet tp,
//            MaSizeFillet s,
//            MaMauFillet mau
//        where p.Ngay <= @ngay
//            and p.Ngay >= @fromDate
//            and p.MaXuong = @xuongId
//            and p.MaLoaiCa = la.Ma
//            and p.MaThanhPham = tp.Ma
//            and p.MaSize = s.Ma
//            and p.MaMau = mau.Ma
//            and isnull(p.GhiChu,'') <>'HUY'
//        GROUP BY p.MaLo,
//            p.MaNhanVien,
//            la.Ten,
//            tp.Ten,
//            s.Ten,
//            mau.Ten,
//            p.CaTRa,
//            tp.DinhMucHaoHut,
//            p.Ngay
//    ) p
//    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//order by p.[Ngày],
//    n.MaHoSo,
//    p.[Lô],
//    p.[Thành Phẩm],
//    p.[Size],
//    p.[Cá Trả]";

                //thêm check in out để lấy thời gian vào ra
//var query =
//                    @"
//       ;WITH CheckInOutData AS (
//    SELECT 
//        c.MaChamCong,
//        MIN(c.ThoiGian) AS ThoiGianVao,
//        MAX(c.ThoiGian) AS ThoiGianRa
//    FROM CheckInOut c 
//    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @ngay
//    GROUP BY c.MaChamCong
//)
//SELECT 
//        p.Ngay,
//        CAST(DATEDIFF(second, MIN(p.Gio), MAX(p.Gio)) / 3600.0 as decimal(18, 3)) as TGLamViec,
//        p.MaNhanVien,
//		n.MaHoSo,
//		n.Name as TenNhanVien,
//        p.MaLo as Lo,
//        la.Ten as LoaiCaName,
//        tp.Ten as ThanhPhamName,
//        s.Ten as SizeName,
//        mau.Ten as Mau,
//        p.CaTra as CaTra,
//        COUNT(*) as SoRo,
//        SUM(p.TrongLuong) as TrongLuong,
//        SUM(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut,
//		ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
//		ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
//		DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
//    FROM 
//        PhieuCanBTPFilletv2 p
//		left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
//        LEFT join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
//        left join MaSizeFillet s on p.MaSize = s.Ma
//        left join MaMauFillet mau on p.MaMau = mau.Ma
//        left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//		left join CheckInOutData c on n.MaChamCong = c.MaChamCong
//    WHERE 
//        p.Ngay <= @ngay
//        AND p.Ngay >= @fromDate
//        AND p.MaXuong = @xuongId
//        AND ISNULL(p.GhiChu,'') <> 'HUY'

//    GROUP BY 
//        p.MaLo,
//        p.MaNhanVien,
//		n.MaHoSo,
//		n.Name,
//        la.Ten,
//        tp.Ten,
//        s.Ten,
//        mau.Ten,
//        p.CaTRa,
//        tp.DinhMucHaoHut,
//        p.Ngay,
//		c.ThoiGianVao,
//		c.ThoiGianRa";
var query =
                    @"
      SELECT 
        p.Ngay,
        CAST(DATEDIFF(second, MIN(p.Gio), MAX(p.Gio)) / 3600.0 as decimal(18, 3)) as TGLamViec,
        --p.MaNhanVien,
		--n.MaHoSo,
		--n.Name as TenNhanVien,
        p.MaLo as Lo,
        la.Ten as LoaiCaName,
        tp.Ten as ThanhPhamName,
        s.Ten as SizeName,
        mau.Ten as Mau,
        p.CaTra as CaTra,
        COUNT(*) as SoRo,
        SUM(p.TrongLuong) as TrongLuong
        --SUM(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut,
		--ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
		--ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
		--DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
    FROM 
        PhieuCanBTPFilletv2 p
		left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
        LEFT join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
        left join MaSizeFillet s on p.MaSize = s.Ma
        left join MaMauFillet mau on p.MaMau = mau.Ma
        --left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
		--left join CheckInOutData c on n.MaChamCong = c.MaChamCong
    WHERE 
        p.Ngay <= @ngay
        AND p.Ngay >= @fromDate
        AND p.MaXuong = @xuongId
        --AND ISNULL(p.GhiChu,'') <> 'HUY'

    GROUP BY 
        p.MaLo,
        --p.MaNhanVien,
		--n.MaHoSo,
		--n.Name,
        la.Ten,
        tp.Ten,
        s.Ten,
        mau.Ten,
        p.CaTRa,
        tp.DinhMucHaoHut,
        p.Ngay
		--c.ThoiGianVao,
		--c.ThoiGianRa";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopPhucVus<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
//                var query =
//                    @"Select 
//    p.[Ngày] as Ngay,
//    p.[Thời Gian Làm Việc (h)] as TGLamViec,
//    n.MaHoSo as MaHoSo,
//    n.Name as TenNhanVien,
//    p.[Lô] as Lo,
//    p.[Loại Cá] as LoaiCaName,
//    p.[Thành Phẩm] ThanhPhamName,
//    p.[Size] as SizeName,
//    p.[Màu] as Mau,
//    p.[Cá Trả] as CaTra,
//    p.[Số Rổ] as SoRo,
//    p.[Trọng Lượng] as TrongLuong,
//    p.[TL Trước Hao Hụt] as TLTruocHaoHut
//from (
//        Select p.Ngay as [Ngày],
//            cast(
//                DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
//            ) as [Thời Gian Làm Việc (h)],
//            -- n.MaHoSo as [Mã Hồ Sơ],
//            -- n.Name as [Tên Nhân Viên],
//            p.MaNhanVienPhucVu,
//            p.MaLo as [Lô],
//            la.Ten as [Loại Cá],
//            tp.Ten as [Thành Phẩm],
//            s.Ten as [Size],
//            mau.Ten as [Màu],
//            p.CaTra as [Cá Trả],
//            Count(*) as [Số Rổ],
//            Sum(p.TrongLuong) as [Trọng Lượng],
//            Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
//        from PhieuCanBTPFilletv2 p,
//            MaLoaiCaFillet la,
//            MaThanhPhamFillet tp,
//            MaSizeFillet s,
//            MaMauFillet mau
//        where p.Ngay <= @ngay
//            and p.Ngay >= @fromDate
//            and p.MaXuong = @xuongId -- and p.MaNhanVienPhucVu = n.MaNhanVien
//            and p.MaLoaiCa = la.Ma
//            and p.MaThanhPham = tp.Ma
//            and p.MaSize = s.Ma
//            and p.MaMau = mau.Ma
//        GROUP BY -- n.MaHoSo,
//            -- n.Name,
//            p.MaLo,
//            p.MaNhanVienPhucVu,
//            la.Ten,
//            tp.Ten,
//            s.Ten,
//            mau.Ten,
//            p.CaTRa,
//            tp.DinhMucHaoHut,
//            p.Ngay
//    ) p
//    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
//order by p.[Ngày],
//    n.MaHoSo,
//    p.[Lô],
//    p.[Thành Phẩm],
//    p.[Size],
//    p.[Cá Trả]";
//thêm checkinout để lấy thời gian vào ra
var query =
                    @"Select 
			p.Ngay,
            cast(
                DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
            ) as TGLamViec,
            p.MaNhanVienPhucVu,
			n.MaHoSo as MaHoSo,
            n.Name as TenNhanVien,
            p.MaLo as Lo,
            la.Ten as LoaiCaName,
            tp.Ten as ThanhPhamName,
            s.Ten as SizeName,
            mau.Ten as Mau,
            p.CaTra,
            Count(*) as SoRo,
            Sum(p.TrongLuong) as TrongLuong,
            Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut,
			MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
			MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
			DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
			NhanVienDaiThanh n,
			CheckInOut c
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
			and p.MaNhanVienPhucVu = n.MaNhanVien
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
			AND n.MaChamCong = c.MaChamCong AND c.ThoiGian = p.Ngay AND c.ThoiGian >= @fromDate AND c.ThoiGian <= @ngay 
        GROUP BY 
            p.MaLo,
            p.MaNhanVienPhucVu,
            la.Ten,
            tp.Ten,
            s.Ten,
            mau.Ten,
            p.CaTRa,
            tp.DinhMucHaoHut,
            p.Ngay,
			n.MaHoSo,
			n.Name,
			n.MaChamCong,
			c.ThoiGian";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
//                var query =
//                    @"Select
//    p.Ngay as Ngay,
//    cast(
//        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
//    ) as TGLamViec,
   
//    p.MaLo as Lo,
//    p.MaThanhPham,
//    tp.Ten as ThanhPhamName,
//p.MaXuong,
//    p.CaTra,
//    Count(*) as SoRo,
//    Sum(p.TrongLuong) as TrongLuong,
//    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut
//from
//    PhieuCanBTPFilletv2 p,
//    MaThanhPhamFillet tp
//where
//    p.Ngay <= @ngay
//    and p.Ngay >= @fromDate
//    and p.MaXuong = @xuongId
//    and p.MaThanhPham = tp.Ma
//    -- and isnull(p.GhiChu,'') <>'HUY'
//GROUP BY
//    p.MaLo,
//    tp.Ten,
//    p.CaTRa,
//    tp.DinhMucHaoHut,
//    p.Ngay,
//    p.MaThanhPham,
//p.MaXuong   
//order by
//    p.Ngay,
//    p.MaLo,
//    tp.Ten,
//    p.CaTra";
var query =
                    @"Select
    p.Ngay as Ngay,
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as TGLamViec,
   
    p.MaLo as Lo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
p.MaXuong,
    p.CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut
from
    PhieuCanBTPFilletv2 p
    left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
and tp.IsNguyenCon = 0 and p.IsEnabled = 1
    -- and isnull(p.GhiChu,'') <>'HUY'
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay,
    p.MaThanhPham,
	p.MaXuong   
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamsByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
    p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    p.MaLo as Lo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut,
    p.MaXuong
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp,
	NhanVienDaiThanh nv
where
    p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
	and p.MaNhanVien = nv.MaNhanVien
    and isnull(p.GhiChu,'') <>'HUY'
	and p.MaNhanVien = @maNhanVien
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay,
    p.MaThanhPham,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name,
    p.MaXuong
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien = maNhanVien, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamsByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
    p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    p.MaLo as Lo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut,
    p.MaXuong
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp,
	NhanVienDaiThanh nv,
    TheTu t
where
    p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
	and p.MaNhanVien = nv.MaNhanVien
    and isnull(p.GhiChu,'') <>'HUY'
	and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay,
    p.MaThanhPham,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name,
p.MaXuong
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maThe = maThe, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamsByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
    p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    p.MaLo as Lo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut,
    p.MaXuong
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp,
	NhanVienDaiThanh nv
where
    p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
	and p.MaNhanVien = nv.MaNhanVien
    and isnull(p.GhiChu,'') <>'HUY'
	and nv.MaHoSo = @maHoSo
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay,
    p.MaThanhPham,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name,
p.MaXuong
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo = maHoSo, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamBTPFilletv2<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
    p.Ngay,
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as ThoiGianLamViec,
   
    p.MaLo as Lo,
    tp.Ten as ThanhPham,
    p.CaTra as CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
     and isnull(p.GhiChu,'') <>'HUY'
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopLo<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
    p.Ngay as Ngay,
    p.MaLo as Lo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
	p.MaSize,
	s.Ten as SizeName,
	p.MaXuong,
    p.CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as TLTruocHaoHut
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp,
	MaSizeFillet s
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and isnull(p.GhiChu,'') <>'HUY' and tp.IsNguyenCon = 0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay,
    p.MaThanhPham,
	p.MaXuong,
	p.MaSize,
	s.Ten
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
	s.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region BTP FILLET HAI NẮM
        public DataTable GetChiTietsHN(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select p.*,
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên]
from (
        select p.STT,
            P.Ngay as [Ngày],
            cast(
                CONVERT(
                    varchar(19),
                    cast(p.Ngay as datetime) + DATEADD(
                        ms,
                        - DATEPART(ms, cast(p.Gio as datetime)),
                        cast(p.Gio as datetime)
                    ),
                    120
                ) as datetime
            ) as [Giờ],
            p.MaLo as [Lô],
            p.MaNhanVien as [Mã Nhân Viên],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            p.MaThe as [Mã Thẻ],
            p.TrongLuong as [Trọng Lượng],
            p.TrongLuongTare as [Tare],
            p.IsEnabled as [Mở Khỏa],
            may.Ten as [Máy Lạng Da],
            p.MaMayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
            p.GhiChu as [Ghi Chú]
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MayLangDa may
        where p.Ngay >= @fromDate
            and p.Ngay <= @ngay
            and p.MaXuong = @xuongId
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
            and p.MaMayLangDa = may.Ma
    ) p
    LEFT JOIN NhanVienDaiThanh n on p.[Mã Nhân Viên] = n.MaNhanVien
order by p.[Lô],
    p.[Máy Cân],
    p.STT desc";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTongHopsHN(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select p.*,
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên]
from (
        Select p.Ngay as [Ngày],
            cast(
                DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
            ) as [Thời Gian Làm Việc (h)],
            p.MaNhanVien,
            p.MaLo as [Lô],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            Count(*) as [Số Rổ],
            Sum(p.TrongLuong) as [Trọng Lượng],
            Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
        GROUP BY p.MaNhanVien,
            p.MaLo,
            la.Ten,
            tp.Ten,
            s.Ten,
            mau.Ten,
            p.CaTRa,
            tp.DinhMucHaoHut,
            p.Ngay
    ) p
    left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
order by p.[Ngày],
    n.MaHoSo,
    p.[Lô],
    p.[Thành Phẩm],
    p.[Size],
    p.[Cá Trả]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTongHopThanhPhamsHN(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
   
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public List<T> GetChiTiets_TG<T>(DateTime dateTime)
        {
            try
            {
                var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong * tp.DinhMucHaoHut + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong* tp.DinhMucHaoHut,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    p.MaLo,
    p.MaXuong,
    p.MaMayCan,
    0 as DonGia,
    0 * p.TrongLuong* tp.DinhMucHaoHut as ThanhTien,
    p.MaHoSo
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            PhieuCanBTPFilletv2 p,
            NhanVienDaiThanh nv
        where
            p.Ngay = @ngay
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
order by
    p.STT";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsLastMinutes<T>(int minu)
        {
            try
            {
                var now = DateTime.Now;
                var fromTime = now.AddMinutes(-1*minu).TimeOfDay;
                var toTime = now.TimeOfDay;

                var ngay = now.Date; 

                var query = @"
            SELECT TOP 100 *
            FROM PhieuCanBTPFilletv2
            WHERE Ngay = @ngay
              AND Gio BETWEEN @fromTime AND @toTime
            ORDER BY Gio DESC";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay, fromTime, toTime }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTiets_TG<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong* tp.DinhMucHaoHut + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong* tp.DinhMucHaoHut as TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    p.MaLo,
    p.MaXuong,
    p.MaMayCan,
    0 as DonGia,
    0 * p.TrongLuong* tp.DinhMucHaoHut as ThanhTien,
    p.MaHoSo,
    p.MaSize
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            PhieuCanBTPFilletv2 p,
            NhanVienDaiThanh nv
        where
            p.Ngay <= @ngay and p.Ngay>=@fromDate
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
order by
    p.STT";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = toDate.Date, fromDate = fromDate.Date }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T? GetLastByThe<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            try
            {
                var query = "Select top(1) * from PhieuCanBTPFilletv2 WITH(READPAST) where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled order by Gio Desc";
                // var query = @"Select top(1) * from PhieuCanBTPFilletv2 WITH(READPAST)  where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled order by Gio Desc";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<T>(
                            query,
                            new { ngay = dateTime.Date, theId = theId, isEnabled = isEnabled })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T? GetLastByThe<T>(DateTime dateTime, string theId)
        {
            try
            {
                var query = "Select top(1) * from PhieuCanBTPFilletv2 WITH(READPAST) where Ngay = @ngay And MaThe = @theId order by Gio Desc";
                // var query = @"Select top(1) * from PhieuCanBTPFilletv2 WITH(READPAST)  where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled order by Gio Desc";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<T>(
                            query,
                            new { ngay = dateTime.Date, theId = theId})
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetMaxSTT(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select isnull( Max(STT),0) as STT from PhieuCanBTPFilletv2 where  Ngay =@ngay and MaXuong = @xuongId and MaMayCan =@mayCanId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<int>(query, new { ngay = dateTime.Date, xuongId, mayCanId })
                    .SingleOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetNumNhanVienDaChiaCa(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    COUNT(Distinct MaNhanVien)
from
    PhieuCanBTPFilletv2
where
    Ngay = @ngay
    and MaXuong = @xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId, int stt)
        {
            try
            {
                var query =
                    @"Select * from PhieuCanBTPFilletv2 Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId and STT >@stt order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { ngay = dateTime.Date, xuongId = xuongId, mayCanId = mayCanId, stt })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanBTPFilletv2 Where Ngay = @ngay ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanBTPFilletv2 Where Ngay = @ngay and MaXuong= @xuongId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanBTPFilletv2 Where Ngay = @ngay and MaXuong= @xuongId and MaMayCan = @mayCanId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { ngay = dateTime.Date, xuongId = xuongId, mayCanId = mayCanId })
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId, string nhanVienId, bool isPhucVu = false)
        {
            try
            {
                var query = @"SELECT
    n.MaNhanVien,
    n.Name as NhanVienName,
    la.Ten as LoaiCaName,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    Count(p.TrongLuong) as SoRo,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    PhieuCanBTPFilletv2 p,
    NhanVienDaiThanh n,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp
Where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien= @nhanVienId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
Group By
    n.MaNhanVien,
    p.MaLo,
    la.Ten,
    p.MaSize,
    tp.Ten,
    n.Name";
                if (isPhucVu == true)
                {
                    query = @"SELECT
     n.MaNhanVien,
    n.Name as NhanVienName,
    la.Ten as LoaiCaName,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    Count(p.TrongLuong) as SoRo,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    PhieuCanBTPFilletv2 p,
    NhanVienDaiThanh n,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp
Where
p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = @nhanVienId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
Group By
    n.MaNhanVien,
    p.MaLo,
    la.Ten,
    p.MaSize,
    tp.Ten,
    n.Name";
                }

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId, nhanVienId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuong(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPFilletv2 Where  Ngay= @ngay and MaXuong = @xuongId and Gio between @fromTime and @toTime and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new { ngay = dateTime.Date, xuongId = xuongId, fromTime = fromTime, toTime = toTime });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPFilletv2 Where  Ngay= @ngay and MaXuong = @xuongId and Gio between @fromTime and @toTime and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            mayLangDa = mayLangDa,
                            fromTime = fromTime,
                            toTime = toTime
                        });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa,
            string exThanhPhamId)
        {
            try
            {
                var query =
                    $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPFilletv2 Where  Ngay= @ngay and MaXuong = @xuongId and Gio >= @fromTime and Gio < @toTime and ISNULL(GhiChu,'') <> 'HUY' and MaThanhPham not in ({@exThanhPhamId})";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            mayLangDa = mayLangDa,
                            fromTime = fromTime,
                            toTime = toTime
                        });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa,
            string exThanhPhamId,
            string sizeId)
        {
            try
            {
                var query =
                    $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPFilletv2 Where  Ngay= @ngay and MaXuong = @xuongId  and Gio >= @fromTime and Gio< @toTime and ISNULL(GhiChu,'') <> 'HUY' and MaSize = @sizeId and MaThanhPham not in ({@exThanhPhamId})";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            mayLangDa = mayLangDa,
                            fromTime = fromTime,
                            toTime = toTime,
                            sizeId = sizeId
                        });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public IList<string> GetSqlsInBatches(IList<Models.Repos.Models.PhieuCanBTPFilletv2> phieuCans)
        {
            var insertSql = @"INSERT INTO [dbo].[PhieuCanBTPFilletv2]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaUserCan]
           ,[MaMayCan]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[MaNhanVien]
           ,[MaMayLangDa]
           ,[TrongLuong]
           ,[IsEnabled]
           ,[MaXuong]
           ,[CaTra]
           ,[GhiChu],[TrongLuongTare],[MaNhanVienPhucVu]) VALUES";
            var valuesSql =
                @"({0},'{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}','{11}', '{12}', {13}, {14}, '{15}', {16},'{17}',{18},'{19}')";
            var batchSize = 1000;

            var sqlsToExecute = new List<string>();
            var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
                var valuesToInsert = phieuCanToInsert.Select(
                    x => string.Format(
                        valuesSql,
                        x.STT,
                        x.Ngay.ToString("yyyy-MM-dd"),
                        x.Gio.ToString(@"hh\:mm\:ss"),
                        x.MaUserCan,
                        x.MaMayCan,
                        x.MaLoaiCa,
                        x.MaMau,
                        x.MaSize,
                        x.MaThanhPham,
                        x.MaLo,
                        x.MaThe,
                        x.MaNhanVien,
                        x.MaMayLangDa,
                        x.TrongLuong,
                        x.IsEnabled == true ? 1 : 0,
                        x.MaXuong,
                        x.CaTra == true ? 1 : 0,
                        "Sync",
                        x.TrongLuongTare,
                        x.MaNhanVienPhucVu));
                sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
            }

            return sqlsToExecute;
        }
        public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            try
            {
                var query =
                    @"Select p.MaNhanVien,n.Name as NhanVienName,p.MaLo,la.Ten as LoaiCaName,tp.Ten as ThanhPhamName,p.CaTra,p.IsEnabled,COUNT(*) as SoRo,Sum(p.TrongLuong) as TrongLuong from PhieuCanBTPFilletv2 p, NhanVienDaiThanh n, MaLoaiCaFillet la, MaThanhPhamFillet tp where p.Ngay= @ngay and p.MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and n.MaHoSo = @maHoSo and p.MaLoaiCa = la.Ma and p.MaThanhPham = tp.Ma group by n.Name, p.MaNhanVien,p.MaLo,la.Ten ,tp.Ten,p.CaTra,p.IsEnabled order by p.MaLo,p.IsEnabled,tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { ngay = dateTime.Date, xuongId = xuongId, maHoSo = maHoSo })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopPhucVus(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    mau.Ten as [Màu],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
from
    PhieuCanBTPFilletv2 p,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp,
    MaSizeFillet s,
    MaMauFillet mau,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopPhucVus(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select p.[Ngày],
    p.[Thời Gian Làm Việc (h)],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.[Lô],
    p.[Loại Cá],
    p.[Thành Phẩm],
    p.[Size],
    p.[Màu],
    p.[Cá Trả],
    p.[Số Rổ],
    p.[Trọng Lượng],
    p.[TL Trước Hao Hụt]
from (
        Select p.Ngay as [Ngày],
            cast(
                DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
            ) as [Thời Gian Làm Việc (h)],
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            p.MaNhanVienPhucVu,
            p.MaLo as [Lô],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            Count(*) as [Số Rổ],
            Sum(p.TrongLuong) as [Trọng Lượng],
            Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId -- and p.MaNhanVienPhucVu = n.MaNhanVien
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
        GROUP BY -- n.MaHoSo,
            -- n.Name,
            p.MaLo,
            p.MaNhanVienPhucVu,
            la.Ten,
            tp.Ten,
            s.Ten,
            mau.Ten,
            p.CaTRa,
            tp.DinhMucHaoHut,
            p.Ngay
    ) p
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
order by p.[Ngày],
    n.MaHoSo,
    p.[Lô],
    p.[Thành Phẩm],
    p.[Size],
    p.[Cá Trả]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHops(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    mau.Ten as [Màu],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    PhieuCanBTPFilletv2 p,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp,
    MaSizeFillet s,
    MaMauFillet mau,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHops(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"SELECT p.[Ngày],
    p.[Thời Gian Làm Việc (h)],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.[Lô],
    p.[Loại Cá],
    p.[Thành Phẩm],
    p.[Size],
    p.[Màu],
    p.[Cá Trả],
    p.[Số Rổ],
    p.[Trọng Lượng],
    p.[TL Trước Hao Hụt]
from (
        Select p.Ngay as [Ngày],
            cast(
                DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
            ) as [Thời Gian Làm Việc (h)],
            -- n.MaHoSo as [Mã Hồ Sơ],
            -- n.Name as [Tên Nhân Viên],
            p.MaNhanVien,
            p.MaLo as [Lô],
            la.Ten as [Loại Cá],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            mau.Ten as [Màu],
            p.CaTra as [Cá Trả],
            Count(*) as [Số Rổ],
            Sum(p.TrongLuong) as [Trọng Lượng],
            Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
        from PhieuCanBTPFilletv2 p,
            MaLoaiCaFillet la,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.MaLoaiCa = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
            and isnull(p.GhiChu,'') <>'HUY'
        GROUP BY p.MaLo,
            p.MaNhanVien,
            la.Ten,
            tp.Ten,
            s.Ten,
            mau.Ten,
            p.CaTRa,
            tp.DinhMucHaoHut,
            p.Ngay
    ) p
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
order by p.[Ngày],
    n.MaHoSo,
    p.[Lô],
    p.[Thành Phẩm],
    p.[Size],
    p.[Cá Trả]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHopThanhPham<T>(DateTime dateTime, string maLo, string xuongId)
        {
            try
            {
                var query = @"Select
    p.MaThanhPham,
    SUM(p.TrongLuong) as TrongLuongHienTai
from
    PhieuCanBTPFilletv2 p
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaLo =@maLo
GROUP BY
    p.MaThanhPham";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query<T>(query, new { ngay = dateTime.Date, maLo, xuongId }).ToList();
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopThanhPhams(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    PhieuCanBTPFilletv2 p,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp,
    MaSizeFillet s,
    MaMauFillet mau,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra,
    tp.DinhMucHaoHut
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
   
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    PhieuCanBTPFilletv2 p,
    MaThanhPhamFillet tp
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
     and isnull(p.GhiChu,'') <>'HUY'
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTRa,
    tp.DinhMucHaoHut,
    p.Ngay
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHopTheoMayLangDa<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"
Select
    p.MaLo,
    may.Ten as MayLangDaName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    p.IsEnabled,
    COUNT(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPFilletv2 p,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaMayLangDa = may.Ma
group by
    p.MaLo,
    may.Ten,
    la.Ten,
    tp.Ten,
    p.CaTra,
    p.IsEnabled
order by
    p.MaLo,
    p.IsEnabled,
    tp.Ten";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result.ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<int, decimal> GetTongSoRoTongTrongLuong(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var query =
                    "Select Count(*) as Item1,ISNULL(Sum(TrongLuong) ,0) As Item2 from PhieuCanBTPFilletv2 where MaNhanVien = @nhanVienId and Ngay =@ngay and ISNULL(GhiChu,'') <> 'HUY'";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
                    .SingleOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }
        public int Insert<T>(List<T> items)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(qrInsert, items);
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }

        public int Update(string id, bool isEnabled)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdateIdIsEnabled, new { id, isEnabled });
            return rows;
        }
        public int Set(int stt, DateTime ngay, string mayCan, string maXuong, bool isEnabled, string nhanVienId)
        {
            var query = @"UPDATE [dbo].[PhieuCanBTPFilletv2]
   SET 
      [MaNhanVien] = @MaNhanVien 
      ,[IsEnabled] = @IsEnabled 
      
      
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, new { STT = stt, Ngay = ngay.Date, MaMayCan = mayCan, MaXuong = maXuong, IsEnabled = isEnabled, MaNhanVien = nhanVienId });
            return rows;
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanBTPFilletv2 where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, num })
                    .Result
                    .ToList();
                return items;
            }
        }
        public int Update<T>(List<T> items)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(qrUpdate, items);
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public List<T> GetTongHopThanhPhamDatBTPFillets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
var query = @"select
p.MaThanhPham as MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
sum(p.TrongLuong)  as TrongLuong
from PhieuCanBTPFilletv2 p 
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma 
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId and tp.IsDat = 1
group by p.MaThanhPham, tp.Ten
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { toDate = toDate.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetSanLuongDatNguyenConBTPFillets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
var query = @"select
p.MaThanhPham as MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
sum(p.TrongLuong)  as TrongLuong
from PhieuCanBTPFilletv2 p 
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma 
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId and tp.IsNguyenCon = 1
group by p.MaThanhPham, tp.Ten
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { toDate = toDate.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Không bao gồm dạt nguyên con
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<T> GetTongHopThanhPhamDatRjNguyenConBTPFillets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
var query = @"select
p.MaThanhPham as MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
sum(p.TrongLuong)  as TrongLuong
from PhieuCanBTPFilletv2 p 
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma 
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId and tp.IsDat = 1 and tp.IsNguyenCon = 0
group by p.MaThanhPham, tp.Ten
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { toDate = toDate.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCanBTPFillet_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaUserCan,
p.MaMayCan,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLo,
p.MaThe,
p.MaNhanVien,
nv.Name as MaNhanVienName,
p.MaMayLangDa,
mld.Ten as MayLangDaName,
p.TrongLuong,
p.IsEnabled,
p.MaXuong,
x.Ten as XuongName,
p.CaTra,
p.GhiChu,
p.TrongLuongTare,
p.MaNhanVienPhucVu,
pv.Name as NhanVienPhucVuName,
CONCAT(p.STT, '|', p.Ngay, '|', p.MaMayCan, '|', p.MaXuong) as STT_Ngay_MaMayCan_MaXuong
from PhieuCanBTPFilletv2 p
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MayLangDa mld on p.MaMayLangDa = mld.Ma
left join XiNghiep x on p.MaXuong = x.Ma
left join NhanVienDaiThanh pv on p.MaNhanVienPhucVu = pv.MaNhanVien
where p.Ngay = @dateTime and p.MaXuong = @xuongId
order by
p.STT desc";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { dateTime = dateTime.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        #endregion
    }
}
