using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TCongDoan
    {
        private readonly string connectionString;
        private string tableName = @"T_CongDoan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_CongDoan]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[T_CongDoan]
           ([Ma]
           ,[Ten]
           ,[SuDung]
,[MaLuong]
,[MaLoaiNguyenLieu]
,[Idx])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
,@MaLuong
,@MaLoaiNguyenLieu
,@Idx)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[T_CongDoan]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
,[MaLuong] = @MaLuong
,[MaloaiNguyenLieu] = @MaLoaiNguyenLieu
,[Idx] = @Idx
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from T_CongDoan";

        public TCongDoan()
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
