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
-- Тестовые сценарии для сложных запросов
-- =========================
-- 1. Есть оборудование без фотографий: equipment.id 19, 20.
-- 2. Есть оборудование без тарифов: equipment.id 18, 19, 20.
-- 3. Есть клиенты без заказов: users.id 13-20.
-- 4. Есть заказы без платежей: rentOrders.id 17, 19, 20.
-- 5. Есть оборудование в нескольких точках аренды: equipment.id 1, 3, 5, 6, 8, 19, 20.
-- 6. Есть заказы с несколькими позициями и с quantity > 1.
-- 7. Есть оборудование, которое ни разу не арендовали: equipment.id 2, 7, 18, 19, 20.

-- =========================
-- Справочники
-- =========================
INSERT INTO
    roles (id, title)
VALUES
    (1, 'Admin'),
    (2, 'Senior manager'),
    (3, 'Manager'),
    (4, 'Client'),
    (5, 'Cashier'),
    (6, 'Technician'),
    (7, 'Accountant'),
    (8, 'Support'),
    (9, 'Analyst'),
    (10, 'Instructor'),
    (11, 'Content manager'),
    (12, 'Operator'),
    (13, 'Shift lead'),
    (14, 'Marketing'),
    (15, 'HR'),
    (16, 'Legal'),
    (17, 'Security'),
    (18, 'Partner'),
    (19, 'Courier'),
    (20, 'Auditor');

INSERT INTO
    orderStatus (id, title)
VALUES
    (1, 'Created'),
    (2, 'Confirmed'),
    (3, 'In progress'),
    (4, 'Completed'),
    (5, 'Cancelled'),
    (6, 'Awaiting pickup'),
    (7, 'Awaiting return'),
    (8, 'Overdue'),
    (9, 'Deposit paid'),
    (10, 'Packed'),
    (11, 'Ready to issue'),
    (12, 'On route'),
    (13, 'Inspection'),
    (14, 'Maintenance'),
    (15, 'Draft'),
    (16, 'Need callback'),
    (17, 'Partially issued'),
    (18, 'Extended'),
    (19, 'Closed'),
    (20, 'Archived');

INSERT INTO
    paymentMethods (id, title)
VALUES
    (1, 'Cash'),
    (2, 'Card'),
    (3, 'Online'),
    (4, 'SBP'),
    (5, 'Apple Pay'),
    (6, 'Google Pay'),
    (7, 'Bank transfer'),
    (8, 'QR code'),
    (9, 'Gift certificate'),
    (10, 'Loyalty points'),
    (11, 'Invoice'),
    (12, 'Terminal'),
    (13, 'Corporate account'),
    (14, 'Deposit transfer'),
    (15, 'Split payment'),
    (16, 'Subscription'),
    (17, 'Cashless'),
    (18, 'Visa'),
    (19, 'MasterCard'),
    (20, 'Mir');

INSERT INTO
    paymentStatuses (id, title)
VALUES
    (1, 'Pending'),
    (2, 'Paid'),
    (3, 'Failed'),
    (4, 'Refunded'),
    (5, 'Cancelled'),
    (6, 'Partial'),
    (7, 'Authorized'),
    (8, 'Captured'),
    (9, 'Chargeback'),
    (10, 'Reversed'),
    (11, 'Processing'),
    (12, 'On hold'),
    (13, 'Expired'),
    (14, 'Awaiting confirmation'),
    (15, 'Awaiting refund'),
    (16, 'Refunded partial'),
    (17, 'Disputed'),
    (18, 'Write-off'),
    (19, 'Closed'),
    (20, 'Manual review');

INSERT INTO
    categories (id, title, description)
