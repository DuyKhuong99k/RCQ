using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

namespace Services
{
    public partial class ToastService: IToastService
    {
        public event Action<ToastOption>? ShowToastTrigger;
        public void ShowToast(ToastOption options)
        {
            ShowToastTrigger?.Invoke(options);
        }

        private const string NoContent = "Không có nội dung";

        public void ShowToast(string? content, string title = "Thông Báo", ToastType type = ToastType.Info)
        {
            var o = new ToastOption
            {
                Title = title,
                Content = content??NoContent
            };
            o.SetType(type);
            ShowToast(o);
        }
        public void ShowToast(string? content, ToastType type)
        {
            var o = new ToastOption
            {
                Title =  "Thông Báo",
                Content = content??NoContent
            };
            o.SetType(type);
            ShowToast(o);
        }
        public Task ShowConfirmationToast(string content, string title, Func<bool, Task> callback)
        {
            var toastOption = new ToastOption
            {
                Title = title,
                Content = content,
                Timeout = 0, // Không tự động đóng
                Buttons = new List<ToastButtonOption>
                {
                    new ToastButtonOption
                    {
                        Text = "Yes",
                        CssClass = "btn-success",
                        Callback = async (result) => await callback(true)
                    },
                    new ToastButtonOption
                    {
                        Text = "No",
                        CssClass = "btn-danger",
                        Callback = async (result) => await callback(false)
                    }
                }
            };
            ShowToastTrigger?.Invoke(toastOption);
            return Task.CompletedTask;
        }
    }
}
