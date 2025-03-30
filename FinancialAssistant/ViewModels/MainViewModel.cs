using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentPage;

        public MainViewModel()
        {
            CurrentPage = new HomeViewModel();
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            CurrentPage = destination switch
            {
                "HomeView" => new HomeViewModel(),
                "TransactionsView" => new TransactionsViewModel(),
                "ReportsView" => new ReportsViewModel(),
                "SettingsView" => new SettingsViewModel(),
                _ => CurrentPage
            };
        }
    }
}
