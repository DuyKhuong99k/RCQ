using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_MapSanPhamTinhLuong
    {
        private readonly string connectionString;
        private string tableName = @"HQ_MapSanPhamTinhLuong";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_MapSanPhamTinhLuong]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_MapSanPhamTinhLuong]
           ([Id]
           ,[MaSanPham]
           ,[MaThanhPham],[NgayGio],[MaLoaiNguyenLieu])
     VALUES
           (@Id
           ,@MaSanPham
           ,@MaThanhPham,@NgayGio, @MaLoaiNguyenLieu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_MapSanPhamTinhLuong]
   SET [MaSanPham] = @MaSanPham
      ,[MaThanhPham] = @MaThanhPham,[NgayGio] = @NgayGio, [MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_MapSanPhamTinhLuong";

        public HQ_MapSanPhamTinhLuong()
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
         public List<T> GetAllsFullField<T>(DateTime dateTime)
        {
            var query = @";WITH MapSanPhamTinhLuong AS (
    SELECT 
        p.Id,
        p.MaSanPham,
		dg.Ten as SanPhamName,
        p.MaThanhPham,
		tp.Ten as ThanhPhamName,
        p.MaSize,
        s.Ten as SizeName,
        p.MaLoaiNguyenLieu,
        lnl.Ten as LoaiNguyenLieuName,
        p.NgayGio,
        ROW_NUMBER() OVER (
            PARTITION BY p.MaThanhPham, p.MaSanPham , p.MaSize, p.MaLoaiNguyenLieu
            ORDER BY p.NgayGio DESC
        ) AS RowNum
    FROM 
        HQ_MapSanPhamTinhLuong p,
		DG_SanPhamTinhLuong dg,
		HQ_ThanhPham tp,
        HQ_Size s,
        HQ_LoaiNguyenLieu lnl
    WHERE 
        p.NgayGio <= @dateTime
		and p.MaSanPham = dg.Ma
		and p.MaThanhPham = tp.Id
        and p.MaSize = s.Id
        and p.MaLoaiNguyenLieu = lnl.Id
) 
SELECT 
    Id,
    MaSanPham,
	SanPhamName,
	MaThanhPham,
    ThanhPhamName,
    MaSize,
    SizeName,
    MaLoaiNguyenLieu,
    LoaiNguyenLieuName,
    NgayGio
FROM 
    MapSanPhamTinhLuong
WHERE 
    RowNum = 1;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { dateTime })
                .ToList();
            return items;
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
