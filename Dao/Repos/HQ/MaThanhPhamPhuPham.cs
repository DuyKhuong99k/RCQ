using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamPhuPham
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamPhuPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamPhuPham]
      WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamPhuPham]
           ([MaCa]
           ,[Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max],[BarvoId])
     VALUES
           (@MaCa 
           ,@Ma 
           ,@Ten 
           ,@SuDung  
           ,@Min 
           ,@Max,@BarvoId)";
        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamPhuPham]
   SET [Ten] = @Ten 
      ,[SuDung] = @SuDung 
      ,[Min] = @Min 
      ,[Max] = @Max ,[BarvoId] =@BarvoId
 WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from MaThanhPhamPhuPham";

        public MaThanhPhamPhuPham()
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
