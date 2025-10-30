using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Security.Crypt;
using System.Reflection;
namespace AppViewModels
{
    public class DecryptJsonString
    {
        private static DecryptJsonString _instance;
        public class WebAppSettingModel
        {
            public string ComName { get; set; }
            public string ApiHostUrl { get; set; }
            public string ApiWssHostUrl { get; set; }
            public string HostUrl { get; set; }
            public string HubMayCanCODE { get; set; }
        }
        
        public WebAppSettingModel GetJsonDataWebAppSetting()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "WebAppSetting.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var jsonData = System.IO.File.ReadAllText(filePath);

            //Giai ma chuoi json string
            var key = AppViewModels.AppViewModel.Instance.DefaultKey;
            var decryptJson = "";
            if (jsonData != null)
            {
                decryptJson = ED.DecryptString(jsonData, key);
            }
            // Deserialize dữ liệu thành đối tượng WebAppSettingModel
            var jsonDone = JsonConvert.DeserializeObject<WebAppSettingModel>(decryptJson);

            return jsonDone;
        }
        public class DatabaseConnectSettingModel
        {
            public string Db { get; set; }
            public string Pass { get; set; }
            public string ReaderPort { get; set; }
            public string ServerName { get; set; }
            public int TimeOut { get; set; }
            public string Usr { get; set; }
        }
        public DatabaseConnectSettingModel GetJsonDatabaseConnectSetting()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "DatabaseConnectSetting.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var jsonData = System.IO.File.ReadAllText(filePath);

            //Giai ma chuoi json string
            //var key = AppViewModels.AppViewModel.Instance.DefaultKey;
            var decryptJson = "";
            if (jsonData != null)
            {
                decryptJson = ED.DecryptString(jsonData);
            }
            // Deserialize dữ liệu thành đối tượng WebAppSettingModel
            var jsonDone = JsonConvert.DeserializeObject<DatabaseConnectSettingModel>(decryptJson);

            return jsonDone;
        }
        public static DecryptJsonString Instance => _instance ??= new DecryptJsonString();
    }
}
