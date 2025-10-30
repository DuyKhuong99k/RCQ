using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsEx;
using Dao.Repos.HQ;
using Dao.Repos;
namespace BravoModelV1.Dao
{
    public partial class PhieuCanSanXuat
    {
        private readonly string connectionString;
        private readonly string queryGetsByDateCongDoan =
            @"Select * from PhieuCanSanXuat Where Ngay =@ngay and MaCongDoan = @congDoanId";
        private readonly string queryDeleteByDateCongDoan = @"DELETE FROM [dbo].[PhieuCanSanXuat]
      WHERE  Ngay =@ngay and MaCongDoan = @congDoanId";
        public PhieuCanSanXuat(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public List<T> Gets<T>(DateTime dateTime, string congDoanId)
        {
            var query = queryGetsByDateCongDoan;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(query, new { ngay = dateTime.Date, congDoanId }).ToList();
            return rows;
        }
        public int InsertBatch<T>(List<T> items)
        {
            var batches = DbExtensions.GetSqlsInBatches(items);
            var row = 0;
            var database = new Database(connectionString);
            foreach (var batche in batches) row += database.ExecuteNonQuery(batche);
            return row;
        }
        public int Delete(DateTime dateTime, string congDoanId)
        {
            var query = queryDeleteByDateCongDoan;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, new { ngay = dateTime.Date, congDoanId });
            return rows;
        }
    }
}
