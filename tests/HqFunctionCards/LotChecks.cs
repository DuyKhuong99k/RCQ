using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using PMSHub.Components.Pages;
using Vars;
using ViewModels.Repos.HQ;
using ViewModels.Repos.Hubs.IServices;

internal static class LotChecks
{
    public static async Task Run(string connectionString, SqlConnection sql, Action<bool,string> check)
    {
        await sql.ExecuteAsync(@"
            CREATE TABLE dbo.HQ_Lo (Id varchar(50) NOT NULL PRIMARY KEY,NgayNguyenLieu date NOT NULL,
                SuDung bit NOT NULL,MNgay datetime2 NOT NULL);
            INSERT dbo.HQ_Lo VALUES
                ('001','20250219',1,'2025-02-19T14:21:32.9481578'),
                ('OLD_OFF','20250219',0,'2025-02-19T14:21:32.9481578'),
                ('NEW','20261026',1,'2026-10-26T10:00:00'),
                ('0007','20261026',1,'2026-10-26T10:00:00'),
                ('NEW_OFF','20261026',0,'2026-10-26T10:00:00');");
        const BindingFlags flags=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
        var appSingleton=typeof(AppViewModels.AppViewModel).GetField("_instance",BindingFlags.NonPublic|BindingFlags.Static)!;
        var oldApp=appSingleton.GetValue(null);
        var oldEf=AppViewModels.Base.Ins.ConnectionString2;
        try
        {
            // All settings changes stay inside this isolated test process. No config file is read/written.
            AppViewModels.Base.Ins.ConnectionString2=connectionString;
            var app=(AppViewModels.AppViewModel)RuntimeHelpers.GetUninitializedObject(typeof(AppViewModels.AppViewModel));
            app.ComName="RCQTG";app.LoKv=AppKV.Hq;appSingleton.SetValue(null,app);
            var hq=(HQ_LoViewModel)RuntimeHelpers.GetUninitializedObject(typeof(HQ_LoViewModel));
            var legacy=(LoViewModel)RuntimeHelpers.GetUninitializedObject(typeof(LoViewModel));
            hq.ReloadHq();
            var expected=new[]{"0007","NEW","001"};
            check(hq.Items.SequenceEqual(expected),"Active lot 001 from 2025 is selectable; inactive old/new lots excluded; stable ordering");
            check(hq.Gets("1").SequenceEqual(expected),"HQ string selector uses active status instead of 30/365 day window");
            check(legacy.Gets("1").SequenceEqual(expected),"Legacy selector's HQ branch agrees with dedicated HQ selector");
            var delta=hq.Gets("20261001",1,100);
            check(delta.Count==3 && delta.All(x=>x.Id!="001") && delta.Any(x=>x.Id=="NEW_OFF" && !x.SuDung),
                "Firmware incremental API still filters by original MNgay and includes disable updates");
            var originalDate=await sql.ExecuteScalarAsync<DateTime>("SELECT MNgay FROM dbo.HQ_Lo WHERE Id='001'");
            check(originalDate==new DateTime(2025,2,19,14,21,32).AddTicks(9481578),"Selection/reload does not restamp historical MNgay");

            // Actual Devices page methods: an HQ scale selects HQ catalogue even with non-HQ global LoKv.
            app.LoKv=AppKV.NguyenLieu;
            legacy.SelectedItem="WRONG_LEGACY_LOT";
            var main=Mock<IMainService>((m,_)=>m.Name switch
            {"get_VmApp"=>app,"get_VmLoHq"=>hq,"get_VmLo"=>legacy,_=>throw new Exception(m.Name)});
            var commands=new List<(string Machine,string Lot)>();
            var received=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var machines=Mock<IMayCansService>((m,a)=>
            {
                if(m.Name!="CommandSetLo")throw new Exception(m.Name);
                commands.Add(((string)a![0]!, (string)a[1]!));received.TrySetResult();return Task.FromResult(true);
            });
            var toasts=new List<string>();
            var toast=Mock<IToastService>((m,a)=>{toasts.Add(a![0]?.ToString()??"");return null;});
            var page=(Devices)RuntimeHelpers.GetUninitializedObject(typeof(Devices));
            typeof(Devices).GetField("SelectedItems",flags)!.SetValue(page,new List<MayCan>());
            foreach(var (property,value) in new (string,object)[]{("MainService",main),("MayCanService",machines),("ToastService",toast)})
                typeof(Devices).GetProperty(property,flags)!.SetValue(page,value);
            var open=typeof(Devices).GetMethod("OpenDialogLo",flags)!;
            var source=typeof(Devices).GetProperty("UseHqLotSource",flags)!;
            var visible=typeof(Devices).GetProperty("IsVisibleLo",flags)!;
            var machine=new MayCan{Id="7779520",WKv=AppKV.Hq,MaLo="001"};
            open.Invoke(page,new object[]{machine});
            check((bool)source.GetValue(page)! && (bool)visible.GetValue(page)! && hq.Items.Contains("001") && hq.SelectedItem=="001",
                "HQ popup uses refreshed HQ catalogue/current lot independently of company HL or global LoKv");
            typeof(Devices).GetMethod("CloseDialogLo",flags)!.Invoke(page,null);
            await received.Task.WaitAsync(TimeSpan.FromSeconds(3));
            check(commands.Count==1 && commands[0]==("7779520","001"),"Confirm sends the displayed HQ selection, not stale legacy SelectedItem");
            await sql.ExecuteAsync("UPDATE dbo.HQ_Lo SET SuDung=0 WHERE Id='0007'");
            open.Invoke(page,new object[]{machine});
            check(!hq.Items.Contains("0007") && hq.Items.Contains("001"),"Reopening popup refreshes active flags instead of retaining stale cache");
            var cached=hq.Items;
            AppViewModels.Base.Ins.ConnectionString2="INVALID_TEST_CONNECTION_STRING";
            open.Invoke(page,new object[]{machine});
            check(!(bool)visible.GetValue(page)! && ReferenceEquals(cached,hq.Items) && toasts.Count==1,
                "Failed SQL refresh preserves prior cache, shows error and does not terminate page event");
            AppViewModels.Base.Ins.ConnectionString2=connectionString;
            open.Invoke(page,new object[]{machine});
            check((bool)visible.GetValue(page)! && hq.Items.Contains("001"),"Popup works again after SQL settings recover");
            var nonHq=new MayCan{Id="OTHER",WKv=AppKV.TPFillet};
            app.ComName="PMS";open.Invoke(page,new object[]{nonHq});
            check(!(bool)source.GetValue(page)!,"Non-HQ/non-HL scales retain legacy catalogue routing");
            app.ComName="HL";
            check((bool)source.GetValue(page)!,"Existing HL-specific catalogue routing remains supported");
        }
        finally
        {
            appSingleton.SetValue(null,oldApp);
            AppViewModels.Base.Ins.ConnectionString2=oldEf;
        }
    }
    private static T Mock<T>(Func<MethodInfo,object?[]?,object?> handler) where T:class
    {var proxy=DispatchProxy.Create<T,TestProxy>();((TestProxy)(object)proxy).Handler=handler;return proxy;}
}
