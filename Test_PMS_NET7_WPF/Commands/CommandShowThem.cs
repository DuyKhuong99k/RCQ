using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModels.Repos.HQ;

namespace Test_PMS_NET7_WPF.Commands
{
    public partial class CommandShowThem : ObservableObject
    {
        private static CommandShowThem ins;
        private bool _xiNghiepThemCanExcute => XiNghiepViewModel.Instance.IsVailSelectedItem;
        private bool _viTriFilletThemCanExcute => ViTriFilletViewModel.Instance.IsVailSelectedItem;
        private CommandShowThem() { }
        public static CommandShowThem Ins
        {
            get => ins ??= new CommandShowThem();
            set => ins = value;
        }
        [RelayCommand]
        public void XiNghiep_()
        {
            try
            {
                var vm = XiNghiepViewModel.Instance;
                vm.IdItemIsReadOnly = false;
                vm.IsAdd = true;
                vm.IsEdit = false;
                vm.IsWindowItemShown = true;
                vm.Item = vm.CreateDefaultNew();
                var view = new Test_PMS_NET7_WPF.View.XiNghiep.XiNghiepItemView();
                view.ShowDialog();;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }
        [RelayCommand]
        public void XiNghiep2_()
        {
            try
            {
                var vm = XiNghiepViewModel.Instance;
                vm.IdItemIsReadOnly = false;
                vm.IsAdd = true;
                vm.IsEdit = false;
                vm.Item = vm.CreateDefaultNew();
                AppViewModel.Instance.IsPanelVisible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }
        [RelayCommand]
        public void ViTriFillet_()
        {
            try
            {
                var vm = ViTriFilletViewModel.Instance;
                vm.IdItemIsReadOnly = false;
                vm.IsAdd = true;
                vm.IsEdit = false;
                vm.Item = vm.CreateDefaultNew();
                AppViewModel.Instance.IsPanelVisible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }
    }
}
