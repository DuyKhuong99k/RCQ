using AppViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Models.Repos;
using Newtonsoft.Json.Serialization;
using PMS.Middlewares;
using PMS.Attrs;
using Syncfusion.Licensing;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using PMS.Controllers.NhaAn.DanhMucNhaAn;
using Microsoft.AspNetCore.SignalR;
using PMS.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
// Add services to the Session
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMvc().AddSessionStateTempDataProvider();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient(AppViewModels.AppViewModel.Instance.HttpClientName, client =>
{
    // code to configure headers etc..
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    // Phải chỉnh lại  khi có thể để sử dụng SSL. Lỗi Bảo mât
    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
    return handler;
});
// Add cookie middleware
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None;
    options.Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Có thể thay đổi tùy theo yêu cầu an toàn
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "JWTToken"; // Tên của cookie chứa token
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Thời hạn của cookie
        options.SlidingExpiration = true; // Cho phép thời hạn của cookie được gia hạn mỗi khi có yêu cầu mới
        options.LoginPath = "/Authentication/Login";
        options.ReturnUrlParameter = "ReturnUrl";
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<dbPMScontext>(options =>
{
    options.UseSqlServer(Base.Ins.ConnectionString);
});
//builder.Services.AddCors();
//builder.Services.AddCors(option =>
//{
//    option.AddPolicy("RefreshToken", builder =>
//    {
//        builder.WithOrigins(AppViewModels.AppViewModel.Instance.ApiHostUrl) // Cấp nguồn gốc cho phép
//        .AllowAnyMethod().AllowAnyHeader();
//    });
//});
//
//builder.Services.AddHostedService<AutoApproveMenuService>(); // background service tự động phê duyệt menu
builder.Services.AddHostedService<AutoAlertExpityNoticeServices>(); 
builder.Services.AddScoped<HQ_ThucDonController>(); // cần thêm dòng này để DI controller background service
builder.Services.AddSignalR();
var app = builder.Build();
// Lấy IHttpClientFactory từ services
var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();

// Gọi phương thức Initialize của AuthenticationHelpers
AuthenticationHelpers.Initialize(httpClientFactory);


//SyncfusionLicenseProvider.RegisterLicense(
//    "Mgo+DSMBMAY9C3t2VFhiQlJPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSH5RdEdiWndeeXxdRmk=");
SyncfusionLicenseProvider.RegisterLicense(
    "Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXlfeXRTRWBcUUB1WEY=");
