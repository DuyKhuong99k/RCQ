using Attrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars
{
    public enum TrangThaiLamViec
    {
        [EnumInformation("Nghĩ Việc", "-")] NghiViec,
        [EnumInformation("Đang Làm", "HD")] DangLam,
        [EnumInformation("", "")] KhongXacDinh
    }
}
