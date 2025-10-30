using System.Globalization;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.A_Model;
using Models.Repos.Models;
using MvvmHelpers;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.OutputCaching;
using Models.Repos;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Models.Repos.SoketModels;

namespace ViewModels.Repos.HQ
{
    public partial class CoiViewModel : ObservableObject
    {
        private static CoiViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaCoiXepKhuon? item;
        [ObservableProperty] private ObservableRangeCollection<MaCoiXepKhuon> items = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaCoiXepKhuon? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        [ObservableProperty] private ObservableRangeCollection<Models.Repos.A_Model.CoiTamChiTiet> coiTams = new();
        private readonly SynchronizationContext synchronizationContext;
        private readonly object _lock = new();

        private CoiViewModel()
        {
            try
            {
                Reload();
            } catch(Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public void AddCoiTam(CoiTamChiTiet item)
        {
            lock(_lock)
            {
                CoiTams.Add(item);
            }
        }

        public bool CheckBeforeAdd(string coiTamId, string xuongId, PhieuCanChinhXepKhuon phieuCan)
        {
            var coi = Find(coiTamId, xuongId);
            if (coi == null) return false;
            if (coi.Count > 0)
            {
                if (coi.MaChieuXa != phieuCan.MaChieuXa || coi.MaLo != phieuCan.MaLo ||
                    coi.MaThanhPham != phieuCan.MaThanhPhamChinh || coi.MaSize != phieuCan.MaSizeChinh ||
                    coi.IsTaiChe != phieuCan.TaiChe)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
                
        }

        public bool SetCoi(string coiTamId, string xuongId, string maCoiChinh)
        {
            var coi = Find(coiTamId, xuongId);
            if (coi == null) return false;
            if (coi.Count > 0)
            {
                coi.MaCoiChinh = maCoiChinh;
                for (int i = 0; i < coi.Items.Count; i++)
                {
                    var item = coi.Items[i];
                    item.MaCoiChinh = maCoiChinh;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CoiTamAddPhieuCan(string coiTamId, string xuongId, PhieuCanChinhXepKhuon phieuCan)
        {
            var coi = Find(coiTamId, xuongId);
            if (coi == null) return;
            if (coi.Count > 0)
            {
               coi.Items.Insert(0,phieuCan);
            }
            else
            {
                coi.MaLo = phieuCan.MaLo;
                coi.MaThanhPham = phieuCan.MaThanhPhamChinh;
                coi.MaSize = phieuCan.MaSizeChinh;
                coi.MaChieuXa = phieuCan.MaChieuXa;
                coi.IsTaiChe = phieuCan.TaiChe;
                coi.MaCoiChinh = phieuCan.MaCoiChinh;
                coi.Items.Add(phieuCan);

            }
                
        }
        public List<MaCoiXepKhuon_U> GetUs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ////return db.AoUs.Where(x=>x.MNgay.Date >= date.Date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
            //return db.AoUs .Where(x => x.MNgay.Date >= date.Date)
            //    .GroupBy(x => x.MaAo)
            //    .Select(g => g.OrderByDescending(x => x.MNgay).FirstOrDefault())
            //    .Where(x => x != null) 
            //    .OrderByDescending(x => x!.MNgay)
            //    .Skip((PageIndex - 1) * PageSize)
            //    .Take(PageSize)
            //    .ToList();
            var latestDates = db.MaCoiXepKhuonUs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.MaCoi)
                .Select(g => new { MaCoi = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.MaCoiXepKhuonUs.Where(x => x.MNgay.Date >= date.Date) 
                join latest in latestDates
                    on new { hq.MaCoi, hq.Id } 
                    equals new { latest.MaCoi, Id = latest.MaxId }
                orderby hq.MNgay descending
                select hq;

            var result = query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return result;
        }
        public List<MaCoiXepKhuon_D> GetDs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return db.MaCoiXepKhuonDs.Where(x=>x.MNgay.Date >= date.Date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }
        public void CoiTamAddItem(CoiTamChiTiet item, PhieuCanChinhXepKhuon phieuCan) { item.AddItem(phieuCan); }

        public void CoiTamAddItems(CoiTamChiTiet item, List<PhieuCanChinhXepKhuon> phieuCans)
        { item.AddItems(phieuCans); }

        public void AddCoiTams(List<CoiTamChiTiet> items)
        {
            lock(_lock)
            {
                CoiTams.AddRange(items);
            }
        }

        public void RemoveCoiTam(CoiTamChiTiet item)
        {
            lock(_lock)
            {
                CoiTams.Remove(item);
            }
        }

        public void RemoveById(string id)
        {
            lock(_lock)
            {
                var item = CoiTams.FirstOrDefault(x => x.MaCoi == id);
                if(item != null)
                {
                    CoiTams.Remove(item);
                }
            }
        }

        public void ClearCoiTams()
        {
            lock(_lock)
            {
                CoiTams.Clear();
            }
        }

        public static CoiViewModel Instance => instance ??= new CoiViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;

        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaCoiXepKhuon CopyItem(MaCoiXepKhuon item)
        {
            return new MaCoiXepKhuon
            {
                Ma = item.Ma,
                Ten = item.Ten,
                Tam = item.Tam,
                TrongLuongMax = item.TrongLuongMax,
            };
        }
        public MaCoiXepKhuon CopySelectedItem() { return CopyItem(SelectedItem); }

        public MaCoiXepKhuon CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0").Max();
            var id = (int.Parse(maxId) + 1).ToString("000");
            return new MaCoiXepKhuon() { Tam = false, Ma = id, TrongLuongMax = 999 };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaCoiXepKhuon item)
        {
            try
            {
                if(Delete(item) > 0)
                    lock(_lock)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
                        if(_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            //Items.Insert(index,item);
                        }
                    }
            } catch(Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.Gets<T>();
        }

        public List<T> GetLatestWeightPerXuongAndCoi<T>()
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.GetLatestWeightPerXuongAndCoi<T>();
        }
        public List<T> GetLatestWeightPerXuongAndCoiRa<T>()
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.GetLatestWeightPerXuongAndCoiRa<T>();
        }
        public List<T> GetCoiTams<T>()
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.GetCoiTams<T>();
        } 
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaCoiXepKhuon item)
        {
            try
            {
                if(Insert(item) > 0)
                {
                    var items = new List<MaCoiXepKhuon>();
                    lock(_lock)
                    {
                        Items.Add(item);
                    }
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                    IsWindowItemShown = false;
                } else
                {
                    VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
                }
            } catch(Exception e)
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
                if(Insert(Item) > 0)
                {
                    Items.Insert(0, Item);
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                }
            } catch(Exception e)
            {
                VmMessage.SetExceptionCommand.Execute(e);
                //throw;
            }
        }

