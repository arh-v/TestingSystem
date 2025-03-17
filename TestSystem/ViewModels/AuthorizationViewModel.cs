using System.Windows.Input;
using TestSystem.Database;

namespace TestSystem.ViewModels;

public class AuthorizationViewModel : ViewModel
{
    public TextField<string> Login { get; set; }

    public TextField<string> Password { get; set; }

    public ICommand LoginCommand { get; }

    public ICommand CancelCommand { get; }

    public event EventHandler<LoginEventArgs> OnAuthorize;
    public event EventHandler<CancelEventArgs> OnCancel;

    public AuthorizationViewModel()
    {
        Login = new(p => p != "");
        Password = new();
        LoginCommand = new RelayCommand(Authorize);
        CancelCommand = new RelayCommand(() => OnCancel?.Invoke(this, new()));
    }

    private void Authorize()
    {
        using var db = new DataBaseContext();
        var username = Login.Text?.ToLower();
        var reg = db.RegisteredUsers.ToList();
        var registeredUsers = (from user in db.RegisteredUsers
                               where user.Login == $"{username}"
                               select user).ToList();

        if (registeredUsers.Count == 0)
        {
            Login.SetWarningColor();
            return;
        }

        var registered = registeredUsers[0];
        Login.SetDefaultColor();
        var viewPasswordHash = Password.GetTextHash();

        if (registered.Password != viewPasswordHash)
        {
            Password.SetWarningColor();
            return;
        }

        Password.SetDefaultColor();
        OnAuthorize?.Invoke(this, new(username, registered.IsExpert == 1));
    }

    public class LoginEventArgs : EventArgs
    {
        public string Login { get; }

        public bool IsExpert { get; }

        public LoginEventArgs(string user, bool isExpert)
        {
            Login = user;
            IsExpert = isExpert;
        }
    }

    public class CancelEventArgs : EventArgs { }
}