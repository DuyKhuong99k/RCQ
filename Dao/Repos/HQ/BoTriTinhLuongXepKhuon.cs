using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class BoTriTinhLuonXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"BoTriTinhLuonXepKhuon";
        private readonly string qrDelete = @"Delete [dbo].[BoTriTinhLuonXepKhuon] WHERE [MaNhanVien] = @MaNhanVien and  [Ngay] = @Ngay and [MaCongViec] = @MaCongViec and [MaXuong] = @MaXuong""
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[BoTriTinhLuonXepKhuon] ([MaNhanVien] ,[Ngay] ,[MaCongViec] ,[MaXuong] ,[IsNhom],[TyLeHuong] ,[TyLeTru] ,[SoGio])  VALUES (@MaNhanVien,@Ngay,@MaCongViec,@MaXuong,@IsNhom,@TyLeHuong ,@TyLeTru ,@SoGio)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[BoTriTinhLuonXepKhuon] SET [IsNhom] = @IsNhom, [TyLeHuong] = @TyLeHuong,[TyLeTru] = @TyLeTru,[SoGio] = @SoGio WHERE [MaNhanVien] = @MaNhanVien and [Ngay] = @Ngay and [MaCongViec] = @MaCongViec and  [MaXuong] = @MaXuong
";

        private readonly string qrGetAll = "Select * from BoTriTinhLuonXepKhuon";


        public BoTriTinhLuonXepKhuon(string? _connectionString = null)
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
        public List<Models.Repos.Models.BoTriTinhLuonXepKhuon> GetNhanViens(DateTime dateTime, string xuongId, string congViecId)
        {
            try
            {
                var query =
                    "Select * from BoTriTinhLuonXepKhuon b where b.Ngay = @ngay and b.MaXuong = @xuongId and b.MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.BoTriTinhLuonXepKhuon>(
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
    }
}
