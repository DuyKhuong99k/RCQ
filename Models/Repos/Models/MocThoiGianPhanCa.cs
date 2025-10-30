using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MocThoiGianPhanCa))]
[PrimaryKey("Ngay", "MaXuong")]
public partial class MocThoiGianPhanCa
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime ModifyDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifyBy { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
