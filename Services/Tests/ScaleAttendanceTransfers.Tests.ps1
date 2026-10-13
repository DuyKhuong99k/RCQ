$ErrorActionPreference = 'Stop'
$processor = Get-Content -Raw (Join-Path $PSScriptRoot '..\ScaleAttendanceProcessor.cs')
$transfer = Get-Content -Raw (Join-Path $PSScriptRoot '..\ScaleAttendanceTransferService.cs')
$expiry = [regex]::Match($processor, '(?s)internal static bool IsShiftExpired\(.*?;').Value.Replace('internal', 'public')
$message = [regex]::Match($processor, '(?s)internal static string OtherMachineMessage\(.*?;').Value.Replace('internal', 'public')
$open = [regex]::Match($transfer, '(?s)public static List<CheckInOut> OpenShifts\(.*?\.ToList\(\);').Value
$open = $open.Replace('dbPMScontext', 'TestDb').Replace('.AsNoTracking()', '')
if (!$expiry -or !$message -or !$open) { throw 'Không tìm thấy quy tắc cần kiểm tra.' }
if (-not ('TransferPolicyTest' -as [type])) {
    $stub = @'
#nullable enable
using System; using System.Collections.Generic; using System.Linq;
public class CheckInOut {
 public string Id {get;set;} = ""; public string MaChamCong {get;set;} = "";
 public int MaSoMay {get;set;} public DateTime ThoiGian {get;set;}
 public string? TrangThai {get;set;} public string? VaoId {get;set;}
}
public class TestDb { public IQueryable<CheckInOut> CheckInOuts {get;set;} = new List<CheckInOut>().AsQueryable(); }
public static class TransferPolicyTest {
'@
    Add-Type -TypeDefinition ($stub + $expiry + $message + $open + '}')
}
function Assert-Equal($actual, $expected, $label) {
    if ($actual -ne $expected) { throw "$label : expected=$expected actual=$actual" }
    Write-Output "PASS: $label"
}
Assert-Equal ($processor.Contains('if (active.Any(x => x.MaSoMay != machine))')) $true 'Chặn khác máy cho cả Vào và Ra'
Assert-Equal ($processor.Contains('var scannedMachine = machine;')) $false 'Không tự đổi máy để chấp nhận Ra khác máy'
function Event($id, $card, $machine, $time, $direction, $vaoId = $null) {
    $row = [CheckInOut]::new(); $row.Id = $id; $row.MaChamCong = $card; $row.MaSoMay = $machine
    $row.ThoiGian = [datetime]$time; $row.TrangThai = $direction; $row.VaoId = $vaoId
    return $row
}
function Database($events) {
    $db = [TestDb]::new()
    $list = [System.Collections.Generic.List[CheckInOut]]::new()
    foreach ($event in $events) { $list.Add($event) }
    $db.CheckInOuts = [System.Linq.Queryable]::AsQueryable[CheckInOut]($list)
    return $db
}
$a = Event 'A1' 'card1' 7779520 '2026-10-07T08:00:00' 'Vào'
$db = Database @($a)
Assert-Equal ([TransferPolicyTest]::OpenShifts($db, 'card1').Count) 1 'Phát hiện ca đang mở tại A'
Assert-Equal ([TransferPolicyTest]::OpenShifts($db, 'card2').Count) 0 'Không lấy nhầm thẻ khác'
$outA = Event 'A2' 'card1' 7779520 '2026-10-07T10:00:00' 'Ra' 'A1'
$inB = Event 'B1' 'card1' 7779521 '2026-10-07T10:00:00' 'Vào'
$db = Database @($a, $outA, $inB)
$open = [TransferPolicyTest]::OpenShifts($db, 'card1')
Assert-Equal $open.Count 1 'Chuyển máy chỉ còn một ca mở'
Assert-Equal $open[0].MaSoMay 7779521 'Ca mở chuyển sang B'
$outB = Event 'B2' 'card1' 7779521 '2026-10-07T12:00:00' 'Ra' 'B1'
$db = Database @($a, $outA, $inB, $outB)
Assert-Equal ([TransferPolicyTest]::OpenShifts($db, 'card1').Count) 0 'Ra tại B đóng ca cuối'
$db = Database @($a, $inB)
Assert-Equal ([TransferPolicyTest]::OpenShifts($db, 'card1').Count) 2 'Phát hiện dữ liệu cũ có hai ca mở'
$root = [datetime]'2026-10-06T22:00:00'
Assert-Equal ([TransferPolicyTest]::IsShiftExpired($root, [datetime]'2026-10-07T14:00:01', 16, $true)) $true 'Ca máy cũ vẫn hết hạn sau 16 giờ'
$newMachineStart = [datetime]'2026-10-07T02:00:00'
Assert-Equal ([TransferPolicyTest]::IsShiftExpired($newMachineStart, [datetime]'2026-10-07T14:00:01', 16, $true)) $false 'Ca máy mới tính giới hạn từ phiếu Vào mới'
Assert-Equal ([TransferPolicyTest]::IsShiftExpired($root, [datetime]'2026-10-07T06:00:00', 16, $true)) $false 'Ca đêm chuyển máy còn hạn đến sáng'
Assert-Equal ([TransferPolicyTest]::OtherMachineMessage(7779520)) 'Đang Vào tại 7779520. Ra tại máy cũ hoặc xác nhận chuyển máy.' 'Thông báo ngắn cho firmware'
