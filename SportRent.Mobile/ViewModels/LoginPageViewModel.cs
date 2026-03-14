using System.Collections.ObjectModel;
using SportRent.Mobile.Models;
using SportRent.Mobile.Services;

namespace SportRent.Mobile.ViewModels;

public sealed class LoginPageViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserSessionService _userSessionService;

    private bool _isBusy;
    private string _email = "ivan.petrov@sport-rent.local";
    private string _password = "client123";
    private string? _errorMessage;
    private bool _hasInitialized;

    public LoginPageViewModel(IAuthenticationService authenticationService, IUserSessionService userSessionService)
    {
        _authenticationService = authenticationService;
        _userSessionService = userSessionService;
        DemoAccounts = [];
    }

    public ObservableCollection<DemoAccount> DemoAccounts { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
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

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_hasInitialized)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            IReadOnlyList<DemoAccount> accounts = await _authenticationService.GetDemoAccountsAsync(cancellationToken);

            DemoAccounts.Clear();
            foreach (DemoAccount account in accounts)
            {
                DemoAccounts.Add(account);
            }

            if (DemoAccounts.Count > 0)
            {
                ApplyDemoAccount(DemoAccounts[0]);
            }

            _hasInitialized = true;
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

    public void ApplyDemoAccount(DemoAccount account)
    {
        Email = account.Email;
        Password = account.Password;
    }

    public async Task<bool> SignInAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            UserSession? session = await _authenticationService.SignInAsync(Email, Password, cancellationToken);
            if (session is null)
            {
                ErrorMessage = "Неверная почта или пароль.";
                return false;
            }

            _userSessionService.SetCurrentUser(session);
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
