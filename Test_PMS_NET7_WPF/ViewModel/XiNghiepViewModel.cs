using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Test_PMS_NET7_WPF.ViewModel
{
    public partial class XiNghiepViewModel : ObservableObject
    {
        private static XiNghiepViewModel instance;
        private  ViewModels.Repos.HQ.XiNghiepViewModel vm = ViewModels.Repos.HQ.XiNghiepViewModel.Instance;

        public ObservableRangeCollection<Models.Repos.Models.XiNghiep> Items => vm.Items;
        public static XiNghiepViewModel Instance
        {
            get => instance??=new ();
        }

        private XiNghiepViewModel()
        {

        }
        private ObservableRangeCollection<Models.Repos.Models.XiNghiep> Gets()
        {
            var items = new ObservableRangeCollection<XiNghiep>();
            App.Current.Dispatcher.Invoke(() =>
            {
              items= vm.Items;
            });
            return items;
        }
        [RelayCommand]
        private void Reload()
        {
           
            App.Current.Dispatcher.Invoke(() =>
            {
                vm.Reload_Command.Execute(null);
            });
            
           
           
        }
    }
}
