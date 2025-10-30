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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using Models.Repos.NotiModel;
using ViewModels.Repos.API;
using WebAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KhuVucKetChuyenController : ControllerBase
    {
        private readonly dbPMScontext _context;
        public KhuVucKetChuyenController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{dateTime}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllsByDate(string dateTime)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmKhuVucKetChuyen.Gets(date1);
            return items;
        }


        [HttpGet("{dateTime}/{xuongId}/{userName}")]
        [Authorize]
        public async Task<IActionResult> CommandXoa(string dateTime, string xuongId, string userName, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                //AppViewModel.Ins.IsBusy = true;
                //AppViewModel.Ins.BusyString = "Đang thục hiện xóa kết chuyển...";
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đang thục hiện xóa kết chuyển...");
                await Task.Delay(100).ConfigureAwait(true);
                var khuVucs = new List<KC_KhuVucTrangThai>();

                string listStringKhuVucTrangThaiSelectedItems = dataT.Item1;
                string[] arrayKhuVucTrangThais = listStringKhuVucTrangThaiSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var khuVucTrangThaiSelectedItem in arrayKhuVucTrangThais)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = khuVucTrangThaiSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var khuVucTrangThai = JsonSerializer.Deserialize<KC_KhuVucTrangThai>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (khuVucTrangThai != null)
                    {
                        khuVucs.Add(khuVucTrangThai);
                    }
                }

                //foreach (var item in KhuVucTrangThaiSelectedItems)
                //{
                //    khuVucs.Add((KC_KhuVucTrangThai)item);
                //}

                if (khuVucs.Any())
                {
                    foreach (var khuVuc in khuVucs)
                    {
                        // AppViewModel.Ins.BusyString =$@"Đang thục hiện xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}...";
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đang thục hiện xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}...");
                        await Task.Delay(500).ConfigureAwait(true);
                        var logKS = new LogKiemSoatKetChuyen()
                        {
                            Action = "XOA",
                            GioChuyen = DateTime.Now.TimeOfDay,
                            NgayChuyen = date1,
                            MaXuong = khuVuc.MaXuong,
                            PCName = AppViewModels.AppViewModel.Instance.PCName,
                            UserName = userName,
                            tab = khuVuc.Kc_KhuVuc.Tab
                        };
                        if (Vm.VmLogKetChuyen.Insert(logKS) > 0)
                        {
                            //if(khuVuc.Kc_KhuVuc.Tab == 9)
                            //{
                            //    if(BV_PhieuCanFilletViewModel.Ins.Delete(AppViewModel.Ins.DateTimeNow.Date) <= 0)
                            //    {
                            //        AppViewModel.Ins.BusyString = $@"Xóa kết chuyển Khu Vực {khuVuc.Kc_KhuVuc.Name} không thành công";
                            //        await Task.Delay(1000).ConfigureAwait(true);
                            //        //throw new Exception($@"Xóa kết chuyển Khu Vực {khuVuc.Kc_KhuVuc.Name} không thành công");
                            //    }
                            //} else
                            //{
                            if (Vm.VmLogKetChuyen.Delete(date1, khuVuc.Kc_KhuVuc.Tab) > 0)
                            {
                                if (khuVuc.Kc_KhuVuc.Tab == 1)
                                {
                                    if (Vm.VmBV_PhieuCanDinhHinh.Delete(date1) <= 0)
                                    {
                                        //AppViewModel.Ins.BusyString =
                                        //    $@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}";
                                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                                        await Task.Delay(1000).ConfigureAwait(true);
                                        //throw new Exception($@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                                    }
                                }
                                else
                                {
                                    if (khuVuc.Kc_KhuVuc.KhuVucId == "09")
                                    {
                                        if (Vm.VmBV_PhieuCanDinhHinh.Delete(date1) <= 0)
                                        {
                                            //AppViewModel.Ins.BusyString =$@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}";
                                            await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                                            await Task.Delay(1000).ConfigureAwait(true);
                                        }
                                    }
                                    else
                                    {
                                        if (Vm.VmBV_PhieuCanDinhHinh.Delete(date1, khuVuc.Kc_KhuVuc.KhuVucId) <= 0)
                                        {
                                            //AppViewModel.Ins.BusyString =$@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}";
                                            await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                                            await Task.Delay(1000).ConfigureAwait(true);
                                            //throw new Exception($@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //AppViewModel.Ins.BusyString =$@"Không thể xóa log kết chuyển {khuVuc.Kc_KhuVuc.Name}";
                                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Không thể xóa kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                                await Task.Delay(1000).ConfigureAwait(true);
                                //throw new Exception($@"Không thể xóa log kết chuyển {khuVuc.Kc_KhuVuc.Name}");
                            }

                        }
                        else
                        {
                            //AppViewModel.Ins.BusyString = $@"Thêm Log Không Thành Công";
                            await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thêm Log Không Thành Công");
                            await Task.Delay(1000).ConfigureAwait(true);
                            //throw new Exception("Thêm Log Không Thành Công");
                            return BadRequest(new ApiResponse
                            {
                                Success = false,
                                Message = "Thêm Log Không Thành Công!"
                            });
                        }

                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thành công!"
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu khu vực!"
                    });
                }
            }
            catch (Exception exception)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Lỗi" + exception.ToString());
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { exception.Message }
                });
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
    }
}
