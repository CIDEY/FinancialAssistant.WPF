using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Goal
    {
        public long Id { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedDate { get; set; }
        public double CurrentProgress { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? Deadline { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;
        public double TargetAmount { get; set; }
        public long UserId { get; set; }

        // Навигационное свойство
        public User User { get; set; }
    }
}
