using Microsoft.Data.Sqlite;
using SportRent.Mobile.Infrastructure;
using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public sealed class SportRentCatalogService : SqliteServiceBase, ISportRentCatalogService
{
    public SportRentCatalogService(ILocalDatabaseService localDatabaseService)
        : base(localDatabaseService)
    {
    }

    public async Task<CatalogSnapshot> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        IReadOnlyList<CatalogCategory> categories = await LoadCategoriesAsync(connection, cancellationToken);
        IReadOnlyList<CatalogEquipmentItem> equipment = await LoadEquipmentAsync(connection, cancellationToken);
        CatalogStats stats = await LoadStatsAsync(connection, cancellationToken);

        return new CatalogSnapshot
        {
            Categories = categories,
            Equipment = equipment,
            Stats = stats
        };
    }

    public async Task<EquipmentDetails?> GetEquipmentDetailsAsync(int equipmentId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        const string detailsSql = """
            SELECT
                e.id,
                e.idCategory,
                e.title,
                c.title AS categoryTitle,
                b.title AS brandTitle,
                et.title AS typeTitle,
                e.model,
                e.description,
                (
                    SELECT i.url
                    FROM equipmentPhotos ep
                    INNER JOIN images i ON i.id = ep.idImage
                    WHERE ep.idEquipment = e.id
                    ORDER BY ep.id
                    LIMIT 1
                ) AS imageUrl,
                COALESCE((
                    SELECT MIN(er.price)
                    FROM equipmentRates er
                    WHERE er.idEquipment = e.id
                ), 0) AS minPrice,
                COALESCE((
                    SELECT rt.title
                    FROM equipmentRates er
                    INNER JOIN rentalTypes rt ON rt.id = er.idRentalType
                    WHERE er.idEquipment = e.id
                    ORDER BY er.price ASC, rt.unitHours ASC
                    LIMIT 1
                ), '') AS minPriceType,
                COALESCE((
                    SELECT MAX(er.deposit)
                    FROM equipmentRates er
                    WHERE er.idEquipment = e.id
                ), 0) AS maxDeposit,
                COALESCE((
                    SELECT SUM(rpe.availableQuantity)
                    FROM rentalPointEquipment rpe
                    WHERE rpe.idEquipment = e.id
                ), 0) AS availableUnits,
                COALESCE((
                    SELECT COUNT(DISTINCT rpe.idRentalPoint)
                    FROM rentalPointEquipment rpe
                    WHERE rpe.idEquipment = e.id
                      AND rpe.availableQuantity > 0
                ), 0) AS rentalPointCount
            FROM equipment e
            INNER JOIN categories c ON c.id = e.idCategory
            INNER JOIN brands b ON b.id = e.idBrand
            INNER JOIN equipmentTypes et ON et.id = e.idType
            WHERE e.id = $equipmentId;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = detailsSql;
        command.Parameters.AddWithValue("$equipmentId", equipmentId);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        string categoryTitle = reader.GetString(reader.GetOrdinal("categoryTitle"));
        string typeTitle = reader.GetString(reader.GetOrdinal("typeTitle"));
        (string accentColor, string accentSurfaceColor, string symbolText) = EquipmentVisualIdentity.Resolve(categoryTitle, typeTitle);

        IReadOnlyList<EquipmentRate> rates = await LoadRatesAsync(connection, equipmentId, cancellationToken);
        IReadOnlyList<RentalPointAvailability> rentalPoints = await LoadRentalPointsAsync(connection, equipmentId, cancellationToken);

        return new EquipmentDetails
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            CategoryId = reader.GetInt32(reader.GetOrdinal("idCategory")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            CategoryTitle = categoryTitle,
            BrandTitle = reader.GetString(reader.GetOrdinal("brandTitle")),
            TypeTitle = typeTitle,
            Model = reader.IsDBNull(reader.GetOrdinal("model")) ? null : reader.GetString(reader.GetOrdinal("model")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
            ImageUrl = reader.IsDBNull(reader.GetOrdinal("imageUrl")) ? null : reader.GetString(reader.GetOrdinal("imageUrl")),
            StartingPrice = reader.GetInt32(reader.GetOrdinal("minPrice")),
            StartingRentalTypeTitle = reader.GetString(reader.GetOrdinal("minPriceType")),
            Deposit = reader.GetInt32(reader.GetOrdinal("maxDeposit")),
            AvailableUnits = reader.GetInt32(reader.GetOrdinal("availableUnits")),
            RentalPointCount = reader.GetInt32(reader.GetOrdinal("rentalPointCount")),
            AccentColor = accentColor,
            AccentSurfaceColor = accentSurfaceColor,
            SymbolText = symbolText,
            Rates = rates,
            RentalPoints = rentalPoints
        };
    }

    private static async Task<IReadOnlyList<CatalogCategory>> LoadCategoriesAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                c.id,
                c.title,
                COALESCE(c.description, '') AS description
            FROM categories c
            ORDER BY c.title;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        List<CatalogCategory> categories = [];
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            categories.Add(new CatalogCategory
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Description = reader.GetString(reader.GetOrdinal("description"))
            });
        }

        return categories;
    }

    private static async Task<IReadOnlyList<CatalogEquipmentItem>> LoadEquipmentAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                e.id,
                e.idCategory,
                e.title,
                c.title AS categoryTitle,
                b.title AS brandTitle,
                et.title AS typeTitle,
                e.model,
                e.description,
                (
                    SELECT i.url
                    FROM equipmentPhotos ep
                    INNER JOIN images i ON i.id = ep.idImage
                    WHERE ep.idEquipment = e.id
                    ORDER BY ep.id
                    LIMIT 1
                ) AS imageUrl,
                COALESCE((
                    SELECT MIN(er.price)
                    FROM equipmentRates er
                    WHERE er.idEquipment = e.id
                ), 0) AS minPrice,
                COALESCE((
                    SELECT rt.title
                    FROM equipmentRates er
                    INNER JOIN rentalTypes rt ON rt.id = er.idRentalType
                    WHERE er.idEquipment = e.id
                    ORDER BY er.price ASC, rt.unitHours ASC
                    LIMIT 1
                ), '') AS minPriceType,
                COALESCE((
                    SELECT MAX(er.deposit)
                    FROM equipmentRates er
                    WHERE er.idEquipment = e.id
                ), 0) AS maxDeposit,
                COALESCE((
                    SELECT SUM(rpe.availableQuantity)
                    FROM rentalPointEquipment rpe
                    WHERE rpe.idEquipment = e.id
                ), 0) AS availableUnits,
                COALESCE((
                    SELECT COUNT(DISTINCT rpe.idRentalPoint)
                    FROM rentalPointEquipment rpe
                    WHERE rpe.idEquipment = e.id
                      AND rpe.availableQuantity > 0
                ), 0) AS rentalPointCount
            FROM equipment e
            INNER JOIN categories c ON c.id = e.idCategory
            INNER JOIN brands b ON b.id = e.idBrand
            INNER JOIN equipmentTypes et ON et.id = e.idType
            ORDER BY c.title, e.title;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        List<CatalogEquipmentItem> items = [];
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            string categoryTitle = reader.GetString(reader.GetOrdinal("categoryTitle"));
            string typeTitle = reader.GetString(reader.GetOrdinal("typeTitle"));
            (string accentColor, string accentSurfaceColor, string symbolText) = EquipmentVisualIdentity.Resolve(categoryTitle, typeTitle);

            items.Add(new CatalogEquipmentItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("idCategory")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                CategoryTitle = categoryTitle,
                BrandTitle = reader.GetString(reader.GetOrdinal("brandTitle")),
                TypeTitle = typeTitle,
                Model = reader.IsDBNull(reader.GetOrdinal("model")) ? null : reader.GetString(reader.GetOrdinal("model")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("imageUrl")) ? null : reader.GetString(reader.GetOrdinal("imageUrl")),
                StartingPrice = reader.GetInt32(reader.GetOrdinal("minPrice")),
                StartingRentalTypeTitle = reader.GetString(reader.GetOrdinal("minPriceType")),
                Deposit = reader.GetInt32(reader.GetOrdinal("maxDeposit")),
                AvailableUnits = reader.GetInt32(reader.GetOrdinal("availableUnits")),
                RentalPointCount = reader.GetInt32(reader.GetOrdinal("rentalPointCount")),
                AccentColor = accentColor,
                AccentSurfaceColor = accentSurfaceColor,
                SymbolText = symbolText
            });
        }

        return items;
    }

    private static async Task<CatalogStats> LoadStatsAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                (SELECT COUNT(*) FROM equipment) AS totalEquipment,
                (SELECT COUNT(*) FROM rentalPoints) AS totalRentalPoints,
                (SELECT COUNT(*) FROM categories) AS totalCategories;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return new CatalogStats
        {
            TotalEquipment = reader.GetInt32(reader.GetOrdinal("totalEquipment")),
            TotalRentalPoints = reader.GetInt32(reader.GetOrdinal("totalRentalPoints")),
            TotalCategories = reader.GetInt32(reader.GetOrdinal("totalCategories"))
        };
    }

    private static async Task<IReadOnlyList<EquipmentRate>> LoadRatesAsync(SqliteConnection connection, int equipmentId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                rt.title,
                rt.code,
                rt.unitHours,
                er.price,
                er.deposit
            FROM equipmentRates er
            INNER JOIN rentalTypes rt ON rt.id = er.idRentalType
            WHERE er.idEquipment = $equipmentId
            ORDER BY rt.unitHours;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$equipmentId", equipmentId);

        List<EquipmentRate> rates = [];
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            rates.Add(new EquipmentRate
            {
                Title = reader.GetString(reader.GetOrdinal("title")),
                Code = reader.GetString(reader.GetOrdinal("code")),
                UnitHours = reader.GetInt32(reader.GetOrdinal("unitHours")),
                Price = reader.GetInt32(reader.GetOrdinal("price")),
                Deposit = reader.GetInt32(reader.GetOrdinal("deposit"))
            });
        }

        return rates;
    }

    private static async Task<IReadOnlyList<RentalPointAvailability>> LoadRentalPointsAsync(SqliteConnection connection, int equipmentId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                rpe.id,
                rpe.idRentalPoint,
                rp.name AS rentalPointName,
                rp.address,
                rp.phone,
                ec.title AS conditionTitle,
                es.title AS sizeTitle,
                rpe.availableQuantity,
                rpe.totalQuantity
            FROM rentalPointEquipment rpe
            INNER JOIN rentalPoints rp ON rp.id = rpe.idRentalPoint
            INNER JOIN equipmentConditions ec ON ec.id = rpe.idEquipmentCondition
            INNER JOIN equipmentSizes es ON es.id = rpe.idSize
            WHERE rpe.idEquipment = $equipmentId
            ORDER BY rpe.availableQuantity DESC, rp.name, es.title;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$equipmentId", equipmentId);

        List<RentalPointAvailability> rentalPoints = [];
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            rentalPoints.Add(new RentalPointAvailability
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                RentalPointId = reader.GetInt32(reader.GetOrdinal("idRentalPoint")),
                RentalPointName = reader.GetString(reader.GetOrdinal("rentalPointName")),
                Address = reader.GetString(reader.GetOrdinal("address")),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                ConditionTitle = reader.GetString(reader.GetOrdinal("conditionTitle")),
                SizeTitle = reader.GetString(reader.GetOrdinal("sizeTitle")),
                AvailableQuantity = reader.GetInt32(reader.GetOrdinal("availableQuantity")),
                TotalQuantity = reader.GetInt32(reader.GetOrdinal("totalQuantity"))
            });
        }

        return rentalPoints;
    }

}
