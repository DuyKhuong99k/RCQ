using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class NhanVienPhuTheoBanFillet
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienPhuTheoBanFillet";
        private readonly string qrDelete = @"Delete NhanVienPhuTheoBanFillet Where  [MaNhanVien] = @MaNhanVien and [Ngay] =@Ngay and [MaBanCatTiet] = @MaBanCatTiet and  [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"
INSERT INTO [NhanVienPhuTheoBanFillet] ([MaNhanVien],[Ngay],[MaBanCatTiet],[MaXuong]) VALUES (@MaNhanVien,@Ngay,@MaBanCatTiet ,@MaXuong)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[NhanVienPhuTheoBanFillet]
   SET 
      [MaBanCatTiet] = @MaBanCatTiet,[MaXuong] =@MaXuong
 WHERE [MaNhanVien] = @MaNhanVien and [Ngay] =@Ngay and [MaBanCatTiet] = @MaBanCatTiet and  [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from NhanVienPhuTheoBanFillet";


        public NhanVienPhuTheoBanFillet(string? _connectionString = null)
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
                    "Delete NhanVienPhuTheoBanFillet Where  [MaNhanVien] = @MaNhanVien and [Ngay] =@Ngay and [MaBanCatTiet] = @MaBanCatTiet and  [MaXuong] = @MaXuong";
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
        public List<Models.Repos.Models.NhanVienPhuTheoBanFillet> Gets(DateTime dateTime, string xuongId, string banId)
        {
            try
            {
                var query =
                    "Select * from NhanVienPhuTheoBanFillet Where Ngay=@ngay and MaXuong = @xuongId and MaBanCatTiet =@banId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienPhuTheoBanFillet>(
                            query,
                            new
                            {
                                ngay = dateTime,
                                xuongId = xuongId,
                                banId = banId
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
                    "INSERT INTO [NhanVienPhuTheoBanFillet] ([MaNhanVien],[Ngay],[MaBanCatTiet],[MaXuong]) VALUES (@MaNhanVien,@Ngay,@MaBanCatTiet ,@MaXuong)";
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
    }
}
