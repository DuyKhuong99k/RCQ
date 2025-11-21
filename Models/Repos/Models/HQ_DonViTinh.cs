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
    [Table("HQ_DonViTinh")]
    public class HQ_DonViTinh
    {
        [Key]
        [Column("Id",TypeName = "bigint")]
        public long Id { get; set; }

        [Column("Ten", TypeName = "nvarchar(50)")]
        public string Ten { get; set; }

        [Column("SuDung", TypeName = "bit")]
        public bool SuDung { get; set; }
    }
}