        public bool IsVailSelectedItem => SelectedItem != null;

        private bool IsItemPass(MaCoiXepKhuon item)
        {
            return item != null &&
                item.Ten != null &&
                item.Ten.Trim() != string.Empty &&
                item.Ma != null &&
                item.Ma.Trim() != string.Empty;
        }
        [RelayCommand]
        private void ForceRaseCanExcute()
        {
            try
            {
                Insert_Command.NotifyCanExecuteChanged();
                Delete_Command.NotifyCanExecuteChanged();
            } catch(Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        public void Reload()
        {
            lock(_lock)
            {
                Items.Clear();
            }

            var items = Gets<MaCoiXepKhuon>();
            if(items.Any())
                lock(_lock)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach(var item in items)
                        {
                            Items.Add(item);
                        }
                    } catch(NotSupportedException e)
                    {
                    }
                }
        }

        public void ReloadCoiTam(DateTime dateTime )
        {
            lock(_lock)
            {
                CoiTams.Clear();
            }

            var _coitams = GetCoiTams<CoiTamChiTiet>();
            var _phieuCanOnCoiTams = PhieuCanChinhXepKhuonViewModel.Instance.GetPhieuCanOnCoiTams<PhieuCanChinhXepKhuon>(dateTime);
            if (_coitams.Any())
            {

                for (int i = 0; i < _coitams.Count; i++)
                {
                    var _coiTam = _coitams[i];
                    var phieuCans = _phieuCanOnCoiTams.Where(x => x.MaCoiTam == (string)_coiTam.MaCoi && x.MaXuong == (string)_coiTam.MaXuong).ToList();
                    var malo = phieuCans.Select(x => x.MaLo).FirstOrDefault();
                    var mathanhpham = phieuCans.Select(x => x.MaThanhPhamChinh).FirstOrDefault();
                    var masize = phieuCans.Select(x => x.MaSizeChinh).FirstOrDefault();
                    var machieuxa = phieuCans.Select(x => x.MaChieuXa).FirstOrDefault();
                    var taiche = phieuCans.Select(x => x.TaiChe).FirstOrDefault();
                    var macoichinh = phieuCans.Select(x => x.MaCoiChinh).FirstOrDefault();
                    var item = new CoiTamChiTiet()
                    {
                        DisplayName = "",
                        MaCoi = _coiTam.MaCoi,
                        Ten = _coiTam.Ten,
                        TrongLuongMax = _coiTam.TrongLuongMax,
                        MaXuong = _coiTam.MaXuong,
                        MaLo = malo,
                        MaThanhPham = mathanhpham,
                        MaSize = masize,
                        MaChieuXa = machieuxa,
                        IsTaiChe = taiche,
                        MaCoiChinh = macoichinh
                    };
                    item.Items.AddRange(phieuCans);
                  

                }
                lock (_lock)
                {
                    CoiTams.AddRange(_coitams);
                }
            }
            

        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<MaCoiXepKhuon> obj)
        {
            try
            {
                Reload();
            } catch(Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaCoiXepKhuon();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaCoiXepKhuon item)
        {
            try
            {
                if(Update(item) > 0)
                    lock(_lock)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
                        if(_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, item);
                        }
                    }
            } catch(Exception e)
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
                if(Update(Item) > 0)
                    lock(_lock)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
                        if(_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, Item);
                            SelectedItem = Item;
                        }
                    }
            } catch(Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public MaCoiXepKhuon? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Ma == ma);
            } catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public CoiTamChiTiet? Find(string coiTamId, string xuongId)
        {
            try
            {
                return CoiTams.FirstOrDefault(x => x.MaCoi == coiTamId && x.MaXuong == xuongId);
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
            } catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

