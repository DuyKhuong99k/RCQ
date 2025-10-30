using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_DonHang")] // Added
public partial class T_DonHang
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhachHang { get; set; }

    public bool IsKetThuc { get; set; }
}
