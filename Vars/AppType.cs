using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars
{
    public enum AppType
    {
        /// <summary>
        /// 2 the dau vao 1 dau ra
        /// </summary>
        [Display(Name ="2 in 1 out")]
        _default,

        /// <summary>
        /// 1 the dau vao 2 the dau ra
        /// </summary>
        [Display(Name = "1 in 2 out")]
        _type1,

        /// <summary>
        /// 2 the vao 2 the ra
        /// </summary>
        [Display(Name = "2 in 2 out")]
        _type2,

        /// <summary>
        /// 1 the vao 1 the ra (the nhan vien)
        /// </summary>
        [Display(Name = "1 in 1 out (staff card)")]
        _type3,

        /// <summary>
        /// 1 the ra ko dau vao
        /// </summary>
        [Display(Name = "1 out no in")]
        _type4,
        /// <summary>
        /// 1 the dau vao 2 the dau ra - su dung cơ chế phân phát bàn - Đại Thành Fillet
        /// </summary>
        [Display(Name = "1 in 2 out - table distribution - Dai Thanh Fillet")]
        _type1_2,
        /// <summary>
        /// offline
        /// </summary>
        [Display(Name = "Offline")]
        _type5,
    }
}
