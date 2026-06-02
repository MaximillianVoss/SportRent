using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

public partial class CreateOrderPage : ContentPage, IQueryAttributable
{
    private int? _pendingEquipmentId;

    public CreateOrderPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<CreateOrderPageViewModel>();
    }

    private CreateOrderPageViewModel ViewModel => (CreateOrderPageViewModel)BindingContext;

    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//catalog", true);
    }

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_pendingEquipmentId.HasValue)
        {
            await ViewModel.LoadAsync(_pendingEquipmentId.Value);
        }
    }

    private void OnRateTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is RentalRateOptionViewModel option)
        {
            ViewModel.SelectRate(option);
        }
    }

    private void OnRentalPointTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is RentalPointOptionViewModel option)
        {
            ViewModel.SelectRentalPoint(option);
        }
    }

    private void OnIncreaseQuantityClicked(object? sender, EventArgs e)
    {
        ViewModel.IncreaseQuantity();
    }

    private void OnDecreaseQuantityClicked(object? sender, EventArgs e)
    {
        ViewModel.DecreaseQuantity();
    }

    private void OnIncreasePeriodClicked(object? sender, EventArgs e)
    {
        ViewModel.IncreasePeriodCount();
    }

    private void OnDecreasePeriodClicked(object? sender, EventArgs e)
    {
        ViewModel.DecreasePeriodCount();
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        int? orderId = await ViewModel.SubmitAsync();
        if (!orderId.HasValue)
        {
            return;
        }

        await DisplayAlert("Заказ оформлен", $"Заказ #{orderId.Value} добавлен в историю.", "OK");
        await Shell.Current.GoToAsync("//orders");
    }
}
