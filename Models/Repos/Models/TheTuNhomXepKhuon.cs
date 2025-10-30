using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TheTuNhomXepKhuon")] // This is the table name in the database
public partial class TheTuNhomXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTheTu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhom { get; set; } = null!;
}