VALUES
    (1, 'Ski', 'Skis, poles and alpine sets for winter rental.'),
    (2, 'Snowboard', 'Snowboards and related riding gear.'),
    (3, 'Bicycle', 'City, trail and mountain bicycles.'),
    (4, 'Protection', 'Helmets, pads and protective equipment.'),
    (5, 'Ski Boots', 'Boots for alpine and carving skis.'),
    (6, 'Snowboard Boots', 'Boots for freestyle and all-mountain boards.'),
    (7, 'Skates', 'Ice skates for public skating sessions.'),
    (8, 'Scooters', 'Kick scooters for children and adults.'),
    (9, 'Rollers', 'Inline skates for city rides.'),
    (10, 'Camping', 'Tents and camping equipment.'),
    (11, 'Kayaks', 'Inflatable kayaks and accessories.'),
    (12, 'SUP Boards', 'Stand-up paddle boards for lakes and rivers.'),
    (13, 'Climbing', 'Harnesses and climbing starter kits.'),
    (14, 'Tennis', 'Rackets and tennis accessories.'),
    (15, 'Fitness', 'Heart rate monitors and fitness gear.'),
    (16, 'Hiking', 'Backpacks, poles and trekking essentials.'),
    (17, 'Fishing', 'Fishing kits and accessory sets.'),
    (18, 'Accessories', 'Universal add-ons for rental equipment.'),
    (19, 'Winter Apparel', 'Jackets, pants and thermal layers.'),
    (20, 'Navigation', 'Sports GPS devices and sensors.');

INSERT INTO
    brands (id, title)
VALUES
    (1, 'Rossignol'),
    (2, 'Atomic'),
    (3, 'Burton'),
    (4, 'Head'),
    (5, 'Trek'),
    (6, 'Scott'),
    (7, 'Merida'),
    (8, 'Giro'),
    (9, 'Fox'),
    (10, 'Salomon'),
    (11, 'K2'),
    (12, 'Nordway'),
    (13, 'Tech Team'),
    (14, 'Rollerblade'),
    (15, 'Outventure'),
    (16, 'Intex'),
    (17, 'Gladiator'),
    (18, 'Petzl'),
    (19, 'Babolat'),
    (20, 'Garmin');

INSERT INTO
    equipmentType (id, title)
VALUES
    (1, 'Adult'),
    (2, 'Child'),
    (3, 'Universal'),
    (4, 'Men'),
    (5, 'Women'),
    (6, 'Junior'),
    (7, 'Teen'),
    (8, 'Beginner'),
    (9, 'Intermediate'),
    (10, 'Advanced'),
    (11, 'Pro'),
    (12, 'City'),
    (13, 'Trail'),
    (14, 'Mountain'),
    (15, 'Road'),
    (16, 'Freestyle'),
    (17, 'Touring'),
    (18, 'Protective'),
    (19, 'Camping'),
    (20, 'Water');

INSERT INTO
    equipmentConditions (id, title)
VALUES
    (1, 'New'),
    (2, 'Excellent'),
    (3, 'Very good'),
    (4, 'Good'),
    (5, 'Used'),
    (6, 'After service'),
    (7, 'Demo'),
    (8, 'Refurbished'),
    (9, 'Fresh wax'),
    (10, 'Sharpened'),
    (11, 'Dry storage'),
    (12, 'Rental ready'),
    (13, 'Minor wear'),
    (14, 'Medium wear'),
    (15, 'Ready for trail'),
    (16, 'Ready for city'),
    (17, 'Sanitized'),
    (18, 'Inflated'),
    (19, 'Packed'),
    (20, 'Checked');

INSERT INTO
    equipmentSizes (id, title)
VALUES
    (1, 'XXS'),
    (2, 'XS'),
    (3, 'S'),
    (4, 'M'),
    (5, 'L'),
    (6, 'XL'),
    (7, 'XXL'),
    (8, 'XXXL'),
    (9, '34'),
    (10, '36'),
    (11, '38'),
    (12, '40'),
    (13, '42'),
    (14, '44'),
    (15, '46'),
    (16, '48'),
    (17, '50'),
    (18, '52'),
    (19, '54'),
    (20, 'Universal');

INSERT INTO
    rentalTypes (id, title, code, unitHours)
