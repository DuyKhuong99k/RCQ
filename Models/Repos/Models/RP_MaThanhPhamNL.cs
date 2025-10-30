using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_MaThanhPhamNL")] // Added
public partial class RP_MaThanhPhamNL
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamNL { get; set; } = null!;
}
