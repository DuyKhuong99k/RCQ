using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public class PLC
    {
        private readonly string connectionString;
        private string tableName = @"PLC";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PLC]
      WHERE Id=@Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PLC]
           ([Id]
           ,[Name]
           ,[IP]
           ,[Port])
     VALUES
           (@Id
           ,@Name
           ,@IP
           ,@Port)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[PLC]
   SET [Name] = @Name
      ,[IP] = @IP
      ,[Port] = @Port
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from PLC";

        public PLC()
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
