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
    public partial class CommandShowSua : ObservableObject
    {
        private static CommandShowSua ins;
        private bool _xiNghiepSuaCanExcute => XiNghiepViewModel.Instance.IsVailSelectedItem;
        private bool _viTriFilletSuaCanExcute => ViTriFilletViewModel.Instance.IsVailSelectedItem;
        public CommandShowSua() { }
        public static CommandShowSua Ins
        {
            get => ins ??= new CommandShowSua();
            set => ins = value;
        }
        [RelayCommand]
        public void XiNghiep_()
        {
            try
            {
                var vm = XiNghiepViewModel.Instance;
                vm.IdItemIsReadOnly = false;
                vm.IsAdd = false;
                vm.IsEdit = true;
                vm.Item = vm.CopySelectedItem();
                var view = new Test_PMS_NET7_WPF.View.XiNghiep.XiNghiepItemView();
                view.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
            
        }
        [RelayCommand(CanExecute =nameof(_xiNghiepSuaCanExcute))]
        public void XiNghiep2_()
        {
            try
            {
                var vm = XiNghiepViewModel.Instance;
                vm.IdItemIsReadOnly = false;
                vm.IsAdd = false;
                vm.IsEdit = true;
                vm.Item = vm.CopySelectedItem();
               AppViewModel.Instance.IsPanelVisible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }
        [RelayCommand(CanExecute = nameof(_viTriFilletSuaCanExcute))]
        public void ViTriFillet_()
        {
            try
            {
                var vm = ViTriFilletViewModel.Instance;
                vm.IdItemIsReadOnly = false;
                vm.IsAdd = false;
                vm.IsEdit = true;
                vm.Item = vm.CopySelectedItem();
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
