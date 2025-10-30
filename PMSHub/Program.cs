using AppViewModels;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Newtonsoft.Json.Serialization;
using PMSHub.Components;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using ViewModels.Repos.Hubs;
using ViewModels.Repos.Hubs.IServices;
using Services;
using Dapper;
using PMSHub.Components.Pages;
using ToolsEx;

var builder = WebApplication.CreateBuilder(args);
// Đăng ký HttpClient factory
builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
{
    //if (app.Environment.IsDevelopment()) //Only add details when debugging.
    //{
    o.DetailedErrors = true;
    //}
});
SqlMapper.AddTypeHandler(new Models.Repos.Handler.DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new Models.Repos.Handler.TimeOnlyTypeHandler());

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
#if DEBUG
    options.IdleTimeout = TimeSpan.FromSeconds(3);
#else
    options.IdleTimeout = TimeSpan.FromMinutes(30);
#endif
    
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new DefaultNamingStrategy
        {
            ProcessDictionaryKeys = false,
            OverrideSpecifiedNames = false
        }
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {

            builder.WithOrigins("https://*")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Cho phép truy cập có credentials
        });
});
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);         // Server gửi ping mỗi 15s
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});
builder.Services.AddSingleton<MainViewModel>();
builder.Services.AddSingleton<IMainService, MainViewModel>();
builder.Services.AddSingleton<ICoiService, CoiService>();
builder.Services.AddSingleton<IMayCansService, MayCansService>();
builder.Services.AddSingleton(typeof(HighThroughputQueue<>));

builder.Services.AddSingleton<ICommunicationService, CommunicationService>();
builder.Services.AddSingleton<ICommitService, CommitService>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddScoped<IDeviceViewService, DeviceViewService>();
builder.Services.AddScoped<ICoiViewService, CoiViewService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ISessionService, SesssionService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IToastService,ToastService>();

builder.Services.AddTransient<IConverterService, ConverterService>();
builder.Services.AddTransient<IPLCService, PLCService>();


builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureHttpsDefaults(o => { o.ClientCertificateMode = ClientCertificateMode.NoCertificate; });
});
//builder.Services.AddDbContext<dbPMScontext>(options =>
//{
//    options.UseSqlServer(Base.Ins.ConnectionString,
//        sqlOptions =>
//        {
//            sqlOptions.EnableRetryOnFailure(
//                5,
//                TimeSpan.FromSeconds(30),
//                null);
//        }
//    );
//});
var app = builder.Build();
// #if DEBUG
// if (Environment.MachineName== "NP")
// {
//25.xx
SyncfusionLicenseProvider.RegisterLicense(
    "Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXtcdHRWQmFfVUx0XUM=");
////23.xx
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxfdnRVRmlcUkV/WEQ=");
// }
// #else
// //23.xx
// Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXtcdHRWQmFfVUx0XUM=");
// #endif

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseWebSockets();
app.UseRouting();
//app.UseResponseCompression();

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSession(); 
app.UseAuthorization();

// Th�m middleware session v�o pipeline
app.MapControllers();
app.MapHub<ChatHub>("chathub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();