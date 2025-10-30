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
        public bool Login(string username, string tocken)
        {
            //_deviceViewService.Load(AppKV.Main,"");
            Login(username);
            return true;
        }


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

        public void Login(string username)
        {
            var result = Handlers.ErrorHandler.Handle(() =>
            {
                var userId = mainService.VmNguoiDung.GetId(username);
                if (userId != null)
                {
                    userService.AddUserArea(userId??-1);
                    httpContextAccessor.HttpContext?.Session.SetString("Username", username);
                    httpContextAccessor.HttpContext?.Session.SetString("UserId", (userId??-1).ToString());
                     //var val = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                }
            });
            if (!result.IsSuccess)
                Console.WriteLine(result.Error?.Message);

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
        public bool LoginLocal(string userName, string passWord)
        {
            var isLoginSuccess = false;
            var result = Handlers.ErrorHandler.Handle(() =>
            {
                var user = mainService.VmNguoiDung.GetByUserName(userName).FirstOrDefault();
              
                    if (user != null && ED.DecryptString(user.Password, _defaultKey).Equals(passWord))
                    {
                        userService.AddUserArea(user.Id);
                        httpContextAccessor.HttpContext?.Session.SetString("Username", userName);
                        httpContextAccessor.HttpContext?.Session.SetString("UserId", (user.Id).ToString());

                        isLoginSuccess = IsUserAuthenticated(); //true;
                    }
                
                
            });
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.Error?.Message);
                isLoginSuccess = false;
            }
            // Kiểm tra lại session đã được tạo hay chưa
            //if (!IsSessionCreated())
            //{
            //    Console.WriteLine("Session không được tạo thành công. Thử lại.");
            //    isLoginSuccess = false;
            //}
            return isLoginSuccess;
            
                

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
