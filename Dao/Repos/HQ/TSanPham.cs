using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TSanPham
    {
        private readonly string connectionString;
        private string tableName = @"T_SanPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_SanPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_SanPham]
           ([Ma]
           ,[Ten]
           ,[MaLoaiNguyenLieu]
           ,[MaSize]
           ,[MaThanhPham]
           ,[SuDung],[MaNL])
     VALUES
           (@Ma
           ,@Ten
           ,@MaLoaiNguyenLieu
           ,@MaSize
           ,@MaThanhPham
           ,@SuDung,@MaNL)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_SanPham]
   SET [Ten] = @Ten
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[MaSize] = @MaSize
      ,[MaThanhPham] = @MaThanhPham
      ,[SuDung] = @SuDung, [MaNL] = @MaNL
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from T_SanPham";

        public TSanPham()
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
