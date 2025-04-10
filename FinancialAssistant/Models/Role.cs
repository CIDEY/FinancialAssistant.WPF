using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Models
{
    public class Role
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }  // Добавляем отсутствующее свойство

        public List<UserRole> UserRoles { get; set; } = new();
    }
}
