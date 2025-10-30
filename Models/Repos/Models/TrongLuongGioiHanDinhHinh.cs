using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TrongLuongGioiHanDinhHinh")] // This is the table name in the database
[PrimaryKey("STT", "Ngay", "MaLo")]
public partial class TrongLuongGioiHanDinhHinh
{
    [Key]
    public int STT { get; set; }

    [Key]
    public DateOnly Ngay { get; set; }

    public TimeOnly Gio { get; set; }

    [Key]
    [StringLength(50)]
    public string MaLo { get; set; } = null!;

    public double TrongLuong { get; set; }

    public double BienDoGiaoDong { get; set; }
}