VALUES
    (1, 'Hourly', 'HOUR', 1),
    (2, 'Two hours', '2H', 2),
    (3, 'Three hours', '3H', 3),
    (4, 'Half day', 'HALF_DAY', 6),
    (5, 'Daily', 'DAY', 24),
    (6, 'Two days', '2DAY', 48),
    (7, 'Three days', '3DAY', 72),
    (8, 'Weekend', 'WEEKEND', 48),
    (9, 'Weekly', 'WEEK', 168),
    (10, 'Ten days', '10DAY', 240),
    (11, 'Two weeks', '2WEEK', 336),
    (12, 'Monthly', 'MONTH', 720),
    (13, 'Morning session', 'MORNING', 5),
    (14, 'Evening session', 'EVENING', 5),
    (15, 'Family day', 'FAMILY', 24),
    (16, 'Test ride', 'TEST', 2),
    (17, 'Lesson slot', 'LESSON', 3),
    (18, 'Holiday week', 'HOLIDAY', 192),
    (19, 'Corporate day', 'CORPORATE', 24),
    (20, 'Expedition', 'EXPEDITION', 336);

-- =========================
-- Пользователи
-- =========================
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
VALUES
    (1, 4, 'Ivan', 'Petrov', '+79990000001', 'hash_user_01', '2026-02-20 09:00:00'),
    (2, 4, 'Maria', 'Smirnova', '+79990000002', 'hash_user_02', '2026-02-21 09:15:00'),
    (3, 4, 'Aleksei', 'Kuznetsov', '+79990000003', 'hash_user_03', '2026-02-22 09:30:00'),
    (4, 4, 'Elena', 'Popova', '+79990000004', 'hash_user_04', '2026-02-23 10:00:00'),
    (5, 4, 'Dmitrii', 'Volkov', '+79990000005', 'hash_user_05', '2026-02-24 10:15:00'),
    (6, 4, 'Olga', 'Morozova', '+79990000006', 'hash_user_06', '2026-02-25 10:30:00'),
    (7, 4, 'Sergei', 'Fedorov', '+79990000007', 'hash_user_07', '2026-02-26 11:00:00'),
    (8, 4, 'Natalia', 'Vasileva', '+79990000008', 'hash_user_08', '2026-02-27 11:15:00'),
    (9, 4, 'Artem', 'Sokolov', '+79990000009', 'hash_user_09', '2026-02-28 11:30:00'),
    (10, 4, 'Irina', 'Lebedeva', '+79990000010', 'hash_user_10', '2026-03-01 12:00:00'),
    (11, 4, 'Pavel', 'Novikov', '+79990000011', 'hash_user_11', '2026-03-02 12:15:00'),
    (12, 4, 'Svetlana', 'Kozlova', '+79990000012', 'hash_user_12', '2026-03-03 12:30:00'),
    (13, 4, 'Nikolai', 'Pavlov', '+79990000013', 'hash_user_13', '2026-03-04 13:00:00'),
    (14, 4, 'Yulia', 'Semenova', '+79990000014', 'hash_user_14', '2026-03-05 13:15:00'),
    (15, 4, 'Viktor', 'Andreev', '+79990000015', 'hash_user_15', '2026-03-06 13:30:00'),
    (16, 4, 'Tatiana', 'Makarova', '+79990000016', 'hash_user_16', '2026-03-07 14:00:00'),
    (17, 4, 'Konstantin', 'Zakharov', '+79990000017', 'hash_user_17', '2026-03-08 14:15:00'),
    (18, 4, 'Anna', 'Danilova', '+79990000018', 'hash_user_18', '2026-03-09 14:30:00'),
    (19, 4, 'Roman', 'Egorov', '+79990000019', 'hash_user_19', '2026-03-10 15:00:00'),
    (20, 4, 'Ksenia', 'Belova', '+79990000020', 'hash_user_20', '2026-03-11 15:15:00');

-- =========================
-- Точки аренды
-- =========================
INSERT INTO
    rentalPoints (id, name, address, phone)
