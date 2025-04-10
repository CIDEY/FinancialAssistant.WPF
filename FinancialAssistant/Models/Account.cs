using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Account
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }  // Добавляем недостающее свойство

        public long UserId { get; set; }
        public User User { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public List<Transaction> Transactions { get; set; } = new();
    }
}
