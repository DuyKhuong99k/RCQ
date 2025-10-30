using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ
{
    public partial class UserAreaViewModel : ObservableObject
    {
        private static UserAreaViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private UserArea? item;
        [ObservableProperty] private ObservableRangeCollection<UserArea> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private UserArea? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private UserAreaViewModel()
        {
            try
            {
                //Reload();
                

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                ////throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static UserAreaViewModel Instance => instance ??= new UserAreaViewModel();
        public bool IsVailSelectedItem => SelectedItem != null;
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<T> Gets<T>(string? connStr = null)
        {
            
            var dao = new Dao.Repos.HQ.UserArea(connStr);
            return dao.Gets<T>();
        }
        public List<T> Gets<T>(int userId,string? connStr = null)
        {
            
            var dao = new Dao.Repos.HQ.UserArea(connStr);
            return dao.Gets<T>(userId);
        }
    }
}
