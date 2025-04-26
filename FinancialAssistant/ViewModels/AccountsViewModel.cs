using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.CustomView;
using FinancialAssistant.Models;
using FinancialAssistant.Pages;
using FinancialAssistant.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace FinancialAssistant.ViewModels
{
    public partial class AccountsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _type;

        public AccountsViewModel(Account account, ObservableCollection<Currency> currencies)
        {
            Type = account.Type;
            Currencies = currencies;
            CurrentAccount = account;
        }

        public Account CurrentAccount { get; }

        private readonly DBService _dbService;
        private readonly long _userId;

        [ObservableProperty]
        private ObservableCollection<Account> _accounts = new();

        [ObservableProperty]
        private Account _selectedAccount;

        [ObservableProperty]
        private bool _isAccountSelected;

        [ObservableProperty]
        private bool _isAccountPopupOpen;

        [ObservableProperty]
        private UserControl _accountPopupContent;

        [ObservableProperty]
        private ObservableCollection<Currency> _currencies;

        public AccountsViewModel(long userId)
        {
            _userId = userId;
            _dbService = new DBService();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            await LoadAccounts();
            Currencies = new ObservableCollection<Currency>(await _dbService.GetCurrenciesAsync());
        }

        public async Task LoadAccounts()
        {
            try
            {
                var accounts = await _dbService.GetAccountsAsync(_userId);
                Accounts = new ObservableCollection<Account>(accounts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки счетов: {ex.Message}");
            }
        }

        [RelayCommand]
        private void OpenAddAccountPopup()
        {
            var newAccount = new Account { UserId = _userId };
            
            var addEditDialog = new EditAccountDialog();

            addEditDialog.DataContext = 
                new AddEditAccountViewModel(this, false, null);
            
            AccountPopupContent = addEditDialog;
            IsAccountPopupOpen = true;
        }

        [RelayCommand]
        private void OpenEditAccountPopup(Account account)
        {
            if (account == null) return;

            SelectedAccount = account;

            var editDialog = new EditAccountDialog();
            
            editDialog.DataContext =
                new AddEditAccountViewModel(this, false, account);

            AccountPopupContent = editDialog;
            IsAccountPopupOpen = true;
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            if (SelectedAccount == null ||
                MessageBox.Show("Удалить этот счет?", "Подтверждение",
                    MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                await _dbService.DeleteAccount(SelectedAccount.Id);
                Accounts.Remove(SelectedAccount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }

        partial void OnSelectedAccountChanged(Account value) => IsAccountSelected = value != null;
    }
}