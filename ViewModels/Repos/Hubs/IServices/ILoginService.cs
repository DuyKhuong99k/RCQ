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
        public bool Login(string username,string tocken);
        bool IsUserAuthenticated();
        public bool IsSessionCreated();
        void Login(string username);
        bool LoginLocal(string userName,string passWord);
        Task Logout();
        void CreateDefaultSession();
        public ObservableRangeCollection<UserArea> UserAreas {get; set; }
    }
}
