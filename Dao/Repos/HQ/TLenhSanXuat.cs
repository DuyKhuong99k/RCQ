using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TLenhSanXuat
    {
        private readonly string connectionString;
        private string tableName = @"T_LenhSanXuat";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_LenhSanXuat]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_LenhSanXuat]
           ([Ma]
           ,[Ten]
           ,[NgayTao]
           ,[NgayBatDau]
           ,[NgayKetThuc]
           ,[HoanThanh]
           ,[SuDung]
           ,[GhiChu])
     VALUES
           (@Ma
           ,@Ten
           ,@NgayTao
           ,@NgayBatDau
           ,@NgayKetThuc
           ,@HoanThanh
           ,@SuDung
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_LenhSanXuat]
   SET [Ten] = @Ten 
      ,[NgayTao] = @NgayTao 
      ,[NgayBatDau] = @NgayBatDau 
      ,[NgayKetThuc] = @NgayKetThuc
      ,[HoanThanh] = @HoanThanh
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from T_LenhSanXuat";

        public TLenhSanXuat()
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
