using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_LoaiNguyenLieu")] // Added
public partial class T_LoaiNguyenLieu
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    public bool SuDung { get; set; }
}
