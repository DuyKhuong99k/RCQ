using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamXepKhuonBlock_Color))] // "MaThanhPhamXepKhuonBlock_Color
public partial class MaThanhPhamXepKhuonBlock_Color
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPham { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNet { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaChatLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ColorCode { get; set; } = null!;

    public DateTime Ngay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
