using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Services;
using FinancialAssistant.WindowsStart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

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
            // Проверяем, что логин и пароль не пустые
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Логин или пароль пустые.");
                return;
            }

            try
            {
                // Асинхронно проверяем аутентификацию
                bool isAuthenticated = await _dBService.AuthValidation(Login, Password);

                if (isAuthenticated)
                {
                    MessageBox.Show("Авторизация произошла успешно.");

                    StartWindow startWindow = new StartWindow();
                    startWindow.Show();
                    // Логика после успешной аутентификации
                    // Например, переход на другой экран или уведомление пользователя
                }
                else
                {
                    MessageBox.Show("Авторизация произошла не удачно.");
                    // Логика при неудачной аутентификации
                    // Например, установка сообщения об ошибке
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
                // Например, логирование ошибки или уведомление пользователя
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
                await _dBService.CreateUser(Login, Email, Password);
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
