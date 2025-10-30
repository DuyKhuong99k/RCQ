using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamSoCheDinhHinh))] // "MaThanhPhamSoCheDinhHinh
public partial class MaThanhPhamSoCheDinhHinh
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public int X { get; set; }

    public int Y { get; set; }

    public double Min { get; set; }

    public double Max { get; set; }

    public bool TinhKiem { get; set; }

    public bool TinhPhucVu { get; set; }

    public bool CaMuoi { get; set; }

    public bool NguyenLieu { get; set; }

    public bool Ban09 { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }

    public bool LoaiGui { get; set; }

    public bool TruocLangDa { get; set; }

    public bool SauLangDa { get; set; }

    public bool Nhan { get; set; }

    public bool TinhGio { get; set; }

    public bool BatCO { get; set; }

    public bool IsNhapTay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Createdby { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Modifiedby { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDateTime { get; set; }
}
