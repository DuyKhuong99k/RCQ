using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class ClientInfoComparer : IEqualityComparer<ClientInfo>
    {
        public bool Equals(ClientInfo x, ClientInfo y)
        {
            // So sánh các phần tử dựa trên ConnectedId
            return x?.ConnectedId == y?.ConnectedId;
        }

        public int GetHashCode(ClientInfo obj)
        {
            // Lấy mã băm từ ConnectedId
            return obj?.ConnectedId?.GetHashCode() ?? 0;
        }
    }
}
