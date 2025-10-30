using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class LoTheoLine
    {
        private readonly string connectionString;
        private string tableName = @"LoTheoLine";
        private readonly string qrDelete = @"DELETE FROM [dbo].[LoTheoLine] WHERE Id = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[LoTheoLine]
           ([Id]
           ,[MaLine]
           ,[MaLo]
           ,[Ngay]
           ,[Gio],[CodeId])
     VALUES
           (@Id
           ,@MaLine
           ,@MaLo 
           ,@Ngay 
           ,@Gio,@CodeId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[LoTheoLine]
   SET [MaLine] = @MaLine
      ,[MaLo] = @MaLo
      ,[Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[CodeId] = @CodeId
 WHERE Id = @Id";


        private readonly string qrGetAll = "Select * from LoTheoLine";

        public LoTheoLine()
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

        public List<T> GetLoTheoLines<T>(DateTime ngay,string maLo)
        {
            try
            {
                var query = @"
                select
                p.Id,
                p.CodeId,
                p.MaLo,
                p.MaLine,
                l.Ten as LineName,
                p.Ngay,
                p.Gio
                from LoTheoLine p
                left join LineFilletv2 l on p.MaLine = l.Ma
                where p.Ngay = @ngay and p.MaLo = @maLo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date, maLo = maLo }).Result
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

        public List<T> GetLoTheoLinesMoiNhat<T>(DateTime ngay,string maLo)
        {
            try
            {
                var query = @"
                ;WITH Ranked AS (
                    SELECT 
                        p.Id,
                        p.CodeId,
                        p.Ngay,
                        p.Gio,
		                p.MaLo,
                        p.MaLine,
                        l.Ten AS LineName,
                        ROW_NUMBER() OVER (
                            PARTITION BY p.MaLine
                            ORDER BY 
                                TRY_CONVERT(time(0), p.Gio) DESC,
                                p.Id DESC
                        ) AS rn
                    FROM LoTheoLine p
    
                    LEFT JOIN LineFilletv2 l ON p.MaLine = l.Ma
                    WHERE p.Ngay = @ngay
                      AND p.MaLo = @maLo

                )
                SELECT 
                    Id,MaLo,MaLine, LineName, Ngay, Gio,CodeId
                FROM Ranked
                WHERE rn = 1
                ORDER BY TRY_CONVERT(time(0), Gio) DESC;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date, maLo =maLo}).Result
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
