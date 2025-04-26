using FinancialAssistant.Models;
using FinancialAssistant.Services;
using FinancialAssistant.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinancialAssistant.CustomView
{
    /// <summary>
    /// Логика взаимодействия для EditAccountDialog.xaml
    /// </summary>
    public partial class EditAccountDialog : UserControl
    {
        private readonly DBService _dbService;
        public AccountsViewModel ViewModel { get; private set; }
        public AccountsViewModel ViewModelPopup { get; private set; }

        public EditAccountDialog(
            //Account account, ObservableCollection<Currency> currencies, 
            //ObservableCollection<string> accountTypes, Func<Account, Task> onSave,
            //AccountsViewModel accountsViewModel
            )
        {
            InitializeComponent();
            //ViewModel = new AccountsViewModel(account, currencies, accountTypes, onSave);
            //ViewModelPopup = accountsViewModel;
            //DataContext = ViewModel;
            //_dbService = new();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем диалоговое окно без сохранения изменений
            ViewModelPopup.IsAccountPopupOpen = false; // Устанавливаем результат диалога как "false"
            /*this.Close();*/ // Закрываем окно
        }

        private void LoadCurrencySymbol(string currencyCode)
        {
            var currency = _dbService.GetCurrencyByCode(currencyCode);
            //if (currency != null)
            //{
            //    CurrencySymbolTextBlock.Text = currency.Symbol; // Отображаем символ валюты
            //}
            //else
            //{
            //    CurrencySymbolTextBlock.Text = currencyCode; // Если валюта не найдена, отображаем код
            //}
        }
    }
}
