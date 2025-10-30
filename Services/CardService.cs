using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Services
{
    public partial class CardService(IMainService mainService) : ObservableObject, ICardService
    {
        [ObservableProperty]
        private ObservableRangeCollection<TheTu> itemsByNhanVien = new();
        [ObservableProperty]
        private ObservableRangeCollection<TheThanhPham> itemsByChucNang = new();
        [ObservableProperty]
        private ObservableRangeCollection<TheRo> itemsByMau = new();
        [ObservableProperty]
        private TheTu itemNhanVien;
        [ObservableProperty]
        private TheRo itemRo;
        [ObservableProperty]
        private TheThanhPham itemChucNang ;
        public void LoadByNhanVien(string nhanVienId)
        {
            try
            {
                lock (ItemsByNhanVien)
                {
                    ItemsByNhanVien.Clear();
                }
                var items = mainService.VmThe.Items.Where(x => x.MaNhanVien == nhanVienId).ToList();
                if (items.Any())
                {
                   ItemsByNhanVien.AddRange(items);
                }   

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void LoadByColorCode(string colorCode)
        {
            try
            {
                lock (ItemsByMau)
                {
                    ItemsByMau.Clear();
                }

                var items = mainService.VmTheRo.Items.Where(x => x.ColorCode == colorCode).ToList();
                if (items.Any())
                {
                    ItemsByMau.AddRange(items);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }   
        public string Find(string theId)
        {
            try
            {
                var item = mainService.VmThe.Items.FirstOrDefault(x => x.MaTheTu == theId);
                if (item != null)
                {
                    return
                        $"{theId} Mã: {item.MaNhanVien} - Mã Số: {item.MaSo} - Tên: {item.NhanVienName}";
                }
                var itemthanhpham = mainService.VmTheThanhPham.Items.FirstOrDefault(x => x.MaThe == theId);
                if (itemthanhpham != null)
                {
                    return
                        $"Thẻ Chức Năng {theId}";
                }
                var itemro = mainService.VmTheRo.Items.FirstOrDefault(x => x.MaThe == theId);
                if (itemro != null)
                {
                    return
                        $"Thẻ Rổ {theId}";
                }

                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
