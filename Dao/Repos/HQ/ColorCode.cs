using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public partial class ColorCode
    {
        private readonly string connectionString;
        private string tableName = @"ColorCode";
        private readonly string qrDelete = @"DELETE FROM [dbo].[ColorCode]
      WHERE [Code] = @Code 
      and [Code2] = @Code2";

        private readonly string qrInsert = @"INSERT INTO [dbo].[ColorCode]
           ([Code]
           ,[Code2]
           ,[Name],[ColorRGB]
           )
     VALUES
           (@Code
           ,@Code2
           ,@Name,@ColorRGB)";

        private readonly string qrUpdate = @"UPDATE [dbo].[ColorCode]
   SET [Name] = @Name ,[ColorRGB] =@ColorRGB
      
 WHERE ([Code] = @Code) 
      and [Code2] = @Code2";

        private readonly string qrGetAll = "Select * from ColorCode";

        public ColorCode()
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
