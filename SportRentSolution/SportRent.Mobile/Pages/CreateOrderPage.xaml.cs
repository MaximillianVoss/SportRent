using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

/// <summary>
/// Code-behind экрана оформления аренды: связывает элементы формы с CreateOrderPageViewModel.
/// </summary>
public partial class CreateOrderPage : ContentPage, IQueryAttributable
{
    private int? _pendingEquipmentId;

    public CreateOrderPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<CreateOrderPageViewModel>();
    }

    private CreateOrderPageViewModel ViewModel => (CreateOrderPageViewModel)BindingContext;

    /// <summary>
    /// Возвращает пользователя с оформления аренды к каталогу.
    /// </summary>
    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.Catalog, true);
    }

    /// <summary>
    /// Принимает equipmentId из Shell-навигации и откладывает загрузку до OnAppearing.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("equipmentId", out object? rawValue) || rawValue is null)
        {
            return;
        }

        if (rawValue is int equipmentId)
        {
            _pendingEquipmentId = equipmentId;
            return;
        }

        if (int.TryParse(rawValue.ToString(), out equipmentId))
        {
            _pendingEquipmentId = equipmentId;
        }
    }

    /// <summary>
    /// Загружает инвентарь после получения параметров маршрута.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_pendingEquipmentId.HasValue)
        {
            // Загрузка выполняется здесь, потому что query-параметры приходят до полного появления страницы.
            await ViewModel.LoadAsync(_pendingEquipmentId.Value);
        }
    }

    /// <summary>
    /// Обрабатывает выбор тарифа аренды.
    /// </summary>
    private void OnRateTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is RentalRateOptionViewModel option)
        {
            ViewModel.SelectRate(option);
        }
    }

    /// <summary>
    /// Обрабатывает выбор пункта проката.
    /// </summary>
    private void OnRentalPointTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is RentalPointOptionViewModel option)
        {
            ViewModel.SelectRentalPoint(option);
        }
    }

    /// <summary>
    /// Увеличивает количество единиц инвентаря в форме заказа.
    /// </summary>
    private void OnIncreaseQuantityClicked(object? sender, EventArgs e)
    {
        ViewModel.IncreaseQuantity();
    }

    /// <summary>
    /// Уменьшает количество единиц инвентаря в форме заказа.
    /// </summary>
    private void OnDecreaseQuantityClicked(object? sender, EventArgs e)
    {
        ViewModel.DecreaseQuantity();
    }

    /// <summary>
    /// Увеличивает количество периодов аренды.
    /// </summary>
    private void OnIncreasePeriodClicked(object? sender, EventArgs e)
    {
        ViewModel.IncreasePeriodCount();
    }

    /// <summary>
    /// Уменьшает количество периодов аренды.
    /// </summary>
    private void OnDecreasePeriodClicked(object? sender, EventArgs e)
    {
        ViewModel.DecreasePeriodCount();
    }

    /// <summary>
    /// Оформляет заказ и переводит пользователя в историю заказов.
    /// </summary>
    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        int? orderId = await ViewModel.SubmitAsync();
        if (!orderId.HasValue)
        {
            return;
        }

        await DisplayAlert("Заказ оформлен", $"Заказ #{orderId.Value} добавлен в историю.", "OK");
        await Shell.Current.GoToAsync(AppRoutes.Orders);
    }
}
