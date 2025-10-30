using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vars;

namespace ViewModels.Repos.Hubs.IServices
{

   public  interface IConverterService
   {
       public string ThanhPhamToName(string? id,AppKV kv);
       public string LoaiNguyenLieuToName(string? id);
       public string SizeToName(string? id,AppKV kv);
       public string XuongToName(string? id);
       public string NhanVienToName(string? id);
       public string NhanVienToMaHoSo(string? id);
       public string ChieuXaToName(string? id);
       public string CoiToName(string? id);
       public string AoToName(string? id);
       public string PhuongTienToName(string? id);
        public string KhachHangXepKhuonToName(string? id);
       public string ChatLuongXepKhuonToName(string? id);
       public string NetXepKhuonToName(string? id);
       public string MauToName(string? id, AppKV kv);
       public string CongViecToName(string? id,AppKV kv);
   }
}
