using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace Models.Repos.A_Model
{
    public partial class CoiTamChiTiet : ObservableObject
    {
        [ObservableProperty] private string maCoi = null!;
        [ObservableProperty] private string? displayName;
        [ObservableProperty] private double trongLuongMax;
        [ObservableProperty] private string ten;
        [ObservableProperty] private string maXuong;
        [ObservableProperty] private string? maLo;
        [ObservableProperty] private string? maThanhPham;
        [ObservableProperty] private string? maSize;
        [ObservableProperty] private string? maChieuXa;
        [ObservableProperty] private bool? isTaiChe;
        [ObservableProperty] private string? maCoiChinh;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Count))]
        [NotifyPropertyChangedFor(nameof(Sum))]
        private ObservableRangeCollection<PhieuCanChinhXepKhuon> items = new();

        public int Count => Items.Count;
        public decimal Sum => Items.Sum(x => x.TrongLuong);
        private readonly object lockObj = new();
        public void AddItem(PhieuCanChinhXepKhuon item)
        {
            lock (lockObj)
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(PhieuCanChinhXepKhuon item)
        {
            lock (lockObj)
            {
                Items.Remove(item);
            }
        }
        public void ClearItems()
        {
            lock (lockObj)
            {
                Items.Clear();
            }
        }
        public void AddItems(IEnumerable<PhieuCanChinhXepKhuon> items)
        {
            lock (lockObj)
            {
                Items.AddRange(items);
            }
        }
        public void RemoveItems(IEnumerable<PhieuCanChinhXepKhuon> items)
        {
            lock (lockObj)
            {
                foreach (var item in items)
                {
                    Items.Remove(item);
                }
            }
        }
    }
}
