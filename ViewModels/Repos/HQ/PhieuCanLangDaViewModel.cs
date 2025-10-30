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
    public partial class PhieuCanLangDaViewModel : ObservableObject
    {
        private static PhieuCanLangDaViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanLangDa? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanLangDa> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanLangDa? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanLangDaViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static PhieuCanLangDaViewModel Instance => instance ??= new PhieuCanLangDaViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanLangDa CopyItem(PhieuCanLangDa item)
        {
            return new PhieuCanLangDa
            {
                GhiChu = item.MayCan,
                MayCan = item.MayCan,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaNhanVien = item.MaNhanVien,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaThe = item.MaThe,
                MaUserCan = item.MaUserCan,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                STT = item.STT,
                TrongLuong = item.TrongLuong,
            };
        }
        public PhieuCanLangDa CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanLangDa CreateDefaultNew()
        {
            var stt = 0;
            if (Items.Any())
            {
                stt = Items.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            }
            else
            {
                stt = 1;
            }

            return new PhieuCanLangDa()
            {
                STT = stt,
                Ngay = AppViewModel.Instance.DateTimeNow,
                Gio = DateTime.Now.TimeOfDay,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                MaThe = ".",
                MayCan = AppViewModel.Instance.PCName,
                MaMau = MauLangDaViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaSize = SizeLangDaViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaLoaiCa = LoaiCaLangDaViewModel.Instance.Items.FirstOrDefault()?.Ma,
                // MaUserCan = AppViewModel.Ins.UserName
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanLangDa item)
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
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanLangDa item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanLangDa>();
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
        private bool IsItemPass(PhieuCanLangDa item)
        {
            return item != null &&
                 item.Gio != null &&
                 item.MaLoaiCa != null &&
                 item.MaLoaiCa.Trim() != string.Empty &&
                 item.Ngay != null &&
                 item.MayCan != null &&
                 item.MayCan.Trim() != string.Empty &&
                 item.STT > 0 &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty &&
                 item.MaUserCan != null &&
                 item.MaUserCan.Trim() != string.Empty &&
                 item.MaLoaiCa != null &&
                 item.MaLoaiCa.Trim() != string.Empty &&
                 item.MaMau != null &&
                 item.MaMau.Trim() != string.Empty &&
                 item.MaSize != null &&
                 item.MaSize.Trim() != string.Empty &&
                 item.MaThanhPham != null &&
                 item.MaThanhPham.Trim() != string.Empty &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != string.Empty &&
                 item.MaThe != null &&
                 item.MaThe.Trim() != string.Empty &&
                 item.TrongLuong > 0 &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != string.Empty;
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

            var items = Gets<PhieuCanLangDa>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanLangDa> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanLangDa item)
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

        #region Báo Cáo
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.GetChiTiets<T>(fromDate, toDate);
        }
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.GetChiTietByMaNhanViens<T>(fromDate, toDate,maNhanVien,xuongId);
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.GetChiTietByMaHoSos<T>(fromDate, toDate,maHoSo,xuongId);
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanLangDa();
            return dao.GetChiTietByMaThes<T>(fromDate, toDate,maThe,xuongId);
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanLangDa();
                return dao.GetTongHops<T>(fromDate, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate,string maNhanVien,string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanLangDa();
                return dao.GetTongHopThanhPhamByMaNhanViens<T>(fromDate, toDate,maNhanVien,xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate,string maThe,string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanLangDa();
                return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, toDate,maThe,xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanLangDa();
                return dao.GetTongHopThanhPhamByMaHoSos<T>(fromDate, toDate,maHoSo,xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
