using Dapper;
using Microsoft.Data.SqlClient;


namespace Dao.Repos.HQ
{
    public partial class HQ_ThucDon
    {
        private readonly string connectionString;
        private string tableName = @"HQ_ThucDon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_ThucDon]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_ThucDon]
           (
           [Ngay]
           ,[NgayTao]
           ,[NguoiTao]
           ,[Ten]
           ,[ThietBi]
           ,[GhiChu])
     VALUES
           (
           @Ngay
           ,@NgayTao
           ,@NguoiTao
           ,@Ten
           ,@ThietBi
           ,@GhiChu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_ThucDon]
   SET [Ngay] =@Ngay
           ,[NgayTao] = @NgayTao
           ,[NguoiTao] = @NguoiTao
           ,[Ten] = @Ten
           ,[ThietBi] = @ThietBi
           ,[GhiChu] = @GhiChu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_ThucDon";

        public HQ_ThucDon()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

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

        public int GetMaxId()
        {
            try
            {
                var query = @"SELECT TOP 1 
p.Id 
FROM HQ_ThucDon p 
ORDER BY p.Id DESC";

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var maxId = connection.QueryFirstOrDefault<int>(query);
                return maxId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetDanhSachThucDon<T>(DateTime ngay)
        {
            try
            {
                var query = @"SELECT 
    td.Id as ThucDonId,
    td.Ngay AS NgayThucDon,
    td.Ten AS TenThucDon,
    td.NgayTao,
    td.NguoiTao,
    td.GhiChu,
    td.ThietBi,
    CASE 
        WHEN htd.ThucDonId IS NOT NULL THEN 2 -- Ưu tiên trạng thái bị hủy
        WHEN dtd.ThucDonId IS NOT NULL THEN 1 
        ELSE 0 -- chưa duyệt
    END AS TrangThaiThucDon
FROM HQ_ThucDon td
LEFT JOIN HQ_DuyetThucDon dtd ON td.Id = dtd.ThucDonId
LEFT JOIN HQ_HuyThucDon htd ON td.Id = htd.ThucDonId
WHERE td.Ngay = @ngay
ORDER BY td.NgayTao DESC

";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = ngay.Date}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetChiTietTheoThucDon<T>(int thucDonId)
        {
            try
            {
                var query = @"select
ct.Id,
ct.ThucDonId,
td.Ngay as NgayThucDon,
td.NgayTao as NgayTaoThucDon,
td.NguoiTao as NguoiTaiThucDon,
td.ThietBi,
td.Ten as TenThucDon,
ct.MonAnId,
ma.Ten as TenMonAn,
ma.LoaiMonAnId,
lma.Ten as TenLoaiMonAn,
ct.GhiChu
from HQ_ThucDonChiTiet ct
left join HQ_ThucDon td on td.Id = ct.ThucDonId
left join HQ_MonAn ma on ma.Id = ct.MonAnId
left join HQ_LoaiMonAn lma on lma.Id = ma.LoaiMonAnId
where ct.ThucDonId = @thucDonId
order by 
td.Ngay desc,
ct.Id desc,
td.NgayTao desc

";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { thucDonId = thucDonId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetDanhSachTrangThaiThucDonTungNgay<T>()
        {
            try
            {
                var query = @" ;WITH
DuyetCount AS (
    SELECT ThucDonId, COUNT(*) AS SoLuongDuyet
    FROM HQ_DuyetThucDon
    GROUP BY ThucDonId
),
HuyCount AS (
    SELECT ThucDonId, COUNT(*) AS SoLuongHuy
    FROM HQ_HuyThucDon
    GROUP BY ThucDonId
),
ChuaDuyetCount AS (
    SELECT
        td.Id,
        COUNT(*) AS SoLuongChuaDuyet
    FROM HQ_ThucDon td
    LEFT JOIN HQ_DuyetThucDon dtd ON td.Id = dtd.ThucDonId
    LEFT JOIN HQ_HuyThucDon htd ON td.Id = htd.ThucDonId
    WHERE dtd.ThucDonId IS NULL AND htd.ThucDonId IS NULL
    GROUP BY td.Id
)
SELECT
    td.Ngay AS NgayThucDon,
    MAX(td.NgayTao) AS NgayTao,
    MAX(td.NguoiTao) AS NguoiTao,
    MAX(td.GhiChu) AS GhiChu,
    MAX(td.ThietBi) AS ThietBi,
    CASE
        -- Có cả hủy và duyệt => ưu tiên duyệt, trả 1
        WHEN SUM(CASE WHEN hc.SoLuongHuy > 0 THEN 1 ELSE 0 END) > 0
         AND SUM(CASE WHEN dc.SoLuongDuyet > 0 THEN 1 ELSE 0 END) > 0 THEN 1
        
        -- Có hủy, không có duyệt
        WHEN SUM(CASE WHEN hc.SoLuongHuy > 0 THEN 1 ELSE 0 END) > 0
         AND SUM(CASE WHEN dc.SoLuongDuyet > 0 THEN 1 ELSE 0 END) = 0 THEN
            CASE 
                WHEN SUM(CASE WHEN cdc.SoLuongChuaDuyet > 0 THEN 1 ELSE 0 END) > 0 THEN 0  -- còn chưa duyệt
                ELSE 2     -- không còn chưa duyệt, chỉ hủy
            END
        
        -- Có duyệt, không có hủy
        WHEN SUM(CASE WHEN dc.SoLuongDuyet > 0 THEN 1 ELSE 0 END) > 0
         AND SUM(CASE WHEN hc.SoLuongHuy > 0 THEN 1 ELSE 0 END) = 0 THEN 1
        
        -- Không có hủy và duyệt => 0 (chưa duyệt)
        ELSE 0
    END AS TrangThaiThucDon
FROM HQ_ThucDon td
LEFT JOIN DuyetCount dc ON td.Id = dc.ThucDonId
LEFT JOIN HuyCount hc ON td.Id = hc.ThucDonId
LEFT JOIN ChuaDuyetCount cdc ON td.Id = cdc.Id
GROUP BY td.Ngay
ORDER BY td.Ngay DESC;
";
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
