using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    public class VmLogGhiNhanLoiCan
{
    public int STT { get; set; }
    public DateTime Ngay { get; set; }
    public TimeSpan Gio { get; set; }
    public TimeSpan? GioBTP { get; set; }

    public string? MaUserCan { get; set; }
    public string? MaMayCan { get; set; }

    public string? MaLoaiCa { get; set; }
    public string? TenLoaiCa { get; set; }

    public string? MaMau { get; set; }
    public string? TenMau { get; set; }

    public string? MaSize { get; set; }
    public string? TenSize { get; set; }

    public string? MaThanhPham { get; set; }
    public string? TenThanhPham { get; set; }

    public string? MaLo { get; set; }
    public string? MaThe { get; set; }

    public decimal? TrongLuongNhan { get; set; }
    public decimal? TrongLuongTra { get; set; }
        public decimal? TrongLuongTare { get; set; }
        public decimal? TrongLuongBu { get; set; }

        public decimal? DinhMucThucTe { get; set; }
    public decimal? DinhMucYeuCau { get; set; }

    public string? MaXuong { get; set; }
    public string? TenXuong { get; set; }

    public string? MaNhanVien { get; set; }
    public string? MaHoSo { get; set; }
    public string? TenNhanVien { get; set; }

    public string? GhiChu { get; set; }
    public string? ThongBaoLoi { get; set; }

    public string? Id { get; set; }
        public bool ChiSanLuong { get; set; }
        public bool SuDung { get; set; }
        public bool IsOffline { get; set; }
    }
}
