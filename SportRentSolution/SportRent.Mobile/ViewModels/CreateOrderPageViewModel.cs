using System.Collections.ObjectModel;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.Models;
using SportRent.Mobile.Services;

namespace SportRent.Mobile.ViewModels;

/// <summary>
/// ViewModel экрана оформления аренды с выбором тарифа, пункта проката и периода.
/// </summary>
public sealed class CreateOrderPageViewModel : ViewModelBase
{
    private readonly ISportRentCatalogService _catalogService;
    private readonly IOrdersService _ordersService;
    private readonly IUserSessionService _userSessionService;

    private bool _isBusy;
    private int? _loadedEquipmentId;
    private int _equipmentId;
    private string _title = string.Empty;
    private string _categoryTitle = string.Empty;
    private string _brandModelText = string.Empty;
    private string _descriptionText = string.Empty;
    private string? _imageUrl;
    private string _accentColor = "#163B2D";
    private string _accentSurfaceColor = "#E5F0EA";
    private string _symbolText = "SR";
    private DateTime _startDate = DateTime.Today.AddDays(1);
    private TimeSpan _startTime = new(10, 0, 0);
    private int _quantity = 1;
    private int _periodCount = 1;
    private string _endDateText = string.Empty;
    private string _rentalAmountText = string.Empty;
    private string _depositText = string.Empty;
    private string _totalPaymentText = string.Empty;
    private string? _errorMessage;
    private string _successMessage = string.Empty;

    public CreateOrderPageViewModel(
        ISportRentCatalogService catalogService,
        IOrdersService ordersService,
        IUserSessionService userSessionService)
    {
        _catalogService = catalogService;
        _ordersService = ordersService;
        _userSessionService = userSessionService;
        Rates = [];
        RentalPoints = [];
    }

    public ObservableCollection<RentalRateOptionViewModel> Rates { get; }

