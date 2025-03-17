using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class TestingFormViewModel : ViewModel, IDisposable
{
    private DataBaseContext _db = new();
    private bool _disposed = false;
    private int _selected = 0;

    public int SelectedTabIndex
    {
        get => _selected;
        set => Set(ref _selected, value);
    }

    public Test Entity { get; }

    public string ParticipantFullname { get; }

    public ICommand FinishCommand { get; }

    public EventHandler OnFinish;

    public TestingFormViewModel(string participantFullname, int testId)
    {
        ParticipantFullname = participantFullname;
        Entity = (from test in _db.Tests
                      .Include(t => t.Modules.OrderBy(m => m.Number))
                      .ThenInclude(m => m.Questions.OrderBy(q => q.Number))
                      .ThenInclude(q => q.Answers)
                      .Include(t => t.Diagnoses)
                  where test.Id == testId
                  select test).FirstOrDefault();

        if (Entity == null) throw new ArgumentException(nameof(testId));

        FinishCommand = new RelayCommand(Finish);
        SelectedTabIndex = 0;
    }

    private void Finish()
    {
        var finishedTest = new FinishedTest(Entity.Name, Entity.Author, DateTime.Now.ToString(), ParticipantFullname);
        var modulesWithScores = Entity.Modules
            .Select(m => (module: m, score: m.Questions
                .SelectMany(q => q.Answers
                    .Where(a => a.IsSelected)
                    .Select(a => a.Score))
                .Sum()));
        var finishedModules = modulesWithScores
            .Select(ms =>
            {
                var m = ms.module;
                var recomendations = DiagnoseSelector(Entity.Diagnoses, ms.score);
                return new FinishedModule(m.Name, m.Number, recomendations, ms.score);
            });

        foreach (var module in finishedModules)
        {
            finishedTest.FinishedModules.Add(module);
        }

        _db.FinishedTests.Add(finishedTest);
        _db.SaveChanges();
        OnFinish?.Invoke(this, EventArgs.Empty);
        SelectedTabIndex = -1; //чтобы не возникали ошибки привязки
    }

    private string DiagnoseSelector(ICollection<Diagnose> diagnoses, int score)
    {
        var sb = new StringBuilder();

        foreach (var diagnose in diagnoses)
        {
            if (score < diagnose.ScoreFrom || score > diagnose.ScoreTo) continue;

            sb.AppendLine($"Диагноз: {diagnose.Name}\nРекомендация: {diagnose.Recomendation}");
        }

        return sb.ToString();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _db.Dispose();
        }

        _disposed = true;
    }

    ~TestingFormViewModel()
    {
        Dispose(false);
    }
}