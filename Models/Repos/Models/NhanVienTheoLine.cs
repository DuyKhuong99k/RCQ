using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienTheoLine")] // "NhanVienTheoLine
[PrimaryKey("Id","Ngay", "CodeId")]
public partial class NhanVienTheoLine
{
    [Key]
    public int Id { get; set; }
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLine { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaViTri { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;
}
