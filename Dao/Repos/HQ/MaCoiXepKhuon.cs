using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaCoiXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaCoiXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaCoiXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaCoiXepKhuon]
           ([Ma]
           ,[Ten]
           ,[TrongLuongMax]
           ,[Tam])
     VALUES
           (@Ma 
           ,@Ten 
           ,@TrongLuongMax 
           ,@Tam)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaCoiXepKhuon]
   SET [Ten] = @Ten  
      ,[TrongLuongMax] = @TrongLuongMax 
      ,[Tam] = @Tam
 WHERE  [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaCoiXepKhuon";
        private readonly string qrGetCoiTams = @"
        SELECT c.Ma as MaCoi,
            xn.Ma as MaXuong
        from (
                Select *
                from MaCoiXepKhuon
                where Tam = 1
            ) c
            LEFT join XiNghiep xn on 1 = 1";

        private readonly string qrGetLatestWeightPerXuongAndCoi = @"WITH NgayGanNhat AS (
    SELECT TOP 4 CAST(Ngay AS date) AS NgayCan
    FROM PhieuCanChinhXepKhuon
    WHERE ISNULL(MaCoiChinh, '') != ''
    GROUP BY CAST(Ngay AS date)
    ORDER BY CAST(Ngay AS date) DESC
),
PhieuCan_CTE AS (
    SELECT 
        MaXuong,
        MaCoiChinh AS MaCoi,
        CAST(Ngay AS datetime) + CAST(Gio AS datetime) AS ThoiGianCan,
        TrongLuong
    FROM PhieuCanChinhXepKhuon
    WHERE 
        ISNULL(MaCoiChinh, '') != ''
        AND CAST(Ngay AS date) IN (SELECT NgayCan FROM NgayGanNhat)
),
PhieuCan_Lag AS (
    SELECT 
        *,
        LAG(ThoiGianCan) OVER (PARTITION BY MaXuong, MaCoi ORDER BY ThoiGianCan) AS ThoiGianTruoc
    FROM PhieuCan_CTE
),
PhieuCan_DanhDau AS (
    SELECT *,
        CASE 
            WHEN DATEDIFF(MINUTE, ThoiGianTruoc, ThoiGianCan) > 30 OR ThoiGianTruoc IS NULL THEN 1 
            ELSE 0 
        END AS NhomMoi
    FROM PhieuCan_Lag
),
PhieuCan_DanhSoNhom AS (
    SELECT *,
        SUM(NhomMoi) OVER (PARTITION BY MaXuong, MaCoi ORDER BY ThoiGianCan ROWS UNBOUNDED PRECEDING) AS NhomSo
    FROM PhieuCan_DanhDau
),
PhieuCan_NhomGanNhat AS (
    SELECT 
        MaXuong,
        MaCoi,
        NhomSo,
        MAX(ThoiGianCan) AS ThoiGianGanNhat,
        SUM(TrongLuong) AS TongTrongLuong,
		COUNT(*) AS SoRo
    FROM PhieuCan_DanhSoNhom
    GROUP BY MaXuong, MaCoi, NhomSo
),
PhieuCan_ChonMoiNhat AS (
    SELECT *,
        ROW_NUMBER() OVER (PARTITION BY MaXuong, MaCoi ORDER BY ThoiGianGanNhat DESC) AS rn
    FROM PhieuCan_NhomGanNhat
)
SELECT MaXuong, MaCoi as Ma, ThoiGianGanNhat as ThoiGianPhieuGanNhat, TongTrongLuong as TrongLuongHienTai,SoRo
FROM PhieuCan_ChonMoiNhat
WHERE rn = 1
ORDER BY MaXuong, MaCoi;
";
        private readonly string qrGetLatestWeightPerXuongAndCoiRa = @"WITH NgayGanNhat AS (
    SELECT TOP 4 CAST(Ngay AS date) AS NgayCan
    FROM PhieuCanRaCoi
    WHERE ISNULL(MaCoi, '') != ''
    GROUP BY CAST(Ngay AS date)
    ORDER BY CAST(Ngay AS date) DESC
),
PhieuCan_CTE AS (
    SELECT 
        MaXuong,
         MaCoi,
        CAST(Ngay AS datetime) + CAST(Gio AS datetime) AS ThoiGianCan,
        TrongLuong
    FROM PhieuCanRaCoi
    WHERE 
        ISNULL(MaCoi, '') != ''
        AND CAST(Ngay AS date) IN (SELECT NgayCan FROM NgayGanNhat)
),
PhieuCan_Lag AS (
    SELECT 
        *,
        LAG(ThoiGianCan) OVER (PARTITION BY MaXuong, MaCoi ORDER BY ThoiGianCan) AS ThoiGianTruoc
    FROM PhieuCan_CTE
),
PhieuCan_DanhDau AS (
    SELECT *,
        CASE 
            WHEN DATEDIFF(MINUTE, ThoiGianTruoc, ThoiGianCan) > 30 OR ThoiGianTruoc IS NULL THEN 1 
            ELSE 0 
        END AS NhomMoi
    FROM PhieuCan_Lag
),
PhieuCan_DanhSoNhom AS (
    SELECT *,
        SUM(NhomMoi) OVER (PARTITION BY MaXuong, MaCoi ORDER BY ThoiGianCan ROWS UNBOUNDED PRECEDING) AS NhomSo
    FROM PhieuCan_DanhDau
),
PhieuCan_NhomGanNhat AS (
    SELECT 
        MaXuong,
        MaCoi,
        NhomSo,
        MAX(ThoiGianCan) AS ThoiGianGanNhat,
        SUM(TrongLuong) AS TongTrongLuong,
		COUNT(*) AS SoRo
    FROM PhieuCan_DanhSoNhom
    GROUP BY MaXuong, MaCoi, NhomSo
),
PhieuCan_ChonMoiNhat AS (
    SELECT *,
        ROW_NUMBER() OVER (PARTITION BY MaXuong, MaCoi ORDER BY ThoiGianGanNhat DESC) AS rn
    FROM PhieuCan_NhomGanNhat
)
SELECT MaXuong, MaCoi as Ma, ThoiGianGanNhat as ThoiGianPhieuGanNhat, TongTrongLuong as TrongLuongHienTai,SoRo
FROM PhieuCan_ChonMoiNhat
WHERE rn = 1
ORDER BY MaXuong, MaCoi
";

        public MaCoiXepKhuon()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetCoiTams<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetCoiTams).ToList();
            return rows;
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
        public List<T> GetLatestWeightPerXuongAndCoi<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetLatestWeightPerXuongAndCoi).ToList();
            return rows;
        }public List<T> GetLatestWeightPerXuongAndCoiRa<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetLatestWeightPerXuongAndCoiRa).ToList();
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
