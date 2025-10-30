using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppModels;

namespace AppViewModels
{
    public partial class MessageViewModel : ObservableObject
    {
        private static MessageViewModel _instance;
        [ObservableProperty] private Exception? _exception;
        [ObservableProperty] private Message _item = new();
        [ObservableProperty] private bool isShow = false;

        /// <summary>
        /// Arg1: messString, Arg2: title,Arg3 + Arg4: type button return Arg4
        /// <summary>The message box displays an OK button.</summary>
        /// OK = 0,
        /// <summary>The message box displays OK and Cancel buttons.</summary>
        ///OKCancel = 1,
        /// <summary>The message box displays Yes, No, and Cancel buttons.</summary>
        ///YesNoCancel = 3,
        /// <summary>The message box displays Yes and No buttons.</summary>
        ///YesNo = 4,
        /// return Arg4
        /// <summary>The message box returns no result.</summary>
        ///None = 0,
        /// <summary>The result value of the message box is OK.</summary>
        ///OK = 1,
        /// <summary>The result value of the message box is Cancel.</summary>
        ///Cancel = 2,
        /// <summary>The result value of the message box is Yes.</summary>
        ///Yes = 6,
        /// <summary>The result value of the message box is No.</summary>
        ///No = 7,
        /// </summary>
        [ObservableProperty] private Func<string, string, int, int> _messageBoxShow;

        private MessageViewModel()
        {
        }

        public static MessageViewModel Instance => _instance ??= new();

        [RelayCommand]
        private void SetException(Exception exception)
        {
            try
            {
                Exception = exception;
                var item = new Message()
                {
                    MessageString = exception.ToString()
                };
                if (IsShow == true)
                {
                    MessageBoxShow(item.MessageString, "Exception", 0);
                }
                //SetMessage(item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        [RelayCommand]
        private void SetMessage(Message item)
        {
            try
            {
                Item = item;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }
    }
}
