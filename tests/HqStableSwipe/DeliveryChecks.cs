using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using Models.Repos.Models;
using Newtonsoft.Json.Linq;
using Services;
using Vars;

internal static class DeliveryChecks
{
    public static async Task Run(Action<bool,string> check)
    {
        var never = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var timeout = await HqFirmwareSend.RunAsync(_ => never.Task, TimeSpan.FromMilliseconds(80));
        check(!timeout.Sent && timeout.Reason == "TIMEOUT" && timeout.Milliseconds < 1500,
            "Outbound send ignoring cancellation is still bounded, no infinite wait");
        never.SetException(new IOException("late test fault"));
        var fast = await HqFirmwareSend.RunAsync(_ => Task.CompletedTask, HqFirmwareSend.ReplyBudget);
        check(fast.Sent && fast.Milliseconds < 500,"Healthy sends have no added artificial delay");
        var called = false;
        var cancelled = await HqFirmwareSend.RunAsync(_ => { called = true; return Task.CompletedTask; },
            HqFirmwareSend.ReplyBudget,new CancellationToken(true));
        check(!called && cancelled.Reason == "DISCONNECTED","Do not enqueue send after connection was aborted");
        var failed = await HqFirmwareSend.RunAsync(_ => throw new IOException("network"),HqFirmwareSend.ReplyBudget);
        check(!failed.Sent && failed.Reason == nameof(IOException),"Synchronous send failure is a delivery result, not SQL failure");
        string Envelope(string field,JObject detail) => new JObject { ["IsError"]=field=="DataError",
            ["Data"]=null,[field]=detail.ToString() }.ToString();
        check(HqFirmwareSend.SaveStatus(Envelope("DataError",new JObject {["SaveStatus"]="SAVED"}))=="SAVED",
            "Audit preserves saved SQL status even for totals error Data=null/DataError");
        check(HqFirmwareSend.SaveStatus(Envelope("Data",new JObject {["SaveStatus"]="NOT_SAVED"}))=="NOT_SAVED" &&
              HqFirmwareSend.SaveStatus("malformed")=="UNKNOWN","Audit never guesses an unknown SQL outcome");
        check(HqFirmwareSend.SaveStatus(Envelope("Data",new JObject {["IsValid"]=true}))=="SAVED",
            "Successful attendance reply has saved audit status without payload changes");

        var machine = new MayCan {Id="DELIVERY_ONLY",ConnectionId="ORIGINAL",WKv=AppKV.Hq,MType="BOARD"};
        using(var scope = HqFirmwareSend.Begin("ORIGINAL",CancellationToken.None))
        {
            machine.ConnectionId="RECONNECTED";
            var accepted=await HqFirmwareSend.IdentityAsync(machine,(connection,_) =>
            { check(connection=="ORIGINAL","Identity remains bound to original Hub connection after reconnect"); return Task.FromResult(false); });
            check(!accepted && scope.SendFailure?.Target=="SETNHANVIEN","Identity failure is carried to final response boundary");
            scope.Fail("SECOND","ignored");
            check(scope.SendFailure?.Target=="SETNHANVIEN" && scope.TryBeginReply() && !scope.TryBeginReply(),
                "First send failure wins; final reply can only be attempted once");
        }
        check(HqFirmwareSend.Current==null,"Send scope is restored after request");

        var selectedConnections=new List<string>(); var sentFrames=new List<(string Target,object?[] Args,CancellationToken Token)>();
        var proxy=Mock<ISingleClientProxy>((_,a)=>
        { sentFrames.Add(((string)a![0]!, (object?[])a[1]!, (CancellationToken)a[2]!)); return Task.CompletedTask; });
        var clients=Mock<IHubClients>((m,a)=>
        { if(m.Name!="Client") throw new Exception(m.Name); selectedConnections.Add((string)a![0]!); return proxy; });
        var context=Mock<IHubContext<ChatHub>>((_,_)=>clients);
        var service=NewService(machine,context);
        check(!await service.SendHqIdentityAsync(machine.Id,"ORIGINAL","0030","0030","Name",CancellationToken.None) && sentFrames.Count==0,
            "Real identity sender never redirects old response to a new firmware connection");
        check(await service.CommandSetNhanVien(machine.Id,"0030","0030","Name"),"Real HQ identity send succeeds normally");
        check(sentFrames.Count==1 && sentFrames[0].Target=="SETNHANVIEN" &&
            sentFrames[0].Args.SequenceEqual(new object?[]{"0030","0030","Name"}) &&
            sentFrames[0].Token.CanBeCanceled && selectedConnections.Single()=="RECONNECTED",
            "Firmware identity target/three arguments unchanged; cancellation token added");

        using var release=new ManualResetEventSlim(false);
        var entered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var applied=new List<string>(); var notifyCount=0; var ambientLeaked=false;
        try
        {
            using(var scope=HqFirmwareSend.Begin("UI_SCOPE",CancellationToken.None))
                HqOnlineUi.Add(machine,Row("BLOCK"),_=>
                { ambientLeaked=HqFirmwareSend.Current!=null; entered.TrySetResult(); release.Wait(); });
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
            var watch=Stopwatch.StartNew();
            for(var i=0;i<400;i++)
            {
                HqOnlineUi.Add(machine,Row(i.ToString()),row=>applied.Add(row.Id));
                HqOnlineUi.Notify(machine,()=>Interlocked.Increment(ref notifyCount));
            }
            var state=HqOnlineUi.Inspect(machine);
            check(watch.ElapsedMilliseconds<1000 && state.PendingRows==100 && state.DroppedDisplayRows==300,
                "Blocked browser callback cannot block producer or grow display queue beyond 100 rows");
            var other=new MayCan {Id="INDEPENDENT_UI"};
            var otherDone=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            HqOnlineUi.Notify(other,()=>otherDone.SetResult());
            await otherDone.Task.WaitAsync(TimeSpan.FromSeconds(2));
            check(true,"Slow display for one machine does not block another machine");
        }
        finally { release.Set(); }
        await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));
        check(!ambientLeaked && HqFirmwareSend.Current==null,"UI worker does not retain Hub AsyncLocal send scope");
        check(applied.SequenceEqual(Enumerable.Range(300,100).Select(x=>x.ToString())) && notifyCount==1,
            "Mailbox keeps newest 100 display rows in order and coalesces repeated refresh requests");
        HqOnlineUi.Notify(machine,()=>throw new IOException("test UI error"));
        HqOnlineUi.Add(machine,Row("AFTER_ERROR"),row=>applied.Add(row.Id));
        await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));
        check(applied.Last()=="AFTER_ERROR","A failing display callback never stops subsequent updates");

        var original=machine.Items; var oldEvents=0;
        original.CollectionChanged+=(_,_)=>Interlocked.Increment(ref oldEvents);
        for(var i=0;i<130;i++) service.AddItem(machine,Row(i.ToString()));
        service.AddItem(machine,Row("129"));
        check(original.Count==0 && oldEvents==0 && !ReferenceEquals(original,machine.Items),
            "HQ AddItem publishes a new collection snapshot without mutating the grid's old collection");
        check(machine.Items.Count==100 && machine.Items.OfType<HQ_PhieuCan>().Count(x=>x.Id=="129")==1,
            "Published display list is capped at 100 and replay Id is not duplicated");
        // Let all older timer work settle before testing a deliberately blocked subscriber.
        await Task.Delay(120);
        var callbacks=0; var active=0; var overlapping=false;
        using var releaseNotify=new ManualResetEventSlim(false);
        var notifyEntered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var latestSeen=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        service.ItemUpdated+=row=>
        {
            if(Interlocked.Increment(ref active)>1) overlapping=true;
            try
            {
                if(Interlocked.Increment(ref callbacks)==1) { notifyEntered.TrySetResult(); releaseNotify.Wait(); }
                if(row.ThongBao=="LATEST") latestSeen.TrySetResult();
            }
            finally { Interlocked.Decrement(ref active); }
        };
        try
        {
            service.NotifyItemChanged(machine);
            await notifyEntered.Task.WaitAsync(TimeSpan.FromSeconds(3));
            for(var i=0;i<100;i++) service.NotifyItemChanged(machine);
            await Task.Delay(80);
            check(callbacks==1 && !overlapping,"Slow ItemUpdated handler does not create overlapping timer callbacks");
            machine.ThongBao="LATEST";
            service.NotifyItemChanged(machine);
        }
        finally { releaseNotify.Set(); }
        await latestSeen.Task.WaitAsync(TimeSpan.FromSeconds(3));
        await Task.Delay(120);
        check(callbacks>=2 && !overlapping,"Notification arriving during flush is not erased by a blanket Clear");
        var before=callbacks;
        for(var i=0;i<40;i++) {service.NotifyItemChanged(machine); await Task.Delay(5);}
        check(callbacks>before,"Continuous traffic refreshes the web instead of postponing debounce forever");
        await Task.Delay(120);
    }
    private static HQ_PhieuCan Row(string id)=>new() {Id=id,MayCan="TEST_ONLY",MaLo="LO",MaSize="SIZE",
        MaThanhPham="PRODUCT",MaLoaiNguyenLieu="MATERIAL",TheId="CARD",TheIdNhanVien="CARD"};
    private static MayCansService NewService(MayCan machine,IHubContext<ChatHub> context)
    {
        var service=(MayCansService)RuntimeHelpers.GetUninitializedObject(typeof(MayCansService));
        void Field(string name,object value)=>typeof(MayCansService).GetField(name,BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(service,value);
        Field("items",new ConcurrentDictionary<string,MayCan>(new[]{new KeyValuePair<string,MayCan>(machine.Id,machine)}));
        Field("hubContext",context); Field("_pendingUpdates",new ConcurrentDictionary<string,MayCan>());
        Field("_notifyTimerLock",new object()); Field("_notifyIntervalMilliseconds",20);
        return service;
    }
    private static T Mock<T>(Func<MethodInfo,object?[]?,object?> handler) where T:class
    {var result=DispatchProxy.Create<T,ServiceProxy>();((ServiceProxy)(object)result).Handler=handler;return result;}
}