VALUES
    (1, 'SportRent Tverskaya', 'Moscow, Tverskaya st. 10', '+74950000001'),
    (2, 'SportRent Sokol', 'Moscow, Leningradskiy ave. 74', '+74950000002'),
    (3, 'SportRent Gorky Park', 'Moscow, Krymskiy Val 9', '+74950000003'),
    (4, 'SportRent Sokolniki', 'Moscow, Sokolnicheskiy Val 1', '+74950000004'),
    (5, 'SportRent Izmailovo', 'Moscow, Izmailovskiy Park 2', '+74950000005'),
    (6, 'SportRent Khimki', 'Khimki, Lenina ave. 5', '+74950000006'),
    (7, 'SportRent Krasnogorsk', 'Krasnogorsk, Rechnaya st. 7', '+74950000007'),
    (8, 'SportRent Odintsovo', 'Odintsovo, Mozhayskoe sh. 14', '+74950000008'),
    (9, 'SportRent Mytishchi', 'Mytishchi, Olimpiyskiy ave. 12', '+74950000009'),
    (10, 'SportRent Krylatskoe', 'Moscow, Krylatskaya st. 15', '+74950000010'),
    (11, 'SportRent Strogino', 'Moscow, Marshala Katukova st. 21', '+74950000011'),
    (12, 'SportRent Vorobyovy Gory', 'Moscow, Kosygina st. 28', '+74950000012'),
    (13, 'SportRent Balashikha', 'Balashikha, Sovetskaya st. 18', '+74950000013'),
    (14, 'SportRent Reutov', 'Reutov, Yuzhnaya st. 9', '+74950000014'),
    (15, 'SportRent Podolsk', 'Podolsk, Kirova st. 4', '+74950000015'),
    (16, 'SportRent Domodedovo', 'Domodedovo, Kashirskoe sh. 3', '+74950000016'),
    (17, 'SportRent Korolev', 'Korolev, Kosmonavtov ave. 27', '+74950000017'),
    (18, 'SportRent Lyubertsy', 'Lyubertsy, Oktiabrskiy ave. 33', '+74950000018'),
    (19, 'SportRent Zelenograd', 'Zelenograd, Panfilovskiy ave. 11', '+74950000019'),
    (20, 'SportRent Peredelkino', 'Moscow, Chobotovskaya st. 6', '+74950000020');

-- =========================
-- Изображения
-- =========================
INSERT INTO
    images (id, url, dateCreated)
VALUES
    (1, 'images/rossignol_experience_76_main.jpg', '2026-03-01 09:00:00'),
    (2, 'images/rossignol_experience_76_side.jpg', '2026-03-01 09:05:00'),
    (3, 'images/atomic_redster_j2_main.jpg', '2026-03-01 09:10:00'),
    (4, 'images/burton_custom_camber_main.jpg', '2026-03-01 09:15:00'),
    (5, 'images/head_day_lyt_main.jpg', '2026-03-01 09:20:00'),
    (6, 'images/trek_marlin_6_main.jpg', '2026-03-01 09:25:00'),
    (7, 'images/trek_marlin_6_side.jpg', '2026-03-01 09:30:00'),
    (8, 'images/scott_aspect_950_main.jpg', '2026-03-01 09:35:00'),
    (9, 'images/merida_matts_j24_main.jpg', '2026-03-01 09:40:00'),
    (10, 'images/giro_fixture_mips_main.jpg', '2026-03-01 09:45:00'),
    (11, 'images/fox_launch_d3o_main.jpg', '2026-03-01 09:50:00'),
    (12, 'images/salomon_spro_100_main.jpg', '2026-03-01 09:55:00'),
    (13, 'images/k2_raider_main.jpg', '2026-03-01 10:00:00'),
    (14, 'images/nordway_fh_one_main.jpg', '2026-03-01 10:05:00'),
    (15, 'images/tech_team_ragtag_main.jpg', '2026-03-01 10:10:00'),
    (16, 'images/rollerblade_macroblade_80_main.jpg', '2026-03-01 10:15:00'),
    (17, 'images/outventure_dome_3_main.jpg', '2026-03-01 10:20:00'),
    (18, 'images/intex_challenger_k1_main.jpg', '2026-03-01 10:25:00'),
    (19, 'images/gladiator_origin_108_main.jpg', '2026-03-01 10:30:00'),
    (20, 'images/petzl_corax_kit_main.jpg', '2026-03-01 10:35:00'),
    (21, 'images/babolat_boost_drive_main.jpg', '2026-03-01 10:40:00'),
    (22, 'images/garmin_hrm_dual_main.jpg', '2026-03-01 10:45:00'),
    (23, 'images/storefront_sport_rent_tverskaya.jpg', '2026-03-01 10:50:00'),
    (24, 'images/storefront_sport_rent_gorky_park.jpg', '2026-03-01 10:55:00');

