using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Models;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

/// <summary>
/// Code-behind экрана входа: передает события формы в LoginPageViewModel.
/// </summary>
public partial class LoginPage : ContentPage
{
    private bool _hasInitialized;

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<LoginPageViewModel>();
    }

    private LoginPageViewModel ViewModel => (LoginPageViewModel)BindingContext;

    /// <summary>
    /// При первом показе загружает демо-аккаунты для проверки приложения.
    /// </summary>
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

    /// <summary>
    /// Обрабатывает кнопку входа в систему.
    /// </summary>
    private async void OnSignInClicked(object? sender, EventArgs e)
    {
        await ViewModel.SignInAsync();
    }

    /// <summary>
    /// Подставляет выбранный демо-аккаунт в форму входа.
    /// </summary>
    private void OnDemoAccountClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is DemoAccount account)
        {
            ViewModel.ApplyDemoAccount(account);
        }
    }
}
