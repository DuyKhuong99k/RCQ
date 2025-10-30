using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("UserArea")] // This is the table name in the database
public partial class UserArea
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int WKv { get; set; }

    public DateTime NgayGio { get; set; }
}
