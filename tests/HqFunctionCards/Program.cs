using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Models.Repos.Models;
using PMSHub.Components.Pages;
using Syncfusion.Blazor.Inputs;
using ViewModels.Repos.Hubs.IServices;
using CardDao = Dao.Repos.HQ.TheThanhPham;
using CardVm = ViewModels.Repos.HQ.TheThanhPhamViewModel;

if (!args.Contains("--sql-localdb")) throw new Exception("Explicit --sql-localdb required; no factory config is read.");
var checks = 0;
void Check(bool ok, string label) { if (!ok) throw new Exception("FAIL: " + label); checks++; Console.WriteLine("PASS: " + label); }
var name = "RCQ_FunctionCardTest_" + Guid.NewGuid().ToString("N");
if (!name.StartsWith("RCQ_FunctionCardTest_") || name.Length != 53) throw new Exception("Fixture guard");
var masterString = @"Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=True;TrustServerCertificate=True;Pooling=False";
var connectionString = new SqlConnectionStringBuilder(masterString) { InitialCatalog = name }.ConnectionString;
await using var master = new SqlConnection(masterString);
await master.OpenAsync();
await master.ExecuteAsync($"CREATE DATABASE [{name}]");
await using var sql = new SqlConnection(connectionString);
try
{
    await sql.OpenAsync();
    await sql.ExecuteAsync(@"
        CREATE TABLE dbo.TheThanhPham (
            MaThe varchar(50) NOT NULL PRIMARY KEY, MaThanhPham varchar(50), MaSizeDinhHinh varchar(50),
            TrongLuongTare decimal(18,3) NOT NULL CHECK (TrongLuongTare>=0), MaMayLangDa varchar(50),
            MaThanhPhamFillet varchar(50), MaThanhPhamPhuPham varchar(50), MaSizeFillet varchar(50),
            IsZero bit, MaLoaiNguyenLieu varchar(50), IsRestart bit, MaQuyTrinhT varchar(50),
            STTPhieuPhanCoChiTietT int NOT NULL, MaCoiXepKhuon varchar(50), NgayGio datetime2 NOT NULL,
            MaSize varchar(50), LoLevel int NOT NULL, MaChatLuongChinhXepKhuon varchar(50),
            MaChatLuongPhuXepKhuon varchar(50), MaSizeChinhXepKhuon varchar(50), MaSizePhuXepKhuon varchar(50),
            MaThanhPhamChinhXepKhuon varchar(50), MaThanhPhamPhuXepKhuon varchar(50));
        CREATE TABLE dbo.HQ_ThanhPham (Id varchar(50) PRIMARY KEY, Ten nvarchar(100));
        CREATE TABLE dbo.HQ_Size (Id varchar(50) PRIMARY KEY, Ten nvarchar(100));
        CREATE TABLE dbo.HQ_LoaiNguyenLieu (Id varchar(50) PRIMARY KEY, Ten nvarchar(100));
        INSERT dbo.HQ_ThanhPham VALUES ('TP1',N'Thành phẩm 1'),('TP2',N'Thành phẩm 2');
        INSERT dbo.HQ_Size VALUES ('20X20',N'20x20'),('10X10',N'10x10');
        INSERT dbo.HQ_LoaiNguyenLieu VALUES ('NL1',N'Nguyên liệu 1'),('NL2',N'Nguyên liệu 2');");
    Check(await sql.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.tables WHERE name LIKE '%ChieuXa%'")==0 &&
        await sql.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID('dbo.TheThanhPham') AND name='MaChieuXa'")==0,
        "Fixture has neither irradiation tables nor MaChieuXa column");
    var dao = new CardDao(connectionString);
    TheThanhPham Card(string id) => new() { MaThe=id,MaThanhPham="TP1",MaSize="20X20",MaLoaiNguyenLieu="NL1",
        TrongLuongTare=.80m,IsZero=true,IsRestart=false,NgayGio=new DateTime(2026,10,26,9,0,0) };
    var first = Card("0004348780");
    SqlException? schemaError=null;
    try { dao.Insert(first); } catch (SqlException ex) {schemaError=ex;}
    Check(schemaError?.Number==207,"Reproduce original legacy INSERT failure against schema without MaChieuXa");
    Check(dao.InsertHq(first)==1,"HQ INSERT succeeds without adding any SQL column/table");
    var loaded=dao.GetHqs<TheThanhPham>().Single();
    Check(loaded.MaThe==first.MaThe && loaded.MaThanhPham=="TP1" && loaded.MaSize=="20X20" &&
        loaded.MaLoaiNguyenLieu=="NL1" && loaded.TrongLuongTare==.80m && loaded.IsZero==true && loaded.IsRestart==false,
        "Reload preserves UID leading zeros, codes, tare and zero/restart");
    Check(loaded.ThanhPhamName=="Thành phẩm 1" && loaded.SizeName=="20x20" && loaded.LoaiNguyenLieuName=="Nguyên liệu 1",
        "HQ reload joins only existing HQ catalogues");
    first.MaThanhPham="TP2";first.MaLoaiNguyenLieu="NL2";first.MaSize="10X10";first.IsRestart=true;
    Check(dao.UpdateHq(first)==1,"HQ single UPDATE does not require irradiation");
    loaded=dao.GetHqs<TheThanhPham>().Single();
    Check(loaded.MaThanhPham=="TP2" && loaded.MaLoaiNguyenLieu=="NL2" && loaded.MaSize=="10X10" && loaded.IsRestart==true,
        "Product, material, size and restart are really persisted, not just changed in cache");
    var second=Card("0004348781");dao.InsertHq(second);
    first.TrongLuongTare=.70m;second.TrongLuongTare=.60m;
    Check(dao.UpdateHq(new List<TheThanhPham>{first,second})==2,"HQ batch UPDATE succeeds");
    first.TrongLuongTare=.50m;second.TrongLuongTare=-1m;
    try {dao.UpdateHq(new List<TheThanhPham>{first,second});throw new Exception("Expected test CHECK failure");}
    catch(SqlException ex) {Check(ex.Number==547,"SQL failure on second batch row is surfaced");}
    var after=dao.GetHqs<TheThanhPham>().ToDictionary(x=>x.MaThe);
    Check(after[first.MaThe].TrongLuongTare==.70m && after[second.MaThe].TrongLuongTare==.60m,
        "Failed HQ batch rolls back all rows, no partially applied edits");
    SqlException? duplicate=null;
    try {dao.InsertHq(first);}catch(SqlException ex){duplicate=ex;}
    Check(duplicate?.Number is 2601 or 2627 && dao.GetHqs<TheThanhPham>().Count==2,"Duplicate UID never overwrites existing card");

    // Exercise the actual Razor event boundary with real SQL exceptions. No render, no factory config.
    var page=new TheChucNang();
    Exception injected=schemaError!;
    var main=Mock<IMainService>((_,_)=>throw injected);
    var logger=Mock<ILogger<TheChucNang>>((_,_)=>throw new IOException("Test logger failure"));
    const BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
    typeof(TheChucNang).GetProperty("MainService",flags)!.SetValue(page,main);
    typeof(TheChucNang).GetProperty("Logger",flags)!.SetValue(page,logger);
    var handler=typeof(TheChucNang).GetMethod("ValueChangeHandler",flags)!;
    var message=typeof(TheChucNang).GetField("ThongBao",flags)!;
    foreach(var (error,expected) in new (Exception,string)[] {
        (schemaError!,"Cấu trúc SQL chưa phù hợp. Báo quản lý."),
        (duplicate!,"Thẻ đã tồn tại. Tải lại danh sách."),
        (new IOException("SECRET_DETAIL"),"Không xử lý được thẻ. Tải lại danh sách để kiểm tra.")})
    {
        injected=error;
        handler.Invoke(page,new object[]{new ChangedEventArgs{Value="0004348780"}});
        Check((string?)message.GetValue(page)==expected,"Razor catches error and failing logger without terminating event: "+error.GetType().Name);
        Check((string?)typeof(TheChucNang).GetField("TheId",flags)!.GetValue(page)=="","Input is reset even after database error");
    }
    handler.Invoke(page,new object[]{new ChangedEventArgs{Value=null!}});
    handler.Invoke(page,new object[]{new ChangedEventArgs{Value="123"}});
    Check(true,"Cleared/partial UID input is ignored safely");

    // Verify profile routing without constructing any production VM or reading a settings file.
    var singleton=typeof(AppViewModels.AppViewModel).GetField("_instance",BindingFlags.NonPublic|BindingFlags.Static)!;
    var previous=singleton.GetValue(null);
    try
    {
        var app=(AppViewModels.AppViewModel)RuntimeHelpers.GetUninitializedObject(typeof(AppViewModels.AppViewModel));
        singleton.SetValue(null,app);
        var vm=(CardVm)RuntimeHelpers.GetUninitializedObject(typeof(CardVm));
        var profile=typeof(CardVm).GetProperty("IsHq",flags)!;
        foreach(var code in new[]{"RCQTG","rcqtg","PMS"})
        {app.ComName=code;Check((bool)profile.GetValue(vm)! == (code!="PMS"),"HQ/legacy profile routing: "+code);}
    }
    finally {singleton.SetValue(null,previous);}

    await LotChecks.Run(connectionString, sql, Check);

    // Legacy installations retain their irradiation writes when that schema exists.
    await sql.ExecuteAsync("ALTER TABLE dbo.TheThanhPham ADD MaChieuXa varchar(50) NULL");
    var legacy=Card("0004348782");legacy.MaChieuXa="CX1";
    Check(dao.Insert(legacy)==1,"Legacy INSERT still supports MaChieuXa");
    legacy.MaChieuXa="CX2";Check(dao.Update(legacy)==1,"Legacy UPDATE still supports MaChieuXa");
    Check(await sql.ExecuteScalarAsync<string>("SELECT MaChieuXa FROM dbo.TheThanhPham WHERE MaThe=@MaThe",legacy)=="CX2",
        "Legacy irradiation value is retained");
    Check(dao.Delete(second)==1 && dao.GetHqs<TheThanhPham>().Count==2,"Delete and reload work after earlier SQL errors");
    Console.WriteLine($"PASS: {checks} HQ function-card assertions; no factory DB/config accessed.");
}
finally
{
    await sql.CloseAsync();
    await master.ExecuteAsync($"DROP DATABASE [{name}]");
    Console.WriteLine("CLEANUP: isolated GUID LocalDB fixture deleted.");
}

static T Mock<T>(Func<MethodInfo,object?[]?,object?> handler) where T:class
{var proxy=DispatchProxy.Create<T,TestProxy>();((TestProxy)(object)proxy).Handler=handler;return proxy;}
public class TestProxy:DispatchProxy
{public Func<MethodInfo,object?[]?,object?> Handler=null!;protected override object? Invoke(MethodInfo? m,object?[]? a)=>Handler(m!,a);}
