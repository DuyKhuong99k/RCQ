using System.Globalization;
using Models.Repos.Models;

namespace Services;

/// <summary>Quy tắc ca chung theo thành phẩm, dùng cả online và dữ liệu đồng bộ.</summary>
public static class ScaleSharedAttendancePolicy
{
    public enum CheckInAction { Repeat, Continue, Switch }

    public static CheckInAction SelectCheckInAction(CheckInOut shift, int machine,
        string? shiftProduct, string? selectedProduct)
    {
        if (shift.MaSoMay == machine && (string.IsNullOrWhiteSpace(shiftProduct) ||
            string.IsNullOrWhiteSpace(selectedProduct) || SameProduct(shiftProduct, selectedProduct)))
            return CheckInAction.Repeat;
        return SameProduct(shiftProduct, selectedProduct) ? CheckInAction.Continue : CheckInAction.Switch;
    }

    public static bool SameProduct(string? left, string? right) =>
        !string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right) &&
        string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);

    public static bool CanUseMachine(CheckInOut shift, IEnumerable<ScaleAttendanceScan> scans,
        int machine, DateTime time, string? productCode)
    {
        if (time < shift.ThoiGian) return false;
        if (shift.MaSoMay == machine)
        {
            var initial = scans.FirstOrDefault(x => x.Id == shift.Id);
            return time != shift.ThoiGian || string.IsNullOrWhiteSpace(initial?.MaThanhPham) ||
                SameProduct(initial.MaThanhPham, productCode);
        }
        return scans.Any(scan => scan.VaoId == shift.Id && scan.MaSoMay == machine &&
            scan.TrangThai == "Vào" && scan.ThoiGian <= time && SameProduct(scan.MaThanhPham, productCode));
    }

    // Máy gốc giữ cách ghi sản lượng cũ. Máy tham gia chỉ được cộng phiếu
    // từ sau lần Vào được chấp nhận, đúng mã thành phẩm đã lưu của lượt đó.
    public static IQueryable<HQ_PhieuCan> ShiftWeights(IQueryable<HQ_PhieuCan> weights,
        IQueryable<ScaleAttendanceScan> scans, CheckInOut shift, DateTime end,
        IQueryable<CheckInOut>? events = null)
    {
        var device = shift.MaSoMay.ToString(CultureInfo.InvariantCulture);
        if (events != null)
            weights = weights.Where(weight => !events.Any(checkout => checkout.VaoId == shift.Id &&
                checkout.TrangThai == "Ra" && checkout.ThoiGian <= weight.NgayGio &&
                events.Any(next => next.MaChamCong == shift.MaChamCong && next.TrangThai == "Vào" &&
                    next.Id != shift.Id && next.ThoiGian == checkout.ThoiGian &&
                    next.MaSoMay.ToString() == weight.MayCan &&
                    (!scans.Any(initial => initial.Id == next.Id && initial.MaThanhPham != null && initial.MaThanhPham != "") ||
                     scans.Any(initial => initial.Id == next.Id && initial.MaThanhPham == weight.MaThanhPham)))));
        return weights.Where(weight => weight.TheIdNhanVien == shift.MaChamCong &&
            weight.NgayGio >= shift.ThoiGian && weight.NgayGio <= end &&
            weight.ChiSanLuong && weight.Status == 1 &&
            (weight.NgayGio != shift.ThoiGian ||
             !scans.Any(initial => initial.Id == shift.Id && initial.MaThanhPham != null && initial.MaThanhPham != "") ||
             scans.Any(initial => initial.Id == shift.Id && initial.MaThanhPham == weight.MaThanhPham)) &&
            (weight.MayCan == device || scans.Any(scan => scan.VaoId == shift.Id &&
                scan.MaSoMay.ToString() == weight.MayCan && scan.TrangThai == "Vào" &&
                scan.ThoiGian <= weight.NgayGio && scan.MaThanhPham == weight.MaThanhPham)));
    }

    public static IQueryable<CheckInOut> ShiftsAtTime(IQueryable<CheckInOut> events, string card, DateTime time) =>
        events.Where(shift => shift.MaChamCong == card && shift.TrangThai == "Vào" && shift.ThoiGian <= time &&
            !events.Any(next => next.MaChamCong == card && next.ThoiGian <= time &&
                ((next.TrangThai == "Ra" && next.VaoId == shift.Id) ||
                 (next.MaSoMay == shift.MaSoMay && next.ThoiGian > shift.ThoiGian &&
                  (next.TrangThai == "Vào" || (next.TrangThai == "Ra" && next.VaoId == null))))));

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
