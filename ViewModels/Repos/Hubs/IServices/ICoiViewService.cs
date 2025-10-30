using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Repos.Models;
using MvvmHelpers;
using Vars;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface ICoiViewService: INotifyPropertyChanged,IDisposable
    {
        public ObservableRangeCollection<PLCChiTiet> Items { get; set; }
        
        public List<PLCChiTiet> SelectedItems { get; set; }
        public XiNghiep? XiNghiepSelectedItem { get; set; }
        public bool Command_PAUSE(PLCChiTiet item);

        public bool Command_RESUME(PLCChiTiet item);

        public Task<bool> Command_RUN(PLCChiTiet item);

        public bool Command_RUNOUT(PLCChiTiet item);

        public bool Command_SETPARAMETER(PLCChiTiet item);
        public bool Command_SETPARAMATERANDDATA(PLCChiTiet item);
        public bool Command_STARTTIMER();
        public bool Command_SETDATA(PLCChiTiet item);

        public Task<bool> Command_STOP(PLCChiTiet item);
        public void Load();
        public void Clear();
        public event EventHandler? DevicesChanged;
    }
}
