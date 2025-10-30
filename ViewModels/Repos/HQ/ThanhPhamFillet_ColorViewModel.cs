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
    public partial class ThanhPhamFillet_ColorViewModel : ObservableObject
    {
        private static ThanhPhamFillet_ColorViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPhamFillet_Color? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamFillet_Color> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPhamFillet_Color? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamFillet_ColorViewModel()
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

        public static ThanhPhamFillet_ColorViewModel Instance => instance ??= new ThanhPhamFillet_ColorViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPhamFillet_Color CopyItem(MaThanhPhamFillet_Color item)
        {
            return new MaThanhPhamFillet_Color
            {
                ColorCode = item.ColorCode,
                MaLo = item.MaLo,
                MaThanhPham = item.MaThanhPham,
                MaSize = item.MaSize,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay
            };
        }
        public MaThanhPhamFillet_Color CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPhamFillet_Color CreateDefaultNew()
        {
            return new MaThanhPhamFillet_Color
            {
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                MaXuong = XiNghiepViewModel.Instance?.SelectedItem?.Ma??""
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_Color();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPhamFillet_Color item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaThanhPham == item.MaThanhPham);
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
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_Color();
            return dao.Gets<T>();
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_Color();
            return dao.Insert(item);
        }

        public List<T> GetsLastDay<T>()
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_Color();
            return dao.GetsLastDay<T>();
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPhamFillet_Color item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPhamFillet_Color>();
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
        private bool IsItemPass(MaThanhPhamFillet_Color item)
        {
        return item != null &&
             item.ColorCode != null &&
             item.ColorCode.Trim() != string.Empty &&
             item.MaLo != null &&
             item.MaLo.Trim() != string.Empty &&
             item.MaThanhPham != null &&
             item.MaThanhPham.Trim() != string.Empty &&

             item.MaXuong != null &&
             item.MaXuong.Trim() != string.Empty &&
             item.Ngay != null;
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

            var items = GetsLastDay<MaThanhPhamFillet_Color>();
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
        private void Reload_(ObservableRangeCollection<MaThanhPhamFillet_Color> obj)
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

        public int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_Color();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPhamFillet_Color item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaThanhPham == item.MaThanhPham);
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
                        var _item = Items.SingleOrDefault(x => x.MaThanhPham == Item.MaThanhPham);
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
        public MaThanhPhamFillet_Color? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaThanhPham == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(MaThanhPhamFillet_Color item)
        {
            try
            {

                return Find(item.MaThanhPham) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