    public ObservableCollection<RentalPointOptionViewModel> RentalPoints { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
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

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (!SetProperty(ref _startDate, value))
            {
                return;
            }

            RecalculateSummary();
        }
    }

    public TimeSpan StartTime
    {
        get => _startTime;
        set
        {
            if (!SetProperty(ref _startTime, value))
            {
                return;
            }

            RecalculateSummary();
        }
    }

    public int Quantity
    {
        get => _quantity;
        private set
        {
            if (!SetProperty(ref _quantity, value))
            {
                return;
            }

            OnPropertyChanged(nameof(QuantityText));
            RecalculateSummary();
        }
    }

    public int PeriodCount
    {
        get => _periodCount;
        private set
        {
            if (!SetProperty(ref _periodCount, value))
            {
                return;
            }

            OnPropertyChanged(nameof(PeriodCountText));
            RecalculateSummary();
        }
    }

    public string QuantityText => Quantity.ToString();

    public string PeriodCountText => PeriodCount.ToString();

    public string EndDateText
    {
        get => _endDateText;
        private set => SetProperty(ref _endDateText, value);
    }

    public string RentalAmountText
    {
        get => _rentalAmountText;
        private set => SetProperty(ref _rentalAmountText, value);
    }

    public string DepositText
    {
        get => _depositText;
        private set => SetProperty(ref _depositText, value);
    }

    public string TotalPaymentText
    {
        get => _totalPaymentText;
        private set => SetProperty(ref _totalPaymentText, value);
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

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    /// <summary>
    /// Загружает данные инвентаря, доступные тарифы и пункты проката для оформления заказа.
    /// </summary>
    public async Task LoadAsync(int equipmentId, CancellationToken cancellationToken = default)
    {
        if (IsBusy || _loadedEquipmentId == equipmentId)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            SuccessMessage = string.Empty;

            EquipmentDetails? equipment = await _catalogService.GetEquipmentDetailsAsync(equipmentId, cancellationToken);
            if (equipment is null)
            {
                ErrorMessage = "Инвентарь не найден.";
                return;
            }

            _loadedEquipmentId = equipmentId;
            _equipmentId = equipment.Id;
            Title = equipment.Title;
            CategoryTitle = equipment.CategoryTitle;
            BrandModelText = equipment.BrandModelText;
            DescriptionText = equipment.DescriptionText;
            ImageUrl = equipment.ImageUrl;
            AccentColor = equipment.AccentColor;
            AccentSurfaceColor = equipment.AccentSurfaceColor;
            SymbolText = equipment.SymbolText;

            // Тарифы и пункты проката пересоздаются для выбранной карточки инвентаря.
            Rates.Clear();
            foreach (EquipmentRate rate in equipment.Rates)
            {
                Rates.Add(new RentalRateOptionViewModel
                {
                    Rate = rate,
                    IsSelected = false
                });
            }

            RentalPoints.Clear();
            foreach (RentalPointAvailability rentalPoint in equipment.RentalPoints.Where(point => point.IsAvailable))
            {
                RentalPoints.Add(new RentalPointOptionViewModel
                {
                    Availability = rentalPoint,
                    IsSelected = false
                });
            }

            if (Rates.Count > 0)
            {
                SelectRate(Rates[0]);
            }

            if (RentalPoints.Count > 0)
            {
                SelectRentalPoint(RentalPoints[0]);
            }

            Quantity = 1;
            PeriodCount = 1;
            RecalculateSummary();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Выбирает тариф аренды и пересчитывает итоговую стоимость заказа.
    /// </summary>
    public void SelectRate(RentalRateOptionViewModel option)
    {
        foreach (RentalRateOptionViewModel rate in Rates)
        {
            rate.IsSelected = ReferenceEquals(rate, option);
        }

        RecalculateSummary();
    }

    /// <summary>
    /// Выбирает пункт проката и ограничивает количество доступным остатком.
    /// </summary>
    public void SelectRentalPoint(RentalPointOptionViewModel option)
    {
        foreach (RentalPointOptionViewModel rentalPoint in RentalPoints)
        {
            rentalPoint.IsSelected = ReferenceEquals(rentalPoint, option);
        }

        if (Quantity > GetMaxQuantity())
        {
            Quantity = GetMaxQuantity();
        }

        RecalculateSummary();
    }

    public void IncreaseQuantity()
    {
        if (Quantity < GetMaxQuantity())
        {
            Quantity++;
        }
    }

    public void DecreaseQuantity()
    {
        if (Quantity > 1)
        {
            Quantity--;
        }
    }

    public void IncreasePeriodCount()
    {
        if (PeriodCount < 30)
        {
            PeriodCount++;
        }
    }

    public void DecreasePeriodCount()
    {
        if (PeriodCount > 1)
        {
            PeriodCount--;
        }
    }

    /// <summary>
    /// Проверяет форму и создает заказ через сервис заказов.
    /// </summary>
    public async Task<int?> SubmitAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return null;
        }

        UserSession? currentUser = _userSessionService.CurrentUser;
        RentalRateOptionViewModel? selectedRate = Rates.FirstOrDefault(rate => rate.IsSelected);
        RentalPointOptionViewModel? selectedPoint = RentalPoints.FirstOrDefault(point => point.IsSelected);

        if (currentUser is null)
        {
            ErrorMessage = "Сессия пользователя завершилась. Войдите снова.";
            return null;
        }

        if (selectedRate is null || selectedPoint is null)
        {
            ErrorMessage = "Выберите тариф и пункт проката.";
            return null;
        }

        DateTime startAt = StartDate.Date.Add(StartTime);
        if (startAt < DateTime.Now.AddMinutes(-5))
        {
            ErrorMessage = "Дата начала аренды должна быть не раньше текущего времени.";
            return null;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            SuccessMessage = string.Empty;

            // Сервис создает заказ, резервирует остаток и добавляет мок-платеж в одной транзакции.
            int orderId = await _ordersService.CreateOrderAsync(new CreateOrderRequest
            {
                UserId = currentUser.UserId,
                EquipmentId = _equipmentId,
                EquipmentTitle = Title,
                RentalPointEquipmentId = selectedPoint.RentalPointEquipmentId,
                Quantity = Quantity,
                UnitHours = selectedRate.Rate.UnitHours,
                PeriodCount = PeriodCount,
                PricePerUnit = selectedRate.Rate.Price,
                DepositPerUnit = selectedRate.Rate.Deposit,
                StartAt = startAt,
                Description = $"Заказ через мобильное приложение: {Title}"
            }, cancellationToken);

            SuccessMessage = $"Заказ #{orderId} оформлен.";
            return orderId;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Пересчитывает дату окончания, стоимость аренды, залог и итоговый платеж.
    /// </summary>
    private void RecalculateSummary()
    {
        RentalRateOptionViewModel? selectedRate = Rates.FirstOrDefault(rate => rate.IsSelected);
        if (selectedRate is null)
        {
            EndDateText = "Выберите тариф";
            RentalAmountText = "-";
            DepositText = "-";
            TotalPaymentText = "-";
            return;
        }

        DateTime startAt = StartDate.Date.Add(StartTime);
        DateTime endAt = startAt.AddHours(selectedRate.Rate.UnitHours * PeriodCount);
        int rentalAmount = selectedRate.Rate.Price * PeriodCount * Quantity;
        int depositAmount = selectedRate.Rate.Deposit * Quantity;

        EndDateText = DisplayFormatter.ToDateTime(endAt);
        RentalAmountText = DisplayFormatter.ToCurrency(rentalAmount);
        DepositText = depositAmount > 0 ? DisplayFormatter.ToCurrency(depositAmount) : "Без залога";
        TotalPaymentText = DisplayFormatter.ToCurrency(rentalAmount + depositAmount);
    }

    /// <summary>
    /// Возвращает максимально доступное количество для выбранного пункта проката.
    /// </summary>
    private int GetMaxQuantity()
    {
        RentalPointOptionViewModel? selectedPoint = RentalPoints.FirstOrDefault(point => point.IsSelected);
        return Math.Max(1, selectedPoint?.AvailableQuantity ?? 1);
    }
}
