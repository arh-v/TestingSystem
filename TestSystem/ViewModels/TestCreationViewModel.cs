using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class TestCreationViewModel : ViewModel, IDisposable
{
    private ObservableCollection<ModuleCreationViewModel> _modules = [];
    private ObservableCollection<DiagnoseCreationViewModel> _diagnoses = [];
    private DataBaseContext _db = new();
    private bool _disposed = false;
    private string _message;

    public Test Entity { get; }

    public TextField<string> Name { get; set; }

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged();
        }
    }

    public IReadOnlyCollection<ModuleCreationViewModel> Modules => _modules;

    public IReadOnlyCollection<DiagnoseCreationViewModel> Diagnoses => _diagnoses;

    public ICommand CancelCommand { get; }

    public ICommand SaveCommand { get; }

    public ICommand AddModuleCommand { get; }

    public ICommand AddDiagnoseCommand { get; }

    public event EventHandler<CancelEventArgs> OnCancel;

    public TestCreationViewModel(int testId) : this("", testId)
    {
        if (testId == -1) throw new ArgumentException(nameof(testId));
    }

    public TestCreationViewModel(string username) : this(username, -1)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentException(nameof(username));
    }

    private TestCreationViewModel(string username, int testId)
    {
        Entity = testId != -1 ? LoadTest(testId) : new(username);
        Name = new(Entity, p => p != "", nameof(Entity.Name));
        CancelCommand = new RelayCommand(() => OnCancel?.Invoke(this, new(Entity.Author)));
        SaveCommand = new RelayCommand(Save);
        AddModuleCommand = new RelayCommand(AddModule);
        AddDiagnoseCommand = new RelayCommand(AddDiagnose);

        if (testId != -1)
        {
            Name.Text = Entity.Name;
        }
    }

    private Test LoadTest(int testId)
    {
        var entity = (from test in _db.Tests
                          .Include(t => t.Modules.OrderBy(m => m.Number))
                          .ThenInclude(m => m.Questions.OrderBy(q => q.Number))
                          .ThenInclude(q => q.Answers)
                          .Include(t => t.Diagnoses)
                      where test.Id == testId
                      select test).FirstOrDefault();

        if (entity == null) throw new ArgumentException(nameof(testId));

        foreach (var module in entity.Modules)
        {
            _modules.Add(CreateModuleVm(module));
        }

        foreach (var diagnose in entity.Diagnoses)
        {
            _diagnoses.Add(CreateDiagnoseVm(diagnose));
        }

        return entity;
    }

    private ModuleCreationViewModel CreateModuleVm(Module m = null)
    {
        var module = new ModuleCreationViewModel(_modules.Count + 1, m);
        module.OnDelete += DeleteModule;
        module.OnMoveUp += MoveModuleUp;
        module.OnMoveDown += MoveModuleDown;
        return module;
    }

    private void AddModule()
    {
        var module = CreateModuleVm();
        _modules.Add(module);
        Entity.Modules.Add(module.Entity);
    }

    private void DeleteModule(object sender, EventArgs e)
    {
        if (sender is not ModuleCreationViewModel module) return;

        var lowerCount = _modules.Count - module.Number;
        _modules.Remove(module);
        Entity.Modules.Remove(module.Entity);

        while (lowerCount-- > 0)
        {
            _modules[module.Number - 1 + lowerCount].Number--;
        }
    }

    private void MoveModuleUp(object sender, EventArgs e)
    {
        if (sender is not ModuleCreationViewModel module) return;

        if (module.Number == 1) return;

        var upper = _modules[module.Number - 2];
        _modules.Move(module.Number - 1, module.Number - 2);
        module.Number--;
        upper.Number++;
    }

    private void MoveModuleDown(object sender, EventArgs e)
    {
        if (sender is not ModuleCreationViewModel module) return;

        if (module.Number == _modules.Count) return;

        var lower = _modules[module.Number];
        _modules.Move(module.Number - 1, module.Number);
        lower.Number--;
        module.Number++;
    }

    public DiagnoseCreationViewModel CreateDiagnoseVm(Diagnose d = null)
    {
        var diagnose = new DiagnoseCreationViewModel(d);
        diagnose.OnDelete += DeleteDiagnose;
        return diagnose;
    }

    private void AddDiagnose()
    {
        var diagnose = CreateDiagnoseVm();
        _diagnoses.Add(diagnose);
        Entity.Diagnoses.Add(diagnose.Entity);
    }

    private void DeleteDiagnose(object sender, EventArgs e)
    {
        if (sender is not DiagnoseCreationViewModel diagnose) return;

        _diagnoses.Remove(diagnose);
        Entity.Diagnoses.Remove(diagnose.Entity);
    }

    private void Save()
    {
        if (!CheckDataIsCorrect()) return;

        if (_db.Tests.Where(test => test.Id == Entity.Id).Count() == 0)
        {
            _db.Tests.Add(Entity);
        }

        _db.SaveChanges();
        OnCancel.Invoke(this, new(Entity.Author));
    }

    private bool CheckDataIsCorrect()
    {
        foreach (var module in _modules)
        {
            if (!module.CheckDataIsCorrect())
            {
                Message = "Модули заполнены неправильно!" + 
                    " Проверьте правильность заполнения: должно быть не меньше двух вопросов";
                return false;
            }
        }

        foreach (var diagnose in _diagnoses)
        {
            if (!diagnose.CheckDataIsCorrect())
            {
                Message = "Диагнозы заполнены неправильно!";
                return false;
            }
        }

        if (_modules.Count < 1)
        {
            Message = "Должно быть не меньше одного модуля!";
            return false;
        }

        if (_diagnoses.Count < 2)
        {
            Message = "Должно быть не меньше одного диагноза";
            return false;
        }

        if (!Name.IsCorrect)
        {
            Message = "Назовите анкету!";
            return false;
        }

        return true;
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

    ~TestCreationViewModel()
    {
        // Do not re-create Dispose clean-up code here.
        // Calling Dispose(disposing: false) is optimal in terms of
        // readability and maintainability.
        Dispose(false);
    }
}

