using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class IncomeCategory
    {
        public long Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;
        public long UserId { get; set; }

        // Навигационное свойство
        public User User { get; set; }

        // Добавьте это свойство
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
