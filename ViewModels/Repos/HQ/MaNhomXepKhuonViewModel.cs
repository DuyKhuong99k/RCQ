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
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace ViewModels.Repos.HQ
{
    public class MaNhomXepKhuonViewModel
    {
        private static MaNhomXepKhuonViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static MaNhomXepKhuonViewModel Instance => instance ??= new MaNhomXepKhuonViewModel();
        public List<Models.Repos.Models.MaNhomXepKhuon> Gets(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.MaNhomXepKhuon(connStr);
                return dao.Gets(xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
