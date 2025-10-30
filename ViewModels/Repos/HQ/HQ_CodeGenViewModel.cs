using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Models.Repos;
using Models.Repos.Models;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Globalization;

namespace ViewModels.Repos.HQ
{
    public partial class HQ_CodeGenViewModel : ObservableObject
    {
        private static HQ_CodeGenViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_CodeGen? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_CodeGen> items = new();
        [ObservableProperty] private ObservableRangeCollection<HQ_CodeGen> usedItems = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private HQ_CodeGen? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private HQ_CodeGenViewModel()
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

        public static HQ_CodeGenViewModel Instance => instance ??= new HQ_CodeGenViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public HQ_CodeGen CopyItem(HQ_CodeGen item)
        {
            return new HQ_CodeGen
            {
                Id = item.Id,
                NgayTao = item.NgayTao,
                DangKyIds = item.DangKyIds,
                Printed = item.Printed,
            };
        }
        public HQ_CodeGen CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public HQ_CodeGen CreateDefaultNew()
        {
            var maxId = Items
                   .Select(x => x.Id)
                   .DefaultIfEmpty(0)
                   .Max();
            var id = maxId + 1;
            return new HQ_CodeGen
            {
                Id = id,
                NgayTao = DateTime.Now,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_CodeGen();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(HQ_CodeGen item)
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
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public async Task DeleteAsync(HQ_CodeGen item)
        {
            try
            {
                var result = await Task.Run(() => Delete(item));

                if (result > 0)
                {
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            Items.Remove(_item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.HQ_CodeGen();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_CodeGen();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public void Insert_(HQ_CodeGen item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<HQ_CodeGen>();
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
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public async Task InsertAsync(HQ_CodeGen item)
        {
            try
            {
                var result = await Task.Run(() => Insert(item));

                if (result > 0)
                {
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
        private bool IsItemPass(HQ_CodeGen item)
        {
            return item != null && item.NgayTao != null && item.DangKyIds.Trim() != "" &&
                   item.Printed != null;
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
                UsedItems.Clear();
            }

            var items = Gets<HQ_CodeGen>();
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
        private void Reload_(ObservableRangeCollection<HQ_CodeGen> obj)
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
            var dao = new Dao.Repos.HQ.HQ_CodeGen();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public void Update_(HQ_CodeGen item)
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
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public async Task UpdateAsync(HQ_CodeGen item)
        {
            try
            {
                var result = await Task.Run(() => Update(item));

                if (result > 0)
                {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
        public HQ_CodeGen? Find(long id)
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
        public async Task<HQ_CodeGen?> FindAsync(long id)
        {
            try
            {
                return await Task.Run(() => Items.FirstOrDefault(x => x.Id == id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(HQ_CodeGen item)
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
