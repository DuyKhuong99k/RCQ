using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Repos.HQ;
using Test_PMS_NET7_WPF.Commands;

namespace Test_PMS_NET7_WPF.ViewModel
{
    public class MainViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private static MainViewModel ins;
        
        private MainViewModel()
        {
            VmMessage.MessageBoxShow = CommandShow.MessageBoxShow;
            VmApp.CloseWindow = CommandShow.CloseWindow_Command;
            VmApp.IsPanelVisible = false;

        }

        public CommandShow CommandShow => CommandShow.Ins;
        public CommandShowThem CommandShowThem => CommandShowThem.Ins;
        public CommandShowSua CommandShowSua => CommandShowSua.Ins;
        public AppViewModel VmApp => AppViewModel.Instance;
        public MessageViewModel VmMessage => MessageViewModel.Instance;
        public Test_PMS_NET7_WPF.ViewModel.XiNghiepViewModel  VmXiNghiepUI => Test_PMS_NET7_WPF.ViewModel.XiNghiepViewModel.Instance;
        public ViewModels.Repos.HQ.XiNghiepViewModel  VmXiNghiep => ViewModels.Repos.HQ.XiNghiepViewModel.Instance;

        public ViTriFilletViewModel VmViTriFillet => ViTriFilletViewModel.Instance;
        public TChiTietVoXoViewModel VmTChiTietVoXo => TChiTietVoXoViewModel.Instance;
        public BanCatTietViewModel VmBanCatTiet => BanCatTietViewModel.Instance;
        public static MainViewModel Ins
        {
            get => ins ??= new MainViewModel();
            set => ins = value;
        }

        

        
    }
}
