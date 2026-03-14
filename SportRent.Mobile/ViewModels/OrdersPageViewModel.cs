using System.Collections.ObjectModel;
using SportRent.Mobile.Models;
using SportRent.Mobile.Services;

namespace SportRent.Mobile.ViewModels;

public sealed class OrdersPageViewModel : ViewModelBase
{
    private readonly IOrdersService _ordersService;
    private readonly IUserSessionService _userSessionService;

    private bool _isBusy;
    private string _summaryText = "Загрузка заказов...";
    private string? _errorMessage;

    public OrdersPageViewModel(IOrdersService ordersService, IUserSessionService userSessionService)
    {
        _ordersService = ordersService;
        _userSessionService = userSessionService;
        Orders = [];
    }

    public ObservableCollection<UserOrder> Orders { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string SummaryText
    {
        get => _summaryText;
        private set => SetProperty(ref _summaryText, value);
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

    public async Task InitializeAsync(bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        if (IsBusy && !forceRefresh)
        {
            return;
        }

        UserSession? currentUser = _userSessionService.CurrentUser;
        if (currentUser is null)
        {
            Orders.Clear();
            SummaryText = "Сначала войдите в аккаунт.";
            ErrorMessage = "Пользовательская сессия отсутствует.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            IReadOnlyList<UserOrder> orders = await _ordersService.GetOrdersAsync(currentUser.UserId, cancellationToken);

            Orders.Clear();
            foreach (UserOrder order in orders)
            {
                Orders.Add(order);
            }

            SummaryText = orders.Count switch
            {
                0 => "Пока нет оформленных заказов",
                1 => "1 заказ в истории",
                _ => $"{orders.Count} заказов в истории"
            };
        }
        catch (Exception ex)
        {
            Orders.Clear();
            SummaryText = "Заказы недоступны";
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
