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
    public partial class PhieuCanCaNgop
    {
        private readonly string connectionString;
        private readonly string queryDeletebyDate = @"DELETE FROM [dbo].[PhieuCanCaNgop] WHERE  [Ngay] = @ngay";
        private readonly string queryGetsAll = @"Select * from PhieuCanCaNgop";
        private readonly string queryGetsByDate = @"Select * from PhieuCanCaNgop Where Ngay =@ngay";

        private readonly string queryInsert = @"INSERT INTO [dbo].[PhieuCanCaNgop]
           ([ID]
           ,[ThoiGian]
           ,[Ngay]
           ,[MaNhaCungCap]
           ,[MaVung]
           ,[MaAo]
           ,[MaCa]
           ,[SoLuong]
           ,[TenNhaCungCap],[MaPhuongTien],[TenPhuongTien],[TenVungNuoi],[TenAo],[TenCa].[MaPhuongTien])
     VALUES
           (@Ma
           ,@ThoiGian
           ,@Ngay
           ,@MaNhaCungCap
           ,@MaVung
           ,@MaAo,@MaCa,@SoLuong
           ,@TenNhaCungCap,@MaPhuongTien,@TenPhuongTien,@TenVungNuoi,@TenAo,@TenCa,@MaPhuongTien)";
        public PhieuCanCaNgop(string? _connectionString = null)
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            var query = queryGetsAll;
            using var connnection = new SqlConnection(connectionString);
            connnection.Open();
            var rows = connnection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
            return rows;
        }
        public List<T> Gets<T>()
        {
            var query = queryGetsAll;
            using var connnection = new SqlConnection(connectionString);
            connnection.Open();
            var rows = connnection.Query<T>(query).ToList();
            return rows;
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
