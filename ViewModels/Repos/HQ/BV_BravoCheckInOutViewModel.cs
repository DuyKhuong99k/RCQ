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
    public partial class BV_BravoCheckInOutViewModel
    {
        private static BV_BravoCheckInOutViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static BV_BravoCheckInOutViewModel Instance => instance ??= new BV_BravoCheckInOutViewModel();
        public List<string> GetEmployeeCodes(DateTime dateTime, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.BravoCheckInOut(connStr);
                return dao.GetEmployeeCodes(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> EmployeeCodes(DateTime dateTime)
        {
            var employeeCodes = GetEmployeeCodes(dateTime);
            return employeeCodes;
        }
        //[RelayCommand]
        //private void LoadEmployeeCodes(DateTime dateTime)
        //{
        //    try
        //    {
        //        EmployeeCodes.Clear();
        //        EmployeeCodes.AddRange(GetEmployeeCodes(dateTime));
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //    }
        //}
    }
}
