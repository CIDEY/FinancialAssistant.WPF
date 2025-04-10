using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; } // Например: USD, EUR
        public string Symbol { get; set; } // Например: $, €
        public decimal Rate { get; set; } // Изменяем с double на decimal

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
