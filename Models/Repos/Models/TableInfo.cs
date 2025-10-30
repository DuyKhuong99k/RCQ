using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table("TableInfo")] // This is the table name in the database
    public class TableInfo
    {
        [Key]
        [StringLength(200)]
        [Required]
        public string TableName { get; set; }
        public long? RecordCount { get; set; }
        [Column(TypeName = "datetime2(7)")]
        public DateTime CreationTime { get; set; }
        [Column(TypeName = "datetime2(7)")]
        public DateTime RecentTime { get; set; }
        public double TimeElapsed { get; set; }
        public double SizeMB { get; set; }

    }
}
