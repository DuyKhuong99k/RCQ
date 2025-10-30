using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BT_NhanVienTheoNhom
    {
        private readonly string connectionString;
        private string tableName = @"BT_NhanVienTheoNhom";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BT_NhanVienTheoNhom]
      WHERE [MaNhanVien] = @MaNhanVien 
      and [MaNhom] = @MaNhom 
      and [Ngay] = @Ngay
      and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"
INSERT INTO[dbo].[BT_NhanVienTheoNhom]
           ([MaNhanVien]
           ,[MaNhom]
           ,[Ngay]
           ,[TyLeTru]
           ,[TyLeHuong],[SoGio],[MaXuong])
     VALUES
           (@MaNhanVien 
           ,@MaNhom 
           ,@Ngay
           ,@TyLeTru 
           ,@TyLeHuong,@SoGio,@MaXuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BT_NhanVienTheoNhom]
   SET [TyLeTru] = @TyLeTru 
      ,[TyLeHuong] = @TyLeHuong, [SoGio]= @SoGio  
 WHERE [MaNhanVien] = @MaNhanVien 
      and [MaNhom] = @MaNhom 
      and [Ngay] = @Ngay 
      and [MaXuong] = @MaXuong
";

        private readonly string qrGetAll = "Select * from BT_NhanVienTheoNhom";
        private readonly string qrGetByDateXuong = "Select * from BT_NhanVienTheoNhom Where Ngay=@ngay and MaXuong = @xuongId";
        private readonly string qrGetByDateXuongNhanVienNhom = "Select * from BT_NhanVienTheoNhom Where Ngay=@ngay and MaXuong = @xuongId and MaNhom = @nhomId and MaNhanVien = @nhanVienId";

        public BT_NhanVienTheoNhom()
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
        public T Get<T>(DateTime dateTime, string nhanVienId, string nhomId, string xuongId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByDateXuongNhanVienNhom, new { ngay = dateTime.Date, xuongId, nhomId, nhanVienId }).SingleOrDefault();
            return rows;
        }
        public List<T> Gets<T>(DateTime dateTine, string xuongId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll, new { ngay = dateTine.Date, xuongId }).ToList();
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = "Select * from BT_NhanVienTheoNhom where Ngay = @ngay ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
