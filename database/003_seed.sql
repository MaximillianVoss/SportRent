PRAGMA foreign_keys = ON;

-- =========================
-- Начальные данные справочников
-- =========================

INSERT INTO roles (title) VALUES
('Клиент'),
('Менеджер'),
('Администратор');

INSERT INTO orderStatuses (title) VALUES
('Создан'),
('Подтвержден'),
('В аренде'),
('Завершен'),
('Отменен');

INSERT INTO paymentMethods (title) VALUES
('Наличные'),
('Карта'),
('Онлайн');

INSERT INTO paymentStatuses (title) VALUES
('Ожидает оплаты'),
('Оплачено'),
('Ошибка'),
('Возврат');

INSERT INTO equipmentConditions (title) VALUES
('Новое'),
('Хорошее'),
('Б/У'),
('Требует обслуживания');

INSERT INTO rentalTypes (title, code, unitHours) VALUES
('Почасовая', 'hour', 1),
('Посуточная', 'day', 24);

-- =========================
-- Тестовые точки проката и справочники инвентаря
-- =========================

INSERT INTO rentalPoints (name, address, phone) VALUES
('SportRent Москва Центр', 'Москва, Ленинградский проспект, 10', '+79990000001'),
('SportRent Санкт-Петербург Центр', 'Санкт-Петербург, Невский проспект, 25', '+79990000002'),
('SportRent Казань Центр', 'Казань, улица Баумана, 15', '+79990000003');

INSERT INTO categories (title, description) VALUES
('Велосипеды', 'Городские, горные и спортивные велосипеды'),
('Лыжи', 'Комплекты лыжного инвентаря'),
('Сноуборды', 'Сноуборды и сопутствующее снаряжение'),
('Самокаты', 'Городские и спортивные самокаты'),
('Защита', 'Шлемы, наколенники и другая защита');

INSERT INTO brands (title) VALUES
('Trek'),
('Fischer'),
('Burton'),
('Xiaomi'),
('Uvex');

INSERT INTO equipmentTypes (title) VALUES
('Велосипед'),
('Лыжи'),
('Сноуборд'),
('Самокат'),
('Шлем');

INSERT INTO equipmentSizes (title) VALUES
('M'),
('L'),
('158'),
('Универсальный');

-- =========================
-- Тестовые пользователи
-- passwordHash пока заглушка
-- =========================

INSERT INTO users (idRole, firstName, lastName, phone, passwordHash) VALUES
(1, 'Иван', 'Петров', '+79991111111', 'hash_client_1'),
(2, 'Мария', 'Соколова', '+79992222222', 'hash_manager_1'),
(3, 'Алексей', 'Иванов', '+79993333333', 'hash_admin_1');

-- =========================
-- Тестовый инвентарь
-- деньги в копейках
-- =========================

INSERT INTO equipment (
    idCategory,
    idBrand,
    idType,
    title,
    description,
    model
) VALUES
(1, 1, 1, 'Горный велосипед', 'Надежный велосипед для города и пересеченной местности', 'Marlin 7'),
(2, 2, 2, 'Лыжный комплект', 'Комплект лыж для зимнего проката', 'RC4'),
(3, 3, 3, 'Сноуборд', 'Сноуборд для активного отдыха', 'Custom'),
(4, 4, 4, 'Электросамокат', 'Электросамокат для городской аренды', 'Mi Scooter'),
(5, 5, 5, 'Шлем защитный', 'Защитный шлем для вело- и лыжного спорта', 'Race 1');

INSERT INTO equipmentRates (
    idEquipment,
    idRentalType,
    price,
    deposit
) VALUES
(1, 1, 30000, 500000),
(1, 2, 150000, 500000),
(2, 1, 40000, 700000),
(2, 2, 180000, 700000),
(3, 1, 45000, 800000),
(3, 2, 200000, 800000),
(4, 1, 25000, 400000),
(4, 2, 120000, 400000),
(5, 1, 5000, 50000),
(5, 2, 20000, 50000);

INSERT INTO rentalPointEquipment (
    idRentalPoint,
    idEquipment,
    idEquipmentCondition,
    idSize,
    totalQuantity,
    availableQuantity
) VALUES
(1, 1, 2, 1, 5, 4),
(2, 2, 1, 2, 6, 5),
(2, 3, 2, 3, 4, 3),
(3, 4, 3, 4, 8, 7),
(1, 5, 1, 1, 10, 9);

INSERT INTO images (url) VALUES
('https://example.com/bike1.jpg'),
('https://example.com/ski1.jpg'),
('https://example.com/board1.jpg'),
('https://example.com/scooter1.jpg'),
('https://example.com/helmet1.jpg');

INSERT INTO equipmentPhotos (idEquipment, idImage) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);

-- =========================
-- Тестовый заказ
-- =========================

INSERT INTO rentOrders (
    idUser,
    idStatus,
    idRentalPointIssue,
    idRentalPointReturn,
    dateCreated,
    dateStart,
    dateEnd,
    amount,
    depositAmount,
    description
) VALUES
(1, 2, 1, 1, '2026-03-11 09:30:00', '2026-03-11 10:00:00', '2026-03-12 10:00:00', 170000, 550000, 'Тестовый заказ на велосипед и шлем');

-- =========================
-- Позиции заказа
-- =========================

INSERT INTO orderItems (
    idOrder,
    idRentalPointEquipment,
    quantity,
    pricePerUnit,
    amount
) VALUES
(1, 1, 1, 150000, 150000),
(1, 5, 1, 20000, 20000);

-- =========================
-- Платеж
-- =========================

INSERT INTO payments (
    idOrder,
    idPaymentMethod,
    idStatus,
    dateCreated,
    amount
) VALUES
(1, 2, 2, '2026-03-11 09:35:00', 720000);
