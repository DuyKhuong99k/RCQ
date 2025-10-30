using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public class HQ_PhieuThongKeSanXuat
    {
        private readonly string connectionString;
        private string tableName = @"HQ_PhieuThongKeSanXuat";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_PhieuThongKeSanXuat] WHERE [Id] = @Id
        ";

        private readonly string qrInsert = @"
            INSERT INTO [dbo].[HQ_PhieuThongKeSanXuat]
            ([Ngay]
           ,[SoChungTu]
           ,[Ca]
           ,[MaTo]
           ,[TenTo]
           ,[MaNhanVien]
           ,[TenNhanVien]
           ,[MaCongViec]
           ,[TenCongViec]
           ,[SanLuong]
           ,[GioBatDau]
           ,[GioKetThuc]
           ,[NgayGioTao])
            VALUES
            (@Ngay
           ,@SoChungTu
           ,@Ca
           ,@MaTo
           ,@TenTo
           ,@MaNhanVien
           ,@TenNhanVien
           ,@MaCongViec
           ,@TenCongViec
           ,@SanLuong
           ,@GioBatDau
           ,@GioKetThuc
           ,@NgayGioTao)
        ";

        private readonly string qrUpdate = @"
        UPDATE [dbo].[HQ_PhieuThongKeSanXuat]
        SET [Ngay] = @Ngay
      ,[SoChungTu] = @SoChungTu
      ,[Ca] = @Ca
      ,[MaTo] = @MaTo
      ,[TenTo] = @TenTo
      ,[MaNhanVien] = @MaNhanVien
      ,[TenNhanVien] = @TenNhanVien
      ,[MaCongViec] = @MaCongViec
      ,[TenCongViec] = @TenCongViec
      ,[SanLuong] = @SanLuong,
      ,[GioBatDau] = @GioBatDau
      ,[GioKetThuc] = @GioKetThuc
      ,[NgayGioTao] = @NgayGioTao
        WHERE [Id] = @Id
        ";

        private readonly string qrGetAll = "Select * from HQ_PhieuThongKeSanXuat";

        public HQ_PhieuThongKeSanXuat(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            var query = @"select * from HQ_PhieuThongKeSanXuat p where p.Ngay = @ngay";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date })
                    .Result
                    .ToList();
                return items;
            }
        }
        public int DeleteList<T>(List<T> items, DateTime ngay)
        {
            if (items == null || !items.Any())
                return 0;

            string query = $@"DELETE FROM HQ_PhieuThongKeSanXuat WHERE Ngay = @ngay";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute(
                    query,
                    new { ngay = ngay.Date }
                );

                return affectedRows;
            }
        }

        // Hàm trợ giúp để lấy giá trị của một thuộc tính bằng tên
        private object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Property {propertyName} không tồn tại trong đối tượng {obj.GetType().Name}");

            return propertyInfo.GetValue(obj);
        }
        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }
        public int Insert<T>(List<T> items)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var rows = connection.Execute(qrInsert, items);
                return rows;
            }
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
