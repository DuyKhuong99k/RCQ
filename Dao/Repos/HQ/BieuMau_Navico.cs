using Dapper;
using Microsoft.Data.SqlClient;


namespace Dao.Repos.HQ
{
    public partial class BieuMau_Navico
    {
        private readonly string connectionString;
        private string tableName = @"BieuMau_Navico";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BieuMau_Navico]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BieuMau_Navico]
           ([Id]
           ,[Ngay]
           ,[MaNhanVien]
           ,[BoPhan]
           ,[MaCongDoan]
           ,[NguyenLieu_Ngay]
           ,[NguyenLieu_Dem]
           ,[ThanhPham_Ngay]
           ,[ThanhPham_Dem]
           ,[MaNhanVienCan]
           ,[MaNhaMay]
           ,[ThoiDiemGhiNhanSanLuong]
           ,[MaXuong]
           ,[MaKV])
     VALUES
           (@Id
           ,@Ngay
           ,@MaNhanVien
           ,@BoPhan
           ,@MaCongDoan
           ,@NguyenLieu_Ngay
           ,@NguyenLieu_Dem
           ,@ThanhPham_Ngay
           ,@ThanhPham_Dem
           ,@MaNhanVienCan
           ,@MaNhaMay
           ,@ThoiDiemGhiNhanSanLuong
           ,@MaXuong
           ,@MaKV)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BieuMau_Navico]
   SET [Ngay] = @Ngay
           ,[MaNhanVien] =@MaNhanVien
           ,[BoPhan]=@BoPhan
           ,[MaCongDoan]=@MaCongDoan
           ,[NguyenLieu_Ngay]=@NguyenLieu_Ngay
           ,[NguyenLieu_Dem]=@NguyenLieu_Dem
           ,[ThanhPham_Ngay]=@ThanhPham_Ngay
           ,[ThanhPham_Dem]=@ThanhPham_Dem
           ,[MaNhanVienCan]=@MaNhanVienCan
           ,[MaNhaMay]=@MaNhaMay
           ,[ThoiDiemGhiNhanSanLuong]=@ThoiDiemGhiNhanSanLuong
           ,[MaXuong]=@MaXuong
           ,[MaKV]=@MaKV
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from BieuMau_Navico";

        public BieuMau_Navico()
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
