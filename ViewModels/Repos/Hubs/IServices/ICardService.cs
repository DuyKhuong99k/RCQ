using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Repos.Models;
using MvvmHelpers;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface ICardService
    {
        public ObservableRangeCollection<TheTu> ItemsByNhanVien { get; set; }
        public ObservableRangeCollection<TheThanhPham> ItemsByChucNang { get; set; } 
        public ObservableRangeCollection<TheRo> ItemsByMau { get; set; }
        public TheTu ItemNhanVien { get; set; }
        public TheRo ItemRo { get; set; }
        public TheThanhPham ItemChucNang { get; set; }
        public void LoadByNhanVien(string nhanVienId);
        public void LoadByColorCode(string colorCode);
        public string Find(string theId);
    }
}
