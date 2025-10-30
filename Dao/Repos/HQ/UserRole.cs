using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public partial class UserRole
    {
        private string connectionString;
        private string tableName = @"UserRole";
        private readonly string qrDelete = "DELETE FROM [dbo].[UserRole] WHERE [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[UserRole]
           (
           [UserId]
           ,[RoleId])
          
     VALUES
           (
      ,@UserId
      ,@RoleId
     )";

        private readonly string qrUpdate = @"
";

        private readonly string qrGetAll = "Select * from UserRole";

        public UserRole(string? _connectionString = null)
        {
            connectionString =_connectionString??AppViewModels.Base.Ins.ConnectionString;

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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
rp.Id,
rp.RoleId,
r.Name as RoleName,
rp.Fu,
rp.Func,
rp.CreatedDateTime,
rp.Status

from 
RolePermistion rp,
Role r
where
rp.RoleId = r.Id
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetsFullField<T>(int userId)
        {
            try
            {
                var query = @"select * from
(select
us.Id,
us.UserId,
nd.UserName,
us.RoleId,r.Name as RoleName
from 
UserRole us,
NguoiDung nd,
Role r
where 
us.UserId = nd.Id
and us.RoleId = r.Id) p
where p.UserId = @userId
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new{userId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetsFullFieldById<T>(int userRoleId)
        {
            try
            {
                var query = @"select * from
(select
us.Id,
us.UserId,
nd.UserName,
us.RoleId,r.Name as RoleName
from 
UserRole us,
NguoiDung nd,
Role r
where 
us.UserId = nd.Id
and us.RoleId = r.Id) p
where p.Id = @userRoleId
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new{userRoleId}).ToList();
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
