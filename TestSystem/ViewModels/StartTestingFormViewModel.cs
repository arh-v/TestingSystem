using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels
{
    class StartTestingFormViewModel : ViewModel
    {
        private List<Test> _selectionList = [];
        private int _selectedTestIndex = -1;

        public TextField<string> Surname { get; set; }

        public TextField<string> Name { get; set; }

        public TextField<string> Patronymic { get; set; }

        public IReadOnlyCollection<Test> SelectionList => _selectionList;

        public int SelectedTestIndex
        {
            get => _selectedTestIndex;
            set => Set(ref _selectedTestIndex, value);
        }

        public ICommand StartTestingCommand { get; }

        public ICommand CancelCommand { get; }

        public event EventHandler<StartTestingEventArgs> OnStartTesting;
        public event EventHandler<CancelEventArgs> OnCancel;

        public StartTestingFormViewModel()
        {
            Name = new(p => p != "");
            Surname = new(p => p != "");
            Patronymic = new();
            StartTestingCommand = new RelayCommand(StartTesting);
            CancelCommand = new RelayCommand(() => OnCancel?.Invoke(this, new()));
            LoadTests();
        }

        private void StartTesting()
        {
            if (SelectedTestIndex == -1) return;

            if (!Name.IsCorrect || !Surname.IsCorrect || !Patronymic.IsCorrect) return;

            var fullname = $"{Surname.Text} {Name.Text} {Patronymic.Text}";
            var test = _selectionList[SelectedTestIndex];
            OnStartTesting?.Invoke(this, new(test.Id, fullname));
        }

        private void LoadTests()
        {
            using var db = new DataBaseContext();
            var tttt = db.Tests.ToList();
            var tests = from test in db.Tests
                        select test;

            foreach (var test in tests)
            {
                _selectionList.Add(test);
            }
        }

        public class StartTestingEventArgs(int testId, string participantFullname) : EventArgs
        {
            public int TestId { get; } = testId;

            public string ParticipantFullname { get; } = participantFullname;
        }

        public class CancelEventArgs : EventArgs { }
    }
}
