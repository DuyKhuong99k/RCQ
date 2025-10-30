using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;
namespace Dao.Repos.HQ
{
    public partial class ToKiem
    {
        private readonly string connectionString;
        private string tableName = @"ToKiem";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"

";

        private readonly string qrUpdate = @"

";

        private readonly string qrGetAll = "Select * from ToKiem";

        public ToKiem(string? _connectionString = null)
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

        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
        public List<Models.Repos.Models.ToKiem> GetKiemsByXuongId(string xuongId)
        {
            try
            {
                var query = "Select * from ToKiem where MaXuong =@xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.ToKiem>(query, new { xuongId = xuongId }).Result.ToList();
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
