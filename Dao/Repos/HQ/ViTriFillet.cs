using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class ViTriFillet
    {
        private readonly string connectionString;
        private string tableName = @"ViTriFillet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[ViTriFillet]
            WHERE [Ma] = @Ma";
        private readonly string qrInsert = @"
    INSERT INTO [dbo].[ViTriFillet]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[MacDinh]
           ,[CodeId])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@MacDinh
           ,@CodeId)";
        private readonly string qrUpdate = @"
UPDATE [dbo].[ViTriFillet]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[MacDinh] = @MacDinh
      ,[CodeId] = @CodeId
 WHERE [Ma] = @Ma";
        private readonly string qrGetAll = "Select * from ViTriFillet";
        public ViTriFillet()
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
