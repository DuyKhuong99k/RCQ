using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class GioVaoRaTinhLuongXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"GioVaoRaTinhLuongXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[GioVaoRaTinhLuongXepKhuon] WHERE [STT] = @STT and [MaNhanVien] = @MaNhanVien and [Ngay] = @Ngay and [MaCongViec] = @MaCongViec and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[GioVaoRaTinhLuongXepKhuon] ([STT] ,[MaNhanVien] ,[Ngay] ,[GioVao] ,[GioRa] ,[MaCongViec] ,[MaXuong] ,[MaCa] ,[CreateDateTime] ,[CreateBy]  ,[ModifyDateTime] ,[ModifyBy] ,[SuDung] ,[GhiChu] ,[MaMayCan]) VALUES (@STT,@MaNhanVien,@Ngay,@GioVao, @GioRa,@MaCongViec,@MaXuong,@MaCa,@CreateDateTime ,@CreateBy  ,@ModifyDateTime ,@ModifyBy ,@SuDung ,@GhiChu ,@MaMayCan)";
        private readonly string qrUpdate = @"UPDATE [dbo].[GioVaoRaTinhLuongXepKhuon] SET  [GioVao] = @GioVao, [GioRa] = @GioRa ,[MaCa] =@MaCa, [CreateDateTime] = @CreateDateTime,[CreateBy] = @CreateBy,[ModifyDateTime] = @ModifyDateTime,[ModifyBy] = @ModifyBy,[SuDung] = @SuDung,[GhiChu] = @GhiChu,[MaMayCan] = @MaMayCan WHERE [STT] = @STT and [MaNhanVien] = @MaNhanVien and [Ngay] = @Ngay and [MaCongViec] = @MaCongViec and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from GioVaoRaTinhLuongXepKhuon";


        public GioVaoRaTinhLuongXepKhuon(string? _connectionString = null)
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
        public List<TimeSpan> GetListTime(DateTime dateTime, string xuongId, string congViecId, IEnumerable<string> ids)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var query =
                    $@"select * from ((SELECT GioVao as colmn FROM GioVaoRaTinhLuongXepKhuon where  Ngay = @ngay and MaXuong = @xuongId and MaCongViec = @congViecId and SuDung=1 and MaNhanVien in{listOfIdsJoined}) UNION (SELECT GioRa as colmn FROM GioVaoRaTinhLuongXepKhuon where Ngay = @ngay and MaXuong = @xuongId and MaCongViec = @congViecId and SuDung=1 and MaNhanVien in{listOfIdsJoined})) g where g.colmn is not null order by colmn";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TimeSpan>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                congViecId = congViecId
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
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> Gets(
           DateTime dateTime,
           TimeSpan fromTime,
           TimeSpan toTime,
           string xuongId,
           IEnumerable<string> ids,
           string congViecId)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var qurey =
                    $@"Select * from GioVaoRaTinhLuongXepKhuon Where Ngay = @ngay and GioVao <= @fromTime and GioRa >=@toTime and MaNhanVien in {listOfIdsJoined} and MaCongViec = @congViecId and MaXuong = @xuongId and SuDung=1";
                using (var connnection = new SqlConnection(connectionString))
                {
                    connnection.Open();
                    var items = connnection.QueryAsync<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon>(
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
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> Gets(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> ids)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var qurey =
                    $@"Select * from GioVaoRaTinhLuongXepKhuon Where Ngay = @ngay and GioVao <= @fromTime and GioRa >=@toTime and MaNhanVien in {listOfIdsJoined} and MaXuong = @xuongId and SuDung=1";
                using (var connnection = new SqlConnection(connectionString))
                {
                    connnection.Open();
                    var items = connnection.QueryAsync<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon>(
                            qurey,
                            new
                            {
                                ngay = dateTime.Date,
                                fromTime = fromTime,
                                toTime = toTime,
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
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> GetsTheoCa(
            DateTime dateTime,
            string xuongId,
            string congViecId,
            string caId)
        {
            try
            {
                var query =
                    "Select * from GioVaoRaTinhLuongXepKhuon where Ngay =@ngay and MaXuong= @xuongId and MaCongViec = @congViecId and MaCa = @caId and SuDung=1 order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                congViecId = congViecId,
                                caId = caId
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
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> GetsTheoCa(
            DateTime dateTime,
            string xuongId,
            string congViecId)
        {
            try
            {
                var query =
                    "Select * from GioVaoRaTinhLuongXepKhuon where Ngay =@ngay and MaXuong= @xuongId and MaCongViec = @congViecId and SuDung=1 order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                congViecId = congViecId
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
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from GioVaoRaTinhLuongXepKhuon Where Ngay=@ngay and MaXuong = @xuongId and SuDung=1";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon>(
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
