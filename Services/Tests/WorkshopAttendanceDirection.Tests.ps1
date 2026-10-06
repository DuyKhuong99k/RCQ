$ErrorActionPreference = 'Stop'

# Test quy tắc trạng thái từ chính mã nguồn, không kết nối hay ghi database.
$sourcePath = Join-Path $PSScriptRoot '..\WorkshopHub.cs'
$source = Get-Content -Raw $sourcePath
$method = [regex]::Match($source,
    'internal static string NextStatus\(string\? previousStatus\)\s*=>\s*previousStatus == "Vào" \? "Ra" : "Vào";').Value
if (-not $method) { throw 'Không tìm thấy quy tắc NextStatus trong mã nguồn.' }
$method = $method.Replace('internal static', 'public static').Replace('string?', 'string')
if (-not ('WorkshopDirectionTest' -as [type])) {
    Add-Type -TypeDefinition ('public static class WorkshopDirectionTest {' + $method + '}')
}

function Assert-Direction($previous, $expected, $label) {
    $actual = [WorkshopDirectionTest]::NextStatus($previous)
    if ($actual -ne $expected) { throw "$label : expected $expected, actual $actual" }
    Write-Output "PASS: $label"
}

Assert-Direction $null 'Vào' 'Thẻ chưa có lịch sử hợp lệ'
Assert-Direction 'Vào' 'Ra' 'Sau lượt Vào trong cùng ngày -> Ra'
Assert-Direction $null 'Vào' 'Sang ngày mới không có lịch sử trong ngày -> Vào'
Assert-Direction 'Ra' 'Vào' 'Sau lượt Ra -> lượt Vào tiếp theo'

# Bảo vệ phần truy vấn: chỉ xét trạng thái hợp lệ trong ngày.
$lookup = [regex]::Match($source, '(?s)var latest =.*?\.FirstOrDefault\(\);').Value
if (-not $lookup.Contains('x.TrangThai == "Vào" || x.TrangThai == "Ra"') -or
    -not $lookup.Contains('.OrderByDescending(x => x.ThoiGian)') -or
    -not $lookup.Contains('x.ThoiGian >= dayStart && x.ThoiGian < dayEnd') -or
    -not $source.Contains('var dayStart = when.Date;')) {
    throw 'Truy vấn lịch sử phải giới hạn trong ngày.'
}
if ($source.IndexOf('if (existing != null)') -gt $source.IndexOf('var latest =')) {
    throw 'Phải kiểm tra gửi trùng trước khi xác định trạng thái.'
}
if (-not $source.Contains('latest.ThoiGian > when')) {
    throw 'Thiếu kiểm tra dữ liệu đến sai thứ tự.'
}
if (-not $source.Contains('status == "Ra" && latest?.TrangThai != "Vào"')) {
    throw 'Phải từ chối Ra khi không có Vào trong ngày.'
}
Write-Output 'PASS: reset theo ngày; bỏ trạng thái không hợp lệ; kiểm tra trùng và giờ cũ; Ra phải có Vào trong ngày.'
