using FinancialAssistant.Models;
using System;
using System.Collections.Generic;
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

namespace FinancialAssistant.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccountsView.xaml
    /// </summary>
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {
            InitializeComponent();
        }

        private void ViewAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Account account)
            {
                //  TODO: Вызвать команду просмотра счета из ViewModel
                //  Пример:
                //  if (DataContext is AccountsViewModel viewModel)
                //  {
                //      viewModel.ViewAccountCommand.Execute(account);
                //  }
            }
        }

        private void Transfer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Account account)
            {
                //  TODO: Вызвать команду перевода из ViewModel
                //  Пример:
                //  if (DataContext is AccountsViewModel viewModel)
                //  {
                //      viewModel.TransferCommand.Execute(account);
                //  }
            }
        }

        // Добавьте это свойство
        public bool IsAccountPopupOpen
        {
            get { return (bool)GetValue(IsAccountPopupOpenProperty); }
            set { SetValue(IsAccountPopupOpenProperty, value); }
        }

        // Используйте DependencyProperty для поддержки привязки данных
        public static readonly DependencyProperty IsAccountPopupOpenProperty =
            DependencyProperty.Register("IsAccountPopupOpen", typeof(bool), typeof(AccountsView), new PropertyMetadata(false));
    }
}
