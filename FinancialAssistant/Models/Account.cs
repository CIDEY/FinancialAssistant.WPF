using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class Account
{
    public long Id { get; set; }

    public double Balance { get; set; }

    public int Currencyid { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public long Userid { get; set; }
}
