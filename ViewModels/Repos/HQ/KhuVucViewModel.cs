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
    public partial class KhuVucViewModel : ObservableObject
    {
        private static KhuVucViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private Models.Repos.A_Model.KhuVuc? item;
        [ObservableProperty] private ObservableRangeCollection<Models.Repos.A_Model.KhuVuc> items = new();

        //[ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private Models.Repos.A_Model.KhuVuc? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private KhuVucViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static KhuVucViewModel Instance => instance ??= new KhuVucViewModel();
        public List<Models.Repos.A_Model.KhuVuc> Gets()
        {
            try
            {
                var items = new List<Models.Repos.A_Model.KhuVuc>
                {
                    new Models.Repos.A_Model.KhuVuc
                    {
                        Id = "FL",
                        Name = "Fillet"
                    },
                    new Models.Repos.A_Model.KhuVuc
                    {
                        Id = "DH",
                        Name = "Định Hình"
                    }
                };
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public static KhuVucViewModel Instance => instance ??= new KhuVucViewModel();

        //private AppViewModel VmApp => AppViewModel.Instance;
        //private MessageViewModel VmMessage => MessageViewModel.Instance;

        //public T_KhachHang CopyItem(T_KhachHang item)
        //{
        //    return new T_KhachHang
        //    {
        //        SuDung = item.SuDung,
        //        Ma = item?.Ma,
        //        Ten = item?.Ten
        //    };
        //}
        //public T_KhachHang CopySelectedItem()
        //{
        //    return CopyItem(SelectedItem);
        //}

        //public T_KhachHang CreateDefaultNew()
        //{
        //    var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
        //           .Select(x => int.Parse(x.Ma))
        //           .DefaultIfEmpty(0)
        //           .Max();
        //    var id = $"{(maxId + 1).ToString()}";
        //    return new T_KhachHang
        //    {
        //        SuDung = true,
        //        Ma = id
        //    };
        //}

        //private int Delete<T>(T item)
        //{
        //    var dao = new Dao.Repos.HQ.TKhachHang();
        //    return dao.Delete(item);
        //}

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Delete_(T_KhachHang item)
        //{
        //    try
        //    {
        //        if (Delete(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    //Items.Insert(index,item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //private List<T> Gets<T>()
        //{
        //    var dao = new Dao.Repos.HQ.TKhachHang();
        //    return dao.Gets<T>();
        //}

        //private int Insert<T>(T item)
        //{
        //    var dao = new Dao.Repos.HQ.TKhachHang();
        //    return dao.Insert(item);
        //}

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Insert_(T_KhachHang item)
        //{
        //    try
        //    {
        //        if (Insert(item) > 0)
        //        {
        //            var items = new List<T_KhachHang>();
        //            lock (Items)
        //            {
        //                Items.Add(item);
        //            }
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //            IsWindowItemShown = false;
        //        }
        //        else
        //        {
        //            VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //[RelayCommand()]
        //private void Insert2_()
        //{
        //    try
        //    {
        //        if (Insert(Item) > 0)
        //        {
        //            Items.Insert(0, Item);
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        VmMessage.SetExceptionCommand.Execute(e);
        //        //throw;
        //    }
        //}
        //public bool IsVailSelectedItem => SelectedItem != null;
        //private bool IsItemPass(T_KhachHang item)
        //{
        //    return item != null && item.Ten != null && item.Ten.Trim() != "" &&
        //           item.SuDung != null;
        //}
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
        //public void Reload()
        //{
        //    lock (Items)
        //    {
        //        Items.Clear();
        //    }

        //    var items = Gets<T_KhachHang>();
        //    if (items.Any())
        //        lock (Items)
        //        {
        //            try
        //            {
        //                //Items.AddRange(items);
        //                foreach (var item in items)
        //                {
        //                    Items.Add(item);
        //                }

        //                var ramdom = new Random();
        //                var i = ramdom.Next(0, items.Count - 1);
        //                Items.RemoveAt(i);
        //            }
        //            catch (NotSupportedException e)
        //            {

        //            }

        //        }


        //}

        //[RelayCommand]
        //private void Reload_(ObservableRangeCollection<T_KhachHang> obj)
        //{
        //    try
        //    {
        //        Reload();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //private int Update<T>(T item)
        //{
        //    var dao = new Dao.Repos.HQ.TKhachHang();
        //    return dao.Update(item);
        //}

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Update_(T_KhachHang item)
        //{
        //    try
        //    {
        //        if (Update(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
        //[RelayCommand()]
        //private void Update2_()
        //{
        //    try
        //    {
        //        if (Update(Item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, Item);
        //                    SelectedItem = Item;
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
    }
}
