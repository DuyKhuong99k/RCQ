using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_PhieuXuat
    {
        private readonly string connectionString;
        private string tableName = @"PD_PhieuXuat";
        private readonly string qrDelete = @"
DELETE FROM [dbo].[PD_PhieuXuat] WHERE 
 [STT] = @STT 
      and [Ngay] = @Ngay 
      and [PCName] = @PCName 
      and [MaXuong] = @MaXuong";
        private readonly string qrInsert = @" INSERT INTO [dbo].[PD_PhieuXuat]
           ([SoPhieu]
           ,[STT]
           ,[Ngay]
           ,[PCName]
           ,[MaXuong]
           ,[SuDung]
           ,[CreateDateTime]
           ,[CreateBy]
           ,[ModifiedDateTime]
           ,[ModifiedBy]
           ,[MaNhanVien]
           ,[GhiChu])
     VALUES
           (@SoPhieu 
           ,@STT 
           ,@Ngay 
           ,@PCName 
           ,@MaXuong 
           ,@SuDung 
           ,@CreateDateTime 
           ,@CreateBy 
           ,@ModifiedDateTime 
           ,@ModifiedBy 
           ,@MaNhanVien 
           ,@GhiChu) ";

        private readonly string qrUpdate = @"UPDATE [dbo].[PD_PhieuXuat]
   SET [SoPhieu] = @SoPhieu 
      ,[SuDung] = @SuDung 
      ,[CreateDateTime] = @CreateDateTime 
      ,[CreateBy] = @CreateBy 
      ,[ModifiedDateTime] = @ModifiedDateTime 
      ,[ModifiedBy] = @ModifiedBy 
      ,[MaNhanVien] = @MaNhanVien 
      ,[GhiChu] = @GhiChu 
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay
      and [PCName] = @PCName 
      and [MaXuong] = @MaXuong ";

        private readonly string qrGetAll = "Select * from PD_PhieuXuat";

        public PD_PhieuXuat()
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
