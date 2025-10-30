using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TheTu
    {
        private readonly string connectionString;
        private string tableName = @"TheTu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TheTu]
      WHERE [MaTheTu] = @MaTheTu ";

        private readonly string qrInsert = @"INSERT INTO [dbo].[TheTu]
           ([MaTheTu]
           ,[MaNhanVien]
           ,[NgayGio]
           ,[PCName])
     VALUES
           (@MaTheTu 
           ,@MaNhanVien 
           ,@NgayGio 
           ,@PCName )";

        private readonly string qrUpdate = @"UPDATE [dbo].[TheTu]
   SET [MaNhanVien] = @MaNhanVien, [NgayGio] = @NgayGio, 
       [PCName] = @PCName
 WHERE [MaTheTu] = @MaTheTu ";

        private readonly string qrGetAll = @"Select p.*,
    nv.Name as NhanVienName,
    nv.MaHoSo as MaSo
from TheTu p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien";

        public TheTu()
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
        public int Delete<T>(List<T> items)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, items);
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

        public int Update<T>(List<T> items)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, items);
            return rows;
        }
    }
}
