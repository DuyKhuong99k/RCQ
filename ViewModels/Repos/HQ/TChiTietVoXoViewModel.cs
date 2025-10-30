using AppModels;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModels.Repos.HQ
{
    public partial class TChiTietVoXoViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private static TChiTietVoXoViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private T_ChiTietVoXo? item;
        [ObservableProperty] private ObservableRangeCollection<T_ChiTietVoXo> items = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private T_ChiTietVoXo? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private TChiTietVoXoViewModel()
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
        public static TChiTietVoXoViewModel Instance => instance ??= new TChiTietVoXoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public T_ChiTietVoXo CopyItem(T_ChiTietVoXo item)
        {
            return new T_ChiTietVoXo
            {
                Id = item.Id,
                MaPhieuPhanCo = item.MaPhieuPhanCo,
                VoXo = item.VoXo,
                MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                MaThanhPham = item.MaThanhPham,
                MaCongDoan = item.MaCongDoan,
                MaSize = item.MaSize,
                MaQuyTrinh = item.MaQuyTrinh,
                NgayGio = item.NgayGio,
                UserName = item.UserName,
                PCName = item.PCName,
                MaSizeVoXo = item.MaSizeVoXo,
            };
        }
        public T_ChiTietVoXo CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public T_ChiTietVoXo CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Id, out var rl))
                   .Select(x => int.Parse(x.Id))
                   .DefaultIfEmpty(0)
                   .Max();
            //var vmSanPham = PMSMSharedv1.ViewModel.T_SanPhamViewModel.Ins;
            //var vmBon = PMSMSharedv1.ViewModel.T_BonViewModel.Ins;
            var id = $"{(maxId + 1).ToString("000000000")}";
            return new T_ChiTietVoXo
            {
                Id = id,
                NgayGio = DateTime.Now,
                //UserName = AppViewModel.Instance.UserName,
                //Pcname = AppViewModel.Instance.PCName
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TChiTietVoXo();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(T_ChiTietVoXo item)
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
            var dao = new Dao.Repos.HQ.TChiTietVoXo();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TChiTietVoXo();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(T_ChiTietVoXo item)
        {
            try
            {
                if (Insert(item) > 0)
                {
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
        private bool IsItemPass(T_ChiTietVoXo item)
        {
            return Item != null &&
                 Item.Id != null &&
                 Item.Id.Trim() != "" &&
                 Item.VoXo != 0;
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
        private void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<T_ChiTietVoXo>();
            if (items.Any())
                lock (Items)
                {
                    Items.AddRange(items);
                }


        }

        [RelayCommand]
        private void Reload_()
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
            var dao = new Dao.Repos.HQ.TChiTietVoXo();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(T_ChiTietVoXo item)
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
        public T_ChiTietVoXo? Find(string ma)
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
    }
}
