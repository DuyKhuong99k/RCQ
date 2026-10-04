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
Assert-Direction 'Vào' 'Ra' 'Vào 22:00 hôm trước -> Ra 06:00 hôm sau'
Assert-Direction 'Ra' 'Vào' 'Sau lượt Ra -> lượt Vào tiếp theo'

# Bảo vệ phần truy vấn: không giới hạn ngày, chỉ xét trạng thái hợp lệ.
$lookup = [regex]::Match($source, '(?s)var latest =.*?\.FirstOrDefault\(\);').Value
if (-not $lookup.Contains('x.TrangThai == "Vào" || x.TrangThai == "Ra"') -or
    -not $lookup.Contains('.OrderByDescending(x => x.ThoiGian)') -or
    $lookup.Contains('when.Date')) {
    throw 'Truy vấn lịch sử không đúng quy tắc xuyên ngày.'
}
if ($source.IndexOf('if (existing != null)') -gt $source.IndexOf('var latest =')) {
    throw 'Phải kiểm tra gửi trùng trước khi xác định trạng thái.'
}
if (-not $source.Contains('latest.ThoiGian > when')) {
    throw 'Thiếu kiểm tra dữ liệu đến sai thứ tự.'
}
Write-Output 'PASS: truy vấn xuyên ngày; bỏ trạng thái không hợp lệ; trùng kiểm tra trước; kiểm tra giờ cũ.'
