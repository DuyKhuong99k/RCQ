using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vars;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface ICommitService
    {
        public Tuple<bool, string>  Commit(List<DataCoummunication> datas,ClientInfo client,string cmd);
        public Tuple<bool, string>  Commit(string data,ClientInfo client,string cmd);
        public Tuple<bool, string> CheckTheId(string data, AppKV kv,string cmd);
        public Task<Tuple<bool, string>> SetChiSanLuong(string dataJson, ClientInfo client, string cmd);
        public Task<Tuple<bool, string>> SetXacDinh(string dataJson, ClientInfo client, string cmd);
        public Task<Tuple<bool, string>> CommitLite(string dataJson, ClientInfo client, string cmd);
        public Task<Tuple<bool, string>> CommitCoi(string dataJson, ClientInfo client, string cmd);
    }
}
