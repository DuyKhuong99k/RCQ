using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaCoiXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaCoiXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaCoiXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaCoiXepKhuon]
           ([Ma]
           ,[Ten]
           ,[TrongLuongMax]
           ,[Tam])
     VALUES
           (@Ma 
           ,@Ten 
           ,@TrongLuongMax 
           ,@Tam)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaCoiXepKhuon]
   SET [Ten] = @Ten  
      ,[TrongLuongMax] = @TrongLuongMax 
      ,[Tam] = @Tam
 WHERE  [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaCoiXepKhuon";
        private readonly string qrGetCoiTams = @"
        SELECT c.Ma as MaCoi,
            xn.Ma as MaXuong
        from (
                Select *
                from MaCoiXepKhuon
                where Tam = 1
            ) c
            LEFT join XiNghiep xn on 1 = 1";

        public MaCoiXepKhuon()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetCoiTams<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetCoiTams).ToList();
            return rows;
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
