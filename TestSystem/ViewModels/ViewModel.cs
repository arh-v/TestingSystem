using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestSystem.ViewModels;

public class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand<T> : ICommand
{
    private Action<T> action;
    public RelayCommand(Action<T> action) => this.action = action;
    public bool CanExecute(object parameter) => true;
#pragma warning disable CS0067
    public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
    public void Execute(object parameter) => action((T)parameter);
}

public class RelayCommand : ICommand
{
    private Action action;
    public RelayCommand(Action action) => this.action = action;
    public bool CanExecute(object parameter) => true;
#pragma warning disable CS0067
    public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
    public void Execute(object parameter) => action();
}