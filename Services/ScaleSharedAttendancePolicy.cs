using System.Globalization;
using Models.Repos.Models;

namespace Services;

/// <summary>Quy tắc ca chung theo thành phẩm, dùng cả online và dữ liệu đồng bộ.</summary>
public static class ScaleSharedAttendancePolicy
{
    public static bool SameProduct(string? left, string? right) =>
        !string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right) &&
        string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);

    public static bool CanUseMachine(CheckInOut shift, IEnumerable<ScaleAttendanceScan> scans,
        int machine, DateTime time, string? productCode) =>
        shift.MaSoMay == machine || scans.Any(scan => scan.VaoId == shift.Id &&
            scan.MaSoMay == machine && scan.TrangThai == "Vào" && scan.ThoiGian <= time &&
            SameProduct(scan.MaThanhPham, productCode));

    // Máy gốc giữ cách ghi sản lượng cũ. Máy tham gia chỉ được cộng phiếu
    // từ sau lần Vào được chấp nhận, đúng mã thành phẩm đã lưu của lượt đó.
    public static IQueryable<HQ_PhieuCan> ShiftWeights(IQueryable<HQ_PhieuCan> weights,
        IQueryable<ScaleAttendanceScan> scans, CheckInOut shift, DateTime end)
    {
        var device = shift.MaSoMay.ToString(CultureInfo.InvariantCulture);
        return weights.Where(weight => weight.TheIdNhanVien == shift.MaChamCong &&
            weight.NgayGio >= shift.ThoiGian && weight.NgayGio <= end &&
            weight.ChiSanLuong && weight.Status == 1 &&
            (weight.MayCan == device || scans.Any(scan => scan.VaoId == shift.Id &&
                scan.MaSoMay.ToString() == weight.MayCan && scan.TrangThai == "Vào" &&
                scan.ThoiGian <= weight.NgayGio && scan.MaThanhPham == weight.MaThanhPham)));
    }

    public static string? ValidateContinuation(CheckInOut shift, string? shiftProduct,
        string? selectedProduct, DateTime time, int maxShiftHours)
    {
        if (maxShiftHours is < 1 or > 24) return "Cấu hình ca không hợp lệ.";
        if (time <= shift.ThoiGian) return "Giờ chuyển phải sau giờ Vào. Kiểm tra giờ máy.";
        if (ScaleAttendanceTransferService.IsExpired(shift, time, maxShiftHours))
            return "Ca đã hết hạn. Cần kiểm tra giờ Ra thực tế.";
        if (shift.Isforget != 0) return "Ca đang lỗi. Nhờ quản lý kiểm tra.";
        if (string.IsNullOrWhiteSpace(shiftProduct))
            return "Chưa xác định thành phẩm của ca cũ. Nhờ quản lý kiểm tra.";
        if (string.IsNullOrWhiteSpace(selectedProduct)) return "Chưa chọn thành phẩm tại cân mới.";
        return null;
    }
}
