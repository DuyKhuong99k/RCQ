using System.Net.Http.Headers;
using Newtonsoft.Json;
using PMS.Models;
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
    public async Task<ApiResponse?> PostAsync(HttpContext context, string apiUrl,StringContent? content)
    {
        try
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", context.Session.GetString("JWTToken"));
            var response = await client.PostAsync(apiUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);

                return apiResponse;
            }
            else
            {
                var apiResponseErros = JsonSerializer.Deserialize<ApiResponse>(responseBody);
                return apiResponseErros;
            }

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