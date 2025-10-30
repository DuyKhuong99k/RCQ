using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public class CoiMonitor
    {
        private readonly string connectionString;
        private string tableName = @"CoiMonitor";
        private readonly string qrDelete = @"DELETE FROM [dbo].[CoiMonitor]
      WHERE Id=@Id";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[CoiMonitor]
           ([Id],[MaCoi],[MaXuong],[MaThe],[NgayGio],[InLocked],[OutLocked],[NgayNguyenLieu],[TimeROut])
     VALUES
           (@Id,@MaCoi,@MaXuong,@MaThe,@NgayGio,@InLocked,@OutLocked,@NgayNguyenLieu,@TimeROut)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[CoiMonitor]
   Set [MaCoi] = @MaCoi
      ,[MaXuong] = @MaXuong
      ,[MaThe] = @MaThe
      ,[NgayGio] = @NgayGio
      ,[InLocked] = @InLocked
      ,[OutLocked] = @OutLocked, [NgayNguyenLieu] = @NgayNguyenLieu, [TimeROut] = @TimeROut
 WHERE Id = @Id
";

        private readonly string qrGetAll = "Select * from CoiMonitor";
        private readonly string qrGetLast3Day = "Select * from CoiMonitor where NgayNguyenLieu >= DATEADD(day,-3,NgayNguyenLieu) order by NgayGio";
        private readonly string qrGetInfos = @"WITH RankedData AS (
    SELECT Id,
        MaCoi,
        MaXuong,
        NgayGio,
        NgayNguyenLieu,
        MaThe,
        ROW_NUMBER() OVER (
            PARTITION BY MaCoi,
            MaXuong
            ORDER BY NgayGio DESC
        ) AS rn
    FROM CoiMonitor
    where MaCoi is NOT NULL
        and MaCoi != ''
        and NgayNguyenLieu >= '{0}'
        and InLocked != OutLocked
)
select mtor.*,
    p.TrongLuong,
    p.MaChatLuong,
    p.MaThanhPhamChinh as MaThanhPham,
    p.MaSizeChinh as MaSize,
    p.MaLo,
    p.MaChieuXa,
    p.MayQuay,
    p.ThoiGianQuay,
    p.ThoiGianBatDauQuay,
    p.NgayBatDauQuay,
    p.MaMayCan,
    p.MaCoiTam
from (
        SELECT Id,
            MaCoi,
            MaXuong,
            NgayGio,
            NgayNguyenLieu,
            MaThe
        FROM RankedData
        WHERE rn = 1
    ) mtor,
    phieucanchinhxepkhuon p
where mtor.Id = p.IdMonitor";
        public CoiMonitor()
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
        public List<T> GetsLast3Day<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetLast3Day).ToList();
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
        public List<T> GetInfos<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var date = DateTime.Now.AddDays(-5).Date;
            var rows = connection.Query<T>(string.Format(qrGetInfos, date.ToString("yyyy-MM-dd"))).ToList();
            return rows;
        }

    }
}
