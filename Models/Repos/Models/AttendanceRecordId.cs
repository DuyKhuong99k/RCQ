using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

public static class AttendanceRecordId
{
    // Một thiết bị chỉ có một lượt chấm công trong mỗi giây.
    public static string Create(string deviceCode, DateTime time)
    {
        var device = deviceCode.Trim();
        if (device.Length is 0 or > 50)
            throw new ArgumentException("Mã thiết bị phải có từ 1 đến 50 ký tự.");
        return device + "." + time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }

    public static bool IsDuplicate(DbUpdateException error) =>
        error.InnerException is SqlException sql && sql.Number is 2601 or 2627;
}
