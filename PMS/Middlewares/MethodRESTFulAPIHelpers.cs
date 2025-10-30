using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;
using PMS.Models;
using JsonException = Newtonsoft.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PMS.Middlewares;

public class MethodRESTFulAPIHelpers : IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient client;


    public MethodRESTFulAPIHelpers(IHttpClientFactory httpClientFactory)
    {
        client = httpClientFactory.CreateClient(AppViewModels.AppViewModel.Instance.HttpClientName);
        _httpClientFactory = httpClientFactory;
    }

    public void Dispose()
    {
        client.Dispose();
    }
    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }

    public async Task<ApiResponse?> DelateApiAsyncWithTokenAsync<T>(HttpContext context, string apiUrl)
    {
        try
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
            var response = await client.DeleteAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

                return apiResponse;
            }

            return new ApiResponse
            {
                Message = $"Có lỗi xảy ra khi gọi API: {response.ReasonPhrase}",
                Data = null,
                Success = false
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Message = $"Có lỗi xảy ra khi gọi API: {ex.Message}",
                Data = null,
                Success = false
            };
        }
    }

    /// <summary>
    ///     GET API không cần xác thực TOKEN
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="apiurl"></param>
    /// <returns></returns>
    public async Task<List<T>> GetAsync<T>(string apiurl)
    {
        IEnumerable<object> dataSource = null;
        var response = await client.GetAsync(apiurl);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var sourceObjects = JsonConvert.DeserializeObject<IEnumerable<object>>(data);
            if (sourceObjects != null) dataSource = sourceObjects;
        }


        var jsonString = JsonConvert.SerializeObject(dataSource);
        var apiData = JsonConvert.DeserializeObject<List<T>>(jsonString)?.ToList();
        return apiData;
    }

    /// <summary>
    ///     Get data table syncfustion, param name="context" = HttpContext để lấy session
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="apiUrl"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(HttpContext context, string apiUrl)
    {
        try
        {
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var sourceObject = JsonConvert.DeserializeObject<T>(data);
                return sourceObject;
            }


            Console.WriteLine($"Lỗi trong GetApiAsyncWithToken2: {response.ReasonPhrase}");
            return default; // Trả về giá trị mặc định cho kiểu T
        }
        catch (Exception ex)
        {
            // Xử lý các ngoại lệ (ghi log, hiển thị thông báo, v.v.)
            Console.WriteLine($"Lỗi trong GetApiAsyncWithToken2: {ex.Message}");
            return default; // Trả về giá trị mặc định cho kiểu T
        }
    }

    // public async Task<T> GetAsync<T>(HttpContext context, string apiUrl)
    // {
    //     try
    //     {
    //         var client = _httpClientFactory.CreateClient();
    //         client.DefaultRequestHeaders.Clear();
    //
    //         var token = context.Session.GetString("JWTToken");
    //         if (!string.IsNullOrEmpty(token))
    //         {
    //             client.DefaultRequestHeaders.Authorization =
    //                 new AuthenticationHeaderValue("Bearer", token);
    //         }
    //
    //         var response = await client.GetAsync(apiUrl);
    //         if (response.IsSuccessStatusCode)
    //         {
    //             var data = await response.Content.ReadAsStringAsync();
    //             return JsonConvert.DeserializeObject<T>(data);
    //         }
    //
    //         Console.WriteLine($"[GET] Lỗi API: {response.StatusCode} - {response.ReasonPhrase}");
    //         return default;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"[GET] Exception: {ex.Message}");
    //         return default;
    //     }
    // }
    public async Task<List<T>> GetAsync2<T>(HttpContext context, string apiUrl)
    {
        try
        {
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var sourceObject = JsonConvert.DeserializeObject<List<T>>(data).ToList();
                return sourceObject;
            }

            Console.WriteLine($"Lỗi trong GetApiAsyncWithToken2: {response.ReasonPhrase}");
            return default; // Trả về giá trị mặc định cho kiểu T
        }
        catch (Exception ex)
        {
            // Xử lý các ngoại lệ (ghi log, hiển thị thông báo, v.v.)
            Console.WriteLine($"Lỗi trong GetApiAsyncWithToken2: {ex.Message}");
            return default; // Trả về giá trị mặc định cho kiểu T
        }
    }
    //public async Task<ApiResponse?> PostAsync(HttpContext context, string apiUrl,
    //    StringContent? content)
    //{
    //    try
    //    {
    //        client.DefaultRequestHeaders.Authorization =
    //            new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
    //        var response = await client.PostAsync(apiUrl, content);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            var responseBody = await response.Content.ReadAsStringAsync();
    //            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

    //            return apiResponse;
    //        }


    //        return new ApiResponse
    //        {
    //            Message = $"Có lỗi xảy ra khi gọi API: {response.ReasonPhrase}",
    //            Data = null,
    //            Success = false
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse
    //        {
    //            Message = $"Có lỗi xảy ra khi gọi API: {ex.Message}",
    //            Data = null,
    //            Success = false
    //        };
    //    }
    //}
    //public async Task<ApiResponse?> PostAsync(HttpContext context, string apiUrl,StringContent? content)
    //{
    //    try
    //    {
    //        client.DefaultRequestHeaders.Authorization =
    //            new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
    //        var response = await client.PostAsync(apiUrl, content);
    //        var responseBody = await response.Content.ReadAsStringAsync();
    //        if (response.IsSuccessStatusCode)
    //        {

    //            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

    //            return apiResponse;
    //        }
    //        else
    //        {
    //            var apiResponseErros = JsonSerializer.Deserialize<ApiResponse>(responseBody);
    //            return apiResponseErros;
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse
    //        {
    //            Message = $"Có lỗi xảy ra khi gọi API: {ex.Message}",
    //            Data = null,
    //            Success = false
    //        };
    //    }
    //}
    public async Task<ApiResponse?> PostAsync(HttpContext context, string apiUrl, StringContent? content)
    {
        try
        {
            // Khởi tạo HttpClient nếu chưa có (bạn nên khai báo ở ngoài class để tái sử dụng)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            using var client = new HttpClient(handler);
            //using var client = new HttpClient();

            // Gán JWT token từ session nếu có
            var token = context.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Gửi request POST
            var response = await client.PostAsync(apiUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody, options);
                    return apiResponse ?? new ApiResponse
                    {
                        Success = false,
                        Message = "API trả về dữ liệu rỗng.",
                        Data = null
                    };
                }
                catch (JsonException)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không thể phân tích dữ liệu trả về từ API.",
                        Data = null
                    };
                }
            }
            else
            {
                try
                {
                    // Thử deserialize theo kiểu ProblemDetails để lấy thông tin lỗi chi tiết
                    var problem = JsonSerializer.Deserialize<ProblemDetails>(responseBody, options);

                    var errorMessages = problem?.Errors?
                        .SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"))
                        .ToList();

                    var finalMessage = problem?.Title;
                    if (errorMessages?.Any() == true)
                    {
                        finalMessage += "\n" + string.Join("\n", errorMessages);
                    }

                    return new ApiResponse
                    {
                        Success = false,
                        Message = finalMessage ?? $"API trả về lỗi {(int)response.StatusCode}: {response.ReasonPhrase}",
                        Data = null
                    };
                }
                catch (JsonException)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = $"API lỗi và không thể phân tích nội dung lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}",
                        Data = responseBody
                    };
                }
            }
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse
            {
                Success = false,
                Message = $"Không thể kết nối API: {ex.Message}",
                Data = null
            };
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "Timeout khi gọi API. Vui lòng kiểm tra kết nối mạng hoặc server.",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Success = false,
                Message = $"Đã xảy ra lỗi không xác định: {ex.Message}",
                Data = null
            };
        }
    }




    public async Task<ApiResponse?> PutApiAsyncWithTokenAsync<T>(HttpContext context, string apiUrl,
        StringContent content)
    {
        try
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
            var response = await client.PutAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

                return apiResponse;
            }

            return new ApiResponse
            {
                Message = $"Có lỗi xảy ra khi gọi API: {response.ReasonPhrase}",
                Data = null,
                Success = false
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Message = $"Có lỗi xảy ra khi gọi API: {ex.Message}",
                Data = null,
                Success = false
            };
        }
    }
}