-- =========================
-- Оборудование
-- =========================
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
VALUES
    (1, 1, 1, 10, 'Rossignol Experience 76', 'All-mountain ski set for groomed slopes and weekend rental.', 'RE76-2025'),
    (2, 1, 2, 2, 'Atomic Redster J2 130', 'Junior carving skis for children between 125 and 140 cm height.', 'ARJ2-130'),
    (3, 2, 3, 1, 'Burton Custom Camber', 'Versatile snowboard for riders who want stable carving and park sessions.', 'BCC-158'),
    (4, 2, 4, 5, 'Head Day Lyt', 'Lightweight women snowboard for resort riding and easy turns.', 'HDL-147'),
    (5, 3, 5, 14, 'Trek Marlin 6', 'Hardtail mountain bike for city parks and light trails.', 'TM6-29'),
    (6, 3, 6, 14, 'Scott Aspect 950', 'Reliable trail bike with front suspension and hydraulic brakes.', 'SA950-29'),
    (7, 3, 7, 2, 'Merida Matts J24', 'Child bicycle for park rides and bike school sessions.', 'MMJ24'),
    (8, 4, 8, 18, 'Giro Fixture MIPS Helmet', 'Universal protective helmet for cycling, scooter and roller use.', 'GFM-UNI'),
    (9, 4, 9, 18, 'Fox Launch D3O Knee Pads', 'Flexible knee protection for trail riding and skate practice.', 'FLD3O-M'),
    (10, 5, 10, 1, 'Salomon S/Pro 100', 'Comfortable ski boots for all-day alpine riding.', 'SSP100-44'),
    (11, 6, 11, 1, 'K2 Raider', 'Soft snowboard boots for beginner and intermediate riders.', 'K2R-44'),
    (12, 7, 12, 3, 'Nordway FH One', 'Recreational ice skates for indoor and outdoor rinks.', 'NFH1-40'),
    (13, 8, 13, 2, 'Tech Team Ragtag', 'Compact child scooter for city walks and park alleys.', 'TTR-125'),
    (14, 9, 14, 1, 'Rollerblade Macroblade 80', 'Fitness inline skates for city asphalt and embankments.', 'RBM80-44'),
    (15, 10, 15, 19, 'Outventure Dome 3', 'Three-person tent for weekend camping and short trips.', 'OD3-GREEN'),
    (16, 11, 16, 20, 'Intex Challenger K1', 'Inflatable single-seat kayak for calm water rental.', 'ICK1-SET'),
    (17, 12, 17, 20, 'Gladiator Origin 10.8', 'Stable SUP board for beginners and family water sessions.', 'G108-ALL'),
    (18, 13, 18, 3, 'Petzl Corax Kit', 'Starter climbing harness kit for indoor climbing walls.', 'PCK-UNI'),
    (19, 14, 19, 1, 'Babolat Boost Drive', 'Light tennis racket for entry-level training matches.', 'BBD-102'),
    (20, 15, 20, 3, 'Garmin HRM-Dual', 'Heart rate sensor for cardio rentals and training tests.', 'GHRM-DUAL');

-- =========================
-- Фото оборудования
-- =========================
INSERT INTO
    equipmentPhotos (id, idEquipment, idImage)
VALUES
    (1, 1, 1),
    (2, 1, 2),
    (3, 2, 3),
    (4, 3, 4),
    (5, 4, 5),
    (6, 5, 6),
    (7, 5, 7),
    (8, 6, 8),
    (9, 7, 9),
    (10, 8, 10),
    (11, 9, 11),
    (12, 10, 12),
    (13, 11, 13),
    (14, 12, 14),
    (15, 13, 15),
    (16, 14, 16),
    (17, 15, 17),
    (18, 16, 18),
    (19, 17, 19),
    (20, 18, 20);

