using System.Timers;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.A_Model;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;

namespace ViewModels.Repos.HQ;

public partial class DashboardViewModel : ObservableObject
{
    private static DashboardViewModel instance;
    private readonly Timer _timer;
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
            _timer = new Timer { Interval = 1000, AutoReset = true }; // 90 giây
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            //VmMessage.SetExceptionCommand.Execute(e);
        }
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

        UserList.Add(newUser);
    }

    private List<ListUserWaitView> GetKeepListUserWaitView(List<ListUserWaitView> userList)
    {
        var currentTime = DateTime.Now;

        // Lọc các phần tử có thời gian lớn hơn hoặc bằng thời gian hiện tại

        //var list = userList.Where(x => x != null).ToList();
        //var filteredUsers = list.Where(x => (currentTime - x.ThoiGian).Milliseconds < vmApp.IntervalDashBoard).ToList();
 
        var filteredUsers = userList.Where(x => x != null && (currentTime - x.ThoiGian).Milliseconds < vmApp.IntervalDashBoard)
            .GroupBy(x => x.XuongId)
            .Select(g => new ListUserWaitView { XuongId = g.Key, ThoiGian = g.Max(x => x.ThoiGian) , Id = ""})
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
}