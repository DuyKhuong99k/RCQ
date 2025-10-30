using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class NhomSoCheDinhHinh
    {
        private readonly string connectionString;
        public NhomSoCheDinhHinh(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public List<Models.Repos.Models.NhomSoCheDinhHinh> Gets(string xuongId)
        {
            try
            {
                var query = "Select * from NhomSoCheDinhHinh where MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhomSoCheDinhHinh>(query, new { xuongId = xuongId }).Result
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
        public List<Models.Repos.Models.NhomSoCheDinhHinh> Gets(string xuongId, bool isChinh)
        {
            try
            {
                var query = "Select * from NhomSoCheDinhHinh where MaXuong = @xuongId and IsNhomChinh = @isChinh";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhomSoCheDinhHinh>(
                            query,
                            new { xuongId = xuongId, isChinh = isChinh })
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
