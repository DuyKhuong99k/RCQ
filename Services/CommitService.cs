using System.Globalization;
using Models.Repos.Models;
using Newtonsoft.Json;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services;

public class CommitService(IMayCansService mayCansService, ICoiService coiService, IMainService mainService)
    : ICommitService
{
    private readonly object _lockItems = new();

    #region ICommitService Members

    public Tuple<bool, string> Commit(List<DataCoummunication> datas, ClientInfo client, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";
        var kv = client.WKv;
        switch (kv)
        {
            case AppKV.Main:
                break;
            case AppKV.DauAo:

                break;
            case AppKV.NguyenLieu:
                {
                }
                break;
            case AppKV.BTPFillet:
                break;
            case AppKV.TPFillet:

                break;
            case AppKV.BTPFilletv2:
                break;
            case AppKV.TPFilletv2:
                break;
            case AppKV.PhuPham:
                break;
            case AppKV.BTPDinhHinh:
                break;
            case AppKV.TPDinhHinh:
                break;
            case AppKV.XepKhuon:
                break;
            case AppKV.BaoTu:
                break;
            case AppKV.CaoThit:
                break;
            case AppKV.XepKhuonRaCoi:
            case AppKV.XepKhuonPhu:
            case AppKV.XepKhuonKXL:

            default:
                error.ErrorString = "KHU VỰC KHÔNG XÁC ĐỊNH";
                break;
        }


        if (isOK == false)
            dataErrorJson = JsonConvert.SerializeObject(error);

        return new Tuple<bool, string>(isOK, dataErrorJson);
    }

    public Tuple<bool, string> Commit(string dataJson, ClientInfo client, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";
        var kv = client.WKv;

        switch (kv)
        {
            case AppKV.BTPDinhHinh:
                {
                    var data = JsonConvert.DeserializeObject<DataCoummunication>(dataJson);
                    if (data == null)
                    {
                        error.ErrorString = "Data không đúng chuẩn";
                        error.Cmd = cmd;
                    }
                    else
                    {
                        if (data.TheId != null)
                        {
                            var phieuCanLast =
                                mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(DateTime.Now,
                                    data.TheId);
                            if (phieuCanLast != null && phieuCanLast.IsEnabled == false)
                            {
                                error.ErrorString = "RỔ CÁ CHƯA SỬA";
                            }
                            else
                            {
                                var item = mainService.VmPhieuCanBTPDinhHinh.CreateDefaultNew(client.Id, data);
                                var rl = mainService.VmPhieuCanBTPDinhHinh.Insert(item);
                                if (rl > 0)
                                {
                                    mainService.VmPhieuCanBTPDinhHinh.Items.Add(item);
                                    isOK = true;
                                    var _data = mainService.VmPhieuCanBTPDinhHinh.Convert(item);
                                    dataErrorJson = JsonConvert.SerializeObject(_data);
                                }
                                else
                                {
                                    error.ErrorString = "KHÔNG THỂ THÊM PHIẾU CÂN";
                                }
                            }
                        }
                    }


                    break;
                }
            case AppKV.TPDinhHinh:
                {
                    var data = JsonConvert.DeserializeObject<DataCoummunication>(dataJson);
                    if (data == null)
                    {
                        error.ErrorString = "Data không đúng chuẩn";
                        error.Cmd = cmd;
                    }
                    else
                    {
                        if (data.TheId != null)
                        {
                            var phieuCanmBTP =
                                mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(DateTime.Now,
                                    data.TheId, false);
                            if (phieuCanmBTP == null)
                            {
                                error.ErrorString = "CHƯA CÂN ĐẦU VÀO";
                            }
                            else
                            {
                                var item = mainService.VmPhieuCanTPDinhHinh.CreateDefaultNew(client.Id, phieuCanmBTP, data);
                                phieuCanmBTP.IsEnabled = true;
                                var uprl = mainService.VmPhieuCanBTPDinhHinh.Update(phieuCanmBTP);
                                if (uprl > 0)
                                {
                                    var itemBTP = mainService.VmPhieuCanBTPDinhHinh.Items
                                        .FirstOrDefault(x => x.Id == phieuCanmBTP.Id && x.Id != null && x.Id.Trim() != "");
                                    itemBTP.IsEnabled = true;
                                    var rl = mainService.VmPhieuCanTPDinhHinh.Insert(item);
                                    if (rl > 0)
                                    {
                                        mainService.VmPhieuCanTPDinhHinh.Items.Add(item);
                                        isOK = true;
                                        var _data = mainService.VmPhieuCanTPDinhHinh.Convert(item);
                                        dataErrorJson = JsonConvert.SerializeObject(_data);
                                    }
                                }
                                else
                                {
                                    error.ErrorString = "KHÔNG CẬP NHẬT PHIẾU CÂN BTP";
                                }
                            }
                        }
                    }

                    break;
                }
            case AppKV.Main:
                break;
            case AppKV.DauAo:
                break;
            case AppKV.NguyenLieu:
                break;
            case AppKV.BTPFillet:
                break;
            case AppKV.TPFillet:
                break;
            case AppKV.BTPFilletv2:
                break;
            case AppKV.TPFilletv2:
                break;
            case AppKV.PhuPham:
                break;
            case AppKV.XepKhuon:
                {
                    break;
                }
            case AppKV.BaoTu:
                break;
            case AppKV.CaoThit:
                break;
            default:
                error.ErrorString = "KHU VỰC KHÔNG XÁC ĐỊNH";
                break;
        }

        //error.ErrorString = "Không Xác Định";
        if (isOK == false)
            dataErrorJson = JsonConvert.SerializeObject(error);

        return new Tuple<bool, string>(isOK, dataErrorJson);
    }

    public Tuple<bool, string> CheckTheId(string dataJson, AppKV kv, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";
        switch (kv)
        {
            case AppKV.BTPDinhHinh:
                {
                    var data = JsonConvert.DeserializeObject<DataCoummunication>(dataJson);
                    if (data == null)
                    {
                        error.ErrorString = "Data không đúng chuẩn";
                        error.Cmd = cmd;
                    }
                    else
                    {
                        var phieuCanLast =
                            mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(DateTime.Now, data.TheId);
                        if (phieuCanLast != null && phieuCanLast.IsEnabled == false)
                            error.ErrorString = "RỔ CÁ CHƯA SỬA";
                        else
                            isOK = true;
                    }

                    break;
                }
            case AppKV.TPDinhHinh:
                {
                    var data = JsonConvert.DeserializeObject<DataCoummunication>(dataJson);
                    if (data == null)
                    {
                        error.ErrorString = "Data không đúng chuẩn";
                        error.Cmd = cmd;
                    }
                    else
                    {
                        var phieuCanLast =
                            mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(DateTime.Now, data.TheId,
                                false);
                        if (phieuCanLast == null)
                        {
                            error.ErrorString = "CHƯA CÂN ĐẦU VÀO";
                        }
                        else
                        {
                            var _data = mainService.VmPhieuCanBTPDinhHinh.Convert(phieuCanLast);
                            dataErrorJson = JsonConvert.SerializeObject(_data);
                            isOK = true;
                        }
                    }

                    break;
                }
        }

        //error.ErrorString = "Không Xác Định";
        if (isOK == false)
            dataErrorJson = JsonConvert.SerializeObject(error);

        return new Tuple<bool, string>(isOK, dataErrorJson);
    }
    public async Task<Tuple<bool, string>> SetChiSanLuong(string dataJson, ClientInfo client, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";

        try
        {
            var dataLite = JsonConvert.DeserializeObject<SetChiSanLuong>(dataJson);
            if (dataLite != null)
            {
                Console.WriteLine(dataJson);
                var maycan = mayCansService.Find(client.Id);
                if (maycan != null)
                {
                    maycan.IsChiSangLuong = Convert.ToBoolean(dataLite.ChiSanLuong);
                    isOK = true;
                }
                else
                {
                    error.ErrorString = "Không Tìm Thấy Máy Cân";
                }
            }
            else
            {
                error.ErrorString = "Dữ liệu không hợp lệ";
            }
        }
        catch (Exception ex)
        {
            error.ErrorString = $"Lỗi: {ex.Message}";
        }

        if (isOK == false)
            dataErrorJson = JsonConvert.SerializeObject(error);

        return new Tuple<bool, string>(isOK, dataErrorJson);
    }

    public async Task<Tuple<bool, string>> SetXacDinh(string dataJson, ClientInfo client, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";

        try
        {
            var dataLite = JsonConvert.DeserializeObject<SetXacDinh>(dataJson);
            if (dataLite != null)
            {
                Console.WriteLine(dataJson);
                var maycan = mayCansService.Find(client.Id);
                if (maycan != null)
                {
                    maycan.IsXacDinhLoaiThanhPham = Convert.ToBoolean(dataLite.XacDinh);
                    isOK = true;
                }
                else
                {
                    error.ErrorString = "Không Tìm Thấy Máy Cân";
                }
            }
            else
            {
                error.ErrorString = "Dữ liệu không hợp lệ";
            }
        }
        catch (Exception ex)
        {
            error.ErrorString = $"Lỗi: {ex.Message}";
        }

        if (isOK == false)
            dataErrorJson = JsonConvert.SerializeObject(error);

        return new Tuple<bool, string>(isOK, dataErrorJson);
    }
    public async Task<Tuple<bool, string>> CommitCoi(string dataJson, ClientInfo client, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";
        //if (cmd == "COIINSETIDMONITOR")
        //{
        //    var data = JsonConvert.DeserializeObject<DataCoi>(dataJson);
        //    if (data != null)
        //    {
        //        var coi = coiService.Find(data.MaCoi, data.MaXuong);
        //        if (coi != null)
        //        {
        //            coi.IdMonitor = data.IdMonitor;
        //            coi.TrongLuong = data.TrongLuong;

        //            isOK = true;
        //            dataErrorJson = JsonConvert.SerializeObject(coi);
        //        }
        //        else
        //        {
        //            error.ErrorString = "Không Tìm Thấy Cối";
        //        }
        //    }
        //}
        //else if (cmd == "COIGETLITEREPORT")
        //{
        //    var data = JsonConvert.DeserializeObject<DataCoi>(dataJson);
        //    if (data != null)
        //    {
        //        var coi = coiService.Find(data.MaCoi, data.MaXuong);
        //        if (coi != null)
        //            try
        //            {
        //                var reports =
        //                    mainService.VmPhieuCanChinhXepKhuon.GetLiteReport<object>(coi.IdMonitor ?? "null");
        //                lock (coi.LiteReports)
        //                {
        //                    coi.LiteReports.Clear();
        //                    coi.LiteReports.AddRange(reports);
        //                }

        //                isOK = true;
        //                dataErrorJson = JsonConvert.SerializeObject(coi);
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e);
        //                //throw;
        //            }
        //        else
        //            error.ErrorString = "Không Tìm Thấy Cối";
        //    }
        //}
        //if()
        //else
        //{
        //    error.ErrorString = "Không Có CMD";
        //}

        if (isOK == false) dataErrorJson = JsonConvert.SerializeObject(error);
        coiService.NotifyItemChanged();
        return new Tuple<bool, string>(isOK, dataErrorJson);
    }

    /// <summary>
    ///     Check Lite
    /// </summary>
    /// <param name="dataJson"></param>
    /// <param name="client"></param>
    /// <param name="cmd"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Tuple<bool, string>> CommitLite(string dataJson, ClientInfo client, string cmd)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        var isOK = false;
        var dataErrorJson = "";

        var dataLite = JsonConvert.DeserializeObject<DataLite>(dataJson);

        if (dataLite != null)
        {
            var maycan = mayCansService.Find(client.Id);
            try
            {
                var isChuyenFillet = false;
                // var isHuman = true;
                if (maycan != null)
                {
                    if (maycan.IsNapThe)
                    {
                        maycan.NapTheId = dataLite.TheId;
                        var dataRes = new DataLiteRes
                        {
                            TheId = dataLite.TheId,
                            IsDataAction = 0,
                            MessStr = "Nạp Thẻ"
                        };
                        error.ErrorString = dataRes.MessStr;
                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                        isOK = true;
                    }
                    else
                    {
                        //Set update time to board
                        try
                        {
                            var boardTime =
                                DateTime.ParseExact(dataLite.NgayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            if (Math.Abs((boardTime - DateTime.Now).TotalSeconds) > 5)
                                await mayCansService.CommandSetUpdateTime(maycan.Id);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            //throw;
                        }


                        maycan.ColorString = null;
                        maycan.ThongBao = null;
                        maycan.TrongLuong = dataLite.TrongLuong;
                        maycan.TheView = dataLite.TheId;
                        maycan.ErrorString = null;
                        try
                        {
                            if (maycan.TheAddDateTime != null && maycan.TheView == dataLite.Id)
                            {
                                var timout =
                                    (DateTime.Now - maycan.TheAddDateTime)?.TotalMilliseconds;
                                if (timout > 0 && timout < 500)
                                {
                                    isOK = true;

                                    return new Tuple<bool, string>(isOK, dataErrorJson);
                                }
                            }

                            maycan.TheAddDateTime = DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            // throw;
                        }

                        if (dataLite.Stated == "ST" && dataLite.TrongLuong > 0)
                        {
                            maycan.IsStated = true;
                        }
                        else
                        {
                            if (mainService.VmApp.ComName.ToUpper().Trim() ==
                                nameof(ComNames.HL) && dataLite.TrongLuong > 5 && (
                                    maycan.WKv == AppKV.TPFilletv2 ||
                                    maycan.WKv == AppKV.BTPFilletv2))
                                maycan.IsStated = true;
                            else
                                maycan.IsStated = false;
                        }


                        maycan.TrongLuongTare = dataLite.TrongLuongTare;
                        if (mainService.VmApp.ComName?.ToUpper().Trim() == nameof(ComNames.HL) &&
                            maycan.WKv == AppKV.TPFillet && maycan.MaThanhPham != null &&
                            maycan.MaThanhPham.Trim() != "")
                        {
                            var thanhPham =
                                mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                    x.Ma == maycan.MaThanhPham);
                            if (thanhPham != null && thanhPham.IsChuyenFillet) isChuyenFillet = true;
                        }

                        if (dataLite.TheId.Trim() == "" &&
                            (maycan.WKv == AppKV.XepKhuon || maycan.WKv == AppKV.XepKhuonRaCoi))
                        {
                            maycan.TheNhanVien = "0";
                            maycan.TheNhanVienAddDateTime = DateTime.Now;
                            maycan.MaNhanVien = "PMS";
                        }
                        else
                        {
                            if (dataLite.IsWaiting == false)
                            {
                                if (maycan.WKv == AppKV.TPDinhHinh && maycan.AType == AppType._default)
                                {
                                    maycan.TheRo = dataLite.TheId;
                                    maycan.TheRoAddDateTime = DateTime.Now;
                                }
                                else
                                {
                                    if (maycan.The != null && maycan.The == dataLite.Id)
                                    {
                                        var timems = Math.Abs(
                                            (maycan.TheRoAddDateTime - DateTime.Now)?.TotalMilliseconds ??
                                            0);
                                        if (timems > 0 && timems < 1000)
                                        {
                                            error.ErrorString = "OK";
                                            dataErrorJson = JsonConvert.SerializeObject(error);
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                            return new Tuple<bool, string>(isOK, dataErrorJson);
                                        }
                                    }

                                    var theThanhPham =
                                        mainService.VmTheThanhPham.Items.FirstOrDefault(x => x.MaThe == dataLite.TheId);
                                    if (theThanhPham != null)
                                    {
                                        var rl = await CheckTheThanhPham(theThanhPham, maycan);
                                        isOK = rl.Item1;
                                        error.ErrorString = rl.Item2;
                                        maycan.TheRo = null;
                                        maycan.TheRoAddDateTime = null;
                                        maycan.TheNhanVien = null;
                                        maycan.MaHoSo = null;
                                        maycan.TheNhanVienAddDateTime = null;
                                        maycan.NhanVienName = null;
                                    }
                                    else
                                    {
                                        var theRo = mainService.VmTheRo.Items.FirstOrDefault(x =>
                                            x.MaThe == dataLite.TheId);
                                        if (theRo != null)
                                        {
                                            var resetNhanVien = false;
                                            if (maycan.TheNhanVienAddDateTime != null)
                                            {
                                                var timeOutTheNhanVien = (DateTime.Now - maycan.TheNhanVienAddDateTime)
                                                    ?.TotalMilliseconds ?? 0;
                                                if (timeOutTheNhanVien > 600000)
                                                {
                                                    resetNhanVien = true;
                                                }
                                            }

                                            if (mainService.VmApp.ComName != nameof(ComNames.DAITHANH) ||
                                                resetNhanVien == true)
                                            {
                                                maycan.TheNhanVien = null;
                                                maycan.MaHoSo = null;
                                                maycan.TheNhanVienAddDateTime = null;
                                                maycan.NhanVienName = null;
                                                maycan.MaNhanVien = null;
                                            }

                                            maycan.IsKhongTheDauVao = false;
                                            if (maycan.IsSuDungMauThanhPham && maycan.WKv != AppKV.TPDinhHinh)
                                            {
                                                var rlMauThe = CheckMauThe(theRo.ColorCode, maycan);
                                                //isOK = rlMauThe.Item1;
                                                error.ErrorString = rlMauThe.Item2;
                                            }

                                            maycan.TheRo = theRo.MaThe;
                                            maycan.TheRoAddDateTime = DateTime.Now;
                                        }
                                        else
                                        {
                                            var theNhanvien =
                                                mainService.VmThe.Items.FirstOrDefault(x =>
                                                    x.MaTheTu == dataLite.TheId);
                                            if (theNhanvien == null)
                                            {
                                                error.ErrorString = "Không Tìm Thấy Thẻ";
                                                maycan.ColorString = "red";
                                                //InsertLogLoiCan(maycan,dataLite,"Không Tìm Thấy Thẻ");
                                            }
                                            else
                                            {
                                                var nhanVien =
                                                    mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                        x.MaNhanVien == theNhanvien.MaNhanVien);
                                                if (nhanVien == null)
                                                {
                                                    error.ErrorString = "Không Tìm Thấy Nhân Viên";
                                                    maycan.ColorString = "red";
                                                }
                                                else
                                                {
                                                    //await Task.Delay(100);
                                                    //await mayCansService.CommandSetNhanVien(maycan.Id, nhanVien.MaNhanVien,
                                                    //    nhanVien.MaHoSo ?? "",
                                                    //    nhanVien.Name ?? "");
                                                    //await Task.Delay(200);
                                                    if (nhanVien.IsPhucVu)
                                                    {
                                                        maycan.MaNhanVienPhucVu = nhanVien.MaNhanVien;
                                                        maycan.TheNhanVienPhucVuAddDateTime = DateTime.Now;
                                                        await mayCansService.CommandSetNhanVienPhucVu(
                                                            maycan.Id, nhanVien.MaNhanVien, nhanVien.MaHoSo);
                                                        isOK = true;
                                                    }
                                                    else
                                                    {
                                                        if (nhanVien.IsHuman)
                                                        {
                                                            if (nhanVien.IsChucNang == false)
                                                            {
                                                                maycan.MaNhanVien = nhanVien.MaNhanVien;
                                                                maycan.TheNhanVienAddDateTime = DateTime.Now;
                                                                maycan.TheNhanVien = dataLite.TheId;
                                                                maycan.MaHoSo = nhanVien.MaHoSo;
                                                                maycan.NhanVienName = nhanVien.Name;

                                                                await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                    maycan?.MaNhanVien ?? "",
                                                                    maycan.MaHoSo ?? "", maycan.NhanVienName ?? "");
                                                                await Task.Delay(150);
                                                                if (mainService.VmApp.ComName ==
                                                                    nameof(ComNames.DAITHANH) &&
                                                                    maycan.WKv == AppKV.TPDinhHinh &&
                                                                    maycan.AType == AppType._type1)
                                                                {
                                                                    maycan.TheRo = null;
                                                                    maycan.TheRoAddDateTime = null;
                                                                    var dataRes = new DataLiteRes
                                                                    {
                                                                        TheId = dataLite.TheId,
                                                                        IsDataAction = 0,
                                                                        MessStr = "QUÉT THẺ MÀU"
                                                                    };
                                                                    error.ErrorString = dataRes.MessStr;
                                                                    dataErrorJson =
                                                                        JsonConvert.SerializeObject(dataRes);
                                                                    isOK = true;
                                                                }
                                                                else
                                                                {
                                                                    error.ErrorString =
                                                                        "Quét Thẻ Rổ Sau Đó Quét Thẻ Nhân Viên";
                                                                }
                                                            }

                                                            //maycan.IsNhanVienCongCu = false;
                                                        }
                                                        else if (mainService.VmApp.ComName.ToUpper().Trim() ==
                                                                 nameof(ComNames.HL) &&
                                                                 (maycan.WKv == AppKV.TPDinhHinh ||
                                                                  maycan.WKv == AppKV.TPFilletv2))
                                                        {
                                                            if (nhanVien.IsChucNang)
                                                            {
                                                                if (maycan.IsNhanVienCongCu)
                                                                {
                                                                    maycan.IsNhanVienCongCu = false;
                                                                }
                                                                else
                                                                {
                                                                    maycan.IsNhanVienCongCu = true;
                                                                    maycan.IsKhongTheDauVao = true;
                                                                    maycan.TheRo = null;
                                                                    // if (maycan.IsNhanVienCongCu == true)
                                                                    // {
                                                                    if ((maycan.WKv == AppKV.BTPFilletv2 ||
                                                                         maycan.WKv == AppKV.BTPDinhHinh) &&
                                                                        maycan.AType == AppType._type1)
                                                                        error.ErrorString = "Quét Thẻ Rổ Để Mở Khoá";
                                                                    else if ((maycan.WKv == AppKV.TPFilletv2 ||
                                                                              maycan.WKv == AppKV.TPDinhHinh) &&
                                                                             maycan.AType == AppType._type1)
                                                                        error.ErrorString = "Chế độ không thẻ đầu vào";
                                                                    // break;
                                                                    // }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                maycan.IsNhanVienCongCu = false;
                                                                maycan.TheRo = theRo.MaThe;
                                                                maycan.TheRoAddDateTime = DateTime.Now;
                                                                maycan.TheNhanVien = null;
                                                                maycan.MaHoSo = null;
                                                                maycan.TheNhanVienAddDateTime = null;
                                                                maycan.NhanVienName = null;
                                                            }
                                                        }
                                                    }

                                                    // Set nhanvien
                                                }
                                            }

                                            // var timems = Math.Abs(
                                            //     (maycan.TheRoAddDateTime - DateTime.Now)?.TotalMilliseconds ??
                                            //     0);
                                            // if (timems > 0 && timems > 1000)
                                            // {
                                            //     //maycan.TheRo = null;
                                            //     maycan.IsKhongTheDauVao = false;
                                            // }

                                            if (maycan.AType == AppType._type1 || isChuyenFillet)
                                                maycan.TheRoAddDateTime = null;
                                        }
                                    }

                                    //if (maycan.TheRo != null && maycan.TheRo.Trim() != "" && maycan.TheRoAddDateTime != null)
                                    //{
                                    //    var timems = (DateTime.Now - maycan.TheRoAddDateTime)?.TotalMilliseconds ?? 0;
                                    //    if (timems > mainService.VmApp.CardExpired)
                                    //    {
                                    //        maycan.TheRo = null;
                                    //        maycan.TheRoAddDateTime = null;
                                    //    }
                                    //}
                                    // Thông báo thông tin thay đổi
                                }
                            }
                            else
                            {
                                if (maycan.AType != AppType._type1 && isChuyenFillet == false)
                                {
                                    maycan.TheRo = dataLite.TheId;
                                    maycan.TheRoAddDateTime = DateTime.Now;
                                    //maycan.TheNhanVien = dataLite.TheId;
                                    //maycan.TheNhanVienAddDateTime = DateTime.Now;
                                }
                                else
                                {
                                    maycan.TheNhanVien = dataLite.TheId;
                                    maycan.TheNhanVienAddDateTime = DateTime.Now;
                                }

                                //Console.WriteLine($"{dataLite.Id} - {dataLite.Stated} - {dataLite.TrongLuong}");
                            }
                        }

                        switch (maycan.WKv)
                        {
                            case AppKV.Main:
                                break;
                            case AppKV.DauAo:
                                break;
                            case AppKV.NguyenLieu:
                                {
                                    if (maycan.IsStated == false)
                                    {
                                        if (dataLite.IsWaiting == false)
                                        {
                                            maycan.The = dataLite.TheId;
                                            maycan.TheAddDateTime = DateTime.Now;
                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                            //await Task.Delay(0);
                                            await mayCansService.CommandSetWaiting(maycan.Id);
                                            //await Task.Delay(100);
                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 0,
                                                MessStr = "CHỜ CÂN BẰNG"
                                            };
                                            error.ErrorString = dataRes.MessStr;
                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            maycan.ColorString = null;
                                            isOK = true;
                                        }
                                        else
                                        {
                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                    else
                                    {
                                        if (maycan.TrongLuong < 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                            maycan.ColorString = "red";
                                        }
                                        else if (maycan.TrongLuong == 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                            maycan.ColorString = "red";
                                        }
                                        else
                                        {
                                            if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Lô";
                                                break;
                                            }

                                            if (maycan?.MaAo == null || maycan.MaAo.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Ao";
                                                break;
                                            }

                                            if (maycan?.MaNhaCungCap == null || maycan.MaNhaCungCap.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Nhà Cung Cấp";
                                                break;
                                            }

                                            if (maycan?.MaPhuongTien == null || maycan.MaPhuongTien.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Phương Tiện";
                                                break;
                                            }

                                            if (maycan?.BanId == null || maycan.BanId.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Bàn Cắt Tiết";
                                                break;
                                            }

                                            if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn size";
                                                break;
                                            }

                                            if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Thành Phẩm";
                                                break;
                                            }

                                            var thanhPham =
                                                mainService.VmThanhPhamNguyenLieu.Items.FirstOrDefault(x =>
                                                    x.Ma == maycan.MaThanhPham);
                                            if (thanhPham != null)
                                            {
                                                var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                if (dataLite.TrongLuong > thanhPhamMax)
                                                {
                                                    error.ErrorString =
                                                        $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                    break;
                                                }
                                            }

                                            if (maycan.MaNhanVien == null || maycan.MaNhanVien.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Nhân Viên";
                                                break;
                                            }

                                            var ngayGio = DateTime.Now;

                                            var phieuCan = new PhieuCanNguyenLieu
                                            {
                                                MaLoaiCa =
                                                    mainService.VmLoaiCaNguyenLieu.Items.FirstOrDefault()?.Ma ??
                                                    "",
                                                MaMau =
                                                    mainService.VmMauNguyenLieu.Items.FirstOrDefault()?.Ma ?? "",
                                                MaMayTinhCan = maycan.Id,
                                                MaSize = maycan.MaSize,
                                                MaLoaiThanhPham = maycan.MaThanhPham,
                                                MaUserCan = maycan.Id,
                                                MaXuongSanXuat = maycan.MaXuong,
                                                Ngay = ngayGio.Date,
                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                TrongLuong = dataLite.TrongLuong,
                                                ThoiGianCan = ngayGio,
                                                MSL = maycan.MaLo,
                                                SuDung = true,
                                                MaAo = maycan.MaAo ?? "",
                                                MaPhuongTien = maycan.MaPhuongTien ?? "",
                                                Chuyen = maycan.ChuyenNL ?? 0,
                                                MaBanCatTiet = maycan.BanId ?? "",
                                                NhaCC = maycan.MaNhaCungCap ?? "",
                                                Pheu = maycan.Pheu ?? "",
                                                TrongLuongOrg = maycan.TrongLuong,
                                                TyLeNuoc = maycan.TyLeNuoc
                                            };

                                            if (mainService.VmPhieuCanNguyenLieu.Insert(phieuCan) > 0)
                                            {
                                                maycan.TongSoRo++;
                                                maycan.TongTrongLuong += dataLite.TrongLuong;

                                                lock (_lockItems)
                                                {
                                                    maycan.Items.Insert(0, phieuCan);
                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                        maycan.Items.Remove(maycan.Items.Last());
                                                }

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    TrongLuongYeuCau = 0,
                                                    Id = dataLite.Id,
                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                    MaLo = maycan.MaLo,
                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                    MaNhanVienBanKiem = "",
                                                    MaSize = maycan.MaSize,
                                                    MaThanhPham = maycan.MaThanhPham,
                                                    MayCan = maycan.Id,
                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                    STT = 0,
                                                    Status = 0,
                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                    TheIdChucNang = "",
                                                    TrongLuongTra = 0,
                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                    TongSoRo = maycan.TongSoRo,
                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                    TongSoRoCan = maycan.TongSoRo,
                                                    MaNhanVien = maycan.MaNhanVien
                                                };
                                                error.ErrorString = dataRes.MessStr;
                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                maycan.ColorString = "greenyellow";
                                                isOK = true;
                                            }
                                            else
                                            {
                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                maycan.ColorString = "red";
                                            }

                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                }
                                break;
                            case AppKV.BTPFillet:

                                {
                                    break;
                                }
                            case AppKV.TPFillet:
                                {
                                    if (isChuyenFillet) goto case AppKV.BTPFilletv2;

                                    switch (maycan.AType)
                                    {
                                        case AppType._default:
                                            break;
                                        case AppType._type1:
                                            break;
                                        case AppType._type2:
                                            break;
                                        case AppType._type3:
                                            break;
                                        case AppType._type4:
                                            {
                                                if (maycan.IsStated == false)
                                                {
                                                    if (dataLite.IsWaiting == false)
                                                    {
                                                        // maycan.The = dataLite.TheId;
                                                        // maycan.TheAddDateTime = DateTime.Now;
                                                        // //error.ErrorString = "CHỜ CÂN BẰNG";
                                                        // //await Task.Delay(0);
                                                        // await mayCansService.CommandSetWaiting(maycan.Id);
                                                        // //await Task.Delay(100);
                                                        // var dataRes = new DataLiteRes
                                                        // {
                                                        //     TheId = dataLite.TheId,
                                                        //     IsDataAction = 0,
                                                        //     MessStr = "CHỜ CÂN BẰNG"
                                                        // };
                                                        // error.ErrorString = dataRes.MessStr;
                                                        // dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        // maycan.ColorString = null;
                                                        // isOK = true;
                                                        error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                                else
                                                {
                                                    if (maycan.TrongLuong < 0)
                                                    {
                                                        error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                        maycan.ColorString = "red";
                                                    }
                                                    else if (maycan.TrongLuong == 0)
                                                    {
                                                        error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                        maycan.ColorString = "red";
                                                    }
                                                    else
                                                    {
                                                        if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Lô";
                                                            break;
                                                        }

                                                        if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn size";
                                                            break;
                                                        }

                                                        if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Thành Phẩm";
                                                            break;
                                                        }

                                                        var thanhPham =
                                                            mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                x.Ma == maycan.MaThanhPham);
                                                        if (thanhPham != null)
                                                        {
                                                            var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                            if (dataLite.TrongLuong > thanhPhamMax)
                                                            {
                                                                error.ErrorString =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                                break;
                                                            }
                                                        }

                                                        if (maycan.MaNhanVien == null || maycan.MaNhanVien.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Nhân Viên";
                                                            break;
                                                        }

                                                        var ngayGio = DateTime.Now;

                                                        var phieuCan = new PhieuCanTPFillet
                                                        {
                                                            MaLoaiCa =
                                                                mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ??
                                                                "",
                                                            MaMau =
                                                                mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                            MaMayTinhCan = maycan.Id,
                                                            MaNhanVien = maycan.MaNhanVien,
                                                            MaSize = maycan.MaSize,
                                                            MaLoaiThanhPham = maycan.MaThanhPham,
                                                            MaTheTu = dataLite.TheId,
                                                            MaUserCan = maycan.Id,
                                                            MaXuongSanXuat = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            TrongLuong = dataLite.TrongLuong,
                                                            ThoiGianCan = ngayGio.TimeOfDay,
                                                            HoVaTen = maycan.NhanVienName,
                                                            LoaiCan = "TP",
                                                            MSL = maycan.MaLo,
                                                            SuDung = true,
                                                            Id = dataLite.Id
                                                        };
                                                        var itemOld = maycan.Items.FirstOrDefault();
                                                        if (itemOld != null)
                                                        {
                                                            var _item = itemOld as PhieuCanTPFillet;

                                                            if (_item != null)
                                                            {
                                                                var timeout = (ngayGio.TimeOfDay - _item.ThoiGianCan)
                                                                    .TotalSeconds;
                                                                var nhanvienIdOld = _item.MaNhanVien;
                                                                if (_item.Ngay.Date == ngayGio.Date && timeout < 10 &&
                                                                    maycan.MaNhanVien == nhanvienIdOld)
                                                                {
                                                                    error.ErrorString = "RỔ CÁ VỪA CÂN";
                                                                    maycan.ColorString = "red";
                                                                    maycan.TheRo = null;
                                                                    maycan.TheNhanVien = null;
                                                                    maycan.MaNhanVien = null;
                                                                    maycan.The = null;
                                                                    maycan.TheRoAddDateTime = null;
                                                                    maycan.TheNhanVienAddDateTime = null;
                                                                    break;
                                                                }

                                                                if (mainService.VmApp.ComName.Trim() == nameof(ComNames.NV))
                                                                    if (_item.Ngay.Date == ngayGio.Date && timeout < 120 &&
                                                                        maycan.MaNhanVien == nhanvienIdOld)
                                                                    {
                                                                        error.ErrorString = "NHÂN VIÊN ĐÃ CÂN";
                                                                        maycan.ColorString = "red";
                                                                        maycan.TheRo = null;
                                                                        maycan.TheNhanVien = null;
                                                                        maycan.MaNhanVien = null;
                                                                        maycan.The = null;
                                                                        maycan.TheRoAddDateTime = null;
                                                                        maycan.TheNhanVienAddDateTime = null;
                                                                        break;
                                                                    }
                                                            }
                                                        }

                                                        if (mainService.VmPhieuCanTPFillet.Insert(phieuCan) > 0)
                                                        {
                                                            maycan.TongSoRo++;
                                                            maycan.TongTrongLuong += dataLite.TrongLuong;

                                                            // var tongtrongluong =
                                                            //     mainService.VmPhieuCanTPFillet
                                                            //         .GetSoRoTongTrongLuongByNhanVienId(ngayGio.Date,
                                                            //             maycan.MaNhanVien);
                                                            var tongtrongluong = new Tuple<int, decimal>(0, 0M);
                                                            try
                                                            {
                                                                if (thanhPham != null && thanhPham.IsNguyenLieuXeBuom)
                                                                    tongtrongluong = mainService.VmPhieuCanTPFillet
                                                                        .GetSoRoTongTrongLuongByNhanVienIdandThanhPhamId(
                                                                            ngayGio.Date,
                                                                            maycan.MaNhanVien, maycan.MaThanhPham);
                                                                else
                                                                    tongtrongluong = mainService.VmPhieuCanTPFillet
                                                                        .GetSoRoTongTrongLuongByNhanVienId(ngayGio.Date,
                                                                            maycan.MaNhanVien);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine(e);
                                                                // throw;
                                                            }

                                                            lock (_lockItems)
                                                            {
                                                                maycan.Items.Insert(0, phieuCan);
                                                                if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                    maycan.Items.Remove(maycan.Items.Last());
                                                            }

                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 1,
                                                                MessStr = "ĐÃ HOÀN THÀNH",
                                                                ChiSanLuong = 0,
                                                                TrongLuongYeuCau = 0,
                                                                Id = dataLite.Id,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                MaLo = maycan.MaLo,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                MaNhanVienBanKiem = "",
                                                                MaSize = maycan.MaSize,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MayCan = maycan.Id,
                                                                Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                STT = 0,
                                                                Status = 0,
                                                                TrongLuongNhan = dataLite.TrongLuong,
                                                                TheIdChucNang = "",
                                                                TrongLuongTra = 0,
                                                                TongTrongLuongCan = maycan.TongTrongLuong,
                                                                TongSoRo = tongtrongluong.Item1, //maycan.TongSoRo,
                                                                TongTrongLuong = tongtrongluong.Item2, //maycan.TongTrongLuong,
                                                                TongSoRoCan = maycan.TongSoRo,
                                                                MaNhanVien = maycan.MaNhanVien
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            maycan.ColorString = "greenyellow";
                                                            isOK = true;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                            }
                                            break;
                                        case AppType._type1_2:
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                                break;
                            case AppKV.BTPFilletv2:
                                {
                                    switch (maycan.AType)
                                    {
                                        case AppType._default:
                                            if (isChuyenFillet) goto case AppType._type1;

                                            break;
                                        case AppType._type1:
                                            {
                                                var isNotOut = false;
                                                if (maycan.IsNhanVienCongCu)
                                                {
                                                    var items = mainService.VmPhieuCanBtpFilletv2.Gets<PhieuCanBTPFilletv2>(
                                                        DateTime.Now, maycan.TheRo);
                                                    if (items.Any())
                                                    {
                                                        foreach (var item in items)
                                                        {
                                                            item.IsEnabled = false;
                                                            item.GhiChu = "Mở khoá phiếu cân";
                                                        }

                                                        var rl = mainService.VmPhieuCanBtpFilletv2.Update(items);
                                                        error.ErrorString =
                                                            $"Đã mở khoá {rl} phiếu cân";
                                                        isOK = true;
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "Không tìm thấy RỔ CÁ";
                                                        maycan.ColorString = "red";
                                                    }

                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = null;
                                                    maycan.TheRoAddDateTime = null;
                                                    maycan.TheNhanVienAddDateTime = null;
                                                    break;
                                                }

                                                if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                    maycan.TheRoAddDateTime != null)
                                                {
                                                    maycan.The = dataLite.TheId;
                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thanhPham =
                                                        mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (thanhPham != null)
                                                    {
                                                        var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                        if (dataLite.TrongLuong > thanhPhamMax)
                                                        {
                                                            error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        var thanhPhamMin = (decimal)(thanhPham.Min ?? 0);
                                                        if (dataLite.TrongLuong < thanhPhamMin)
                                                        {
                                                            error.ErrorString =
                                                                $"TRỌNG LƯỢNG NHỎ HƠN GIỚI HẠN: {thanhPhamMin} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (thanhPham.IsNguyenCon && thanhPham.IsXeBuom == true &&
                                                            thanhPham.IsNguyenLieuXeBuom == false && thanhPham.IsDat == true &&
                                                            thanhPham.IsChuyenFillet == true &&
                                                            thanhPham.IsNguyenConNXB == false &&
                                                            thanhPham.IsGiaoXepKhuon == false)
                                                        {
                                                            isNotOut = true;
                                                        }
                                                    }

                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBtpFilletv2.GetLastByTheId<PhieuCanBTPFilletv2>(
                                                            DateTime.Now, maycan.The);
                                                    var ngayGio = DateTime.Now;
                                                    var timelast = 100.0;
                                                    if (phieuCanLast != null)
                                                        timelast = (ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes;

                                                    if (phieuCanLast != null &&
                                                        phieuCanLast.IsEnabled == false) //&& timelast < 5)
                                                    {
                                                        //     //var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                        //     //    x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                        //     error.ErrorString =
                                                        //         @$"RỔ CÁ CHƯA SỬA - {Math.Round((ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes, 1)} phút";
                                                        var _thanhPham =
                                                            mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                x.Ma == phieuCanLast.MaThanhPham);
                                                        //var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                        //    x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                        var _thanhPhamName = _thanhPham?.Ten ?? phieuCanLast.MaThanhPham;
                                                        if (timelast < 1)
                                                        {
                                                            timelast = timelast * 60;
                                                            error.ErrorString =
                                                                @$"{_thanhPhamName} - {phieuCanLast.TrongLuong} CHƯA SỬA - {Math.Round(timelast, 1)} s";
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString =
                                                                @$"{_thanhPhamName} - {phieuCanLast.TrongLuong} CHƯA SỬA - {Math.Round(timelast, 1)} phút";
                                                        }

                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        var isWaiting = false;
                                                        if (maycan.IsStated == false)
                                                        {
                                                            if (maycan.TrongLuong < 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                            else if (maycan.TrongLuong == 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            else
                                                                error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                            if (dataLite.IsWaiting == false)
                                                            {
                                                                maycan.The = dataLite.TheId;
                                                                maycan.TheAddDateTime = DateTime.Now;
                                                                //error.ErrorString = "CHỜ CÂN BẰNG";
                                                                //await Task.Delay(0);
                                                                //await mayCansService.CommandSetWaiting(maycan.Id);
                                                                //await Task.Delay(100);
                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 0,
                                                                    MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "red";
                                                                // isWaiting = true;
                                                                // isOK = true;
                                                                maycan.TheRo = null;
                                                                maycan.TheNhanVien = null;
                                                                maycan.MaNhanVien = null;
                                                                maycan.The = null;
                                                                maycan.TheRoAddDateTime = null;
                                                                maycan.TheNhanVienAddDateTime = null;
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                                maycan.ColorString = "red";
                                                                maycan.TheRo = null;
                                                                maycan.TheNhanVien = null;
                                                                maycan.MaNhanVien = null;
                                                                maycan.The = null;
                                                                maycan.TheRoAddDateTime = null;
                                                                maycan.TheNhanVienAddDateTime = null;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var stt = maycan.Items
                                                                .Where(x => ((PhieuCanBTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanBTPFilletv2)x).STT)
                                                                .DefaultIfEmpty(0).Max();
                                                            var phieuCan = new PhieuCanBTPFilletv2
                                                            {
                                                                STT = stt + 1,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                Id = dataLite.Id,
                                                                MaLo = maycan.MaLo,
                                                                MaSize = maycan.MaSize,
                                                                MaNhanVien = maycan.MaNhanVien,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                CaTra = false,
                                                                GhiChu = "",
                                                                Gio = ngayGio.TimeOfDay,
                                                                IsEnabled = isNotOut, //false,
                                                                MaLoaiCa =
                                                                    mainService.VmLoaiCaDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                                MaMau = mainService.VmMauDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                        "",
                                                                MaMayCan = maycan.Id,
                                                                MaMayLangDa = "M1",
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaThe = maycan?.The ?? "",
                                                                MaUserCan = maycan?.Id ?? "",
                                                                MaXuong = maycan?.MaXuong ?? "",
                                                                Ngay = ngayGio.Date,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu
                                                            };
                                                            if (mainService.VmPhieuCanBtpFilletv2.Insert(phieuCan) > 0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                try
                                                                {
                                                                    await mayCansService.CommandSUCCESS(maycan.Id);
                                                                    await Task.Delay(200);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    // Console.WriteLine(e);
                                                                    // throw;
                                                                }

                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 2,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = 0,
                                                                    Id = dataLite.Id,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu ?? "",
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = maycan.MaSize,
                                                                    MaThanhPham = maycan.MaThanhPham,
                                                                    MayCan = maycan.Id,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = 0,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = maycan.TongSoRo,
                                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                isOK = true;
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        if (isWaiting == false)
                                                        {
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                }


                                                break;
                                            }

                                        case AppType._type2:
                                            if (isChuyenFillet) goto case AppType._type1;

                                            break;
                                        case AppType._type3:
                                            {
                                                if (isChuyenFillet) goto case AppType._type1;

                                                if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                    maycan.TheNhanVienAddDateTime != null)
                                                {
                                                    maycan.The = dataLite.TheId;
                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thanhPham =
                                                        mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (thanhPham != null)
                                                    {
                                                        var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                        if (dataLite.TrongLuong > thanhPhamMax)
                                                        {
                                                            error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }
                                                    }

                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBtpFilletv2.GetLastByTheId<PhieuCanBTPFilletv2>(
                                                            DateTime.Now, maycan.TheNhanVien);
                                                    var ngayGio = DateTime.Now;
                                                    if (phieuCanLast != null && phieuCanLast.IsEnabled == false)
                                                    {
                                                        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                            x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                        error.ErrorString =
                                                            @$"RỔ CÁ CHƯA SỬA - {nv?.MaHoSo}- {Math.Round((ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes, 1)} phút";
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }

                                                    else if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                             maycan.TheNhanVienAddDateTime != null && maycan.MaNhanVien != null)
                                                    {
                                                        if (maycan.IsStated == false)
                                                        {
                                                            if (maycan.TrongLuong < 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                            else if (maycan.TrongLuong == 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            else
                                                                error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else
                                                        {
                                                            var stt = maycan.Items
                                                                .Where(x => ((PhieuCanBTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanBTPFilletv2)x).STT)
                                                                .DefaultIfEmpty(0).Max();
                                                            var phieuCan = new PhieuCanBTPFilletv2
                                                            {
                                                                STT = stt + 1,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                Id = dataLite.Id,
                                                                MaLo = maycan.MaLo,
                                                                MaSize = maycan.MaSize,
                                                                MaNhanVien = maycan.MaNhanVien,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                CaTra = false,
                                                                GhiChu = "",
                                                                Gio = ngayGio.TimeOfDay,
                                                                IsEnabled = false,
                                                                MaLoaiCa =
                                                                    mainService.VmLoaiCaDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                                MaMau = mainService.VmMauDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                        "",
                                                                MaMayCan = maycan.Id,
                                                                MaMayLangDa = "M1",
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaThe = maycan.TheNhanVien,
                                                                MaUserCan = maycan.Id,
                                                                MaXuong = maycan.MaXuong,
                                                                Ngay = ngayGio.Date,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu
                                                            };
                                                            if (mainService.VmPhieuCanBtpFilletv2.Insert(phieuCan) > 0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 1,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = 0,
                                                                    Id = dataLite.Id,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = maycan.MaSize,
                                                                    MaThanhPham = maycan.MaThanhPham,
                                                                    MayCan = maycan.Id,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = 0,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = maycan.TongSoRo,
                                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                isOK = true;
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 0,
                                                            MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                        };
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        isOK = true;
                                                    }
                                                }
                                            }
                                            break;
                                        case AppType._type4:
                                            {
                                                if (isChuyenFillet) goto case AppType._type1;

                                                maycan.TheRo = dataLite.TheId;
                                                maycan.The = null;
                                                if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Lô";
                                                    break;
                                                }

                                                if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn size";
                                                    break;
                                                }

                                                if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Thành Phẩm";
                                                    break;
                                                }

                                                var thanhPham =
                                                    mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                        x.Ma == maycan.MaThanhPham);
                                                if (thanhPham != null)
                                                {
                                                    var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                    if (dataLite.TrongLuong > thanhPhamMax)
                                                    {
                                                        error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                        break;
                                                    }
                                                }

                                                if (maycan.MaNhanVien == null || maycan.MaNhanVien.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Nhân Viên";
                                                    break;
                                                }

                                                if (maycan.IsStated == false)
                                                {
                                                    if (maycan.TrongLuong < 0)
                                                        error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                    else if (maycan.TrongLuong == 0)
                                                        error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                    else
                                                        error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                    maycan.ColorString = "red";
                                                }
                                                else
                                                {
                                                    var ngayGio = DateTime.Now;
                                                    var stt = maycan.Items
                                                        .Where(x => ((PhieuCanBTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                        .Select(x => ((PhieuCanBTPFilletv2)x).STT)
                                                        .DefaultIfEmpty(0).Max();
                                                    var phieuCan = new PhieuCanBTPFilletv2
                                                    {
                                                        STT = stt + 1,
                                                        CaTra = false,
                                                        Gio = ngayGio.TimeOfDay,
                                                        IsEnabled = true,
                                                        MaLo = maycan.MaLo,
                                                        MaLoaiCa = mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                        MaMau = mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                        MaMayCan = maycan.Id,
                                                        MaMayLangDa = "M1",
                                                        MaNhanVien = maycan.MaNhanVien,
                                                        MaSize = maycan.MaSize,
                                                        MaThanhPham = maycan.MaThanhPham,
                                                        MaThe = dataLite.TheId,
                                                        MaUserCan = maycan.Id,
                                                        MaXuong = maycan.MaXuong,
                                                        Ngay = ngayGio.Date,
                                                        TrongLuong = dataLite.TrongLuong,
                                                        MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                        TrongLuongTare = dataLite.TrongLuongTare,
                                                        Id = dataLite.Id
                                                    };

                                                    if (mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCan) > 0)
                                                    {
                                                        maycan.TongSoRo++;
                                                        maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                        lock (_lockItems)
                                                        {
                                                            maycan.Items.Insert(0, phieuCan);
                                                            if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                maycan.Items.Remove(maycan.Items.Last());
                                                        }

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            TrongLuongYeuCau = 0,
                                                            Id = dataLite.Id,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            MaLo = maycan.MaLo,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            MaNhanVienBanKiem = "",
                                                            MaSize = maycan.MaSize,
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MayCan = maycan.Id,
                                                            Ngay = ngayGio.ToString("yyyyMMdd"),
                                                            NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                            STT = phieuCan.STT,
                                                            Status = 0,
                                                            TrongLuongNhan = dataLite.TrongLuong,
                                                            TheIdChucNang = "",
                                                            TrongLuongTra = 0,
                                                            TongTrongLuongCan = maycan.TongTrongLuong,
                                                            TongSoRo = maycan.TongSoRo,
                                                            TongTrongLuong = maycan.TongTrongLuong,
                                                            TongSoRoCan = maycan.TongSoRo,
                                                            MaNhanVien = maycan.MaNhanVien
                                                        };
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        maycan.ColorString = "greenyellow";
                                                        isOK = true;
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                        maycan.ColorString = "red";
                                                    }

                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = null;
                                                    maycan.TheRoAddDateTime = null;
                                                    maycan.TheNhanVienAddDateTime = null;
                                                }
                                            }
                                            break;
                                        case AppType._type1_2:
                                            {
                                                if (isChuyenFillet) goto case AppType._type1;

                                                if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                    maycan.TheRoAddDateTime != null)
                                                {
                                                    maycan.The = null;
                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        break;
                                                    }

                                                    var thanhPham =
                                                        mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (thanhPham != null)
                                                    {
                                                        var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                        if (dataLite.TrongLuong > thanhPhamMax)
                                                        {
                                                            error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                            break;
                                                        }
                                                    }

                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBtpFilletv2.GetLastByTheId<PhieuCanBTPFilletv2>(
                                                            DateTime.Now, maycan.TheRo);

                                                    if (phieuCanLast != null && phieuCanLast.IsEnabled == false)
                                                    {
                                                        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                            x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                        error.ErrorString = @$"RỔ CÁ CHƯA SỬA - {nv?.MaHoSo}";
                                                    }
                                                    else
                                                    {
                                                        if (dataLite.Stated != "ST")
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                        }
                                                        else
                                                        {
                                                            var ngayGio = DateTime.Now;
                                                            var stt = maycan.Items
                                                                .Where(x => ((PhieuCanTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanTPFilletv2)x).STT)
                                                                .DefaultIfEmpty(0).Max();

                                                            var phieuCan = new PhieuCanBTPFilletv2
                                                            {
                                                                STT = stt + 1,
                                                                CaTra = false,
                                                                Gio = ngayGio.TimeOfDay,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaSize = maycan.MaSize,
                                                                MaLo = maycan.MaLo,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                MaThe = maycan.TheRo,
                                                                MaXuong = maycan.MaXuong,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                MaMayCan = maycan.Id,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                IsEnabled = false,
                                                                MaLoaiCa =
                                                                    mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                                MaMau =
                                                                    mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                                MaUserCan = maycan.Id,
                                                                MaMayLangDa = "",
                                                                Ngay = ngayGio.Date,
                                                                Id = dataLite.Id
                                                            };

                                                            // Đại Thành Cân nhiều rổ
                                                            phieuCan.TrongLuongTare *= 2;
                                                            if (mainService.VmPhieuCanBtpFilletv2.Insert(phieuCan) > 0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                                //mainService.VmPhieuCanBtpFilletv2.Items.Add(phieuCan);
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                maycan.TheRo = null;
                                                                maycan.The = dataLite.Id;
                                                                isOK = true;
                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = 0,
                                                                    Id = dataLite.Id,
                                                                    MaThanhPham = phieuCan.MaThanhPham,
                                                                    MaLo = phieuCan.MaLo,
                                                                    MaSize = phieuCan.MaSize,
                                                                    TrongLuongTare = phieuCan.TrongLuongTare,
                                                                    TrongLuongTra = 0,
                                                                    MaNhanVienPhucVu = phieuCan.MaNhanVienPhucVu,
                                                                    MayCan = phieuCan.MaMayCan,
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    STT = 0,
                                                                    TheId = phieuCan.MaThe,
                                                                    Status = 0,
                                                                    TrongLuongNhan = phieuCan.TrongLuong,
                                                                    TheIdChucNang = "",
                                                                    IsDataAction = 1,
                                                                    MessStr = "ĐÃ HOÀN THÀNH"
                                                                };
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);

                                                                break;
                                                            }

                                                            maycan.TheRo = null;
                                                            maycan.The = dataLite.Id;
                                                            error.ErrorString = "KHÔNG THỂ THÊM PHIẾU CÂN";
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }
                            case AppKV.TPFilletv2:
                                {
                                    switch (maycan.AType)
                                    {
                                        case AppType._default:
                                            break;
                                        case AppType._type1:
                                            {
                                                var iscal = false;
                                                if (dataLite.IsWaiting == false && maycan.TheRo != null &&
                                                    maycan.TheRo.Trim() != "" && maycan.TheRoAddDateTime != null)
                                                {
                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBtpFilletv2.GetLastByTheId<PhieuCanBTPFilletv2>(
                                                            DateTime.Now, dataLite.TheId);
                                                    if ((phieuCanLast == null ||
                                                         (phieuCanLast != null && phieuCanLast.IsEnabled)) &&
                                                        maycan.IsKhongTheDauVao == false)
                                                    {
                                                        var timelast = 1000.0;
                                                        try
                                                        {
                                                            var _phieuCanLast = maycan.Items.FirstOrDefault();
                                                            if (_phieuCanLast != null && phieuCanLast != null)
                                                            {
                                                                var _phieuCanLast2 = _phieuCanLast as PhieuCanTPFilletv2;
                                                                if (_phieuCanLast2.MaThe == phieuCanLast.MaThe)
                                                                    timelast = (DateTime.Now.TimeOfDay - _phieuCanLast2.Gio)
                                                                        .TotalSeconds;
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            // Console.WriteLine(e);
                                                            // throw;
                                                        }

                                                        if (timelast > 300)
                                                            error.ErrorString = @"CHƯA CÂN ĐẦU VÀO";
                                                        else
                                                            error.ErrorString = @$"Đã cân {timelast:#,##0.00}s";

                                                        isOK = false;
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        if (maycan.IsKhongTheDauVao == false)
                                                        {
                                                            maycan.MaLo = phieuCanLast.MaLo;
                                                            maycan.MaSize = phieuCanLast.MaSize;
                                                            maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                            maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                            maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                            maycan.STTBTP = phieuCanLast.STT;
                                                            maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                            maycan.IdIn = phieuCanLast.Id;
                                                            maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                            maycan.CaTra = phieuCanLast.CaTra;
                                                            maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                            maycan.MaMau = phieuCanLast.MaMau;
                                                            maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                        }
                                                        else
                                                        {
                                                            maycan.MaLo = phieuCanLast.MaLo;
                                                            maycan.MaSize = phieuCanLast.MaSize;
                                                            maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                            maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                            maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                            maycan.STTBTP = phieuCanLast.STT;
                                                            maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                            maycan.IdIn = phieuCanLast.Id;
                                                            maycan.CaTra = phieuCanLast.CaTra;
                                                            maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                            maycan.MaMau = phieuCanLast.MaMau;
                                                            maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                        }

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 0,
                                                            MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                        };
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        isOK = true;
                                                        _ = Task.Run(async () =>
                                                        {
                                                            try
                                                            {
                                                                await Task.Delay(200);
                                                                await mayCansService.CommandSetLo(maycan.Id,
                                                                    maycan.MaLo);
                                                                await Task.Delay(50);
                                                                var _size =
                                                                    mainService.VmSizeFillet.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaSize);
                                                                if (_size != null)
                                                                    await mayCansService.CommandSetSize(maycan.Id,
                                                                        maycan.MaSize,
                                                                        _size.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _thanhPham = mainService.VmThanhPhamFillet.Items
                                                                    .FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaThanhPham);
                                                                if (_thanhPham != null)
                                                                    await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                        maycan.MaThanhPham,
                                                                        _thanhPham?.Ten ?? "");
                                                                // await Task.Delay(50);
                                                                // var _nhanVien =
                                                                //     mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                //         x.MaNhanVien == maycan.MaNhanVien);
                                                                // if (_nhanVien != null)
                                                                // {
                                                                //     maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                //     await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                //         maycan?.MaNhanVien ?? "",
                                                                //         _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                // }
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine(e);
                                                                //throw;
                                                            }
                                                        });
                                                        //iscal = true;
                                                    }
                                                }
                                                else if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                         maycan.TheNhanVien != null &&
                                                         maycan.TheNhanVien.Trim() != "" &&
                                                         maycan.TheNhanVienAddDateTime != null)
                                                {
                                                    iscal = true;
                                                }

                                                if (iscal)
                                                {
                                                    if (maycan.IsStated == false)
                                                    {
                                                        if (maycan.TrongLuong < 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else if (maycan.TrongLuong == 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                        }

                                                        if (dataLite.IsWaiting == false)
                                                        {
                                                            maycan.The = dataLite.TheId;
                                                            maycan.TheAddDateTime = DateTime.Now;
                                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                                            //await Task.Delay(0);
                                                            // await Task.Delay(200);
                                                            // await mayCansService.CommandSetWaiting(maycan.Id);
                                                            // await Task.Delay(500);
                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 0,
                                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            // maycan.ColorString = null;
                                                            // isOK = true;
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (maycan.TrongLuongNhan < dataLite.TrongLuong)
                                                        {
                                                            error.ErrorString =
                                                                $"Trọng lượng trả lớn hơn nhận {maycan.TrongLuongNhan}";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        maycan.The = maycan.TheRo;
                                                        if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Lô";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn size";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Thành Phẩm";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        // var _thanhPham =
                                                        //     mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                        //         x.Ma == maycan.MaThanhPham);
                                                        // if (_thanhPham != null)
                                                        // {
                                                        //     var thanhPhamMax = (decimal)(_thanhPham.Max ?? 0);
                                                        //     if (dataLite.TrongLuong > thanhPhamMax)
                                                        //     {
                                                        //         error.ErrorString =
                                                        //             $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                        //         maycan.ColorString = "red";
                                                        //         break;
                                                        //     }
                                                        // }

                                                        var ngayGio = DateTime.Now;
                                                        var stt = maycan.Items
                                                            .Where(x => ((PhieuCanTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                            .Select(x => ((PhieuCanTPFilletv2)x).STT)
                                                            .DefaultIfEmpty(0).Max();
                                                        if (maycan.IsKhongTheDauVao)
                                                        {
                                                            maycan.IdIn = maycan.Id;
                                                            maycan.STTBTP = stt + 1;
                                                            maycan.MayCanIdBTP = maycan.Id;
                                                            maycan.TrongLuongNhan = dataLite.TrongLuong;
                                                            var phieuCanBtp = new PhieuCanBTPFilletv2
                                                            {
                                                                Id = maycan.IdIn,
                                                                CaTra = false,
                                                                GhiChu = "Không Cân Đầu vào",
                                                                Gio = ngayGio.TimeOfDay,
                                                                IsEnabled = true,
                                                                MaLo = maycan.MaLo,
                                                                MaLoaiCa = maycan.MaLoaiCa,
                                                                MaMau = maycan.MaMau,
                                                                MaMayLangDa = "M1",
                                                                MaMayCan = maycan.Id,
                                                                MaSize = maycan.MaSize,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaThe = maycan?.TheRo,
                                                                MaUserCan = maycan.Id,
                                                                MaXuong = maycan.MaXuong,
                                                                Ngay = ngayGio.Date,
                                                                STT = stt + 1,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                TrongLuongTare = dataLite.TrongLuongTare
                                                            };
                                                            var rlInBTP = mainService.VmPhieuCanBtpFilletv2.Insert(phieuCanBtp);
                                                            if (rlInBTP <= 0)
                                                            {
                                                                error.ErrorString =
                                                                    "Không thể thêm đầu vào";
                                                                maycan.ColorString = "red";
                                                                break;
                                                            }
                                                        }

                                                        var thoiGianHT = (ngayGio.TimeOfDay - maycan.ThoiGianBTP).TotalMinutes;
                                                        var phieuCan = new PhieuCanTPFilletv2
                                                        {
                                                            STT = stt + 1,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            Id = dataLite.Id,
                                                            MaLo = maycan.MaLo,
                                                            MaSize = maycan.MaSize,
                                                            MaNhanVien = maycan?.MaNhanVien,
                                                            TrongLuongTra = dataLite.TrongLuong,
                                                            CaTra = false,
                                                            GhiChu = "",
                                                            Gio = ngayGio.TimeOfDay,
                                                            MaLoaiCa =
                                                                maycan.MaLoaiCa,
                                                            MaMau = maycan.MaMau,
                                                            MaMayCan = maycan?.Id,
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MaThe = maycan?.TheRo,
                                                            MaUserCan = maycan.Id,
                                                            MaXuong = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            IdIn = maycan.IdIn,
                                                            MaMayCanBTP = maycan.MayCanIdBTP,
                                                            STTBTP = maycan.STTBTP,
                                                            TrongLuongNhan = maycan.TrongLuongNhan,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            ThoiGianHT = (decimal)Math.Round(thoiGianHT, 2)
                                                        };
                                                        var dinhMuc = mainService.VmDinhMucFillet.Items
                                                            .FirstOrDefault(x => x.CaTra == maycan.CaTra &&
                                                                                 x.Ngay.Date == mainService.VmApp.DateTimeNow
                                                                                     .Date &&
                                                                                 x.MaLo == maycan.MaLo &&
                                                                                 x.MaLoaiCa == maycan.MaLoaiCa &&
                                                                                 x.MaMau == maycan.MaMau &&
                                                                                 x.MaSize == maycan.MaSize &&
                                                                                 x.MaThanhPham == maycan.MaThanhPham &&
                                                                                 x.MaXuong == maycan.MaXuong &&
                                                                                 x.SuDung);
                                                        if (dinhMuc != null)
                                                        {
                                                            phieuCan.DinhMucYeuCau = (decimal)dinhMuc.DinhMuc;
                                                        }
                                                        else
                                                        {
                                                            if (maycan.CaTra)
                                                            {
                                                                //phieuCan.DinhMucYeuCau = dinhMuc.DinhMuc;
                                                            }
                                                            else
                                                            {
                                                                var thanhPham =
                                                                    mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaThanhPham);
                                                                if (thanhPham != null)
                                                                    phieuCan.DinhMucYeuCau = thanhPham.DinhMuc;
                                                            }
                                                        }

                                                        var dinhMucThuc = phieuCan.TrongLuongNhan == 0
                                                            ? 0
                                                            : Math.Round(phieuCan.TrongLuongNhan / phieuCan.TrongLuongTra, 2);
                                                        phieuCan.DinhMucThucTe = dinhMucThuc;
                                                        var trongLuongYeuCau = phieuCan.DinhMucYeuCau * phieuCan.TrongLuongTra;

                                                        if (trongLuongYeuCau > phieuCan.TrongLuongTra)
                                                            phieuCan.DanhGia = false;
                                                        else
                                                            phieuCan.DanhGia = true;

                                                        if (trongLuongYeuCau == 0) trongLuongYeuCau = phieuCan.TrongLuongTra;

                                                        if (mainService.VmPhieuCanTpFilletv2.Insert(phieuCan) > 0)
                                                        {
                                                            //theem ham cap nhat by id
                                                            if (mainService.VmPhieuCanBtpFilletv2.Update(maycan.IdIn ?? "-",
                                                                    true) >
                                                                0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                try
                                                                {
                                                                    await mayCansService.CommandSUCCESS(maycan.Id);
                                                                    await Task.Delay(200);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    // Console.WriteLine(e);
                                                                    // throw;
                                                                }

                                                                var thongtintonghop =
                                                                    mainService.VmPhieuCanTpFilletv2
                                                                        .GetSoRoTongTrongLuongByNhanVienId(ngayGio.Date,
                                                                            maycan.MaNhanVien);
                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 2,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = trongLuongYeuCau,
                                                                    Id = phieuCan.Id,
                                                                    Id2 = phieuCan.IdIn,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = phieuCan.MaSize,
                                                                    MaThanhPham = phieuCan.MaThanhPham,
                                                                    MayCan = phieuCan.MaMayCan,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = phieuCan.TrongLuongNhan,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = phieuCan.TrongLuongTra,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = thongtintonghop.Item1, //maycan.TongSoRo,
                                                                    TongTrongLuong =
                                                                        thongtintonghop.Item2, //maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien,
                                                                    DinhMucThucTe = dinhMucThuc
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                var maNhanVienTemp = maycan.MaNhanVien;
                                                                isOK = true;
                                                                // _ = Task.Run(async () =>
                                                                // {
                                                                //     try
                                                                //     {
                                                                //         await Task.Delay(200);
                                                                //         await mayCansService.CommandSetLo(maycan.Id,
                                                                //             maycan.MaLo);
                                                                //         await Task.Delay(50);
                                                                //         var _size =
                                                                //             mainService.VmSizeFillet.Items.FirstOrDefault(
                                                                //                 x =>
                                                                //                     x.Ma == maycan.MaSize);
                                                                //         if (_size != null)
                                                                //             await mayCansService.CommandSetSize(maycan.Id,
                                                                //                 maycan.MaSize,
                                                                //                 _size.Ten ?? "");
                                                                //         await Task.Delay(50);
                                                                //         var _thanhPham = mainService.VmThanhPhamFillet.Items
                                                                //             .FirstOrDefault(x =>
                                                                //                 x.Ma == maycan.MaThanhPham);
                                                                //         if (_thanhPham != null)
                                                                //             await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                //                 maycan.MaThanhPham,
                                                                //                 _thanhPham?.Ten ?? "");
                                                                //         await Task.Delay(50);
                                                                //         var _nhanVien =
                                                                //             mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                //                 x.MaNhanVien == maNhanVienTemp);
                                                                //         if (_nhanVien != null)
                                                                //         {
                                                                //             maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                //             await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                //                 maycan?.MaNhanVien ?? "",
                                                                //                 _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                //         }
                                                                //     }
                                                                //     catch (Exception e)
                                                                //     {
                                                                //         Console.WriteLine(e);
                                                                //         //throw;
                                                                //     }
                                                                // });
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "KHÔNG THỂ CẬP NHẬT PHIẾU CÂN BTP";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                            }
                                            break;

                                        case AppType._type2:
                                            break;
                                        case AppType._type3:
                                            {
                                                goto default;
                                            }
                                            break;
                                        case AppType._type4:
                                            {
                                                if (maycan.IsStated == false)
                                                {
                                                    if (dataLite.IsWaiting == false)
                                                    {
                                                        maycan.The = dataLite.TheId;
                                                        maycan.TheAddDateTime = DateTime.Now;
                                                        //error.ErrorString = "CHỜ CÂN BẰNG";
                                                        //await Task.Delay(0);
                                                        await mayCansService.CommandSetWaiting(maycan.Id);
                                                        //await Task.Delay(100);
                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 0,
                                                            MessStr = "CHỜ CÂN BẰNG"
                                                        };
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        maycan.ColorString = null;
                                                        isOK = true;
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                                else
                                                {
                                                    if (maycan.TrongLuong < 0)
                                                    {
                                                        error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                        maycan.ColorString = "red";
                                                    }
                                                    else if (maycan.TrongLuong == 0)
                                                    {
                                                        error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                        maycan.ColorString = "red";
                                                    }
                                                    else
                                                    {
                                                        if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Lô";
                                                            break;
                                                        }

                                                        if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn size";
                                                            break;
                                                        }

                                                        if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Thành Phẩm";
                                                            break;
                                                        }

                                                        var thanhPham =
                                                            mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                x.Ma == maycan.MaThanhPham);
                                                        if (thanhPham != null)
                                                        {
                                                            var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                            if (dataLite.TrongLuong > thanhPhamMax)
                                                            {
                                                                error.ErrorString =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                                break;
                                                            }
                                                        }

                                                        if (maycan.MaNhanVien == null || maycan.MaNhanVien.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Nhân Viên";
                                                            break;
                                                        }

                                                        var ngayGio = DateTime.Now;
                                                        var stt = maycan.Items
                                                            .Where(x => ((PhieuCanTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                            .Select(x => ((PhieuCanTPFilletv2)x).STT)
                                                            .DefaultIfEmpty(0).Max();
                                                        var phieuCan = new PhieuCanTPFilletv2
                                                        {
                                                            STT = stt + 1,
                                                            CaTra = false,
                                                            Gio = ngayGio.TimeOfDay,
                                                            MaLo = maycan.MaLo,
                                                            MaLoaiCa =
                                                                mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ??
                                                                "",
                                                            MaMau =
                                                                mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                            MaMayCan = maycan.Id,
                                                            MaNhanVien = maycan.MaNhanVien,
                                                            MaSize = maycan.MaSize,
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MaThe = dataLite.TheId,
                                                            MaUserCan = maycan.Id,
                                                            MaXuong = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            Id = dataLite.Id,
                                                            TrongLuongNhan = 0,
                                                            TrongLuongTra = dataLite.TrongLuong
                                                        };
                                                        var dinhMuc = mainService.VmDinhMucFillet.Items
                                                            .FirstOrDefault(x => x.CaTra == maycan.CaTra &&
                                                                                 x.Ngay.Date == mainService.VmApp.DateTimeNow
                                                                                     .Date &&
                                                                                 x.MaLo == maycan.MaLo &&
                                                                                 x.MaLoaiCa == maycan.MaLoaiCa &&
                                                                                 x.MaMau == maycan.MaMau &&
                                                                                 x.MaSize == maycan.MaSize &&
                                                                                 x.MaThanhPham == maycan.MaThanhPham &&
                                                                                 x.MaXuong == maycan.MaXuong &&
                                                                                 x.SuDung);
                                                        if (dinhMuc != null)
                                                        {
                                                            phieuCan.DinhMucYeuCau = (decimal)dinhMuc.DinhMuc;
                                                        }
                                                        else
                                                        {
                                                            if (maycan.CaTra)
                                                            {
                                                                //phieuCan.DinhMucYeuCau = dinhMuc.DinhMuc;
                                                            }
                                                            else
                                                            {
                                                                if (thanhPham != null)
                                                                    phieuCan.DinhMucYeuCau = thanhPham.DinhMuc;
                                                            }
                                                        }

                                                        var dinhMucThuc = phieuCan.TrongLuongNhan == 0
                                                            ? 0
                                                            : Math.Round(phieuCan.TrongLuongNhan / phieuCan.TrongLuongTra, 2);
                                                        phieuCan.DinhMucThucTe = dinhMucThuc;
                                                        var trongLuongYeuCau = phieuCan.DinhMucYeuCau * phieuCan.TrongLuongTra;
                                                        if (mainService.VmPhieuCanTpFilletv2.Insert(phieuCan) > 0)
                                                        {
                                                            maycan.TongSoRo++;
                                                            maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                            lock (_lockItems)
                                                            {
                                                                maycan.Items.Insert(0, phieuCan);
                                                                if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                    maycan.Items.Remove(maycan.Items.Last());
                                                            }

                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 1,
                                                                MessStr = "ĐÃ HOÀN THÀNH",
                                                                ChiSanLuong = 0,
                                                                TrongLuongYeuCau = trongLuongYeuCau,
                                                                Id = dataLite.Id,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                MaLo = maycan.MaLo,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                MaNhanVienBanKiem = "",
                                                                MaSize = maycan.MaSize,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MayCan = maycan.Id,
                                                                Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                STT = phieuCan.STT,
                                                                Status = 0,
                                                                TrongLuongNhan = dataLite.TrongLuong,
                                                                TheIdChucNang = "",
                                                                TrongLuongTra = 0,
                                                                TongTrongLuongCan = maycan.TongTrongLuong,
                                                                TongSoRo = maycan.TongSoRo,
                                                                TongTrongLuong = maycan.TongTrongLuong,
                                                                TongSoRoCan = maycan.TongSoRo,
                                                                MaNhanVien = maycan.MaNhanVien,
                                                                DinhMucThucTe = phieuCan.DinhMucThucTe
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            maycan.ColorString = "greenyellow";
                                                            isOK = true;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                            }
                                            break;
                                        case AppType._type1_2:
                                            {
                                                maycan.ThePhieuSanLuongId = null;
                                                maycan.BanId = null;
                                                maycan.STTBTP = null;
                                                maycan.MayCanIdBTP = null;
                                                maycan.IdIn = null;
                                                maycan.TrongLuongNhan = 0;
                                                if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                    maycan.TheRoAddDateTime != null)
                                                {
                                                    maycan.The = null;
                                                    //if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    //{
                                                    //    error.ErrorString = "Chưa chọn Lô";
                                                    //    break;
                                                    //}

                                                    //if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    //{
                                                    //    error.ErrorString = "Chưa chọn size";
                                                    //    break;
                                                    //}

                                                    //if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    //{
                                                    //    error.ErrorString = "Chưa chọn Thành Phẩm";
                                                    //    break;
                                                    //}


                                                    var phieuCanBTP =
                                                        mainService.VmPhieuCanBtpFilletv2.GetLastByTheId<PhieuCanBTPFilletv2>(
                                                            DateTime.Now, maycan.TheRo);

                                                    if (phieuCanBTP == null && phieuCanBTP.IsEnabled == false)
                                                    {
                                                        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                            x.MaNhanVien == phieuCanBTP.MaNhanVien);
                                                        error.ErrorString = @$"RỔ CÁ CHƯA SỬA - {nv?.MaHoSo}";
                                                        maycan.TheRo = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        break;
                                                    }

                                                    if (phieuCanBTP.IsEnabled)
                                                    {
                                                        error.ErrorString = @"Chưa Cân Đầu Vào";
                                                        maycan.TheRo = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        break;
                                                    }

                                                    maycan.MaLo = phieuCanBTP.MaLo;
                                                    maycan.MaSize = phieuCanBTP.MaSize;
                                                    maycan.MaThanhPham = phieuCanBTP.MaThanhPham;
                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = dataLite.Id;
                                                    maycan.STTBTP = phieuCanBTP.STT;
                                                    maycan.MayCanIdBTP = phieuCanBTP.MaMayCan;
                                                    maycan.TrongLuongNhan = phieuCanBTP.TrongLuong;
                                                    maycan.IdIn = phieuCanBTP.Id;
                                                    isOK = true;
                                                    break;
                                                    //var stt = mainService.VmPhieuCanBtpFilletv2.Items
                                                    //    .Where(x => x.MaMayCan == maycan.Id).Select(x => x.STT).DefaultIfEmpty(0)
                                                    //    .Max();
                                                    //var phieuCan = new PhieuCanBTPFilletv2()
                                                    //{
                                                    //    STT = stt + 1,
                                                    //    CaTra = false,
                                                    //    Gio = DateTime.Now.TimeOfDay,
                                                    //    MaThanhPham = maycan.MaThanhPham,
                                                    //    MaSize = maycan.MaSize,
                                                    //    MaLo = maycan.MaLo,
                                                    //    TrongLuongTare = dataLite.TrongLuongTare,
                                                    //    MaThe = maycan.TheRo,
                                                    //    MaXuong = maycan.MaXuong,
                                                    //    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                    //    MaMayCan = maycan.Id,
                                                    //    TrongLuong = dataLite.TrongLuong,
                                                    //    IsEnabled = false,
                                                    //    MaLoaiCa = mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                    //    MaMau = mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                    //    MaUserCan = maycan.Id,
                                                    //    MaMayLangDa = "",
                                                    //    Ngay = DateTime.Now.Date
                                                    //};

                                                    //// Đại Thành Cân nhiều rổ
                                                    //phieuCan.TrongLuongTare *= 2;
                                                    //if (mainService.VmPhieuCanBtpFilletv2.Insert(phieuCan) > 0)
                                                    //{
                                                    //    maycan.TongSoRo++;
                                                    //    maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                    //    isOK = true;
                                                    //    maycan.TheRo = null;

                                                    //    break;
                                                    //}
                                                    //else
                                                    //{
                                                    //    error.ErrorString = @$"Không Thể Thêm";
                                                    //    break;
                                                    //}
                                                }

                                                if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                    maycan.TheNhanVienAddDateTime != null && maycan.MaNhanVien != null)
                                                {
                                                    var isAllowAdd = false;
                                                    var banId = mainService.VmNhanVienTheoBan.GetBanId(DateTime.Now,
                                                        maycan.MaXuong, maycan.MaNhanVien);
                                                    maycan.BanId = banId;
                                                    if (maycan.TheRoAddDateTime != null && maycan.The != null &&
                                                        maycan.The.Trim() != "")
                                                    {
                                                        isAllowAdd = true;
                                                    }
                                                    else if (maycan.TheRoAddDateTime == null)
                                                    {
                                                        if (banId == null || banId.Trim() == "")
                                                        {
                                                            error.ErrorString =
                                                                "NHÂN VIÊN CHƯA SẮP BÀN";

                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                            break;
                                                        }

                                                        var phieuSanLuong =
                                                            mainService.VmThePhieuSanLuongFillet
                                                                .GetsLastByBan<ThePhieuSanLuongFillet>(
                                                                    DateTime.Now, banId).FirstOrDefault();
                                                        if (phieuSanLuong != null)
                                                        {
                                                            maycan.MaLo = phieuSanLuong.MaLo;
                                                            maycan.MaThanhPham = phieuSanLuong.MaThanhPham;
                                                            maycan.MaSize = phieuSanLuong.MaSize;
                                                            maycan.TrongLuongNhan = phieuSanLuong.TrongLuongNhan;
                                                            maycan.STTBTP = phieuSanLuong.STT_PC_BTP;
                                                            maycan.MayCanIdBTP = phieuSanLuong.MayCan_PC_BTP;
                                                            maycan.The = phieuSanLuong.MaThe;
                                                            maycan.ThePhieuSanLuongId = phieuSanLuong.Id;
                                                            maycan.IdIn = phieuSanLuong.IdIn;
                                                            isAllowAdd = true;
                                                        }
                                                        else
                                                        {
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                            error.ErrorString =
                                                                "CHƯA XÁC NHẬN CHO BÀN - HOẶC CHƯA CÂN ĐẦU VÀO";
                                                            break;
                                                        }
                                                    }

                                                    if (isAllowAdd)
                                                    {
                                                        if (dataLite.Stated != "ST")
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                        }
                                                        else
                                                        {
                                                            var ngayGio = DateTime.Now;
                                                            var stt = maycan.Items
                                                                .Where(x => ((PhieuCanTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanTPFilletv2)x).STT).Max();
                                                            var phieuCanTP = new PhieuCanTPFilletv2
                                                            {
                                                                STT = stt + 1,
                                                                CaTra = false,
                                                                DinhMucThucTe = 0,
                                                                DinhMucYeuCau = 0,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaSize = maycan.MaSize,
                                                                MaNhanVien = maycan.MaNhanVien,
                                                                MaLo = maycan.MaLo,
                                                                MaXuong = maycan.MaXuong,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                MaThe = maycan.The ?? "",
                                                                STTBTP = maycan.STTBTP,
                                                                TrongLuongNhan = maycan.TrongLuongNhan,
                                                                TrongLuongTra = dataLite.TrongLuong,
                                                                MaMayCan = maycan.Id,
                                                                Gio = ngayGio.TimeOfDay,
                                                                MaBan = maycan.BanId,
                                                                MaLoaiCa =
                                                                    mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                                MaMau =
                                                                    mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                                MaMayCanBTP = maycan.MayCanIdBTP,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                MaUserCan = maycan.Id,
                                                                Ngay = ngayGio.Date,
                                                                ThePhieuSanLuongId = maycan.ThePhieuSanLuongId,
                                                                Id = dataLite.Id,
                                                                IdIn = maycan.IdIn
                                                            };
                                                            var phieuSanLuong =
                                                                mainService.VmThePhieuSanLuongFillet.CreateNew(phieuCanTP);
                                                            if (maycan.TheRoAddDateTime != null)
                                                                phieuCanTP.ThePhieuSanLuongId = phieuSanLuong.Id;
                                                            if (mainService.VmPhieuCanTpFilletv2.Insert(phieuCanTP) > 0)
                                                            {
                                                                if (maycan.TheRoAddDateTime != null)
                                                                {
                                                                    if (mainService.VmPhieuCanBtpFilletv2.Set(
                                                                            maycan?.STTBTP ?? 0,
                                                                            DateTime.Now.Date, maycan?.MayCanIdBTP ?? "",
                                                                            maycan?.MaXuong ?? "",
                                                                            true, phieuCanTP.MaNhanVien) > 0)
                                                                    {
                                                                        mainService.VmThePhieuSanLuongFillet.SetTheIsDoneByBan(
                                                                            DateTime.Now,
                                                                            phieuSanLuong.MaBan, true);
                                                                        if (mainService.VmThePhieuSanLuongFillet
                                                                                .Insert(phieuSanLuong) <= 0)
                                                                        {
                                                                            error.ErrorString =
                                                                                "KHÔNG THỂ THÊM THÔNG TIN THẺ ĐẦU VÀO";
                                                                            break;
                                                                        }

                                                                        maycan.TongSoRo++;
                                                                        maycan.TongTrongLuong += phieuCanTP.TrongLuongTra;
                                                                        maycan.Items.Insert(0, phieuCanTP);
                                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                            maycan.Items.Remove(maycan.Items.Last());
                                                                        isOK = true;
                                                                        var dataRes = new DataLiteRes
                                                                        {
                                                                            ChiSanLuong = 0,
                                                                            TrongLuongYeuCau = 0,
                                                                            Id = dataLite.Id,
                                                                            MaThanhPham = phieuCanTP.MaThanhPham,
                                                                            MaLo = phieuCanTP.MaLo,
                                                                            MaSize = phieuCanTP.MaSize,
                                                                            TrongLuongTare = phieuCanTP.TrongLuongTare,
                                                                            TrongLuongTra = phieuCanTP.TrongLuongTra,
                                                                            MaNhanVienPhucVu = phieuCanTP.MaNhanVienPhucVu,
                                                                            MayCan = phieuCanTP.MaMayCan,
                                                                            NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                            Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                            STT = 0,
                                                                            TheId = phieuCanTP.MaThe,
                                                                            Status = 0,
                                                                            TrongLuongNhan = phieuCanTP.TrongLuongNhan,

                                                                            TheIdChucNang = "",
                                                                            Id2 = phieuCanTP.IdIn,
                                                                            IsDataAction = 1,
                                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                                            MaNhanVien = maycan.MaNhanVien
                                                                        };
                                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                        maycan.TheRo = null;
                                                                        maycan.TheNhanVien = null;
                                                                        maycan.MaNhanVien = null;
                                                                        maycan.The = null;
                                                                        maycan.TheRoAddDateTime = null;
                                                                        maycan.TheNhanVienAddDateTime = null;
                                                                        break;
                                                                    }

                                                                    maycan.TheRo = null;
                                                                    maycan.TheNhanVien = null;
                                                                    maycan.MaNhanVien = null;
                                                                    maycan.The = null;
                                                                    maycan.TheRoAddDateTime = null;
                                                                    maycan.TheNhanVienAddDateTime = null;
                                                                    error.ErrorString = "LỖI CẬP NHẬT PC BTP";
                                                                    break;
                                                                }

                                                                {
                                                                    maycan.TongSoRo++;
                                                                    maycan.TongTrongLuong += phieuCanTP.TrongLuongTra;
                                                                    maycan.Items.Insert(0, phieuCanTP);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                    isOK = true;
                                                                    var dataRes = new DataLiteRes
                                                                    {
                                                                        ChiSanLuong = 0,
                                                                        TrongLuongYeuCau = 0,
                                                                        Id = dataLite.Id,
                                                                        MaThanhPham = phieuCanTP.MaThanhPham,
                                                                        MaLo = phieuCanTP.MaLo,
                                                                        MaSize = phieuCanTP.MaSize,
                                                                        TrongLuongTare = phieuCanTP.TrongLuongTare,
                                                                        TrongLuongTra = phieuCanTP.TrongLuongTra,
                                                                        MaNhanVienPhucVu = phieuCanTP.MaNhanVienPhucVu,
                                                                        MayCan = phieuCanTP.MaMayCan,
                                                                        NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                        Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                        STT = 0,
                                                                        TheId = phieuCanTP.MaThe,
                                                                        Status = 0,
                                                                        TrongLuongNhan = phieuCanTP.TrongLuongNhan,
                                                                        TheIdChucNang = "",
                                                                        Id2 = phieuCanTP.IdIn,
                                                                        IsDataAction = 1,
                                                                        MessStr = "ĐÃ HOÀN THÀNH"
                                                                    };
                                                                    dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                    maycan.TheRo = null;
                                                                    maycan.TheNhanVien = null;
                                                                    maycan.MaNhanVien = null;
                                                                    maycan.The = null;
                                                                    maycan.TheRoAddDateTime = null;
                                                                    maycan.TheNhanVienAddDateTime = null;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                maycan.TheRo = null;
                                                                maycan.TheNhanVien = null;
                                                                maycan.MaNhanVien = null;
                                                                maycan.The = null;
                                                                maycan.TheRoAddDateTime = null;
                                                                maycan.TheNhanVienAddDateTime = null;
                                                                error.ErrorString = "KHÔNG THỂ THÊM PHIẾU CÂN";
                                                            }
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                        default:
                                            {
                                                var iscal = false;
                                                if (dataLite.IsWaiting == false)
                                                {
                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBtpFilletv2.GetLastByTheId<PhieuCanBTPFilletv2>(
                                                            DateTime.Now, dataLite.TheId);
                                                    if (phieuCanLast == null && phieuCanLast.IsEnabled == false)
                                                    {
                                                        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                            x.MaNhanVien == phieuCanLast?.MaNhanVien);
                                                        error.ErrorString = @"CHƯA CÂN ĐẦU VÀO";
                                                        isOK = false;
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        maycan.MaLo = phieuCanLast.MaLo;
                                                        maycan.MaSize = phieuCanLast.MaSize;
                                                        maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                        maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                        maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                        maycan.STTBTP = phieuCanLast.STT;
                                                        maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                        maycan.IdIn = phieuCanLast.Id;
                                                        maycan.CaTra = phieuCanLast.CaTra;
                                                        maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                        maycan.MaMau = phieuCanLast.MaMau;


                                                        iscal = true;
                                                    }
                                                }
                                                else
                                                {
                                                    iscal = true;
                                                }

                                                if (iscal)
                                                {
                                                    if (maycan.IsStated == false)
                                                    {
                                                        if (maycan.TrongLuong < 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else if (maycan.TrongLuong == 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                        }

                                                        if (dataLite.IsWaiting == false)
                                                        {
                                                            maycan.The = dataLite.TheId;
                                                            maycan.TheAddDateTime = DateTime.Now;
                                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                                            //await Task.Delay(0);
                                                            await mayCansService.CommandSetWaiting(maycan.Id);
                                                            //await Task.Delay(100);
                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 0,
                                                                MessStr = "CHỜ CÂN BẰNG"
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            maycan.ColorString = null;
                                                            isOK = true;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Lô";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn size";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Thành Phẩm";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        var _thanhPham =
                                                            mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                x.Ma == maycan.MaThanhPham);
                                                        if (_thanhPham != null)
                                                        {
                                                            var thanhPhamMax = (decimal)(_thanhPham.Max ?? 0);
                                                            if (dataLite.TrongLuong > thanhPhamMax)
                                                            {
                                                                error.ErrorString =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                                maycan.ColorString = "red";
                                                                break;
                                                            }
                                                        }

                                                        var ngayGio = DateTime.Now;
                                                        var stt = maycan.Items
                                                            .Where(x => ((PhieuCanTPFilletv2)x).Ngay.Date == ngayGio.Date)
                                                            .Select(x => ((PhieuCanTPFilletv2)x).STT)
                                                            .DefaultIfEmpty(0).Max();
                                                        var phieuCan = new PhieuCanTPFilletv2
                                                        {
                                                            STT = stt + 1,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            Id = dataLite.Id,
                                                            MaLo = maycan.MaLo,
                                                            MaSize = maycan.MaSize,
                                                            MaNhanVien = maycan?.MaNhanVien,
                                                            TrongLuongTra = dataLite.TrongLuong,
                                                            CaTra = false,
                                                            GhiChu = "",
                                                            Gio = ngayGio.TimeOfDay,
                                                            MaLoaiCa =
                                                                maycan.MaLoaiCa,
                                                            MaMau = maycan.MaMau,
                                                            MaMayCan = maycan?.Id,
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MaThe = maycan?.TheRo,
                                                            MaUserCan = maycan.Id,
                                                            MaXuong = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            IdIn = maycan.IdIn,
                                                            MaMayCanBTP = maycan.MayCanIdBTP,
                                                            STTBTP = maycan.STTBTP,
                                                            TrongLuongNhan = maycan.TrongLuongNhan,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            DinhMucThucTe = 0,
                                                            DinhMucYeuCau = 0
                                                        };
                                                        var dinhMuc = mainService.VmDinhMucFillet.Items
                                                            .FirstOrDefault(x => x.CaTra == maycan.CaTra &&
                                                                                 x.Ngay.Date == mainService.VmApp.DateTimeNow
                                                                                     .Date &&
                                                                                 x.MaLo == maycan.MaLo &&
                                                                                 x.MaLoaiCa == maycan.MaLoaiCa &&
                                                                                 x.MaMau == maycan.MaMau &&
                                                                                 x.MaSize == maycan.MaSize &&
                                                                                 x.MaThanhPham == maycan.MaThanhPham &&
                                                                                 x.MaXuong == maycan.MaXuong &&
                                                                                 x.SuDung);
                                                        if (dinhMuc != null)
                                                        {
                                                            phieuCan.DinhMucYeuCau = (decimal)dinhMuc.DinhMuc;
                                                        }
                                                        else
                                                        {
                                                            if (maycan.CaTra)
                                                            {
                                                                //phieuCan.DinhMucYeuCau = dinhMuc.DinhMuc;
                                                            }
                                                            else
                                                            {
                                                                var thanhPham =
                                                                    mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaThanhPham);
                                                                if (thanhPham != null)
                                                                    phieuCan.DinhMucYeuCau = thanhPham.DinhMuc;
                                                            }
                                                        }

                                                        var dinhMucThuc = phieuCan.TrongLuongNhan == 0
                                                            ? 0
                                                            : Math.Round(phieuCan.TrongLuongNhan / phieuCan.TrongLuongTra, 2);
                                                        phieuCan.DinhMucThucTe = dinhMucThuc;
                                                        var trongLuongYeuCau = phieuCan.DinhMucYeuCau * phieuCan.TrongLuongTra;
                                                        if (mainService.VmPhieuCanTpFilletv2.Insert(phieuCan) > 0)
                                                        {
                                                            //theem ham cap nhat by id
                                                            if (mainService.VmPhieuCanBtpFilletv2.Update(maycan.IdIn ?? "-",
                                                                    true) >
                                                                0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 1,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = trongLuongYeuCau,
                                                                    Id = phieuCan.Id,
                                                                    Id2 = phieuCan.IdIn,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = phieuCan.MaSize,
                                                                    MaThanhPham = phieuCan.MaThanhPham,
                                                                    MayCan = phieuCan.MaMayCan,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = phieuCan.TrongLuongNhan,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = phieuCan.TrongLuongTra,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = maycan.TongSoRo,
                                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                var maNhanVienTemp = maycan.MaNhanVien;
                                                                isOK = true;
                                                                _ = Task.Run(async () =>
                                                                {
                                                                    try
                                                                    {
                                                                        await Task.Delay(200);
                                                                        await mayCansService.CommandSetLo(maycan.Id,
                                                                            maycan.MaLo);
                                                                        await Task.Delay(50);
                                                                        var _size =
                                                                            mainService.VmSizeDinhHinh.Items.FirstOrDefault(x =>
                                                                                x.Ma == maycan.MaSize);
                                                                        if (_size != null)
                                                                            await mayCansService.CommandSetSize(maycan.Id,
                                                                                maycan.MaSize,
                                                                                _size.Ten ?? "");
                                                                        await Task.Delay(50);
                                                                        var _thanhPham = mainService.VmThanhPhamDinhHinh.Items
                                                                            .FirstOrDefault(x =>
                                                                                x.Ma == maycan.MaThanhPham);
                                                                        if (_thanhPham != null)
                                                                            await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                                maycan.MaThanhPham,
                                                                                _thanhPham?.Ten ?? "");
                                                                        await Task.Delay(50);
                                                                        var _nhanVien =
                                                                            mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                                x.MaNhanVien == maNhanVienTemp);
                                                                        if (_nhanVien != null)
                                                                        {
                                                                            maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                            await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                                maycan?.MaNhanVien ?? "",
                                                                                _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine(e);
                                                                        //throw;
                                                                    }
                                                                });
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "KHÔNG THỂ CẬP NHẬT PHIẾU CÂN BTP";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }
                            case AppKV.PhuPham:
                                {
                                    if (maycan.IsStated == false)
                                    {
                                        if (dataLite.IsWaiting == false)
                                        {
                                            maycan.The = dataLite.TheId;
                                            maycan.TheAddDateTime = DateTime.Now;
                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                            //await Task.Delay(0);
                                            await mayCansService.CommandSetWaiting(maycan.Id);
                                            //await Task.Delay(100);
                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 0,
                                                MessStr = "CHỜ CÂN BẰNG"
                                            };
                                            error.ErrorString = dataRes.MessStr;
                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            maycan.ColorString = null;
                                            isOK = true;
                                        }
                                        else
                                        {
                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                    else
                                    {
                                        if (maycan.TrongLuong < 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                            maycan.ColorString = "red";
                                        }
                                        else if (maycan.TrongLuong == 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                            maycan.ColorString = "red";
                                        }
                                        else
                                        {
                                            if (maycan?.MaKhachHang == null || maycan.MaKhachHang.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn khách hàng";
                                                break;
                                            }

                                            if (maycan?.MaPhuongTien == null || maycan.MaPhuongTien.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Phương Tiện";
                                                break;
                                            }

                                            if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn size";
                                                break;
                                            }

                                            if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Thành Phẩm";
                                                break;
                                            }

                                            var thanhPham =
                                                mainService.VmThanhPhamPhuPham.Items.FirstOrDefault(x =>
                                                    x.Ma == maycan.MaThanhPham);
                                            if (thanhPham != null)
                                            {
                                                var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                if (dataLite.TrongLuong > thanhPhamMax)
                                                {
                                                    error.ErrorString =
                                                        $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                    break;
                                                }
                                            }

                                            var ngayGio = DateTime.Now;

                                            var phieuCan = new PhieuCanPhuPham
                                            {
                                                MaLoaiCa =
                                                    mainService.VmLoaiCaPhuPham.Items.FirstOrDefault()?.Ma ??
                                                    "",
                                                MaMau =
                                                    mainService.VmMauNguyenLieu.Items.FirstOrDefault()?.Ma ?? "",
                                                MaMayTinhCan = maycan.Id,
                                                MaSize = maycan.MaSize,
                                                MaLoaiThanhPham = maycan.MaThanhPham,
                                                MaUserCan = maycan.Id,
                                                MaXuongSanXuat = maycan.MaXuong,
                                                Ngay = ngayGio.Date,
                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                TrongLuong = dataLite.TrongLuong,
                                                ThoiGianCan = ngayGio,
                                                MSL = maycan.MaLo,
                                                SuDung = true,
                                                MaPhuongTien = maycan.MaPhuongTien ?? "",
                                                NgayCan = ngayGio,
                                                NhaMuaHang = maycan.MaKhachHang
                                            };

                                            if (mainService.VmPhieuCanPhuPham.Insert(phieuCan) > 0)
                                            {
                                                maycan.TongSoRo++;
                                                maycan.TongTrongLuong += dataLite.TrongLuong;
                                                lock (_lockItems)
                                                {
                                                    maycan.Items.Insert(0, phieuCan);
                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                        maycan.Items.Remove(maycan.Items.Last());
                                                }

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    TrongLuongYeuCau = 0,
                                                    Id = dataLite.Id,
                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                    MaLo = maycan.MaLo,
                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                    MaNhanVienBanKiem = "",
                                                    MaSize = maycan.MaSize,
                                                    MaThanhPham = maycan.MaThanhPham,
                                                    MayCan = maycan.Id,
                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                    STT = 0,
                                                    Status = 0,
                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                    TheIdChucNang = "",
                                                    TrongLuongTra = 0,
                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                    TongSoRo = maycan.TongSoRo,
                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                    TongSoRoCan = maycan.TongSoRo,
                                                    MaNhanVien = maycan.MaNhanVien
                                                };
                                                error.ErrorString = dataRes.MessStr;
                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                maycan.ColorString = "greenyellow";
                                                isOK = true;
                                            }
                                            else
                                            {
                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                maycan.ColorString = "red";
                                            }

                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }

                                    break;
                                }
                            case AppKV.BTPDinhHinh:
                                {
                                    switch (maycan.AType)
                                    {
                                        case AppType._default:
                                            {
                                                if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                    maycan.TheRoAddDateTime != null)
                                                {
                                                    maycan.The = dataLite.TheId;
                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thanhPham =
                                                        mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (thanhPham != null)
                                                    {
                                                        var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                        if (dataLite.TrongLuong > thanhPhamMax)
                                                        {
                                                            error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }
                                                    }

                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(
                                                            DateTime.Now, maycan.TheRo, false);
                                                    var ngayGio = DateTime.Now;
                                                    if (phieuCanLast != null)
                                                    {
                                                        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                            x.MaNhanVien == phieuCanLast.MaNhanVien);

                                                        error.ErrorString =
                                                            @$"RỔ CÁ CHƯA SỬA - {nv?.MaHoSo}- {Math.Round((ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes, 1)} phút";
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }

                                                    else if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                             maycan.TheNhanVienAddDateTime != null && maycan.MaNhanVien != null)
                                                    {
                                                        if (maycan.IsStated == false)
                                                        {
                                                            if (maycan.TrongLuong < 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                            else if (maycan.TrongLuong == 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            else
                                                                error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else
                                                        {
                                                            var stt = maycan.Items
                                                                .Where(x => ((PhieuCanBTPDinhHinh)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanBTPDinhHinh)x).STT)
                                                                .DefaultIfEmpty(0).Max();
                                                            var phieuCan = new PhieuCanBTPDinhHinh
                                                            {
                                                                STT = stt + 1,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                Id = dataLite.Id,
                                                                MaLo = maycan.MaLo,
                                                                MaSize = maycan.MaSize,
                                                                MaNhanVien = maycan.MaNhanVien,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                CaTra = false,
                                                                ChiSanLuong = false,
                                                                GhiChu = "",
                                                                Gio = ngayGio.TimeOfDay,
                                                                IsEnabled = false,
                                                                IsOffline = false,
                                                                MaLoaiCa =
                                                                    mainService.VmLoaiCaDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                                MaMau = mainService.VmMauDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                        "",
                                                                MaMayCan = maycan.Id,
                                                                MaMayLangDa = "M1",
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaThe = maycan.TheRo,
                                                                MaUserCan = maycan.Id,
                                                                MaXuong = maycan.MaXuong,
                                                                Ngay = ngayGio.Date,
                                                                TrongLuongBu = 0,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu ?? ""
                                                            };
                                                            if (mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCan) > 0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 1,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = 0,
                                                                    Id = dataLite.Id,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = maycan.MaSize,
                                                                    MaThanhPham = maycan.MaThanhPham,
                                                                    MayCan = maycan.Id,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = 0,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = maycan.TongSoRo,
                                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                isOK = true;
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 0,
                                                            MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                        };
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        isOK = true;
                                                    }

                                                    //else
                                                    //{
                                                    //    if (dataLite.Stated != "ST")
                                                    //    {
                                                    //        error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                    //    }
                                                    //    else
                                                    //    {

                                                    //        var stt = maycan.Items.Select(x => ((PhieuCanTPFilletv2)x).STT)
                                                    //            .DefaultIfEmpty(0).Max();
                                                    //        var ngayGio = DateTime.Now;
                                                    //        var phieuCan = new PhieuCanBTPFilletv2
                                                    //        {
                                                    //            STT = stt + 1,
                                                    //            CaTra = false,
                                                    //            Gio = ngayGio.TimeOfDay,
                                                    //            MaThanhPham = maycan.MaThanhPham,
                                                    //            MaSize = maycan.MaSize,
                                                    //            MaLo = maycan.MaLo,
                                                    //            TrongLuongTare = dataLite.TrongLuongTare,
                                                    //            MaThe = maycan.TheRo,
                                                    //            MaXuong = maycan.MaXuong,
                                                    //            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                    //            MaMayCan = maycan.Id,
                                                    //            TrongLuong = dataLite.TrongLuong,
                                                    //            IsEnabled = false,
                                                    //            MaLoaiCa = mainService.VmLoaiCaFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                    //            MaMau = mainService.VmMauFillet.Items.FirstOrDefault()?.Ma ?? "",
                                                    //            MaUserCan = maycan.Id,
                                                    //            MaMayLangDa = "",
                                                    //            Ngay = ngayGio.Date,
                                                    //            Id = dataLite.Id
                                                    //        };

                                                    //        // Đại Thành Cân nhiều rổ
                                                    //        phieuCan.TrongLuongTare *= 2;
                                                    //        if (mainService.VmPhieuCanBtpFilletv2.Insert(phieuCan) > 0)
                                                    //        {
                                                    //            maycan.TongSoRo++;
                                                    //            maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                    //            //mainService.VmPhieuCanBtpFilletv2.Items.Add(phieuCan);
                                                    //            maycan.Items.Insert(0, phieuCan);
                                                    //            if (maycan.Items.Count > 15) maycan.Items.Remove(maycan.Items.Last());

                                                    //            maycan.TheRo = null;
                                                    //            maycan.The = dataLite.Id;
                                                    //            isOK = true;
                                                    //            var dataRes = new DataLiteRes
                                                    //            {
                                                    //                ChiSanLuong = 0,
                                                    //                DinhMucYeuCau = 0,
                                                    //                Id = dataLite.Id,
                                                    //                MaThanhPham = phieuCan.MaThanhPham,
                                                    //                MaLo = phieuCan.MaLo,
                                                    //                MaSize = phieuCan.MaSize,
                                                    //                TrongLuongTare = phieuCan.TrongLuongTare,
                                                    //                TrongLuongTra = 0,
                                                    //                MaNhanVienPhucVu = phieuCan.MaNhanVienPhucVu,
                                                    //                MayCan = phieuCan.MaMayCan,
                                                    //                NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                    //                Ngay = ngayGio.ToString("yyyyMMdd"),
                                                    //                STT = 0,
                                                    //                TheId = phieuCan.MaThe,
                                                    //                Status = 0,
                                                    //                TrongLuongNhan = phieuCan.TrongLuong,
                                                    //                TheIdChucNang = ""
                                                    //            };
                                                    //            dataErrorJson = JsonConvert.SerializeObject(dataRes);

                                                    //            break;
                                                    //        }

                                                    //        maycan.TheRo = null;
                                                    //        maycan.The = dataLite.Id;
                                                    //        error.ErrorString = "KHÔNG THỂ THÊM PHIẾU CÂN";
                                                    //    }
                                                    //}
                                                }


                                                break;
                                            }
                                        case AppType._type1:
                                            {
                                                if (maycan.IsNhanVienCongCu)
                                                {
                                                    var items = mainService.VmPhieuCanBTPDinhHinh.Gets<PhieuCanBTPDinhHinh>(
                                                        DateTime.Now, maycan.TheRo);
                                                    if (items.Any())
                                                    {
                                                        foreach (var item in items)
                                                        {
                                                            item.IsEnabled = false;
                                                            item.GhiChu = "Mở khoá phiếu cân";
                                                        }

                                                        var rl = mainService.VmPhieuCanBTPDinhHinh.Update(items);
                                                        error.ErrorString =
                                                            $"Đã mở khoá {rl} phiếu cân";
                                                        isOK = true;
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "Không tìm thấy RỔ CÁ";
                                                        maycan.ColorString = "red";
                                                    }

                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = null;
                                                    maycan.TheRoAddDateTime = null;
                                                    maycan.TheNhanVienAddDateTime = null;
                                                    break;
                                                }

                                                if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                    maycan.TheRoAddDateTime != null)
                                                {
                                                    maycan.The = dataLite.TheId;
                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thanhPham =
                                                        mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (thanhPham != null)
                                                    {
                                                        var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                        if (dataLite.TrongLuong > thanhPhamMax)
                                                        {
                                                            error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        var thanhPhamMin = (decimal)(thanhPham.Min ?? 0);
                                                        if (dataLite.TrongLuong < thanhPhamMin)
                                                        {
                                                            error.ErrorString =
                                                                $"TRỌNG LƯỢNG NHỎ HƠN GIỚI HẠN: {thanhPhamMin} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }
                                                    }

                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(
                                                            DateTime.Now, maycan.The);
                                                    var ngayGio = DateTime.Now;
                                                    var timelast = 100.0;
                                                    if (phieuCanLast != null)
                                                        timelast = (ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes;

                                                    var isKhongCanDauRa = false;
                                                    if (phieuCanLast != null &&
                                                        phieuCanLast.IsEnabled == false) //&& timelast < 5)
                                                    {
                                                        var _thanhPham =
                                                            mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                                x.Ma == phieuCanLast.MaThanhPham);
                                                        //var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                        //    x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                        var _thanhPhamName = _thanhPham?.Ten ?? phieuCanLast.MaThanhPham;
                                                        if (timelast < 1)
                                                        {
                                                            timelast = timelast * 60;
                                                            error.ErrorString =
                                                                @$"{_thanhPhamName} - {phieuCanLast.TrongLuong} CHƯA SỬA - {Math.Round(timelast, 1)} s";
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString =
                                                                @$"{_thanhPhamName} - {phieuCanLast.TrongLuong} CHƯA SỬA - {Math.Round(timelast, 1)} phút";
                                                        }

                                                        //await Task.Delay(200);
                                                        maycan.ColorString = "red";
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        var isWaiting = false;
                                                        if (maycan.IsStated == false)
                                                        {
                                                            if (maycan.TrongLuong < 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                            else if (maycan.TrongLuong == 0)
                                                                error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            else
                                                                error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                            if (dataLite.IsWaiting == false)
                                                            {
                                                                maycan.The = dataLite.TheId;
                                                                maycan.TheAddDateTime = DateTime.Now;
                                                                //error.ErrorString = "CHỜ CÂN BẰNG";
                                                                //await Task.Delay(0);
                                                                // await mayCansService.CommandSetWaiting(maycan.Id);
                                                                // //await Task.Delay(100);
                                                                // var dataRes = new DataLiteRes
                                                                // {
                                                                //     TheId = dataLite.TheId,
                                                                //     IsDataAction = 0,
                                                                //     MessStr = "CHỜ CÂN BẰNG"
                                                                // };
                                                                // error.ErrorString = dataRes.MessStr;
                                                                // dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                // maycan.ColorString = null;
                                                                // isWaiting = true;
                                                                // isOK = true;
                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 0,
                                                                    MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                // maycan.ColorString = null;
                                                                // isOK = true;
                                                                maycan.ColorString = "red";
                                                                maycan.TheRo = null;
                                                                maycan.TheNhanVien = null;
                                                                maycan.MaNhanVien = null;
                                                                maycan.The = null;
                                                                maycan.TheRoAddDateTime = null;
                                                                maycan.TheNhanVienAddDateTime = null;
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                                maycan.ColorString = "red";
                                                                maycan.TheRo = null;
                                                                maycan.TheNhanVien = null;
                                                                maycan.MaNhanVien = null;
                                                                maycan.The = null;
                                                                maycan.TheRoAddDateTime = null;
                                                                maycan.TheNhanVienAddDateTime = null;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var stt = maycan.Items
                                                                .Where(x => ((PhieuCanBTPDinhHinh)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanBTPDinhHinh)x).STT)
                                                                .DefaultIfEmpty(0).Max();
                                                            var phieuCan = new PhieuCanBTPDinhHinh
                                                            {
                                                                STT = stt + 1,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                Id = dataLite.Id,
                                                                MaLo = maycan.MaLo,
                                                                MaSize = maycan.MaSize,
                                                                MaNhanVien = maycan.MaNhanVien,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                CaTra = false,
                                                                GhiChu = "",
                                                                Gio = ngayGio.TimeOfDay,
                                                                IsEnabled = isKhongCanDauRa,
                                                                MaLoaiCa =
                                                                    mainService.VmLoaiCaDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                                MaMau = mainService.VmMauDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                        "",
                                                                MaMayCan = maycan.Id,
                                                                MaMayLangDa = "M1",
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaThe = maycan?.The ?? "",
                                                                MaUserCan = maycan?.Id ?? "",
                                                                MaXuong = maycan?.MaXuong ?? "",
                                                                Ngay = ngayGio.Date,
                                                                ChiSanLuong = false,
                                                                IsOffline = false,
                                                                TrongLuongBu = 0,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu ?? ""
                                                            };
                                                            if (mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCan) > 0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                try
                                                                {
                                                                    if (mainService.VmApp.ComName == nameof(ComNames.DAITHANH))
                                                                        await mayCansService.CommandSUCCESS3(maycan.Id);
                                                                    else
                                                                        await mayCansService.CommandSUCCESS(maycan.Id);

                                                                    await Task.Delay(200);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    // Console.WriteLine(e);
                                                                    // throw;
                                                                }

                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 2,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = 0,
                                                                    Id = dataLite.Id,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu ?? "",
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = maycan.MaSize,
                                                                    MaThanhPham = maycan.MaThanhPham,
                                                                    MayCan = maycan.Id,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = 0,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = maycan.TongSoRo,
                                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                isOK = true;
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        if (isWaiting == false)
                                                        {
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                }


                                                break;
                                            }

                                        case AppType._type2:
                                            break;
                                        case AppType._type3:
                                            if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                maycan.TheNhanVienAddDateTime != null)
                                            {
                                                maycan.The = dataLite.TheId;
                                                if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                {
                                                    if (mainService.VmApp.ComName == nameof(ComNames.NV))
                                                    {
                                                        maycan.MaLo = "LO_TEST";
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }


                                                }

                                                if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                {
                                                    if (mainService.VmApp.ComName == nameof(ComNames.NV))
                                                    {
                                                        maycan.MaSize = "0";
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                }

                                                if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Thành Phẩm";
                                                    maycan.ColorString = "red";
                                                    break;
                                                }

                                                var thanhPham =
                                                    mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                        x.Ma == maycan.MaThanhPham);
                                                if (thanhPham != null)
                                                {
                                                    var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                    if (dataLite.TrongLuong > thanhPhamMax)
                                                    {
                                                        error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thanhPhamMin = (decimal)(thanhPham.Min ?? 0);
                                                    if (dataLite.TrongLuong < thanhPhamMin)
                                                    {
                                                        error.ErrorString =
                                                            $"TRỌNG LƯỢNG NHỎ HƠN GIỚI HẠN: {thanhPhamMin} kg";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }
                                                }

                                                var phieuCanLast =
                                                    mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(
                                                        DateTime.Now, maycan.TheNhanVien, false);
                                                var ngayGio = DateTime.Now;
                                                if (phieuCanLast != null)
                                                {
                                                    var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                        x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                    error.ErrorString =
                                                        @$"RỔ CÁ CHƯA SỬA - {nv?.MaHoSo}- {Math.Round((ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes, 1)} phút";
                                                    maycan.ColorString = "red";
                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = null;
                                                    maycan.TheRoAddDateTime = null;
                                                    maycan.TheNhanVienAddDateTime = null;
                                                }

                                                else if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                         maycan.TheNhanVienAddDateTime != null && maycan.MaNhanVien != null)
                                                {
                                                    var isWaiting = false;
                                                    if (maycan.IsStated == false)
                                                    {
                                                        if (maycan.TrongLuong < 0)
                                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                        else if (maycan.TrongLuong == 0)
                                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                        else
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                        maycan.ColorString = "red";
                                                        if (dataLite.IsWaiting == false)
                                                        {
                                                            maycan.The = dataLite.TheId;
                                                            maycan.TheAddDateTime = DateTime.Now;
                                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                                            //await Task.Delay(0);
                                                            //await mayCansService.CommandSetWaiting(maycan.Id);
                                                            //await Task.Delay(100);
                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 0,
                                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            // maycan.ColorString = null;
                                                            // isOK = true;
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var stt = maycan.Items
                                                            .Where(x => ((PhieuCanBTPDinhHinh)x).Ngay.Date == ngayGio.Date)
                                                            .Select(x => ((PhieuCanBTPDinhHinh)x).STT)
                                                            .DefaultIfEmpty(0).Max();
                                                        var phieuCan = new PhieuCanBTPDinhHinh
                                                        {
                                                            STT = stt + 1,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            Id = dataLite.Id,
                                                            MaLo = maycan.MaLo,
                                                            MaSize = maycan.MaSize,
                                                            MaNhanVien = maycan.MaNhanVien,
                                                            TrongLuong = dataLite.TrongLuong,
                                                            CaTra = false,
                                                            ChiSanLuong = false,
                                                            GhiChu = "",
                                                            Gio = ngayGio.TimeOfDay,
                                                            IsEnabled = false,
                                                            IsOffline = false,
                                                            MaLoaiCa =
                                                                mainService.VmLoaiCaDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                "",
                                                            MaMau = mainService.VmMauDinhHinh.Items.FirstOrDefault()?.Ma ??
                                                                    "",
                                                            MaMayCan = maycan.Id,
                                                            MaMayLangDa = "M1",
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MaThe = maycan.TheNhanVien,
                                                            MaUserCan = maycan.Id,
                                                            MaXuong = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            TrongLuongBu = 0
                                                        };
                                                        if (mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCan) > 0)
                                                        {
                                                            maycan.TongSoRo++;
                                                            maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                            lock (_lockItems)
                                                            {
                                                                maycan.Items.Insert(0, phieuCan);
                                                                if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                    maycan.Items.Remove(maycan.Items.Last());
                                                            }

                                                            var thongtintonghop =
                                                                mainService.VmPhieuCanBTPDinhHinh
                                                                    .GetSoRoTongTrongLuongByNhanVienId(ngayGio.Date,
                                                                        maycan.MaNhanVien);
                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 1,
                                                                MessStr = "ĐÃ HOÀN THÀNH",
                                                                ChiSanLuong = 0,
                                                                TrongLuongYeuCau = 0,
                                                                Id = dataLite.Id,
                                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                                MaLo = maycan.MaLo,
                                                                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu ?? "",
                                                                MaNhanVienBanKiem = "",
                                                                MaSize = maycan.MaSize,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MayCan = maycan.Id,
                                                                Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                STT = phieuCan.STT,
                                                                Status = 0,
                                                                TrongLuongNhan = dataLite.TrongLuong,
                                                                TheIdChucNang = "",
                                                                TrongLuongTra = 0,
                                                                TongTrongLuongCan = maycan.TongTrongLuong,
                                                                TongSoRo = thongtintonghop.Item1, //maycan.TongSoRo,
                                                                TongTrongLuong =
                                                                    thongtintonghop.Item2, //maycan.TongTrongLuong,
                                                                TongSoRoCan = maycan.TongSoRo,
                                                                MaNhanVien = maycan.MaNhanVien
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            maycan.ColorString = "greenyellow";
                                                            isOK = true;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                        }
                                                    }

                                                    if (isWaiting == false)
                                                    {
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                                else
                                                {
                                                    var dataRes = new DataLiteRes
                                                    {
                                                        TheId = dataLite.TheId,
                                                        IsDataAction = 0,
                                                        MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                    };
                                                    error.ErrorString = dataRes.MessStr;
                                                    dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                    isOK = true;
                                                }
                                            }

                                            break;
                                        case AppType._type4:
                                            break;
                                        case AppType._type1_2:
                                            {
                                                break;
                                            }
                                    }

                                    break;
                                }
                            case AppKV.TPDinhHinh:
                                {
                                    switch (maycan.AType)
                                    {
                                        case AppType._type3:
                                            {
                                                maycan.TheRo = dataLite.TheId;
                                                goto default;
                                            }
                                        case AppType._default:
                                            goto default;
                                        case AppType._type1:
                                            {
                                                var iscal = false;
                                                if (mainService.VmApp.ComName == nameof(ComNames.DAITHANH))
                                                {
                                                    if (dataLite.IsWaiting == false && maycan.TheRo != null &&
                                                        maycan.TheRo.Trim() != "" && maycan.TheRoAddDateTime != null)
                                                    {
                                                        var phieuCanLast =
                                                            mainService.VmPhieuCanBTPDinhHinh
                                                                .GetLastByTheId<PhieuCanBTPDinhHinh>(
                                                                    DateTime.Now, dataLite.TheId);

                                                        if ((phieuCanLast == null ||
                                                             (phieuCanLast != null && phieuCanLast.IsEnabled)) &&
                                                            maycan.IsKhongTheDauVao == false)
                                                        {
                                                            var timelast = 1000.0;
                                                            try
                                                            {
                                                                var _phieuCanLast = maycan.Items.FirstOrDefault();
                                                                if (_phieuCanLast != null && phieuCanLast != null)
                                                                {
                                                                    var _phieuCanLast2 = _phieuCanLast as PhieuCanTPDinhHinh;
                                                                    if (_phieuCanLast2.MaThe == phieuCanLast.MaThe)
                                                                        timelast = (DateTime.Now.TimeOfDay - _phieuCanLast2.Gio)
                                                                            .TotalSeconds;
                                                                }
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                // Console.WriteLine(e);
                                                                // throw;
                                                            }

                                                            if (timelast > 300)
                                                                error.ErrorString = @"CHƯA CÂN ĐẦU VÀO";
                                                            else
                                                                error.ErrorString = @$"Đã cân {timelast:#,##0.00}s";
                                                            isOK = false;
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                        else
                                                        {
                                                            if (maycan.IsKhongTheDauVao == false)
                                                            {
                                                                maycan.MaLo = phieuCanLast.MaLo;
                                                                maycan.MaSize = phieuCanLast.MaSize;
                                                                maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                                //maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                                maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                                maycan.STTBTP = phieuCanLast.STT;
                                                                maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                                maycan.IdIn = phieuCanLast.Id;
                                                                maycan.CaTra = phieuCanLast.CaTra;
                                                                maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                                maycan.MaMau = phieuCanLast.MaMau;
                                                                maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                            }
                                                            else
                                                            {
                                                                maycan.MaLo = phieuCanLast.MaLo;
                                                                maycan.MaSize = phieuCanLast.MaSize;
                                                                maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                                maycan.MaNhanVien = phieuCanLast.MaNhanVien;

                                                                maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                                maycan.STTBTP = phieuCanLast.STT;
                                                                maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                                maycan.IdIn = phieuCanLast.Id;
                                                                maycan.CaTra = phieuCanLast.CaTra;
                                                                maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                                maycan.MaMau = phieuCanLast.MaMau;
                                                                maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                            }

                                                            // var dataRes = new DataLiteRes
                                                            // {
                                                            //     TheId = dataLite.TheId,
                                                            //     IsDataAction = 0,
                                                            //     MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                            // };
                                                            // error.ErrorString = dataRes.MessStr;
                                                            // dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            // isOK = true;
                                                            _ = Task.Run(async () =>
                                                            {
                                                                try
                                                                {
                                                                    await Task.Delay(200);
                                                                    await mayCansService.CommandSetLo(maycan.Id,
                                                                        maycan.MaLo);
                                                                    await Task.Delay(50);
                                                                    var _size =
                                                                        mainService.VmSizeDinhHinh.Items.FirstOrDefault(x =>
                                                                            x.Ma == maycan.MaSize);
                                                                    if (_size != null)
                                                                        await mayCansService.CommandSetSize(maycan.Id,
                                                                            maycan.MaSize,
                                                                            _size.Ten ?? "");
                                                                    await Task.Delay(50);
                                                                    var _thanhPham = mainService.VmThanhPhamDinhHinh.Items
                                                                        .FirstOrDefault(x =>
                                                                            x.Ma == maycan.MaThanhPham);
                                                                    if (_thanhPham != null)
                                                                        await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                            maycan.MaThanhPham,
                                                                            _thanhPham?.Ten ?? "");
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine(e);
                                                                    //throw;
                                                                }
                                                            });
                                                            //iscal = true;
                                                        }
                                                        //iscal = true;
                                                    }

                                                    maycan.ThongBao =
                                                        $"TR {maycan.TheRo} - {maycan.TheNhanVien} - {maycan.TheNhanVienAddDateTime}";
                                                    if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                        maycan.TheNhanVien != null &&
                                                        maycan.TheNhanVien.Trim() != "" &&
                                                        maycan.TheNhanVienAddDateTime != null)
                                                    {
                                                        iscal = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (dataLite.IsWaiting == false && maycan.TheRo != null &&
                                                        maycan.TheRo.Trim() != "" && maycan.TheRoAddDateTime != null)
                                                    {
                                                        var phieuCanLast =
                                                            mainService.VmPhieuCanBTPDinhHinh
                                                                .GetLastByTheId<PhieuCanBTPDinhHinh>(
                                                                    DateTime.Now, dataLite.TheId);

                                                        if ((phieuCanLast == null ||
                                                             (phieuCanLast != null && phieuCanLast.IsEnabled)) &&
                                                            maycan.IsKhongTheDauVao == false)
                                                        {
                                                            var timelast = 1000.0;
                                                            try
                                                            {
                                                                var _phieuCanLast = maycan.Items.FirstOrDefault();
                                                                if (_phieuCanLast != null && phieuCanLast != null)
                                                                {
                                                                    var _phieuCanLast2 = _phieuCanLast as PhieuCanTPDinhHinh;
                                                                    if (_phieuCanLast2.MaThe == phieuCanLast.MaThe)
                                                                        timelast = (DateTime.Now.TimeOfDay - _phieuCanLast2.Gio)
                                                                            .TotalSeconds;
                                                                }
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                // Console.WriteLine(e);
                                                                // throw;
                                                            }

                                                            if (timelast > 300)
                                                                error.ErrorString = @"CHƯA CÂN ĐẦU VÀO";
                                                            else
                                                                error.ErrorString = @$"Đã cân {timelast:#,##0.00}s";
                                                            isOK = false;
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                            // if (_thanhPham != null)
                                                            // {
                                                            //     isKhongCanDauRa = _thanhPham.IsKhongDauRa;
                                                            // }
                                                        }
                                                        else
                                                        {
                                                            if (maycan.IsKhongTheDauVao == false)
                                                            {
                                                                maycan.MaLo = phieuCanLast.MaLo;
                                                                maycan.MaSize = phieuCanLast.MaSize;
                                                                maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                                maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                                maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                                maycan.STTBTP = phieuCanLast.STT;
                                                                maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                                maycan.IdIn = phieuCanLast.Id;
                                                                maycan.CaTra = phieuCanLast.CaTra;
                                                                maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                                maycan.MaMau = phieuCanLast.MaMau;
                                                                maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                            }
                                                            else
                                                            {
                                                                maycan.MaLo = phieuCanLast.MaLo;
                                                                maycan.MaSize = phieuCanLast.MaSize;
                                                                maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                                maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                                maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                                maycan.STTBTP = phieuCanLast.STT;
                                                                maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                                maycan.IdIn = phieuCanLast.Id;
                                                                maycan.CaTra = phieuCanLast.CaTra;
                                                                maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                                maycan.MaMau = phieuCanLast.MaMau;
                                                                maycan.ThoiGianBTP = phieuCanLast.Gio;
                                                            }

                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 0,
                                                                MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            isOK = true;
                                                            _ = Task.Run(async () =>
                                                            {
                                                                try
                                                                {
                                                                    await Task.Delay(200);
                                                                    await mayCansService.CommandSetLo(maycan.Id,
                                                                        maycan.MaLo);
                                                                    await Task.Delay(50);
                                                                    var _size =
                                                                        mainService.VmSizeDinhHinh.Items.FirstOrDefault(x =>
                                                                            x.Ma == maycan.MaSize);
                                                                    if (_size != null)
                                                                        await mayCansService.CommandSetSize(maycan.Id,
                                                                            maycan.MaSize,
                                                                            _size.Ten ?? "");
                                                                    await Task.Delay(50);
                                                                    var _thanhPham = mainService.VmThanhPhamDinhHinh.Items
                                                                        .FirstOrDefault(x =>
                                                                            x.Ma == maycan.MaThanhPham);
                                                                    if (_thanhPham != null)
                                                                        await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                            maycan.MaThanhPham,
                                                                            _thanhPham?.Ten ?? "");
                                                                    // await Task.Delay(50);
                                                                    // var _nhanVien =
                                                                    //     mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                    //         x.MaNhanVien == maycan.MaNhanVien);
                                                                    // if (_nhanVien != null)
                                                                    // {
                                                                    //     maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                    //     await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                    //         maycan?.MaNhanVien ?? "",
                                                                    //         _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                    // }
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine(e);
                                                                    //throw;
                                                                }
                                                            });
                                                            //iscal = true;
                                                        }
                                                    }
                                                    else if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                             maycan.TheNhanVien != null &&
                                                             maycan.TheNhanVien.Trim() != "" &&
                                                             maycan.TheNhanVienAddDateTime != null)
                                                    {
                                                        iscal = true;
                                                    }
                                                }


                                                if (iscal)
                                                {
                                                    if (maycan.IsStated == false)
                                                    {
                                                        if (maycan.TrongLuong < 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                            maycan.ColorString = "red";
                                                        }
                                                        else if (maycan.TrongLuong == 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            maycan.ColorString = "red";
                                                        }

                                                        else
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                        }

                                                        if (dataLite.IsWaiting == false)
                                                        {
                                                            maycan.The = dataLite.TheId;
                                                            maycan.TheAddDateTime = DateTime.Now;
                                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                                            //await Task.Delay(0);
                                                            // await Task.Delay(200);
                                                            // await mayCansService.CommandSetWaiting(maycan.Id);
                                                            // //await Task.Delay(100);
                                                            // var dataRes = new DataLiteRes
                                                            // {
                                                            //     TheId = dataLite.TheId,
                                                            //     IsDataAction = 0,
                                                            //     MessStr = "CHỜ CÂN BẰNG"
                                                            // };
                                                            // error.ErrorString = dataRes.MessStr;
                                                            // dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            // maycan.ColorString = null;
                                                            // isOK = true;
                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 0,
                                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            // maycan.ColorString = null;
                                                            // isOK = true;
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                            maycan.ColorString = "red";
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (maycan.TrongLuongNhan < dataLite.TrongLuong)
                                                        {
                                                            error.ErrorString =
                                                                $"Trọng lượng trả lớn hơn nhận {maycan.TrongLuongNhan}";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        maycan.The = maycan.TheRo;

                                                        if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Lô";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn size";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Thành Phẩm";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        if (maycan.MaNhanVien == null || maycan.MaNhanVien.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Nhân Viên";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }

                                                        // var _thanhPham =
                                                        //     mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                        //         x.Ma == maycan.MaThanhPham);
                                                        // if (_thanhPham != null)
                                                        // {
                                                        //     var thanhPhamMax = (decimal)(_thanhPham.Max ?? 0);
                                                        //     if (dataLite.TrongLuong > thanhPhamMax)
                                                        //     {
                                                        //         error.ErrorString =
                                                        //             $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                        //         maycan.ColorString = "red";
                                                        //         break;
                                                        //     }
                                                        // }

                                                        var ngayGio = DateTime.Now;
                                                        var stt = maycan.Items
                                                            .Where(x => ((PhieuCanTPDinhHinh)x).Ngay.Date == ngayGio.Date)
                                                            .Select(x => ((PhieuCanTPDinhHinh)x).STT)
                                                            .DefaultIfEmpty(0).Max();
                                                        if (maycan.IsKhongTheDauVao)
                                                        {
                                                            maycan.IdIn = maycan.Id;
                                                            maycan.STTBTP = stt + 1;
                                                            maycan.MayCanIdBTP = maycan.Id;
                                                            maycan.TrongLuongNhan = dataLite.TrongLuong;
                                                            var phieuCanBtp = new PhieuCanBTPDinhHinh
                                                            {
                                                                Id = maycan.IdIn,
                                                                CaTra = false,
                                                                GhiChu = "Không Cân Đầu vào",
                                                                Gio = ngayGio.TimeOfDay,
                                                                IsEnabled = true,
                                                                MaLo = maycan.MaLo,
                                                                MaLoaiCa = maycan.MaLoaiCa,
                                                                MaMau = maycan.MaMau,
                                                                MaMayLangDa = "M1",
                                                                MaMayCan = maycan.Id,
                                                                MaSize = maycan.MaSize,
                                                                MaThanhPham = maycan.MaThanhPham,
                                                                MaThe = maycan?.TheRo,
                                                                MaUserCan = maycan.Id,
                                                                MaXuong = maycan.MaXuong,
                                                                Ngay = ngayGio.Date,
                                                                STT = stt + 1,
                                                                TrongLuong = dataLite.TrongLuong,
                                                                TrongLuongTare = dataLite.TrongLuongTare
                                                            };
                                                            var rlInBTP = mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCanBtp);
                                                            if (rlInBTP <= 0)
                                                            {
                                                                error.ErrorString =
                                                                    "Không thể thêm đầu vào";
                                                                maycan.ColorString = "red";
                                                                break;
                                                            }
                                                        }

                                                        var thoiGianHT = (ngayGio.TimeOfDay - maycan.ThoiGianBTP).TotalMinutes;
                                                        var phieuCan = new PhieuCanTPDinhHinh
                                                        {
                                                            STT = stt + 1,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            Id = dataLite.Id,
                                                            MaLo = maycan.MaLo,
                                                            MaSize = maycan.MaSize,
                                                            MaNhanVien = maycan?.MaNhanVien,
                                                            TrongLuongTra = dataLite.TrongLuong,
                                                            CaTra = false,
                                                            GhiChu = "",
                                                            Gio = ngayGio.TimeOfDay,
                                                            MaLoaiCa =
                                                                maycan.MaLoaiCa,
                                                            MaMau = maycan.MaMau,
                                                            MaMayCan = maycan?.Id,
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MaThe = maycan?.TheRo,
                                                            MaUserCan = maycan.Id,
                                                            MaXuong = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            IdIn = maycan.IdIn ?? "",
                                                            MaMayCanBTP = maycan.MayCanIdBTP,
                                                            STTBTP = maycan.STTBTP,
                                                            TrongLuongNhan = maycan.TrongLuongNhan,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            ChiSanLuong = false,
                                                            MaNhanVienBanKiem = "",
                                                            SuDung = true,
                                                            IsOffline = false,
                                                            TrongLuongBu = 0,
                                                            ThoiGianHT = (decimal)Math.Round(thoiGianHT, 2)
                                                        };
                                                        var dinhMuc = mainService.VmDinhMucDinhHinh.Items
                                                            .FirstOrDefault(x => x.CaTra == maycan.CaTra &&
                                                                                 x.Ngay.Date == mainService.VmApp.DateTimeNow
                                                                                     .Date &&
                                                                                 x.MaLo == maycan.MaLo &&
                                                                                 x.MaLoaiCa == maycan.MaLoaiCa &&
                                                                                 x.MaMau == maycan.MaMau &&
                                                                                 x.MaSize == maycan.MaSize &&
                                                                                 x.MaThanhPham == maycan.MaThanhPham &&
                                                                                 x.MaXuong == maycan.MaXuong &&
                                                                                 x.SuDung);
                                                        var thanhPham =
                                                            mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                                x.Ma == maycan.MaThanhPham);

                                                        if (dinhMuc != null)
                                                        {
                                                            phieuCan.DinhMucYeuCau = (decimal)dinhMuc.DinhMuc;
                                                        }
                                                        else
                                                        {
                                                            if (maycan.CaTra)
                                                            {
                                                                //phieuCan.DinhMucYeuCau = dinhMuc.DinhMuc;
                                                            }
                                                            else
                                                            {
                                                                if (thanhPham != null)
                                                                    phieuCan.DinhMucYeuCau = (decimal)thanhPham.DinhMuc;
                                                            }
                                                        }

                                                        var dinhMucThuc = phieuCan.TrongLuongNhan == 0
                                                            ? 0
                                                            : Math.Round(phieuCan.TrongLuongNhan / phieuCan.TrongLuongTra, 2);
                                                        phieuCan.DinhMucThucTe = dinhMucThuc;


                                                        var trongLuongYeuCau = phieuCan.DinhMucYeuCau > 0
                                                            ? Math.Round(phieuCan.TrongLuongNhan / phieuCan.DinhMucYeuCau, 2)
                                                            : phieuCan
                                                                .TrongLuongTra; //phieuCan.DinhMucYeuCau * phieuCan.TrongLuongTra;
                                                        if (trongLuongYeuCau > phieuCan.TrongLuongTra)
                                                            phieuCan.DanhGia = false;
                                                        else
                                                            phieuCan.DanhGia = true;

                                                        if (trongLuongYeuCau == 0) trongLuongYeuCau = phieuCan.TrongLuongTra;

                                                        if (mainService.VmPhieuCanTPDinhHinh.Insert(phieuCan) > 0)
                                                        {
                                                            var insertedRows = 0;
                                                            if (mainService.VmApp.ComName == nameof(ComNames.DAITHANH))
                                                                insertedRows = mainService.VmPhieuCanBTPDinhHinh.Update(
                                                                    maycan.STTBTP ?? 0,
                                                                    ngayGio.Date, maycan.MayCanIdBTP, maycan.MaXuong, true);
                                                            else
                                                                insertedRows = mainService.VmPhieuCanBTPDinhHinh.Update(
                                                                    maycan.IdIn ?? "-",
                                                                    true);
                                                            //theem ham cap nhat by id
                                                            if (insertedRows >
                                                                0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                try
                                                                {
                                                                    await Task.Delay(100);
                                                                    if (mainService.VmApp.ComName == nameof(ComNames.DAITHANH))
                                                                    {
                                                                        var tyLe = phieuCan.TrongLuongTra <= 0
                                                                            ? 0
                                                                            : Math.Round(Math.Abs(
                                                                                    trongLuongYeuCau - phieuCan.TrongLuongTra) /
                                                                                phieuCan.TrongLuongTra, 2);

                                                                        // if(phieuCan.DanhGia == false)
                                                                        // {
                                                                        //     await mayCansService.CommandSUCCESS2(maycan.Id);
                                                                        // }
                                                                        // else
                                                                        // {
                                                                        if (phieuCan.DanhGia == true)
                                                                        {
                                                                            var tyLeDinhMucDau = thanhPham.TyLeDinhMucDau;
                                                                            if (mainService?.VmThanhPhamDinhHinhTyLe?.Items !=
                                                                                null)


                                                                            {
                                                                                var thanhPhamTyLe = mainService
                                                                                    .VmThanhPhamDinhHinhTyLe.Items
                                                                                    .Where(x =>
                                                                                        x.MaThanhPham == maycan.MaThanhPham &&
                                                                                        x.MaXuong == maycan.MaXuong)
                                                                                    .OrderByDescending(x => x.NgayGio)
                                                                                    .FirstOrDefault();

                                                                                if (thanhPhamTyLe != null)
                                                                                    tyLeDinhMucDau = thanhPhamTyLe.TyLeDau;
                                                                            }

                                                                            if (tyLe > tyLeDinhMucDau)
                                                                            {
                                                                                await mayCansService.CommandSUCCESS(maycan.Id);
                                                                                maycan.ErrorString =
                                                                                    $"{phieuCan.TrongLuongNhan}-{trongLuongYeuCau} - {tyLe} > {tyLeDinhMucDau}- VƯỢT TỶ LỆ ĐẦU";
                                                                            }
                                                                            else
                                                                            {
                                                                                await mayCansService.CommandSUCCESS3(maycan.Id);
                                                                                maycan.ErrorString =
                                                                                    $"{phieuCan.TrongLuongNhan}-{trongLuongYeuCau} - {tyLe} <= {tyLeDinhMucDau} - ĐẠT TỶ LỆ ĐẦU";
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            var tyLeDinhMucRot = thanhPham.TyLeDinhMucRot;
                                                                            if (mainService?.VmThanhPhamDinhHinhTyLe?.Items !=
                                                                                null)


                                                                            {
                                                                                var thanhPhamTyLe = mainService
                                                                                    .VmThanhPhamDinhHinhTyLe.Items
                                                                                    .Where(x =>
                                                                                        x.MaThanhPham == maycan.MaThanhPham &&
                                                                                        x.MaXuong == maycan.MaXuong)
                                                                                    .OrderByDescending(x => x.NgayGio)
                                                                                    .FirstOrDefault();

                                                                                if (thanhPhamTyLe != null)
                                                                                    tyLeDinhMucRot = thanhPhamTyLe.TyLeRot;
                                                                            }

                                                                            if (tyLe > tyLeDinhMucRot)
                                                                            {
                                                                                await mayCansService.CommandSUCCESS4(maycan.Id);
                                                                                maycan.ErrorString =
                                                                                    $"{phieuCan.TrongLuongNhan}-{trongLuongYeuCau} - {tyLe} > {tyLeDinhMucRot} - VƯỢT TỶ LỆ RỚT";
                                                                            }
                                                                            else
                                                                            {
                                                                                await mayCansService.CommandSUCCESS2(maycan.Id);
                                                                                maycan.ErrorString =
                                                                                    $"{phieuCan.TrongLuongNhan}-{trongLuongYeuCau}- {tyLe} <= {tyLeDinhMucRot} - RỚT";
                                                                            }
                                                                        }
                                                                        // await mayCansService.CommandSUCCESS(maycan.Id);
                                                                        // var tyleDau = mainService.VmThanhPhamDinhHinh
                                                                        //     .SelectedItem.TyLeDinhMucDau;
                                                                        // var thanhPhamTyLe = VmThanhPhamTyLe.Items
                                                                        //     .Where(x => x.MaThanhPham == VmThanhPham.SelectedItem.Ma &&
                                                                        //         x.MaXuong == VmXiNghiep.XiNghiepSelectedItem?.Ma)
                                                                        //     .OrderByDescending(x => x.NgayGio).FirstOrDefault();
                                                                        // if (thanhPhamTyLe != null) tyLeDinhMucDau = thanhPhamTyLe.TyLeDau;


                                                                        // }
                                                                    }
                                                                    else
                                                                    {
                                                                        await mayCansService.CommandSUCCESS(maycan.Id);
                                                                    }

                                                                    await Task.Delay(100);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    // Console.WriteLine(e);
                                                                    maycan.ErrorString = e.ToString();
                                                                    // throw;
                                                                }

                                                                var thongtintonghop =
                                                                    mainService.VmPhieuCanTPDinhHinh
                                                                        .GetSoRoTongTrongLuongByNhanVienId(ngayGio.Date,
                                                                            maycan.MaNhanVien);
                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 2,
                                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = trongLuongYeuCau,
                                                                    Id = phieuCan.Id,
                                                                    Id2 = phieuCan.IdIn,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                    MaNhanVienBanKiem = "",
                                                                    MaSize = phieuCan.MaSize,
                                                                    MaThanhPham = phieuCan.MaThanhPham,
                                                                    MayCan = phieuCan.MaMayCan,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = phieuCan.TrongLuongNhan,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = phieuCan.TrongLuongTra,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = thongtintonghop.Item1, //maycan.TongSoRo,
                                                                    TongTrongLuong =
                                                                        thongtintonghop.Item2, //maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien,
                                                                    DinhMucThucTe = phieuCan.DinhMucThucTe
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                var maNhanVienTemp = maycan.MaNhanVien;
                                                                isOK = true;
                                                                // _ = Task.Run(async () =>
                                                                // {
                                                                //     try
                                                                //     {
                                                                //         await Task.Delay(200);
                                                                //         await mayCansService.CommandSetLo(maycan.Id,
                                                                //             maycan.MaLo);
                                                                //         await Task.Delay(50);
                                                                //         var _size =
                                                                //             mainService.VmSizeFillet.Items.FirstOrDefault(
                                                                //                 x =>
                                                                //                     x.Ma == maycan.MaSize);
                                                                //         if (_size != null)
                                                                //             await mayCansService.CommandSetSize(maycan.Id,
                                                                //                 maycan.MaSize,
                                                                //                 _size.Ten ?? "");
                                                                //         await Task.Delay(50);
                                                                //         var _thanhPham = mainService.VmThanhPhamFillet.Items
                                                                //             .FirstOrDefault(x =>
                                                                //                 x.Ma == maycan.MaThanhPham);
                                                                //         if (_thanhPham != null)
                                                                //             await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                //                 maycan.MaThanhPham,
                                                                //                 _thanhPham?.Ten ?? "");
                                                                //         await Task.Delay(50);
                                                                //         var _nhanVien =
                                                                //             mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                //                 x.MaNhanVien == maNhanVienTemp);
                                                                //         if (_nhanVien != null)
                                                                //         {
                                                                //             maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                //             await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                //                 maycan?.MaNhanVien ?? "",
                                                                //                 _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                //         }
                                                                //     }
                                                                //     catch (Exception e)
                                                                //     {
                                                                //         Console.WriteLine(e);
                                                                //         //throw;
                                                                //     }
                                                                // });
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "KHÔNG THỂ CẬP NHẬT PHIẾU CÂN BTP";
                                                                maycan.ColorString = "red";
                                                            }
                                                        }

                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }
                                                else
                                                {
                                                    maycan.ColorString = "red";
                                                }
                                            }
                                            break;

                                        case AppType._type2:
                                            break;

                                        case AppType._type4:
                                            break;
                                        case AppType._type1_2:
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                var iscal = false;
                                                if (dataLite.IsWaiting == false)
                                                {
                                                    var phieuCanLast =
                                                        mainService.VmPhieuCanBTPDinhHinh.GetLastByTheId<PhieuCanBTPDinhHinh>(
                                                            DateTime.Now, dataLite.TheId, false);
                                                    if (phieuCanLast == null && maycan.IsChiSangLuong == false)
                                                    {
                                                        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                            x.MaNhanVien == phieuCanLast?.MaNhanVien);
                                                        error.ErrorString = @"CHƯA CÂN ĐẦU VÀO";
                                                        isOK = false;
                                                        maycan.ColorString = "red";
                                                        InsertLogLoiCan(maycan,dataLite,"CHƯA CÂN ĐẦU VÀO");
                                                        //SetError(maycan, dataLite,cmd,"CHƯA CÂN ĐẦU VÀO");
                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                    else
                                                    {
                                                        if (phieuCanLast == null && maycan.IsChiSangLuong == true)
                                                        {
                                                            iscal = true;
                                                        }
                                                        else
                                                        {
                                                            if (phieuCanLast != null && maycan.IsChiSangLuong == false)
                                                            {
                                                                maycan.MaLo = phieuCanLast.MaLo;
                                                                maycan.MaSize = phieuCanLast.MaSize;
                                                                if (maycan.IsXacDinhLoaiThanhPham == false)
                                                                {
                                                                    maycan.MaThanhPham = phieuCanLast.MaThanhPham;
                                                                }

                                                                maycan.MaNhanVien = phieuCanLast.MaNhanVien;
                                                                maycan.MayCanIdBTP = phieuCanLast.MaMayCan;
                                                                maycan.STTBTP = phieuCanLast.STT;
                                                                maycan.TrongLuongNhan = phieuCanLast.TrongLuong;
                                                                maycan.IdIn = phieuCanLast.Id;
                                                                maycan.CaTra = phieuCanLast.CaTra;
                                                                maycan.MaLoaiCa = phieuCanLast.MaLoaiCa;
                                                                maycan.MaMau = phieuCanLast.MaMau;
                                                                iscal = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    iscal = true;
                                                }

                                                if (iscal)
                                                {
                                                    if (maycan.IsStated == false)
                                                    {
                                                        if (maycan.TrongLuong < 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                            
                                                            maycan.ColorString = "red";
                                                            InsertLogLoiCan(maycan,dataLite,"TRỌNG LƯỢNG NHỎ HƠN 0");
                                                            //SetError(maycan, dataLite,cmd,"TRỌNG LƯỢNG BẰNG 0");
                                                        }
                                                        else if (maycan.TrongLuong == 0)
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                            
                                                            maycan.ColorString = "red";
                                                            InsertLogLoiCan(maycan,dataLite,"TRỌNG LƯỢNG BẰNG 0");
                                                            //SetError(maycan, dataLite,cmd,"TRỌNG LƯỢNG BẰNG 0");
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                            
                                                            maycan.ColorString = "red";
                                                            InsertLogLoiCan(maycan,dataLite,"TRỌNG LƯỢNG KHÔNG CÂN BẰNG");
                                                            //SetError(maycan, dataLite,cmd,"TRỌNG LƯỢNG KHÔNG CÂN BẰNG");
                                                        }

                                                        if (dataLite.IsWaiting == false)
                                                        {
                                                            maycan.The = dataLite.TheId;
                                                            maycan.TheAddDateTime = DateTime.Now;
                                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                                            //await Task.Delay(0);
                                                            // await Task.Delay(200);
                                                            // await mayCansService.CommandSetWaiting(maycan.Id);
                                                            //await Task.Delay(100);
                                                            var dataRes = new DataLiteRes
                                                            {
                                                                TheId = dataLite.TheId,
                                                                IsDataAction = 0,
                                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                                            };
                                                            error.ErrorString = dataRes.MessStr;
                                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                            // maycan.ColorString = null;
                                                            isOK = true;
                                                            maycan.ColorString = "red";
                                                            InsertLogLoiCan(maycan,dataLite,"TRỌNG LƯỢNG KHÔNG CÂN BẰNG");
                                                            //SetError(maycan, dataLite,cmd,"TRỌNG LƯỢNG KHÔNG CÂN BẰNG");
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                        else
                                                        {
                                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                                            
                                                            maycan.ColorString = "red";
                                                            //SetError(maycan, dataLite,cmd,"KHÔNG THỂ CHỜ CÂN BẰNG");
                                                            InsertLogLoiCan(maycan,dataLite,"KHÔNG THỂ CHỜ CÂN BẰNG");
                                                            
                                                            maycan.TheRo = null;
                                                            maycan.TheNhanVien = null;
                                                            maycan.MaNhanVien = null;
                                                            maycan.The = null;
                                                            maycan.TheRoAddDateTime = null;
                                                            maycan.TheNhanVienAddDateTime = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                        {
                                                            if (mainService.VmApp.ComName == nameof(ComNames.NV))
                                                            {
                                                                maycan.MaLo = "LO_TEST";
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "Chưa chọn Lô";
                                                                
                                                                maycan.ColorString = "red";
                                                                InsertLogLoiCan(maycan,dataLite,"Chưa chọn Lô");
                                                                //SetError(maycan, dataLite,cmd,"Chưa chọn Lô");
                                                                break;
                                                            }
                                                        }

                                                        if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                        {
                                                            if (mainService.VmApp.ComName == nameof(ComNames.NV))
                                                            {
                                                                maycan.MaSize = "0";
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "Chưa chọn size";
                                                                
                                                                maycan.ColorString = "red";
                                                                InsertLogLoiCan(maycan,dataLite,"Chưa chọn size");
                                                                //SetError(maycan, dataLite,cmd,"Chưa chọn size");
                                                                break;
                                                            }

                                                        }

                                                        if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                        {
                                                            error.ErrorString = "Chưa chọn Thành Phẩm";
                                                            
                                                            maycan.ColorString = "red";
                                                            //SetError(maycan, dataLite,cmd,"Chưa chọn Thành Phẩm");
                                                            InsertLogLoiCan(maycan,dataLite,"Chưa chọn Thành Phẩm");
                                                            break;
                                                        }

                                                        var _thanhPham =
                                                            mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                                x.Ma == maycan.MaThanhPham);
                                                        if (_thanhPham != null)
                                                        {
                                                            var thanhPhamMax = (decimal)(_thanhPham.Max ?? 0);
                                                            if (dataLite.TrongLuong > thanhPhamMax)
                                                            {
                                                                error.ErrorString =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                                var errStr =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                                
                                                                maycan.ColorString = "red";
                                                                InsertLogLoiCan(maycan,dataLite,errStr);
                                                                
                                                                //SetError(maycan, dataLite,cmd,errStr);
                                                                break;
                                                            }

                                                            var thanhPhamMin = (decimal)(_thanhPham.Min ?? 0);
                                                            if (mainService.VmApp.ComName == nameof(ComNames.NV) &&
                                                                maycan.IsChiSangLuong &&
                                                                (decimal.Compare(dataLite.TrongLuong, thanhPhamMax) == 1 ||
                                                                 decimal.Compare(dataLite.TrongLuong, thanhPhamMin) == -1))
                                                            {
                                                                error.ErrorString =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMin} - {thanhPhamMax} kg";
                                                                maycan.ColorString = "red";
                                                                var errStr =
                                                                    $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMin} - {thanhPhamMax} kg";
                                                                //SetError(maycan, dataLite,cmd,errStr);
                                                                InsertLogLoiCan(maycan,dataLite,errStr);
                                                                break;
                                                            }
                                                        }

                                                        if (dataLite.TrongLuong >= maycan.TrongLuongNhan && maycan.IsChiSangLuong == false)
                                                        {
                                                            error.ErrorString =
                                                                "TRỌNG LƯỢNG KHÔNG ĐƯỢC LỚN HƠN TRỌNG LƯỢNG NHẬN";
                                                            maycan.ColorString = "red";
                                                            //SetError(maycan, dataLite,cmd,"TRỌNG LƯỢNG KHÔNG ĐƯỢC LỚN HƠN TRỌNG LƯỢNG NHẬN");
                                                            InsertLogLoiCan(maycan,dataLite,"TRỌNG LƯỢNG KHÔNG ĐƯỢC LỚN HƠN TRỌNG LƯỢNG NHẬN");
                                                            break;
                                                        }

                                                        var ngayGio = DateTime.Now;
                                                        var stt = maycan.Items
                                                            .Where(x => ((PhieuCanTPDinhHinh)x).Ngay.Date == ngayGio.Date)
                                                            .Select(x => ((PhieuCanTPDinhHinh)x).STT)
                                                            .DefaultIfEmpty(0).Max();
                                                        if (maycan.IsChiSangLuong)
                                                        {
                                                            maycan.STTBTP = stt + 1;
                                                            maycan.MayCanIdBTP = maycan.Id;
                                                            maycan.TrongLuongNhan = dataLite.TrongLuong;
                                                            maycan.IdIn = maycan.Id;
                                                            maycan.MaLoaiCa = "A";
                                                            maycan.MaMau = "0";
                                                        }


                                                        var phieuCan = new PhieuCanTPDinhHinh
                                                        {
                                                            STT = stt + 1,
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            Id = dataLite.Id,
                                                            MaLo = maycan.MaLo,
                                                            MaSize = maycan.MaSize,
                                                            MaNhanVien = maycan?.MaNhanVien,
                                                            TrongLuongTra = dataLite.TrongLuong,
                                                            CaTra = false,
                                                            ChiSanLuong = maycan.IsChiSangLuong,
                                                            GhiChu = "",
                                                            Gio = ngayGio.TimeOfDay,
                                                            IsOffline = false,
                                                            MaLoaiCa =
                                                                maycan.MaLoaiCa ?? "",
                                                            MaMau = maycan.MaMau ?? "",
                                                            MaMayCan = maycan?.Id,
                                                            MaThanhPham = maycan.MaThanhPham,
                                                            MaThe = maycan?.TheRo,
                                                            MaUserCan = maycan.Id,
                                                            MaXuong = maycan.MaXuong,
                                                            Ngay = ngayGio.Date,
                                                            TrongLuongBu = 0,
                                                            IdIn = maycan.IdIn,
                                                            MaMayCanBTP = maycan.MayCanIdBTP,
                                                            STTBTP = maycan.STTBTP,
                                                            SuDung = true,
                                                            TrongLuongNhan = maycan.TrongLuongNhan,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu
                                                        };

                                                        //var dinhMuc = mainService.VmDinhMucDinhHinh.Items
                                                        //    .FirstOrDefault(x => x.CaTra == maycan.CaTra &&
                                                        //                         x.Ngay.Date == mainService.VmApp.DateTimeNow
                                                        //                             .Date &&
                                                        //                         x.MaLo == maycan.MaLo &&
                                                        //                         x.MaLoaiCa == maycan.MaLoaiCa &&
                                                        //                         x.MaMau == maycan.MaMau &&
                                                        //                         x.MaSize == maycan.MaSize &&
                                                        //                         x.MaThanhPham == maycan.MaThanhPham &&
                                                        //                         x.MaXuong == maycan.MaXuong &&
                                                        //                         x.SuDung);


                                                        var dinhMuc = mainService.VmThanhPhamDinhHinh.Items
                                                            .FirstOrDefault(x => x.Ma == maycan.MaThanhPham && x.SuDung == true);
                                                        
                                                        if (dinhMuc != null)
                                                        {
                                                            phieuCan.DinhMucYeuCau = (decimal)dinhMuc.DinhMuc;
                                                        }
                                                        else
                                                        {
                                                            if (maycan.CaTra)
                                                            {
                                                                //phieuCan.DinhMucYeuCau = dinhMuc.DinhMuc;
                                                            }
                                                            else
                                                            {
                                                                var thanhPham =
                                                                    mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaThanhPham);
                                                                if (thanhPham != null)
                                                                    phieuCan.DinhMucYeuCau = (decimal)thanhPham.DinhMuc;
                                                            }
                                                        }

                                                        var dinhMucThuc = phieuCan.TrongLuongNhan == 0
                                                            ? 0
                                                            : Math.Round(phieuCan.TrongLuongNhan / phieuCan.TrongLuongTra, 2);
                                                        phieuCan.DinhMucThucTe = dinhMucThuc;
                                                        //var trongLuongYeuCau = phieuCan.DinhMucYeuCau * phieuCan.TrongLuongTra;
                                                        var trongLuongYeuCau = phieuCan.DinhMucYeuCau > 0
                                                            ? Math.Round(phieuCan.TrongLuongNhan / phieuCan.DinhMucYeuCau, 2)
                                                            : phieuCan.TrongLuongTra;
                                                        if (trongLuongYeuCau > phieuCan.TrongLuongTra)
                                                            phieuCan.DanhGia = false;
                                                        else
                                                            phieuCan.DanhGia = true;
                                                        if (maycan.IsChiSangLuong)
                                                        {
                                                            phieuCan.GhiChu = "Chỉ Sảng Lượng";
                                                            phieuCan.DinhMucChuan = dinhMuc.DinhMucKhongDauVao;
                                                            phieuCan.TrongLuongNhan = phieuCan.TrongLuongTra * dinhMuc.DinhMucKhongDauVao;
                                                            phieuCan.DinhMucThucTe = phieuCan.TrongLuongTra == 0
                                                                ? 0
                                                                : Math.Round(phieuCan.TrongLuongNhan / phieuCan.TrongLuongTra, 2);
                                                        }
                                                        if (mainService.VmPhieuCanTPDinhHinh.Insert(phieuCan) > 0)
                                                        {
                                                            var insertedRows = 0;
                                                            if (mainService.VmApp.ComName == nameof(ComNames.DAITHANH) ||
                                                                mainService.VmApp.ComName == nameof(ComNames.NV))
                                                            {
                                                                if (maycan.IsChiSangLuong)
                                                                {
                                                                    var phieucanBTP = new PhieuCanBTPDinhHinh()
                                                                    {
                                                                        STT = phieuCan.STT,
                                                                        TrongLuongTare = phieuCan.TrongLuongTare,
                                                                        Id = phieuCan.Id,
                                                                        MaLo = phieuCan.MaLo,
                                                                        MaSize = phieuCan.MaSize,
                                                                        MaNhanVien = phieuCan.MaNhanVien,
                                                                        CaTra = false,
                                                                        ChiSanLuong = phieuCan.ChiSanLuong,
                                                                        GhiChu = "Chi Sảng Lượng",
                                                                        Gio = ngayGio.TimeOfDay,
                                                                        IsOffline = false,
                                                                        MaLoaiCa =
                                                                            phieuCan.MaLoaiCa,
                                                                        MaMau = phieuCan.MaMau,
                                                                        MaMayCan = phieuCan.MaMayCan,
                                                                        MaThanhPham = phieuCan.MaThanhPham,
                                                                        MaThe = phieuCan.MaThe,
                                                                        MaUserCan = phieuCan.Id,
                                                                        MaXuong = phieuCan.MaXuong,
                                                                        Ngay = ngayGio.Date,
                                                                        TrongLuongBu = 0,
                                                                        IsEnabled = true,
                                                                        MaMayLangDa = "M1",
                                                                        TrongLuong = phieuCan.TrongLuongNhan,

                                                                    };
                                                                    insertedRows =
                                                                        mainService.VmPhieuCanBTPDinhHinh.Insert(phieucanBTP);
                                                                }
                                                                else
                                                                {
                                                                    if (maycan.IsXacDinhLoaiThanhPham)
                                                                    {
                                                                        insertedRows = mainService.VmPhieuCanBTPDinhHinh.Update(
                                                                            maycan.STTBTP ?? 0,
                                                                            ngayGio.Date, maycan.MayCanIdBTP, maycan.MaXuong, maycan.MaThanhPham,
                                                                            true);
                                                                    }
                                                                    else
                                                                    {
                                                                        insertedRows = mainService.VmPhieuCanBTPDinhHinh.Update(
                                                                            maycan.STTBTP ?? 0,
                                                                            ngayGio.Date, maycan.MayCanIdBTP, maycan.MaXuong,
                                                                            true);
                                                                    }
                                                                }
                                                            }

                                                            else
                                                                insertedRows = mainService.VmPhieuCanBTPDinhHinh.Update(
                                                                    maycan.IdIn ?? "-",
                                                                    true);

                                                            //theem ham cap nhat by id
                                                            if (insertedRows >
                                                                0)
                                                            {
                                                                maycan.TongSoRo++;
                                                                maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                                lock (_lockItems)
                                                                {
                                                                    maycan.Items.Insert(0, phieuCan);
                                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                        maycan.Items.Remove(maycan.Items.Last());
                                                                }

                                                                var thongtintonghop =
                                                                    mainService.VmPhieuCanTPDinhHinh
                                                                        .GetSoRoTongTrongLuongByNhanVienId(ngayGio.Date,
                                                                            maycan.MaNhanVien);
                                                                var tongTLTPNhan = mainService.VmPhieuCanTPDinhHinh.GetTongTrongLuongThanhPhamNhanByNhanVienId(ngayGio.Date,
                                                                            maycan.MaNhanVien, phieuCan.MaThanhPham);
                                                                var tongTLTP = mainService.VmPhieuCanTPDinhHinh.GetTongTrongLuongThanhPhamByNhanVienId(ngayGio.Date,
                                                                            maycan.MaNhanVien, phieuCan.MaThanhPham);
                                                                var dinhMucTP = tongTLTPNhan == 0
                                                            ? 0
                                                            : Math.Round(tongTLTPNhan / tongTLTP, 2);
                                                                var dataRes = new DataLiteRes
                                                                {
                                                                    TheId = dataLite.TheId,
                                                                    IsDataAction = 1,
                                                                    MessStr = maycan.IsChiSangLuong == true ? "Sảng Lượng" : "ĐÃ HOÀN THÀNH",
                                                                    ChiSanLuong = 0,
                                                                    TrongLuongYeuCau = trongLuongYeuCau,
                                                                    Id = phieuCan.Id,
                                                                    Id2 = phieuCan.IdIn,
                                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                                    MaLo = maycan.MaLo,
                                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                                    MaNhanVienBanKiem = phieuCan.MaNhanVienBanKiem,
                                                                    MaSize = phieuCan.MaSize,
                                                                    MaThanhPham = phieuCan.MaThanhPham,
                                                                    MayCan = phieuCan.MaMayCan,
                                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                                    STT = phieuCan.STT,
                                                                    Status = 0,
                                                                    TrongLuongNhan = phieuCan.TrongLuongNhan,
                                                                    TheIdChucNang = "",
                                                                    TrongLuongTra = phieuCan.TrongLuongTra,
                                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                                    TongSoRo = thongtintonghop.Item1, //maycan.TongSoRo,
                                                                    TongTrongLuong =
                                                                        thongtintonghop.Item2, //maycan.TongTrongLuong,
                                                                    TongSoRoCan = maycan.TongSoRo,
                                                                    MaNhanVien = maycan.MaNhanVien,
                                                                    DinhMucThucTe = phieuCan.DinhMucThucTe,
                                                                    TongTLTP = tongTLTP,
                                                                    DinhMucTP = dinhMucTP,
                                                                    DanhGia = phieuCan.DanhGia
                                                                };
                                                                error.ErrorString = dataRes.MessStr;
                                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                                maycan.ColorString = "greenyellow";
                                                                var maNhanVienTemp = maycan.MaNhanVien;
                                                                isOK = true;
                                                                _ = Task.Run(async () =>
                                                                {
                                                                    try
                                                                    {
                                                                        await Task.Delay(200);
                                                                        await mayCansService.CommandSetLo(maycan.Id,
                                                                            maycan.MaLo);
                                                                        await Task.Delay(50);
                                                                        var _size =
                                                                            mainService.VmSizeDinhHinh.Items.FirstOrDefault(x =>
                                                                                x.Ma == maycan.MaSize);
                                                                        if (_size != null)
                                                                            await mayCansService.CommandSetSize(maycan.Id,
                                                                                maycan.MaSize,
                                                                                _size.Ten ?? "");
                                                                        await Task.Delay(50);
                                                                        var _thanhPham = mainService.VmThanhPhamDinhHinh.Items
                                                                            .FirstOrDefault(x =>
                                                                                x.Ma == maycan.MaThanhPham);
                                                                        if (_thanhPham != null)
                                                                            await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                                maycan.MaThanhPham,
                                                                                _thanhPham?.Ten ?? "");
                                                                        await Task.Delay(50);
                                                                        var _nhanVien =
                                                                            mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                                x.MaNhanVien == maNhanVienTemp);
                                                                        if (_nhanVien != null)
                                                                        {
                                                                            maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                            await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                                maycan?.MaNhanVien ?? "",
                                                                                _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine(e);
                                                                        //throw;
                                                                    }
                                                                });
                                                            }
                                                            else
                                                            {
                                                                error.ErrorString = "KHÔNG THỂ CẬP NHẬT PHIẾU CÂN BTP";
                                                                maycan.ColorString = "red";
                                                                //SetError(maycan, dataLite,cmd,"KHÔNG THỂ CẬP NHẬT PHIẾU CÂN BTP");
                                                                InsertLogLoiCan(maycan,dataLite,"KHÔNG THỂ CẬP NHẬT PHIẾU CÂN BTP");
                                                            }
                                                        }

                                                        else
                                                        {
                                                            error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                            maycan.ColorString = "red";
                                                            InsertLogLoiCan(maycan,dataLite,"KHÔNG THỂ THỂ THÊM PHIẾU CÂN");
                                                            //SetError(maycan, dataLite,cmd,"KHÔNG THỂ THỂ THÊM PHIẾU CÂN");
                                                        }

                                                        maycan.TheRo = null;
                                                        maycan.TheNhanVien = null;
                                                        maycan.MaNhanVien = null;
                                                        maycan.The = null;
                                                        maycan.TheRoAddDateTime = null;
                                                        maycan.TheNhanVienAddDateTime = null;
                                                    }
                                                }

                                                //if (maycan.TheRo != null && maycan.TheRo.Trim() != "" &&
                                                //    maycan.TheRoAddDateTime != null)
                                                //{
                                                //    maycan.The = dataLite.TheId;
                                                //    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                //    {
                                                //        error.ErrorString = "Chưa chọn Lô";
                                                //        break;
                                                //    }

                                                //    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                //    {
                                                //        error.ErrorString = "Chưa chọn size";
                                                //        break;
                                                //    }

                                                //    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                //    {
                                                //        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                //        break;
                                                //    }

                                                //    var thanhPham =
                                                //        mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                                //            x.Ma == maycan.MaThanhPham);
                                                //    if (thanhPham != null)
                                                //    {
                                                //        var thanhPhamMax = (decimal)(thanhPham.Max ?? 0);
                                                //        if (dataLite.TrongLuong > thanhPhamMax)
                                                //        {
                                                //            error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                //            break;
                                                //        }
                                                //    }


                                                //    if (phieuCanLast != null)
                                                //    {
                                                //        var nv = mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                //            x.MaNhanVien == phieuCanLast.MaNhanVien);
                                                //        error.ErrorString =
                                                //            @$"RỔ CÁ CHƯA SỬA - {nv?.MaHoSo}- {Math.Round((ngayGio.TimeOfDay - phieuCanLast.Gio).TotalMinutes, 1)} phút";
                                                //        maycan.TheRo = null;
                                                //        maycan.TheNhanVien = null;
                                                //        maycan.MaNhanVien = null;
                                                //        maycan.The = null;
                                                //        maycan.TheRoAddDateTime = null;
                                                //        maycan.TheNhanVienAddDateTime = null;
                                                //    }

                                                //    else if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                //             maycan.TheNhanVienAddDateTime != null && maycan.MaNhanVien != null)
                                                //    {
                                                //        if (maycan.IsStated == false)
                                                //        {
                                                //            if (maycan.TrongLuong < 0)
                                                //                error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                                //            else if (maycan.TrongLuong == 0)
                                                //                error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                //            else
                                                //                error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                //        }
                                                //        else
                                                //        {
                                                //            var stt = maycan.Items.Select(x => ((PhieuCanBTPDinhHinh)x).STT)
                                                //                .DefaultIfEmpty(0).Max();
                                                //            var phieuCan = new PhieuCanBTPDinhHinh
                                                //            {
                                                //                STT = stt + 1,
                                                //                TrongLuongTare = dataLite.TrongLuongTare,
                                                //                Id = dataLite.Id,
                                                //                MaLo = maycan.MaLo,
                                                //                MaSize = maycan.MaSize,
                                                //                MaNhanVien = maycan.MaNhanVien,
                                                //                TrongLuong = dataLite.TrongLuong,
                                                //                CaTra = false,
                                                //                ChiSanLuong = false,
                                                //                GhiChu = "",
                                                //                Gio = ngayGio.TimeOfDay,
                                                //                IsEnabled = false,
                                                //                IsOffline = false,
                                                //                MaLoaiCa =
                                                //                    mainService.VmLoaiCaDinhHinh.Items.FirstOrDefault()?.Ma ?? "",
                                                //                MaMau = mainService.VmMauDinhHinh.Items.FirstOrDefault()?.Ma ?? "",
                                                //                MaMayCan = maycan.Id,
                                                //                MaMayLangDa = "M1",
                                                //                MaThanhPham = maycan.MaThanhPham,
                                                //                MaThe = maycan.TheRo,
                                                //                MaUserCan = maycan.Id,
                                                //                MaXuong = maycan.MaXuong,
                                                //                Ngay = ngayGio.Date,
                                                //                TrongLuongBu = 0
                                                //            };
                                                //            if (mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCan) > 0)
                                                //            {
                                                //                maycan.TongSoRo++;
                                                //                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                //                maycan.Items.Insert(0, phieuCan);
                                                //                if (maycan.Items.Count > 15)
                                                //                    maycan.Items.Remove(maycan.Items.Last());

                                                //                var dataRes = new DataLiteRes
                                                //                {
                                                //                    TheId = dataLite.TheId,
                                                //                    IsDataAction = 1,
                                                //                    MessStr = "ĐÃ HOÀN THÀNH",
                                                //                    ChiSanLuong = 0,
                                                //                    DinhMucYeuCau = 0,
                                                //                    Id = dataLite.Id,
                                                //                    TrongLuongTare = dataLite.TrongLuongTare,
                                                //                    MaLo = maycan.MaLo,
                                                //                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                //                    MaNhanVienBanKiem = "",
                                                //                    MaSize = maycan.MaSize,
                                                //                    MaThanhPham = maycan.MaThanhPham,
                                                //                    MayCan = maycan.Id,
                                                //                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                //                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                //                    STT = phieuCan.STT,
                                                //                    Status = 0,
                                                //                    TrongLuongNhan = dataLite.TrongLuong,
                                                //                    TheIdChucNang = "",
                                                //                    TrongLuongTra = 0
                                                //                };
                                                //                error.ErrorString = dataRes.MessStr;
                                                //                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                //                isOK = true;
                                                //            }
                                                //            else
                                                //            {
                                                //                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                //            }
                                                //        }

                                                //        maycan.TheRo = null;
                                                //        maycan.TheNhanVien = null;
                                                //        maycan.MaNhanVien = null;
                                                //        maycan.The = null;
                                                //        maycan.TheRoAddDateTime = null;
                                                //        maycan.TheNhanVienAddDateTime = null;
                                                //    }
                                                //    else
                                                //    {
                                                //        var dataRes = new DataLiteRes
                                                //        {
                                                //            TheId = dataLite.TheId,
                                                //            IsDataAction = 0,
                                                //            MessStr = "QUÉT THẺ XÁC NHẬN NHÂN VIÊN"
                                                //        };
                                                //        error.ErrorString = dataRes.MessStr;
                                                //        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                //        isOK = true;
                                                //    }
                                                //}


                                                break;
                                            }
                                    }

                                    break;
                                }
                            case AppKV.XepKhuon:
                                {
                                    if (maycan.IsStated == false)
                                    {
                                        if (dataLite.IsWaiting == false)
                                        {
                                            maycan.The = dataLite.TheId;
                                            maycan.TheAddDateTime = DateTime.Now;
                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                            //await Task.Delay(0);
                                            // await Task.Delay(200);
                                            // await mayCansService.CommandSetWaiting(maycan.Id);
                                            // //await Task.Delay(100);
                                            // var dataRes = new DataLiteRes
                                            // {
                                            //     TheId = dataLite.TheId,
                                            //     IsDataAction = 0,
                                            //     MessStr = "CHỜ CÂN BẰNG"
                                            // };
                                            // error.ErrorString = dataRes.MessStr;
                                            // dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            // maycan.ColorString = null;
                                            // isOK = true;
                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 0,
                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                            };
                                            error.ErrorString = dataRes.MessStr;
                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            // maycan.ColorString = null;
                                            // isOK = true;
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                        else
                                        {
                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                    else
                                    {
                                        if (maycan.TrongLuong < 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                            maycan.ColorString = "red";
                                        }
                                        else if (maycan.TrongLuong == 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                            maycan.ColorString = "red";
                                        }
                                        else
                                        {
                                            var ngayGio = DateTime.Now;

                                            if (!coiService.CoiInfos.TryGetValue($"{maycan.MaCoi}_{maycan.MaXuong}",
                                                    out var coi))
                                            {
                                                error.ErrorString = "Không Tìm Thấy Thông Tin Cối";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            var thoiGian = (ngayGio - (coi.ThoiGianPhieuGanNhat != null
                                                ? coi.ThoiGianPhieuGanNhat
                                                : ngayGio))?.TotalMinutes;
                                            if (thoiGian != null && thoiGian <= coi.ThoiGianGiua2LanVaoCoi)
                                            {
                                                var trongLuongSoSanh = coi.TrongLuongHienTai + dataLite.TrongLuong -
                                                                       (decimal)coi.TrongLuongMax;
                                                var trongLuongCoiMaxTheoThanhPham =
                                                    mainService.VmTrongLuongCoiTheoThanhPham.Find(coi.Ma,
                                                        maycan?.MaThanhPham ?? "", maycan?.MaXuong ?? "");


                                                var trongLuongTrenhLech = (trongLuongCoiMaxTheoThanhPham?.TrongLuongMax ??
                                                                           (decimal)coi.TrongLuongMax) * 0.01m;
                                                if (trongLuongSoSanh > trongLuongTrenhLech)
                                                {
                                                    error.ErrorString =
                                                        $"Trọng Lượng Vượt {trongLuongSoSanh.ToString("###.00")} +/- {trongLuongTrenhLech.ToString("###.00")}";
                                                    maycan.ColorString = "red";
                                                    break;
                                                }
                                                //coi.TrongLuongHienTai += dataLite.TrongLuong;
                                                //coi.ThoiGianPhieuGanNhat = ngayGio;
                                            }
                                            else
                                            {
                                                coi.TrongLuongHienTai = 0;
                                                coi.SoRo = 0;
                                            }

                                            if (maycan.TheNhanVien != null && maycan.TheNhanVien.Trim() != "" &&
                                                maycan.TheNhanVienAddDateTime != null && maycan.MaNhanVien != null)
                                            {
                                                if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Lô";
                                                    break;
                                                }

                                                if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn size";
                                                    break;
                                                }

                                                if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Thành Phẩm";
                                                    break;
                                                }

                                                var thanhPham =
                                                    mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                                                        x.Ma == maycan.MaThanhPham);
                                                if (thanhPham != null)
                                                {
                                                    var thanhPhamMax = (decimal)thanhPham.Max;
                                                    if (dataLite.TrongLuong > thanhPhamMax)
                                                    {
                                                        error.ErrorString =
                                                            $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                        break;
                                                    }

                                                    var thanhPhamMin = (decimal)thanhPham.Min;
                                                    if (dataLite.TrongLuong < thanhPhamMin)
                                                    {
                                                        error.ErrorString =
                                                            $"TRỌNG LƯỢNG DƯỚI GIỚI HẠN: {thanhPhamMin} kg";
                                                        break;
                                                    }
                                                }

                                                if (maycan.MaCoi == null || maycan.MaCoi.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Cối";
                                                    maycan.ColorString = "red";
                                                    break;
                                                }

                                                if (maycan.MaChatLuong == null || maycan.MaChatLuong.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Chất Lượng";
                                                    maycan.ColorString = "red";
                                                    break;
                                                }

                                                if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                                {
                                                    error.ErrorString = "Chưa chọn Chiếu Xạ";
                                                    maycan.ColorString = "red";
                                                    maycan.MaChieuXa = mainService.VmChieuXaXepKhuon.Items
                                                        .FirstOrDefault()?.Ma ?? "";
                                                    // break;
                                                }
                                                // if (maycan?.MaCoiTam == null || maycan.MaCoiTam.Trim() == "")
                                                // {
                                                //     error.ErrorString = "Chưa chọn Còi Tạm";
                                                //     break;
                                                // }
                                                //
                                                // if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                                // {
                                                //     error.ErrorString = "Chưa chọn Chiếu Xạ";
                                                //     break;
                                                // }

                                                //var ngayGio = DateTime.Now;
                                                var stt = 0;
                                                try
                                                {
                                                    stt = maycan.Items
                                                        .Where(x => ((PhieuCanChinhXepKhuon)x).NgayNguyenLieu.Date ==
                                                                    maycan.NgayNguyenLieu.Date)
                                                        .Select(x => ((PhieuCanChinhXepKhuon)x).STT)
                                                        .DefaultIfEmpty(0).Max();
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                    // throw;
                                                }

                                                if (maycan.NgayNguyenLieu == DateTime.MinValue)
                                                    maycan.NgayNguyenLieu = ngayGio.Date;

                                                if (maycan.Items.Count > 0)
                                                    try
                                                    {
                                                        var lastTime = maycan.Items
                                                            .Where(x => ((PhieuCanChinhXepKhuon)x).Ngay.Date ==
                                                                        ngayGio.Date)
                                                            .Select(x => ((PhieuCanChinhXepKhuon)x).ThoiGianBatDauQuay)
                                                            .DefaultIfEmpty(TimeSpan.MinValue).Max();
                                                        if (lastTime != null && lastTime.HasValue &&
                                                            lastTime.Value != TimeSpan.MinValue)
                                                        {
                                                            var timeout = (ngayGio.TimeOfDay - lastTime.Value).TotalSeconds;
                                                            if (Math.Abs(timeout) < 3)
                                                            {
                                                                error.ErrorString = "Thời Gian Quá Ngắn";
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine(e);
                                                        // throw;
                                                    }
                                                // if(maycan.NgayNguyenLieu.Date != ngayGio.Date)
                                                // {
                                                //     if(maycan.Items.Count > 0)
                                                //     {
                                                //         var lastItem = maycan.Items.FirstOrDefault();
                                                //             
                                                //         if (lastItem != null)
                                                //         {
                                                //             
                                                //         }
                                                //     }
                                                //     
                                                // }

                                                var phieuCan = new PhieuCanChinhXepKhuon
                                                {
                                                    MaLoaiCa =
                                                        mainService.VmLoaiCaXepKhuon.Items.FirstOrDefault()?.Ma ??
                                                        "",
                                                    MaMau =
                                                        "",
                                                    MaMayCan = maycan.Id,
                                                    MaChieuXa = maycan.MaChieuXa,
                                                    Id = dataLite.Id,
                                                    ChuyenXuong = false,
                                                    DaQuay = false,
                                                    Forced = false,
                                                    GhiChu = "",
                                                    Gio = ngayGio.TimeOfDay,
                                                    IdMonitor = "",
                                                    LuotQuay = 0,
                                                    MaChatLuong = maycan.MaChatLuong,
                                                    MaCoiChinh = maycan.MaCoi,
                                                    MaCoiTam = "",
                                                    MaKhuVuc = "",
                                                    MaLo = maycan.MaLo,
                                                    MaNhanVien = maycan.MaNhanVien,
                                                    MaUserCan = maycan.Id,
                                                    Ngay = ngayGio.Date,
                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                    TrongLuong = dataLite.TrongLuong,
                                                    MaNhanVienPvPhanCo = "",
                                                    MaNhom = "",
                                                    MaSizeChinh = maycan.MaSize,
                                                    MaThanhPhamChinh = maycan.MaThanhPham,
                                                    MaXuong = maycan.MaXuong,
                                                    MayQuay = "",
                                                    NgayBatDauQuay = ngayGio.Date,
                                                    NgayNguyenLieu = maycan.NgayNguyenLieu.Date,
                                                    NgayRaCoi = ngayGio.Date,
                                                    STT = stt + 1,
                                                    TaiChe = false,
                                                    ThoiGianBatDauQuay = ngayGio.TimeOfDay,
                                                    ThoiGianQuay = 999,
                                                    ThoiGianRaCoi = ngayGio.TimeOfDay
                                                };

                                                if (mainService.VmPhieuCanChinhXepKhuon.Insert(phieuCan) > 0)
                                                {
                                                    coi.TrongLuongHienTai += dataLite.TrongLuong;
                                                    coi.SoRo++;
                                                    coi.ThoiGianPhieuGanNhat = ngayGio;
                                                    maycan.TongSoRo = coi.SoRo;
                                                    maycan.TongTrongLuong = coi.TrongLuongHienTai;
                                                    lock (_lockItems)
                                                    {
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());
                                                    }

                                                    var dataRes = new DataLiteRes
                                                    {
                                                        TheId = dataLite.TheId,
                                                        IsDataAction = 1,
                                                        MessStr = "ĐÃ HOÀN THÀNH",
                                                        ChiSanLuong = 0,
                                                        TrongLuongYeuCau = 0,
                                                        Id = dataLite.Id,
                                                        TrongLuongTare = dataLite.TrongLuongTare,
                                                        MaLo = maycan.MaLo,
                                                        MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                        MaNhanVienBanKiem = "",
                                                        MaSize = maycan.MaSize,
                                                        MaThanhPham = maycan.MaThanhPham,
                                                        MayCan = maycan.Id,
                                                        Ngay = ngayGio.ToString("yyyyMMdd"),
                                                        NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                        STT = 0,
                                                        Status = 0,
                                                        TrongLuongNhan = dataLite.TrongLuong,
                                                        TheIdChucNang = "",
                                                        TrongLuongTra = 0,
                                                        TongTrongLuongCan = maycan.TongTrongLuong,
                                                        TongSoRo = maycan.TongSoRo,
                                                        TongTrongLuong = maycan.TongTrongLuong,
                                                        TongSoRoCan = maycan.TongSoRo,
                                                        MaNhanVien = maycan.MaNhanVien
                                                    };

                                                    error.ErrorString = dataRes.MessStr;
                                                    dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                    maycan.ColorString = "greenyellow";
                                                    isOK = true;
                                                }
                                                else
                                                {
                                                    error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                    maycan.ColorString = "red";
                                                }

                                                maycan.TheRo = null;
                                                maycan.TheNhanVien = null;
                                                maycan.MaNhanVien = null;
                                                maycan.The = null;
                                                maycan.TheRoAddDateTime = null;
                                                maycan.TheNhanVienAddDateTime = null;
                                            }
                                        }
                                    }

                                    break;
                                }
                            case AppKV.BaoTu:
                                break;
                            case AppKV.CaoThit:
                                break;
                            case AppKV.XepKhuonRaCoi:
                                {
                                    try
                                    {
                                        if (mainService.VmApp.ComName?.ToUpper() == nameof(ComNames.HL))
                                        {
                                            if (maycan.MaNhanVien != null)
                                            {
                                                if (maycan.IsStated == false)
                                                {
                                                    if (maycan.TrongLuong < 0)
                                                        error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                    else if (maycan.TrongLuong == 0)
                                                        error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                    else
                                                        error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                    maycan.ColorString = "red";
                                                }
                                                else
                                                {
                                                    //var coi = coiService.CoiInfos.FirstOrDefault(x =>
                                                    //    x.Ma == maycan.MaCoi && x.MaXuong == maycan.MaXuong);
                                                    //if (!Clients.TryGetValue(clientInfo.ConnectedId, out var item)) return;
                                                    var ngayGio = DateTime.Now;
                                                    if (!coiService.CoiInfos.TryGetValue($"{maycan.MaCoi}_{maycan.MaXuong}",
                                                            out var coi))
                                                    {
                                                        error.ErrorString = "Không Tìm Thấy Thông Tin Cối";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thoiGian = (ngayGio - (coi.ThoiGianPhieuGanNhatRa != null
                                                        ? coi.ThoiGianPhieuGanNhatRa
                                                        : ngayGio))?.TotalMinutes;
                                                    if (thoiGian == null || thoiGian > coi.ThoiGianGiua2LanVaoCoi)
                                                    {
                                                        coi.TrongLuongHienTaiRa = 0;
                                                        coi.SoRoRa = 0;
                                                    }
                                                    //var thoiGian = (ngayGio - (coi.ThoiGianPhieuGanNhat != null
                                                    //    ? coi.ThoiGianPhieuGanNhat
                                                    //    : ngayGio))?.TotalMinutes;
                                                    //if (thoiGian <= coi.ThoiGianGiua2LanVaoCoi)
                                                    //{
                                                    //    var trongLuongSoSanh = coi.TrongLuongHienTai + dataLite.TrongLuong -
                                                    //                           (decimal)coi.TrongLuongMax;
                                                    //    if (trongLuongSoSanh > 10)
                                                    //    {
                                                    //        error.ErrorString = $"Trọng Lượng Vượt {trongLuongSoSanh}";
                                                    //        maycan.ColorString = "red";
                                                    //        break;
                                                    //    }
                                                    //    //coi.TrongLuongHienTai += dataLite.TrongLuong;
                                                    //    //coi.ThoiGianPhieuGanNhat = ngayGio;
                                                    //}
                                                    //else
                                                    //{
                                                    //    coi.TrongLuongHienTai = 0;
                                                    //}


                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaCoi == null || maycan.MaCoi.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Cối";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var _thanhPham =
                                                        mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (_thanhPham != null)
                                                        if (maycan.ThamSoTangTrong == null || maycan.ThamSoTangTrong == 0M)
                                                            maycan.ThamSoTangTrong = _thanhPham.ThamSoTangTrong;

                                                    if (maycan.MaChatLuong == null || maycan.MaChatLuong.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Chất Lượng";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Chiều Xá";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }


                                                    var stt = maycan.Items
                                                        .Where(x => ((PhieuCanRaCoi)x).NgayNguyenLieu.Date == ngayGio.Date)
                                                        .Select(x => ((PhieuCanRaCoi)x).STT)
                                                        .DefaultIfEmpty(0).Max();
                                                    if (maycan.NgayNguyenLieu == DateTime.MinValue)
                                                        maycan.NgayNguyenLieu = ngayGio.Date;

                                                    if (maycan.Items.Count > 0)
                                                        try
                                                        {
                                                            var lastTime = maycan.Items
                                                                .Where(x => ((PhieuCanRaCoi)x).Ngay.Date == ngayGio.Date)
                                                                .Select(x => ((PhieuCanRaCoi)x).Gio)
                                                                .DefaultIfEmpty(TimeSpan.MinValue).Max();
                                                            if (lastTime != null &&
                                                                lastTime != TimeSpan.MinValue)
                                                            {
                                                                var timeout = (ngayGio.TimeOfDay - lastTime).TotalSeconds;
                                                                if (Math.Abs(timeout) < 3)
                                                                {
                                                                    error.ErrorString = "Thời Gian Quá Ngắn";
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine(e);
                                                            // throw;
                                                        }

                                                    var phieuCan = new PhieuCanRaCoi
                                                    {
                                                        STT = stt + 1,
                                                        TrongLuongTare = dataLite.TrongLuongTare,
                                                        Id = dataLite.Id,
                                                        MaLo = maycan.MaLo,
                                                        MaSize = maycan.MaSize,
                                                        MaNhanVien = maycan?.MaNhanVien ?? "",
                                                        GhiChu = "",
                                                        Gio = ngayGio.TimeOfDay,
                                                        MaThanhPham = maycan?.MaThanhPham ?? "",
                                                        MaThe = maycan?.TheRo ?? "",
                                                        MaXuong = maycan?.MaXuong ?? "",
                                                        Ngay = ngayGio.Date,
                                                        TrongLuong = dataLite.TrongLuong,
                                                        MaChatLuong = maycan?.MaChatLuong ?? "",
                                                        MaChieuXa = maycan?.MaChieuXa ?? "",
                                                        MayCan = maycan?.Id ?? "",
                                                        IdMonitor = maycan?.IdMonitor ?? "",
                                                        MaCoi = maycan?.MaCoi ?? "",
                                                        NgayNguyenLieu = maycan.NgayNguyenLieu.Date,
                                                        ThamSoTangTrong = maycan.ThamSoTangTrong ?? 1M
                                                    };
                                                    if (mainService.VmPhieuCanRaCoi.Insert(phieuCan) > 0)
                                                    {
                                                        //theem ham cap nhat by id
                                                        coi.TrongLuongHienTaiRa += dataLite.TrongLuong;
                                                        coi.SoRoRa++;
                                                        coi.ThoiGianPhieuGanNhatRa = ngayGio;
                                                        maycan.TongSoRo = coi.SoRoRa;
                                                        maycan.TongTrongLuong = coi.TrongLuongHienTaiRa;
                                                        lock (_lockItems)
                                                        {
                                                            maycan.Items.Insert(0, phieuCan);
                                                            if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                maycan.Items.Remove(maycan.Items.Last());
                                                        }

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            TrongLuongYeuCau = 0,
                                                            Id = phieuCan.Id,
                                                            Id2 = "",
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            MaLo = maycan.MaLo,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            MaNhanVienBanKiem = "",
                                                            MaSize = phieuCan.MaSize,
                                                            MaThanhPham = phieuCan.MaThanhPham,
                                                            MayCan = phieuCan.MayCan,
                                                            Ngay = ngayGio.ToString("yyyyMMdd"),
                                                            NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                            STT = phieuCan.STT,
                                                            Status = 0,
                                                            TrongLuongNhan = phieuCan.TrongLuong,
                                                            TheIdChucNang = "",
                                                            TrongLuongTra = phieuCan.TrongLuong,
                                                            MaNhanVien = maycan.MaNhanVien,
                                                            TongTrongLuongCan = maycan.TongTrongLuong,
                                                            TongSoRo = maycan.TongSoRo,
                                                            TongTrongLuong = maycan.TongTrongLuong,
                                                            TongSoRoCan = maycan.TongSoRo
                                                        };

                                                        // coi.TrongLuongHienTai -= dataLite.TrongLuong;
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        maycan.ColorString = "greenyellow";
                                                        var maNhanVienTemp = maycan.MaNhanVien;
                                                        isOK = true;
                                                        _ = Task.Run(async () =>
                                                        {
                                                            try
                                                            {
                                                                await Task.Delay(200);
                                                                await mayCansService.CommandSetLo(maycan.Id, maycan.MaLo);
                                                                await Task.Delay(50);
                                                                var _size =
                                                                    mainService.VmSizeDinhHinh.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaSize);
                                                                if (_size != null)
                                                                    await mayCansService.CommandSetSize(maycan.Id,
                                                                        maycan.MaSize,
                                                                        _size.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _thanhPham = mainService.VmThanhPhamDinhHinh.Items
                                                                    .FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaThanhPham);
                                                                if (_thanhPham != null)
                                                                    await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                        maycan.MaThanhPham,
                                                                        _thanhPham?.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _nhanVien =
                                                                    mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                        x.MaNhanVien == maNhanVienTemp);
                                                                if (_nhanVien != null)
                                                                {
                                                                    maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                    await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                        maycan?.MaNhanVien ?? "",
                                                                        _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                }

                                                                await Task.Delay(50);
                                                                var _chieuXa =
                                                                    mainService.VmChieuXaXepKhuon.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaChieuXa);
                                                                if (_chieuXa != null)
                                                                    await mayCansService.CommandSetChieuXa(maycan.Id,
                                                                        maycan.MaChieuXa,
                                                                        _chieuXa.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _chatLuong =
                                                                    mainService.VmChatLuong.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaChatLuong);
                                                                if (_chatLuong != null)
                                                                    await mayCansService.CommandSetChatLuong(maycan.Id,
                                                                        maycan.MaChatLuong,
                                                                        _chatLuong.Ten ?? "");
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine(e);
                                                                //throw;
                                                            }
                                                        });
                                                    }

                                                    else
                                                    {
                                                        error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                        maycan.ColorString = "red";
                                                    }

                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = null;
                                                    maycan.TheRoAddDateTime = null;
                                                    maycan.TheNhanVienAddDateTime = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (maycan.MaNhanVien != null)
                                            {
                                                if (maycan.IsStated == false)
                                                {
                                                    if (maycan.TrongLuong < 0)
                                                        error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";

                                                    else if (maycan.TrongLuong == 0)
                                                        error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                                    else
                                                        error.ErrorString = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG";
                                                    maycan.ColorString = "red";
                                                }
                                                else
                                                {
                                                    //var coi = coiService.CoiInfos.FirstOrDefault(x =>
                                                    //    x.Ma == maycan.MaCoi && x.MaXuong == maycan.MaXuong);
                                                    //if (!Clients.TryGetValue(clientInfo.ConnectedId, out var item)) return;
                                                    var ngayGio = DateTime.Now;
                                                    if (!coiService.CoiInfos.TryGetValue($"{maycan.MaCoi}_{maycan.MaXuong}",
                                                            out var coi))
                                                    {
                                                        error.ErrorString = "Không Tìm Thấy Thông Tin Cối";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var thoiGian = (ngayGio - (coi.ThoiGianPhieuGanNhat != null
                                                        ? coi.ThoiGianPhieuGanNhat
                                                        : ngayGio))?.TotalMinutes;
                                                    if (thoiGian > coi.ThoiGianGiua2LanVaoCoi)
                                                    {
                                                        coi.TrongLuongHienTaiRa = 0;
                                                        coi.SoRoRa = 0;
                                                    }
                                                    //var thoiGian = (ngayGio - (coi.ThoiGianPhieuGanNhat != null
                                                    //    ? coi.ThoiGianPhieuGanNhat
                                                    //    : ngayGio))?.TotalMinutes;
                                                    //if (thoiGian <= coi.ThoiGianGiua2LanVaoCoi)
                                                    //{
                                                    //    var trongLuongSoSanh = coi.TrongLuongHienTai + dataLite.TrongLuong -
                                                    //                           (decimal)coi.TrongLuongMax;
                                                    //    if (trongLuongSoSanh > 10)
                                                    //    {
                                                    //        error.ErrorString = $"Trọng Lượng Vượt {trongLuongSoSanh}";
                                                    //        maycan.ColorString = "red";
                                                    //        break;
                                                    //    }
                                                    //    //coi.TrongLuongHienTai += dataLite.TrongLuong;
                                                    //    //coi.ThoiGianPhieuGanNhat = ngayGio;
                                                    //}
                                                    //else
                                                    //{
                                                    //    coi.TrongLuongHienTai = 0;
                                                    //}


                                                    if (maycan.MaLo == null || maycan.MaLo.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Lô";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaSize == null || maycan.MaSize.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn size";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Thành Phẩm";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    if (maycan.MaCoi == null || maycan.MaCoi.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Cối";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    var _thanhPham =
                                                        mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                                                            x.Ma == maycan.MaThanhPham);
                                                    if (_thanhPham != null)
                                                    {
                                                        var thanhPhamMax = (decimal)_thanhPham.Max;
                                                        if (dataLite.TrongLuong > thanhPhamMax)
                                                        {
                                                            error.ErrorString =
                                                                $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                            maycan.ColorString = "red";
                                                            break;
                                                        }
                                                    }

                                                    if (maycan.MaChatLuong == null || maycan.MaChatLuong.Trim() == "")
                                                    {
                                                        error.ErrorString = "Chưa chọn Chất Lượng";
                                                        maycan.ColorString = "red";
                                                        break;
                                                    }

                                                    // if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                                    // {
                                                    //     error.ErrorString = "Chưa chọn Chiều Xá";
                                                    //     maycan.ColorString = "red";
                                                    //     break;
                                                    // }


                                                    var stt = maycan.Items
                                                        .Where(x => ((PhieuCanRaCoi)x).Ngay.Date == ngayGio.Date)
                                                        .Select(x => ((PhieuCanRaCoi)x).STT)
                                                        .DefaultIfEmpty(0).Max();
                                                    var phieuCan = new PhieuCanRaCoi
                                                    {
                                                        STT = stt + 1,
                                                        TrongLuongTare = dataLite.TrongLuongTare,
                                                        Id = dataLite.Id,
                                                        MaLo = maycan.MaLo,
                                                        MaSize = maycan.MaSize,
                                                        MaNhanVien = maycan?.MaNhanVien,
                                                        GhiChu = "",
                                                        Gio = ngayGio.TimeOfDay,
                                                        MaThanhPham = maycan.MaThanhPham,
                                                        MaThe = maycan?.TheRo,
                                                        MaXuong = maycan.MaXuong,
                                                        Ngay = ngayGio.Date,
                                                        TrongLuong = dataLite.TrongLuong,
                                                        MaChatLuong = maycan.MaChatLuong,
                                                        MaChieuXa = maycan.MaChieuXa,
                                                        MayCan = maycan.Id,
                                                        IdMonitor = maycan.IdMonitor ?? "",
                                                        MaCoi = maycan.MaCoi,
                                                        NgayNguyenLieu = maycan.NgayNguyenLieu
                                                    };
                                                    if (mainService.VmPhieuCanRaCoi.Insert(phieuCan) > 0)
                                                    {
                                                        //theem ham cap nhat by id
                                                        coi.TrongLuongHienTaiRa += dataLite.TrongLuong;
                                                        coi.SoRoRa++;
                                                        coi.ThoiGianPhieuGanNhatRa = ngayGio;
                                                        maycan.TongSoRo = coi.SoRoRa;
                                                        maycan.TongTrongLuong = coi.TrongLuongHienTaiRa;
                                                        lock (_lockItems)
                                                        {
                                                            maycan.Items.Insert(0, phieuCan);
                                                            if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                                maycan.Items.Remove(maycan.Items.Last());
                                                        }

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            TrongLuongYeuCau = 0,
                                                            Id = phieuCan.Id,
                                                            Id2 = "",
                                                            TrongLuongTare = dataLite.TrongLuongTare,
                                                            MaLo = maycan.MaLo,
                                                            MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                            MaNhanVienBanKiem = "",
                                                            MaSize = phieuCan.MaSize,
                                                            MaThanhPham = phieuCan.MaThanhPham,
                                                            MayCan = phieuCan.MayCan,
                                                            Ngay = ngayGio.ToString("yyyyMMdd"),
                                                            NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                            STT = phieuCan.STT,
                                                            Status = 0,
                                                            TrongLuongNhan = phieuCan.TrongLuong,
                                                            TheIdChucNang = "",
                                                            TrongLuongTra = phieuCan.TrongLuong,
                                                            MaNhanVien = maycan.MaNhanVien
                                                        };

                                                        // coi.TrongLuongHienTai -= dataLite.TrongLuong;
                                                        error.ErrorString = dataRes.MessStr;
                                                        dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                        maycan.ColorString = "greenyellow";
                                                        var maNhanVienTemp = maycan.MaNhanVien;
                                                        isOK = true;
                                                        _ = Task.Run(async () =>
                                                        {
                                                            try
                                                            {
                                                                await Task.Delay(200);
                                                                await mayCansService.CommandSetLo(maycan.Id, maycan.MaLo);
                                                                await Task.Delay(50);
                                                                var _size =
                                                                    mainService.VmSizeDinhHinh.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaSize);
                                                                if (_size != null)
                                                                    await mayCansService.CommandSetSize(maycan.Id,
                                                                        maycan.MaSize,
                                                                        _size.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _thanhPham = mainService.VmThanhPhamDinhHinh.Items
                                                                    .FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaThanhPham);
                                                                if (_thanhPham != null)
                                                                    await mayCansService.CommandSetThanhPham(maycan.Id,
                                                                        maycan.MaThanhPham,
                                                                        _thanhPham?.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _nhanVien =
                                                                    mainService.VmNhanVien.Items.FirstOrDefault(x =>
                                                                        x.MaNhanVien == maNhanVienTemp);
                                                                if (_nhanVien != null)
                                                                {
                                                                    maycan.MaHoSo = _nhanVien.MaHoSo;
                                                                    await mayCansService.CommandSetNhanVien(maycan.Id,
                                                                        maycan?.MaNhanVien ?? "",
                                                                        _nhanVien?.MaHoSo ?? "", _nhanVien?.Name ?? "");
                                                                }

                                                                await Task.Delay(50);
                                                                var _chieuXa =
                                                                    mainService.VmChieuXaXepKhuon.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaChieuXa);
                                                                if (_chieuXa != null)
                                                                    await mayCansService.CommandSetChieuXa(maycan.Id,
                                                                        maycan.MaChieuXa,
                                                                        _chieuXa.Ten ?? "");
                                                                await Task.Delay(50);
                                                                var _chatLuong =
                                                                    mainService.VmChatLuong.Items.FirstOrDefault(x =>
                                                                        x.Ma == maycan.MaChatLuong);
                                                                if (_chatLuong != null)
                                                                    await mayCansService.CommandSetChatLuong(maycan.Id,
                                                                        maycan.MaChatLuong,
                                                                        _chatLuong.Ten ?? "");
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine(e);
                                                                //throw;
                                                            }
                                                        });
                                                    }

                                                    else
                                                    {
                                                        error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                        maycan.ColorString = "red";
                                                    }

                                                    maycan.TheRo = null;
                                                    maycan.TheNhanVien = null;
                                                    maycan.MaNhanVien = null;
                                                    maycan.The = null;
                                                    maycan.TheRoAddDateTime = null;
                                                    maycan.TheNhanVienAddDateTime = null;
                                                }
                                            }
                                            else
                                            {
                                                var phieuMonitor = mainService.VmCoiMonitor.Items
                                                    .Where(x => x.MaThe == dataLite.TheId && x.MaXuong == maycan.MaXuong &&
                                                                ((x.OutLocked && x.InLocked == false) ||
                                                                 (x.OutLocked == false && x.InLocked)) &&
                                                                x.TimeROut != null)
                                                    .MaxBy(x => x.NgayGio);
                                                if (phieuMonitor != null)
                                                {
                                                    if (phieuMonitor.OutLocked)
                                                    {
                                                        var copyOfPhieuMonitor =
                                                            mainService.VmCoiMonitor.CopyItem(phieuMonitor);
                                                        copyOfPhieuMonitor.OutLocked = false;
                                                        copyOfPhieuMonitor.InLocked = true;
                                                        var rl = mainService.VmCoiMonitor.Update(copyOfPhieuMonitor);
                                                        if (rl < 0)
                                                        {
                                                            error.ErrorString = "KHÔNG THỂ CẬP NHẬT";
                                                        }
                                                        else
                                                        {
                                                            phieuMonitor.OutLocked = false;
                                                            phieuMonitor.InLocked = true;
                                                        }
                                                    }

                                                    var phieuCans =
                                                        mainService.VmPhieuCanChinhXepKhuon
                                                            .GetsByIdMonitor<PhieuCanChinhXepKhuon>(
                                                                phieuMonitor.Id);
                                                    if (phieuCans.Any())
                                                    {
                                                        var objs = mainService.VmPhieuCanChinhXepKhuon.Items
                                                            .GroupBy(x => new
                                                            {
                                                                x.MaLo,
                                                                x.MaThanhPhamChinh,
                                                                x.MaSizeChinh,
                                                                x.MaChatLuong,
                                                                x.MaChieuXa
                                                            }).Select(g => new
                                                            {
                                                                g.Key.MaLo,
                                                                g.Key.MaThanhPhamChinh,
                                                                g.Key.MaSizeChinh,
                                                                g.Key.MaChatLuong,
                                                                g.Key.MaChieuXa,
                                                                TrongLuong = g.Sum(x => x.TrongLuong)
                                                            }).OrderByDescending(x => x.TrongLuong).ToList();
                                                        var obj = objs.FirstOrDefault();
                                                        if (obj != null)
                                                        {
                                                            maycan.MaLo = obj.MaLo;
                                                            maycan.MaThanhPham = obj.MaThanhPhamChinh;
                                                            maycan.MaSize = obj.MaSizeChinh;
                                                            maycan.MaChatLuong = obj.MaChatLuong;
                                                            maycan.MaChieuXa = obj.MaChieuXa;
                                                            maycan.MaXuong = maycan.MaXuong;
                                                            maycan.MaCoi = phieuMonitor.MaCoi;
                                                            maycan.IdMonitor = phieuMonitor.Id;
                                                            maycan.NgayNguyenLieu = phieuMonitor.NgayNguyenLieu;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        error.ErrorString = "KHÔNG TÌM THẤY PHIẾU CÂN";
                                                    }
                                                }
                                                else
                                                {
                                                    error.ErrorString = "KHÔNG TÌM THẤY PHIẾU THEO DÕI";
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        maycan.MaNhanVien = null;
                                        Console.WriteLine(e);
                                        //throw;
                                    }

                                    break;
                                }
                            case AppKV.XepKhuonPhu:
                                {
                                    if (maycan.IsStated == false)
                                    {
                                        if (dataLite.IsWaiting == false)
                                        {
                                            maycan.The = dataLite.TheId;
                                            maycan.TheAddDateTime = DateTime.Now;
                                            //error.ErrorString = "CHỜ CÂN BẰNG";
                                            //await Task.Delay(0);
                                            //await Task.Delay(200);
                                            //await mayCansService.CommandSetWaiting(maycan.Id);
                                            //await Task.Delay(100);
                                            // var dataRes = new DataLiteRes
                                            // {
                                            //     TheId = dataLite.TheId,
                                            //     IsDataAction = 0,
                                            //     MessStr = "CHỜ CÂN BẰNG"
                                            // };
                                            // error.ErrorString = dataRes.MessStr;
                                            // dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            // maycan.ColorString = null;
                                            // isOK = true;
                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 0,
                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                            };
                                            error.ErrorString = dataRes.MessStr;
                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            // maycan.ColorString = null;
                                            // isOK = true;
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                        else
                                        {
                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                    else
                                    {
                                        if (maycan.TrongLuong < 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                            maycan.ColorString = "red";
                                        }
                                        else if (maycan.TrongLuong == 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                            maycan.ColorString = "red";
                                        }
                                        else
                                        {
                                            if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Lô";
                                                break;
                                            }

                                            // if (maycan?.MaPhuongTien == null || maycan.MaPhuongTien.Trim() == "")
                                            // {
                                            //     error.ErrorString = "Chưa chọn Phương Tiện";
                                            //     break;
                                            // }

                                            if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn size";
                                                break;
                                            }

                                            if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Thành Phẩm";
                                                break;
                                            }

                                            var thanhPham =
                                                mainService.VmThanhPhamPhuXepKhuon.Items.FirstOrDefault(x =>
                                                    x.Ma == maycan.MaThanhPham);
                                            if (thanhPham != null)
                                            {
                                                var thanhPhamMax = (decimal)thanhPham.Max;
                                                if (dataLite.TrongLuong > thanhPhamMax)
                                                {
                                                    error.ErrorString =
                                                        $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                    break;
                                                }
                                            }

                                            // if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                            // {
                                            //     error.ErrorString = "Chưa chọn Chiếu Xạ";
                                            //     maycan.ColorString = "red";
                                            //     break;
                                            // }

                                            var ngayGio = DateTime.Now;
                                            var stt = maycan.Items
                                                .Where(x => ((PhieuCanPhuXepKhuon)x).Ngay.Date == ngayGio.Date)
                                                .Select(x => ((PhieuCanPhuXepKhuon)x).STT)
                                                .DefaultIfEmpty(0).Max();
                                            var phieuCan = new PhieuCanPhuXepKhuon
                                            {
                                                MaLoaiCa =
                                                    mainService.VmLoaiCaXepKhuon.Items.FirstOrDefault()?.Ma ??
                                                    "",
                                                MaMau =
                                                    "",

                                                MaSize = maycan.MaSize,
                                                MaUserCan = dataLite.Id,
                                                Ngay = ngayGio.Date,
                                                TrongLuongTare = dataLite.TrongLuongTare,
                                                TrongLuong = dataLite.TrongLuong,
                                                Id = maycan.Id,
                                                Gio = ngayGio.TimeOfDay,
                                                MaLo = maycan.MaLo,
                                                MaXuong = maycan.MaXuong,
                                                MaThanhPham = maycan.MaThanhPham,
                                                GhiChu = "",
                                                MaMayCan = maycan.Id,
                                                MaNhanVien = maycan.MaNhanVien,
                                                STT = stt + 1,
                                                MaNhom = "",
                                                MaKhuVuc = "",
                                                MaChatLuong = maycan.MaChatLuong,
                                                MaChieuXa = maycan.MaChieuXa ?? ""
                                            };

                                            if (mainService.VmPhieuCanPhuXepKhuon.Insert(phieuCan) > 0)
                                            {
                                                maycan.TongSoRo++;
                                                maycan.TongTrongLuong += dataLite.TrongLuong;
                                                lock (_lockItems)
                                                {
                                                    maycan.Items.Insert(0, phieuCan);
                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                        maycan.Items.Remove(maycan.Items.Last());
                                                }

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    TrongLuongYeuCau = 0,
                                                    Id = dataLite.Id,
                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                    MaLo = maycan.MaLo,
                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                    MaNhanVienBanKiem = "",
                                                    MaSize = maycan.MaSize,
                                                    MaThanhPham = maycan.MaThanhPham,
                                                    MayCan = maycan.Id,
                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                    STT = 0,
                                                    Status = 0,
                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                    TheIdChucNang = "",
                                                    TrongLuongTra = 0,
                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                    TongSoRo = maycan.TongSoRo,
                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                    TongSoRoCan = maycan.TongSoRo,
                                                    MaNhanVien = maycan.MaNhanVien
                                                };
                                                error.ErrorString = dataRes.MessStr;
                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                maycan.ColorString = "greenyellow";
                                                isOK = true;
                                            }
                                            else
                                            {
                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                maycan.ColorString = "red";
                                            }

                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                }
                                break;
                            case AppKV.XepKhuonKXL:
                                break;
                            case AppKV.XepKhuonBlock:
                                {
                                    if (maycan.IsStated == false)
                                    {
                                        if (dataLite.IsWaiting == false)
                                        {
                                            maycan.The = dataLite.TheId;
                                            maycan.TheAddDateTime = DateTime.Now;
                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 0,
                                                MessStr = "TRỌNG LƯỢNG KHÔNG CÂN BẰNG"
                                            };
                                            error.ErrorString = dataRes.MessStr;
                                            dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                        else
                                        {
                                            error.ErrorString = "KHÔNG THỂ CHỜ CÂN BẰNG";
                                            maycan.ColorString = "red";
                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                    else
                                    {
                                        if (maycan.TrongLuong < 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG NHỎ HƠN 0";
                                            maycan.ColorString = "red";
                                        }
                                        else if (maycan.TrongLuong == 0)
                                        {
                                            error.ErrorString = "TRỌNG LƯỢNG BẰNG 0";
                                            maycan.ColorString = "red";
                                        }
                                        else
                                        {
                                            // if (maycan?.MaLo == null || maycan.MaLo.Trim() == "")
                                            // {
                                            //     error.ErrorString = "Chưa chọn Lô";
                                            //     break;
                                            // }

                                            if (maycan?.MaSize == null || maycan.MaSize.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn size";
                                                break;
                                            }

                                            if (maycan?.MaThanhPham == null || maycan.MaThanhPham.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Thành Phẩm";
                                                break;
                                            }

                                            var thanhPham =
                                                mainService.VmThanhPhamPhuXepKhuon.Items.FirstOrDefault(x =>
                                                    x.Ma == maycan.MaThanhPham);
                                            if (thanhPham != null)
                                            {
                                                var thanhPhamMax = (decimal)thanhPham.Max;
                                                if (dataLite.TrongLuong > thanhPhamMax)
                                                {
                                                    error.ErrorString =
                                                        $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
                                                    break;
                                                }
                                            }

                                            if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Chiếu Xạ";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            if (maycan.MaChatLuong == null || maycan.MaChatLuong.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Chất Lượng";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            if (maycan.MaKhachHang == null || maycan.MaKhachHang.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Khách Hàng";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            if (maycan.MaNet == null || maycan.MaNet.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn NET";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            var net = mainService.VmNETXepKhuon.Items.FirstOrDefault(x =>
                                                x.Ma == maycan.MaNet);
                                            if (net != null)
                                            {
                                                if (Math.Abs((double)dataLite.TrongLuong - net.TrongLuong) >= net.BienDo)
                                                {
                                                    error.ErrorString =
                                                        $"TRỌNG LƯỢNG KHÔNG ĐÚNG THEO NET {net.TrongLuong} ± {net.BienDo}";
                                                    maycan.ColorString = "red";
                                                    break;
                                                }
                                            }

                                            if (maycan.MaMau == null || maycan.MaMau.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Màu";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            if (maycan.MaCongViec == null || maycan.MaCongViec.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Công Việc";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            var ngayGio = DateTime.Now;
                                            var stt = maycan.Items
                                                .Where(x => ((PhieuCanXepKhuonBlock)x).Ngay.Date == ngayGio.Date)
                                                .Select(x => ((PhieuCanXepKhuonBlock)x).STT)
                                                .DefaultIfEmpty(0).Max();
                                            var phieuCan = new PhieuCanXepKhuonBlock()
                                            {
                                                MaMau =
                                                    "",

                                                MaSize = maycan.MaSize,
                                                MaUserCan = maycan.Id,
                                                Ngay = ngayGio.Date,
                                                TrongLuong = dataLite.TrongLuong,
                                                Id = dataLite.Id,
                                                Gio = ngayGio.TimeOfDay,
                                                MaLo = "", //maycan.MaLo,
                                                MaXuong = maycan.MaXuong,
                                                MaThanhPham = maycan.MaThanhPham,
                                                GhiChu = "",
                                                MaMayCan = maycan.Id,
                                                MaNhanVien = maycan.MaNhanVien ?? "",
                                                STT = stt + 1,
                                                MaChatLuong = maycan.MaChatLuong,
                                                MaChieuXa = maycan.MaChieuXa,
                                                MaKhachHang = maycan.MaKhachHang,
                                                MaNet = maycan.MaNet,
                                                MaCongDoan = maycan.MaCongViec,
                                            };

                                            if (mainService.VmPhieuCanXepKhuonBlock.Insert(phieuCan) > 0)
                                            {
                                                maycan.TongSoRo++;
                                                maycan.TongTrongLuong += dataLite.TrongLuong;
                                                lock (_lockItems)
                                                {
                                                    maycan.Items.Insert(0, phieuCan);
                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                        maycan.Items.Remove(maycan.Items.Last());
                                                }

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    TrongLuongYeuCau = 0,
                                                    Id = dataLite.Id,
                                                    TrongLuongTare = dataLite.TrongLuongTare,
                                                    MaLo = maycan.MaLo,
                                                    MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                                                    MaNhanVienBanKiem = "",
                                                    MaSize = maycan.MaSize,
                                                    MaThanhPham = maycan.MaThanhPham,
                                                    MayCan = maycan.Id,
                                                    Ngay = ngayGio.ToString("yyyyMMdd"),
                                                    NgayGio = ngayGio.ToString("yyyyMMddHHmmss"),
                                                    STT = 0,
                                                    Status = 0,
                                                    TrongLuongNhan = dataLite.TrongLuong,
                                                    TheIdChucNang = "",
                                                    TrongLuongTra = 0,
                                                    TongTrongLuongCan = maycan.TongTrongLuong,
                                                    TongSoRo = maycan.TongSoRo,
                                                    TongTrongLuong = maycan.TongTrongLuong,
                                                    TongSoRoCan = maycan.TongSoRo,
                                                    MaNhanVien = maycan.MaNhanVien
                                                };
                                                error.ErrorString = dataRes.MessStr;
                                                dataErrorJson = JsonConvert.SerializeObject(dataRes);
                                                maycan.ColorString = "greenyellow";
                                                isOK = true;
                                            }
                                            else
                                            {
                                                error.ErrorString = @"KHÔNG THỂ THỂ THÊM PHIẾU CÂN";
                                                maycan.ColorString = "red";
                                            }

                                            maycan.TheRo = null;
                                            maycan.TheNhanVien = null;
                                            maycan.MaNhanVien = null;
                                            maycan.The = null;
                                            maycan.TheRoAddDateTime = null;
                                            maycan.TheNhanVienAddDateTime = null;
                                        }
                                    }
                                }
                                break;
                        }

                        if (maycan != null) maycan.ThongBao = $"{error.ErrorString} - {DateTime.Now:s}";
                    }
                }
                else
                {
                    error.ErrorString = "Máy Cân Không Tồn Tại";
                }

                mayCansService.NotifyItemChanged(maycan);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                error.ErrorString = e.ToString(); //"Có lỗi xảy ra";

                if (maycan != null)
                {
                    maycan.ThongBao = e.Message.Trim();
                    maycan.TheRo = null;
                    maycan.TheNhanVien = null;
                    maycan.MaNhanVien = null;
                    maycan.The = null;
                    maycan.TheRoAddDateTime = null;
                    maycan.TheNhanVienAddDateTime = null;
                    mayCansService.NotifyItemChanged(maycan);
                }
            }
        }
        else
        {
            error.ErrorString = "Data Không Đúng";
        }


        if (isOK == false) dataErrorJson = JsonConvert.SerializeObject(error);

        return new Tuple<bool, string>(isOK, dataErrorJson);
    }
    private void SetError(
    MayCan maycan,
    DataLite dataLite,
    string cmd,
    string message)
    {
        var error = new DataError
        {
            Cmd = cmd,
            ErrorString = "Không Xác Định"
        };
        error.ErrorString = message;
        maycan.ColorString = "red";

        InsertLogLoiCan(maycan, dataLite, message);
    }
    private void InsertLogLoiCan(MayCan maycan, DataLite dataLite, string thongBaoLoi)
    {
        try
        {
            var now = DateTime.Now;

            // Lấy STT lớn nhất trong ngày của máy đó
            var lastStt = mainService.VmLogGhiNhanLoiCan
                .GetMaxSTTByNgayVaMay(now.Date, maycan.Id);

            var log = new LogGhiNhanLoiCan
            {
                STT = lastStt + 1,
                Ngay = now.Date,
                Gio = now.TimeOfDay,
                GioBTP = maycan.ThoiGianBTP,

                MaUserCan = maycan.Id,
                MaMayCan = maycan.Id,

                MaLoaiCa = maycan.MaLoaiCa,
                MaMau = maycan.MaMau,
                MaSize = maycan.MaSize,
                MaThanhPham = maycan.MaThanhPham,
                MaLo = maycan.MaLo,

                MaThe = maycan.TheRo ?? dataLite.TheId,

                TrongLuongNhan = maycan.TrongLuongNhan,
                TrongLuongTra = dataLite.TrongLuong,
                TrongLuongTare = dataLite.TrongLuongTare,
                TrongLuongBu = 0,

                DinhMucThucTe = null,
                DinhMucYeuCau = null,

                MaXuong = maycan.MaXuong,
                MaNhanVien = maycan.MaNhanVien,
                MaNhanVienPhucVu = maycan.MaNhanVienPhucVu,
                MaNhanVienBanKiem = null,

                CaTra = maycan.CaTra,
                STTBTP = maycan.STTBTP,
                MaMayCanBTP = maycan.MayCanIdBTP,

                GhiChu = "",
                ChiSanLuong = maycan.IsChiSangLuong,
                SuDung = true,
                IsOffline = false,

                Id = dataLite.Id,
                IdIn = maycan.IdIn,

                ThongBaoLoi = thongBaoLoi
            };

            mainService.VmLogGhiNhanLoiCan.Insert(log);
        }
        catch (Exception ex)
        {
            Console.WriteLine("InsertLogLoiCan Error: " + ex.Message);
        }
    }

    #endregion

    private Tuple<bool, string, string> CheckMauThe(string colorCode, MayCan mayCan)
    {
        var error = "";
        var dataJson = "";
        var isOK = false;

        switch (mayCan.WKv)
        {
            case AppKV.Main:
                break;
            case AppKV.DauAo:
                break;
            case AppKV.NguyenLieu:
                break;
            case AppKV.BTPFillet:
                {
                    if (mainService.VmThanhPhamFilletColor.Items.Any())
                    {
                        var thanhPhamMau = mainService.VmThanhPhamFilletColor.Items.FirstOrDefault(x =>
                            x.ColorCode == colorCode && x.MaXuong == mayCan.MaXuong);
                        if (thanhPhamMau == null)
                        {
                            error = "KHÔNG TÌM THẤY MÀU THÀNH PHẨM TƯƠNG ỨNG";
                        }
                        else
                        {
                            var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                x.Ma == thanhPhamMau.MaThanhPham);
                            if (item != null)
                            {
                                mayCan.MaThanhPham = item.Ma;
                                mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten);
                                isOK = true;
                            }
                            else
                            {
                                error = "KHÔNG TÌM THẤY THÀNH PHẨM TƯƠNG ỨNG";
                            }
                        }
                    }
                    else
                    {
                        error = "KHÔNG CÓ MÀU";
                    }

                    break;
                }
            case AppKV.TPFillet:
                {
                    if (mainService.VmThanhPhamFilletColor.Items.Any())
                    {
                        var thanhPhamMau = mainService.VmThanhPhamFilletColor.Items.FirstOrDefault(x =>
                            x.ColorCode == colorCode && x.MaXuong == mayCan.MaXuong);
                        if (thanhPhamMau == null)
                        {
                            error = "KHÔNG TÌM THẤY MÀU THÀNH PHẨM TƯƠNG ỨNG";
                        }
                        else
                        {
                            var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                x.Ma == thanhPhamMau.MaThanhPham);
                            if (item != null)
                            {
                                mayCan.MaThanhPham = item.Ma;
                                mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten);
                                isOK = true;
                            }
                            else
                            {
                                error = "KHÔNG TÌM THẤY THÀNH PHẨM TƯƠNG ỨNG";
                            }
                        }
                    }
                    else
                    {
                        error = "KHÔNG CÓ MÀU";
                    }

                    break;
                }
            case AppKV.BTPFilletv2:
                {
                    if (mainService.VmThanhPhamFilletColor.Items.Any())
                    {
                        var thanhPhamMau = mainService.VmThanhPhamFilletColor.Items.FirstOrDefault(x =>
                            x.ColorCode == colorCode && x.MaXuong == mayCan.MaXuong);
                        if (thanhPhamMau == null)
                        {
                            error = "KHÔNG TÌM THẤY MÀU THÀNH PHẨM TƯƠNG ỨNG";
                        }
                        else
                        {
                            var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                x.Ma == thanhPhamMau.MaThanhPham);
                            if (item != null)
                            {
                                mayCan.MaThanhPham = item.Ma;
                                mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten);
                                isOK = true;
                            }
                            else
                            {
                                error = "KHÔNG TÌM THẤY THÀNH PHẨM TƯƠNG ỨNG";
                            }
                        }
                    }
                    else
                    {
                        error = "KHÔNG CÓ MÀU";
                    }

                    break;
                }
            case AppKV.TPFilletv2:
                {
                    if (mainService.VmThanhPhamFilletColor.Items.Any())
                    {
                        var thanhPhamMau = mainService.VmThanhPhamFilletColor.Items.FirstOrDefault(x =>
                            x.ColorCode == colorCode && x.MaXuong == mayCan.MaXuong);
                        if (thanhPhamMau == null)
                        {
                            error = "KHÔNG TÌM THẤY MÀU THÀNH PHẨM TƯƠNG ỨNG";
                        }
                        else
                        {
                            var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x =>
                                x.Ma == thanhPhamMau.MaThanhPham);
                            if (item != null)
                            {
                                mayCan.MaThanhPham = item.Ma;
                                mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten);
                                isOK = true;
                            }
                            else
                            {
                                error = "KHÔNG TÌM THẤY THÀNH PHẨM TƯƠNG ỨNG";
                            }
                        }
                    }
                    else
                    {
                        error = "KHÔNG CÓ MÀU";
                    }

                    break;
                }
            case AppKV.PhuPham:
                break;
            case AppKV.BTPDinhHinh:
                {
                    if (mainService.VmThanhPhamDinhHinhColor.Items.Any())
                    {
                        var thanhPhamMau =
                            mainService.VmThanhPhamDinhHinhColor.Items.FirstOrDefault(x =>
                                x.ColorCode == colorCode && x.MaXuong == mayCan.MaXuong);
                        if (thanhPhamMau == null)
                        {
                            error = "KHÔNG TÌM THẤY MÀU THÀNH PHẨM TƯƠNG ỨNG";
                        }
                        else
                        {
                            var item = mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                x.Ma == thanhPhamMau.MaThanhPham);
                            if (item != null)
                            {
                                mayCan.MaThanhPham = item.Ma;
                                mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten);
                                isOK = true;
                            }
                            else
                            {
                                error = "KHÔNG TÌM THẤY THÀNH PHẨM TƯƠNG ỨNG";
                            }
                        }
                    }
                    else
                    {
                        error = "KHÔNG CÓ MÀU";
                    }

                    break;
                }
            case AppKV.TPDinhHinh:
                {
                    if (mainService.VmThanhPhamDinhHinhColor.Items.Any())
                    {
                        var thanhPhamMau =
                            mainService.VmThanhPhamDinhHinhColor.Items.FirstOrDefault(x =>
                                x.ColorCode == colorCode && x.MaXuong == mayCan.MaXuong);
                        if (thanhPhamMau == null)
                        {
                            error = "KHÔNG TÌM THẤY MÀU THÀNH PHẨM TƯƠNG ỨNG";
                        }
                        else
                        {
                            var item = mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x =>
                                x.Ma == thanhPhamMau.MaThanhPham);
                            if (item != null)
                            {
                                mayCan.MaThanhPham = item.Ma;
                                mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten);
                                isOK = true;
                            }
                            else
                            {
                                error = "KHÔNG TÌM THẤY THÀNH PHẨM TƯƠNG ỨNG";
                            }
                        }
                    }
                    else
                    {
                        error = "KHÔNG CÓ MÀU";
                    }

                    break;
                }
            case AppKV.XepKhuon:
                break;
            case AppKV.BaoTu:
                break;
            case AppKV.CaoThit:
                break;
            case AppKV.XepKhuonRaCoi:
                break;
            default:
                error = "KHU VỰC KHÔNG XÁC ĐỊNH";
                break;
        }

        return new Tuple<bool, string, string>(isOK, error, dataJson);
    }

    private async Task<Tuple<bool, string, string>> CheckTheThanhPham(TheThanhPham the, MayCan mayCan)
    {
        var error = "";
        var dataJson = "";
        var isOK = false;

        switch (mayCan.WKv)
        {
            case AppKV.Main:
                break;
            case AppKV.DauAo:
                break;
            case AppKV.NguyenLieu:
                break;
            case AppKV.BTPFillet:
                {
                    if (the.MaThanhPhamFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamFillet;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamFillet = null;
                        }
                    }

                    if (the.MaSizeFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeFillet;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeFillet = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.TPFillet:
                {
                    if (the.MaThanhPhamFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamFillet;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamFillet = null;
                        }
                    }

                    if (the.MaSizeFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeFillet;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeFillet = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.BTPFilletv2:
                {
                    if (the.MaThanhPhamFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamFillet;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamFillet = null;
                        }
                    }

                    if (the.MaSizeFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeFillet;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeFillet = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.TPFilletv2:
                {
                    if (the.MaThanhPhamFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamFillet;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamFillet = null;
                        }
                    }

                    if (the.MaSizeFillet != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeFillet;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeFillet = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.PhuPham:
                break;
            case AppKV.BTPDinhHinh:
                {
                    if (the.MaThanhPham != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaThanhPham);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPham;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPham = null;
                        }
                    }

                    if (the.MaSizeDinhHinh != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizeDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaSizeDinhHinh);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeDinhHinh;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeDinhHinh = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true) await mayCansService.CommandZero(mayCan.Id);
                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.TPDinhHinh:
                {
                    if (the.MaThanhPham != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaThanhPham);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPham;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPham = null;
                        }
                    }

                    if (the.MaSizeDinhHinh != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizeDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaSizeDinhHinh);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeDinhHinh;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeDinhHinh = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.XepKhuon:
                {
                    if (the.MaThanhPhamChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                            x.Ma == the.MaThanhPhamChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamChinhXepKhuon;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamChinhXepKhuon = null;
                        }
                    }

                    if (the.MaSizeChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item =
                            mainService.VmSizeChinhXepKhuon.Items.FirstOrDefault(x => x.Ma == the.MaSizeChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeChinhXepKhuon;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeChinhXepKhuon = null;
                        }
                    }

                    if (the.MaChatLuongChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmChatLuong.Items.FirstOrDefault(x => x.Ma == the.MaChatLuongChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaChatLuong = the.MaChatLuongChinhXepKhuon;
                            await mayCansService.CommandSetChatLuong(mayCan.Id, mayCan.MaChatLuong, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaChatLuongChinhXepKhuon = null;
                        }
                    }

                    if (the.MaCoiXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmCoi.Items.FirstOrDefault(x => x.Ma == the.MaCoiXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaCoi = the.MaCoiXepKhuon;
                            await mayCansService.CommandSetCoi(mayCan.Id, mayCan.MaCoi, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaCoiXepKhuon = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.BaoTu:
                break;
            case AppKV.CaoThit:
                break;
            case AppKV.XepKhuonRaCoi:
                {
                    if (the.MaThanhPhamChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                            x.Ma == the.MaThanhPhamChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamChinhXepKhuon;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamChinhXepKhuon = null;
                        }
                    }

                    if (the.MaSizeChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item =
                            mainService.VmSizeChinhXepKhuon.Items.FirstOrDefault(x => x.Ma == the.MaSizeChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeChinhXepKhuon;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }

                        else
                        {
                            the.MaSizeChinhXepKhuon = null;
                        }
                    }

                    if (the.MaChatLuongChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmChatLuong.Items.FirstOrDefault(x => x.Ma == the.MaChatLuongChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaChatLuong = the.MaChatLuongChinhXepKhuon;
                            await mayCansService.CommandSetChatLuong(mayCan.Id, mayCan.MaChatLuong, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaChatLuongChinhXepKhuon = null;
                        }
                    }

                    if (the.MaCoiXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmCoi.Items.FirstOrDefault(x => x.Ma == the.MaCoiXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaCoi = the.MaCoiXepKhuon;
                            await mayCansService.CommandSetCoi(mayCan.Id, mayCan.MaCoi, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaCoiXepKhuon = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.XepKhuonPhu:
                {
                    if (the.MaThanhPhamPhuXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamPhuXepKhuon.Items.FirstOrDefault(x =>
                            x.Ma == the.MaThanhPhamPhuXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamPhuXepKhuon;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamPhuXepKhuon = null;
                        }
                    }

                    if (the.MaSizePhuXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmSizePhuXepKhuon.Items.FirstOrDefault(x => x.Ma == the.MaSizePhuXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizePhuXepKhuon;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaSizePhuXepKhuon = null;
                        }
                    }

                    if (the.MaChatLuongPhuXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmChatLuong.Items.FirstOrDefault(x => x.Ma == the.MaChatLuongPhuXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaChatLuong = the.MaChatLuongPhuXepKhuon;
                            await mayCansService.CommandSetChatLuong(mayCan.Id, mayCan.MaChatLuong, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaChatLuongPhuXepKhuon = null;
                        }
                    }

                    if (the.MaCoiXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmCoi.Items.FirstOrDefault(x => x.Ma == the.MaCoiXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaCoi = the.MaCoiXepKhuon;
                            await mayCansService.CommandSetCoi(mayCan.Id, mayCan.MaCoi, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaCoiXepKhuon = null;
                        }
                    }

                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            case AppKV.XepKhuonBlock:
                {
                    if (the.MaThanhPhamChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                            x.Ma == the.MaThanhPhamChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaThanhPham = the.MaThanhPhamChinhXepKhuon;
                            await mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaThanhPhamChinhXepKhuon = null;
                        }
                    }

                    if (the.MaSizeChinhXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item =
                            mainService.VmSizeChinhXepKhuon.Items.FirstOrDefault(x => x.Ma == the.MaSizeChinhXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaSize = the.MaSizeChinhXepKhuon;
                            await mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaSizeChinhXepKhuon = null;
                        }
                    }

                    if (the.MaChatLuongPhuXepKhuon != null)
                    {
                        await Task.Delay(150);
                        var item = mainService.VmChatLuong.Items.FirstOrDefault(x => x.Ma == the.MaChatLuongPhuXepKhuon);
                        if (item != null)
                        {
                            mayCan.MaChatLuong = the.MaChatLuongPhuXepKhuon;
                            await mayCansService.CommandSetChatLuong(mayCan.Id, mayCan.MaChatLuong, item.Ten ?? "");
                        }
                        else
                        {
                            the.MaChatLuongPhuXepKhuon = null;
                        }
                    }


                    if (the.TrongLuongTare > 0)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                    }

                    if (the.IsZero == true)
                    {
                        await Task.Delay(150);
                        await mayCansService.CommandZero(mayCan.Id);
                    }

                    if (the.LoLevel > 0)
                    {
                        var lo = mainService.VmLoHq.GetByLevel(DateTime.Now, the.LoLevel);
                        if (lo != "")
                        {
                            await Task.Delay(150);
                            await mayCansService.CommandSetLo(mayCan.Id, lo);
                        }
                    }

                    isOK = true;
                    break;
                }
            default:
                error = "KHU VỰC KHÔNG XÁC ĐỊNH";
                break;
        }

        return new Tuple<bool, string, string>(isOK, error, dataJson);
    }

    private bool CommitPhieuCan()
    {
        return false;
    }
}