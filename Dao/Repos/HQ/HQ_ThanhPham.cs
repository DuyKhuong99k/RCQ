using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_ThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"HQ_ThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_ThanhPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[HQ_ThanhPham]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max]
           ,[BravoId]
           ,[_type])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung 
           ,@Min
           ,@Max
           ,@BravoId
           ,@_type)";

        private readonly string qrUpdate = @"UPDATE [dbo].[HQ_ThanhPham]
   SET [Ten] = @Ten  
      ,[SuDung] = @SuDung 
      ,[Min] = @Min
      ,[Max] = @Max
      ,[BravoId]
      ,[_type] = @_type
 WHERE  [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from HQ_ThanhPham";

        public HQ_ThanhPham()
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
