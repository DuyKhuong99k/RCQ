using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_NhanVien_D))]
    public partial class HQ_NhanVien_D
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaNhanVien { get; set; } = null!;

        [StringLength(50)]
        public string? MaChamCong { get; set; }

        [StringLength(50)]
        public string? MaHoSo { get; set; }

        [StringLength(50)]
        public string? Xuong { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? BirthDate { get; set; }

        [StringLength(50)]
        public string? DeptCode0 { get; set; }

        [StringLength(50)]
        public string? DeptName0 { get; set; }

        [StringLength(50)]
        public string? ChucVu { get; set; }

        [StringLength(50)]
        public string? GenderName { get; set; }

        [StringLength(50)]
        public string? Tel { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? JobPositionName0 { get; set; }

        [StringLength(50)]
        public string? FirstWorkingDate { get; set; }

        public bool IsShowDinhMuc { get; set; }

        public bool IsContracting { get; set; }

        public bool IsPhucVu { get; set; }

        public bool IsHuman { get; set; }

        public bool IsGiaCong { get; set; }

        public int AC { get; set; }

        public bool IsChucNang { get; set; }

        /// <summary>
        /// -1 : Trừ ra khỏi tổng lượng| 0: Không cộng không trừ chỉ ghi nhận dữ liệu|1: Cộng vào tổng sản lượng
        /// </summary>
        public int LoaiSanLuong { get; set; }

        public bool IsBanKiem { get; set; }

        public bool IsNhanVienCat { get; set; }

        public bool IsNhom { get; set; }

        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime Ngay { get; set; } = DateTime.Now;
    }
}
