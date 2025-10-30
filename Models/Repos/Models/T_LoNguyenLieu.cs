using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_LoNguyenLieu")] // Added
public partial class T_LoNguyenLieu
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhaCungCap { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhomLo { get; set; }

    public DateTime NgayTao { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNL { get; set; }
}
