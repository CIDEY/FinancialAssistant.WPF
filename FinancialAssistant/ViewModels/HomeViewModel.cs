using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FinancialAssistant.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private decimal _totalBalance = 284_500.75m;

        [ObservableProperty]
        private decimal _monthlyIncome = 128_300.00m;

        [ObservableProperty]
        private decimal _monthlyExpense = 93_750.25m;

        [ObservableProperty]
        private string _profitIcon = "ArrowTopRight";

        [ObservableProperty]
        private Brush _profitColor = (Brush)new BrushConverter().ConvertFrom("#10B981");

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> CurrencyFormatter { get; set; }
        public Func<double, string> DateFormatter { get; set; }

        public HomeViewModel()
        {
            InitializeChart();
            UpdateProfit();
            InitializeFormatters();
        }

        private void InitializeChart()
        {
            var rnd = new Random();
            var now = DateTime.Now;

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Доходы",
                    Values = new ChartValues<decimal>(),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#10B981"),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                },
                new LineSeries
                {
                    Title = "Расходы",
                    Values = new ChartValues<decimal>(),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#EF4444"),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                }
            };

            var labelsList = new System.Collections.Generic.List<string>();

            for (var i = 11; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                labelsList.Add(date.ToString("MMM yyyy", CultureInfo.CurrentCulture));
                SeriesCollection[0].Values.Add((decimal)(rnd.Next(80_000, 150_000)));
                SeriesCollection[1].Values.Add((decimal)(rnd.Next(50_000, 100_000)));
            }

            Labels = labelsList.ToArray();
        }

        private void UpdateProfit()
        {
            var profit = MonthlyIncome - MonthlyExpense;
            NetProfit = $"{profit:N2} ₽";

            if (profit >= 0)
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

        private void InitializeFormatters()
        {
            CurrencyFormatter = value => $"{value:N0} ₽";
            DateFormatter = value =>
                new DateTime((long)value).ToString("MMM yyyy", CultureInfo.CurrentCulture);
        }

        [ObservableProperty]
        private string _netProfit;
    }
}
