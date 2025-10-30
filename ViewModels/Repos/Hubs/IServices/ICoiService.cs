using System.Collections.Concurrent;
using System.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices;

public interface ICoiService : INotifyPropertyChanged, IDisposable
{
    public ObservableRangeCollection<PLCChiTiet> Items { get; set; }
    public ConcurrentDictionary<string, MaCoiXepKhuon> CoiInfos { get; set; }
    public List<PLCChiTiet> SelectedItems { get; set; }
    public event EventHandler? ItemChanged;
    public bool Command_PAUSE(PLCChiTiet item);

    public bool Command_RESUME(PLCChiTiet item);

    public Task<bool> Command_RUN(PLCChiTiet item);

    public bool Command_RUNOUT(PLCChiTiet item);

    public bool Command_SETPARAMETER(PLCChiTiet item);
    public bool Command_SETPARAMATERANDDATA(PLCChiTiet item);
    public bool Command_STARTTIMER();
    public bool Command_SETDATA(PLCChiTiet item);

    public Task<bool> Command_STOP(PLCChiTiet item);
    public PLCChiTiet? Find(string id);
    public PLCChiTiet? Find(string coiId, string xuongId);
    public void Load();
    public void NotifyItemChanged();
    public void Remove(string id);
    public void Update(PLCChiTiet item);
    public DataCoi GetDataCoi(PLCChiTiet item);
}