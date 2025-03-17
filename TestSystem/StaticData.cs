namespace TestSystem;

public static class StaticData
{
    public static string[] AnswerTypes { get; }

    static StaticData()
    {
        AnswerTypes = new string[2];
        AnswerTypes[0] = "Один";
        AnswerTypes[1] = "Несколько";
    }

    public enum AnswerType
    {
        Single,
        Multiple
    }
}

