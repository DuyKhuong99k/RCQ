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
    public partial class Thit_PhieuCanPhaLocViewModel : ObservableObject
    {
        private static Thit_PhieuCanPhaLocViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private Thit_PhieuCanPhaLoc? item;
        [ObservableProperty] private ObservableRangeCollection<Thit_PhieuCanPhaLoc> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private Thit_PhieuCanPhaLoc? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private Thit_PhieuCanPhaLocViewModel()
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

        public static Thit_PhieuCanPhaLocViewModel Instance => instance ??= new Thit_PhieuCanPhaLocViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public Thit_PhieuCanPhaLoc CopyItem(Thit_PhieuCanPhaLoc item)
        {
            return new Thit_PhieuCanPhaLoc
            {
                MaLoi = item.MaLoi,
                MaLo = item.MaLo,
                ItemCode = item.ItemCode,
                SuDung = item.SuDung,
                TrongLuong = item.TrongLuong,
                MaLoaiThanhPham = item.MaLoaiThanhPham,
                MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                InOut = item.InOut,
                IsLoi = item.IsLoi,
                MaMayCan = item.MaMayCan,
                MaNhanVien = item.MaNhanVien,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                MaSize = item.MaSize,
                MaThe = item.MaThe,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                STT = item.STT,
                IsNL = item.IsNL,
                IsSX = item.IsSX
            };
        }
        public Thit_PhieuCanPhaLoc CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public Thit_PhieuCanPhaLoc CreateDefaultNew()
        {
            var stt = 0;
            if (Items.Any())
                stt = Items.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            else
                stt = 1;

            return new Thit_PhieuCanPhaLoc
            {
                STT = stt,
                Gio = DateTime.Now.TimeOfDay,
                //MaLo = LoViewModel.Instance.ItemsWithSize?.FirstOrDefault()?.Item2,
                MaMayCan = AppViewModel.Instance.PCName,
                MaSize = SizeFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThe = "0",
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                Ngay = AppViewModel.Instance.DateTimeNow,
                MaLoaiThanhPham =
                    Thit_MaThanhPhamPhaLocViewModel.Instance.Items.FirstOrDefault()?.Ma,
                TrongLuong = 0,
                InOut = false,
                IsLoi = false,
                ItemCode = "none",
                MaLoaiNguyenLieu =
                    Thit_LoaiNguyenLieuPhaLocViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaLoi = MaLoiViewModel.Instance.Items.FirstOrDefault()?.Ma,
                SuDung = true,
                IsSX = false,
                IsNL = false
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.Thit_PhieuCanPhaLoc();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(Thit_PhieuCanPhaLoc item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
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
            var dao = new Dao.Repos.HQ.Thit_PhieuCanPhaLoc();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.Thit_PhieuCanPhaLoc();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(Thit_PhieuCanPhaLoc item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<Thit_PhieuCanPhaLoc>();
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
        private bool IsItemPass(Thit_PhieuCanPhaLoc item)
        {
            return item != null &&
                 item.Gio != null &&
                 item.Ngay != null &&
                 item.MaMayCan != null &&
                 item.MaMayCan.Trim() != string.Empty &&
                 item.STT > 0 &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty &&
                 item.MaSize != null &&
                 item.MaSize.Trim() != string.Empty &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != string.Empty &&
                 item.MaThe != null &&
                 item.MaThe.Trim() != string.Empty &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != string.Empty &&
                 item.MaLoaiNguyenLieu != null &&
                 item.MaLoaiNguyenLieu.Trim() != "" &&
                 item.MaLoaiThanhPham != null &&
                 item.MaLoaiThanhPham.Trim() != "" &&
                 item.TrongLuong > 0 &&
                 item.ItemCode != null &&
                 item.ItemCode.Trim() != "" &&
                 item.MaLoi != null &&
                 item.MaLoi.Trim() != "";
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

            var items = Gets<Thit_PhieuCanPhaLoc>();
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
        private void Reload_(ObservableRangeCollection<Thit_PhieuCanPhaLoc> obj)
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
            var dao = new Dao.Repos.HQ.Thit_PhieuCanPhaLoc();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(Thit_PhieuCanPhaLoc item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
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
                        var _item = Items.SingleOrDefault(x => x.STT == Item.STT);
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
        public Thit_PhieuCanPhaLoc? Find(DateTime dateTime,string xuongId,string mayCan,int stt)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == stt && x.Ngay.Date == dateTime.Date && x.MaXuong == xuongId && x.MaMayCan == mayCan);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(Thit_PhieuCanPhaLoc item)
        {
            try
            {

                return Find(item.Ngay,item.MaXuong,item.MaMayCan,item.STT) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
