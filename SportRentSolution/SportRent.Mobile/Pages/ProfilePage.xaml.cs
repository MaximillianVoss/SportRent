using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

/// <summary>
/// Code-behind профиля: обновляет данные пользователя и обрабатывает выход.
/// </summary>
public partial class ProfilePage : ContentPage
{
    private bool _hasInitialized;

    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<ProfilePageViewModel>();
    }

    private ProfilePageViewModel ViewModel => (ProfilePageViewModel)BindingContext;

    /// <summary>
    /// Возвращает пользователя из профиля к каталогу.
    /// </summary>
    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.Catalog, true);
    }

    /// <summary>
    /// Загружает профиль при открытии экрана и обновляет его при повторном показе.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasInitialized)
        {
            await ViewModel.InitializeAsync(forceRefresh: true);
            return;
        }

        _hasInitialized = true;
        await ViewModel.InitializeAsync();
    }

    /// <summary>
    /// Обновляет профиль по жесту Pull-to-refresh.
    /// </summary>
    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await ViewModel.InitializeAsync(forceRefresh: true);
    }

    /// <summary>
    /// Завершает текущую сессию пользователя.
    /// </summary>
    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        ViewModel.SignOut();
    }
}
