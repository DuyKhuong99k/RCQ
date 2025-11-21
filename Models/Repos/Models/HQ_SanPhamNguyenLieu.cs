using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table("HQ_SanPhamNguyenLieu")]
    public class HQ_SanPhamNguyenLieu
    {
        [Key]
        [Column("Id", TypeName = "bigint")]
        public long Id { get; set; }

        [Column("Ten", TypeName = "nvarchar(500)")]
        public string Ten { get; set; }

        [Column("SuDung", TypeName = "bit")]
        public bool SuDung { get; set; }

        [Column("MNgay", TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; }

        [Column("Min", TypeName = "decimal(18, 3)")]
        public decimal Min { get; set; }

        [Column("Max", TypeName = "decimal(18, 3)")]
        public decimal Max { get; set; }

        [Column("MaQuyCach", TypeName = "bigint")]
        public long MaQuyCach { get; set; }
    }
}
