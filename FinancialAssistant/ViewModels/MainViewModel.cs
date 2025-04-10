using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Classes;
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

        public MainViewModel(long userId)
        {
            CurrentPage = new HomeViewModel(AppContextSession.CurrentUserId);
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            CurrentPage = destination switch
            {
                "HomeView" => new HomeViewModel(AppContextSession.CurrentUserId),
                "TransactionsView" => new TransactionsViewModel(/*_currentUserId*/),
                "ReportsView" => new ReportsViewModel(/*_currentUserId*/),
                "SettingsView" => new SettingsViewModel(/*_currentUserId*/),
                _ => CurrentPage
            };
        }
    }
}
