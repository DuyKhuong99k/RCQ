using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MocThoiGianPhanCa
    {
        private readonly string connectionString;
        private string tableName = @"MocThoiGianPhanCa";
        private readonly string qrDelete = @"
DELETE FROM [dbo].[MocThoiGianPhanCa]
      WHERE [Ngay] = @Ngay and [MaXuong] = @MaXuong
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MocThoiGianPhanCa]
           ([Ngay]
           ,[Gio]
           ,[CreateDateTime]
           ,[CreateBy]
           ,[ModifyDateTime]
           ,[ModifyBy],[MaXuong])
     VALUES
           (@Ngay 
           ,@Gio 
           ,@CreateDateTime 
           ,@CreateBy 
           ,@ModifyDateTime 
           ,@ModifyBy ,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MocThoiGianPhanCa]
   SET [Gio] = @Gio
      ,[CreateDateTime] = @CreateDateTime 
      ,[CreateBy] = @CreateBy 
      ,[ModifyDateTime] = @ModifyDateTime
      ,[ModifyBy] = @ModifyBy
 WHERE [Ngay] = @Ngay and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from MocThoiGianPhanCa";

        public MocThoiGianPhanCa()
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
        public T Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from MocThoiGianPhanCa where Ngay =@ngay and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId = xuongId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
