using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("Tare")] // This is the table name in the database
public partial class Tare
{
    [Key]
    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeBu { get; set; }
}
