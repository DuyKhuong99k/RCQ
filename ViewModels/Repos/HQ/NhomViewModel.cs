using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.HQ
{
    internal class NhomViewModel
    {
        private static NhomViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static NhomViewModel Instance => instance ??= new NhomViewModel();
    }
}
