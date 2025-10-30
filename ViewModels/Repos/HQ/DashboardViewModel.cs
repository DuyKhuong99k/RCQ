using System;
using System.Threading;
using System.Timers;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.A_Model;
using MvvmHelpers;
using Vars;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;


namespace ViewModels.Repos.HQ;

public partial class DashboardViewModel : ObservableObject, IDisposable
{
    private static DashboardViewModel instance;
    private readonly Timer _timer;
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private bool _isDisposed;

    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopGheVungNuoiDaiThanhSite = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamBlockXepKhuon = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamBTPDinhHinh = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamBTPFilletv2 = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamChinhXepKhuon = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamKHCXepKhuon = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamNguyenLieu = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamPhuPham = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamPhuXepKhuon = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamSoCheDinhHinh = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamTaiChe = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamTPDinhHinh = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamTPFilletv2 = new();

    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamBTPXeBuom = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamTPXeBuom = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamRaCoi = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopDinhMucSanLuongTheoChuyenDinhHinh = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopDinhMucSanLuongTheoThanhPhamDinhHinh = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopDinhMucSanLuongTheoChuyenFillet = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopTyLeThoiGianVaDinhMucDinhHinh = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhom = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopTyLeThoiGianVaDinhMucFillet = new();
    //HQ
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamHQ = new();
    [ObservableProperty] public ObservableRangeCollection<object> itemTongHopThanhPhamHQGrid = new();

    [ObservableProperty] public ObservableRangeCollection<ListUserWaitView> userList = new();

