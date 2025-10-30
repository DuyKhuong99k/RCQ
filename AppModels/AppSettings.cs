using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace AppModels;

[JsonObject(MemberSerialization.OptOut)]
public partial class AppSettings : ObservableObject
{
    [ObservableProperty] private string _db = "PMS_HQ";

    [ObservableProperty] private string _pass = "Sql@123456789";

    [ObservableProperty] private string _readerPort = "COM1";

    //[ObservableProperty] private string _serverName = "data.pms-vn.com,4751";
    [ObservableProperty] private string _serverName = ".";

    [ObservableProperty] private int _timeOut = 3;

    [ObservableProperty] private string _usr = "pmsvn";

    [ObservableProperty] private string _dbBravo = "";
    [ObservableProperty] private string _usrBravo = "";
    [ObservableProperty] private string _passBravo = "";
    [ObservableProperty] private string _serverNameBravo = "";

    public AppSettings GetValues()
    {
        return new AppSettings()
        {
            Db = "PMS",
            Pass = Pass,
            ReaderPort = ReaderPort,
            Usr = Usr,
            ServerName = "115.74.218.101,2400",
            TimeOut = 10,
            DbBravo = DbBravo,
            PassBravo = PassBravo,
            UsrBravo = UsrBravo,
            ServerNameBravo = ServerNameBravo
        };
    }

}