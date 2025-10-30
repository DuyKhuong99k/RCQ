using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using System.Diagnostics;
using System.Windows;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Vars.Hubs;
using Microsoft.Data.SqlClient;
namespace Models.Repos.AppModel
{
    public partial class SanLuongNhanVienKiem : ObservableObject
    {
        [ObservableProperty] private decimal _donGia;
        [ObservableProperty] private decimal _gioTyLe;
        [ObservableProperty] private string _maSanPham;
        [ObservableProperty] private NhanViensKiem _nhanVienKiem;
        [ObservableProperty] private decimal _phanTramHuong;
        [ObservableProperty] private decimal _soGio;
        [ObservableProperty] private decimal _thanhTien;
        [ObservableProperty] private decimal _trongLuongHuong;
        [ObservableProperty] private decimal _trongLuongNhanThem;
        [ObservableProperty] private decimal _trongLuongNhom;
        [ObservableProperty] private decimal _trongLuongPhanChia;
        [ObservableProperty] private decimal _trongLuongTrenGio;
        [ObservableProperty] private decimal _tyLeTru;

        [ObservableProperty] bool isChamCong = false;
    }
}
