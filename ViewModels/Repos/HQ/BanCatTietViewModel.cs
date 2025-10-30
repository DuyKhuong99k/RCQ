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
using System.Collections.ObjectModel;

namespace ViewModels.Repos.HQ
{
    public partial class BanCatTietViewModel : ObservableObject
    {
        private static BanCatTietViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private BanCatTiet? item;
        [ObservableProperty] private ObservableRangeCollection<BanCatTiet> items = new();
        [ObservableProperty] private ObservableCollection<BanCatTiet> _banCatTiets = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private BanCatTiet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private BanCatTietViewModel()
        {
            try
            {
                Reload();
                BanCatTiets = new ObservableCollection<BanCatTiet>(Get());


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static BanCatTietViewModel Instance => instance ??= new BanCatTietViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public BanCatTiet CopyItem(BanCatTiet item)
        {
            return new BanCatTiet
            {
                Ma = item.Ma,
                SuDung = item.SuDung,
                Ten = item.Ten,
                ColSpanX1 = item.ColSpanX1,
                IsShowX1 = item.IsShowX1,
                X1 = item.X1,
                X2 = item.X2,
                Y1 = item.Y1,
                Y2 = item.Y2,
                ColSpanX2 = item.ColSpanX2,
                IsShowX2 = item.IsShowX2,
                DoUuTienX1 = item.DoUuTienX1,
                DoUuTienX2 = item.DoUuTienX2,
                IsDetect = item.IsDetect,
                IsFalse = item.IsFalse,
                IsKhoaX1 = item.IsKhoaX1,
                IsKhoaX2 = item.IsKhoaX2,
                SoLanChiaCaX1 = item.SoLanChiaCaX1,
                SoLanChiaCaX2 = item.SoLanChiaCaX2,
                ThoiGianQuangDuongPheu1X1 = item.ThoiGianQuangDuongPheu1X1,
                ThoiGianQuangDuongPheu1X2 = item.ThoiGianQuangDuongPheu1X2,
                ThoiGianQuangDuongPheu2X1 = item.ThoiGianQuangDuongPheu2X1,
                ThoiGianQuangDuongPheu2X2 = item.ThoiGianQuangDuongPheu2X2,
                PlcAdr = item.PlcAdr,
                PlcValue = item.PlcValue,
                TimeOpen = item.TimeOpen,
                VongChiaCa = item.VongChiaCa,
                PlcYadr = item.PlcYadr,
                PlcYValue = item.PlcYValue,
                ViTri_HX1 = item.ViTri_HX1,
                ViTri_HX2 = item.ViTri_HX2,
                ThoiGianNhanCaX1 = item.ThoiGianNhanCaX1,
                ThoiGianNhanCaX2 = item.ThoiGianNhanCaX2,
                PlcOffAdr = item.PlcOffAdr,
                PlcOffValue = item.PlcOffValue,
                IsCaMuoiX1 = item.IsCaMuoiX1,
                IsCaMuoiX2 = item.IsCaMuoiX2,


            };
        }
        public BanCatTiet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public BanCatTiet CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
                   .Select(x => int.Parse(x.Ma))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new BanCatTiet
            {
                SuDung = true,
                Ma = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BanCatTiet();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(BanCatTiet item)
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
            var dao = new Dao.Repos.HQ.BanCatTiet();
            return dao.Gets<T>();
        }
        public List<BanCatTiet> Get()
        {
            try
            {
                var dao = new Dao.Repos.HQ.BanCatTiet();
                return dao.Gets<BanCatTiet>(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BanCatTiet();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(BanCatTiet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<BanCatTiet>();
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
        private bool IsItemPass(BanCatTiet item)
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
            }

            var items = Gets<BanCatTiet>();
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
        private void Reload_(ObservableRangeCollection<BanCatTiet> obj)
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
            var dao = new Dao.Repos.HQ.BanCatTiet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(BanCatTiet item)
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
        public BanCatTiet? Find(string ma)
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

        public bool Exists(string ma)
        {
            try
            {

                return Find(ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #region Tính Lương Phụ Fillet
        public List<NhanVienPhuTheoBanFillet> BanCatTietSelectChangedCommand(DateTime dateTime, string xuongId, string maBanCatTiet)
        {
            try
            {
                var nhanViens = NhanVienViewModel.Instance
                    .GetNhanViensPhuByBanCatTiet(maBanCatTiet, dateTime, xuongId);
                if (!nhanViens.Any())
                {
                    nhanViens = NhanVienViewModel.Instance
                        .GetNhanViensPhuByBanCatTiet(maBanCatTiet, new DateTime(2020, 03, 21), xuongId);
                    //MessageBox.Show("Đã tải danh sách cài đặt mặt định vui lòng nhấn lưu để xác nhận cho hôm nay");
                }

                nhanViens.All(
                    x =>
                    {
                        x.Ngay = dateTime;
                        return true;
                    });
                var NhanVienPhuBanCatTietFillets = new List<NhanVienPhuTheoBanFillet>(nhanViens);
                return NhanVienPhuBanCatTietFillets.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienPhuTheoBanFillet>();
                //MessageBox.Show(exception.Message);
                throw;
            }
        }
        public List<BanCatTiet> Get(string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BanCatTiet(connStr);
                return dao.Gets<BanCatTiet>(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
