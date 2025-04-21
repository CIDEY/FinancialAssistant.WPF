using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FinancialAssistant.Converters
{
    public class TransactionTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TransactionType type
                ? type == TransactionType.Income
                    ? new SolidColorBrush(Color.FromRgb(59, 130, 246)) // Blue 
                    : new SolidColorBrush(Color.FromRgb(239, 68, 68)) // Red
                : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
