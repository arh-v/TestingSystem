using System.Windows.Input;

namespace TestSystem.ViewModels;

public class StartMenuViewModel : ViewModel
{
    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }

    public ICommand StartTestingCommand { get; }

    public event EventHandler<LoginEventArgs> OnLogin;
    public event EventHandler<RegistrationEventArgs> OnRegister;
    public event EventHandler<TestingEventArgs> OnTesting;

    public StartMenuViewModel()
    {
        LoginCommand = new RelayCommand(() => OnLogin?.Invoke(this, new()));
        RegisterCommand = new RelayCommand(() => OnRegister?.Invoke(this, new()));
        StartTestingCommand = new RelayCommand(() => OnTesting?.Invoke(this, new()));
    }

    public class LoginEventArgs : EventArgs { }

    public class RegistrationEventArgs : EventArgs { }

    public class TestingEventArgs : EventArgs { }
}
