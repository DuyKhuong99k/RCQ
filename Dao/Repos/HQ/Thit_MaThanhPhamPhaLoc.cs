using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class Thit_MaThanhPhamPhaLoc
    {
        private readonly string connectionString;
        private string tableName = @"Thit_MaThanhPhamPhaLoc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[Thit_MaThanhPhamPhaLoc]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[Thit_MaThanhPhamPhaLoc]
           ([Ma]
           ,[Ten]
           ,[MaLoaiNguyenLieu]
           ,[MaLuong]
           ,[GhiChu]
           ,[SuDung],[IsMang],[CodeId])
     VALUES
           (@Ma
           ,@Ten
           ,@MaLoaiNguyenLieu
           ,@MaLuong
           ,@GhiChu
           ,@SuDung,@IsMang,@CodeId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[Thit_MaThanhPhamPhaLoc]
   SET [Ten] = @Ten
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[MaLuong] = @MaLuong
      ,[GhiChu] = @GhiChu
      ,[SuDung] = @SuDung,[IsMang]=@IsMang,[CodeId] = @CodeId
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from Thit_MaThanhPhamPhaLoc";

        public Thit_MaThanhPhamPhaLoc()
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
    }
}
