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
    public partial class FilletV2_PhieuCanTongHopTinhLuong : ObservableObject
    {
        [ObservableProperty] private bool _caTra;
        [ObservableProperty] private bool _danhGia;
        [ObservableProperty] private decimal _dinhMucThucTe;
        [ObservableProperty] private decimal _dinhMucYeuCau;
        [ObservableProperty] private string _loaiCaName;
        [ObservableProperty] private string _maHoSo;
        [ObservableProperty] private string _maNhanVien;
        [ObservableProperty] private string _maThanhPham;
        [ObservableProperty] private string _sizeName;
        [ObservableProperty] private int _soRo;
        [ObservableProperty] private int _status = 0;
        [ObservableProperty] private string _tenNhanVien;
        [ObservableProperty] private string _thanhPhamName;
        [ObservableProperty] private decimal _trongLuongNhan;
        [ObservableProperty] private decimal _trongLuongTra;
        [ObservableProperty] private string barvoId;
        [ObservableProperty] private string caLamViec = "CT04";
        [ObservableProperty] private DateTime createdAt = DateTime.Now;
        [ObservableProperty] private decimal dinhMucThucTeOrg;
        [ObservableProperty] private decimal donGia;
        [ObservableProperty] private bool isTangCa;
        [ObservableProperty] private string maLo;
        [ObservableProperty] private string maSanPham;
        [ObservableProperty] private string maThanhPhamOrg;
        [ObservableProperty] private int soRoOrg;
        [ObservableProperty] private bool suDung = true;
        [ObservableProperty] private string thanhPhamNameOrg;
        [ObservableProperty] private decimal thanhTien;
        [ObservableProperty] private decimal trongLuongNhanOrg;
        [ObservableProperty] private decimal trongLuongTraOrg;
        [ObservableProperty] private decimal tyLe;
        [ObservableProperty] private bool isChamCong = false;
        [ObservableProperty] private string maXuong;
        [ObservableProperty] private string banName;
        [ObservableProperty] private int soBanDuocSap;
    }
}
