using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class LoTheoLine
    {
        private readonly string connectionString;
        private string tableName = @"LoTheoLine";
        private readonly string qrDelete = @"DELETE FROM [dbo].[LoTheoLine] WHERE Id = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[LoTheoLine]
           ([Id]
           ,[MaLine]
           ,[MaLo]
           ,[Ngay]
           ,[Gio],[CodeId])
     VALUES
           (@Id
           ,@MaLine
           ,@MaLo 
           ,@Ngay 
           ,@Gio,@CodeId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[LoTheoLine]
   SET [MaLine] = @MaLine
      ,[MaLo] = @MaLo
      ,[Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[CodeId] = @CodeId
 WHERE Id = @Id";


        private readonly string qrGetAll = "Select * from LoTheoLine";

        public LoTheoLine()
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
