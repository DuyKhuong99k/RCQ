using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NhanVienTheoLine
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienTheoLine";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NhanVienTheoLine] WHERE Id = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NhanVienTheoLine]
           ([Id]
           ,[MaLine]
           ,[MaNhanVien]
           ,[MaViTri]
           ,[Ngay]
           ,[Gio],[CodeId])
     VALUES
           (@Id
           ,@MaLine
           ,@MaNhanVien
           ,@MaViTri
           ,@Ngay 
           ,@Gio,@CodeId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NhanVienTheoLine]
   SET [Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[MaNhanVien] = @MaNhanVien
      ,[MaLine] = @MaLine
      ,[MaViTri] = @MaViTri
      ,[CodeId] = @CodeId
 WHERE Id = @Id";

        private readonly string qrGetAll = "Select * from NhanVienTheoLine";

        public NhanVienTheoLine()
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

        public List<T> GetNhanVienTheoLineViTri<T>(DateTime ngay,string maLine, string maViTri)
        {
            try
            {
                var query = @"
select 
        p.Id,
                p.CodeId,
                p.Ngay,
                p.Gio,
                p.MaNhanVien,
				nv.MaHoSo,
                nv.Name as NhanVienName,
				nv.DeptName0 as NhomName,
                p.MaLine,
                l.Ten as LineName,
                p.MaViTri,
                vt.Ten as ViTriName
                from NhanVienTheoLine p 
                left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
                left join LineFilletv2 l on p.MaLine = l.Ma
                left join ViTriFillet vt on p.MaViTri = vt.Ma
                where p.Ngay = @ngay and p.MaLine = @maLine and p.MaViTri = @maViTri";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date, maLine = maLine, maViTri = maViTri }).Result
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

        public List<T> GetNhanVienTheoLineViTriMoiNhat<T>(DateTime ngay,string maLine, string maViTri)
        {
            try
            {
                var query = @"
;WITH Ranked AS (
    SELECT 
        p.Id,
        p.CodeId,
        p.Ngay,
        p.Gio,
        p.MaNhanVien,
        nv.MaHoSo,
        nv.Name AS NhanVienName,
        nv.DeptName0 AS NhomName,
        p.MaLine,
        l.Ten AS LineName,
        p.MaViTri,
        vt.Ten AS ViTriName,
        ROW_NUMBER() OVER (
            PARTITION BY p.MaNhanVien
            ORDER BY 
                TRY_CONVERT(time(0), p.Gio) DESC,
                p.Id DESC
        ) AS rn
    FROM NhanVienTheoLine p
    LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
    LEFT JOIN LineFilletv2 l ON p.MaLine = l.Ma
    LEFT JOIN ViTriFillet vt ON p.MaViTri = vt.Ma
    WHERE p.Ngay = @ngay
      AND p.MaLine = @maLine
      AND p.MaViTri = @maViTri
)
SELECT 
    Id, CodeId, Ngay, Gio, MaNhanVien, MaHoSo, NhanVienName, NhomName,
    MaLine, LineName, MaViTri, ViTriName
FROM Ranked
WHERE rn = 1
ORDER BY TRY_CONVERT(time(0), Gio) DESC;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date, maLine = maLine, maViTri = maViTri }).Result
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


        public List<T> GetNhanVienTheoLineViTriCuoiCung<T>(string maLine, string maViTri)
        {
            try
            {
                var query = @";WITH Ranked AS (
    SELECT 
        p.Id,
        p.CodeId,
        p.Ngay,
        p.Gio,
        p.MaNhanVien,
        nv.MaHoSo,
        nv.Name AS NhanVienName,
        nv.DeptName0 AS NhomName,
        p.MaLine,
        l.Ten AS LineName,
        p.MaViTri,
        vt.Ten AS ViTriName,
        ROW_NUMBER() OVER (
            PARTITION BY p.MaNhanVien
            ORDER BY 
                TRY_CONVERT(time(0), p.Gio) DESC,
                p.Id DESC
        ) AS rn
    FROM NhanVienTheoLine p
    LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
    LEFT JOIN LineFilletv2 l ON p.MaLine = l.Ma
    LEFT JOIN ViTriFillet vt ON p.MaViTri = vt.Ma
    WHERE p.MaLine = @maLine
      AND p.MaViTri = @maViTri
)
SELECT 
    Id, CodeId, Ngay, Gio, MaNhanVien, MaHoSo, NhanVienName, NhomName,
    MaLine, LineName, MaViTri, ViTriName
FROM Ranked
WHERE rn = 1
ORDER BY TRY_CONVERT(time(0), Gio) DESC;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new {maLine = maLine, maViTri = maViTri }).Result
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
