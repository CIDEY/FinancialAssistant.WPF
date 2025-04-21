using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Classes;
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
    /// Логика взаимодействия для AddTransactionDialog.xaml
    /// </summary>
    public partial class AddTransactionDialog : UserControl
    {
        //private readonly DBService _dbService;
        //public TransactionsViewModel ViewModel { get; private set; }
        //public TransactionsViewModel ViewModelPopup { get; private set; }
        //public ObservableCollection<Account> AccountList { get; set; }
        //public Account SelectedAccount { get; set; }

        //public AddTransactionDialog()
        //{
        //    InitializeComponent();
        //}

        public AddTransactionDialog(/*TransactionsViewModel transactionsViewModel*//*, List<Account> accounts*/)
        {
            InitializeComponent();
        }

    }
}
