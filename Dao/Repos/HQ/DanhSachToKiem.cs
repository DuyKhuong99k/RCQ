using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class DanhSachToKiem
    {
        private readonly string connectionString;
        private string tableName = @"DanhSachToKiem";
        private readonly string qrDelete = @"Delete DanhSachToKiem where MaToKiem= @MaToKiem and MaXuong = @MaXuong and MaNhanVien = @MaNhanVien and Ngay = @Ngay and LoaiBoTri = @LoaiBoTri ";

        private readonly string qrInsert = @"
Insert Into DanhSachToKiem ([MaToKiem],[MaXuong],[MaNhanVien],[Ngay],[LoaiBoTri],[TyLe],[SoGio],[TyLeTru]) Values (@MaToKiem,@MaXuong,@MaNhanVien,@Ngay,@LoaiBoTri,@TyLe,@SoGio,@TyLeTru)";

        private readonly string qrUpdate = @"Update DanhSachToKiem Set SoGio =@SoGio, TyLe= @TyLe, TyLeTru =@TyLeTru where MaToKiem= @MaToKiem and MaXuong = @MaXuong and MaNhanVien = @MaNhanVien and Ngay = @Ngay and LoaiBoTri = @LoaiBoTri";

        private readonly string qrGetAll = "Select * from DanhSachToKiem";

        public DanhSachToKiem(string? _connectionString = null)
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
                    "Delete DanhSachToKiem where MaToKiem= @MaToKiem and MaXuong = @MaXuong and MaNhanVien = @MaNhanVien and Ngay = @Ngay and LoaiBoTri = @LoaiBoTri ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var affectedRows = connection.ExecuteAsync(query, entities, transaction).Result;
                        transaction.Commit();
                        return affectedRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                    "Insert Into DanhSachToKiem ([MaToKiem],[MaXuong],[MaNhanVien],[Ngay],[LoaiBoTri],[TyLe],[SoGio],[TyLeTru]) Values (@MaToKiem,@MaXuong,@MaNhanVien,@Ngay,@LoaiBoTri,@TyLe,@SoGio,@TyLeTru)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var affectedRows = connection.ExecuteAsync(query, entities, transaction).Result;
                        transaction.Commit();
                        return affectedRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
        public int Update<TEntity>(List<TEntity> entities)
        {
            try
            {
                var query =
                    "Update DanhSachToKiem Set SoGio =@SoGio, TyLe= @TyLe, TyLeTru =@TyLeTru where MaToKiem= @MaToKiem and MaXuong = @MaXuong and MaNhanVien = @MaNhanVien and Ngay = @Ngay and LoaiBoTri = @LoaiBoTri";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var affectedRows = connection.ExecuteAsync(query, entities, transaction).Result;
                        transaction.Commit();
                        return affectedRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Models.Repos.Models.DanhSachToKiem> GetDanhSachToKiemsByXuongIdNgay(string xuongId, DateTime dateTime, int type = 0)
        {
            try
            {
                var query = "Select * from DanhSachToKiem where MaXuong =@xuongId and Ngay=@ngay and LoaiBoTri=@type";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.DanhSachToKiem>(
                            query,
                            new { xuongId = xuongId, ngay = dateTime.Date, type = type })
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

        public DateTime? GetMaxDateByXuongId(string xuongId, int type = 0)
        {
            try
            {
                var query = "Select Max(Ngay) from DanhSachToKiem where MaXuong =@xuongId and LoaiBoTri=@type";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<DateTime?>(query, new { xuongId = xuongId, type = type })
                        .Result
                        .SingleOrDefault();
                    return item;
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
