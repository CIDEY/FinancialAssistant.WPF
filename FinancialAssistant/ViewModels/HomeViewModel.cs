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

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> CurrencyFormatter { get; set; }

        public HomeViewModel(long userId)
        {
            _userId = userId;
            _dbService = new();
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.ExecuteAsync(null);
        }

        public IAsyncRelayCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var summary = await _dbService.GetFinancialSummary(_userId, DateTime.Today);
                var history = await _dbService.GetYearlyHistory(_userId);

                TotalBalance = summary.TotalBalance; // Убрано приведение типов
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

        private void UpdateChart(List<DBService.MonthlyHistory> history)
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Доходы",
                    Values = new ChartValues<decimal>(history.Select(h => (decimal)h.Income)),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#10B981"),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                },
                new LineSeries
                {
                    Title = "Расходы",
                    Values = new ChartValues<decimal>(history.Select(h => (decimal)h.Expense)),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#EF4444"),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                }
            };

            Labels = history.Select(h =>
                CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(h.Month))
                .ToArray();

            CurrencyFormatter = value => $"{value:N0} ₽";
        }

        [ObservableProperty]
        private string _netProfit;
    }
}
