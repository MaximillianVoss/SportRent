using System.Collections.ObjectModel;
using SportRent.Mobile.Models;
using SportRent.Mobile.Services;

namespace SportRent.Mobile.ViewModels;

public sealed class EquipmentDetailsPageViewModel : ViewModelBase
{
    private readonly ISportRentCatalogService _catalogService;

    private bool _isBusy;
    private int _equipmentId;
    private string _title = string.Empty;
    private string _categoryTitle = string.Empty;
    private string _brandModelText = string.Empty;
    private string _typeTitle = string.Empty;
    private string _descriptionText = string.Empty;
    private string? _imageUrl;
    private string _startingPriceText = string.Empty;
    private string _depositText = string.Empty;
    private string _availabilityText = string.Empty;
    private string _accentColor = "#163B2D";
    private string _accentSurfaceColor = "#E5F0EA";
    private string _symbolText = "SR";
    private string? _errorMessage;
    private bool _hasAvailableUnits;
    private int? _loadedEquipmentId;

    public EquipmentDetailsPageViewModel(ISportRentCatalogService catalogService)
    {
        _catalogService = catalogService;
        Rates = [];
        RentalPoints = [];
    }

    public ObservableCollection<EquipmentRate> Rates { get; }

    public ObservableCollection<RentalPointAvailability> RentalPoints { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public int EquipmentId
    {
        get => _equipmentId;
        private set => SetProperty(ref _equipmentId, value);
    }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string CategoryTitle
    {
        get => _categoryTitle;
        private set => SetProperty(ref _categoryTitle, value);
    }

    public string BrandModelText
    {
        get => _brandModelText;
        private set => SetProperty(ref _brandModelText, value);
    }

    public string TypeTitle
    {
        get => _typeTitle;
        private set => SetProperty(ref _typeTitle, value);
    }

    public string DescriptionText
    {
        get => _descriptionText;
        private set => SetProperty(ref _descriptionText, value);
    }

    public string? ImageUrl
    {
        get => _imageUrl;
        private set
        {
            if (SetProperty(ref _imageUrl, value))
            {
                OnPropertyChanged(nameof(HasImage));
            }
        }
    }

    public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

    public string StartingPriceText
    {
        get => _startingPriceText;
        private set => SetProperty(ref _startingPriceText, value);
    }

    public string DepositText
    {
        get => _depositText;
        private set => SetProperty(ref _depositText, value);
    }

    public string AvailabilityText
    {
        get => _availabilityText;
        private set => SetProperty(ref _availabilityText, value);
    }

    public string AccentColor
    {
        get => _accentColor;
        private set => SetProperty(ref _accentColor, value);
    }

    public string AccentSurfaceColor
    {
        get => _accentSurfaceColor;
        private set => SetProperty(ref _accentSurfaceColor, value);
    }

    public string SymbolText
    {
        get => _symbolText;
        private set => SetProperty(ref _symbolText, value);
    }

    public bool HasAvailableUnits
    {
        get => _hasAvailableUnits;
        private set => SetProperty(ref _hasAvailableUnits, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasRates => Rates.Count > 0;

    public bool HasRentalPoints => RentalPoints.Count > 0;

    public async Task LoadAsync(int equipmentId, bool forceReload = false, CancellationToken cancellationToken = default)
    {
        if (IsBusy || (!forceReload && _loadedEquipmentId == equipmentId))
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            EquipmentDetails? equipment = await _catalogService.GetEquipmentDetailsAsync(equipmentId, cancellationToken);
            if (equipment is null)
            {
                ErrorMessage = "Карточка инвентаря не найдена.";
                return;
            }

            _loadedEquipmentId = equipmentId;
            EquipmentId = equipment.Id;

            Title = equipment.Title;
            CategoryTitle = equipment.CategoryTitle;
            BrandModelText = equipment.BrandModelText;
            TypeTitle = equipment.TypeTitle;
            DescriptionText = equipment.DescriptionText;
            ImageUrl = equipment.ImageUrl;
            StartingPriceText = equipment.StartingPriceText;
            DepositText = equipment.DepositText;
            AvailabilityText = equipment.AvailabilityText;
            AccentColor = equipment.AccentColor;
            AccentSurfaceColor = equipment.AccentSurfaceColor;
            SymbolText = equipment.SymbolText;
            HasAvailableUnits = equipment.AvailableUnits > 0;

            ReplaceCollection(Rates, equipment.Rates);
            ReplaceCollection(RentalPoints, equipment.RentalPoints);
            OnPropertyChanged(nameof(HasRates));
            OnPropertyChanged(nameof(HasRentalPoints));
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasAvailableUnits = false;
            ReplaceCollection(Rates, []);
            ReplaceCollection(RentalPoints, []);
            OnPropertyChanged(nameof(HasRates));
            OnPropertyChanged(nameof(HasRentalPoints));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        foreach (T item in items)
        {
            collection.Add(item);
        }
    }
}
