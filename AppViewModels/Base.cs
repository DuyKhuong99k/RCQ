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
    public partial class Base: ObservableObject
    {
        private static Base _ins;

        
        [ObservableProperty, NotifyPropertyChangedFor(nameof(ConnectionString))] 
        private string _db = string.Empty;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _pass = string.Empty;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _serverName = string.Empty;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private int _timeOut = 3;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(ConnectionString))]
        private string _usr = string.Empty;

        private Base()
        {
        }
        
        /// <summary>
        ///     Connection String
        /// </summary>
        public string ConnectionString =>
            $@"data source={ServerName};initial catalog={Db};user id={Usr};password={Pass};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout={TimeOut};TrustServerCertificate=True;";
        public string ConnectionStringBravo { get; set; } =
            @"data source=115.74.218.101,2400;initial catalog=DataTransfer;user id=pms;password=@Pms#2020;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True;";
        [ObservableProperty] private string? connectionString2 =null;
        public static Base Ins => _ins ??= new ();
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
        }

    }
}
