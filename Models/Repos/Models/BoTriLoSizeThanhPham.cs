using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BoTriLoSizeThanhPham))]
[PrimaryKey("Id", "CodeId")]
public partial class BoTriLoSizeThanhPham
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaViTri { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    public DateOnly Ngay { get; set; }

    public TimeOnly Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizePhu { get; set; }
}
