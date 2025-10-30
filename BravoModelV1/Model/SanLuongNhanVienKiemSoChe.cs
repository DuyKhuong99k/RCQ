using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
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

namespace BravoModelV1.Model
{
    public partial class SanLuongNhanVienKiemSoChe : ObservableObject
    {
        [ObservableProperty] private decimal _gioTyLe;
        [ObservableProperty] private string _maNhanVien;
        [ObservableProperty] private string _maNhomKiem;
        [ObservableProperty] private decimal _soGio;
        [ObservableProperty] private decimal _trongLuongHuong;
        [ObservableProperty] private decimal _trongLuongNhanThem;
        [ObservableProperty] private decimal _trongLuongNhom;
        [ObservableProperty] private decimal _trongLuongPhanChia;
        [ObservableProperty] private decimal _trongLuongTrenGio;
        [ObservableProperty] private decimal _tyLeHuong;
        [ObservableProperty] private decimal _tyLeTru;
        [ObservableProperty] private decimal donGia;
        [ObservableProperty] private bool isChamCong = false;
        [ObservableProperty] private string maSanPham;
        [ObservableProperty] private decimal thanhTien;
    }
}
