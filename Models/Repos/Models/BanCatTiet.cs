using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BanCatTiet))]
public partial class BanCatTiet
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    public int X1 { get; set; }

    public int Y1 { get; set; }

    public int X2 { get; set; }

    public int Y2 { get; set; }

    public int ColSpanX1 { get; set; }

    public int ColSpanX2 { get; set; }

    public bool IsShowX1 { get; set; }

    public bool IsShowX2 { get; set; }

    public bool IsDetect { get; set; }

    public bool IsFalse { get; set; }

    public int DoUuTienX1 { get; set; }

    public int DoUuTienX2 { get; set; }

    public int SoLanChiaCaX1 { get; set; }

    public int SoLanChiaCaX2 { get; set; }

    public bool IsKhoaX1 { get; set; }

    public bool IsKhoaX2 { get; set; }

    public int ThoiGianQuangDuongPheu1X1 { get; set; }

    public int ThoiGianQuangDuongPheu1X2 { get; set; }

    public int ThoiGianQuangDuongPheu2X1 { get; set; }

    public int ThoiGianQuangDuongPheu2X2 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PlcAdr { get; set; } = null!;

    [Column(TypeName = "time(7)")]
    public TimeSpan TimeOpen { get; set; }

    public bool PlcValue { get; set; }

    public int VongChiaCa { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PlcYadr { get; set; } = null!;

    public bool PlcYValue { get; set; }

    public int ViTri_HX1 { get; set; }

    public int ViTri_HX2 { get; set; }

    public int ThoiGianNhanCaX1 { get; set; }

    public int ThoiGianNhanCaX2 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PlcOffAdr { get; set; } = null!;

    public bool PlcOffValue { get; set; }

    public bool IsCaMuoiX1 { get; set; }

    public bool IsCaMuoiX2 { get; set; }
}
