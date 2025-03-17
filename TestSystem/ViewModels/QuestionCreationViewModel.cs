using System.Collections.ObjectModel;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class QuestionCreationViewModel : ViewModel
{
    private ObservableCollection<AnswerCreationViewModel> _answers = [];
    private int _selectedAnswer = -1;

    public Question Entity { get; }

    public TextField<string> Body { get; set; }

    public int SelectedAnswer
    {
        get => _selectedAnswer;
        set
        {
            if (value < -1 || value >= _answers.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Set(ref _selectedAnswer, value);
        }
    }

    public int Number
    {
        get => Entity.Number;
        set
        {
            if (value.Equals(Entity.Number)) return;

            Entity.Number = value;
            OnPropertyChanged();
        }
    }

    public int SelectedAnswerTypeIndex
    {
        get => Entity.AnswerType;
        set
        {
            if (value < 0 || value >= StaticData.AnswerTypes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value.Equals(Entity.AnswerType)) return;

            Entity.AnswerType = value;
            OnPropertyChanged();
        }
    }

    public IReadOnlyCollection<AnswerCreationViewModel> Answers => _answers;

    public ICommand DeleteCommand { get; }

    public ICommand AddAnswerCommand { get; }

    public ICommand RemoveAnswerCommand { get; }

    public ICommand MoveUpCommand { get; }

    public ICommand MoveDownCommand { get; }

    public EventHandler OnDelete;
    public EventHandler OnMoveUp;
    public EventHandler OnMoveDown;

    public QuestionCreationViewModel(int number) : this(number, null) { }

    public QuestionCreationViewModel(int number, Question question)
    {
        Entity = question ?? new Question(number);
        Body = new(Entity, p => p != "", nameof(Entity.Content));
        DeleteCommand = new RelayCommand(() => OnDelete?.Invoke(this, EventArgs.Empty));
        AddAnswerCommand = new RelayCommand(AddAnswer);
        RemoveAnswerCommand = new RelayCommand(RemoveAnswer);
        MoveUpCommand = new RelayCommand(() => OnMoveUp?.Invoke(this, EventArgs.Empty));
        MoveDownCommand = new RelayCommand(() => OnMoveDown?.Invoke(this, EventArgs.Empty));

        if (question != null)
        {
            LoadAnswers();
            Body.Text = question.Content;
        }
    }

    private void LoadAnswers()
    {
        foreach (var answer in Entity.Answers)
        {
            _answers.Add(new AnswerCreationViewModel(answer));
        }
    }

    private void AddAnswer()
    {
        var answer = new AnswerCreationViewModel();
        _answers.Add(answer);
        Entity.Answers.Add(answer.Entity);
    }

    private void RemoveAnswer()
    {
        if (SelectedAnswer == -1) return;

        var answer = _answers[SelectedAnswer];
        _answers.RemoveAt(SelectedAnswer);
        Entity.Answers.Remove(answer.Entity);
    }

    public bool CheckDataIsCorrect()
    {
        foreach (var answer in _answers)
        {
            if (!answer.CheckDataIsCorrect()) return false;
        }

        return _answers.Count >= 2 && Body.IsCorrect;
    }
}

