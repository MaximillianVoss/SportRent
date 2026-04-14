PRAGMA foreign_keys = ON;

-- #region Справочники
CREATE TABLE
    IF NOT EXISTS roles (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS orderStatus (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS paymentMethods (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS paymentStatuses (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS categories (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL,
        description TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS brands (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS equipmentType (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS equipmentConditions (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS equipmentSizes (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL
    );

CREATE TABLE
    IF NOT EXISTS rentalTypes (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL,
        code TEXT NOT NULL,
        unitHours INTEGER NOT NULL
    );

-- #endregion
-- #region Пользователи
CREATE TABLE
    IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idRole INTEGER,
        firstName TEXT NOT NULL,
        lastName TEXT NOT NULL,
        phone TEXT NOT NULL,
        passwordHash TEXT NOT NULL,
        dateCreated TEXT NOT NULL,
        FOREIGN KEY (idRole) REFERENCES roles (id) ON DELETE SET NULL
    );

-- #endregion
-- #region Точки аренды
CREATE TABLE
    IF NOT EXISTS rentalPoints (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        address TEXT NOT NULL,
        phone TEXT NOT NULL
    );

-- #endregion
-- #region Изображения
CREATE TABLE
    IF NOT EXISTS images (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        url TEXT NOT NULL,
        dateCreated TEXT NOT NULL
    );

-- #endregion
-- #region Оборудование
CREATE TABLE
    IF NOT EXISTS equipment (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idCategory INTEGER,
        idBrand INTEGER,
        idType INTEGER,
        title TEXT NOT NULL,
        description TEXT NOT NULL,
        model TEXT NOT NULL,
        FOREIGN KEY (idCategory) REFERENCES categories (id) ON DELETE SET NULL,
        FOREIGN KEY (idBrand) REFERENCES brands (id) ON DELETE SET NULL,
        FOREIGN KEY (idType) REFERENCES equipmentType (id) ON DELETE SET NULL
    );

CREATE TABLE
    IF NOT EXISTS equipmentPhotos (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idEquipment INTEGER NOT NULL,
        idImage INTEGER NOT NULL,
        FOREIGN KEY (idEquipment) REFERENCES equipment (id) ON DELETE CASCADE,
        FOREIGN KEY (idImage) REFERENCES images (id) ON DELETE CASCADE
    );

CREATE TABLE
    IF NOT EXISTS equipmentRates (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idEquipment INTEGER NOT NULL,
        idRentalType INTEGER NOT NULL,
        price REAL NOT NULL,
        deposit REAL NOT NULL DEFAULT 0,
        FOREIGN KEY (idEquipment) REFERENCES equipment (id) ON DELETE CASCADE,
        FOREIGN KEY (idRentalType) REFERENCES rentalTypes (id) ON DELETE RESTRICT
    );

CREATE TABLE
    IF NOT EXISTS rentalPointEquipment (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idRentalPoint INTEGER NOT NULL,
        idEquipment INTEGER NOT NULL,
        idEquipmentCondition INTEGER,
        idSize INTEGER,
        totalQuantity INTEGER NOT NULL DEFAULT 0,
        availableQuantity INTEGER NOT NULL DEFAULT 0,
        FOREIGN KEY (idRentalPoint) REFERENCES rentalPoints (id) ON DELETE CASCADE,
        FOREIGN KEY (idEquipment) REFERENCES equipment (id) ON DELETE CASCADE,
        FOREIGN KEY (idEquipmentCondition) REFERENCES equipmentConditions (id) ON DELETE SET NULL,
        FOREIGN KEY (idSize) REFERENCES equipmentSizes (id) ON DELETE SET NULL
    );

-- #endregion
-- #region Заказы аренды
CREATE TABLE
    IF NOT EXISTS rentOrders (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idUser INTEGER,
        idStatus INTEGER,
        dateCreated TEXT NOT NULL,
        dateStart TEXT NOT NULL,
        dateEnd TEXT NOT NULL,
        amount REAL NOT NULL,
        depositAmount REAL NOT NULL,
        description TEXT NOT NULL,
        FOREIGN KEY (idUser) REFERENCES users (id) ON DELETE SET NULL,
        FOREIGN KEY (idStatus) REFERENCES orderStatus (id) ON DELETE SET NULL
    );

CREATE TABLE
    IF NOT EXISTS orderItems (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idOrder INTEGER NOT NULL,
        idEquipment INTEGER NOT NULL,
        quantity INTEGER NOT NULL,
        pricePerUnit REAL NOT NULL,
        amount REAL NOT NULL,
        FOREIGN KEY (idOrder) REFERENCES rentOrders (id) ON DELETE CASCADE,
        FOREIGN KEY (idEquipment) REFERENCES equipment (id) ON DELETE RESTRICT
    );

-- #endregion
-- #region Платежи
CREATE TABLE
    IF NOT EXISTS payments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        idOrder INTEGER NOT NULL,
        idMethod INTEGER,
        idStatus INTEGER,
        dateCreated TEXT NOT NULL,
        amount REAL NOT NULL,
        FOREIGN KEY (idOrder) REFERENCES rentOrders (id) ON DELETE CASCADE,
        FOREIGN KEY (idMethod) REFERENCES paymentMethods (id) ON DELETE SET NULL,
        FOREIGN KEY (idStatus) REFERENCES paymentStatuses (id) ON DELETE SET NULL
    );

-- #endregion