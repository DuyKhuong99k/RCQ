using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using AppModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using Newtonsoft.Json;
using Vars;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;
using AppModels;
using Security.Crypt;

namespace AppViewModels
{
    public partial class AppViewModel : ObservableObject
    {
        private static AppViewModel _instance;
        private readonly Timer _timer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PreMonthDatetime))]
        private DateTime _dateTimeNow = DateTime.Now;

        [ObservableProperty] private ICommand _showDialogCommand;
        [ObservableProperty] private ICommand _closeWindow;
        [ObservableProperty] private string _pCName = Environment.MachineName;
        [ObservableProperty] private AppSettings _settings = new();
        [ObservableProperty] private string _theId = "";
        [ObservableProperty] private DateTime _timeRe = DateTime.Now;
        [ObservableProperty] private double _timeCardNextRead = 1000;
        [ObservableProperty] private decimal _trongLuong = 0;
        [ObservableProperty] private string apiHostUrl = @"https://localhost:7073";
        [ObservableProperty] private string apiWssHostUrl = @"wss://localhost:7073";

        [ObservableProperty] private string hostUrl = @"https://localhost:7208";

        //[ObservableProperty] private string hubsMayCanURL = @"https://192.168.1.5:7251";
        [ObservableProperty] private string hubsMayCanCODE = @"CODENAME";
        [ObservableProperty] private string redirectLoginUrl = @"/Authentication/Login";
        [ObservableProperty] private string redirectHomeUrl = @"/Home/Index";
        [ObservableProperty] private DateTime _dateReport = DateTime.Now;

        [ObservableProperty] private string _xuongId = "1";

        //[ObservableProperty] private AppType appType = AppType._default;
        [ObservableProperty] private AppType dinhHinhAppType = AppType._default;
        [ObservableProperty] private AppType filletAppType = AppType._type4;
        [ObservableProperty] private string _userName = "default";
        [ObservableProperty] private AppKV loKv = AppKV.NguyenLieu;
        [ObservableProperty] private string? comName = "PMS";
        [ObservableProperty] private Func<bool> checkServerOnline;
        [ObservableProperty] private string httpClientName = "HttpClientName";
        [ObservableProperty] private string appPath = $@"{AppDomain.CurrentDomain.BaseDirectory}";
        [ObservableProperty] private DateTime fromDate = DateTime.Now;
        [ObservableProperty] private string _defaultKey = "PMS_VN";

        [ObservableProperty]
        private ObservableRangeCollection<Tuple<int, string>> tabXKNames =
            new ObservableRangeCollection<Tuple<int, string>>();

        [ObservableProperty] private int maxRowsView = 15;
        [ObservableProperty] private int maxRowsLogView = 20;
        [ObservableProperty] private bool isUseVirtualKeyboard = true;
        [ObservableProperty] private bool apDungTyLeDauRotQuaMuc = true;
        [ObservableProperty] private bool isShowMoney = true;
        [ObservableProperty] private int timeSetIntervalDashBoardView = 10000;
        [ObservableProperty] private long intervalDashBoard = 25000; //120000;
        [ObservableProperty] private decimal chiSoTyLeTangTrongRaCoi = 0;
        [ObservableProperty] private int mocThoiGian1;
        [ObservableProperty] private int mocThoiGian2;
        [ObservableProperty] private int mocThoiGianFL;
        [ObservableProperty] public decimal chiSoKyVongFL = 770;
        [ObservableProperty] public decimal chiSoKyVongDH = 130;
        [ObservableProperty] public string messageExpityNotece = "";
        [ObservableProperty] public string company = ""; //pms or phanbach
        /// <summary>
        /// Thời Gian hết hạn ghi nhận của 1 thẻ tính bằng ms
        /// </summary>
        [ObservableProperty] private int cardExpired = 5000;

        #region Chắt Thêm ẩn hiên UC

        [ObservableProperty] bool _isPanelVisible;
        [ObservableProperty] private ICommand _showPanelCommand;
        [ObservableProperty] private ICommand _hidePanelCommand;

        #endregion

        private AppViewModel()
        {
#if DEBUG
            ////Setting Database
            //ComName = "RQTG";
            //var modelDatabaseConnectSetting = GetJsonDatabaseConnectSetting();


            ////Set lại thông tin cứng
            //var setting = Settings.GetValues();
            ////if (modelDatabaseConnectSetting != null)
            ////{
            ////    setting.Db = modelDatabaseConnectSetting.Db;
            ////    setting.Pass = modelDatabaseConnectSetting.Pass;
            ////    setting.ReaderPort = modelDatabaseConnectSetting.ReaderPort;
            ////    setting.ServerName = modelDatabaseConnectSetting.ServerName;
            ////    setting.TimeOut = modelDatabaseConnectSetting.TimeOut;
            ////    setting.Usr = modelDatabaseConnectSetting.Usr;
            ////    Settings = setting;
            ////}
            //////Setting WebApp
            ////var modelWebAppSetting = GetJsonDataWebAppSetting();
            ////if (modelWebAppSetting != null)
            ////{
            ////    ComName = modelWebAppSetting.ComName;
            ////    ApiHostUrl = modelWebAppSetting.ApiHostUrl;
            ////    ApiWssHostUrl = modelWebAppSetting.ApiWssHostUrl;
            ////    HostUrl = modelWebAppSetting.HostUrl;
            ////    HubsMayCanCODE = modelWebAppSetting.HubMayCanCODE;
            ////    LoKv = modelWebAppSetting.LoKv;
            ////}
            //LoKv = AppKV.Hq;
            //setting.Db = "PMS_HQ";
            //setting.ServerName = "data.pms-vn.com,4751";
            ////setting.ServerName = "data.pms-vn";
            //setting.TimeOut = 30;
            ////Base.Ins.SetConnectionStringCommand.Execute(setting);
            //Base.Ins.SetConnectionStringCommand.Execute(setting);
            ////Base.Ins.SetConnectionStringCommand.Execute(Settings.GetValues());
            ////Base.Ins.SetConnectionStringCommand.Execute(Settings.GetValues());
            //Base.Ins.ConnectionStringBravo = Base.Ins.ConnectionString;

            //Base.Ins.ConnectionString2 = Base.Ins.ConnectionString;
            ////ApiHostUrl = @"https://data.pms-vn.com:8999";
            //ApiHostUrl = @"https://localhost:44371";
            ////Base.Ins.ConnectionString2 = "server=data.pms-vn.com;database=PMS_HQ;uid=pmsvn;pwd=Sql@123456789;TrustServerCertificate=True";

            var modelDatabaseConnectSetting = GetJsonDatabaseConnectSetting();


            //Set lại thông tin cứng
            var setting = Settings.GetValues();
            if (modelDatabaseConnectSetting != null)
            {
                setting.Db = modelDatabaseConnectSetting.Db;
                setting.Pass = modelDatabaseConnectSetting.Pass;
                setting.ReaderPort = modelDatabaseConnectSetting.ReaderPort;
                setting.ServerName = modelDatabaseConnectSetting.ServerName;
                setting.TimeOut = modelDatabaseConnectSetting.TimeOut;
                setting.Usr = modelDatabaseConnectSetting.Usr;
                //kết nối bravo
                setting.DbBravo = modelDatabaseConnectSetting.DbBravo;
                setting.UsrBravo = modelDatabaseConnectSetting.UsrBravo;
                setting.PassBravo = modelDatabaseConnectSetting.PassBravo;
                setting.ServerNameBravo = modelDatabaseConnectSetting.ServerNameBravo;
                Settings = setting;
            }

            //Settings.ServerName = "10.10.26.50,1566";
            //Settings.ServerName = "data.pms-vn.com,4751";
            Settings.TimeOut = 30;
            Settings.ServerName = "192.168.1.163";
            setting.Db = "PMS_HQ";
            Base.Ins.SetConnectionStringCommand.Execute(setting);
            //Base.Ins.SetConnectionStringCommand.Execute(Settings.GetValues());
            Base.Ins.ConnectionString2 = Base.Ins.ConnectionString;
            //ComName ="CMX";
            ////Setting WebApp
            var modelWebAppSetting = GetJsonDataWebAppSetting();
            if (modelWebAppSetting != null)
            {
                ComName = modelWebAppSetting.ComName;
                ApiHostUrl = modelWebAppSetting.ApiHostUrl;
                ApiWssHostUrl = modelWebAppSetting.ApiWssHostUrl;
                HostUrl = modelWebAppSetting.HostUrl;
                HubsMayCanCODE = modelWebAppSetting.HubMayCanCODE;
                LoKv = modelWebAppSetting.LoKv;
            }
            ComName = nameof(ComNames.NV);
            LoKv = AppKV.Hq;
            ChiSoTyLeTangTrongRaCoi = 1.07m;
            MocThoiGian1 = 30;
            MocThoiGian2 = 40;
            MocThoiGianFL = 15;
            MessageExpityNotece = "";
            if (ComName == nameof(ComNames.HL))
            {
                MessageExpityNotece = "Chúng tôi trân trọng đề nghị Quý khách xác nhận nghiệm thu dự án vào ngày 20/08/2025.<br>Vui lòng phản hồi để chúng tôi hoàn tất thủ tục";
            }
            Company = "PhanBach";
#else
            //ComName = "RQTG";
            var modelDatabaseConnectSetting = GetJsonDatabaseConnectSetting();


            //Set lại thông tin cứng
            var setting = Settings.GetValues();
            if (modelDatabaseConnectSetting != null)
            {
                setting.Db = modelDatabaseConnectSetting.Db;
                setting.Pass = modelDatabaseConnectSetting.Pass;
                setting.ReaderPort = modelDatabaseConnectSetting.ReaderPort;
                setting.ServerName = modelDatabaseConnectSetting.ServerName;
                setting.TimeOut = modelDatabaseConnectSetting.TimeOut;
                setting.Usr = modelDatabaseConnectSetting.Usr;
//kết nối bravo
                setting.DbBravo = modelDatabaseConnectSetting.DbBravo;
                setting.UsrBravo = modelDatabaseConnectSetting.UsrBravo;
                setting.PassBravo = modelDatabaseConnectSetting.PassBravo;
                setting.ServerNameBravo = modelDatabaseConnectSetting.ServerNameBravo;
                Settings = setting;
            }

            Base.Ins.SetConnectionStringCommand.Execute(setting);
            //Base.Ins.SetConnectionStringCommand.Execute(Settings.GetValues());
            Base.Ins.ConnectionString2 = Base.Ins.ConnectionString;
            //ComName ="CMX";
            ////Setting WebApp
            var modelWebAppSetting = GetJsonDataWebAppSetting();
            if (modelWebAppSetting != null)
            {
                ComName = modelWebAppSetting.ComName;
                ApiHostUrl = modelWebAppSetting.ApiHostUrl;
                ApiWssHostUrl = modelWebAppSetting.ApiWssHostUrl;
                HostUrl = modelWebAppSetting.HostUrl;
                HubsMayCanCODE = modelWebAppSetting.HubMayCanCODE;
                LoKv = modelWebAppSetting.LoKv;
            }
            ChiSoTyLeTangTrongRaCoi = 1.07M;
            MocThoiGian1 = 30;
            MocThoiGian2 = 40;
            MocThoiGianFL = 15;
            MessageExpityNotece = "";
            if (ComName == nameof(ComNames.HL))
            {
                MessageExpityNotece = "Chúng tôi trân trọng đề nghị Quý khách xác nhận nghiệm thu dự án vào ngày 20/08/2025.<br>Vui lòng phản hồi để chúng tôi hoàn tất thủ tục";
            } 
            Company = "PhanBach";

#endif
            //ApiHostUrl = @"https://data.pms-vn.com:8999";
            //HubsMayCanURL = @"https://192.168.1.5:7251";

            HubsMayCanCODE = @"CODENAME";

            //Base.Ins.SetConnectionStringCommand.Execute(Settings.GetValues());

            _timer = new Timer { Interval = 100, AutoReset = true };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            #region Chắt thêm Show hide UCView

            // Set Default Panel Visibility //
            IsPanelVisible = false;

            #endregion

            GenerateTabXKNames();
        }

        #region Setting

        public class WebAppSettingModel
        {
            public string ComName { get; set; }
            public string ApiHostUrl { get; set; }
            public string ApiWssHostUrl { get; set; }
            public string HostUrl { get; set; }
            public string HubMayCanCODE { get; set; }
            public AppKV LoKv { get; set; }
        }

        public WebAppSettingModel? GetJsonDataWebAppSetting()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "WebAppSetting.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var firstLine = "";
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                firstLine = lines[0];
            }

            //Giai ma chuoi json string
            var key = DefaultKey;
            var decryptJson = "";
            if (firstLine != null)
            {
                decryptJson = ED.DecryptString(firstLine, key);
            }

            // Deserialize dữ liệu thành đối tượng WebAppSettingModel
            var jsonDone = JsonConvert.DeserializeObject<WebAppSettingModel>(decryptJson);

            return jsonDone;
        }

        public class DatabaseConnectSettingModel
        {
            public string Db { get; set; }
            public string Pass { get; set; }
            public string ReaderPort { get; set; }
            public string ServerName { get; set; }
            public int TimeOut { get; set; }
            public string Usr { get; set; }
            public string DbBravo { get; set; }
            public string UsrBravo { get; set; }
            public string PassBravo { get; set; }
            public string ServerNameBravo { get; set; }
        }

        public DatabaseConnectSettingModel? GetJsonDatabaseConnectSetting()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc",
                "DatabaseConnectSetting.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var firstLine = "";
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                firstLine = lines[0];
            }

            //Giai ma chuoi json string
            var key = DefaultKey;
            var decryptJson = "";
            if (firstLine != null)
            {
                decryptJson = ED.DecryptString(firstLine, key);
            }

            // Deserialize dữ liệu thành đối tượng WebAppSettingModel
            var jsonDone = JsonConvert.DeserializeObject<DatabaseConnectSettingModel>(decryptJson);

            return jsonDone;
        }

        #endregion

        private void GenerateTabXKNames()
        {
            TabXKNames.Clear();
            TabXKNames.AddRange(
                new List<Tuple<int, string>>
                {
                    new Tuple<int, string>(0, "XK_Phu"),
                    new Tuple<int, string>(1, "XK_Chinh"),
                    new Tuple<int, string>(2, "XK_KXL"),
                    new Tuple<int, string>(3, "XK_Block"),
                    new Tuple<int, string>(4, "XK_TaiChe"),
                    new Tuple<int, string>(5, "XK_PhuGia")
                });
        }

        public static AppViewModel Instance => _instance ??= new AppViewModel();
        public DateTime PreMonthDatetime => _dateTimeNow.AddMonths(-1);

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var action = (DateTime arg) => { DateTimeNow = arg; };
                action.Invoke(DateTime.Now);
                //var datetime = new DateTime(2025, 05, 22);
                //datetime = datetime.Add(DateTime.Now.TimeOfDay);
                //action.Invoke(datetime);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                //throw;
            }
        }

        [ObservableProperty] public ICommand _shutDownCommand;

        public double GetTimeOut()
        {
            return Math.Abs((DateTimeNow - TimeRe).TotalMilliseconds);
        }

        public class HubsMayCanModel
        {
            public string NameMayCanHubs { get; set; }
            public string HubsMayCanUrl { get; set; }
            public int TypeMayCanHubs { get; set; }
        }

        public List<HubsMayCanModel> GetAllHubsUrlJsonData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "HubsUrl.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var jsonData = System.IO.File.ReadAllText(filePath);

            // Deserialize dữ liệu thành danh sách các đối tượng HubsMayCanModel
            var hubsMayCanDataList = JsonConvert.DeserializeObject<List<HubsMayCanModel>>(jsonData);

            return hubsMayCanDataList;
        }

        #region chắt thêm ẩn hiện UCView

        [RelayCommand]
        public void ShowPanel_()
        {
            IsPanelVisible = true;
        }

        [RelayCommand]
        public void HidePanel_()
        {
            IsPanelVisible = false;
        }

        #endregion

        public string DecryBiosId(string sanitizedBiosId)
        {
            var BiosIdEncry = "";

            if (sanitizedBiosId.Contains("_"))
            {
                // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
                BiosIdEncry = sanitizedBiosId.Replace("_", "/");
            }
            else
            {
                // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
                BiosIdEncry = sanitizedBiosId;
            }

            var biosIdDecry = Security.Crypt.ED.DecryptString(BiosIdEncry);
            return biosIdDecry;
        }


        public void SetChiSoKyVongFL(decimal value)
        {
            ChiSoKyVongFL = value;
        }
        public void SetChiSoKyVongDH(decimal value)
        {
            ChiSoKyVongDH = value;
        }
    }
}