using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class LoginService:ObservableObject,ILoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        [ObservableProperty] private ObservableRangeCollection<UserArea> userAreas = new();
        private readonly IMainService mainService;
        private readonly IUserService _userService;
        private readonly string _defaultKey = "PMS";
        public bool Login(string username, string tocken)
        {
            //_deviceViewService.Load(AppKV.Main,"");
            Login(username);
            return true;
        }
       

        public LoginService(IHttpContextAccessor httpContextAccessor, IMainService _mainService, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            mainService = _mainService;
            _userService = userService;
        }
        public bool IsUserAuthenticated()
        {
            var result = Handlers.ErrorHandler.Handle<bool>(() =>
            {
                var val = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                return val!= null && val != "-1";
            });
            return result is { IsSuccess: true, Result: true };
        }
        public bool IsSessionCreated()
        {
            var result = Handlers.ErrorHandler.Handle<bool>(() =>
            {
                var val = _httpContextAccessor.HttpContext?.Session.GetString("SessionCreated");
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
                    _userService.AddUserArea(userId??-1);
                    _httpContextAccessor.HttpContext?.Session.SetString("Username", username);
                    _httpContextAccessor.HttpContext?.Session.SetString("UserId", (userId??-1).ToString());
                     //var val = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                }
            });
            if (!result.IsSuccess)
                Console.WriteLine(result.Error?.Message);

        }
        public bool LoginLocal(string userName, string passWord)
        {
            var result = Handlers.ErrorHandler.Handle(() =>
            {
                var user = mainService.VmNguoiDung.GetByUserName(userName).FirstOrDefault();
                if (user != null || ED.DecryptString(user.Password, _defaultKey).Equals(passWord))
                {
                    _userService.AddUserArea(user.Id);
                    _httpContextAccessor.HttpContext?.Session.SetString("Username", userName);
                    _httpContextAccessor.HttpContext?.Session.SetString("UserId", (user.Id).ToString());
                    
                }
                
            });
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.Error?.Message);
                return false;
            }
            // Kiểm tra lại session đã được tạo hay chưa
            if (!IsSessionCreated())
            {
                Console.WriteLine("Session không được tạo thành công. Thử lại.");
                return false;
            }
            return true;
            
                

        }

        public async Task Logout()
        {
            var result = Handlers.ErrorHandler.Handle(() =>
            {
                var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                if (userId != null)
                {
                    _userService.RemoveUserArea(int.Parse(userId));
                    _httpContextAccessor.HttpContext?.Session.Remove("UserId");
                }
                _httpContextAccessor.HttpContext?.Session.Remove("Username");
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
              _httpContextAccessor.HttpContext?.Session.SetString("Username", "");
              _httpContextAccessor.HttpContext?.Session.SetString("UserId", "-1");
              _httpContextAccessor.HttpContext?.Session.SetString("SessionCreated", "1");  
        }
    }
}
