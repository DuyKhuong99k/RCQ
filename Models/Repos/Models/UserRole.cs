using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("UserRole")] // This is the table name in the database
public partial class UserRole
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRole")]
    public virtual Role? Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserRole")]
    public virtual NguoiDung? User { get; set; }
}
