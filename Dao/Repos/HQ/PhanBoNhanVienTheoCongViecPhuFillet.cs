using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class PhanBoNhanVienTheoCongViecPhuFillet
    {
        private readonly string connectionString;
        private string tableName = @"PhanBoNhanVienTheoCongViecPhuFillet";
        private readonly string qrDelete = "Delete PhanBoNhanVienTheoCongViecPhuFillet Where  [MaNhanVien]=@MaNhanVien and [MaCongViecPhuFillet] =@MaCongViecPhuFillet and [Ngay]=@Ngay and [MaXuong] =@MaXuong";

        private readonly string qrInsert = @"
Insert into PhanBoNhanVienTheoCongViecPhuFillet  ([MaNhanVien],[MaCongViecPhuFillet],[Ngay],[MaXuong],[TyLeHuong],[TyLeTru]) VALUES (@MaNhanVien,@MaCongViecPhuFillet,@Ngay,@MaXuong,@TyLeHuong,@TyLeTru)";
        private readonly string qrUpdate = @"
Update PhanBoNhanVienTheoCongViecPhuFillet set [TyLeHuong] = @TyLeHuong ,[TyLeTru] =@TyLeTru where [MaNhanVien]=@MaNhanVien and  [MaCongViecPhuFillet]=@MaCongViecPhuFillet and [Ngay]=@Ngay and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from PhanBoNhanVienTheoCongViecPhuFillet";


        public PhanBoNhanVienTheoCongViecPhuFillet(string? _connectionString = null)
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
        public List<Models.Repos.Models.PhanBoNhanVienTheoCongViecPhuFillet> Get(string thanhPhamId, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhanBoNhanVienTheoCongViecPhuFillet where Ngay= @ngay and MaCongViecPhuFillet = @thanhPhamId and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhanBoNhanVienTheoCongViecPhuFillet>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                thanhPhamId = thanhPhamId,
                                xuongId = xuongId
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
        public List<Models.Repos.Models.PhanBoNhanVienTheoCongViecPhuFillet> Get(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from PhanBoNhanVienTheoCongViecPhuFillet where Ngay= @ngay and MaXuong =@xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhanBoNhanVienTheoCongViecPhuFillet>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId
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
