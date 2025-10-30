using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_CongThucChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"PD_CongThucChiTiet";
        private readonly string qrDelete = @" DELETE FROM [dbo].[PD_CongThucChiTiet]
      WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaCongThuc] = @MaCongThuc";

        private readonly string qrInsert = @" INSERT INTO [dbo].[PD_CongThucChiTiet]
           ([STT]
           ,[Ngay]
           ,[MaCongThuc]
           ,[MaSanPham]
           ,[SanLuong]
           ,[TyLe]
           ,[BienDo])
     VALUES
           (@STT 
           ,@Ngay 
           ,@MaCongThuc 
           ,@MaSanPham 
           ,@SanLuong 
           ,@TyLe 
           ,@BienDo)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[PD_CongThucChiTiet]
   SET [MaSanPham] = @MaSanPham 
      ,[SanLuong] = @SanLuong 
      ,[TyLe] = @TyLe 
      ,[BienDo] = @BienDo
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaCongThuc] = @MaCongThuc";

        private readonly string qrGetAll = "Select * from PD_CongThucChiTiet";

        public PD_CongThucChiTiet(string? _connectionString = null)
        {
           connectionString =_connectionString??AppViewModels.Base.Ins.ConnectionString;

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
        public List<T> GetsFullField<T>(string maCongThuc, DateTime ngay)
        {
            try
            {
                var query = @"select
ct2.STT,
ct2.Ngay,
ct2.MaCongThuc,
ct.Ten as CongThucName,
ct2.MaSanPham,
sp.Ten as SanPhamName,
ct2.SanLuong,
ct2.TyLe,
ct2.BienDo
from (
Select c.* 
from PD_CongThucChiTiet c,
( Select 
Ngay,MaSanPham,
ROW_NUMBER() over (partition by MaSanPham order by Ngay desc) id 
FROM PD_CongThucChiTiet 
WHERE 
MaCongThuc = @maCongThuc 
and Ngay <= @ngay)
rl 
where 
c.MaCongThuc = @maCongThuc 
and c.Ngay= rl.Ngay 
and rl.id =1 
and rl.MaSanPham = c.MaSanPham) ct2
left join PD_CongThuc ct on ct2.MaCongThuc = ct.Ma
left join PD_SanPham sp on ct2.MaSanPham = sp.Ma";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { maCongThuc, ngay = ngay.Date}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
