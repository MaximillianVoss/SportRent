using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

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
    /// Returns from the profile screen to the catalog tab.
    /// </summary>
    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.Catalog, true);
    }

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

    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await ViewModel.InitializeAsync(forceRefresh: true);
    }

    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        ViewModel.SignOut();
    }
}
