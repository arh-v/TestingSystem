using System.Windows.Input;
using TestSystem.Database;
using TestSystem.Managers;

namespace TestSystem.ViewModels;

class RegistrationViewModel : ViewModel
{
    public bool _isExpert = true;

    public TextField<string> Login { get; set; }

    public TextField<long> Password { get; set; }

    public TextField<string> RepeatPassword { get; set; }

    public bool IsExpert
    {
        get => _isExpert;
        set => Set(ref _isExpert, value);
    }

    public ICommand RegisterCommand { get; }

    public ICommand CancelCommand { get; }

    public event EventHandler<RegisterEventArgs> OnSucсessfulRegistration;
    public event EventHandler<CancelEventArgs> OnCancel;

    public RegistrationViewModel()
    {
        Login = new(p => p != "");
        Password = new();
        RepeatPassword = new();
        RegisterCommand = new RelayCommand(Register);
        CancelCommand = new RelayCommand(() => OnCancel?.Invoke(this, new()));
    }

    private void Register()
    {
        if (string.IsNullOrEmpty(Login.Text))
        {
            Login.SetWarningColor();
            return;
        }

        var username = Login.Text.ToLower();
        using var db = new DataBaseContext();
        var registeredUsers = (from user in db.RegisteredUsers
                               where user.Login == username
                               select user).ToList();

        if (registeredUsers.Count != 0)
        {
            Login.SetWarningColor();
            return;
        }

        Login.SetDefaultColor();

        if (RepeatPassword.Text != Password.Text)
        {
            RepeatPassword.SetWarningColor();
            return;
        }

        RepeatPassword.SetDefaultColor();
        
        var passwordHash = Password.GetTextHash();
        db.RegisteredUsers.Add(new(username, IsExpert ? 1 : 0, passwordHash));
        db.SaveChanges();
        OnSucсessfulRegistration.Invoke(this, new(username));
    }

    public class RegisterEventArgs : EventArgs
    {
        public string Login { get; }

        public RegisterEventArgs(string login)
        {
            Login = login;
        }
    }

    public class CancelEventArgs : EventArgs { }
}