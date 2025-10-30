using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;

namespace Dao.Repos.HQ
{
    public partial class PhuongTien_TrongLuongDauAo
    {
        private readonly string connectionString;
        private string tableName = @"PhuongTien_TrongLuongDauAo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhuongTien_TrongLuongDauAo]
      WHERE [MaPhuongTien] = @MaPhuongTien 
      and [MaAo] = @MaAo and [Ngay] = @Ngay and [MaNhaCungCap] =@MaNhaCungCap and Chuyen =@Chuyen";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhuongTien_TrongLuongDauAo]
           ([MaPhuongTien]
           ,[Ngay]
           ,[MaAo]
           ,[CaManh]
           ,[CaNgopAoGhe]
           ,[CaNgopAoXe]
           ,[TongHam]
           ,[CaNgayTruoc]
           ,[CaConLai]
           ,[ThuKy]
           ,[ApTai]
           ,[STTChuyen]
           ,[TyLeMoi]
           ,[GhiChu]
           ,[GioXuatPhat]
           ,[NgayXuatPhat]
           ,[NgayBatCa]
           ,[CreateDateTime]
           ,[CreateBy]
           ,[ModifiedDateTime]
           ,[ModifiedBy],[IsVungNuoiBlocked],[MaNhaCungCap],[CaNgopAoBanNgoai],[Chuyen])
     VALUES
           (@MaPhuongTien
           ,@Ngay
           ,@MaAo
           ,@CaManh
           ,@CaNgopAoGhe
           ,@CaNgopAoXe
           ,@TongHam
           ,@CaNgayTruoc
           ,@CaConLai
           ,@ThuKy
           ,@ApTai
           ,@STTChuyen
           ,@TyLeMoi
           ,@GhiChu
           ,@GioXuatPhat
           ,@NgayXuatPhat
           ,@NgayBatCa
           ,@CreateDateTime
           ,@CreateBy
           ,@ModifiedDateTime
           ,@ModifiedBy,@IsVungNuoiBlocked,@MaNhaCungCap,@CaNgopAoBanNgoai,@Chuyen)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhuongTien_TrongLuongDauAo]
   SET [CaManh] = @CaManh
      ,[CaNgopAoGhe] = @CaNgopAoGhe
      ,[CaNgopAoXe] = @CaNgopAoXe
      ,[TongHam] = @TongHam
      ,[CaNgayTruoc] = @CaNgayTruoc
      ,[CaConLai] = @CaConLai
      ,[ThuKy] = @ThuKy
      ,[ApTai] = @ApTai
      ,[STTChuyen] = @STTChuyen
      ,[TyLeMoi] = @TyLeMoi
      ,[GhiChu] = @GhiChu
      ,[GioXuatPhat] = @GioXuatPhat
      ,[NgayXuatPhat] = @NgayXuatPhat
      ,[NgayBatCa] = @NgayBatCa
      ,[CreateDateTime] = @CreateDateTime
      ,[CreateBy] = @CreateBy
      ,[ModifiedDateTime] = @ModifiedDateTime
      ,[ModifiedBy] = @ModifiedBy
      ,[IsVungNuoiBlocked] =@IsVungNuoiBlocked,
[CaNgopAoBanNgoai] =@CaNgopAoBanNgoai,
       
      
 WHERE [MaPhuongTien] = @MaPhuongTien 
      and [Ngay] = @Ngay 
      and [MaAo] = @MaAo 
        and [MaNhaCungCap] =@MaNhaCungCap and Chuyen =@Chuyen";

        private readonly string qrGetAll = "Select * from PhuongTien_TrongLuongDauAo";

        public PhuongTien_TrongLuongDauAo()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }

        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }

        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }

        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }

        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
        public List<T> GetThuKys<T>()
        {
            try
            {
                var query = @"Select
    Distinct ThuKy
from
    PhuongTien_TrongLuongDauAo
where
    ThuKy <> ''";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetApTais<T>()
        {
            try
            {
                var query = @"Select
    Distinct ApTai
from
    PhuongTien_TrongLuongDauAo
where
    ApTai <> ''
order by
    ApTai";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Gets_NgayBatCa<T>(DateTime dateTime)
        {
            try
            {
                var query = @"
Select
p.MaPhuongTien,
pt.Ten as PhuongTienName,
p.Ngay,
p.MaAo,
p.CaManh,
p.CaNgopAoGhe,
p.CaNgopAoXe,
p.TongHam,
p.CaNgayTruoc,
p.CaConLai,
p.ThuKy,
p.ApTai,
p.STTChuyen,
p.TyLeMoi,
p.GhiChu,
p.GioXuatPhat,
p.NgayXuatPhat,
p.NgayBatCa,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.IsVungNuoiBlocked,
p.MaNhaCungCap,
ncc.Ten as NhaCungCapName,
p.CaNgopAoBanNgoai,
p.Chuyen

from PhuongTien_TrongLuongDauAo p
left join PhuongTienChoNguyenLieu pt on p.MaPhuongTien = pt.Ma
left join NhaCungCapNguyenLieu ncc on p.MaNhaCungCap = ncc.Ma
where 
NgayBatCa = @ngay 
order by MaPhuongTien";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets_NgayNX<T>(DateTime dateTime)
        {
            try
            {
                var query = @"
Select
p.MaPhuongTien,
pt.Ten as PhuongTienName,
p.Ngay,
p.MaAo,
p.CaManh,
p.CaNgopAoGhe,
p.CaNgopAoXe,
p.TongHam,
p.CaNgayTruoc,
p.CaConLai,
p.ThuKy,
p.ApTai,
p.STTChuyen,
p.TyLeMoi,
p.GhiChu,
p.GioXuatPhat,
p.NgayXuatPhat,
p.NgayBatCa,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.IsVungNuoiBlocked,
p.MaNhaCungCap,
ncc.Ten as NhaCungCapName,
p.CaNgopAoBanNgoai,
p.Chuyen

from PhuongTien_TrongLuongDauAo p
left join PhuongTienChoNguyenLieu pt on p.MaPhuongTien = pt.Ma
left join NhaCungCapNguyenLieu ncc on p.MaNhaCungCap = ncc.Ma
where 
Ngay = @ngay 
order by MaPhuongTien";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public Models.Repos.Models.PhuongTien_TrongLuongDauAo Get(string maPhuongTien, DateTime ngay, string maAo, string maNhaCungCap,int chuyen)
        {
            var query = @"
 select * from PhuongTien_TrongLuongDauAo
 where
 MaPhuongTien = @maPhuongTien 
 and Ngay = @ngay
 and MaAo = @maAo
 and MaNhaCungCap = @maNhaCungCap
 and Chuyen = @chuyen
";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.QueryAsync(query, new { maPhuongTien, ngay = ngay.Date, maAo, maNhaCungCap,chuyen }).Result.FirstOrDefault();
            return item;
        }
    }

}
