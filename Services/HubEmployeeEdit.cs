using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using Models.Repos.Models;

namespace Services;

public sealed class HubEmployeeEdit
{
    [Required(ErrorMessage = "Nhập mã nhân viên."), StringLength(50, ErrorMessage = "Mã nhân viên tối đa 50 ký tự.")]
    public string MaNhanVien { get; set; } = "";
    [Required(ErrorMessage = "Nhập mã hồ sơ."), StringLength(50, ErrorMessage = "Mã hồ sơ tối đa 50 ký tự.")]
    public string MaHoSo { get; set; } = "";
    [Required(ErrorMessage = "Nhập tên nhân viên."), StringLength(100, ErrorMessage = "Tên nhân viên tối đa 100 ký tự.")]
    public string Name { get; set; } = "";
    [StringLength(50, ErrorMessage = "Mã chấm công tối đa 50 ký tự.")] public string? MaChamCong { get; set; }
    [StringLength(50, ErrorMessage = "Tên nhóm tối đa 50 ký tự.")] public string? DeptName0 { get; set; }
    [Range(-1, 1, ErrorMessage = "Loại sản lượng chỉ nhận -1, 0 hoặc 1.")]
    public int LoaiSanLuong { get; set; } = 1;
    public bool IsContracting { get; set; } = true;
    public bool IsHuman { get; set; } = true;
    public bool IsPhucVu { get; set; }
    public bool IsGiaCong { get; set; }
    public bool IsBanKiem { get; set; }
    public bool IsChucNang { get; set; }
    public bool IsNhom { get; set; }
    public bool IsNhanVienCat { get; set; }
    public bool IsShowDinhMuc { get; set; } = true;
    public DateTime ModifiedAt { get; set; }

    public HubEmployeeEdit Copy() => (HubEmployeeEdit)MemberwiseClone();

    // The same mapping serves SQL projection and form copies; do not load unused profile metadata.
    public static Expression<Func<NhanVienDaiThanh, HubEmployeeEdit>> Projection { get; } = employee => new()
    {
        MaNhanVien = employee.MaNhanVien, MaHoSo = employee.MaHoSo ?? "", Name = employee.Name ?? "",
        MaChamCong = employee.MaChamCong, DeptName0 = employee.DeptName0, LoaiSanLuong = employee.LoaiSanLuong,
        IsContracting = employee.IsContracting, IsHuman = employee.IsHuman, IsPhucVu = employee.IsPhucVu,
        IsGiaCong = employee.IsGiaCong, IsBanKiem = employee.IsBanKiem, IsChucNang = employee.IsChucNang,
        IsNhom = employee.IsNhom, IsNhanVienCat = employee.IsNhanVienCat, IsShowDinhMuc = employee.IsShowDinhMuc,
        ModifiedAt = employee.MNgay
    };

    private static readonly Func<NhanVienDaiThanh, HubEmployeeEdit> Map = Projection.Compile();

    public static HubEmployeeEdit From(NhanVienDaiThanh employee) => Map(employee);

    public static string SuggestCode(IEnumerable<string> codes)
    {
        var maximum = BigInteger.Zero;
        foreach (var code in codes)
            if (!string.IsNullOrEmpty(code) && code.All(char.IsAsciiDigit) &&
                BigInteger.TryParse(code, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > maximum)
                maximum = number;
        var suggested = (maximum + BigInteger.One).ToString("D6", CultureInfo.InvariantCulture);
        return suggested.Length <= 50 ? suggested : "";
    }
}

public sealed record HubEmployeeList(string WorkshopCode, string WorkshopName, string SuggestedCode,
    IReadOnlyList<HubEmployeeEdit> Employees);

public sealed record HubEmployeeResult(bool Success, string Message, bool CatalogRefreshFailed = false);
