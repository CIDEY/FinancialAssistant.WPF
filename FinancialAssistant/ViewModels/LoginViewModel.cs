using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Classes;
using FinancialAssistant.Services;
using FinancialAssistant.WindowsStart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static FinancialAssistant.Services.DBService;

namespace FinancialAssistant.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        DBService _dBService;
        public LoginViewModel()
        {
            _dBService = new();
        }

        [ObservableProperty]
        private string _login;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [RelayCommand]
        private async Task LoginTo()
        {
            try
            {
                var userId = await _dBService.AuthValidation(Login, Password);

                if (userId == null)
                {
                    MessageBox.Show("Ошибка авторизации");
                    return;
                }
                else
                {
                    AppContextSession.CurrentUserId = userId.Value;

                    var startWindow = new StartWindow();
                    startWindow.Show();

                    // Закрываем окно авторизации
                    Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w is MainWindow)?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task Register()
        {
            // Проверяем, что логин, email и пароль не пустые
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Логин, Email и пароль не могут быть пустыми.");
                return;
            }

            // Проверка на корректность email
            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Некорректный Email.");
                return;
            }

            // Проверка на минимальную длину пароля
            if (Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.");
                return;
            }

            // Проверка на уникальность логина (например, проверка в базе данных)
            if (await _dBService.IsLoginTaken(Login))
            {
                MessageBox.Show("Этот логин уже занят. Пожалуйста, выберите другой.");
                return;
            }

            // Проверка на уникальность email (например, проверка в базе данных)
            if (await _dBService.IsEmailTaken(Email))
            {
                MessageBox.Show("Этот Email уже зарегистрирован. Пожалуйста, используйте другой.");
                return;
            }

            try
            {
                // Регистрация пользователя
                UserRegistrationDto user = new();
                user.Email = Email;
                user.Password = Password;
                user.Login = Login;
                await _dBService.CreateUser(user);
                MessageBox.Show("Регистрация прошла успешно.");
                // Логика после успешной регистрации, например, переход на экран входа
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}");
            }
        }

        private bool IsValidEmail(string email)
        {
            // Простой регулярный выражение для проверки email
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
