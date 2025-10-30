using AppModels;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Vars.Hubs;
using Microsoft.Data.SqlClient;

namespace ViewModels.Repos.HQ
{
    public partial class PhieuCanTPDinhHinh_TyLeViewModel
    {
         private static PhieuCanTPDinhHinh_TyLeViewModel instance ;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static PhieuCanTPDinhHinh_TyLeViewModel Instance => instance ??= new PhieuCanTPDinhHinh_TyLeViewModel();
        public List<T> GetsLast<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamDinhHinh_TyLe(connStr);
            return dao.GetsLast<T>(dateTime, xuongId);
        }
    }
}
