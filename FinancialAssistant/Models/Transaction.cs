using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public decimal Amount { get; set; } // Изменено на decimal
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public TransactionType Type { get; set; } // Enum вместо string
        public long? CategoryId { get; set; }
        public TransactionCategory? Category { get; set; }

        // Навигационные свойства
        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        public IncomeCategory IncomeCategory { get; set; } // Это свойство уже должно быть
    }
}
