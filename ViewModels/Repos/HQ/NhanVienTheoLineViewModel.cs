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
    public partial class NhanVienTheoLineViewModel : ObservableObject
    {
        private static NhanVienTheoLineViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NhanVienTheoLine? item;
        [ObservableProperty] private ObservableRangeCollection<NhanVienTheoLine> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NhanVienTheoLine? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NhanVienTheoLineViewModel()
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

        public static NhanVienTheoLineViewModel Instance => instance ??= new NhanVienTheoLineViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetsFullField<T>(DateTime ngay, string maLine, string maViTri)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoLine();
            return dao.GetNhanVienTheoLineViTri<T>(ngay, maLine, maViTri);
        }
        public List<T> GetsFullFieldLastNew<T>(DateTime ngay, string maLine, string maViTri)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoLine();
            return dao.GetNhanVienTheoLineViTriMoiNhat<T>(ngay, maLine, maViTri);
        }


        public NhanVienTheoLine CopyItem(NhanVienTheoLine item)
        {
            return new NhanVienTheoLine
            {
                Id = item.Id,
                MaNhanVien = item.MaNhanVien,
                MaLine = item.MaLine,
                MaViTri = item.MaViTri,
                Ngay = item.Ngay,
                Gio = item.Gio,
                CodeId = item.CodeId
            };
        }
        public NhanVienTheoLine CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NhanVienTheoLine CreateDefaultNew()
        {
            var maxId = Items
                .Select(x => x.Id)
                .DefaultIfEmpty(0)
                .Max();
            //var id = $"{(maxId + 1).ToString("000000")}";
            var id = maxId + 1;
            return new NhanVienTheoLine
            {
                Id = id,
                CodeId = XiNghiepViewModel.Instance.SelectedItem?.CodeId,
                Ngay = AppViewModel.Instance.DateTimeNow,
                Gio = AppViewModel.Instance.DateTimeNow.TimeOfDay
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoLine();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NhanVienTheoLine item)
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
            var dao = new Dao.Repos.HQ.NhanVienTheoLine();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoLine();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NhanVienTheoLine item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NhanVienTheoLine>();
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
        private bool IsItemPass(NhanVienTheoLine item)
        {
            return item != null &&
                 item.Id != 0 &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != "" &&
                 item.MaLine != null &&
                 item.MaLine.Trim() != "" &&
                 item.MaViTri != null &&
                 item.MaViTri.Trim() != "" &&
                 item.Ngay != null &&
                 item.Gio != null;
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

            var items = Gets<NhanVienTheoLine>();
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
        private void Reload_(ObservableRangeCollection<NhanVienTheoLine> obj)
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
            var dao = new Dao.Repos.HQ.NhanVienTheoLine();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NhanVienTheoLine item)
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
        public NhanVienTheoLine? Find(int ma)
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

        public bool Exists(NhanVienTheoLine item)
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
