using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AppModels
{
    public partial class Message : ObservableObject
    {
        [ObservableProperty] private string _codeId = string.Empty;
        [ObservableProperty] private string _messageString = string.Empty;
        [ObservableProperty] private string _messageType = string.Empty;
        [ObservableProperty] private string _colorCode = string.Empty;

    }
}
