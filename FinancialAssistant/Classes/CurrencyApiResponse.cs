using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Classes
{
    public class CurrencyApiResponse
    {
        public Dictionary<string, CurrencyInfo> Valute { get; set; }
    }
}
