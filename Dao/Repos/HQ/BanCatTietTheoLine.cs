using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BanCatTietTheoLine
    {
        private readonly string connectionString;
        private string tableName = @"BanCatTietTheoLine";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BanCatTietTheoLine]
      WHERE [MaBanCatTiet] = @MaBanCatTiet
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BanCatTietTheoLine]
           ([MaBanCatTiet]
           ,[MaLine]
           ,[MaXuong])
     VALUES
           (@MaBanCatTiet
           ,@MaLine
           ,@MaXuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BanCatTietTheoLine]
   SET [MaLine] = @MaLine
      ,[MaXuong] = @MaXuong
 WHERE [MaBanCatTiet] = @MaBanCatTiet
";

        private readonly string qrGetAll = "Select * from BanCatTietTheoLine";

        public BanCatTietTheoLine()
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
