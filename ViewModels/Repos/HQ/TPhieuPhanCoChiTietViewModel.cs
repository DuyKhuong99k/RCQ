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
    public partial class TPhieuPhanCoChiTietViewModel : ObservableObject
    {
        private static TPhieuPhanCoChiTietViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private T_PhieuPhanCoChiTiet? item;
        [ObservableProperty] private ObservableRangeCollection<T_PhieuPhanCoChiTiet> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private T_PhieuPhanCoChiTiet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private TPhieuPhanCoChiTietViewModel()
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

        public static TPhieuPhanCoChiTietViewModel Instance => instance ??= new TPhieuPhanCoChiTietViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public T_PhieuPhanCoChiTiet CopyItem(T_PhieuPhanCoChiTiet item)
        {
            return new T_PhieuPhanCoChiTiet
            {
                GhiChu = item.GhiChu,
                GramDau = item.GramDau,
                GramCuoi = item.GramCuoi,
                VoXo = item.VoXo,
                TyLe = item.TyLe,
                MaKhachHang = item.MaKhachHang,
                MaKhangSinh = item.MaKhangSinh,
                MaPhieuPhanCo = item.MaPhieuPhanCo,
                MaPhuGia = item.MaPhuGia,
                MaQuyTrinh = item.MaQuyTrinh,
                MaSize = item.MaSize,
                MaThongTinPhu = item.MaThongTinPhu,
                MaTrangThaiNguyenLieu = item.MaTrangThaiNguyenLieu,
                STT = item.STT,
                MaCongDoan = item.MaCongDoan
            };
        }
        public T_PhieuPhanCoChiTiet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public T_PhieuPhanCoChiTiet CreateDefaultNew()
        {
            var vmPhieuPhanCo = TPhieuPhanCoViewModel.Instance;
            var stt = Items.Select(x => x.STT).DefaultIfEmpty(0).Max() + 1;
            return new T_PhieuPhanCoChiTiet
            {
                STT = stt,
                MaPhieuPhanCo = vmPhieuPhanCo.SelectedItem?.Ma,
                TyLe = 0,
                VoXo = 0,
                GramCuoi = 0,
                GramDau = 0
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TPhieuPhanCoChiTiet();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(T_PhieuPhanCoChiTiet item)
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
            var dao = new Dao.Repos.HQ.TPhieuPhanCoChiTiet();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TPhieuPhanCoChiTiet();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(T_PhieuPhanCoChiTiet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<T_PhieuPhanCoChiTiet>();
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
        private bool IsItemPass(T_PhieuPhanCoChiTiet item)
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

            var items = Gets<T_PhieuPhanCoChiTiet>();
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
        private void Reload_(ObservableRangeCollection<T_PhieuPhanCoChiTiet> obj)
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
            var dao = new Dao.Repos.HQ.TPhieuPhanCoChiTiet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(T_PhieuPhanCoChiTiet item)
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
        public T_PhieuPhanCoChiTiet? Find(int stt)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == stt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(T_PhieuPhanCoChiTiet item)
        {
            try
            {

                return Find(item.STT) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
