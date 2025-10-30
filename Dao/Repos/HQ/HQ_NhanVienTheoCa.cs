using Dapper;
using Microsoft.Data.SqlClient;


namespace Dao.Repos.HQ
{
    public partial class HQ_NhanVienTheoCa
    {
        private readonly string connectionString;
        private string tableName = @"HQ_NhanVienTheoCa";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_NhanVienTheoCa]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_NhanVienTheoCa]
           ([Id]
           ,[CaId]
           ,[NhanVienId,[NgayGio])
     VALUES
           (@Id
           ,@CaId
           ,@NhanVienId,@NgayGio)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_NhanVienTheoCa]
   SET [CaId] = @CaId
      ,[NhanVienId] = @NhanVienId,[NgayGio] = @NgayGio
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_NhanVienTheoCa";

        public HQ_NhanVienTheoCa()
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
         public List<T> GetNews<T>(DateTime dateTime)
        {
            var query = @"
;WITH NhanVienTheoCa AS (
    SELECT 
        p.Id,
        p.NhanVienId,
		nv.MaHoSo,
		nv.Name as NhanVienName,
		nv.DeptName0 as NhomName,
        p.CaId,
		c.Ten as CaName,
        p.NgayGio,
        ROW_NUMBER() OVER (
            PARTITION BY p.NhanVienId
            ORDER BY p.NgayGio DESC
        ) AS RowNum
    FROM 
        HQ_NhanVienTheoCa p,
		HQ_Ca c,
		NhanVienDaiThanh nv
    WHERE 
        p.NgayGio < @dateTime
		and p.NhanVienId = nv.MaNhanVien
		and p.CaId = c.Id
) 
SELECT 
    Id,
    NhanVienId,
	MaHoSo,
	NhanVienName,
	NhomName,
    CaId,
	CaName,
    NgayGio
FROM 
    NhanVienTheoCa
WHERE 
    RowNum = 1;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { dateTime })
                .ToList();
            return items;
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
    }
}
