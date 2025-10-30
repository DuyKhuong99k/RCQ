using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TrangThaiNhanVienTamThoi")] // This is the table name in the database
[PrimaryKey("STT", "Ngay", "MaXuong", "KhuVuc")]
public partial class TrangThaiNhanVienTamThoi
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }
    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public bool IsPhucVu { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string KhuVuc { get; set; } = null!;
}
