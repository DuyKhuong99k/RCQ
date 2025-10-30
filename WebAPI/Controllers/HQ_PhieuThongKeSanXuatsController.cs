using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using AppModels;
using AppViewModels;
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

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HQ_PhieuThongKeSanXuatsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_PhieuThongKeSanXuatsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{ngay}")]
        [Authorize]
        public IActionResult GetAlls(string ngay)
        {
            DateTime date = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.HqPhieuThongKeSanXuats
                .Where(x => x.Ngay.Date == date.Date)
                .OrderByDescending(x => x.SoChungTu)
                .ToList();

            if (items.Count == 0)
            {
                // Nếu không có dữ liệu, trả về danh sách trống
                return Ok(new List<HQ_PhieuThongKeSanXuat>());
            }

            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(int id)
        {
            var item = _context.HqPhieuThongKeSanXuats.FirstOrDefault(x => x.Id == id);
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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            try
            {
                // Deserialize the incoming data
                var models = JsonSerializer.Deserialize<List<HQ_PhieuThongKeSanXuat>>(dataT.Item1);

                if (models == null || !models.Any())
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào trống hoặc không hợp lệ."
                    });
                }

                // Validate each model using ModelState
                var errors = new List<string>();
                foreach (var model in models)
                {
                    if (!TryValidateModel(model))
                    {
                        errors.AddRange(ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage));
                    }
                }

                if (errors.Any())
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ.",
                        Errors = errors
                    });
                }

                // Check for duplicate entries in the database (if necessary)
                var duplicateEntries = models
                    .Where(model => _context.HqPhieuThongKeSanXuats.Any(db => db.SoChungTu == model.SoChungTu))
                    .ToList();

                if (duplicateEntries.Any())
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Một số mã chứng từ đã tồn tại trong cơ sở dữ liệu.",
                        Errors = duplicateEntries.Select(d => $"SoChungTu: {d.SoChungTu}").ToList()
                    });
                }

                // Prepare the data for insertion
                foreach (var model in models)
                {
                    var newItem = new HQ_PhieuThongKeSanXuat
                    {
                        Ngay = model.Ngay,
                        SoChungTu = model.SoChungTu,
                        Ca = model.Ca,
                        MaTo = model.MaTo,
                        TenTo = model.TenTo,
                        MaNhanVien = model.MaNhanVien,
                        TenNhanVien = model.TenNhanVien,
                        MaCongViec = model.MaCongViec,
                        TenCongViec = model.TenCongViec,
                        SanLuong = model.SanLuong,
                        GioBatDau = model.GioBatDau,
                        GioKetThuc = model.GioKetThuc,
                        NgayGioTao = model.NgayGioTao
                    };

                    _context.HqPhieuThongKeSanXuats.Add(newItem);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thêm thành công!"
                });
            }
            catch (JsonException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi khi xử lý dữ liệu đầu vào.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] HQ_PhieuThongKeSanXuat model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.HqPhieuThongKeSanXuats.FindAsync(id);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại."
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
            item.Id = model.Id;
            item.Ngay = model.Ngay;
            item.SoChungTu = model.SoChungTu;
            item.Ca = model.Ca;
            item.MaTo = model.MaTo;
            item.TenTo = model.TenTo;
            item.MaNhanVien = model.MaNhanVien;
            item.TenNhanVien = model.TenNhanVien;
            item.MaCongViec = model.MaCongViec;
            item.SanLuong = model.SanLuong;
            item.GioBatDau = model.GioBatDau;
            item.GioKetThuc = model.GioKetThuc;
            item.NgayGioTao = model.NgayGioTao;
            //item.MNgay = model.MNgay;
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await _context.HqPhieuThongKeSanXuats.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhà cung cấp không tồn tại."
                });
            }

            _context.HqPhieuThongKeSanXuats.Remove(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xóa.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đã xoá!"
            });
        }

        class ItemTime
        {
            public DateTime Ngay { get; set; }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertBravo(Tuple<string> dataT)
        {
            try
            {
                var model = JsonSerializer.Deserialize<ItemTime>(dataT.Item1);
                string date = model.Ngay.ToString("yyyy-MM-dd");
                //var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
                DateTime ngay = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var conectionstring = AppViewModels.Base.Ins.ConnectionString;
                var conectionstringBravo = AppViewModels.Base.Ins.ConnectionStringBravo;

                var itemBravos = Vm.VmHQ_PhieuThongKeSanXuat.Gets<HQ_PhieuThongKeSanXuat>(ngay, conectionstringBravo);
                try
                {
                    // nếu dữ liệu chưa có trong serverBravo thì thêm mới
                    if (itemBravos.Count == 0)
                    {
                        var items = Vm.VmHQ_PhieuThongKeSanXuat.Gets<HQ_PhieuThongKeSanXuat>(ngay, conectionstring);
                        // lấy số chungưu lớn nhất
                        var maxSoChungTu = items
                        .Select(x => x.SoChungTu)
                        .Where(s => !string.IsNullOrWhiteSpace(s) && s.Contains("-"))
                       .Select(s =>
                       {
                           var parts = s.Split('-');
                           var so = parts[1];
                           return int.TryParse(so.Substring(so.Length - 2), out int stt) ? stt : 0;
                       }).Max();
                        // lọc dữ liệu theo số chứng từ lớn nhất
                        var filteredItems = items
                        .Where(x =>
                        {
                            if (string.IsNullOrWhiteSpace(x.SoChungTu)) return false;
                            var parts = x.SoChungTu.Split('-');
                            if (parts.Length != 2) return false;

                            var so = parts[1];
                            if (so.Length < 2) return false;

                            // Lấy 2 số cuối
                            return int.TryParse(so.Substring(so.Length - 2), out int stt) && stt == maxSoChungTu;
                        })
                        .ToList();
                        var rows = 0;
                        rows = Vm.VmHQ_PhieuThongKeSanXuat.Insert(filteredItems, conectionstringBravo);
                        //await Task.Delay(1000);
                        //await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");
                        var soChungTu = items
                            .Select(x => x.SoChungTu)
                            .FirstOrDefault(s =>
                            {
                                if (string.IsNullOrWhiteSpace(s)) return false;
                                var parts = s.Split('-');
                                if (parts.Length != 2) return false;
                                return int.TryParse(parts[1], out int stt) && stt == maxSoChungTu;
                            });
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thực hiện thêm {rows} dòng dữ liệu của Số Chứng từ [{soChungTu}] ngày [{date}]"
                        });
                    }
                    // nếu dữ liệu đã có trong serverBravo thì xóa dữ liệu theo ngày của Phiếu Thống kế ở Bravo rôi Insert lại dữ liệu từ server PMS
                    else
                    {
                        var items = Vm.VmHQ_PhieuThongKeSanXuat.Gets<HQ_PhieuThongKeSanXuat>(ngay, conectionstring);

                        // lấy số chứng từ lớn nhất
                        var maxSoChungTu = items
                            .Select(x => x.SoChungTu)
                            .Where(s => !string.IsNullOrWhiteSpace(s) && s.Contains("-"))
                            .Select(s =>
                            {
                                var parts = s.Split('-');
                                return int.TryParse(parts[1], out int stt) ? stt : 0;
                            }).Max();

                        // kiểm tra tồn tại ở bra hay không
                        var soChungTuTonTai = itemBravos
                            .Select(x => x.SoChungTu)
                            .FirstOrDefault(s =>
                            {
                                if (string.IsNullOrWhiteSpace(s)) return false;
                                var parts = s.Split('-');
                                if (parts.Length != 2) return false;
                                return int.TryParse(parts[1], out int stt) && stt == maxSoChungTu;
                            });

                        if (!string.IsNullOrEmpty(soChungTuTonTai))
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = $"Dữ liệu với Số chứng từ [{soChungTuTonTai}] đã được kết chuyển trước đó.",
                                Errors = new List<string> { $"SoChungTu [{soChungTuTonTai}] đã tồn tại ở Bravo." }
                            });
                        }

                        //xóa và insert nếu chưa tồn tại
                        var rows = 0;
                        rows = Vm.VmHQ_PhieuThongKeSanXuat.Delete(itemBravos, ngay, conectionstringBravo);

                        var filteredItems = items
                            .Where(x =>
                            {
                                if (string.IsNullOrWhiteSpace(x.SoChungTu)) return false;
                                var parts = x.SoChungTu.Split('-');
                                if (parts.Length != 2) return false;
                                return int.TryParse(parts[1], out int stt) && stt == maxSoChungTu;
                            }).ToList();

                        rows = Vm.VmHQ_PhieuThongKeSanXuat.Insert(filteredItems, conectionstringBravo);
                        var soChungTu = items
                           .Select(x => x.SoChungTu)
                           .FirstOrDefault(s =>
                           {
                               if (string.IsNullOrWhiteSpace(s)) return false;
                               var parts = s.Split('-');
                               if (parts.Length != 2) return false;
                               return int.TryParse(parts[1], out int stt) && stt == maxSoChungTu;
                           });
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thực hiện thêm {rows} dòng dữ liệu của Số Chứng từ [{soChungTu}] ngày [{date}]"
                        });
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi.",
                        Errors = new List<string> { ex.Message }
                    });
                }

            }
            //}
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tải dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }

        }
    }
}
