using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class Goal
{
    public long Id { get; set; }

    public DateTime Createddate { get; set; }

    public double Currentprogress { get; set; }

    public string? Deadline { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = null!;

    public double Targetamount { get; set; }

    public long Userid { get; set; }
}
