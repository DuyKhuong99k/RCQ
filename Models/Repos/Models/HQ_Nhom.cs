using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_Nhom))]
    public class HQ_Nhom
    {
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Id { get; set; }
        [StringLength(200)]
        public string Ten { get; set; }
        public bool SuDung { get; set; } = true;
        public string GhiChu { get; set; }
    }
}
