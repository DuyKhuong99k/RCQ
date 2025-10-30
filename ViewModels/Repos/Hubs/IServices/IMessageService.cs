using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface IMessageService
    {
        public event Action<string> AlertRequested;
        public void Show(string message);
    }
}
