using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienPhucVuTheoBan")] // "NhanVienPhucVuTheoBan
[PrimaryKey("MaNhanVien", "Ngay", "MaBan", "MaXuong", "KhuVuc")]
public partial class NhanVienPhucVuTheoBan
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    public DateOnly Ngay { get; set; }

    public TimeOnly Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaBan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    /// <summary>
    /// FL or SC
    /// </summary>
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string KhuVuc { get; set; } = null!;
}
