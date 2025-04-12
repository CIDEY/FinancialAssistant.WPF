using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Models;
using FinancialAssistant.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using FinancialAssistant.CustomView;

namespace FinancialAssistant.ViewModels
{
    public partial class AccountsViewModel : ObservableObject
    {
        private readonly DBService _dbService;
        private readonly long _userId;

        [ObservableProperty]
        private ObservableCollection<Account> _accounts = new();

        [ObservableProperty]
        private Account _selectedAccount;

        [ObservableProperty]
        private bool _isAccountSelected;

        public ICommand AddAccountCommand { get; }
        public ICommand EditAccountCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        public AccountsViewModel(long userId)
        {
            _userId = userId;
            _dbService = new DBService();

            AddAccountCommand = new RelayCommand(AddAccount);
            EditAccountCommand = new RelayCommand(EditAccount);
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
            LoadAccounts();
        }

        private async void LoadAccounts()
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

        private async void AddAccount()
        {
            var newAccount = new Account
            {
                Name = "Новый счет",
                Balance = 0,
                Type = "Основной",
                UserId = _userId,
                CurrencyId = 1 // Default currency
            };

            try
            {
                await _dbService.AddAccount(newAccount);
                Accounts.Add(newAccount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания счета: {ex.Message}");
            }
        }

        private async void EditAccount()
        {
            if (SelectedAccount == null) return;

            // Реализация диалога редактирования
            //var dialog = new EditAccountDialog(SelectedAccount);
            //var result = await DialogHost.Show(dialog, "RootDialog");

            //if (result is true)
            //{
            //    try
            //    {
            //        await _dbService.UpdateAccount(SelectedAccount);
            //        LoadAccounts(); // Обновляем список
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Ошибка обновления: {ex.Message}");
            //    }
            //}
        }

        private async void DeleteAccount()
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
    }
}
