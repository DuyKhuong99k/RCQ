using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_QuyTrinhTheoNgay")] // This is the table name in the database
public partial class T_QuyTrinhTheoNgay
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaQuyTrinh { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TheId { get; set; }
}
