using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public interface ISportRentCatalogService
{
    Task<CatalogSnapshot> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<EquipmentDetails?> GetEquipmentDetailsAsync(int equipmentId, CancellationToken cancellationToken = default);
}
