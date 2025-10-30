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
    public partial class PhieuCanBotCa
    {
        private readonly string connectionString;
        private readonly string queryDeleteByDate = @"DELETE FROM [dbo].[PhieuCanBotCa] WHERE  [Ngay] = @ngay";
        private readonly string queryGetAlls = @"Select * from PhieuCanBotCa";
        private readonly string queryGetsByDate = @"Select * from PhieuCanBotCa Where Ngay= @ngay";

        private readonly string queryInsert = @"INSERT INTO [dbo].[PhieuCanBotCa]
           ([ID]
           ,[ThoiGian]
           ,[Ngay]
           ,[MaKhachHang]
           ,[MaSanPham]
           ,[SoLuong]
           ,[TenKhachHang],[MaPhuongTien],[TenPhuongTien],[TenSanPham],)
     VALUES
           (@Ma
           ,@ThoiGian
           ,@Ngay
           ,@MaKhachHang
           ,@MaSanPham
           ,@SoLuong
           ,@TenKhachHang,@MaPhuongTien,@TenPhuongTien,@TenSanPham";
        public PhieuCanBotCa(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public int Delete(DateTime dateTime)
        {
            var query = queryDeleteByDate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, new { ngay = dateTime.Date });
            return rows;
        }
        public List<T> Gets<T>()
        {
            try
            {
                var query = queryGetAlls;
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = queryGetsByDate;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
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
