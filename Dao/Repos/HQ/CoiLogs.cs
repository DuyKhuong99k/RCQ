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
    public class CoiLogs
    {
        private readonly string connectionString;
        private string tableName = @"CoiLogs";
        private readonly string qrDelete = @"DELETE FROM [dbo].[CoiLogs]
      WHERE Id=@Id";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[CoiLogs]
           ([MaCoi]
           ,[MaXuong]
           ,[ActionName]
           ,[NgayGio],[HzQuay],[HzRa],[TimeQuay],[IsError],[ErrorStr],[Decription],[IdMonitor],[NhanVienId],[MaChatLuong])
     VALUES
           (@MaCoi
           ,@MaXuong
           ,@ActionName
           ,@NgayGio,@HzQuay,@HzRa,@TimeQuay,@IsError,@ErrorStr,@Decription,@IdMonitor,@NhanVienId,@MaChatLuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[CoiLogs]
   SET [MaCoi] = @MaCoi
      ,[MaXuong] = @MaXuong
      ,[ActionName] = @ActionName
      ,[NgayGio] = @NgayGio, [HzQuay] = @HzQuay, [HzRa] = @HzRa, [TimeQuay] = @TimeQuay, [IsError] = @IsError, [ErrorStr] = @ErrorStr, [Decription] = @Decription, [IdMonitor] = @IdMonitor, [NhanVienId] = @NhanVienId, [MaChatLuong] = @MaChatLuong
 WHERE Id = @Id
";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaCoi ORDER BY NgayGio DESC) AS RowNum
    FROM PhieuCanChinhXepKhuon where Cast(NgayGio as Date) =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";

        private readonly string qrGeLasts5Day = @"WITH RankedData AS (
    SELECT *,
        ROW_NUMBER() OVER (
            PARTITION BY MaCoi,
            MaXuong
            ORDER BY NgayGio DESC
        ) AS rn
    FROM CoiLogs
    where MaCoi is NOT NULL
        and MaCoi != ''
        and NgayGio >= '{0}'
)
SELECT *
        FROM RankedData
        WHERE rn = 1";

        private readonly string qrGetAll = "Select * from CoiLogs";
        public List<T> GetLasts5Day<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var date = DateTime.Now.AddDays(-5).Date;
            var rows = connection.Query<T>(string.Format( qrGeLasts5Day,date.ToString("yyyy-MM-dd"))).ToList();
            return rows;
        }
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
        public CoiLogs()
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
