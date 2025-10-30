using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NguoiDung_ThongTin
    {
        private readonly string connectionString;
        private string tableName = @"NguoiDung_ThongTin";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NguoiDung_ThongTin]
      WHERE [TenNguoiDung] = @TenNguoiDung";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NguoiDung_ThongTin]
           ([TenNguoiDung]
           ,[MaNhanVien]
           ,[MatKhau]
           ,[SuDung]
           ,[MoFormTrucTiep],[FolderName],[FileName])
     VALUES
           (@TenNguoiDung
           ,@MaNhanVien
           ,@MatKhau
           ,@SuDung
           ,@MoFormTrucTiep,@FolderName,@FileName)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NguoiDung_ThongTin]
   SET [MaNhanVien] = @MaNhanVien
      ,[MatKhau] = @MatKhau
      ,[SuDung] = @SuDung
      ,[MoFormTrucTiep] = @MoFormTrucTiep
 WHERE  [TenNguoiDung] = @TenNguoiDung";

        private readonly string qrGetAll = "Select * from NguoiDung_ThongTin";

        public NguoiDung_ThongTin()
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
