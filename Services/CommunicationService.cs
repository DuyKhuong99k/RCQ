using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

public class CommunicationService(IUserService userService, ICommitService commitService,
    IOptions<ScaleAttendanceOptions> attendanceOptions, IMayCansService? mayCansService = null,
    ILogger<CommunicationService>? logger = null, IAttendanceMode? attendanceMode = null) : ICommunicationService
{
    #region ICommunicationService Members

    public bool AuthProc(string connectedId, string id, int wType, string dataJson)
    {
        var item = userService.GetByConnectedId(connectedId);
        if (item == null) return false;
        userService.Set(item, id, wType);
        return true;
    }

    public async Task<string> Route(ClientInfo? client, string cmd, int len, string dataJson)
    {
        var dataTranfer = new DataTranfer();
        var isError = true;
        var error = new DataError();
        string? liteSaveStatus = null;
        // Attendance and weight backups may contain Vietnamese: accept exact UTF-8 byte length too.
        if (dataJson.Length != len &&
            !(new[] { "CHAMCONG", "PHIEUCANADD", "PHIEUCANADDS" }.Contains(cmd, StringComparer.OrdinalIgnoreCase) &&
              Encoding.UTF8.GetByteCount(dataJson) == len))
        {
            error.ErrorString = "Dữ Liệu Không Toàn Vẹn";
            error.Cmd = cmd;
        }
        else if (client == null)
        {
            error.ErrorString = "Client Null";
            error.Cmd = cmd;
        }
        else if (client.IsActive == false)
        {
            error.ErrorString = "Client chưa active";
            error.Cmd = cmd;
        }
        else
        {
            switch (cmd.ToUpper())
            {
                case "CHAMCONG":
                {
                    if (attendanceMode?.Current.Enabled == false)
                    {
                        var disabled = ScaleAttendanceProcessor.Failure(AttendanceModeMessages.DisabledCode, AttendanceModeMessages.Disabled);
                        dataTranfer.DataError = disabled.Json;
                        break;
                    }
                    var selected = mayCansService?.Find(client.Id);
                    var result = selected?.WKv == Vars.AppKV.Hq
                        ? await ProcessHqAttendance(client, dataJson, selected)
                        : ScaleAttendanceProcessor.Process(client, dataJson, attendanceOptions.Value.MaxShiftHours,
                            selected?.MaThanhPham, selected?.MaLoaiNguyenLieu);
                    isError = !result.Success;
                    if (result.Success) dataTranfer.Data = result.Json;
                    else dataTranfer.DataError = result.Json;
                    break;
                }
                case "PHIEUCANADD":
                {
                    var val = commitService.Commit(dataJson, client, cmd);
                    if (val.Item1)
                    {
                        isError = false;
                        dataTranfer.Data = val.Item2;
                    }
                    else
                    {
                        //error.ErrorString = val.Item2;
                        //error.Cmd = cmd;
                        dataTranfer.DataError = val.Item2;
                    }


                    break;
                }
                case "PHIEUCANADDS":
                {
                    List<DataCoummunication>? dataComs;
                    try { dataComs = JsonConvert.DeserializeObject<List<DataCoummunication>>(dataJson); }
                    catch (JsonException) {
                        error.ErrorString = "JSON danh sách phiếu backup không hợp lệ";
                        error.Cmd = cmd;
                        break;
                    }
                    if (dataComs == null)
                    {
                        error.ErrorString = "Data không đúng chuẩn";
                        error.Cmd = cmd;
                    }
                    else
                    {
                        var val = commitService.Commit(dataComs, client, cmd);
                        if (val.Item1)
                        {
                            isError = false;
                            dataTranfer.Data = val.Item2;
                        }
                        else
                        {
                            //error.ErrorString = val.Item2;
                            //error.Cmd = cmd;
                            dataTranfer.DataError = val.Item2;
                        }
                    }


                    break;
                }
                case "PHIEUCANCHECKCARD":
                {
                    var checkingRl = commitService.CheckTheId(dataJson, client.WKv, cmd);

                    if (checkingRl.Item1)
                    {
                        isError = false;
                        dataTranfer.Data = checkingRl.Item2;
                    }
                    else
                    {
                        //error.ErrorString = checkingRl.Item2;
                        //error.Cmd = cmd;
                        dataTranfer.DataError = checkingRl.Item2;
                    }


                    break;
                }
                case "LITE":
                {
                    try
                    {
                        var val = await commitService.CommitLite(dataJson, client, cmd);
                        if (val.Item1)
                        {
                            isError = false;
                            dataTranfer.Data = val.Item2;
                        }
                        else
                        {
                            //error.ErrorString = val.Item2;
                            //error.Cmd = cmd;
                            dataTranfer.DataError = val.Item2;
                        }
                    }
                    catch (JsonException e)
                    {
                        HqFirmwareSend.Log($"[LITE JSON ERROR] Machine={client.Id}; {e}");
                        error.Cmd = cmd;
                        error.ErrorString = ScaleOnlineMessages.InvalidPayload;
                        liteSaveStatus = HqOnlineResponse.NotSaved;
                    }
                    catch (Exception e)
                    {
                        HqFirmwareSend.Log(e.ToString());
                        error.Cmd = cmd;
                        error.ErrorString = ScaleOnlineMessages.ProcessingError;
                        liteSaveStatus = HqOnlineResponse.Unknown;
                    }


                    break;
                }
                case "COIINSETIDMONITOR":
                {
                    var val = await commitService.CommitCoi(dataJson, client, cmd);
                    if (val.Item1)
                    {
                        isError = false;
                        dataTranfer.Data = val.Item2;
                    }
                    else
                    {
                        //error.ErrorString = val.Item2;
                        //error.Cmd = cmd;
                        dataTranfer.DataError = val.Item2;
                    }

                    break;
                }
                case "COIGETLITEREPORT":
                {
                    var val = await commitService.CommitCoi(dataJson, client, cmd);
                    if (val.Item1)
                    {
                        isError = false;
                        dataTranfer.Data = val.Item2;
                    }
                    else
                    {
                        //error.ErrorString = val.Item2;
                        //error.Cmd = cmd;
                        dataTranfer.DataError = val.Item2;
                    }

                    break;
                }
                case "CHISANLUONG":
                {
                    await commitService.SetChiSanLuong(dataJson, client, cmd);
                    break;
                }
                case "XACDINH":
                {
                    await commitService.SetXacDinh(dataJson, client, cmd);
                    break;
                }
                case "WLOFFLINE":
                {
                    break;
                }
                default:
                {
                    error.ErrorString = "Không Có CMD";
                    error.Cmd = cmd;
                    dataTranfer.DataError = JsonConvert.SerializeObject(error);
                    break;
                }
            }
            //}
        }

        // Không để lỗi độ dài/client/active biến thành phản hồi lỗi trống.
        if (isError && dataTranfer.DataError == null && !string.IsNullOrWhiteSpace(error.ErrorString))
            dataTranfer.DataError = JsonConvert.SerializeObject(error);
        if (isError && string.Equals(cmd, "LITE", StringComparison.OrdinalIgnoreCase) &&
            (client?.WKv == Vars.AppKV.Hq || (client != null && mayCansService?.Find(client.Id)?.WKv == Vars.AppKV.Hq)))
        {
            DataError? detail = null;
            if (!string.IsNullOrWhiteSpace(dataTranfer.DataError))
            {
                try { detail = JsonConvert.DeserializeObject<DataError>(dataTranfer.DataError); }
                catch (JsonException) { /* Never forward an empty/unstructured HQ error to the scale. */ }
            }
            var message = !string.IsNullOrWhiteSpace(detail?.ErrorString) ? detail.ErrorString :
                !string.IsNullOrWhiteSpace(error.ErrorString) ? error.ErrorString : ScaleOnlineMessages.ProcessingError;
            var correlated = HqOnlineResponse.Error(cmd, message, HqOnlineResponse.Correlation(dataJson), client?.Id,
                detail?.SaveStatus ?? liteSaveStatus ??
                (!string.IsNullOrWhiteSpace(error.ErrorString) ? HqOnlineResponse.NotSaved : HqOnlineResponse.Unknown));
            correlated.ErrorCode = detail?.ErrorCode ?? 0;
            dataTranfer.DataError = JsonConvert.SerializeObject(correlated);
        }
        dataTranfer.IsError = isError;
        var data = JsonConvert.SerializeObject(dataTranfer);
        // var lendata = ASCIIEncoding.Unicode.GetByteCount(data);
        return data;
    }

    #endregion

    private async Task<(bool Success, string Json)> ProcessHqAttendance(ClientInfo client, string json,
        Models.Repos.Models.MayCan machine)
    {
        ScaleAttendanceProcessor.Request? request;
        try { request = JsonConvert.DeserializeObject<ScaleAttendanceProcessor.Request>(json); }
        catch (JsonException) { return ScaleAttendanceProcessor.Failure("INVALID_JSON", "JSON chấm công không hợp lệ."); }
        if (request == null) return ScaleAttendanceProcessor.Failure("INVALID_JSON", "JSON chấm công không hợp lệ.");
        var captured = HqScaleSelection.Capture(machine);
        (bool Success, string Json) result;
        try
        {
            result = await HqOnlineWeightWriter.ForMachine(machine.Id, () =>
            {
                // Ra must remain usable while a new work context is pending.
                var selectionError = HqScaleSelection.CheckAttendance(machine, captured, request.TrangThai);
                return Task.FromResult(selectionError != null
                    ? ScaleAttendanceProcessor.Failure("WORK_CONTEXT_PENDING", selectionError, request)
                    : ScaleAttendanceProcessor.Process(client, json, attendanceOptions.Value.MaxShiftHours,
                        captured.Product, captured.Material));
            });
        }
        catch (HqOnlineWeightWriter.QueueTimeoutException)
        { result = ScaleAttendanceProcessor.Failure("BUSY", "Cân bận. Chưa ghi chấm công", request); }
        var code = JObject.Parse(result.Json).Value<string>("Code") ?? (result.Success ? "SAVED" : "REJECTED");
        var card = request.TheId?.Trim() ?? "";
        try
        {
            logger?.LogInformation("[HQ ATTENDANCE RESULT] Machine={Machine} Id={Id} CardTail={CardTail} Direction={Direction} DeviceTime={Time} Product={Product} Material={Material} Revision={Revision} Success={Success} Code={Code}",
                machine.Id, request.ChamCongId, card.Length > 4 ? card[^4..] : card, request.TrangThai,
                request.NgayGio, captured.Product, captured.Material, captured.Revision, result.Success, code);
        }
        catch { /* Diagnostics must never turn a committed Vao into a failed response. */ }
        return result;
    }
}
