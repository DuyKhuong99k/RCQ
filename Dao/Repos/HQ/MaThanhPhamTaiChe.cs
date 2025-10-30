using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamTaiChe
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamTaiChe";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamTaiChe]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamTaiChe]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[BravoId]
           ,[_type])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@BravoId
           ,@_type)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamTaiChe]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[BravoId] = @BravoId
      ,[_type] = @_type
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaThanhPhamTaiChe";

        public MaThanhPhamTaiChe()
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
