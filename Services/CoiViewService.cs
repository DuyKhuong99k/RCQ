using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Services;

public partial class CoiViewService : ObservableObject, ICoiViewService
{
    private readonly ICoiService coiService;
    private readonly IMessageService messageService;
    [ObservableProperty] private ObservableRangeCollection<PLCChiTiet> items = new();
    [ObservableProperty] private List<PLCChiTiet> selectedItems;
    [ObservableProperty] private XiNghiep? xiNghiepSelectedItem;

    public CoiViewService(ICoiService coiService, IMessageService messageService)
    {
        this.coiService = coiService;
        this.messageService = messageService;
        //Load();
    }

    private MessageViewModel VmMessage => MessageViewModel.Instance;

    public void Load()
    {
        var items = coiService.Items;
        Items.AddRange(items);

        //foreach (var item in items) Items.AddRange(items);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public bool Command_PAUSE(PLCChiTiet item)
    {
        try
        {
            coiService.Command_PAUSE(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public bool Command_RESUME(PLCChiTiet item)
    {
        try
        {
            coiService.Command_RESUME(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public bool Command_RUNOUT(PLCChiTiet item)
    {
        try
        {
            coiService.Command_RUNOUT(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public async Task<bool> Command_RUN(PLCChiTiet item)
    {
        try
        {
            await coiService.Command_RUN(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public bool Command_SETPARAMETER(PLCChiTiet item)
    {
        try
        {
            coiService.Command_SETPARAMETER(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }
    public bool Command_SETPARAMATERANDDATA(PLCChiTiet item)
    {
        try
        {
            coiService.Command_SETPARAMATERANDDATA(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }
    public bool Command_SETDATA(PLCChiTiet item)
    {
        try
        {
            // Do data

            coiService.Command_SETDATA(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public bool Command_STARTTIMER()
    {
        try
        {
            coiService.Command_STARTTIMER();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public async Task<bool> Command_STOP(PLCChiTiet item)
    {
        try
        {
            await coiService.Command_STOP(item);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            messageService.Show(e.ToString());
            return false;
        }
    }

    public event EventHandler? DevicesChanged;

    public void Dispose()
    {
    }
}