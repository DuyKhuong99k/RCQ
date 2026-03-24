using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public class LogGhiNhanLoiCan
    {
        
        private readonly string connectionString;
        private string tableName = @"LogGhiNhanLoiCan";

        private readonly string qrInsert = @"INSERT INTO [dbo].[LogGhiNhanLoiCan]
           ([STT]
          ,[Ngay]
          ,[Gio]
          ,[GioBTP]
          ,[MaUserCan]
          ,[MaMayCan]
          ,[MaLoaiCa]
          ,[MaMau]
          ,[MaSize]
          ,[MaThanhPham]
          ,[MaLo]
          ,[MaThe]
          ,[TrongLuongNhan]
          ,[TrongLuongTra]
          ,[DinhMucThucTe]
          ,[DinhMucYeuCau]
          ,[MaXuong]
          ,[MaNhanVien]
          ,[CaTra]
          ,[STTBTP]
          ,[MaMayCanBTP]
          ,[GhiChu]
          ,[ChiSanLuong]
          ,[SuDung]
          ,[TrongLuongTare]
          ,[TrongLuongBu]
          ,[IsOffline]
          ,[MaNhanVienPhucVu]
          ,[Id]
          ,[IdIn]
          ,[ThongBaoLoi]
          ,[MaNhanVienBanKiem])
     VALUES
           (@STT,
          @Ngay,
          @Gio,
          @GioBTP,
          @MaUserCan,
          @MaMayCan,
          @MaLoaiCa,
          @MaMau,
          @MaSize,
          @MaThanhPham,
          @MaLo,
          @MaThe,
          @TrongLuongNhan,
          @TrongLuongTra,
          @DinhMucThucTe,
          @DinhMucYeuCau,
          @MaXuong,
          @MaNhanVien,
          @CaTra,
          @STTBTP,
          @MaMayCanBTP,
          @GhiChu,
          @ChiSanLuong,
          @SuDung,
          @TrongLuongTare,
          @TrongLuongBu,
          @IsOffline,
          @MaNhanVienPhucVu,
          @Id,
          @IdIn,
          @ThongBaoLoi,
          @MaNhanVienBanKiem)";

        

        private readonly string qrGetAll = "Select * from LogGhiNhanLoiCan";

        public LogGhiNhanLoiCan()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }

        
        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public List<VmLogGhiNhanLoiCan> GetAllsFullField(DateTime ngay, string maMayCan)
        {
            var query = @"SELECT
STT,
      p.Ngay,
      p.Gio,
      p.GioBTP,
      p.MaUserCan,
      p.MaMayCan,
      p.MaLoaiCa,
	  lc.Ten as TenLoaiCa,
      p.MaMau,
	  m.Ten as TenMau,
      p.MaSize,
	  s.Ten as TenSize,
      p.MaThanhPham,
	  tp.Ten as TenThanhPham,
      p.MaLo,
      p.MaThe,
      p.TrongLuongNhan,
      p.TrongLuongTra,
      p.DinhMucThucTe,
      p.DinhMucYeuCau,
      p.MaXuong,
	  x.Ten as TenXuong,
      p.MaNhanVien,
	  nv.MaHoSo,
	  nv.Name as TenNhanVien,
      p.CaTra,
      p.STTBTP,
      p.MaMayCanBTP,
      p.GhiChu,
      p.ChiSanLuong,
      p.SuDung,
      p.TrongLuongTare,
      p.TrongLuongBu,
      p.IsOffline,
      p.MaNhanVienPhucVu,
      p.Id,
      p.IdIn,
      p.ThongBaoLoi,
      p.MaNhanVienBanKiem
	  
	  from LogGhiNhanLoiCan p
	  left join MaLoaiCaDinhHinh lc on lc.Ma = p.MaLoaiCa
	  left join MaMauDinhHinh m on m.Ma = p.MaMau
	  left join MaSizeDinhHinh s on s.Ma = p.MaSize
	  left join MaThanhPhamDinhHinh tp on tp.Ma = p.MaThanhPham
	  left join XiNghiep x on x.Ma = p.MaXuong
	  left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien

	  where p.Ngay = @ngay and p.MaMayCan = @maMayCan order by p.Gio desc";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<VmLogGhiNhanLoiCan>(query, new {ngay = ngay.Date, maMayCan})
                .ToList();
            return items;
        }

        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }

        public int GetMaxSTTByNgayVaMay(DateTime ngay, string maMayCan)
{
    var query = @"SELECT ISNULL(MAX(STT),0)
                  FROM LogGhiNhanLoiCan
                  WHERE Ngay = @ngay AND MaMayCan = @maMayCan";

    using var connection = new SqlConnection(connectionString);
    connection.Open();

    return connection.ExecuteScalar<int>(query, new { ngay, maMayCan });
}
    }
    
}
