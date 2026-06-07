using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SportRent.Mobile.ViewModels;

/// <summary>
/// Базовая ViewModel с поддержкой уведомлений об изменении свойств для XAML-привязок.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Обновляет поле и вызывает уведомление только при реальном изменении значения.
    /// </summary>
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Сообщает XAML-привязкам, что значение свойства изменилось.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
