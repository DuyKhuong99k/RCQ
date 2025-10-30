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
    public partial class PhieuLuongFillet : ObservableObject
    {
        [ObservableProperty] private int _status = 0;
        [ObservableProperty] private string bravoId;
        [ObservableProperty] private string caLamViec = "CT04";
        [ObservableProperty] private DateTime createdAt = DateTime.Now;
        [ObservableProperty] private decimal donGia;
        [ObservableProperty] private bool isChamCong = false;
        [ObservableProperty] private bool isTangCa;
        [ObservableProperty] private string maHoSo;
        [ObservableProperty] private string maLo;
        [ObservableProperty] private string maNhanVien;
        [ObservableProperty] private string maSanPham;
        [ObservableProperty] private string maThanhPham;
        [ObservableProperty] private string maThanhPhamOrg;
        [ObservableProperty] private string maXuong;
        [ObservableProperty] private DateTime ngay;
        [ObservableProperty] private string nhanVienName;
        [ObservableProperty] private int soRo;
        [ObservableProperty] private int soRoOrg;
        [ObservableProperty] private bool suDung = true;
        [ObservableProperty] private string tenSanPham;
        [ObservableProperty] private string thanhPhamNameOrg;
        [ObservableProperty] private decimal thanhTien;
        [ObservableProperty] private decimal trongLuong;
        [ObservableProperty] private decimal trongLuongOrg;
        [ObservableProperty] private decimal tyLe;

    }
}
