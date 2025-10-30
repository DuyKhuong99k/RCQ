using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class BoTriLoSizeThanhPhamViewModel : ObservableObject
    {
        private static BoTriLoSizeThanhPhamViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private BoTriLoSizeThanhPham? item;
        [ObservableProperty] private ObservableRangeCollection<BoTriLoSizeThanhPham> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private BoTriLoSizeThanhPham? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private BoTriLoSizeThanhPhamViewModel()
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

        public static BoTriLoSizeThanhPhamViewModel Instance => instance ??= new BoTriLoSizeThanhPhamViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public BoTriLoSizeThanhPham CopyItem(BoTriLoSizeThanhPham item)
        {
            return new BoTriLoSizeThanhPham
            {
                Id = item.Id,
                MaLo = item.MaLo,
                MaViTri = item.MaViTri,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                Ngay = item.Ngay,
                Gio = item.Gio,
                CodeId = item.CodeId,
                MaSizePhu = item.MaSizePhu
            };
        }
        public BoTriLoSizeThanhPham CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public BoTriLoSizeThanhPham CreateDefaultNew()
        {
            var maxId = Items.Select(x => x.Id).DefaultIfEmpty(0).Max();
            //var id = $"{(maxId + 1).ToString("000000")}";
            var id = maxId + 1;
            return new BoTriLoSizeThanhPham
            {
                //Ngay = VmApp.DateTimeNow,
                //Gio = DateTime.Now.TimeOfDay,
                Id = id,
                //CodeId = VmXiNghiep.XiNghiepSelectedItem?.CodeId
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BoTriLoSizeThanhPham();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(BoTriLoSizeThanhPham item)
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
            var dao = new Dao.Repos.HQ.BoTriLoSizeThanhPham();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BoTriLoSizeThanhPham();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(BoTriLoSizeThanhPham item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<BoTriLoSizeThanhPham>();
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
        private bool IsItemPass(BoTriLoSizeThanhPham item)
        {
            return item != null &&
                 item.Id != 0 &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != "" &&
                 item.MaViTri != null &&
                 item.MaViTri.Trim() != "" &&
                 item.MaSize != null &&
                 item.MaSize.Trim() != "" &&
                 item.MaThanhPham != null &&
                 item.MaThanhPham.Trim() != "" &&
                 item.Ngay != null &&
                 item.Gio != null;
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

            var items = Gets<BoTriLoSizeThanhPham>();
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
        private void Reload_(ObservableRangeCollection<BoTriLoSizeThanhPham> obj)
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
            var dao = new Dao.Repos.HQ.BoTriLoSizeThanhPham();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(BoTriLoSizeThanhPham item)
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
        public BoTriLoSizeThanhPham? Find(int id)
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

        public bool Exists(BoTriLoSizeThanhPham item)
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
