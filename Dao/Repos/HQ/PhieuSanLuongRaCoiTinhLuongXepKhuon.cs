using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dao.Repos.HQ
{
    public partial class PhieuSanLuongRaCoiTinhLuongXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"PhieuSanLuongRaCoiTinhLuongXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuSanLuongRaCoiTinhLuongXepKhuon] WHERE [STT] = @STT and [Ngay] = @Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
        private readonly string qrInsert = @"
INSERT INTO [dbo].[PhieuSanLuongRaCoiTinhLuongXepKhuon] ([STT] ,[Ngay] ,[MaXuong] ,[MaMayCan] ,[MaUserCan] ,[Gio] ,[NgayThem] ,[GioRaCoi] ,[MaCoi] ,[LuotRaCoi] ,[MaNhom] ,[TrongLuong] ,[GhiChu])  VALUES (@STT, @Ngay, @MaXuong,@MaMayCan, @MaUserCan,@Gio, @NgayThem,@GioRaCoi,@MaCoi,@LuotRaCoi,@MaNhom,@TrongLuong, @GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuSanLuongRaCoiTinhLuongXepKhuon] SET [MaUserCan] = @MaUserCan, [Gio] = @Gio, [NgayThem] = @NgayThem,[GioRaCoi] =@GioRaCoi, [MaCoi] = @MaCoi,[LuotRaCoi] = @LuotRaCoi,[MaNhom] = @MaNhom,[TrongLuong] = @TrongLuong,[GhiChu] = @GhiChu WHERE [STT] = @STT and [Ngay] = @Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuSanLuongRaCoiTinhLuongXepKhuon";

        public PhieuSanLuongRaCoiTinhLuongXepKhuon()
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
        public int Delete(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "DELETE FROM [dbo].[PhieuSanLuongRaCoiTinhLuongXepKhuon] WHERE [Ngay] = @ngay and [MaXuong] = @xuongId ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, new { ngay = dateTime.Date, xuongId = xuongId });
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
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
        public int Insert(List<Models.Repos.Models.PhieuSanLuongRaCoiTinhLuongXepKhuon> items)
        {
            try
            {
                var query =
                    "INSERT INTO [dbo].[PhieuSanLuongRaCoiTinhLuongXepKhuon] ([STT] ,[Ngay] ,[MaXuong] ,[MaMayCan] ,[MaUserCan] ,[Gio] ,[NgayThem] ,[GioRaCoi] ,[MaCoi] ,[LuotRaCoi] ,[MaNhom] ,[TrongLuong] ,[GhiChu])  VALUES (@STT, @Ngay, @MaXuong,@MaMayCan, @MaUserCan,@Gio, @NgayThem,@GioRaCoi,@MaCoi,@LuotRaCoi,@MaNhom,@TrongLuong, @GhiChu)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, items);
                    return rows;
                }
            }
            catch (Exception)
            {
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
        public int Update(List<Models.Repos.Models.PhieuSanLuongRaCoiTinhLuongXepKhuon> items)
        {
            try
            {
                var query =
                    "UPDATE [dbo].[PhieuSanLuongRaCoiTinhLuongXepKhuon] SET [MaUserCan] = @MaUserCan, [Gio] = @Gio, [NgayThem] = @NgayThem,[GioRaCoi] =@GioRaCoi, [MaCoi] = @MaCoi,[LuotRaCoi] = @LuotRaCoi,[MaNhom] = @MaNhom,[TrongLuong] = @TrongLuong,[GhiChu] = @GhiChu WHERE [STT] = @STT and [Ngay] = @Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, items);
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsFullField<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaUserCan,
p.Gio,
p.NgayThem,
p.GioRaCoi,
p.MaCoi,
c.Ten as CoiName,
p.LuotRaCoi,
p.MaNhom,
n.Ten as NhomName,
p.TrongLuong,
p.GhiChu
from PhieuSanLuongRaCoiTinhLuongXepKhuon p
left join XiNghiep x on x.Ma = p.MaXuong
left join MaCoiXepKhuon c ON c.Ma = p.MaCoi
left join MaNhomXepKhuon n on n.Ma = p.MaNhom
where p.Ngay = @ngay and p.MaXuong = @xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new
                {
                    ngay = dateTime.Date,
                    xuongId = xuongId
                }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<Models.Repos.Models.PhieuSanLuongRaCoiTinhLuongXepKhuon> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuSanLuongRaCoiTinhLuongXepKhuon where Ngay = @ngay and MaXuong = @xuongId order by Gio";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhieuSanLuongRaCoiTinhLuongXepKhuon>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId
                            })
                        .Result
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
        public List<T> GetLuotRaCoi<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select p.*, ROW_NUMBER() over (partition by p.MaCoi order by p.GioRaCoi) as [LuotRaCoi]
from
(Select p.MaCoiChinh as MaCoi,p.ThoiGianBatDauQuay,p.Forced, case when  p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay) end as GioRaCoi,Sum(p.TrongLuong) as TrongLuong from PhieuCanChinhXepKhuon p where ngay=@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong != @xuongId)) and MaCoiChinh is not null and DaQuay =1 group by MaCoiChinh,ThoiGianBatDauQuay,Forced,ThoiGianRaCoi,ThoiGianQuay ) p order by MaCoi,ThoiGianBatDauQuay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
