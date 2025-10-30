using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos
{
    public class Database
    {
        public Database(string connectionString = null)
        {
            ConnectionString = connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }

        public void DbContextUpdateDatabase()
        {
            try
            {
                using var dbContext = new Models.Repos.dbPMScontext();
                dbContext.Database.SetConnectionString(ConnectionString);
                //dbContext.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private string ConnectionString { get; }

        public bool CheckDatabaseExists(string dataBase)
        {
            var cmdText = "SELECT * FROM master.dbo.sysdatabases WHERE name ='" + dataBase + "'";
            var isExist = false;
            using (var con = new SqlConnection(ConnectionString))
            {
                con.ChangeDatabase("master");
                con.Open();
                using (var cmd = new SqlCommand(cmdText, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        isExist = reader.HasRows;
                    }
                }

                con.Close();
            }

            return isExist;
        }

        public bool CheckServerOnline()
        {
            try
            {
                using var connnection = new SqlConnection(ConnectionString);
                var query = @"Select 1 ";
                connnection.Open();
                var item = connnection.QueryAsync<int>(query).Result.SingleOrDefault();
                if (item == 1)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public void CreateDatabase(string dataBase)
        {
            using var con = new SqlConnection(ConnectionString);
            con.ChangeDatabase("master");
            var str = "CREATE DATABASE " + dataBase;
            using var cmd = new SqlCommand(str, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public int ExecuteNonQuery(string query)
        {
            try
            {
                using var connection = new SqlConnection(ConnectionString);
                connection.Open();
                var item = connection.Execute(query);
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public DateTime GetSysDateTime()
        {
            using var connnection = new SqlConnection(ConnectionString);
            var query = @"Select SYSDATETIME() as 'SYSDATETIME()';";
            connnection.Open();
            var item = connnection.Query<DateTime>(query).SingleOrDefault();
            return item;
        }
    }
}
