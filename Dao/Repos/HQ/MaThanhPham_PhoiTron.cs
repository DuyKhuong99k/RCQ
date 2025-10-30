using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPham_PhoiTron
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPham_PhoiTron";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPham_PhoiTron]
      WHERE [Ngay] = @Ngay 
      and [MaThanhPhamOrg] = @MaThanhPhamOrg 
      and [MaThanhPhamDes] = @MaThanhPhamDes and MaXuong =@MaXuong and MaLo =@MaLo";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPham_PhoiTron]
           ([Ngay]
           ,[MaThanhPhamOrg]
           ,[MaThanhPhamDes]
           ,[MaKhuVuc]
           ,[TyLe]
           ,[CreateBy]
           ,[CreateDateTime]
           ,[ModifyBy]
           ,[ModifyDateTime],[MaXuong],[MaLo])
     VALUES
           (@Ngay 
           ,@MaThanhPhamOrg 
           ,@MaThanhPhamDes 
           ,@MaKhuVuc 
           ,@TyLe 
           ,@CreateBy 
           ,@CreateDateTime 
           ,@ModifyBy 
           ,@ModifyDateTime,@MaXuong ,@MaLo)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPham_PhoiTron]
   SET [MaKhuVuc] = @MaKhuVuc 
      ,[TyLe] = @TyLe 
      ,[CreateBy] = @CreateBy 
      ,[CreateDateTime] = @CreateDateTime 
      ,[ModifyBy] = @ModifyBy 
      ,[ModifyDateTime] = @ModifyDateTime 
 WHERE [Ngay] = @Ngay 
      and [MaThanhPhamOrg] = @MaThanhPhamOrg 
      and [MaThanhPhamDes] = @MaThanhPhamDes and MaXuong = @MaXuong and MaLo =@MaLo ";

        private readonly string qrGetAll = "Select * from MaThanhPham_PhoiTron";

        public MaThanhPham_PhoiTron(string? _connectionString = null)
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
        public List<T> GetsWithKhuVucDH<T>(DateTime dateTime, string maKhuVuc)
        {
            try
            {
                var query = @"select
p.Ngay,
p.MaThanhPhamOrg,
tpo.Ten as ThanhPhamOrgName,
p.MaThanhPhamDes,
tpd.Ten as ThanhPhamDesName,
p.MaKhuVuc,
p.MaXuong,
x.Ten as XuongName,
p.MaLo,
p.TyLe,
p.CreateBy,
p.CreateDateTime,
p.ModifyBy,
p.ModifyDateTime
from MaThanhPham_PhoiTron p
left join MaThanhPhamDinhHinh tpd on tpd.Ma = p.MaThanhPhamDes
left join MaThanhPhamDinhHinh tpo on tpo.Ma = p.MaThanhPhamOrg
left join XiNghiep x on x.Ma = p.MaXuong
where p.Ngay = @ngay and p.MaKhuVuc = @maKhuVuc
order by 
p.Ngay desc";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, maKhuVuc }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetsWithKhuVucFL<T>(DateTime dateTime, string maKhuVuc)
        {
            var query = @"select
p.Ngay,
p.MaThanhPhamOrg,
tpo.Ten as ThanhPhamOrgName,
p.MaThanhPhamDes,
tpd.Ten as ThanhPhamDesName,
p.MaKhuVuc,
p.MaXuong,
x.Ten as XuongName,
p.MaLo,
p.TyLe,
p.CreateBy,
p.CreateDateTime,
p.ModifyBy,
p.ModifyDateTime
from MaThanhPham_PhoiTron p
left join MaThanhPhamFillet tpd on tpd.Ma = p.MaThanhPhamDes
left join MaThanhPhamFillet tpo on tpo.Ma = p.MaThanhPhamOrg
left join XiNghiep x on x.Ma = p.MaXuong
where p.Ngay = @ngay and p.MaKhuVuc = @maKhuVuc
order by 
p.Ngay desc";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date, maKhuVuc }).ToList();
            return items;
        }
    }
}
