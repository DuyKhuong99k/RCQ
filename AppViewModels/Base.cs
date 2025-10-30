using AppModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppViewModels
{
    public partial class Base : ObservableObject
    {
        private static Base _ins;


        [ObservableProperty, NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _db = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _pass = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _serverName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private int _timeOut = 3;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _usr = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionStringBravo))]
        private string _dbBravo = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionStringBravo))]
        private string _passBravo = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionStringBravo))]
        private string _usrBravo = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConnectionStringBravo))]
        private string _serverNameBravo = string.Empty;

        private Base()
        {
        }

        /// <summary>
        ///     Connection String
        /// </summary>
        public string ConnectionString =>
            $@"data source={ServerName};initial catalog={Db};user id={Usr};password={Pass};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout={TimeOut};TrustServerCertificate=True;";
        public string ConnectionStringBravo =>
            $@"data source={ServerNameBravo};initial catalog={DbBravo};user id={UsrBravo};password={PassBravo};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout={TimeOut};TrustServerCertificate=True;";

        [ObservableProperty] private string? connectionString2 = null;

        public static Base Ins => _ins ??= new();

        /// <summary>
        /// Set connection string
        /// </summary>
        /// <param name="settings">Not Null</param>
        [RelayCommand]
        private void SetConnectionString(AppSettings settings)
        {
            this.Db = settings.Db;
            this.Pass = settings.Pass;
            this.ServerName = settings.ServerName;
            this.TimeOut = settings.TimeOut;
            this.Usr = settings.Usr;
            this.DbBravo = settings.DbBravo;
            this.PassBravo = settings.PassBravo;
            this.UsrBravo = settings.UsrBravo;
            this.ServerNameBravo = settings.ServerNameBravo;
        }

    }
}
