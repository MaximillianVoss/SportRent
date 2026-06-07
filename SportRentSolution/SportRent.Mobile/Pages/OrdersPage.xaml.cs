using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.Models;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

/// <summary>
/// Code-behind истории заказов: передает команды оплаты и отмены в OrdersPageViewModel.
/// </summary>
public partial class OrdersPage : ContentPage
{
    private bool _hasInitialized;

    public OrdersPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<OrdersPageViewModel>();
    }

    private OrdersPageViewModel ViewModel => (OrdersPageViewModel)BindingContext;

    /// <summary>
    /// Возвращает пользователя из истории заказов к каталогу.
    /// </summary>
    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.Catalog, true);
    }

    /// <summary>
    /// Загружает или обновляет историю заказов при открытии экрана.
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
    /// Обновляет список заказов по жесту Pull-to-refresh.
    /// </summary>
    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await ViewModel.InitializeAsync(forceRefresh: true);
    }

    /// <summary>
    /// Запускает тестовую оплату выбранного заказа.
    /// </summary>
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

    /// <summary>
    /// Запрашивает подтверждение и отменяет неоплаченный заказ.
    /// </summary>
    private async void OnCancelOrderClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not UserOrder order)
        {
            return;
        }

        bool shouldCancel = await DisplayAlert(
            "Отмена заказа",
            $"Отменить заказ #{order.Id}? Инвентарь снова станет доступен в каталоге.",
            "Отменить",
            "Назад");

        if (!shouldCancel)
        {
            return;
        }

        // ViewModel проверит статус заказа и вызовет сервис, который вернет остаток в каталог.
        bool isCanceled = await ViewModel.CancelOrderAsync(order);
        if (isCanceled)
        {
            await DisplayAlert("Заказ отменен", $"Заказ #{order.Id} отменен.", "OK");
        }
    }
}
