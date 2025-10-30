using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos;
using Models.Repos.Models;
using MvvmHelpers;
using ViewModels.Repos.Hubs;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ
{
    public partial class MayCanViewModel:ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<MayCan> items = new();
        private  dbPMScontext _context;
        private static MayCanViewModel instance;
        private MayCanViewModel()
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

       
        private void Load()
        {
            try
            {
                var items = _context.MayCans.ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static MayCanViewModel Instance => instance ??= new MayCanViewModel();
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

    }
}
