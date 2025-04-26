using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Models;
using FinancialAssistant.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinancialAssistant.ViewModels
{
    public partial class AddEditAccountViewModel : ObservableObject
    {
        private readonly DBService _dbService;
        public AccountsViewModel ViewModel { get; private set; }

        [ObservableProperty]
        private bool _isEdit;

        [ObservableProperty]
        private string _type;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _currencyId;

        [ObservableProperty]
        private ObservableCollection<string> _accountTypes;

        [ObservableProperty]
        private ObservableCollection<Currency> _currencies;

        [ObservableProperty]
        private Account _selectedAccount;
        
        public AddEditAccountViewModel(AccountsViewModel accountsViewModel, bool isEdit, Account selectedAccount)
        {
            ViewModel = accountsViewModel;
            IsEdit = selectedAccount != null ? true : false;

            SelectedAccount = selectedAccount;

            Currencies = accountsViewModel.Currencies;

            AccountTypes = new ObservableCollection<string>
            {
                "Основной", "Сберегательный", "Кредитный"
            };

            if (IsEdit)
            {
                Name = selectedAccount.Name;
                CurrencyId = selectedAccount.CurrencyId;
                Type = selectedAccount.Type;
                //CreatedAt
            }
        }

        public AddEditAccountViewModel(AccountsViewModel accountsViewModel, int a)
        {
            ViewModel = accountsViewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsAccountPopupOpen = false; // Устанавливаем результат диалога как "false"
        }

        [RelayCommand]
        private void Cancel()
        {
            ViewModel.IsAccountPopupOpen = false;
        }

        //CurrentAccount.Name = Name;
        //    CurrentAccount.CurrencyId = CurrencyId;
        //    CurrentAccount.Type = Type;

        [RelayCommand]
        private async Task Save()
        {
            if (IsEdit)
                await SaveEditedAccount(SelectedAccount, ViewModel);
            else
                await SaveNewAccount(SelectedAccount, ViewModel);
        }

        //[RelayCommand]
        private async Task SaveNewAccount(Account account, AccountsViewModel accountsViewModel)
        {
            try
            {
                await _dbService.AddAccount(account);
                accountsViewModel.Accounts.Add(account);
                accountsViewModel.IsAccountPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания счета: {ex.Message}");
            }
        }

        //[RelayCommand]
        private async Task SaveEditedAccount(Account account, AccountsViewModel accountsViewModel)
        {
            try
            {
                await _dbService.UpdateAccount(account);
                await accountsViewModel.LoadAccounts(); // Обновляем список
                accountsViewModel.IsAccountPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления счета: {ex.Message}");
            }
        }
    }
}
