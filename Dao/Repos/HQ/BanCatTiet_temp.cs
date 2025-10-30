using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BanCatTiet_temp
    {
        private readonly string connectionString;
        private string tableName = @"BanCatTiet_temp";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BanCatTiet_temp]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BanCatTiet_temp]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[X1]
           ,[Y1]
           ,[X2]
           ,[Y2]
           ,[ColSpanX1]
           ,[ColSpanX2]
           ,[IsShowX1]
           ,[IsShowX2]
           ,[IsDetect]
           ,[IsFalse])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@X1
           ,@Y1
           ,@X2
           ,@Y2
           ,@ColSpanX1
           ,@ColSpanX2
           ,@IsShowX1
           ,@IsShowX2
           ,@IsDetect
           ,@IsFalse)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BanCatTiet_temp]
   SET [Ten] = @Ten
           ,[SuDung] = @SuDung
           ,[X1] = @X1
           ,[Y1] = @Y1
           ,[X2] =@X2
           ,[Y2] =@Y2
           ,[ColSpanX1] =@ColSpanX1
           ,[ColSpanX2] =@ColSpanX2
           ,[IsShowX1] =@IsShowX1
           ,[IsShowX2] = @IsShowX2
           ,[IsDetect]=@IsDetect
           ,[IsFalse]=@IsFalse
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from BanCatTiet_temp";

        public BanCatTiet_temp()
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
