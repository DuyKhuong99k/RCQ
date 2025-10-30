using Microsoft.AspNetCore.OutputCaching;
using Models.Repos.Models;
using SLMP;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Services;

public class PLCService(IMainService mainService) : ObservableObject, IPLCService
{
    private readonly int connTimeout = 1000;

    private readonly int recvTimeout = 1000;
    private readonly int sendTimeout = 1000;
    private readonly int timeDelay = 100;

    public void Connect()
    {
        foreach (var item in mainService.VmPLC.Items)
        {
            var cfg = new SlmpConfig();
            cfg.Initialize(item.IP, item.Port);
            cfg.ConnTimeout = connTimeout;
            cfg.RecvTimeout = recvTimeout;
            cfg.SendTimeout = sendTimeout;

            var client = new SlmpClient();
            client.Initialize(cfg);
            if (item.Client != null)
                if (item.Client.IsConnected())
                    item.Client.Disconnect();
            item.Client = client;

            // Check if the client is already connected
            if (!client.IsConnected())
            { var handeRl= Handlers.ErrorHandler.Handle(() =>
                {
                    client.Connect();
                });
                if (!handeRl.IsSuccess)
                {
                    // Log the error and continue with the next item
                    Console.WriteLine($"Failed to connect to {item.IP}:{item.Port}. Error: {handeRl.Error?.Message}");
                }
            }
        }
    }

    public void Disconnect()
    {
        var result = Handlers.ErrorHandler.Handle(() =>
        {
            foreach (var item in mainService.VmPLC.Items)
                if (item.Client != null)
                    if (item.Client.IsConnected())
                        item.Client.Disconnect();
        });
        if (!result.IsSuccess)
        {
            // Log the error
            Console.WriteLine($"Failed to disconnect from PLC. Error: {result.Error?.Message}");
        }
        
        //foreach (var item in mainService.VmPLC.Items)
        //    if (item.Client != null)
        //        if (item.Client.IsConnected())
        //            item.Client.Disconnect();
    }

    public void GetParameter(ref PLCChiTiet plcChiTiet)
    {
        var plcId = plcChiTiet.PLCId;
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcId);
        var oldItem = plcChiTiet;
        if (plc != null)
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    plcChiTiet.TanSoQuay = plc.Client.ReadWordDevice(plcChiTiet.HzQuay);
                    plcChiTiet.TanSoRa = plc.Client.ReadWordDevice(plcChiTiet.HzRa);
                    plcChiTiet.ThoiGianQuay = plc.Client.ReadWordDevice(plcChiTiet.TimeQuay);
                    var runstatus = plc.Client.ReadBitDevice(plcChiTiet.RUNSTATUS);
                    var pausestatus = plc.Client.ReadBitDevice(plcChiTiet.PAUSE);
                    var runoutstatus = plc.Client.ReadBitDevice(plcChiTiet.RUNOUT);

                    if (runstatus && !pausestatus && !runoutstatus)
                        plcChiTiet.State = 1;
                    else if (pausestatus && runoutstatus && !runoutstatus)
                        plcChiTiet.State = 2;
                    else if (runoutstatus && runoutstatus && !pausestatus)
                        plcChiTiet.State = 3;
                    else if (!runstatus)
                        plcChiTiet.State = -1;
                    

                    if (oldItem.State != plcChiTiet.State)
                    {
                        if (plcChiTiet.State == 1)
                        {
                            if (oldItem.State == 2)
                            {
                                WriteLogs(nameof(LogsActionName.RESUME_DETECT), plcChiTiet);
                            }
                            else
                            {
                                WriteLogs(nameof(LogsActionName.RUN_DETECT), plcChiTiet);
                                plcChiTiet.TimeRun = DateTime.Now;
                            }

                            plcChiTiet.IsPause = false;
                            plcChiTiet.IsRunOut = false;
                            plcChiTiet.IsPowerOn = true;
                        }
                        else if (plcChiTiet.State == 2)
                        {
                            WriteLogs(nameof(LogsActionName.PAUSE_DETECT), plcChiTiet);
                            plcChiTiet.IsPause = true;
                            plcChiTiet.IsRunOut = false;
                            plcChiTiet.IsPowerOn = true;
                        }
                        else if (plcChiTiet.State == -1)
                        {
                            WriteLogs(nameof(LogsActionName.STOP_DETECT), plcChiTiet);
                            plcChiTiet.TimeStop = DateTime.Now;
                            plcChiTiet.IsPause = false;
                            plcChiTiet.IsRunOut = false;
                            plcChiTiet.IsPowerOn = false;
                        }
                        else if (plcChiTiet.State == 3)
                        {
                            if (oldItem.State == 2)
                            {
                                WriteLogs(nameof(LogsActionName.RESUME_DETECT), plcChiTiet);
                            }
                            else
                            {
                                WriteLogs(nameof(LogsActionName.RUNOUT_DETECT), plcChiTiet);
                                plcChiTiet.TimeRaCoi = DateTime.Now;
                            }

                            plcChiTiet.IsRunOut = true;
                            plcChiTiet.IsPause = false;
                            plcChiTiet.IsPowerOn = true;
                        }
                        else if (plcChiTiet.State == 4)
                        {
                            WriteLogs(nameof(LogsActionName.RUNOUT_WAITING_DETECT), plcChiTiet);
                        }
                        else if (plcChiTiet.State == 0)
                        {
                            WriteLogs(nameof(LogsActionName.WAITING_DETECT), plcChiTiet);
                        }
                    }

