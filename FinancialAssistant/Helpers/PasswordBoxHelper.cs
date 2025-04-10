using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace FinancialAssistant.Helpers
{
    public class PasswordBoxHelper : DependencyObject
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnPasswordChanged));

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged; // Отписываемся от события
                passwordBox.Password = (string)e.NewValue; // Устанавливаем новое значение пароля
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged; // Подписываемся на событие
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetPassword(passwordBox, passwordBox.Password); // Обновляем значение в зависимости от изменения пароля
                Console.WriteLine($"Password changed: {passwordBox.Password}"); // Отладочное сообщение
            }
        }
    }
}
