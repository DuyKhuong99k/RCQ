using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface IToastService
    {
        public event Action<ToastOption> ShowToastTrigger;
        public void ShowToast(ToastOption options);
        public void ShowToast(String? content,String title ="Thông Báo",  ToastType type = ToastType.Info);
        public void ShowToast(string? content, ToastType type);
        Task ShowConfirmationToast(string content, string title, Func<bool, Task> callback);
    }
}
