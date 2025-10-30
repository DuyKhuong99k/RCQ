using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMSSetUpRolepermistion.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace PMSSetUpRolepermistion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public class FuFunc
        {
            public string Fu { get; set; }
            public string Func { get; set; }
        }
        public IActionResult GetFuFuncOginals()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "FuFunc.json");

            // Đọc nội dung của tệp JSON
            var jsonData = System.IO.File.ReadAllText(filePath);

            // Chuyển đổi chuỗi JSON thành danh sách đối tượng FuFunc
            var fuFuncList = JsonConvert.DeserializeObject<List<FuFunc>>(jsonData);

            // Trả về dữ liệu JSON dưới dạng JsonResult
            return Json(fuFuncList);
        }


        public IActionResult AddDataFuFunc(string fu, string func)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "FuFunc.json");
                List<FuFunc> dataEntries;
                if (System.IO.File.Exists(filePath))
                {
                    // Đọc nội dung hiện tại của file
                    string jsonContent = System.IO.File.ReadAllText(filePath);

                    // Deserialize nội dung JSON thành danh sách đối tượng
                    dataEntries = JsonConvert.DeserializeObject<List<FuFunc>>(jsonContent) ?? new List<FuFunc>();
                }
                else
                {
                    // Nếu file không tồn tại, tạo danh sách mới
                    dataEntries = new List<FuFunc>();
                }
                // Thêm dữ liệu mới vào danh sách
                dataEntries.Add(new FuFunc { Fu = fu, Func = func });
                // Serialize danh sách thành JSON
                string updatedJson = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);
                // Ghi JSON vào file
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được thêm thành công!"
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        public IActionResult InfoUpdate(string currentFuFunc)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = currentFuFunc.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }
            string currentFu = result.Count > 0 && result[0].Length > 0 ? result[0][0] : string.Empty;
            string currentFunc = result.Count > 0 && result[0].Length > 1 ? result[0][1] : string.Empty;


            return Json(new
            {
                isSuccess = true,
                Fu = currentFu,
                Func = currentFunc,
                Messages = ""
            });
        }
        public IActionResult UpdateDataFuFunc(string currentFuFunc, string newFu, string newFunc)
        {
            try
            {
                // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
                string[] subArrays = currentFuFunc.Split('|', StringSplitOptions.RemoveEmptyEntries);

                // Khởi tạo danh sách kết quả
                List<string[]> result = new List<string[]>();

                // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
                foreach (string subArray in subArrays)
                {
                    string[] elements = subArray.Split(',');
                    result.Add(elements);
                }
                string currentFu = result.Count > 0 && result[0].Length > 0 ? result[0][0] : string.Empty;
                string currentFunc = result.Count > 0 && result[0].Length > 1 ? result[0][1] : string.Empty;


                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "FuFunc.json");
                // Kiểm tra file có tồn tại không
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"File không tồn tại."
                    });
                }

                // Đọc nội dung hiện tại của file
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Deserialize nội dung JSON thành danh sách đối tượng
                List<FuFunc> dataEntries = JsonConvert.DeserializeObject<List<FuFunc>>(jsonContent) ?? new List<FuFunc>();

                // Tìm và cập nhật mục phù hợp
                bool isUpdated = false;
                foreach (var entry in dataEntries)
                {
                    if (entry.Fu == currentFu && entry.Func == currentFunc)
                    {
                        entry.Fu = newFu;
                        entry.Func = newFunc;
                        isUpdated = true;
                        break;
                    }
                }

                if (!isUpdated)
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"Không tìm thấy dữ liệu để cập nhật."
                    });
                }

                // Serialize danh sách thành JSON
                string updatedJson = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);

                // Ghi JSON vào file
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được cập nhật thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }


        public IActionResult DeleteDataFuFunc(string currentFuFunc)
        {
            try
            {
                // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
                string[] subArrays = currentFuFunc.Split('|', StringSplitOptions.RemoveEmptyEntries);

                // Khởi tạo danh sách kết quả
                List<string[]> result = new List<string[]>();

                // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
                foreach (string subArray in subArrays)
                {
                    string[] elements = subArray.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    result.Add(elements);
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "FuFunc.json");
                // Kiểm tra file có tồn tại không
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "File không tồn tại"
                    });
                }

                // Đọc nội dung hiện tại của file
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Deserialize nội dung JSON thành danh sách đối tượng
                List<FuFunc> dataEntries = JsonConvert.DeserializeObject<List<FuFunc>>(jsonContent) ?? new List<FuFunc>();

                // Xóa các mục phù hợp
                int deleteCount = 0;
                foreach (string[] elements in result)
                {
                    if (elements.Length >= 2)
                    {
                        string fu = elements[0];
                        string func = elements[1];

                        dataEntries.RemoveAll(entry => entry.Fu == fu && entry.Func == func);
                        deleteCount++;
                    }
                }

                if (deleteCount == 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không tìm thấy dữ liệu để xóa."
                    });
                }

                // Serialize danh sách thành JSON
                string updatedJson = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);

                // Ghi JSON vào file
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được xóa thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public IActionResult ExportJsonFuFunc()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "FuFunc.json");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File không tồn tại!");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = $"FuFunc.json";

                return File(fileBytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public IActionResult FuFuncManagementByComp()
        {
            return View();
        }

        public IActionResult GetNameFileJsonFuFunc()
        {
            // Đường dẫn tới thư mục 'Company'
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company");

            // Kiểm tra thư mục có tồn tại không
            if (Directory.Exists(directoryPath))
            {
                // Lấy danh sách các file JSON
                List<string> jsonFileNames = new List<string>();
                string[] files = Directory.GetFiles(directoryPath, "*.json");

                foreach (string file in files)
                {
                    // Lấy tên file (không kèm đường dẫn)
                    jsonFileNames.Add(Path.GetFileName(file));
                }

                return Json(new
                {
                    isSuccess = true,
                    Messages = "",
                    Data = jsonFileNames.ToArray()
                });

            }
            else
            {
                Console.WriteLine($"Thư mục không tồn tại: {directoryPath}");
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Thư mục không tồn tại: {directoryPath}",
                });
            }
        }

        public IActionResult CreateJsonFile(string companyName)
        {
            // Đường dẫn tới thư mục 'Company'
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company");

            try
            {
                // Kiểm tra và tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Tên file mới
                string fileName = $"{companyName}.json";

                // Đường dẫn đầy đủ của file
                string filePath = Path.Combine(directoryPath, fileName);

                // Kiểm tra nếu file đã tồn tại
                if (System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"File {fileName} đã tồn tại."
                    });
                }

                // Nội dung mặc định của file JSON
                var defaultContent = new[]
                {
                    new
                    {
                        Fu = "JSONFILE",
                        Func = companyName
                    }
                };

                // Chuyển đổi đối tượng mặc định thành chuỗi JSON
                string jsonContent = JsonConvert.SerializeObject(defaultContent, Formatting.Indented);

                // Tạo file JSON mới với nội dung mặc định
                System.IO.File.WriteAllText(filePath, jsonContent);

                return Json(new
                {
                    isSuccess = true,
                    Messages = $"File {fileName} đã được tạo thành công.",
                    FilePath = filePath
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Đã xảy ra lỗi: {ex.Message}"
                });
            }
        }


        public IActionResult GetFuFuncWithCompany(string company)
        {
            try
            {
                // Đường dẫn tới tệp JSON
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company", $"{company}");

                // Kiểm tra xem tệp có tồn tại hay không
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"Tệp {company} không tồn tại."
                    });
                }

                // Đọc nội dung của tệp JSON
                var jsonData = System.IO.File.ReadAllText(filePath);

                // Kiểm tra nếu nội dung tệp trống
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"Tệp {company} không có dữ liệu."
                    });
                }

                // Chuyển đổi chuỗi JSON thành danh sách đối tượng FuFunc
                var fuFuncList = JsonConvert.DeserializeObject<List<FuFunc>>(jsonData) ?? new List<FuFunc>();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu được lấy thành công.",
                    Data = fuFuncList
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Đã xảy ra lỗi: {ex.Message}"
                });
            }
        }
        public class FuFuncEqualityComparer : IEqualityComparer<FuFunc>
        {
            public bool Equals(FuFunc x, FuFunc y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;

                return x.Fu == y.Fu && x.Func == y.Func;
            }

            public int GetHashCode(FuFunc obj)
            {
                if (obj == null)
                    return 0;

                return (obj.Fu?.GetHashCode() ?? 0) ^ (obj.Func?.GetHashCode() ?? 0);
            }
        }

        public IActionResult GetDataMultiSelectRolePermistion(string company)
        {
            // Lấy dữ liệu từ GetFuFuncWithCompany
            var rolePermissionsResult = GetFuFuncWithCompany(company) as JsonResult;
            var rolePermissionsData = rolePermissionsResult?.Value as dynamic;

            // Lấy danh sách RolePermistion (cần cast về List<FuFunc>)
            var rolePermissions = rolePermissionsData?.Data as List<FuFunc> ?? new List<FuFunc>();

            // Lấy dữ liệu gốc từ GetFuFuncOginals
            var jsonResult = GetFuFuncOginals() as JsonResult;
            var fuFuncList = jsonResult?.Value as List<FuFunc> ?? new List<FuFunc>();

            if (fuFuncList != null && rolePermissions != null)
            {
                // Lấy danh sách Fu và Func đã có từ RolePermistion
                var existingFuFunc = rolePermissions.Select(rp => new FuFunc { Fu = rp.Fu, Func = rp.Func }).ToList();

                // Loại bỏ các cặp Fu và Func đã có
                var filteredFuFunc = fuFuncList.Except(existingFuFunc, new FuFuncEqualityComparer()).ToList();

                return Json(filteredFuFunc);

            }

            // Xử lý trường hợp không lấy được giá trị hoặc có lỗi khác
            return Json(new { error = "Không thể lấy dữ liệu JSON." });
        }

        public async Task<IActionResult> DoInsertDecentralization(string company, string fu, string func)
        {
            try
            {
                if (string.IsNullOrEmpty(company) || string.IsNullOrEmpty(fu) || string.IsNullOrEmpty(func))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var jsonDataResult = GetFuFuncOginals();
                if (jsonDataResult is not JsonResult jsonResult)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không thể lấy dữ liệu JSON."
                    });
                }
                var jsonData = System.Text.Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonResult.Value)));
                // Deserialize JSON thành danh sách đối tượng FuFunc
                var fuFuncList = JsonConvert.DeserializeObject<List<FuFunc>>(jsonData);

                // Xây dựng bảng ánh xạ giữa fu và danh sách func
                var fuFuncMap = fuFuncList.GroupBy(ff => ff.Fu)
                                          .ToDictionary(group => group.Key, group => group.Select(ff => ff.Func).ToList());
                // Tạo danh sách đối tượng RolePermistion
                //var rolePermissionList = new List<RolePermistion>();
                // Duyệt qua từng giá trị fu từ param
                var fuValues = fu.Split(',').Select(value => value.Trim());
                foreach (var fuValue in fuValues)
                {
                    // Kiểm tra xem có fu nào tương ứng trong bảng ánh xạ không
                    if (fuFuncMap.TryGetValue(fuValue, out var funcList))
                    {
                        //// Duyệt qua từng giá trị func từ param
                        var funcValues = func.Split(',').Select(value => value.Trim());

                        // Kiểm tra xem danh sách funcList có chứa ít nhất một giá trị nào đó từ funcValues không
                        if (funcList.Any(funcValues.Contains))
                        {
                            // Tìm ra tất cả các giá trị func từ funcValues mà có trong funcList
                            var matchingFuncValues = funcValues.Intersect(funcList);

                            foreach (var matchingFuncValue in matchingFuncValues)
                            {
                                AddDataFuFuncWitCompany(company, fuValue, matchingFuncValue);
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = $"Không tìm thấy ánh xạ cho Fu: {fuValue}, Func: {string.Join(", ", funcList)}."
                            });
                        }
                    }
                    else
                    {
                        // Xử lý trường hợp không tìm thấy ánh xạ
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = $"Không tìm thấy ánh xạ cho Fu: {fuValue}."
                        });
                    }
                }
                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được thêm thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Đã xảy ra lỗi: " + ex.Message.ToString()
                });
            }
        }

        public IActionResult AddDataFuFuncWitCompany(string company, string fu, string func)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company", $"{company}");
                List<FuFunc> dataEntries;
                if (System.IO.File.Exists(filePath))
                {
                    // Đọc nội dung hiện tại của file
                    string jsonContent = System.IO.File.ReadAllText(filePath);

                    // Deserialize nội dung JSON thành danh sách đối tượng
                    dataEntries = JsonConvert.DeserializeObject<List<FuFunc>>(jsonContent) ?? new List<FuFunc>();
                }
                else
                {
                    // Nếu file không tồn tại, tạo danh sách mới
                    dataEntries = new List<FuFunc>();
                }
                // Thêm dữ liệu mới vào danh sách
                dataEntries.Add(new FuFunc { Fu = fu, Func = func });
                // Serialize danh sách thành JSON
                string updatedJson = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);
                // Ghi JSON vào file
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được thêm thành công!"
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        public IActionResult InfoUpdateWitCompany(string currentFuFunc)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = currentFuFunc.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }
            string currentFu = result.Count > 0 && result[0].Length > 0 ? result[0][0] : string.Empty;
            string currentFunc = result.Count > 0 && result[0].Length > 1 ? result[0][1] : string.Empty;


            return Json(new
            {
                isSuccess = true,
                Fu = currentFu,
                Func = currentFunc,
                Messages = ""
            });
        }
        public IActionResult UpdateDataFuFuncWitCompany(string company, string currentFuFunc, string newFu, string newFunc)
        {
            try
            {
                // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
                string[] subArrays = currentFuFunc.Split('|', StringSplitOptions.RemoveEmptyEntries);

                // Khởi tạo danh sách kết quả
                List<string[]> result = new List<string[]>();

                // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
                foreach (string subArray in subArrays)
                {
                    string[] elements = subArray.Split(',');
                    result.Add(elements);
                }
                string currentFu = result.Count > 0 && result[0].Length > 0 ? result[0][0] : string.Empty;
                string currentFunc = result.Count > 0 && result[0].Length > 1 ? result[0][1] : string.Empty;


                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company", $"{company}");
                // Kiểm tra file có tồn tại không
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"File không tồn tại."
                    });
                }

                // Đọc nội dung hiện tại của file
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Deserialize nội dung JSON thành danh sách đối tượng
                List<FuFunc> dataEntries = JsonConvert.DeserializeObject<List<FuFunc>>(jsonContent) ?? new List<FuFunc>();

                // Tìm và cập nhật mục phù hợp
                bool isUpdated = false;
                foreach (var entry in dataEntries)
                {
                    if (entry.Fu == currentFu && entry.Func == currentFunc)
                    {
                        entry.Fu = newFu;
                        entry.Func = newFunc;
                        isUpdated = true;
                        break;
                    }
                }

                if (!isUpdated)
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"Không tìm thấy dữ liệu để cập nhật."
                    });
                }

                // Serialize danh sách thành JSON
                string updatedJson = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);

                // Ghi JSON vào file
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được cập nhật thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }


        public IActionResult DeleteDataFuFuncWitCompany(string company, string currentFuFunc)
        {
            try
            {
                // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
                string[] subArrays = currentFuFunc.Split('|', StringSplitOptions.RemoveEmptyEntries);

                // Khởi tạo danh sách kết quả
                List<string[]> result = new List<string[]>();

                // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
                foreach (string subArray in subArrays)
                {
                    string[] elements = subArray.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    result.Add(elements);
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company", $"{company}");
                // Kiểm tra file có tồn tại không
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "File không tồn tại"
                    });
                }

                // Đọc nội dung hiện tại của file
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Deserialize nội dung JSON thành danh sách đối tượng
                List<FuFunc> dataEntries = JsonConvert.DeserializeObject<List<FuFunc>>(jsonContent) ?? new List<FuFunc>();

                // Xóa các mục phù hợp
                int deleteCount = 0;
                foreach (string[] elements in result)
                {
                    if (elements.Length >= 2)
                    {
                        string fu = elements[0];
                        string func = elements[1];

                        dataEntries.RemoveAll(entry => entry.Fu == fu && entry.Func == func);
                        deleteCount++;
                    }
                }

                if (deleteCount == 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không tìm thấy dữ liệu để xóa."
                    });
                }

                // Serialize danh sách thành JSON
                string updatedJson = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);

                // Ghi JSON vào file
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Dữ liệu đã được xóa thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public IActionResult ExportJsonFuFuncWitCompany(string company)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "Company", $"{company}");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File không tồn tại!");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = $"{company}";

                return File(fileBytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
