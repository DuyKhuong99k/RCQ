using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Repos.Models;
using MvvmHelpers;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices
{
    public partial interface IMayCansService: INotifyPropertyChanged
    {
        public ObservableRangeCollection<MayCan> Items { get; set; }
        public List<MayCan> SelectedItems { get; set; }
        public MayCan? Find(string id);
        public MayCan? Find(string id, bool isConnected);
        public void AddItem(MayCan mayCan, object item);
        public void Remove(string id);
        public void Load();
        public void Dis(string id);
        public void DisConnectionId(string id);
        public Task ExpandHandler(MayCan mayCan);
        public void SetAllExpand(bool isExpanded);
        public void SetIsConnected(bool isConnected,string id);
        public void Update(MayCan mayCan);
        public event EventHandler? ItemChanged;
        public void NotifyItemChanged();
        public Task<bool> CommandZero(string id);
        public Task<bool> CommandSetNapThe(string id,bool isNapThe);
        public Task<bool> CommandTare(string id,decimal? trongLuongTare);
        public Task<bool> CommandGetLo(string id);
        public Task<bool> CommandSetLo(string id, string maLo);
        public Task<bool> CommandGetThanhPham(string id);
        public Task<bool> CommandSetThanhPham(string id, string thanhPhamId, string thanhPhamName);
        public Task<bool> CommandGetSize(string id);
        public Task<bool> CommandSetSize(string id, string sizeId,string sizeName);
        public Task<bool> CommandSetNhanVien(string id,string nhanVienId,string maHoSo,string nhanVienName);
        public Task<bool> CommandGetNhanVien(string id);
        public Task<bool> CommandSetWaiting(string id);
        public Task<bool> CommandSetUpdateTime(string id);
        public Task<bool> CommandSendMess(string id, string mess);
        public Task<bool> CommandSendMessageByConnectionId(string id, string MessStr);
        public Task<bool> CommandSetChieuXa(string id, string chieuXaId, string chieuXaName);
        public Task<bool> CommandSetChatLuong(string id, string chatLuongId, string chatLuongName);
        public Task<bool> CommandGetChatLuong(string id);
        public Task<bool> CommandGetChieuXa(string id);
        public Task<bool> CommandSetInfoCoi(string id, string dataCoiJson);

    }
}
