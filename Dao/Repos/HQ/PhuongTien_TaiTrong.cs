using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhuongTien_TaiTrong
    {
        private readonly string connectionString;
        private string tableName = @"PhuongTien_TaiTrong";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhuongTien_TaiTrong]
      WHERE [MaPhuongTien] = @MaPhuongTien
      and [NgayApDung] = @NgayApDung";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhuongTien_TaiTrong]
           ([MaPhuongTien]
           ,[NgayApDung]
           ,[TaiTrong]
           ,[GhiChu])
     VALUES
           (@MaPhuongTien
           ,@NgayApDung
           ,@TaiTrong
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhuongTien_TaiTrong]
   SET [TaiTrong] = @TaiTrong
      ,[GhiChu] = @GhiChu
 WHERE [MaPhuongTien] = @MaPhuongTien
      and [NgayApDung] = @NgayApDung";

        private readonly string qrGetAll = "Select * from PhuongTien_TaiTrong";

        public PhuongTien_TaiTrong()
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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
  pttt.MaPhuongTien,
  pt.Ten,
  pttt.NgayApDung,
  pttt.TaiTrong,
  pttt.GhiChu
  from 
  PhuongTien_TaiTrong pttt,
  PhuongTienChoNguyenLieu pt
  where pttt.MaPhuongTien = pt.Ma";
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
    }
}
