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
    [NotMapped]
    public decimal TrongLuongHienTai { get; set; }

    [NotMapped] public int SoRo { get; set; } = 0;
    [NotMapped]
    public DateTime? ThoiGianPhieuGanNhatRa { get; set; }
    [NotMapped]
    public decimal TrongLuongHienTaiRa { get; set; }

    [NotMapped] public int SoRoRa { get; set; } = 0;
    [NotMapped]
    public DateTime? ThoiGianPhieuGanNhat { get; set; }
    /// <summary>
    /// Thời gian giữa 2 lần vào coi - Phut
    /// </summary>
    [NotMapped] public double ThoiGianGiua2LanVaoCoi { get; set; } = 30; 
 
    public bool Tam { get; set; }
    [NotMapped]
    public ObservableRangeCollection<CoiTamChiTiet> CoiTamChiTiets { get; set; } = new();
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;

    [NotMapped] public string MaXuong { get; set; } = "1";

}
