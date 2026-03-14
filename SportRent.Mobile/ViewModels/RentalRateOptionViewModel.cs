using SportRent.Mobile.Models;

namespace SportRent.Mobile.ViewModels;

public sealed class RentalRateOptionViewModel : ViewModelBase
{
    private bool _isSelected;

    public required EquipmentRate Rate { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (!SetProperty(ref _isSelected, value))
            {
                return;
            }

            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(PrimaryTextColor));
            OnPropertyChanged(nameof(SecondaryTextColor));
        }
    }

    public string Title => Rate.RentalTypeTitle;

    public string MetaText => $"{Rate.DurationText} · {Rate.PriceText}";

    public string DepositText => Rate.DepositText;

    public string BackgroundColor => IsSelected ? "#163B2D" : "#F8F4EC";

    public string BorderColor => IsSelected ? "#163B2D" : "#D7D1C0";

    public string PrimaryTextColor => IsSelected ? "#FFFFFF" : "#23332D";

    public string SecondaryTextColor => IsSelected ? "#DDE9E2" : "#5B655F";
}
