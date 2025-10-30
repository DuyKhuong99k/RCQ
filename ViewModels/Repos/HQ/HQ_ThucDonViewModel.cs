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
    public partial class HQ_ThucDonViewModel : ObservableObject
    {
        private static HQ_ThucDonViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_ThucDon? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_ThucDon> items = new();
        [ObservableProperty] private ObservableRangeCollection<HQ_ThucDon> usedItems = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private HQ_ThucDon? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private HQ_ThucDonViewModel()
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

        public static HQ_ThucDonViewModel Instance => instance ??= new HQ_ThucDonViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public HQ_ThucDon CopyItem(HQ_ThucDon item)
        {
            return new HQ_ThucDon
            {
                Id = item.Id,
                Ngay = item.Ngay,
                NgayTao = item.NgayTao,
                NguoiTao = item.NguoiTao,
                Ten = item.Ten,
                ThietBi = item.ThietBi,
                GhiChu = item.GhiChu,
            };
        }
        public HQ_ThucDon CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public HQ_ThucDon CreateDefaultNew()
        {
            var maxId = Items
                   .Select(x => x.Id)
                   .DefaultIfEmpty(0)
                   .Max();
            var id = maxId + 1;
            return new HQ_ThucDon
            {
                Id = id,
                NgayTao = DateTime.Now,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(HQ_ThucDon item)
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
        public async Task DeleteAsync(HQ_ThucDon item)
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
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.Gets<T>();
        }
        public int GetMaxId()
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.GetMaxId();
        }
        public List<T> GetDanhSachThucDon<T>(DateTime ngay)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.GetDanhSachThucDon<T>(ngay);
        }
        public List<T> GetChiTietTheoThucDon<T>(int thucDonId)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.GetChiTietTheoThucDon<T>(thucDonId);
        }
        public List<T> GetDanhSachTrangThaiThucDonTungNgay<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.GetDanhSachTrangThaiThucDonTungNgay<T>();
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(HQ_ThucDon item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<HQ_ThucDon>();
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
        public async Task InsertAsync(HQ_ThucDon item)
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
        public bool IsVailSelectedItem => SelectedItem != null;
        private bool IsItemPass(HQ_ThucDon item)
        {
            return item != null && item.Ngay != null && item.NgayTao != null &&
                   item.NguoiTao != null && item.Ten != null && item.ThietBi != null;
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

            var items = Gets<HQ_ThucDon>();
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
        private void Reload_(ObservableRangeCollection<HQ_ThucDon> obj)
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
            var dao = new Dao.Repos.HQ.HQ_ThucDon();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(HQ_ThucDon item)
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
        public async Task UpdateAsync(HQ_ThucDon item)
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
        public HQ_ThucDon? Find(long id)
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
        public async Task<HQ_ThucDon?> FindAsync(long id)
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

        public bool Exists(HQ_ThucDon item)
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
