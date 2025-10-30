using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("Thit_LoaiNguyenLieuPhaLoc")] // This is the table name in the database
public partial class Thit_LoaiNguyenLieuPhaLoc
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool IsMang { get; set; }

    public string? GhiChu { get; set; }

    public bool SuDung { get; set; }
}
