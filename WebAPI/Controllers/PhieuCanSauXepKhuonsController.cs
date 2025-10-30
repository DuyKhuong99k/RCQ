using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanSauXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanSauXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhieuCanSauXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanSauXepKhuon>>> GetPhieuCanSauXepKhuon()
        {
            if (_context.PhieuCanSauXepKhuon == null)
            {
                return NotFound();
            }
            return await _context.PhieuCanSauXepKhuon.ToListAsync();
        }

        // GET: api/PhieuCanSauXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanSauXepKhuon>> GetPhieuCanSauXepKhuon(int id)
        {
            if (_context.PhieuCanSauXepKhuon == null)
            {
                return NotFound();
            }
            var phieuCanSauXepKhuon = await _context.PhieuCanSauXepKhuon.FindAsync(id);

            if (phieuCanSauXepKhuon == null)
            {
                return NotFound();
            }

            return phieuCanSauXepKhuon;
        }

        // PUT: api/PhieuCanSauXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanSauXepKhuon(int id, PhieuCanSauXepKhuon phieuCanSauXepKhuon)
        {
            if (id != phieuCanSauXepKhuon.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanSauXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanSauXepKhuonExists(id))
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

        // POST: api/PhieuCanSauXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanSauXepKhuon>> PostPhieuCanSauXepKhuon(PhieuCanSauXepKhuon phieuCanSauXepKhuon)
        {
            if (_context.PhieuCanSauXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.PhieuCanSauXepKhuon'  is null.");
            }
            _context.PhieuCanSauXepKhuon.Add(phieuCanSauXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanSauXepKhuonExists(phieuCanSauXepKhuon.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanSauXepKhuon", new { id = phieuCanSauXepKhuon.STT }, phieuCanSauXepKhuon);
        }

        // DELETE: api/PhieuCanSauXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanSauXepKhuon(int id)
        {
            if (_context.PhieuCanSauXepKhuon == null)
            {
                return NotFound();
            }
            var phieuCanSauXepKhuon = await _context.PhieuCanSauXepKhuon.FindAsync(id);
            if (phieuCanSauXepKhuon == null)
            {
                return NotFound();
            }

            _context.PhieuCanSauXepKhuon.Remove(phieuCanSauXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanSauXepKhuonExists(int id)
        {
            return (_context.PhieuCanSauXepKhuon?.Any(e => e.STT == id)).GetValueOrDefault();
        }
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanSauXepKhuon == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanSauXepKhuon.GetPhieuCan_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanSauXepKhuon.Where(x => x.NgayNguyenLieu == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSauXepKhuon>(dataT.Item1);
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
            // ... kiểm tra mã nhân viên đã tồn tại chưa ...
            if (_context.PhieuCanSauXepKhuon.Any(u => u.STT == model.STT && u.NgayNguyenLieu == model.NgayNguyenLieu && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanSauXepKhuon
            {
                STT = model.STT,
                Ngay = model.Ngay,
                NgayNguyenLieu = model.NgayNguyenLieu,
                Gio = model.Gio,
                MaXuong = model.MaXuong,
                MaMayCan = model.MaMayCan,
                MaLo = model.MaLo,
                MaLoaiCa = model.MaLoaiCa,
                MaThanhPham = model.MaThanhPham,
                MaSize = model.MaSize,
                MaChieuXa = model.MaChieuXa,
                MaCoi = model.MaCoi,
                TrongLuong = model.TrongLuong,
                TrongLuongTare = model.TrongLuongTare,
                ChiTietLuotRaCoiId = model.ChiTietLuotRaCoiId,
                MaNhanVien = model.MaNhanVien,
                MaUserCan = model.MaUserCan,
                GhiChu = model.GhiChu,
                MaThe = model.MaThe,
                TyLeMaBang = model.TyLeMaBang,
                IsTam = model.IsTam
            };
            _context.PhieuCanSauXepKhuon.Add(newItem);
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
        [HttpGet("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maMayCan, string maXuong)
        {
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanSauXepKhuon.FirstOrDefault(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSauXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSauXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaLo,
                item.MaNhanVien,
                item.MaThanhPham,
                item.MaSize,
                item.MaCoi,
                item.MaLoaiCa,
                item.MaChieuXa,
                item.TyLeMaBang,
                item.IsTam
            }, options);

            item.MaLo = model.MaLo;
            item.MaNhanVien = model.MaNhanVien;
            item.MaThanhPham = model.MaThanhPham;
            item.MaSize = model.MaSize;
            item.MaCoi = model.MaCoi;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaChieuXa = model.MaChieuXa;
            item.TyLeMaBang = model.TyLeMaBang;
            item.IsTam = model.IsTam;
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSauXepKhuon>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSauXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.STT,
                item.TrongLuong,
                item.TrongLuongTare,
            }, options);

            var newItem = new PhieuCanSauXepKhuon
            {
                STT = model.STT,
                Ngay = item.Ngay,
                NgayNguyenLieu = model.NgayNguyenLieu,
                Gio = item.Gio,
                MaXuong = item.MaXuong,
                MaMayCan = item.MaMayCan,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaSize = item.MaSize,
                MaChieuXa = item.MaChieuXa,
                MaCoi = item.MaCoi,
                TrongLuong = model.TrongLuong,
                TrongLuongTare = model.TrongLuongTare,
                ChiTietLuotRaCoiId = item.ChiTietLuotRaCoiId,
                MaNhanVien = item.MaNhanVien,
                MaUserCan = item.MaUserCan,
                MaThe = item.MaThe,
                TyLeMaBang = item.TyLeMaBang,
                IsTam = item.IsTam,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanSauXepKhuon.Remove(item);
                    _context.PhieuCanSauXepKhuon.Add(newItem);
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSauXepKhuon>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSauXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaXuong,
            }, options);

            var newItem = new PhieuCanSauXepKhuon
            {
                STT = item.STT,
                Ngay = item.Ngay,
                NgayNguyenLieu = model.NgayNguyenLieu,
                Gio = item.Gio,
                MaXuong = model.MaXuong,
                MaMayCan = item.MaMayCan,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaSize = item.MaSize,
                MaChieuXa = item.MaChieuXa,
                MaCoi = item.MaCoi,
                TrongLuong = item.TrongLuong,
                TrongLuongTare = item.TrongLuongTare,
                ChiTietLuotRaCoiId = item.ChiTietLuotRaCoiId,
                MaNhanVien = item.MaNhanVien,
                MaUserCan = item.MaUserCan,
                MaThe = item.MaThe,
                TyLeMaBang = item.TyLeMaBang,
                IsTam = item.IsTam,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanSauXepKhuon.Remove(item);
                    _context.PhieuCanSauXepKhuon.Add(newItem);
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

        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSauXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSauXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSauXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSauXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaThanhPham
            }, options);
            item.MaThanhPham = model.MaThanhPham;
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
    }
}
