using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class ToastOption
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public ToastPositionX PositionX { get; set; } = ToastPositionX.Right;
        public ToastPositionY PositionY { get; set; } = ToastPositionY.Bottom;
        public bool IsShowCloseButton { get; set; } = true;
        public int Timeout { get; set; } = 5000;
        public int ExtendedTimeout { get; set; } = 3000;
        public string Theme { get; set; } = "light";
        public string Icon { get; set; } = "e-info toast-icons";
        public bool IsShowProgressBar { get; set; } = true;
        public string CssClass { get; set; } = "e-toast-info";
        public List<ToastButtonOption>? Buttons { get; set; } // Danh sách nút tùy chỉnh
      
        public void SetType(ToastType type)
        {
            switch (type)
            {
                case ToastType.Info:
                    Icon = "e-info toast-icons";
                    CssClass = "e-toast-info";
                    break;
                case ToastType.Success:
                    Icon = "e-success toast-icons";
                    CssClass = "e-toast-success";
                    break;
                case ToastType.Warning:
                    Icon = "e-warning toast-icons";
                    CssClass = "e-toast-warning";
                    break;
                case ToastType.Error:
                    Icon = "e-error toast-icons";
                    CssClass = "e-toast-danger";
                    break;
            }
        }
    }
    public class ToastButtonOption
    {
        public string Text { get; set; } // Văn bản hiển thị trên nút
        public string CssClass { get; set; } // Lớp CSS cho nút
        public Func<bool, Task>? Callback { get; set; } // Hàm xử lý khi nhấn nút
    }
    public enum ToastType
    {
        Info,
        Success,
        Warning,
        Error
    }
    public enum ToastPositionX
    {
        Left,
        Right,
        Center
    }
    public enum ToastPositionY
    {
        Top,
        Bottom
    }
}
