using System;
using System.Collections.Generic;

namespace TestSystem.Database;

public partial class Module(int number) : IDbEntity
{
    public int Id { get; set; }

    public int Number { get; set; } = number;

    public string Name { get; set; }

    public int Test { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual Test? TestNavigation { get; set; }
}
