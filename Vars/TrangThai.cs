using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars
{
    public enum TrangThai
    {
        /// <summary>
        /// Khởi Tạo
        /// </summary>
        [Description("Khởi Tạo")] KT,

        /// <summary>
        /// Hoàn Thành
        /// </summary>
        [Description("Hoàn Thành")] HT,

        /// <summary>
        /// Khóa
        /// </summary>
        [Description("Khóa")] KHOA,
    }
}
