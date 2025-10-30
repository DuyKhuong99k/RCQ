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
    public class PLCChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"PLCChiTiet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PLCChiTiet]
      WHERE Id=@Id";

        private readonly string qrInsert = @"USE [PMS_HQ]
GO

INSERT INTO [dbo].[PLCChiTiet]
           ([Id]
           ,[MaCoi]
           ,[MaXuong]
           ,[PLCId]
           ,[RUN]
           ,[STOP]
           ,[TimeQuay]
           ,[HzQuay]
           ,[HzRa],[TimeQuayDef],[HzQuayDef],[HzRaDef],[INVERTER],[RUNSTATUS],[PAUSE],[RUNOUT])
     VALUES
           (@Id
           ,@MaCoi
           ,@MaXuong
           ,@PLCId
           ,@RUN
           ,@STOP
           ,@TimeQuay
           ,@HzQuay
           ,@HzRa,@TimeQuayDef,@HzQuayDef,@HzRaDef,@INVERTER,@RUNSTATUS,@PAUSE,@RUNOUT)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[PLCChiTiet]
   SET [MaCoi] = @MaCoi
      ,[MaXuong] = @MaXuong
      ,[PLCId] = @PLCId
      ,[RUN] = @RUN
      ,[STOP] = @STOP
      ,[TimeQuay] = @TimeQuay
      ,[HzQuay] = @HzQuay
      ,[HzRa] = @HzRa,[TimeQuayDef] = @TimeQuayDef,[HzQuayDef] = @HzQuayDef,[HzRaDef] = @HzRaDef,[INVERTER] = @INVERTER,[RUNSTATUS] = @RUNSTATUS,[PAUSE] = @PAUSE,[RUNOUT] = @RUNOUT
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from PLCChiTiet";

        public PLCChiTiet()
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
