using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class CongViecTinhLuongXepKhuonTheoLoaiThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"CongViecTinhLuongXepKhuonTheoLoaiThanhPham";
        private readonly string qrDelete = @"DELETE FROM[dbo].[CongViecTinhLuongXepKhuonTheoLoaiThanhPham] WHERE  [MaCongViec] =@MaCongViec and [MaThanhPhamDinhHinh] =@MaThanhPhamDinhHinh and [MaThanhPhamPhuXepKhuon] = @MaThanhPhamPhuXepKhuon and [MaThanhPhamChinhXepKhuon] =@MaThanhPhamChinhXepKhuon and [MaThanhPhamBlockXepKhuon] = @MaThanhPhamBlockXepKhuon and [MaThanhPhamKHCXepKhuon] = @MaThanhPhamKHCXepKhuon and [MaThanhPhamTaiChe]=@MaThanhPhamTaiChe and [MaThanhPhamSoChe] = @MaThanhPhamSoChe and [MaCongViecTaiChe] = @MaCongViecTaiChe and [MaCongDoan] = @MaCongDoan and [MaChieuXaChinhXepKhuon] = @MaChieuXaChinhXepKhuon";

        private readonly string qrInsert = @"INSERT INTO[dbo].[CongViecTinhLuongXepKhuonTheoLoaiThanhPham] ([MaCongViec] ,[MaThanhPhamDinhHinh] ,[MaThanhPhamPhuXepKhuon] ,[MaThanhPhamChinhXepKhuon] ,[MaThanhPhamBlockXepKhuon] ,[MaThanhPhamKHCXepKhuon] ,[MaThanhPhamTaiChe],[MaThanhPhamSoChe],[MaCongViecTaiChe],[MaCongDoan],[MaChieuXaChinhXepKhuon]) VALUES (@MaCongViec,@MaThanhPhamDinhHinh,@MaThanhPhamPhuXepKhuon,@MaThanhPhamChinhXepKhuon,@MaThanhPhamBlockXepKhuon,@MaThanhPhamKHCXepKhuon,@MaThanhPhamTaiChe,@MaThanhPhamSoChe,@MaCongViecTaiChe,@MaCongDoan,@MaChieuXaChinhXepKhuon) ";

        private readonly string qrUpdate = @"";

        private readonly string qrGetAll = "Select * from CongViecTinhLuongXepKhuonTheoLoaiThanhPham";


        public CongViecTinhLuongXepKhuonTheoLoaiThanhPham(string? _connectionString = null)
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
        public List<string> GetThanhPhamIdsPhuXepKhuon(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaThanhPhamPhuXepKhuon from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<string, string>> GetThanhPhamChieuXaIdsChinhXepKhuon(string congViecId)
        {
            try
            {
                var query =
                    "Select ISNull(MaThanhPhamChinhXepKhuon,'') As Item1,IsNull(MaChieuXaChinhXepKhuon,'') as Item2 from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId Group By MaThanhPhamChinhXepKhuon,MaChieuXaChinhXepKhuon";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<Tuple<string, string>>(query, new { congViecId = congViecId })
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsBlockXepKhuon(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaThanhPhamBlockXepKhuon from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsKHCXepKhuon(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaThanhPhamKHCXepKhuon from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsTaiChe(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaThanhPhamTaiChe from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetConViecTaiCheIds(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaCongViecTaiChe from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<string, string>> GetThanhPhamCongDoanIdsBlockXepKhuon(string congViecId)
        {
            try
            {
                var query =
                    "Select ISNull(MaThanhPhamBlockXepKhuon,'') As Item1,IsNull(MaCongDoan,'') as Item2 from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId Group By MaThanhPhamBlockXepKhuon,MaCongDoan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<Tuple<string, string>>(query, new { congViecId = congViecId })
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsTPDinhHinh(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaThanhPhamDinhHinh from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsSoChe(string congViecId)
        {
            try
            {
                var query =
                    "Select distinct MaThanhPhamSoChe from CongViecTinhLuongXepKhuonTheoLoaiThanhPham where MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<string>(query, new { congViecId = congViecId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
p.MaCongViec,
cv.Ten as CongViecName,
p.MaThanhPhamDinhHinh,
dh.Ten as ThanhPhamDinhHinhName,
p.MaThanhPhamPhuXepKhuon,
pxk.Ten as ThanhPhamPhuXepKhuonName,
p.MaThanhPhamChinhXepKhuon,
cxk.Ten as ThanhPhamChinhXepKhuonName,
p.MaThanhPhamBlockXepKhuon,
bxk.Ten as BlockXepKhuonName,
p.MaThanhPhamKHCXepKhuon,
khcxk.Ten as KHCXepKhuonName,
p.MaThanhPhamTaiChe,
tc.Ten as TaiCheName,
p.MaThanhPhamSoChe,
sc.Ten as SoCheName,
p.MaCongViecTaiChe,
cvtc.Ten as CongViecTaiCheName,
p.MaCongDoan,
cd.Ten as CongDoanName,
p.MaChieuXaChinhXepKhuon,
cx.Ten as ChieuXaName
from CongViecTinhLuongXepKhuonTheoLoaiThanhPham p
left join CongViecTinhLuongXepKhuon cv on cv.Ma = p.MaCongViec
left join MaThanhPhamDinhHinh dh on dh.Ma = p.MaThanhPhamDinhHinh
left join MaThanhPhamXepKhuon pxk on pxk.Ma = p.MaThanhPhamPhuXepKhuon
left join MaThanhPhamChinhXepKhuon cxk on cxk.Ma = p.MaThanhPhamChinhXepKhuon
left join MaThanhPhamXepKhuonBlock bxk on bxk.Ma = p.MaThanhPhamBlockXepKhuon
left join MaThanhPhamXepKhuonKHC khcxk on khcxk.Ma = p.MaThanhPhamKHCXepKhuon
left join MaThanhPhamTaiChe tc on tc.Ma = p.MaThanhPhamTaiChe
left join MaThanhPhamSoCheDinhHinh sc on sc.Ma = p.MaThanhPhamSoChe
left join MaCongViecTaiChe cvtc on cvtc.Ma = p.MaCongViecTaiChe
left join MaCongDoanXepKhuon cd on cd.Ma = p .MaCongDoan
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXaChinhXepKhuon";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
