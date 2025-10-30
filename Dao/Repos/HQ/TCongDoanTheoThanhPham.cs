using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TCongDoanTheoThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"T_CongDoanTheoThanhPhamTheoThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_CongDoanTheoThanhPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[T_CongDoanTheoThanhPham]
           ([Ma]
           ,[MaThanhPhamPham]
           ,[MaCongDoan]
,[SuDung]
,[Idx])
     VALUES
           (@Ma
           ,@MaThanhPhamPham
           ,@MaCongDoan
,@SuDung
,@Idx)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[T_CongDoanTheoThanhPham]
   SET [MaThanhPhamPham] = @MaThanhPhamPham
      ,[MaCongDoan] = @MaCongDoan
,[SuDung] = @SuDung
,[Idx] = @Idx
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from T_CongDoanTheoThanhPham";

        public TCongDoanTheoThanhPham()
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
