using System.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json.Linq;

namespace Services;

/// <summary>Historical HTTP tickets. Deliberately separate from live LITE/ST validation.</summary>
public static class HqHttpBackupWriter
{
    public sealed record Result(HQ_PhieuCan Item, bool Inserted);

    public static HQ_PhieuCan Parse(JObject json, string registeredMachine)
    {
        string Text(string key, int limit = 50, bool required = true)
        {
            var token = json[key];
            if (token == null || token.Type == JTokenType.Null)
            {
                if (!required) return "";
                throw new ArgumentException($"Thiếu {key} trong phiếu backup.");
            }
            if (token.Type != JTokenType.String)
                throw new ArgumentException($"Sai {key} trong phiếu backup.");
            var value = token.Value<string>()!;
            if (value.Length > limit || value.Any(char.IsControl) || (required && string.IsNullOrWhiteSpace(value)))
                throw new ArgumentException($"Sai {key} trong phiếu backup.");
            return value;
        }
        decimal Number(string key, bool required = false)
        {
            var token = json[key];
            if (token == null && !required) return 0;
            const NumberStyles style = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
            if (token == null || token.Type is not (JTokenType.String or JTokenType.Integer or JTokenType.Float) ||
                !decimal.TryParse(token.Type == JTokenType.String ? token.Value<string>() : token.ToString(Newtonsoft.Json.Formatting.None),
                    style, CultureInfo.InvariantCulture, out var value))
                throw new ArgumentException($"Sai {key} trong phiếu backup.");
            return value;
        }

        // Legacy offline firmware checked stability BEFORE writing SQLite and sends no Stated.
        // Absence is compatible, not proof of ST. If explicitly supplied, only ST is accepted.
        if (json.TryGetValue("Stated", out var state) &&
            (state.Type != JTokenType.String || state.Value<string>() != "ST"))
            throw new ArgumentException("Phiếu backup có trạng thái không hợp lệ.");
        var machine = Text("MayCan");
        if (machine != registeredMachine) throw new ArgumentException("Phiếu backup không thuộc máy đăng ký.");
        var id = Text("Id", 100);
        if (!id.StartsWith(machine + ".", StringComparison.Ordinal))
            throw new ArgumentException("Id backup không khớp mã máy.");
        var timeText = Text("NgayGio", 14);
        if (!DateTime.TryParseExact(timeText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var time) || time > ScaleWorkshopHistory.BusinessNow())
            throw new ArgumentException("Ngày giờ backup không hợp lệ hoặc ở tương lai.");
        var weight = Number("TrongLuongNhan", true);
        var tare = Number("TrongLuongTare");
        if (!HqWeightValues.IsValidTicket(weight, tare))
            throw new ArgumentException("Số kg hoặc bì phiếu backup không hợp lệ.");
        var stt = 0;
        if (json.TryGetValue("STT", out var ordinal) &&
            (ordinal.Type is not (JTokenType.Integer or JTokenType.String) ||
             !int.TryParse(ordinal.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out stt)))
            throw new ArgumentException("STT phiếu backup không hợp lệ.");
        var card = Text("TheId");
        return new HQ_PhieuCan
        {
            // Keep original Id/time/codes; never re-stamp an offline ticket at upload time.
            Id = id, STT = stt, Ngay = DateOnly.FromDateTime(time), NgayGio = time,
            MayCan = machine, MaLo = Text("MaLo"), MaSize = Text("MaSize"), MaThanhPham = Text("MaThanhPham"),
            MaLoaiNguyenLieu = Text("MaLoaiNguyenLieu"), MaNhanVien = Text("MaNhanVien"),
            TheId = card, TheIdNhanVien = card, TrongLuong = weight, TrongLuongTare = tare,
            MaXuong = Text("MaXuong", required: false), MaNhanVienPhucVu = "", MaNhanVienBanKiem = "",
            ChiSanLuong = true, Status = 1, GhiChu = ""
        };
    }

    public static Result Save(dbPMScontext db, HQ_PhieuCan requested)
    {
        using var transaction = db.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        // Same card-first and Id resource as WSS online/Sync; no second DAO connection.
        ScaleAttendanceTransferService.LockCard(db, requested.TheIdNhanVien);
        var resource = "HQWeightBackup:" + requested.Id;
        db.Database.ExecuteSqlInterpolated($@"DECLARE @result int;
            EXEC @result=sys.sp_getapplock @Resource={resource},@LockMode='Exclusive',
                @LockOwner='Transaction',@LockTimeout=5000;
            IF @result<0 THROW 51001,N'Không khóa được phiếu backup.',1;");
        var existing = db.HqPhieuCans.AsNoTracking().SingleOrDefault(x => x.Id == requested.Id);
        if (existing != null)
        {
            // A retry after machine configuration changes must compare with the original ticket,
            // not today's workshop. Omitted workshop is intentionally not guessed on replay.
            if (!HqWeightBackupProcessor.MatchesExisting(existing, requested))
                throw new ArgumentException("Id đã có dữ liệu khác. Không ghi đè.");
            transaction.Commit();
            return new(existing, false);
        }

        var historicalWorkshop = ScaleWorkshopHistory.Resolve(db, requested.MayCan, requested.NgayGio);
        if (string.IsNullOrWhiteSpace(historicalWorkshop))
            throw new ArgumentException("Chưa xác định xưởng tại giờ backup. Kiểm tra lịch sử chuyển xưởng.");
        if (historicalWorkshop.Length > 50 || (!string.IsNullOrWhiteSpace(requested.MaXuong) &&
            !string.Equals(requested.MaXuong, historicalWorkshop, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Xưởng phiếu backup khác lịch sử chuyển cân.");
        requested.MaXuong = historicalWorkshop;
        // Preserve the existing offline 30-second policy; do not impose it on online LITE.
        if (HqWeightDuplicatePolicy.HasDuplicate(db, requested))
            throw new ArgumentException(HqWeightDuplicatePolicy.ErrorMessage);
        db.HqPhieuCans.Add(requested);
        db.SaveChanges();
        transaction.Commit();
        return new(requested, true);
    }
}
