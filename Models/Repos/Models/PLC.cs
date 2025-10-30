using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Interface;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PLC")] // Added
public partial class PLC
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string IP { get; set; } = null!;

    public int Port { get; set; }
    [NotMapped]
    public ISlmpClient? Client {get; set; }
}
