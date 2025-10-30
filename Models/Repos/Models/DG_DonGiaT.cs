using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DG_DonGiaT))] // "DG_DonGiaT
public partial class DG_DonGiaT
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongDoan { get; set; } = null!;

    public int Gia { get; set; }

    public DateTime NgayGio { get; set; }
}
