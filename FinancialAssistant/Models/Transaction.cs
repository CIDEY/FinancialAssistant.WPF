using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class Transaction
{
    public long Id { get; set; }

    public int AccountId { get; set; }

    public double Amount { get; set; }

    public DateOnly Date { get; set; }

    public string? Description { get; set; }

    public int? Expensecategoryid { get; set; }

    public int? Incomecategoryid { get; set; }

    public string Type { get; set; } = null!;
}
