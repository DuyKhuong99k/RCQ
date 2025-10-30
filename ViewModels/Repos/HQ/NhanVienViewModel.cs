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
using BravoModelV1.Model;
using Models.Repos.AppModel;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ViewModels.Repos.HQ
{
    public partial class NhanVienViewModel : ObservableObject
    {
        private static NhanVienViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NhanVienDaiThanh? item;
        [ObservableProperty] private ObservableRangeCollection<NhanVienDaiThanh> items = new();
        [ObservableProperty] private ObservableRangeCollection<NhanVienDaiThanh> _nhanVienDaiThanhs = new();
        [ObservableProperty] private ObservableCollection<NhanVienDaiThanh> _nhanVienDaiThanhsTo = new();
        [ObservableProperty] private ObservableCollection<NhanVienDaiThanh> _nhanViens = new();
        [ObservableProperty] private ObservableCollection<BravoModelV1.Model.NhanVienKiem> _nhanViensKiems = new();
        [ObservableProperty] private ObservableCollection<BravoModelV1.Model.SanLuongNhanVienKiem> _sanLuongNhanVienKiems = new();
        [ObservableProperty] private ObservableCollection<BravoModelV1.Model.SanLuongNhanVienSoChe> _sanLuongNhanVienSoChes = new();

        [ObservableProperty] private ObservableRangeCollection<string> nhoms = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NhanVienDaiThanh? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        [ObservableProperty] private bool _isPhucVuCaNhan = true;

        private readonly SynchronizationContext synchronizationContext;
        private NhanVienViewModel()
        {
            try
            {
                Reload();
                NhanViens = new ObservableCollection<NhanVienDaiThanh>(GetNhanViens());


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static NhanVienViewModel Instance => instance ??= new NhanVienViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<NhanVienDaiThanh> GetNhanViens(string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh();
                return dao.GetNhanVienDaiThanhs();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void GetNhoms(ObservableRangeCollection<NhanVienDaiThanh> items)
        {
            var _items = items.Select(x => x.DeptName0).Distinct().OrderBy(x => x).ToList();

            if (_items.Any())
            {
                Nhoms.Clear();
                foreach (var item in _items)
                {
                    Nhoms.Add(item);
                }

                //Nhoms.AddRange(_items);
            }
        }

        public NhanVienDaiThanh CopyItem(NhanVienDaiThanh item)
        {
            return new NhanVienDaiThanh
            {
                MaNhanVien = item.MaNhanVien,
                Address = item.Address,
                BirthDate = item.BirthDate,
                ChucVu = item.ChucVu,
                DeptCode0 = item.DeptCode0,
                DeptName0 = item.DeptName0,
                FirstWorkingDate = item.FirstWorkingDate == "HD" ? @"Đang Làm" : @"Nghĩ Việc",
                GenderName = item.GenderName,
                JobPositionName0 = item.JobPositionName0,
                MaChamCong = item.MaChamCong,
                MaHoSo = item.MaHoSo,
                Name = item.Name,
                Tel = item.Tel,
                Xuong = item.Xuong,
                IsContracting = item.FirstWorkingDate == "HD",
                IsPhucVu = item.IsPhucVu,
                IsShowDinhMuc = item.IsShowDinhMuc,
                IsHuman = item.IsHuman,
                IsGiaCong = item.IsGiaCong,
                AC = item.AC,
                IsChucNang = item.IsChucNang,
                LoaiSanLuong = item.LoaiSanLuong,
                IsBanKiem = item.IsBanKiem,
                IsNhanVienCat = item.IsNhanVienCat,
                IsNhom = item.IsNhom
            };
        }
        public NhanVienDaiThanh CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NhanVienDaiThanh CreateDefaultNew()
        {

            return new NhanVienDaiThanh
            {

            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienDaiThanh();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NhanVienDaiThanh item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
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
            var dao = new Dao.Repos.HQ.NhanVienDaiThanh();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienDaiThanh();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NhanVienDaiThanh item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NhanVienDaiThanh>();
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
        private bool IsItemPass(NhanVienDaiThanh item)
        {
            return item != null && item.MaNhanVien != null && item.MaNhanVien.Trim() != ""
                  ;
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

            var items = Gets<NhanVienDaiThanh>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach (var item in items)
                        {
                            Items.Add(item);
                            //chắt thêm
                            NhanVienDaiThanhs.Add(item);
                        }


                    }
                    catch (NotSupportedException e)
                    {

                    }

                }
            GetNhoms(Items);

        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<NhanVienDaiThanh> obj)
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
            var dao = new Dao.Repos.HQ.NhanVienDaiThanh();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NhanVienDaiThanh item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
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
                        var _item = Items.SingleOrDefault(x => x.MaNhanVien == Item.MaNhanVien);
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
        public NhanVienDaiThanh? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaNhanVien == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(NhanVienDaiThanh item)
        {
            try
            {

                return Find(item.MaNhanVien) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<ToKiem> GetToKiemsByXuongId(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.ToKiem(connStr);
                var items = dao.GetKiemsByXuongId(xuongId);
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<DanhSachToKiem> GetDanhSachToKiemsByXuongIdNgay(string xuongId, DateTime dateTime, int type, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DanhSachToKiem(connStr);
                var items = dao.GetDanhSachToKiemsByXuongIdNgay(xuongId, dateTime, type);
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public DateTime? GetMaxNgayKiemByXuongId(string xuongId, int type, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DanhSachToKiem(connStr);
                var item = dao.GetMaxDateByXuongId(xuongId, type);
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<NhanVienDaiThanh> GetDanhSachBanKiemMacDinh(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                var items = dao.GetNhanVienDaiThanhs(xuongId, "K");
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<NhanVienDaiThanh> GetNhanVienDaiThanhs(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                return dao.GetNhanVienDaiThanhs(xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<NhanVienDaiThanh> GetNhanVienDaiThanhs(IEnumerable<string> ids, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                return dao.GetNhanVienDaiThanhs(ids);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhanVienDaiThanh> GetNhanVienDaiThanhs(string xuongId, int startMaHoSo, int endMaHoSo, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                return dao.GetNhanVienDaiThanhs(xuongId, startMaHoSo, endMaHoSo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<NhanVienDaiThanh> GetNhanVienDaiThanhsNotIn(string xuongId, List<NhanVienDaiThanh> nhanVienDaiThanhs, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                return dao.GetNhanVienDaiThanhsNotIn(xuongId, nhanVienDaiThanhs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<BravoModelV1.Model.NhanVienKiem> GetDefaultNhanViensKiems(int numTo, string xuongId)
        {
            try
            {
                List<BravoModelV1.Model.NhanVienKiem> nhanViensKiems = new List<BravoModelV1.Model.NhanVienKiem>();
                var nhanViens = GetDanhSachBanKiemMacDinh(xuongId);
                if (nhanViens.Any())
                {
                    var numMemPerTo = nhanViens.Count / numTo;
                    for (int i = 0; i < numTo; i++)
                    {
                        for (int j = 0; j < numMemPerTo; j++)
                        {
                            var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                            {
                                ToId = $@"XKK{i + 1}",
                                Name = $@"XKK{i + 1}",
                                NhanVienDaiThanh = nhanViens[i * numMemPerTo + j],
                                SoGio = 8,
                                TyLe = 1,
                                TyLeTru = 0,
                            };
                            nhanViensKiems.Add(nhanVienKiem);
                        }

                        if (i == (numTo - 1))
                        {
                            for (int j = numTo * numMemPerTo; j < nhanViens.Count; j++)
                            {
                                var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                                {
                                    ToId = $@"XKK{i + 1}",
                                    Name = $@"XKK{i + 1}",
                                    NhanVienDaiThanh = nhanViens[j],
                                    SoGio = 8,
                                    TyLe = 1,
                                    TyLeTru = 0
                                };
                                nhanViensKiems.Add(nhanVienKiem);
                            }
                        }
                    }
                }

                return nhanViensKiems;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<BravoModelV1.Model.NhanVienKiem> GetDefaultNhanViensSanLuongKiems(string xuongId)
        {
            try
            {
                List<BravoModelV1.Model.NhanVienKiem> nhanViensKiems = new List<BravoModelV1.Model.NhanVienKiem>();
                var tos = GetToKiemsByXuongId(xuongId);
                if (xuongId == "1")
                {
                    try
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var nhanviens = new List<NhanVienDaiThanh>();
                            if (i == 0)
                            {
                                nhanviens = GetNhanVienDaiThanhs(xuongId, 1, 150);
                            }
                            else if (i == 1)
                            {
                                nhanviens = GetNhanVienDaiThanhs(xuongId, 151, 300);
                            }
                            else
                            {
                                var nhanviensNotIn = nhanViensKiems.Select(x => x.NhanVienDaiThanh).ToList();
                                nhanviens = GetNhanVienDaiThanhsNotIn(xuongId, nhanviensNotIn);
                            }

                            foreach (var nhanVienDaiThanh in nhanviens)
                            {
                                var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                                {
                                    ToId = tos[i].Ma,
                                    Name = tos[i].Ma,
                                    NhanVienDaiThanh = nhanVienDaiThanh
                                };
                                nhanViensKiems.Add(nhanVienKiem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                }
                else if (xuongId == "2")
                {
                    try
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            var nhanviens = GetNhanVienDaiThanhs(xuongId);
                            foreach (var nhanVienDaiThanh in nhanviens)
                            {
                                var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                                {
                                    ToId = tos[i].Ma,
                                    Name = tos[i].Ma,
                                    NhanVienDaiThanh = nhanVienDaiThanh
                                };
                                nhanViensKiems.Add(nhanVienKiem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                }

                return nhanViensKiems;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<BravoModelV1.Model.NhanVienKiem> GetNhanViensKiems(string xuongId, DateTime dateTime, int type)
        {
            try
            {
                var nhanVienKiems = new List<BravoModelV1.Model.NhanVienKiem>();
                var toKiems = GetToKiemsByXuongId(xuongId);
                var numTo = toKiems.Count;
                var danhSachKiems = GetDanhSachToKiemsByXuongIdNgay(xuongId, dateTime, type);
                var ngay = GetMaxNgayKiemByXuongId(xuongId, type);
                if (!danhSachKiems.Any() && (ngay == null || ngay != dateTime.Date))
                {
                    danhSachKiems = GetDanhSachToKiemsByXuongIdNgay(xuongId, new DateTime(2020, 03, 21), type);
                    if (!danhSachKiems.Any())
                    {
                        if (type == 0)
                        {
                            nhanVienKiems =
                                GetDefaultNhanViensKiems(numTo, xuongId);
                        }
                        else if (type == 1)
                        {
                            nhanVienKiems =
                                GetDefaultNhanViensSanLuongKiems(xuongId);
                        }
                    }
                    else
                    {
                        foreach (var danhSachToKiem in danhSachKiems)
                        {
                            var nhanVien = NhanVienDaiThanhs.SingleOrDefault(x => x.MaNhanVien == danhSachToKiem.MaNhanVien);
                            var item = new BravoModelV1.Model.NhanVienKiem()
                            {
                                ToId = danhSachToKiem.MaToKiem,
                                Name = danhSachToKiem.MaToKiem,
                                SoGio = danhSachToKiem.SoGio,
                                TyLe = danhSachToKiem.TyLe,
                                TyLeTru = danhSachToKiem.TyLeTru,
                                NhanVienDaiThanh = nhanVien
                            };
                            nhanVienKiems.Add(item);
                        }
                    }
                }
                else
                {
                    foreach (var danhSachToKiem in danhSachKiems)
                    {
                        var nhanVien = NhanVienDaiThanhs.SingleOrDefault(x => x.MaNhanVien == danhSachToKiem.MaNhanVien);
                        var item = new BravoModelV1.Model.NhanVienKiem()
                        {
                            ToId = danhSachToKiem.MaToKiem,
                            Name = danhSachToKiem.MaToKiem,
                            SoGio = danhSachToKiem.SoGio,
                            TyLe = danhSachToKiem.TyLe,
                            TyLeTru = danhSachToKiem.TyLeTru,
                            NhanVienDaiThanh = nhanVien
                        };
                        nhanVienKiems.Add(item);
                    }
                }

                //nhanVienKiems.Add;
                return nhanVienKiems;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<BravoModelV1.Model.SanLuongNhanVienKiem> GetSanLuongNhanVienKiems(List<BravoModelV1.Model.NhanVienKiem> nhanViensKiems)
        {
            try
            {
                var items = new List<BravoModelV1.Model.SanLuongNhanVienKiem>();
                foreach (var nhanViensKiem in nhanViensKiems)
                {
                    var item = new BravoModelV1.Model.SanLuongNhanVienKiem()
                    {
                        NhanVienKiem = nhanViensKiem,
                        PhanTramHuong = (decimal)nhanViensKiem.TyLe,
                        SoGio = (decimal)nhanViensKiem.SoGio,
                        TrongLuongHuong = 0,
                        TrongLuongPhanChia = 0,
                        TrongLuongTrenGio = 0,
                        TyLeTru = (decimal)nhanViensKiem.TyLeTru
                    };
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetNhomKiems(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.ToKiem(connStr);
                var items = dao.GetKiemsByXuongId(xuongId).Select(x => x.Ma).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetNhomKiemsWithType2(string xuongId, string? connStr = null)
        {
            try
            {
                var items = new List<string>() { "DV001" };
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int InsertToKiems(List<DanhSachToKiem> danhSachToKiems, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DanhSachToKiem(connStr);
                return dao.Insert<DanhSachToKiem>(danhSachToKiems);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int InsertToKiem(DanhSachToKiem danhSachToKiem, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DanhSachToKiem(connStr);
                return dao.Insert<DanhSachToKiem>(danhSachToKiem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int UpdateToKiems(List<DanhSachToKiem> danhSachToKiems, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DanhSachToKiem(connStr);
                return dao.Update<DanhSachToKiem>(danhSachToKiems);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int DeleteToKiems(List<DanhSachToKiem> danhSachToKiems, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DanhSachToKiem(connStr);
                return dao.Delete<DanhSachToKiem>(danhSachToKiems);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<SanLuongKiemNhom> GetSanLuongKiemNhoms(
            List<BravoModelV1.Model.NhanVienKiem> nhanViensKiems,
            List<BravoModelV1.Model.NhanVienKiem> nhanViensKiemsSanLuong,
            DateTime dateTime,
            string xuongId
            , string? connStr = null)
        {
            try
            {
                var sanLuongKiemNhoms = new List<SanLuongKiemNhom>();
                var nhomKiems = nhanViensKiemsSanLuong.Select(x => x.ToId).Distinct().ToList();
                foreach (var nhomKiem in nhomKiems)
                {
                    var nhanviens = nhanViensKiemsSanLuong.Where(x => x.ToId == nhomKiem)
                        .Select(x => x.NhanVienDaiThanh)
                        .Distinct()
                        .ToList();
                    var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
                    var thanhPhamExDao = new Dao.Repos.HQ.MaThanhPhamExDinhHinh(connStr);
                    var thanhPhamExDinhHinhs = thanhPhamExDao.GetMaThanhPhamExDinhHinhs();
                    var sanLuong = dao.GetSanLuongTras(dateTime, xuongId, nhanviens, thanhPhamExDinhHinhs);
                    var soGio = nhanViensKiems.Where(x => x.ToId == nhomKiem).Select(x => x.SoGio * x.TyLe).Sum();
                    var sanLuongTrenGio = soGio == 0 ? 0 : sanLuong / (decimal)soGio;
                    var sanLuongKiemNhom = new SanLuongKiemNhom()
                    {
                        ToKiem = new ToKiem() { Ma = nhomKiem, Name = nhomKiem, MaXuong = xuongId },
                        TongSanLuong = sanLuong,
                        TrongLuongTrenGio = sanLuongTrenGio,
                        TongGio = (decimal)soGio
                    };
                    sanLuongKiemNhoms.Add(sanLuongKiemNhom);
                }

                return sanLuongKiemNhoms;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<BravoModelV1.Model.SanLuongNhanVienKiem> SanLuongNhanVienKiem(DateTime dateTime, string xuongId)
        {
            try
            {
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                var nhanVienKiems = NhanVienViewModel.Instance.GetNhanViensKiems(xuongId, dateTime, 0);
                SanLuongNhanVienKiems.Clear();
                SanLuongNhanVienKiems = new ObservableCollection<BravoModelV1.Model.SanLuongNhanVienKiem>(NhanVienViewModel.Instance.GetSanLuongNhanVienKiems(nhanVienKiems));
                var nhanVienKiemsSangLuong = NhanVienViewModel.Instance.GetNhanViensKiems(xuongId, dateTime, 1);
                var sanLuongKiemNhoms = NhanVienViewModel.Instance.GetSanLuongKiemNhoms(nhanVienKiems, nhanVienKiemsSangLuong, dateTime, xuongId);
                var sanPhamBravo = SanPhamBravoViewModel.Instance.GetSanPhamKiem();
                var donGia = DG_DonGiaViewModel.Instance.Gets2<DG_DonGia>(dateTime, sanPhamBravo?.Id);
                foreach (var sanLuongKiemNhom in sanLuongKiemNhoms)
                {
                    var rl = SanLuongNhanVienKiems.Where(
                            x => x.NhanVienKiem.ToId == sanLuongKiemNhom.ToKiem.Ma)
                        .All(
                            x =>
                            {
                                x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                                x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                                x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                                x.TrongLuongHuong =
                                    sanLuongKiemNhom.TrongLuongTrenGio *
                                    x.SoGio *
                                    x.PhanTramHuong -
                                    sanLuongKiemNhom.TrongLuongTrenGio *
                                    x.SoGio *
                                    x.PhanTramHuong *
                                    x.TyLeTru;
                                x.GioTyLe = x.SoGio * x.PhanTramHuong;
                                x.MaSanPham = sanPhamBravo?.Id;
                                x.DonGia = donGia?.DonGia ?? 0;
                                x.ThanhTien = (donGia?.DonGia ?? 0) *
                                              x.TrongLuongHuong *
                                              (donGia?.HeSo ?? 1);
                                x.IsChamCong = BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime).FirstOrDefault(e => e == x.NhanVienKiem.NhanVienDaiThanh.MaNhanVien) == null ? false : true;
                                return true;
                            });
                }
                //List<BravoModelV1.Model.SanLuongNhanVienKiem> items = new List<BravoModelV1.Model.SanLuongNhanVienKiem>(SanLuongNhanVienKiems);
                return SanLuongNhanVienKiems.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<BravoModelV1.Model.SanLuongNhanVienKiem>();
                // throw;
                // MessageBox.Show(ex.Message);
            }
        }
        public List<BravoModelV1.Model.NhanVienKiem> ReloadDanhSachMacDinhCaiDatNhanVienTL(DateTime dateTime, string xuongId, int type)
        {
            try
            {

                // Clear danh sách nhân viên kiểm
                NhanViensKiems.Clear();

                // Lấy danh sách nhân viên kiểm mới
                NhanViensKiems = new ObservableRangeCollection<BravoModelV1.Model.NhanVienKiem>(GetNhanViensKiems(xuongId, dateTime, type));

                //// Kiểm tra loại và cập nhật NhomKiems
                //if (type != 2)
                //{
                //    var items = GetNhomKiems(xuongId);

                //    Application.Current.Dispatcher?.Invoke(
                //        new Action(
                //            () => { NhomKiems = new ObservableCollection<string>(items); }));
                //}
                //else
                //{
                //    var items = new List<string>() { "DV001" };
                //    Application.Current.Dispatcher?.Invoke(
                //        new Action(
                //            () => { NhomKiems = new ObservableCollection<string>(items); }));
                //}
                return NhanViensKiems.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<BravoModelV1.Model.NhanVienKiem>();
                //MessageBox.Show(ex.Message);
            }
        }
        public List<NhanVienDaiThanh> LoadGridNhanVienDaiThanhTo(string nhomKiem)
        {
            try
            {
                var items = NhanViensKiems.Where(x => x.ToId == nhomKiem).Select(x => x.NhanVienDaiThanh).ToList();
                NhanVienDaiThanhsTo = new ObservableRangeCollection<NhanVienDaiThanh>(items);
                return NhanVienDaiThanhsTo.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<NhanVienDaiThanh>();
                // throw;
            }
        }

        public void MoveTo(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienKiemSelectedItems = dataT.Item1;
                string[] nhanVienKiems = listNhanVienKiemSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<BravoModelV1.Model.NhanVienKiem>();
                foreach (var nhanVienKiemSelectedItem in nhanVienKiems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienKiemSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienKiem = JsonSerializer.Deserialize<BravoModelV1.Model.NhanVienKiem>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienKiem != null)
                    {
                        items.Add((BravoModelV1.Model.NhanVienKiem)nhanVienKiem);
                    }
                }

                foreach (var nhanViensKiem in items)
                {
                    NhanViensKiems.Remove(nhanViensKiem);
                    if (nhanViensKiem.ToId != nhomKiem)
                    {
                        NhanVienDaiThanhsTo.Add(nhanViensKiem.NhanVienDaiThanh);
                    }

                    var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                    {
                        ToId = nhomKiem,
                        Name = nhomKiem,
                        NhanVienDaiThanh = nhanViensKiem.NhanVienDaiThanh,
                        SoGio = nhanViensKiem.SoGio,
                        TyLe = nhanViensKiem.TyLe,
                        TyLeTru = nhanViensKiem.TyLeTru
                    };
                    NhanViensKiems.Add(nhanVienKiem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                VmMessage.SetExceptionCommand.Execute(ex);
            }
        }
        public void RemoveTo(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienKiemToSelectedItems = dataT.Item1;
                string[] nhanVienKiemTos = listNhanVienKiemToSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<NhanVienDaiThanh>();
                foreach (var nhanVienKiemToSelectedItem in nhanVienKiemTos)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienKiemToSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienKiemToSelectItem = JsonSerializer.Deserialize<NhanVienDaiThanh>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienKiemToSelectItem != null)
                    {
                        items.Add(nhanVienKiemToSelectItem);
                    }
                }
                foreach (var nhanVienDaiThanh in items)
                {
                    NhanVienDaiThanhsTo.Remove(nhanVienDaiThanh);
                    var item = NhanViensKiems.SingleOrDefault(
                        x => x.NhanVienDaiThanh == nhanVienDaiThanh && x.ToId == nhomKiem);
                    if (item != null)
                    {
                        NhanViensKiems.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
                VmMessage.SetExceptionCommand.Execute(ex);
            }
        }
        public void SaveCaiDatNhanVienKiem(DateTime dateTime, string xuongId, int type)
        {
            try
            {
                //var type = p is int ? (int)p : -1;
                var tokiemsOgrin = GetDanhSachToKiemsByXuongIdNgay(xuongId, dateTime, type);
                var tokiemsDelete = new List<DanhSachToKiem>();
                var toKiemsInsert = new List<DanhSachToKiem>();
                var toKiemsUpdate = new List<DanhSachToKiem>();
                var nhanVienKiems = NhanViensKiems.ToList();
                foreach (var danhSachToKiem in tokiemsOgrin)
                {
                    var item = nhanVienKiems.SingleOrDefault(
                        x => x.ToId == danhSachToKiem.MaToKiem && x.NhanVienDaiThanh.MaNhanVien == danhSachToKiem.MaNhanVien);

                    if (item != null)
                    {
                        nhanVienKiems.Remove(item);
                        if (type == 0 || type == 2)
                        {
                            toKiemsUpdate.Add(
                                new DanhSachToKiem()
                                {
                                    MaToKiem = item.ToId,
                                    MaXuong = xuongId,
                                    Ngay = dateTime,
                                    MaNhanVien = item.NhanVienDaiThanh.MaNhanVien,
                                    LoaiBoTri = type,
                                    SoGio = item.SoGio,
                                    TyLe = item.TyLe,
                                    TyLeTru = item.TyLeTru
                                });
                        }
                    }
                    else
                    {
                        tokiemsDelete.Add(danhSachToKiem);
                    }
                }

                foreach (var nhanViensKiem in nhanVienKiems)
                {
                    var item = new DanhSachToKiem()
                    {
                        MaToKiem = nhanViensKiem.ToId,
                        MaXuong = xuongId,
                        Ngay = dateTime,
                        MaNhanVien = nhanViensKiem.NhanVienDaiThanh.MaNhanVien,
                        LoaiBoTri = type,
                        SoGio = nhanViensKiem.SoGio,
                        TyLe = nhanViensKiem.TyLe,
                        TyLeTru = nhanViensKiem.TyLeTru
                    };
                    toKiemsInsert.Add(item);
                }

                var del = DeleteToKiems(tokiemsDelete);
                var ins = InsertToKiems(toKiemsInsert);
                var uda = UpdateToKiems(toKiemsUpdate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                // MessageBox.Show(ex.Message);
            }
        }
        public void AddNhanVienDaiThanhTo(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienDaiThanhSelectedItems = dataT.Item1;
                string[] nhanVienDaiThanhSelectItems = listNhanVienDaiThanhSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<NhanVienDaiThanh>();
                foreach (var nhanVien in nhanVienDaiThanhSelectItems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVien.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienDaiThanhSelectItem = JsonSerializer.Deserialize<NhanVienDaiThanh>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienDaiThanhSelectItem != null)
                    {
                        items.Add((NhanVienDaiThanh)nhanVienDaiThanhSelectItem);
                    }
                    //items.Add((NhanVienDaiThanh)nhanVien);
                }

                foreach (var nhanVienDaiThanh in items)
                {
                    var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                    {
                        ToId = nhomKiem,
                        Name = nhomKiem,
                        NhanVienDaiThanh = nhanVienDaiThanh,
                        SoGio = 8,
                        TyLe = 1,
                        TyLeTru = 0
                    };


                    var item = NhanViensKiems.SingleOrDefault(
                        x => x.NhanVienDaiThanh.MaNhanVien == nhanVienDaiThanh.MaNhanVien &&
                             x.ToId == nhomKiem);
                    if (item == null)
                    {
                        NhanViensKiems.Add(nhanVienKiem);
                        var itemTo = NhanVienDaiThanhsTo.SingleOrDefault(x => x.MaNhanVien == nhanVienDaiThanh.MaNhanVien);
                        if (itemTo == null)
                        {
                            NhanVienDaiThanhsTo.Add(nhanVienDaiThanh);
                        }
                    }
                    else
                    {
                        //MessageBox.Show(
                        //    $@"Nhân viên {item.NhanVienDaiThanh.MaNhanVien}:{item.NhanVienDaiThanh.MaHoSo}:{item.NhanVienDaiThanh.Name} đã tồn tại trong tổ {item.toId}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                //MessageBox.Show(ex.Message);
            }
        }
        #region Sơ Chế
        public List<SanLuongSoCheNhom> GetSanLuongNhomsSoChe(List<BoTriNhomSoChe> nhanViensSoChe, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var sanLuongKiemNhoms = new List<SanLuongSoCheNhom>();
                var nhomKiems = nhanViensSoChe.Select(x => x.MaNhomSoChe).Distinct().ToList();
                foreach (var nhomKiem in nhomKiems)
                {
                    var nhanviens = nhanViensSoChe.Where(x => x.MaNhomSoChe == nhomKiem)
                        .Select(x => x.MaNhanVien)
                        .Distinct();
                    var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                    var thanhPhamDao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh(connStr);

                    var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>();
                    var nhom = NhomSoCheDinhHinhViewModel.Instance.Gets(xuongId).SingleOrDefault(x => x.Ma == nhomKiem);
                    if (nhom != null && nhom.IsNhomChinh == true)
                    {
                        var thanhPhams = thanhPhamDao.Gets(false);
                        var thanhIds = thanhPhams.Select(x => x.Ma).Distinct();
                        phieuCans = dao.GetSanLuongs<BravoModelV1.Model.PhieuCanKiemDinhHinh>(dateTime, xuongId, thanhIds, nhanviens);
                    }
                    else
                    {
                        var thanhPhams = thanhPhamDao.GetThanhPhamSoCheDinhHinhsCaMuoi(false);
                        var thanhIds = thanhPhams.Select(x => x.Ma).Distinct();
                        phieuCans = dao.GetSanLuongs<BravoModelV1.Model.PhieuCanKiemDinhHinh>(dateTime, xuongId, thanhIds, nhanviens);
                    }

                    var soGio = nhanViensSoChe.Where(x => x.MaNhomSoChe == nhomKiem)
                        .Select(x => x.SoGio * x.TyLeHuong)
                        .Sum();
                    var sanLuongTongs = (from p in phieuCans
                                         group p by new { p.MaSanPham, p.TenSanPham }
                        into g
                                         select new
                                         {
                                             g.Key.MaSanPham,
                                             g.Key.TenSanPham,
                                             SanLuong = g.Sum(x => x.TrongLuong),
                                             SanLuongTrenGio = soGio == 0 ? 0 : g.Sum(x => x.TrongLuong) / (decimal)soGio
                                         }).ToList();
                    foreach (var item in sanLuongTongs)
                    {
                        var sanLuongKiemNhom = new SanLuongSoCheNhom()
                        {
                            ToKiem = new ToKiem() { Ma = nhomKiem, Name = nhomKiem, MaXuong = xuongId },
                            TongSanLuong = item.SanLuong,
                            TrongLuongTrenGio = item.SanLuongTrenGio,
                            TongGio = (decimal)soGio,
                            MaSanPham = item.MaSanPham,
                            TenSanPham = item.TenSanPham
                        };
                        sanLuongKiemNhoms.Add(sanLuongKiemNhom);
                    }
                }

                return sanLuongKiemNhoms;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<SanLuongNhanVienSoChe> GetSanLuongNhanViensSoChe(List<BoTriNhomSoChe> nhanViensKiems)
        {
            try
            {
                var items = new List<SanLuongNhanVienSoChe>();
                foreach (var nhanViensKiem in nhanViensKiems)
                {
                    var item = new SanLuongNhanVienSoChe()
                    {
                        MaNhanVien = nhanViensKiem.MaNhanVien,
                        TyLeHuong = (decimal)nhanViensKiem.TyLeHuong,
                        SoGio = (decimal)nhanViensKiem.SoGio,
                        TrongLuongHuong = 0,
                        TrongLuongPhanChia = 0,
                        TrongLuongTrenGio = 0,
                        TyLeTru = (decimal)nhanViensKiem.TyLeTru,
                        MaNhomSoChe = nhanViensKiem.MaNhomSoChe,
                        TrongLuongNhanThem = 0,
                        TrongLuongNhom = 0
                    };
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<BravoModelV1.Model.SanLuongNhanVienSoChe> ReloadSanLuongSoChe(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                //PhieuCanViewModel.Ins.IsBusy = true;
                Task.Delay(1000);
                //SanLuongNhanVienSoChes.Clear();
                var sanLuongNhanVienSoChesRs = new List<BravoModelV1.Model.SanLuongNhanVienSoChe>();
                //
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                var thanhPhamDao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh(connStr);
                var thanhPhamKoTinhGios = thanhPhamDao.GetThanhPhamSoCheDinhHinhsTinhGio(false);
                var thanhPhamKoTinhGioSanPhamIds = thanhPhamKoTinhGios.Select(x => x.BravoId).ToList();
                var nhanVienKiems = BoTriNhomSoCheViewModel.Instance.Gets(dateTime, xuongId);
                if (nhanVienKiems == null || !nhanVienKiems.Any())
                {
                    nhanVienKiems = BoTriNhomSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId);
                }

                //SanLuongNhanVienSoChes =
                //    new ObservableCollection<SanLuongNhanVienSoChe>(GetSanLuongNhanViensSoChe(nhanVienKiems));
                var sanLuongKiemNhoms = GetSanLuongNhomsSoChe(nhanVienKiems, dateTime, xuongId);
                var sanLuongNhanVienSoChesTemp = new List<SanLuongNhanVienSoChe>();
                var nhomsChinh = NhomSoCheDinhHinhViewModel.Instance.Gets(xuongId, true);
                var botris = new List<BoTriNhomSoChe>();
                foreach (var nhom in nhomsChinh)
                {
                    var botrisItem = BoTriNhomSoCheViewModel.Instance.Gets(dateTime, xuongId, nhom.Ma);

                    if (botrisItem == null || !botrisItem.Any())
                    {
                        botrisItem = BoTriNhomSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId, nhom.Ma);
                    }

                    if (botrisItem != null && botrisItem.Any())
                    {
                        botris.AddRange(botrisItem);
                    }
                }

                if (botris.Any())
                {
                    botris.All(
                        x =>
                        {
                            x.Ngay = dateTime.Date;
                            return true;
                        });
                }

                var nhanviens = botris.Select(x => x.MaNhanVien).Distinct();

                foreach (var sanLuongKiemNhom in sanLuongKiemNhoms)
                {
                    var nhanvienVienNhoms = nhanVienKiems.Where(x => x.MaNhomSoChe == sanLuongKiemNhom.ToKiem.Ma)
                        .ToList();
                    var sanLuongNhanVienSoChes = GetSanLuongNhanViensSoChe(nhanvienVienNhoms);
                    if (!thanhPhamKoTinhGioSanPhamIds.Contains(sanLuongKiemNhom.MaSanPham))
                    {
                        var gioTyLe = sanLuongNhanVienSoChes.Where(x => x.MaNhomSoChe == sanLuongKiemNhom.ToKiem.Ma)
                            .Select(x => x.SoGio * x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = gioTyLe == 0 ? 0 : sanLuongKiemNhom.TongSanLuong / gioTyLe;

                        var rl = sanLuongNhanVienSoChes.Where(x => x.MaNhomSoChe == sanLuongKiemNhom.ToKiem.Ma)
                            .All(
                                x =>
                                {
                                    x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                                    x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                                    x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                                    x.TrongLuongHuong =
                                        sanLuongDonVi *
                                        x.SoGio *
                                        x.TyLeHuong -
                                        sanLuongDonVi *
                                        x.SoGio *
                                        x.TyLeHuong *
                                        x.TyLeTru;
                                    x.MaSanPham = sanLuongKiemNhom.MaSanPham;
                                    x.TenSanPham = sanLuongKiemNhom.TenSanPham;
                                    x.GioTyLe = x.SoGio * x.TyLeHuong;
                                    return true;
                                });
                    }
                    else
                    {
                        var gioTyLe = sanLuongNhanVienSoChes.Where(x => x.MaNhomSoChe == sanLuongKiemNhom.ToKiem.Ma)
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = gioTyLe == 0 ? 0 : sanLuongKiemNhom.TongSanLuong / gioTyLe;

                        var rl = sanLuongNhanVienSoChes.Where(x => x.MaNhomSoChe == sanLuongKiemNhom.ToKiem.Ma)
                            .All(
                                x =>
                                {
                                    x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                                    x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                                    x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                                    x.TrongLuongHuong = sanLuongDonVi * x.TyLeHuong;
                                    x.MaSanPham = sanLuongKiemNhom.MaSanPham;
                                    x.TenSanPham = sanLuongKiemNhom.TenSanPham;
                                    x.GioTyLe = x.SoGio * x.TyLeHuong;
                                    return true;
                                });
                    }
                    sanLuongNhanVienSoChesTemp.AddRange(sanLuongNhanVienSoChes.Where(x => x.TrongLuongHuong > 0).ToList());
                }
                var sanLuongNhomSoCheChinh_nhanthem = sanLuongNhanVienSoChesTemp.Where(x => nhanviens.Contains(x.MaNhanVien)).ToList();
                sanLuongNhanVienSoChesTemp = sanLuongNhanVienSoChesTemp.Where(x => !nhanviens.Contains(x.MaNhanVien)).ToList();
                if (sanLuongNhomSoCheChinh_nhanthem.Any())
                {
                    foreach (var nhom in nhomsChinh)
                    {
                        var nhanViensNhom = botris.Where(x => x.MaNhomSoChe == nhom.MaHoSo)
                            .Select(x => x.MaNhanVien)
                            .ToList();
                        var sanPhams = sanLuongNhomSoCheChinh_nhanthem.Where(
                                x => nhanViensNhom.Contains(x.MaNhanVien))
                            .GroupBy(x => new { x.MaSanPham, x.MaNhomSoChe, x.TenSanPham })
                            .Select(
                                x => new
                                {
                                    x.Key.MaSanPham,
                                    x.Key.MaNhomSoChe,
                                    x.Key.TenSanPham,
                                    TrongLuong = x.Sum(g => g.TrongLuongHuong)
                                })
                            .ToList();
                        foreach (var item in sanPhams)
                        {
                            var boTrisNhom = botris.Where(x => x.MaNhomSoChe == nhom.MaHoSo).ToList();
                            var gioTyLe = boTrisNhom
                                .Select(x => x.SoGio * x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var sanLuongDonVi = gioTyLe == 0 ? 0 : item.TrongLuong / (decimal)gioTyLe;
                            foreach (var boTriNhom in boTrisNhom)
                            {
                                var sanLuong = new BravoModelV1.Model.SanLuongNhanVienSoChe()
                                {
                                    MaNhanVien = boTriNhom.MaNhanVien,
                                    MaNhomSoChe = nhom.MaHoSo,
                                    TyLeHuong = (decimal)boTriNhom.TyLeHuong,
                                    TyLeTru = (decimal)boTriNhom.TyLeTru,
                                    TenSanPham = item.TenSanPham,
                                    MaSanPham = item.MaSanPham,
                                    TrongLuongNhom = item.TrongLuong,
                                    SoGio = (decimal)boTriNhom.SoGio,
                                    TrongLuongHuong =
                                        (decimal)boTriNhom.SoGio *
                                        sanLuongDonVi *
                                        (decimal)boTriNhom.TyLeHuong -
                                        sanLuongDonVi *
                                        (decimal)boTriNhom.SoGio *
                                        (decimal)boTriNhom.TyLeHuong *
                                        (decimal)boTriNhom.TyLeTru
                                };
                                sanLuongNhanVienSoChesTemp.Add(sanLuong);
                            }
                        }
                    }
                }
                foreach (var sanLuongNhanVienSoChe in sanLuongNhanVienSoChesTemp)
                {
                    sanLuongNhanVienSoChesRs.Add(sanLuongNhanVienSoChe);
                }
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                var thanhPhams = thanhPhamDao.GetThanhPhamSoCheDinhHinhsCaMuoi(true);
                var thanhIds = thanhPhams.Select(x => x.Ma).Distinct();
                var phieuCans = dao.GetSanLuongsNotInNhanVienIds<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                    dateTime,
                    xuongId,
                    thanhIds,
                    nhanviens);

                //-----Nhom Kiem SoChe
                var nhanVienKiemsKiem = BoTriNhomKiemSoCheViewModel.Instance.Gets(dateTime, xuongId);
                if (nhanVienKiemsKiem == null || !nhanVienKiemsKiem.Any())
                {
                    nhanVienKiemsKiem = BoTriNhomKiemSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId);
                    nhanVienKiemsKiem.All(
                        x =>
                        {
                            x.Ngay = dateTime.Date;
                            return true;
                        });
                }

                //--------------------------------------
                var nhanVienDaiThanhs = Gets<NhanVienDaiThanh>();
                var nhomKiemSoChes = NhomKiemSoCheViewModel.Instance.Gets(xuongId);
                var KiemIds = (from n in nhanVienDaiThanhs
                               from k in nhomKiemSoChes
                               where k.MaHoSo == n.MaHoSo
                               select n.MaNhanVien).Distinct()
                    .ToList();
                var phieuCansKiem = phieuCans.Where(x => KiemIds.Contains(x.MaNhanVien)).ToList();
                var phieuCansNotKiem = phieuCans.Where(x => !KiemIds.Contains(x.MaNhanVien)).ToList();

                foreach (var item in phieuCansKiem)
                {
                    foreach (var nhomKiemSoChe in nhomKiemSoChes)
                    {
                        var botrisNhomKiemSoChe = nhanVienKiemsKiem.Where(x => x.MaNhomKiem == nhomKiemSoChe.Ma)
                            .ToList();
                        if (botrisNhomKiemSoChe.Any())
                        {
                            var tongGioTyLe = botrisNhomKiemSoChe.Select(x => x.SoGio * x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var trongLuongDonVi = tongGioTyLe == 0 ? 0 : item.TrongLuong / (decimal)tongGioTyLe;
                            foreach (var botriNhomKiemSoChe in botrisNhomKiemSoChe)
                            {
                                var sanLuong = new BravoModelV1.Model.SanLuongNhanVienSoChe()
                                {
                                    MaNhanVien = botriNhomKiemSoChe.MaNhanVien,
                                    MaNhomSoChe = nhomKiemSoChe.MaHoSo,
                                    TyLeHuong = (decimal)botriNhomKiemSoChe.TyLeHuong,
                                    TyLeTru = (decimal)botriNhomKiemSoChe.TyLeTru,
                                    TenSanPham = item.TenSanPham,
                                    MaSanPham = item.MaSanPham,
                                    TrongLuongNhom = item.TrongLuong,
                                    SoGio = (decimal)botriNhomKiemSoChe.SoGio,
                                    TrongLuongHuong =
                                        (decimal)botriNhomKiemSoChe.SoGio *
                                        trongLuongDonVi *
                                        (decimal)botriNhomKiemSoChe.TyLeHuong -
                                        trongLuongDonVi *
                                        (decimal)botriNhomKiemSoChe.SoGio *
                                        (decimal)botriNhomKiemSoChe.TyLeHuong *
                                        (decimal)botriNhomKiemSoChe.TyLeTru
                                };
                                sanLuongNhanVienSoChesRs.Add(sanLuong);
                            }
                        }
                    }
                }

                var nhomSoCheNotChinhs = NhomSoCheDinhHinhViewModel.Instance.Gets(xuongId).Where(x => x.IsNhomChinh == false).ToList();

                foreach (var item in phieuCansNotKiem)
                {
                    var nhanVienInfo = nhanVienDaiThanhs.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                    if (nhanVienInfo != null)
                    {
                        var nhomSoCheNhanVienDaiDien = nhomSoCheNotChinhs.SingleOrDefault(
                            x => x.MaHoSo == nhanVienInfo.MaHoSo);
                        if (nhomSoCheNhanVienDaiDien != null)
                        {
                            var nhanVienInNhomDaiDiens = nhanVienKiems.Where(x => x.MaNhomSoChe == nhomSoCheNhanVienDaiDien.MaHoSo).ToList();
                            var tongTyLe = nhanVienInNhomDaiDiens.Select(x => x.SoGio * x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var trongLuongDonVi = tongTyLe == 0 ? 0 : item.TrongLuong / (decimal)tongTyLe;
                            foreach (var nhanVienInNhomDaiDien in nhanVienInNhomDaiDiens)
                            {
                                var sanLuong = new BravoModelV1.Model.SanLuongNhanVienSoChe()
                                {
                                    MaNhanVien = nhanVienInNhomDaiDien.MaNhanVien,
                                    MaNhomSoChe = nhomSoCheNhanVienDaiDien.MaHoSo,
                                    MaSanPham = item.MaSanPham,
                                    TenSanPham = item.TenSanPham,
                                    TrongLuongHuong =
                                        trongLuongDonVi *
                                        (decimal)(nhanVienInNhomDaiDien.SoGio * nhanVienInNhomDaiDien.TyLeHuong) -
                                        trongLuongDonVi *
                                        (decimal)(nhanVienInNhomDaiDien.SoGio *
                                                  nhanVienInNhomDaiDien.TyLeHuong *
                                                  nhanVienInNhomDaiDien.TyLeTru),
                                    TyLeHuong = (decimal)nhanVienInNhomDaiDien.TyLeHuong,
                                    TyLeTru = (decimal)nhanVienInNhomDaiDien.TyLeTru,
                                    SoGio = (decimal)nhanVienInNhomDaiDien.SoGio
                                };

                                sanLuongNhanVienSoChesRs.Add(sanLuong);
                            }
                        }
                        else
                        {
                            var sanLuong = new BravoModelV1.Model.SanLuongNhanVienSoChe()
                            {
                                MaNhanVien = item.MaNhanVien,
                                MaNhomSoChe = string.Empty,
                                MaSanPham = item.MaSanPham,
                                TenSanPham = item.TenSanPham,
                                TrongLuongHuong = item.TrongLuong
                            };

                            sanLuongNhanVienSoChesRs.Add(sanLuong);
                        }
                    }
                }

                var finaRl = sanLuongNhanVienSoChesRs.GroupBy(
                        x => new
                        {
                            x.MaNhanVien,
                            x.MaNhomSoChe,
                            x.MaSanPham,
                            x.TenSanPham,
                            x.SoGio,
                            x.TyLeHuong,
                            x.TyLeTru
                        })
                    .Select(
                        x => new BravoModelV1.Model.SanLuongNhanVienSoChe()
                        {
                            MaNhanVien = x.Key.MaNhanVien,
                            MaNhomSoChe = x.Key.MaNhomSoChe,
                            TyLeTru = x.Key.TyLeTru,
                            MaSanPham = x.Key.MaSanPham,
                            TenSanPham = x.Key.TenSanPham,
                            TyLeHuong = x.Key.TyLeHuong,
                            SoGio = x.Key.SoGio,
                            TrongLuongHuong = Math.Round(x.Sum(g => g.TrongLuongHuong), 2)
                        })
                    .ToList();
                var donGias = DG_DonGiaViewModel.Instance.Gets<Models.Repos.Models.DG_DonGia>(dateTime, @"SP", true);
                var itemtonghoptinhluongs = (from item in finaRl
                                             join dg in donGias on new { item.MaSanPham } equals new
                                             {
                                                 dg.MaSanPham
                                             } into gj
                                             from jItem in gj.DefaultIfEmpty()
                                             select new BravoModelV1.Model.SanLuongNhanVienSoChe
                                             {
                                                 GioTyLe = item.GioTyLe,
                                                 TrongLuongHuong = item.TrongLuongHuong,
                                                 MaSanPham = item.MaSanPham,
                                                 MaNhanVien = item.MaNhanVien,
                                                 MaNhomSoChe = item.MaNhomSoChe,
                                                 SoGio = item.SoGio,
                                                 TenSanPham = item.TenSanPham,
                                                 TrongLuongNhanThem = item.TrongLuongNhanThem,
                                                 TrongLuongNhom = item.TrongLuongNhom,
                                                 TrongLuongPhanChia = item.TrongLuongPhanChia,
                                                 TrongLuongTrenGio = item.TrongLuongTrenGio,
                                                 TyLeHuong = item.TyLeHuong,
                                                 TyLeTru = item.TyLeTru,
                                                 DonGia = jItem?.DonGia ?? 0,
                                                 ThanhTien = item.TrongLuongHuong * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
                                                 IsChamCong = BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                    .FirstOrDefault(x => x == item.MaNhanVien) == null ? false : true
                                             }).ToList();
                sanLuongNhanVienSoChesRs.Clear();
                foreach (var itemtonghoptinhluong in itemtonghoptinhluongs)
                {
                    sanLuongNhanVienSoChesRs.Add(itemtonghoptinhluong);
                }

                return sanLuongNhanVienSoChesRs;
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                return new List<SanLuongNhanVienSoChe>();
                //throw;
            }
        }
        #endregion
        #region Tính Lương Kiểm Sơ Chế
        public List<SanLuongNhanVienKiemSoChe> GetSanLuongNhanVienKiems(List<BoTriNhomKiemSoChe> nhanViensKiems)
        {
            try
            {
                var items = new List<SanLuongNhanVienKiemSoChe>();
                foreach (var nhanViensKiem in nhanViensKiems)
                {
                    var item = new SanLuongNhanVienKiemSoChe()
                    {
                        MaNhanVien = nhanViensKiem.MaNhanVien,
                        MaNhomKiem = nhanViensKiem.MaNhomKiem,
                        TrongLuongNhanThem = 0,
                        TrongLuongNhom = 0,
                        TyLeHuong = (decimal)nhanViensKiem.TyLeHuong,
                        SoGio = (decimal)nhanViensKiem.SoGio,
                        TrongLuongHuong = 0,
                        TrongLuongPhanChia = 0,
                        TrongLuongTrenGio = 0,
                        TyLeTru = (decimal)nhanViensKiem.TyLeTru
                    };
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<SanLuongKiemNhom> GetSanLuongKiemNhoms(
            List<BoTriNhomKiemSoChe> nhanViensKiems,
            DateTime dateTime,
            string xuongId, string? connStr = null)
        {
            try
            {
                var sanLuongKiemNhoms = new List<SanLuongKiemNhom>();
                var nhomKiems = nhanViensKiems.Select(x => x.MaNhomKiem).Distinct().ToList();
                foreach (var nhomKiem in nhomKiems)
                {
                    var nhanviens = nhanViensKiems.Where(x => x.MaNhomKiem == nhomKiem)
                        .Select(x => x.MaNhanVien)
                        .Distinct();
                    var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                    var thanhPhamDao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh(connStr);
                    var thanhPhams = thanhPhamDao.GetThanhPhamSoCheDinhHinhsTinhKiem(true);
                    var thanhIds = thanhPhams.Select(x => x.Ma).Distinct();
                    var sanLuong = dao.GetSanLuongs(dateTime, xuongId, thanhIds);
                    var soGio = nhanViensKiems.Where(x => x.MaNhomKiem == nhomKiem)
                        .Select(x => x.SoGio * x.TyLeHuong)
                        .Sum();
                    var sanLuongTrenGio = soGio == 0 ? 0 : sanLuong / (decimal)soGio;
                    var sanLuongKiemNhom = new SanLuongKiemNhom()
                    {
                        ToKiem = new ToKiem() { Ma = nhomKiem, Name = nhomKiem, MaXuong = xuongId },
                        TongSanLuong = sanLuong,
                        TrongLuongTrenGio = sanLuongTrenGio,
                        TongGio = (decimal)soGio
                    };
                    sanLuongKiemNhoms.Add(sanLuongKiemNhom);
                }

                return sanLuongKiemNhoms;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<BravoModelV1.Model.SanLuongNhanVienKiemSoChe> ReloadSanLuongKiemSoChe(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                Task.Delay(1000);
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                var nhanVienKiems = BoTriNhomKiemSoCheViewModel.Instance.Gets(dateTime, xuongId);
                if (nhanVienKiems == null || !nhanVienKiems.Any())
                {
                    nhanVienKiems = BoTriNhomKiemSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId);
                    nhanVienKiems.All(
                        x =>
                        {
                            x.Ngay = dateTime.Date;
                            return true;
                        });
                }
                var SanLuongNhanVienKiemSoChe = new List<SanLuongNhanVienKiemSoChe>();
                SanLuongNhanVienKiemSoChe = GetSanLuongNhanVienKiems(nhanVienKiems);
                var sanLuongKiemNhoms = GetSanLuongKiemNhoms(nhanVienKiems, dateTime, xuongId);
                var sanPhamBravo = SanPhamBravoViewModel.Instance.GetSanPhamKiemSoChe();
                var donGia = DG_DonGiaViewModel.Instance.Gets2<DG_DonGia>(dateTime, sanPhamBravo?.Id);
                foreach (var sanLuongKiemNhom in sanLuongKiemNhoms)
                {
                    var rl = SanLuongNhanVienKiemSoChe.Where(x => x.MaNhomKiem == sanLuongKiemNhom.ToKiem.Ma)
                        .All(
                            x =>
                            {
                                x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                                x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                                x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                                x.TrongLuongHuong =
                                    sanLuongKiemNhom.TrongLuongTrenGio *
                                    x.SoGio *
                                    x.TyLeHuong -
                                    sanLuongKiemNhom.TrongLuongTrenGio *
                                    x.SoGio *
                                    x.TyLeHuong *
                                    x.TyLeTru;
                                x.GioTyLe = x.SoGio * x.TyLeHuong;
                                x.MaSanPham = sanPhamBravo?.Id;
                                x.DonGia = donGia?.DonGia ?? 0;
                                x.ThanhTien = (donGia?.DonGia ?? 0) * x.TrongLuongHuong * donGia?.HeSo ?? 0;
                                x.IsChamCong = BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                   .FirstOrDefault(e => e == x.MaNhanVien) ==
                                               null
                                    ? false
                                    : true;
                                //x.IsChamCong = false;
                                return true;
                            });

                }
                return SanLuongNhanVienKiemSoChe.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                return new List<SanLuongNhanVienKiemSoChe>();
                //throw;
            }
        }
        #endregion
        #region Tính Lương Phục Vụ
        public List<SanLuongKiemNhom> GetSanLuongPhucVus(List<BravoModelV1.Model.NhanVienKiem> nhanViensKiems, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var sanLuongKiemNhoms = new List<SanLuongKiemNhom>();
                var nhomKiems = nhanViensKiems.Select(x => x.ToId).Distinct().ToList();
                foreach (var nhomKiem in nhomKiems)
                {
                    //var nhanvienDao = new Modelv1.Dao.NhanVienDaiThanh(SettingViewModel.Ins.ConnectionString);
                    var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
                    var phieuCanSoCheDao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                    var thanhPhamExDao = new Dao.Repos.HQ.MaThanhPhamExDinhHinh(connStr);
                    var thanhPhamExSoCheDao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh(connStr);

                    //var nhanviens = nhanvienDao.GetNhanVienDaiThanhs(xuongId);
                    var thanhPhamExDinhHinhs = new List<MaThanhPhamExDinhHinh>();
                    thanhPhamExDinhHinhs = thanhPhamExDao.GetMaThanhPhamExDinhHinhs();
                    var sanLuong = dao.GetSanLuongTras(dateTime, xuongId, thanhPhamExDinhHinhs);
                    var thanhPhamExSoChes = thanhPhamExSoCheDao.GetThanhPhamSoCheDinhHinhsPhucVu(true);
                    var idsThanhPhamSoChes = thanhPhamExSoChes.Select(x => x.Ma);
                    var sanLuongSoChe = phieuCanSoCheDao.GetSanLuongs(dateTime, xuongId, idsThanhPhamSoChes);
                    sanLuong += sanLuongSoChe;
                    var soGio = nhanViensKiems.Where(x => x.ToId == nhomKiem).Select(x => x.SoGio * x.TyLe).Sum();
                    var sanLuongTrenGio = soGio == 0 ? 0 : sanLuong / (decimal)soGio;
                    var sanLuongKiemNhom = new SanLuongKiemNhom()
                    {
                        ToKiem = new ToKiem() { Ma = nhomKiem, Name = nhomKiem, MaXuong = xuongId },
                        TongSanLuong = sanLuong,
                        TrongLuongTrenGio = sanLuongTrenGio,
                        TongGio = (decimal)soGio
                    };
                    sanLuongKiemNhoms.Add(sanLuongKiemNhom);
                }

                return sanLuongKiemNhoms;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<BravoModelV1.Model.SanLuongNhanVienKiem> ReloadSanLuongPhucVu(DateTime dateTime, string xuongId)
        {
            try
            {
                Task.Delay(1000);
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);

                var nhanVienKiems = GetNhanViensKiems(xuongId, dateTime, 2);
                var sanLuongDichVus = new List<BravoModelV1.Model.SanLuongNhanVienKiem>();
                sanLuongDichVus = new List<BravoModelV1.Model.SanLuongNhanVienKiem>(GetSanLuongNhanVienKiems(nhanVienKiems));
                var nhanVienKiemsSangLuong = GetNhanViensKiems(xuongId, dateTime, 2);
                var sanLuongKiemNhoms = GetSanLuongPhucVus(nhanVienKiems, dateTime, xuongId);
                var sanPhamBravo = SanPhamBravoViewModel.Instance.GetSanPhamPhucVu();
                var donGia = DG_DonGiaViewModel.Instance.Gets2<DG_DonGia>(dateTime, sanPhamBravo?.Id);
                foreach (var sanLuongKiemNhom in sanLuongKiemNhoms)
                {
                    var rl = sanLuongDichVus.Where(x => x.NhanVienKiem.ToId == sanLuongKiemNhom.ToKiem.Ma)
                        .All(
                            x =>
                            {
                                x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                                x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                                x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                                x.TrongLuongHuong =
                                    sanLuongKiemNhom.TrongLuongTrenGio *
                                    x.SoGio *
                                    x.PhanTramHuong -
                                    sanLuongKiemNhom.TrongLuongTrenGio *
                                    x.SoGio *
                                    x.PhanTramHuong *
                                    x.TyLeTru;
                                x.GioTyLe = x.SoGio * x.PhanTramHuong;
                                x.MaSanPham = sanPhamBravo?.Id;
                                x.DonGia = donGia?.DonGia ?? 0;
                                x.ThanhTien = (donGia?.DonGia ?? 0) *
                                              x.TrongLuongHuong *
                                              (donGia?.HeSo ?? 1);
                                x.IsChamCong =
                                     BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                        .FirstOrDefault(
                                            e => e == x.NhanVienKiem.NhanVienDaiThanh.MaNhanVien) ==
                                    null
                                        ? false
                                        : true;
                                return true;
                            });
                    //var tongTrongLuongNhanThem = SanLuongDichVus
                    //    .Where(x => x.PhanTramHuong < 1 && x.NhanVienKiem.toId == sanLuongKiemNhom.ToKiem.Ma)
                    //    .Select(x => x.TrongLuongPhanChia - x.TrongLuongHuong)
                    //    .Sum();
                    //var tongGioThem = SanLuongDichVus
                    //    .Where(x => x.NhanVienKiem.toId == sanLuongKiemNhom.ToKiem.Ma)
                    //    .Select(x => x.GioTyLe)
                    //    .DefaultIfEmpty(0)
                    //    .Sum();
                    ////var tongGioThem = SanLuongDichVus
                    ////    .Where(x => x.PhanTramHuong >= 1 && x.NhanVienKiem.toId == sanLuongKiemNhom.ToKiem.Ma)
                    ////    .Select(x => x.SoGio)
                    ////    .DefaultIfEmpty(0)
                    ////    .Sum();
                    //var trongLuongThemTrenGio = tongGioThem == 0 ? 0 : tongTrongLuongNhanThem / tongGioThem;
                    //        var rl2 = SanLuongDichVus
                    //            .Where(x => x.NhanVienKiem.toId == sanLuongKiemNhom.ToKiem.Ma && x.PhanTramHuong >= 1)
                    //            .All(x =>
                    //{
                    //    x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                    //    x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                    //    x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                    //    x.TrongLuongNhanThem = trongLuongThemTrenGio * x.SoGio;

                    //    x.TrongLuongHuong =
                    //                        (x.TrongLuongHuong - x.TrongLuongHuong * x.TyLeTru) + x.TrongLuongNhanThem;
                    //    return true;
                    //});
                    //        var rl2 = SanLuongDichVus
                    //            .Where(x => x.NhanVienKiem.toId == sanLuongKiemNhom.ToKiem.Ma)
                    //            .All(x =>
                    //{
                    //    x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                    //    x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                    //    x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                    //    x.TrongLuongNhanThem = Math.Round(trongLuongThemTrenGio * x.GioTyLe, 2);

                    //    x.TrongLuongHuong =
                    //                                   Math.Round((x.TrongLuongHuong - x.TrongLuongHuong * x.TyLeTru) +
                    //        x.TrongLuongNhanThem,
                    //                                              2);
                    //    return true;
                    //});

                    #region Nhom So Che

                    var danhSachNhomSoChes = BoTriNhomSoCheViewModel.Instance.Gets(dateTime, xuongId);
                    //var id = danhSachNhomSoChes.Where(x => x.MaNhanVien == "000003603").ToList();
                    if (danhSachNhomSoChes == null || !danhSachNhomSoChes.Any())
                    {
                        danhSachNhomSoChes = BoTriNhomSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId);
                    }

                    var nhomSoChes =
                        NhomSoCheDinhHinhViewModel.Instance.Gets(xuongId, true);
                    var nhanVienNhomSoChes = (from d in danhSachNhomSoChes
                                              from t in nhomSoChes
                                              where d.MaNhomSoChe == t.Ma
                                              select new { d, MaHoSo = t.MaHoSo }).ToList();
                    var thongTinNhanVienCuaNhomSoChes = (from n in NhanVienDaiThanhs
                                                         from nh in nhomSoChes
                                                         where n.MaHoSo == nh.MaHoSo
                                                         select n).ToList();
                    var maNhanVienNhomSoCheIds = nhanVienNhomSoChes.Select(x => x.d.MaNhanVien).Distinct();
                    var sanLuongNhomSoChes = sanLuongDichVus.Where(
                            x => maNhanVienNhomSoCheIds.Contains(x.NhanVienKiem.NhanVienDaiThanh
                                .MaNhanVien))
                        .ToList();
                    foreach (var item in maNhanVienNhomSoCheIds)
                    {
                        var sanLuongDichVuss = sanLuongDichVus.ToList();
                        sanLuongDichVuss.RemoveAll(x => x.NhanVienKiem.NhanVienDaiThanh.MaNhanVien == item);
                        sanLuongDichVus = new List<BravoModelV1.Model.SanLuongNhanVienKiem>(sanLuongDichVuss);
                    }

                    foreach (var thongTinNhom in thongTinNhanVienCuaNhomSoChes)
                    {
                        var nhanVienInNhomSoChes = (from n in NhanVienViewModel.Instance.NhanVienDaiThanhs
                                                    from nk in nhanVienNhomSoChes
                                                    where n.MaNhanVien == nk.d.MaNhanVien && nk.MaHoSo == thongTinNhom.MaHoSo
                                                    select new
                                                    {
                                                        d = nk.d,
                                                        MaHoSo = nk.MaHoSo,
                                                        TenNhanVien = n.Name,
                                                        NhanVien = n
                                                    })
                            .ToList(
                            );
                        var nhanVienInNhomSoCheIds = nhanVienInNhomSoChes.Select(x => x.d.MaNhanVien)
                            .ToList();
                        //var nhanVienInToKiemSoChes = nhanVienKiemSoChes.Where(x => x.MaHoSo == maHoSoKiem).ToList();
                        //var maHoSoInToKiemIds = nhanVienInToKiems.Select(x => x.MaHoSo).ToList();
                        var sanLuongInNhomSoChes = sanLuongNhomSoChes.Where(
                                x => nhanVienInNhomSoCheIds.Contains(x.NhanVienKiem.NhanVienDaiThanh
                                    .MaNhanVien))
                            .Select(x => x.TrongLuongHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        if (sanLuongInNhomSoChes > 0)
                        {
                            var tongTyLe = nhanVienInNhomSoChes.Select(x => x.d.TyLeHuong * x.d.SoGio)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var sanLuongDonVi =
                                tongTyLe == 0 ? 0 : sanLuongInNhomSoChes / (decimal)tongTyLe;
                            foreach (var nhanVien in nhanVienInNhomSoChes)
                            {
                                var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                                {
                                    ToId = nhanVien.MaHoSo,
                                    Name = nhanVien.MaHoSo,
                                    SoGio = nhanVien.d.SoGio,
                                    TyLe = nhanVien.d.TyLeHuong,
                                    TyLeTru = nhanVien.d.TyLeTru,
                                    NhanVienDaiThanh = nhanVien.NhanVien
                                };
                                var sanluongDichVu = new BravoModelV1.Model.SanLuongNhanVienKiem()
                                {
                                    NhanVienKiem = nhanVienKiem,
                                    TyLeTru = (decimal)nhanVien.d.TyLeTru,
                                    SoGio = (decimal)nhanVien.d.SoGio,
                                    PhanTramHuong = (decimal)nhanVien.d.TyLeHuong,
                                    TrongLuongHuong =
                                        Math.Round(
                                            (sanLuongDonVi *
                                             (decimal)nhanVien.d.TyLeHuong *
                                             (decimal)nhanVien.d.SoGio -
                                             sanLuongDonVi *
                                             (decimal)nhanVien.d.TyLeHuong *
                                             (decimal)nhanVien.d.SoGio *
                                             (decimal)nhanVien.d.TyLeTru),
                                            2),
                                    DonGia = donGia?.DonGia ?? 0,
                                    ThanhTien =
                                        (donGia?.DonGia ?? 0) *
                                        Math.Round(
                                            (sanLuongDonVi *
                                             (decimal)nhanVien.d.TyLeHuong *
                                             (decimal)nhanVien.d.SoGio -
                                             sanLuongDonVi *
                                             (decimal)nhanVien.d.TyLeHuong *
                                             (decimal)nhanVien.d.SoGio *
                                             (decimal)nhanVien.d.TyLeTru),
                                            2) *
                                        (donGia?.HeSo ?? 1),
                                    IsChamCong =
                                     BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                            .FirstOrDefault(
                                                e => e == nhanVienKiem.NhanVienDaiThanh.MaNhanVien) ==
                                        null
                                            ? false
                                            : true
                                };
                                sanLuongDichVus.Add(sanluongDichVu);
                            }
                        }
                    }

                    #endregion Nhom So Che

                    //        var rl3 = SanLuongDichVus
                    //            .Where(x => x.NhanVienKiem.toId == sanLuongKiemNhom.ToKiem.Ma && x.PhanTramHuong < 1)
                    //            .All(x =>
                    //{
                    //    x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                    //    x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                    //    x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                    //    x.TrongLuongHuong =
                    //                        (x.TrongLuongHuong - x.TrongLuongHuong * x.TyLeTru);
                    //    return true;
                    //});
                }
                return sanLuongDichVus.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                //MessageBox.Show(ex.Message);
                return new List<BravoModelV1.Model.SanLuongNhanVienKiem>();
            }
        }



        public List<BravoModelV1.Model.NhanVienKiem> ReloadNhanVienPhucVu(DateTime dateTime, string xuongId)
        {
            try
            {
                var nhanVienKiems = new List<BravoModelV1.Model.NhanVienKiem>();
                nhanVienKiems.Clear();
                nhanVienKiems = new List<BravoModelV1.Model.NhanVienKiem>(GetNhanViensKiems(xuongId, dateTime, 2));
                return nhanVienKiems.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                //MessageBox.Show(ex.Message);
                return new List<BravoModelV1.Model.NhanVienKiem>();
            }
        }
        public void MoveToPhucVu(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienKiemSelectedItems = dataT.Item1;
                string[] nhanVienKiems = listNhanVienKiemSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<BravoModelV1.Model.NhanVienKiem>();
                foreach (var nhanVienKiemSelectedItem in nhanVienKiems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienKiemSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienKiem = JsonSerializer.Deserialize<BravoModelV1.Model.NhanVienKiem>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienKiem != null)
                    {
                        items.Add((BravoModelV1.Model.NhanVienKiem)nhanVienKiem);
                    }
                }

                foreach (var nhanViensKiem in items)
                {
                    NhanViensKiems.Remove(nhanViensKiem);
                    if (nhanViensKiem.ToId != nhomKiem)
                    {
                        NhanVienDaiThanhsTo.Add(nhanViensKiem.NhanVienDaiThanh);
                    }

                    var nhanVienKiem = new BravoModelV1.Model.NhanVienKiem()
                    {
                        ToId = nhomKiem,
                        Name = nhomKiem,
                        NhanVienDaiThanh = nhanViensKiem.NhanVienDaiThanh,
                        SoGio = nhanViensKiem.SoGio,
                        TyLe = nhanViensKiem.TyLe,
                        TyLeTru = nhanViensKiem.TyLeTru
                    };
                    NhanViensKiems.Add(nhanVienKiem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                VmMessage.SetExceptionCommand.Execute(ex);
            }
        }
        #endregion
        #region Tính Lương Phụ Fillet
        public List<NhanVienDaiThanh> GetNhanVienInIds(IEnumerable<string> ids, string? connStr = null)
        {
            try
            {
                var nhanVienDao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                return nhanVienDao.GetNhanVienDaiThanhInIds(listOfIdsJoined);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhanVienDaiThanh> GetNhanVienByLoaiThanhPhamPhu(string thanhPhamPhuId, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var nhanVienDaiThanhs = new List<NhanVienDaiThanh>();
                var phanBoDao = new Dao.Repos.HQ.PhanBoNhanVienTheoCongViecPhuFillet(connStr);
                var items = phanBoDao.Get(thanhPhamPhuId, dateTime, xuongId);
                if (items != null && items.Any())
                {
                    var nhanVienDao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                    var ids = items.Select(x => x.MaNhanVien).Distinct();

                    var nhanviens = GetNhanVienInIds(ids);
                    if (nhanviens != null && nhanviens.Any())
                    {
                        nhanVienDaiThanhs.AddRange(nhanviens);
                    }
                }


                return nhanVienDaiThanhs;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhanBoNhanVienTheoCongViecPhuFillet> GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(string thanhPhamId, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var phanBoDao = new Dao.Repos.HQ.PhanBoNhanVienTheoCongViecPhuFillet(connStr);
                return phanBoDao.Get(thanhPhamId, dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhanVienPhuTheoBanFillet> GetNhanViensPhuByBanCatTiet(
            string banId,
            DateTime dateTime,
            string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienPhuTheoBanFillet(connStr);
                return dao.Gets(dateTime, xuongId, banId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhanVienPhuTheoLine> GetNhanViensPhuByLine(
           string lineId,
           DateTime dateTime,
           string xuongId,
           string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienPhuTheoLine(connStr);
                return dao.GetNhanviens(dateTime, xuongId, lineId, congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhanVienSanLuongTheoLineFillet> GetNhanViensSanLuong(
            DateTime dateTime,
            string lineId,
            string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienSanLuongTheoLineFillet(connStr);
                return dao.GetNhanViens(dateTime, lineId, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BravoModelV1.Model.PhieuCanKiemDinhHinh> GetPhieuCanPhuFilletTinhLuongs(
            List<BravoModelV1.Model.SanLuongPhuFillet> sanLuongs,
            int khuVucId, DateTime dateTime)
        {
            try
            {
                var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>();
                var congViecs = ThanhPhamPhuFilletViewModel.Instance.Get();
                foreach (var item in sanLuongs)
                {
                    var sanPham = congViecs.SingleOrDefault(x => x.Ma == item.MaThanhPham);
                    phieuCans.Add(
                        new BravoModelV1.Model.PhieuCanKiemDinhHinh()
                        {
                            CaLamViec = "CT04",
                            MaNhanVien = item.MaNhanVien,
                            MaSanPham = sanPham?.SanPhamId,
                            Ngay = dateTime,
                            TenSanPham = sanPham?.Ten,
                            TrongLuong = Math.Round((decimal)item.SanLuongHuong, 2),
                            _Status = 0,
                            KhuVuc = khuVucId.ToString("00")
                        });
                }

                return phieuCans;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BravoModelV1.Model.SanLuongPhuFillet> ReloadSanLuongPhuFillet(DateTime dateTime, string xuongId)
        {
            try
            {
                var sanLuongPhuFillets = new List<SanLuongPhuFillet>();
                //1 TNNL
                var tnnlId = "TNNL";
                var nhanVienPhuTNNL = GetNhanVienByLoaiThanhPhamPhu(tnnlId, dateTime, xuongId);
                var idsTNNL = nhanVienPhuTNNL.Select(x => x.MaNhanVien).Distinct();
                var GioTNNLs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsTNNL, tnnlId, xuongId);
                var phanBoTNNLs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(tnnlId, dateTime, xuongId);
                for (int i = 0; i < GioTNNLs.Count(); i++)
                {
                    if (i == (GioTNNLs.Count() - 1))
                    {
                        break;
                    }

                    var sanLuongTong = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongNguyenLieu(GioTNNLs[i], GioTNNLs[i + 1], dateTime, xuongId);

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(dateTime, GioTNNLs[i], GioTNNLs[i + 1], idsTNNL, tnnlId, xuongId);
                    var soGio = (GioTNNLs[i + 1] - GioTNNLs[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        var tongTyLe = phanBoTNNLs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoTNNLs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = tnnlId,
                                    SanLuongHuong =
                                        (phanBo.TyLeHuong *
                                         sanLuongDonVi -
                                         (phanBo.TyLeHuong * sanLuongDonVi) *
                                         phanBo.TyLeTru),
                                    SoGio = soGio,
                                    SanLuongTru = ((phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru),
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Add(itemSanluong);
                            }
                        }
                    }
                }

                //2 Vun De Vun Chim Tuoi
                var vdId = "VD";
                var nhanVienPhuVD = GetNhanVienByLoaiThanhPhamPhu(vdId, dateTime, xuongId);
                var idsVD = nhanVienPhuVD.Select(x => x.MaNhanVien).Distinct();
                var GioVDs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsVD, vdId, xuongId);
                var phanBoVDs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(vdId, dateTime, xuongId);
                if (phanBoVDs.Any())
                {
                    var sanLuongTong = PhieuCanPhuPhamViewModel.Instance.GetSanLuongPhuPham(dateTime, xuongId, "B");
                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Gets(dateTime, vdId, idsVD, xuongId);
                    double tongGios = 0;
                    var sanLuongVDs = new List<SanLuongPhuFillet>();
                    foreach (var phanBoVD in phanBoVDs)
                    {
                        var soGio = gioVaoRaFillets.Where(x => x.MaNhanVien == phanBoVD.MaNhanVien)
                            .Select(x => (x.GioRa.Value - x.GioVao).TotalHours)
                            .DefaultIfEmpty(0)
                            .Sum();
                        if (soGio > 0)
                        {
                            var itemSanluong = new SanLuongPhuFillet()
                            {
                                MaNhanVien = phanBoVD.MaNhanVien,
                                MaThanhPham = vdId,
                                SoGio = soGio,
                                TyLeTru = phanBoVD.TyLeTru,
                                TyLeHuong = phanBoVD.TyLeHuong,
                                GioTyLe = phanBoVD.TyLeHuong * soGio
                            };
                            sanLuongVDs.Add(itemSanluong);
                        }

                        tongGios += soGio;
                    }

                    var tongGioTyLe = sanLuongVDs.Select(x => x.TyLeHuong * x.SoGio).DefaultIfEmpty(0).Sum();
                    var sanLuongDonVi = tongGioTyLe == 0 ? 0 : sanLuongTong / tongGioTyLe;
                    sanLuongVDs.All(
                        x =>
                        {
                            x.SanLuongHuong = x.GioTyLe * sanLuongDonVi - x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                            x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                            return true;
                        });
                    sanLuongPhuFillets.AddRange(sanLuongVDs);
                }

                //for(int i = 0; i < GioVDs.Count(); i++)
                //{
                //    if(i == (GioVDs.Count() - 1))
                //    {
                //        break;
                //    }

                //    var sanLuongTong = PhieuCanVm.GetSanLuongPhuPham(GioVDs[i],
                //                                                     GioVDs[i + 1],
                //                                                     MainViewModel.DateTimeNowShared,
                //                                                     XuongIdShared,
                //                                                     "B");
                //    var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                //                                         GioVDs[i],
                //                                         GioVDs[i + 1],
                //                                         idsVD,
                //                                         vdId);
                //    var soGio = (GioVDs[i + 1] - GioVDs[i]).TotalHours;
                //    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //    if(gioVaoRaFillets.Any())
                //    {
                //        var tongTyLe = phanBoVDs.Where(x => ids.Contains(x.MaNhanVien))
                //            .Select(x => x.TyLeHuong)
                //            .DefaultIfEmpty(0)
                //            .Sum();
                //        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //        foreach(var nhanVien in gioVaoRaFillets)
                //        {
                //            var phanBo = phanBoVDs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //            if(phanBo != null)
                //            {
                //                var itemSanluong = new SanLuongPhuFillet()
                //                {
                //                    STT = nhanVien.STT,
                //                    MaNhanVien = nhanVien.MaNhanVien,
                //                    MaThanhPham = vdId,
                //                    SanLuongHuong =
                //                    (phanBo.TyLeHuong *
                //                        sanLuongDonVi -
                //                        (phanBo.TyLeHuong * sanLuongDonVi) *
                //                        phanBo.TyLeTru),
                //                    SoGio = soGio,
                //                    SanLuongTru = ((phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru),
                //                    TyLeTru = phanBo.TyLeTru,
                //                    TyLeHuong = phanBo.TyLeHuong
                //                };
                //                sanLuongPhuFillets.Add(itemSanluong);
                //            }
                //        }
                //    }
                //}
                //2.1 Vun De Vun Do Tuoi
                var vdDTId = "VDDT";
                var nhanVienPhuVDDT = GetNhanVienByLoaiThanhPhamPhu(vdDTId, dateTime, xuongId);
                var idsVDDT = nhanVienPhuVDDT.Select(x => x.MaNhanVien).Distinct();
                var GioVDDTs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsVDDT, vdDTId, xuongId);
                var phanBoVDDTs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(vdDTId, dateTime, xuongId);

                if (phanBoVDDTs.Any())
                {
                    var sanLuongTong = PhieuCanPhuPhamViewModel.Instance.GetSanLuongPhuPham(dateTime, xuongId, "D");
                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Gets(dateTime, vdDTId, idsVDDT, xuongId);
                    double tongGios = 0;
                    var sanLuongVDDTs = new List<SanLuongPhuFillet>();
                    foreach (var phanBoVDDT in phanBoVDDTs)
                    {
                        var soGio = gioVaoRaFillets.Where(x => x.MaNhanVien == phanBoVDDT.MaNhanVien)
                            .Select(x => (x.GioRa.Value - x.GioVao).TotalHours)
                            .DefaultIfEmpty(0)
                            .Sum();
                        if (soGio > 0)
                        {
                            var itemSanluong = new SanLuongPhuFillet()
                            {
                                MaNhanVien = phanBoVDDT.MaNhanVien,
                                MaThanhPham = vdDTId,
                                SoGio = soGio,
                                TyLeTru = phanBoVDDT.TyLeTru,
                                TyLeHuong = phanBoVDDT.TyLeHuong,
                                GioTyLe = phanBoVDDT.TyLeHuong * soGio
                            };
                            sanLuongVDDTs.Add(itemSanluong);
                        }

                        tongGios += soGio;
                    }

                    var tongGioTyLe = sanLuongVDDTs.Select(x => x.TyLeHuong * x.SoGio).DefaultIfEmpty(0).Sum();
                    var sanLuongDonVi = tongGioTyLe == 0 ? 0 : sanLuongTong / tongGioTyLe;
                    sanLuongVDDTs.All(
                        x =>
                        {
                            x.SanLuongHuong = x.GioTyLe * sanLuongDonVi - x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                            x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                            return true;
                        });
                    sanLuongPhuFillets.AddRange(sanLuongVDDTs);
                }
                //var vdDTId = "VDDT";
                //var nhanVienPhuVDDT = NhanVienVm.GetNhanVienByLoaiThanhPhamPhu(vdDTId, DateTimeNowShared, XuongIdShared);
                //var idsVDDT = nhanVienPhuVDDT.Select(x => x.MaNhanVien).Distinct();
                //var GioVDDTs = GioVaoRaVm.GetListTime(DateTimeNowShared, idsVDDT, vdDTId);
                //var phanBoVDDTs = NhanVienVm.GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(vdDTId,
                //                                                                          MainViewModel.DateTimeNowShared,
                //                                                                          MainViewModel.XuongIdShared);
                //for(int i = 0; i < GioVDDTs.Count(); i++)
                //{
                //    if(i == (GioVDDTs.Count() - 1))
                //    {
                //        break;
                //    }

                //    var sanLuongTong = PhieuCanVm.GetSanLuongPhuPham(GioVDDTs[i],
                //                                                     GioVDDTs[i + 1],
                //                                                     MainViewModel.DateTimeNowShared,
                //                                                     XuongIdShared,
                //                                                     "D");
                //    var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                //                                         GioVDDTs[i],
                //                                         GioVDDTs[i + 1],
                //                                         idsVDDT,
                //                                         vdDTId);
                //    var soGio = (GioVDDTs[i + 1] - GioVDDTs[i]).TotalHours;
                //    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //    if(gioVaoRaFillets.Any())
                //    {
                //        var tongTyLe = phanBoVDDTs.Where(x => ids.Contains(x.MaNhanVien))
                //            .Select(x => x.TyLeHuong)
                //            .DefaultIfEmpty(0)
                //            .Sum();
                //        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //        foreach(var nhanVien in gioVaoRaFillets)
                //        {
                //            var phanBo = phanBoVDDTs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //            if(phanBo != null)
                //            {
                //                var itemSanluong = new SanLuongPhuFillet()
                //                {
                //                    STT = nhanVien.STT,
                //                    MaNhanVien = nhanVien.MaNhanVien,
                //                    MaThanhPham = vdDTId,
                //                    SanLuongHuong =
                //                    (phanBo.TyLeHuong *
                //                        sanLuongDonVi -
                //                        (phanBo.TyLeHuong * sanLuongDonVi) *
                //                        phanBo.TyLeTru),
                //                    SoGio = soGio,
                //                    SanLuongTru = ((phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru),
                //                    TyLeTru = phanBo.TyLeTru,
                //                    TyLeHuong = phanBo.TyLeHuong
                //                };
                //                sanLuongPhuFillets.Add(itemSanluong);
                //            }
                //        }
                //    }
                //}
                //2.2 Vun De Vun Mo Tuoi
                var vdMTId = "VDMT";
                var nhanVienPhuVDMT = GetNhanVienByLoaiThanhPhamPhu(vdMTId, dateTime, xuongId);
                var idsVDMT = nhanVienPhuVDMT.Select(x => x.MaNhanVien).Distinct();
                var GioVDMTs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsVDMT, vdMTId, xuongId);
                var phanBoVDMTs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(vdMTId, dateTime, xuongId);

                if (phanBoVDMTs.Any())
                {
                    var sanLuongTong = PhieuCanPhuPhamViewModel.Instance.GetSanLuongPhuPham(dateTime, xuongId, "A");
                    var sanLuongThem = PhieuCanPhuPhamViewModel.Instance.GetSanLuongPhuPham(dateTime, xuongId, "M");
                    sanLuongTong += sanLuongThem;
                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Gets(dateTime, vdMTId, idsVDMT, xuongId);
                    double tongGios = 0;
                    var sanLuongVDMTs = new List<SanLuongPhuFillet>();
                    foreach (var phanBoVDMT in phanBoVDMTs)
                    {
                        var soGio = gioVaoRaFillets.Where(x => x.MaNhanVien == phanBoVDMT.MaNhanVien)
                            .Select(x => (x.GioRa.Value - x.GioVao).TotalHours)
                            .DefaultIfEmpty(0)
                            .Sum();
                        if (soGio > 0)
                        {
                            var itemSanluong = new SanLuongPhuFillet()
                            {
                                MaNhanVien = phanBoVDMT.MaNhanVien,
                                MaThanhPham = vdMTId,
                                SoGio = soGio,
                                TyLeTru = phanBoVDMT.TyLeTru,
                                TyLeHuong = phanBoVDMT.TyLeHuong,
                                GioTyLe = phanBoVDMT.TyLeHuong * soGio
                            };
                            sanLuongVDMTs.Add(itemSanluong);
                        }

                        tongGios += soGio;
                    }

                    var tongGioTyLe = sanLuongVDMTs.Select(x => x.TyLeHuong * x.SoGio).DefaultIfEmpty(0).Sum();
                    var sanLuongDonVi = tongGioTyLe == 0 ? 0 : sanLuongTong / tongGioTyLe;
                    sanLuongVDMTs.All(
                        x =>
                        {
                            x.SanLuongHuong = x.GioTyLe * sanLuongDonVi - x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                            x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                            return true;
                        });
                    sanLuongPhuFillets.AddRange(sanLuongVDMTs);
                }
                //var vdMTId = "VDMT";
                //var nhanVienPhuVDMT = NhanVienVm.GetNhanVienByLoaiThanhPhamPhu(vdMTId, DateTimeNowShared, XuongIdShared);
                //var idsVDMT = nhanVienPhuVDMT.Select(x => x.MaNhanVien).Distinct();
                //var GioVDMTs = GioVaoRaVm.GetListTime(DateTimeNowShared, idsVDMT, vdMTId);
                //var phanBoVDMTs = NhanVienVm.GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(vdMTId,
                //                                                                          MainViewModel.DateTimeNowShared,
                //                                                                          MainViewModel.XuongIdShared);
                //for(int i = 0; i < GioVDMTs.Count(); i++)
                //{
                //    if(i == (GioVDMTs.Count() - 1))
                //    {
                //        break;
                //    }

                //    var sanLuongTong = PhieuCanVm.GetSanLuongPhuPham(GioVDMTs[i],
                //                                                     GioVDMTs[i + 1],
                //                                                     MainViewModel.DateTimeNowShared,
                //                                                     XuongIdShared,
                //                                                     "A");
                //    var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                //                                         GioVDMTs[i],
                //                                         GioVDMTs[i + 1],
                //                                         idsVDMT,
                //                                         vdMTId);
                //    var soGio = (GioVDMTs[i + 1] - GioVDMTs[i]).TotalHours;
                //    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //    if(gioVaoRaFillets.Any())
                //    {
                //        var tongTyLe = phanBoVDMTs.Where(x => ids.Contains(x.MaNhanVien))
                //            .Select(x => x.TyLeHuong)
                //            .DefaultIfEmpty(0)
                //            .Sum();
                //        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //        foreach(var nhanVien in gioVaoRaFillets)
                //        {
                //            var phanBo = phanBoVDMTs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //            if(phanBo != null)
                //            {
                //                var itemSanluong = new SanLuongPhuFillet()
                //                {
                //                    STT = nhanVien.STT,
                //                    MaNhanVien = nhanVien.MaNhanVien,
                //                    MaThanhPham = vdMTId,
                //                    SanLuongHuong =
                //                    (phanBo.TyLeHuong *
                //                        sanLuongDonVi -
                //                        (phanBo.TyLeHuong * sanLuongDonVi) *
                //                        phanBo.TyLeTru),
                //                    SoGio = soGio,
                //                    SanLuongTru = ((phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru),
                //                    TyLeTru = phanBo.TyLeTru,
                //                    TyLeHuong = phanBo.TyLeHuong
                //                };
                //                sanLuongPhuFillets.Add(itemSanluong);
                //            }
                //        }
                //    }
                //}
                //3 Keo Dau
                var kDId = "KD";
                var nhanVienPhuKD = GetNhanVienByLoaiThanhPhamPhu(kDId, dateTime, xuongId);
                var idsKD = nhanVienPhuKD.Select(x => x.MaNhanVien).Distinct();
                var GioKDs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsKD, kDId, xuongId);
                var phanBoKDs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(kDId, dateTime, xuongId);
                for (int i = 0; i < GioKDs.Count(); i++)
                {
                    if (i == (GioKDs.Count() - 1))
                    {
                        break;
                    }

                    var sanLuongTong = PhieuCanTPFilletv2ViewModel.Instance.GetSanLuongTPFilletv2(GioKDs[i], GioKDs[i + 1], dateTime, xuongId);
                    //var sanLuongSoChe = PhieuCanVm.GetSanLuongSoChe(GioKDs[i],
                    //                                                GioKDs[i + 1],
                    //                                                DateTimeNowShared,
                    //                                                XuongIdShared);
                    //sanLuongTong += sanLuongSoChe;

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioKDs[i],
                        GioKDs[i + 1],
                        idsKD,
                        kDId,
                        xuongId);
                    var soGio = (GioKDs[i + 1] - GioKDs[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoKDs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoKDs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = kDId,
                                    SanLuongHuong =
                                        (phanBo.TyLeHuong *
                                         sanLuongDonVi -
                                         (phanBo.TyLeHuong * sanLuongDonVi) *
                                         phanBo.TyLeTru),
                                    SoGio = soGio,
                                    SanLuongTru = ((phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru),
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Add(itemSanluong);
                            }
                        }
                    }
                }

                //4 Rua Ca
                var ruCId = "RuC";
                var nhanVienPhuRuC = GetNhanVienByLoaiThanhPhamPhu(ruCId,
                    dateTime,
                    xuongId);
                var idsRuC = nhanVienPhuRuC.Select(x => x.MaNhanVien).Distinct();
                var GioRuCs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsRuC, ruCId, xuongId);
                var phanBoRuCs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(ruCId,
                    dateTime,
                    xuongId);
                for (int i = 0; i < GioRuCs.Count(); i++)
                {
                    if (i == (GioRuCs.Count() - 1))
                    {
                        break;
                    }

                    var sanLuongTong = PhieuCanTPFilletv2ViewModel.Instance.GetSanLuongTPFilletTruCaMuoi(
                        GioRuCs[i],
                        GioRuCs[i + 1],
                        dateTime,
                    xuongId);
                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioRuCs[i],
                        GioRuCs[i + 1],
                        idsRuC,
                        ruCId,
                        xuongId);
                    var soGio = (GioRuCs[i + 1] - GioRuCs[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoRuCs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoRuCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = ruCId,
                                    SanLuongHuong =
                                        phanBo.TyLeHuong *
                                        sanLuongDonVi -
                                        (phanBo.TyLeHuong * sanLuongDonVi) *
                                        phanBo.TyLeTru,
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Add(itemSanluong);
                            }
                        }
                    }
                }

                var sanLuongCo = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoCheBatCo(
                    dateTime,
                    xuongId,
                    true);
                //5 Cắt Tiết Ca Lon
                //******************************************************************************************************
                var cTId = "CT";
                var nhanVienPhuCT = GetNhanVienByLoaiThanhPhamPhu(cTId, dateTime, xuongId);
                var phanBoCTs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(cTId, dateTime, xuongId);
                var _nhanVienBanCT = new List<Models.Repos.Models.NhanVienPhuTheoBanFillet>();
                foreach (var item in BanCatTietViewModel.Instance.BanCatTiets)
                {
                    var ns = GetNhanViensPhuByBanCatTiet(item.Ma, dateTime, xuongId);
                    _nhanVienBanCT.AddRange(ns);
                }

                var _nhanViens = new List<Models.Repos.Models.NhanVienPhuTheoBanFillet>();
                foreach (var item in nhanVienPhuCT)
                {
                    var nv = _nhanVienBanCT.FirstOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                    if (nv != null)
                    {
                        _nhanViens.Add(nv);
                    }
                }

                var _idsCT = _nhanViens.Select(x => x.MaNhanVien).Distinct();
                var _GioCTs = GioVaoRaViewModel.Instance.GetListTime(dateTime, _idsCT, cTId, xuongId);

                for (int i = 0; i < _GioCTs.Count(); i++)
                {
                    if (i == (_GioCTs.Count() - 1))
                    {
                        break;
                    }

                    double sanLuongTong = 0;
                    foreach (var item in BanCatTietViewModel.Instance.BanCatTiets)
                    {
                        var numBan = int.Parse(item.Ma.Substring(2, 2));
                        if (numBan >= 9)
                        {
                            continue;
                        }

                        sanLuongTong += PhieuCanNguyenLieuViewModel.Instance.GetSanLuongNguyenLieuTruNgop(
                            _GioCTs[i],
                            _GioCTs[i + 1],
                            dateTime,
                            xuongId,
                            item.Ma,
                            "1");
                    }

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        _GioCTs[i],
                        _GioCTs[i + 1],
                        _idsCT,
                        cTId,
                        xuongId);
                    var soGio = (_GioCTs[i + 1] - _GioCTs[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoCTs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoCTs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = cTId,
                                    SanLuongHuong =
                                        phanBo.TyLeHuong *
                                        sanLuongDonVi -
                                        (phanBo.TyLeHuong * sanLuongDonVi) *
                                        phanBo.TyLeTru,
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Add(itemSanluong);
                            }
                        }
                    }
                }

                //*******************************************************************************************************
                //var cTId = "CT";
                //var nhanVienPhuCT = NhanVienVm.GetNhanVienByLoaiThanhPhamPhu(cTId, DateTimeNowShared, XuongIdShared);
                //var phanBoCTs = NhanVienVm.GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                //    cTId,
                //    MainViewModel.DateTimeNowShared,
                //    MainViewModel.XuongIdShared);
                //foreach (var banCatTiet in BanCatTietViewModel.Ins.BanCatTiets)
                //{
                //    var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                //    if (numBan >= 9)
                //    {
                //        continue;
                //    }
                //    var nhanVienBanCT = NhanVienVm.GetNhanViensPhuByBanCatTiet(
                //        banCatTiet.Ma,
                //        DateTimeNowShared,
                //        XuongIdShared);

                //    var nhanViens = new List<Modelv1.EF.NhanVienPhuTheoBanFillet>();
                //    foreach (var item in nhanVienPhuCT)
                //    {
                //        var nv = nhanVienBanCT.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                //        if (nv != null)
                //        {
                //            nhanViens.Add(nv);
                //        }
                //    }
                //    var idsCT = nhanViens.Select(x => x.MaNhanVien).Distinct();
                //    var GioCTs = GioVaoRaVm.GetListTime(DateTimeNowShared, idsCT, cTId, XuongIdShared);

                //    for (int i = 0; i < GioCTs.Count(); i++)
                //    {
                //        if (i == (GioCTs.Count() - 1))
                //        {
                //            break;
                //        }
                //        var sanLuongTong = PhieuCanVm.GetSanLuongNguyenLieuTruNgop(
                //            GioCTs[i],
                //            GioCTs[i + 1],
                //            MainViewModel.DateTimeNowShared,
                //            XuongIdShared,
                //            banCatTiet.Ma,
                //            "1");
                //        var gioVaoRaFillets = GioVaoRaVm.Get(
                //            MainViewModel.DateTimeNowShared,
                //            GioCTs[i],
                //            GioCTs[i + 1],
                //            idsCT,
                //            cTId,
                //            XuongIdShared);
                //        var soGio = (GioCTs[i + 1] - GioCTs[i]).TotalHours;
                //        var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //        if (gioVaoRaFillets.Any())
                //        {
                //            var tongTyLe = phanBoCTs.Where(x => ids.Contains(x.MaNhanVien))
                //                .Select(x => x.TyLeHuong)
                //                .DefaultIfEmpty(0)
                //                .Sum();
                //            var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //            //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //            foreach (var nhanVien in gioVaoRaFillets)
                //            {
                //                var phanBo = phanBoCTs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //                if (phanBo != null)
                //                {
                //                    var itemSanluong = new SanLuongPhuFillet()
                //                    {
                //                        STT = nhanVien.STT,
                //                        MaNhanVien = nhanVien.MaNhanVien,
                //                        MaThanhPham = cTId,
                //                        SanLuongHuong =
                //                        phanBo.TyLeHuong *
                //                            sanLuongDonVi -
                //                            (phanBo.TyLeHuong * sanLuongDonVi) *
                //                            phanBo.TyLeTru,
                //                        SoGio = soGio,
                //                        SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                //                        TyLeTru = phanBo.TyLeTru,
                //                        TyLeHuong = phanBo.TyLeHuong
                //                    };
                //                    sanLuongPhuFillets.Add(itemSanluong);
                //                }
                //            }
                //        }
                //    }
                //}
                //****************************************************************************************
                //5.1 Cắt Tiết Ca Nho
                var cTNId = "CTN";
                var nhanVienPhuCTN = GetNhanVienByLoaiThanhPhamPhu(
                    cTNId,
                    dateTime,
                    xuongId);
                var phanBoCTNs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    cTNId,
                    dateTime,
                    xuongId);
                var nhanVienBanCTN = new List<Models.Repos.Models.NhanVienPhuTheoBanFillet>();
                foreach (var banCatTiet in BanCatTietViewModel.Instance.BanCatTiets)
                {
                    var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                    if (numBan >= 9)
                    {
                        continue;
                    }

                    var ns = GetNhanViensPhuByBanCatTiet(banCatTiet.Ma, dateTime, xuongId);
                    nhanVienBanCTN.AddRange(ns);
                }

                var nhanViensCTN = new List<Models.Repos.Models.NhanVienPhuTheoBanFillet>();
                foreach (var item in nhanVienPhuCTN)
                {
                    var nv = nhanVienBanCTN.FirstOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                    if (nv != null)
                    {
                        nhanVienBanCTN.Add(nv);
                    }
                }

                var idsCT = nhanVienBanCTN.Select(x => x.MaNhanVien).Distinct();
                var GioCTs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsCT, cTNId, xuongId);

                for (int i = 0; i < GioCTs.Count(); i++)
                {
                    if (i == (GioCTs.Count() - 1))
                    {
                        break;
                    }

                    double sanLuongTong = 0;
                    foreach (var item in BanCatTietViewModel.Instance.BanCatTiets)
                    {
                        var numBan = int.Parse(item.Ma.Substring(2, 2));
                        if (numBan >= 9)
                        {
                            continue;
                        }

                        sanLuongTong += PhieuCanNguyenLieuViewModel.Instance.GetSanLuongNguyenLieuTruNgop(
                            GioCTs[i],
                            GioCTs[i + 1],
                            dateTime,
                            xuongId,
                            item.Ma,
                            "2");
                    }

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioCTs[i],
                        GioCTs[i + 1],
                        idsCT,
                        cTNId,
                        xuongId);
                    var soGio = (GioCTs[i + 1] - GioCTs[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoCTNs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoCTNs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = cTNId,
                                    SanLuongHuong =
                                        phanBo.TyLeHuong *
                                        sanLuongDonVi -
                                        (phanBo.TyLeHuong * sanLuongDonVi) *
                                        phanBo.TyLeTru,
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Add(itemSanluong);
                            }
                        }
                    }
                }


                //6 Rai Ca
                var rCId = "RC";
                var nhanVienPhuRC = GetNhanVienByLoaiThanhPhamPhu(rCId, dateTime, xuongId);
                var phanBoRCs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    rCId,
                    dateTime,
                    xuongId);

                double count = 0;
                foreach (var banCatTiet in BanCatTietViewModel.Instance.BanCatTiets)
                {
                    var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                    if (numBan >= 9)
                    {
                        continue;
                    }

                    var nhanVienBanCT = GetNhanViensPhuByBanCatTiet(
                        banCatTiet.Ma,
                        dateTime,
                        xuongId);
                    var nhanViens = new List<Models.Repos.Models.NhanVienPhuTheoBanFillet>();
                    foreach (var item in nhanVienPhuRC)
                    {
                        var nv = nhanVienBanCT.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                        if (nv != null)
                        {
                            nhanViens.Add(nv);
                        }
                    }

                    var idsRC = nhanViens.Select(x => x.MaNhanVien).Distinct();
                    var GioRCs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsRC, rCId, xuongId);

                    for (int i = 0; i < GioRCs.Count(); i++)
                    {
                        if (i == (GioRCs.Count() - 1))
                        {
                            break;
                        }

                        var sanLuongTong = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongNguyenLieu(
                            GioRCs[i],
                            GioRCs[i + 1],
                            dateTime,
                            xuongId,
                            banCatTiet.Ma);
                        //var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                        //if(numBan == 9)
                        //{
                        //    sanLuongTong = PhieuCanVm.GetSanLuongCaNgop(GioRCs[i],
                        //                                                GioRCs[i + 1],
                        //                                                MainViewModel.DateTimeNowShared,
                        //                                                XuongIdShared,
                        //                                                banCatTiet.Ma);
                        //}
                        var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                            dateTime,
                            GioRCs[i],
                            GioRCs[i + 1],
                            idsRC,
                            rCId,
                            xuongId);
                        var soGio = (GioRCs[i + 1] - GioRCs[i]).TotalHours;
                        var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                        if (gioVaoRaFillets.Any())
                        {
                            var tongTyLe = phanBoRCs.Where(x => ids.Contains(x.MaNhanVien))
                                .Select(x => x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                            //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                            foreach (var nhanVien in gioVaoRaFillets)
                            {
                                var phanBo = phanBoRCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                                if (phanBo != null)
                                {
                                    var itemSanluong = new SanLuongPhuFillet()
                                    {
                                        STT = nhanVien.STT,
                                        MaNhanVien = nhanVien.MaNhanVien,
                                        MaThanhPham = rCId,
                                        SanLuongHuong =
                                            phanBo.TyLeHuong *
                                            sanLuongDonVi -
                                            (phanBo.TyLeHuong * sanLuongDonVi) *
                                            phanBo.TyLeTru,
                                        SoGio = soGio,
                                        SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                        TyLeTru = phanBo.TyLeTru,
                                        TyLeHuong = phanBo.TyLeHuong
                                    };
                                    sanLuongPhuFillets.Add(itemSanluong);
                                    count += itemSanluong.SanLuongHuong;
                                }
                            }
                        }
                    }
                }

                //7 Bat Ca
                var bCId = "BC";
                var phanBoBCs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    bCId,
                    dateTime,
                    xuongId);
                var nhanVienPhuBC = GetNhanVienByLoaiThanhPhamPhu(bCId, dateTime, xuongId);
                double sanLuongNguyenLieu = 0;
                foreach (var banCatTiet in BanCatTietViewModel.Instance.BanCatTiets)
                {
                    var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                    if (numBan >= 9)
                    {
                        continue;
                    }

                    var sanLuongNguyenLieuBan = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongNguyenLieu(
                        dateTime,
                        xuongId,
                        banCatTiet.Ma);

                    sanLuongNguyenLieu += sanLuongNguyenLieuBan;
                    //var nhanVienBanCT = NhanVienVm.GetNhanViensPhuByBanCatTiet(banCatTiet.Ma,
                    //                                                           DateTimeNowShared,
                    //                                                           XuongIdShared);

                    //var nhanViens = new List<Modelv1.EF.NhanVienPhuTheoBanFillet>();
                    //foreach(var nhanVienDaiThanh in nhanVienPhuBC)
                    //{
                    //    var nv = nhanVienBanCT.SingleOrDefault(x => x.MaNhanVien == nhanVienDaiThanh.MaNhanVien);
                    //    if(nv != null)
                    //    {
                    //        nhanViens.Add(nv);
                    //    }
                    //}
                    //var idsBC = nhanViens.Select(x => x.MaNhanVien).Distinct();
                    //var GioBCs = GioVaoRaVm.GetListTime(DateTimeNowShared, idsBC, bCId);

                    //for(int i = 0; i < GioBCs.Count(); i++)
                    //{
                    //    if(i == (GioBCs.Count() - 1))
                    //    {
                    //        break;
                    //    }
                    //    var sanLuongTong = PhieuCanVm.GetSanLuongNguyenLieu(GioBCs[i],
                    //                                                        GioBCs[i + 1],
                    //                                                        MainViewModel.DateTimeNowShared,
                    //                                                        XuongIdShared,
                    //                                                        banCatTiet.Ma);
                    //    var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                    //                                         GioBCs[i],
                    //                                         GioBCs[i + 1],
                    //                                         idsBC,
                    //                                         bCId);
                    //    var soGio = (GioBCs[i + 1] - GioBCs[i]).TotalHours;
                    //    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    //    if(gioVaoRaFillets.Any())
                    //    {
                    //        var tongTyLe = phanBoBCs.Where(x => ids.Contains(x.MaNhanVien))
                    //            .Select(x => x.TyLeHuong)
                    //            .DefaultIfEmpty(0)
                    //            .Sum();
                    //        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                    //        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                    //        foreach(var nhanVien in gioVaoRaFillets)
                    //        {
                    //            var phanBo = phanBoBCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                    //            if(phanBo != null)
                    //            {
                    //                var itemSanluong = new SanLuongPhuFillet()
                    //                {
                    //                    STT = nhanVien.STT,
                    //                    MaNhanVien = nhanVien.MaNhanVien,
                    //                    MaThanhPham = bCId,
                    //                    SanLuongHuong =
                    //                    phanBo.TyLeHuong *
                    //                        sanLuongDonVi -
                    //                        (phanBo.TyLeHuong * sanLuongDonVi) *
                    //                        phanBo.TyLeTru,
                    //                    SoGio = soGio,
                    //                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                    //                    TyLeTru = phanBo.TyLeTru,
                    //                    TyLeHuong = phanBo.TyLeHuong
                    //                };
                    //                sanLuongPhuFillets.Add(itemSanluong);
                    //            }
                    //        }
                    //    }
                    //}
                }

                var sanLuongCanSoCheBatCa = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                    dateTime,
                    xuongId,
                    true);
                if (nhanVienPhuBC.Any())
                {
                    if (phanBoBCs.Any())
                    {
                        var nhanVienIds = nhanVienPhuBC.Select(x => x.MaNhanVien).Distinct().ToList();
                        var tongTrongLuong = sanLuongNguyenLieu - sanLuongCanSoCheBatCa;
                        var GioBCs = GioVaoRaViewModel.Instance.Gets(dateTime, bCId, nhanVienIds, xuongId);
                        double tongGios = 0;
                        var sanLuongBCs = new List<SanLuongPhuFillet>();
                        foreach (var phanBoBC in phanBoBCs)
                        {
                            var soGio = GioBCs.Where(x => x.MaNhanVien == phanBoBC.MaNhanVien)
                                .Select(x => (x.GioRa.Value - x.GioVao).TotalHours)
                                .DefaultIfEmpty(0)
                                .Sum();
                            if (soGio > 0)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    MaNhanVien = phanBoBC.MaNhanVien,
                                    MaThanhPham = bCId,
                                    SoGio = soGio,
                                    TyLeTru = phanBoBC.TyLeTru,
                                    TyLeHuong = phanBoBC.TyLeHuong,
                                    GioTyLe = phanBoBC.TyLeHuong * soGio
                                };
                                sanLuongBCs.Add(itemSanluong);
                            }

                            tongGios += soGio;
                        }

                        var tongGioTyLe = sanLuongBCs.Select(x => x.TyLeHuong * x.SoGio).DefaultIfEmpty(0).Sum();
                        var sanLuongDonVi = tongGioTyLe == 0 ? 0 : tongTrongLuong / tongGioTyLe;
                        sanLuongBCs.All(
                            x =>
                            {
                                x.SanLuongHuong = x.GioTyLe * sanLuongDonVi - x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                                x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                                return true;
                            });
                        sanLuongPhuFillets.AddRange(sanLuongBCs);
                    }
                }

                //7.1 Bat Ca Ban 9
                var bC09Id = "BC09";
                var phanBoBC09s = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    bC09Id,
                    dateTime,
                    xuongId);
                var nhanVienPhuBC09 = GetNhanVienByLoaiThanhPhamPhu(
                    bC09Id,
                    dateTime,
                    xuongId);
                double sanLuongNguyenLieu09 = 0;
                foreach (var banCatTiet in BanCatTietViewModel.Instance.BanCatTiets)
                {
                    var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                    if (numBan != 9)
                    {
                        continue;
                    }

                    var sanLuongNguyenLieuBan = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongNguyenLieu(dateTime, xuongId, banCatTiet.Ma);

                    sanLuongNguyenLieu09 += sanLuongNguyenLieuBan;
                }

                var sanLuongCanSoCheBatCa09 = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                    dateTime,
                    xuongId,
                    false,
                    true);
                if (nhanVienPhuBC09.Any())
                {
                    if (phanBoBC09s.Any())
                    {
                        var nhanVienIds = nhanVienPhuBC09.Select(x => x.MaNhanVien).Distinct().ToList();
                        var tongTrongLuong = sanLuongNguyenLieu09 - sanLuongCanSoCheBatCa09;
                        if (tongTrongLuong > 0)
                        {
                        }

                        var GioBCs = GioVaoRaViewModel.Instance.Gets(dateTime, bC09Id, nhanVienIds, xuongId);
                        double tongGios = 0;
                        var sanLuongBC09s = new List<SanLuongPhuFillet>();
                        foreach (var phanBoBC09 in phanBoBC09s)
                        {
                            var soGio = GioBCs.Where(x => x.MaNhanVien == phanBoBC09.MaNhanVien)
                                .Select(x => (x.GioRa.Value - x.GioVao).TotalHours)
                                .DefaultIfEmpty(0)
                                .Sum();
                            if (soGio > 0)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    MaNhanVien = phanBoBC09.MaNhanVien,
                                    MaThanhPham = bC09Id,
                                    SoGio = soGio,
                                    TyLeTru = phanBoBC09.TyLeTru,
                                    TyLeHuong = phanBoBC09.TyLeHuong,
                                    GioTyLe = phanBoBC09.TyLeHuong * soGio
                                };
                                sanLuongBC09s.Add(itemSanluong);
                            }

                            tongGios += soGio;
                        }

                        var tongGioTyLe = sanLuongBC09s.Select(x => x.TyLeHuong * x.SoGio).DefaultIfEmpty(0).Sum();
                        var sanLuongDonVi = tongGioTyLe == 0 ? 0 : tongTrongLuong / tongGioTyLe;
                        sanLuongBC09s.All(
                            x =>
                            {
                                x.SanLuongHuong = x.GioTyLe * sanLuongDonVi - x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                                x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                                return true;
                            });
                        sanLuongPhuFillets.AddRange(sanLuongBC09s);
                    }
                }
                //------------------------------------------------------


                //foreach(var banCatTiet in BanCatTietViewModel.Ins.BanCatTiets)
                //{
                //    if(banCatTiet.Ma != "CT09")
                //    {
                //        continue;
                //    }
                //    var nhanVienBanCT = NhanVienVm.GetNhanViensPhuByBanCatTiet(banCatTiet.Ma,
                //                                                               DateTimeNowShared,
                //                                                               XuongIdShared);
                //    var nhanViens = new List<Modelv1.EF.NhanVienPhuTheoBanFillet>();
                //    foreach(var nhanVienDaiThanh in nhanVienPhuBC09)
                //    {
                //        var nv = nhanVienBanCT.SingleOrDefault(x => x.MaNhanVien == nhanVienDaiThanh.MaNhanVien);
                //        if(nv != null)
                //        {
                //            nhanViens.Add(nv);
                //        }
                //    }
                //    var idsBC = nhanViens.Select(x => x.MaNhanVien).Distinct();
                //    var GioBCs = GioVaoRaVm.GetListTime(DateTimeNowShared, idsBC, bC09Id);

                //    for(int i = 0; i < GioBCs.Count(); i++)
                //    {
                //        if(i == (GioBCs.Count() - 1))
                //        {
                //            break;
                //        }
                //        var sanLuongTong = PhieuCanVm.GetSanLuongNguyenLieu(GioBCs[i],
                //                                                            GioBCs[i + 1],
                //                                                            MainViewModel.DateTimeNowShared,
                //                                                            XuongIdShared,
                //                                                            banCatTiet.Ma);
                //        var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                //                                             GioBCs[i],
                //                                             GioBCs[i + 1],
                //                                             idsBC,
                //                                             bC09Id);
                //        var soGio = (GioBCs[i + 1] - GioBCs[i]).TotalHours;
                //        var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //        if(gioVaoRaFillets.Any())
                //        {
                //            var tongTyLe = phanBoBC09s.Where(x => ids.Contains(x.MaNhanVien))
                //                .Select(x => x.TyLeHuong)
                //                .DefaultIfEmpty(0)
                //                .Sum();
                //            var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //            //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //            foreach(var nhanVien in gioVaoRaFillets)
                //            {
                //                var phanBo = phanBoBC09s.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //                if(phanBo != null)
                //                {
                //                    var itemSanluong = new SanLuongPhuFillet()
                //                    {
                //                        STT = nhanVien.STT,
                //                        MaNhanVien = nhanVien.MaNhanVien,
                //                        MaThanhPham = bC09Id,
                //                        SanLuongHuong =
                //                        phanBo.TyLeHuong *
                //                            sanLuongDonVi -
                //                            (phanBo.TyLeHuong * sanLuongDonVi) *
                //                            phanBo.TyLeTru,
                //                        SoGio = soGio,
                //                        SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                //                        TyLeTru = phanBo.TyLeTru,
                //                        TyLeHuong = phanBo.TyLeHuong
                //                    };
                //                    sanLuongPhuFillets.Add(itemSanluong);
                //                }
                //            }
                //        }
                //    }
                //}
                //8 Phuc Vu
                var pVId = "PV";
                var lines = LineFilletViewModel.Instance.Get(xuongId);

                if (IsPhucVuCaNhan == true)
                {
                    var items = PhieuCanTPFilletv2ViewModel.Instance.GetPhieuCanTongHopNhanVienPhucVusv2<SanLuongPhuFillet>(
                        dateTime,
                        pVId,
                        xuongId,
                        true);
                    sanLuongPhuFillets.AddRange(items);
                }
                else
                {
                    var phanBoPVs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                        pVId,
                        dateTime,
                        xuongId);
                    var nhanVienPhuPV = GetNhanVienByLoaiThanhPhamPhu(
                        pVId,
                        dateTime,
                        xuongId);

                    foreach (var line in lines)
                    {
                        var nhanVienLine = GetNhanViensPhuByLine(
                            line.Ma,
                            dateTime,
                        xuongId,
                            pVId);
                        var nhanViens = new List<NhanVienPhuTheoLine>();
                        foreach (var item in nhanVienPhuPV)
                        {
                            var nv = nhanVienLine.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                            if (nv != null)
                            {
                                nhanViens.Add(nv);
                            }
                        }

                        var idsPV = nhanViens.Select(x => x.MaNhanVien).Distinct();
                        var GioBCs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsPV, pVId, xuongId);

                        for (int i = 0; i < GioBCs.Count(); i++)
                        {
                            if (i == (GioBCs.Count() - 1))
                            {
                                break;
                            }

                            var nhanVienSanLuongs = GetNhanViensSanLuong(
                                dateTime,
                                line.Ma,
                                xuongId);
                            var idsSanLuong = nhanVienSanLuongs.Select(x => x.MaNhanVien);
                            var sanLuongTong = PhieuCanTPFilletv2ViewModel.Instance.GetSanLuongTPFillet(
                                GioBCs[i],
                                GioBCs[i + 1],
                                dateTime,
                                xuongId,
                                idsSanLuong);

                            var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                                dateTime,
                                GioBCs[i],
                                GioBCs[i + 1],
                                idsPV,
                                pVId,
                                xuongId);
                            var soGio = (GioBCs[i + 1] - GioBCs[i]).TotalHours;
                            var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                            if (gioVaoRaFillets.Any())
                            {
                                var tongTyLe = phanBoPVs.Where(x => ids.Contains(x.MaNhanVien))
                                    .Select(x => x.TyLeHuong)
                                    .DefaultIfEmpty(0)
                                    .Sum();
                                var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                                //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                                foreach (var nhanVien in gioVaoRaFillets)
                                {
                                    var phanBo =
                                        phanBoPVs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                                    if (phanBo != null)
                                    {
                                        var itemSanluong = new SanLuongPhuFillet()
                                        {
                                            STT = nhanVien.STT,
                                            MaNhanVien = nhanVien.MaNhanVien,
                                            MaThanhPham = pVId,
                                            SanLuongHuong =
                                                phanBo.TyLeHuong *
                                                sanLuongDonVi -
                                                (phanBo.TyLeHuong * sanLuongDonVi) *
                                                phanBo.TyLeTru,
                                            SoGio = soGio,
                                            SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                            TyLeTru = phanBo.TyLeTru,
                                            TyLeHuong = phanBo.TyLeHuong
                                        };
                                        sanLuongPhuFillets.Add(itemSanluong);
                                    }
                                }
                            }
                        }
                    }
                }

                //9 Lang Da Ca Lon
                var lDId = "LD";

                var nhanVienPhuLD = GetNhanVienByLoaiThanhPhamPhu(lDId, dateTime, xuongId);
                var phanBoLDs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    lDId,
                    dateTime,
                    xuongId);

                foreach (var line in lines)
                {
                    var nhanVienLine = GetNhanViensPhuByLine(
                        line.Ma,
                        dateTime,
                        xuongId,
                        lDId);
                    var nhanViens = new List<NhanVienPhuTheoLine>();
                    foreach (var item in nhanVienPhuLD)
                    {
                        var nv = nhanVienLine.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                        if (nv != null)
                        {
                            nhanViens.Add(nv);
                        }
                    }

                    var idsLD = nhanViens.Select(x => x.MaNhanVien).Distinct();
                    var GioLDs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsLD, lDId, xuongId);

                    for (int i = 0; i < GioLDs.Count(); i++)
                    {
                        if (i == (GioLDs.Count() - 1))
                        {
                            break;
                        }

                        var mayLangDa = "M1";
                        if (line.Ma == "002")
                        {
                            mayLangDa = "M2";
                        }

                        var sanLuongTong = PhieuCanBTPDinhHinhViewModel.Instance.GetSanLuongBTPDinhHinh(
                            GioLDs[i],
                            GioLDs[i + 1],
                            dateTime,
                            xuongId,
                            mayLangDa,
                            "'G','H','003','004','R'",
                            "1");
                        //slLangDa += sanLuongTong;
                        //var msls = PhieuCanVm.GetMSLs(GioLDs[i],
                        //                              GioLDs[i + 1],
                        //                              MainViewModel.DateTimeNowShared,
                        //                              XuongIdShared,
                        //                              "1");
                        //var sanLuongSoChe = PhieuCanVm.GetSanLuongSoChe(GioLDs[i],
                        //                                                GioLDs[i + 1],
                        //                                                MainViewModel.DateTimeNowShared,
                        //                                                XuongIdShared,
                        //                                                msls,
                        //                                                true);
                        //sanLuongTong += sanLuongSoChe;
                        var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                            dateTime,
                            GioLDs[i],
                            GioLDs[i + 1],
                            idsLD,
                            lDId,
                            xuongId);
                        var soGio = (GioLDs[i + 1] - GioLDs[i]).TotalHours;
                        var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                        if (gioVaoRaFillets.Any())
                        {
                            var tongTyLe = phanBoLDs.Where(x => ids.Contains(x.MaNhanVien))
                                .Select(x => x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                            //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                            foreach (var nhanVien in gioVaoRaFillets)
                            {
                                var phanBo = phanBoLDs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                                if (phanBo != null)
                                {
                                    var itemSanluong = new SanLuongPhuFillet()
                                    {
                                        STT = nhanVien.STT,
                                        MaNhanVien = nhanVien.MaNhanVien,
                                        MaThanhPham = lDId,
                                        SanLuongHuong =
                                            phanBo.TyLeHuong *
                                            sanLuongDonVi -
                                            (phanBo.TyLeHuong * sanLuongDonVi) *
                                            phanBo.TyLeTru,
                                        SoGio = soGio,
                                        SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                        TyLeTru = phanBo.TyLeTru,
                                        TyLeHuong = phanBo.TyLeHuong
                                    };
                                    sanLuongPhuFillets.Add(itemSanluong);
                                }
                            }
                        }
                    }
                }

                //9.1 Lang Da Ca Nho
                var lDNId = "LDN";
                var nhanVienPhuLDN = GetNhanVienByLoaiThanhPhamPhu(
                    lDNId,
                    dateTime,
                    xuongId);
                var phanBoLDNs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    lDNId,
                    dateTime,
                    xuongId);

                foreach (var line in lines)
                {
                    var nhanVienLine = GetNhanViensPhuByLine(
                        line.Ma,
                        dateTime,
                        xuongId,
                        lDNId);
                    var nhanViens = new List<NhanVienPhuTheoLine>();
                    foreach (var item in nhanVienPhuLDN)
                    {
                        var nv = nhanVienLine.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                        if (nv != null)
                        {
                            nhanViens.Add(nv);
                        }
                    }

                    var idsLD = nhanViens.Select(x => x.MaNhanVien).Distinct();
                    var GioLDs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsLD, lDNId, xuongId);

                    for (int i = 0; i < GioLDs.Count(); i++)
                    {
                        if (i == (GioLDs.Count() - 1))
                        {
                            break;
                        }

                        var mayLangDa = "M1";
                        if (line.Ma == "002")
                        {
                            mayLangDa = "M2";
                        }

                        var sanLuongTong = PhieuCanBTPDinhHinhViewModel.Instance.GetSanLuongBTPDinhHinh(
                            GioLDs[i],
                            GioLDs[i + 1],
                            dateTime,
                            xuongId,
                            mayLangDa,
                            "'G','H','003','004','R'",
                            "2");
                        //var msls = PhieuCanVm.GetMSLs(GioLDs[i],
                        //                              GioLDs[i + 1],
                        //                              MainViewModel.DateTimeNowShared,
                        //                              XuongIdShared,
                        //                              "2");
                        //var sanLuongSoChe = PhieuCanVm.GetSanLuongSoChe(GioLDs[i],
                        //                                                GioLDs[i + 1],
                        //                                                MainViewModel.DateTimeNowShared,
                        //                                                XuongIdShared,
                        //                                                msls,
                        //                                                true);
                        //sanLuongTong += sanLuongSoChe;
                        var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                            dateTime,
                            GioLDs[i],
                            GioLDs[i + 1],
                            idsLD,
                            lDNId,
                            xuongId);
                        var soGio = (GioLDs[i + 1] - GioLDs[i]).TotalHours;
                        var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                        if (gioVaoRaFillets.Any())
                        {
                            var tongTyLe = phanBoLDNs.Where(x => ids.Contains(x.MaNhanVien))
                                .Select(x => x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                            //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                            foreach (var nhanVien in gioVaoRaFillets)
                            {
                                var phanBo = phanBoLDNs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                                if (phanBo != null)
                                {
                                    var itemSanluong = new SanLuongPhuFillet()
                                    {
                                        STT = nhanVien.STT,
                                        MaNhanVien = nhanVien.MaNhanVien,
                                        MaThanhPham = lDNId,
                                        SanLuongHuong =
                                            phanBo.TyLeHuong *
                                            sanLuongDonVi -
                                            (phanBo.TyLeHuong * sanLuongDonVi) *
                                            phanBo.TyLeTru,
                                        SoGio = soGio,
                                        SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                        TyLeTru = phanBo.TyLeTru,
                                        TyLeHuong = phanBo.TyLeHuong
                                    };
                                    sanLuongPhuFillets.Add(itemSanluong);
                                }
                            }
                        }
                    }
                }

                //10 Can Ca
                var cCId = "CC";
                double trongluongdebug = 0;
                var nhanVienPhuCC = GetNhanVienByLoaiThanhPhamPhu(cCId, dateTime, xuongId);
                var phanBoCCs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(
                    cCId,
                    dateTime,
                    xuongId);
                foreach (var line in lines)
                {
                    var nhanVienLine = GetNhanViensPhuByLine(
                        line.Ma,
                        dateTime,
                        xuongId,
                        cCId);
                    var nhanViens = new List<NhanVienPhuTheoLine>();
                    foreach (var item in nhanVienPhuCC)
                    {
                        var nv = nhanVienLine.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                        if (nv != null)
                        {
                            nhanViens.Add(nv);
                        }
                    }

                    var idsCC = nhanViens.Select(x => x.MaNhanVien).Distinct();
                    var GioCCs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsCC, cCId, xuongId);

                    for (int i = 0; i < GioCCs.Count(); i++)
                    {
                        if (i == (GioCCs.Count() - 1))
                        {
                            break;
                        }

                        var mayLangDa = "M1";
                        if (line.Ma == "002")
                        {
                            mayLangDa = "M2";
                        }

                        var sanLuongTong = PhieuCanBTPDinhHinhViewModel.Instance.GetSanLuongBTPDinhHinh(
                            GioCCs[i],
                            GioCCs[i + 1],
                            dateTime,
                            xuongId,
                            mayLangDa,
                            "'003', '004'");
                        trongluongdebug += sanLuongTong;
                        var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                            dateTime,
                            GioCCs[i],
                            GioCCs[i + 1],
                            idsCC,
                            cCId,
                            xuongId);
                        var soGio = (GioCCs[i + 1] - GioCCs[i]).TotalHours;
                        var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                        if (gioVaoRaFillets.Any())
                        {
                            var tongTyLe = phanBoCCs.Where(x => ids.Contains(x.MaNhanVien))
                                .Select(x => x.TyLeHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                            //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                            foreach (var nhanVien in gioVaoRaFillets)
                            {
                                var phanBo = phanBoCCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                                if (phanBo != null)
                                {
                                    var itemSanluong = new SanLuongPhuFillet()
                                    {
                                        STT = nhanVien.STT,
                                        MaNhanVien = nhanVien.MaNhanVien,
                                        MaThanhPham = cCId,
                                        SanLuongHuong =
                                            phanBo.TyLeHuong *
                                            sanLuongDonVi -
                                            (phanBo.TyLeHuong * sanLuongDonVi) *
                                            phanBo.TyLeTru,
                                        SoGio = soGio,
                                        SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                        TyLeTru = phanBo.TyLeTru,
                                        TyLeHuong = phanBo.TyLeHuong
                                    };
                                    sanLuongPhuFillets.Add(itemSanluong);
                                }
                            }
                        }
                    }
                }

                //11 Phuc Vu Da
                var pVDId = "PVD";
                var phanBoPVDs = GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(pVDId, dateTime, xuongId);
                var nhanVienPhuPVD = GetNhanVienByLoaiThanhPhamPhu(pVDId, dateTime, xuongId);


                var idsPVD = nhanVienPhuPVD.Select(x => x.MaNhanVien).Distinct();
                var GioPVDs = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsPVD, pVDId, xuongId);

                for (int i = 0; i < GioPVDs.Count(); i++)
                {
                    if (i == (GioPVDs.Count() - 1))
                    {
                        break;
                    }

                    var sanLuongTong = PhieuCanTPFilletv2ViewModel.Instance.GetSanLuongTPFilletTruCaMuoi(
                        GioPVDs[i],
                        GioPVDs[i + 1],
                       dateTime, xuongId);
                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioPVDs[i],
                        GioPVDs[i + 1],
                        idsPVD,
                        pVDId,
                        xuongId);
                    var soGio = (GioPVDs[i + 1] - GioPVDs[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoPVDs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoPVDs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = pVDId,
                                    SanLuongHuong =
                                        phanBo.TyLeHuong *
                                        sanLuongDonVi -
                                        (phanBo.TyLeHuong * sanLuongDonVi) *
                                        phanBo.TyLeTru,
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Add(itemSanluong);
                            }
                        }
                    }
                }


                //-------------
                var finaRl = sanLuongPhuFillets.GroupBy(x => new { x.MaNhanVien, x.MaThanhPham })
                    .Select(
                        g => new SanLuongPhuFillet()
                        {
                            MaNhanVien = g.Key.MaNhanVien,
                            MaThanhPham = g.Key.MaThanhPham,
                            SanLuongHuong = g.Sum(x => x.SanLuongHuong),
                            SoGio = g.Average(x => x.SoGio),
                            SanLuongTru = g.Sum(x => x.SanLuongTru),
                            TyLeHuong = g.Average(x => x.TyLeHuong),
                            TyLeTru = g.Average(x => x.TyLeTru),
                        })
                    .OrderByDescending(x => x.MaNhanVien)
                    .ToList();
                var gioVaoRas = GioVaoRaViewModel.Instance.Get(dateTime, xuongId);
                foreach (var item in finaRl)
                {
                    var gioNhanViens = gioVaoRas.Where(
                            x => x.MaNhanVien == item.MaNhanVien && x.MaCongViec == item.MaThanhPham)
                        .ToList();
                    double soGio = 0;
                    foreach (var itemGio in gioNhanViens)
                    {
                        if (itemGio.GioRa != null)
                        {
                            soGio += (itemGio.GioRa.Value - itemGio.GioVao).TotalHours;
                        }
                    }

                    sanLuongPhuFillets.Add(
                        new SanLuongPhuFillet()
                        {
                            GioTyLe = item.GioTyLe,
                            MaNhanVien = item.MaNhanVien,
                            MaThanhPham = item.MaThanhPham,
                            SanLuongHuong = Math.Round((item.SanLuongHuong), 2),
                            SanLuongTrenGio = item.SanLuongTrenGio,
                            SanLuongTru = item.SanLuongTru,
                            SoGio = soGio,
                            STT = item.STT,
                            TyLeHuong = item.TyLeHuong,
                            TyLeTru = item.TyLeTru,
                        });
                }

                var sanLuongCaNgopTruBan = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongCaNgopTruBan(
                    dateTime,
                    xuongId,
                    "1");
                var soGioCaNgopTyLeTruBan = sanLuongPhuFillets.Where(x => x.MaThanhPham == cTId)
                    .Select(x => x.SoGio * x.TyLeHuong)
                    .DefaultIfEmpty(0)
                    .Sum();
                var sanLuongTrenGioCaNgopTruBan = soGioCaNgopTyLeTruBan == 0
                    ? 0
                    : sanLuongCaNgopTruBan / soGioCaNgopTyLeTruBan;
                sanLuongPhuFillets.Where(x => x.MaThanhPham == cTId)
                    .All(
                        x =>
                        {
                            x.SanLuongHuong += Math.Round(x.SoGio * x.TyLeHuong * sanLuongTrenGioCaNgopTruBan, 2);
                            return true;
                        });


                var sanLuongCaNgopTruBanN = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongCaNgopTruBan(
                    dateTime,
                    xuongId,
                    "2");
                var soGioCaNgopTyLeTruBanN = sanLuongPhuFillets.Where(x => x.MaThanhPham == cTNId)
                    .Select(x => x.SoGio * x.TyLeHuong)
                    .DefaultIfEmpty(0)
                    .Sum();
                var sanLuongTrenGioCaNgopTruBanN = soGioCaNgopTyLeTruBanN == 0
                    ? 0
                    : sanLuongCaNgopTruBanN / soGioCaNgopTyLeTruBanN;
                sanLuongPhuFillets.Where(x => x.MaThanhPham == cTNId)
                    .All(
                        x =>
                        {
                            x.SanLuongHuong += Math.Round(x.SoGio * x.TyLeHuong * sanLuongTrenGioCaNgopTruBanN, 2);
                            return true;
                        });
                //Tinh Lan 2

                //BatCa
                //var nhanVienBatCaIds = phanBoBCs.Select(x => x.MaNhanVien).Distinct();
                //var GioBC2s = GioVaoRaVm.GetListTime(DateTimeNowShared, nhanVienBatCaIds, bCId);
                //for(int i = 0; i < GioBC2s.Count(); i++)
                //{
                //    if(i == (GioBC2s.Count() - 1))
                //    {
                //        break;
                //    }
                //    var sanLuongTong = PhieuCanVm.GetSanLuongSoChe(GioBC2s[i],
                //                                                   GioBC2s[i + 1],
                //                                                   MainViewModel.DateTimeNowShared,
                //                                                   XuongIdShared,
                //                                                   true);
                //    var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                //                                         GioBC2s[i],
                //                                         GioBC2s[i + 1],
                //                                         nhanVienBatCaIds,
                //                                         bCId);
                //    var soGio = (GioBC2s[i + 1] - GioBC2s[i]).TotalHours;
                //    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //    if(gioVaoRaFillets.Any())
                //    {
                //        var tongTyLe = phanBoBCs.Where(x => ids.Contains(x.MaNhanVien))
                //            .Select(x => x.TyLeHuong)
                //            .DefaultIfEmpty(0)
                //            .Sum();
                //        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //        foreach(var nhanVien in gioVaoRaFillets)
                //        {
                //            var phanBo = phanBoBCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //            if(phanBo != null)
                //            {
                //                var itemSanluong = new SanLuongPhuFillet()
                //                {
                //                    STT = nhanVien.STT,
                //                    MaNhanVien = nhanVien.MaNhanVien,
                //                    MaThanhPham = bCId,
                //                    SanLuongHuong =
                //                    phanBo.TyLeHuong *
                //                        sanLuongDonVi,
                //                    SoGio = soGio,
                //                    TyLeTru = phanBo.TyLeTru,
                //                    TyLeHuong = phanBo.TyLeHuong
                //                };
                //                SanLuongPhuFillets.Where(x => x.MaThanhPham == bCId &&
                //                    x.MaNhanVien == itemSanluong.MaNhanVien)
                //                    .All(x =>
                //                    {
                //                        x.SanLuongHuong -= itemSanluong.SanLuongHuong;
                //                        return true;
                //                    });
                //            }
                //        }
                //    }
                //}
                //BatCa09
                //var nhanVienBatCa09Ids = phanBoBC09s.Select(x => x.MaNhanVien).Distinct();
                //var GioBC092s = GioVaoRaVm.GetListTime(DateTimeNowShared, nhanVienBatCa09Ids, bC09Id);
                //for(int i = 0; i < GioBC092s.Count(); i++)
                //{
                //    if(i == (GioBC092s.Count() - 1))
                //    {
                //        break;
                //    }
                //    var sanLuongTong = PhieuCanVm.GetSanLuongSoChe(GioBC092s[i],
                //                                                   GioBC092s[i + 1],
                //                                                   MainViewModel.DateTimeNowShared,
                //                                                   XuongIdShared,
                //                                                   false,
                //                                                   true);
                //    var gioVaoRaFillets = GioVaoRaVm.Get(MainViewModel.DateTimeNowShared,
                //                                         GioBC092s[i],
                //                                         GioBC092s[i + 1],
                //                                         nhanVienBatCa09Ids,
                //                                         bC09Id);
                //    var soGio = (GioBC092s[i + 1] - GioBC092s[i]).TotalHours;
                //    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                //    if(gioVaoRaFillets.Any())
                //    {
                //        var tongTyLe = phanBoBC09s.Where(x => ids.Contains(x.MaNhanVien))
                //            .Select(x => x.TyLeHuong)
                //            .DefaultIfEmpty(0)
                //            .Sum();
                //        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                //        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                //        foreach(var nhanVien in gioVaoRaFillets)
                //        {
                //            var phanBo = phanBoBC09s.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                //            if(phanBo != null)
                //            {
                //                var itemSanluong = new SanLuongPhuFillet()
                //                {
                //                    STT = nhanVien.STT,
                //                    MaNhanVien = nhanVien.MaNhanVien,
                //                    MaThanhPham = bC09Id,
                //                    SanLuongHuong =
                //                    phanBo.TyLeHuong *
                //                        sanLuongDonVi,
                //                    SoGio = soGio,
                //                    TyLeTru = phanBo.TyLeTru,
                //                    TyLeHuong = phanBo.TyLeHuong
                //                };
                //                SanLuongPhuFillets.Where(x => x.MaThanhPham == bC09Id &&
                //                    x.MaNhanVien == itemSanluong.MaNhanVien)
                //                    .All(x =>
                //                    {
                //                        x.SanLuongHuong -= itemSanluong.SanLuongHuong;
                //                        return true;
                //                    });
                //            }
                //        }
                //    }
                //}


                // Can Ca

                var idsCC2 = phanBoCCs.Select(x => x.MaNhanVien).Distinct();
                var GioCCs2 = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsCC2, cCId, xuongId);

                for (int i = 0; i < GioCCs2.Count(); i++)
                {
                    if (i == (GioCCs2.Count() - 1))
                    {
                        break;
                    }

                    var sanLuongTong = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioCCs2[i],
                        GioCCs2[i + 1],
                        dateTime,
                        xuongId,
                        false,
                        false,
                        true,
                        false,
                        false);

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioCCs2[i],
                        GioCCs2[i + 1],
                        idsCC2,
                        cCId,
                        xuongId);
                    var soGio = (GioCCs2[i + 1] - GioCCs2[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoCCs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoCCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = cCId,
                                    SanLuongHuong =
                                        Math.Round(
                                            phanBo.TyLeHuong *
                                            sanLuongDonVi -
                                            (phanBo.TyLeHuong * sanLuongDonVi) *
                                            phanBo.TyLeTru,
                                            2),
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Where(
                                        x => x.MaThanhPham == cCId && x.MaNhanVien == itemSanluong.MaNhanVien)
                                    .All(
                                        x =>
                                        {
                                            x.SanLuongHuong += Math.Round(itemSanluong.SanLuongHuong, 2);
                                            return true;
                                        });
                            }
                        }
                    }
                }

                // Lang Da Lon
                //double tl = 0;
                var idsLDL2 = phanBoLDs.Select(x => x.MaNhanVien).Distinct();
                var GioLDLs2 = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsLDL2, lDId, xuongId);

                for (int i = 0; i < GioLDLs2.Count(); i++)
                {
                    if (i == (GioLDLs2.Count() - 1))
                    {
                        break;
                    }

                    var msls = PhieuCanNguyenLieuViewModel.Instance.GetMSLs(
                        GioLDLs2[i],
                        GioLDLs2[i + 1],
                        dateTime,
                        xuongId,
                        "1");
                    var sanLuongTong = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDLs2[i],
                        GioLDLs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        false,
                        false,
                        true);
                    var sanLuongTruocLangDa = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDLs2[i],
                        GioLDLs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        true,
                        false,
                        true);
                    var sanLuongSauLangDa = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDLs2[i],
                        GioLDLs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        false,
                        true,
                        false);
                    var sanLuongSauLangDaNhan = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDLs2[i],
                        GioLDLs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        false,
                        true,
                        true);
                    sanLuongTong += sanLuongTruocLangDa + sanLuongSauLangDa - sanLuongSauLangDaNhan;
                    //var sanLuongTong = PhieuCanVm.GetSanLuongSoChe(GioLDLs2[i],
                    //                                               GioLDLs2[i + 1],
                    //                                               MainViewModel.DateTimeNowShared,
                    //                                               XuongIdShared,
                    //                                               false,
                    //                                               false,
                    //                                               true);

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioLDLs2[i],
                        GioLDLs2[i + 1],
                        idsLDL2,
                        lDId,
                        xuongId);
                    var soGio = (GioLDLs2[i + 1] - GioLDLs2[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoLDs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoLDs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = lDId,
                                    SanLuongHuong =
                                        Math.Round(
                                            phanBo.TyLeHuong *
                                            sanLuongDonVi -
                                            (phanBo.TyLeHuong * sanLuongDonVi) *
                                            phanBo.TyLeTru,
                                            2),
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Where(
                                        x => x.MaThanhPham == lDId && x.MaNhanVien == itemSanluong.MaNhanVien)
                                    .All(
                                        x =>
                                        {
                                            x.SanLuongHuong += Math.Round(itemSanluong.SanLuongHuong, 2);
                                            return true;
                                        });
                            }
                        }
                    }
                }
                // Lang Da Nho

                var idsLDN2 = phanBoLDNs.Select(x => x.MaNhanVien).Distinct();
                var GioLDNs2 = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsLDN2, lDNId, xuongId);

                for (int i = 0; i < GioLDNs2.Count(); i++)
                {
                    if (i == (GioLDNs2.Count() - 1))
                    {
                        break;
                    }
                    //var sanLuongTong = PhieuCanVm.GetSanLuongSoChe(GioLDNs2[i],
                    //                                               GioLDNs2[i + 1],
                    //                                               MainViewModel.DateTimeNowShared,
                    //                                               XuongIdShared,
                    //                                               false,
                    //                                               false,
                    //                                               true);

                    var msls = PhieuCanNguyenLieuViewModel.Instance.GetMSLs(
                        GioLDNs2[i],
                        GioLDNs2[i + 1],
                        dateTime,
                        xuongId,
                        "2");
                    var sanLuongTong = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDNs2[i],
                        GioLDNs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        false,
                        false,
                        true);
                    var sanLuongTruocLangDa = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDNs2[i],
                        GioLDNs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        true,
                        false,
                        true);
                    var sanLuongSauLangDa = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDNs2[i],
                        GioLDNs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        false,
                        true,
                        false);
                    var sanLuongSauLangDaNhan = PhieuCanSoCheDinhHinhViewModel.Instance.GetSanLuongSoChe(
                        GioLDNs2[i],
                        GioLDNs2[i + 1],
                        dateTime,
                        xuongId,
                        msls,
                        true,
                        false,
                        true,
                        true);
                    sanLuongTong += sanLuongTruocLangDa + sanLuongSauLangDa - sanLuongSauLangDaNhan;

                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioLDNs2[i],
                        GioLDNs2[i + 1],
                        idsLDN2,
                        lDNId,
                        xuongId);
                    var soGio = (GioLDNs2[i + 1] - GioLDNs2[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoLDNs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoLDNs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = lDNId,
                                    SanLuongHuong =
                                        phanBo.TyLeHuong *
                                        sanLuongDonVi -
                                        (phanBo.TyLeHuong * sanLuongDonVi) *
                                        phanBo.TyLeTru,
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Where(
                                        x => x.MaThanhPham == lDNId && x.MaNhanVien == itemSanluong.MaNhanVien)
                                    .All(
                                        x =>
                                        {
                                            x.SanLuongHuong += Math.Round(itemSanluong.SanLuongHuong, 2);
                                            return true;
                                        });
                            }
                        }
                    }
                }
                //6 Rai Ca
                //var rCId = "RC";
                //var nhanVienPhuRC = NhanVienVm.GetNhanVienByLoaiThanhPhamPhu(rCId, DateTimeNowShared, XuongIdShared);
                //var phanBoRCs = NhanVienVm.GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(rCId,
                //                                                                        MainViewModel.DateTimeNowShared,
                //                                                                        MainViewModel.XuongIdShared);

                //double count = 0;
                //foreach (var banCatTiet in BanCatTietViewModel.Ins.BanCatTiets)
                //{
                //    var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                //    if (numBan >= 9)
                //    {
                //        continue;
                //    }
                //    var nhanVienBanCT = NhanVienVm.GetNhanViensPhuByBanCatTiet(banCatTiet.Ma,
                //                                                               DateTimeNowShared,
                //                                                               XuongIdShared);
                //    var nhanViens = new List<Modelv1.EF.NhanVienPhuTheoBanFillet>();
                //    foreach (var item in nhanVienPhuRC)
                //    {
                //        var nv = nhanVienBanCT.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                //        if (nv != null)
                //        {
                //            nhanViens.Add(nv);
                //        }
                //    }
                var idsRC2 = phanBoRCs.Select(x => x.MaNhanVien).Distinct();
                var GioRCs2 = GioVaoRaViewModel.Instance.GetListTime(dateTime, idsRC2, rCId, xuongId);

                for (int i = 0; i < GioRCs2.Count(); i++)
                {
                    if (i == (GioRCs2.Count() - 1))
                    {
                        break;
                    }

                    var sanLuongTong = PhieuCanNguyenLieuViewModel.Instance.GetSanLuongCaNgop(
                        GioRCs2[i],
                        GioRCs2[i + 1],
                        dateTime,
                        xuongId,
                        "CT09");
                    //var numBan = int.Parse(banCatTiet.Ma.Substring(2, 2));
                    //if(numBan == 9)
                    //{
                    //    sanLuongTong = PhieuCanVm.GetSanLuongCaNgop(GioRCs[i],
                    //                                                GioRCs[i + 1],
                    //                                                MainViewModel.DateTimeNowShared,
                    //                                                XuongIdShared,
                    //                                                banCatTiet.Ma);
                    //}
                    var gioVaoRaFillets = GioVaoRaViewModel.Instance.Get(
                        dateTime,
                        GioRCs2[i],
                        GioRCs2[i + 1],
                        idsRC2,
                        rCId,
                        xuongId);
                    var soGio = (GioRCs2[i + 1] - GioRCs2[i]).TotalHours;
                    var ids = gioVaoRaFillets.Select(x => x.MaNhanVien).Distinct();
                    if (gioVaoRaFillets.Any())
                    {
                        var tongTyLe = phanBoRCs.Where(x => ids.Contains(x.MaNhanVien))
                            .Select(x => x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                        //var sanLuongChia = sanLuongTong / gioVaoRaFillets.Count();
                        foreach (var nhanVien in gioVaoRaFillets)
                        {
                            var phanBo = phanBoRCs.SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                            if (phanBo != null)
                            {
                                var itemSanluong = new SanLuongPhuFillet()
                                {
                                    STT = nhanVien.STT,
                                    MaNhanVien = nhanVien.MaNhanVien,
                                    MaThanhPham = rCId,
                                    SanLuongHuong =
                                        phanBo.TyLeHuong *
                                        sanLuongDonVi -
                                        (phanBo.TyLeHuong * sanLuongDonVi) *
                                        phanBo.TyLeTru,
                                    SoGio = soGio,
                                    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru,
                                    TyLeTru = phanBo.TyLeTru,
                                    TyLeHuong = phanBo.TyLeHuong
                                };
                                sanLuongPhuFillets.Where(
                                        x => x.MaThanhPham == rCId && x.MaNhanVien == itemSanluong.MaNhanVien)
                                    .All(
                                        x =>
                                        {
                                            x.SanLuongHuong += Math.Round(itemSanluong.SanLuongHuong, 2);
                                            return true;
                                        });
                            }
                        }
                    }
                }

                //Tru Ca CO
                if (sanLuongCo > 0)
                {
                    var nhanVienCTIds = sanLuongPhuFillets.Where(x => x.MaThanhPham == cTId)
                        .Select(x => x.MaNhanVien)
                        .Distinct();
                    var tongGioTyLeCT = sanLuongPhuFillets.Where(x => x.MaThanhPham == cTId)
                        .Where(x => nhanVienCTIds.Contains(x.MaNhanVien))
                        .Select(x => x.TyLeHuong * x.SoGio)
                        .DefaultIfEmpty(0)
                        .Sum();
                    var sanLuongDonViCT = tongGioTyLeCT == 0 ? 0 : sanLuongCo / tongGioTyLeCT;
                    sanLuongPhuFillets.Where(x => x.MaThanhPham == cTId)
                        .All(
                            x =>
                            {
                                x.SanLuongHuong = x.SanLuongHuong - (sanLuongDonViCT * x.SoGio * x.TyLeHuong);
                                return true;
                            });
                    var nhanVienRCIds = sanLuongPhuFillets.Where(x => x.MaThanhPham == rCId)
                        .Select(x => x.MaNhanVien)
                        .Distinct();
                    var tongGioTyLeRC = sanLuongPhuFillets.Where(x => x.MaThanhPham == rCId)
                        .Where(x => nhanVienRCIds.Contains(x.MaNhanVien))
                        .Select(x => x.TyLeHuong * x.SoGio)
                        .DefaultIfEmpty(0)
                        .Sum();
                    var sanLuongDonViRC = tongGioTyLeRC == 0 ? 0 : sanLuongCo / tongGioTyLeRC;
                    sanLuongPhuFillets.Where(x => x.MaThanhPham == rCId)
                        .All(
                            x =>
                            {
                                x.SanLuongHuong = x.SanLuongHuong - (sanLuongDonViRC * x.SoGio * x.TyLeHuong);
                                return true;
                            });
                }


                //Nhóm sơ chế
                var danhSachNhomsSoChes = BoTriNhomSoCheViewModel.Instance.Gets(dateTime, xuongId);
                if (danhSachNhomsSoChes == null || !danhSachNhomsSoChes.Any())
                {
                    danhSachNhomsSoChes = BoTriNhomSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId);
                }

                var nhomSoChes = NhomSoCheDinhHinhViewModel.Instance.Gets(xuongId, true);
                var nhanVienNhomSoChes = (from d in danhSachNhomsSoChes
                                          from t in nhomSoChes
                                          where d.MaNhomSoChe == t.Ma
                                          select new { d, MaHoSo = t.MaHoSo }).ToList();
                var thongTinNhanVienCuaNhomsSoChes = (from n in NhanViens
                                                      from nh in nhomSoChes
                                                      where n.MaHoSo == nh.MaHoSo
                                                      select n).ToList();
                var maNhaVienNhomSoCheIds = nhanVienNhomSoChes.Select(x => x.d.MaNhanVien).Distinct().ToList();
                var sanLuongNhomSoChes = sanLuongPhuFillets.Where(x => maNhaVienNhomSoCheIds.Contains(x.MaNhanVien))
                    .ToList();
                if (sanLuongNhomSoChes.Any())
                {
                    var sanLuongNhanViens = sanLuongPhuFillets.ToList();
                    foreach (var item in maNhaVienNhomSoCheIds)
                    {
                        sanLuongNhanViens.RemoveAll(x => x.MaNhanVien == item);
                    }

                    sanLuongPhuFillets = new List<SanLuongPhuFillet>(sanLuongNhanViens);
                    foreach (var thongTin in thongTinNhanVienCuaNhomsSoChes)
                    {
                        var nhanVienInNhomSoChes = (from n in NhanViens
                                                    from nk in nhanVienNhomSoChes
                                                    where n.MaNhanVien == nk.d.MaNhanVien && nk.MaHoSo == thongTin.MaHoSo
                                                    select new { d = nk.d, MaHoSo = nk.MaHoSo, TenNhanVien = n.Name, NhanVien = n }).ToList(
                        );
                        var nhanVienInNhomSoCheIds = nhanVienInNhomSoChes.Select(x => x.d.MaNhanVien).ToList();
                        var congViecs = sanLuongNhomSoChes.Select(x => new { x.MaThanhPham, x.BravoId })
                            .Distinct()
                            .ToList();
                        var tongTyLe = nhanVienInNhomSoChes.Select(x => x.d.TyLeHuong * x.d.SoGio)
                            .DefaultIfEmpty(0)
                            .Sum();
                        foreach (var congviec in congViecs)
                        {
                            var sanLuongInNhomSoChes = sanLuongNhomSoChes.Where(
                                    x => nhanVienInNhomSoCheIds.Contains(x.MaNhanVien) &&
                                         x.MaThanhPham == congviec.MaThanhPham)
                                .Select(x => x.SanLuongHuong)
                                .DefaultIfEmpty(0)
                                .Sum();
                            if (sanLuongInNhomSoChes > 0)
                            {
                                var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongInNhomSoChes / tongTyLe;
                                foreach (var nhanVien in nhanVienInNhomSoChes)
                                {
                                    var sanLuongPhuFillet = new SanLuongPhuFillet()
                                    {
                                        BravoId = congviec.BravoId,
                                        MaNhanVien = nhanVien.d.MaNhanVien,
                                        MaThanhPham = congviec.MaThanhPham,
                                        TyLeHuong = nhanVien.d.TyLeHuong,
                                        TyLeTru = nhanVien.d.TyLeTru,
                                        SanLuongTru =
                                            Math.Round(
                                                nhanVien.d.TyLeHuong *
                                                nhanVien.d.SoGio *
                                                nhanVien.d.TyLeTru *
                                                sanLuongDonVi,
                                                2),
                                        SanLuongHuong =
                                            Math.Round(
                                                nhanVien.d.TyLeHuong *
                                                nhanVien.d.SoGio *
                                                sanLuongDonVi -
                                                nhanVien.d.TyLeHuong *
                                                nhanVien.d.SoGio *
                                                nhanVien.d.TyLeTru *
                                                sanLuongDonVi,
                                                2)
                                    };
                                    sanLuongPhuFillets.Add(sanLuongPhuFillet);
                                }
                            }
                        }
                    }
                }

                var donGias = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>(dateTime, @"SP", true);
                var thanhPhamPhus = ThanhPhamPhuFilletViewModel.Instance.Get();
                var itemtonghoptinhluongs = (from item in sanLuongPhuFillets
                                             from tp in thanhPhamPhus
                                             where item.MaThanhPham == tp.Ma
                                             join dg in donGias on new { BravoId = tp.SanPhamId } equals new
                                             {
                                                 BravoId = dg.MaSanPham
                                             } into gj
                                             from jItem in gj.DefaultIfEmpty()
                                             select new SanLuongPhuFillet
                                             {
                                                 BravoId = jItem?.MaSanPham,
                                                 GioTyLe = item.GioTyLe,
                                                 MaNhanVien = item.MaNhanVien,
                                                 MaThanhPham = item.MaThanhPham,
                                                 SanLuongHuong = item.SanLuongHuong,
                                                 SanLuongTrenGio = item.SanLuongTrenGio,
                                                 SanLuongTru = item.SanLuongTru,
                                                 SoGio = item.SoGio,
                                                 STT = item.STT,
                                                 TyLeHuong = item.TyLeHuong,
                                                 TyLeTru = item.TyLeTru,
                                                 DonGia = jItem?.DonGia ?? 0,
                                                 ThanhTien = (decimal)item.SanLuongHuong * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1)
                                             }).ToList();
                sanLuongPhuFillets = new List<SanLuongPhuFillet>(itemtonghoptinhluongs);
                return sanLuongPhuFillets.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                return new List<SanLuongPhuFillet>();
            }
        }

        #region Giờ vào ra
        public List<NhanVienDaiThanh> GetByMaHoSo(string maHoSo, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienDaiThanh(connStr);
                return dao.GetByMaHoSo(maHoSo);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BravoModelV1.Model.PhanBoNhanVienPhu> FindNhanVienByMaHoSo(DateTime dateTime, string xuongId, string thanhPham, string maHoSoInput)
        {
            try
            {
                var nhanViens = GetByMaHoSo(maHoSoInput.Trim());
                if (nhanViens != null)
                {
                    foreach (var nhanVien in nhanViens)
                    {
                        var phanBoNhanVienPhuFillets = new List<PhanBoNhanVienPhu>(ThanhPhamPhuFilletViewModel.Instance.ThanhPhamPhuFilletSelectChanged(dateTime, xuongId, thanhPham));
                        var items = phanBoNhanVienPhuFillets.Where(x => x.MaNhanVien == nhanVien.MaNhanVien).ToList();
                        if (items.Any())
                        {
                            foreach (var item in items)
                            {
                                var index = phanBoNhanVienPhuFillets.IndexOf(item);
                                phanBoNhanVienPhuFillets.RemoveAt(index);
                                phanBoNhanVienPhuFillets.Insert(0, item);
                            }
                        }
                        return phanBoNhanVienPhuFillets.ToList();
                    }
                }
                return new List<PhanBoNhanVienPhu>();
            }
            catch (Exception ex)
            {
                return new List<PhanBoNhanVienPhu>();
                //throw;
            }
        }
        public List<GioVaoRaFillet> NhanViensFindGhiNhanSelectedItemChanged(DateTime dateTime, string xuongId, string thanhPham, List<BravoModelV1.Model.PhanBoNhanVienPhu> items)
        {
            try
            {
                if (items.Count == 1)
                {
                    var phanBo =
                        items.First() as BravoModelV1.Model.PhanBoNhanVienPhu;
                    if (phanBo != null)
                    {
                        var gioVaoRaFillet = new List<GioVaoRaFillet>();
                        gioVaoRaFillet = new List<GioVaoRaFillet>(
                            GioVaoRaViewModel.Instance
                                .GetGioVaoRa(
                                    phanBo.MaNhanVien,
                                    dateTime,
                                    thanhPham,
                                    xuongId));
                        return gioVaoRaFillet.ToList();
                    }
                    else
                    {
                        return new List<GioVaoRaFillet>();
                    }
                }
                else
                {
                    return new List<GioVaoRaFillet>();
                }

                //if (NhanVienFindGhiNhanSelectedItem != null)
                //{
                //    GioVaoRaViewModel.Ins.GioVaoRaFillets = new ObservableCollection<GioVaoRaFillet>(GioVaoRaViewModel.Ins
                //        .GetGioVaoRa(NhanVienFindGhiNhanSelectedItem.MaNhanVien, MainViewModel.DateTimeNowShared));
                //} else
                //{
                //    GioVaoRaViewModel.Ins.GioVaoRaFillets.Clear();
                //}
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                return new List<GioVaoRaFillet>();
            }
        }
        public List<GioVaoRaFillet> NhanViensFindGhiNhanSelectedItemChanged(DateTime dateTime, string xuongId, string thanhPham, string maNhanVien)
        {
            try
            {
                if (maNhanVien != null)
                {

                    var gioVaoRaFillet = new List<GioVaoRaFillet>();
                    gioVaoRaFillet = new List<GioVaoRaFillet>(
                        GioVaoRaViewModel.Instance
                            .GetGioVaoRa(
                                maNhanVien,
                                dateTime,
                                thanhPham,
                                xuongId));
                    return gioVaoRaFillet.ToList();

                }
                else
                {
                    return new List<GioVaoRaFillet>();
                }

                //if (NhanVienFindGhiNhanSelectedItem != null)
                //{
                //    GioVaoRaViewModel.Ins.GioVaoRaFillets = new ObservableCollection<GioVaoRaFillet>(GioVaoRaViewModel.Ins
                //        .GetGioVaoRa(NhanVienFindGhiNhanSelectedItem.MaNhanVien, MainViewModel.DateTimeNowShared));
                //} else
                //{
                //    GioVaoRaViewModel.Ins.GioVaoRaFillets.Clear();
                //}
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                return new List<GioVaoRaFillet>();
            }
        }
        #endregion
        #region nhân viên bàn va line
        public List<PhanBoNhanVienTheoCongViecPhuFillet> GetPhanBo(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhanBoNhanVienTheoCongViecPhuFillet(connStr);
                return dao.Get(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int InsertNhanVienBan(List<NhanVienPhuTheoBanFillet> ds, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienPhuTheoBanFillet(connStr);
                return dao.Insert<NhanVienPhuTheoBanFillet>(ds);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int DeleteNhanVienBan(List<NhanVienPhuTheoBanFillet> ds, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienPhuTheoBanFillet(connStr);
                return dao.Delete<NhanVienPhuTheoBanFillet>(ds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int InsertNhanVienLine(List<NhanVienPhuTheoLine> ds, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienPhuTheoLine(connStr);
                return dao.Insert<NhanVienPhuTheoLine>(ds);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int DeleteNhanVienLine(List<NhanVienPhuTheoLine> ds, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhanVienPhuTheoLine(connStr);
                return dao.Delete<NhanVienPhuTheoLine>(ds);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhanVienDaiThanh> ReloadNhanVienBanLine(DateTime dateTime, string xuongId)
        {
            try
            {
                var nhanVienFindsGhiNhan = new List<NhanVienDaiThanh>();
                var phanBoItems = GetPhanBo(dateTime, xuongId);
                var ids = phanBoItems.Select(x => x.MaNhanVien).Distinct();
                nhanVienFindsGhiNhan.Clear();
                nhanVienFindsGhiNhan = new List<NhanVienDaiThanh>(GetNhanVienInIds(ids));
                return nhanVienFindsGhiNhan.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienDaiThanh>();
                //throw;
            }
        }
        public List<NhanVienDaiThanh> FindNhanVienByMaHoSoNhanVienBanLine(DateTime dateTime, string xuongId, string maHoSo)
        {
            try
            {
                var nhanVienFindsGhiNhan = new List<NhanVienDaiThanh>(ReloadNhanVienBanLine(dateTime, xuongId));
                if (maHoSo.Trim() != string.Empty)
                {

                    var items = nhanVienFindsGhiNhan.Where(x => x.MaHoSo == maHoSo.Trim()).ToList();
                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            var index = nhanVienFindsGhiNhan.IndexOf(item);
                            nhanVienFindsGhiNhan.RemoveAt(index);
                            nhanVienFindsGhiNhan.Insert(0, item);
                        }

                        //NhanVienFindGhiNhanSelectedItem = NhanVienFindsGhiNhan.First();
                    }
                    return nhanVienFindsGhiNhan.ToList();
                }
                return new List<NhanVienDaiThanh>();
            }
            catch (Exception exception)
            {
                return new List<NhanVienDaiThanh>();

                //throw;
            }
        }
        public List<NhanVienPhuTheoBanFillet> AddNhanVienTheoBanPhuFilletTL(DateTime dateTime, string xuongId, string maBanCatTiet, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienFindGhiNhanSelectedItems = dataT.Item1;
                string[] nhanVienFindGhiNhans = listNhanVienFindGhiNhanSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var NhanVienFindGhiNhanSelectedItems = new List<NhanVienDaiThanh>();
                foreach (var nhanVienGhiNhanSelectedItem in nhanVienFindGhiNhans)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienGhiNhanSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienFindGhiNhan = JsonSerializer.Deserialize<NhanVienDaiThanh>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienFindGhiNhan != null)
                    {
                        NhanVienFindGhiNhanSelectedItems.Add(nhanVienFindGhiNhan);
                    }
                }

                var items = new List<NhanVienDaiThanh>();
                foreach (var obj in NhanVienFindGhiNhanSelectedItems)
                {
                    var item = (NhanVienDaiThanh)obj;
                    items.Add(item);
                }
                var nhanVienPhuBanCatTietFillets = new List<NhanVienPhuTheoBanFillet>(BanCatTietViewModel.Instance.BanCatTietSelectChangedCommand(dateTime, xuongId, maBanCatTiet));
                foreach (var item in items)
                {
                    var nhanVien = nhanVienPhuBanCatTietFillets.SingleOrDefault(
                        x => x.MaNhanVien == item.MaNhanVien);

                    if (nhanVien == null)
                    {
                        nhanVienPhuBanCatTietFillets.Add(
                            new NhanVienPhuTheoBanFillet()
                            {
                                MaBanCatTiet = maBanCatTiet,
                                MaNhanVien = item.MaNhanVien,
                                MaXuong = xuongId,
                                Ngay = dateTime.Date,
                            });
                        InsertNhanVienBan(nhanVienPhuBanCatTietFillets);
                    }
                }
                return nhanVienPhuBanCatTietFillets.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                return new List<NhanVienPhuTheoBanFillet>();
            }
        }
        public List<NhanVienPhuTheoLine> AddNhanVienTheoLinePhuFilletTL(DateTime dateTime, string xuongId, string maLine, string maThanhPham, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienFindGhiNhanSelectedItems = dataT.Item1;
                string[] nhanVienFindGhiNhans = listNhanVienFindGhiNhanSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var nhanVienFindGhiNhanSelectedItems = new List<NhanVienDaiThanh>();

                var itemsLineInsert = new List<NhanVienPhuTheoLine>();

                foreach (var nhanVienGhiNhanSelectedItem in nhanVienFindGhiNhans)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienGhiNhanSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienFindGhiNhan = JsonSerializer.Deserialize<NhanVienDaiThanh>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienFindGhiNhan != null)
                    {
                        nhanVienFindGhiNhanSelectedItems.Add(nhanVienFindGhiNhan);
                    }
                }

                var nhanVienPhuTheoLines = new List<NhanVienPhuTheoLine>(LineViewModel.Instance.LineSelectChangedCommand(dateTime, xuongId, maLine, maThanhPham));

                foreach (var item in nhanVienFindGhiNhanSelectedItems)
                {
                    var nhanVien = nhanVienPhuTheoLines.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                    if (nhanVien == null)
                    {
                        nhanVienPhuTheoLines.Add(
                            new NhanVienPhuTheoLine()
                            {
                                Ngay = dateTime.Date,
                                MaXuong = xuongId,
                                MaNhanVien = item.MaNhanVien,
                                MaLine = maLine,
                                MaCongViec = maThanhPham
                            });
                    }
                }

                // Chỉ gọi InsertNhanVienLine một lần sau khi tất cả các đối tượng đã được thêm
                InsertNhanVienLine(nhanVienPhuTheoLines);

                return nhanVienPhuTheoLines.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienPhuTheoLine>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }

        public List<NhanVienPhuTheoBanFillet> RemoveNhanVienTheoBanPhuFilletTL(DateTime dateTime, string xuongId, string maBanCatTiet, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienPhuTheoBanFilletSelectedItems = dataT.Item1;
                string[] nhanVienPhuTheoBanFillets = listNhanVienPhuTheoBanFilletSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var NhanVienPhuTheoBanFilletSelectedItems = new List<NhanVienPhuTheoBanFillet>();
                foreach (var nhanVienPhuTheoBanFilletSelectedItem in nhanVienPhuTheoBanFillets)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienPhuTheoBanFilletSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienPhuTheoBanFillet = JsonSerializer.Deserialize<NhanVienPhuTheoBanFillet>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienPhuTheoBanFillet != null)
                    {
                        NhanVienPhuTheoBanFilletSelectedItems.Add(nhanVienPhuTheoBanFillet);
                    }
                }

                var items = new List<NhanVienPhuTheoBanFillet>();
                foreach (var obj in NhanVienPhuTheoBanFilletSelectedItems)
                {
                    var item = (NhanVienPhuTheoBanFillet)obj;
                    items.Add(item);
                }
                var nhanVienPhuBanCatTietFillets = new List<NhanVienPhuTheoBanFillet>(BanCatTietViewModel.Instance.BanCatTietSelectChangedCommand(dateTime, xuongId, maBanCatTiet));
                var itemsToDelete = new List<NhanVienPhuTheoBanFillet>();
                foreach (var item in items)
                {
                    var nhanVien = nhanVienPhuBanCatTietFillets.SingleOrDefault(
                        x => x.MaNhanVien == item.MaNhanVien);
                    if (nhanVien != null)
                    {
                        itemsToDelete.Add(nhanVien);
                    }
                }

                // Xóa tất cả các nhân viên khỏi danh sách
                if (itemsToDelete.Count > 0)
                {
                    nhanVienPhuBanCatTietFillets.RemoveAll(x => itemsToDelete.Contains(x));
                    DeleteNhanVienBan(itemsToDelete);
                }

                return nhanVienPhuBanCatTietFillets.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienPhuTheoBanFillet>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }

        public List<NhanVienPhuTheoLine> RemoveNhanVienTheoLinePhuFilletTL(DateTime dateTime, string xuongId, string maLine, string maThanhPham, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienPhuTheoLineSelectedItems = dataT.Item1;
                string[] nhanVienPhuTheoLineSelectedItems = listNhanVienPhuTheoLineSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var NhanVienPhuTheoLineSelectedItems = new List<NhanVienPhuTheoLine>();
                foreach (var nhanVienPhuTheoLineSelectedItem in nhanVienPhuTheoLineSelectedItems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienPhuTheoLineSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienPhuTheoLine = JsonSerializer.Deserialize<NhanVienPhuTheoLine>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienPhuTheoLine != null)
                    {
                        NhanVienPhuTheoLineSelectedItems.Add(nhanVienPhuTheoLine);
                    }
                }

                var items = new List<NhanVienPhuTheoLine>();
                foreach (var obj in NhanVienPhuTheoLineSelectedItems)
                {
                    var item = (NhanVienPhuTheoLine)obj;
                    items.Add(item);
                }
                var nhanVienPhuTheoLines = new List<NhanVienPhuTheoLine>(LineViewModel.Instance.LineSelectChangedCommand(dateTime, xuongId, maLine, maThanhPham));
                var itemsToDelete = new List<NhanVienPhuTheoLine>();

                foreach (var item in items)
                {
                    var nhanVien = nhanVienPhuTheoLines.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                    if (nhanVien != null)
                    {
                        itemsToDelete.Add(nhanVien);
                    }
                }
                // Xóa tất cả các nhân viên khỏi danh sách
                if (itemsToDelete.Count > 0)
                {
                    nhanVienPhuTheoLines.RemoveAll(x => itemsToDelete.Contains(x));
                    DeleteNhanVienLine(itemsToDelete);
                }

                return nhanVienPhuTheoLines.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienPhuTheoLine>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
        #endregion
        #endregion
        #region Tính Lương Xếp Khuôn
        public List<BoTriTinhLuonXepKhuon> GetNhanVienCongViecs(DateTime dateTime, string xuongId, string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriTinhLuonXepKhuon(connStr);
                return dao.GetNhanViens(dateTime, xuongId, congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MaNhanVienTheoNhomXepKhuon> GetNhanVienNhomXepKhuons(DateTime dateTime, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.MaNhanVienTheoNhomXepKhuon(connStr);
                return dao.Gets(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public decimal GetSanLuongPhuXepKhuon(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon(connStr);
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamIds);
        }
        public decimal GetSanLuongChinhXepKhuon(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<Tuple<string, string>> thanhPhamCongDoanIds)
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamCongDoanIds);
        }
        public decimal GetSanLuongBlockXepKhuon(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            List<Tuple<string, string>> thanhPhamCongDoanIds)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamCongDoanIds);
        }
        public decimal GetSanLuongKHCXepKhuon(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonKHC();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamIds);
        }
        public decimal GetSanLuongCongViecTaiChe(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> congViecIds)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
            return dao.GetSanLuongCongViec(dateTime, fromTime, toTime, xuongId, congViecIds);
        }
        public decimal GetSanLuongTPDinhHinh(
           DateTime dateTime,
           TimeSpan fromTime,
           TimeSpan toTime,
           string xuongId,
           IEnumerable<string> thanhPhamIds)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamIds);
        }
        public decimal GetSanLuongSoChe(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds)
        {
            var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamIds);
        }
        public decimal GetSanLuongChinhXepKhuon(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<Tuple<string, string>> thanhPhamChieuXaIds,
            string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamChieuXaIds, nhanVienId);
        }
        public decimal GetSanLuongBlockXepKhuon(
           DateTime dateTime,
           TimeSpan fromTime,
           TimeSpan toTime,
           string xuongId,
           IEnumerable<Tuple<string, string>> thanhPhamCongDoanIds,
           string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamCongDoanIds, nhanVienId);
        }
        public decimal GetSanLuongKHCXepKhuon(
           DateTime dateTime,
           TimeSpan fromTime,
           TimeSpan toTime,
           string xuongId,
           IEnumerable<string> thanhPhamIds,
           string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonKHC();
            return dao.GetSanLuong(dateTime, fromTime, toTime, xuongId, thanhPhamIds, nhanVienId);
        }
        public decimal GetSanLuongCongViecTaiChe(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> cognViecIds,
            string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
            return dao.GetSanLuongCongViec(dateTime, fromTime, toTime, xuongId, cognViecIds, nhanVienId);
        }
        public List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon> ReloadSanLuongXepKhuon(DateTime dateTime, string xuongId)
        {
            try
            {
                //await Task.Delay(500).ConfigureAwait(true);
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                var sanLuongTinhLuongXepKhuons = new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>();
                var nhoms = MaNhomXepKhuonViewModel.Instance.Gets(xuongId);
                var nhomNhanVienIds = (from nh in nhoms
                                       from n in NhanViens
                                       where nh.Ma == n.MaHoSo
                                       select n).Distinct()
                    .ToList();
                var congViecs = CongViecTinhLuongXepKhuonViewModel.Instance.Gets();
                var nhanVienNhoms = GetNhanVienNhomXepKhuons(dateTime);
                foreach (var congViec in congViecs)
                {
                    var nhanVienCongViecs = GetNhanVienCongViecs(dateTime, xuongId, congViec.Ma);
                    if (congViec.LoaiDuLieu == 5 || congViec.LoaiDuLieu == 6)
                    {
                        var nhanVienIds = (from n in nhanVienCongViecs
                                           from nh in nhomNhanVienIds
                                           where n.MaNhanVien != nh.MaNhanVien
                                           select n.MaNhanVien).ToList();
                        var gios = GioVaoRaViewModel.Instance
                            .GetListTime(
                                dateTime,
                                xuongId,
                                congViec.Ma,
                                nhanVienIds);
                        for (int i = 0; i < gios.Count; i++)
                        {
                            if (i == (gios.Count() - 1))
                            {
                                break;
                            }

                            //var thanhPhamIdsTPDinhHinh = CongViecViewModel.Ins.GetThanhPhamIdsTPDinhHinh(congViec.Ma);
                            var thanhPhamIdsPhuXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsPhuXepKhuon(congViec.Ma);
                            var thanhPhamChieuXaIdsChinhXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamChieuXaIdChinhXepKhuon(congViec.Ma);
                            var thanhPhamIdsBlockXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsBlockCXepKhuon(congViec.Ma);
                            // var thanhPham
                            var thanhPhamIdsKHCXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsKHCXepKhuon(congViec.Ma);
                            var thanhPhamIdsTaiChe = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsTaiChe(congViec.Ma);
                            var congViecIdsTaiChe = CongViecTinhLuongXepKhuonViewModel.Instance.GetCongViecIdsTaiChe(congViec.Ma);
                            var thanhPhamCongDoanIdsBlockXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamCongDoanIdsBlockXepKhuon(congViec.Ma);
                            //var sanLuongTPDinhHinh = PhieuCanViewModel.Ins
                            //    .GetSanLuongTPDinhHinh(AppViewModel.Ins.DateTimeNow,
                            //                           gios[i],
                            //                           gios[i + 1],
                            //                           MainViewModel.XuongIdShared,
                            //                           thanhPhamIdsTPDinhHinh);
                            var soGio = (gios[i + 1] - gios[i]).TotalHours;
                            var gioVaoRas = GioVaoRaViewModel.Instance
                                .Gets(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    nhanVienIds,
                                    congViec.Ma);
                            if (gioVaoRas.Any())
                            {
                                var nhanVienCongViecNhom = (from nv in nhanVienCongViecs
                                                            from n in NhanViens
                                                            where nv.IsNhom == true && nv.MaNhanVien == n.MaNhanVien
                                                            select n).ToList();
                                if (nhanVienCongViecNhom.Any())
                                {
                                    foreach (var nhom in nhanVienCongViecNhom)
                                    {

                                        var nhanVienInNhomId = nhanVienNhoms
                                            .Where(x => x.MaNhom == nhom.MaHoSo)
                                            .Select(x => x.MaNhanVien)
                                            .Distinct()
                                            .ToList();
                                        var gioVaoRaNhanVienNhoms = gioVaoRas.Where(
                                                x => nhanVienInNhomId.Contains(x.MaNhanVien))
                                            .ToList();

                                        decimal sanLuongTong = 0;
                                        var sanLuongPhuXepKhuon =
                                            GetSanLuongPhuXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamIdsPhuXepKhuon);
                                        var sanLuongChinhXepKhuon =
                                            GetSanLuongChinhXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamChieuXaIdsChinhXepKhuon);
                                        var sanLuongBlockXepKhuon =
                                            GetSanLuongBlockXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamCongDoanIdsBlockXepKhuon);

                                        var sanLuongKHCXepKhuon = GetSanLuongKHCXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamIdsKHCXepKhuon);

                                        var sanLuongTaiChe =
                                            GetSanLuongCongViecTaiChe(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                congViecIdsTaiChe);

                                        sanLuongTong = sanLuongPhuXepKhuon +
                                                       sanLuongChinhXepKhuon +
                                                       sanLuongBlockXepKhuon +
                                                       sanLuongKHCXepKhuon +
                                                       sanLuongTaiChe;
                                        var ids = gioVaoRaNhanVienNhoms.Select(x => x.MaNhanVien)
                                            .Distinct()
                                            .ToList();
                                        var tongTyLe = nhanVienCongViecs.Where(x => ids.Contains(x.MaNhanVien))
                                            .Select(x => x.TyLeHuong)
                                            .DefaultIfEmpty(0)
                                            .Sum();
                                        var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                                        foreach (var nhanVien in gioVaoRaNhanVienNhoms)
                                        {
                                            var phanBo = nhanVienCongViecs.SingleOrDefault(
                                                x => x.MaNhanVien == nhanVien.MaNhanVien);
                                            if (phanBo != null)
                                            {
                                                var maNhom = NhanViens
                                                    .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien)
                                                    ?.MaHoSo;
                                                var nhanvienNhomids = (from nh in nhoms
                                                                       from n in NhanViens
                                                                       where n.MaHoSo == nh.Ma
                                                                       select n.MaNhanVien).ToList();
                                                var nhanViensInNhomGui = nhanVienNhoms
                                                    .Where(
                                                        x => x.MaNhom == maNhom &&
                                                             !nhanvienNhomids.Contains(x.MaNhanVien))
                                                    .ToList();
                                                var sanLuongItem = Math.Round(
                                                    (phanBo.TyLeHuong *
                                                     sanLuongDonVi -
                                                     (phanBo.TyLeHuong * sanLuongDonVi) *
                                                     phanBo.TyLeTru),
                                                    2);
                                                if (nhanViensInNhomGui.Any())
                                                {
                                                    var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(
                                                            x => x.MaNhanVien)
                                                        .ToList();
                                                    var gioVaoRasNhomGui = GioVaoRaViewModel.Instance
                                                        .Gets(
                                                            dateTime,
                                                            gios[i],
                                                            gios[i + 1],
                                                            xuongId,
                                                            nhanViensInNhomGuiIds);
                                                    var nhanVienIdVaoRaNhomGuis = gioVaoRasNhomGui.Select(
                                                            x => x.MaNhanVien)
                                                        .Distinct()
                                                        .ToList();
                                                    var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                                            x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                        .Select(x => x.TyLeHuong)
                                                        .Sum();
                                                    var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                                        ? 0
                                                        : sanLuongItem / tongTyLeNhomGui;
                                                    foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                                    {
                                                        var nhanVienNhomGui = nhanViensInNhomGui.SingleOrDefault(
                                                            x => x.MaNhanVien == gioVaoRaNhanVienId);
                                                        var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                        {
                                                            CaId = nhanVien.MaCa,
                                                            MaCongViec = congViec.Ma,
                                                            BravoId = congViec.BravoId,
                                                            MaNhanVien = gioVaoRaNhanVienId,
                                                            SanLuongHuong =
                                                                nhanVienNhomGui.TyLeHuong *
                                                                sanLuongDonViNhomGui -
                                                                nhanVienNhomGui.TyLeHuong *
                                                                nhanVienNhomGui.TyLeTru *
                                                                sanLuongDonViNhomGui,
                                                            SoGio = nhanVienNhomGui.SoGio,
                                                            TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                            TyLeTru = nhanVienNhomGui.TyLeTru,
                                                            SanLuongTru =
                                                                nhanVienNhomGui.TyLeHuong *
                                                                nhanVienNhomGui.TyLeTru *
                                                                sanLuongDonViNhomGui
                                                        };
                                                        if (itemSanLuong.CaId == "002")
                                                        {
                                                            itemSanLuong.BravoId = congViec.BravoIdDem;
                                                        }

                                                        sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                    }
                                                }
                                                else
                                                {
                                                    var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                    {
                                                        CaId = nhanVien.MaCa,
                                                        MaCongViec = congViec.Ma,
                                                        BravoId = congViec.BravoId,
                                                        MaNhanVien = nhanVien.MaNhanVien,
                                                        SanLuongHuong =
                                                            Math.Round(
                                                                (phanBo.TyLeHuong *
                                                                 sanLuongDonVi -
                                                                 (phanBo.TyLeHuong * sanLuongDonVi) *
                                                                 phanBo.TyLeTru),
                                                                2),
                                                        SoGio = (decimal)soGio,
                                                        TyLeHuong = phanBo.TyLeHuong,
                                                        TyLeTru = phanBo.TyLeTru,
                                                        SanLuongTru =
                                                            (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                                    };
                                                    if (itemSanLuong.CaId == "002")
                                                    {
                                                        itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    }

                                                    sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                }
                                                //var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                //{
                                                //    CaId = nhanVien.MaCa,
                                                //    MaCongViec = congViec.Ma,
                                                //    BravoId = congViec.BravoId,
                                                //    MaNhanVien = nhanVien.MaNhanVien,
                                                //    SanLuongHuong =
                                                //    Math.Round((phanBo.TyLeHuong *
                                                //        sanLuongDonVi -
                                                //        (phanBo.TyLeHuong * sanLuongDonVi) *
                                                //        phanBo.TyLeTru),
                                                //               2),
                                                //    SoGio = (decimal)soGio,
                                                //    TyLeHuong = phanBo.TyLeHuong,
                                                //    TyLeTru = phanBo.TyLeTru,
                                                //    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                                //};
                                                //sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (congViec.LoaiDuLieu == 1 || congViec.LoaiDuLieu == 3)
                    {
                        var nhanVienIds = (from n in nhanVienCongViecs
                                           from nh in nhomNhanVienIds
                                           where n.MaNhanVien != nh.MaNhanVien
                                           select n.MaNhanVien).ToList();
                        var gios = GioVaoRaViewModel.Instance
                            .GetListTime(
                                dateTime,
                                xuongId,
                                congViec.Ma,
                                nhanVienIds);
                        //if(gios.Count >= 2)
                        //{
                        //    var maxGio = gios.Select(x => x).Max();
                        //    if(maxGio > new TimeSpan(19, 0, 0))
                        //    {
                        //        gios.Add(new TimeSpan(19, 0, 0));
                        //        gios = gios.Distinct().OrderBy(x => x).ToList();
                        //    }
                        //}
                        decimal tong = 0;
                        for (int i = 0; i < gios.Count; i++)
                        {
                            if (i == (gios.Count() - 1))
                            {
                                break;
                            }

                            var thanhPhamIdsTPDinhHinh = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsTPDinhHinh(congViec.Ma);
                            var thanhPhamIdsSoChe = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsSoChe(congViec.Ma);

                            var thanhPhamIdsPhuXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamIdsPhuXepKhuon(congViec.Ma);
                            var thanhPhamChieuXaIdsChinhXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamChieuXaIdChinhXepKhuon(congViec.Ma);
                            var thanhPhamIdsKHCXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamIdsKHCXepKhuon(congViec.Ma);
                            var congViecIdsTaiChe = CongViecTinhLuongXepKhuonViewModel.Instance.GetCongViecIdsTaiChe(congViec.Ma);
                            var thanhPhamCongDoanIdsBlockXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamCongDoanIdsBlockXepKhuon(congViec.Ma);

                            var sanLuongTPDinhHinh =
                                GetSanLuongTPDinhHinh(
                                   dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    thanhPhamIdsTPDinhHinh);
                            var sanLuongPhuXepKhuon1 =
                                GetSanLuongPhuXepKhuon(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    thanhPhamIdsPhuXepKhuon);
                            var sanLuongLuongSoChe = GetSanLuongSoChe(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    thanhPhamIdsSoChe);
                            tong += sanLuongLuongSoChe + sanLuongPhuXepKhuon1;
                            var soGio = (gios[i + 1] - gios[i]).TotalHours;
                            var gioVaoRas = GioVaoRaViewModel.Instance
                                .Gets(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    nhanVienIds,
                                    congViec.Ma);

                            if (sanLuongTPDinhHinh > 0 || sanLuongLuongSoChe > 0)
                            {
                                if (gioVaoRas.Any())
                                {
                                    var sanLuongTong = sanLuongTPDinhHinh + sanLuongLuongSoChe;
                                    if (sanLuongTPDinhHinh > 0)
                                    {
                                        sanLuongTong -= sanLuongPhuXepKhuon1;
                                    }

                                    var ids = gioVaoRas.Select(x => x.MaNhanVien).Distinct();
                                    var tongTyLe = Math.Round(
                                        nhanVienCongViecs.Where(x => ids.Contains(x.MaNhanVien))
                                            .Select(x => x.TyLeHuong)
                                            .DefaultIfEmpty(0)
                                            .Sum(),
                                        2);
                                    var sanLuongDonVi = Math.Round(
                                        tongTyLe == 0 ? 0 : Math.Round(sanLuongTong, 2) / tongTyLe,
                                        2);
                                    foreach (var nhanVien in gioVaoRas)
                                    {
                                        var phanBo = nhanVienCongViecs.SingleOrDefault(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien);

                                        if (phanBo != null)
                                        {
                                            var maNhom = NhanViens
                                                .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien)?.MaHoSo;


                                            var nhanvienNhomids = (from nh in nhoms
                                                                   from n in NhanViens
                                                                   where n.MaHoSo == nh.Ma
                                                                   select n.MaNhanVien).ToList();
                                            var nhanViensInNhomGui = nhanVienNhoms
                                                .Where(
                                                    x => x.MaNhom == maNhom &&
                                                         !nhanvienNhomids.Contains(x.MaNhanVien))
                                                .ToList();
                                            var sanLuongItem = Math.Round(
                                                (phanBo.TyLeHuong *
                                                 sanLuongDonVi -
                                                 (phanBo.TyLeHuong * sanLuongDonVi) *
                                                 phanBo.TyLeTru),
                                                2);
                                            if (nhanViensInNhomGui.Any())
                                            {
                                                var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(
                                                        x => x.MaNhanVien)
                                                    .ToList();
                                                var gioVaoRasNhomGui = GioVaoRaViewModel.Instance
                                                    .Gets(
                                                        dateTime,
                                                        gios[i],
                                                        gios[i + 1],
                                                        xuongId,
                                                        nhanViensInNhomGuiIds);
                                                var nhanVienIdVaoRaNhomGuis = gioVaoRasNhomGui.Select(
                                                        x => x.MaNhanVien)
                                                    .Distinct()
                                                    .ToList();
                                                var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                                        x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                    .Select(x => x.TyLeHuong)
                                                    .Sum();
                                                var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                                    ? 0
                                                    : sanLuongItem / tongTyLeNhomGui;
                                                foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                                {
                                                    var nhanVienNhomGui = nhanViensInNhomGui.SingleOrDefault(
                                                        x => x.MaNhanVien == gioVaoRaNhanVienId);
                                                    var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                    {
                                                        CaId = nhanVien.MaCa,
                                                        MaCongViec = congViec.Ma,
                                                        BravoId = congViec.BravoId,
                                                        MaNhanVien = gioVaoRaNhanVienId,
                                                        SanLuongHuong =
                                                            nhanVienNhomGui.TyLeHuong *
                                                            sanLuongDonViNhomGui -
                                                            nhanVienNhomGui.TyLeHuong *
                                                            nhanVienNhomGui.TyLeTru *
                                                            sanLuongDonViNhomGui,
                                                        SoGio = nhanVienNhomGui.SoGio,
                                                        TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                        TyLeTru = nhanVienNhomGui.TyLeTru,
                                                        SanLuongTru =
                                                            nhanVienNhomGui.TyLeHuong *
                                                            nhanVienNhomGui.TyLeTru *
                                                            sanLuongDonViNhomGui
                                                    };
                                                    if (itemSanLuong.CaId == "002")
                                                    {
                                                        itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    }

                                                    //if (gios[i] >= new TimeSpan(19, 0, 0))
                                                    //{
                                                    //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    //}
                                                    sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                }
                                            }
                                            else
                                            {
                                                var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                {
                                                    CaId = nhanVien.MaCa,
                                                    MaCongViec = congViec.Ma,
                                                    BravoId = congViec.BravoId,
                                                    MaNhanVien = nhanVien.MaNhanVien,
                                                    SanLuongHuong =
                                                        Math.Round(
                                                            (phanBo.TyLeHuong *
                                                             sanLuongDonVi -
                                                             (phanBo.TyLeHuong * sanLuongDonVi) *
                                                             phanBo.TyLeTru),
                                                            2),
                                                    SoGio = (decimal)soGio,
                                                    TyLeHuong = phanBo.TyLeHuong,
                                                    TyLeTru = phanBo.TyLeTru,
                                                    SanLuongTru =
                                                        (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                                };
                                                if (itemSanLuong.CaId == "002")
                                                {
                                                    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                }

                                                //if (gios[i] >= new TimeSpan(19, 0, 0))
                                                //{
                                                //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                //}
                                                sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                            }
                                            //var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                            //{
                                            //    CaId = nhanVien.MaCa,
                                            //    MaCongViec = congViec.Ma,
                                            //    BravoId = congViec.BravoId,
                                            //    MaNhanVien = nhanVien.MaNhanVien,
                                            //    SanLuongHuong =
                                            //    Math.Round((Math.Round(phanBo.TyLeHuong, 2) *
                                            //        sanLuongDonVi -
                                            //        (phanBo.TyLeHuong * sanLuongDonVi) *
                                            //        phanBo.TyLeTru),
                                            //               2),
                                            //    SoGio = (decimal)soGio,
                                            //    TyLeHuong = phanBo.TyLeHuong,
                                            //    TyLeTru = phanBo.TyLeTru,
                                            //    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                            //};
                                            //sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                        }
                                    }
                                }
                            }

                            if (gioVaoRas.Any() && sanLuongTPDinhHinh <= 0)
                            {
                                var nhanVienCongViecNhoms = (from nv in nhanVienCongViecs
                                                             from n in NhanViens
                                                             where nv.IsNhom == true && nv.MaNhanVien == n.MaNhanVien
                                                             select n).ToList();
                                if (nhanVienCongViecNhoms.Any())
                                {
                                    decimal sanLuongTong = 0;
                                    foreach (var nhanVienCongViecNhom in nhanVienCongViecNhoms)
                                    {
                                        var sanLuongPhuXepKhuon =
                                            GetSanLuongPhuXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamIdsPhuXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);

                                        var sanLuongChinhXepKhuon =
                                            GetSanLuongChinhXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamChieuXaIdsChinhXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);
                                        var sanLuongBlockXepKhuon = GetSanLuongBlockXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamCongDoanIdsBlockXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);

                                        var sanLuongKHCXepKhuon = GetSanLuongKHCXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamIdsKHCXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);

                                        var sanLuongTaiChe = GetSanLuongCongViecTaiChe(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                congViecIdsTaiChe,
                                                nhanVienCongViecNhom.MaNhanVien);
                                        sanLuongTong += sanLuongPhuXepKhuon +
                                                        sanLuongChinhXepKhuon +
                                                        sanLuongBlockXepKhuon +
                                                        sanLuongKHCXepKhuon +
                                                        sanLuongTaiChe;
                                    }

                                    var nvIds = nhanVienCongViecs.Select(x => x.MaNhanVien).Distinct().ToList();
                                    var gioVaoRaNhanVienNhoms = gioVaoRas.Where(x => nvIds.Contains(x.MaNhanVien))
                                        .ToList();
                                    var nhanvienCongViecIds = gioVaoRaNhanVienNhoms.Select(x => x.MaNhanVien)
                                        .ToList();
                                    var tongTyLe = nhanVienCongViecs.Where(
                                            x => nhanvienCongViecIds.Contains(x.MaNhanVien))
                                        .Select(x => x.TyLeHuong)
                                        .DefaultIfEmpty(0)
                                        .Sum();

                                    var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                                    if (gioVaoRaNhanVienNhoms.Any())
                                    {
                                        foreach (var nhanVien in gioVaoRaNhanVienNhoms)
                                        {
                                            var phanBo = nhanVienCongViecs.SingleOrDefault(
                                                x => x.MaNhanVien == nhanVien.MaNhanVien);
                                            if (phanBo != null)
                                            {
                                                var maNhom = NhanViens
                                                    .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien)
                                                    ?.MaHoSo;

                                                //var nhanViensInNhomGui = NhanVienViewModel.Ins.NhanVienNhoms
                                                //    .Where(x => x.MaNhom == maNhom)
                                                //    .ToList();
                                                var nhanvienNhomids = (from nh in nhoms
                                                                       from n in NhanViens
                                                                       where n.MaHoSo == nh.Ma
                                                                       select n.MaNhanVien).ToList();
                                                var nhanViensInNhomGui = nhanVienNhoms
                                                    .Where(
                                                        x => x.MaNhom == maNhom &&
                                                             !nhanvienNhomids.Contains(x.MaNhanVien))
                                                    .ToList();
                                                var sanLuongItem = Math.Round(
                                                    (phanBo.TyLeHuong *
                                                     sanLuongDonVi -
                                                     (phanBo.TyLeHuong * sanLuongDonVi) *
                                                     phanBo.TyLeTru),
                                                    2);
                                                if (nhanViensInNhomGui.Any())
                                                {
                                                    if (sanLuongItem > 0)
                                                    {
                                                        var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(
                                                                x => x.MaNhanVien)
                                                            .ToList();
                                                        var gioVaoRasNhomGui = GioVaoRaViewModel.Instance
                                                            .Gets(
                                                                dateTime,
                                                                gios[i],
                                                                gios[i + 1],
                                                                xuongId,
                                                                nhanViensInNhomGuiIds);
                                                        var nhanVienIdVaoRaNhomGuis = gioVaoRasNhomGui.Select(
                                                                x => x.MaNhanVien)
                                                            .Distinct()
                                                            .ToList();
                                                        var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                                                x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                            .Select(x => x.TyLeHuong)
                                                            .Sum();
                                                        //var tongTyLeNhomGui = nhanViensInNhomGui.Where(x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                        //   .Select(x => x.SoGio * x.TyLeHuong)
                                                        //   .Sum();
                                                        var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                                            ? 0
                                                            : sanLuongItem / tongTyLeNhomGui;
                                                        foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                                        {
                                                            var nhanVienNhomGui =
                                                                nhanViensInNhomGui.SingleOrDefault(
                                                                    x => x.MaNhanVien == gioVaoRaNhanVienId);
                                                            var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                            {
                                                                CaId = nhanVien.MaCa,
                                                                MaCongViec = congViec.Ma,
                                                                BravoId = congViec.BravoId,
                                                                MaNhanVien = gioVaoRaNhanVienId,
                                                                SanLuongHuong =
                                                                    nhanVienNhomGui.TyLeHuong *
                                                                    sanLuongDonViNhomGui -
                                                                    nhanVienNhomGui.TyLeHuong *
                                                                    nhanVienNhomGui.TyLeTru *
                                                                    sanLuongDonViNhomGui,
                                                                SoGio = nhanVienNhomGui.SoGio,
                                                                TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                                TyLeTru = nhanVienNhomGui.TyLeTru,
                                                                SanLuongTru =
                                                                    nhanVienNhomGui.TyLeHuong *
                                                                    nhanVienNhomGui.TyLeTru *
                                                                    sanLuongDonViNhomGui
                                                            };
                                                            if (itemSanLuong.CaId == "002")
                                                            {
                                                                itemSanLuong.BravoId = congViec.BravoIdDem;
                                                            }

                                                            //if(gios[i] >= new TimeSpan(19, 0, 0))
                                                            //{
                                                            //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                            //}
                                                            //var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                            //{
                                                            //    CaId = nhanVien.MaCa,
                                                            //    MaCongViec = congViec.Ma,
                                                            //    BravoId = congViec.BravoId,
                                                            //    MaNhanVien = gioVaoRaNhanVienId,
                                                            //    SanLuongHuong =
                                                            //    nhanVienNhomGui.SoGio *
                                                            //        nhanVienNhomGui.TyLeHuong *
                                                            //        sanLuongDonViNhomGui -
                                                            //        nhanVienNhomGui.SoGio *
                                                            //        nhanVienNhomGui.TyLeHuong *
                                                            //        nhanVienNhomGui.TyLeTru *
                                                            //        sanLuongDonViNhomGui,
                                                            //    SoGio = (decimal)nhanVienNhomGui.SoGio,
                                                            //    TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                            //    TyLeTru = nhanVienNhomGui.TyLeTru,
                                                            //    SanLuongTru =
                                                            //    nhanVienNhomGui.SoGio *
                                                            //        nhanVienNhomGui.TyLeHuong *
                                                            //        nhanVienNhomGui.TyLeTru *
                                                            //        sanLuongDonViNhomGui
                                                            //};
                                                            sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                    {
                                                        CaId = nhanVien.MaCa,
                                                        MaCongViec = congViec.Ma,
                                                        BravoId = congViec.BravoId,
                                                        MaNhanVien = nhanVien.MaNhanVien,
                                                        SanLuongHuong = sanLuongItem,
                                                        SoGio = (decimal)soGio,
                                                        TyLeHuong = phanBo.TyLeHuong,
                                                        TyLeTru = phanBo.TyLeTru,
                                                        SanLuongTru =
                                                            (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                                    };
                                                    if (itemSanLuong.CaId == "002")
                                                    {
                                                        itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    }

                                                    //if(gios[i] >= new TimeSpan(19, 0, 0))
                                                    //{
                                                    //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    //}
                                                    sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                }
                                            }
                                        }
                                    }
                                }

                                var nhanVienIdInNhoms = nhanVienNhoms
                                    .Select(x => x.MaNhanVien)
                                    .Distinct()
                                    .ToList();
                                var nhanVienIdNotInNhom = nhanVienCongViecs.Where(
                                        x => x.IsNhom == false && !nhanVienIdInNhoms.Contains(x.MaNhanVien))
                                    .ToList();
                            }
                        }
                    }

                    if (congViec.LoaiDuLieu == 7)
                    {
                        var nhanVienIds = (from n in nhanVienCongViecs
                                           from nh in nhomNhanVienIds
                                           where n.MaNhanVien != nh.MaNhanVien
                                           select n.MaNhanVien).ToList();
                        var gios = GioVaoRaViewModel.Instance
                            .GetListTime(
                               dateTime,
                                xuongId,
                                congViec.Ma,
                                nhanVienIds);
                        var mocThoiGianPhanCa = MocThoiGianPhanCaViewModel.Instance.Gets(dateTime, xuongId);
                        var mocGioDem = new TimeSpan(19, 0, 0);
                        if (mocThoiGianPhanCa != null)
                        {
                            mocGioDem = mocThoiGianPhanCa.Gio;
                        }

                        if (gios.Count >= 2)
                        {
                            var maxGio = gios.Select(x => x).Max();
                            if (maxGio > mocGioDem)
                            {
                                gios.Add(mocGioDem);
                                gios = gios.Distinct().OrderBy(x => x).ToList();
                            }
                        }

                        decimal tong = 0;
                        for (int i = 0; i < gios.Count; i++)
                        {
                            if (i == (gios.Count() - 1))
                            {
                                break;
                            }

                            var thanhPhamIdsTPDinhHinh = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamIdsTPDinhHinh(congViec.Ma);
                            var thanhPhamIdsSoChe = CongViecTinhLuongXepKhuonViewModel.Instance.GetThanhPhamIdsSoChe(congViec.Ma);

                            var thanhPhamIdsPhuXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamIdsPhuXepKhuon(congViec.Ma);
                            var thanhPhamChieuXaIdsChinhXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamChieuXaIdChinhXepKhuon(congViec.Ma);
                            var thanhPhamIdsKHCXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamIdsKHCXepKhuon(congViec.Ma);
                            var congViecIdsTaiChe = CongViecTinhLuongXepKhuonViewModel.Instance.GetCongViecIdsTaiChe(congViec.Ma);
                            var thanhPhamCongDoanIdsBlockXepKhuon = CongViecTinhLuongXepKhuonViewModel.Instance
                                .GetThanhPhamCongDoanIdsBlockXepKhuon(congViec.Ma);

                            var sanLuongTPDinhHinh = GetSanLuongTPDinhHinh(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    thanhPhamIdsTPDinhHinh);
                            var sanLuongPhuXepKhuon1 = GetSanLuongPhuXepKhuon(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    thanhPhamIdsPhuXepKhuon);
                            var sanLuongLuongSoChe = GetSanLuongSoChe(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    thanhPhamIdsSoChe);
                            tong += sanLuongLuongSoChe + sanLuongPhuXepKhuon1;
                            var soGio = (gios[i + 1] - gios[i]).TotalHours;
                            var gioVaoRas = GioVaoRaViewModel.Instance
                                .Gets(
                                    dateTime,
                                    gios[i],
                                    gios[i + 1],
                                    xuongId,
                                    nhanVienIds,
                                    congViec.Ma);

                            if (sanLuongTPDinhHinh > 0 || sanLuongLuongSoChe > 0)
                            {
                                if (gioVaoRas.Any())
                                {
                                    var sanLuongTong = sanLuongTPDinhHinh + sanLuongLuongSoChe;
                                    if (sanLuongTPDinhHinh > 0)
                                    {
                                        sanLuongTong -= sanLuongPhuXepKhuon1;
                                    }

                                    var ids = gioVaoRas.Select(x => x.MaNhanVien).Distinct();
                                    var tongTyLe = Math.Round(
                                        nhanVienCongViecs.Where(x => ids.Contains(x.MaNhanVien))
                                            .Select(x => x.TyLeHuong)
                                            .DefaultIfEmpty(0)
                                            .Sum(),
                                        2);
                                    var sanLuongDonVi = Math.Round(
                                        tongTyLe == 0 ? 0 : Math.Round(sanLuongTong, 2) / tongTyLe,
                                        2);
                                    foreach (var nhanVien in gioVaoRas)
                                    {
                                        var phanBo = nhanVienCongViecs.SingleOrDefault(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien);

                                        if (phanBo != null)
                                        {
                                            var maNhom = NhanViens
                                                .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien)?.MaHoSo;


                                            var nhanvienNhomids = (from nh in nhoms
                                                                   from n in NhanViens
                                                                   where n.MaHoSo == nh.Ma
                                                                   select n.MaNhanVien).ToList();
                                            var nhanViensInNhomGui = nhanVienNhoms
                                                .Where(
                                                    x => x.MaNhom == maNhom &&
                                                         !nhanvienNhomids.Contains(x.MaNhanVien))
                                                .ToList();
                                            var sanLuongItem = Math.Round(
                                                (phanBo.TyLeHuong *
                                                 sanLuongDonVi -
                                                 (phanBo.TyLeHuong * sanLuongDonVi) *
                                                 phanBo.TyLeTru),
                                                2);
                                            if (nhanViensInNhomGui.Any())
                                            {
                                                var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(
                                                        x => x.MaNhanVien)
                                                    .ToList();
                                                var gioVaoRasNhomGui = GioVaoRaViewModel.Instance
                                                    .Gets(
                                                        dateTime,
                                                        gios[i],
                                                        gios[i + 1],
                                                        xuongId,
                                                        nhanViensInNhomGuiIds);
                                                var nhanVienIdVaoRaNhomGuis = gioVaoRasNhomGui.Select(
                                                        x => x.MaNhanVien)
                                                    .Distinct()
                                                    .ToList();
                                                var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                                        x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                    .Select(x => x.TyLeHuong)
                                                    .Sum();
                                                var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                                    ? 0
                                                    : sanLuongItem / tongTyLeNhomGui;
                                                foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                                {
                                                    var nhanVienNhomGui = nhanViensInNhomGui.SingleOrDefault(
                                                        x => x.MaNhanVien == gioVaoRaNhanVienId);
                                                    var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                    {
                                                        CaId = nhanVien.MaCa,
                                                        MaCongViec = congViec.Ma,
                                                        BravoId = congViec.BravoId,
                                                        MaNhanVien = gioVaoRaNhanVienId,
                                                        SanLuongHuong =
                                                            nhanVienNhomGui.TyLeHuong *
                                                            sanLuongDonViNhomGui -
                                                            nhanVienNhomGui.TyLeHuong *
                                                            nhanVienNhomGui.TyLeTru *
                                                            sanLuongDonViNhomGui,
                                                        SoGio = nhanVienNhomGui.SoGio,
                                                        TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                        TyLeTru = nhanVienNhomGui.TyLeTru,
                                                        SanLuongTru =
                                                            nhanVienNhomGui.TyLeHuong *
                                                            nhanVienNhomGui.TyLeTru *
                                                            sanLuongDonViNhomGui
                                                    };
                                                    //if (itemSanLuong.CaId == "002")
                                                    //{
                                                    //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    //}
                                                    if (gios[i] >= mocGioDem)
                                                    {
                                                        itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    }

                                                    sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                }
                                            }
                                            else
                                            {
                                                var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                {
                                                    CaId = nhanVien.MaCa,
                                                    MaCongViec = congViec.Ma,
                                                    BravoId = congViec.BravoId,
                                                    MaNhanVien = nhanVien.MaNhanVien,
                                                    SanLuongHuong =
                                                        Math.Round(
                                                            (phanBo.TyLeHuong *
                                                             sanLuongDonVi -
                                                             (phanBo.TyLeHuong * sanLuongDonVi) *
                                                             phanBo.TyLeTru),
                                                            2),
                                                    SoGio = (decimal)soGio,
                                                    TyLeHuong = phanBo.TyLeHuong,
                                                    TyLeTru = phanBo.TyLeTru,
                                                    SanLuongTru =
                                                        (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                                };
                                                //if (itemSanLuong.CaId == "002")
                                                //{
                                                //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                //}
                                                if (gios[i] >= mocGioDem)
                                                {
                                                    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                }
                                                //sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                            }
                                            //var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                            //{
                                            //    CaId = nhanVien.MaCa,
                                            //    MaCongViec = congViec.Ma,
                                            //    BravoId = congViec.BravoId,
                                            //    MaNhanVien = nhanVien.MaNhanVien,
                                            //    SanLuongHuong =
                                            //    Math.Round((Math.Round(phanBo.TyLeHuong, 2) *
                                            //        sanLuongDonVi -
                                            //        (phanBo.TyLeHuong * sanLuongDonVi) *
                                            //        phanBo.TyLeTru),
                                            //               2),
                                            //    SoGio = (decimal)soGio,
                                            //    TyLeHuong = phanBo.TyLeHuong,
                                            //    TyLeTru = phanBo.TyLeTru,
                                            //    SanLuongTru = (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                            //};
                                            //sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                        }
                                    }
                                }
                            }

                            if (gioVaoRas.Any() && sanLuongTPDinhHinh <= 0)
                            {
                                var nhanVienCongViecNhoms = (from nv in nhanVienCongViecs
                                                             from n in NhanViens
                                                             where nv.IsNhom == true && nv.MaNhanVien == n.MaNhanVien
                                                             select n).ToList();
                                if (nhanVienCongViecNhoms.Any())
                                {
                                    decimal sanLuongTong = 0;
                                    foreach (var nhanVienCongViecNhom in nhanVienCongViecNhoms)
                                    {
                                        var sanLuongPhuXepKhuon = GetSanLuongPhuXepKhuon(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamIdsPhuXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);

                                        var sanLuongChinhXepKhuon = GetSanLuongChinhXepKhuon(
                                                 dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamChieuXaIdsChinhXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);
                                        var sanLuongBlockXepKhuon = GetSanLuongBlockXepKhuon(
                                                 dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamCongDoanIdsBlockXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);

                                        var sanLuongKHCXepKhuon = GetSanLuongKHCXepKhuon(
                                                 dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                thanhPhamIdsKHCXepKhuon,
                                                nhanVienCongViecNhom.MaNhanVien);

                                        var sanLuongTaiChe = GetSanLuongCongViecTaiChe(
                                                dateTime,
                                                gios[i],
                                                gios[i + 1],
                                                xuongId,
                                                congViecIdsTaiChe,
                                                nhanVienCongViecNhom.MaNhanVien);
                                        sanLuongTong += sanLuongPhuXepKhuon +
                                                        sanLuongChinhXepKhuon +
                                                        sanLuongBlockXepKhuon +
                                                        sanLuongKHCXepKhuon +
                                                        sanLuongTaiChe;
                                    }

                                    var nvIds = nhanVienCongViecs.Select(x => x.MaNhanVien).Distinct().ToList();
                                    var gioVaoRaNhanVienNhoms = gioVaoRas.Where(x => nvIds.Contains(x.MaNhanVien))
                                        .ToList();
                                    var nhanvienCongViecIds = gioVaoRaNhanVienNhoms.Select(x => x.MaNhanVien)
                                        .ToList();
                                    var tongTyLe = nhanVienCongViecs.Where(
                                            x => nhanvienCongViecIds.Contains(x.MaNhanVien))
                                        .Select(x => x.TyLeHuong)
                                        .DefaultIfEmpty(0)
                                        .Sum();

                                    var sanLuongDonVi = tongTyLe == 0 ? 0 : sanLuongTong / tongTyLe;
                                    if (gioVaoRaNhanVienNhoms.Any())
                                    {
                                        foreach (var nhanVien in gioVaoRaNhanVienNhoms)
                                        {
                                            var phanBo = nhanVienCongViecs.SingleOrDefault(
                                                x => x.MaNhanVien == nhanVien.MaNhanVien);
                                            if (phanBo != null)
                                            {
                                                var maNhom = NhanViens
                                                    .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien)
                                                    ?.MaHoSo;

                                                //var nhanViensInNhomGui = NhanVienViewModel.Ins.NhanVienNhoms
                                                //    .Where(x => x.MaNhom == maNhom)
                                                //    .ToList();
                                                var nhanvienNhomids = (from nh in nhoms
                                                                       from n in NhanViens
                                                                       where n.MaHoSo == nh.Ma
                                                                       select n.MaNhanVien).ToList();
                                                var nhanViensInNhomGui = nhanVienNhoms
                                                    .Where(
                                                        x => x.MaNhom == maNhom &&
                                                             !nhanvienNhomids.Contains(x.MaNhanVien))
                                                    .ToList();
                                                var sanLuongItem = Math.Round(
                                                    (phanBo.TyLeHuong *
                                                     sanLuongDonVi -
                                                     (phanBo.TyLeHuong * sanLuongDonVi) *
                                                     phanBo.TyLeTru),
                                                    2);
                                                if (nhanViensInNhomGui.Any())
                                                {
                                                    if (sanLuongItem > 0)
                                                    {
                                                        var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(
                                                                x => x.MaNhanVien)
                                                            .ToList();
                                                        var gioVaoRasNhomGui = GioVaoRaViewModel.Instance
                                                            .Gets(
                                                                dateTime,
                                                                gios[i],
                                                                gios[i + 1],
                                                                xuongId,
                                                                nhanViensInNhomGuiIds);
                                                        var nhanVienIdVaoRaNhomGuis = gioVaoRasNhomGui.Select(
                                                                x => x.MaNhanVien)
                                                            .Distinct()
                                                            .ToList();
                                                        var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                                                x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                            .Select(x => x.TyLeHuong)
                                                            .Sum();
                                                        //var tongTyLeNhomGui = nhanViensInNhomGui.Where(x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                        //   .Select(x => x.SoGio * x.TyLeHuong)
                                                        //   .Sum();
                                                        var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                                            ? 0
                                                            : sanLuongItem / tongTyLeNhomGui;
                                                        foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                                        {
                                                            var nhanVienNhomGui =
                                                                nhanViensInNhomGui.SingleOrDefault(
                                                                    x => x.MaNhanVien == gioVaoRaNhanVienId);
                                                            var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                            {
                                                                CaId = nhanVien.MaCa,
                                                                MaCongViec = congViec.Ma,
                                                                BravoId = congViec.BravoId,
                                                                MaNhanVien = gioVaoRaNhanVienId,
                                                                SanLuongHuong =
                                                                    nhanVienNhomGui.TyLeHuong *
                                                                    sanLuongDonViNhomGui -
                                                                    nhanVienNhomGui.TyLeHuong *
                                                                    nhanVienNhomGui.TyLeTru *
                                                                    sanLuongDonViNhomGui,
                                                                SoGio = nhanVienNhomGui.SoGio,
                                                                TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                                TyLeTru = nhanVienNhomGui.TyLeTru,
                                                                SanLuongTru =
                                                                    nhanVienNhomGui.TyLeHuong *
                                                                    nhanVienNhomGui.TyLeTru *
                                                                    sanLuongDonViNhomGui
                                                            };
                                                            //if (itemSanLuong.CaId == "002")
                                                            //{
                                                            //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                            //}
                                                            if (gios[i] >= mocGioDem)
                                                            {
                                                                itemSanLuong.BravoId = congViec.BravoIdDem;
                                                            }

                                                            //var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                            //{
                                                            //    CaId = nhanVien.MaCa,
                                                            //    MaCongViec = congViec.Ma,
                                                            //    BravoId = congViec.BravoId,
                                                            //    MaNhanVien = gioVaoRaNhanVienId,
                                                            //    SanLuongHuong =
                                                            //    nhanVienNhomGui.SoGio *
                                                            //        nhanVienNhomGui.TyLeHuong *
                                                            //        sanLuongDonViNhomGui -
                                                            //        nhanVienNhomGui.SoGio *
                                                            //        nhanVienNhomGui.TyLeHuong *
                                                            //        nhanVienNhomGui.TyLeTru *
                                                            //        sanLuongDonViNhomGui,
                                                            //    SoGio = (decimal)nhanVienNhomGui.SoGio,
                                                            //    TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                            //    TyLeTru = nhanVienNhomGui.TyLeTru,
                                                            //    SanLuongTru =
                                                            //    nhanVienNhomGui.SoGio *
                                                            //        nhanVienNhomGui.TyLeHuong *
                                                            //        nhanVienNhomGui.TyLeTru *
                                                            //        sanLuongDonViNhomGui
                                                            //};
                                                            sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                    {
                                                        CaId = nhanVien.MaCa,
                                                        MaCongViec = congViec.Ma,
                                                        BravoId = congViec.BravoId,
                                                        MaNhanVien = nhanVien.MaNhanVien,
                                                        SanLuongHuong = sanLuongItem,
                                                        SoGio = (decimal)soGio,
                                                        TyLeHuong = phanBo.TyLeHuong,
                                                        TyLeTru = phanBo.TyLeTru,
                                                        SanLuongTru =
                                                            (phanBo.TyLeHuong * sanLuongDonVi) * phanBo.TyLeTru
                                                    };
                                                    //if (itemSanLuong.CaId == "002")
                                                    //{
                                                    //    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    //}
                                                    if (gios[i] >= mocGioDem)
                                                    {
                                                        itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    }

                                                    sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                }
                                            }
                                        }
                                    }
                                }

                                var nhanVienIdInNhoms = nhanVienNhoms
                                    .Select(x => x.MaNhanVien)
                                    .Distinct()
                                    .ToList();
                                var nhanVienIdNotInNhom = nhanVienCongViecs.Where(
                                        x => x.IsNhom == false && !nhanVienIdInNhoms.Contains(x.MaNhanVien))
                                    .ToList();
                            }
                        }
                    }

                    if (congViec.LoaiDuLieu == 2 || congViec.LoaiDuLieu == 3 || congViec.LoaiDuLieu == 6)
                    {
                        var congViecSanLuongs = CongViecTinhLuongXepKhuonViewModel.Instance
                            .Gets(dateTime, xuongId, congViec.Ma);
                        foreach (var congViecSanLuong in congViecSanLuongs)
                        {
                            var gioVaoRasCaCongViec = GioVaoRaViewModel.Instance
                                .GetsTheoCa(
                                    dateTime,
                                    xuongId,
                                    congViec.Ma,
                                    congViecSanLuong.MaCa);
                            var sanLuonTinhLuongs = new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>();
                            double tongGio = 0;
                            var nhanVienIds = gioVaoRasCaCongViec.Select(x => x.MaNhanVien).Distinct().ToList();
                            foreach (var nhanVienId in nhanVienIds)
                            {
                                var gioVaoRasNhanVien = gioVaoRasCaCongViec.Where(x => x.MaNhanVien == nhanVienId)
                                    .ToList();
                                double soGio = 0;
                                foreach (var gioVaoRaNhanVien in gioVaoRasNhanVien)
                                {
                                    if (gioVaoRaNhanVien.GioRa != null)
                                    {
                                        soGio += (gioVaoRaNhanVien.GioRa.Value - gioVaoRaNhanVien.GioVao)
                                            .TotalHours;
                                    }
                                }

                                var nhanVienCongViec = nhanVienCongViecs.SingleOrDefault(
                                    x => x.MaNhanVien == nhanVienId);
                                if (nhanVienCongViec != null)
                                {
                                    var sanLuongTinhluong = new BravoModelV1.Model.SanLuongTinhLuongXepKhuon()
                                    {
                                        CaId = congViecSanLuong.MaCa,
                                        BravoId = congViec.BravoId,
                                        MaCongViec = congViec.Ma,
                                        MaNhanVien = nhanVienId,
                                        SoGio = (decimal)soGio,
                                        TyLeHuong = nhanVienCongViec.TyLeHuong,
                                        TyLeTru = nhanVienCongViec.TyLeTru,
                                        GioTyLe = (decimal)soGio * nhanVienCongViec.TyLeHuong
                                    };
                                    if (sanLuongTinhluong.CaId == "002")
                                    {
                                        sanLuongTinhluong.BravoId = congViec.BravoIdDem;
                                    }

                                    sanLuonTinhLuongs.Add(sanLuongTinhluong);
                                    tongGio += soGio;
                                }
                            }

                            var tongGioTyLe = sanLuonTinhLuongs.Select(x => x.TyLeHuong * x.SoGio)
                                .DefaultIfEmpty(0)
                                .Sum();

                            var sanLuongDonVi = tongGioTyLe == 0 ? 0 : congViecSanLuong.SanLuong / tongGioTyLe;
                            sanLuonTinhLuongs.All(
                                x =>
                                {
                                    x.SanLuongHuong = x.GioTyLe *
                                                      sanLuongDonVi -
                                                      x.GioTyLe *
                                                      sanLuongDonVi *
                                                      x.TyLeTru;
                                    x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                                    return true;
                                });
                            var sanLuongToRemoves = new List<SanLuongTinhLuongXepKhuon>();
                            foreach (var sanLuonTinhLuong in sanLuonTinhLuongs)
                            {
                                var maNhom = NhanViens
                                    .SingleOrDefault(x => x.MaNhanVien == sanLuonTinhLuong.MaNhanVien)?.MaHoSo;
                                var nhanvienNhomids = (from nh in nhoms
                                                       from n in NhanViens
                                                       where n.MaHoSo == nh.Ma
                                                       select n.MaNhanVien).ToList();
                                var nhanViensInNhomGui = nhanVienNhoms
                                    .Where(x => x.MaNhom == maNhom && !nhanvienNhomids.Contains(x.MaNhanVien))
                                    .ToList();
                                var sanLuongItem = sanLuonTinhLuong.SanLuongHuong;
                                if (nhanViensInNhomGui.Any())
                                {
                                    sanLuongToRemoves.Add(sanLuonTinhLuong);
                                    var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(x => x.MaNhanVien)
                                        .ToList();
                                    //var gioVaoRasNhomGui = GioVaoRaViewModel.Ins
                                    //    .Gets(AppViewModel.Ins.DateTimeNow,
                                    //        gios[i],
                                    //        gios[i + 1],
                                    //        MainViewModel.XuongIdShared,
                                    //        nhanViensInNhomGuiIds);
                                    var nhanVienIdVaoRaNhomGuis = nhanViensInNhomGuiIds;
                                    var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                            x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                        .Select(x => x.TyLeHuong)
                                        .Sum();
                                    var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                        ? 0
                                        : sanLuongItem / tongTyLeNhomGui;
                                    foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                    {
                                        var nhanVienNhomGui = nhanViensInNhomGui.SingleOrDefault(
                                            x => x.MaNhanVien == gioVaoRaNhanVienId);
                                        var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                        {
                                            CaId = sanLuonTinhLuong.CaId,
                                            MaCongViec = sanLuonTinhLuong.MaCongViec,
                                            BravoId = sanLuonTinhLuong.BravoId,
                                            MaNhanVien = gioVaoRaNhanVienId,
                                            SanLuongHuong =
                                                nhanVienNhomGui.TyLeHuong *
                                                sanLuongDonViNhomGui -
                                                nhanVienNhomGui.TyLeHuong *
                                                nhanVienNhomGui.TyLeTru *
                                                sanLuongDonViNhomGui,
                                            SoGio = nhanVienNhomGui.SoGio,
                                            TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                            TyLeTru = nhanVienNhomGui.TyLeTru,
                                            SanLuongTru =
                                                nhanVienNhomGui.TyLeHuong *
                                                nhanVienNhomGui.TyLeTru *
                                                sanLuongDonViNhomGui
                                        };
                                        if (sanLuonTinhLuong.CaId == "002")
                                        {
                                            itemSanLuong.BravoId = congViec.BravoIdDem;
                                        }

                                        sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                    }
                                }
                            }

                            foreach (var sanLuongToRemove in sanLuongToRemoves)
                            {
                                sanLuonTinhLuongs.Remove(sanLuongToRemove);
                            }

                            sanLuongTinhLuongXepKhuons.AddRange(sanLuonTinhLuongs);
                        }
                    }

                    if (congViec.LoaiDuLieu == 4)
                    {
                        var PhieuSanLuongs = PhieuSanLuongViewModel.Instance.CommandReload(dateTime, xuongId);
                        var phieuSanLuongs = PhieuSanLuongs; //PhieuSanLuongViewModel.Ins
                                                             //.GetLuotRaCoi(AppViewModel.Ins.DateTimeNow, MainViewModel.XuongIdShared);
                        var tong = phieuSanLuongs.Select(x => x.TrongLuong).Sum();
                        if (phieuSanLuongs != null && phieuSanLuongs.Any())
                        {
                            foreach (var item in phieuSanLuongs)
                            {
                                var caId = "001";
                                if (item.GioRaCoi < new TimeSpan(19, 0, 0))
                                {
                                    caId = "001";
                                }
                                else
                                {
                                    caId = "002";
                                }

                                var gioVaoRasCaCongViec = GioVaoRaViewModel.Instance
                                    .GetsTheoCa(
                                        dateTime,
                                        xuongId,
                                        congViec.Ma);
                                var sanLuonTinhLuongs = new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>();
                                double tongGio = 0;
                                var nhomTLs = (from nh in nhanVienCongViecs
                                               from n in NhanViens
                                               where nh.MaNhanVien == n.MaNhanVien && nh.IsNhom == true
                                               select n).Distinct()
                                    .ToList();
                                foreach (var nhom in nhomTLs)
                                {
                                    if (nhom.MaHoSo == item.MaNhom)
                                    {
                                        var Cas = CaViewModel.Instance.Gets();
                                        var nh = nhom.MaHoSo;
                                        var nhanVienIds = (from nvh in nhanVienNhoms
                                                           from gvr in gioVaoRasCaCongViec
                                                           from ca in Cas
                                                           where nvh.MaNhanVien == gvr.MaNhanVien &&
                                                                 nvh.MaNhom == nhom.MaHoSo &&
                                                                 gvr.GioVao <= item.GioRaCoi &&
                                                                 gvr.GioRa > item.GioRaCoi &&
                                                                 gvr.MaCa == ca.Ma
                                                           select new { gvr.MaNhanVien, CaId = ca.Ma }).Distinct()
                                            .ToList();
                                        foreach (var nhanVienId in nhanVienIds)
                                        {
                                            var gioVaoRasNhanVien = gioVaoRasCaCongViec.Where(
                                                    x => x.MaNhanVien == nhanVienId.MaNhanVien)
                                                .ToList();
                                            double soGio = 0;
                                            foreach (var gioVaoRaNhanVien in gioVaoRasNhanVien)
                                            {
                                                if (gioVaoRaNhanVien.GioRa != null)
                                                {
                                                    soGio +=
                                                        (gioVaoRaNhanVien.GioRa.Value - gioVaoRaNhanVien.GioVao)
                                                        .TotalHours;
                                                }
                                            }

                                            var nhanVienCongViec = nhanVienCongViecs.SingleOrDefault(
                                                x => x.MaNhanVien == nhanVienId.MaNhanVien);
                                            if (nhanVienCongViec != null)
                                            {
                                                var sanLuongTinhluong = new BravoModelV1.Model.SanLuongTinhLuongXepKhuon()
                                                {
                                                    CaId = nhanVienId.CaId,
                                                    BravoId = congViec.BravoId,
                                                    MaCongViec = congViec.Ma,
                                                    MaNhanVien = nhanVienId.MaNhanVien,
                                                    SoGio = (decimal)soGio,
                                                    TyLeHuong = nhanVienCongViec.TyLeHuong,
                                                    TyLeTru = nhanVienCongViec.TyLeTru,
                                                    GioTyLe = (decimal)soGio * nhanVienCongViec.TyLeHuong
                                                };
                                                if (caId == "002")
                                                {
                                                    sanLuongTinhluong.BravoId = congViec.BravoIdDem;
                                                }

                                                sanLuonTinhLuongs.Add(sanLuongTinhluong);
                                                tongGio += soGio;
                                            }
                                        }

                                        //var tongGioTyLe = sanLuonTinhLuongs.Select(x => x.TyLeHuong * x.SoGio)
                                        //    .DefaultIfEmpty(0)
                                        //    .Sum();
                                        var tongGioTyLe = sanLuonTinhLuongs.Select(x => x.TyLeHuong * 1)
                                            .DefaultIfEmpty(0)
                                            .Sum();

                                        var sanLuongDonVi = tongGioTyLe == 0 ? 0 : item.TrongLuong / tongGioTyLe;
                                        //sanLuonTinhLuongs.All(x =>
                                        //{
                                        //    x.SanLuongHuong = Math.Round((x.GioTyLe *
                                        //        sanLuongDonVi -
                                        //        x.GioTyLe *
                                        //        sanLuongDonVi *
                                        //        x.TyLeTru),
                                        //                                 2);
                                        //    x.SanLuongTru = x.GioTyLe * sanLuongDonVi * x.TyLeTru;
                                        //    return true;
                                        //});

                                        sanLuonTinhLuongs.All(
                                            x =>
                                            {
                                                x.SanLuongHuong = Math.Round(
                                                    (x.TyLeHuong *
                                                     sanLuongDonVi -
                                                     x.TyLeHuong *
                                                     sanLuongDonVi *
                                                     x.TyLeTru),
                                                    2);
                                                x.SanLuongTru = x.TyLeHuong * sanLuongDonVi * x.TyLeTru;
                                                return true;
                                            });

                                        foreach (var sanLuonTinhLuong in sanLuonTinhLuongs)
                                        {
                                            var maNhom = NhanViens
                                                .SingleOrDefault(x => x.MaNhanVien == sanLuonTinhLuong.MaNhanVien)
                                                ?.MaHoSo;
                                            var nhanvienNhomids = (from nhs in nhoms
                                                                   from n in NhanViens
                                                                   where n.MaHoSo == nhs.Ma
                                                                   select n.MaNhanVien).ToList();
                                            var nhanViensInNhomGui = nhanVienNhoms
                                                .Where(
                                                    x => x.MaNhom == maNhom &&
                                                         !nhanvienNhomids.Contains(x.MaNhanVien))
                                                .ToList();
                                            var sanLuongItem = sanLuonTinhLuong.SanLuongHuong;
                                            if (nhanViensInNhomGui.Any())
                                            {
                                                var nhanViensInNhomGuiIds = nhanViensInNhomGui.Select(
                                                        x => x.MaNhanVien)
                                                    .ToList();
                                                //var gioVaoRasNhomGui = GioVaoRaViewModel.Ins
                                                //    .Gets(AppViewModel.Ins.DateTimeNow,
                                                //        gios[i],
                                                //        gios[i + 1],
                                                //        MainViewModel.XuongIdShared,
                                                //        nhanViensInNhomGuiIds);
                                                var nhanVienIdVaoRaNhomGuis = nhanViensInNhomGuiIds;
                                                var tongTyLeNhomGui = nhanViensInNhomGui.Where(
                                                        x => nhanVienIdVaoRaNhomGuis.Contains(x.MaNhanVien))
                                                    .Select(x => x.TyLeHuong)
                                                    .Sum();
                                                var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                                    ? 0
                                                    : sanLuongItem / tongTyLeNhomGui;
                                                foreach (var gioVaoRaNhanVienId in nhanVienIdVaoRaNhomGuis)
                                                {
                                                    var nhanVienNhomGui = nhanViensInNhomGui.SingleOrDefault(
                                                        x => x.MaNhanVien == gioVaoRaNhanVienId);
                                                    var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                    {
                                                        CaId = sanLuonTinhLuong.CaId,
                                                        MaCongViec = congViec.Ma,
                                                        BravoId = congViec.BravoId,
                                                        MaNhanVien = gioVaoRaNhanVienId,
                                                        SanLuongHuong =
                                                            nhanVienNhomGui.TyLeHuong *
                                                            sanLuongDonViNhomGui -
                                                            nhanVienNhomGui.TyLeHuong *
                                                            nhanVienNhomGui.TyLeTru *
                                                            sanLuongDonViNhomGui,
                                                        SoGio = nhanVienNhomGui.SoGio,
                                                        TyLeHuong = nhanVienNhomGui.TyLeHuong,
                                                        TyLeTru = nhanVienNhomGui.TyLeTru,
                                                        SanLuongTru =
                                                            nhanVienNhomGui.TyLeHuong *
                                                            nhanVienNhomGui.TyLeTru *
                                                            sanLuongDonViNhomGui
                                                    };
                                                    if (caId == "002")
                                                    {
                                                        itemSanLuong.BravoId = congViec.BravoIdDem;
                                                    }

                                                    sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                                }
                                            }
                                            else
                                            {
                                                var itemSanLuong = new SanLuongTinhLuongXepKhuon()
                                                {
                                                    CaId = sanLuonTinhLuong.CaId,
                                                    MaCongViec = congViec.Ma,
                                                    BravoId = congViec.BravoId,
                                                    MaNhanVien = sanLuonTinhLuong.MaNhanVien,
                                                    SanLuongHuong = sanLuonTinhLuong.SanLuongHuong,
                                                    SoGio = sanLuonTinhLuong.SoGio,
                                                    TyLeHuong = sanLuonTinhLuong.TyLeHuong,
                                                    TyLeTru = sanLuonTinhLuong.TyLeTru,
                                                    SanLuongTru = sanLuonTinhLuong.TyLeTru
                                                };
                                                if (caId == "002")
                                                {
                                                    itemSanLuong.BravoId = congViec.BravoIdDem;
                                                }

                                                sanLuongTinhLuongXepKhuons.Add(itemSanLuong);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var finaRl = sanLuongTinhLuongXepKhuons.GroupBy(
                        x => new { x.MaNhanVien, x.MaCongViec, x.BravoId, x.CaId })
                    .Select(
                        g => new SanLuongTinhLuongXepKhuon()
                        {
                            MaNhanVien = g.Key.MaNhanVien,
                            CaId = g.Key.CaId,
                            BravoId = g.Key.BravoId,
                            MaCongViec = g.Key.MaCongViec,
                            SanLuongHuong = Math.Round(g.Sum(x => x.SanLuongHuong), 2),
                            SanLuongTru = g.Sum(x => x.SanLuongTru),
                            TyLeHuong = g.Average(x => x.TyLeHuong),
                            TyLeTru = g.Average(x => x.TyLeTru),
                            SoGio = g.Sum(x => x.SoGio)
                        })
                    .OrderByDescending(x => x.MaNhanVien)
                    .ToList();

                var gioVaoRasFn = GioVaoRaViewModel.Instance
                    .Gets(dateTime, xuongId);
                foreach (var item in finaRl)
                {
                    var gioNhanViens = gioVaoRasFn.Where(
                            x => x.MaNhanVien == item.MaNhanVien &&
                                 x.MaCongViec == item.MaCongViec &&
                                 x.MaCa == item.CaId)
                        .ToList();
                    double soGio = 0;
                    foreach (var itemGio in gioNhanViens)
                    {
                        if (itemGio.GioRa != null)
                        {
                            soGio += (itemGio.GioRa.Value - itemGio.GioVao).TotalHours;
                        }
                    }

                    sanLuongTinhLuongXepKhuons.Add(
                        new SanLuongTinhLuongXepKhuon()
                        {
                            BravoId = item.BravoId,
                            CaId = item.CaId,
                            MaCongViec = item.MaCongViec,
                            GioTyLe = item.GioTyLe,
                            MaNhanVien = item.MaNhanVien,
                            SanLuongHuong = item.SanLuongHuong,
                            SanLuongTrenGio = item.SanLuongTrenGio,
                            SanLuongTru = item.SanLuongTru,
                            SoGio = (decimal)soGio,
                            TyLeHuong = item.TyLeHuong,
                            TyLeTru = item.TyLeTru
                        });
                }

                var donGias = DG_DonGiaViewModel.Instance
                    .Gets<DG_DonGia>(dateTime, @"SP", true);
                var itemtonghoptinhluongs = (from item in sanLuongTinhLuongXepKhuons
                                             join dg in donGias on new { item.BravoId } equals new
                                             {
                                                 BravoId = dg.MaSanPham
                                             } into gj
                                             from jItem in gj.DefaultIfEmpty()
                                             select new SanLuongTinhLuongXepKhuon
                                             {
                                                 GioTyLe = item.GioTyLe,
                                                 BravoId = item.BravoId,
                                                 CaId = item.CaId,
                                                 SanLuongHuong = item.SanLuongHuong,
                                                 MaCongViec = item.MaCongViec,
                                                 MaNhanVien = item.MaNhanVien,
                                                 SanLuongTrenGio = item.SanLuongTrenGio,
                                                 SanLuongTru = item.SanLuongTru,
                                                 SoGio = item.SoGio,
                                                 TyLeHuong = item.TyLeHuong,
                                                 TyLeTru = item.TyLeTru,
                                                 DonGia = jItem?.DonGia ?? 0,
                                                 ThanhTien = item.SanLuongHuong * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
                                                 IsChamCong = BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                         .FirstOrDefault(x => x == item.MaNhanVien) ==
                                                     null
                                                         ? false
                                                         : true
                                             }).ToList();
                sanLuongTinhLuongXepKhuons.Clear();
                sanLuongTinhLuongXepKhuons = new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>(itemtonghoptinhluongs);
                return sanLuongTinhLuongXepKhuons.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                return new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>();
            }

        }

        #region trực tiếp
        public List<T> GetSanLuongBlockXepKhuon<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
            return dao.GetSanLuongTinhLuong<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongChinhXepKhuon<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetSanLuongTinhLuong<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongKHCXepKhuon<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonKHC();
            return dao.GetSanLuongTinhLuong<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongPhuXepKhuon<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
            return dao.GetSanLuongTinhLuong<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongTaiChe<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
            return dao.GetSanLuongTinhLuong<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongBlockXepKhuonInIds<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
            return dao.GetSanLuongTinhLuongInIds<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongChinhXepKhuonInIds<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetSanLuongTinhLuongInIds<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongKHCXepKhuonInIds<T>(
           DateTime dateTime,
           string xuongId,
           IEnumerable<string> nhanVienIdsEx,
           TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonKHC();
            return dao.GetSanLuongTinhLuongInIds<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongPhuXepKhuonInIds<T>(
           DateTime dateTime,
           string xuongId,
           IEnumerable<string> nhanVienIdsEx,
           TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
            return dao.GetSanLuongTinhLuongInIds<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<T> GetSanLuongTaiCheInIds<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
            return dao.GetSanLuongTinhLuongInIds<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian);
        }
        public List<BoTriTinhLuonXepKhuon> GetNhanVienCongViecs(
            DateTime dateTime,
            string xuongId,
            string congViecId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriTinhLuonXepKhuon();
                return dao.GetNhanViens(dateTime, xuongId, congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetSanLuongTinhLuongInIds_PhucVuPhanCo<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian, string bravoId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetSanLuongTinhLuongInIds_PhucVuPhanCo<T>(dateTime, xuongId, nhanVienIdsEx, mocThoiGian,
                bravoId);
        }
        public List<BravoModelV1.Model.PhieuCanKiemDinhHinh> CommandReloadTrucTiep(DateTime dateTime, string xuongId)
        {
            try
            {
                //AppViewModel.Ins.IsBusy = true;
                //await Task.Delay(500).ConfigureAwait(true);
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                var mocThoiGian = new TimeSpan(17, 0, 0);
                var SanLuongTrucTieps = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>();
                //SanLuongTrucTieps.Clear();
                var Nhoms = MaNhomXepKhuonViewModel.Instance.Gets(xuongId);
                var nhanVienIdsEx = (from nh in Nhoms
                                     from n in NhanVienViewModel.Instance.NhanViens
                                     where n.MaHoSo == nh.Ma
                                     select n.MaNhanVien).Distinct()
                    .ToList();
                var nhanVienNhoms = GetNhanVienNhomXepKhuons(dateTime);
                var sanLuongTrucTieps = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>();
                sanLuongTrucTieps.AddRange(
                        GetSanLuongBlockXepKhuon<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongTrucTieps.AddRange(
                    GetSanLuongChinhXepKhuon<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongTrucTieps.AddRange(
                   GetSanLuongKHCXepKhuon<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongTrucTieps.AddRange(
                   GetSanLuongPhuXepKhuon<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                           dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongTrucTieps.AddRange(
                  GetSanLuongTaiChe<PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                var sanLuongNhoms = new List<PhieuCanKiemDinhHinh>();
                sanLuongNhoms.AddRange(
                 GetSanLuongBlockXepKhuonInIds<PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongNhoms.AddRange(
                   GetSanLuongChinhXepKhuonInIds<PhieuCanKiemDinhHinh>(
                             dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongNhoms.AddRange(
                 GetSanLuongKHCXepKhuonInIds<PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongNhoms.AddRange(
                        GetSanLuongPhuXepKhuonInIds<PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                sanLuongNhoms.AddRange(
                   GetSanLuongTaiCheInIds<PhieuCanKiemDinhHinh>(
                            dateTime,
                            xuongId,
                            nhanVienIdsEx,
                            mocThoiGian));
                var caId = "CT04";
                foreach (var item in sanLuongNhoms)
                {
                    var nhomItem = NhanVienViewModel.Instance.NhanViens
                        .SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                    if (nhomItem != null)
                    {
                        var nhanViensInNhom = nhanVienNhoms
                            .Where(x => x.MaNhom == nhomItem.MaHoSo)
                            .ToList();
                        if (nhanViensInNhom.Any())
                        {
                            var nhanvienNhomids = (from nh in Nhoms
                                                   from n in NhanVienViewModel.Instance.NhanViens
                                                   where n.MaHoSo == nh.Ma
                                                   select n.MaNhanVien).ToList();
                            var tongTyLe = nhanViensInNhom.Select(x => x.TyLeHuong).DefaultIfEmpty(0).Sum();
                            var sanLuongDonVi = tongTyLe == 0 ? 0 : item.TrongLuong / tongTyLe;

                            foreach (var nhanVien in nhanViensInNhom)
                            {
                                var nhanViensInNhomGui = nhanVienNhoms
                                    .Where(
                                        x => x.MaNhom == nhanVien.MaNhom && !nhanvienNhomids.Contains(x.MaNhanVien))
                                    .ToList();
                                var sanLuongItem = sanLuongDonVi *
                                                   nhanVien.TyLeHuong -
                                                   sanLuongDonVi *
                                                   nhanVien.TyLeHuong *
                                                   nhanVien.TyLeTru;

                                if (nhanViensInNhomGui.Any())
                                {
                                    var tongTyLeNhomGui = nhanViensInNhomGui.Select(x => x.TyLeHuong).Sum();
                                    var sanLuongDonViNhomGui = tongTyLeNhomGui == 0
                                        ? 0
                                        : sanLuongItem / tongTyLeNhomGui;
                                    foreach (var nhanVienInNhomGui in nhanViensInNhomGui)
                                    {
                                        var phieuSanLuong = new PhieuCanKiemDinhHinh()
                                        {
                                            CaLamViec = caId,
                                            MaNhanVien = nhanVienInNhomGui.MaNhanVien,
                                            Ngay = item.Ngay,
                                            MaSanPham = item.MaSanPham,
                                            TenSanPham = item.TenSanPham,
                                            TrongLuong =
                                                (sanLuongDonViNhomGui *
                                                 nhanVienInNhomGui.TyLeHuong -
                                                 sanLuongDonViNhomGui *
                                                 nhanVienInNhomGui.TyLeHuong *
                                                 nhanVienInNhomGui.TyLeTru),
                                            _Status = item._Status,
                                        };

                                        sanLuongTrucTieps.Add(phieuSanLuong);
                                    }
                                }
                                else
                                {
                                    var phieuSanLuong = new PhieuCanKiemDinhHinh()
                                    {
                                        CaLamViec = caId,
                                        MaNhanVien = nhanVien.MaNhanVien,
                                        Ngay = item.Ngay,
                                        MaSanPham = item.MaSanPham,
                                        TenSanPham = item.TenSanPham,
                                        TrongLuong = sanLuongItem,
                                        _Status = item._Status,
                                    };
                                    sanLuongTrucTieps.Add(phieuSanLuong);
                                }
                            }
                        }
                    }
                }
                var CongViecs = CongViecTinhLuongXepKhuonViewModel.Instance.Gets();
                var congViecPhucVus = CongViecs.Where(x => x.LoaiDuLieu == 8).ToList();
                foreach (var congViec in congViecPhucVus)
                {
                    var nhanVienCongViecs = GetNhanVienCongViecs(
                            dateTime,
                            xuongId,
                            congViec.Ma);
                    if (nhanVienCongViecs.Any())
                    {
                        var nhanvienIds = nhanVienCongViecs.Select(x => x.MaNhanVien).ToList();
                        var sanLuongs =
                            GetSanLuongTinhLuongInIds_PhucVuPhanCo<PhieuCanKiemDinhHinh>(
                                 dateTime,
                            xuongId,
                                nhanvienIds,
                                mocThoiGian,
                                congViec.BravoId);
                        sanLuongTrucTieps.AddRange(sanLuongs);
                    }
                }

                //Kiem Dinh Hinh
                var nhanVienKiems = GetNhanViensKiems(xuongId, dateTime, 0);
                //var sanLuongNhanVienKiems =
                //         NhanVienViewModel.Ins.GetSanLuongNhanVienKiems(nhanVienKiems);
                //var nhanVienKiemsSangLuong = NhanVienViewModel.Ins
                //    .GetNhanViensKiems(MainViewModel.XuongIdShared,
                //                       AppViewModel.Ins.DateTimeNow,
                //                       1);

                var nhomKiems = ToKiemViewModel.Instance.Gets(xuongId);
                var nhomKiemWithNhanVienIdMaNhoms = (from nk in nhomKiems
                                                     from n in NhanVienViewModel.Instance.NhanViens
                                                     where n.MaHoSo == nk.MaHoSo
                                                     select new { n.MaNhanVien, n.MaHoSo, ToId = nk.Ma }).ToList();
                foreach (var nhomKiemWithNhanVienIdMaNhom in nhomKiemWithNhanVienIdMaNhoms)
                {
                    var sanLuongNhomKiems = sanLuongTrucTieps.Where(
                            x => x.MaNhanVien == nhomKiemWithNhanVienIdMaNhom.MaNhanVien)
                        .ToList();
                    foreach (var sanLuongNhomKiem in sanLuongNhomKiems)
                    {
                        var nhanVienInNhoms = nhanVienKiems.Where(x => x.ToId == nhomKiemWithNhanVienIdMaNhom.ToId)
                            .ToList();
                        if (nhanVienInNhoms.Any())
                        {
                            sanLuongTrucTieps.Remove(sanLuongNhomKiem);
                            var tongGioTyLe = nhanVienInNhoms.Select(x => x.SoGio * x.TyLe).DefaultIfEmpty(0).Sum();
                            var trongLuongDonVi = tongGioTyLe == 0
                                ? 0
                                : sanLuongNhomKiem.TrongLuong / (decimal)tongGioTyLe;
                            foreach (var nhanVienInNhom in nhanVienInNhoms)
                            {
                                var phieuCan = new PhieuCanKiemDinhHinh()
                                {
                                    CaLamViec = caId,
                                    _Status = sanLuongNhomKiem._Status,
                                    TenSanPham = sanLuongNhomKiem.TenSanPham,
                                    Ngay = sanLuongNhomKiem.Ngay,
                                    MaNhanVien = nhanVienInNhom.NhanVienDaiThanh.MaNhanVien,
                                    MaSanPham = sanLuongNhomKiem.MaSanPham,
                                    TrongLuong =
                                        trongLuongDonVi *
                                        (decimal)(nhanVienInNhom.SoGio * nhanVienInNhom.TyLe) -
                                        trongLuongDonVi *
                                        (decimal)(nhanVienInNhom.SoGio *
                                                  nhanVienInNhom.TyLe *
                                                  nhanVienInNhom.TyLeTru)
                                };
                                sanLuongTrucTieps.Add(phieuCan);
                            }
                        }
                    }
                }

                sanLuongTrucTieps.All(
                    x =>
                    {
                        x.TrongLuong = x.TrongLuong;
                        x.KhuVuc = "08";
                        return true;
                    });
                var finaRl = sanLuongTrucTieps.GroupBy(
                        x => new { x.MaNhanVien, x._Status, x.TenSanPham, x.Ngay, x.MaSanPham, x.KhuVuc })
                    .Select(
                        g => new BravoModelV1.Model.PhieuCanKiemDinhHinh()
                        {
                            CaLamViec = caId,
                            MaSanPham = g.Key.MaSanPham,
                            MaNhanVien = g.Key.MaNhanVien,
                            Ngay = g.Key.Ngay,
                            TenSanPham = g.Key.TenSanPham,
                            _Status = g.Key._Status,
                            KhuVuc = g.Key.KhuVuc,
                            TrongLuong = Math.Round(g.Sum(x => x.TrongLuong), 2)
                        })
                    .OrderByDescending(x => x.MaNhanVien)
                    .ToList();
                var donGias = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>(dateTime, @"SP", true);
                var itemtonghoptinhluongs = (from item in finaRl
                                             join dg in donGias on new { item.MaSanPham } equals new
                                             {
                                                 dg.MaSanPham
                                             } into gj
                                             from jItem in gj.DefaultIfEmpty()
                                             select new BravoModelV1.Model.PhieuCanKiemDinhHinh
                                             {
                                                 CaLamViec = item.CaLamViec,
                                                 KhuVuc = item.KhuVuc,
                                                 MaNhanVien = item.MaNhanVien,
                                                 MaSanPham = item.MaSanPham,
                                                 Ngay = item.Ngay,
                                                 TenSanPham = item.TenSanPham,
                                                 TrongLuong = item.TrongLuong,
                                                 _Status = item._Status,
                                                 DonGia = jItem?.DonGia ?? 0,
                                                 ThanhTien = item.TrongLuong * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
                                                 IsChamCong =
                                                     BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                         .FirstOrDefault(x => x == item.MaNhanVien) ==
                                                     null
                                                         ? false
                                                         : true
                                             }).ToList();
                SanLuongTrucTieps = new List<PhieuCanKiemDinhHinh>(itemtonghoptinhluongs);
                return SanLuongTrucTieps.ToList();
                //var sumSp64 = SanLuongTrucTieps.Where(x => x.MaSanPham == "SP-064").Sum(x => x.TrongLuong);
            }
            catch
                (Exception exception)
            {
                return new List<PhieuCanKiemDinhHinh>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
        #endregion
        #endregion
        #region Tính Lương Bao Tử
        public List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon> CommandReloadTinhLuongBaoTu(DateTime dateTime,string xuongId)
        {
            try
            {
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                //SanLuongTinhLuongs.Clear();
                var SanLuongTinhLuongs = new List<SanLuongTinhLuongXepKhuon>();
                var nhanVienTheoNhoms = new List<BT_NhanVienTheoNhom>();
                nhanVienTheoNhoms = BT_NhanVienTheoNhomViewModel.Instance.Gets(dateTime);
                if (!nhanVienTheoNhoms.Any())
                {
                    nhanVienTheoNhoms = BT_NhanVienTheoNhomViewModel.Instance.Gets(new DateTime(2020, 01, 01));
                }

                var nhoms = BT_NhomTinhLuongViewModel.Instance.Gets();
                var sanLuongs = BT_PhieuCanViewModel.Instance
                    .GetTongHopsTinhLuong<SanLuongTinhLuongXepKhuon>(
                        dateTime,
                        xuongId);
                var sanLuongNhoms = (from s in sanLuongs
                                     from nh in nhoms
                                     where s.MaHoSo == nh.Ma
                                     select s).ToList();
                var sanLuongNhanVienNhoms = new List<SanLuongTinhLuongXepKhuon>();
                foreach (var sanLuongNhom in sanLuongNhoms)
                {
                    sanLuongs.Remove(sanLuongNhom);
                    var nhanVienInNhoms = nhanVienTheoNhoms.Where(x => x.MaNhom == sanLuongNhom.MaHoSo).ToList();
                    if (nhanVienInNhoms.Any())
                    {
                        var gioTyLe = nhanVienInNhoms.Select(x => x.TyLeHuong * x.SoGio).DefaultIfEmpty(0).Sum();
                        var trongLuongDonVi = gioTyLe == 0 ? 0 : sanLuongNhom.SanLuongHuong / gioTyLe;
                        foreach (var nhanVienInNhom in nhanVienInNhoms)
                        {
                            var sanLuongTinhLuong = new SanLuongTinhLuongXepKhuon()
                            {
                                BravoId = sanLuongNhom.BravoId,
                                CaId = sanLuongNhom.CaId,
                                GioTyLe = gioTyLe,
                                MaCongViec = sanLuongNhom.MaCongViec,
                                MaNhanVien = nhanVienInNhom.MaNhanVien,
                                MaNhom = sanLuongNhom.MaHoSo,
                                SoGio = nhanVienInNhom.SoGio,
                                TyLeHuong = nhanVienInNhom.TyLeHuong,
                                TyLeTru = nhanVienInNhom.TyLeTru,
                                SanLuongTru =
                                    nhanVienInNhom.TyLeTru *
                                    trongLuongDonVi *
                                    nhanVienInNhom.SoGio *
                                    nhanVienInNhom.TyLeHuong,
                                SanLuongHuong =
                                    nhanVienInNhom.TyLeHuong *
                                    nhanVienInNhom.SoGio *
                                    trongLuongDonVi -
                                    nhanVienInNhom.TyLeHuong *
                                    nhanVienInNhom.SoGio *
                                    trongLuongDonVi *
                                    nhanVienInNhom.TyLeTru,
                                TenCongViec = sanLuongNhom.TenCongViec,
                                Ngay = sanLuongNhom.Ngay
                            };
                            sanLuongNhanVienNhoms.Add(sanLuongTinhLuong);
                        }
                    }
                }

                sanLuongs.AddRange(sanLuongNhanVienNhoms);
                var donGias = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>(dateTime, @"SP", true);
                var itemtonghoptinhluongs = (from item in sanLuongs
                                             join dg in donGias on new { item.BravoId } equals new
                                             {
                                                 BravoId = dg.MaSanPham
                                             } into gj
                                             from jItem in gj.DefaultIfEmpty()
                                             select new SanLuongTinhLuongXepKhuon
                                             {
                                                 BravoId = item.BravoId,
                                                 CaId = item.CaId,
                                                 GioTyLe = item.GioTyLe,
                                                 MaCongViec = item.MaCongViec,
                                                 MaHoSo = item.MaHoSo,
                                                 MaNhanVien = item.MaNhanVien,
                                                 MaNhom = item.MaNhom,
                                                 Ngay = item.Ngay,
                                                 SanLuongHuong = item.SanLuongHuong,
                                                 SanLuongTrenGio = item.SanLuongTrenGio,
                                                 SanLuongTru = item.SanLuongTru,
                                                 SoGio = item.SoGio,
                                                 TenCongViec = item.TenCongViec,
                                                 TyLeHuong = item.TyLeHuong,
                                                 TyLeTru = item.TyLeTru,
                                                 DonGia = jItem?.DonGia ?? 0,
                                                 ThanhTien = item.SanLuongHuong * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
                                                 IsChamCong =
                                                    BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                         .FirstOrDefault(x => x == item.MaNhanVien) ==
                                                     null
                                                         ? false
                                                         : true
                                             }).ToList();
                SanLuongTinhLuongs.Clear();
                SanLuongTinhLuongs = new List<SanLuongTinhLuongXepKhuon>(itemtonghoptinhluongs);
                return SanLuongTinhLuongs.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                return new List<SanLuongTinhLuongXepKhuon>();
            }
        }
        #endregion


    }
}
