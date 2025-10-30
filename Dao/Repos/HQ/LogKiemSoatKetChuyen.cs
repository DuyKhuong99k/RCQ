using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class LogKiemSoatKetChuyen
    {
        private readonly string connectionString;
        private string tableName = @"LogKiemSoatKetChuyen";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[LogKiemSoatKetChuyen]
           ([UserName]
           ,[PCName]
           ,[MaXuong]
           ,[tab]
           ,[Action]
           ,[NgayChuyen]
           ,[GioChuyen])
     VALUES
           (@UserName 
           ,@PCName 
           ,@MaXuong 
           ,@tab 
           ,@Action 
           ,@NgayChuyen 
           ,@GioChuyen)";

        private readonly string qrUpdate = @"

";

        private readonly string qrGetAll = "Select * from LogKiemSoatKetChuyen";

        public LogKiemSoatKetChuyen()
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
