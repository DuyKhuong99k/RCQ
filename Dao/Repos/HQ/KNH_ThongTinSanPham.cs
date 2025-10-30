using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class KNH_ThongTinSanPham
    {
        private readonly string connectionString;
        private string tableName = @"KNH_ThongTinSanPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[KNH_ThongTinSanPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[KNH_ThongTinSanPham]
           ([Ma]
           ,[Ten]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[KNH_ThongTinSanPham]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from KNH_ThongTinSanPham";

        public KNH_ThongTinSanPham()
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
