using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using Security.Crypt;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppSetting _appSettings;
    private readonly dbPMScontext _context;
    private readonly string _defaultKey = "PMS";
    private readonly int _defaultRole = 4;

    public UserController(dbPMScontext context, IOptionsMonitor<AppSetting> optionsMonitor)
    {
        _context = context;
        _appSettings = optionsMonitor.CurrentValue;
    }

    public double HourExpires { get; set; } = 1;

    private MainViewModel Vm => MainViewModel.Instance;

    private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
    }

    [HttpPost("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // Kiểm tra xem ID người dùng có hợp lệ không
        if (id <= 0)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "ID người dùng không hợp lệ."
            });

        // Xóa Refresh Tokens liên quan đến người dùng
        var refreshTokens = _context.RefreshTokens.Where(rt => rt.UserId == id).ToList();
        _context.RefreshTokens.RemoveRange(refreshTokens);

        // Xóa User Roles liên quan đến người dùng
        var userRoles = _context.UserRoles.Where(ur => ur.UserId == id).ToList();
        _context.UserRoles.RemoveRange(userRoles);

        // Xóa người dùng
        var user = await _context.NguoiDungs.FindAsync(id);
        if (user == null)
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Người dùng không tồn tại."
            });

        _context.NguoiDungs.Remove(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi xóa người dùng.",
                Errors = new List<string> { ex.Message }
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Xóa người dùng thành công!"
        });
    }

    private string GenerateRefreshToken()
    {
        var random = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        return Convert.ToBase64String(random);
    }

    // Tạo token và thêm vai trò của người dùng vào token
    private async Task<TokenModel> GenerateToken(NguoiDung nguoiDung, double hour)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
        var utc60 = DateTime.UtcNow.AddMinutes(hour * 60);
        var secs = (utc60 - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", nguoiDung.Id.ToString()),
                new Claim("UserName", nguoiDung.UserName),
                new Claim(ClaimTypes.Name, nguoiDung.HoTen),
                new Claim("ExpiresSec", secs.ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
                //new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),

            Expires = utc60, // thời hạn hiệu lực của Token là 1h, sau 1h thì Token sẽ mất hiệu lực

            //Ký vào Token 1 khoá bí mật
            //Tương đương chỉ có thể xác minh token đó nếu nó được tạo ra bằng khóa bí mật tương ứng.
            //Nếu người khác có được token nhưng không biết khóa bí mật, họ không thể truy cập thông tin bên trong trang web bằng token này được.
            //Khuyết điểm của JWT: nếu Token bị lộ thì vẫn có thể đọc được các thông tin cơ bản trong Claim, nên hạn chế những thông tin nhạy cảm trong claim.
            //Tuy nhiên, lưu ý rằng nếu mã ký hiệu (SecretKey) bị lộ, ai đó có thể tạo ra các token giả mạo với thông tin khác nhau và ký bằng khóa bí mật đó.
            //Vì vậy, việc giữ cho mã ký hiệu được bảo vệ là cực kỳ quan trọng để đảm bảo an toàn của hệ thống.
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
                SecurityAlgorithms.HmacSha512Signature)
        };


        // Lấy danh sách vai trò của người dùng và thêm vào token
        var roles = await GetUserRoles(nguoiDung);
        foreach (var role in roles) tokenDescription.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

        try
        {
            // Tạo token và refresh token, lưu thông tin refresh token vào cơ sở dữ liệu
            var token = jwtTokenHandler.CreateToken(tokenDescription);


            var accessToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = nguoiDung.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(hour)
            };
            await _context.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expires = tokenDescription.Expires,
                HourExpires = hour
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<List<int?>> GetIdUserRoles(NguoiDung user)
    {
        var roles = await _context.UserRoles
            .Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToListAsync();
        return roles;
    }

    // GET: api/NguoiDung/{id}
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetNguoiDungById(int id)
    {
        var nguoiDung = _context.NguoiDungs.FirstOrDefault(u => u.Id == id);
        if (nguoiDung == null)
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Người dùng không tồn tại."
            });

        return Ok(nguoiDung);
    }

    [HttpGet("{username}")]
    [Authorize]
    public IActionResult GetNguoiDungByUserName(string username)
    {
        var nguoiDung = _context.NguoiDungs.FirstOrDefault(u => u.UserName == username);
        if (nguoiDung == null)
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Người dùng không tồn tại."
            });

        return Ok(nguoiDung);
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetNguoiDungNeededDatas()
    {
        var nguoiDungs = _context.NguoiDungs.OrderByDescending(x => x.Id).ToList();

        // Chỉ lấy UserName và HoTen
        var nguoiDungsSelected = nguoiDungs.Select(item => new
        {
            item.Id, item.HoTen
        });

        return Ok(nguoiDungsSelected);
    }

    // GET: api/NguoiDung
    [HttpGet]
    [Authorize]
    public IActionResult GetNguoiDungs()
    {
        var nguoiDungs = _context.NguoiDungs.OrderByDescending(x => x.Id).ToList();

        // Thêm cột STT vào dữ liệu
        var nguoiDungsWithSTT = nguoiDungs.Select((item, index) => new
        {
            STT = index + 1,
            item.Id,
            item.UserName,
            item.Password,
            item.HoTen,
            item.Email,
            item.MaNhanVien,
            item.PhoneNumber,
            item.Address,
            item.Status,
            item.IsActive
        });

        return Ok(nguoiDungsWithSTT);
    }

    [HttpGet]
    public async Task<List<string>> GetRoles(string username)
    {
        var userId = await _context.NguoiDungs.Where(x => x.UserName == username).Select(x => x.Id).FirstAsync();

        var roles = await _context.UserRoles
            .Where(x => x.UserId == userId)
            .Join(_context.Roles, x => x.RoleId, r => r.Id, (x, r) => r.Name).ToListAsync();
        return roles;
    }

    [HttpGet("{username}")]
    public async Task<List<Role>> GetRolesFull(string username)
    {
        var userId = await _context.NguoiDungs.Where(x => x.UserName == username).Select(x => x.Id).FirstAsync();

        var roleIds = await _context.UserRoles
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId).Distinct().ToListAsync();
        try
        {
            var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();

            return roles;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    //lấy danh sách role người dùng
    private async Task<List<string>> GetUserRoles(NguoiDung user)
    {
        var roles = await _context.UserRoles
            .Where(x => x.UserId == user.Id)
            .Join(_context.Roles, x => x.RoleId, r => r.Id, (x, r) => r.Name).ToListAsync();
        return roles;
    }

    //đăng xuất
    [HttpPost]
    [AllowAnonymous] // Yêu cầu người dùng đã xác thực trước khi truy cập
    public async Task<IActionResult> Logout()
    {
        // Lấy token từ request header
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        // Trích xuất thông tin từ chuỗi token
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(token);
        // Lấy giá trị Id từ decodedToken
        var tokenId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (!string.IsNullOrEmpty(token))
            try
            {
                // Tìm và xác minh refresh token liên quan đến người dùng
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.JwtId == tokenId);
                // Xác định xem token trong request header là access token hay refresh token
                if (storedToken != null)
                {
                    // Đánh dấu refresh token đã sử dụng và thu hồi
                    storedToken.IsUsed = true;
                    storedToken.IsRevoked = true;
                    _context.Update(storedToken);
                    await _context.SaveChangesAsync();
                    // Xoá cookie bằng cách đăng xuất
                    // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    //// Gọi hàm huỷ token trong session hoặc cookie (đối với access token)
                    //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    // Gửi response thành công
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Đăng xuất thành công!"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Token không hợp lệ!"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.ToString()
                });
                ;
            }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = "Không tìm thấy token trong header."
        });
    }

    [HttpPost("{username}")]
    [Authorize]
    public async Task<IActionResult> RecoverAccount(string username, [FromBody] RecoverPasswordModel recoverModel)
    {
        // Kiểm tra xem tài khoản có tồn tại không
        var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Tài khoản không tồn tại."
            });

        // Chỉ cho phép thay đổi mật khẩu
        if (string.IsNullOrEmpty(recoverModel.NewPassword))
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Mật khẩu mới không được để trống."
            });

        // Cập nhật mật khẩu
        user.Password = ED.EncryptString(recoverModel.NewPassword, _defaultKey);

        // Lưu thay đổi vào cơ sở dữ liệu
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi khôi phục mật khẩu.",
                Errors = new List<string> { ex.Message }
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Mật khẩu đã được thay đổi thành công!"
        });
    }

    // đăng ký
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        //kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ.",
                Errors = errors
            });
        }

        // ... kiểm tra người dùng đã tồn tại chưa ...
        if (_context.NguoiDungs.Any(u => u.UserName == model.UserName))
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Tên người dùng đã tồn tại."
            });
        // Tạo đối tượng NguoiDung mới từ dữ liệu đăng ký
        var newUser = new NguoiDung
        {
            UserName = model.UserName,
            HoTen = model.HoTen,
            Password = ED.EncryptString(model.Password, _defaultKey),
            DefaultKey = _defaultKey,
            IsActive = false, //Được mặc định là ko kích hoạt, chỉ quyền cấp là Admin mới có thể kích hoạt
            Status = 4, // 1: ĐANG LÀM, 2: TẠM NGHĨ, 3: THÔI VIỆC, 4: THỬ VIỆC
            MaNhanVien = model.MaNhanVien,
            PhoneNumber = model.PhoneNumber
        };

        // Thêm người dùng mới vào cơ sở dữ liệu
        _context.NguoiDungs.Add(newUser);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi lưu dữ liệu.",
                Errors = new List<string> { ex.Message }
            });
        }

        // Tạo bản ghi UserRole để liên kết người dùng với vai trò "user"
        var defaultUserRole = new UserRole
        {
            UserId = newUser.Id,
            RoleId = _defaultRole // "Guest"
        };

        _context.UserRoles.Add(defaultUserRole);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi lưu dữ liệu.",
                Errors = new List<string> { ex.Message }
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Đăng Ký thành công!"
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RenewToken(TokenModel model)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
        var tokenValidateParam = new TokenValidationParameters
        {
            //tự cấp token
            ValidateIssuer = false,
            ValidateAudience = false,

            //ký vào token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false // ko kiem tra token het han
        };
        var hander = await Handlers.ErrorHandler.HandleAsync<OkObjectResult>(async () =>
        {
             var tokenInVerification =
                jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);
            //var jwtSecurityToken = validatedToken as JwtSecurityToken;
            // check 2: chech thuat toan ma hoa token
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                    StringComparison
                        .InvariantCultureIgnoreCase); //StringComparison.InvariantCultureIgnoreCase ko phan biet hoa thuong
                if (result == false)
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token không hợp lệ!"
                    });
            }

            // check refresh token cos trong database khoong?
            var storedToken =
                _context.RefreshTokens.FirstOrDefault(x =>
                    x.Token == model.RefreshToken); // sai ở đây, về nghiên cứu lại
            if (storedToken == null)
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "RefreshTokens không tìm thấy!"
                });
            // check 5: check refresh token da Used/revoked chua
            if (storedToken.IsUsed)
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "RefreshTokens đã được sử dụng"
                });
            if (storedToken.IsRevoked) // co thu hoi hay chua
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "RefreshTokens đã bị thu hồi"
                });
            // check 6: Accesstoken id == JwtId in Refeshtoken
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedToken.JwtId != jti)
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tokens không khớp!"
                });

            // check 3: kiem tra token da het han chua?
            var utcExpireDate = long.Parse(tokenInVerification.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            //if (expireDate > DateTime.UtcNow)
            //{
            //    return Ok(new ApiResponse
            //    {
            //        Success = false,
            //        Message = "Token chưa hết hạn!"
            //    });
            //}
            // Kiểm tra thời gian còn lại của AccessToken (ví dụ: 5 phút)
            var remainingTime = expireDate - DateTime.UtcNow;
            if (remainingTime <= TimeSpan.FromMinutes(5))
                // AccessToken còn ít hơn hoặc bằng 5 phút, cấp AccessToken mới
                if (storedToken != null && !storedToken.IsUsed && !storedToken.IsRevoked)
                {
                    // Thu hồi AccessToken cũ
                    storedToken.IsRevoked = true;
                    storedToken.IsUsed = true;
                    _context.Update(storedToken);
                    await _context.SaveChangesAsync();

                    // Tạo AccessToken mới và gửi lại cho người dùng
                    var user = await _context.NguoiDungs.SingleOrDefaultAsync(nd => nd.Id == storedToken.UserId);
                    var newToken = await GenerateToken(user, model.HourExpires); //  HourExpires lấy từ model.
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Cấp AccessToken mới thành công",
                        Data = newToken
                    });
                }

            // AccessToken còn hợp lệ, không cần cấp mới
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "AccessToken vẫn hợp lệ, không cấp mới!"
            });

        });
        if (!hander.IsSuccess)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = "Có gì đó không đúng! vui lòng kiểm tra" + hander.Error?.Message
            });
        }
        else
        {
            return hander.Result;
        }
    }

    // API Update thông tin người dùng
    [HttpPost("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterModel model)
    {
        // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
        if (id == null || id <= 0)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "ID người dùng không hợp lệ."
            });

        // Kiểm tra xem người dùng có tồn tại trong cơ sở dữ liệu không
        var user = await _context.NguoiDungs.FindAsync(id);
        if (user == null)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Người dùng không tồn tại."
            });

        // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ.",
                Errors = errors
            });
        }

        // Cập nhật thông tin người dùng từ dữ liệu đầu vào
        user.AvatarImg = model.AvatarImg;
        user.HoTen = model.HoTen;
        user.Email = model.Email;
        user.MaNhanVien = model.MaNhanVien;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        user.CardID = model.CardID;
        user.IsActive = model.IsActive;
        user.Status = model.Status;
        // Cập nhật các trường thông tin khác tại đây nếu cần

        // Lưu thay đổi vào cơ sở dữ liệu
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                Errors = new List<string> { ex.Message }
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Cập nhật thông tin người dùng thành công!"
        });
    }

    [HttpPost("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUserSetting(int id, [FromBody] UpdateUsertSetting model)
    {
        // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
        if (id == null || id <= 0)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "ID người dùng không hợp lệ."
            });

        // Kiểm tra xem người dùng có tồn tại trong cơ sở dữ liệu không
        var user = await _context.NguoiDungs.FindAsync(id);
        if (user == null)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Người dùng không tồn tại."
            });

        // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ.",
                Errors = errors
            });
        }

        // Cập nhật thông tin người dùng từ dữ liệu đầu vào
        user.AvatarImg = model.AvatarImg;
        user.HoTen = model.HoTen;
        user.Email = model.Email;
        user.MaNhanVien = model.MaNhanVien;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        // Lưu thay đổi vào cơ sở dữ liệu
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                Errors = new List<string> { ex.Message }
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Cập nhật thông tin người dùng thành công!"
        });
    }

    public async Task<IActionResult> UploadImg(IFormFile file)
    {
        try
        {
            // Kiểm tra xem tệp có tồn tại không và có kích thước lớn hơn không
            if (file == null || file.Length == 0) return BadRequest("Không tìm thấy tệp hoặc tệp trống.");

            // Tạo thư mục nếu nó chưa tồn tại
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Authentication", "images",
                    "Avatar")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Authentication",
                    "images", "Avatar"));

            // Tạo đường dẫn đầy đủ cho tệp ảnh mới
            var filePath =
                Path.Combine(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Authentication", "images", "Avatar"),
                    file.FileName);

            // Mở luồng để sao chép dữ liệu từ tệp được tải lên vào tệp mới
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về một phản hồi thành công nếu tải lên thành công
            return Ok(new { isSuccess = true, Message = "Tải lên tệp ảnh thành công." });
        }
        catch (Exception ex)
        {
            // Trả về một phản hồi lỗi nếu có lỗi xảy ra
            return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
        }
    }

    //đăng nhập
    [HttpPost]
    [AllowAnonymous] // Cho phép người dùng chưa xác thực truy cập
    public async Task<IActionResult> Validate(LoginModel model)
    {
        try
        {
            var count = _context.NguoiDungs.Count();
            // Kiểm tra thong tin user
            var user = _context.NguoiDungs.SingleOrDefault(p => p.UserName == model.UserName);
            if (user == null || !ED.DecryptString(user.Password, _defaultKey).Equals(model.Password)) // khong dung
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tên người dùng hoặc mật khẩu không hợp lệ!"
                });
            if (user.IsActive == false)
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tài khoản chưa được kích hoạt! Vui lòng liên hệ quản trị viên!"
                });
            // Tạo và trả về token sau khi xác thực thành công
            //hourExpires = 7;
            if (model.RememberMe)
                HourExpires = model.Hour;
            else
                HourExpires = 1;
            var token = await GenerateToken(user, HourExpires);
            // Đặt cookie với giá trị token
            //HttpContext.Response.Cookies.Append("JWTToken", token.AccessToken, new CookieOptions
            //{
            //    HttpOnly = true, // Ngăn chặn truy cập từ phía JavaScript
            //    Secure = true,   // Yêu cầu sử dụng kết nối an toàn (HTTPS)
            //    SameSite = SameSiteMode.None, // Cho phép gửi cookie trong các yêu cầu cross-site khi là none
            //    Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(HourExpires)), // Thời hạn của cookie
            //    Path = "/", // Đặt path của cookie (tùy chọn)
            //});
            //// Thêm JWT token và thời hạn của token vào cookie
            //HttpContext.Response.Cookies.Append("JWTToken", token.AccessToken, new Microsoft.AspNetCore.Http.CookieOptions
            //{
            //    HttpOnly = true, // Ngăn chặn truy cập từ phía JavaScript
            //    Secure = true,   // Yêu cầu sử dụng kết nối an toàn (HTTPS)
            //    SameSite = SameSiteMode.None, // Cho phép gửi cookie trong các yêu cầu cross-site khi là none
            //    Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(token.HourExpires)), // Thời hạn của cookie
            //});
            //var identity = new ClaimsIdentity(new[]
            //       {
            //            new Claim(ClaimTypes.Name, model.UserName),
            //            new Claim("jwt", token.AccessToken),
            //        }, "MyCookies");

            //// Tạo Principal
            //var principal = new ClaimsPrincipal(identity);

            //// Đặt cookie xác thực
            //await HttpContext.SignInAsync("MyCookies",principal);
            ////ênd cokie
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xác thực thành công!",
                Data = token
            });
        }
        catch (Exception e)
        {
            return Ok(new ApiResponse
            {
                Success = false,
                Message = e.ToString()
            });
            ;
        }
    }
}