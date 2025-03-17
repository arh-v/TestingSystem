using System.Configuration;
using System.Data;
using System.Windows;
using TestSystem.ViewModels;

namespace TestSystem;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private MainWindow Window { get; set; }
    public MainViewModel MainViewModel { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainViewModel = new MainViewModel();
        Window = new MainWindow { DataContext = MainViewModel };
        Window.Show();
    }
}

