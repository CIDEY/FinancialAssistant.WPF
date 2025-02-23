using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FinancialAssistant
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
