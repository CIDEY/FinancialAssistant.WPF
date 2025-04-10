using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Goal
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public double CurrentProgress { get; set; }
        public string? Deadline { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;
        public double TargetAmount { get; set; }
        public long UserId { get; set; }

        // Навигационное свойство
        public User User { get; set; }
    }
}
