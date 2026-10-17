using System.Globalization;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json;
using Vars;
using Vars.Hubs;

namespace Services;

/// <summary>Restore HQ weight tickets at their original time; never overwrite an existing Id.</summary>
public static class HqWeightBackupProcessor
{
    public static HQ_PhieuCan Normalize(DataCoummunication data, string machine, DateTime now)
    {
        if (!int.TryParse(machine, NumberStyles.None, CultureInfo.InvariantCulture, out var number) || number <= 0)
            throw new ArgumentException("Mã máy HQ không hợp lệ.");
        if (!string.IsNullOrWhiteSpace(data.MayCan) && data.MayCan != machine)
            throw new ArgumentException("Phiếu backup không thuộc máy đang kết nối.");
        if (!DateTime.TryParseExact(data.NgayGio,
            new[] { "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss" },
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var time) || time > now)
            throw new ArgumentException("Ngày giờ backup không hợp lệ hoặc nằm trong tương lai.");
        var id = AttendanceRecordId.Create(machine, time);
        var suppliedIds = new[] { data.Id, data.IdIn, data.IdOut }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        if (suppliedIds.Length == 0 || suppliedIds.Any(x => x != id))
            throw new ArgumentException("Id backup không khớp máy và ngày giờ (Id/IdIn/IdOut).");
        if (!string.IsNullOrWhiteSpace(data.TheId) && !string.IsNullOrWhiteSpace(data.TheIdNhanVien)
            && data.TheId != data.TheIdNhanVien)
            throw new ArgumentException("Mã thẻ backup không thống nhất.");
        string Required(string? value, string label) => string.IsNullOrWhiteSpace(value) || value.Length > 50
            ? throw new ArgumentException($"Thiếu hoặc sai {label} trong phiếu backup {id}.") : value;
        string Optional(string? value, string label) => string.IsNullOrEmpty(value) ? ""
            : value.Length > 50 ? throw new ArgumentException($"{label} quá dài trong phiếu {id}.") : value;
        var card = Required(string.IsNullOrWhiteSpace(data.TheIdNhanVien) ? data.TheId : data.TheIdNhanVien, "mã thẻ");
        var weight = data.TrongLuongNhan ?? data.TrongLuong;
        if (data.TrongLuongNhan.HasValue && data.TrongLuong != 0 && data.TrongLuong != data.TrongLuongNhan)
            throw new ArgumentException("Hai trường trọng lượng backup không thống nhất.");
        if (weight <= 0 || weight >= 1000000000000000m || data.TrongLuongTare < 0 || data.TrongLuongTare > weight
            || decimal.Round(weight,3) != weight || decimal.Round(data.TrongLuongTare,3) != data.TrongLuongTare)
            throw new ArgumentException($"Trọng lượng/tare không hợp lệ trong phiếu {id}.");
        if (data.Status is < 0 or > 2)
            throw new ArgumentException($"Phiếu {id} bị hủy hoặc có trạng thái không hợp lệ; không khôi phục tự động.");
        if (data.ChiSanLuong.HasValue && data.ChiSanLuong != 1)
            throw new ArgumentException($"Phiếu {id} không phải phiếu sản lượng HQ; không tự đổi ChiSanLuong.");
        return new HQ_PhieuCan {
            Id=id, STT=data.STT ?? 0, Ngay=DateOnly.FromDateTime(time), NgayGio=time, MayCan=machine,
            MaLo=Required(data.MaLo,"mã lô"), MaSize=Required(data.MaSize,"mã size"),
            MaThanhPham=Required(data.MaThanhPham,"mã thành phẩm"),
            MaLoaiNguyenLieu=Optional(data.MaLoaiNguyenLieu,"mã nguyên liệu"),
            MaXuong=Optional(data.MaXuong,"mã xưởng"), MaNhanVien=Optional(data.MaNhanVien,"mã nhân viên"),
            MaNhanVienPhucVu=Optional(data.MaNhanVienPhucVu,"nhân viên phục vụ"),
            MaNhanVienBanKiem=Optional(data.MaNhanVienKiem,"nhân viên kiểm"),
            TrongLuong=weight, TrongLuongTare=data.TrongLuongTare,
            ChiSanLuong=true, Status=1, TheId=card, TheIdNhanVien=card,
            GhiChu="[HQ_BACKUP] " + now.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture)
        };
    }

    private static bool CodeEquals(string? left, string? right) => string.Equals(left,right,StringComparison.OrdinalIgnoreCase);
    public static bool MatchesExisting(HQ_PhieuCan saved, HQ_PhieuCan requested) =>
        saved.Id == requested.Id && saved.MayCan == requested.MayCan && saved.NgayGio == requested.NgayGio
        && saved.TheIdNhanVien == requested.TheIdNhanVien && saved.TrongLuong == requested.TrongLuong
        && saved.TrongLuongTare == requested.TrongLuongTare && CodeEquals(saved.MaLo,requested.MaLo)
        && CodeEquals(saved.MaSize,requested.MaSize) && CodeEquals(saved.MaThanhPham,requested.MaThanhPham)
        && (string.IsNullOrEmpty(requested.MaLoaiNguyenLieu) || CodeEquals(saved.MaLoaiNguyenLieu,requested.MaLoaiNguyenLieu))
        && (string.IsNullOrEmpty(requested.MaNhanVien) || CodeEquals(saved.MaNhanVien,requested.MaNhanVien))
        && (string.IsNullOrEmpty(requested.MaXuong) || CodeEquals(saved.MaXuong,requested.MaXuong))
        && (string.IsNullOrEmpty(requested.MaNhanVienPhucVu) || CodeEquals(saved.MaNhanVienPhucVu,requested.MaNhanVienPhucVu))
        && (string.IsNullOrEmpty(requested.MaNhanVienBanKiem) || CodeEquals(saved.MaNhanVienBanKiem,requested.MaNhanVienBanKiem))
        && saved.Status == 1 && saved.ChiSanLuong;

    public static string? ValidateHistoricalScan(CheckInOut? scan, int machine, DateTime time, int maxHours)
    {
        if (maxHours is < 1 or > 24) return "Giới hạn ca chấm công không hợp lệ.";
        if (scan is null || scan.TrangThai != "Vào") return "Không có ca Vào tại thời điểm cân. Đồng bộ chấm công và kiểm tra ca trước.";
        if (scan.MaSoMay != machine) return "Ca Vào tại thời điểm cân thuộc máy khác.";
        if (scan.ThoiGian > time || ScaleAttendanceProcessor.IsShiftExpired(scan.ThoiGian,time,maxHours,scan.CaDem))
            return "Ca tại thời điểm cân đã hết hạn.";
        if (scan.Isforget != 0) return "Ca tại thời điểm cân có lỗi. Cần kiểm tra phiếu chấm công trước.";
        return null;
    }

    public sealed record HistoricalMetadata(string? MaLoaiNguyenLieu,string? MaXuong);
    public static void ResolveHistoricalMetadata(HQ_PhieuCan item,IReadOnlyList<HistoricalMetadata> references)
    {
        if(references.Count!=1 || string.IsNullOrWhiteSpace(references[0].MaLoaiNguyenLieu)
            || string.IsNullOrWhiteSpace(references[0].MaXuong))
            throw new ArgumentException($"Phiếu {item.Id} thiếu mã nguyên liệu/xưởng và không có lịch sử khớp duy nhất. Firmware cần gửi đủ mã.");
        if(!string.IsNullOrEmpty(item.MaLoaiNguyenLieu) && !CodeEquals(item.MaLoaiNguyenLieu,references[0].MaLoaiNguyenLieu)
            || !string.IsNullOrEmpty(item.MaXuong) && !CodeEquals(item.MaXuong,references[0].MaXuong))
            throw new ArgumentException($"Phiếu {item.Id}: mã nguyên liệu/xưởng khác lịch sử. Không tự suy đoán.");
        item.MaLoaiNguyenLieu=references[0].MaLoaiNguyenLieu!; item.MaXuong=references[0].MaXuong!;
    }

    public static Tuple<bool,string> Process(IReadOnlyList<DataCoummunication> records, ClientInfo client,
        string cmd, int maxHours, bool batch, Action<IReadOnlyList<HQ_PhieuCan>>? onCommitted = null)
    {
        try {
            if (records.Count == 0 || records.Any(x => x is null)) throw new ArgumentException("Backup không có phiếu hợp lệ.");
            var now=DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime;
            var items=records.Select(x=>Normalize(x,client.Id,now)).ToArray();
            if (items.Select(x=>x.Id).Distinct(StringComparer.Ordinal).Count()!=items.Length)
                throw new ArgumentException("Một đợt backup có Id trùng. Chưa lưu đợt này.");
            using var db=new dbPMScontext();
            var machine=db.MayCans.AsNoTracking().FirstOrDefault(x=>x.Id==client.Id);
            if (machine?.WKv != AppKV.Hq) throw new ArgumentException("Máy backup không phải máy HQ đã đăng ký.");
            using var transaction=db.Database.BeginTransaction(IsolationLevel.ReadCommitted);
            foreach(var card in items.Select(x=>x.TheIdNhanVien).Distinct().OrderBy(x=>x,StringComparer.Ordinal))
                ScaleAttendanceTransferService.LockCard(db,card);
            // Id-level serialization shared by concurrent Sync/Resync requests.
            foreach(var id in items.Select(x=>x.Id).OrderBy(x=>x,StringComparer.Ordinal)) {
                var resource="HQWeightBackup:"+id;
                db.Database.ExecuteSqlInterpolated($@"DECLARE @result int;
                    EXEC @result=sys.sp_getapplock @Resource={resource},@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=5000;
                    IF @result<0 THROW 51001,N'Không thể khóa phiếu backup.',1;");
            }
            var inserted=0; var existing=0;
            var acknowledgedItems=new List<HQ_PhieuCan>();

            var itemIds = items.Select(x => x.Id).ToList();
            var savedMap = db.HqPhieuCans.AsNoTracking()
                .Where(x => itemIds.Contains(x.Id))
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            var cardCodes = items.Select(x => x.TheIdNhanVien).Distinct().ToList();
            var cardsMap = db.TheTu.AsNoTracking()
                .Where(x => cardCodes.Contains(x.MaTheTu))
                .ToDictionary(x => x.MaTheTu, StringComparer.Ordinal);

            var empCodes = cardsMap.Values.Select(x => x.MaNhanVien).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var employeesMap = db.NhanVienDaiThanh.AsNoTracking()
                .Where(x => empCodes.Contains(x.MaNhanVien))
                .ToDictionary(x => x.MaNhanVien, StringComparer.Ordinal);

            var workshopHistory = db.LichSuXuongMayCans.AsNoTracking()
                .Where(x => x.MayCan == client.Id)
                .OrderByDescending(x => x.TuThoiGian)
                .ToList();

            var minTime = items.Min(x => x.NgayGio).AddHours(-maxHours);
            var maxTime = items.Max(x => x.NgayGio);
            var preloadedScans = db.CheckInOuts.AsNoTracking()
                .Where(x => cardCodes.Contains(x.MaChamCong)
                    && (x.TrangThai == "Vào" || x.TrangThai == "Ra")
                    && x.ThoiGian >= minTime && x.ThoiGian <= maxTime)
                .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
                .ToList();

            foreach(var item in items) {
                if(savedMap.TryGetValue(item.Id, out var saved)) {
                    if(!MatchesExisting(saved,item)) throw new ArgumentException($"Id {item.Id} đã có dữ liệu khác. Không ghi đè.");
                    acknowledgedItems.Add(saved); existing++; continue;
                }
                cardsMap.TryGetValue(item.TheIdNhanVien, out var card);
                if(string.IsNullOrWhiteSpace(card?.MaNhanVien)) throw new ArgumentException($"Thẻ {item.TheIdNhanVien} chưa gán nhân viên.");
                if(!string.IsNullOrEmpty(item.MaNhanVien) && item.MaNhanVien!=card.MaNhanVien)
                    throw new ArgumentException($"Phiếu {item.Id}: mã nhân viên không khớp thẻ; cần đối chiếu lịch sử.");
                employeesMap.TryGetValue(card.MaNhanVien, out var employee);
                if(!ScaleProductionAttendancePolicy.IsProductionCard(employee))
                    throw new ArgumentException($"Thẻ {item.TheIdNhanVien} không thuộc nhân viên hoặc nhóm sản xuất hợp lệ.");
                item.MaNhanVien=card.MaNhanVien;
                var historicalWorkshop = ScaleWorkshopHistory.Resolve(workshopHistory, item.MayCan, item.NgayGio);
                if (!string.IsNullOrWhiteSpace(historicalWorkshop)) {
                    if (!string.IsNullOrEmpty(item.MaXuong) && !CodeEquals(item.MaXuong,historicalWorkshop))
                        throw new ArgumentException($"Phiếu {item.Id}: xưởng gửi lên khác lịch sử chuyển cân. Cần đối chiếu.");
                    item.MaXuong = historicalWorkshop;
                }
                // Missing legacy fields may only come from one unambiguous matching historical tuple.
                if(string.IsNullOrEmpty(item.MaLoaiNguyenLieu) || string.IsNullOrEmpty(item.MaXuong)) {
                    var references=db.HqPhieuCans.AsNoTracking().Where(x=>x.MayCan==item.MayCan && x.Ngay==item.Ngay
                        && x.MaLo==item.MaLo && x.MaSize==item.MaSize && x.MaThanhPham==item.MaThanhPham && x.Status==1
                        && (item.MaXuong=="" || x.MaXuong==item.MaXuong))
                        .Select(x=>new {x.MaLoaiNguyenLieu,x.MaXuong}).Distinct().ToList();
                    ResolveHistoricalMetadata(item,references.Select(x=>new HistoricalMetadata(x.MaLoaiNguyenLieu,x.MaXuong)).ToArray());
                }
                if(ScaleProductionAttendancePolicy.RequiresAttendance(employee)) {
                    var scan = preloadedScans
                        .Where(x => x.MaChamCong == item.TheIdNhanVien && x.ThoiGian <= item.NgayGio)
                        .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
                        .FirstOrDefault();
                    if (scan == null) {
                        scan = db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == item.TheIdNhanVien
                            && (x.TrangThai == "Vào" || x.TrangThai == "Ra") && x.ThoiGian <= item.NgayGio)
                            .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id).FirstOrDefault();
                    }
                    // Dữ liệu cũ đúng cân giữ đường kiểm tra đã có; ca chung cần
                    // đối chiếu cả máy tham gia và mốc đóng/mở tại giờ cân gốc.
                    var error = ValidateHistoricalScan(scan, int.Parse(client.Id, CultureInfo.InvariantCulture), item.NgayGio, maxHours);
                    if (error != null || scan?.ThoiGian == item.NgayGio)
                        error = ScaleSharedAttendanceService.HistoricalWeightError(db, item.TheIdNhanVien,
                            int.Parse(client.Id, CultureInfo.InvariantCulture), item.NgayGio, item.MaThanhPham, maxHours);
                    if(error!=null) throw new ArgumentException($"Phiếu {item.Id}: {error}");
                }
                db.HqPhieuCans.Add(item); acknowledgedItems.Add(item); inserted++;
            }
            db.SaveChanges(); transaction.Commit();
            // Updating UI state must never turn a committed database batch into a failure acknowledgement.
            try { onCommitted?.Invoke(acknowledgedItems); }
            catch(Exception ex) { Console.WriteLine($"[HQ BACKUP UI ERROR] Machine={client.Id}; {ex.Message}"); }
            var acknowledgements=acknowledgedItems.Select(x=>new DataCoummunication {
                Id=x.Id, IdIn=x.Id, IdOut=x.Id, MayCan=x.MayCan, NgayGio=x.NgayGio.ToString("yyyyMMddHHmmss"),
                TheId=x.TheIdNhanVien, TheIdNhanVien=x.TheIdNhanVien, MaNhanVien=x.MaNhanVien,
                MaLo=x.MaLo,MaSize=x.MaSize,MaThanhPham=x.MaThanhPham,MaLoaiNguyenLieu=x.MaLoaiNguyenLieu,MaXuong=x.MaXuong,
                TrongLuong=x.TrongLuong,TrongLuongTare=x.TrongLuongTare,Status=1,ChiSanLuong=1,STT=x.STT
            }).ToArray();
            Console.WriteLine($"[HQ BACKUP OK] Machine={client.Id}; Cmd={cmd}; Received={items.Length}; Inserted={inserted}; Existing={existing}");
            return Tuple.Create(true,JsonConvert.SerializeObject(batch?(object)acknowledgements:acknowledgements[0]));
        } catch(Exception ex) {
            Console.WriteLine($"[HQ BACKUP ERROR] Machine={client.Id}; Cmd={cmd}; {ex}");
            var message=ex is ArgumentException ? ex.Message : "Không lưu được backup HQ. Đợt này chưa được xác nhận lưu; kiểm tra log PMSHub rồi gửi lại cùng Id.";
            return Tuple.Create(false,JsonConvert.SerializeObject(new DataError {Cmd=cmd,ErrorString=message}));
        }
    }
}
