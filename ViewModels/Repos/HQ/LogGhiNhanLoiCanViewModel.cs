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
using System.Globalization;
using Models.Repos;

namespace ViewModels.Repos.HQ
{
    public partial class LogGhiNhanLoiCanViewModel : ObservableObject
    {
        private static LogGhiNhanLoiCanViewModel instance;
        [ObservableProperty] private LogGhiNhanLoiCan? item;
        [ObservableProperty] private ObservableRangeCollection<LogGhiNhanLoiCan> items = new();



        private LogGhiNhanLoiCanViewModel()
        {
            try
            {
               // Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static LogGhiNhanLoiCanViewModel Instance => instance ??= new LogGhiNhanLoiCanViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        
       


        public List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.LogGhiNhanLoiCan();
            return dao.Gets<T>();
        }
        public List<VmLogGhiNhanLoiCan> GetAllsFullField(DateTime ngay, string maMayCan)
        {
            var dao = new Dao.Repos.HQ.LogGhiNhanLoiCan();
             return dao.GetAllsFullField(ngay, maMayCan);
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.LogGhiNhanLoiCan();
            return dao.Insert(item);
        }

        public int GetMaxSTTByNgayVaMay(DateTime ngay, string maMayCan)
{
    var dao = new Dao.Repos.HQ.LogGhiNhanLoiCan();
    return dao.GetMaxSTTByNgayVaMay(ngay, maMayCan);
}

    }
}
