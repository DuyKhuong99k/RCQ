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
using Models.Repos.AppModel;

namespace BravoModelV1.Model
{
    public partial class SanLuongPhuFillet : ObservableObject
    {
        [ObservableProperty] private string _bravoId;
        [ObservableProperty] private double _gioTyLe;
        [ObservableProperty] private string _maNhanVien;
        [ObservableProperty] private string _maThanhPham;
        [ObservableProperty] private double _sanLuongHuong;
        [ObservableProperty] private double _sanLuongTrenGio;
        [ObservableProperty] private double _sanLuongTru;
        [ObservableProperty] private double _soGio;
        [ObservableProperty] private int _sTT;
        [ObservableProperty] private double _tyLeHuong;
        [ObservableProperty] private double _tyLeTru;
        [ObservableProperty] decimal donGia;
        [ObservableProperty] decimal thanhTien;
    }
}
