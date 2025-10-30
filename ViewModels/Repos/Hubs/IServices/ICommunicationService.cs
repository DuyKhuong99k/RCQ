using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface ICommunicationService
    {
       public bool AuthProc(string connectedId, string id, int wType, string dataJson);
       public Task<string> Route(ClientInfo? client, string cmd, int len, string dataJson);
       
    }
}
