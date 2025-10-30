using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TChiTietBon
    {
        private readonly string connectionString;
        private string tableName = @"T_ChiTietBon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_ChiTietBon]
            WHERE [Id] = @Id";
        private readonly string qrInsert = @"
    INSERT INTO [dbo].[T_ChiTietBon]
           ([Id]
           ,[MaBon]
           ,[MaSanPham]
           ,[TrongLuong]
           ,[Luot]
           ,[NgayBatDau]
           ,[GioBatDau]
           ,[NgayKetThuc]
           ,[GioKetThuc]
           ,[GhiChu]
           ,[DaHoanThanh]
           ,[TrongLuongTP]
           ,[TrongLuongConLai]
           ,[MaXuong]
           ,[DaChuyen]
           ,[MaBonGoc])
     VALUES
            (@Id
           ,@MaBon
           ,@MaSanPham
           ,@TrongLuong
           ,@Luot
           ,@NgayBatDau
           ,@GioBatDau
           ,@NgayKetThuc
           ,@GioKetThuc
           ,@GhiChu
           ,@DaHoanThanh
           ,@TrongLuongTP
           ,@TrongLuongConLai
           ,@MaXuong
           ,@DaChuyen
           ,@MaBonGoc)";
        private readonly string qrUpdate = @"
    UPDATE [dbo].[T_ChiTietBon]
    SET
           ,[MaBon] = @MaBon
           ,[MaSanPham] = @MaSanPham
           ,[TrongLuong] = @TrongLuong
           ,[Luot] = @Luot
           ,[NgayBatDau] = @NgayBatDau
           ,[GioBatDau] = @GioBatDau
           ,[NgayKetThuc] = @NgayKetThuc
           ,[GioKetThuc] = @GioKetThuc
           ,[GhiChu] = @GhiChu
           ,[DaHoanThanh] = @DaHoanThanh
           ,[TrongLuongTP] = @TrongLuongTP
           ,[TrongLuongConLai] = @TrongLuongConLai
           ,[MaXuong] = @MaXuong
           ,[DaChuyen] = @DaChuyen
           ,[MaBonGoc] = @MaBonGoc
     WHERE [Id] = @Id";
        private readonly string qrGetAll = "Select * from T_ChiTietBon";
        public TChiTietBon()
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
    }
}
