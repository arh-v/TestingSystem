using TestSystem.Database;

namespace TestSystem.ViewModels;

class AnswerCreationViewModel : ViewModel
{
    public Answer Entity { get; }

    public TextField<int> Score { get; set; }

    public TextField<string> Content { get; set; }

    public AnswerCreationViewModel() : this(null) { }

    public AnswerCreationViewModel(Answer answer)
    {
        Entity = answer ?? new Answer();
        Score = new(Entity, nameof(Entity.Score));
        Content = new(Entity, p => p != "", nameof(Entity.Content));

        if (answer != null)
        {
            Score.Text = answer.Score.ToString();
            Content.Text = answer.Content.ToString();
        }
    }

    public bool CheckDataIsCorrect()
    {
        return Score.IsCorrect && Content.IsCorrect;
    }
}