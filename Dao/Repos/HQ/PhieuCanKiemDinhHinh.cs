using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public class PhieuCanKiemDinhHinh
    {
        private readonly string connectionString;
        public PhieuCanKiemDinhHinh(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public int GetCountByDate(DateTime dateTime)
        {
            try
            {
                var query = "Select Count(*) from PhieuCanKiemDinhHinh Where Ngay=@ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryAsync<int>(query, new { ngay = dateTime.Date }).Result
                        .ToList()
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

        public List<TEntity> GetByDate<TEntity>(DateTime dateTime, string bravoId)
        {
            try
            {
                var query = "Select * from PhieuCanKiemDinhHinh Where Ngay=@ngay and MaSanPham = @bravoId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, bravoId = bravoId })
                        .Result
                        .ToList();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<TEntity> GetByDate<TEntity>(DateTime dateTime, IEnumerable<string> bravoIds)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", bravoIds.ToArray()) + "')";
                var query = $@"Select * from PhieuCanKiemDinhHinh Where Ngay=@ngay and MaSanPham in {listOfIdsJoined}";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date }).Result.ToList();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Insert<TEntity>(List<TEntity> phieuCans)
        {
            try
            {
                var query =
                    "INSERT INTO PhieuCanKiemDinhHinh (Ngay,CaLamViec,MaNhanVien,MaSanPham,TenSanPham,TrongLuong,_Status,KhuVuc) values (@Ngay,@CaLamViec,@MaNhanVien,@MaSanPham,@TenSanPham,@TrongLuong,@_Status,@KhuVuc)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var affectedRows = connection.Execute(query, phieuCans, transaction);
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
    }
}
