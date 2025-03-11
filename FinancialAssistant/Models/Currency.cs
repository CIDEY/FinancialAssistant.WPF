using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class Currency
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public double Rate { get; set; }

    public string Symbol { get; set; } = null!;
}
