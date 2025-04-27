using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.CustomView;
using FinancialAssistant.Models;
using FinancialAssistant.Services;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
        private async Task ExportToExcel(Account account)
        {
            // Установите контекст лицензии
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Установите на NonCommercial для бесплатного использования

            if (account == null)
            {
                MessageBox.Show("Счет не выбран.");
                return;
            }

            try
            {
                var transactions = await _dbService.GetTransactionsByAccountIdAsync(account.Id);
                if (transactions == null || transactions.Count == 0)
                {
                    MessageBox.Show("Нет транзакций у этого счета.");
                    return;
                }

                // Определите путь к папке "Excel Reports"
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excel Reports");
                Directory.CreateDirectory(directoryPath); // Создайте папку, если она не существует

                // Создайте новый Excel пакет
                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Transactions");

                    // Заголовки
                    worksheet.Cells["A1"].Value = "Дата";
                    worksheet.Cells["B1"].Value = "Сумма";
                    worksheet.Cells["C1"].Value = "Описание";
                    worksheet.Cells["D1"].Value = "Категория";
                    worksheet.Cells["E1"].Value = "Валюта";

                    int row = 2;
                    foreach (var transaction in transactions)
                    {
                        worksheet.Cells[$"A{row}"].Value = transaction.Date.ToString("dd.MM.yyyy");
                        worksheet.Cells[$"B{row}"].Value = transaction.Amount;
                        worksheet.Cells[$"C{row}"].Value = transaction.Description;
                        worksheet.Cells[$"D{row}"].Value = transaction.Category?.Name ?? string.Empty;
                        worksheet.Cells[$"E{row}"].Value = account.Currency.Symbol;
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Сохраните файл в указанной директории
                    string fileName = $"Transactions_{account.Name}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    string filePath = Path.Combine(directoryPath, fileName);
                    package.SaveAs(new FileInfo(filePath));

                    // Открытие файла
                    System.Diagnostics.Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

                    MessageBox.Show("Экспорт завершен успешно! Файл сохранен в " + filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
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

        partial void OnSelectedAccountChanged(Account value) => IsAccountSelected = value != null;
    }
}