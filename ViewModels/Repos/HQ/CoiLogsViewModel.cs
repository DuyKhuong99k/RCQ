using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ
{
    public partial class CoiLogsViewModel : ObservableObject
    {
        private static CoiLogsViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private CoiLogs? item;
        [ObservableProperty] private ObservableRangeCollection<CoiLogs> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private CoiLogs? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private CoiLogsViewModel()
        {
            try
            {
                Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        
        public static CoiLogsViewModel Instance => instance ??= new CoiLogsViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public CoiLogs CopyItem(CoiLogs item)
        {
            return new CoiLogs
            {
                
                Id = item.Id,
                MaCoi = item.MaCoi,
                MaXuong = item.MaXuong,
                NgayGio = item.NgayGio,
                ActionName = item.ActionName,
                TimeQuay = item.TimeQuay,
                HzQuay = item.HzQuay,
                HzRa = item.HzRa,
                IsError = item.IsError,
                ErrorStr = item.ErrorStr,
                Decription = item.Decription,
                IdMonitor = item.IdMonitor,
                NhanVienId = item.NhanVienId
            };
        }
        public CoiLogs CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public CoiLogs CreateDefaultNew()
        {
           
            return new CoiLogs
            {
                NgayGio = DateTime.Now,
                IsError = false
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.CoiLogs();
            return dao.Delete(item);
        }

        public List<T> GetLasts5Day<T>()
        {
            var dao = new Dao.Repos.HQ.CoiLogs();
            return dao.GetLasts5Day<T>();
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.CoiLogs();
            return dao.GetsLast<T>(dateTime, num);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(CoiLogs item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            //Items.Insert(index,item);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.CoiLogs();
            return dao.Gets<T>();
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.CoiLogs();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(CoiLogs item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<CoiLogs>();
                    lock (Items)
                    {
                        Items.Add(item);
                    }
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                    IsWindowItemShown = false;
                }
                else
                {
                    VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        [RelayCommand()]
        private void Insert2_()
        {
            try
            {
                if (Insert(Item) > 0)
                {
                    Items.Insert(0, Item);
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                }
            }
            catch (Exception e)
            {
                VmMessage.SetExceptionCommand.Execute(e);
                //throw;
            }
        }
        public bool IsVailSelectedItem => SelectedItem != null;
        private bool IsItemPass(CoiLogs item)
        {
            return item != null && item.MaCoi != null && item.MaCoi.Trim() !="" && item.MaXuong != null && item.MaXuong.Trim() != "" && item.ActionName != null && item.ActionName.Trim() != "" && item.NgayGio != null;
        }
        //[RelayCommand]
        //private void ForceRaseCanExcute()
        //{
        //    try
        //    {
        //        Insert_Command.NotifyCanExecuteChanged();
        //        Delete_Command.NotifyCanExecuteChanged();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //    }
        //}
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<CoiLogs>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }

                      
                    }
                    catch (NotSupportedException e)
                    {

                    }

                }


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<CoiLogs> obj)
        {
            try
            {
                Reload();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.CoiLogs();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(CoiLogs item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, item);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        [RelayCommand()]
        private void Update2_()
        {
            try
            {
                if (Update(Item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == Item.Id);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, Item);
                            SelectedItem = Item;
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public CoiLogs? Find(long ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Id == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(CoiLogs item)
        {
            try
            {

                return Find(item.Id) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
