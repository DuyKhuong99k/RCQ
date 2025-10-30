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
using BravoModelV1.Dao;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BV_PhieuCanCaGiongController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BV_PhieuCanCaGiongController(dbPMScontext context)
        {
            _context = context;
        }

        private MainViewModel Vm => MainViewModel.Instance;
        [HttpPost("{ngay}")]
        [Authorize]
        public IActionResult CheckItemServer(string ngay)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmBV_PhieuCanCaGiong = Vm.VmBV_PhieuCanCaGiong;
                //lấy danh sách item từ server
                var itemsServer = vmBV_PhieuCanCaGiong.Gets<object>(dateTime);

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
                    // Nếu người dùng chọn OK, xóa các mục tương ứng API DeleteItemServer
                }
                else
                {
                    // tiếp tục các API Upload, bỏ qua API DeleteItemServer
                    return Ok(new ApiResponse
                    {
                        Success = true,
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
        [HttpPost("{ngay}")]
        [Authorize]
        public IActionResult DeleteItemServer(string ngay)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmBV_PhieuCanCaGiong = Vm.VmBV_PhieuCanCaGiong;
                vmBV_PhieuCanCaGiong.Delete(dateTime);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $@"Đã xoá các mục tương ứng ở server!"
                });
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
        [HttpPost("{ngay}")]
        [Authorize]
        public IActionResult ConfirmItemCountUpload(string ngay)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmPhieuCanCaGiong = Vm.VmPhieuCanCaGiong;
                var items = vmPhieuCanCaGiong.Gets<PhieuCanCaGiongVungNuoiDaiThanhSide>(dateTime);
                if (items != null && items.Any())
                {
                    // hiển thị thông báo trên trong hộp thoại OK/Cancel ở client
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = $@"Thực Hiện Trên {items.Count} Items"
                    });
                    // client chọn Cancel thì dừng tiến trình, không thực hiện chức năng nữa
                    // client chọn OK , gọi các API Upload theo từng công đoạn đã chọn
                }
                else
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = $@"Không có dữ liệu Phiếu Cân Cá Giống"
                    });
                }
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
        [HttpPost("{ngay}")]
        [Authorize]
        public IActionResult Upload(string ngay)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                var vmApp = AppViewModels.AppViewModel.Instance;
                var vmPhieuCanCaGiong = Vm.VmPhieuCanCaGiong;
                var vmBV_PhieuCanCaGiong = Vm.VmBV_PhieuCanCaGiong;
                var items = vmPhieuCanCaGiong.Gets<PhieuCanCaGiongVungNuoiDaiThanhSide>(dateTime);
                if (items != null && items.Any())
                {
                    var _items = (from p in items
                                  select new BravoModelV1.Model.PhieuCanCaGiong
                                  {
                                      MaCa = p.TenLoaiCa.NonUnicode(true).Replace(" ", "").ToUpper(),
                                      Ngay = p.Ngay,
                                      MaNhaCungCap = p.TenChuAo.NonUnicode(true).Replace(" ", "").ToUpper(),
                                      MaAo = p.TenAo,
                                      MaPhuongTien = p.MaGhe.NonUnicode(true).Replace(" ", "").ToUpper(),
                                      MaVung = p.MaMayCan,
                                      ThoiGian = p.Gio,
                                      SoLuong = p.TrongLuong,
                                      ID =
                                          $@"{vmApp.DateTimeNow:yyyyMMdd}.{vmApp.PCName}.{p.BiosId}.{p.Ngay:yyyyMMdd}.{p.MaMayCan}.{p.STT}"
                                              .NonUnicode().Replace(" ", "").ToUpper(),
                                      TenAo = p.TenAo,
                                      TenCa = p.TenLoaiCa,
                                      TenNhaCungCap = p.TenChuAo,
                                      TenPhuongTien = p.MaGhe,
                                      TenVungNuoi = p.MaMayCan,
                                  }).ToList();
                    if (_items.Any())
                    {
                        var rows = vmBV_PhieuCanCaGiong.InsertBatch(_items);
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
                            Message = "Không thể tổng hợp được dữ liệu Phiếu Cân Cá Giống.",
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
    }
}
