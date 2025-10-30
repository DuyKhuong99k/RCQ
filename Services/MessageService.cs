using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Repos.Hubs.IServices;

namespace Services
{
    public class MessageService : IMessageService
    {
        public event Action<string>? AlertRequested;

        public void Show(string message)
        {
            AlertRequested?.Invoke(message);
        }
    }
}
