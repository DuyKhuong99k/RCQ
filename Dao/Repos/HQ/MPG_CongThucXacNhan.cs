using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MPG_CongThucXacNhan
    {
        private readonly string connectionString;
        private string tableName = @"MPG_CongThucXacNhan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MPG_CongThucXacNhan]
      WHERE [MaCongThuc] = @MaCongThuc
      and [Block] = @Block
      and [Ngay] = @Ngay";
        private readonly string qrInsert = @"INSERT INTO [dbo].[MPG_CongThucXacNhan]
           ([MaCongThuc]
           ,[Block]
           ,[IsXacNhan]
           ,[Ngay]
           ,[Gio]
           ,[SuDung],[TrongLuongNguyenLieu])
     VALUES
           (@MaCongThuc
           ,@Block
           ,@IsXacNhan
           ,@Ngay
           ,@Gio
           ,@SuDung,@TrongLuongNguyenLieu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MPG_CongThucXacNhan]
   SET [IsXacNhan] = @IsXacNhan
      ,[Gio] = @Gio
      ,[SuDung] = @SuDung,
    [TrongLuongNguyenLieu] =@TrongLuongNguyenLieu
 WHERE [MaCongThuc] = @MaCongThuc
      and [Block] = @Block
      and [Ngay] = @Ngay";

        private readonly string qrGetAll = "Select * from MPG_CongThucXacNhan";

        public MPG_CongThucXacNhan()
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
