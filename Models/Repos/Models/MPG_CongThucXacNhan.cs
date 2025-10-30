using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MPG_CongThucXacNhan))]
[PrimaryKey("MaCongThuc", "Block", "Ngay")]
public partial class MPG_CongThucXacNhan
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongThuc { get; set; } = null!;

    [Key]
    public int Block { get; set; }

    public bool IsXacNhan { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    public bool SuDung { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongNguyenLieu { get; set; }
}
