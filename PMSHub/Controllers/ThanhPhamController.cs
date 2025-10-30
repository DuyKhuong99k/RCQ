using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ThanhPhamController(IMainService mainService, IMayCansService mayCansService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay, int PageIndex, int PageSize)
        {
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }

                var items = mainService.VmThanhPhamHq.Items.Where(x => x.MNgay.Date >= date.Date)
                    .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                    {
                        Id = x.Id, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                        Min = $"{x.Min}", Max = $"{x.Max}"
                    }).ToList();
                var json = new
                {
                    data = items,
                    total = items.Count,
                    PageIndex
                };

                return Ok(json);
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    switch (mayCan.WKv)
                    {
                        case AppKV.Main:
                            break;
                        case AppKV.DauAo:
                            break;
                        case AppKV.NguyenLieu:
                            var itemNL = mainService.VmThanhPhamNguyenLieu.Items.Where(x => x.SuDung == true)
                                .OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                .Select(x => new
                                {
                                    Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                                    Min = $"{x.Min ?? 0.0}", Max = $"{x.Max ?? 9999.0}"
                                }).ToList();
                            var jsonNL = new
                            {
                                data = itemNL,
                                total = itemNL.Count,
                                PageIndex
                            };
                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                            goto case AppKV.TPFilletv2;
                        case AppKV.TPFillet:
                        {
                            if (mainService.VmApp.ComName?.ToUpper().Trim() == nameof(ComNames.HL))
                            {
                                var itemfltp = mainService.VmThanhPhamFillet.Items
                                    .Where(x => x.SuDung == true && x.IsXeBuom == true ).OrderByDescending(x => x.MNgay)
                                    .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x =>
                                        new
                                        {
                                            Id = x.Ma, Ten = x.Ten, x.SuDung,
                                            MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                                            Min = $"{x.Min ?? 0.0}", Max = $"{x.Max ?? 9999.0}"
                                        }).ToList();
                                var jsonfltp = new
                                {
                                    data = itemfltp,
                                    total = itemfltp.Count,
                                    PageIndex
                                };
                                return Ok(jsonfltp);
                            }
                            else
                            {
                                goto case AppKV.TPFilletv2;
                            }
                        }

                        case AppKV.BTPFilletv2:
                            goto case AppKV.TPFilletv2;
                        case AppKV.TPFilletv2:
                            var itemfl = mainService.VmThanhPhamFillet.Items.Where(x => x.SuDung == true && x.IsXeBuom == false && x.IsNguyenLieuXeBuom == false)
                                .OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                .Select(x => new
                                {
                                    Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                                    Min = $"{x.Min ?? 0.0}", Max = $"{x.Max ?? 9999.0}"
                                }).ToList();
                            var jsonfl = new
                            {
                                data = itemfl,
                                total = itemfl.Count,
                                PageIndex
                            };
                            return Ok(jsonfl);
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:

                        {
                            if (mainService.VmApp.ComName == nameof(ComNames.NV))
                            {
                                var xiNghiep = mainService.VmXiNghiep.Items.FirstOrDefault(x => x.Ma == mayCan.MaXuong);
                                if (xiNghiep != null)
                                {
                                    var itemdinhhinhs = mainService.VmThanhPhamDinhHinh.Items.Where(x => (x.SuDung == true && x.CodeId == xiNghiep.CodeId )|| (x.CodeId != null && x.CodeId.Trim() =="" && x.SuDung == true ))
                                        .OrderByDescending(x => x.Ten).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                        .Select(x => new
                                        {
                                            Id = x.Ma, Ten = x.Ten, x.SuDung,
                                            MNgay = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min ?? 0.0}",
                                            Max = $"{x.Max ?? 9999.0}"
                                        }).ToList();
                                    var jsondinhhinh = new
                                    {
                                        data = itemdinhhinhs,
                                        total = itemdinhhinhs.Count,
                                        PageIndex
                                    };
                                    return Ok(jsondinhhinh);
                                }

                            }
                            else
                            {
                                var itemdinhhinhs = mainService.VmThanhPhamDinhHinh.Items.Where(x => x.SuDung == true)
                                    .OrderByDescending(x => x.Ten).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                    .Select(x => new
                                    {
                                        Id = x.Ma, Ten = x.Ten, x.SuDung,
                                        MNgay = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min ?? 0.0}",
                                        Max = $"{x.Max ?? 9999.0}"
                                    }).ToList();
                                var jsondinhhinh = new
                                {
                                    data = itemdinhhinhs,
                                    total = itemdinhhinhs.Count,
                                    PageIndex
                                };
                                return Ok(jsondinhhinh);
                            }
                            
                            
                           
                        }
                            
                            break;
                        case AppKV.XepKhuon:
                            var itemXks = mainService.VmThanhPhamChinhXepKhuon.Items.Where(x => x.SuDung == true)
                                .OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                .Select(x => new
                                {
                                    Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                                    Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList();
                            var jsoChinh = new
                            {
                                data = itemXks,
                                total = itemXks.Count,
                                PageIndex
                            };
                            return Ok(jsoChinh);
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            goto case AppKV.XepKhuon;
                        case AppKV.XepKhuonBlock:
                            goto case AppKV.XepKhuon;
                        case AppKV.XepKhuonKXL:
                            goto case AppKV.XepKhuonPhu;
                        case AppKV.XepKhuonPhu:
                            var itemphs = mainService.VmThanhPhamPhuXepKhuon.Items.Where(x => x.SuDung == true)
                                .OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                .Select(x => new
                                {
                                    Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                                    Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList();
                            var jsonph = new
                            {
                                data = itemphs,
                                total = itemphs.Count,
                                PageIndex
                            };
                            return Ok(jsonph);
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var itemHqs = mainService.VmThanhPhamHq.Items.Where(x => x.SuDung == true)
                                .OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize)
                                .Select(x => new
                                {
                                    Id = x.Id, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                                    Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList();
                            var json = new
                            {
                                data = itemHqs,
                                total = itemHqs.Count,
                                PageIndex
                            };

                            return Ok(json);
                            break;
                    }
                }
            }

            return NotFound("Not Found");
            //var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            //if (index == -1)
            //{
            //    DateTime date = new DateTime();
            //    try
            //    {
            //        date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //    var items = mainService.VmThanhPhamHq.Items.Where(x=>x.MNgay.Date >= date.Date).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay =$"{x.MNgay.ToString("yyyyMMddHHmmss")}",Min = $"{x.Min}",Max = $"{x.Max}"}).ToList();
            //    var json = new
            //    {
            //        data = items,
            //        total = items.Count,
            //        PageIndex 
            //    };

            //    return Ok(json);
            //}
            //else
            //{
            //    var _data = Ngay.Substring(index + 1).Split(',');
            //    var mayCanId = _data.FirstOrDefault() ?? "";
            //    var mayCan = mayCansService.Find(mayCanId);
            //    if (mayCan != null)
            //    {
            //        switch (mayCan.WKv)
            //        {
            //            case AppKV.Main:
            //                break;
            //            case AppKV.DauAo:
            //                break;
            //            case AppKV.NguyenLieu:
            //                break;
            //            case AppKV.BTPFillet:
            //                break;
            //            case AppKV.TPFillet:
            //                break;
            //            case AppKV.BTPFilletv2:
            //                break;
            //            case AppKV.TPFilletv2:
            //                break;
            //            case AppKV.PhuPham:
            //                break;
            //            case AppKV.BTPDinhHinh:

            //            case AppKV.TPDinhHinh:
            //                var itemdinhhinhs = mainService.VmThanhPhamDinhHinh.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.Ten).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Ma,Ten=x.Ten,x.SuDung,MNgay =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}",Min = $"{x.Min}",Max = $"{x.Max}"}).ToList();
            //                var jsondinhhinh = new
            //                {
            //                    data = itemdinhhinhs,
            //                    total = itemdinhhinhs.Count,
            //                    PageIndex
            //                };
            //                return Ok(jsondinhhinh);
            //                break;
            //            case AppKV.XepKhuon:
            //                break;
            //            case AppKV.BaoTu:
            //                break;
            //            case AppKV.CaoThit:
            //                break;
            //            case AppKV.XepKhuonRaCoi:
            //                break;
            //            case AppKV.XepKhuonPhu:
            //                break;
            //            case AppKV.XepKhuonKXL:
            //                break;
            //            case AppKV.PhuPhamv2:
            //                break;
            //            case AppKV.Hq:
            //                var itemHqs = mainService.VmThanhPhamHq.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay =$"{x.MNgay.ToString("yyyyMMddHHmmss")}",Min = $"{x.Min}",Max = $"{x.Max}"}).ToList();
            //                var json = new
            //                {
            //                    data = itemHqs,
            //                    total = itemHqs.Count,
            //                    PageIndex
            //                };

            //                return Ok(json);
            //                break;
            //        }
            //    }
            //}

            ////switch (Ngay)
            ////{
            ////    case nameof( AppKV.Main):
            ////        break;
            ////    case nameof(AppKV.DauAo):
            ////        break;
            ////    case nameof(AppKV.NguyenLieu):
            ////        break;
            ////    case nameof(AppKV.BTPFillet):
            ////        break;
            ////    case nameof(AppKV.TPFillet):
            ////        break;
            ////    case nameof(AppKV.BTPFilletv2):
            ////        break;
            ////    case nameof(AppKV.TPFilletv2):
            ////        break;
            ////    case nameof(AppKV.PhuPham):
            ////        break;
            ////    case nameof(AppKV.BTPDinhHinh):
            ////        break;
            ////    case nameof(AppKV.TPDinhHinh):
            ////        break;
            ////    case nameof(AppKV.XepKhuon):
            ////        break;
            ////    case nameof(AppKV.BaoTu):
            ////        break;
            ////    case nameof(AppKV.CaoThit):
            ////        break;
            ////    case nameof(AppKV.XepKhuonRaCoi):
            ////        break;
            ////    case nameof(AppKV.XepKhuonPhu):
            ////        break;
            ////    case nameof(AppKV.XepKhuonKXL):
            ////        break;
            ////    case nameof(AppKV.PhuPhamv2):
            ////        break;
            ////    case nameof(AppKV.Hq):
            ////        var items = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
            ////        var json = new
            ////        {
            ////            data = items.Select(x=> new {x.Id,MNgay=x.MNgay.ToString("yyyyMMdd"),x.SuDung,NgayNguyenLieu=$"{x.MNgay.ToString("yyyyMMddHHmmss")}"}),
            ////            total = items.Count,
            ////            PageIndex 
            ////        };

            ////        return Ok(json);
            ////        break;
            ////}
            //return NotFound("Not Found");
        }

        [HttpGet]
        public IActionResult GetUs(string Ngay, int PageIndex, int PageSize)
        {
            //var items = mainService.VmThanhPhamHq.GetUs(Ngay, PageIndex, PageSize);
            //var json = new
            //{
            //    data = items.Select(x=>new {Id=x.ThanhPhamId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",Min = $"{x.Min}",Max = $"{x.Max}"}).ToList(),
            //    total = items.Count,
            //    PageIndex 
            //};

            //return Ok(json);
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
                var items = mainService.VmThanhPhamHq.GetUs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x => new
                    {
                        Id = x.ThanhPhamId, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                        Min = $"{x.Min}", Max = $"{x.Max}"
                    }).ToList(),
                    total = items.Count,
                    PageIndex
                };

                return Ok(json);
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    switch (mayCan.WKv)
                    {
                        case AppKV.Main:
                            break;
                        case AppKV.DauAo:
                            break;
                        case AppKV.NguyenLieu:
                            var itemNL = mainService.VmThanhPhamNguyenLieu.GetUs(mNgay, PageIndex, PageSize);
                            var jsonNL = new
                            {
                                data = itemNL.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min ?? 0.0}",
                                    Max = $"{x.Max ?? 9999.0}"
                                }).ToList(),
                                total = itemNL.Count,
                                PageIndex
                            };
                            return Ok(jsonNL);
                            break;
                        //case AppKV.BTPFillet:
                        //case AppKV.TPFillet:
                        //case AppKV.BTPFilletv2:
                        case AppKV.BTPFillet:
                            goto case AppKV.TPFilletv2;
                        case AppKV.TPFillet:
                        {
                            if (mainService.VmApp.ComName?.ToUpper().Trim() == nameof(ComNames.HL))
                            {
                                var itemfltp = mainService.VmThanhPhamFillet.GetUs(mNgay, PageIndex, PageSize)
                                    .Where(x => x.IsXeBuom == true).ToList();
                                var jsonfltp = new
                                {
                                    data = itemfltp.Select(x => new
                                    {
                                        Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                        MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min ?? 0.0}",
                                        Max = $"{x.Max ?? 9999.0}"
                                    }).ToList(),
                                    total = itemfltp.Count,
                                    PageIndex
                                };
                                return Ok(jsonfltp);
                            }
                            else
                            {
                                goto case AppKV.TPFilletv2;
                            }
                        }
                        case AppKV.BTPFilletv2:
                            goto case AppKV.TPFilletv2;
                        case AppKV.TPFilletv2:
                            var itemfl = mainService.VmThanhPhamFillet.GetUs(mNgay, PageIndex, PageSize).Where(x => x.IsXeBuom == false).ToList();
                            var jsonfl = new
                            {
                                data = itemfl.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min ?? 0.0}",
                                    Max = $"{x.Max ?? 9999.0}"
                                }).ToList(),
                                total = itemfl.Count,
                                PageIndex
                            };
                            return Ok(jsonfl);
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            //var itemdinhhinhs = mainService.VmSizeDinhHinh.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.Ten).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Ma,Ten=x.Ten,x.SuDung,MNgay =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}"}).ToList();
                            //var jsondinhhinh = new
                            //{
                            //    data = itemdinhhinhs,
                            //    total = itemdinhhinhs.Count,
                            //    PageIndex
                            //};
                            //return Ok(jsondinhhinh);
                            break;
                        case AppKV.XepKhuon:
                            var itemXks = mainService.VmThanhPhamChinhXepKhuon.GetUs(mNgay, PageIndex, PageSize);
                            var jsoChinh = new
                            {
                                data = itemXks.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = itemXks.Count,
                                PageIndex
                            };
                            return Ok(jsoChinh);
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;

                        case AppKV.XepKhuonKXL:
                            goto case AppKV.XepKhuonPhu;
                        case AppKV.XepKhuonPhu:
                            var itemphs = mainService.VmThanhPhamPhuXepKhuon.GetUs(mNgay, PageIndex, PageSize);
                            var jsonph = new
                            {
                                data = itemphs.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = itemphs.Count,
                                PageIndex
                            };
                            return Ok(jsonph);
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmThanhPhamHq.GetUs(mNgay, PageIndex, PageSize);
                            var json = new
                            {
                                data = items.Select(x => new
                                {
                                    Id = x.ThanhPhamId, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = items.Count,
                                PageIndex
                            };
                            return Ok(json);
                            break;
                    }
                }
            }

            return NotFound("Not Found");
        }

        [HttpGet]
        public IActionResult GetDs(string Ngay, int PageIndex, int PageSize)
        {
            //var items = mainService.VmThanhPhamHq.GetDs(Ngay, PageIndex, PageSize);
            //var json = new
            //{
            //    data = items.Select(x=>new {Id=x.ThanhPhamId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",Min = $"{x.Min}",Max = $"{x.Max}"}).ToList(),
            //    total = items.Count,
            //    PageIndex 
            //};

            //return Ok(json);
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
                var items = mainService.VmThanhPhamHq.GetDs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x => new
                    {
                        Id = x.ThanhPhamId, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}",
                        Min = $"{x.Min}", Max = $"{x.Max}"
                    }).ToList(),
                    total = items.Count,
                    PageIndex
                };

                return Ok(json);
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    switch (mayCan.WKv)
                    {
                        case AppKV.Main:
                            break;
                        case AppKV.DauAo:
                            break;
                        case AppKV.NguyenLieu:
                            var itemNL = mainService.VmThanhPhamNguyenLieu.GetDs(mNgay, PageIndex, PageSize);
                            var jsonNL = new
                            {
                                data = itemNL.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = itemNL.Count,
                                PageIndex
                            };
                            return Ok(jsonNL);
                            break;
                        //case AppKV.BTPFillet:
                        //case AppKV.TPFillet:
                        //case AppKV.BTPFilletv2:
                        case AppKV.BTPFillet:
                            goto case AppKV.TPFilletv2;
                        case AppKV.TPFillet:
                        {
                            if (mainService.VmApp.ComName?.ToUpper().Trim() == nameof(ComNames.HL))
                            {
                                var itemfltp = mainService.VmThanhPhamFillet.GetDs(mNgay, PageIndex, PageSize)
                                    .Where(x => x.IsXeBuom == true).ToList();
                                var jsonfltp = new
                                {
                                    data = itemfltp,
                                    total = itemfltp.Count,
                                    PageIndex
                                };
                                return Ok(jsonfltp);
                            }
                            else
                            {
                                goto case AppKV.TPFilletv2;
                            }
                        }
                        case AppKV.BTPFilletv2:
                            goto case AppKV.TPFilletv2;
                        case AppKV.TPFilletv2:
                            var itemfl = mainService.VmThanhPhamFillet.GetDs(mNgay, PageIndex, PageSize).Where(x => x.IsXeBuom == false).ToList();
                            var jsonfl = new
                            {
                                data = itemfl.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = itemfl.Count,
                                PageIndex
                            };
                            return Ok(jsonfl);
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            //var itemdinhhinhs = mainService.VmSizeDinhHinh.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.Ten).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Ma,Ten=x.Ten,x.SuDung,MNgay =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}"}).ToList();
                            //var jsondinhhinh = new
                            //{
                            //    data = itemdinhhinhs,
                            //    total = itemdinhhinhs.Count,
                            //    PageIndex
                            //};
                            //return Ok(jsondinhhinh);
                            break;
                        case AppKV.XepKhuon:
                            var itemXks = mainService.VmThanhPhamChinhXepKhuon.GetDs(mNgay, PageIndex, PageSize);
                            var jsoChinh = new
                            {
                                data = itemXks.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = itemXks.Count,
                                PageIndex
                            };
                            return Ok(jsoChinh);
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;

                        case AppKV.XepKhuonKXL:
                            goto case AppKV.XepKhuonPhu;
                        case AppKV.XepKhuonPhu:
                            var itemphs = mainService.VmThanhPhamPhuXepKhuon.GetDs(mNgay, PageIndex, PageSize);
                            var jsonph = new
                            {
                                data = itemphs.Select(x => new
                                {
                                    Id = x.MaThanhPham, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = itemphs.Count,
                                PageIndex
                            };
                            return Ok(jsonph);
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmThanhPhamHq.GetDs(mNgay, PageIndex, PageSize);
                            var json = new
                            {
                                data = items.Select(x => new
                                {
                                    Id = x.ThanhPhamId, Ten = x.Ten, x.SuDung,
                                    MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}", Min = $"{x.Min}", Max = $"{x.Max}"
                                }).ToList(),
                                total = items.Count,
                                PageIndex
                            };
                            return Ok(json);
                            break;
                    }
                }
            }

            return NotFound("Not Found");
        }
    }
}