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

namespace Models.Repos.NotiModel
{
    public partial class KC_KhuVucTrangThai : ObservableObject
    {
        [ObservableProperty]private KC_KhuVuc kc_KhuVuc;
        [ObservableProperty]private string maXuong;
    }
}
