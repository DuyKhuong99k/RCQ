using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class NhanVienSanLuongTheoLineFillet
    {
        private readonly string connectionString;
        public NhanVienSanLuongTheoLineFillet(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public List<Models.Repos.Models.NhanVienSanLuongTheoLineFillet> GetNhanViens(DateTime dateTime, string lineId, string xuongId)
        {
            try
            {
                var query =
                    "Select * from NhanVienSanLuongTheoLineFillet where Ngay = @ngay and MaXuong = @xuongId and MaLine = @lineId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienSanLuongTheoLineFillet>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                lineId = lineId,
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