-- =========================
-- Тарифы на оборудование
-- =========================
INSERT INTO
    equipmentRates (id, idEquipment, idRentalType, price, deposit)
VALUES
    (1, 1, 1, 700, 12000),
    (2, 1, 5, 2600, 12000),
    (3, 1, 9, 12000, 12000),
    (4, 2, 4, 900, 7000),
    (5, 2, 5, 1400, 7000),
    (6, 3, 1, 650, 10000),
    (7, 3, 5, 2400, 10000),
    (8, 3, 9, 11000, 10000),
    (9, 4, 5, 2200, 9000),
    (10, 4, 8, 3800, 9000),
    (11, 5, 1, 450, 8000),
    (12, 5, 5, 1800, 8000),
    (13, 5, 9, 8500, 8000),
    (14, 6, 1, 480, 8500),
    (15, 6, 5, 1900, 8500),
    (16, 7, 5, 1300, 6000),
    (17, 8, 5, 350, 1500),
    (18, 9, 5, 400, 2000),
    (19, 10, 5, 950, 5000),
    (20, 10, 9, 4200, 5000),
    (21, 11, 5, 850, 4500),
    (22, 12, 1, 300, 2500),
    (23, 12, 5, 1100, 2500),
    (24, 13, 1, 250, 1500),
    (25, 13, 5, 900, 1500),
    (26, 14, 1, 380, 3000),
    (27, 14, 5, 1400, 3000),
    (28, 15, 5, 1600, 5000),
    (29, 16, 5, 2100, 9000),
    (30, 17, 5, 2300, 10000);

-- =========================
-- Наличие оборудования в точках аренды
-- =========================
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
VALUES
    (1, 1, 1, 1, 15, 6, 3),
    (2, 2, 1, 2, 15, 4, 2),
    (3, 3, 2, 3, 11, 5, 4),
    (4, 1, 3, 1, 15, 5, 2),
    (5, 4, 3, 2, 14, 3, 0),
    (6, 5, 4, 4, 13, 4, 1),
    (7, 2, 5, 2, 16, 7, 5),
    (8, 6, 5, 3, 16, 5, 4),
    (9, 7, 6, 3, 17, 6, 2),
    (10, 8, 6, 4, 17, 4, 0),
    (11, 8, 7, 2, 10, 6, 3),
    (12, 3, 8, 1, 20, 12, 10),
    (13, 4, 9, 2, 5, 10, 7),
    (14, 1, 10, 2, 14, 8, 5),
    (15, 1, 11, 3, 14, 6, 4),
    (16, 9, 12, 1, 12, 10, 6),
    (17, 9, 13, 2, 4, 8, 5),
    (18, 10, 14, 3, 5, 7, 3),
    (19, 5, 15, 1, 20, 5, 2),
    (20, 6, 16, 2, 20, 4, 1),
    (21, 6, 17, 1, 20, 3, 0),
    (22, 7, 18, 2, 20, 2, 2),
    (23, 8, 19, 1, 20, 4, 4),
    (24, 8, 20, 1, 20, 6, 6),
    (25, 3, 5, 2, 16, 3, 1),
    (26, 4, 5, 4, 16, 2, 0),
    (27, 2, 3, 3, 15, 2, 1),
    (28, 1, 8, 1, 20, 6, 6),
    (29, 5, 19, 1, 20, 2, 2),
    (30, 10, 20, 1, 20, 3, 0);

