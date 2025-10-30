using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Services
{
    public partial class UserService(IMayCansService mayCansService,IMainService mainService) : ObservableObject, IUserService
    {
        //[ObservableProperty] private HashSet<ClientInfo> clients = [];
        [ObservableProperty] private ConcurrentDictionary<string, ClientInfo> clients = new();
        [ObservableProperty] private ObservableRangeCollection<UserArea> userAreas =new();
        public void RemoveByConnectedId(string connectedId)
        {
            // Tìm ClientInfo có ConnectedId trùng khớp và remove nó từ HashSet
            //var clientToRemove = Clients.FirstOrDefault(client => client.ConnectedId == connectedId);
            //if (clientToRemove != null) Clients.Remove(clientToRemove);
            if (Clients.ContainsKey(connectedId))
            {
                Clients.TryRemove(connectedId, out _);
            }
        }
        
        public ClientInfo? GetByConnectedId(string connectedId)
        {
            //var item = Clients.FirstOrDefault(x => x.ConnectedId == connectedId);
            //return item;
            if (Clients.ContainsKey(connectedId))
            {
                return Clients[connectedId];
            }
            return null;
        }

        public void AddUserArea(int userId)
        {
            var items = mainService.VmUserArea.Gets<UserArea>(userId);
            if(items != null && items.Any()) {
                {
                    RemoveUserArea(userId);
                    foreach (var item in items)
                    {
                        UserAreas.Add(item);
                    }
                }
            }
        }
        public void RemoveUserArea(int userId)
        {
            var findItems = UserAreas.Where(x => x.UserId == userId).ToList();
            if(findItems != null && findItems.Any())
            {
                foreach (var item in findItems)
                {
                    UserAreas.Remove(item);
                }
            }
        }
        public void Set(ClientInfo clientInfo, string id, int wType, bool isActive = true)
        {
            //var item = Clients.FirstOrDefault(x => x.ConnectedId == clientInfo.ConnectedId);
            //if (item == null) return;
            if (!Clients.TryGetValue(clientInfo.ConnectedId, out var item)) return;
            var mayCan = mayCansService.Find(id);
            if (mayCan != null)
            {
                if (wType != -1)
                {
                    item.WKv = (AppKV)wType;
                }
                else
                {
                    item.WKv = mayCan.WKv;
                }

                item.IsActive = isActive;
                item.Id = id;
                item.IsMayCan = true;
                mayCan.IsActive = isActive;
                mayCan.ConnectionId = clientInfo.ConnectedId;
                mayCan.DateTimeConnected = clientInfo.DateTimeConnected;
                //mayCan.IPAddr = clientInfo.IPAddr;
                mayCan.IsConnected = true;
                mayCan.WKv = item.WKv;

                mayCansService.NotifyItemChanged(mayCan);

            }
            else
            {

                item.WKv = (AppKV)wType;

                item.IsActive = isActive;
                item.Id = id;
                item.IsMayCan = false;
            }

        }
    }
}
