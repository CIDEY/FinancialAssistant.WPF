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
        private TransactionType? _selectedFilterType = null;

        public List<TransactionType?> FilterTypes { get; } = new() { null, TransactionType.Income, TransactionType.Expense };

        private ObservableCollection<Transaction> _allTransactions;

        [ObservableProperty]
        private bool _isAddTransactionPopupOpen; 

        [ObservableProperty]
        private UserControl _addTransactionPopupContent; 

        private readonly DBService _dbService;
        private readonly long _userId;

        [ObservableProperty]
        private ObservableCollection<Transaction> _transactionList;

        public TransactionsViewModel(long userId)
        {
            _dbService = new DBService();
            _userId = userId;
            SelectedFilterType = null;
            LoadTransactions();
        }

        partial void OnSelectedFilterTypeChanged(TransactionType? value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allTransactions == null) return;

            var filtered = SelectedFilterType == null
                ? _allTransactions
                : _allTransactions.Where(t => t.Type == SelectedFilterType);

            TransactionList = new ObservableCollection<Transaction>(filtered);
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

        public async Task LoadTransactions()
        {
            //try
            //{
            //    var accounts = await _dbService.GetTransactionsByPeriod(_userId, DateOnly.MinValue, DateOnly.MaxValue);
            //    TransactionList = new ObservableCollection<Transaction>(accounts);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка загрузки транзакций: {ex.Message}");
            //}

            try
            {
                var transactions = await _dbService.GetTransactionsByPeriod(_userId, DateOnly.MinValue, DateOnly.MaxValue);
                _allTransactions = new ObservableCollection<Transaction>(transactions);
                ApplyFilter(); // Применяем фильтр после загрузки
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}
