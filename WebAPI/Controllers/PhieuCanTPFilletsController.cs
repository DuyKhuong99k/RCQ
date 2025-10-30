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


        

        #region Xẻ Bướm


        #region BTP
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietBTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetChiTietBTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamBTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamBTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLTPSBTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopLTPSBTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVienBTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopNhanVienBTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        #endregion
        #region TP
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetChiTietTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLTPSTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopLTPSTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVienTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopNhanVienTPXeBuoms<object>(from, to, xuongId);
            return items;
        }

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopDinhMucTPXeBuoms(string fromDate, string toDate, string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopDinhMucTPXeBuoms<object>(from, to, xuongId);
            return items;
        }
        #endregion
        
        #endregion

        #region Dashboard
        [HttpGet("{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamBTPXeBuomsDB_St(string xuongId)
        {
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamBTPXeBuom.OfType<dynamic>().Where(x => x.MaXuongSanXuat == xuongId).ToList();
            return items;
        }

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamBTPXeBuomsDB(string fromDate,string toDate,string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamBTPXeBuoms<object>(date1, date2, xuongId);
            return items;
        }



        [HttpGet("{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamTPXeBuomsDB_St(string xuongId)
        {
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamTPXeBuom.OfType<dynamic>().Where(x => x.MaXuongSanXuat == xuongId).ToList();
            return items;
        }

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamTPXeBuomsDB(string fromDate,string toDate,string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamTPXeBuoms<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamTPXeBuomsDB2(string fromDate, string toDate, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamTPXeBuoms2<object>(date1, date2, xuongId);
            return items;
        }
        #endregion
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

                    await TPSoftAction_Fillet(dateTime, xuongId);
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

        #region XLPC
        #region BTP XẺ BƯỚM
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanBTPXeBuom_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPFillet == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetPhieuCanBTPXeBuom_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanRaCois.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertBTPXeBuom(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            //kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            if (_context.PhieuCanTPFillet.Any(u => u.MaMayTinhCan == model.MaMayTinhCan && u.MaUserCan == model.MaUserCan && u.ThoiGianCan == model.ThoiGianCan && u.Ngay == model.Ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanTPFillet
            {
                MaMayTinhCan = model.MaMayTinhCan,
                MaUserCan = model.MaUserCan,
                ThoiGianCan = model.ThoiGianCan,
                Ngay = model.Ngay,
                MaXuongSanXuat = model.MaXuongSanXuat,
                MSL = model.MSL,
                MaLoaiCa = model.MaLoaiCa,
                MaLoaiThanhPham = model.MaLoaiThanhPham,
                MaSize = model.MaSize,
                MaMau = model.MaMau,
                //MaNhanVien = "",
                //HoVaTen = "",
                MaTheTu = model.MaTheTu,
                TrongLuong = model.TrongLuong,
                //SuDung = true,
                GhiChu = model.GhiChu,
                //MaNhanVienPhucVu = "",
                //LoaiCan = "",
                TrongLuongTare = model.TrongLuongTare,
                Id = model.Id
            };
            _context.PhieuCanTPFillet.Add(newItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu." + ex.Message.ToString(),
                    Errors = new List<string> { ex.Message.ToString() }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm thành công!"
            });
        }
        [HttpGet("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public IActionResult GetsByMaBTPXeBuom(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngay)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan time = TimeSpan.Parse(thoiGianCan);
            var item = _context.PhieuCanTPFillet.FirstOrDefault(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == time && x.Ngay == dateTime);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại!."
                });
            }
            return Ok(item);
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{gio}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> UpdateBTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan gio, string ngay, Tuple<string> dataT)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == gio && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MSL,
                item.MaLoaiCa,
                item.MaLoaiThanhPham,
                item.MaSize,
                item.MaMau,

            }, options);
            item.MSL = model.MSL;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaLoaiThanhPham = model.MaLoaiThanhPham;
            item.MaSize = model.MaSize;
            item.MaMau = model.MaMau;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message
}
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> DeleteBTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {

            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var oldData = JsonSerializer.Serialize(new
            {
                item.TrongLuong,
                item.TrongLuongTare,
            }, options);

            var newItem = new PhieuCanTPFillet
            {
                MaMayTinhCan = item.MaMayTinhCan,
                MaUserCan = item.MaUserCan,
                ThoiGianCan = item.ThoiGianCan,
                Ngay = item.Ngay,
                MaXuongSanXuat = item.MaXuongSanXuat,
                MSL = item.MSL,
                MaLoaiCa = item.MaLoaiCa,
                MaLoaiThanhPham = item.MaLoaiThanhPham,
                MaSize = item.MaSize,
                MaMau = item.MaMau,
                //MaNhanVien = "",
                //HoVaTen = "",
                MaTheTu = item.MaTheTu,
                TrongLuong = model.TrongLuong,
                //SuDung = true,
                //MaNhanVienPhucVu = "",
                //LoaiCan = "",
                TrongLuongTare = model.TrongLuongTare,
                //Id = ""
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanTPFillet.Remove(item);
                    _context.PhieuCanTPFillet.Add(newItem);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong_BTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaXuongSanXuat
            }, options);
            item.MaXuongSanXuat = model.MaXuongSanXuat;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize_BTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaSize
            }, options);
            item.MaSize = model.MaSize;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham_BTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaLoaiThanhPham
            }, options);
            item.MaLoaiThanhPham = model.MaLoaiThanhPham;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenLo_BTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MSL
            }, options);
            item.MSL = model.MSL;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenMau_BTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaMau
            }, options);
            item.MaMau = model.MaMau;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        #endregion
        #region TP XẺ BƯỚM
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTPXeBuom_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPFillet == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFillet.GetPhieuCanTPXeBuom_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong_TPXeBuom(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanRaCois.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertTPXeBuom(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            //kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            if (_context.PhieuCanTPFillet.Any(u => u.MaMayTinhCan == model.MaMayTinhCan && u.MaUserCan == model.MaUserCan && u.ThoiGianCan == model.ThoiGianCan && u.Ngay == model.Ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanTPFillet
            {
                MaMayTinhCan = model.MaMayTinhCan,
                MaUserCan = model.MaUserCan,
                ThoiGianCan = model.ThoiGianCan,
                Ngay = model.Ngay,
                MaXuongSanXuat = model.MaXuongSanXuat,
                MSL = model.MSL,
                MaLoaiCa = model.MaLoaiCa,
                MaLoaiThanhPham = model.MaLoaiThanhPham,
                MaSize = model.MaSize,
                MaMau = model.MaMau,
                MaNhanVien = model.MaNhanVien,
                //HoVaTen = "",
                MaTheTu = model.MaTheTu,
                TrongLuong = model.TrongLuong,
                //SuDung = true,
                GhiChu = model.GhiChu,
                //MaNhanVienPhucVu = "",
                //LoaiCan = "",
                TrongLuongTare = model.TrongLuongTare,
                Id = model.Id
            };
            _context.PhieuCanTPFillet.Add(newItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu." + ex.Message.ToString(),
                    Errors = new List<string> { ex.Message.ToString() }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm thành công!"
            });
        }
        [HttpGet("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public IActionResult GetsByMaTPXeBuom(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngay)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan time = TimeSpan.Parse(thoiGianCan);
            var item = _context.PhieuCanTPFillet.FirstOrDefault(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == time && x.Ngay == dateTime);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại!."
                });
            }
            return Ok(item);
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{gio}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> UpdateTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan gio, string ngay, Tuple<string> dataT)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == gio && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MSL,
                item.MaLoaiCa,
                item.MaLoaiThanhPham,
                item.MaSize,
                item.MaMau,
                item.MaNhanVien
            }, options);
            item.MSL = model.MSL;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaLoaiThanhPham = model.MaLoaiThanhPham;
            item.MaSize = model.MaSize;
            item.MaMau = model.MaMau;
            item.MaNhanVien = model.MaNhanVien;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message
}
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> DeleteTPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {

            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var oldData = JsonSerializer.Serialize(new
            {
                item.TrongLuong,
                item.TrongLuongTare,
            }, options);

            var newItem = new PhieuCanTPFillet
            {
                MaMayTinhCan = item.MaMayTinhCan,
                MaUserCan = item.MaUserCan,
                ThoiGianCan = item.ThoiGianCan,
                Ngay = item.Ngay,
                MaXuongSanXuat = item.MaXuongSanXuat,
                MSL = item.MSL,
                MaLoaiCa = item.MaLoaiCa,
                MaLoaiThanhPham = item.MaLoaiThanhPham,
                MaSize = item.MaSize,
                MaMau = item.MaMau,
                MaNhanVien = item.MaNhanVien,
                //HoVaTen = "",
                MaTheTu = item.MaTheTu,
                TrongLuong = model.TrongLuong,
                //SuDung = true,
                //MaNhanVienPhucVu = "",
                //LoaiCan = "",
                TrongLuongTare = model.TrongLuongTare,
                //Id = ""
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanTPFillet.Remove(item);
                    _context.PhieuCanTPFillet.Add(newItem);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong_TPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaXuongSanXuat
            }, options);
            item.MaXuongSanXuat = model.MaXuongSanXuat;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize_TPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaSize
            }, options);
            item.MaSize = model.MaSize;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham_TPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaLoaiThanhPham
            }, options);
            item.MaLoaiThanhPham = model.MaLoaiThanhPham;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenLo_TPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MSL
            }, options);
            item.MSL = model.MSL;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngay}")]
        [Authorize]
        public async Task<IActionResult> ChuyenMau_TPXeBuom(string maMayTinhCan, string maUserCan, TimeSpan thoiGianCan, string ngay, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngay))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFillet.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCan && x.Ngay == dateTime);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaMau
            }, options);
            item.MaMau = model.MaMau;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        #endregion
        #endregion
    }
}
