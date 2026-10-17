using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Models.Repos.Models;
using MvvmHelpers;
using Security.Crypt;
using Vars;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Services
{
    public partial class LoginService(
        IHttpContextAccessor httpContextAccessor,
        IMainService mainService,
        IUserService userService)
        : ObservableObject, ILoginService
    {
        [ObservableProperty] private ObservableRangeCollection<UserArea> userAreas = new();
        private string _defaultKey = "PMS";


        public bool IsUserAuthenticated()
        {
            var result = Handlers.ErrorHandler.Handle<bool>(() =>
            {
                var val = httpContextAccessor.HttpContext?.Session.GetString("UserId");
                return val!= null && val != "-1";
            });
            return result is { IsSuccess: true, Result: true };
        }
        
        public bool IsSessionCreated()
        {
            var result = Handlers.ErrorHandler.Handle<bool>(() =>
            {
                var val = httpContextAccessor.HttpContext?.Session.GetString("SessionCreated");
                return val != null && val == "1";
            });
            return result is { IsSuccess: true, Result: true };
        }


        public string GetSessionVal(string key)
        {
            var result = Handlers.ErrorHandler.Handle<string>(() =>
            {
                var val = httpContextAccessor.HttpContext?.Session.GetString(key);
                return val;
            });
            if (result.IsSuccess)
                return result.Result;
            else
                return string.Empty;
        }
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (NguoiDung? User, DateTime Expired)> _userCache = new();

        public bool LoginLocal(string userName, string passWord)
        {
            var isLoginSuccess = false;
            var result = Handlers.ErrorHandler.Handle(() =>
            {
                NguoiDung? user = null;
                var now = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(userName) && _userCache.TryGetValue(userName, out var cached) && cached.Expired > now)
                {
                    user = cached.User;
                }
                else
                {
                    user = mainService.VmNguoiDung.GetByUserName(userName).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        _userCache[userName] = (user, now.AddSeconds(user != null ? 60 : 10));
                    }
                }

                isLoginSuccess = LoginLocalUser(user, passWord);
                if (!isLoginSuccess && !string.IsNullOrWhiteSpace(userName))
                {
                    _userCache.TryRemove(userName, out _);
                }
            });
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.Error?.Message);
                isLoginSuccess = false;
            }
            return isLoginSuccess;
        }

        // Dùng cùng nhánh này trong kiểm thử cô lập, không cần truy cập SQL.
        internal bool LoginLocalUser(NguoiDung? user, string passWord)
        {
            // NULL chưa được xác nhận hoạt động: không cho tạo phiên.
            if (user?.IsActive != true || string.IsNullOrEmpty(passWord)
                || string.IsNullOrEmpty(user.Password)) return false;
            var storedPassword = ED.DecryptString(user.Password, _defaultKey);
            if (string.IsNullOrEmpty(storedPassword) || !storedPassword.Equals(passWord)) return false;
            var session = httpContextAccessor.HttpContext?.Session;
            if (session == null) return false;
            userService.AddUserArea(user.Id);
            session.SetString("Username", user.UserName);
            session.SetString("UserId", user.Id.ToString());
            return IsUserAuthenticated();
        }

        public async Task Logout()
        {
            var result = Handlers.ErrorHandler.Handle(() =>
            {
                var userId = httpContextAccessor.HttpContext?.Session.GetString("UserId");
                if (userId != null)
                {
                    userService.RemoveUserArea(int.Parse(userId));
                    httpContextAccessor.HttpContext?.Session.Remove("UserId");
                }
                httpContextAccessor.HttpContext?.Session.Remove("Username");
            });
            if (!result.IsSuccess)
                Console.WriteLine(result.Error?.Message);
            //var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            //if(userId != null)
            //{
            //    _userService.RemoveUserArea(int.Parse(userId));
            //    _httpContextAccessor.HttpContext?.Session.Remove("UserId");
            //}
            //_httpContextAccessor.HttpContext?.Session.Remove("Username");
            
        }
        public void CreateDefaultSession()
        {
              httpContextAccessor.HttpContext?.Session.SetString("Username", "");
              httpContextAccessor.HttpContext?.Session.SetString("UserId", "-1");
              httpContextAccessor.HttpContext?.Session.SetString("SessionCreated", "1");  
        }
        public string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", string.Empty).ToLower();
        }
        private byte[] encryptData(string data)
        {
            var md5Hasher =
                new MD5CryptoServiceProvider(
                );
            byte[] hashedBytes;
            var encoder = new UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
    }
}