-- =========================
-- Заказы аренды
-- =========================
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
VALUES
    (1, 1, 4, '2026-03-10 10:00:00', '2026-03-15 09:00:00', '2026-03-16 20:00:00', 0, 0, 'Weekend ski rental for Ivan'),
    (2, 1, 4, '2026-03-18 18:00:00', '2026-03-19 18:30:00', '2026-03-19 22:00:00', 0, 0, 'After-work bike ride for Ivan'),
    (3, 2, 2, '2026-03-20 12:10:00', '2026-04-16 09:00:00', '2026-04-18 20:00:00', 0, 0, 'Snowboard weekend for Maria'),
    (4, 2, 1, '2026-04-10 16:45:00', '2026-04-15 10:00:00', '2026-04-15 18:00:00', 0, 0, 'Protection set for Maria'),
    (5, 3, 4, '2026-03-05 11:20:00', '2026-03-07 08:00:00', '2026-03-09 19:00:00', 0, 0, 'Family bike weekend for Aleksei'),
    (6, 3, 5, '2026-03-22 13:00:00', '2026-03-25 09:00:00', '2026-03-26 19:00:00', 0, 0, 'Cancelled camping booking for Aleksei'),
    (7, 4, 3, '2026-04-12 15:30:00', '2026-04-14 08:00:00', '2026-04-17 21:00:00', 0, 0, 'Camping kit for Elena'),
    (8, 4, 4, '2026-03-28 09:40:00', '2026-03-30 18:00:00', '2026-03-30 22:00:00', 0, 0, 'Rollerblades evening for Elena'),
    (9, 5, 2, '2026-04-01 14:00:00', '2026-04-18 10:00:00', '2026-04-19 20:00:00', 0, 0, 'Kayak session for Dmitrii'),
    (10, 5, 4, '2026-03-12 10:30:00', '2026-03-13 09:00:00', '2026-03-13 22:00:00', 0, 0, 'Ski boots setup for Dmitrii'),
    (11, 6, 4, '2026-03-08 11:10:00', '2026-03-09 09:00:00', '2026-03-10 20:00:00', 0, 0, 'Ski boots rental for Olga'),
    (12, 6, 4, '2026-03-26 17:20:00', '2026-03-27 10:00:00', '2026-03-27 23:00:00', 0, 0, 'Helmet and pads for Olga'),
    (13, 7, 2, '2026-04-04 12:00:00', '2026-04-20 09:00:00', '2026-04-21 20:00:00', 0, 0, 'Bike and helmet for Sergei'),
    (14, 8, 4, '2026-03-14 15:10:00', '2026-03-15 08:00:00', '2026-03-17 21:00:00', 0, 0, 'Snowboard weekend for Natalia'),
    (15, 9, 3, '2026-04-09 13:25:00', '2026-04-14 10:00:00', '2026-04-16 19:00:00', 0, 0, 'SUP board session for Artem'),
    (16, 10, 4, '2026-03-11 18:20:00', '2026-03-11 19:00:00', '2026-03-11 22:00:00', 0, 0, 'Ice skates for Irina'),
    (17, 10, 1, '2026-04-13 09:10:00', '2026-04-19 11:00:00', '2026-04-20 18:00:00', 0, 0, 'Scooter booking for Irina'),
    (18, 11, 4, '2026-03-17 10:50:00', '2026-03-18 09:00:00', '2026-03-18 21:00:00', 0, 0, 'Protection set for Pavel'),
    (19, 12, 2, '2026-04-08 16:15:00', '2026-04-22 09:00:00', '2026-04-24 21:00:00', 0, 0, 'Corporate bike booking for Svetlana'),
    (20, 12, 1, '2026-04-14 12:05:00', '2026-04-25 08:00:00', '2026-04-27 20:00:00', 0, 0, 'Family ski package for Svetlana');

-- =========================
-- Позиции заказов
-- =========================
INSERT INTO
    orderItems (
        id,
        idOrder,
        idEquipment,
        quantity,
        pricePerUnit,
        amount
    )
