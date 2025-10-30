using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhuongTienChoNguyenLieu_temp
    {
        private readonly string connectionString;
        private string tableName = @"PhuongTienChoNguyenLieu_temp";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhuongTienChoNguyenLieu_temp]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhuongTienChoNguyenLieu_temp]
           ([Ma]
           ,[Ten]
           ,[SuDung],[IsGhe],[VungNuoiId])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung,@IsGhe,@VungNuoiId)";
        private readonly string qrUpdate = @"
UPDATE [dbo].[PhuongTienChoNguyenLieu_temp]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung,@[IsGhe] = @IsGhe, [VungNuoiId] = @VungNuoiId
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from PhuongTienChoNguyenLieu_temp";

        public PhuongTienChoNguyenLieu_temp()
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
