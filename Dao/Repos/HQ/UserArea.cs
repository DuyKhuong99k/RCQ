using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public partial class UserArea
    {
        private string connectionString;
        private string tableName = @"UserArea";
        private readonly string qrDelete = "DELETE FROM [dbo].[UserArea] WHERE [Id] = @Id";

        private readonly string qrInsert = @"

INSERT INTO [dbo].[UserArea]
           ([UserId]
           ,[WKv]
           ,[NgayGio])
     VALUES
           (@UserId
           ,@WKv
           ,@NgayGio)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[UserArea]
   SET [UserId] = @UserId
      ,[WKv] = @WKv
      ,[NgayGio] = @NgayGio
 WHERE Id =@Id

";

        private readonly string qrGetAll = "Select * from UserArea";
        private readonly string qrGetsByUserId = @"select * from UserArea where UserId = @userId";

        public UserArea(string? _connectionString = null)
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
        public List<T> Gets<T>(int userId)
        {
            try
            {
                var query = qrGetsByUserId;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new{userId}).ToList();
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
