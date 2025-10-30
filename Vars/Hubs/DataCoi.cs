using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class DataCoi
    {
        public string CoiId {get; set; }
        public bool IsRun {get; set; }
        public string XuongId {get; set; }
        public string? MaChatLuong {get; set; }
        public DateTime? ThoiGianBatDauQuay {get; set; }
        public string MayQuay {get; set; }
        public int? ThoiGianQuay { get; set; }
        public bool Forced { get; set; } = false;
        public DateTime? ThoiGianRaCoi { get; set; }
        public bool IsUpdateUI { get; set; } = false;
       
        /// <summary>
        /// Item1: Ten Coi
        /// Item2: Ten May Can
        /// </summary>
        public List<Tuple <string,string>> CoiTamInfos { get; set; }
    }
}
