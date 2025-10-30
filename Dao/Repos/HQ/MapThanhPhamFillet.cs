using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MapThanhPhamFillet
    {
        private readonly string connectionString;
        private string tableName = @"MaNhomXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MapThanhPhamFillet]
       WHERE [MaLoaiCaFillet] = @MaLoaiCaFillet 
      and [MaTPFillet] = @MaTPFillet
      and [MaSize] = @MaSize
      adn [IsTangCa] = @IsTangCa";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MapThanhPhamFillet]
           ([MaLoaiCaFillet]
           ,[MaTPFillet]
           ,[MaSize]
           ,[MaBravoFillet]
           ,[IsTangCa])
     VALUES
           (@MaLoaiCaFillet
           ,@MaTPFillet
           ,@MaSize
           ,@MaBravoFillet
           ,@IsTangCa)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MapThanhPhamFillet]
   SET [MaLoaiCaFillet] = @MaLoaiCaFillet 
      ,[MaTPFillet] = @MaTPFillet
      ,[MaSize] = @MaSize
      ,[MaBravoFillet] = @MaBravoFillet
      ,[IsTangCa] = @IsTangCa
 WHERE [MaLoaiCaFillet] = @MaLoaiCaFillet 
      and [MaTPFillet] = @MaTPFillet
      and [MaSize] = @MaSize
      and [IsTangCa] = @IsTangCa";

        private readonly string qrGetAll = "Select * from MaNhomXepKhuon";

        public MapThanhPhamFillet(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

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
mtp.MaLoaiCaFillet,
lc.Ten as LoaiCaName,
mtp.MaTPFillet,
tp.Ten as ThanhPhamName,
mtp.MaSize,
s.Ten as SizeName,
mtp.MaBravoFillet,
tl.Ten as SLTLName,
mtp.IsTangCa
from MapThanhPhamFillet mtp
left join MaLoaiCaFillet lc on lc.Ma = mtp.MaLoaiCaFillet
left join MaThanhPhamFillet tp on tp.Ma = mtp.MaTPFillet
left join MaSizeFillet s on s.Ma = mtp.MaSize
left join DG_SanPhamTinhLuong tl on tl.Ma = mtp.MaBravoFillet

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
    }
}
