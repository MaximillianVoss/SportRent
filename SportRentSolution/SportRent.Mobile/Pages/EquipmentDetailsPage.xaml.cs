using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

public partial class EquipmentDetailsPage : ContentPage, IQueryAttributable
{
    private int? _pendingEquipmentId;

    public EquipmentDetailsPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetRequiredService<EquipmentDetailsPageViewModel>();
    }

    private EquipmentDetailsPageViewModel ViewModel => (EquipmentDetailsPageViewModel)BindingContext;

    /// <summary>
    /// Returns from the equipment card to the catalog tab.
    /// </summary>
    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.Catalog, true);
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

    private async void OnCreateOrderClicked(object? sender, EventArgs e)
    {
        if (ViewModel.EquipmentId <= 0)
        {
            return;
        }

        await Shell.Current.GoToAsync(nameof(CreateOrderPage), true, new Dictionary<string, object>
        {
            ["equipmentId"] = ViewModel.EquipmentId
        });
    }
}
