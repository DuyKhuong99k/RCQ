using Dapper;
using Microsoft.Data.SqlClient;
namespace Dao.Repos.HQ
{
    public partial class PhieuCanRaDong
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanRaDong";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[PhieuCanRaDong] ([STT] ,[Ngay] ,[MaXuong] ,[MaMayCan] ,[Id] ,[Gio] ,[MaUserCan] ,[GhiChu] ,[MaLo] ,[MaSize] ,[MaChatLuong] ,[MaLoaiCa] ,[MaMau] ,[TrongLuong]) VALUES (@STT,@Ngay,@MaXuong,@MaMayCan,@Id,@Gio,@MaUserCan,@GhiChu,@MaLo, @MaSize, @MaChatLuong, @MaLoaiCa,@MaMau,@TrongLuong)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[PhieuCanRaDong] SET  [Id] =@Id,[Gio] = @Gio, [MaUserCan] =@MaUserCan, [GhiChu] = @GhiChu, [MaLo] = @MaLo, [MaSize] = @MaSize, [MaChatLuong] = @MaChatLuong, [MaLoaiCa] = @MaLoaiCa, [MaMau] = @MaMau, [TrongLuong] = @TrongLuong WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanRaDong";

        public PhieuCanRaDong()
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

        public List<T> GetPhieuCanRaDongXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.Id,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.TrongLuong
from PhieuCanRaDong p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaSizeTaiChe s on p.MaSize = s.Ma
left join MaChatLuongTaiChe cl on p.MaChatLuong = cl.Ma
left join MaLoaiCaTaiChe lc on p.MaLoaiCa = lc.Ma
left join MaMauTaiChe m on p.MaMau = m.Ma

where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate
and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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
