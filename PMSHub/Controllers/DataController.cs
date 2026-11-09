using System.Globalization;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json.Linq;
using PMSHub.Components;
using Services;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DataController(IMayCansService mayCansService, IMainService mainService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get(string key)
    {
        var count = HttpContext.Session.GetString(key) ?? "";
       // HttpContext.Session.SetInt32(key, count);
        return Ok(new { count });
        //return Ok();
    }

    [HttpPost]
    public IActionResult Post([FromBody] object data)
    {
        IActionResult? hqResponse = null;
        string? hqWeightError = null;
        var result = ErrorHandler.Handle(() =>
        {
            //"{\"Id\":\"%s\",\"STT\":%d,\"NgayGio\":\"%s\",\"MayCan\":\"%s\",\"MaLo\":\"%s\","
            //"\"MaSize\":\"%s\",\"MaThanhPham\":\"%s\",\"MaNhanVien\":\"%s\","
            //"\"TrongLuongNhan\":\"%s\",\"TrongLuongTra\":\"%s\",\"TrongLuongTare\":\"%s\","
            //"\"TheId\":\"%s\",\"TheChucNang\":\"%s\",\"Ngay\":\"%s\",\"MaLoaiNguyenLieu\":\"%s\"}"
            var jsonString = data.ToString() ?? "";
            var jObject = JObject.Parse(jsonString);

            // HQ HTTP backup is historical, not live LITE. Route before legacy field conversions.
            var backupMachineId = (string?)jObject["MayCan"];
            var backupMachine = mayCansService.Find(backupMachineId ?? "");
            if (backupMachine == null)
            {
                hqWeightError = "Máy gửi phiếu chưa được đăng ký trên Hub.";
                throw new ArgumentException(hqWeightError);
            }
            if (backupMachine.WKv == AppKV.Hq)
            {
                hqResponse = SaveHqHttp(jObject, backupMachine, false);
                return;
            }

            //var item = jObject["data"]?[0];

            var id = (string)(jObject["Id"] ?? "")!;
            var stt = (int)(jObject["STT"] ?? 0);
            var ngayGio = (string)(jObject["NgayGio"] ?? "")!;
            var maycan = (string)(jObject["MayCan"] ?? "")!;
            var maLo = (string)(jObject["MaLo"] ?? "")!;
            var maSize = (string)(jObject["MaSize"] ?? "")!;
            var maThanhPham = (string)(jObject["MaThanhPham"] ?? "")!;
            var maNhanVien = (string)(jObject["MaNhanVien"] ?? "")!;
            var trongLuongNhan = (decimal)(jObject["TrongLuongNhan"] ?? 0);
            var trongLuongTra = (decimal)(jObject["TrongLuongTra"] ?? 0);
            //var trongLuongTare = (decimal)(jObject["TrongLuongTare"] ?? 0);
            var trongLuongTareStr = (string)(jObject["TrongLuongTare"] ?? "");
            var trongLuongTare = 0M;
            if(trongLuongTareStr.Trim() != "")
            {
                try
                {
                   var _trongLuongTare = (decimal)(jObject["TrongLuongTare"] ?? 0);
                   trongLuongTare = _trongLuongTare;
                }
                catch (Exception)
                {
                    //throw new Exception("Invalid TrongLuongTare format");
                }
            }
            
            
            var theId = (string)(jObject["TheId"] ?? "")!;
            var theChucNang = (string)(jObject["TheChucNang"] ?? "")!;
            var ngay = (string)(jObject["Ngay"] ?? "")!;
            var maLoaiNguyenLieu = (string)(jObject["MaLoaiNguyenLieu"] ?? "")!;
            var maAo= (string)(jObject["MaAo"] ?? "")!;
            var maPhuongTien = (string)(jObject["MaPhuongTien"] ?? "")!;
            //var mayCanId = (string)jObject["MayCan"];
            //var trongLuong = (decimal)jObject["TrongLuongNhan"];
            //var theId = (string)jObject["TheId"];
            //var ngayGio = (string)jObject["NgayGio"];
            var maCoi = (string)(jObject["MaCoi"] ?? "")!;
            var maChatLuong = (string)(jObject["MaChatLuong"] ?? "")!;
            var maChieuXa = (string)(jObject["MaChieuXa"] ?? "")!;
            
            var mayCan = backupMachine;
            
            if (mayCan != null)
            {


                switch (mayCan.WKv)
                {
                    case AppKV.NguyenLieu:
                    {
                        var phieuCan = mainService.VmPhieuCanNguyenLieu.Get<object>(id);
                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanNguyenLieu.CreateDefaultNew();
                            item.Id = id;
                            item.MSL = maLo;
                            item.MaLoaiThanhPham = maThanhPham;
                            item.TrongLuong = trongLuongNhan;
                            item.MaXuongSanXuat = mayCan.MaXuong;
                            item.TrongLuongTare = trongLuongTare;
                            item.MaMayTinhCan = mayCan.Id;
                            item.MaSize = maSize;
                            item.MaUserCan = mayCan.Id;
                            item.ThoiGianCan = _ngayGio;
                            item.Ngay = _ngayGio.Date;
                            item.NhaCC = mayCan?.MaNhaCungCap ?? "";
                            item.MaPhuongTien = maPhuongTien;
                            item.MaAo =maAo;
                            var rl = mainService.VmPhieuCanNguyenLieu.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);
                        }

                        break;
                    }
                    case AppKV.TPFillet:
                    {
                        var phieuCan = mainService.VmPhieuCanTPFillet.Get<object>(id);
                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanTPFillet.CreateDefaultNew();
                            item.Id = id;
                            item.MSL = maLo;
                            item.MaLoaiThanhPham = maThanhPham;
                            item.TrongLuong = trongLuongNhan;
                            item.MaXuongSanXuat = mayCan.MaXuong;
                            item.TrongLuongTare = trongLuongTare;
                            item.MaMayTinhCan = mayCan.Id;
                            item.MaSize = maSize;
                            item.MaUserCan = mayCan.Id;
                            item.ThoiGianCan = _ngayGio.TimeOfDay;
                            item.Ngay = _ngayGio.Date;
                            item.MaLoaiCa = mayCan.MaLoaiCa;
                            item.MaMau = mayCan.MaMau;
                            item.MaNhanVien = mayCan.MaNhanVien;
                            item.HoVaTen = "";
                            item.MaTheTu = theId;
                            item.SuDung = true;
                            item.LoaiCan = "TP";
                            var rl = mainService.VmPhieuCanTPFillet.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);
                        }

                        break;
                    }
                    case AppKV.PhuPham:
                    {
                        var phieuCan = mainService.VmPhieuCanPhuPham.Get<object>(id);
                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanPhuPham.CreateDefaultNew();
                            item.Id = id;
                            item.MSL = maLo;
                            item.MaLoaiThanhPham = maThanhPham;
                            item.MaLoaiCa = "";
                            item.MaSize = maSize;
                            item.MaMau = "";
                            item.MaXuongSanXuat = mayCan.MaXuong;
                            item.TrongLuong = trongLuongNhan;
                            item.TrongLuongTare = trongLuongTare;
                            item.MaMayTinhCan = mayCan.Id;
                            item.MaUserCan = mayCan.Id;
                            item.ThoiGianCan = _ngayGio;
                            item.NgayCan = _ngayGio;
                            item.Ngay = _ngayGio.Date;
                            item.NhaMuaHang = "";
                            item.MaPhuongTien = "";
                            item.SuDung = true;
                            var rl = mainService.VmPhieuCanPhuPham.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);

                        }

                        break;
                    }
                    case AppKV.PhuPhamv2:
                    {
                        var phieuCan = mainService.VmPhieuCanPhuPhamv2.Get<object>(id);

                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanPhuPhamv2.CreateDefaultNew();
                            item.Id = id;
                            item.Gio = _ngayGio.TimeOfDay;
                            item.MaLo = maLo;
                            item.MaMayCan = mayCan.Id;
                            item.MaNhanVien = maNhanVien;
                            item.MaThanhPham = maThanhPham;
                            item.MaThe = theId;
                            item.MaXuong = mayCan.MaXuong;
                            item.Ngay = _ngayGio.Date;
                            item.STT = stt;
                            item.TrongLuong = trongLuongNhan;
                            item.SuDung = true;
                            var rl = mainService.VmPhieuCanPhuPhamv2.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);
                        }

                        break;
                    }
                    case AppKV.XepKhuon:
                    {
                        var phieuCan = mainService.VmPhieuCanChinhXepKhuon.Get<object>(id);
                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanChinhXepKhuon.CreateDefaultNew();
                            item.Id = id;
                            item.Gio = _ngayGio.TimeOfDay;
                            item.MaLo = maLo;
                            item.MaMayCan = mayCan.Id;
                            item.MaNhanVien = maNhanVien;
                            item.MaXuong = mayCan.MaXuong;
                            item.Ngay = _ngayGio.Date;
                            item.STT = stt;
                            item.TrongLuong = trongLuongNhan;
                            item.ChuyenXuong = false;
                            item.DaQuay = false;
                            item.Forced = false;
                            item.GhiChu = "";
                            item.MaKhuVuc = "";
                            item.MaLoaiCa = "";
                            item.MaMau = "";
                            item.MaNhom = "";
                            item.MaChatLuong =maChatLuong;
                            item.MaChieuXa = maChieuXa;
                            item.IdMonitor = "";
                            item.LuotQuay = 0;
                            item.MaCoiChinh = maCoi;
                            item.MaCoiTam = "";
                            item.MaNhanVienPvPhanCo = "";
                            item.MaSizeChinh = maSize;
                            item.MaThanhPhamChinh = maThanhPham;
                            item.MaUserCan = mayCan.Id;
                            item.TaiChe = false;
                            item.MayQuay = "";
                            item.TrongLuong = trongLuongNhan;
                            item.TrongLuongTare = trongLuongTare;
                            
                            var rl = mainService.VmPhieuCanChinhXepKhuon.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);
                        }

                        break;
                    }
                    case AppKV.XepKhuonRaCoi:
                    {
                        var phieuCan = mainService.VmPhieuCanRaCoi.Get<object>(id);
                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanRaCoi.CreateDefaultNew();
                            item.Id = id;
                            item.Gio = _ngayGio.TimeOfDay;
                            item.MaLo = maLo;
                            item.MaNhanVien = maNhanVien;
                            item.MaXuong = mayCan.MaXuong;
                            item.Ngay = _ngayGio.Date;
                            item.STT = stt;
                            item.TrongLuong = trongLuongNhan;
                            item.GhiChu = "";
                            item.MaChieuXa = maChieuXa;
                            item.IdMonitor = "";
                            item.TrongLuong = trongLuongNhan;
                            item.TrongLuongTare = trongLuongTare;
                            item.MayCan = mayCan.Id;
                            item.MaChatLuong = maChatLuong;
                            item.MaCoi = maCoi;
                            item.MaSize = maSize;
                            item.MaThanhPham = maThanhPham;
                            item.MaThe = theId;
                            item.NgayNguyenLieu = item.Ngay;
                            
                            
                            var rl = mainService.VmPhieuCanChinhXepKhuon.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);
                        }

                        break;
                    }
                    case AppKV.XepKhuonPhu:
                    {
                        var phieuCan = mainService.VmPhieuCanPhuXepKhuon.Get<object>(id);
                        if (phieuCan == null)
                        {
                            var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var item = mainService.VmPhieuCanPhuXepKhuon.CreateDefaultNew();
                            item.Id = id;
                            item.Gio = _ngayGio.TimeOfDay;
                            item.MaLo = maLo;
                            item.MaLoaiCa = "";
                            item.MaMau = "";
                            item.MaKhuVuc = "";
                            item.MaNhanVien = maNhanVien;
                            item.MaNhom = "";
                            item.MaUserCan = mayCan.Id;
                            item.MaXuong = mayCan.MaXuong;
                            item.MaMayCan = mayCan.Id;
                            item.TrongLuong = trongLuongNhan;
                            item.GhiChu = "";
                            item.MaSize = maSize;
                            item.MaThanhPham = maThanhPham;
                            item.Ngay = _ngayGio.Date;
                            item.STT = stt;
                            item.TrongLuongTare = trongLuongTare;
                            item.MaChatLuong = maChatLuong;
                            item.MaChieuXa = maChieuXa;
                            var rl = mainService.VmPhieuCanPhuXepKhuon.Insert(item);
                            if (rl == 0) throw new Exception("Insert failed");
                            if (mayCan != null) mayCansService.AddItem(mayCan, item);
                        }

                        break;
                    }

                }
            }

            if (mayCan != null)
            {
                mayCan.TrongLuong = trongLuongNhan;
                mayCan.IsStated = true;
                mayCan.TheView = theId;
                mayCan.ThongBao = "Đã Hoàn Thành - " + ngayGio;
            }

            mayCansService.NotifyItemChanged(mayCan);
        });
        if (hqResponse != null) return hqResponse;
        if (!result.IsSuccess)
        {
            Console.WriteLine($"[HTTP TICKET ERROR] {result.Error}");
            //switch (errorCode)
            //{
            //    case "EXITS":
            //        return Conflict(new { message = "Dữ liệu đã tồn tại." });
            //        break;
            //    default:
                    return BadRequest(new { message = hqWeightError ?? $"Error: {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
            //} 
            
        }
            
        // Process the received data
        return Ok(new { message = $"Data received: {DateTime.Now.ToString("yyyyMMddHHmmss")}" });//Ok(new { Status = $"Data received {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
    }

    [HttpPost]
    public IActionResult PostSilent([FromBody] object yourData)
    {
        try
        {
            var json = JObject.Parse(yourData?.ToString() ?? "");
            if (json["MayCan"]?.Type != JTokenType.String || string.IsNullOrWhiteSpace((string?)json["MayCan"]))
                return BadRequest(new { Status = "Thiếu hoặc sai mã máy backup." });
            var machine = mayCansService.Find((string?)json["MayCan"] ?? "");
            if (machine?.WKv != AppKV.Hq)
                return BadRequest(new { Status = "Máy backup không phải máy HQ đã đăng ký." });
            return SaveHqHttp(json, machine, true);
        }
        catch (Newtonsoft.Json.JsonException)
        {
            return BadRequest(new { Status = "JSON phiếu backup không hợp lệ." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HQ HTTP BACKUP ROUTE ERROR] {ex}");
            return StatusCode(503, new { Status = "Chưa xác nhận lưu backup. Kiểm tra log Hub." });
        }
    }

    // Overridable only for isolated database tests; production still uses normal server configuration.
    protected virtual dbPMScontext CreateHqBackupDb() => new();

    private IActionResult SaveHqHttp(JObject json, MayCan machine, bool silent)
    {
        string? ticketId = null;
        IActionResult Error(int status, string message, string saveStatus) =>
            StatusCode(status, silent ? (object)new { Status = message, SaveStatus = saveStatus }
                                     : new { message, SaveStatus = saveStatus });
        try
        {
            var requested = HqHttpBackupWriter.Parse(json, machine.Id);
            ticketId = requested.Id;
            HqHttpBackupWriter.Result saved;
            using (var db = CreateHqBackupDb()) saved = HqHttpBackupWriter.Save(db, requested);
            // UI failure after COMMIT must not turn a stored ticket into HTTP 400.
            try
            {
                if (saved.Inserted && !silent) mayCansService.AddItem(machine, saved.Item);
                machine.TrongLuong = saved.Item.TrongLuong;
                machine.TheView = saved.Item.TheIdNhanVien;
                machine.ThongBao = "Đã Hoàn Thành - " + saved.Item.NgayGio.ToString("yyyyMMddHHmmss");
                mayCansService.NotifyItemChanged(machine);
            }
            catch (Exception ex) { Console.WriteLine($"[HQ HTTP BACKUP UI ERROR] Id={saved.Item.Id}; {ex}"); }
            return Ok(silent
                ? (object)new { Status = $"Data received {DateTime.Now:yyyyMMddHHmmss}", Id = saved.Item.Id, duplicated = !saved.Inserted }
                : new { message = $"Data received: {DateTime.Now:yyyyMMddHHmmss}", Id = saved.Item.Id, duplicated = !saved.Inserted });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[HQ HTTP BACKUP REJECTED] Machine={machine.Id}; Id={ticketId}; {ex.Message}");
            return Error(400, ex.Message, HqOnlineResponse.NotSaved);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HQ HTTP BACKUP ERROR] Machine={machine.Id}; Id={ticketId}; {ex}");
            return Error(503, "Chưa xác nhận lưu backup. Kiểm tra log Hub.", HqOnlineResponse.Unknown);
        }
    }
}
