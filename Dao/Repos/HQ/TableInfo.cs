using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{

    public partial class TableInfo
    {
         private readonly string connectionString;
        public TableInfo(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetsTableInfo<T>(string tableName)
        {
            try
            {
                var query = @"
SELECT 
    t.name AS TableName, 
    SUM(p.rows) AS RecordCount,
    MIN(CreationTime) AS CreationTime,
    MAX(RecentTime) AS RecentTime,
    DATEDIFF(DAY, MIN(CreationTime), MAX(RecentTime)) AS TimeElapsed,
    SUM(a.total_pages * 8.0) / 1024 AS SizeMB
FROM 
    sys.tables t
INNER JOIN 
    sys.indexes i ON t.object_id = i.object_id
INNER JOIN 
    sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
INNER JOIN 
    sys.allocation_units a ON p.partition_id = a.container_id
INNER JOIN 
    (
        SELECT 
            MIN(Ngay) AS CreationTime,
            MAX(Ngay) AS RecentTime
        FROM 
            " + tableName + @"
    ) AS subquery ON 1=1
WHERE 
    t.name = @tableName
GROUP BY 
    t.name;";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new {tableName}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
