using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Classes;
using FinancialAssistant.CustomView;
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
using System.Windows.Controls;

namespace FinancialAssistant.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isAddTransactionPopupOpen; // Исправлено Transction → Transaction

        [ObservableProperty]
        private UserControl _addTransactionPopupContent; // Исправлено Transction → Transaction

        private readonly DBService _dbService;
        private readonly long _userId;

        [ObservableProperty]
        private ObservableCollection<Transaction> _transactionList;

        public TransactionsViewModel(long userId)
        {
            _dbService = new DBService();
            _userId = userId;
            LoadTransaction();
        }

        [RelayCommand]
        private async void AddTransactionPopup() // Удален параметр Transaction
        {
            // Получаем аккаунты асинхронно
            List<Account> accounts = await _dbService.GetAccountsAsync(AppContextSession.CurrentUserId);

            //List<TransactionCategory> = await _db

            // Создаем диалоговое окно после получения аккаунтов
            var editDialog = new AddTransactionDialog();
            editDialog.DataContext = new AddTransactionDialogViewModel(this, accounts);

            // Обновляем UI
            AddTransactionPopupContent = editDialog;
            IsAddTransactionPopupOpen = true;
        }

        public async Task LoadTransaction()
        {
            try
            {
                var accounts = await _dbService.GetTransactionsByPeriod(_userId, DateOnly.MinValue, DateOnly.MaxValue);
                TransactionList = new ObservableCollection<Transaction>(accounts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки транзакций: {ex.Message}");
            }
        }
    }
}
