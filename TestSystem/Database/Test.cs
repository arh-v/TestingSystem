using System;
using System.Collections.Generic;

namespace TestSystem.Database;

public partial class Test(string author) : IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Author { get; set; } = author;

    public virtual RegisteredUser? AuthorNavigation { get; set; }

    public virtual ICollection<Diagnose> Diagnoses { get; set; } = new List<Diagnose>();

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
}
