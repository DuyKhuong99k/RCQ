using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RolePermistion")] // Added
public partial class RolePermistion
{
    [Key]
    public int Id { get; set; }

    public int RoleId { get; set; }

    [StringLength(500)]
    public string Fu { get; set; } = null!;

    [StringLength(500)]
    public string Func { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    /// <summary>
    /// -1 không sử dụng, 1 sử dụng
    /// </summary>
    public int Status { get; set; }
}
