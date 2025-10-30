using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TBon
    {
        private readonly string connectionString;
        private string tableName = @"T_Bon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_Bon]
            WHERE [Ma] = @Ma";
        private readonly string qrInsert = @"
    INSERT INTO [dbo].[T_Bon]
           ([Ma]
           ,[Ten]
           ,[IsKhoa]
           ,[IsDangRa]
           ,[TrongLuong]
           ,[GhiChu]
           ,[SuDung]
           ,[MaXuong]
           ,[TrongLuongThongBao])
     VALUES
           (@Ma
           ,@Ten
            ,@IsKhoa
            ,@IsDangRa
            ,@TrongLuong
            ,@GhiChu
           ,@SuDung
           ,@MaXuong
           ,@TrongLuongThongBao)";
        private readonly string qrUpdate = @"
UPDATE [dbo].[T_Bon]
   SET [Ten] = @Ten
           ,[IsKhoa] = @IsKhoa
           ,[IsDangRa] = IsDangRa
           ,[TrongLuong] = @TrongLuong
           ,[GhiChu] = @GhiChu
           ,[SuDung] = @SuDung
           ,[MaXuong] = @MaXuong
           ,[TrongLuongThongBao = @TrongLuongThongBao]
 WHERE [Ma] = @Ma";
        private readonly string qrGetAll = "Select * from T_Bon";
        public TBon()
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
