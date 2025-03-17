using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TestSystem.Database;

public partial class Question(int number) : IDbEntity
{
    public int Id { get; set; }

    public string Content { get; set; }

    public int AnswerType { get; set; }

    public int Module { get; set; }

    public int Number { get; set; } = number;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Module? ModuleNavigation { get; set; }
}
