using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class TransactionCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public TransactionType Type { get; set; }

        public List<Transaction> Transactions { get; set; } = new();
    }
}
