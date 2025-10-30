using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaSizeChinhXepKhuon))] // "MaSizeChinhXepKhuon
public partial class MaSizeChinhXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    public int _type { get; set; }

    public int Idx { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;
}
