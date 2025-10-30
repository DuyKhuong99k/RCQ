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
    public partial class NhanVienKiem  : ObservableObject
    {
         [ObservableProperty]private string _toId;
         [ObservableProperty]private string _name;
         [ObservableProperty]private double _soGio;
         [ObservableProperty]private double _tyLe;
         [ObservableProperty]private double _tyLeTru;
         [ObservableProperty]private NhanVienDaiThanh _nhanVienDaiThanh = new NhanVienDaiThanh();
    }
}
