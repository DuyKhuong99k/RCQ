using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaSanPhamDinhHinhBravo))] // "MaSanPhamDinhHinhBravo
public partial class MaSanPhamDinhHinhBravo
{
    [StringLength(50)]
    [Unicode(false)]
    public string ID { get; set; } = null!;

    [StringLength(500)]
    public string Name { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string DaiThanhId { get; set; } = null!;
}
