using System.Collections.ObjectModel;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class ExpertMenuViewModel : ViewModel, IDisposable
{
    private ObservableCollection<Test> _tests = [];
    private int _selectedTestIndex = -1;
    private DataBaseContext _db;
    private bool _disposed = false;

    public int SelectedTestIndex
    {
        get => _selectedTestIndex;
        set => Set(ref _selectedTestIndex, value);
    }

    public IReadOnlyList<Test> Tests => _tests;

    public string Username { get; }

    public ICommand ExitCommand { get; }

    public ICommand CreateTestCommand { get; }

    public ICommand ChangeTestCommand { get; }

    public ICommand DeleteTestCommand { get; }

    public event EventHandler OnExit;
    public event EventHandler<CreateTestEventArgs> OnTestCreation;
    public event EventHandler<ChangeTestEventArgs> OnTestChange;
    public event EventHandler OnTestDeleting;

    public ExpertMenuViewModel(string username)
    {
        _db = new DataBaseContext();
        Username = username;
        ExitCommand = new RelayCommand(() => OnExit?.Invoke(this, EventArgs.Empty));
        CreateTestCommand = new RelayCommand(() => OnTestCreation?.Invoke(this, new(Username)));
        DeleteTestCommand = new RelayCommand(DeleteTest);
        ChangeTestCommand = new RelayCommand(ChangeTest);
        LoadTests();
    }

    private void DeleteTest()
    {
        if (SelectedTestIndex == -1) return;

        var test = _tests[SelectedTestIndex];
        _tests.RemoveAt(SelectedTestIndex);
        _db.Tests.Remove(test);
        _db.SaveChanges();
    }

    private void ChangeTest()
    {
        if (SelectedTestIndex == -1) return;

        OnTestChange.Invoke(this, new(_tests[SelectedTestIndex].Id));
    }

    private void LoadTests()
    {
        var tests = from test in _db.Tests
                    where test.Author == Username
                    select test;

        foreach (var test in tests)
        {
            _tests.Add(test);
        }
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

    public class CreateTestEventArgs(string username) : EventArgs
    {
        public string Username { get; } = username;
    }

    public class ChangeTestEventArgs(int testId) : EventArgs
    {
        public int TestId { get; } = testId;
    }

    ~ExpertMenuViewModel()
    {
        Dispose(false);
    }
}