using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class KD_Tui
    {
        private readonly string connectionString;
        private string tableName = @"KD_Tui";
        private readonly string qrDelete = @"DELETE FROM [dbo].[KD_Tui]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[KD_Tui]
           ([Ma]
           ,[Ten]
           ,[MaDonHang]
           ,[PhuTroi]
           ,[PhuTroiMax]
           ,[TongTrongLuong]
           ,[SuDung]
           ,[CreateDate]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaQuyCach])
     VALUES
           (@Ma
           ,@Ten
           ,@MaDonHang
           ,@PhuTroi
           ,@PhuTroiMax
           ,@TongTrongLuong
           ,@SuDung
           ,@CreateDate
           ,@MaSize
           ,@MaThanhPham
           ,@MaQuyCach)";

        private readonly string qrUpdate = @"UPDATE [dbo].[KD_Tui]
   SET [Ten] = @Ten
      ,[MaDonHang] = @MaDonHang
      ,[PhuTroi] = @PhuTroi
      ,[PhuTroiMax] = @PhuTroiMax
      ,[TongTrongLuong] = @TongTrongLuong
      ,[SuDung] = @SuDung
      ,[CreateDate] = @CreateDate
      ,[MaSize] = @MaSize
      ,[MaThanhPham] = @MaThanhPham
      ,[MaQuyCach] = @MaQuyCach
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from KD_Tui";

        public KD_Tui()
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
