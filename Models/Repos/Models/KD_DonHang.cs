using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(KD_DonHang))] // "KD_DonHang
public partial class KD_DonHang
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public DateOnly CreateDate { get; set; }

    public DateOnly Ngay { get; set; }

    public bool IsCompleted { get; set; }

    public bool SuDung { get; set; }
}
