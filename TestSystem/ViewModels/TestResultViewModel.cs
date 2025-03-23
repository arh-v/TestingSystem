using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class TestResultViewModel : ViewModel
{
    public FinishedTest Entity { get; }

    public ICommand OkCommand { get; }

    public EventHandler OnOk;

    public TestResultViewModel(int finishedTestId)
    {
        using var db = new DataBaseContext();

        Entity = (from test in db.FinishedTests.Include(test => test.FinishedModules)
                  where test.Id == finishedTestId
                  select test).FirstOrDefault();

        if (Entity == null) throw new ArgumentException(nameof(finishedTestId));

        OkCommand = new RelayCommand(() => OnOk?.Invoke(this, EventArgs.Empty));
    }
}

