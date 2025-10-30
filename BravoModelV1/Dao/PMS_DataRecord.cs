using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Dao
{
    public partial class PMS_DataRecord
    {
        private readonly string connectionString;
        public PMS_DataRecord(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public int ExecuteBatche(string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = connection;
                    connection.Open();
                    var rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public IList<string> GetSqlsInBatches(IList<BravoModelV1.EF.PMS_DataRecord> phieuCans)
        {
            var insertSql = @"INSERT INTO[dbo].[PMS_DataRecord]
           ([ID],[CMND],[ThoiGian],[Status],[CongDoanID],[TrongLuong],[DateCreate],[DateSync]) VALUES";
            var valuesSql = @"('{0}','{1}','{2}', {3}, '{4}', {5}, '{6}', '{7}')";
            var batchSize = 1000;

            var sqlsToExecute = new List<string>();
            var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
                var valuesToInsert = phieuCanToInsert.Select(
                    x => string.Format(
                        valuesSql,
                        x.ID,
                        x.CMND,
                        x.ThoiGian
                            .ToString("yyyy-MM-dd hh:mm:ss"),
                        x.Status == true ? 1 : 0,
                        x.CongDoanID,
                        x.TrongLuong,
                        x.DateCreate
                            .ToString("yyyy-MM-dd hh:mm:ss"),
                        x.DateSync?.ToString("yyyy-MM-dd hh:mm:ss")));
                sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
            }

            return sqlsToExecute;
        }
        public IList<string> GetSqlsInUpdateBatches(IList<BravoModelV1.EF.PMS_DataRecord> phieuCans)
        {
            var insertSql = @"IF OBJECT_ID('tempdb.dbo.#tempPhieuCan', 'U') IS NOT NULL
    DROP TABLE #tempPhieuCan; INSERT INTO #tempPhieuCan
           ([ID],[CMND],[ThoiGian],[Status],[CongDoanID],[TrongLuong],[DateCreate],[DateSync]) VALUES";
            var valuesSql = @"('{0}','{1}','{2}', {3}, '{4}', {5}, '{6}', '{7}')";
            var updateSql = @"Update[dbo].[PMS_DataRecord]
    Set CMND = d.CMND,
        ThoiGian = d.ThoiGian,
        Status = d.Status,

        CongDoanID = d.CongDoanID,
        TrongLuong = d.TrongLuong,
        DateCreate = d.DateCreate,
        DateSync = d.DateSync

    From
    [dbo].[PMS_DataRecord] s
    Inner join

((select * from[dbo].[PMS_DataRecord]
UNION
select * from  #tempPhieuCan)
EXCEPT
select* from[dbo].[PMS_DataRecord])  d
on
s.ID = d.ID;";
            var batchSize = 1000;

            var sqlsToExecute = new List<string>();
            var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
                var valuesToInsert = phieuCanToInsert.Select(
                    x => string.Format(
                        valuesSql,
                        x.ID,
                        x.CMND,
                        x.ThoiGian
                            .ToString("yyyy-MM-dd hh:mm:ss"),
                        x.Status == true ? 1 : 0,
                        x.CongDoanID,
                        x.TrongLuong,
                        x.DateCreate
                            .ToString("yyyy-MM-dd hh:mm:ss"),
                        "1900-01-01 00:00:00.000"));
                sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert) + ";" + updateSql);
            }

            return sqlsToExecute;
        }
        public List<T> Gets<T>(DateTime dateTime, string khuVucId, string xuongId)
        {
            try
            {
                var query =
                    $@"Select * from PMS_DataRecord where CAST(ThoiGian as date) = @ngay and  CHARINDEX('-{xuongId}-K{khuVucId}-',ID)>0";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = dateTime }).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
         public int Insert<T>(List<T> items)
        {
            try
            {
                var query = @"
INSERT INTO[dbo].[PMS_DataRecord]
           ([ID]
           ,[CMND]
           ,[ThoiGian]
           ,[Status]
           ,[CongDoanID]
           ,[TrongLuong]
           ,[DateCreate]
           ,[DateSync])
     VALUES
           (@ID
           ,@CMND 
           ,@ThoiGian 
           ,@Status 
           ,@CongDoanID 
           ,@TrongLuong 
           ,@DateCreate 
           ,@DateSync)";
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

        public int Update<T>(List<T> items)
        {
            try
            {
                var query = @" UPDATE [dbo].[PMS_DataRecord]
   SET [CMND] = @CMND 
      ,[ThoiGian] = @ThoiGian 
      ,[Status] = @Status 
      ,[CongDoanID] = @CongDoanID 
      ,[TrongLuong] = @TrongLuong 
      ,[DateCreate] = @DateCreate 
      ,[DateSync] = @DateSync
 WHERE [ID] = @ID 
      ";
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

        public int Delete(List<string> ids)
        {
            var listOfIdsJoined = $@"('{string.Join("','", ids.ToArray())}')";
            var query = $@"delete from PMS_DataRecord where [ID] In {listOfIdsJoined}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var rows = connection.Execute(query);
                return rows;
            }
        }

        public int Delete<T>(List<T> items)
        {
            var query = @" Delete [dbo].[PMS_DataRecord] WHERE [ID] = @ID 
      ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var rows = connection.Execute(query, items);
                return rows;
            }
        }

    }
}
