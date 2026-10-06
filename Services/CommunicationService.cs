using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

public class CommunicationService(IUserService userService, ICommitService commitService,
    IOptions<ScaleAttendanceOptions> attendanceOptions) : ICommunicationService
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
        // CHAMCONG chứa tiếng Việt: firmware có thể gửi độ dài byte UTF-8.
        if (dataJson.Length != len &&
            !(string.Equals(cmd, "CHAMCONG", StringComparison.OrdinalIgnoreCase) &&
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
                    var result = ScaleAttendanceProcessor.Process(client, dataJson, attendanceOptions.Value.MaxShiftHours);
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
                    var dataComs = JsonConvert.DeserializeObject<List<DataCoummunication>>(dataJson);
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        // throw;
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
        dataTranfer.IsError = isError;
        var data = JsonConvert.SerializeObject(dataTranfer);
        // var lendata = ASCIIEncoding.Unicode.GetByteCount(data);
        return data;
    }

    #endregion
}
