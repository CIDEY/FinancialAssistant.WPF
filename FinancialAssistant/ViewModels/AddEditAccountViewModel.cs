using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Classes;
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
            _dbService = new();

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
            }
        }

        public AddEditAccountViewModel(AccountsViewModel accountsViewModel, int a)
        {
            ViewModel = accountsViewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsAccountPopupOpen = false;
        }

        [RelayCommand]
        private void Cancel()
        {
            ViewModel.IsAccountPopupOpen = false;
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (IsEdit)
                {
                    // Получаем отслеживаемую сущность из контекста
                    var existingAccount = await _dbService.GetAccountByIdAsync(SelectedAccount.Id);

                    // Обновляем свойства
                    existingAccount.Name = Name;
                    existingAccount.CurrencyId = CurrencyId;
                    existingAccount.Type = Type;

                    await _dbService.UpdateAccount(existingAccount);
                    await ViewModel.LoadAccounts();
                }
                else
                {
                    Account account = new()
                    {
                        Name = Name,
                        CurrencyId = CurrencyId,
                        Type = Type,
                        CreatedAt = DateTime.Now,
                        UserId = AppContextSession.CurrentUserId,
                        Balance = 0
                    };

                    await _dbService.AddAccount(account);
                    ViewModel.Accounts.Add(account);
                }

                ViewModel.IsAccountPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания счета: {ex.Message}");
            }
        }
    }
}
