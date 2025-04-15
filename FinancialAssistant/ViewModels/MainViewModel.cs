using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Classes;
using FinancialAssistant.Services;
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

        private readonly DBService _dbService;
        private bool _currencyRatesUpdated;

        public MainViewModel(long userId)
        {
            CurrentPage = new HomeViewModel(AppContextSession.CurrentUserId);
            _dbService = new();
            UpdateCurrencyRates();
        }

        private async void UpdateCurrencyRates()
        {
            if (_currencyRatesUpdated) return;

            await _dbService.UpdateCurrencyRatesAsync();
            _currencyRatesUpdated = true;
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
                "AccountsView" => new AccountsViewModel(AppContextSession.CurrentUserId),
                _ => CurrentPage
            };
        }
    }
}
