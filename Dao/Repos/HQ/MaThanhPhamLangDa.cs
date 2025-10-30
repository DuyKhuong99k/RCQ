using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamLangDa
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamLangDa";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamLangDa]
      WHERE [Ma] = @Ma and [MaCa] = @MaCa";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamLangDa]
           ([MaCa]
           ,[Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max]
           ,[BravoId])
     VALUES
           (@MaCa
           ,@Ma
           ,@Ten
           ,@SuDung
           ,@Min
           ,@Max
           ,@BravoId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamLangDa]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[Min] = @Min
      ,[Max] = @Max
      ,[BravoId] = @BravoId
 WHERE [MaCa] = @MaCa
      and [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaThanhPhamLangDa";

        public MaThanhPhamLangDa()
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
