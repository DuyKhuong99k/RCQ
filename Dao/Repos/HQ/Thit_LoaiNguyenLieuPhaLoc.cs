using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class Thit_LoaiNguyenLieuPhaLoc
    {
        private readonly string connectionString;
        private string tableName = @"Thit_LoaiNguyenLieuPhaLoc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[Thit_LoaiNguyenLieuPhaLoc]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[Thit_LoaiNguyenLieuPhaLoc]
           ([Ma]
           ,[Ten]
           ,[IsMang]
           ,[GhiChu]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@IsMang
           ,@GhiChu
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[Thit_LoaiNguyenLieuPhaLoc]
   SET [Ten] = @Ten
      ,[IsMang] = @IsMang
      ,[GhiChu] = @GhiChu
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from Thit_LoaiNguyenLieuPhaLoc";

        public Thit_LoaiNguyenLieuPhaLoc()
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
