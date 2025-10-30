using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("SettingDashboard")] // Added
public partial class SettingDashboard
{
    [Key]
    public int NumberDate { get; set; }
}
