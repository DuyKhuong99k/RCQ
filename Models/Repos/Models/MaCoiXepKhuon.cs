using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Models.Repos.A_Model;
using MvvmHelpers;

namespace Models.Repos.Models;
[Table(nameof(MaCoiXepKhuon))] // "MaCoiXepKhuon
public partial class MaCoiXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public double TrongLuongMax { get; set; }

    public bool Tam { get; set; }
    [NotMapped]
    public ObservableRangeCollection<CoiTamChiTiet> CoiTamChiTiets { get; set; } = new();
}
