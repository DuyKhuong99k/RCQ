using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_SanPham
    {
        private readonly string connectionString;
        private string tableName = @"PD_SanPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PD_SanPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO[dbo].[PD_SanPham]
           ([Ma]
           ,[Ten]
           ,[MaLoai]
           ,[MaDonVi]
           ,[SuDung]
           ,[GhiChu])
     VALUES
           (@Ma
           ,@Ten
           ,@MaLoai 
           ,@MaDonVi 
           ,@SuDung
           ,@GhiChu)
";

        private readonly string qrUpdate = @" 
UPDATE [dbo].[PD_SanPham]
   SET [Ten] = @Ten
      ,[MaLoai] = @MaLoai
      ,[MaDonVi] =@MaDonVi 
      ,[SuDung] = @SuDung 
      ,[GhiChu] = @GhiChu
 WHERE  [Ma] = @Ma
";
        private readonly string qrGetAll = "Select * from PD_SanPham";

        public PD_SanPham(string? _connectionString = null)
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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
sp.Ma,
sp.Ten,
sp.MaLoai,
lsp.Ten as LoaiSanPhamName,
sp.MaDonVi,
dv.Ten as DonViName,
sp.SuDung,
sp.GhiChu
from PD_SanPham sp
left join PD_DonViTinh dv on sp.MaDonVi = dv.Ma
left join PD_LoaiSanPham lsp on sp.MaLoai = lsp.Ma";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
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
