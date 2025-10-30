using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class DinhMucFillet
    {
        private readonly string connectionString;
        private string tableName = @"DinhMucFillet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[DinhMucFillet]
      WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [Gio] = @Gio 
      and [MaLo] = @MaLo 
      and [MaLoaiCa] = @MaLoaiCa 
      and [MaMau] = @MaMau 
      and [MaSize] = @MaSize 
      and [MaThanhPham] = @MaThanhPham 
      and [MaXuong] = @MaXuong 
      and [CaTra] = @CaTra";

        private readonly string qrInsert = @"INSERT INTO [dbo].[DinhMucFillet]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaLo]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaXuong]
           ,[CaTra]
           ,[DinhMuc]
           ,[SuDung])
     VALUES
           (@STT 
           ,@Ngay 
           ,@Gio 
           ,@MaLo 
           ,@MaLoaiCa 
           ,@MaMau 
           ,@MaSize 
           ,@MaThanhPham 
           ,@MaXuong 
           ,@CaTra 
           ,@DinhMuc 
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[DinhMucFillet]
   SET  
      [DinhMuc] = @DinhMuc 
      ,[SuDung] = @SuDung 
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [Gio] = @Gio 
      and [MaLo] = @MaLo 
      and [MaLoaiCa] = @MaLoaiCa 
      and [MaMau] = @MaMau 
      and [MaSize] = @MaSize 
      and [MaThanhPham] = @MaThanhPham 
      and [MaXuong] = @MaXuong 
      and [CaTra] = @CaTra";
        private readonly string qrLast = @"Select
    *
from
    (
        Select
            dm.STT,
            dm.Ngay,
            dm.Gio,
            dm.MaLo,
            dm.MaLoaiCa,
            dm.MaMau,
            dm.MaSize,
            dm.MaThanhPham,
            dm.MaXuong,
            dm.CaTra,
            dm.DinhMuc,
            dm.SuDung,
            ROW_NUMBER() OVER (
                PARTITION BY MaLo,
                MaLoaiCa,
                MaMau,
                MaSize,
                MaThanhPham,
                CaTra,
                Ngay
                ORDER BY
                    Gio DESC
            ) AS [ROW NUMBER]
        from
            DinhMucFillet dm
        where
            Ngay = @ngay
            And SuDung = 1
    ) dm where dm.[ROW NUMBER] =1
order by
    dm.STT";
        private readonly string qrGetAll = "Select * from DinhMucFillet";

        public DinhMucFillet(string? _connectionString = null)
        {
            connectionString = _connectionString??AppViewModels.Base.Ins.ConnectionString;

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
        public List<T> GetsLast<T>(DateTime dateTime)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrLast, new {ngay = dateTime.Date}).ToList();
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
                var query = @"SELECT 
    dm.STT,
    dm.Ngay,
    dm.Gio,
    dm.MaLo,
    dm.MaLoaiCa,
    lc.Ten AS LoaiCaName,
    dm.MaMau,
    m.Ten AS MauName,
    dm.MaSize,
    s.Ten AS SizeName,
    dm.MaThanhPham,
    tp.Ten AS ThanhPhamName,
    dm.MaXuong,
    x.Ten AS XuongName,
    dm.CaTra,
    dm.DinhMuc,
    dm.SuDung
FROM 
    DinhMucFillet dm
LEFT JOIN 
    MaLoaiCaFillet lc ON lc.Ma = dm.MaLoaiCa
LEFT JOIN 
    MaMauFillet m ON m.Ma = dm.MaMau
LEFT JOIN 
    MaSizeFillet s ON s.Ma = dm.MaSize
LEFT JOIN 
    MaThanhPhamFillet tp ON tp.Ma = dm.MaThanhPham
LEFT JOIN 
    XiNghiep x ON x.Ma = dm.MaXuong
ORDER BY 
    dm.Ngay DESC, dm.Gio DESC

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
