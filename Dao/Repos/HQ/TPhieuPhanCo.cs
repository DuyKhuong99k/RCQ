using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TPhieuPhanCo
    {
        private readonly string connectionString;
        private string tableName = @"T_PhieuPhanCo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_PhieuPhanCo]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_PhieuPhanCo]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[NgayTao]
           ,[GhiChu]
           ,[MaThanhPham], [LoaiPhieuPhanCo])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@NgayTao
           ,@GhiChu
           ,@MaThanhPham, @LoaiPhieuPhanCo)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_PhieuPhanCo]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[NgayTao] = @NgayTao
      ,[GhiChu] = @GhiChu
      ,[MaThanhPham] = @MaThanhPham, [LoaiPhieuPhanCo] = @LoaiPhieuPhanCo
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_PhieuPhanCo";

        public TPhieuPhanCo()
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
