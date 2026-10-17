using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Models.Repos.Models;
using Security.Crypt;
using Services;
using ViewModels.Repos.Hubs.IServices;

internal static class LoginAccountChecks
{
    public static void Run(Action<bool,string> check)
    {
        var method=typeof(LoginService).GetMethod("LoginLocalUser",BindingFlags.Instance|BindingFlags.NonPublic)!;
        var user=new NguoiDung {Id=7,UserName="local-test-user",IsActive=true,
            Password=ED.EncryptString("test-only-password","PMS")};
        foreach(var active in new bool?[]{false,null,true}) {
            user.IsActive=active;
            var session=new TestSession();
            var context=new DefaultHttpContext();
            context.Features.Set<ISessionFeature>(new TestSessionFeature {Session=session});
            var areas=DispatchProxy.Create<IUserService,LoginUserProxy>();
            var service=new LoginService(new HttpContextAccessor {HttpContext=context},null!,areas);
            var success=(bool)method.Invoke(service,new object?[]{user,"test-only-password"})!;
            check(success==(active==true),$"Đăng nhập kiểm tra IsActive={active?.ToString() ?? "NULL"}");
            check(active==true ? session.GetString("UserId")=="7" && session.GetString("Username")==user.UserName
                : !session.Keys.Any(),"Chỉ tài khoản hoạt động và đúng mật khẩu mới tạo phiên");
            check(((LoginUserProxy)(object)areas).AreaLoads==(active==true?1:0),"Không nạp quyền cho tài khoản khóa/NULL");
        }
        user.IsActive=true;
        foreach(var password in new[]{"wrong-password",""}) {
            var session=new TestSession();
            var context=new DefaultHttpContext();
            context.Features.Set<ISessionFeature>(new TestSessionFeature {Session=session});
            var areas=DispatchProxy.Create<IUserService,LoginUserProxy>();
            var service=new LoginService(new HttpContextAccessor {HttpContext=context},null!,areas);
            check(!(bool)method.Invoke(service,new object?[]{user,password})! && !session.Keys.Any()
                && ((LoginUserProxy)(object)areas).AreaLoads==0,"Mật khẩu sai/rỗng không tạo phiên hoặc nạp quyền");
            check(!(bool)method.Invoke(service,new object?[]{null,password})!,"Không có tài khoản không được đăng nhập");
        }
        var noSessionService=new LoginService(new HttpContextAccessor {HttpContext=null},null!,null!);
        check(!(bool)method.Invoke(noSessionService,new object?[]{user,"test-only-password"})!,"Thiếu HTTP context không tạo đăng nhập thành công giả");
        user.Password="not-valid-encrypted-data";
        check(!(bool)method.Invoke(noSessionService,new object?[]{user,""})!,"Mật khẩu lưu bị hỏng không đăng nhập bằng chuỗi rỗng");
    }

    private sealed class TestSessionFeature : ISessionFeature { public ISession Session {get;set;}=default!; }
    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string,byte[]> data=new();
        public bool IsAvailable=>true;
        public string Id=>"isolated-login-test";
        public IEnumerable<string> Keys=>data.Keys;
        public void Clear()=>data.Clear();
        public void Remove(string key)=>data.Remove(key);
        public void Set(string key,byte[] value)=>data[key]=value;
        public bool TryGetValue(string key,out byte[] value)=>data.TryGetValue(key,out value!);
        public Task LoadAsync(CancellationToken cancellationToken=default)=>Task.CompletedTask;
        public Task CommitAsync(CancellationToken cancellationToken=default)=>Task.CompletedTask;
    }
}

public class LoginUserProxy : DispatchProxy
{
    public int AreaLoads {get;private set;}
    protected override object? Invoke(MethodInfo? method,object?[]? args)
    {
        if(method?.Name=="AddUserArea") {AreaLoads++;return null;}
        throw new InvalidOperationException("Unexpected user service call: "+method?.Name);
    }
}
