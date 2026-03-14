using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Models;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private bool _hasInitialized;

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<LoginPageViewModel>();
    }

    private LoginPageViewModel ViewModel => (LoginPageViewModel)BindingContext;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasInitialized)
        {
            return;
        }

        _hasInitialized = true;
        await ViewModel.InitializeAsync();
    }

    private async void OnSignInClicked(object? sender, EventArgs e)
    {
        await ViewModel.SignInAsync();
    }

    private void OnDemoAccountClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is DemoAccount account)
        {
            ViewModel.ApplyDemoAccount(account);
        }
    }
}
