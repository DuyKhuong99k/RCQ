using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TQuyTrinh
    {
        private readonly string connectionString;
        private string tableName = @"T_QuyTrinh";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_QuyTrinh]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_QuyTrinh]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[GhiChu],[IsNguyenLieu],[LoaiQuyTrinh])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung 
           ,@GhiChu,@IsNguyenLieu,@LoaiQuyTrinh)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_QuyTrinh]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu,[IsNguyenLieu] = @IsNguyenLieu,[LoaiQuyTrinh]=@LoaiQuyTrinh
 WHERE [Ma] = @Ma";


        private readonly string qrGetAll = "Select * from T_QuyTrinh";

        public TQuyTrinh()
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
