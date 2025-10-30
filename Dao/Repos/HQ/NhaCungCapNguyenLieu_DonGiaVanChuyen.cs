using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NhaCungCapNguyenLieu_DonGiaVanChuyen
    {
        private readonly string connectionString;
        private string tableName = @"NhaCungCapNguyenLieu_DonGiaVanChuyen";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NhaCungCapNguyenLieu_DonGiaVanChuyen]
      WHERE [MaNhaCC] = @MaNhaCC
      and [NgayApDung] = @NgayApDung";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NhaCungCapNguyenLieu_DonGiaVanChuyen]
           ([MaNhaCC]
           ,[NgayApDung]
           ,[DonGia]
           ,[GhiChu])
     VALUES
           (@MaNhaCC
           ,@NgayApDung
           ,@DonGia
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NhaCungCapNguyenLieu_DonGiaVanChuyen]
   SET [DonGia] = @DonGia
      ,[GhiChu] = @GhiChu 
 WHERE [MaNhaCC] = @MaNhaCC
      and [NgayApDung] = @NgayApDung";

        private readonly string qrGetAll = "Select * from NhaCungCapNguyenLieu_DonGiaVanChuyen";

        public NhaCungCapNguyenLieu_DonGiaVanChuyen()
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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
  dgvc.MaNhaCC,
  ncc.Ten,
  dgvc.NgayApDung,
  dgvc.DonGia,
  dgvc.GhiChu
  from 
  NhaCungCapNguyenLieu_DonGiaVanChuyen dgvc,
  NhaCungCapNguyenLieu ncc
  where dgvc.MaNhaCC = ncc.Ma";
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
