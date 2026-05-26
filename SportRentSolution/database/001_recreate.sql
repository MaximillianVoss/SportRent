PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS payments;
DROP TABLE IF EXISTS orderItems;
DROP TABLE IF EXISTS rentOrders;

DROP TABLE IF EXISTS equipmentPhotos;
DROP TABLE IF EXISTS images;
DROP TABLE IF EXISTS rentalPointEquipment;
DROP TABLE IF EXISTS equipmentRates;
DROP TABLE IF EXISTS equipment;

DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS rentalPoints;

DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS orderStatuses;
DROP TABLE IF EXISTS paymentMethods;
DROP TABLE IF EXISTS paymentStatuses;
DROP TABLE IF EXISTS equipmentConditions;
DROP TABLE IF EXISTS rentalTypes;
DROP TABLE IF EXISTS categories;
DROP TABLE IF EXISTS brands;
DROP TABLE IF EXISTS equipmentTypes;
DROP TABLE IF EXISTS equipmentSizes;

PRAGMA foreign_keys = ON;
