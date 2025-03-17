using System;
using System.Collections.Generic;

namespace TestSystem.Database;

public partial class FinishedTest(string name, string author, string finishDate, string participantFullname) : IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = name;

    public string Author { get; set; } = author;

    public string FinishDate { get; set; } = finishDate;

    public string ParticipantFullname { get; set; } = participantFullname;

    public virtual ICollection<FinishedModule> FinishedModules { get; set; } = new List<FinishedModule>();
}
