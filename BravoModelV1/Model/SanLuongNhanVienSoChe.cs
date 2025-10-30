using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Model
{
    public partial class SanLuongNhanVienSoChe : ObservableObject
    {
        [ObservableProperty] private decimal _GioTyLe;
        [ObservableProperty] private string _maNhanVien;
        [ObservableProperty] private string _maNhomSoChe;
        [ObservableProperty] private string _maSanPham;
        [ObservableProperty] private decimal _soGio;
        [ObservableProperty] private string _tenSanPham;
        [ObservableProperty] private decimal _trongLuongHuong;
        [ObservableProperty] private decimal _trongLuongNhanThem;
        [ObservableProperty] private decimal _trongLuongNhom;
        [ObservableProperty] private decimal _trongLuongPhanChia;
        [ObservableProperty] private decimal _trongLuongTrenGio;
        [ObservableProperty] private decimal _tyLeHuong;
        [ObservableProperty] private decimal _tyLeTru;
        [ObservableProperty] private decimal donGia;
        [ObservableProperty] private bool isChamCong = false;
        [ObservableProperty] private decimal thanhTien;
    }
}
