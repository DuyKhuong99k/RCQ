using System;
using System.Collections.Generic;
using System.Drawing;
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
using ToolEx;
using ToolsEx;
using AppViewModels;
namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BV_PhieuCanSanXuatController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BV_PhieuCanSanXuatController(dbPMScontext context)
        {
            _context = context;
        }

        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetCongDoans()
        {
            var items = Vm.VmBV_PhieuCanSanXuat.GetCongDoans();
            return Ok(items);
        }

        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult CheckItemServer(string ngay, string congDoanId)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                //lấy danh sách item từ server
                var itemsServer = vmBV_PhieuCanSanXuat.Gets<object>(dateTime, congDoanId);

                // kiểm tra itemsServer có dữ liệu hay không?  
                if (itemsServer.Any())
                {

                    // Trả về thông báo có dữ liệu
                    return Ok(new ApiResponse
                    {
                        Success = true, // CÓ DỮ LIỆU
                        Message = $@"Thực Hiện Lại Thao Tác!"
                    });
                    // Ghi chú: Thực hiện thao tác DeleteItemServer dựa trên phản hồi phía client
                    // Nếu người dùng chọn Cancel, kết thúc hàm, không thực hiện chức năng nữa
                    // Nếu người dùng chọn OK, xóa các mục tương ứng API DeleteItemServer -> ConfirmItemCount
                }
                else
                {
                    // tiếp tục các API Upload, bỏ qua API DeleteItemServer
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = $@"Tiếp Tục Tiến Trình!"
                    });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi." + ex.Message.ToString(),
                    Errors = new List<string> { ex.Message.ToString() }
                });
            }

        }
        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult DeleteItemServer(string ngay, string congDoanId)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                vmBV_PhieuCanSanXuat.Delete(dateTime, congDoanId);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $@"Đã xoá các mục tương ứng ở server!"
                });
                // tiếp tục gọi các API ConfirmItemCountUpload để upload theo từng công đoạn đã chọn
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xoá dữ liệu." + ex.Message.ToString(),
                    Errors = new List<string> { ex.Message.ToString() }
                });
            }


        }

        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult ConfirmItemCountUpload(string ngay, string congDoanId)
        {
            DateTime dateTime;
            try
            {
                dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Ngày không hợp lệ."
                });
            }

            try
            {
                var itemDictionary = new Dictionary<string, Func<DateTime, IEnumerable<dynamic>>>
                {
                    { "TN", date => Vm.VmPhieuCanNguyenLieu.Gets<dynamic>(date) },
                    { "FL", date => Vm.VmPhieuCanTPFillet.Gets<dynamic>(date) },
                    { "DH", date => Vm.VmPhieuCanTPDinhHinh.Gets<dynamic>(date) },
                    { "XK", date => Vm.VmPhieuCanChinhXepKhuon.Gets<dynamic>(date) },
                    { "CXBN", date => Vm.VmPhieuCanPhuPham.Gets<dynamic>(date) }
                };

                if (!itemDictionary.TryGetValue(congDoanId, out var getItems))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Mã công đoạn không hợp lệ."
                    });
                }

                var items = getItems(dateTime);

                if (items != null && items.Any())
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = $@"Thực Hiện Trên {items.Count()} Items"
                    });
                }
                else
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = $@"Không có dữ liệu Phiếu Cân {congDoanId}"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xử lý dữ liệu." + ex.Message,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult Upload_DH(string ngay, string congDoanId)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmApp = AppViewModels.AppViewModel.Instance;
                var vmPhieuCan = Vm.VmPhieuCanTPDinhHinh;
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                var items = vmPhieuCan.Gets<dynamic>(dateTime);
                if (items != null && items.Any())
                {
                    var _items = (from p in items
                                  select new BravoModelV1.Model.PhieuCanSanXuat
                                  {
                                      Ngay = Convert.ToDateTime(p.Ngay),
                                      MaCongDoan = congDoanId,
                                      MaSanPham = p.BravoId.ToString(),
                                      ThoiGian = TimeSpan.Parse(p.Gio.ToString()),
                                      SoLuong = Convert.ToDecimal(p.TrongLuongTra),
                                      ID =
                                          $@"{vmApp.DateTimeNow:yyyyMMdd}.{vmApp.PCName}.{AppViewModels.HardId.GetValue()}.{congDoanId}.{Convert.ToDateTime(p.Ngay):yyyyMMdd}.{p.MaMayCan.ToString()}.{p.STT.ToString()}.{p.MaXuong.ToString()}".NonUnicode().Replace(" ", "").ToUpper(),
                                      TenKhachHang = "",
                                      TenSanPham = p.BravoId.ToString(),
                                      MaChuyen = "",
                                      MaKhachHang = "",
                                      MaXuong = p.MaXuong.ToString(),
                                      TenChuyen = "",
                                      TenCongDoan = @"Định hình",
                                      TenXuong = $@"Xưởng {p.MaXuong}",
                                  }).ToList();
                    if (_items.Any())
                    {
                        var rows = vmBV_PhieuCanSanXuat.InsertBatch(_items);
                        //MessageBox.Show($@"Đã thêm {rows} item");
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thêm {rows} item"
                        });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Không thể tổng hợp được dữ liệu Phiếu Cân Sản Xuất.",
                        });
                    }
                }
                else
                {
                    //MessageBox.Show("Không Có Dữ Liệu");
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không Có Dữ Liệu.",
                    });
                }
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
        }

        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult Upload_FL(string ngay, string congDoanId)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmApp = AppViewModels.AppViewModel.Instance;
                var vmPhieuCan = Vm.VmPhieuCanTPFillet;
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                var items = vmPhieuCan.Gets<dynamic>(dateTime);
                if (items != null && items.Any())
                {
                    var _items = (from p in items
                                  select new BravoModelV1.Model.PhieuCanSanXuat
                                  {
                                      Ngay = Convert.ToDateTime(p.Ngay),
                                      MaCongDoan = congDoanId,
                                      MaSanPham = p.BravoId.ToString(),
                                      ThoiGian = TimeSpan.Parse(p.ThoiGianCan.ToString()),
                                      SoLuong = Convert.ToDecimal(p.TrongLuong),
                                      ID =
                                          $@"{vmApp.DateTimeNow:yyyyMMdd}.{vmApp.PCName}.{AppViewModels.HardId.GetValue()}.{congDoanId}.{Convert.ToDateTime(p.Ngay):yyyyMMdd}.{p.MaMayTinhCan.ToString()}.{TimeSpan.Parse(p.ThoiGianCan.ToString())}.{p.MaUserCan.ToString()}"
                                              .NonUnicode().Replace(" ", "").ToUpper(),
                                      TenKhachHang = "",
                                      TenSanPham = p.BravoId.ToString(),
                                      MaChuyen = "",
                                      MaKhachHang = "",
                                      MaXuong = p.MaXuongSanXuat.ToString(),
                                      TenChuyen = "",
                                      TenCongDoan = @"Fillet",
                                      TenXuong = $@"Xưởng {p.MaXuongSanXuat}",
                                  }).ToList();
                    if (_items.Any())
                    {
                        var rows = vmBV_PhieuCanSanXuat.InsertBatch(_items);
                        //MessageBox.Show($@"Đã thêm {rows} item");
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thêm {rows} item"
                        });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Không thể tổng hợp được dữ liệu Phiếu Cân Sản Xuất.",
                        });
                    }
                }
                else
                {
                    //MessageBox.Show("Không Có Dữ Liệu");
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không Có Dữ Liệu.",
                    });
                }
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
        }
        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult Upload_PP(string ngay, string congDoanId)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmApp = AppViewModels.AppViewModel.Instance;
                var vmPhieuCan = Vm.VmPhieuCanPhuPham;
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                var items = vmPhieuCan.Gets<dynamic>(dateTime);
                if (items != null && items.Any())
                {
                    var _items = (from p in items
                                  select new BravoModelV1.Model.PhieuCanSanXuat
                                  {
                                      Ngay = Convert.ToDateTime(p.Ngay),
                                      MaCongDoan = congDoanId,
                                      MaSanPham = $@"{p.ThanhPhamName}".NonUnicode(true).Replace(" ", "").ToUpper(),
                                      ThoiGian = Convert.ToDateTime(p.ThoiGianCan).TimeOfDay,
                                      SoLuong = Convert.ToDecimal(p.TrongLuong),
                                      ID =
                                          $@"{vmApp.DateTimeNow:yyyyMMdd}.{vmApp.PCName}.{AppViewModels.HardId.GetValue()}.{congDoanId}.{Convert.ToDateTime(p.NgayCan):yyyyMMddHHmmss.fff}.{p.MaMayTinhCan.ToString()}.{Convert.ToDateTime(p.ThoiGianCan):yyyyMMddHHmmss.fff}.{p.MaXuongSanXuat.ToString()}"
                                              .NonUnicode().Replace(" ", "").ToUpper(),
                                      TenKhachHang = p.NhaMuaHangName.ToString(),
                                      TenSanPham = $@"{p.ThanhPhamName}",
                                      MaChuyen = "",
                                      MaKhachHang = p.NhaMuaHang.ToString(),
                                      MaXuong = p.MaXuongSanXuat.ToString(),
                                      TenChuyen = "",
                                      TenCongDoan = @"Phụ Phẩm - Cân xuất bán ngoài",
                                      TenXuong = $@"Xưởng {p.MaXuongSanXuat}",
                                  }).ToList();
                    if (_items.Any())
                    {
                        var rows = vmBV_PhieuCanSanXuat.InsertBatch(_items);
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thêm {rows} item"
                        });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Không thể tổng hợp được dữ liệu Phiếu Cân Sản Xuất.",
                        });
                    }
                }
                else
                {
                    //MessageBox.Show("Không Có Dữ Liệu");
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không Có Dữ Liệu.",
                    });
                }
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
        }
        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult Upload_TN(string ngay, string congDoanId)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmApp = AppViewModels.AppViewModel.Instance;
                var vmPhieuCan = Vm.VmPhieuCanNguyenLieu;
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                var items = vmPhieuCan.Gets<dynamic>(dateTime);
                if (items != null && items.Any())
                {
                    var _items = (from p in items
                                  select new BravoModelV1.Model.PhieuCanSanXuat
                                  {
                                      Ngay = Convert.ToDateTime(p.Ngay) ?? DateTime.MinValue,
                                      MaCongDoan = congDoanId,
                                      MaSanPham = p.MaLoaiThanhPham.ToString(),
                                      ThoiGian = Convert.ToDateTime(p.ThoiGianCan).TimeOfDay,
                                      SoLuong = Convert.ToDecimal(p.TrongLuong) ?? 0,
                                      ID =
                                          $@"{vmApp.DateTimeNow:yyyyMMdd}.{vmApp.PCName}.{AppViewModels.HardId.GetValue()}.{congDoanId}.{p.Ngay:yyyyMMdd}.{p.MaMayTinhCan}.{p.ThoiGianCan.ToString()}"
                                              .NonUnicode().Replace(" ", "").ToUpper(),
                                      TenKhachHang = "",
                                      TenSanPham = p.ThanhPhamName.ToString(),
                                      MaChuyen = "",
                                      MaKhachHang = "",
                                      MaXuong = p.MaXuongSanXuat.ToString(),
                                      TenChuyen = "",
                                      TenCongDoan = @"Tiếp Nhận",
                                      TenXuong = $@"Xưởng {p.MaXuongSanXuat}",
                                  }).ToList();
                    if (_items.Any())
                    {
                        var rows = vmBV_PhieuCanSanXuat.InsertBatch(_items);
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thêm {rows} item"
                        });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Không thể tổng hợp được dữ liệu Phiếu Cân Sản Xuất.",
                        });
                    }
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không Có Dữ Liệu.",
                    });
                }
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
        }
        [HttpPost("{ngay}/{congDoanId}")]
        [Authorize]
        public IActionResult Upload_XK(string ngay, string congDoanId)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmApp = AppViewModels.AppViewModel.Instance;
                var vmPhieuCan = Vm.VmPhieuCanChinhXepKhuon;
                var vmBV_PhieuCanSanXuat = Vm.VmBV_PhieuCanSanXuat;
                var items = vmPhieuCan.Gets<dynamic>(dateTime);
                if (items != null && items.Any())
                {
                    var _items = (from p in items
                                  select new BravoModelV1.Model.PhieuCanSanXuat
                                  {
                                      Ngay = Convert.ToDateTime(p.Ngay),
                                      MaCongDoan = congDoanId,
                                      MaSanPham = $@"{p.ThanhPhamName}".NonUnicode(true).Replace(" ", "").ToUpper(),
                                      ThoiGian = TimeSpan.Parse(p.Gio.ToString()),
                                      SoLuong = Convert.ToDecimal(p.TrongLuong),
                                      ID =
                                          $@"{vmApp.DateTimeNow:yyyyMMdd}.{vmApp.PCName}.{AppViewModels.HardId.GetValue()}.{congDoanId}.{Convert.ToDateTime(p.Ngay):yyyyMMdd}.{p.MaMayCan.ToString()}.{p.MaXuong.ToString()}.{p.STT.ToString()}"
                                              .NonUnicode().Replace(" ", "").ToUpper(),
                                      TenKhachHang = "",
                                      TenSanPham = $@"{p.ThanhPhamName}",
                                      MaChuyen = "",
                                      MaKhachHang = "",
                                      MaXuong = p.MaXuong.ToString(),
                                      TenChuyen = "",
                                      TenCongDoan = @"Xếp Khuôn",
                                      TenXuong = $@"Xưởng {p.MaXuong}",
                                  }).ToList();
                    if (_items.Any())
                    {
                        var rows = vmBV_PhieuCanSanXuat.InsertBatch(_items);
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = $@"Đã thêm {rows} item"
                        });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Không thể tổng hợp được dữ liệu Phiếu Cân Sản Xuất.",
                        });
                    }
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không Có Dữ Liệu.",
                    });
                }
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
        }
    }
}
