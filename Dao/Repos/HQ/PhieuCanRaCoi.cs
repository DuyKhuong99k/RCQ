using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public class PhieuCanRaCoi
    {
        private readonly string connectionString;
        private string tableName = "PhieuCanRaCoi";
        private readonly string qrInsert = @"
INSERT INTO [dbo].[PhieuCanRaCoi]
           ([Id]
           ,[IdMonitor]
           ,[Ngay]
           ,[Gio]
           ,[MaXuong]
           ,[MayCan]
           ,[NgayNguyenLieu]
           ,[TrongLuong]
           ,[TrongLuongTare]
           ,[MaLo]
           ,[MaThanhPham]
           ,[MaSize]
           ,[MaChieuXa]
           ,[MaChatLuong]
           ,[MaNhanVien]
           ,[MaCoi]
           ,[MaThe]
           ,[GhiChu],[STT])
     VALUES
           (@Id
           ,@IdMonitor
           ,@Ngay
           ,@Gio
           ,@MaXuong
           ,@MayCan
           ,@NgayNguyenLieu
           ,@TrongLuong
           ,@TrongLuongTare
           ,@MaLo
           ,@MaThanhPham
           ,@MaSize
           ,@MaChieuXa
           ,@MaChatLuong
           ,@MaNhanVien
           ,@MaCoi
           ,@MaThe
           ,@GhiChu,@STT)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanRaCoi]
   SET [IdMonitor] = @IdMonitor
      ,[Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[MaXuong] =@MaXuong
      ,[MayCan] = @MayCan
      ,[NgayNguyenLieu] = @NgayNguyenLieu
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongTare] = @TrongLuongTare
      ,[MaLo] = @MaLo
      ,[MaThanhPham] = @MaThanhPham
      ,[MaSize] = @MaSize
      ,[MaChieuXa] = @MaChieuXa
      ,[MaChatLuong] = @MaChatLuong
      ,[MaNhanVien] = @MaNhanVien
      ,[MaCoi] = @MaCoi
      ,[MaThe] = @MaThe
      ,[GhiChu] = @GhiChu,[STT] = @STT
 WHERE [Id] = @Id

";

        private string qrDelete = @"
DELETE FROM [dbo].[PhieuCanRaCoi]
      WHERE Id =@Id
";
        private readonly string qrGetAll = "Select * from PhieuCanRaCoi";
        private readonly string qrGetByDate = "Select * from PhieuCanRaCoi where Ngay = @Ngay";
        private readonly string qrGetByDateRange = "Select * from PhieuCanRaCoi where Ngay >= @NgayStart and Ngay <= @NgayEnd";
        private readonly string qrGetByNgayXuong = "Select * from PhieuCanRaCoi where Ngay = @Ngay and MaXuong =@MaXuong";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanRaCoi where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = qrGetsLastByNumAndMayCan;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, num })
                    .Result
                    .ToList();
                return items;
            }
        }
        public PhieuCanRaCoi(string? _connectionString = null)
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
        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public List<T> Gets<T>(DateTime date)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByDate, new { Ngay = date.Date }).ToList();
            return rows;
        }
        public List<T> Gets<T>(DateTime dateStart, DateTime dateEnd)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByDateRange, new { NgayStart = dateStart.Date, NgayEnd = dateEnd.Date }).ToList();
            return rows;
        }
        public List<T> Gets<T>(DateTime date, string maXuong)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByNgayXuong, new { Ngay = date.Date, MaXuong = maXuong }).ToList();
            return rows;
        }
    }
}
