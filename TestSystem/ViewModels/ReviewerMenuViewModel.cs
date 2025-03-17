using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

class ReviewerMenuViewModel : ViewModel, IDisposable
{
    private DataBaseContext _db = new();
    private ObservableCollection<FinishedTest> _testsResults = [];
    private int _selectedResultIndex = -1;
    private bool _disposed = false;

    public int SelectedResultIndex
    {
        get => _selectedResultIndex;
        set => Set(ref _selectedResultIndex, value);
    }

    public IReadOnlyList<FinishedTest> TestsResults => _testsResults;

    public string Username { get; }

    public ICommand ExitCommand { get; }

    public ICommand OpenResultCommand { get; }

    public ICommand DeleteResultCommand { get; }

    public ICommand DeleteAllResultsCommand { get; }

    public ICommand ExportResultCommand { get; }

    public event EventHandler OnExit;
    public event EventHandler<OpenResultEventArgs> OnResultOpening;

    public ReviewerMenuViewModel(string username)
    {
        Username = username;
        ExitCommand = new RelayCommand(() => OnExit?.Invoke(this, EventArgs.Empty));
        OpenResultCommand = new RelayCommand(OpenResult);
        DeleteResultCommand = new RelayCommand(DeleteResult);
        DeleteAllResultsCommand = new RelayCommand(DeleteAllResults);
        ExportResultCommand = new RelayCommand(ExportResult);
        LoadResults();
    }

    private void OpenResult()
    {
        if (SelectedResultIndex == -1) return;

        OnResultOpening?.Invoke(this, new(_testsResults[SelectedResultIndex].Id, Username));
    }

    private void DeleteResult()
    {
        if (SelectedResultIndex == -1) return;

        var finishedTest = _testsResults[SelectedResultIndex];
        _testsResults.RemoveAt(SelectedResultIndex);
        _db.Remove(finishedTest);
        _db.SaveChanges();
    }

    private void DeleteAllResults()
    {
        foreach (var test in _testsResults)
        {
            _db.Remove(test);
        }

        _testsResults.Clear();
        _db.SaveChanges();
    }

    private void ExportResult()
    {
        if (SelectedResultIndex == -1) return;

        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
        saveFileDialog.Filter = "Текстовые файлы (*.docx)|*.docx";

        if (saveFileDialog.ShowDialog() != true) return;

        var filePath = saveFileDialog.FileName;

        var modules = (from test in _db.FinishedTests.Include(test => test.FinishedModules)
                     where test.Id == _testsResults[SelectedResultIndex].Id
                     select test)
                    .SelectMany(test => test.FinishedModules).ToList();
        var lines = modules.Where(m => !string.IsNullOrEmpty(m.Recomendations))
                    .SelectMany(m => m.Recomendations
                        .Split(["\r\n", "\n"], StringSplitOptions.None))
                    .ToList();

        using var myDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
        var mainPart = myDocument.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());
        
        foreach ( var line in lines)
        {
            var paragraf = body.AppendChild(new Paragraph());
            var run = paragraf.AppendChild(new Run());
            run.AppendChild(new Text(line));
        }

        mainPart.Document.Save();
    }

    private void LoadResults()
    {
        var results = from test in _db.FinishedTests
                      select test;

        foreach (var result in results)
        {
            _testsResults.Add(result);
        }
    }

    public class OpenResultEventArgs(int finishedTestId, string reviewerName) : EventArgs
    {
        public int FinishedTestId { get; } = finishedTestId;

        public string ReviewerName { get; } = reviewerName;
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

    ~ReviewerMenuViewModel()
    {
        Dispose(false);
    }
}

