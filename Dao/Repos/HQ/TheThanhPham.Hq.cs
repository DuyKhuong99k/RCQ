using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ;

public partial class TheThanhPham
{
    // HQ has no irradiation feature. Keep existing shared fields (including required
    // legacy defaults), but never require MaChieuXa or an irradiation lookup table.
    private const string HqInsertSql = @"
        INSERT INTO dbo.TheThanhPham
        (MaThe,MaThanhPham,MaSizeDinhHinh,TrongLuongTare,MaMayLangDa,MaThanhPhamFillet,
         MaThanhPhamPhuPham,MaSizeFillet,IsZero,MaLoaiNguyenLieu,IsRestart,MaQuyTrinhT,
         STTPhieuPhanCoChiTietT,MaCoiXepKhuon,NgayGio,MaSize,LoLevel,
         MaChatLuongChinhXepKhuon,MaChatLuongPhuXepKhuon,MaSizeChinhXepKhuon,
         MaSizePhuXepKhuon,MaThanhPhamChinhXepKhuon,MaThanhPhamPhuXepKhuon)
        VALUES
        (@MaThe,@MaThanhPham,@MaSizeDinhHinh,@TrongLuongTare,@MaMayLangDa,@MaThanhPhamFillet,
         @MaThanhPhamPhuPham,@MaSizeFillet,@IsZero,@MaLoaiNguyenLieu,@IsRestart,@MaQuyTrinhT,
         @STTPhieuPhanCoChiTietT,@MaCoiXepKhuon,@NgayGio,@MaSize,@LoLevel,
         @MaChatLuongChinhXepKhuon,@MaChatLuongPhuXepKhuon,@MaSizeChinhXepKhuon,
         @MaSizePhuXepKhuon,@MaThanhPhamChinhXepKhuon,@MaThanhPhamPhuXepKhuon)";

    private const string HqUpdateSql = @"
        UPDATE dbo.TheThanhPham SET
            MaThanhPham=@MaThanhPham,MaSizeDinhHinh=@MaSizeDinhHinh,TrongLuongTare=@TrongLuongTare,
            MaMayLangDa=@MaMayLangDa,MaThanhPhamFillet=@MaThanhPhamFillet,
            MaThanhPhamPhuPham=@MaThanhPhamPhuPham,MaSizeFillet=@MaSizeFillet,IsZero=@IsZero,
            MaLoaiNguyenLieu=@MaLoaiNguyenLieu,IsRestart=@IsRestart,MaQuyTrinhT=@MaQuyTrinhT,
            STTPhieuPhanCoChiTietT=@STTPhieuPhanCoChiTietT,MaCoiXepKhuon=@MaCoiXepKhuon,
            NgayGio=@NgayGio,MaSize=@MaSize,LoLevel=@LoLevel,
            MaChatLuongChinhXepKhuon=@MaChatLuongChinhXepKhuon,
            MaChatLuongPhuXepKhuon=@MaChatLuongPhuXepKhuon,MaSizeChinhXepKhuon=@MaSizeChinhXepKhuon,
            MaSizePhuXepKhuon=@MaSizePhuXepKhuon,MaThanhPhamChinhXepKhuon=@MaThanhPhamChinhXepKhuon,
            MaThanhPhamPhuXepKhuon=@MaThanhPhamPhuXepKhuon
        WHERE MaThe=@MaThe";

    public int InsertHq<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection.Execute(HqInsertSql, item);
    }

    public int UpdateHq<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection.Execute(HqUpdateSql, item);
    }

    public int UpdateHq<T>(List<T> items)
    {
        if (items.Count == 0) return 0;
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var affected = connection.Execute(HqUpdateSql, items, transaction);
        transaction.Commit();
        return affected;
    }
}
