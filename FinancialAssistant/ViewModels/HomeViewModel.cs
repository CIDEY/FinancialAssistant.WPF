using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using FinancialAssistant.Services;
using FinancialAssistant.Models;
using System.Transactions;
using static FinancialAssistant.Services.DBService;

namespace FinancialAssistant.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DBService _dbService;
        private readonly long _userId;

        [ObservableProperty]
        private decimal _totalBalance;

        [ObservableProperty]
        private decimal _monthlyIncome;

        [ObservableProperty]
        private decimal _monthlyExpense;

        [ObservableProperty]
        private string _profitIcon = "ArrowTopRight";

        [ObservableProperty]
        private Brush _profitColor = (Brush)new BrushConverter().ConvertFrom("#10B981");

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        public SeriesCollection _seriesCollection;

        [ObservableProperty]
        public string[] _labels;
        public Func<double, string> CurrencyFormatter { get; set; }


        private DateTime _startDate;
        private DateTime _endDate;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                LoadDataAsync(); // Перезагружаем данные при изменении даты
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                LoadDataAsync(); // Перезагружаем данные при изменении даты
            }
        }

        public HomeViewModel(long userId)
        {
            _userId = userId;
            _dbService = new();
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.ExecuteAsync(null);

            StartDate = DateTime.Today.AddYears(-1); // Один год назад
            EndDate = DateTime.Today; // Сегодня

            AddRecordCommand = new AsyncRelayCommand(AddRecordAsync); // Добавляем команду
        }

        public IAsyncRelayCommand AddRecordCommand { get; }

        private async Task AddRecordAsync()
        {
            try
            {
                // Получаем существующие аккаунты
                var accounts = await _dbService.GetAccountsAsync(_userId);
                var accountId = accounts.FirstOrDefault()?.Id; // Получаем первый аккаунт

                if (accountId == null)
                {
                    ErrorMessage = "Нет доступных аккаунтов для добавления транзакции.";
                    return;
                }

                // Создаем новую запись
                var newTransaction = new Models.Transaction
                {
                    AccountId = accountId.Value, // Используем существующий AccountId
                    Amount = 100.00m, // Укажите сумму
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Тестовая запись",
                    Type = TransactionType.Expense // Укажите тип транзакции
                };

                // Сохраняем запись в БД
                await _dbService.AddTransactionAsync(newTransaction);
                // Обновляем данные после добавления
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка добавления записи: {ex.Message}";
            }
        }

        public IAsyncRelayCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var summary = await _dbService.GetFinancialSummary(_userId, DateTime.Today);
                var history = await _dbService.GetYearlyHistory(_userId, StartDate, EndDate);

                //// Получаем финансовую сводку за текущий день
                //var summary = await _dbService.GetFinancialSummary(_userId, DateTime.Today);

                //// Получаем историю за последний год
                //var history = await _dbService.GetYearlyHistory(_userId, DateTime.Today.AddMonths(-12), DateTime.Today);

                TotalBalance = summary.TotalBalance;
                MonthlyIncome = summary.MonthlyIncome;
                MonthlyExpense = summary.MonthlyExpense;

                UpdateProfitIndicator(summary.NetProfit);
                UpdateChart(history);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateProfitIndicator(decimal netProfit)
        {
            NetProfit = $"{netProfit:N2} ₽";
            if (netProfit >= 0)
            {
                ProfitIcon = "ArrowTopRight";
                ProfitColor = (Brush)new BrushConverter().ConvertFrom("#10B981");
            }
            else
            {
                ProfitIcon = "ArrowBottomRight";
                ProfitColor = (Brush)new BrushConverter().ConvertFrom("#EF4444");
            }
        }

        private void UpdateChart(List<MonthlyHistory> history)
        {
            // Создаем список для всех месяцев в диапазоне
            var allMonths = new List<MonthlyHistory>();
            var startYear = 2025;
            var startMonth = 1; // Январь
            var endYear = 2025;
            var endMonth = 4; // Апрель

            for (int year = startYear; year <= endYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    if (year == startYear && month < startMonth) continue; // Пропускаем месяцы до начала
                    if (year == endYear && month > endMonth) continue; // Пропускаем месяцы после конца

                    allMonths.Add(new MonthlyHistory
                    {
                        Year = year,
                        Month = month,
                        Income = history.FirstOrDefault(h => h.Year == year && h.Month == month)?.Income ?? 0,
                        Expense = history.FirstOrDefault(h => h.Year == year && h.Month == month)?.Expense ?? 0
                    });
                }
            }

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Доходы",
                    Values = new ChartValues<decimal>(allMonths.Select(h => h.Income)),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#10B981"),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                },
                new LineSeries
                {
                    Title = "Расходы",
                    Values = new ChartValues<decimal>(allMonths.Select(h => h.Expense)),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#EF4444"),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                }
            };

            Labels = allMonths.Select(h => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(h.Month)} {h.Year}").ToArray();
            CurrencyFormatter = value => value.ToString("N0", CultureInfo.CurrentCulture) + " ₽";
        }

        [ObservableProperty]
        private string _netProfit;
    }
}
