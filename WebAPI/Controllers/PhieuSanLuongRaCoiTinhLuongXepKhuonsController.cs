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
    public class PhieuSanLuongRaCoiTinhLuongXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuSanLuongRaCoiTinhLuongXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhieuSanLuongRaCoiTinhLuongXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuSanLuongRaCoiTinhLuongXepKhuon>>> Gets()
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            return await _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.ToListAsync();
        }

        // GET: api/PhieuSanLuongRaCoiTinhLuongXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuSanLuongRaCoiTinhLuongXepKhuon>> Get(int id)
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            var phieuSanLuongRaCoiTinhLuongXepKhuon = await _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.FindAsync(id);

            if (phieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }

            return phieuSanLuongRaCoiTinhLuongXepKhuon;
        }

        // PUT: api/PhieuSanLuongRaCoiTinhLuongXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PhieuSanLuongRaCoiTinhLuongXepKhuon phieuSanLuongRaCoiTinhLuongXepKhuon)
        {
            if (id != phieuSanLuongRaCoiTinhLuongXepKhuon.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuSanLuongRaCoiTinhLuongXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuSanLuongRaCoiTinhLuongXepKhuonExists(id))
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

        // POST: api/PhieuSanLuongRaCoiTinhLuongXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuSanLuongRaCoiTinhLuongXepKhuon>> Post(PhieuSanLuongRaCoiTinhLuongXepKhuon phieuSanLuongRaCoiTinhLuongXepKhuon)
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.PhieuSanLuongRaCoiTinhLuongXepKhuon'  is null.");
            }
            _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.Add(phieuSanLuongRaCoiTinhLuongXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuSanLuongRaCoiTinhLuongXepKhuonExists(phieuSanLuongRaCoiTinhLuongXepKhuon.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuSanLuongRaCoiTinhLuongXepKhuon", new { id = phieuSanLuongRaCoiTinhLuongXepKhuon.STT }, phieuSanLuongRaCoiTinhLuongXepKhuon);
        }

        // DELETE: api/PhieuSanLuongRaCoiTinhLuongXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            var phieuSanLuongRaCoiTinhLuongXepKhuon = await _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.FindAsync(id);
            if (phieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }

            _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.Remove(phieuSanLuongRaCoiTinhLuongXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuSanLuongRaCoiTinhLuongXepKhuonExists(int id)
        {
            return (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon?.Any(e => e.STT == id)).GetValueOrDefault();
        }
        #region Tính Lương
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadTongHopTinhLuong(string dateTime, string xuongId)
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadSanLuongXepKhuon(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> KetChuyenTinhLuongSanLuongXepKhuon(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                string listPhieuCanTongHopTinhLuongSelectedItems = dataT.Item1;
                string[] phieuCanTongHopTinhLuongs = listPhieuCanTongHopTinhLuongSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var phieuCanTongHopTinhLuongSelectedItems = new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>();
                foreach (var phieuCanTongHopTinhLuongSelectedItem in phieuCanTongHopTinhLuongs)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = phieuCanTongHopTinhLuongSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var phieuCanTongHopTinhLuong = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (phieuCanTongHopTinhLuong != null)
                    {
                        phieuCanTongHopTinhLuongSelectedItems.Add((BravoModelV1.Model.SanLuongTinhLuongXepKhuon)phieuCanTongHopTinhLuong);
                    }
                }
                var items = phieuCanTongHopTinhLuongSelectedItems.Cast<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>().ToList();
                var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>(Vm.VmPhieuCanTPDinhHinh.GetPhieuCanTinhLuongs(items.ToList(), 7, date1));
                var ids = phieuCans.Select(x => x.MaSanPham).Distinct().ToList();

                var isF = false;
                if (ids != null)
                {
                    foreach (var item in ids)
                    {
                        var log = LogKetChuyenViewModel.Instance
                            .Get(date1, xuongId, item, 7);
                        if (log != null)
                        {
                            isF = true;
                            break;
                        }
                    }
                }
                if (isF == false)
                {
                    var rows = 0;
                    await Task.Delay(1000);
                    await Task.Run(
                        () =>
                        {
                            rows = Vm.VmPhieuCanTPDinhHinh.Insert(phieuCans);
                            foreach (var item in ids)
                                Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1,
                                            tab = 7,
                                            NgayChuyen = DateTime.Now
                                        });
                        });
                    await Task.Delay(1000);
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");

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
        #region trực tiếp
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadTongHopTinhLuongTrucTiep(string dateTime, string xuongId)
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.CommandReloadTrucTiep(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> KetChuyenTrucTiep(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            try
            {
                string listSanLuongTrucTiepSelectedItems = dataT.Item1;
                string[] sanLuongTrucTieps = listSanLuongTrucTiepSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var sanLuongTrucTiepSelectedItems = new List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>();
                foreach (var sanLuongTrucTiepSelectedItem in sanLuongTrucTieps)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = sanLuongTrucTiepSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var sanLuongTrucTiep = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongTinhLuongXepKhuon>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (sanLuongTrucTiep != null)
                    {
                        sanLuongTrucTiepSelectedItems.Add((BravoModelV1.Model.SanLuongTinhLuongXepKhuon)sanLuongTrucTiep);
                    }
                }
                var items = sanLuongTrucTiepSelectedItems.Cast<BravoModelV1.Model.PhieuCanKiemDinhHinh>().ToList();
                var ids = items.Select(x => x.MaSanPham).Distinct().ToList();
                bool isF = false;
                if (ids != null)
                {

                    foreach (var item in ids)
                    {
                        var log = LogKetChuyenViewModel.Instance
                            .Get(date1, xuongId, item, 8);
                        if (log != null)
                        {
                            isF = true;
                            break;
                        }
                    }
                }

                if (isF == false)
                {
                    var rows = 0;
                    await Task.Delay(1000);
                    await Task.Run(
                        () =>
                        {
                            rows = Vm.VmPhieuCanTPDinhHinh.Insert(items.ToList());
                            foreach (var item in ids)
                                Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1,
                                            tab = 8,
                                            NgayChuyen = DateTime.Now
                                        });
                        });
                    await Task.Delay(1000);
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");

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
        #endregion
        #region cài đặt sản lượng nhân viên theo lượt ra cối
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetAlls(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuSanLuong.Gets(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsFullField(string dateTime, string xuongId)
        {
            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmPhieuSanLuong.GetsFullField<object>(ngayConvert, xuongId);
            return Ok(items);
        }

        [HttpPost("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAlls(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(dateTime) || string.IsNullOrEmpty(xuongId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn đầy đủ thông tin!."
                });
            }


            if (Vm.VmPhieuSanLuong.Delete(date1, xuongId) > 0)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Đã thực hiện!"
                });
            }
            else
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Không Thể Thực Hiện.",

                });
            }
        }
        [HttpGet("{stt}/{ngay}/{maXuong}/{maMayCan}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maXuong, string maMayCan)
        {
            //DateTime ngayConvert = DateTime.Parse(ngay);
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaXuong == maMayCan);
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
        [HttpPost("{stt}/{ngay}/{maXuong}/{maMayCan}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maXuong, string maMayCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuSanLuongRaCoiTinhLuongXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maMayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuSanLuongRaCoiTinhLuongXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaXuong == maMayCan);

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
            item.MaNhom = model.MaNhom;
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
