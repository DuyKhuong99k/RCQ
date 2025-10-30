using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class DG_DonGia
    {
        private readonly string connectionString;
        private string tableName = @"DG_DonGia";
        private readonly string qrDelete = @" DELETE FROM [dbo].[DG_DonGia]
      WHERE [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[DG_DonGia]
           ([Id]
           ,[Ngay]
           ,[Gio]
           ,[MaSanPham]
           ,[MaLoaiDonGia]
           ,[DanhGia]
           ,[DinhMucDown]
           ,[DinhMucUp]
           ,[HeSo]
           ,[DonGia]
           ,[CreateBy]
           ,[CreateDateTime]
           ,[ModifiedBy]
           ,[ModifiedDateTime]
           ,[GhiChu],[HeSoRot],[IsUsedHeSoRot],[Range],[MaSizeDinhHinh],[DonGiaGiaCong],[MaXepHang],[MaThanhPham],[LoaiCan],[MaSizeFillet])
     VALUES
           (@Id
           ,@Ngay
           ,@Gio
           ,@MaSanPham
           ,@MaLoaiDonGia
           ,@DanhGia
           ,@DinhMucDown
           ,@DinhMucUp
           ,@HeSo
           ,@DonGia
           ,@CreateBy
           ,@CreateDateTime
           ,@ModifiedBy
           ,@ModifiedDateTime
           ,@GhiChu,@HeSoRot,@IsUsedHeSoRot,@Range,@MaSizeDinhHinh,@DonGiaGiaCong,@MaXepHang,@MaThanhPham,@LoaiCan,@MaSizeFillet)";
        private readonly string qrUpdate = @"UPDATE [dbo].[DG_DonGia]
   SET [Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[MaSanPham] = @MaSanPham
      ,[MaLoaiDonGia] = @MaLoaiDonGia
      ,[DanhGia] = @DanhGia
      ,[DinhMucDown] = @DinhMucDown
      ,[DinhMucUp] = @DinhMucUp
      ,[HeSo] = @HeSo
      ,[DonGia] = @DonGia
      ,[CreateBy] = @CreateBy
      ,[CreateDateTime] = @CreateDateTime
      ,[ModifiedBy] = @ModifiedBy
      ,[ModifiedDateTime] = @ModifiedDateTime
      ,[GhiChu] = @GhiChu
        ,[HeSoRot] = @HeSoRot
    ,[IsUsedHeSoRot]= @IsUsedHeSoRot,[MaXepHang] =@MaXepHang
,[Range] = @Range ,[MaSizeDinhHinh] = @MaSizeDinhHinh,[DonGiaGiaCong] =@DonGiaGiaCong,[MaThanhPham] = @MaThanhPham,[LoaiCan] = @LoaiCan, [MaSizeFillet] = @MaSizeFillet
 WHERE [Id] = @Id";

        private readonly string qrGetAll = "Select * from DG_DonGia";

        public DG_DonGia(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetsAllList<T>(DateTime dateTime, bool isLast = false)
        {
            try
            {
                var query = @"Select * from DG_DonGia where ngay =@ngay";
                if (isLast == true)
                {
                    query = @"SELECT dg.*,
    sp.Ten as SanPhamName,@ngay as NgayDonGia
from (
        Select *
        from (
                Select *,
                    ROW_NUMBER() over (
                        partition by MaSanPham,
                        MaThanhPham,
                        LoaiCan,
                        DanhGia,
                        DinhMucDown,
                        MaLoaiDonGia,
                        MaSizeDinhHinh,
                        MaXepHang,
                        MaSizeFillet
                        order by Ngay DESC,
                            Gio Desc
                    ) as rowId
                from DG_DonGia
                where Ngay <= @ngay
            ) p
        where p.rowId = 1
    ) dg
    LEFT JOIN DG_SanPhamTinhLuong sp on dg.MaSanPham = sp.Ma";
                }

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                //foreach (var item in items)
                //{
                //    var type = item.GetType();
                //}

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string loaiDonGiaId, bool isLast = false, string loaiCan = "")
        {
            try
            {
                var query = @"Select * from DG_DonGia where ngay =@ngay and MaLoaiDonGia = @loaiDonGiaId";
                if (isLast == true)
                {
                    query = @"Select
    *
from
    (
        Select
            *,
            ROW_NUMBER() over (
                partition by MaSanPham,MaThanhPham,LoaiCan,DanhGia,DinhMucUp,
                        DinhMucDown
                order by
                    Ngay DESC,
                    Gio Desc
            ) as rowId
        from
            DG_DonGia
        where
            Ngay <= @ngay and MaLoaiDonGia = @loaiDonGiaId
            and LoaiCan = @loaiCan
    ) p
where
    p.rowId = 1";
                }

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, loaiDonGiaId, loaiCan }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public T Gets<T>(DateTime dateTime, string sanPhamId)
        {
            try
            {
                var query = @"Select
    *
from
    (
        Select
            *,
            ROW_NUMBER() over (
                partition by MaSanPham,MaThanhPham,LoaiCan,DanhGia
                order by
                    Ngay DESC,
                    Gio Desc
            ) as rowId
        from
            DG_DonGia
        where
            Ngay <= @ngay and MaSanPham = @sanPhamId
    ) p
where
    p.rowId = 1";

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date, sanPhamId }).FirstOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, bool isLast = false,string loaiCan ="")
        {
            try
            {
                var query = @"Select * from DG_DonGia where ngay =@ngay";
                if (isLast == true)
                {
                    query = @"Select
    *
from
    (
        Select
            *,
            ROW_NUMBER() over (
                partition by  MaSanPham,MaThanhPham,LoaiCan,DanhGia,DinhMucDown,MaLoaiDonGia,MaSizeDinhHinh,MaXepHang,MaSizeFillet
                order by
                    Ngay DESC,
                    Gio Desc
            ) as rowId
        from
            DG_DonGia
        where
            Ngay <= @ngay and LoaiCan = @loaiCan
    ) p
where
    p.rowId = 1";
//                    query = @"Select
//    *
//from
//    (
//        Select
//            *,
//            ROW_NUMBER() over (
//                partition by  MaSanPham,DanhGia,DinhMucDown,MaLoaiDonGia,MaSizeDinhHinh,MaXepHang
//                order by
//                    Ngay DESC,
//                    Gio Desc
//            ) as rowId
//        from
//            DG_DonGia
//        where
//            Ngay <= @ngay
//    ) p
//where
//    p.rowId = 1";
                }

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date ,loaiCan }).Result.ToList();
                //foreach (var item in items)
                //{
                //    var type = item.GetType();
                //}

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
        public List<T> GetsFullFieldByYear<T>()
        {
            try
            {
                var query = @"select 
dg.Id
      ,dg.Ngay
      ,dg.Gio
      ,dg.MaSanPham
      ,dg.MaLoaiDonGia
      ,dg.DanhGia
      ,dg.DinhMucDown
      ,dg.DinhMucUp
      ,dg.HeSo
      ,dg.DonGia
      ,dg.CreateBy
      ,dg.CreateDateTime
      ,dg.ModifiedBy
      ,dg.ModifiedDateTime
      ,dg.GhiChu
      ,dg.HeSoRot
      ,dg.IsUsedHeSoRot
      ,dg.Range
      ,dg.MaSizeDinhHinh
      ,dg.DonGiaGiaCong
      ,dg.MaXepHang
      ,dg.MaThanhPham
      ,dg.LoaiCan
      ,dg.MaSizeFillet,
tp.Ten as ThanhPhamFilletName,
ldg.Ten as LoaiDonGiaName,
sdh.Ten as SizeDinhHinhName,
sfl.Ten as SizeFilletName,
sptl.Ten as SanPhamTinhLuongName,
xh.Ten as XepHangName
from DG_DonGia dg
left join MaThanhPhamFillet tp on dg.MaThanhPham = tp.ma
left join DG_LoaiDonGia ldg on dg.MaLoaiDonGia = ldg.Ma
left join MaSizeDinhHinh sdh on dg.MaSizeDinhHinh = sdh.Ma
left join MaSizeFillet sfl on dg.MaSizeFillet = sfl.Ma
left join DG_SanPhamTinhLuong sptl on dg.MaSanPham = sptl.Ma
left join MaXepHang xh on dg.MaXepHang = xh.Ma

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
