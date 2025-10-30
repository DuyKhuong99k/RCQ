using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TrongLuongBinhQuanCaChet")] // This is the table name in the database
[PrimaryKey("Ngay", "MaAo")]
public partial class TrongLuongBinhQuanCaChet
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaAo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuongBinhQuan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;
}
