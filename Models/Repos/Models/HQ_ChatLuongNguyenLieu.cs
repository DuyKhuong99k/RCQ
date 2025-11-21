using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    public class HQ_ChatLuongNguyenLieu
    {
        [Key]
        public long Id { get; set; }
        public string Ten { get; set; } = null!;
        public bool SuDung { get; set; }
    }
}
