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
    public partial class MauCaGiongVungNuoiViewModel : ObservableObject
    {
        private static MauCaGiongVungNuoiViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaMauCaGiongVungNuoi? item;
        [ObservableProperty] private ObservableRangeCollection<MaMauCaGiongVungNuoi> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaMauCaGiongVungNuoi? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private MauCaGiongVungNuoiViewModel()
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

        public static MauCaGiongVungNuoiViewModel Instance => instance ??= new MauCaGiongVungNuoiViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaMauCaGiongVungNuoi CopyItem(MaMauCaGiongVungNuoi item)
        {
            return new MaMauCaGiongVungNuoi
            {
                MaGhe = item.MaGhe,
                Ngay = item.Ngay,
                SoLuong = item.SoLuong,
                TrongLuong = item.TrongLuong,

                TrongLuongDonVi = item.TrongLuongDonVi,
            };
        }
        public MaMauCaGiongVungNuoi CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaMauCaGiongVungNuoi CreateDefaultNew()
        {
            return new MaMauCaGiongVungNuoi()
            {
                Ngay = AppViewModel.Instance.DateTimeNow.Date
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaMauCaGiongVungNuoi();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaMauCaGiongVungNuoi item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ngay == item.Ngay);
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
            var dao = new Dao.Repos.HQ.MaMauCaGiongVungNuoi();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaMauCaGiongVungNuoi();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaMauCaGiongVungNuoi item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaMauCaGiongVungNuoi>();
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
        private bool IsItemPass(MaMauCaGiongVungNuoi item)
        {
            return item != null && item.Ngay != null && item.MaGhe != null && item.MaGhe.Trim() != "";
        }
       
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<MaMauCaGiongVungNuoi>();
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
        private void Reload_(ObservableRangeCollection<MaMauCaGiongVungNuoi> obj)
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
            var dao = new Dao.Repos.HQ.MaMauCaGiongVungNuoi();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaMauCaGiongVungNuoi item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ngay == item.Ngay);
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
                        var _item = Items.SingleOrDefault(x => x.Ngay == Item.Ngay);
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
        public MaMauCaGiongVungNuoi? Find(DateTime ngay)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Ngay == ngay);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(MaMauCaGiongVungNuoi item)
        {
            try
            {

                return Find(item.Ngay) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<T> GetsFullField<T>()
        {
            var dao = new Dao.Repos.HQ.MaMauCaGiongVungNuoi();
            return dao.GetsFullField<T>();
        }
    }
}
