using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class MaCa
    {
        private readonly string connectionString;
        private string tableName = @"MaCa";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaCa]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[MaCa]
           ([Ma]
           ,[Ten]
           ,[BravoId]
           ,[SuDung]
           ,[FromTime]
           ,[ToTime])
     VALUES
           (@Ma
           ,@Ten
           ,@BravoId
           ,@SuDung
           ,@FromTime
           ,@ToTime)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MaCa]
   SET [[Ten] =@Ten
           ,[BravoId] = @BravoId
           ,[SuDung] = @SuDung
           ,[FromTime] = @FromTime
           ,[ToTime] = ToTime
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from MaCa";

        public MaCa()
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
        public List<Models.Repos.Models.MaCa> Gets()
        {
            try
            {
                var query = "Select * from MaCa";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaCa>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
