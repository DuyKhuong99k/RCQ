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
    public partial class PhieuCanCaThit
    {
        private readonly string connectionString;
        private readonly string queryDeletebyDate = @"DELETE FROM [dbo].[PhieuCanCaThit] WHERE  [Ngay] = @ngay";
        private readonly string queryGetsByDate = @"Select * from PhieuCanCaThit Where Ngay = @ngay";
        private readonly string queryGetsAll = @"Select * from PhieuCanCaThit";
        private readonly string queryInsert = @"INSERT INTO [dbo].[PhieuCanCaThit]
           ([ID]
           ,[ThoiGian]
           ,[Ngay]
           ,[MaNhaCungCap]
           ,[MaVung]
           ,[MaAo]
           ,[MaCa]
           ,[SoLuong]
           ,[TenNhaCungCap],[MaPhuongTien],[TenPhuongTien],[TenVungNuoi],[TenAoNuoi],[TenCa])
)
     VALUES
           (@Ma
           ,@ThoiGian
           ,@Ngay
           ,@MaNhaCungCap
           ,@MaVung
           ,@MaAo,@MaCa,@SoLuong
           ,@TenNhaCungCap,@MaPhuongTien,@TenPhuongTien,@TenVungNuoi,@TenAoNuoi,@TenCa)";
        public PhieuCanCaThit(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public int Delete(DateTime dateTime)
        {
            var query = queryDeletebyDate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, new { ngay = dateTime.Date });
            return rows;
        }
        public List<T> Gets<T>(DateTime date)
        {
            try
            {
                var query = queryGetsByDate;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query<T>(query, new { ngay = date.Date }).ToList();
                return rows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<T> Gets<T>()
        {
            try
            {
                var query = queryGetsAll;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query<T>(query).ToList();
                return rows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public int Insert<T>(List<T> items)
        {
            var query = queryInsert;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
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
    }
}
