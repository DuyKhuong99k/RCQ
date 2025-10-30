using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamDinhHinh))] // "MaThanhPhamDinhHinh
[PrimaryKey("MaCa", "Ma")]
public partial class MaThanhPhamDinhHinh
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

    public double DinhMuc { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeDinhMucDau { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeDinhMucRot { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }

    public bool IsDauVaoBatBuoc { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucKhongDauVao { get; set; }

    public bool IsDisplay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucCaTra { get; set; }

    public bool BaoCaoDauRot { get; set; }

    public bool IsSuDungThoiGianGiuaLoaiThanhPham { get; set; }

    public double? MinOut { get; set; }

    public double? MaxOut { get; set; }
}
