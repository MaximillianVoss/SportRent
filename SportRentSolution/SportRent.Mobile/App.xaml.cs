using SportRent.Mobile.Pages;
using SportRent.Mobile.Services;

namespace SportRent.Mobile;

/// <summary>
/// Корневой класс MAUI-приложения, переключающий вход и основной Shell по состоянию сессии.
/// </summary>
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

    /// <summary>
    /// Создает главное окно приложения с актуальной корневой страницей.
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _window = new Window(CreateRootPage());
        return _window;
    }

    /// <summary>
    /// Выбирает корневую страницу: экран входа или основной Shell приложения.
    /// </summary>
    private Page CreateRootPage()
    {
        if (_userSessionService.IsAuthenticated)
        {
            return new AppShell();
        }

        // До входа пользователь видит LoginPage, а после авторизации AppShell открывается автоматически.
        NavigationPage loginNavigation = new(new LoginPage())
        {
            BarBackgroundColor = Color.FromArgb("#163B2D"),
            BarTextColor = Colors.White
        };

        return loginNavigation;
    }

    /// <summary>
    /// Реагирует на вход или выход пользователя и перестраивает корневую страницу.
    /// </summary>
    private void OnSessionChanged(object? sender, EventArgs e)
    {
        if (_window is null)
        {
            return;
        }

        // Событие может прийти не из UI-потока, поэтому переключение страницы выполняется на главном потоке.
        MainThread.BeginInvokeOnMainThread(() => _window.Page = CreateRootPage());
    }
}
