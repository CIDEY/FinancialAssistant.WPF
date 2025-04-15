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
        private readonly Func<Account, Task> _onSave;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _currencyId;

        [ObservableProperty]
        private string _type;

        public ObservableCollection<Currency> Currencies { get; }
        public ObservableCollection<string> AccountTypes { get; }

        public AccountsViewModel(Account account, ObservableCollection<Currency> currencies,
            ObservableCollection<string> accountTypes, Func<Account, Task> onSave)
        {
            _onSave = onSave;
            Name = account.Name;
            CurrencyId = account.CurrencyId;
            Type = account.Type;
            Currencies = currencies;
            AccountTypes = accountTypes;
            CurrentAccount = account;
        }

        public Account CurrentAccount { get; }

        [RelayCommand]
        private async Task Save()
        {
            CurrentAccount.Name = Name;
            CurrentAccount.CurrencyId = CurrencyId;
            CurrentAccount.Type = Type;
            await _onSave?.Invoke(CurrentAccount);
        }

        [RelayCommand]
        private void Cancel()
        {
            // Ничего не делаем, IsAccountPopupOpen во AccountsView закроет Popup
            if (Application.Current.MainWindow.FindName("accountsView") is AccountsView accountsView)
            {
                accountsView.IsAccountPopupOpen = false;
            }
        }

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

        private ObservableCollection<Currency> _currencies;
        private ObservableCollection<string> _accountTypes = new ObservableCollection<string> { "Основной", "Сберегательный", "Кредитный" };

        public AccountsViewModel(long userId)
        {
            _userId = userId;
            _dbService = new DBService();
            LoadData();
        }

        private async Task LoadData()
        {
            await LoadAccounts();
            _currencies = new ObservableCollection<Currency>(await _dbService.GetCurrenciesAsync());
        }

        private async Task LoadAccounts()
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
            var addEditDialog = new EditAccountDialog(newAccount, _currencies, _accountTypes, SaveNewAccount);
            AccountPopupContent = addEditDialog;
            IsAccountPopupOpen = true;
        }

        [RelayCommand]
        private void OpenEditAccountPopup(Account account)
        {
            if (account == null) return;
            var editDialog = new EditAccountDialog(account, _currencies, _accountTypes, SaveEditedAccount);
            AccountPopupContent = editDialog;
            IsAccountPopupOpen = true;
        }

        private async Task SaveNewAccount(Account account)
        {
            try
            {
                await _dbService.AddAccount(account);
                Accounts.Add(account);
                IsAccountPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания счета: {ex.Message}");
            }
        }

        private async Task SaveEditedAccount(Account account)
        {
            try
            {
                await _dbService.UpdateAccount(account);
                await LoadAccounts(); // Обновляем список
                IsAccountPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления счета: {ex.Message}");
            }
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

        partial void OnSelectedAccountChanged(Account value)
        {
            IsAccountSelected = value != null;
        }
    }
}