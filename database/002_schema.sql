PRAGMA foreign_keys = ON;

-- =========================
-- Справочники
-- =========================

CREATE TABLE roles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE orderStatuses (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE paymentMethods (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE paymentStatuses (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE equipmentConditions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE rentalTypes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    code TEXT NOT NULL UNIQUE,
    unitHours INTEGER NOT NULL,
    CHECK (LENGTH(TRIM(title)) > 0),
    CHECK (LENGTH(TRIM(code)) > 0),
    CHECK (unitHours > 0)
);

CREATE TABLE categories (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    description TEXT,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE brands (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE equipmentTypes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

CREATE TABLE equipmentSizes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL UNIQUE,
    CHECK (LENGTH(TRIM(title)) > 0)
);

-- =========================
-- Пункты проката
-- =========================

CREATE TABLE rentalPoints (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    address TEXT NOT NULL,
    phone TEXT,
    CHECK (LENGTH(TRIM(name)) > 0),
    CHECK (LENGTH(TRIM(address)) > 0),
    CHECK (phone IS NULL OR LENGTH(TRIM(phone)) > 0)
);

-- =========================
-- Пользователи
-- =========================

CREATE TABLE users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idRole INTEGER NOT NULL,
    firstName TEXT NOT NULL,
    lastName TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    phone TEXT NOT NULL UNIQUE,
    passwordHash TEXT NOT NULL,
    dateCreated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (idRole) REFERENCES roles(id) ON DELETE RESTRICT,
    CHECK (LENGTH(TRIM(firstName)) > 0),
    CHECK (LENGTH(TRIM(lastName)) > 0),
    CHECK (LENGTH(TRIM(email)) > 0),
    CHECK (LENGTH(TRIM(phone)) > 0),
    CHECK (LENGTH(TRIM(passwordHash)) > 0)
);

-- =========================
-- Инвентарь
-- =========================

CREATE TABLE equipment (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idCategory INTEGER NOT NULL,
    idBrand INTEGER NOT NULL,
    idType INTEGER NOT NULL,
    title TEXT NOT NULL,
    description TEXT,
    model TEXT,

    FOREIGN KEY (idCategory) REFERENCES categories(id) ON DELETE RESTRICT,
    FOREIGN KEY (idBrand) REFERENCES brands(id) ON DELETE RESTRICT,
    FOREIGN KEY (idType) REFERENCES equipmentTypes(id) ON DELETE RESTRICT,

    CHECK (LENGTH(TRIM(title)) > 0),
    CHECK (model IS NULL OR LENGTH(TRIM(model)) > 0)
);

-- =========================
-- Тарифы аренды
-- =========================

CREATE TABLE equipmentRates (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idEquipment INTEGER NOT NULL,
    idRentalType INTEGER NOT NULL,
    price INTEGER NOT NULL,
    deposit INTEGER NOT NULL DEFAULT 0,

    FOREIGN KEY (idEquipment) REFERENCES equipment(id) ON DELETE CASCADE,
    FOREIGN KEY (idRentalType) REFERENCES rentalTypes(id) ON DELETE RESTRICT,

    CHECK (price >= 0),
    CHECK (deposit >= 0),
    UNIQUE (idEquipment, idRentalType)
);

-- =========================
-- Остатки по пунктам проката
-- =========================

CREATE TABLE rentalPointEquipment (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idRentalPoint INTEGER NOT NULL,
    idEquipment INTEGER NOT NULL,
    idEquipmentCondition INTEGER NOT NULL,
    idSize INTEGER NOT NULL,
    totalQuantity INTEGER NOT NULL,
    availableQuantity INTEGER NOT NULL,

    FOREIGN KEY (idRentalPoint) REFERENCES rentalPoints(id) ON DELETE RESTRICT,
    FOREIGN KEY (idEquipment) REFERENCES equipment(id) ON DELETE RESTRICT,
    FOREIGN KEY (idEquipmentCondition) REFERENCES equipmentConditions(id) ON DELETE RESTRICT,
    FOREIGN KEY (idSize) REFERENCES equipmentSizes(id) ON DELETE RESTRICT,

    CHECK (totalQuantity > 0),
    CHECK (availableQuantity >= 0),
    CHECK (availableQuantity <= totalQuantity),
    UNIQUE (idRentalPoint, idEquipment, idEquipmentCondition, idSize)
);

-- =========================
-- Изображения
-- =========================

CREATE TABLE images (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL UNIQUE,
    dateCreated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (LENGTH(TRIM(url)) > 0)
);

CREATE TABLE equipmentPhotos (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idEquipment INTEGER NOT NULL,
    idImage INTEGER NOT NULL,

    FOREIGN KEY (idEquipment) REFERENCES equipment(id) ON DELETE CASCADE,
    FOREIGN KEY (idImage) REFERENCES images(id) ON DELETE CASCADE,

    UNIQUE (idEquipment, idImage)
);

-- =========================
-- Заказы на аренду
-- =========================

CREATE TABLE rentOrders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idUser INTEGER NOT NULL,
    idStatus INTEGER NOT NULL,
    idRentalPointIssue INTEGER NOT NULL,
    idRentalPointReturn INTEGER,
    dateCreated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    dateStart TEXT NOT NULL,
    dateEnd TEXT NOT NULL,
    amount INTEGER NOT NULL DEFAULT 0,
    depositAmount INTEGER NOT NULL DEFAULT 0,
    description TEXT,

    FOREIGN KEY (idUser) REFERENCES users(id) ON DELETE RESTRICT,
    FOREIGN KEY (idStatus) REFERENCES orderStatuses(id) ON DELETE RESTRICT,
    FOREIGN KEY (idRentalPointIssue) REFERENCES rentalPoints(id) ON DELETE RESTRICT,
    FOREIGN KEY (idRentalPointReturn) REFERENCES rentalPoints(id) ON DELETE RESTRICT,

    CHECK (amount >= 0),
    CHECK (depositAmount >= 0),
    CHECK (dateEnd > dateStart)
);

-- =========================
-- Позиции заказа
-- =========================

CREATE TABLE orderItems (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idOrder INTEGER NOT NULL,
    idRentalPointEquipment INTEGER NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    pricePerUnit INTEGER NOT NULL,
    amount INTEGER NOT NULL,

    FOREIGN KEY (idOrder) REFERENCES rentOrders(id) ON DELETE CASCADE,
    FOREIGN KEY (idRentalPointEquipment) REFERENCES rentalPointEquipment(id) ON DELETE RESTRICT,

    CHECK (quantity > 0),
    CHECK (pricePerUnit >= 0),
    CHECK (amount >= 0)
);

-- =========================
-- Платежи
-- =========================

CREATE TABLE payments (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    idOrder INTEGER NOT NULL,
    idPaymentMethod INTEGER NOT NULL,
    idStatus INTEGER NOT NULL,
    dateCreated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    amount INTEGER NOT NULL,

    FOREIGN KEY (idOrder) REFERENCES rentOrders(id) ON DELETE CASCADE,
    FOREIGN KEY (idPaymentMethod) REFERENCES paymentMethods(id) ON DELETE RESTRICT,
    FOREIGN KEY (idStatus) REFERENCES paymentStatuses(id) ON DELETE RESTRICT,

    CHECK (amount >= 0)
);

-- =========================
-- Индексы
-- =========================

CREATE INDEX idx_users_role ON users(idRole);

CREATE INDEX idx_equipment_category ON equipment(idCategory);
CREATE INDEX idx_equipment_brand ON equipment(idBrand);
CREATE INDEX idx_equipment_type ON equipment(idType);

CREATE INDEX idx_equipmentRates_equipment ON equipmentRates(idEquipment);
CREATE INDEX idx_equipmentRates_rentalType ON equipmentRates(idRentalType);

CREATE INDEX idx_rentalPointEquipment_point ON rentalPointEquipment(idRentalPoint);
CREATE INDEX idx_rentalPointEquipment_equipment ON rentalPointEquipment(idEquipment);
CREATE INDEX idx_rentalPointEquipment_condition ON rentalPointEquipment(idEquipmentCondition);
CREATE INDEX idx_rentalPointEquipment_size ON rentalPointEquipment(idSize);

CREATE INDEX idx_equipmentPhotos_equipment ON equipmentPhotos(idEquipment);
CREATE INDEX idx_equipmentPhotos_image ON equipmentPhotos(idImage);

CREATE INDEX idx_rentOrders_user ON rentOrders(idUser);
CREATE INDEX idx_rentOrders_status ON rentOrders(idStatus);
CREATE INDEX idx_rentOrders_issuePoint ON rentOrders(idRentalPointIssue);
CREATE INDEX idx_rentOrders_returnPoint ON rentOrders(idRentalPointReturn);

CREATE INDEX idx_orderItems_order ON orderItems(idOrder);
CREATE INDEX idx_orderItems_rentalPointEquipment ON orderItems(idRentalPointEquipment);

CREATE INDEX idx_payments_order ON payments(idOrder);
CREATE INDEX idx_payments_method ON payments(idPaymentMethod);
CREATE INDEX idx_payments_status ON payments(idStatus);
