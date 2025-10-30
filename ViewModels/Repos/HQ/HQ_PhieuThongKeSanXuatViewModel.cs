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
using System.Globalization;
using Models.Repos;

namespace ViewModels.Repos.HQ
{
    public partial class HQ_PhieuThongKeSanXuatViewModel : ObservableObject
    {
        private static HQ_PhieuThongKeSanXuatViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_PhieuThongKeSanXuat? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_PhieuThongKeSanXuat> items = new();
        [ObservableProperty] private ObservableRangeCollection<HQ_PhieuThongKeSanXuat> usedItems = new();
        [ObservableProperty] private HQ_PhieuThongKeSanXuat? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;

        private HQ_PhieuThongKeSanXuatViewModel()
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
        public static HQ_PhieuThongKeSanXuatViewModel Instance => instance ??= new HQ_PhieuThongKeSanXuatViewModel();
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public HQ_PhieuThongKeSanXuat CopyItem(HQ_PhieuThongKeSanXuat item)
        {
            return new HQ_PhieuThongKeSanXuat
            {
                Id = item.Id,
                Ngay = item.Ngay,
                SoChungTu = item.SoChungTu,
                Ca = item.Ca,
                MaTo = item.MaTo,
                TenTo = item.TenTo,
                MaNhanVien = item.MaNhanVien,
                TenNhanVien = item.TenNhanVien,
                MaCongViec = item.MaCongViec,
                TenCongViec = item.TenCongViec,
                SanLuong = item.SanLuong,
                GioBatDau = item.GioBatDau,
                GioKetThuc = item.GioKetThuc,
                NgayGioTao = item.NgayGioTao
            };
        }

        public List<T> Gets<T>(DateTime dateTime, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuThongKeSanXuat(connStr);
            return dao.Gets<T>(dateTime);
        }

        public int Insert<T>(T item, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuThongKeSanXuat(connStr);
            return dao.Insert(item);
        }
        
        public int Insert(List<HQ_PhieuThongKeSanXuat> phieuThongKeSanXuats, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.HQ_PhieuThongKeSanXuat(connStr);
                var rows = dao.Insert(phieuThongKeSanXuats);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Delete(List<HQ_PhieuThongKeSanXuat> items,DateTime ngay, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuThongKeSanXuat(connStr);
            return dao.DeleteList(items,ngay);
        }
    }
}