VALUES
    (1, 1, 1, 1, 2600, 2600),
    (2, 2, 5, 1, 1800, 1800),
    (3, 2, 8, 1, 350, 350),
    (4, 3, 3, 1, 2400, 2400),
    (5, 3, 11, 1, 850, 850),
    (6, 4, 8, 2, 350, 700),
    (7, 4, 9, 1, 400, 400),
    (8, 5, 6, 1, 1900, 1900),
    (9, 5, 8, 2, 350, 700),
    (10, 6, 15, 1, 1600, 1600),
    (11, 7, 15, 1, 1600, 1600),
    (12, 7, 8, 2, 350, 700),
    (13, 7, 9, 2, 400, 800),
    (14, 8, 14, 1, 1400, 1400),
    (15, 9, 16, 1, 2100, 2100),
    (16, 9, 8, 1, 350, 350),
    (17, 10, 10, 1, 950, 950),
    (18, 11, 10, 1, 950, 950),
    (19, 12, 8, 1, 350, 350),
    (20, 12, 9, 1, 400, 400),
    (21, 13, 5, 1, 1800, 1800),
    (22, 13, 8, 1, 350, 350),
    (23, 14, 4, 1, 2200, 2200),
    (24, 14, 11, 1, 850, 850),
    (25, 15, 17, 1, 2300, 2300),
    (26, 16, 12, 1, 1100, 1100),
    (27, 17, 13, 1, 900, 900),
    (28, 18, 8, 1, 350, 350),
    (29, 18, 9, 1, 400, 400),
    (30, 19, 6, 2, 1900, 3800),
    (31, 19, 8, 2, 350, 700),
    (32, 19, 9, 2, 400, 800),
    (33, 20, 1, 1, 2600, 2600),
    (34, 20, 10, 1, 950, 950),
    (35, 20, 8, 1, 350, 350);

UPDATE rentOrders
SET
    amount = COALESCE(
        (
            SELECT
                SUM(oi.amount)
            FROM
                orderItems AS oi
            WHERE
                oi.idOrder = rentOrders.id
        ),
        0
    ),
    depositAmount = COALESCE(
        (
            SELECT
                SUM(
                    COALESCE(
                        (
                            SELECT
                                MAX(er.deposit)
                            FROM
                                equipmentRates AS er
                            WHERE
                                er.idEquipment = oi.idEquipment
                        ),
                        0
                    ) * oi.quantity
                )
            FROM
                orderItems AS oi
            WHERE
                oi.idOrder = rentOrders.id
        ),
        0
    );

-- =========================
-- Платежи
-- =========================
WITH paymentSeed (
    id,
    idOrder,
    idMethod,
    idStatus,
    dateCreated,
    amountFactor
) AS (
    VALUES
        (1, 1, 2, 2, '2026-03-10 10:20:00', 1.00),
        (2, 2, 4, 2, '2026-03-18 18:10:00', 1.00),
        (3, 3, 3, 1, '2026-03-20 12:30:00', 0.50),
        (4, 4, 1, 1, '2026-04-10 17:00:00', 0.30),
        (5, 5, 2, 2, '2026-03-05 11:40:00', 1.00),
        (6, 6, 7, 4, '2026-03-22 13:20:00', 1.00),
        (7, 7, 2, 2, '2026-04-12 16:00:00', 0.50),
        (8, 7, 15, 2, '2026-04-13 12:00:00', 0.50),
        (9, 8, 1, 2, '2026-03-28 10:00:00', 1.00),
        (10, 9, 3, 1, '2026-04-01 14:20:00', 0.40),
        (11, 10, 2, 2, '2026-03-12 10:45:00', 1.00),
        (12, 11, 2, 2, '2026-03-08 11:25:00', 1.00),
        (13, 12, 4, 2, '2026-03-26 17:35:00', 1.00),
        (14, 13, 2, 2, '2026-04-04 12:15:00', 1.00),
        (15, 14, 3, 2, '2026-03-14 15:30:00', 1.00),
        (16, 15, 2, 2, '2026-04-09 13:40:00', 0.50),
        (17, 15, 1, 2, '2026-04-10 10:00:00', 0.50),
        (18, 16, 1, 2, '2026-03-11 18:40:00', 1.00),
        (19, 18, 2, 2, '2026-03-17 11:05:00', 1.00),
        (20, 3, 2, 3, '2026-03-21 09:00:00', 0.20)
)
INSERT INTO
    payments (id, idOrder, idMethod, idStatus, dateCreated, amount)
SELECT
    ps.id,
    ps.idOrder,
    ps.idMethod,
    ps.idStatus,
    ps.dateCreated,
    ROUND(ro.amount * ps.amountFactor, 2)
FROM
    paymentSeed AS ps
    INNER JOIN rentOrders AS ro ON ro.id = ps.idOrder;
