using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class ThePhieuSanLuongFillet
    {
        private readonly string connectionString;
        private string tableName = @"ThePhieuSanLuongFillet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[ThePhieuSanLuongFillet]
      WHERE  [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[ThePhieuSanLuongFillet]
           ([Id]
           ,[Ngay]
           ,[Gio]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[CaTra]
           ,[MaXuong]
           ,[STT_PC]
           ,[MayCan_PC]
           ,[MaThe]
           ,[TrongLuongTare]
           ,[MaBan]
           ,[MaNhanVien],[IsDone],[STT_PC_BTP],[MayCan_PC_BTP],[TrongLuongNhan],[IdIn])
     VALUES
           (@Id
           ,@Ngay
           ,@Gio
           ,@MaLoaiCa
           ,@MaMau
           ,@MaSize
           ,@MaThanhPham
           ,@MaLo
           ,@CaTra
           ,@MaXuong
           ,@STT_PC
           ,@MayCan_PC
           ,@MaThe
           ,@TrongLuongTare
           ,@MaBan
           ,@MaNhanVien,@IsDone,@STT_PC_BTP,@MayCan_PC_BTP,@TrongLuongNhan,@IdIn)";

        private readonly string qrUpdate = @"UPDATE [dbo].[ThePhieuSanLuongFillet]
        SET [Id]=@Id
      ,[Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[MaLoaiCa] = @MaLoaiCa
      ,[MaMau] = @MaMau
      ,[MaSize] = @MaSize
      ,[MaThanhPham] = @MaThanhPham
      ,[MaLo] = @MaLo
      ,[CaTra] = @CaTra
      ,[MaXuong] = @MaXuong
      ,[STT_PC] = @STT_PC
      ,[MayCan_PC] = @MayCan_PC
      ,[MaThe] = @MaThe
      ,[TrongLuongTare] = @TrongLuongTare
      ,[MaBan] = @MaBan
      ,[MaNhanVien] = @MaNhanVien,
        [IsDone] = @IsDone, [STT_PC_BTP] = @STT_PC_BTP, [MayCan_PC_BTP] = @MayCan_PC_BTP,[TrongLuongNhan] = @TrongLuongNhan
 WHERE [Id] = @Id";

        private readonly string qrGetAll = "Select * from ThePhieuSanLuongFillet";
        private readonly string qrGetsLastByBan =@"
Select * from ThePhieuSanLuongFillet where Ngay = @ngay
            and MaBan =  @banId  and Gio =(
			  Select isnull(MAX(Gio), '00:00:00') as Gio
        from ThePhieuSanLuongFillet
        where Ngay = @ngay
            and MaBan =  @banId )
";

        public ThePhieuSanLuongFillet()
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
        public List<T> GetsLastByBan<T>(DateTime dateTime, string banId)
        {
            try
            {
                var query = qrGetsLastByBan;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, banId }).ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
        public int SetTheIsDoneByBan(DateTime dateTime, string banId, bool isDone)
        {
            try
            {
                var query = @"UPDATE [dbo].[ThePhieuSanLuongFillet]
        SET 
        [IsDone] = @IsDone
 WHERE [Ngay] = @ngay and [MaBan] = @banId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, new { isDone, ngay = dateTime.Date, banId });
                return rows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
