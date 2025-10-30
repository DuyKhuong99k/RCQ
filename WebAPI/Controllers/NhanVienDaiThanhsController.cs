using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NhanVienDaiThanhsController : ControllerBase
    {
        private readonly dbPMScontext _context;
        private readonly AppSetting _appSettings;

        public NhanVienDaiThanhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAllNhanViens()
        {
            var nhanViens = _context.NhanVienDaiThanh.OrderByDescending(x => x.MaNhanVien).ToList();

            return Ok(nhanViens);
        }

        //[HttpGet]
        //[Authorize]
        //public IActionResult GetAllNhanViens(int pageIndex = 1, int pageSize = 500)
        //{
        //    if (pageIndex <= 0 || pageSize <= 0)
        //    {
        //        return BadRequest("pageIndex and pageSize must be greater than zero.");
        //    }

        //    var nhanViens = _context.NhanVienDaiThanh
        //                            .OrderByDescending(x => x.MaNhanVien)
        //                            .Skip((pageIndex - 1) * pageSize)
        //                            .Take(pageSize)
        //                            .ToList();

        //    bool hasMore = nhanViens.Count == pageSize;

        //    var result = new
        //    {
        //        Data = nhanViens,
        //        HasMore = hasMore
        //    };

        //    return Ok(result);
        //}

        [HttpGet]
        [Authorize]
        public IActionResult GetAllNhanVienWithDataNeededs()
        {
            var nhanViens = _context.NhanVienDaiThanh
                                    .OrderByDescending(x => x.MaNhanVien)
                                    .Select(nv => new
                                    {
                                        nv.MaNhanVien,
                                        nv.Name,
                                        nv.DeptName0,
                                        nv.MaHoSo,
                                        nv.MaChamCong
                                        // Chọn các trường khác cần thiết ở đây
                                    })
                                    .ToList();

            return Ok(nhanViens);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAllNhanVienPhucVuWithDataNeededs()
        {
            var nhanViens = _context.NhanVienDaiThanh
                                    .Where(nv => nv.IsPhucVu) // Lọc nhân viên có vai trò Phục Vụ
                                    .OrderByDescending(x => x.MaNhanVien)
                                    .Select(nv => new
                                    {
                                        nv.MaNhanVien,
                                        nv.Name,
                                        nv.MaHoSo,
                                        nv.DeptName0,
                                        nv.MaChamCong
                                        // Chọn các trường khác cần thiết ở đây
                                    })
                                    .ToList();

            return Ok(nhanViens);
        }
        //[HttpGet("{skip}")]
        //[Authorize]
        //public IActionResult GetMoreNhanViens(int skip)
        //{
        //    try
        //    {
        //        // Lấy thêm dữ liệu nhân viên từ cơ sở dữ liệu, bắt đầu từ vị trí skip
        //        var moreNhanViens = _context.NhanVienDaiThanh.OrderByDescending(x => x.MaNhanVien)
        //                                                      .Skip(skip)
        //                                                      .Take(10) // Lấy 10 nhân viên tiếp theo
        //                                                      .ToList();

        //        return Ok(moreNhanViens);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi nếu cần thiết
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        [HttpGet("{page}/{pageSize}")]
        [Authorize]
        public IActionResult GetMoreNhanViens(int page, int pageSize)
        {
            try
            {
                // Tính vị trí bắt đầu dữ liệu cần lấy
                int skip = (page - 1) * pageSize;

                // Lấy dữ liệu nhân viên từ cơ sở dữ liệu, bắt đầu từ vị trí skip
                var moreNhanViens = _context.NhanVienDaiThanh.OrderByDescending(x => x.MaNhanVien)
                                                          .Skip(skip)
                                                          .Take(pageSize)
                                                          .ToList();

                return Ok(moreNhanViens);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần thiết
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET: api/NguoiDung/{id}
        [HttpGet("{maNhanVien}")]
        [Authorize]
        public IActionResult GetNhanVienByMaNhanVien(string maNhanVien)
        {
            var nhanVien = _context.NhanVienDaiThanh.FirstOrDefault(u => u.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên không tồn tại!."
                });
            }

            return Ok(nhanVien);
        }
        [HttpGet("{maHoSo}")]
        [Authorize]
        public IActionResult GetNhanVienByMaHoSo(string maHoSo)
        {
            var nhanVien = _context.NhanVienDaiThanh.FirstOrDefault(u => u.MaHoSo == maHoSo);
            if (nhanVien == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên không tồn tại!."
                });
            }

            return Ok(nhanVien);
        }
        [HttpGet("{maChamCong}")]
        [Authorize]
        public IActionResult GetNhanVienByMaChamCong(string maChamCong)
        {
            var nhanVien = _context.NhanVienDaiThanh.FirstOrDefault(u => u.MaChamCong == maChamCong);
            if (nhanVien == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên không tồn tại!."
                });
            }

            return Ok(nhanVien);
        }
        [HttpGet("{maNhanVien}")]
        [Authorize]
        public IActionResult GetNameNhanVienByMaNhanVien(string maNhanVien)
        {
            var nhanVien = _context.NhanVienDaiThanh.FirstOrDefault(u => u.MaHoSo == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên không tồn tại!."
                });
            }
            var tenNhanVien = nhanVien.Name;
            return Ok(tenNhanVien);
        }
        [HttpGet("{maNhanVien}")]
        [Authorize]
        public IActionResult GetMaHoSoNhanVienByMaNhanVien(string maNhanVien)
        {
            var nhanVien = _context.NhanVienDaiThanh.FirstOrDefault(u => u.MaHoSo == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên không tồn tại!."
                });
            }
            var maHoSo = nhanVien.MaHoSo;
            return Ok(maHoSo);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetNhoms()
        {
            var items = Vm.VmNhanVien.Nhoms;
            if (items == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Danh sách nhóm rỗng!"
                });
            }

            return Ok(items);
        }
        [HttpGet("{xuongId}/{dateTime}/{type}")]
        [Authorize]
        public IActionResult GetNhanViensKiems(string xuongId, string dateTime, int type)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.GetNhanViensKiems(xuongId, date1, type);
            if (items == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Danh sách nhân viên kiểm rỗng!"
                });
            }

            return Ok(items);
        }
        [HttpGet("{xuongId}")]
        [Authorize]
        public IActionResult GetToKiemsByXuongId(string xuongId)
        {
            var items = Vm.VmNhanVien.GetToKiemsByXuongId(xuongId);
            if (items == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Danh sách nhân viên kiểm rỗng!"
                });
            }

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertNhanVien(NhanVienDaiThanh model)
        {
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
            if (_context.NhanVienDaiThanh.Any(u => u.MaNhanVien == model.MaNhanVien))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên đã tồn tại.",
                });
            }
            var newNhanVien = new NhanVienDaiThanh
            {
                MaNhanVien = model.MaNhanVien,
                MaHoSo = model.MaHoSo,
                Name = model.Name,
                LoaiSanLuong = model.LoaiSanLuong,
                DeptName0 = model.DeptName0,
                Xuong = model.Xuong,
                IsContracting = model.IsContracting,
                IsHuman = model.IsHuman,
                IsPhucVu = model.IsPhucVu,
                IsGiaCong = model.IsGiaCong,
                IsBanKiem = model.IsBanKiem,
                IsChucNang = model.IsChucNang,
                IsNhom = model.IsNhom,
                IsShowDinhMuc = model.IsShowDinhMuc,
                MaChamCong = model.MaChamCong

            };
            _context.NhanVienDaiThanh.Add(newNhanVien);
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
                Message = "Thêm nhân viên thành công!"
            });
        }
        [HttpPost("{maNhanVien}")]
        [Authorize]
        public async Task<IActionResult> UpdateNhanVien(string maNhanVien, [FromBody] NhanVienDaiThanh model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhân viên không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var nhanVien = await _context.NhanVienDaiThanh.FindAsync(maNhanVien);
            if (nhanVien == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã Nhân Viên không tồn tại."
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
            var MNgay = DateTime.Now;
            // Cập nhật thông tin người dùng từ dữ liệu đầu vào
            //nhanVien.MaNhanVien = model.MaNhanVien,
            nhanVien.MaHoSo = model.MaHoSo;
            nhanVien.Name = model.Name;
            nhanVien.LoaiSanLuong = model.LoaiSanLuong;
            nhanVien.DeptName0 = model.DeptName0;
            nhanVien.Xuong = model.Xuong;
            nhanVien.IsContracting = model.IsContracting;
            nhanVien.IsHuman = model.IsHuman;
            nhanVien.IsPhucVu = model.IsPhucVu;
            nhanVien.IsGiaCong = model.IsGiaCong;
            nhanVien.IsBanKiem = model.IsBanKiem;
            nhanVien.IsChucNang = model.IsChucNang;
            nhanVien.IsNhom = model.IsNhom;
            nhanVien.IsShowDinhMuc = model.IsShowDinhMuc;
            nhanVien.MNgay = MNgay;
            nhanVien.MaChamCong = model.MaChamCong;

            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                await _context.SaveChangesAsync();
                // Tạo bản ghi mới cho bảng HqLoaiNguyenLieuUs
                var newItemUs = new HQ_NhanVien_U()
                {
                    MaNhanVien = nhanVien.MaNhanVien,
                    MNgay = MNgay,
                    Ngay = DateTime.Now,
                    AC = nhanVien.AC,
                    Address = nhanVien.Address,
                    BirthDate = nhanVien.BirthDate,
                    ChucVu = nhanVien.ChucVu,
                    DeptCode0 = nhanVien.DeptCode0,
                    DeptName0 = nhanVien.DeptName0,
                    FirstWorkingDate = nhanVien.FirstWorkingDate,
                    GenderName = nhanVien.GenderName,
                    IsBanKiem = nhanVien.IsBanKiem,
                    IsChucNang = nhanVien.IsChucNang,
                    IsContracting = nhanVien.IsContracting,
                    IsGiaCong = nhanVien.IsGiaCong,
                    IsHuman = nhanVien.IsHuman,
                    IsNhom = nhanVien.IsNhom,
                    IsPhucVu = nhanVien.IsPhucVu,
                    IsShowDinhMuc = nhanVien.IsShowDinhMuc,
                    LoaiSanLuong = nhanVien.LoaiSanLuong,
                    MaHoSo = nhanVien.MaHoSo,
                    Name = nhanVien.Name,
                    Xuong = nhanVien.Xuong,
                    IsNhanVienCat = nhanVien.IsNhanVienCat,
                    JobPositionName0 = nhanVien.JobPositionName0,
                    MaChamCong = nhanVien.MaChamCong,
                    Tel = nhanVien.Tel,
                };

                // Thêm vào bảng HqLoaiNguyenLieuUs
                _context.HqNhanVienUs.Add(newItemUs);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
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
                Message = "Cập nhật thông tin nhân viên thành công!"
            });
        }
        [HttpPost("{maNhanVien}")]
        [Authorize]
        public async Task<IActionResult> DeleteNhanVien(string maNhanVien)
        {
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn nhân viên!."
                });
            }
            var nhanVien = await _context.NhanVienDaiThanh.FindAsync(maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhân viên không tồn tại."
                });
            }

            _context.NhanVienDaiThanh.Remove(nhanVien);

            try
            {
                var MNgay = DateTime.Now;
                // Tạo bản ghi mới cho bảng HqLoaiNguyenLieuUs để lưu lịch sử
                var newItemUs = new HQ_NhanVien_D()
                {
                    MaNhanVien = nhanVien.MaNhanVien,
                    MNgay = MNgay,
                    Ngay = DateTime.Now,
                    AC = nhanVien.AC,
                    Address = nhanVien.Address,
                    BirthDate = nhanVien.BirthDate,
                    ChucVu = nhanVien.ChucVu,
                    DeptCode0 = nhanVien.DeptCode0,
                    DeptName0 = nhanVien.DeptName0,
                    FirstWorkingDate = nhanVien.FirstWorkingDate,
                    GenderName = nhanVien.GenderName,
                    IsBanKiem = nhanVien.IsBanKiem,
                    IsChucNang = nhanVien.IsChucNang,
                    IsContracting = nhanVien.IsContracting,
                    IsGiaCong = nhanVien.IsGiaCong,
                    IsHuman = nhanVien.IsHuman,
                    IsNhom = nhanVien.IsNhom,
                    IsPhucVu = nhanVien.IsPhucVu,
                    IsShowDinhMuc = nhanVien.IsShowDinhMuc,
                    LoaiSanLuong = nhanVien.LoaiSanLuong,
                    MaHoSo = nhanVien.MaHoSo,
                    Name = nhanVien.Name,
                    Xuong = nhanVien.Xuong,
                    IsNhanVienCat = nhanVien.IsNhanVienCat,
                    JobPositionName0 = nhanVien.JobPositionName0,
                    MaChamCong = nhanVien.MaChamCong,
                    Tel = nhanVien.Tel

                };

                // Thêm bản ghi vào bảng HqLoaiNguyenLieuUs
                _context.HqNhanVienDs.Add(newItemUs);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xóa nhân viên.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa nhân viên thành công!"
            });
        }
        [HttpGet("{xuongId}")]
        [Authorize]
        public IActionResult GetListNhanVienDaiThanhFilltered(string xuongId)
        {
            var items = Vm.VmNhanVien.GetListNhanVienDaiThanhFilltered(xuongId);
            if (items == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Danh sách nhân viên rỗng!"
                });
            }

            return Ok(items);
        }
    }
}
