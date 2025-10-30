using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TNhomLo
    {
        private readonly string connectionString;
        private string tableName = @"T_NhomLo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_NhomLo]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_NhomLo]
           ([Ma]
           ,[Ten]
           ,[NgayTao]
           ,[SuDung],[NgayBatDau],[NgayKetThuc],[NgayNguyenLieu],[DaKetThuc],[GhiChu])
     VALUES
           (@Ma
           ,@Ten
           ,@NgayTao
           ,@SuDung,@NgayBatDau,@NgayKetThuc,@NgayNguyenLieu,@DaKetThuc,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_NhomLo]
   SET [Ten] = @Ten
      ,[NgayTao] = @NgayTao
      ,[SuDung] = @SuDung
      ,[NgayBatDau] = @NgayBatDau
      ,[NgayKetThuc] = @NgayKetThuc
      ,[NgayNguyenLieu] = @NgayNguyenLieu
      ,[DaKetThuc] = @DaKetThuc, [GhiChu] = @GhiChu
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_NhomLo";

        public TNhomLo()
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
