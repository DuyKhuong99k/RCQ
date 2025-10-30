using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_TheThanhPham_U))]
    public partial class HQ_TheThanhPham_U
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaThe { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? MaThanhPham { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaSizeDinhHinh { get; set; }

        [Column(TypeName = "decimal(18, 3)")]
        public decimal TrongLuongTare { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaMayLangDa { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaThanhPhamFillet { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaThanhPhamPhuPham { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaSizeFillet { get; set; }

        public bool? IsZero { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaLoaiNguyenLieu { get; set; }

        public bool? IsRestart { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaQuyTrinhT { get; set; }

        public int STTPhieuPhanCoChiTietT { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaCoiXepKhuon { get; set; }
        [NotMapped]
        public string? ThanhPhamDHName { get; set; }
        [NotMapped]
        public string? SizeDHName { get; set; }
        [NotMapped]
        public string? ThanhPhamFilletName { get; set; }
        [NotMapped]
        public string? SizeFilletName { get; set; }
        [NotMapped]
        public string? ThanhPhamPhuPhamName { get; set; }
        [NotMapped]
        public string? QuyTrinhName { get; set; }
        [NotMapped]
        public string? CoiXepKhuonName { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime Ngay { get; set; } = DateTime.Now;
        [StringLength(50)]
        [Unicode(false)]
        public string? MaSize { get; set; }

    }
}
