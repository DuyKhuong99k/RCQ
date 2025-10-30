using System;
using System.Collections.Generic;
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
        _default,

        /// <summary>
        /// 1 the dau vao 2 the dau ra
        /// </summary>
        _type1,

        /// <summary>
        /// 2 the vao 2 the ra
        /// </summary>
        _type2,

        /// <summary>
        /// 1 the vao 1 the ra (the nhan vien)
        /// </summary>
        _type3,

        /// <summary>
        /// 1 the ra ko dau vao
        /// </summary>
        _type4,
        /// <summary>
        /// 1 the dau vao 2 the dau ra - su dung cơ chế phân phát bàn - Đại Thành Fillet
        /// </summary>
        _type1_2,
        /// <summary>
        /// offline
        /// </summary>
        _type5,
    }
}
