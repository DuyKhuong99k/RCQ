using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Repos.Models;
using MvvmHelpers;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface ILoginService: INotifyPropertyChanged
    {
        bool IsUserAuthenticated();
        public string GetSessionVal(string key);
        public bool IsSessionCreated();
        bool LoginLocal(string userName,string passWord);
        Task Logout();
        void CreateDefaultSession();
        public ObservableRangeCollection<UserArea> UserAreas {get; set; }
    }
}
