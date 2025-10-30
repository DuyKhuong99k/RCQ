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


namespace ViewModels.Repos.HQ
{
    public partial class LoTheoLineViewModel : ObservableObject
    {
        private static LoTheoLineViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private LoTheoLine? item;
        [ObservableProperty] private ObservableRangeCollection<LoTheoLine> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private LoTheoLine? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private LoTheoLineViewModel()
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

        public static LoTheoLineViewModel Instance => instance ??= new LoTheoLineViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public LoTheoLine CopyItem(LoTheoLine item)
        {
            return new LoTheoLine
            {
                Id = item.Id,
                MaLo = item.MaLo,
                MaLine = item.MaLine,
                Ngay = item.Ngay,
                Gio = item.Gio,
                CodeId = item.CodeId
            };
        }
        public LoTheoLine CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public LoTheoLine CreateDefaultNew()
        {
            var maxId = Items
               .Select(x => x.Id)
               .DefaultIfEmpty(0)
               .Max();
            var id = maxId + 1;
            return new LoTheoLine
            {
                Id = id,
                CodeId = XiNghiepViewModel.Instance.SelectedItem?.CodeId,
                Gio = DateTime.Now.TimeOfDay,
                Ngay = AppViewModel.Instance.DateTimeNow
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.LoTheoLine();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(LoTheoLine item)
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
            var dao = new Dao.Repos.HQ.LoTheoLine();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.LoTheoLine();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(LoTheoLine item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<LoTheoLine>();
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
        private bool IsItemPass(LoTheoLine item)
        {
            return item != null && item.MaLo != null && item.MaLo.Trim() != "" &&
                   item.MaLine != null && item.MaLine.Trim() != "";
        }
        [RelayCommand]
        private void ForceRaseCanExcute()
        {
            try
            {
                Insert_Command.NotifyCanExecuteChanged();
                Delete_Command.NotifyCanExecuteChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<LoTheoLine>();
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
        private void Reload_(ObservableRangeCollection<LoTheoLine> obj)
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
            var dao = new Dao.Repos.HQ.LoTheoLine();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(LoTheoLine item)
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
        public LoTheoLine? Find(int id)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(LoTheoLine item)
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

        public List<string> GetDefaultLos()
        {
            try
            {
                var items = new List<string>();
                for (int i = 1; i <= 50; i++)
                {
                    var item = $@"{AppViewModel.Instance.DateTimeNow:yyyyMMdd}.{i:00}";
                    items.Add(item);
                }

                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetLosWithDate(DateTime ngay)
        {
            try
            {
                var items = new List<string>();
                for (int i = 1; i <= 50; i++)
                {
                    var item = $@"{ngay:yyyyMMdd}.{i:00}";
                    items.Add(item);
                }

                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetsFullField<T>(DateTime ngay, string maLo)
        {
            var dao = new Dao.Repos.HQ.LoTheoLine();
            return dao.GetLoTheoLines<T>(ngay,maLo);
        }
        public List<T> GetsFullFieldLastNew<T>(DateTime ngay,string maLo)
        {
            var dao = new Dao.Repos.HQ.LoTheoLine();
            return dao.GetLoTheoLinesMoiNhat<T>(ngay, maLo);
        }
    }
}
