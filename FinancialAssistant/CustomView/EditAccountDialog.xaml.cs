using FinancialAssistant.Models;
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
        public AccountsViewModel ViewModel { get; private set; }

        public EditAccountDialog(Account account, ObservableCollection<Currency> currencies, ObservableCollection<string> accountTypes, Func<Account, Task> onSave)
        {
            InitializeComponent();
            ViewModel = new AccountsViewModel(account, currencies, accountTypes, onSave);
            DataContext = ViewModel;
        }
    }
}
