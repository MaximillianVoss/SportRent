using SportRent.Mobile.Services;

namespace SportRent.Mobile.ViewModels;

public sealed class ProfilePageViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserSessionService _userSessionService;

    private bool _isBusy;
    private string _displayName = string.Empty;
    private string _roleTitle = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string _memberSinceText = string.Empty;
    private int _totalOrders;
    private int _activeOrders;
    private int _completedOrders;
    private string _totalPaidText = string.Empty;
    private string _outstandingText = string.Empty;
    private string? _errorMessage;

    public ProfilePageViewModel(IAuthenticationService authenticationService, IUserSessionService userSessionService)
    {
        _authenticationService = authenticationService;
        _userSessionService = userSessionService;
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string DisplayName
    {
        get => _displayName;
        private set => SetProperty(ref _displayName, value);
    }

    public string RoleTitle
    {
        get => _roleTitle;
        private set => SetProperty(ref _roleTitle, value);
    }

    public string Email
    {
        get => _email;
        private set => SetProperty(ref _email, value);
    }

    public string Phone
    {
        get => _phone;
        private set => SetProperty(ref _phone, value);
    }

    public string MemberSinceText
    {
        get => _memberSinceText;
        private set => SetProperty(ref _memberSinceText, value);
    }

    public int TotalOrders
    {
        get => _totalOrders;
        private set => SetProperty(ref _totalOrders, value);
    }

    public int ActiveOrders
    {
        get => _activeOrders;
        private set => SetProperty(ref _activeOrders, value);
    }

    public int CompletedOrders
    {
        get => _completedOrders;
        private set => SetProperty(ref _completedOrders, value);
    }

    public string TotalPaidText
    {
        get => _totalPaidText;
        private set => SetProperty(ref _totalPaidText, value);
    }

    public string OutstandingText
    {
        get => _outstandingText;
        private set => SetProperty(ref _outstandingText, value);
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

        if (_userSessionService.CurrentUser is null)
        {
            ErrorMessage = "Пользовательская сессия отсутствует.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            Models.UserProfile? profile = await _authenticationService.GetProfileAsync(_userSessionService.CurrentUser.UserId, cancellationToken);
            if (profile is null)
            {
                ErrorMessage = "Профиль пользователя не найден.";
                return;
            }

            DisplayName = profile.DisplayName;
            RoleTitle = profile.RoleTitle;
            Email = profile.Email;
            Phone = profile.Phone;
            MemberSinceText = profile.MemberSinceText;
            TotalOrders = profile.TotalOrders;
            ActiveOrders = profile.ActiveOrders;
            CompletedOrders = profile.CompletedOrders;
            TotalPaidText = profile.TotalPaidText;
            OutstandingText = profile.OutstandingText;
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

    public void SignOut()
    {
        _userSessionService.SignOut();
    }
}
