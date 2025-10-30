using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamDinhHinh
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamDinhHinh";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamDinhHinh]
      WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamDinhHinh]
           ([MaCa]
           ,[Ma]
           ,[Ten]
           ,[SuDung]
           ,[DinhMuc]
           ,[Min]
           ,[Max],[BravoId],[TyLeDinhMucDau],[TyLeDinhMucRot],[TrongLuongTare],[IsDauVaoBatBuoc],[DinhMucKhongDauVao],[IsDisplay],[CodeId],[DinhMucCaTra],[BaoCaoDauRot],[IsSuDungThoiGianGiuaLoaiThanhPham])
     VALUES
           (@MaCa 
           ,@Ma 
           ,@Ten 
           ,@SuDung 
           ,@DinhMuc 
           ,@Min 
           ,@Max,@BravoId,@TyLeDinhMucDau,@TyLeDinhMucRot,@TrongLuongTare,@IsDauVaoBatBuoc, @DinhMucKhongDauVao,@IsDisplay,@CodeId,@DinhMucCaTra,@BaoCaoDauRot,@IsSuDungThoiGianGiuaLoaiThanhPham)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamDinhHinh]
   SET [Ten] = @Ten 
      ,[SuDung] = @SuDung 
      ,[DinhMuc] = @DinhMuc 
      ,[Min] = @Min 
      ,[Max] = @Max,[BravoId]= @BravoId ,[TyLeDinhMucDau] =@TyLeDinhMucDau,[TyLeDinhMucRot] =@TyLeDinhMucRot, [TrongLuongTare] = @TrongLuongTare,[IsDauVaoBatBuoc]= @IsDauVaoBatBuoc,[DinhMucKhongDauVao]=@DinhMucKhongDauVao,[IsDisplay] = @IsDisplay,[CodeId] =@CodeId,[DinhMucCaTra] = @DinhMucCaTra,[BaoCaoDauRot] =@BaoCaoDauRot,[IsSuDungThoiGianGiuaLoaiThanhPham]=@IsSuDungThoiGianGiuaLoaiThanhPham
 WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from MaThanhPhamDinhHinh";

        public MaThanhPhamDinhHinh(string? _connectionString = null)
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
tp.Ma,
tp.MaCa,
tp.Ten,
tp.DinhMuc,
tp.BravoId,
sptl.Ten as SanPhamTinhLuongName,
tp.Min as TLMin,
tp.Max as TLMax,
tp.MinOut as TLMinOut,
tp.MaxOut as TLMaxout,
tp.TyLeDinhMucDau,
tp.TyLeDinhMucRot,
tp.TrongLuongTare,
tp.IsDauVaoBatBuoc,
tp.DinhMucKhongDauVao,
tp.DinhMucCaTra,
tp.IsSuDungThoiGianGiuaLoaiThanhPham,
tp.BaoCaoDauRot,
tp.SuDung,
tp.CodeId,
x.Ten as XuongName
from MaThanhPhamDinhHinh tp
left join DG_SanPhamTinhLuong sptl on tp.BravoId = sptl.Ma
left join XiNghiep x on tp.CodeId = x.Ma
--where tp.CodeId = @xuongId
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
        public List<T> GetsSanPhamTinhLuong<T>()
        {
            try
            {
                var query = "Select BravoId as Id,Ten as [Name],Ma as DaiThanhId from MaThanhPhamDinhHinh";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query).Result.ToList();
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