    private DashboardViewModel()
    {
        try
        {
            ////Reload();
            //var fromDate = AppViewModels.AppViewModel.Instance.DateTimeNow;
            //var toDate = AppViewModels.AppViewModel.Instance.DateTimeNow;
            //var xuongId = AppViewModels.AppViewModel.Instance.XuongId;
            //TongHopThanhPhamBTPFilletv2(fromDate, toDate, xuongId);


            // Khởi tạo và bắt đầu timer
            //_timer = new Timer { Interval = 1000, AutoReset = true }; // 90 giây
            //_timer.Elapsed += _timer_Elapsed;
            //_timer.Start();
            // sửa 21/05/2025 do không hiển thị dữ liệu dashboard mặc dù lúc sáng có
            _timer = new Timer { Interval = 3000, AutoReset = true };
            _timer.Elapsed += async (s, e) => await TimerElapsedAsync();
            _timer.Start();

        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex);
            //LogError("Initialization failed", ex);
            //throw;
            //VmMessage.SetExceptionCommand.Execute(e);
        }
    }
    public void Dispose()
    {
        if (_isDisposed) return;
        _timer?.Stop();
        _timer?.Dispose();
        _semaphore?.Dispose();
        _isDisposed = true;
    }
    public static DashboardViewModel Instance => instance ??= new DashboardViewModel();
    private AppViewModel vmApp => AppViewModel.Instance;

    private async void _timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            _timer.Stop();
            // Kiểm tra xem danh sách chờ có dữ liệu không
            if (UserList == null || !UserList.Any())
            {
                Console.WriteLine("Danh sách hàng đợi không có dữ liệu. Dừng lấy dữ liệu.");
                ItemTongHopThanhPhamBTPFilletv2.Clear();
                ItemTongHopThanhPhamTPFilletv2.Clear();
                ItemTongHopThanhPhamNguyenLieu.Clear();
                ItemTongHopThanhPhamChinhXepKhuon.Clear();
                ItemTongHopGheVungNuoiDaiThanhSite.Clear();
                ItemTongHopThanhPhamSoCheDinhHinh.Clear();
                ItemTongHopThanhPhamPhuXepKhuon.Clear();
                ItemTongHopThanhPhamBlockXepKhuon.Clear();
                ItemTongHopThanhPhamKHCXepKhuon.Clear();
                ItemTongHopThanhPhamTaiChe.Clear();
                ItemTongHopThanhPhamPhuPham.Clear();
                ItemTongHopThanhPhamBTPDinhHinh.Clear();
                ItemTongHopThanhPhamTPDinhHinh.Clear();
                ItemTongHopThanhPhamBTPXeBuom.Clear();
                ItemTongHopThanhPhamTPXeBuom.Clear();
                ItemTongHopThanhPhamRaCoi.Clear();
                //HQ
                ItemTongHopThanhPhamHQ.Clear();
                ItemTongHopThanhPhamHQGrid.Clear();
                _timer.Interval = 3000;
            }
            else
            {
                List<ListUserWaitView> userListAsList;
                lock (UserList)
                {
                    userListAsList = UserList.ToList();
                }

                // Lấy danh sách các phần tử cần giữ lại
                var keepUsers = GetKeepListUserWaitView(userListAsList);

                // Kiểm tra xem danh sách giữ lại có dữ liệu không
                if (!keepUsers.Any())
                {
                    Console.WriteLine("Danh sách phần tử giữ lại không có dữ liệu. Dừng lấy dữ liệu.");
                    _timer.Interval = vmApp.IntervalDashBoard * 2;
                }
                else
                {
                    _timer.Interval = vmApp.IntervalDashBoard;
                }

                try
                {
                    lock (ItemTongHopThanhPhamBTPFilletv2)
                    {
                        ItemTongHopThanhPhamBTPFilletv2.Clear();
                    }
                    lock (ItemTongHopThanhPhamTPFilletv2)
                    {
                        ItemTongHopThanhPhamTPFilletv2.Clear();
                    }
                    lock (ItemTongHopThanhPhamNguyenLieu)
                    {
                        ItemTongHopThanhPhamNguyenLieu.Clear();
                    }
                    lock (ItemTongHopThanhPhamChinhXepKhuon)
                    {
                        ItemTongHopThanhPhamChinhXepKhuon.Clear();
                    }
                    lock (ItemTongHopGheVungNuoiDaiThanhSite)
                    {
                        ItemTongHopGheVungNuoiDaiThanhSite.Clear();
                    }
                    lock (ItemTongHopThanhPhamSoCheDinhHinh)
                    {
                        ItemTongHopThanhPhamSoCheDinhHinh.Clear();
                    }
                    lock (ItemTongHopThanhPhamPhuXepKhuon)
                    {
                        ItemTongHopThanhPhamPhuXepKhuon.Clear();
                    }
                    lock (ItemTongHopThanhPhamBlockXepKhuon)
                    {
                        ItemTongHopThanhPhamBlockXepKhuon.Clear();
                    }
                    lock (ItemTongHopThanhPhamKHCXepKhuon)
                    {
                        ItemTongHopThanhPhamKHCXepKhuon.Clear();
                    }
                    lock (ItemTongHopThanhPhamTaiChe)
                    {
                        ItemTongHopThanhPhamTaiChe.Clear();
                    }
                    lock (ItemTongHopThanhPhamPhuPham)
                    {
                        ItemTongHopThanhPhamPhuPham.Clear();
                    }
                    lock (ItemTongHopThanhPhamBTPDinhHinh)
                    {
                        ItemTongHopThanhPhamBTPDinhHinh.Clear();
                    }
                    lock (ItemTongHopThanhPhamTPDinhHinh)
                    {
                        ItemTongHopThanhPhamTPDinhHinh.Clear();
                    }
                    lock (ItemTongHopThanhPhamBTPXeBuom)
                    {
                        ItemTongHopThanhPhamBTPXeBuom.Clear();
                    }
                    lock (ItemTongHopThanhPhamTPXeBuom)
                    {
                        ItemTongHopThanhPhamTPXeBuom.Clear();
                    }
                    lock (ItemTongHopThanhPhamRaCoi)
                    {
                        ItemTongHopThanhPhamRaCoi.Clear();
                    }
                    //HQ
                    lock (ItemTongHopThanhPhamHQ)
                    {
                        ItemTongHopThanhPhamHQ.Clear();
                    }
                    lock (ItemTongHopThanhPhamHQGrid)
                    {
                        ItemTongHopThanhPhamHQGrid.Clear();
                    }

                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception);
                    //throw;
                }

                var itemFinds = keepUsers;
                if (itemFinds.Any())
                {
                    var xuongIds = XiNghiepViewModel.Instance.Items.Select(x => x.Ma);

                    var fromDate = vmApp.DateTimeNow;
                    var toDate = vmApp.DateTimeNow;
                    //var fromDate = new DateTime(2025, 02, 22, 0, 0, 0);
                    //var toDate = new DateTime(2025, 02, 22, 23, 59, 59);
                    // Thực hiện lấy dữ liệu cho từng xưởng trong danh sách
                    foreach (var xuongId in xuongIds)
                    {
                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamBTPFilletv2(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamTPFilletv2(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamNguyenLieu(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamChinhXepKhuon(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamSoCheDinhHinh(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamPhuXepKhuon(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamBlockXepKhuon(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamKHCXepKhuon(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamTaiChe(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamPhuPham(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamBTPDinhHinh(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamTPDinhHinh(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }
                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamBTPXeBuom(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }
                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamTPXeBuom(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }
                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamRaCois(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }

                        //HQ
                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamHQ(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }
                        await Task.Delay(100);
                        try
                        {
                            TongHopThanhPhamHQGrid(fromDate, toDate, xuongId);
                        }
                        catch (Exception exception)
                        {
                            //Console.WriteLine(exception);
                            //throw;
                        }


                    }
                    await Task.Delay(100);
                    try
                    {
                        TongHopGheVungNuoiDaiThanhSite(fromDate, toDate);
                    }
                    catch (Exception exception)
                    {
                        //Console.WriteLine(exception);
                        //throw;
                    }

                }
                else
                {
                    Console.WriteLine("Đã vượt quá thời gian cho ListUserWaitView. Đang dừng đồng hồ hẹn giờ.");
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
        finally
        {
            _timer.Start();
        }
    }
    private async Task TimerElapsedAsync()
    {
        if (!_semaphore.Wait(0)) return; // Nếu đang chạy thì bỏ qua
        try
        {
            if (UserList == null || !UserList.Any())
            {
                Console.WriteLine("Danh sách hàng đợi không có dữ liệu. Dừng lấy dữ liệu.");
                ClearAllCollections();
                _timer.Interval = 3000;
                return;
            }

            _timer.Interval = vmApp.IntervalDashBoard;
            ClearAllCollections();
            List<ListUserWaitView> userListCopy;
            lock (UserList)
            {
                userListCopy = UserList.ToList();
            }

            var keepUsers = GetKeepListUserWaitView(userListCopy);
            if (!keepUsers.Any())
            {
                Console.WriteLine("No valid users in keep list. Skipping data fetch.");
                return;
            }

            var fromDate = vmApp.DateTimeNow;
            var toDate = vmApp.DateTimeNow;
            var xuongIds = XiNghiepViewModel.Instance.Items.Select(x => x.Ma).ToList();
            var comName = vmApp.ComName;

            foreach (var xuongId in xuongIds)
            {
                
                //if (comName == nameof(ComNames.DAITHANH) || comName == nameof(ComNames.HL))
                //{
                //    await Task.Delay(100); try { TongHopThanhPhamBTPFilletv2(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamTPFilletv2(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamNguyenLieu(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamSoCheDinhHinh(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamBTPDinhHinh(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamTPDinhHinh(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamBTPXeBuom(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamTPXeBuom(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamChinhXepKhuon(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamRaCois(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopDinhMucSanSanLuongTheoChuyenDinhHinh(toDate,toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopDinhMucSanSanLuongTheoThanhPhamDinhHinh(toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopDinhMucSanSanLuongTheoChuyenFillet(toDate,toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopTyLeThoiGianVaDinhMucDinhHinh(toDate,toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhom(toDate,toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopTyLeThoiGianVaDinhMucFillet(toDate,toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //}

                
                //if (comName == nameof(ComNames.DAITHANH))
                //{
                    
                //    await Task.Delay(100); try { TongHopThanhPhamPhuXepKhuon(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamBlockXepKhuon(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamKHCXepKhuon(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamTaiChe(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //    await Task.Delay(100); try { TongHopThanhPhamPhuPham(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //}

                
                //if (comName == nameof(ComNames.RCQTG))
                //{
                    await Task.Delay(100); try { TongHopThanhPhamHQ(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                    await Task.Delay(100); try { TongHopThanhPhamHQGrid(fromDate, toDate, xuongId); } catch (Exception exception) { Console.WriteLine(exception); }
                //}
            }

            
            //if (comName == nameof(ComNames.DAITHANH))
            //{
            //    await Task.Delay(100);
            //    try { TongHopGheVungNuoiDaiThanhSite(fromDate, toDate); } catch { }
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Timer Error: {ex}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void ClearAllCollections()
    {
        var collections = new[]
        {
                //ItemTongHopThanhPhamBTPFilletv2,
                //ItemTongHopThanhPhamTPFilletv2,
                //ItemTongHopThanhPhamNguyenLieu,
                //ItemTongHopThanhPhamChinhXepKhuon,
                //ItemTongHopGheVungNuoiDaiThanhSite,
                //ItemTongHopThanhPhamSoCheDinhHinh,
                //ItemTongHopThanhPhamPhuXepKhuon,
                //ItemTongHopThanhPhamBlockXepKhuon,
                //ItemTongHopThanhPhamKHCXepKhuon,
                //ItemTongHopThanhPhamTaiChe,
                //ItemTongHopThanhPhamPhuPham,
                //ItemTongHopThanhPhamBTPDinhHinh,
                //ItemTongHopThanhPhamTPDinhHinh,
                //ItemTongHopThanhPhamBTPXeBuom,
                //ItemTongHopThanhPhamTPXeBuom,
                //ItemTongHopThanhPhamRaCoi,
                //ItemTongHopDinhMucSanLuongTheoChuyenDinhHinh,
                //ItemTongHopDinhMucSanLuongTheoThanhPhamDinhHinh,
                //ItemTongHopDinhMucSanLuongTheoChuyenFillet,
                //ItemTongHopTyLeThoiGianVaDinhMucDinhHinh,
                //ItemTongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhom,
                //ItemTongHopTyLeThoiGianVaDinhMucFillet,
                //HQ
                ItemTongHopThanhPhamHQ,
                ItemTongHopThanhPhamHQGrid
        };

        foreach (var collection in collections)
        {
            lock (collection)
            {
                collection.Clear();
            }
        }
    }



    //public class ListUserWaitView
    //{
    //    public string Id;
    //    public DateTime ThoiGian;
    //    public string XuongId;
    //}
    public void AddUserWaitView(string id, DateTime thoiGian, string xuongId)
    {
        var newUser = new ListUserWaitView
        {
            Id = id,
            ThoiGian = DateTime.Now,
            XuongId = xuongId
        };

        lock (UserList)
        {
            UserList.Add(newUser);
            //Console.WriteLine($"Added UserWaitView: Id={id}, XuongId={xuongId}, Time={thoiGian}");
        }
    }

    private List<ListUserWaitView> GetKeepListUserWaitView(List<ListUserWaitView> userList)
    {
        var currentTime = DateTime.Now;

        // Lọc các phần tử có thời gian lớn hơn hoặc bằng thời gian hiện tại

        //var list = userList.Where(x => x != null).ToList();
        //var filteredUsers = list.Where(x => (currentTime - x.ThoiGian).Milliseconds < vmApp.IntervalDashBoard).ToList();

        var filteredUsers = userList.Where(x => x != null && (currentTime - x.ThoiGian).Milliseconds < vmApp.IntervalDashBoard)
            .GroupBy(x => x.XuongId)
            .Select(g => new ListUserWaitView { XuongId = g.Key, ThoiGian = g.Max(x => x.ThoiGian), Id = "" })
            .ToList();

        // Lấy danh sách các phần tử cần giữ lại
        //var keepUsers = filteredUsers
        //    .Where(user => user.ThoiGian == filteredUsers
        //        .Where(u => u.XuongId == user.XuongId)
        //        .Max(u => u.ThoiGian))
        //    .Distinct()
        //    .ToList();
        //// Cập nhật lại danh sách UserList
        //userList.Clear(); // Xóa toàn bộ các phần tử hiện tại
        //userList.AddRange(keepUsers); // Thêm các phần tử mới vào danh sách
        lock (UserList)
        {
            UserList.Clear();
            UserList.AddRange(filteredUsers);
        }

        return filteredUsers;
    }
    private void LogError(string message, Exception ex)
    {
        Console.WriteLine($"{message}: {ex.Message}\n{ex.StackTrace}");
        // Optionally, use a proper logging framework like Serilog or NLog
    }
    /// <summary>
    ///     Đang không phân biệt xưởng
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    public void TongHopGheVungNuoiDaiThanhSite(DateTime fromDate, DateTime toDate)
    {
        //lock (ItemTongHopGheVungNuoiDaiThanhSite)
        //{
        //    ItemTongHopGheVungNuoiDaiThanhSite.Clear();
        //}

        var items = PhieuCanVungNuoiDaiThanhSideViewModel.Instance.GetTongHopGhe<object>(fromDate, toDate);
        if (items.Any())
            lock (ItemTongHopGheVungNuoiDaiThanhSite)
            {
                try
                {
                    foreach (var item in items) ItemTongHopGheVungNuoiDaiThanhSite.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamBlockXepKhuon(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamBlockXepKhuon)
        //{
        //    ItemTongHopThanhPhamBlockXepKhuon.Clear();
        //}

        var items = PhieuCanXepKhuonBlockViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamBlockXepKhuon)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamBlockXepKhuon.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamBTPDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamBTPDinhHinh)
        //{
        //    ItemTongHopThanhPhamBTPDinhHinh.Clear();
        //}

        var items = PhieuCanBTPDinhHinhViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamBTPDinhHinh)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamBTPDinhHinh.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamBTPFilletv2(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamBTPFilletv2)
        //{
        //    ItemTongHopThanhPhamBTPFilletv2.Clear();
        //}

        var items = PhieuCanBTPFilletv2ViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamBTPFilletv2)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamBTPFilletv2.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamChinhXepKhuon(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamChinhXepKhuon)
        //{
        //    ItemTongHopThanhPhamChinhXepKhuon.Clear();
        //}

        var items =
            PhieuCanChinhXepKhuonViewModel.Instance.GetPhieuCanTongHopChinhXepKhuons<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamChinhXepKhuon)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamChinhXepKhuon.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamKHCXepKhuon(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamKHCXepKhuon)
        //{
        //    ItemTongHopThanhPhamKHCXepKhuon.Clear();
        //}

        var items = PhieuCanXepKhuonKHCViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamKHCXepKhuon)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamKHCXepKhuon.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamNguyenLieu(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamNguyenLieu)
        //{
        //    ItemTongHopThanhPhamNguyenLieu.Clear();
        //}

        var items = PhieuCanNguyenLieuViewModel.Instance
            .GetTongHopThanhPhamDashBoard<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamNguyenLieu)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamNguyenLieu.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamPhuPham(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamPhuPham)
        //{
        //    ItemTongHopThanhPhamPhuPham.Clear();
        //}

        var items = PhieuCanPhuPhamViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamPhuPham)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamPhuPham.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamPhuXepKhuon(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamPhuXepKhuon)
        //{
        //    ItemTongHopThanhPhamPhuXepKhuon.Clear();
        //}

        var items = PhieuCanPhuXepKhuonViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamPhuXepKhuon)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamPhuXepKhuon.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamSoCheDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamSoCheDinhHinh)
        //{
        //    ItemTongHopThanhPhamSoCheDinhHinh.Clear();
        //}

        var items = PhieuCanSoCheDinhHinhViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamSoCheDinhHinh)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamSoCheDinhHinh.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamTaiChe(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamTaiChe)
        //{
        //    ItemTongHopThanhPhamTaiChe.Clear();
        //}

        var items = PhieuCanTaiCheViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamTaiChe)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamTaiChe.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamTPDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamTPDinhHinh)
        //{
        //    ItemTongHopThanhPhamTPDinhHinh.Clear();
        //}

        var items = PhieuCanTPDinhHinhViewModel.Instance.GetTongHopLoaiThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamTPDinhHinh)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamTPDinhHinh.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamTPFilletv2(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamTPFilletv2)
        //{
        //    ItemTongHopThanhPhamTPFilletv2.Clear();
        //}

        var items = PhieuCanTPFilletv2ViewModel.Instance.GetTongHopThanhPhamFillet<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamTPFilletv2)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamTPFilletv2.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopThanhPhamBTPXeBuom(DateTime fromDate, DateTime toDate, string xuongId)
    {

        var items = PhieuCanTPFilletViewModel.Instance.GetTongHopThanhPhamBTPXeBuoms<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamBTPXeBuom)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamBTPXeBuom.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }
    public void TongHopThanhPhamTPXeBuom(DateTime fromDate, DateTime toDate, string xuongId)
    {

        var items = PhieuCanTPFilletViewModel.Instance.GetTongHopThanhPhamTPXeBuoms<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamTPXeBuom)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamTPXeBuom.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }
    public void TongHopThanhPhamRaCois(DateTime fromDate, DateTime toDate, string xuongId)
    {

        var items = PhieuCanRaCoiViewModel.Instance.GetTongHopThanhPhamRaCois<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamRaCoi)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamRaCoi.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopDinhMucSanSanLuongTheoChuyenDinhHinh(DateTime dateTime,DateTime dateTime2, string xuongId)
    {

        var items = PhieuCanTPDinhHinhViewModel.Instance.GetTongHopDinhMucSanLuongTheoNhom<object>(dateTime,dateTime2, xuongId);
        if (items.Any())
            lock (ItemTongHopDinhMucSanLuongTheoChuyenDinhHinh)
            {
                try
                {
                    foreach (var item in items) ItemTongHopDinhMucSanLuongTheoChuyenDinhHinh.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopDinhMucSanSanLuongTheoChuyenFillet(DateTime dateTime,DateTime dateTime2, string xuongId)
    {

        var items = PhieuCanTPFilletv2ViewModel.Instance.GetTongHopDinhMucSanLuongTheoNhom<object>(dateTime,dateTime2, xuongId);
        if (items.Any())
            lock (ItemTongHopDinhMucSanLuongTheoChuyenFillet)
            {
                try
                {
                    foreach (var item in items) ItemTongHopDinhMucSanLuongTheoChuyenFillet.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopDinhMucSanSanLuongTheoThanhPhamDinhHinh(DateTime dateTime, string xuongId)
    {

        var items = PhieuCanTPDinhHinhViewModel.Instance.GetTongHopDinhMucSanLuongTheoThanhPham<object>(dateTime, xuongId);
        if (items.Any())
            lock (ItemTongHopDinhMucSanLuongTheoThanhPhamDinhHinh)
            {
                try
                {
                    foreach (var item in items) ItemTongHopDinhMucSanLuongTheoThanhPhamDinhHinh.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }
    public void TongHopTyLeThoiGianVaDinhMucFillet(DateTime dateTime,DateTime dateTime2, string xuongId)
    {

        var items = PhieuCanTPFilletv2ViewModel.Instance.GetTongHopTyLeThoiGianVaDinhMuc<object>(dateTime, dateTime2, xuongId,15);
        if (items.Any())
            lock (ItemTongHopTyLeThoiGianVaDinhMucFillet)
            {
                try
                {
                    foreach (var item in items) ItemTongHopTyLeThoiGianVaDinhMucFillet.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }
    public void TongHopTyLeThoiGianVaDinhMucDinhHinh(DateTime dateTime,DateTime dateTime2, string xuongId)
    {
        int mocThoiGian = AppViewModel.Instance.MocThoiGian1;
        var items = PhieuCanTPDinhHinhViewModel.Instance.GetTongHopTyLeThoiGianVaDinhMuc<object>(dateTime, dateTime2, xuongId, mocThoiGian);
        if (items.Any())
            lock (ItemTongHopTyLeThoiGianVaDinhMucDinhHinh)
            {
                try
                {
                    foreach (var item in items) ItemTongHopTyLeThoiGianVaDinhMucDinhHinh.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public void TongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhom(DateTime dateTime,DateTime dateTime2, string xuongId)
    {
        int mocThoiGian1 = AppViewModels.AppViewModel.Instance.MocThoiGian1;
        int mocThoiGian2 = AppViewModels.AppViewModel.Instance.MocThoiGian2;

        var items = PhieuCanTPDinhHinhViewModel.Instance.GetTongHopTyLeThoiGianVaDinhMucTheoNhom<object>(dateTime, dateTime2, xuongId, mocThoiGian1, mocThoiGian2);
        if (items.Any())
            lock (ItemTongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhom)
            {
                try
                {
                    foreach (var item in items) ItemTongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhom.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    #region HQ
    public void TongHopThanhPhamHQ(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamTPFilletv2)
        //{
        //    ItemTongHopThanhPhamTPFilletv2.Clear();
        //}

        var items = HQ_PhieuCanViewModel.Instance.GetTongHopThanhPhamDashboards<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamHQ)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamHQ.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }
    public void TongHopThanhPhamHQGrid(DateTime fromDate, DateTime toDate, string xuongId)
    {
        //lock (ItemTongHopThanhPhamTPFilletv2)
        //{
        //    ItemTongHopThanhPhamTPFilletv2.Clear();
        //}

        var items = HQ_PhieuCanViewModel.Instance.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
        if (items.Any())
            lock (ItemTongHopThanhPhamHQGrid)
            {
                try
                {
                    foreach (var item in items) ItemTongHopThanhPhamHQGrid.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }
    #endregion
}





