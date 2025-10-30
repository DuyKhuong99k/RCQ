using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaMauCaGiongVungNuoi
    {
        private readonly string connectionString;
        private string tableName = @"MaMauCaGiongVungNuoi";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaMauCaGiongVungNuoi]
      WHERE [Ngay] = @Ngay
      and [MaGhe] = @MaGhe";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaMauCaGiongVungNuoi]
           ([Ngay]
           ,[MaGhe]
           ,[SoLuong]
           ,[TrongLuongDonVi]
           ,[TrongLuong])
     VALUES
           (@Ngay
           ,@MaGhe
           ,@SoLuong
           ,@TrongLuongDonVi
           ,@TrongLuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaMauCaGiongVungNuoi]
   SET [SoLuong] = @SoLuong
      ,[TrongLuongDonVi] = @TrongLuongDonVi
      ,[TrongLuong] = @TrongLuong
 WHERE [Ngay] = @Ngay
      and [MaGhe] = @MaGhe";

        private readonly string qrGetAll = "Select * from MaMauCaGiongVungNuoi";

        public MaMauCaGiongVungNuoi()
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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
p.Ngay,
p.MaGhe,
g.Ten as GheName,
p.SoLuong,
p.TrongLuongDonVi,
p.TrongLuong
from MaMauCaGiongVungNuoi p, MaGheVungNuoi g
where p.MaGhe = g.Ma";
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
