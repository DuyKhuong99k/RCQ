using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public partial class NguoiDung
    {
        private string connectionString;
        private string tableName = @"NguoiDung";
        private readonly string qrDelete = "DELETE FROM [dbo].[NguoiDung] WHERE [Id] = @Id";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[NguoiDung]
           ([UserName]
           ,[Password]
           ,[HoTen]
           ,[Email]
           ,[DefaultKey]
           ,[MaNhanVien]
           ,[IsAdmin]
           ,[Status]
           ,[IsActive]
           ,[PhoneNumber]
           ,[Address]
           ,[CardID]
           ,[AvatarImg])
     VALUES
           (@UserName
           ,@Password
           ,@HoTen
           ,@Email
           ,@DefaultKey
           ,@MaNhanVien
           ,@IsAdmin
           ,@Status
           ,@IsActive
           ,@PhoneNumber
           ,@Address
           ,@CardID
           ,@AvatarImg)
     
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[NguoiDung]
   SET [UserName] = @UserName
      ,[Password] = @Password
      ,[HoTen] = @HoTen
      ,[Email] = @Email
      ,[DefaultKey] = @DefaultKey
      ,[MaNhanVien] = @MaNhanVien
      ,[IsAdmin] = @IsAdmin
      ,[Status] = @Status
      ,[IsActive] = @IsActive
      ,[PhoneNumber] = @PhoneNumber
      ,[Address] = @Address
      ,[CardID] = @CardID
      ,[AvatarImg] = @AvatarImg
 WHERE Id =@Id
";

        private readonly string qrGetAll = "Select * from NguoiDung";

        public NguoiDung(string? _connectionString = null)
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
        public int? GetId(string userName)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<int?>($"select Id from NguoiDung where UserName = '{userName}'").FirstOrDefault();
            return rows;
        }

        public List<T> GetByUserName<T>(string userName)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>($"select * from NguoiDung where UserName = '{userName}'").ToList();
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
