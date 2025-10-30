using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Repos.Models;
using MvvmHelpers;
using Vars;
using Vars.Hubs;

namespace ViewModels.Repos.Hubs.IServices
{
    public partial interface IMayCansService : INotifyPropertyChanged
    {
        public ConcurrentDictionary<string, MayCan> Items { get; set; }
        public List<MayCan> SelectedItems { get; set; }
        public MayCan? Find(string id);
        public MayCan? Find(string id, bool isConnected);
        public void AddItem(MayCan mayCan, object item);
        public void AddorUpdate(MayCan mayCan);
        public void Remove(string id);
        public void Load();
        public void Dis(string id);
        public void DisConnectionId(string id);
        public Task ExpandHandler(MayCan mayCan);
        public void SetAllExpand(bool isExpanded);

        public void SetIsConnected(bool isConnected, string id);

        // public void Update(MayCan mayCan);
        // public event EventHandler? ItemChanged;
        public event Action<MayCan>? ItemAdded;
        public event Action<MayCan>? ItemUpdated;
        public event Action<string>? ItemRemoved;
        public void NotifyItemChanged(MayCan mayCan);
        public Task<bool> CommandZero(string id);
        /// <summary>
        /// Màu xanh lá
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> CommandSUCCESS(string id);
        /// <summary>
        /// Màu Cam
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> CommandSUCCESS2(string id);
        /// <summary>
        /// Màu xanh dương
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> CommandSUCCESS3(string id);
        /// <summary>
        /// Màu tím
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> CommandSUCCESS4(string id);
        public Task<bool> CommandSetNapThe(string id, bool isNapThe);
        public Task<bool> CommandTare(string id, decimal? trongLuongTare);
        public Task<bool> CommandGetLo(string id);
        public Task<bool> CommandSetLo(string id, string maLo);
        public Task<bool> CommandGetThanhPham(string id);
        public Task<bool> CommandSetThanhPham(string id, string thanhPhamId, string thanhPhamName);
        public Task<bool> CommandGetNguyenLieu(string id);
        public Task<bool> CommandSetNguyenLieu(string id, string nguyenLieuId, string nguyenLieuName);
        public Task<bool> CommandGetSize(string id);
        public Task<bool> CommandSetSize(string id, string sizeId, string sizeName);
        public Task<bool> CommandSetNhanVien(string id, string nhanVienId, string maHoSo, string nhanVienName);
        public Task<bool> CommandSetNhanVienPhucVu(string id, string nhanVienId, string maHoSo);
        public Task<bool> CommandGetNhanVien(string id);
        public Task<bool> CommandSetWaiting(string id);
        public Task<bool> CommandSetUpdateTime(string id);
        public Task<bool> CommandSendMess(string id, string mess);
        public Task<bool> CommandSendMessageByConnectionId(string id, string MessStr);
        public Task<bool> CommandSetChieuXa(string id, string chieuXaId, string chieuXaName);
        public Task<bool> CommandSetChatLuong(string id, string chatLuongId, string chatLuongName);
        public Task<bool> CommandGetChatLuong(string id);
        public Task<bool> CommandGetChieuXa(string id);
        public Task<bool> CommandGetCoi(string id);
        public Task<bool> CommandSetCoi(string id, string coiId, string coiName);
        public Task<bool> CommandGetAo(string id);
        public Task<bool> CommandSetAo(string id, string aoId, string aoName);
        public Task<bool> CommandGetPhuongTien(string id);
        public Task<bool> CommandSetPhuongTien(string id, string phuongTienId, string phuongTienName);
        public Task<bool> CommandSetKhachHang(string id, string khachHangId, string khachHangName);
        public Task<bool> CommandSetNet(string id, string netId,string netName);
        public Task<bool> CommandSetMau(string id, string mauId,string mauName);
        public Task<bool> CommandSetCongViec(string id, string congViecId, string congViecName);
        public Task<bool> CommandSetInfoCoi(string id, string dataCoiJson);
        public Task<bool> CommandBtnpTare(string id);
        public Task<bool> CommandBtnpZero(string id);
        public Task<bool> CommandBtnpMode(string id);

        public Task<bool> CommandBtnpEnter(string id);

        public Task<bool> CommandBtnpc(string id);

        public Task<bool> CommandBtnpRestart(string id);

        public Task<bool> CommandBtnpRelease(string id);
        public Task<bool> Commandrestartsystem(string id);
        public Task<bool> CommandGetDeviceInfos(string id);
        public Task<bool> CommandPhieuCanSync(string id, DateTime dateTime);
        public Task<bool> CommandGetPhieuCanCountStatus0(string id, DateTime dateTime);
        public Task<bool> CommandDoiKhuVuc(string id, AppKV khuVuc);
        public void Update(string id, AppKV wKv, AppType aType, string xuongId);
    }
}