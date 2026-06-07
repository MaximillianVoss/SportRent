using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

/// <summary>
/// Описывает чтение каталога инвентаря и детальных карточек из локальной базы.
/// </summary>
public interface ISportRentCatalogService
{
    /// <summary>
    /// Загружает полный снимок каталога: категории, карточки инвентаря и сводные метрики.
    /// </summary>
    Task<CatalogSnapshot> GetCatalogAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Загружает детальную карточку конкретного инвентаря с тарифами и пунктами проката.
    /// </summary>
    Task<EquipmentDetails?> GetEquipmentDetailsAsync(int equipmentId, CancellationToken cancellationToken = default);
}
