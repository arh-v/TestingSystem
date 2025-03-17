using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystem.Database;

public partial class FinishedModule(string name, int number, string recomendations, int score) :
    IDbEntity, INotifyPropertyChanged
{
    public int Id { get; set; }

    public string Name { get; set; } = name;

    public int Number { get; set; } = number;

    private string _recomendations = recomendations;

    public string? Recomendations
    {
        get => _recomendations;
        set
        {
            _recomendations = value;
            OnPropertyChanged();
        }
    }

    public int Score { get; set; } = score;

    public int FinishedTest { get; set; }

    public virtual FinishedTest? FinishedTestNavigation { get; set; }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
