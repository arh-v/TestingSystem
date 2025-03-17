using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TestSystem.Database;

public partial class Answer : IDbEntity, INotifyPropertyChanged
{
    public int Id { get; set; }

    public string Content { get; set; }

    public int Score { get; set; }

    public int Question { get; set; }

    public virtual Question? QuestionNavigation { get; set; }

    private bool _isSelected;

    [NotMapped]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
