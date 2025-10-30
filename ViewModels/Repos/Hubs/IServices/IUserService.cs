using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Repos.Models;
using MvvmHelpers;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface IUserService: INotifyPropertyChanged
    {
        public HashSet<ClientInfo> Clients { get; }
        public void RemoveByConnectedId(string connectedId);

        public ClientInfo? GetByConnectedId(string connectedId);

        public void Set(ClientInfo clientInfo, string id, int wType, bool isActive = true);
        public ObservableRangeCollection<UserArea> UserAreas { get; set; }
        public void AddUserArea(int userId);
        public void RemoveUserArea(int userId);
    }
}
