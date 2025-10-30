using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(LoTheoLine))] // "LoTheoLine
[PrimaryKey("Id", "CodeId")]
public partial class LoTheoLine
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLine { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;
}
