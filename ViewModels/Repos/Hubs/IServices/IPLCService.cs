using Models.Repos.Models;

namespace ViewModels.Repos.Hubs.IServices;

public interface IPLCService : IDisposable
{
    public void Connect();

    public void Disconnect();
    public void GetParameter(ref PLCChiTiet plcChiTiet);
    public void SetParameter(PLCChiTiet plcChiTiet);

    public void SetPause(PLCChiTiet plcChiTiet);

    public void SetResume(PLCChiTiet plcChiTiet);
    public Task SetRun(PLCChiTiet plcChiTiet);
    public void SetRUNOUT(PLCChiTiet plcChiTiet);
    public Task SetStop(PLCChiTiet plcChiTiet);

    public int WriteLogs(string actionName, PLCChiTiet plcChiTiet, bool isError = false, string? errorStr = null);
}