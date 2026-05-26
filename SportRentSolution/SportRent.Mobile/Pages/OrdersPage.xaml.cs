using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Models;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

public partial class OrdersPage : ContentPage
{
    private bool _hasInitialized;

    public OrdersPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<OrdersPageViewModel>();
    }

    private OrdersPageViewModel ViewModel => (OrdersPageViewModel)BindingContext;

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

    private async void OnPayOrderClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not UserOrder order)
        {
            return;
        }

        bool isPaid = await ViewModel.PayOrderAsync(order);
        if (isPaid)
        {
            await DisplayAlert("Оплата выполнена", $"Заказ #{order.Id} оплачен тестовым платежом.", "OK");
        }
    }
}
