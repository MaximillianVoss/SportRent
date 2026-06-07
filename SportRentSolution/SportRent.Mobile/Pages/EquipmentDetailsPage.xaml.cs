using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile.Pages;

/// <summary>
/// Code-behind карточки инвентаря: получает route-параметры и запускает оформление аренды.
/// </summary>
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
    /// Возвращает пользователя из карточки инвентаря к каталогу.
    /// </summary>
    private async void OnBackToCatalogClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.Catalog, true);
    }

    /// <summary>
    /// Принимает equipmentId из Shell-навигации для последующей загрузки карточки.
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
    /// Загружает карточку инвентаря после появления страницы.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_pendingEquipmentId.HasValue)
        {
            // Загрузка выполняется после появления страницы, когда BindingContext уже готов к обновлению.
            await ViewModel.LoadAsync(_pendingEquipmentId.Value);
        }
    }

    /// <summary>
    /// Открывает форму оформления аренды для текущего инвентаря.
    /// </summary>
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
