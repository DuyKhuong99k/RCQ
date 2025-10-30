using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BoTriLoSizeThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"BoTriLoSizeThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BoTriLoSizeThanhPham]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BoTriLoSizeThanhPham]
           ([Id]
           ,[MaLo]
           ,[MaViTri]
           ,[MaSize]
           ,[MaThanhPham]
           ,[Ngay]
           ,[Gio]
           ,[CodeId]
           ,[MaSizePhu])
     VALUES
           (@Id
           ,@MaLo
           ,@MaViTri
           ,@MaSize
           ,@MaThanhPham
           ,@Ngay
           ,@Gio
           ,@CodeId
           ,@MaSizePhu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BoTriLoSizeThanhPham]
   SET [MaLo] =@MaLo
           ,[MaViTri] =@MaViTri
           ,[MaSize]=@MaSize
           ,[MaThanhPham]=@MaThanhPham
           ,[Ngay]=@Ngay
           ,[Gio]=@Gio
           ,[CodeId]=@CodeId
           ,[MaSizePhu]=@MaSizePhu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from BoTriLoSizeThanhPham";

        public BoTriLoSizeThanhPham()
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

        public List<T> GetBoTriViTriLoSizeThanhPhams<T>(DateTime ngay,string maLo)
        {
            try
            {
                var query = @"
                select
p.Id,
p.MaLo,
p.MaViTri,
vt.Ten as ViTriName,
p.MaSize,
s.Ten as SizeName,
p.MaSizePhu,
sp.Ten as SizePhuName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.Ngay,
p.Gio,
p.CodeId
from BoTriLoSizeThanhPham p
left join ViTriFillet vt on vt.Ma = p.MaViTri
left join MaSizeFillet s on s.Ma = p.MaSize
left join MaSizeFillet sp on sp.Ma = p.MaSize
left join MaThanhPhamFillet tp on tp.Ma = p.MaThanhPham
where p.Ngay = @ngay and p.MaLo = @maLo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date, maLo = maLo }).Result
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
        public List<T> GetBoTriViTriLoSizeThanhPhamsMoiNhat<T>(DateTime ngay,string maLo)
        {
            try
            {
                var query = @"
                ;WITH Ranked AS (
    SELECT 
        p.Id,
		p.MaLo,
		p.MaViTri,
		vt.Ten as ViTriName,
		p.MaSize,
		s.Ten as SizeName,
		p.MaSizePhu,
		sp.Ten as SizePhuName,
		p.MaThanhPham,
		tp.Ten as ThanhPhamName,
		p.Ngay,
		p.Gio,
		p.CodeId,
        ROW_NUMBER() OVER (
            PARTITION BY p.MaViTri,p.MaSize,p.MaSizePhu,p.MaThanhPham
            ORDER BY 
                TRY_CONVERT(time(0), p.Gio) DESC,
                p.Id DESC
        ) AS rn
    FROM BoTriLoSizeThanhPham p
    left join ViTriFillet vt on vt.Ma = p.MaViTri
	left join MaSizeFillet s on s.Ma = p.MaSize
	left join MaSizeFillet sp on sp.Ma = p.MaSize
	left join MaThanhPhamFillet tp on tp.Ma = p.MaThanhPham
    WHERE p.Ngay = @ngay
      AND p.MaLo = @maLo

)
SELECT 
    Id,
		MaLo,
		MaViTri,
		ViTriName,
		MaSize,
		SizeName,
		MaSizePhu,
		SizePhuName,
		MaThanhPham,
		ThanhPhamName,
		Ngay,
		Gio,
		CodeId
FROM Ranked
WHERE rn = 1
ORDER BY TRY_CONVERT(time(0), Gio) DESC;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date, maLo = maLo }).Result
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
