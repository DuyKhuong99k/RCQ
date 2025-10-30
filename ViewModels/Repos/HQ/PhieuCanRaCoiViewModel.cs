using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ
{
    public partial class PhieuCanRaCoiViewModel : ObservableObject
    {
        private static PhieuCanRaCoiViewModel instance;
        public static PhieuCanRaCoiViewModel Instance => instance ??= new();
        private PhieuCanRaCoiViewModel()
        {
            
        }
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanRaCoi? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanRaCoi> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanRaCoi? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanRaCoi CopyItem(PhieuCanRaCoi item)
        {
            return new PhieuCanRaCoi
            {
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                Ngay = item.Ngay,
                STT = item.STT,
                TrongLuong = item.TrongLuong,
                TrongLuongTare = item.TrongLuongTare,
                MaThe = item.MaThe,
                MaNhanVien = item.MaNhanVien,
                MaChatLuong = item.MaChatLuong,
                IdMonitor = item.IdMonitor,
                Id = item.Id,
                MaXuong = item.MaXuong,
                MayCan = item.MayCan,
                NgayNguyenLieu = item.NgayNguyenLieu,
                MaLo = item.MaLo,
                MaThanhPham = item.MaThanhPham,
                MaSize = item.MaSize,
                MaChieuXa = item.MaChieuXa,
                MaCoi = item.MaCoi,
                

            };
        }
        public PhieuCanRaCoi CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanRaCoi CreateDefaultNew()
        {
            
            return new PhieuCanRaCoi
            {
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                Gio = DateTime.Now.TimeOfDay
            };
        }

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanRaCoi();
            return dao.GetsLast<T>(dateTime, num);
        }
        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanRaCoi();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanRaCoi item)
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
            var dao = new Dao.Repos.HQ.PhieuCanRaCoi();
            return dao.Gets<T>();
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanRaCoi();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanRaCoi item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanRaCoi>();
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
        private bool IsItemPass(PhieuCanRaCoi item)
        {
            return item != null &&
                 item.Gio != null &&
                 item.Ngay != null &&
                 item.TrongLuongTare >= 0 &&
                 item.TrongLuong >= 0 &&
                 !string.IsNullOrEmpty(item.MaCoi) &&
                 !string.IsNullOrEmpty(item.MaChatLuong) &&
                 !string.IsNullOrEmpty(item.MaChieuXa) &&
                 !string.IsNullOrEmpty(item.MaLo) &&
                 !string.IsNullOrEmpty(item.MaSize) &&
                 !string.IsNullOrEmpty(item.MaThanhPham) &&
                 !string.IsNullOrEmpty(item.MaThe) &&
                 !string.IsNullOrEmpty(item.MaNhanVien) &&
                 !string.IsNullOrEmpty(item.MaXuong) &&
                 !string.IsNullOrEmpty(item.MayCan) &&
                 item.NgayNguyenLieu != null &&
                 !string.IsNullOrEmpty(item.IdMonitor) &&
                 !string.IsNullOrEmpty(item.Id);
                 ;
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

            var items = Gets<PhieuCanRaCoi>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanRaCoi> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanRaCoi();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanRaCoi item)
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
        public PhieuCanRaCoi? Find(int ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(int ma)
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
    }
}
