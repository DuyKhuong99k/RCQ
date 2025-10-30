using Dapper;
using Microsoft.Data.SqlClient;
using ToolsEx;

namespace Dao.Repos.HQ
{
    public partial class NhanVienTheoBan
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienTheoBan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NhanVienTheoBan]
        WHERE  [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NhanVienTheoBan]
           ([Id]
           ,[NgayGio]
           ,[MaNhanVien]
           ,[MaBan]
           ,[MaKhuVuc]
           ,[MaXuong]
           ,[PCName]
           ,[UserName],[IsDone]
           )
     VALUES
           (@Id
           ,@NgayGio
           ,@MaNhanVien
           ,@MaBan
           ,@MaKhuVuc
           ,@MaXuong
           ,@PCName
           ,@UserName,@IsDone)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NhanVienTheoBan]
        SET [Id]=@Id
           ,[NgayGio] = @NgayGio
           ,[MaNhanVien] = @MaNhanVien
           ,[MaBan] = @MaBan
           ,[MaKhuVuc] = @MaKhuVuc
           ,[MaXuong] = @MaXuong
           ,[PCName] = @PCName
           ,[UserName] = @UserName,
            [IsDone] = @IsDone
 WHERE [Id] = @Id";
        private readonly string queryUpdateDb = @"IF (
    NOT EXISTS (
        SELECT
            *
        FROM
            INFORMATION_SCHEMA.TABLES
        WHERE
            TABLE_SCHEMA = 'dbo'
            AND TABLE_NAME = 'NhanVienTheoBan'
    )
) BEGIN 
   CREATE TABLE [dbo].[NhanVienTheoBan] (
    [Id]         VARCHAR (50)  NOT NULL,
    [NgayGio]    DATETIME2 (7) CONSTRAINT [DEFAULT_NhanVienTheoBan_NgayGio] DEFAULT (getdate()) NOT NULL,
    [MaNhanVien] VARCHAR (50)  NOT NULL,
    [MaBan]      VARCHAR (50)  NOT NULL,
    [MaKhuVuc]   VARCHAR (50)  NOT NULL,
    [MaXuong]    VARCHAR (50)  NOT NULL,
    [PCName]     VARCHAR (50)  NOT NULL,
    [UserName]   VARCHAR (50)  NULL,
    [IsDone]     BIT           CONSTRAINT [DF_NhanVienTheoBan_IsDone] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_NhanVienTheoBan] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
";
        private readonly string qrGetAll = "Select * from NhanVienTheoBan";

        public NhanVienTheoBan(string? _connectionString = null)
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
        public List<T> Gets<T>(string Id)
        {
            try
            {
                var query = "Select * from NhanVienTheoBan WHERE Id = @Id";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, Id).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = "Select * from NhanVienTheoBan WHERE cast( NgayGio as Date) = @ngay";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime fromDate, DateTime dateTime)
        {
            try
            {
                var query = "Select * from NhanVienTheoBan WHERE cast( NgayGio as Date) <= @toDate and cast( NgayGio as Date) >=@fromDate";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = dateTime.Date }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetNhanVienTheoBans<T>(string maBan, string xuongId)
        {
            try
            {
                var query = "Select * from NhanVienTheoBan where MaBan = @maBan and MaXuong =@xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { maBan = maBan, xuongId = xuongId }).Result
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
        public List<T> GetNhanVienTheoBanFullFileds<T>(string maBan, string xuongId)
        {
            try
            {
                var query = @"Select
nvb.Id,
nvb.MaNhanVien,
nv.MaHoSo,
nv.Name,
nv.DeptName0,
b.Ten as BanName,
nvb.MaKhuVuc,
x.Ten as XuongName,
nvb.NgayGio,
nvb.UserName as Creator,
nvb.PCName
from NhanVienTheoBan nvb 
left join NhanVienDaiThanh nv on nv.MaNhanVien= nvb.MaNhanVien
left join BanFillet b on b.Ma = nvb.MaBan
left join XiNghiep x on x.Ma = nvb.MaXuong
where nvb.MaBan = @maBan and nvb.MaXuong =@xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { maBan = maBan, xuongId = xuongId }).Result
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
        public List<T> GetViTriHienTais<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from NhanVienTheoBan  Where  NgayGio = @ngay and MaXuong = @xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetsLastDate<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.*
from
    (
        Select
            *,
            ROW_NUMBER() OVER(
                PARTITION BY MaNhanVien
                ORDER BY
                    NgayGio desc
            ) as row_number
        from
            NhanVienTheoBan
        Where
                     cast( NgayGio as date)  <= @ngay
                   
            and MaXuong = @xuongId
           
    ) p
where
    row_number = 1 ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetsNhanVienBanDys<T>(DateTime dateTime, string xuongId, string maBan, string thePhieuSlId)
        {
            try
            {
                var query = @"select p.* from 
(select 
nv.MaNhanVien, 
nv.MaBan,
nv.MaXuong,
p.TrongLuongTra as TrongLuong,
 ROW_NUMBER() OVER(
                PARTITION BY nv.MaNhanVien
                ORDER BY
                    NgayGio desc
            ) as row_number
from NhanVienTheoBan nv
left join (select * 
from PhieuCanTPFilletv2 p
where p.ThePhieuSanLuongId = @thePhieuSlId)
p on p.MaNhanVien = nv.MaNhanVien
where
cast(nv.NgayGio as date) <= @ngay
and nv.MaBan =@maBan
and nv.MaXuong= @xuongId)p
where
    row_number = 1";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime, xuongId, maBan, thePhieuSlId }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public T GetLastDateToDay<T>(DateTime dateTime, string nhanVienId, string xuongId)
        {
            try
            {
                var query = @"
Select p.*
from (
        Select *,
            ROW_NUMBER() OVER(
                PARTITION BY MaNhanVien
                ORDER BY NgayGio desc
            ) as row_number
        from NhanVienTheoBan
        Where cast(NgayGio as date) = @ngay
            and MaXuong = @xuongId
            and MaNhanVien = @nhanVienId
    ) p
where row_number = 1
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var item = connection.Query<T>(query, new { ngay = dateTime, xuongId, nhanVienId }).FirstOrDefault();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }
        public int Insert<T>(List<T> items)
        {
            var query = qrInsert;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
        //public int InsertBatch<T>(List<T> items)
        //{
        //    var batches = DbExtensions.GetSqlsInBatches(items);
        //    var row = 0;
        //    var database = new Database(connectionString);
        //    foreach (var batche in batches) row += database.ExecuteNonQuery(batche);

        //    return row;
        //}
        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
        public int Update<T>(List<T> items)
        {
            var query = qrUpdate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
    }
}
