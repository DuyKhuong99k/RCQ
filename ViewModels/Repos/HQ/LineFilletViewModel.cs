using AppModels;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using System.Diagnostics;
using System.Windows;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
namespace ViewModels.Repos.HQ
{
    public partial class LineFilletViewModel : ObservableObject
    {
        private static LineFilletViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private LineFillet? item;
        [ObservableProperty] private ObservableRangeCollection<LineFillet> items = new();
        [ObservableProperty] private ObservableCollection<LineFillet> _lines = new();


        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private LineFilletViewModel()
        {
            try
            {
                // Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static LineFilletViewModel Instance => instance ??= new LineFilletViewModel();

        public List<LineFillet> Get(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.LineFillet(connStr);
                return dao.GetLines(xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LineFillet Get(string lineId, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.LineFillet(connStr);
                return dao.GetById(lineId, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
