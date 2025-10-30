using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Dao.Repos.HQ
{
    public partial class RolePermistion
    {
        private readonly dbPMScontext? _context;
        private string connectionString;
        private string tableName = @"RolePermistion";
        private readonly string qrDelete = @"DELETE FROM [dbo].[RolePermistion]
      WHERE [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[RolePermistion]
           (
           [RoleId],
           [Fu]
           ,[Func]
           ,[CreatedDateTime]
           ,[Status])
     VALUES
           (
       @RoleId
      ,@Fu
      ,@Func
      ,@CreatedDateTime
      ,@Status)";

        private readonly string qrUpdate = @"UPDATE [dbo].[RolePermistion]
   SET [RoleId] = @RoleId
      ,[Fu] = @Fu
      ,[Func] = @Func
      ,[CreatedDateTime] =@CreatedDateTime
      ,[Status] = @Status
 WHERE [Id] = @Id";

        private readonly string qrGetAll = "Select * from RolePermistion";
        private readonly string qrGetsLastAll = @"SELECT Id, RoleId, Fu, Func, CreatedDateTime,Status
FROM (SELECT 
		Id,
        RoleId,
        Fu,
        Func,
        CreatedDateTime,
		status,
        ROW_NUMBER() OVER (PARTITION BY RoleId,
        Fu,
        Func ORDER BY CreatedDateTime DESC) AS RowNum
    FROM RolePermistion) p
WHERE RowNum = 1;";

        public RolePermistion(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public RolePermistion(object context)
        {
            _context = context as dbPMScontext;
            connectionString = _context.Database.GetConnectionString();

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
        public List<T> GetsLastAll<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetsLastAll).ToList();
            return rows;
        }

        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }
        public int Insert(List<Models.Repos.Models.RolePermistion> items)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, items);
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
        public List<T> GetsFullField<T>(int roleId)
        {
            try
            {
                var query = @"select * from(
select
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
rp.RoleId = r.Id) p
where 
p.RoleId = @roleId
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { roleId }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Models.Repos.Models.RolePermistion> GetAllRolePermissions()
        {
            try
            {
                var rolePermissions = _context.RolePermistion.OrderByDescending(x => x.Id).ToList();
                return rolePermissions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public Models.Repos.Models.RolePermistion GetRolePermistionById(int permistionId)
        {
            try
            {
                var rolePermission = _context.RolePermistion.Where(x => x.Id == permistionId).First();
                return rolePermission;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetAllRolePermistionByRoleId<T>(int roleId)
        {
            try
            {
                //var rolePermissions = _context.RolePermistion.Where(x => x.RoleId == roleId).ToList();
                //return rolePermissions;
                var query =
               "select * FROM RolePermistion WHERE RoleId = @roleId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { roleId }).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Models.Repos.Models.RolePermistion> GetRolePermissions(string fu, string func)
        {
            try
            {
                // var query =
                //@"select * from RolePermistion
                // where Fu = @fu and Func = @func
                // order by CreatedDateTime desc";
                // if (_context == null)
                // {
                //     using (var connection = new SqlConnection(connectionString))
                //     {
                //         connection.Open();
                //         var items = connection.QueryAsync<Models.Repos.Models.RolePermistion>(query, new { fu, func }).Result.ToList();
                //         return items;
                //     }
                // }
                // else
                // {
                var rolePermissions = _context.RolePermistion
           .Where(x => x.Fu == fu && x.Func == func)
           .OrderByDescending(x => x.CreatedDateTime)
           .GroupBy(x => x.RoleId)
           .Select(group => group.First())
           .ToList();

                return rolePermissions;
            //}

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
