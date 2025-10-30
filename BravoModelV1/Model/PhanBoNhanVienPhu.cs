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
    public partial class PhanBoNhanVienPhu : ObservableObject
    {
        [ObservableProperty] private string _maNhanVien;
        [ObservableProperty] private string _maCongViec;
        [ObservableProperty] private DateTime _ngay;
        [ObservableProperty] private string _maXuong;
        [ObservableProperty] private double _tyLeHuong;
        [ObservableProperty] private double _tyLeTru;
    }
}
