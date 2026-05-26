using SportRent.Mobile.Pages;
using SportRent.Mobile.Services;

namespace SportRent.Mobile;

public partial class App : Application
{
    private readonly IUserSessionService _userSessionService;
    private Window? _window;

    public App(IUserSessionService userSessionService)
    {
        InitializeComponent();
        _userSessionService = userSessionService;
        _userSessionService.SessionChanged += OnSessionChanged;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        _window = new Window(CreateRootPage());
        return _window;
    }

    private Page CreateRootPage()
    {
        if (_userSessionService.IsAuthenticated)
        {
            return new AppShell();
        }

        NavigationPage loginNavigation = new(new LoginPage())
        {
            BarBackgroundColor = Color.FromArgb("#163B2D"),
            BarTextColor = Colors.White
        };

        return loginNavigation;
    }

    private void OnSessionChanged(object? sender, EventArgs e)
    {
        if (_window is null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() => _window.Page = CreateRootPage());
    }
}
