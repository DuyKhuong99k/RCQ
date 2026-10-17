using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;

namespace Services;

public static class HqWeightDuplicatePolicy
{
    public const string ErrorMessage = "Phiếu cân HQ trùng dữ liệu trong vòng 30 giây. Hai phiếu giống nhau phải cách nhau ít nhất 30 giây.";

    public static IQueryable<HQ_PhieuCan> FindDuplicates(IQueryable<HQ_PhieuCan> weights, HQ_PhieuCan item)
    {
        var from = item.NgayGio.AddSeconds(-30);
        var to = item.NgayGio.AddSeconds(30);
        // Ghi chú backup do HUB sinh chỉ chứa nguồn và thời gian khôi phục.
        var note = item.GhiChu ?? "";
        var systemNote = note == "" || note.StartsWith("[HQ_BACKUP] ", StringComparison.Ordinal);
        return weights.Where(x => x.Id != item.Id
            && x.NgayGio > from && x.NgayGio < to
            && x.STT == item.STT && x.MayCan == item.MayCan
            && x.MaLo == item.MaLo && x.MaSize == item.MaSize
            && x.MaThanhPham == item.MaThanhPham && x.MaLoaiNguyenLieu == item.MaLoaiNguyenLieu
            && (x.MaNhanVien ?? "") == (item.MaNhanVien ?? "")
            && (x.MaNhanVienPhucVu ?? "") == (item.MaNhanVienPhucVu ?? "")
            && (x.MaNhanVienBanKiem ?? "") == (item.MaNhanVienBanKiem ?? "")
            && x.TrongLuong == item.TrongLuong && x.TrongLuongTare == item.TrongLuongTare
            && x.ChiSanLuong == item.ChiSanLuong && x.Status == item.Status
            && x.TheId == item.TheId && x.TheIdNhanVien == item.TheIdNhanVien
            && x.MaXuong == item.MaXuong
            && ((x.GhiChu ?? "") == note
                || (systemNote && (x.GhiChu == null || x.GhiChu == "" || x.GhiChu.StartsWith("[HQ_BACKUP] ")))));
    }

    // Gọi trong transaction đã giữ LockCard đến khi INSERT hoàn tất.
    public static bool HasDuplicate(dbPMScontext db, HQ_PhieuCan item) =>
        FindDuplicates(db.HqPhieuCans.Local.AsQueryable(), item).Any()
        || FindDuplicates(db.HqPhieuCans.AsNoTracking(), item).Any();
}
