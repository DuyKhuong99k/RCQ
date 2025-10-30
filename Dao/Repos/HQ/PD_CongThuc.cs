using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_CongThuc
    {
        private readonly string connectionString;
        private string tableName = @"PD_CongThuc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PD_CongThuc]
      WHERE [Ma] = @Ma";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PD_CongThuc]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[TrongLuongNguyenLieu]
           ,[MaDonViTinh]
           ,[GhiChu]
           ,[CreateDateTime]
           ,[CreateBy]
           ,[ModifiedDateTime]
           ,[ModifiedBy])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung 
           ,@TrongLuongNguyenLieu
           ,@MaDonViTinh
           ,@GhiChu 
           ,@CreateDateTime 
           ,@CreateBy 
           ,@ModifiedDateTime 
           ,@ModifiedBy)";

        private readonly string qrUpdate = @"UPDATE[dbo].[PD_CongThuc]
                SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[TrongLuongNguyenLieu] =@TrongLuongNguyenLieu
      ,[MaDonViTinh] =@MaDonViTinh
      ,[GhiChu] = @GhiChu
      ,[CreateDateTime] =@CreateDateTime
      ,[CreateBy] = @CreateBy
      ,[ModifiedDateTime] = @ModifiedDateTime
      ,[ModifiedBy] = @ModifiedBy
  WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from PD_CongThuc";

        public PD_CongThuc(string? _connectionString = null)
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
ct.Ma,
ct.Ten,
ct.SuDung,
ct.TrongLuongNguyenLieu,
ct.MaDonViTinh,
dv.Ten as DonViTinhName,
ct.GhiChu,
ct.CreateDateTime,
ct.CreateBy,
ct.ModifiedDateTime,
ct.ModifiedBy
from PD_CongThuc ct
left join PD_DonViTinh dv on ct.MaDonViTinh = dv.Ma";
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
    }
}
