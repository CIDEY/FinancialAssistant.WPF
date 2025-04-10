using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class User
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Login { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        public string PasswordHash { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public List<UserRole> UserRoles { get; set; } = new();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public ICollection<IncomeCategory> IncomeCategories { get; set; } = new List<IncomeCategory>();
    }
}
