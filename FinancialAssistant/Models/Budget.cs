using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class Budget
{
    public long Id { get; set; }

    public double Amount { get; set; }

    public double Currentprogress { get; set; }

    public DateTime? Enddate { get; set; }

    public int Expensecategoryid { get; set; }

    public string Period { get; set; } = null!;

    public DateTime Startdate { get; set; }

    public long Userid { get; set; }
}
