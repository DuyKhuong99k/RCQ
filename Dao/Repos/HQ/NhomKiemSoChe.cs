using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class NhomKiemSoChe
    {
        private readonly string connectionString;
        public NhomKiemSoChe(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public List<Models.Repos.Models.NhomKiemSoChe> Gets(string xuongId)
        {
            try
            {
                var query = "Select * from NhomKiemSoChe where MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhomKiemSoChe>(query, new { xuongId = xuongId }).Result
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
