using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;

namespace Models.Repos;

public partial class dbPMScontext
{
    public virtual DbSet<ScaleAttendanceScan> ScaleAttendanceScans { get; set; }
}
