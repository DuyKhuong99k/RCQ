using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class MaNhanVienTheoNhomXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaNhanVienTheoNhomXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaNhanVienTheoNhomXepKhuon] WHERE [MaNhanVien]=@MaNhanVien and[MaNhom] =@MaNhom and [Ngay]=@Ngay";

        private readonly string qrInsert = @"INSERT INTO[dbo].[MaNhanVienTheoNhomXepKhuon]  ([MaNhanVien] ,[MaNhom] ,[Ngay], [TyLeHuong] ,[TyLeTru] ,[SoGio]) VALUES (@MaNhanVien,@MaNhom, @Ngay, @TyLeHuong ,@TyLeTru ,@SoGio)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MaNhanVienTheoNhomXepKhuon] SET [TyLeHuong] = @TyLeHuong, [TyLeTru] = @TyLeTru, [SoGio] = @SoGio WHERE [MaNhanVien]=@MaNhanVien and[MaNhom] =@MaNhom and [Ngay]=@Ngay";

        private readonly string qrGetAll = "Select * from MaNhanVienTheoNhomXepKhuon";

        public MaNhanVienTheoNhomXepKhuon(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public List<Models.Repos.Models.MaNhanVienTheoNhomXepKhuon> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from MaNhanVienTheoNhomXepKhuon Where Ngay = @ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaNhanVienTheoNhomXepKhuon>(
                            query,
                            new
                            {
                                ngay = dateTime.Date
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
    }
}
