using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.HQ
{
    public partial class ToKiemViewModel
    {
         private static ToKiemViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static ToKiemViewModel Instance => instance ??= new ToKiemViewModel();
        public List<Models.Repos.Models.ToKiem> Gets(string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.ToKiem();
                return dao.GetKiemsByXuongId(xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
