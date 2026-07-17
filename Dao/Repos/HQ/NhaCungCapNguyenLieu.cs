using Microsoft.Data.SqlClient;
using System;
using Dapper;

namespace Dao.Repos.HQ
{
    public partial class NhaCungCapNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"NhaCungCapNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NhaCungCapNguyenLieu]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[NhaCungCapNguyenLieu]
           ([Ma]
           ,[Ten]
           ,[SuDung],[CCCD],[DiaChi])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung,@CCCD, @DiaChi)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[NhaCungCapNguyenLieu]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung, [CCCD] = @CCCD, [DiaChi] = @DiaChi
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from NhaCungCapNguyenLieu";

        public NhaCungCapNguyenLieu()
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
