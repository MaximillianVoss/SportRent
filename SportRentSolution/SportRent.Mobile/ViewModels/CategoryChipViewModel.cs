namespace SportRent.Mobile.ViewModels;

public sealed class CategoryChipViewModel : ViewModelBase
{
    private bool _isSelected;

    public int? CategoryId { get; init; }

    public required string Title { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (!SetProperty(ref _isSelected, value))
            {
                return;
            }

            OnPropertyChanged(nameof(ChipBackgroundColor));
            OnPropertyChanged(nameof(ChipTextColor));
            OnPropertyChanged(nameof(ChipBorderColor));
        }
    }

    public string ChipBackgroundColor => IsSelected ? "#163B2D" : "#F4F1E8";

    public string ChipTextColor => IsSelected ? "#FFFFFF" : "#234034";

    public string ChipBorderColor => IsSelected ? "#163B2D" : "#D7D1C0";
}
