using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamNguyenLieu))] // "MaThanhPhamNguyenLieu
[PrimaryKey("MaCa", "Ma")]
public partial class MaThanhPhamNguyenLieu
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCa { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public bool IsSNL { get; set; }

    public bool IsNgopGhe { get; set; }

    public bool IsNgopGheMuoi { get; set; }

    public bool IsMuoiGhePhuPham { get; set; }

    public bool IsNgopAoMuoi { get; set; }

    public bool IsNgopAoPhuPham { get; set; }

    public bool IsDatNho { get; set; }

    public bool IsPhuPhamCaTap { get; set; }

    public bool IsCaCanTin { get; set; }

    public bool IsCaNgopXeMuoi { get; set; }

    public bool IsNgopGhePhuPham { get; set; }

    public bool IsNgopXePhuPham { get; set; }

    public bool IsManh { get; set; }

    [Column(TypeName = "numeric(18, 4)")]
    public decimal TyLeNuoc { get; set; }

    public bool IsCaNgopGheTuoiBanNgoai { get; set; }

    public bool IsCaNgopGheAoBanNgoai { get; set; }
}
