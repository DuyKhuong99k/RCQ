using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    public class HQ_PhieuCanNhapNguyenLieu
    {
        [Key]
        public string Id { get; set; }
        public int STT { get; set; }
        public long IdPhieuCanNguyenLieu { get; set; }
        public string? SoPhieuCanNhap { get; set; } = null!;
        public int SoLanSua { get; set; }
        public long MaKho { get; set; }
        public string? TaiXe { get; set; } = null!;
        public string? CCCD { get; set; } = null!;
        public string? SDT { get; set; } = null!;
        public string? NoiDungGiaoNhan { get; set; } = null!;
        public string MaPhuongTien { get; set; } = null!;
        public long MaSanPham { get; set; }
        public decimal TrongLuongTong { get; set; }
        public decimal TrongLuongXe { get; set; }
        public decimal TrongLuongHang { get; set; }
        public long MaDonVi { get; set; }
        public long? MaChatLuong { get; set; }
        public int? CanHang { get; set; }
        public bool? TruBi { get; set; }
    }
}
