using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamChinhXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamChinhXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamChinhXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamChinhXepKhuon]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max]
           ,[BravoId]
           ,[_type],[ThamSoTangTrong],[DinhMucTangTrong])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung 
           ,@Min
           ,@Max
           ,@BravoId
           ,@_type,@ThamSoTangTrong,@DinhMucTangTrong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamChinhXepKhuon]
   SET [Ten] = @Ten  
      ,[SuDung] = @SuDung 
      ,[Min] = @Min
      ,[Max] = @Max
      ,[BravoId]
      ,[_type] = @_type,[ThamSoTangTrong] = @ThamSoTangTrong, [DinhMucTangTrong] = @DinhMucTangTrong
 WHERE  [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaThanhPhamChinhXepKhuon";

        public MaThanhPhamChinhXepKhuon()
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
