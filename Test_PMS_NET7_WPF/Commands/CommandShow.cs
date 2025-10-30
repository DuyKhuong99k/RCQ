using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppViewModels;
using Models.Repos.Models;
using System.Windows;

namespace Test_PMS_NET7_WPF.Commands
{
    public partial class CommandShow : ObservableObject
    {
        private static CommandShow ins;
        private CommandShow() {
            
        }
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public static CommandShow Ins
        {
            get => ins ??= new CommandShow();
            set => ins = value;
        }

        [RelayCommand]
        private void XiNghiep_()
        {
            try
            {
               
                var view = new Test_PMS_NET7_WPF.View.XiNghiep.XiNghiepTestItemView();
                view.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //VmMessage.SetExceptionCommand.Execute(e);
                //throw;
            }
        }
        [RelayCommand]
        public void ViTriFillet_()
        {
            try
            {
                var view = new Test_PMS_NET7_WPF.View.ViTriFillet.ViTriFilletTestItemView();
                view.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }
        [RelayCommand]
        public void TChiTietVoXo_()
        {
            try
            {
                var view = new Test_PMS_NET7_WPF.View.Tom.ChiTietVoXoTestItemView();
                view.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }
        [RelayCommand]
        public void BanCatTiet_()
        {
            try
            {
                var view = new Test_PMS_NET7_WPF.View.BanCatTiet.BanCatTietTestItemView();
                view.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //throw;
            }
        }

        public void Insert(Window wd)
        {

        }
        public int MessageBoxShow(string message, string title, int button)
        {
                var rl = MessageBox.Show(message, title, (MessageBoxButton)Enum.ToObject(typeof(MessageBoxButton), button));
                return (int)rl;
            
        }
        
        [RelayCommand]
        private void CloseWindow_(Window wd)
        {
            try
            {
                wd?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }
        private int CloseXiNghiepItem_(object o)
        {
            var wd = o as Window;
          wd?.Close();
            return 0;
        }
        
        //private void ShowPanel_()
        //{
        //    try
        //    {
        //        AppViewModel.Instance.IsPanelVisible = AppViewModel.Instance.ShowPanel();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //        //throw;
        //    }
        //}

    }
}
