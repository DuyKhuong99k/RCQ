using Models.Repos.Models;

namespace Services;

/// <summary>Khóa nghiệp vụ của ca: so mã, không so tên và không bao gồm máy/lô/size.</summary>
public sealed record ScaleWorkContext(string Product, string Material, string Workshop)
{
    public static ScaleWorkContext Create(string? product, string? material, string? workshop) =>
        new(product?.Trim() ?? "", material?.Trim() ?? "", workshop?.Trim() ?? "");

    public static ScaleWorkContext? FromSnapshot(ScaleAttendanceScan? scan)
    {
        if (scan == null) return null;
        var context = Create(scan.MaThanhPham, scan.MaLoaiNguyenLieu, scan.MaXuong);
        return context.IsComplete ? context : null;
    }

    public bool IsComplete => Product.Length is > 0 and <= 50 &&
        Material.Length is > 0 and <= 50 && Workshop.Length is > 0 and <= 50;

    public bool Matches(ScaleWorkContext? other) => IsComplete && other?.IsComplete == true &&
        Same(Product, other.Product) && Same(Material, other.Material) && Same(Workshop, other.Workshop);

    private static bool Same(string left, string right) =>
        string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
}
