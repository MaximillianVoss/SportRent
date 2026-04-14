PRAGMA foreign_keys = ON;

-- =========================
-- Очистка таблиц для повторного запуска скрипта
-- =========================
DELETE FROM payments;
DELETE FROM orderItems;
DELETE FROM rentOrders;
DELETE FROM rentalPointEquipment;
DELETE FROM equipmentRates;
DELETE FROM equipmentPhotos;
DELETE FROM equipment;
DELETE FROM images;
DELETE FROM rentalPoints;
DELETE FROM users;
DELETE FROM rentalTypes;
DELETE FROM equipmentSizes;
DELETE FROM equipmentConditions;
DELETE FROM equipmentType;
DELETE FROM brands;
DELETE FROM categories;
DELETE FROM paymentStatuses;
DELETE FROM paymentMethods;
DELETE FROM orderStatus;
DELETE FROM roles;

DELETE FROM sqlite_sequence
WHERE
    name IN (
        'roles',
        'orderStatus',
        'paymentMethods',
        'paymentStatuses',
        'categories',
        'brands',
        'equipmentType',
        'equipmentConditions',
        'equipmentSizes',
        'rentalTypes',
        'users',
        'rentalPoints',
        'images',
        'equipment',
        'equipmentPhotos',
        'equipmentRates',
        'rentalPointEquipment',
        'rentOrders',
        'orderItems',
        'payments'
    );

