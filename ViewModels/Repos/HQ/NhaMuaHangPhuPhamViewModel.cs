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
    public partial class NhaMuaHangPhuPhamViewModel : ObservableObject
    {
        private static NhaMuaHangPhuPhamViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NhaMuaPhuPham? item;
        [ObservableProperty] private ObservableRangeCollection<NhaMuaPhuPham> items = new();
        [ObservableProperty] private ObservableRangeCollection<NhaMuaPhuPham> usedItems = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NhaMuaPhuPham? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NhaMuaHangPhuPhamViewModel()
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

        public static NhaMuaHangPhuPhamViewModel Instance => instance ??= new NhaMuaHangPhuPhamViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public NhaMuaPhuPham CopyItem(NhaMuaPhuPham item)
        {
            return new NhaMuaPhuPham
            {
                SuDung = item.SuDung,
                Ma = item?.Ma,
                Ten = item?.Ten
            };
        }
        public NhaMuaPhuPham CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NhaMuaPhuPham CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
                   .Select(x => int.Parse(x.Ma))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new NhaMuaPhuPham
            {
                SuDung = true,
                Ma = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhaMuaPhuPham();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NhaMuaPhuPham item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
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
            var dao = new Dao.Repos.HQ.NhaMuaPhuPham();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhaMuaPhuPham();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NhaMuaPhuPham item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NhaMuaPhuPham>();
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
        private bool IsItemPass(NhaMuaPhuPham item)
        {
            return item != null && item.Ten != null && item.Ten.Trim() != "" &&
                   item.SuDung != null;
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

            var items = Gets<NhaMuaPhuPham>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach (var item in items)
                        {
                            Items.Add(item);
                            if (item.SuDung == true)
                            {
                                UsedItems.Add(item);
                            }
                        }
                    }
                    catch (NotSupportedException e)
                    {

                    }

                }


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<NhaMuaPhuPham> obj)
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
            var dao = new Dao.Repos.HQ.NhaMuaPhuPham();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NhaMuaPhuPham item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
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
                        var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
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
        public NhaMuaPhuPham? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Ma == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(NhaMuaPhuPham item)
        {
            try
            {

                return Find(item.Ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
