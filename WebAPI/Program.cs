using System.Text;
using AppViewModels;
using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.Repos;
using Newtonsoft.Json.Serialization;
using WebAPI.Models;



// Db config


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
SqlMapper.AddTypeHandler(new Models.Repos.Handler.DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new Models.Repos.Handler.TimeOnlyTypeHandler());
// Configuration
//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json")
//    .Build();
//builder.Services.Configure<AppSetting>(configuration.GetSection("AppSettings"));
//var secretKey = configuration["AppSettings:SecretKey"];
//var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>{
//    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//    {
//        //tự cấp token
//        ValidateIssuer = false,
//        ValidateAudience = false,

//        //ký vào token
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

//        ClockSkew = TimeSpan.Zero
//    };
//});
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AppSetting>(configuration.GetSection("AppSettings"));
var secretKey = configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

// Add cookie middleware
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None;
    options.Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Có thể thay đổi tùy theo yêu cầu an toàn
});
builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        // Thiết lập các tham số xác thực token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //1: Không xác thực Issuer (người tạo ra token) và Audience (đối tượng của token)
            // - ValidateIssuer và ValidateAudience được thiết lập thành false, có nghĩa là không xác thực Issuer (người tạo ra token) và Audience (đối tượng mà token được tạo ra cho mục đích gì).
            // - Cho phép mọi Issuer và Audience được chấp nhận.
            ValidateIssuer = false,
            ValidateAudience = false,

            //2: Xác minh token bằng cách so sánh với khóa bí mật (SecretKey) của ứng dụng
            // - ValidateIssuerSigningKey được thiết lập thành true, có nghĩa là token sẽ được xác minh bằng cách so sánh khóa ký hiệu (Issuer Signing Key) được chứa trong token với secretKeyBytes mà bạn đã cung cấp.
            // - Nếu khóa ký hiệu trong token không khớp với secretKeyBytes, token sẽ bị coi là không hợp lệ và xác thực sẽ thất bại.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),


            // ClockSkew được thiết lập là TimeSpan.Zero để loại bỏ sự chệch lệch thời gian
            // giữa máy chủ và client. Giá trị này sẽ giúp token sẽ không được chấp nhận
            // nếu thời gian hết hạn (exp claim) bằng chính xác thời điểm hiện tại
            ClockSkew = TimeSpan.Zero
        };
        // Thêm cấu hình để xử lý token từ query string cho SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                //var accessToken = context.Request.Query["access_token"];

                //if (!string.IsNullOrEmpty(accessToken))
                //{
                //    try
                //    {
                //        Assuming the client sends an encrypted token
                //       var decryptedToken = Security.Crypt.ED.DecryptString(accessToken);
                //        context.Token = decryptedToken;
                //    }
                //    catch (Exception ex)
                //    {
                //        Log the exception for debugging

                //       Console.WriteLine($"Token decryption failed: {ex.Message}");
                //    }
                //}
                var authorizationHeader = context.Request.Headers["Authorization"];

                if (!string.IsNullOrEmpty(authorizationHeader) && context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
                {
                    var authorizationHeaderValue = authHeaderValues.FirstOrDefault();
                    if (!string.IsNullOrEmpty(authorizationHeaderValue) && authorizationHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        var encryptedToken = authorizationHeaderValue.Substring("Bearer ".Length).Trim();

                        try
                        {
                            // Giải mã token
                            var decryptedToken = Security.Crypt.ED.DecryptString(encryptedToken);
                            context.Token = decryptedToken;
                        }
                        catch (Exception ex)
                        {
                            // Xử lý lỗi giải mã
                            Console.WriteLine($"Token decryption failed: {ex.Message}");
                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });
//.AddCookie(option =>
//{
//    option.Cookie = new CookieBuilder
//    {
//        HttpOnly = true, // Ngăn chặn truy cập từ phía JavaScript
//        Name = "MyCookies",
//        SameSite = SameSiteMode.None, // Cho phép gửi cookie trong các yêu cầu cross-site khi là none
//        Path = "/",
//    };
//    option.LoginPath = new PathString("/Authentication/Login");
//    option.ReturnUrlParameter = "url";
//    option.ExpireTimeSpan = TimeSpan.FromDays(30);
//});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin")); // Admin
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User")); // User
    options.AddPolicy("GuestPolicy", policy => policy.RequireRole("Guest")); // Guest
    options.AddPolicy("AllRoles", policy =>
    {
        policy.RequireRole("Admin", "User", "Guest"); // Yêu cầu người dùng có vai trò "Admin" hoặc "User"
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        // Thêm xác thực JWT vào Swagger
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
        c.AddSecurityDefinition("Bearer", securityScheme);
        var securityRequirement = new OpenApiSecurityRequirement
        {
            { securityScheme, new[] { "Bearer" } }
        };
        c.AddSecurityRequirement(securityRequirement);
    }
);
var vmApp = AppViewModel.Instance;
var datetimenow = vmApp.DateTimeNow;
builder.Services.AddDbContext<dbPMScontext>(options =>
    options.UseSqlServer(Base.Ins.ConnectionString2));


//builder.Services.AddCors(options =>
//    {
//        options.AddPolicy("AllowAll",
//            builder =>
//            {

//                builder.WithOrigins("https://*")
//                    .AllowAnyHeader()
//                    .AllowAnyMethod()
//                    .AllowCredentials(); // Cho phép truy cập có credentials
//            });
//    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins( 
                "https://localhost:44339",   
                vmApp.ApiHostUrl // Domain thực tế
                //"https://staging.yourapp.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Cho phép gửi cookie/token
    });
});
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{


    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowSpecificOrigins");

app.UseCookiePolicy();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
// Add cookie middleware

//app.MapHub<WebAPI.Hub.ProgressHub>("/progressHub"); // Định tuyến SignalR hub
app.MapHub<WebAPI.Hub.ProgressHub>("/progressHub").RequireAuthorization(); // Định tuyến SignalR hub
app.UseHttpsRedirection();


app.MapControllers();
app.Run();