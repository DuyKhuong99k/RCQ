using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_Ca
    {
        private readonly string connectionString;
        private string tableName = @"HQ_Ca";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_Ca]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_Ca]
           ([Id]
           ,[Ma]
           ,[Ten],[NgayGio],[GioBatDau],[GioKetThuc],[IsQuaDem])
     VALUES
           (@Id
           ,@Ten
           ,@NgayGio,@GioBatDau,@GioKetThuc,@IsQuaDem)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_Ca]
   SET [Ten] = @Ten
      ,[NgayGio] = @NgayGio,[GioBatDau] = @GioBatDau,[GioKetThuc] = @GioKetThuc,[IsQuaDem] = @IsQuaDem
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_Ca";

        public HQ_Ca()
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
