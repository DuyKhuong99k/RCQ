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
    public partial class ThanhPhamPhoiTronViewModel : ObservableObject
    {
        private static ThanhPhamPhoiTronViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPham_PhoiTron? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPham_PhoiTron> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPham_PhoiTron? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamPhoiTronViewModel()
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

        public static ThanhPhamPhoiTronViewModel Instance => instance ??= new ThanhPhamPhoiTronViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPham_PhoiTron CopyItem(MaThanhPham_PhoiTron item)
        {
            return new MaThanhPham_PhoiTron
            {
                CreateBy = item.CreateBy,
                CreateDateTime = item.CreateDateTime,
                MaKhuVuc = item.MaKhuVuc,
                MaThanhPhamDes = item.MaThanhPhamDes,
                MaThanhPhamOrg = item.MaThanhPhamOrg,
                ModifyBy = item.ModifyBy,
                ModifyDateTime = item.ModifyDateTime,
                Ngay = item.Ngay,
                TyLe = item.TyLe,
                MaLo = item.MaLo,
                MaXuong = item.MaXuong
            };
        }
        public MaThanhPham_PhoiTron CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPham_PhoiTron CreateDefaultNew()
        {
            return new MaThanhPham_PhoiTron()
            {
                //MaKhuVuc = KhuVucViewModel.Ins.SelectedItem.Id,
                TyLe = 0,
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                //CreateBy = AppViewModel.Instance.UserName,
                CreateDateTime = DateTime.Now,
                //ModifyBy = AppViewModel.Instance.UserName,
                ModifyDateTime = DateTime.Now,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem.Ma
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPham_PhoiTron();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPham_PhoiTron item)
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
            var dao = new Dao.Repos.HQ.MaThanhPham_PhoiTron();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPham_PhoiTron();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPham_PhoiTron item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPham_PhoiTron>();
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
        private bool IsItemPass(MaThanhPham_PhoiTron item)
        {
            return item.Ngay != null &&
                 item.MaThanhPhamDes != null &&
                 item.MaThanhPhamDes.Trim() != string.Empty &&
                 item.MaThanhPhamOrg != null &&
                 item.MaThanhPhamOrg.Trim() != string.Empty &&
                 item.TyLe >= 0 &&
                 item.MaKhuVuc != null &&
                 item.MaKhuVuc.Trim() != string.Empty;
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

            var items = Gets<MaThanhPham_PhoiTron>();
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
        private void Reload_(ObservableRangeCollection<MaThanhPham_PhoiTron> obj)
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
            var dao = new Dao.Repos.HQ.MaThanhPham_PhoiTron();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPham_PhoiTron item)
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
            //}
        }
        public MaThanhPham_PhoiTron? Find(DateTime ngay)
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

        public bool Exists(MaThanhPham_PhoiTron item)
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
        public List<T> GetsAllByMaKhuVucDH<T>(DateTime dateTime, string maKhuVuc, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.MaThanhPham_PhoiTron(connStr);
            return dao.GetsWithKhuVucDH<T>(dateTime, maKhuVuc);
        }
        public List<T> GetsAllByMaKhuVucFL<T>(DateTime dateTime, string maKhuVuc, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.MaThanhPham_PhoiTron(connStr);
            return dao.GetsWithKhuVucFL<T>(dateTime, maKhuVuc);
        }
    }
}
