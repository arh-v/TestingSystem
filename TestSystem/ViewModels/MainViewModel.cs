namespace TestSystem.ViewModels;

public class MainViewModel : ViewModel
{
    private ViewModel _currentContent;

    public StartMenuViewModel StartMenuVm { get; }

    public ViewModel CurrentContent
    {
        get => _currentContent;
        set => Set(ref _currentContent, value);
    }

    public MainViewModel()
    {
        StartMenuVm = new();
        StartMenuVm.OnLogin += StartMenuLoginClick;
        StartMenuVm.OnRegister += StartMenuRegisterClick;
        StartMenuVm.OnTesting += StartMenuTestingClick;
        CurrentContent = StartMenuVm;
    }

    private void StartMenuLoginClick(object sender, StartMenuViewModel.LoginEventArgs e)
    {
        var authVm = new AuthorizationViewModel();
        CurrentContent = authVm;
        authVm.OnCancel += ReturnToStartMenu;
        authVm.OnAuthorize += ProvideAccessToSystem;
    }

    private void StartMenuRegisterClick(object sender, StartMenuViewModel.RegistrationEventArgs e)
    {
        var registrationVm = new RegistrationViewModel();
        CurrentContent = registrationVm;
        registrationVm.OnCancel += ReturnToStartMenu;
        registrationVm.OnSucсessfulRegistration += GoFromRegistrationToLogin;
    }

    private void StartMenuTestingClick(object sender, StartMenuViewModel.TestingEventArgs e)
    {
        var startTestingFormVm = new StartTestingFormViewModel();
        CurrentContent = startTestingFormVm;
        startTestingFormVm.OnCancel += ReturnToStartMenu;
        startTestingFormVm.OnStartTesting += StartTesting;
    }

    private void StartTesting(object sender, StartTestingFormViewModel.StartTestingEventArgs e)
    {
        var testingFormVm = new TestingFormViewModel(e.ParticipantFullname, e.TestId);
        CurrentContent = testingFormVm;
        testingFormVm.OnFinish += OpenTestResultViewer;
    }

    private void ReturnToStartMenu(object sender, EventArgs e)
    {
        if (sender is IDisposable resource)
        {
            resource.Dispose();
        }

        CurrentContent = StartMenuVm;
    }

    private void OpenTestResultViewer(object sender, TestingFormViewModel.FinishEventArgs e)
    {
        if (sender is IDisposable resource)
        {
            resource.Dispose();
        }

        var testResultViewer = new TestResultViewModel(e.FinishedTestId);
        CurrentContent = testResultViewer;
        testResultViewer.OnOk += ReturnToStartMenu;
    }

    private void GoFromRegistrationToLogin(object sender, RegistrationViewModel.RegisterEventArgs e)
    {
        var authVm = new AuthorizationViewModel();
        CurrentContent = authVm;
        authVm.OnCancel += ReturnToStartMenu;
        authVm.OnAuthorize += ProvideAccessToSystem;
        authVm.Login.Text = e.Login;
    }

    private void ProvideAccessToSystem(object sender, AuthorizationViewModel.LoginEventArgs e)
    {
        if (sender is IDisposable resource)
        {
            resource.Dispose();
        }

        if (e.IsExpert)
        {
            var expertMenuVm = new ExpertMenuViewModel(e.Login);
            CurrentContent = expertMenuVm;
            expertMenuVm.OnExit += ReturnToStartMenu;
            expertMenuVm.OnTestCreation += OpenTestCreation;
            expertMenuVm.OnTestChange += OpenTestChanging;
        }
        else
        {
            var reviewerMenuVm = new ReviewerMenuViewModel(e.Login);
            CurrentContent = reviewerMenuVm;
            reviewerMenuVm.OnExit += ReturnToStartMenu;
            reviewerMenuVm.OnResultOpening += OpenTestResultEditor;
        }
    }

    private void OpenTestCreation(object sender, ExpertMenuViewModel.CreateTestEventArgs e)
    {
        if (sender is IDisposable resource)
        {
            resource.Dispose();
        }

        var testCreationVm = new TestCreationViewModel(e.Username);
        CurrentContent = testCreationVm;
        testCreationVm.OnCancel += ReturnToExpertMenu;
    }

    private void OpenTestChanging(object sender, ExpertMenuViewModel.ChangeTestEventArgs e)
    {
        if (sender is IDisposable resource)
        {
            resource.Dispose();
        }

        var testCreationVm = new TestCreationViewModel(e.TestId);
        CurrentContent = testCreationVm;
        testCreationVm.OnCancel += ReturnToExpertMenu;
    }

    private void ReturnToExpertMenu(object sender, TestCreationViewModel.CancelEventArgs e)
    {
        ProvideAccessToSystem(sender, new(e.Username, true));
    }

    private void OpenTestResultEditor(object sender, ReviewerMenuViewModel.OpenResultEventArgs e)
    {
        if (sender is IDisposable resource)
        {
            resource.Dispose();
        }

        var testResultViewer = new TestResultEditingViewModel(e.FinishedTestId, e.ReviewerName);
        CurrentContent = testResultViewer;
        testResultViewer.OnCancel += ReturnToReviewerMenu;
    }

    private void ReturnToReviewerMenu(object sender, TestResultEditingViewModel.CancelEventArgs e)
    {
        ProvideAccessToSystem(sender, new(e.Username, false));
    }
}