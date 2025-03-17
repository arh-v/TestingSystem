using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using TestSystem.Database;

namespace TestSystem.ViewModels;

public class TextField<T> : ViewModel where T : IParsable<T>
{
    private Brush _textColor;
    private Brush _fieldColor;
    private string _content;
    private IDbEntity _entity;
    private PropertyInfo _property;
    private Func<T?, bool> _checker;

    public Brush TextColor
    {
        get => _textColor;
        set => Set(ref _textColor, value);
    }

    public Brush FieldColor
    {
        get => _fieldColor;
        set => Set(ref _fieldColor, value);
    }

    public string Text
    {
        get => _content;
        set
        {
            Set(ref _content, value);
            if (!TryParseWithValidation(value, out T parsed))
            {
                SetWarningColor();
                return;
            }

            SetDefaultColor();

            if (_entity == null) return;

            _property.SetValue(_entity, parsed);
        }
    }

    public bool IsCorrect
    {
        get
        {
            if (!TryParseWithValidation(Text, out _))
            {
                SetWarningColor();
                return false;
            }

            SetDefaultColor();
            return true;
        }
    }

    private bool TryParseWithValidation(string s, out T result)
    {
        result = default;

        if (!T.TryParse(s ??= "" , null, out var parsed)) return false;

        if (!_checker(parsed)) return false;

        result = parsed;
        return true;
    }

    public TextField() :
        this(null, null)
    { }

    public TextField(IDbEntity connected, string property) :
        this(connected, v => true, property)
    { }

    public TextField(Func<T, bool> valueChecker) :
        this(null, valueChecker, null)
    { }

    public TextField(IDbEntity connected, Func<T?, bool> valueChecker, string property)
    {
        _entity = connected;
        _checker = valueChecker;
        _property = _entity?.GetType().GetProperty(property);

        if (_entity != null && _property == null)
        {
            throw new ArgumentException($"{nameof(_entity)} class has no property:", nameof(property));
        }

        _textColor = Brushes.Black;
        _fieldColor = new SolidColorBrush(Color.FromRgb(171, 173, 179));
    }

    public void SetWarningColor()
    {
        TextColor = Brushes.Red;
        FieldColor = Brushes.Red;
    }

    public void SetDefaultColor()
    {
        TextColor = Brushes.Black;
        FieldColor = new SolidColorBrush(Color.FromRgb(171, 173, 179));
    }

    public int? GetTextHash()
    {
        if (_content == null) return null;

        unchecked
        {
            var FNV_prime = (ulong)1099511628211;
            var hash = 14695981039346656037;

            foreach (var item in _content)
            {
                hash = hash ^ item;
                hash = hash * FNV_prime;
            }

            return (int?)hash;
        }
    }
}