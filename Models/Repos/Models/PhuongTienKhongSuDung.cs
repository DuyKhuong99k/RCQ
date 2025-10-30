using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhuongTienKhongSuDung")] // Added
public partial class PhuongTienKhongSuDung
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;
}
