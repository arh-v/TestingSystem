using System;
using System.Collections.Generic;

namespace TestSystem.Database;

public partial class RegisteredUser(string login, int isExpert, int? password = null) : IDbEntity
{
    public string Login { get; set; } = login;

    public int? Password { get; set; } = password;

    public int IsExpert { get; set; } = isExpert;

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
