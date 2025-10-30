using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Dao
{
    public partial class BravoCheckInOut
    {
        private readonly string connectionString;
        public BravoCheckInOut(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public List<string> GetEmployeeCodes(DateTime dateTime)
        {
            var query = @"select distinct EmployeeCode from BravoCheckInOut where CheckDate = @ngay";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<string>(
                        query,
                        new { ngay = dateTime.Date})
                    .Result
                    .ToList();
                return items;
            }
        }

    }
}
