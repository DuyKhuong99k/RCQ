using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhuongTienChoNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"PhuongTienChoNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhuongTienChoNguyenLieu]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhuongTienChoNguyenLieu]
           ([Ma]
           ,[Ten]
           ,[SuDung],[IsHD],[IsGhe],[VungNuoiId],[SoGhe])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung,@IsHD,@IsGhe,@VungNuoiId,@SoGhe)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhuongTienChoNguyenLieu]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung , [IsHD] = @IsHD,[IsGhe] =@IsGhe ,[VungNuoiId] =@VungNuoiId,[SoGhe] = @SoGhe
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from PhuongTienChoNguyenLieu";

        public PhuongTienChoNguyenLieu()
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
