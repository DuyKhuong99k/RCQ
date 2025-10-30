using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using Vars;

namespace Models.Repos.Models;
[Table(nameof(MayCan))] // "MayCan
public partial class MayCan
{
    /// <summary>
        /// Id(tên) máy Cân
        /// </summary>
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Id { get; set; }
        [StringLength(500)]
        public string DisplayName { get; set; }
        public string SCode { get; set; }
        public string Par1 { get; set; }
        public string Par2 { get; set; }
        public string Par3 { get; set; }
        public string Par4 { get; set; }
        public int Idx { get; set; } = 0;
        public bool IsSuDungMauThanhPham { get; set; } = false;
        [StringLength(50)]
        [Unicode(false)]
        public string MaXuong { get; set; }
        /// <summary>
        /// "DESKTOP": Máy cân chạy nền window
        /// "BOARD": Máy cân chạy trên board
        /// "NONE": Chưa xác định
        /// </summary>
        [StringLength(100)]
        [Unicode(false)]
        public string MType { get; set; } = "NONE";
        public AppKV WKv { get; set; } = 0;
        public AppType AType { get; set; } = AppType._default;
        [NotMapped]
        [StringLength(100)]
        [Unicode(false)]
        public string? ConnectionId { get; set; }
        [NotMapped]
        public bool IsConnected { get; set; } = false;

       
        [StringLength(200)]
        public string? IPAddr { get; set; }

      
        [NotMapped]
        public DateTime DateTimeConnected { get; set; } = DateTime.Now;
        [NotMapped]
        public bool IsActive { get; set; } = false;
        [NotMapped]
        public string SubTitle => $@"{DisplayName} - {MType} - {MaXuong}";
        [NotMapped]
        public bool IsExpand { get; set; } = false;
        
        [NotMapped]
        public string MaLo { get; set; }
        [NotMapped]
        public string MaThanhPham { get; set; }
        [NotMapped]
        public string MaSize { get; set; }
        [NotMapped]
        public decimal? TrongLuongTare { get; set; } = 0;
        [NotMapped]
        public decimal TrongLuong { get; set; } = 0;
        [NotMapped]
        public bool IsStated { get; set; } = false;

        [NotMapped] public decimal TrongLuongNhan { get; set; } = 0;
        [NotMapped]
        public string? TheRo { get; set; }
        [NotMapped]
        public DateTime? TheRoAddDateTime { get; set; }
        [NotMapped]
        public DateTime? TheNhanVienAddDateTime { get; set; }
        [NotMapped]
        public DateTime? TheNhanVienPhucVuAddDateTime { get; set; }
        [NotMapped]
        public string? TheNhanVien { get; set; }
        [NotMapped]
        public string? MaNhanVien {get; set; }
        [NotMapped]
        public string? MaHoSo {get; set; }
        [NotMapped]
        public string? MaNhanVienPhucVu {get; set; }
        [NotMapped]
        public string? The { get; set; }
        [NotMapped]
        public DateTime? TheAddDateTime { get; set; }
        [NotMapped]
        public string? TheView { get; set; }

        [NotMapped] 
        public int TongSoRo { get; set; } = 0;
        [NotMapped]
        public decimal TongTrongLuong { get; set; } = decimal.Zero;

        [NotMapped] public ObservableRangeCollection<object> Items { get; set; } = new ObservableRangeCollection<object>();
        [NotMapped] public int? STTBTP { get; set; }
        [NotMapped] public string? MayCanIdBTP { get; set; }
        [NotMapped] public string? BanId { get; set; }
        [NotMapped] public string? ThePhieuSanLuongId { get; set; }
        [NotMapped] public string? IdIn { get; set; }
        [NotMapped] public string? ThongBao { get; set; }
        [NotMapped] public string? ColorString { get; set; }
        [NotMapped] public string? MaLoaiCa { get; set; }
        [NotMapped] public string? MaMau { get; set; }
        [NotMapped] public bool CaTra { get; set; }= false;
        [NotMapped] public bool IsNapThe { get; set; } = false;
        [NotMapped] public string NapTheId { get; set; } = "";
        [NotMapped] public string? MaChieuXa { get; set; }
        [NotMapped] public string? MaChatLuong { get; set; }
        [NotMapped] public string? IdMonitor { get; set; }
        [NotMapped] public string? MaCoi { get; set; }

        [NotMapped] public string? MaCoiTam { get; set; }
        [NotMapped] public DateTime NgayNguyenLieu {get; set; }
        [NotMapped] public string? NhanVienName {get; set; }


        [NotMapped] public string? MaAo { get; set; }
        [NotMapped] public string? MaNhaCungCap { get; set; }
        [NotMapped] public string? MaPhuongTien { get; set; }
        [NotMapped] public int? ChuyenNL { get; set; } = 0;
        [NotMapped] public string? Pheu { get; set; }
        [NotMapped] public decimal TyLeNuoc { get; set; } = 0;
        [NotMapped] public string? MaKhachHang { get; set; }

}
