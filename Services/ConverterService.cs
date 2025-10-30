using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

public class ConverterService(IMainService VmMain) : IConverterService
{
    #region IConverterService Members

    public string XuongToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmXiNghiep.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string NhanVienToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmNhanVien.Items.FirstOrDefault(x => x.MaNhanVien == id);
            if (item != null) name = item.Name ?? "";
        }

        return name;
    }

    public string NhanVienToMaHoSo(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmNhanVien.Items.FirstOrDefault(x => x.MaNhanVien == id);
            if (item != null) name = item.MaHoSo ?? "";
        }

        return name;
    }

    public string ChieuXaToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmChieuXaXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string CoiToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmCoi.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string AoToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmAoNguyenLieu.ItemsAo.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string PhuongTienToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmPhuongTienNguyenLieu.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string KhachHangXepKhuonToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmKhachHangXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string ChatLuongXepKhuonToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmChatLuong.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string NetXepKhuonToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmNETXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string ThanhPhamToName(string? id, AppKV kv)
    {
        var name = "";
        if (id != null)
            try
            {
                switch (kv)
                {
                    case AppKV.Main:
                        break;
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                    {
                        var item = VmMain.VmThanhPhamNguyenLieu.Items.FirstOrDefault(x => x.Ma == id);

                        if (item != null) name = item.Ten;
                        break;
                    }

                    case AppKV.BTPFillet:
                    {
                        var item = VmMain.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.TPFillet:
                    {
                        var item = VmMain.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BTPFilletv2:

                    {
                        var item = VmMain.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.TPFilletv2:
                    {
                        var item = VmMain.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.PhuPham:
                    {
                        var item = VmMain.VmThanhPhamPhuPham.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BTPDinhHinh:
                    {
                        var item = VmMain.VmThanhPhamDinhHinh.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.TPDinhHinh:
                    {
                        var item = VmMain.VmThanhPhamDinhHinh.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.XepKhuon:
                    {
                        var item = VmMain.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                        goto case AppKV.XepKhuon;
                    case AppKV.XepKhuonBlock:
                        goto case AppKV.XepKhuon;
                    case AppKV.XepKhuonPhu:
                    {
                        var item = VmMain.VmThanhPhamPhuXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }

                    case AppKV.XepKhuonKXL:
                    {
                        var item = VmMain.VmThanhPhamKHCXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.Hq:
                    {
                        var item = VmMain.VmThanhPhamHq.Items.FirstOrDefault(x => x.Id == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

        return name;
    }

    public string LoaiNguyenLieuToName(string? id)
    {
        var name = "";
        if (id != null)
        {
            var item = VmMain.VmNguyenLieuHq.Items.FirstOrDefault(x => x.Id == id);
            if (item != null) name = item.Ten ?? "";
        }

        return name;
    }

    public string SizeToName(string? id, AppKV kv)
    {
        var name = "";
        if (id != null)
            try
            {
                switch (kv)
                {
                    case AppKV.Main:
                        break;
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                    {
                        var item = VmMain.VmSizeNguyenLieu.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BTPFillet:
                    {
                        var item = VmMain.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.TPFillet:
                    {
                        var item = VmMain.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BTPFilletv2:
                    {
                        var item = VmMain.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.TPFilletv2:
                    {
                        var item = VmMain.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.PhuPham:
                    {
                        var item = VmMain.VmSizePhuPham.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BTPDinhHinh:
                    {
                        var item = VmMain.VmSizeDinhHinh.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.TPDinhHinh:
                    {
                        var item = VmMain.VmSizeDinhHinh.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.XepKhuon:
                    {
                        var item = VmMain.VmSizeChinhXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                        goto case AppKV.XepKhuon;
                    case AppKV.XepKhuonBlock:
                        goto case AppKV.XepKhuon;
                    case AppKV.XepKhuonPhu:
                    {
                        var item = VmMain.VmSizePhuXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.XepKhuonKXL:
                    {
                        var item = VmMain.VmSizeKHCXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    case AppKV.Hq:
                    {
                        var item = VmMain.VmSizeHq.Items.FirstOrDefault(x => x.Id == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

        return name;
    }

    public string MauToName(string? id,AppKV kv)
    {
        var name = "";
        if (id != null)
        {
            try
            {
                switch (kv)
                {
                    case AppKV.Main:
                        break;
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                        break;
                    case AppKV.BTPFillet:
                        break;
                    case AppKV.TPFillet:
                        break;
                    case AppKV.BTPFilletv2:
                        break;
                    case AppKV.TPFilletv2:
                        break;
                    case AppKV.PhuPham:
                        break;
                    case AppKV.BTPDinhHinh:
                        break;
                    case AppKV.TPDinhHinh:
                        break;
                    case AppKV.XepKhuon:
                        break;
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                        break;
                    case AppKV.XepKhuonPhu:
                        break;
                    case AppKV.XepKhuonKXL:
                        break;
                    case AppKV.PhuPhamv2:
                        break;
                    case AppKV.Hq:
                        break;
                    case AppKV.XepKhuonBlock:
                    {
                        var item = VmMain.VmMauBlockXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
        }
        return name;
    }
    public string CongViecToName(string? id,AppKV kv)
    {
        var name = "";
        if (id != null)
        {
            try
            {
                switch (kv)
                {
                    case AppKV.Main:
                        break;
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                        break;
                    case AppKV.BTPFillet:
                        break;
                    case AppKV.TPFillet:
                        break;
                    case AppKV.BTPFilletv2:
                        break;
                    case AppKV.TPFilletv2:
                        break;
                    case AppKV.PhuPham:
                        break;
                    case AppKV.BTPDinhHinh:
                        break;
                    case AppKV.TPDinhHinh:
                        break;
                    case AppKV.XepKhuon:
                        break;
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                        break;
                    case AppKV.XepKhuonPhu:
                        break;
                    case AppKV.XepKhuonKXL:
                        break;
                    case AppKV.PhuPhamv2:
                        break;
                    case AppKV.Hq:
                        break;
                    case AppKV.XepKhuonBlock:
                    {
                        var item = VmMain.VmCongDoanXepKhuon.Items.FirstOrDefault(x => x.Ma == id);
                        if (item != null) name = item.Ten;
                        break;
                    }
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
        }

        return name;
    }

    #endregion
}