using Microsoft.EntityFrameworkCore.Internal;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class DiagnoseCreationViewModel : ViewModel
{
    public Diagnose Entity { get; }

    public TextField<string> Name { get; }

    public TextField<string> Recomendation { get; }

    public TextField<int> ScoreFrom { get; }

    public TextField<int> ScoreTo { get; }

    public ICommand DeleteCommand { get; }

    public EventHandler OnDelete;

    public DiagnoseCreationViewModel() : this(null) { }

    public DiagnoseCreationViewModel(Diagnose diagnose)
    {
        Entity = diagnose ?? new();
        Name = new(Entity, p => p != "", nameof(Entity.Name));
        Recomendation = new(Entity, p => p != null, nameof(Entity.Recomendation));
        ScoreFrom = new(Entity, nameof(Entity.ScoreFrom));
        ScoreTo = new(Entity, nameof(Entity.ScoreTo));
        DeleteCommand = new RelayCommand(Delete);

        if (diagnose != null)
        {
            Name.Text = diagnose.Name;
            Recomendation.Text = diagnose.Recomendation;
            ScoreFrom.Text = diagnose.ScoreFrom.ToString();
            ScoreTo.Text = diagnose.ScoreTo.ToString();
        }
    }

    private void Delete()
    {
        OnDelete.Invoke(this, EventArgs.Empty);
    }

    public bool CheckDataIsCorrect()
    {
        if (Entity.ScoreFrom > Entity.ScoreTo)
        {
            ScoreFrom.SetWarningColor();
            return false;
        }

        ScoreTo.SetDefaultColor();
        return Name.IsCorrect && Recomendation.IsCorrect && ScoreFrom.IsCorrect && ScoreTo.IsCorrect;
    }
}

