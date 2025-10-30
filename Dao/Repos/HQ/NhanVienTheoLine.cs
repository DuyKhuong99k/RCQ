using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NhanVienTheoLine
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienTheoLine";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NhanVienTheoLine] WHERE Id = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NhanVienTheoLine]
           ([Id]
           ,[MaLine]
           ,[MaNhanVien]
           ,[MaViTri]
           ,[Ngay]
           ,[Gio],[CodeId])
     VALUES
           (@Id
           ,@MaLine
           ,@MaNhanVien
           ,@MaViTri
           ,@Ngay 
           ,@Gio,@CodeId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NhanVienTheoLine]
   SET [Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[MaNhanVien] = @MaNhanVien
      ,[MaLine] = @MaLine
      ,[MaViTri] = @MaViTri
      ,[CodeId] = @CodeId
 WHERE Id = @Id";

        private readonly string qrGetAll = "Select * from NhanVienTheoLine";

        public NhanVienTheoLine()
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
