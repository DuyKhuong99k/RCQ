using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class NhanVienPhuTheoLine
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienPhuTheoLine";
        private readonly string qrDelete = @"Delete NhanVienPhuTheoLine Where  [MaNhanVien] = @MaNhanVien and [Ngay] =@Ngay and [MaLine] = @MaLine and  [MaXuong] = @MaXuong and [MaCongViec]= @MaCongViec";

        private readonly string qrInsert = @"
INSERT INTO NhanVienPhuTheoLine ([MaNhanVien],[Ngay],[MaLine],[MaXuong],[MaCongViec]) VALUES (@MaNhanVien,@Ngay,@MaLine ,@MaXuong,@MaCongViec)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[NhanVienPhuTheoLine]
   SET [MaLine] = @MaLine
      ,[MaCongViec] = @MaCongViec
 WHERE [MaNhanVien] = @MaNhanVien and [Ngay] =@Ngay and [MaLine] = @MaLine and  [MaXuong] = @MaXuong and [MaCongViec]= @MaCongViec";

        private readonly string qrGetAll = "Select * from NhanVienPhuTheoLine";


        public NhanVienPhuTheoLine(string? _connectionString = null)
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
        public int Delete<TEntity>(List<TEntity> entities)
        {
            try
            {
                var query =
                    "Delete NhanVienPhuTheoLine Where  [MaNhanVien] = @MaNhanVien and [Ngay] =@Ngay and [MaLine] = @MaLine and  [MaXuong] = @MaXuong and [MaCongViec]= @MaCongViec";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.ExecuteAsync(query, entities).Result;
                    return affectedRows;
                }
            }
            catch (Exception)
            {
                throw;
            }
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
        public int Insert<TEntity>(List<TEntity> entities)
        {
            try
            {
                var query =
                    "INSERT INTO NhanVienPhuTheoLine ([MaNhanVien],[Ngay],[MaLine],[MaXuong],[MaCongViec]) VALUES (@MaNhanVien,@Ngay,@MaLine ,@MaXuong,@MaCongViec)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.ExecuteAsync(query, entities).Result;
                    return affectedRows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
        public List<Models.Repos.Models.NhanVienPhuTheoLine> GetNhanviens(
            DateTime dateTime,
            string xuongId,
            string lineId,
            string congViecId)
        {
            try
            {
                var query =
                    "Select * from NhanVienPhuTheoLine Where Ngay=@ngay and MaXuong = @xuongId and MaLine = @lineId and MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienPhuTheoLine>(
                            query,
                            new
                            {
                                ngay = dateTime,
                                xuongId = xuongId,
                                lineId = lineId,
                                congViecId
                            })
                        .Result
                        .ToList();
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
