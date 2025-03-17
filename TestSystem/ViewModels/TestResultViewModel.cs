using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class TestResultViewModel : ViewModel, IDisposable
{
    private bool _disposed = false;
    private DataBaseContext _db = new();
    private string _reviewerName;

    public FinishedTest Entity { get; }

    public ICommand SaveCommand { get; }

    public ICommand CancelCommand { get; }

    public EventHandler<CancelEventArgs> OnCancel;

    public TestResultViewModel(int finishedTestId, string reviewerName)
    {
        _reviewerName = reviewerName;
        Entity = (from test in _db.FinishedTests.Include(test => test.FinishedModules)
                  where test.Id == finishedTestId
                  select test).FirstOrDefault();

        if (Entity == null) throw new ArgumentException(nameof(finishedTestId));

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(() => OnCancel?.Invoke(this, new(_reviewerName)));
        _reviewerName = reviewerName;
    }

    private void Save()
    {
        _db.SaveChanges();
        OnCancel?.Invoke(this, new(_reviewerName));
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

    public class CancelEventArgs(string username) : EventArgs
    {
        public string Username { get; } = username;
    }

    ~TestResultViewModel()
    {
        Dispose(false);
    }
}

