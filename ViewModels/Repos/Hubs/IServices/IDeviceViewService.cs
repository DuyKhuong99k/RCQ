using Models.Repos.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vars;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface IDeviceViewService : INotifyPropertyChanged,IDisposable
    {
        public ObservableRangeCollection<MayCan> Items { get; set; }
        
        public List<MayCan> SelectedItems { get; set; }
        public XiNghiep? XiNghiep { get; set; }
        public void Load(int userId);
        public void Clear();
        public event EventHandler? DevicesChanged;
        public string DeviceSelectedId { get; set; }
        public MayCan? DeviceCard { get; set; }
    }
}
