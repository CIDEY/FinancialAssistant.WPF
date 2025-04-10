using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace FinancialAssistant.Selectors
{
    public class PageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HomeTemplate { get; set; }
        public DataTemplate TransactionsTemplate { get; set; }
        public DataTemplate ReportsTemplate { get; set; }
        public DataTemplate SettingsTemplate { get; set; }
        public DataTemplate LoginTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            var viewName = item.GetType().Name;
            return viewName switch
            {
                "HomeViewModel" => HomeTemplate,
                "TransactionsViewModel" => TransactionsTemplate,
                "ReportsViewModel" => ReportsTemplate,
                "SettingsViewModel" => SettingsTemplate,
                
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
