using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Budget
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public double CurrentProgress { get; set; }
        public DateTime? EndDate { get; set; }
        public string Period { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public long UserId { get; set; }

        // Навигационные свойства
        public User User { get; set; }
    }
}
