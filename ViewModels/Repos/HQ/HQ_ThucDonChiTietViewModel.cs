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
    public partial class HQ_ThucDonChiTietViewModel : ObservableObject
    {
        private static HQ_ThucDonChiTietViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_ThucDonChiTiet? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_ThucDonChiTiet> items = new();
        [ObservableProperty] private ObservableRangeCollection<HQ_ThucDonChiTiet> usedItems = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private HQ_ThucDonChiTiet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private HQ_ThucDonChiTietViewModel()
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

        public static HQ_ThucDonChiTietViewModel Instance => instance ??= new HQ_ThucDonChiTietViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public HQ_ThucDonChiTiet CopyItem(HQ_ThucDonChiTiet item)
        {
            return new HQ_ThucDonChiTiet
            {
                Id = item.Id,
                ThucDonId = item.ThucDonId,
                MonAnId = item.MonAnId,
                GhiChu = item.GhiChu,
            };
        }
        public HQ_ThucDonChiTiet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public HQ_ThucDonChiTiet CreateDefaultNew()
        {
            var maxId = Items
                   .Select(x => x.Id)
                   .DefaultIfEmpty(0)
                   .Max();
            var id = maxId + 1;
            return new HQ_ThucDonChiTiet
            {
                Id = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDonChiTiet();
            return dao.Delete(item);
        }
        private int DeleteByThucDonId<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDonChiTiet();
            return dao.DeleteByThucDonId(item);
        }
        private int DeleteByThucDonId(int thucDonId)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDonChiTiet();
            return dao.DeleteByThucDonId(thucDonId);
        }


        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(HQ_ThucDonChiTiet item)
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
        public async Task DeleteAsync(HQ_ThucDonChiTiet item)
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
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public async Task DeleteByThucDonIdsAsync(HQ_ThucDonChiTiet item)
        {
            try
            {
                var result = await Task.Run(() => DeleteByThucDonId(item));

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
        public async Task DeleteByThucDonIdsAsync(int thucDonId)
        {
            try
            {
                var result = await Task.Run(() => DeleteByThucDonId(thucDonId));
                if (result > 0)
                {
                    lock (Items)
                    {
                        Items.Clear();
                        UsedItems.Clear();
                    }

                    var items = Gets<HQ_ThucDonChiTiet>();
                    if (items.Any())
                    {
                        lock (Items)
                        {
                            try
                            {
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
            var dao = new Dao.Repos.HQ.HQ_ThucDonChiTiet();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDonChiTiet();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(HQ_ThucDonChiTiet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<HQ_ThucDonChiTiet>();
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
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        public async Task InsertAsync(HQ_ThucDonChiTiet item)
        {
            try
            {
                var result = await Task.Run(() => Insert(item));

                if (result > 0)
                {
                    //lock (Items)
                    //{
                    //    Items.Add(item);
                    //}
                    Reload();
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
        public bool IsVailSelectedItem => SelectedItem != null;
        private bool IsItemPass(HQ_ThucDonChiTiet item)
        {
            return item != null && item.ThucDonId != null &&
                   item.MonAnId != null;
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

            var items = Gets<HQ_ThucDonChiTiet>();
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
        private void Reload_(ObservableRangeCollection<HQ_ThucDonChiTiet> obj)
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
            var dao = new Dao.Repos.HQ.HQ_ThucDonChiTiet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(HQ_ThucDonChiTiet item)
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
        public async Task UpdateAsync(HQ_ThucDonChiTiet item)
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
        public HQ_ThucDonChiTiet? Find(long id)
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
        public async Task<HQ_ThucDonChiTiet?> FindAsync(long id)
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
        public async Task<List<HQ_ThucDonChiTiet>> FindByThucDonIdAsync(long thucDonId)
        {
            try
            {
                return await Task.Run(() => Items.Where(x => x.ThucDonId == thucDonId).ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error finding ThucDonId {thucDonId}: {e.Message}");
                throw;
            }
        }

        public bool Exists(HQ_ThucDonChiTiet item)
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
