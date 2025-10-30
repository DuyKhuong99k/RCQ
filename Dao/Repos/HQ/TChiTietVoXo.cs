using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TChiTietVoXo
    {
        private readonly string connectionString;
        private string tableName = @"T_ChiTietVoXo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_ChiTietVoXo]
            WHERE [Ma] = @Ma";
        private readonly string qrInsert = @"
    INSERT INTO [dbo].[T_ChiTietVoXo]
           ([Id]
           ,[MaPhieuPhanCo]
           ,[VoXo]
           ,[MaLoaiNguyenLieu]
           ,[MaThanhPham]
           ,[MaCongDoan]
           ,[MaSize]
           ,[MaQuyTrinh]
           ,[NgayGio]
           ,[UserName]
           ,[PCName]
           ,[MaSizeVoXo])
     VALUES
          (@Id
           ,@MaPhieuPhanCo
           ,@VoXo
           ,@MaLoaiNguyenLieu
           ,@MaThanhPham
           ,@MaCongDoan
           ,@MaSize
           ,@MaQuyTrinh
           ,@NgayGio
           ,@UserName
           ,@PCName
           ,@MaSizeVoXo)";
        private readonly string qrUpdate = @"UPDATE [dbo].[T_ChiTietVoXo]
   SET [Id]=@Id,[VoXo] = @VoXo,[MaPhieuPhanCo]=@MaPhieuPhanCo
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[MaThanhPham] = @MaThanhPham
      ,[MaCongDoan] = @MaCongDoan
,[MaSize]=@MaSize,[MaQuyTrinh]=@MaQuyTrinh,[NgayGio]=@NgayGio,[UserName]=@UserName,[PCName]=@PCName,[MaSizeVoXo]=@MaSizeVoXo
 WHERE [Id] = @Id";
        private readonly string qrGetAll = "Select * from T_ChiTietVoXo";
        public TChiTietVoXo()
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
