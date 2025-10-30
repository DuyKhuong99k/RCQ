using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TLoNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"T_LoNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_LoNguyenLieu]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_LoNguyenLieu]
           ([Ma]
           ,[Ten]
           ,[MaNhaCungCap]
           ,[MaNhomLo]
           ,[NgayTao]
           ,[SuDung],[MaSize])
     VALUES
           (@Ma
           ,@Ten
           ,@MaNhaCungCap
           ,@MaNhomLo
           ,@NgayTao
           ,@SuDung,@MaSize)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_LoNguyenLieu]
   SET [Ten] = @Ten
      ,[MaNhaCungCap] = @MaNhaCungCap
      ,[MaNhomLo] = @MaNhomLo
      ,[NgayTao] = @NgayTao
      ,[SuDung] = @SuDung, [MaSize] = @MaSize
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_LoNguyenLieu";

        public TLoNguyenLieu()
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
