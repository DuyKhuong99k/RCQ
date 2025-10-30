using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BT_MaThanhPham))] // "BT_MaThanhPham
public partial class BT_MaThanhPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public int X { get; set; }

    public int Y { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }
}
