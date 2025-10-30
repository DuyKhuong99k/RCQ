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
    public partial class KC_KhuVuc : ObservableObject
    {
        [ObservableProperty] private string khuVucId;
        [ObservableProperty] private int tab;
        [ObservableProperty] private string name;

        public List<KC_KhuVuc> Gets()
        {
            try
            {
                return new List<KC_KhuVuc>()
                {
                    new KC_KhuVuc() { KhuVucId = "01", Tab = 1, Name = "Định Hình" },
                    new KC_KhuVuc() { KhuVucId = "02", Tab = 2, Name = "Tổ Kiểm Định Hình" },
                    new KC_KhuVuc() { KhuVucId = "03", Tab = 3, Name = "Sơ Chế" },
                    new KC_KhuVuc() { KhuVucId = "04", Tab = 4, Name = "Tổ Kiểm Sơ Chế" },
                    new KC_KhuVuc() { KhuVucId = "05", Tab = 5, Name = "Phục Vụ Định Hình" },
                    new KC_KhuVuc() { KhuVucId = "06", Tab = 6, Name = "Phụ Fillet" },
                    new KC_KhuVuc() { KhuVucId = "07", Tab = 7, Name = "Xếp Khuôn Gián Tiếp" },
                    new KC_KhuVuc() { KhuVucId = "08", Tab = 8, Name = "Xếp Khuôn Trực Tiếp" },
                    new KC_KhuVuc() { KhuVucId = "09", Tab = 9, Name = "Fillet" },
                    new KC_KhuVuc() { KhuVucId = "10", Tab = 10, Name = "Bao Tử" },
                    new KC_KhuVuc() { KhuVucId = "11", Tab = 11, Name = "Phụ Phẩm" },
                    new KC_KhuVuc() { KhuVucId = "12", Tab = 12, Name = "Lạng Da" }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
