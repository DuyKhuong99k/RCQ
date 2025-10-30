using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TLoaiNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"T_LoaiNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_LoaiNguyenLieu]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_LoaiNguyenLieu]
           ([Ma]
           ,[Ten]
           ,[MaKhuVuc]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@MaKhuVuc
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_LoaiNguyenLieu]
   SET [Ten] = @Ten
      ,[MaKhuVuc] = @MaKhuVuc
      ,[SuDung] = @SuDung 
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from T_LoaiNguyenLieu";

        public TLoaiNguyenLieu()
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