                    if (oldItem.TanSoQuay != plcChiTiet.TanSoQuay || oldItem.TanSoRa != plcChiTiet.TanSoRa ||
                        oldItem.ThoiGianQuay != plcChiTiet.ThoiGianQuay)
                        WriteLogs(nameof(LogsActionName.PARAMETER_CHANGED_DETECT), plcChiTiet);

                    if (oldItem.IsConnected != plcChiTiet.IsConnected)
                    {
                        if (plcChiTiet.IsConnected)
                            WriteLogs(nameof(LogsActionName.CONNECTED_DETECT), plcChiTiet);
                        else
                            WriteLogs(nameof(LogsActionName.DISCONNECTED_DETECT), plcChiTiet);
                    }
                }
                else
                {
                    plcChiTiet.IsConnected = false;
                    WriteLogs(nameof(LogsActionName.DISCONNECTED_DETECT), plcChiTiet);
                    throw new Exception("PLC is not connected");
                }
            }
        //else
        //{
        //    var err = "Client is not Init";
        //    WriteLogs(nameof(LogsActionName.NOT_INIT), plcChiTiet, true, err);
        //    throw new Exception("Client is not Init");
        //}
        //else
        //{
        //    var err = "PLC is not found";
        //    WriteLogs(nameof(LogsActionName.PLC_NOT_FOUND), plcChiTiet, true, err);
        //    throw new Exception(err);
        //}
    }

    public void SetParameter(PLCChiTiet plcChiTiet)
    {
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcChiTiet.PLCId);
        if (plc != null)
        {
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    plc.Client.WriteWordDevice(plcChiTiet.HzQuay, plcChiTiet.TanSoQuay);
                    plc.Client.WriteWordDevice(plcChiTiet.HzRa, plcChiTiet.TanSoRa);
                    plc.Client.WriteWordDevice(plcChiTiet.TimeQuay, plcChiTiet.ThoiGianQuay);
                    WriteLogs(nameof(LogsActionName.PARAMETER_CHANGED), plcChiTiet);
                }
                else
                {
                    var err = "PLC is not connected";
                    WriteLogs(nameof(LogsActionName.PARAMETER_CHANGED), plcChiTiet, true, err);
                    throw new Exception(err);
                }
            }
            else
            {
                var err = "Client is not Init";
                WriteLogs(nameof(LogsActionName.PARAMETER_CHANGED), plcChiTiet, true, err);
                throw new Exception("Client is not Init");
            }
        }
        else
        {
            var err = "PLC is not found";
            WriteLogs(nameof(LogsActionName.PARAMETER_CHANGED), plcChiTiet, true, err);
            throw new Exception(err);
        }
    }

    public async Task SetRun(PLCChiTiet plcChiTiet)
    {
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcChiTiet.PLCId);
//#if DEBUG
//        //WriteLogs(nameof(LogsActionName.RUN), plcChiTiet);
//#endif
        if (plc != null)
        {
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    if (plcChiTiet.TanSoQuay > 0)
                    {
                        plc.Client.WriteBitDevice(plcChiTiet.PAUSE, false);
                        plc.Client.WriteBitDevice(plcChiTiet.RUNOUT, false);
                        plc.Client.WriteBitDevice(plcChiTiet.RUN, true);
                        await Task.Delay(timeDelay);
                        plc.Client.WriteBitDevice(plcChiTiet.RUN, false);
                        WriteLogs(nameof(LogsActionName.RUN), plcChiTiet);

                        //}
                    }
                    else
                    {
                        var err = "Tần số quay phải lớn hơn 0";
                        WriteLogs(nameof(LogsActionName.RUN), plcChiTiet, true, err);
                        throw new Exception("Tần số quay phải lớn hơn 0");
                    }
                }
                else
                {
                    var err = "PLC is not connected";
                    WriteLogs(nameof(LogsActionName.RUN), plcChiTiet, true, err);
                    throw new Exception("PLC is not connected");
                }
            }

            else
            {
                var err = "Client is not Init";
                WriteLogs(nameof(LogsActionName.RUN), plcChiTiet, true, err);
                throw new Exception("Client is not Init");
            }
        }
        else
        {
            var err = "PLC is not found";
            WriteLogs(nameof(LogsActionName.RUN), plcChiTiet, true, err);
            throw new Exception(err);
        }
    }

    public async Task SetStop(PLCChiTiet plcChiTiet)
    {
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcChiTiet.PLCId);
        if (plc != null)
        {
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    plc.Client.WriteBitDevice(plcChiTiet.PAUSE, false);
                    plc.Client.WriteBitDevice(plcChiTiet.RUNOUT, false);
                    plc.Client.WriteBitDevice(plcChiTiet.STOP, true);
                    await Task.Delay(timeDelay);
                    plc.Client.WriteBitDevice(plcChiTiet.STOP, false);
                    WriteLogs(nameof(LogsActionName.STOP), plcChiTiet);
                }
                else
                {
                    var err = "PLC is not connected";
                    WriteLogs(nameof(LogsActionName.STOP), plcChiTiet, true, err);
                    throw new Exception("PLC is not connected");
                }
            }
            else
            {
                var err = "Client is not Init";
                WriteLogs(nameof(LogsActionName.STOP), plcChiTiet, true, err);
                throw new Exception("Client is not Init");
            }
        }
        else
        {
            var err = "PLC is not found";
            WriteLogs(nameof(LogsActionName.STOP), plcChiTiet, true, err);
            throw new Exception(err);
        }
    }


    public void SetPause(PLCChiTiet plcChiTiet)
    {
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcChiTiet.PLCId);
        if (plc != null)
        {
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    plc.Client.WriteBitDevice(plcChiTiet.PAUSE, true);
                    WriteLogs(nameof(LogsActionName.PAUSE), plcChiTiet);
                }
                else
                {
                    var err = "PLC is not connected";
                    WriteLogs(nameof(LogsActionName.PAUSE), plcChiTiet, true, err);
                    throw new Exception("PLC is not connected");
                }
            }
            else
            {
                var err = "Client is not Init";
                WriteLogs(nameof(LogsActionName.PAUSE), plcChiTiet, true, err);
                throw new Exception("Client is not Init");
            }
        }
        else
        {
            var err = "PLC is not found";
            WriteLogs(nameof(LogsActionName.PAUSE), plcChiTiet, true, err);
            throw new Exception(err);
        }
    }

    public void SetRUNOUT(PLCChiTiet plcChiTiet)
    {
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcChiTiet.PLCId);
        if (plc != null)
        {
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    plc.Client.WriteBitDevice(plcChiTiet.RUNOUT, true);
                    WriteLogs(nameof(LogsActionName.RUNOUT), plcChiTiet);
                }
                else
                {
                    var err = "PLC is not connected";
                    WriteLogs(nameof(LogsActionName.RUNOUT), plcChiTiet, true, err);
                    throw new Exception("PLC is not connected");
                }
            }
            else
            {
                var err = "Client is not Init";
                WriteLogs(nameof(LogsActionName.RUNOUT), plcChiTiet, true, err);
                throw new Exception("Client is not Init");
            }
        }
        else
        {
            var err = "PLC is not found";
            WriteLogs(nameof(LogsActionName.RUNOUT), plcChiTiet, true, err);
            throw new Exception(err);
        }
    }

    public void SetResume(PLCChiTiet plcChiTiet)
    {
        var plc = mainService.VmPLC.Items.FirstOrDefault(x => x.Id == plcChiTiet.PLCId);
        if (plc != null)
        {
            if (plc.Client != null)
            {
                if (plc.Client.IsConnected())
                {
                    //plc.Client.WriteBitDevice(plcChiTiet.RUNOUT,false);
                    plc.Client.WriteBitDevice(plcChiTiet.PAUSE, false);
                    WriteLogs(nameof(LogsActionName.RESUME), plcChiTiet);
                }
                else
                {
                    var err = "PLC is not connected";
                    WriteLogs(nameof(LogsActionName.RESUME), plcChiTiet, true, err);
                    throw new Exception("PLC is not connected");
                }
            }
            else
            {
                var err = "Client is not Init";
                WriteLogs(nameof(LogsActionName.RESUME), plcChiTiet, true, err);
                throw new Exception("Client is not Init");
            }
        }
        else
        {
            var err = "PLC is not found";
            WriteLogs(nameof(LogsActionName.RESUME), plcChiTiet, true, err);
            throw new Exception(err);
        }
    }

    public int WriteLogs(string actionName, PLCChiTiet plcChiTiet, bool isError = false, string? errorStr = null)
    {
        try
        {
            var item = mainService.VmCoiLogs.CreateDefaultNew();
            item.TimeQuay = plcChiTiet.TanSoQuay;
            item.HzRa = plcChiTiet.TanSoRa;
            item.HzQuay = plcChiTiet.TanSoQuay;
            item.MaCoi = plcChiTiet.MaCoi;
            item.MaXuong = plcChiTiet.MaXuong;
            item.ActionName = actionName;
            item.IsError = isError;
            item.ErrorStr = errorStr;
            item.IdMonitor = plcChiTiet.IdMonitor;
            item.NhanVienId = plcChiTiet.NhanVienId;
            item.MaChatLuong = plcChiTiet.MaChatLuong;

            var row = mainService.VmCoiLogs.Insert(item);
            if (plcChiTiet.Logs.Count > mainService.VmApp.MaxRowsLogView)
                plcChiTiet.Logs.RemoveAt(plcChiTiet.Logs.Count - 1);
            plcChiTiet.Logs.Insert(0, item);
            return row;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}