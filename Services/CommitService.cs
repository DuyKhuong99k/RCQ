using System.Globalization;
using Models.Repos.Models;
using Newtonsoft.Json;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

public class CommitService(IMayCansService mayCansService, ICoiService coiService, IMainService mainService)
    : ICommitService
{
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
        try
        {
            if (dataLite != null)
            {
                var maycan = mayCansService.Find(client.Id);
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
                        //maycan.TheAddDateTime = DateTime.Now;
                        if (dataLite.Stated == "ST" && dataLite.TrongLuong > 0)
                            maycan.IsStated = true;
                        else
                            maycan.IsStated = false;

                        maycan.TrongLuongTare = dataLite.TrongLuongTare;
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
                                    var timems = Math.Abs((maycan.TheRoAddDateTime - DateTime.Now)?.TotalMilliseconds ??
                                                          0);
                                    if (timems > 0 && timems < 1000)
                                    {
                                        error.ErrorString = "OK";
                                        dataErrorJson = JsonConvert.SerializeObject(error);

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
                                }
                                else
                                {
                                    var theRo = mainService.VmTheRo.Items.FirstOrDefault(x =>
                                        x.MaThe == dataLite.TheId);
                                    if (theRo != null)
                                    {
                                        maycan.TheRo = theRo.MaThe;
                                        maycan.TheRoAddDateTime = DateTime.Now;
                                        maycan.TheNhanVien = null;
                                        maycan.MaHoSo = null;
                                        maycan.TheNhanVienAddDateTime = null;
                                        maycan.NhanVienName = null;

                                        if (maycan.IsSuDungMauThanhPham)
                                        {
                                            var rlMauThe = CheckMauThe(theRo.ColorCode, maycan);
                                            isOK = rlMauThe.Item1;
                                            error.ErrorString = rlMauThe.Item2;
                                        }
                                    }
                                    else
                                    {
                                        var theNhanvien =
                                            mainService.VmThe.Items.FirstOrDefault(x => x.MaTheTu == dataLite.TheId);
                                        if (theNhanvien == null)
                                        {
                                            error.ErrorString = "Không Tìm Thấy Thẻ";
                                            maycan.ColorString = "red";
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
                                                await Task.Delay(100);
                                                await mayCansService.CommandSetNhanVien(maycan.Id, nhanVien.MaNhanVien,
                                                    nhanVien.MaHoSo ?? "",
                                                    nhanVien.Name ?? "");
                                                await Task.Delay(200);
                                                if (nhanVien.IsPhucVu)
                                                {
                                                    maycan.MaNhanVienPhucVu = nhanVien.MaNhanVien;
                                                    maycan.TheNhanVienPhucVuAddDateTime = DateTime.Now;
                                                }
                                                else
                                                {
                                                    maycan.MaNhanVien = nhanVien.MaNhanVien;
                                                    maycan.TheNhanVienAddDateTime = DateTime.Now;
                                                    maycan.TheNhanVien = dataLite.TheId;
                                                    maycan.MaHoSo = nhanVien.MaHoSo;
                                                    maycan.NhanVienName = nhanVien.Name;
                                                }

                                                // Set nhanvien
                                            }
                                        }

                                        var timems = Math.Abs(
                                            (maycan.TheRoAddDateTime - DateTime.Now)?.TotalMilliseconds ??
                                            0);
                                        if (timems > 0 && timems > 10000) maycan.TheRo = null;
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
                            maycan.TheRo = dataLite.TheId;
                            maycan.TheRoAddDateTime = DateTime.Now;
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
                                            maycan.Items.Insert(0, phieuCan);
                                            if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                maycan.Items.Remove(maycan.Items.Last());

                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 1,
                                                MessStr = "ĐÃ HOÀN THÀNH",
                                                ChiSanLuong = 0,
                                                DinhMucYeuCau = 0,
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
                                                    SuDung = true
                                                };

                                                if (mainService.VmPhieuCanTPFillet.Insert(phieuCan) > 0)
                                                {
                                                    maycan.TongSoRo++;
                                                    maycan.TongTrongLuong += dataLite.TrongLuong;
                                                    maycan.Items.Insert(0, phieuCan);
                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                        maycan.Items.Remove(maycan.Items.Last());

                                                    var dataRes = new DataLiteRes
                                                    {
                                                        TheId = dataLite.TheId,
                                                        IsDataAction = 1,
                                                        MessStr = "ĐÃ HOÀN THÀNH",
                                                        ChiSanLuong = 0,
                                                        DinhMucYeuCau = 0,
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
                                        break;
                                    case AppType._type1:
                                        break;
                                    case AppType._type2:
                                        break;
                                    case AppType._type3:
                                    {
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
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            DinhMucYeuCau = 0,
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
                                                maycan.Items.Insert(0, phieuCan);
                                                if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                    maycan.Items.Remove(maycan.Items.Last());

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    DinhMucYeuCau = 0,
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

                                            if (phieuCanLast != null)
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
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());

                                                        maycan.TheRo = null;
                                                        maycan.The = dataLite.Id;
                                                        isOK = true;
                                                        var dataRes = new DataLiteRes
                                                        {
                                                            ChiSanLuong = 0,
                                                            DinhMucYeuCau = 0,
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
                                                    .FirstOrDefault(
                                                        x => x.CaTra == maycan.CaTra &&
                                                             x.Ngay.Date == mainService.VmApp.DateTimeNow.Date &&
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
                                                if (mainService.VmPhieuCanTpFilletv2.Insert(phieuCan) > 0)
                                                {
                                                    maycan.TongSoRo++;
                                                    maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                    maycan.Items.Insert(0, phieuCan);
                                                    if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                        maycan.Items.Remove(maycan.Items.Last());

                                                    var dataRes = new DataLiteRes
                                                    {
                                                        TheId = dataLite.TheId,
                                                        IsDataAction = 1,
                                                        MessStr = "ĐÃ HOÀN THÀNH",
                                                        ChiSanLuong = 0,
                                                        DinhMucYeuCau = 0,
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

                                            if (phieuCanBTP == null)
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
                                                                    DinhMucYeuCau = 0,
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
                                                                    MaNhanVien = maycan.MaNhanVien,
                                                                    
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
                                                                DinhMucYeuCau = 0,
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
                                            if (phieuCanLast == null)
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
                                                    .FirstOrDefault(
                                                        x => x.CaTra == maycan.CaTra &&
                                                             x.Ngay.Date == mainService.VmApp.DateTimeNow.Date &&
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

                                                if (mainService.VmPhieuCanTpFilletv2.Insert(phieuCan) > 0)
                                                {
                                                    //theem ham cap nhat by id
                                                    if (mainService.VmPhieuCanBtpFilletv2.Update(maycan.IdIn ?? "-",
                                                            true) >
                                                        0)
                                                    {
                                                        maycan.TongSoRo++;
                                                        maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            DinhMucYeuCau = phieuCan.DinhMucYeuCau,
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
                                                                    mainService.VmSizeDinhHinh.Items.FirstOrDefault(
                                                                        x =>
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
                                            maycan.Items.Insert(0, phieuCan);
                                            if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                maycan.Items.Remove(maycan.Items.Last());

                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 1,
                                                MessStr = "ĐÃ HOÀN THÀNH",
                                                ChiSanLuong = 0,
                                                DinhMucYeuCau = 0,
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
                                                        TrongLuongBu = 0
                                                    };
                                                    if (mainService.VmPhieuCanBTPDinhHinh.Insert(phieuCan) > 0)
                                                    {
                                                        maycan.TongSoRo++;
                                                        maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            DinhMucYeuCau = 0,
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
                                        break;
                                    case AppType._type2:
                                        break;
                                    case AppType._type3:
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
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            DinhMucYeuCau = 0,
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
                                            if (phieuCanLast == null)
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
                                                    .Where(x => ((PhieuCanTPDinhHinh)x).Ngay.Date == ngayGio.Date)
                                                    .Select(x => ((PhieuCanTPDinhHinh)x).STT)
                                                    .DefaultIfEmpty(0).Max();
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
                                                    ChiSanLuong = false,
                                                    GhiChu = "",
                                                    Gio = ngayGio.TimeOfDay,
                                                    IsOffline = false,
                                                    MaLoaiCa =
                                                        maycan.MaLoaiCa,
                                                    MaMau = maycan.MaMau,
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
                                                var dinhMuc = mainService.VmDinhMucDinhHinh.Items
                                                    .FirstOrDefault(
                                                        x => x.CaTra == maycan.CaTra &&
                                                             x.Ngay.Date == mainService.VmApp.DateTimeNow.Date &&
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

                                                if (mainService.VmPhieuCanTPDinhHinh.Insert(phieuCan) > 0)
                                                {
                                                    //theem ham cap nhat by id
                                                    if (mainService.VmPhieuCanBTPDinhHinh.Update(maycan.IdIn ?? "-",
                                                            true) >
                                                        0)
                                                    {
                                                        maycan.TongSoRo++;
                                                        maycan.TongTrongLuong += phieuCan.TrongLuongTra;
                                                        maycan.Items.Insert(0, phieuCan);
                                                        if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                            maycan.Items.Remove(maycan.Items.Last());

                                                        var dataRes = new DataLiteRes
                                                        {
                                                            TheId = dataLite.TheId,
                                                            IsDataAction = 1,
                                                            MessStr = "ĐÃ HOÀN THÀNH",
                                                            ChiSanLuong = 0,
                                                            DinhMucYeuCau = phieuCan.DinhMucYeuCau,
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
                                                                    mainService.VmSizeDinhHinh.Items.FirstOrDefault(
                                                                        x =>
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
                                            if (maycan?.MaCoiTam == null || maycan.MaCoiTam.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Còi Tạm";
                                                break;
                                            }

                                            if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Chiếu Xạ";
                                                break;
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
                                                maycan.Items.Insert(0, phieuCan);
                                                if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                    maycan.Items.Remove(maycan.Items.Last());

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    DinhMucYeuCau = 0,
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
                                                mainService.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x =>
                                                    x.Ma == maycan.MaThanhPham);
                                            if (_thanhPham != null)
                                            {
                                                var thanhPhamMax = (decimal)_thanhPham.Max;
                                                if (dataLite.TrongLuong > thanhPhamMax)
                                                {
                                                    error.ErrorString = $"TRỌNG LƯỢNG VƯỢT GIỚI HẠN: {thanhPhamMax} kg";
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

                                            if (maycan.MaChieuXa == null || maycan.MaChieuXa.Trim() == "")
                                            {
                                                error.ErrorString = "Chưa chọn Chiều Xá";
                                                maycan.ColorString = "red";
                                                break;
                                            }

                                            var ngayGio = DateTime.Now;
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
                                                IdMonitor = maycan.IdMonitor,
                                                MaCoi = maycan.MaCoi,
                                                NgayNguyenLieu = maycan.NgayNguyenLieu
                                            };
                                            if (mainService.VmPhieuCanRaCoi.Insert(phieuCan) > 0)
                                            {
                                                //theem ham cap nhat by id
                                                maycan.TongSoRo++;
                                                maycan.TongTrongLuong += phieuCan.TrongLuong;
                                                maycan.Items.Insert(0, phieuCan);
                                                if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                    maycan.Items.Remove(maycan.Items.Last());

                                                var dataRes = new DataLiteRes
                                                {
                                                    TheId = dataLite.TheId,
                                                    IsDataAction = 1,
                                                    MessStr = "ĐÃ HOÀN THÀNH",
                                                    ChiSanLuong = 0,
                                                    DinhMucYeuCau = 0,
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
                                                        await mayCansService.CommandSetLo(maycan.Id, maycan.MaLo);
                                                        await Task.Delay(50);
                                                        var _size =
                                                            mainService.VmSizeDinhHinh.Items.FirstOrDefault(
                                                                x =>
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
                                                            mainService.VmChieuXaXepKhuon.Items.FirstOrDefault(
                                                                x =>
                                                                    x.Ma == maycan.MaChieuXa);
                                                        if (_chieuXa != null)
                                                            await mayCansService.CommandSetChieuXa(maycan.Id,
                                                                maycan.MaChieuXa,
                                                                _chieuXa.Ten ?? "");
                                                        await Task.Delay(50);
                                                        var _chatLuong =
                                                            mainService.VmChatLuong.Items.FirstOrDefault(
                                                                x =>
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
                                                         (x.OutLocked == false && x.InLocked)) && x.TimeROut != null)
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
                                                        x.MaLo, x.MaThanhPhamChinh, x.MaSizeChinh, x.MaChatLuong,
                                                        x.MaChieuXa
                                                    }).Select(g => new
                                                    {
                                                        g.Key.MaLo, g.Key.MaThanhPhamChinh, g.Key.MaSizeChinh,
                                                        g.Key.MaChatLuong, g.Key.MaChieuXa,
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
                                            maycan.Items.Insert(0, phieuCan);
                                            if (maycan.Items.Count > mainService.VmApp.MaxRowsView)
                                                maycan.Items.Remove(maycan.Items.Last());

                                            var dataRes = new DataLiteRes
                                            {
                                                TheId = dataLite.TheId,
                                                IsDataAction = 1,
                                                MessStr = "ĐÃ HOÀN THÀNH",
                                                ChiSanLuong = 0,
                                                DinhMucYeuCau = 0,
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
                        }

                        if (maycan != null) maycan.ThongBao = $"{error.ErrorString} - {DateTime.Now:s}";
                    }
                }
                else
                {
                    error.ErrorString = "Máy Cân Không Tồn Tại";
                }
            }
            else
            {
                error.ErrorString = "Data Không Đúng";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            error.ErrorString = "Có lỗi xảy ra";
        }


        if (isOK == false) dataErrorJson = JsonConvert.SerializeObject(error);
        mayCansService.NotifyItemChanged();
        return new Tuple<bool, string>(isOK, dataErrorJson);
    }

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
                    await Task.Delay(300);
                    var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                    if (item != null)
                    {
                        mayCan.MaThanhPham = the.MaThanhPhamFillet;
                        mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                    }
                    else
                    {
                        the.MaThanhPhamFillet = null;
                    }
                }

                if (the.MaSizeFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                    if (item != null)
                    {
                        mayCan.MaSize = the.MaSizeFillet;
                        mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                    }

                    else
                    {
                        the.MaSizeFillet = null;
                    }
                }

                if (the.TrongLuongTare > 0)
                {
                    await Task.Delay(300);
                    mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                }

                if (the.IsZero == true)
                {
                    await Task.Delay(300);
                    mayCansService.CommandZero(mayCan.Id);
                }

                isOK = true;
                break;
            }
            case AppKV.TPFillet:
            {
                if (the.MaThanhPhamFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                    if (item != null)
                    {
                        mayCan.MaThanhPham = the.MaThanhPhamFillet;
                        mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                    }
                    else
                    {
                        the.MaThanhPhamFillet = null;
                    }
                }

                if (the.MaSizeFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                    if (item != null)
                    {
                        mayCan.MaSize = the.MaSizeFillet;
                        mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                    }

                    else
                    {
                        the.MaSizeFillet = null;
                    }
                }

                if (the.TrongLuongTare > 0)
                {
                    await Task.Delay(300);
                    mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                }

                if (the.IsZero == true)
                {
                    await Task.Delay(300);
                    mayCansService.CommandZero(mayCan.Id);
                }

                isOK = true;
                break;
            }
            case AppKV.BTPFilletv2:
            {
                if (the.MaThanhPhamFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                    if (item != null)
                    {
                        mayCan.MaThanhPham = the.MaThanhPhamFillet;
                        mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                    }
                    else
                    {
                        the.MaThanhPhamFillet = null;
                    }
                }

                if (the.MaSizeFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                    if (item != null)
                    {
                        mayCan.MaSize = the.MaSizeFillet;
                        mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                    }

                    else
                    {
                        the.MaSizeFillet = null;
                    }
                }

                if (the.TrongLuongTare > 0)
                {
                    await Task.Delay(300);
                    mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                }

                if (the.IsZero == true)
                {
                    await Task.Delay(300);
                    mayCansService.CommandZero(mayCan.Id);
                }

                isOK = true;
                break;
            }
            case AppKV.TPFilletv2:
            {
                if (the.MaThanhPhamFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmThanhPhamFillet.Items.FirstOrDefault(x => x.Ma == the.MaThanhPhamFillet);
                    if (item != null)
                    {
                        mayCan.MaThanhPham = the.MaThanhPhamFillet;
                        mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                    }
                    else
                    {
                        the.MaThanhPhamFillet = null;
                    }
                }

                if (the.MaSizeFillet != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmSizeFillet.Items.FirstOrDefault(x => x.Ma == the.MaSizeFillet);
                    if (item != null)
                    {
                        mayCan.MaSize = the.MaSizeFillet;
                        mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                    }

                    else
                    {
                        the.MaSizeFillet = null;
                    }
                }

                if (the.TrongLuongTare > 0)
                {
                    await Task.Delay(300);
                    mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                }

                if (the.IsZero == true)
                {
                    await Task.Delay(300);
                    mayCansService.CommandZero(mayCan.Id);
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
                    await Task.Delay(300);
                    var item = mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaThanhPham);
                    if (item != null)
                    {
                        mayCan.MaThanhPham = the.MaThanhPham;
                        mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                    }
                    else
                    {
                        the.MaThanhPham = null;
                    }
                }

                if (the.MaSizeDinhHinh != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmSizeDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaSizeDinhHinh);
                    if (item != null)
                    {
                        mayCan.MaSize = the.MaSizeDinhHinh;
                        mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                    }

                    else
                    {
                        the.MaSizeDinhHinh = null;
                    }
                }

                if (the.TrongLuongTare > 0)
                {
                    await Task.Delay(300);
                    mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                }

                if (the.IsZero == true) mayCansService.CommandZero(mayCan.Id);
                isOK = true;
                break;
            }
            case AppKV.TPDinhHinh:
            {
                if (the.MaThanhPham != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmThanhPhamDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaThanhPham);
                    if (item != null)
                    {
                        mayCan.MaThanhPham = the.MaThanhPham;
                        mayCansService.CommandSetThanhPham(mayCan.Id, mayCan.MaThanhPham, item.Ten ?? "");
                    }
                    else
                    {
                        the.MaThanhPham = null;
                    }
                }

                if (the.MaSizeDinhHinh != null)
                {
                    await Task.Delay(300);
                    var item = mainService.VmSizeDinhHinh.Items.FirstOrDefault(x => x.Ma == the.MaSizeDinhHinh);
                    if (item != null)
                    {
                        mayCan.MaSize = the.MaSizeDinhHinh;
                        mayCansService.CommandSetSize(mayCan.Id, mayCan.MaSize, item.Ten ?? "");
                    }

                    else
                    {
                        the.MaSizeDinhHinh = null;
                    }
                }

                if (the.TrongLuongTare > 0)
                {
                    await Task.Delay(300);
                    mayCansService.CommandTare(mayCan.Id, the.TrongLuongTare);
                }

                if (the.IsZero == true)
                {
                    await Task.Delay(300);
                    mayCansService.CommandZero(mayCan.Id);
                }

                isOK = true;
                break;
            }
            case AppKV.XepKhuon:
                break;
            case AppKV.BaoTu:
                break;
            case AppKV.CaoThit:
                break;
            case AppKV.XepKhuonRaCoi:
            {
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