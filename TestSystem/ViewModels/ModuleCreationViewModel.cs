using System.Collections.ObjectModel;
using System.Windows.Input;
using TestSystem.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestSystem.ViewModels;

class ModuleCreationViewModel : ViewModel
{
    private ObservableCollection<QuestionCreationViewModel> _questions = [];

    public Module Entity { get; }

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

    public TextField<string> Name { get; set; }

    public IReadOnlyCollection<QuestionCreationViewModel> Questions => _questions;

    public ICommand DeleteCommand { get; }

    public ICommand AddQuestionCommand { get; }

    public ICommand MoveUpCommand { get; }

    public ICommand MoveDownCommand { get; }

    public EventHandler OnDelete;
    public EventHandler OnMoveUp;
    public EventHandler OnMoveDown;

    public ModuleCreationViewModel(int number) : this(number, null) { }

    public ModuleCreationViewModel(int number, Module module)
    {
        Entity = module ?? new(number);
        Name = new(Entity, p => p != "", nameof(Entity.Name));
        AddQuestionCommand = new RelayCommand(AddQuestion);
        DeleteCommand = new RelayCommand(Delete);
        MoveUpCommand = new RelayCommand(() => OnMoveUp?.Invoke(this, EventArgs.Empty));
        MoveDownCommand = new RelayCommand(() => OnMoveDown?.Invoke(this, EventArgs.Empty));

        if (module != null)
        {
            LoadQuestions();
            Name.Text = module.Name;
        }
    }

    private void LoadQuestions()
    {
        foreach (var question in Entity.Questions)
        {
            _questions.Add(CreateQuestionVm(question));
        }
    }

    private QuestionCreationViewModel CreateQuestionVm(Question q = null)
    {
        var question = new QuestionCreationViewModel(_questions.Count + 1, q);
        question.OnDelete += DeleteQuestion;
        question.OnMoveUp += MoveUpQuestion;
        question.OnMoveDown += MoveDownQuestion;
        return question;
    }

    private void AddQuestion()
    {
        var question = CreateQuestionVm();
        _questions.Add(question);
        Entity.Questions.Add(question.Entity);
    }

    private void Delete()
    {
        OnDelete?.Invoke(this, EventArgs.Empty);
    }

    private void DeleteQuestion(object sender, EventArgs e)
    {
        if (sender is not QuestionCreationViewModel question) return;

        var lowerCount = _questions.Count - question.Number;
        _questions.Remove(question);
        Entity.Questions.Remove(question.Entity);

        while (lowerCount-- > 0)
        {
            _questions[question.Number - 1 + lowerCount].Number--;
        }
    }

    private void MoveUpQuestion(object sender, EventArgs e)
    {
        if (sender is not QuestionCreationViewModel question) return;

        if (question.Number == 1) return;

        var upper = _questions[question.Number - 2];
        _questions.Move(question.Number - 1, question.Number - 2);
        question.Number--;
        upper.Number++;
    }

    private void MoveDownQuestion(object sender, EventArgs e)
    {
        if (sender is not QuestionCreationViewModel question) return;

        if (question.Number == _questions.Count) return;

        var lower = _questions[question.Number];
        _questions.Move(question.Number - 1, question.Number);
        lower.Number--;
        question.Number++;
    }

    public bool CheckDataIsCorrect()
    {
        foreach (var question in _questions)
        {
            if (!question.CheckDataIsCorrect()) return false;
        }

        return _questions.Count >= 1 && Name.IsCorrect;
    }
}