-- =========================
-- Справочники: не менее 20 записей в каждой таблице
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    roles (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Admin'
        WHEN 2 THEN 'Manager'
        WHEN 3 THEN 'Client'
        ELSE 'Role ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    orderStatus (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Created'
        WHEN 2 THEN 'Confirmed'
        WHEN 3 THEN 'In progress'
        WHEN 4 THEN 'Completed'
        WHEN 5 THEN 'Cancelled'
        ELSE 'Status ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    paymentMethods (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Cash'
        WHEN 2 THEN 'Card'
        WHEN 3 THEN 'Online'
        ELSE 'Method ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    paymentStatuses (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Pending'
        WHEN 2 THEN 'Paid'
        WHEN 3 THEN 'Failed'
        WHEN 4 THEN 'Refunded'
        ELSE 'Payment status ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    categories (id, title, description)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Ski'
        WHEN 2 THEN 'Snowboard'
        WHEN 3 THEN 'Bicycle'
        WHEN 4 THEN 'Protection'
        ELSE 'Category ' || printf('%02d', n)
    END,
    CASE n
        WHEN 1 THEN 'Ski equipment and accessories'
        WHEN 2 THEN 'Snowboard equipment and accessories'
        WHEN 3 THEN 'Bicycles and riding accessories'
        WHEN 4 THEN 'Helmets, protection and safety gear'
        ELSE 'Seed category description ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    brands (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Rossignol'
        WHEN 2 THEN 'Burton'
        WHEN 3 THEN 'Trek'
        WHEN 4 THEN 'Scott'
        WHEN 5 THEN 'Atomic'
        ELSE 'Brand ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    equipmentType (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Adult'
        WHEN 2 THEN 'Child'
        WHEN 3 THEN 'Universal'
        ELSE 'Type ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    equipmentConditions (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'New'
        WHEN 2 THEN 'Good'
        WHEN 3 THEN 'Used'
        ELSE 'Condition ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    equipmentSizes (id, title)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'XS'
        WHEN 2 THEN 'S'
        WHEN 3 THEN 'M'
        WHEN 4 THEN 'L'
        WHEN 5 THEN 'XL'
        ELSE 'Size ' || printf('%02d', n)
    END
FROM
    seq;

WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    rentalTypes (id, title, code, unitHours)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'Hourly'
        WHEN 2 THEN 'Half day'
        WHEN 3 THEN 'Daily'
        WHEN 4 THEN 'Weekend'
        WHEN 5 THEN 'Weekly'
        ELSE 'Package ' || printf('%02d', n)
    END,
    CASE n
        WHEN 1 THEN 'HOUR'
        WHEN 2 THEN 'HALF_DAY'
        WHEN 3 THEN 'DAY'
        WHEN 4 THEN 'WEEKEND'
        WHEN 5 THEN 'WEEK'
        ELSE 'PKG_' || printf('%02d', n)
    END,
    CASE n
        WHEN 1 THEN 1
        WHEN 2 THEN 12
        WHEN 3 THEN 24
        WHEN 4 THEN 48
        WHEN 5 THEN 168
        ELSE 24 + n * 6
    END
FROM
    seq;

-- =========================
-- Пользователи
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    users (
        id,
        idRole,
        firstName,
        lastName,
        phone,
        passwordHash,
        dateCreated
    )
SELECT
    n,
    n,
    CASE n
        WHEN 1 THEN 'Ivan'
        WHEN 2 THEN 'Petr'
        WHEN 3 THEN 'Anna'
        WHEN 4 THEN 'Olga'
        ELSE 'User' || printf('%02d', n)
    END,
    CASE n
        WHEN 1 THEN 'Ivanov'
        WHEN 2 THEN 'Petrov'
        WHEN 3 THEN 'Smirnova'
        WHEN 4 THEN 'Sokolova'
        ELSE 'Seed' || printf('%02d', n)
    END,
    '+7999' || printf('%07d', n),
    'hash_user_' || printf('%02d', n),
    datetime ('now', '-' || (21 - n) || ' day')
FROM
    seq;

-- =========================
-- Точки аренды
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    rentalPoints (id, name, address, phone)
SELECT
    n,
    CASE n
        WHEN 1 THEN 'SportRent Center'
        WHEN 2 THEN 'SportRent North'
        WHEN 3 THEN 'SportRent Park'
        ELSE 'SportRent Point ' || printf('%02d', n)
    END,
    'Moscow, Seed street ' || n,
    '+7495' || printf('%07d', n)
FROM
    seq;

-- =========================
-- Изображения
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    images (id, url, dateCreated)
SELECT
    n,
    'images/equipment_' || printf('%02d', n) || '.jpg',
    datetime ('now', '-' || (20 - n) || ' day')
FROM
    seq;

-- =========================
-- Оборудование
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    equipment (
        id,
        idCategory,
        idBrand,
        idType,
        title,
        description,
        model
    )
SELECT
    n,
    n,
    n,
    n,
    CASE n
        WHEN 1 THEN 'Ski Set Pro'
        WHEN 2 THEN 'Snowboard X'
        WHEN 3 THEN 'Trek 3500'
        WHEN 4 THEN 'Safe Ride Helmet'
        ELSE 'Equipment ' || printf('%02d', n)
    END,
    CASE n
        WHEN 1 THEN 'Professional ski set for winter rental'
        WHEN 2 THEN 'Freestyle snowboard for advanced riders'
        WHEN 3 THEN 'Mountain bike for city and trail rides'
        WHEN 4 THEN 'Protective helmet for cycling and skating'
        ELSE 'Seed equipment item ' || printf('%02d', n)
    END,
    'MDL-' || printf('%03d', n)
FROM
    seq;

-- =========================
-- Фото оборудования
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    equipmentPhotos (id, idEquipment, idImage)
SELECT
    n,
    n,
    n
FROM
    seq;

-- =========================
-- Тарифы на оборудование
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    equipmentRates (id, idEquipment, idRentalType, price, deposit)
SELECT
    n,
    n,
    n,
    250 + n * 65,
    500 + n * 200
FROM
    seq;

-- =========================
-- Наличие оборудования в точках аренды
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    rentalPointEquipment (
        id,
        idRentalPoint,
        idEquipment,
        idEquipmentCondition,
        idSize,
        totalQuantity,
        availableQuantity
    )
SELECT
    n,
    n,
    n,
    n,
    ((n + 2) % 20) + 1,
    6 + (n % 10),
    (6 + (n % 10)) - (n % 4)
FROM
    seq;

-- =========================
-- Заказы аренды
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    rentOrders (
        id,
        idUser,
        idStatus,
        dateCreated,
        dateStart,
        dateEnd,
        amount,
        depositAmount,
        description
    )
SELECT
    n,
    n,
    n,
    datetime ('now', '-' || (30 - n) || ' day'),
    datetime ('now', '+' || ((n - 1) % 5) || ' day'),
    datetime (
        'now',
        '+' || ((n - 1) % 5) || ' day',
        '+' || (((n - 1) % 3) + 1) || ' day'
    ),
    (((n - 1) % 3) + 1) * (800 + n * 45),
    1000 + n * 150,
    'Rental order #' || printf('%02d', n)
FROM
    seq;

-- =========================
-- Позиции заказов
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    orderItems (
        id,
        idOrder,
        idEquipment,
        quantity,
        pricePerUnit,
        amount
    )
SELECT
    n,
    n,
    n,
    ((n - 1) % 3) + 1,
    800 + n * 45,
    (((n - 1) % 3) + 1) * (800 + n * 45)
FROM
    seq;

-- =========================
-- Платежи
-- =========================
WITH RECURSIVE
    seq(n) AS (
        SELECT
            1
        UNION ALL
        SELECT
            n + 1
        FROM
            seq
        WHERE
            n < 20
    )
INSERT INTO
    payments (id, idOrder, idMethod, idStatus, dateCreated, amount)
SELECT
    n,
    n,
    n,
    n,
    datetime ('now', '-' || (20 - n) || ' day'),
    (((n - 1) % 3) + 1) * (800 + n * 45)
FROM
    seq;
