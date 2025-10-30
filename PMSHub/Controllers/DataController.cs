using System.Globalization;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DataController(IMayCansService mayCansService, IMainService mainService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        //var data = new { Message = "" };
        return Ok();
    }

    [HttpPost]
    public IActionResult Post([FromBody] object yourData)
    {
        var result = ErrorHandler.Handle(() =>
        {
            //"{\"Id\":\"%s\",\"STT\":%d,\"NgayGio\":\"%s\",\"MayCan\":\"%s\",\"MaLo\":\"%s\","
            //"\"MaSize\":\"%s\",\"MaThanhPham\":\"%s\",\"MaNhanVien\":\"%s\","
            //"\"TrongLuongNhan\":\"%s\",\"TrongLuongTra\":\"%s\",\"TrongLuongTare\":\"%s\","
            //"\"TheId\":\"%s\",\"TheChucNang\":\"%s\",\"Ngay\":\"%s\",\"MaLoaiNguyenLieu\":\"%s\"}"
            var jsonString = yourData.ToString() ?? "";
            var jObject = JObject.Parse(jsonString);
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
            var trongLuongTare = (decimal)(jObject["TrongLuongTare"] ?? 0);
            var theId = (string)(jObject["TheId"] ?? "")!;
            var theChucNang = (string)(jObject["TheChucNang"] ?? "")!;
            var ngay = (string)(jObject["Ngay"] ?? "")!;
            var maLoaiNguyenLieu = (string)(jObject["MaLoaiNguyenLieu"] ?? "")!;
            //var mayCanId = (string)jObject["MayCan"];
            //var trongLuong = (decimal)jObject["TrongLuongNhan"];
            //var theId = (string)jObject["TheId"];
            //var ngayGio = (string)jObject["NgayGio"];
            var mayCan = mayCansService.Find(maycan ?? "");
            var phieuCan = mainService.VmPhieuCanHq.Get<object>(id);
            if (phieuCan == null)
            {
                var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var item = mainService.VmPhieuCanHq.CreateNew(id, stt, maLo, maLoaiNguyenLieu, maSize, maThanhPham,
                    maNhanVien, trongLuongNhan, trongLuongTra, trongLuongTare, theId, theChucNang, _ngayGio,
                    maycan ?? "", mayCan?.MaXuong ?? "");

                var rl = mainService.VmPhieuCanHq.Insert(item);
                if (rl == 0) throw new Exception("Insert failed");
                if (mayCan != null)
                {
                    mayCansService.AddItem(mayCan,item);
                }
                
            }


            if (mayCan != null)
            {
                mayCan.TrongLuong = trongLuongNhan;
                mayCan.IsStated = true;
                mayCan.TheView = theId;
                mayCan.ThongBao = "Đã Hoàn Thành - " + ngayGio;
                
            }

            mayCansService.NotifyItemChanged();
        });
        if (!result.IsSuccess)
            return BadRequest(new { Status = $"Error: {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
        // Process the received data
        return Ok(new { Status = $"Data received {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
    }

    [HttpPost]
    public IActionResult PostSilent([FromBody] object yourData)
    {
        var result = ErrorHandler.Handle(() =>
        {
            //"{\"Id\":\"%s\",\"STT\":%d,\"NgayGio\":\"%s\",\"MayCan\":\"%s\",\"MaLo\":\"%s\","
            //"\"MaSize\":\"%s\",\"MaThanhPham\":\"%s\",\"MaNhanVien\":\"%s\","
            //"\"TrongLuongNhan\":\"%s\",\"TrongLuongTra\":\"%s\",\"TrongLuongTare\":\"%s\","
            //"\"TheId\":\"%s\",\"TheChucNang\":\"%s\",\"Ngay\":\"%s\",\"MaLoaiNguyenLieu\":\"%s\"}"
            var jsonString = yourData.ToString() ?? "";
            var jObject = JObject.Parse(jsonString);
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
            var trongLuongTare = (decimal)(jObject["TrongLuongTare"] ?? 0);
            var theId = (string)(jObject["TheId"] ?? "")!;
            var theChucNang = (string)(jObject["TheChucNang"] ?? "")!;
            var ngay = (string)(jObject["Ngay"] ?? "")!;
            var maLoaiNguyenLieu = (string)(jObject["MaLoaiNguyenLieu"] ?? "")!;
            //var mayCanId = (string)jObject["MayCan"];
            //var trongLuong = (decimal)jObject["TrongLuongNhan"];
            //var theId = (string)jObject["TheId"];
            //var ngayGio = (string)jObject["NgayGio"];
            var mayCan = mayCansService.Find(maycan ?? "");
            var phieuCan = mainService.VmPhieuCanHq.Get<object>(id);
            if (phieuCan == null)
            {
                var _ngayGio = DateTime.ParseExact(ngayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var item = mainService.VmPhieuCanHq.CreateNew(id, stt, maLo, maLoaiNguyenLieu, maSize, maThanhPham,
                    maNhanVien, trongLuongNhan, trongLuongTra, trongLuongTare, theId, theChucNang, _ngayGio,
                    maycan ?? "", mayCan?.MaXuong ?? "");

                var rl = mainService.VmPhieuCanHq.Insert(item);
                if (rl == 0) throw new Exception("Insert failed");
            }


            if (mayCan != null)
            {
                mayCan.TrongLuong = trongLuongNhan;
                mayCan.IsStated = true;
                mayCan.TheView = theId;
                mayCan.ThongBao = "Đã Hoàn Thành - " + ngayGio;
            }

            mayCansService.NotifyItemChanged();
        });
        if (!result.IsSuccess)
            return BadRequest(new { Status = $"Error: {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
        // Process the received data
        return Ok(new { Status = $"Data received {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
    }
}