//SyncfusionLicenseProvider.RegisterLicense(
//    "Mgo+DSMBMAY9C3t2VFhiQlJPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSH5RdEdiWndeeXxdRmk=");
//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Đặt TokenCheckMiddleware ở đây
//app.UseMiddleware<PMS.Middlewares.TokenCheckMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PMS.Hubs.ProgressHub>("/progressHub");
app.UseDeveloperExceptionPage();
app.UseSession();
//cho phép trang web truy cập các nguồn khác như API,...
//app.UseCors(builder => builder
//    .WithOrigins($@"{AppViewModels.AppViewModel.Instance.HostUrl}")
//    .AllowAnyMethod()
//    .AllowAnyHeader()
//);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapHub<PMS.Hubs.ProgressHub>("/progressHub"); // Định tuyến SignalR hub
app.UseEndpoints(endpoints =>
{
    // Thêm CORS cho API endpoint
    //endpoints.MapControllers().RequireCors("RefreshToken");
    endpoints.MapControllerRoute(
        "XuLyPhieuCanTom",
        "XuLyPhieuCan/XuLyPhieuCanTom/{action}/{id?}",
        new { controller = "XuLyPhieuCanTom" }
    );

    endpoints.MapControllerRoute(
       "XuLyPhieuCanDinhHinh",
       "XuLyPhieuCan/XuLyPhieuCanDinhHinh/{action}/{id?}",
       new { controller = "XuLyPhieuCanDinhHinh" }
   );


    endpoints.MapControllerRoute(
        "DanhMucT_NhomLo",
        "DanhMuc/Tom/T_NhomLo/{action}/{id?}",
        new { controller = "T_NhomLo" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_LoaiNguyenLieu",
        "DanhMuc/Tom/T_LoaiNguyenLieu/{action}/{id?}",
        new { controller = "T_LoaiNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_Size",
        "DanhMuc/Tom/T_Size/{action}/{id?}",
        new { controller = "T_Size" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_SanPham",
        "DanhMuc/Tom/T_SanPham/{action}/{id?}",
        new { controller = "T_SanPham" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_CongDoan",
        "DanhMuc/Tom/T_CongDoan/{action}/{id?}",
        new { controller = "T_CongDoan" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_CongDoanTheoThanhPham",
        "DanhMuc/Tom/T_CongDoanTheoThanhPham/{action}/{id?}",
        new { controller = "T_CongDoanTheoThanhPham" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_Bon",
        "DanhMuc/Tom/T_Bon/{action}/{id?}",
        new { controller = "T_Bon" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_KhachHang",
        "DanhMuc/Tom/T_KhachHang/{action}/{id?}",
        new { controller = "T_KhachHang" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_DonHang",
        "DanhMuc/Tom/T_DonHang/{action}/{id?}",
        new { controller = "T_DonHang" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_QuyTrinh",
        "DanhMuc/Tom/T_QuyTrinh/{action}/{id?}",
        new { controller = "T_QuyTrinh" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_QuyCach",
        "DanhMuc/Tom/T_QuyCach/{action}/{id?}",
        new { controller = "T_QuyCach" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_PhieuPhanCo",
        "DanhMuc/Tom/T_PhieuPhanCo/{action}/{id?}",
        new { controller = "T_PhieuPhanCo" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_PhuGia",
        "DanhMuc/Tom/T_PhuGia/{action}/{id?}",
        new { controller = "T_PhuGia" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_KhangSinh",
        "DanhMuc/Tom/T_KhangSinh/{action}/{id?}",
        new { controller = "T_KhangSinh" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_ThongTinPhu",
        "DanhMuc/Tom/T_ThongTinPhu/{action}/{id?}",
        new { controller = "T_ThongTinPhu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_TrangThaiNguyenLieu",
        "DanhMuc/Tom/T_TrangThaiNguyenLieu/{action}/{id?}",
        new { controller = "T_TrangThaiNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_HoaChat",
        "DanhMuc/Tom/T_HoaChat/{action}/{id?}",
        new { controller = "T_HoaChat" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_NhomHoaChat",
        "DanhMuc/Tom/T_NhomHoaChat/{action}/{id?}",
        new { controller = "T_NhomHoaChat" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_TyLeHoaChatTheoNhom",
        "DanhMuc/Tom/T_TyLeHoaChatTheoNhom/{action}/{id?}",
        new { controller = "T_TyLeHoaChatTheoNhom" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_ChiTietVoXo",
        "DanhMuc/Tom/T_ChiTietVoXo/{action}/{id?}",
        new { controller = "T_ChiTietVoXo" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_DonGia",
        "DanhMuc/Tom/T_DonGia/{action}/{id?}",
        new { controller = "T_DonGia" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_BaoBi",
        "DanhMuc/Tom/T_BaoBi/{action}/{id?}",
        new { controller = "T_BaoBi" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_TieuChuan",
        "DanhMuc/Tom/T_TieuChuan/{action}/{id?}",
        new { controller = "T_TieuChuan" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_PhieuYeuCau",
        "DanhMuc/Tom/T_PhieuYeuCau/{action}/{id?}",
        new { controller = "T_PhieuYeuCau" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_BieuMauGiao_Import",
        "DanhMuc/Tom/T_BieuMauGiao_Import/{action}/{id?}",
        new { controller = "T_BieuMauGiao_Import" }
    );
    endpoints.MapControllerRoute(
        "DanhMucT_MuaNguyenLieu_Import",
        "DanhMuc/Tom/T_MuaNguyenLieu_Import/{action}/{id?}",
        new { controller = "T_MuaNguyenLieu_Import" }
    );












    endpoints.MapControllerRoute(
       "DanhMucDonGiaSPTL",
       "DanhMuc/SanPhamTinhLuong/DonGiaSPTL/{action}/{id?}",
       new { controller = "DonGiaSPTL" }
   );
    endpoints.MapControllerRoute(
        "DanhMucDinhMucXepHangSPTL",
        "DanhMuc/SanPhamTinhLuong/DinhMucXepHangSPTL/{action}/{id?}",
        new { controller = "DinhMucXepHangSPTL" }
    );
    endpoints.MapControllerRoute(
        "DanhMucMaXepHangSPTL",
        "DanhMuc/SanPhamTinhLuong/MaXepHangSPTL/{action}/{id?}",
        new { controller = "MaXepHangSPTL" }
    );
    endpoints.MapControllerRoute(
        "DanhMucLoaiDonGiaSPTL",
        "DanhMuc/SanPhamTinhLuong/LoaiDonGiaSPTL/{action}/{id?}",
        new { controller = "LoaiDonGiaSPTL" }
    );

    endpoints.MapControllerRoute(
        "DanhMucSanPhamSPTL",
        "DanhMuc/SanPhamTinhLuong/SanPhamSPTL/{action}/{id?}",
        new { controller = "SanPhamSPTL" }
    );















   endpoints.MapControllerRoute(
       "DanhMucBoTriViTriLoSizeThanhPhamFillet",
       "DanhMuc/Fillet/BoTriViTriLoSizeThanhPhamFillet/{action}/{id?}",
       new { controller = "BoTriViTriLoSizeThanhPhamFillet" }
   );
   endpoints.MapControllerRoute(
       "DanhMucBoTriLoTheoLineFillet",
       "DanhMuc/Fillet/BoTriLoTheoLineFillet/{action}/{id?}",
       new { controller = "BoTriLoTheoLineFillet" }
   );
   endpoints.MapControllerRoute(
       "DanhMucBoTriNVTheoChuyenViTriFillet",
       "DanhMuc/Fillet/BoTriNVTheoChuyenViTriFillet/{action}/{id?}",
       new { controller = "BoTriNVTheoChuyenViTriFillet" }
   );
    endpoints.MapControllerRoute(
       "DanhMucBoTriNVTheoSize",
       "DanhMuc/Fillet/BoTriNVTheoSize/{action}/{id?}",
       new { controller = "BoTriNVTheoSize" }
   );
    endpoints.MapControllerRoute(
       "DanhMucViTriFillet",
       "DanhMuc/Fillet/ViTriFillet/{action}/{id?}",
       new { controller = "ViTriFillet" }
   );
    endpoints.MapControllerRoute(
       "DanhMucLineFillet",
       "DanhMuc/Fillet/LineFillet/{action}/{id?}",
       new { controller = "LineFillet" }
   );
    endpoints.MapControllerRoute(
       "DanhMucNhanVienTheoBanFillet",
       "DanhMuc/Fillet/NhanVienTheoBanFillet/{action}/{id?}",
       new { controller = "NhanVienTheoBanFillet" }
   );
    endpoints.MapControllerRoute(
       "DanhMucHanMucTrongLuongFillet",
       "DanhMuc/Fillet/HanMucTrongLuongFillet/{action}/{id?}",
       new { controller = "HanMucTrongLuongFillet" }
   );
    endpoints.MapControllerRoute(
       "DanhMucThanhPhamMacDinhFillet",
       "DanhMuc/Fillet/ThanhPhamMacDinhFillet/{action}/{id?}",
       new { controller = "ThanhPhamMacDinhFillet" }
   );
    endpoints.MapControllerRoute(
       "DanhMucMapTPTinhLuongFillet",
       "DanhMuc/Fillet/MapTPTinhLuongFillet/{action}/{id?}",
       new { controller = "MapTPTinhLuongFillet" }
   );
    endpoints.MapControllerRoute(
        "DanhMucDinhMucFillet",
        "DanhMuc/Fillet/DinhMucFillet/{action}/{id?}",
        new { controller = "DinhMucFillet" }
    );
    endpoints.MapControllerRoute(
        "DanhMucMauFillet",
        "DanhMuc/Fillet/MauFillet/{action}/{id?}",
        new { controller = "MauFillet" }
    );
    endpoints.MapControllerRoute(
        "DanhMucSizeFillet",
        "DanhMuc/Fillet/SizeFillet/{action}/{id?}",
        new { controller = "SizeFillet" }
    );
    endpoints.MapControllerRoute(
        "DanhMucThanhPhamFillet",
        "DanhMuc/Fillet/ThanhPhamFillet/{action}/{id?}",
        new { controller = "ThanhPhamFillet" }
    );
    endpoints.MapControllerRoute(
        "DanhMucLoaiCaFillet",
        "DanhMuc/Fillet/LoaiCaFillet/{action}/{id?}",
        new { controller = "LoaiCaFillet" }
    );
    endpoints.MapControllerRoute(
       "DanhMucBanFillet",
       "DanhMuc/Fillet/BanFillet/{action}/{id?}",
       new { controller = "BanFillet" }
   );

    endpoints.MapControllerRoute(
        "DanhMucMauCaGiongVungNuoi",
        "DanhMuc/NguyenLieu/MauCaGiongVungNuoi/{action}/{id?}",
        new { controller = "MauCaGiongVungNuoi" }
    );
    endpoints.MapControllerRoute(
        "DanhMucNhapDauAoNguyenLieu",
        "DanhMuc/NguyenLieu/NhapDauAoNguyenLieu/{action}/{id?}",
        new { controller = "NhapDauAoNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucNhapCaTraNguyenLieu",
        "DanhMuc/NguyenLieu/NhapCaTraNguyenLieu/{action}/{id?}",
        new { controller = "NhapCaTraNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucMauNguyenLieu",
        "DanhMuc/NguyenLieu/MauNguyenLieu/{action}/{id?}",
        new { controller = "MauNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucSizeNguyenLieu",
        "DanhMuc/NguyenLieu/SizeNguyenLieu/{action}/{id?}",
        new { controller = "SizeNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucThanhPhamNguyenLieu",
        "DanhMuc/NguyenLieu/ThanhPhamNguyenLieu/{action}/{id?}",
        new { controller = "ThanhPhamNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucLoaiCaNguyenLieu",
        "DanhMuc/NguyenLieu/LoaiCaNguyenLieu/{action}/{id?}",
        new { controller = "LoaiCaNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucPhuongTienTaiTrong",
        "DanhMuc/NguyenLieu/PhuongTienTaiTrong/{action}/{id?}",
        new { controller = "PhuongTienTaiTrong" }
    );
    endpoints.MapControllerRoute(
        "DanhMucPhuongTienNguyenLieu",
        "DanhMuc/NguyenLieu/PhuongTienNguyenLieu/{action}/{id?}",
        new { controller = "PhuongTienNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucDonGiaVanChuyen",
        "DanhMuc/NguyenLieu/DonGiaVanChuyen/{action}/{id?}",
        new { controller = "DonGiaVanChuyen" }
    );
    endpoints.MapControllerRoute(
        "DanhMucNhaCungCap",
        "DanhMuc/NguyenLieu/NhaCungCap/{action}/{id?}",
        new { controller = "NhaCungCap" }
    );
    endpoints.MapControllerRoute(
        "DanhMucBanCatTiet",
        "DanhMuc/NguyenLieu/BanCatTiet/{action}/{id?}",
        new { controller = "BanCatTiet" }
    );
    endpoints.MapControllerRoute(
        "DanhMucAo",
        "DanhMuc/NguyenLieu/Ao/{action}/{id?}",
        new { controller = "Ao" }
    );
    endpoints.MapControllerRoute(
        "DanhMucStaff",
        "DanhMuc/Staff/{action}/{id?}",
        new { controller = "Staff" }
    );
    endpoints.MapControllerRoute(
        "BaoCaoBTPDinhHinh",
        "BaoCao/DinhHinh/BTPDinhHinh/{action}/{id?}",
        new { controller = "BTPDinhHinh" }
    );

    endpoints.MapControllerRoute(
        "BaoCaoTPDinhHinh",
        "BaoCao/DinhHinh/TP/{action}/{id?}",
        new { controller = "TP" });

    endpoints.MapControllerRoute(
        "BaoCaoTPFillet",
        "BaoCao/Fillet/TPFillet/{action}/{id?}",
        new { controller = "TPFillet" });

    endpoints.MapControllerRoute(
        "BaoCaoBTPFilletv2",
        "BaoCao/Fillet/BTPFilletv2/{action}/{id?}",
        new { controller = "BTPFilletv2" });

    endpoints.MapControllerRoute(
        "BaoCaoTPFilletv2",
        "BaoCao/Fillet/TPFilletv2/{action}/{id?}",
        new { controller = "TPFilletv2" });

    endpoints.MapControllerRoute(
        "BaoCaoTom",
        "BaoCao/T/T_PhieuCan/{action}/{id?}",
        new { controller = "T_PhieuCan" });


    endpoints.MapControllerRoute(
       "DanhMucLoaiCaDinhHinh",
       "DanhMuc/DinhHinh/LoaiCaDinhHinh/{action}/{id?}",
       new { controller = "LoaiCaDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "DanhMucThanhPhamDinhHinh",
       "DanhMuc/DinhHinh/ThanhPhamDinhHinh/{action}/{id?}",
       new { controller = "ThanhPhamDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "DanhMucThanhPhamTyLeDinhHinh",
       "DanhMuc/DinhHinh/ThanhPhamTyLeDinhHinh/{action}/{id?}",
       new { controller = "ThanhPhamTyLeDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "DanhMucSizeDinhHinh",
       "DanhMuc/DinhHinh/SizeDinhHinh/{action}/{id?}",
       new { controller = "SizeDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "DanhMucMauDinhHinh",
       "DanhMuc/DinhHinh/MauDinhHinh/{action}/{id?}",
       new { controller = "MauDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "DanhMucDinhMucDinhHinh",
       "DanhMuc/DinhHinh/DinhMucDinhHinh/{action}/{id?}",
       new { controller = "DinhMucDinhHinh" }
   );


    endpoints.MapControllerRoute(
       "ChatLuongChinhXepKhuon",
       "DanhMuc/XepKhuon/Chinh/ChatLuongChinhXepKhuon/{action}/{id?}",
       new { controller = "ChatLuongChinhXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ChieuXaChinhXepKhuon",
       "DanhMuc/XepKhuon/Chinh/ChieuXaChinhXepKhuon/{action}/{id?}",
       new { controller = "ChieuXaChinhXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "CoiChinhXepKhuon",
       "DanhMuc/XepKhuon/Chinh/CoiChinhXepKhuon/{action}/{id?}",
       new { controller = "CoiChinhXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "CoiLeXepKhuon",
       "DanhMuc/XepKhuon/Chinh/CoiLeXepKhuon/{action}/{id?}",
       new { controller = "CoiLeXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ChiTietRaCoiXepKhuon",
       "DanhMuc/XepKhuon/Chinh/ChiTietRaCoiXepKhuon/{action}/{id?}",
       new { controller = "ChiTietRaCoiXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "SizeChinhXepKhuon",
       "DanhMuc/XepKhuon/Chinh/SizeChinhXepKhuon/{action}/{id?}",
       new { controller = "SizeChinhXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamChinhXepKhuon",
       "DanhMuc/XepKhuon/Chinh/ThanhPhamChinhXepKhuon/{action}/{id?}",
       new { controller = "ThanhPhamChinhXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "TrongLuongCoiTheoThanhPham",
       "DanhMuc/XepKhuon/Chinh/TrongLuongCoiTheoThanhPham/{action}/{id?}",
       new { controller = "TrongLuongCoiTheoThanhPham" }
   );
     endpoints.MapControllerRoute(
       "TrongLuongCoiTheoSanPham",
       "DanhMuc/XepKhuon/Chinh/TrongLuongCoiTheoSanPham/{action}/{id?}",
       new { controller = "TrongLuongCoiTheoSanPham" }
   );
    endpoints.MapControllerRoute(
       "SizePhuXepKhuon",
       "DanhMuc/XepKhuon/Phu/SizePhuXepKhuon/{action}/{id?}",
       new { controller = "SizePhuXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamPhuXepKhuon",
       "DanhMuc/XepKhuon/Phu/ThanhPhamPhuXepKhuon/{action}/{id?}",
       new { controller = "ThanhPhamPhuXepKhuon" }
   );
    endpoints.MapControllerRoute(
      "SizeKXLXepKhuon",
      "DanhMuc/XepKhuon/KXL/SizeKXLXepKhuon/{action}/{id?}",
      new { controller = "SizeKXLXepKhuon" }
  );
    endpoints.MapControllerRoute(
       "ThanhPhamKXLXepKhuon",
       "DanhMuc/XepKhuon/KXL/ThanhPhamKXLXepKhuon/{action}/{id?}",
       new { controller = "ThanhPhamKXLXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamBlockXepKhuon",
       "DanhMuc/XepKhuon/Block/ThanhPhamBlockXepKhuon/{action}/{id?}",
       new { controller = "ThanhPhamBlockXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "SizeBlockXepKhuon",
       "DanhMuc/XepKhuon/Block/SizeBlockXepKhuon/{action}/{id?}",
       new { controller = "SizeBlockXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ChatLuongBlockXepKhuon",
       "DanhMuc/XepKhuon/Block/ChatLuongBlockXepKhuon/{action}/{id?}",
       new { controller = "ChatLuongBlockXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "MauBlockXepKhuon",
       "DanhMuc/XepKhuon/Block/MauBlockXepKhuon/{action}/{id?}",
       new { controller = "MauBlockXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "NetBlockXepKhuon",
       "DanhMuc/XepKhuon/Block/NetBlockXepKhuon/{action}/{id?}",
       new { controller = "NetBlockXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ChatLuongTaiCheXepKhuon",
       "DanhMuc/XepKhuon/TaiChe/ChatLuongTaiCheXepKhuon/{action}/{id?}",
       new { controller = "ChatLuongTaiCheXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "CongViecTaiCheXepKhuon",
       "DanhMuc/XepKhuon/TaiChe/CongViecTaiCheXepKhuon/{action}/{id?}",
       new { controller = "CongViecTaiCheXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "CongViecTaiCheXepKhuon",
       "DanhMuc/XepKhuon/TaiChe/CongViecTaiCheXepKhuon/{action}/{id?}",
       new { controller = "CongViecTaiCheXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "MauTaiCheXepKhuon",
       "DanhMuc/XepKhuon/TaiChe/MauTaiCheXepKhuon/{action}/{id?}",
       new { controller = "MauTaiCheXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "SizeTaiCheXepKhuon",
       "DanhMuc/XepKhuon/TaiChe/SizeTaiCheXepKhuon/{action}/{id?}",
       new { controller = "SizeTaiCheXepKhuon" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamTaiCheXepKhuon",
       "DanhMuc/XepKhuon/TaiChe/ThanhPhamTaiCheXepKhuon/{action}/{id?}",
       new { controller = "ThanhPhamTaiCheXepKhuon" }
   );

    endpoints.MapControllerRoute(
       "ThanhPhamPhuPham",
       "DanhMuc/PhuPham/ThanhPhamPhuPham/{action}/{id?}",
       new { controller = "ThanhPhamPhuPham" }
   );
    endpoints.MapControllerRoute(
       "LoaiCaPhuPham",
       "DanhMuc/PhuPham/LoaiCaPhuPham/{action}/{id?}",
       new { controller = "LoaiCaPhuPham" }
   );
    endpoints.MapControllerRoute(
       "SizePhuPham",
       "DanhMuc/PhuPham/SizePhuPham/{action}/{id?}",
       new { controller = "SizePhuPham" }
   );
    endpoints.MapControllerRoute(
       "MauPhuPham",
       "DanhMuc/PhuPham/MauPhuPham/{action}/{id?}",
       new { controller = "MauPhuPham" }
   );
    endpoints.MapControllerRoute(
       "PhuongTienPhuPham",
       "DanhMuc/PhuPham/PhuongTienPhuPham/{action}/{id?}",
       new { controller = "PhuongTienPhuPham" }
   );
    endpoints.MapControllerRoute(
       "KhachHangPhuPham",
       "DanhMuc/PhuPham/KhachHangPhuPham/{action}/{id?}",
       new { controller = "KhachHangPhuPham" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamLangDa",
       "DanhMuc/LangDa/ThanhPhamLangDa/{action}/{id?}",
       new { controller = "ThanhPhamLangDa" }
   );
    endpoints.MapControllerRoute(
       "LoaiCaLangDa",
       "DanhMuc/LangDa/LoaiCaLangDa/{action}/{id?}",
       new { controller = "LoaiCaLangDa" }
   );
    endpoints.MapControllerRoute(
       "SizeLangDa",
       "DanhMuc/LangDa/SizeLangDa/{action}/{id?}",
       new { controller = "SizeLangDa" }
   );
    endpoints.MapControllerRoute(
       "SizeLangDa",
       "DanhMuc/LangDa/SizeLangDa/{action}/{id?}",
       new { controller = "SizeLangDa" }
   );
    endpoints.MapControllerRoute(
       "MauLangDa",
       "DanhMuc/LangDa/MauLangDa/{action}/{id?}",
       new { controller = "MauLangDa" }
   );
    endpoints.MapControllerRoute(
       "LoaiCaSoCheDinhHinh",
       "DanhMuc/SoCheDinhHinh/LoaiCaSoCheDinhHinh/{action}/{id?}",
       new { controller = "LoaiCaSoCheDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamSoCheDinhHinh",
       "DanhMuc/SoCheDinhHinh/ThanhPhamSoCheDinhHinh/{action}/{id?}",
       new { controller = "ThanhPhamSoCheDinhHinh" }
   );
    endpoints.MapControllerRoute(
       "KhachHangBaoTu",
       "DanhMuc/BaoTu/KhachHangBaoTu/{action}/{id?}",
       new { controller = "KhachHangBaoTu" }
   );
    endpoints.MapControllerRoute(
       "LoaiCaBaoTu",
       "DanhMuc/BaoTu/LoaiCaBaoTu/{action}/{id?}",
       new { controller = "LoaiCaBaoTu" }
   );
    endpoints.MapControllerRoute(
       "ThanhPhamBaoTu",
       "DanhMuc/BaoTu/ThanhPhamBaoTu/{action}/{id?}",
       new { controller = "ThanhPhamBaoTu" }
   );
    endpoints.MapControllerRoute(
       "DatabaseManagement",
       "DatabaseManagement/{action}/{id?}",
       new { controller = "DatabaseManagement" }
   );
    endpoints.MapControllerRoute(
       "DonViTinhQuanLyPhuGia",
       "DanhMuc/XepKhuon/QuanLyPhuGia/DonViTinh/{action}/{id?}",
       new { controller = "DonViTinh" }
   );
    endpoints.MapControllerRoute(
       "LyDoQuanLyPhuGia",
       "DanhMuc/XepKhuon/QuanLyPhuGia/LyDo/{action}/{id?}",
       new { controller = "LyDo" }
   );
    endpoints.MapControllerRoute(
       "SanPhamQuanLyPhuGia",
       "DanhMuc/XepKhuon/QuanLyPhuGia/SanPham/{action}/{id?}",
       new { controller = "SanPham" }
   );
    endpoints.MapControllerRoute(
       "CongThucQuanLyPhuGia",
       "DanhMuc/XepKhuon/QuanLyPhuGia/CongThuc/{action}/{id?}",
       new { controller = "CongThuc" }
   );
    endpoints.MapControllerRoute(
       "LoaiSanPhamQuanLyPhuGia",
       "DanhMuc/XepKhuon/QuanLyPhuGia/LoaiSanPham/{action}/{id?}",
       new { controller = "LoaiSanPham" }
   );
    endpoints.MapControllerRoute(
       "ChinhTinhLuongDinhHinh",
       "TinhLuong/DinhHinh/Chinh/{action}/{id?}",
       new { controller = "Chinh" }
   );
    endpoints.MapControllerRoute(
       "ToKiemTinhLuongDinhHinh",
       "TinhLuong/DinhHinh/ToKiem/{action}/{id?}",
       new { controller = "ToKiem" }
   );
    endpoints.MapControllerRoute(
       "SoCheTinhLuongDinhHinh",
       "TinhLuong/DinhHinh/SoChe/{action}/{id?}",
       new { controller = "SoChe" }
   );
    endpoints.MapControllerRoute(
       "ToKiemSoCheTinhLuongDinhHinh",
       "TinhLuong/DinhHinh/ToKiemSoChe/{action}/{id?}",
       new { controller = "ToKiemSoChe" }
   );
    endpoints.MapControllerRoute(
       "PhucVuTinhLuongDinhHinh",
       "TinhLuong/DinhHinh/PhucVu/{action}/{id?}",
       new { controller = "PhucVu" }
   );
    endpoints.MapControllerRoute(
       "ChinhTinhLuongPhuFillet",
       "TinhLuong/PhuFillet/ChinhPhuFillet/{action}/{id?}",
       new { controller = "ChinhPhuFillet" }
   );
    endpoints.MapControllerRoute(
       "ChinhTinhLuongFilletv2",
       "TinhLuong/Filletv2/ChinhFilletv2/{action}/{id?}",
       new { controller = "ChinhFilletv2" }
   );
    endpoints.MapControllerRoute(
       "ChinhTinhLuongPhuPhamv2",
       "TinhLuong/PhuPhamv2/ChinhPhuPhamv2/{action}/{id?}",
       new { controller = "ChinhPhuPhamv2" }
   );
    endpoints.MapControllerRoute(
       "ChinhTinhLuongFillet",
       "TinhLuong/Fillet/ChinhFillet/{action}/{id?}",
       new { controller = "ChinhFillet" }
   );
    endpoints.MapControllerRoute(
       "GianTiepTinhLuongXepKhuon",
       "TinhLuong/XepKhuon/GianTiep/{action}/{id?}",
       new { controller = "GianTiep" }
   );
    endpoints.MapControllerRoute(
       "TrucTiepTinhLuongXepKhuon",
       "TinhLuong/XepKhuon/TrucTiep/{action}/{id?}",
       new { controller = "TrucTiep" }
   );
    endpoints.MapControllerRoute(
       "ChinhTinhLuongBaoTu",
       "TinhLuong/BaoTu/ChinhBaoTu/{action}/{id?}",
       new { controller = "ChinhBaoTu" }
   );
    endpoints.MapControllerRoute(
       "QuanLyKetChuyen",
       "TinhLuong/QuanLyKetChuyen/{action}/{id?}",
       new { controller = "QuanLyKetChuyen" }
   );
    endpoints.MapControllerRoute(
       "KetNoiBravo",
       "TinhLuong/KetNoiBravo/{action}/{id?}",
       new { controller = "KetNoiBravo" }
   );
    endpoints.MapControllerRoute(
       "DauAoCaGiong",
       "BaoCao/DauAo/DauAoCaGiong/{action}/{id?}",
       new { controller = "DauAoCaGiong" }
   );
    endpoints.MapControllerRoute(
       "DauAoCaThit",
       "BaoCao/DauAo/DauAoCaThit/{action}/{id?}",
       new { controller = "DauAoCaThit" }
   );
    endpoints.MapControllerRoute(
       "DauAoCaChet",
       "BaoCao/DauAo/DauAoCaChet/{action}/{id?}",
       new { controller = "DauAoCaChet" }
   );
    endpoints.MapControllerRoute(
      "BaoCaoNguyenLieu",
      "BaoCao/NguyenLieu/BaoCaoNguyenLieu/{action}/{id?}",
      new { controller = "BaoCaoNguyenLieu" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoPhuPham",
      "BaoCao/PhuPham/BaoCaoPhuPham/{action}/{id?}",
      new { controller = "BaoCaoPhuPham" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoPhuPhamv2",
      "BaoCao/PhuPham/BaoCaoPhuPhamv2/{action}/{id?}",
      new { controller = "BaoCaoPhuPhamv2" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoLangDa",
      "BaoCao/LangDa/BaoCaoLangDa/{action}/{id?}",
      new { controller = "BaoCaoLangDa" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoSoCheDinhHinh",
      "BaoCao/SoCheDinhHinh/BaoCaoSoCheDinhHinh/{action}/{id?}",
      new { controller = "BaoCaoSoCheDinhHinh" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoSanPhamChinhXepKhuon",
      "BaoCao/XepKhuon/BaoCaoSanPhamChinhXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoSanPhamChinhXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoSanPhamPhuXepKhuon",
      "BaoCao/XepKhuon/BaoCaoSanPhamPhuXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoSanPhamPhuXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoBlockXepKhuon",
      "BaoCao/XepKhuon/BaoCaoBlockXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoBlockXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoKXLXepKhuon",
      "BaoCao/XepKhuon/BaoCaoKXLXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoKXLXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoTaiCheXepKhuon",
      "BaoCao/XepKhuon/BaoCaoTaiCheXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoTaiCheXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoPhuGiaXepKhuon",
      "BaoCao/XepKhuon/BaoCaoPhuGiaXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoPhuGiaXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoRaCoiXepKhuon",
      "BaoCao/XepKhuon/BaoCaoRaCoiXepKhuon/{action}/{id?}",
      new { controller = "BaoCaoRaCoiXepKhuon" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoBaoTu",
      "BaoCao/BaoTu/BaoCaoBaoTu/{action}/{id?}",
      new { controller = "BaoCaoBaoTu" }
  );
    
    endpoints.MapControllerRoute(
      "BaoCaoNhanSu_KeToan",
      "BaoCao/NhanSu-KeToan/BaoCaoNhanSu_KeToan/{action}/{id?}",
      new { controller = "BaoCaoNhanSu_KeToan" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoNguyenLieu_KeToan",
      "BaoCao/NguyenLieu-KeToan/BaoCaoNguyenLieu_KeToan/{action}/{id?}",
      new { controller = "BaoCaoNguyenLieu_KeToan" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoCanNguyenLieuHQ",
      "HQ/BaoCaoCanNguyenLieuHQ/{action}/{id?}",
      new { controller = "BaoCaoCanNguyenLieuHQ" }
  );
    endpoints.MapControllerRoute(
      "BaoCaoHQ",
      "HQ/BaoCaoHQ/{action}/{id?}",
      new { controller = "BaoCaoHQ" }
  );
    endpoints.MapControllerRoute(
        "DanhMucHQ_Color",
        "DanhMucHQ/HQ_Color/{action}/{id?}",
        new { controller = "HQ_Color" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_LoaiNguyenLieu",
        "DanhMucHQ/HQ_LoaiNguyenLieu/{action}/{id?}",
        new { controller = "HQ_LoaiNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_LoChiTiet",
        "DanhMucHQ/HQ_LoChiTiet/{action}/{id?}",
        new { controller = "HQ_LoChiTiet" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_Lo",
        "DanhMucHQ/HQ_Lo/{action}/{id?}",
        new { controller = "HQ_Lo" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_Size",
        "DanhMucHQ/HQ_Size/{action}/{id?}",
        new { controller = "HQ_Size" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_ThanhPham",
        "DanhMucHQ/HQ_ThanhPham/{action}/{id?}",
        new { controller = "HQ_ThanhPham" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_Nhom",
        "DanhMucHQ/HQ_Nhom/{action}/{id?}",
        new { controller = "HQ_Nhom" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_NhanVienTheoNhom",
        "DanhMucHQ/HQ_NhanVienTheoNhom/{action}/{id?}",
        new { controller = "HQ_NhanVienTheoNhom" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_Ca",
        "DanhMucHQ/HQ_Ca/{action}/{id?}",
        new { controller = "HQ_Ca" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_NhanVienTheoCa",
        "DanhMucHQ/HQ_NhanVienTheoCa/{action}/{id?}",
        new { controller = "HQ_NhanVienTheoCa" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_MapSanPhamTinhLuong",
        "DanhMucHQ/HQ_MapSanPhamTinhLuong/{action}/{id?}",
        new { controller = "HQ_MapSanPhamTinhLuong" }
    );
    endpoints.MapControllerRoute(
        "HQ_LoaiMonAn",
        "NhaAn/DanhMucNhaAn/HQ_LoaiMonAn/{action}/{id?}",
        new { controller = "HQ_LoaiMonAn" }
    );
    endpoints.MapControllerRoute(
        "HQ_MonAn",
        "NhaAn/DanhMucNhaAn/HQ_MonAn/{action}/{id?}",
        new { controller = "HQ_MonAn" }
    );
    endpoints.MapControllerRoute(
        "HQ_ThucDon",
        "NhaAn/DanhMucNhaAn/HQ_ThucDon/{action}/{id?}",
        new { controller = "HQ_ThucDon" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_ChatLuongNguyenLieu",
        "DanhMucHQ/HQ_ChatLuongNguyenLieu/{action}/{id?}",
        new { controller = "HQ_ChatLuongNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_DonViTinh",
        "DanhMucHQ/HQ_DonViTinh/{action}/{id?}",
        new { controller = "HQ_DonViTinh" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_KhoNguyenLieu",
        "DanhMucHQ/HQ_KhoNguyenLieu/{action}/{id?}",
        new { controller = "HQ_KhoNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_SanPhamNguyenLieu",
        "DanhMucHQ/HQ_SanPhamNguyenLieu/{action}/{id?}",
        new { controller = "HQ_SanPhamNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_QuyCachNguyenLieu",
        "DanhMucHQ/HQ_QuyCachNguyenLieu/{action}/{id?}",
        new { controller = "HQ_QuyCachNguyenLieu" }
    );
    endpoints.MapControllerRoute(
        "DanhMucHQ_PhuongTienNguyenLieu",
        "DanhMucHQ/HQ_PhuongTienNguyenLieu/{action}/{id?}",
        new { controller = "HQ_PhuongTienNguyenLieu" }
    );
    //endpoints.MapControllerRoute(
    //    "DuyetThucDon",
    //    "NhaAn/DanhMuc/HQ_ThucDon/DuyetThucDon{action}/{id?}",
    //    new { controller = "DuyetThucDon" }
    //);
    //endpoints.MapControllerRoute(
    //    "HuyThucDon",
    //    "NhaAn/DanhMuc/HQ_ThucDon/HuyThucDon{action}/{id?}",
    //    new { controller = "HuyThucDon" }
    //);
    





    endpoints.MapControllerRoute(
        "error",
        "Error/{action}/{id?}",
        new { controller = "Error" });

    endpoints.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}");
});
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();