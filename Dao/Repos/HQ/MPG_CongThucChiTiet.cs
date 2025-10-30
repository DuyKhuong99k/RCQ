using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MPG_CongThucChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"MPG_CongThucChiTiet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MPG_CongThucChiTiet]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MPG_CongThucChiTiet]
           ([Ma]
           ,[Ten]
           ,[MaSanPham]
           ,[MaCongThuc]
           ,[TrongLuong]
           ,[TrongLuongDaCan]
           ,[TyLe]
           ,[IsFull]
           ,[PhuTroi]
           ,[PhuTroiMax]
           ,[SuDung],[IsXacNhan])
     VALUES
           (@Ma
           ,@Ten
           ,@MaSanPham
           ,@MaCongThuc
           ,@TrongLuong
           ,@TrongLuongDaCan
           ,@TyLe
           ,@IsFull
           ,@PhuTroi
           ,@PhuTroiMax
           ,@SuDung,@IsXacNhan)";
        private readonly string qrUpdate = @"UPDATE [dbo].[MPG_CongThucChiTiet]
   SET [Ten] = @Ten
      ,[MaSanPham] = @MaSanPham
      ,[MaCongThuc] = @MaCongThuc
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongDaCan] = @TrongLuongDaCan
      ,[TyLe] = @TyLe
      ,[IsFull] = @IsFull
      ,[PhuTroi] = @PhuTroi
      ,[PhuTroiMax] = @PhuTroiMax
      ,[SuDung] = @SuDung, [IsXacNhan] =@IsXacNhan
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MPG_CongThucChiTiet";

        public MPG_CongThucChiTiet()
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
