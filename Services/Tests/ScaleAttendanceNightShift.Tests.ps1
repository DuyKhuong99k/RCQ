$ErrorActionPreference = 'Stop'
$source = Get-Content -Raw (Join-Path $PSScriptRoot '..\ScaleAttendanceProcessor.cs')
$expiry = [regex]::Match($source, '(?s)internal static bool IsShiftExpired\(.*?;').Value
$timing = [regex]::Match($source, '(?s)internal static AttendanceTiming BuildTiming\(.*?\r?\n    \}').Value
$record = [regex]::Match($source, '(?s)internal sealed record AttendanceTiming\(.*?;').Value
if (!$expiry -or !$timing -or !$record) { throw 'Không tìm thấy các hàm kiểm tra ca.' }
if (-not ('NightShiftPolicyTest' -as [type])) {
    Add-Type -TypeDefinition ("#nullable enable`n" + 'using System; using System.Globalization; public static class NightShiftPolicyTest {' +
        ($expiry + $timing + $record).Replace('internal', 'public') + '}')
}
function Assert-Equal($actual, $expected, $label) {
    if ($actual -ne $expected) { throw "$label : expected=$expected actual=$actual" }
    Write-Output "PASS: $label"
}
$start = [datetime]'2026-10-06T22:00:00'
$end = [datetime]'2026-10-07T06:00:00'
Assert-Equal ([NightShiftPolicyTest]::IsShiftExpired($start, $end, 16, $true)) $false 'Ca đêm 22:00 -> 06:00 còn hạn'
Assert-Equal ([NightShiftPolicyTest]::IsShiftExpired($start, $end, 16, $false)) $true 'Ca thường không ghép qua ngày'
Assert-Equal ([NightShiftPolicyTest]::BuildTiming($start, $end, $true).TongThoiGianGiay) 28800 'Ca đêm đủ 8 giờ'
Assert-Equal ([NightShiftPolicyTest]::BuildTiming($start, $end, $true).TongThoiGian) '08:00:00' 'Định dạng giờ ca đêm'
Assert-Equal ([NightShiftPolicyTest]::IsShiftExpired($start, $start.AddHours(16), 16, $true)) $false 'Đúng 16 giờ vẫn hợp lệ'
Assert-Equal ([NightShiftPolicyTest]::IsShiftExpired($start, $start.AddHours(16).AddSeconds(1), 16, $true)) $true 'Quá 16 giờ phải xử lý lỗi'
Assert-Equal ([NightShiftPolicyTest]::IsShiftExpired([datetime]'2026-10-06T23:00:00', [datetime]'2026-10-07T00:00:00', 16, $false)) $true 'Ca thường hết hạn lúc 00:00'
$rejected = $false
try { [NightShiftPolicyTest]::BuildTiming($start, $end, $false) | Out-Null } catch { $rejected = $true }
Assert-Equal $rejected $true 'Không tính giờ xuyên ngày cho ca thường'
$rejected = $false
try { [NightShiftPolicyTest]::BuildTiming($start, $start.AddSeconds(-1), $true) | Out-Null } catch { $rejected = $true }
Assert-Equal $rejected $true 'Từ chối Ra trước Vào'
Assert-Equal ($source.Contains('request.Cadem = checkIn.CaDem;')) $true 'Ra dùng loại ca của Vào'
