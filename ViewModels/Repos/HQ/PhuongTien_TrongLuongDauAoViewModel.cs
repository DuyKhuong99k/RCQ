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
    public partial class PhuongTien_TrongLuongDauAoViewModel : ObservableObject
    {
        private static PhuongTien_TrongLuongDauAoViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhuongTien_TrongLuongDauAo? item;
        [ObservableProperty] private ObservableRangeCollection<PhuongTien_TrongLuongDauAo> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhuongTien_TrongLuongDauAo? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhuongTien_TrongLuongDauAoViewModel()
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

        public static PhuongTien_TrongLuongDauAoViewModel Instance => instance ??= new PhuongTien_TrongLuongDauAoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhuongTien_TrongLuongDauAo CopyItem(PhuongTien_TrongLuongDauAo item)
        {
            return new PhuongTien_TrongLuongDauAo
            {
                CaNgayTruoc = item.CaNgayTruoc,
                CaManh = item.CaManh,
                CaNgopAoGhe = item.CaNgopAoGhe,
                CaNgopAoXe = item.CaNgopAoXe,
                MaAo = item.MaAo,
                MaPhuongTien = item.MaPhuongTien,
                Ngay = item.Ngay,
                TongHam = item.TongHam,
                ApTai = item.ApTai,
                STTChuyen = item.STTChuyen,
                ThuKy = item.ThuKy,
                CaConLai = item.CaConLai,
                TyLeMoi = item.TyLeMoi,
                GhiChu = item.GhiChu,
                CreateBy = item.CreateBy,
                CreateDateTime = item.CreateDateTime,
                GioXuatPhat = item.GioXuatPhat,
                ModifiedBy = item.ModifiedBy,
                ModifiedDateTime = item.ModifiedDateTime,
                NgayBatCa = item.NgayBatCa,
                NgayXuatPhat = item.NgayXuatPhat,
                IsVungNuoiBlocked = item.IsVungNuoiBlocked,
                MaNhaCungCap = item.MaNhaCungCap,
                CaNgopAoBanNgoai = item.CaNgopAoBanNgoai,
                Chuyen = item.Chuyen
            };
        }
        public PhuongTien_TrongLuongDauAo CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhuongTien_TrongLuongDauAo CreateDefaultNew()
        {
            return new PhuongTien_TrongLuongDauAo
            {
                Ngay = AppViewModel.Instance.DateTimeNow,
                CreateDateTime = DateTime.Now,
                //CreateBy = AppViewModel.Instance.UserName,
                //ModifiedBy = AppViewModel.Instance.UserName,
                ModifiedDateTime = DateTime.Now,
                CaConLai = 0,
                CaManh = 0,
                CaNgayTruoc = 0,
                CaNgopAoGhe = 0,
                CaNgopAoXe = 0,
                NgayBatCa = AppViewModel.Instance.DateTimeNow,
                GioXuatPhat = DateTime.Now.TimeOfDay,
                NgayXuatPhat = AppViewModel.Instance.DateTimeNow,
                STTChuyen = 0,
                TongHam = 0,
                TyLeMoi = 0,
                CaNgopAoBanNgoai = 0,
                Chuyen = 1
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhuongTien_TrongLuongDauAo item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaPhuongTien == item.MaPhuongTien);
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
            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhuongTien_TrongLuongDauAo item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhuongTien_TrongLuongDauAo>();
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
        private bool IsItemPass(PhuongTien_TrongLuongDauAo item)
        {
            return item.MaPhuongTien != null &&
                 item.MaPhuongTien.Trim() != string.Empty &&
                 item.Ngay != null &&
                 item.MaAo != null &&
                 item.MaAo.Trim() != string.Empty &&
                 item.CaManh >= 0 &&
                 item.CaNgopAoGhe >= 0 &&
                 item.CaNgopAoXe >= 0 &&
                 item.TongHam >= 0 &&
                 item.CaNgayTruoc >= 0 &&
                 item.CaConLai >= 0 &&
                 item.STTChuyen >= 0 &&
                 SelectedItem != null &&
                 item.MaNhaCungCap != null &&
                 item.MaNhaCungCap.Trim() != string.Empty;
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

            var items = Gets<PhuongTien_TrongLuongDauAo>();
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
        private void Reload_(ObservableRangeCollection<PhuongTien_TrongLuongDauAo> obj)
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
            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhuongTien_TrongLuongDauAo item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaPhuongTien == item.MaPhuongTien);
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
                        var _item = Items.SingleOrDefault(x => x.MaPhuongTien == Item.MaPhuongTien);
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
        public PhuongTien_TrongLuongDauAo? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaPhuongTien == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(PhuongTien_TrongLuongDauAo item)
        {
            try
            {

                return Find(item.MaPhuongTien) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<string> GetThuKys()
        {
            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.GetThuKys<string>();
        }

        public List<string> GetApTais()
        {
            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.GetApTais<string>();
        }
        public List<int> GetTais()
        {
            return new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public List<T> Gets_NgayBatCa<T>(DateTime dateTime)
        {
            //var connectionString = SettingViewModel.Ins.ConnectionString;
            //if (isServer) connectionString = SettingViewModel.Ins.ConnectionStringRP;

            //if (isOnline) connectionString = ConnectionStringOnline;

            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.Gets_NgayBatCa<T>(dateTime);
        }

        public List<T> Gets_NgayNX<T>(DateTime dateTime)
        {
            //var connectionString = SettingViewModel.Ins.ConnectionString;
            //if (isServer) connectionString = SettingViewModel.Ins.ConnectionStringRP;

            //if (isOnline) connectionString = ConnectionStringOnline;

            var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
            return dao.Gets_NgayNX<T>(dateTime);
        }
        public PhuongTien_TrongLuongDauAo Find<T>(string maPhuongTien, DateTime ngay, string maAo, string maNhaCungCap,int chuyen)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhuongTien_TrongLuongDauAo();
                return dao.Get(maPhuongTien,ngay,maAo,maNhaCungCap,chuyen);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
