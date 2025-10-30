using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_NhanVienTheoNhom
    {
        private readonly string connectionString;
        private string tableName = @"HQ_NhanVienTheoNhom";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_NhanVienTheoNhom]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_NhanVienTheoNhom]
           ([Id]
           ,[MaNhanVien]
           ,[MaNhom],[NgayGioBatDau],[HeSo])
     VALUES
           (@Id
           ,@MaNhanVien
           ,@MaNhom,@NgayGioBatDau,@HeSo)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_NhanVienTheoNhom]
   SET [MaNhanVien] = @MaNhanVien
      ,[MaNhom] = @MaNhom,[NgayGioBatDau] = @NgayGioBatDau, [HeSo] = @HeSo
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_NhanVienTheoNhom";

        public HQ_NhanVienTheoNhom()
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
         public List<T> GetNews<T>(DateTime dateTime)
        {
            var query = @";WITH NhanVienTheoNhom AS (
    SELECT 
        p.Id,
        p.MaNhanVien,
		nv.MaHoSo,
		nv.Name as NhanVienName,
        nv.DeptName0 as NhomGoc,
        p.MaNhom,
		n.Ten as NhomName,
        p.NgayGioBatDau,
        p.HeSo,
        ROW_NUMBER() OVER (
            PARTITION BY p.MaNhanVien 
            ORDER BY NgayGioBatDau DESC
        ) AS RowNum
    FROM 
        HQ_NhanVienTheoNhom p,
		HQ_Nhom n,
		NhanVienDaiThanh nv
    WHERE 
        NgayGioBatDau <= @dateTime
		and p.MaNhanVien = nv.MaNhanVien
		and p.MaNhom = n.Id
)
SELECT 
    Id,
    MaNhanVien,
	MaHoSo,
	NhanVienName,
    NhomGoc,
    MaNhom,
	NhomName,
    NgayGioBatDau,
    HeSo
FROM 
    NhanVienTheoNhom
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
