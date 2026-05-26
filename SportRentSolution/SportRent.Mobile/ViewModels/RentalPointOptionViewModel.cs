using SportRent.Mobile.Models;

namespace SportRent.Mobile.ViewModels;

public sealed class RentalPointOptionViewModel : ViewModelBase
{
    private bool _isSelected;

    public required RentalPointAvailability Availability { get; init; }

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
        }
    }

    public int RentalPointEquipmentId => Availability.Id;

    public int RentalPointId => Availability.RentalPointId;

    public int AvailableQuantity => Availability.AvailableQuantity;

    public string Title => Availability.RentalPointName;

    public string Address => Availability.Address;

    public string MetaText => Availability.MetaText;

    public string PhoneText => Availability.PhoneText;

    public string AvailabilityText => Availability.AvailabilityText;

    public bool IsEnabled => Availability.IsAvailable;

    public string BackgroundColor => IsSelected ? "#EDF5F0" : "#FCFAF6";

    public string BorderColor => IsSelected ? "#163B2D" : "#D7D1C0";
}
