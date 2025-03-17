using System;
using System.Collections.Generic;

namespace TestSystem.Database;

public partial class Diagnose : IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int ScoreFrom { get; set; }

    public int ScoreTo { get; set; }

    public int Test { get; set; }

    public string? Recomendation { get; set; }

    public virtual Test? TestNavigation { get; set; }
}
