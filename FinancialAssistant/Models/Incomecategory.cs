using System;
using System.Collections.Generic;

namespace FinancialAssistant.Models;

public partial class Incomecategory
{
    public long Id { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = null!;

    public long Userid { get; set; }
}
