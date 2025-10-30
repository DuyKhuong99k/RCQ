using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class GioVaoRaFillet
    {
        private readonly string connectionString;
        private string tableName = @"GioVaoRaFillet";
        private readonly string qrDelete = @"Delete GioVaoRaFillet where STT = @STT and Ngay= @Ngay and MaNhanVien = @MaNhanVien and MaCongViec= @MaCongViec and MaXuong = @MaXuong";

        private readonly string qrInsert = @"Insert Into [GioVaoRaFillet] ([STT],[MaNhanVien],[Ngay],[GioVao],[GioRa],[MaCongViec],[MaXuong]) VALUES (@STT,@MaNhanVien,@Ngay,@GioVao,@GioRa,@MaCongViec,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [GioVaoRaFillet] SET [GioVao] = @GioVao,[GioRa] = @GioRa WHERE [STT] = @STT and [MaNhanVien] = @MaNhanVien and [Ngay] = @Ngay and [MaCongViec] = @MaCongViec and MaXuong = @MaXuong";

        private readonly string qrGetAll = "Select * from GioVaoRaFillet";

        public GioVaoRaFillet(string? _connectionString = null)
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
        public int Delete(DateTime dateTime, string nhanVienId, string congViecId, string xuongId)
        {
            try
            {
                var query =
                    "Delete GioVaoRaFillet where Ngay= @ngay and MaNhanVien = @nhanVienId and MaCongViec= @congViecId and MaXuong=@xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                nhanVienId = nhanVienId,
                                congViecId = congViecId,
                                xuongId = xuongId
                            })
                        .Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> Gets(
            DateTime dateTime,
            string congViecId,
            IEnumerable<string> ids,
            string xuongId)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var query =
                    $@"SELECT * FROM GioVaoRaFillet where Ngay = @ngay and MaNhanVien in{listOfIdsJoined} and MaCongViec =@congViecId and MaXuong= @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.GioVaoRaFillet>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                congViecId = congViecId,
                                xuongId = xuongId
                            })
                        .Result
                        .ToList();
                    return items;
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
        public int Insert(Models.Repos.Models.GioVaoRaFillet gioVaoRaFillet)
        {
            try
            {
                var query =
                    "Insert Into [GioVaoRaFillet] ([STT],[MaNhanVien],[Ngay],[GioVao],[GioRa],[MaCongViec],[MaXuong]) VALUES (@STT,@MaNhanVien,@Ngay,@GioVao,@GioRa,@MaCongViec,@MaXuong)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.ExecuteAsync(query, gioVaoRaFillet).Result;
                    return item;
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
        public List<TimeSpan> GetListTime(DateTime dateTime, IEnumerable<string> ids, string maCongViec, string xuongId)
        {
            try
            {
                var listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var query =
                    $@"select * from ((SELECT GioVao as colmn FROM GioVaoRaFillet where  Ngay = @ngay and MaCongViec =@maCongViec and MaXuong =@xuongId and MaNhanVien in{listOfIdsJoined}) UNION (SELECT GioRa as colmn FROM GioVaoRaFillet where Ngay = @ngay and MaCongViec =@maCongViec and MaXuong =@xuongId  and MaNhanVien in{listOfIdsJoined})) g where g.colmn is not null order by colmn";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TimeSpan>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                maCongViec = maCongViec,
                                xuongId = xuongId
                            })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> GetGioVaoRaFillet(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            IEnumerable<string> ids,
            string congViecId,
            string xuongId)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var qurey =
                    $@"Select * from GioVaoRaFillet Where Ngay = @ngay and MaXuong = @xuongId and  GioVao <= @fromTime and GioRa >=@toTime and MaNhanVien in {listOfIdsJoined} and MaCongViec = @congViecId";
                using (var connnection = new SqlConnection(connectionString))
                {
                    connnection.Open();
                    var items = connnection.QueryAsync<Models.Repos.Models.GioVaoRaFillet>(
                            qurey,
                            new
                            {
                                ngay = dateTime.Date,
                                fromTime = fromTime,
                                toTime = toTime,
                                congViecId = congViecId,
                                xuongId = xuongId
                            })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> GetGioVaoRaFillet(
            string nhanVienId,
            DateTime dateTime,
            string congViecId,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from GioVaoRaFillet Where MaNhanVien = @nhanVienId and Ngay=@ngay and MaCongViec = @congViecId and MaXuong= @xuongId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.GioVaoRaFillet>(
                            query,
                            new
                            {
                                nhanVienId = nhanVienId,
                                ngay = dateTime.Date,
                                congViecId = congViecId,
                                xuongId = xuongId
                            })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Models.Repos.Models.GioVaoRaFillet> GetGioVaoRaFillet(DateTime dateTime, string xuongId)
        {
            try
            {
                var qurey = "Select * from GioVaoRaFillet Where Ngay = @ngay and MaXuong=@xuongId";
                using (var connnection = new SqlConnection(connectionString))
                {
                    connnection.Open();
                    var items = connnection.QueryAsync<Models.Repos.Models.GioVaoRaFillet>(
                            qurey,
                            new { ngay = dateTime.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
