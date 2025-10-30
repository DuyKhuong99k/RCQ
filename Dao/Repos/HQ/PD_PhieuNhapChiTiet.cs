using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_PhieuNhapChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"PD_PhieuNhapChiTiet";
        private readonly string qrDelete = @" DELETE FROM [dbo].[PD_PhieuNhapChiTiet]
      WHERE [SoPhieu] = @SoPhieu and [STT] =@STT ";

        private readonly string qrInsert = @" 
INSERT INTO [dbo].[PD_PhieuNhapChiTiet]
           ([SoPhieu]
           ,[STT]
           ,[MaSanPham]
           ,[SanLuong]
           ,[HanSuDung]
           ,[NgaySanXuat]
           ,[SuDung])
     VALUES
           (@SoPhieu 
           ,@STT 
           ,@MaSanPham 
           ,@SanLuong 
           ,@HanSuDung 
           ,@NgaySanXuat 
           ,@SuDung ) ";

        private readonly string qrUpdate = @"UPDATE [dbo].[PD_PhieuNhapChiTiet]
   SET [MaSanPham] = @MaSanPham 
      ,[SanLuong] = @SanLuong 
      ,[HanSuDung] = @HanSuDung 
      ,[NgaySanXuat] = @NgaySanXuat 
      ,[SuDung] = @SuDung 
 WHERE [SoPhieu] = @SoPhieu and [STT] =@STT ";

        private readonly string qrGetAll = "Select * from PD_PhieuNhapChiTiet";

        public PD_PhieuNhapChiTiet()
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
