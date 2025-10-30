using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppViewModels;
using ViewModels.Repos.HQ;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanTPFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanTPFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhieuCanTPFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanTPFillet>>> GetPhieuCanTPFillet()
        {
            if (_context.PhieuCanTPFillet == null)
            {
                return NotFound();
            }
            return await _context.PhieuCanTPFillet.ToListAsync();
        }

        // GET: api/PhieuCanTPFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanTPFillet>> GetPhieuCanTPFillet(string id)
        {
            if (_context.PhieuCanTPFillet == null)
            {
                return NotFound();
            }
            var phieuCanTPFillet = await _context.PhieuCanTPFillet.FindAsync(id);

            if (phieuCanTPFillet == null)
            {
                return NotFound();
            }

            return phieuCanTPFillet;
        }

        // PUT: api/PhieuCanTPFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanTPFillet(string id, PhieuCanTPFillet phieuCanTPFillet)
        {
            if (id != phieuCanTPFillet.MaMayTinhCan)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanTPFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanTPFilletExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PhieuCanTPFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanTPFillet>> PostPhieuCanTPFillet(PhieuCanTPFillet phieuCanTPFillet)
        {
            if (_context.PhieuCanTPFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.PhieuCanTPFillet'  is null.");
            }
            _context.PhieuCanTPFillet.Add(phieuCanTPFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanTPFilletExists(phieuCanTPFillet.MaMayTinhCan))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanTPFillet", new { id = phieuCanTPFillet.MaMayTinhCan }, phieuCanTPFillet);
        }

        // DELETE: api/PhieuCanTPFillets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanTPFillet(string id)
        {
            if (_context.PhieuCanTPFillet == null)
            {
                return NotFound();
            }
            var phieuCanTPFillet = await _context.PhieuCanTPFillet.FindAsync(id);
            if (phieuCanTPFillet == null)
            {
                return NotFound();
            }

            _context.PhieuCanTPFillet.Remove(phieuCanTPFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanTPFilletExists(string id)
        {
            return (_context.PhieuCanTPFillet?.Any(e => e.MaMayTinhCan == id)).GetValueOrDefault();
        }

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanChiTiets(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFillet.GetPhieuCanChiTiets<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopNhanViens(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFillet.GetPhieuCanTongHopNhanViens<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTonghopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFillet.GetPhieuCanTonghopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }
        #region tính lương
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadTongHopTinhLuong(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.ReloadTinhLuong(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> KetChuyenTinhLuong(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {

                string listPhieuCanTongHopTinhLuongSelectedItems = dataT.Item1;
                string[] phieuCanTongHopTinhLuongs = listPhieuCanTongHopTinhLuongSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var phieuCanTongHopTinhLuongSelectedItems = new List<BravoModelV1.Model.PhieuLuongFillet>();
                foreach (var phieuCanTongHopTinhLuongSelectedItem in phieuCanTongHopTinhLuongs)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = phieuCanTongHopTinhLuongSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var phieuCanTongHopTinhLuong = JsonSerializer.Deserialize<BravoModelV1.Model.PhieuLuongFillet>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (phieuCanTongHopTinhLuong != null)
                    {
                        phieuCanTongHopTinhLuongSelectedItems.Add((BravoModelV1.Model.PhieuLuongFillet)phieuCanTongHopTinhLuong);
                    }
                }





                var items = new List<BravoModelV1.Model.PhieuLuongFillet>(); /*PhieuTinhLuongs.Where(x => x.BravoId != null && x.BravoId.Trim() != "").ToList();*/
                foreach (var item in phieuCanTongHopTinhLuongSelectedItems)
                {
                    var phieuCan = (BravoModelV1.Model.PhieuLuongFillet)item;
                    if (phieuCan.BravoId != null && phieuCan.BravoId.Trim() != string.Empty) items.Add(phieuCan);
                }


                //AppViewModel.Ins.IsBusy = true;
                var isF = false;
                //foreach(var phieuCanDinhHinh in itemsP)
                //{
                //    var ps = items.Where(x => x.MaNhanVien == phieuCanDinhHinh.MaNhanVien);
                //    if(ps.Any())
                //    {
                //        isF = true;
                //        break;
                //    }
                //}
                var sanPhamIds = items.Select(x => x.BravoId).Distinct().ToList();
                if (sanPhamIds != null)
                    foreach (var item in sanPhamIds)
                    {
                        var log = Vm.VmLogKetChuyen
                            .Get(date1, xuongId, item, 9);
                        if (log != null)
                        {
                            isF = true;
                            break;
                        }
                    }

                if (isF == false)
                {
                    var rows = 0;
                    await Task.Delay(1000);
                    await Task.Run(
                        () =>
                        {
                            rows = Vm.VmBV_PhieuCanFillet.Insert(items);
                            foreach (var item in sanPhamIds)
                                Vm.VmLogKetChuyen
                                    .Insert(
                                        new LogKetChuyenBravo
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1,
                                            tab = 9,
                                            NgayChuyen = DateTime.Now
                                        });
                        });
                    await Task.Delay(1000);
                    //MessageBox.Show($@"Đã thực hiện trên {rows} dòng dữ liêu");
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");

                    await TPSoftAction_Fillet(dateTime,xuongId);
                }
                else
                {
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thao tác đã thực hiện, Không thể thực hiện lại!");
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Lỗi" + ex.ToString());
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                });
            }

        }
        [HttpPost("{dateTime}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> TPSoftAction_Fillet(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            // Tạo một tham chiếu đến SignalR Hub
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            try
            {

                await hubContext.Clients.All.SendAsync("ReceiveProgress", "Khởi tạo TPSoft");
                await Task.Delay(50).ConfigureAwait(false);
                var phieuCans = PhieuCanTPFilletViewModel.Instance
                    .GetsTPSoft<BravoModelV1.EF.PMS_DataRecord>(date1, xuongId);
                var phieuCanTPSoftServer = Vm.VmPMS_Record.Gets<BravoModelV1.EF.PMS_DataRecord>(date1, "09", xuongId);
                var idTPSofts = phieuCanTPSoftServer.Select(x => x.ID)
                    .DefaultIfEmpty(string.Empty)
                    .Distinct()
                    .ToList();
                var insItems = phieuCans.Where(x => !idTPSofts.Contains(x.ID)).ToList();
                var _udaItems = phieuCans.Where(x => idTPSofts.Contains(x.ID)).ToList();

                var udaItems = (from _ups in _udaItems
                                from tpS in phieuCanTPSoftServer
                                where _ups.ID == tpS.ID
                                select new BravoModelV1.EF.PMS_DataRecord
                                {
                                    CMND = _ups.CMND,
                                    ID = _ups.ID,
                                    CongDoanID = _ups.CongDoanID,
                                    DateCreate = tpS.DateCreate,
                                    DateSync = _ups.DateSync,
                                    Status = _ups.Status,
                                    ThoiGian = _ups.ThoiGian,
                                    TrongLuong = _ups.TrongLuong
                                }).ToList();
                udaItems.All(
                    x =>
                    {
                        x.DateSync = new DateTime(1900, 01, 01, 0, 0, 0);
                        return true;
                    });
                insItems.All(
                    x =>
                    {
                        x.DateCreate = DateTime.Now;
                        return true;
                    });
                if (insItems.Any())
                {
                    var batches = Vm.VmPMS_Record.GetSqlsInBatches(insItems);
                    var rows = 0;
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", "Bắt đầu thêm");
                    await Task.Delay(50).ConfigureAwait(false);
                    foreach (var batche in batches)
                    {
                        rows += Vm.VmPMS_Record.Execute(batche);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Đã thực hiện {rows}/{insItems.Count}");
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }

                if (udaItems.Any())
                {
                    var batches = Vm.VmPMS_Record.GetSqlsInBatches(udaItems);
                    var rows = 0;
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", "Bắt đầu cập nhật");
                    await Task.Delay(50).ConfigureAwait(false);
                    Vm.VmPMS_Record.Delete(udaItems.Select(x => x.ID).ToList());
                    foreach (var batche in batches)
                    {
                        rows += Vm.VmPMS_Record.Execute(batche);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Đã thực hiện {rows}/{udaItems.Count}");
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }

                await hubContext.Clients.All.SendAsync("ReceiveProgress", "Thực hiện xong!");
                await Task.Delay(500).ConfigureAwait(false);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception exception)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Có lỗi xảy ra: {exception.Message}");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { exception.Message }
                });
            }

        }
        #endregion
    }
}
