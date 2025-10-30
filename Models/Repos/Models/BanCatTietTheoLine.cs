using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Table(nameof(BanCatTietTheoLine))]
[PrimaryKey("MaBanCatTiet", "MaXuong")]
public partial class BanCatTietTheoLine
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaBanCatTiet { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLine { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
