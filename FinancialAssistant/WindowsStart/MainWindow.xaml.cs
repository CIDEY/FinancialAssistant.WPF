using FinancialAssistant.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinancialAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                var viewModel = DataContext as LoginViewModel;
                if (viewModel != null)
                {
                    viewModel.Password = passwordBox.Password; // Обновляем свойство Password в ViewModel
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            AnimateTransition(LoginPanel, RegisterPanel);
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            AnimateTransition(RegisterPanel, LoginPanel);
        }

        private void AnimateTransition(UIElement hide, UIElement show)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            fadeOut.Completed += (s, e) =>
            {
                hide.Visibility = Visibility.Collapsed;
                show.Visibility = Visibility.Visible;
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
                show.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };
            hide.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }
    }
}