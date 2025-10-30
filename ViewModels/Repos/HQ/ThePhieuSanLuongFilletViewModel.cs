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
    public partial class ThePhieuSanLuongFilletViewModel : ObservableObject
    {
        private static ThePhieuSanLuongFilletViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private ThePhieuSanLuongFillet? item;
        [ObservableProperty] private ObservableRangeCollection<ThePhieuSanLuongFillet> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private ThePhieuSanLuongFillet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThePhieuSanLuongFilletViewModel()
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

        public static ThePhieuSanLuongFilletViewModel Instance => instance ??= new ThePhieuSanLuongFilletViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        private NhanVienTheoBanViewModel VmNhanVienTheoBan => NhanVienTheoBanViewModel.Instance;

        public ThePhieuSanLuongFillet CopyItem(ThePhieuSanLuongFillet item)
        {
            return new ThePhieuSanLuongFillet
            {
                Id = item.Id,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaLo = item.MaLo,
                CaTra = item.CaTra,
                MaXuong = item.MaXuong,
                STT_PC = item.STT_PC,
                MayCan_PC = item.MayCan_PC,
                MaThe = item.MaThe,
                TrongLuongTare = item.TrongLuongTare,
                MaBan = item.MaBan,
                MaNhanVien = item.MaSize,
                IsDone = item.IsDone,
                STT_PC_BTP = item.STT_PC_BTP,
                MayCan_PC_BTP = item.MayCan_PC_BTP,
                TrongLuongNhan = item.TrongLuongNhan,
            };
        }
        public ThePhieuSanLuongFillet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }
        private string CreateId(string maXuong)
        {
            var header = $@"{VmApp.DateTimeNow:yyyyMMdd}.{VmApp.PCName}.";
            //var maxId = Items.Where(x =>
            //        int.TryParse(x.Id, out var rl) && x.Ngay == VmApp.DateTimeNow.Date && x.MayCan_PC == VmApp.PCName &&
            //        x.MaXuong == XiNghiepViewModel.Ins.XiNghiepSelectedItem.Ma)
            //    .Select(x => x.Id)
            //    .DefaultIfEmpty($@"{header}{"0000000"}")
            //    .Max();
            //var stt = int.Parse(maxId.Substring(header.Length, 7)) + 1;
            var id = $"{header}.{maXuong}.{DateTime.Now.TimeOfDay.ToString(@"hhmmssffffff")}";
            return id;
        }
        private string CreateId()
        {
            var header = $@"{VmApp.DateTimeNow:yyyyMMdd}.{VmApp.PCName}.";
            //var maxId = Items.Where(x =>
            //        int.TryParse(x.Id, out var rl) && x.Ngay == VmApp.DateTimeNow.Date && x.MayCan_PC == VmApp.PCName &&
            //        x.MaXuong == XiNghiepViewModel.Ins.XiNghiepSelectedItem.Ma)
            //    .Select(x => x.Id)
            //    .DefaultIfEmpty($@"{header}{"0000000"}")
            //    .Max();
            //var stt = int.Parse(maxId.Substring(header.Length, 7)) + 1;
            var id = $"{header}.{XiNghiepViewModel.Instance.XiNghiepSelectedItem.Ma}.{DateTime.Now.TimeOfDay.ToString(@"hhmmssffffff")}";
            return id;
        }
        public ThePhieuSanLuongFillet CreateDefaultNew()
        {
            var id = CreateId();

            return new ThePhieuSanLuongFillet
            {
                Id = id,
                IsDone = false,
                //CaTra = AppViewModel.Instance.IsCaTra,
                Gio = DateTime.Now.TimeOfDay,
               // MaLo = AppViewModel.Instance.SelectedItemWithSize.Item2,
                MaLoaiCa = LoaiCaFilletViewModel.Instance.SelectedItem?.Ma,
                MaMau = MauFilletViewModel.Instance.SelectedItem?.Ma,
                MaSize = SizeFilletViewModel.Instance.SelectedItem?.Ma,
                MaThanhPham = ThanhPhamFilletViewModel.Instance.SelectedItem?.Ma,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                Ngay = VmApp.DateTimeNow.Date,
               // TrongLuongTare = VmApp.TrongLuongTareActual,
                MayCan_PC = VmApp.PCName,
                TrongLuongNhan = 0
            };
        }
        public ThePhieuSanLuongFillet CreateNew(PhieuCanTPFilletv2 phieuCan)
        {
            var id = CreateId(phieuCan.MaXuong);
            return new ThePhieuSanLuongFillet
            {
                Id = id,
                IsDone = false,
                CaTra = phieuCan.CaTra,
                MaLoaiCa = phieuCan.MaLoaiCa,
                MaMau = phieuCan.MaMau,
                MaSize = phieuCan.MaSize,
                MaThanhPham = phieuCan.MaThanhPham,
                MaXuong = phieuCan.MaXuong,
                Ngay = phieuCan.Ngay,
                TrongLuongTare = phieuCan.TrongLuongTare,
                MayCan_PC = phieuCan.MaMayCan,
                MayCan_PC_BTP = phieuCan.MaMayCanBTP??"",
                Gio = DateTime.Now.TimeOfDay,
                MaLo = phieuCan.MaLo,
                STT_PC_BTP = phieuCan.STTBTP ?? 0,
                STT_PC = phieuCan.STT,
                MaThe = phieuCan.MaThe,
                MaNhanVien = phieuCan.MaNhanVien,
                MaBan = VmNhanVienTheoBan.GetBanId(phieuCan.MaNhanVien,phieuCan.MaXuong)??"",
                TrongLuongNhan = phieuCan.TrongLuongNhan,
                IdIn = phieuCan.IdIn,
                GhiChu = ""
            };
        }
        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.ThePhieuSanLuongFillet();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(ThePhieuSanLuongFillet item)
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
            var dao = new Dao.Repos.HQ.ThePhieuSanLuongFillet();
            return dao.Gets<T>();
        }
        public List<T> GetsLastByBan<T>(DateTime dateTime, string banId)
        {;
            var dao =  new Dao.Repos.HQ.ThePhieuSanLuongFillet();
            return dao.GetsLastByBan<T>(dateTime, banId);
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.ThePhieuSanLuongFillet();
            return dao.Insert(item);
        }
        public int SetTheIsDoneByBan(DateTime dateTime, string banId, bool isDone)
        {
            var dao = new Dao.Repos.HQ.ThePhieuSanLuongFillet();
            return dao.SetTheIsDoneByBan(dateTime, banId, isDone);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(ThePhieuSanLuongFillet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<ThePhieuSanLuongFillet>();
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
        private bool IsItemPass(ThePhieuSanLuongFillet item)
        {
            return item != null ;
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

            var items = Gets<ThePhieuSanLuongFillet>();
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
        private void Reload_(ObservableRangeCollection<ThePhieuSanLuongFillet> obj)
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
            var dao = new Dao.Repos.HQ.ThePhieuSanLuongFillet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(ThePhieuSanLuongFillet item)
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
        public ThePhieuSanLuongFillet? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Id == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(ThePhieuSanLuongFillet item)
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
