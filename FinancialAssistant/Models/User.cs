using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class User
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public string Login { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public DateTime Registrationdate { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
