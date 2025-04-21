using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Models;
using FinancialAssistant.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.ViewModels
{
    public partial class AddTransactionDialogViewModel : ObservableObject
    {
        private readonly DBService _dbService;

        public TransactionsViewModel ViewModel { get; private set; }
        public TransactionsViewModel ViewModelPopup { get; private set; }

        [ObservableProperty]
        private ObservableCollection<Account> _accountList;

        [ObservableProperty]
        private Account _selectedAccount; // Теперь хранит целый объект Account

        [ObservableProperty]
        private ObservableCollection<string> _transactionTypes;

        [ObservableProperty]
        private string _selectedTransactionTypes;

        [ObservableProperty]
        private decimal _amount;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        [ObservableProperty]
        private string _descriptionText;

        public AddTransactionDialogViewModel(TransactionsViewModel transactionsViewModel, List<Account> accounts)
        {
            AccountList = new ObservableCollection<Account>(accounts);
            TransactionTypes = new ObservableCollection<string>()
            {
                "Снятие",
                "Пополнение"
            };
            //TransactionTypes =
            //ViewModel = new TransactionsViewModel();
            ViewModelPopup = transactionsViewModel;
            _dbService = new();
            //TakeAccounts();
        }

        [RelayCommand]
        private void Cancel()
        {
            ViewModelPopup.IsAddTransactionPopupOpen = false;
        }

        [RelayCommand]
        private async Task Save()
        {
            int selectedCategory;

            if (SelectedTransactionTypes == "Снятие")
                selectedCategory = 0;
            else
                selectedCategory = 1;

            Transaction transaction = new()
            {
                AccountId = SelectedAccount.Id,
                Amount = Amount,
                Date = DateOnly.FromDateTime(SelectedDate),
                Description = DescriptionText,
                CategoryId = selectedCategory
            };

            await _dbService.AddTransactionAsync(transaction);


        }
    }
}