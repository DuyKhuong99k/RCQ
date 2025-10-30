using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DinhMucXepHang))] // "DinhMucXepHang
[PrimaryKey("MaLo", "MaSanPham", "MaXepHang", "Year")]
public partial class DinhMucXepHang
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXepHang { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucUp { get; set; }

    [Key]
    public int Year { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucDown { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMuc { get; set; }
}
