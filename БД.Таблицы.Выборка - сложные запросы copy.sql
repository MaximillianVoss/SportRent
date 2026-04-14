-- Задача 1. Вывести всё оборудование с категорией, брендом и типом
SELECT
    *
FROM
    equipment
    INNER JOIN categories ON idCategory = categories.id
    INNER JOIN brands ON idBrand = brands.id
    INNER JOIN equipmentType ON idType = equipmentType.id;

--Задача 2. Вывести наличие оборудования по точкам аренды
SELECT
    *
FROM
    rentalPointEquipment
    INNER JOIN rentalPoints ON idRentalPoint = rentalPoints.id
    INNER JOIN equipment ON idEquipment = equipment.id;

-- Задача 3. Найти всё доступное оборудование
-- Условие
-- Вывести только то оборудование, которое доступно для аренды хотя бы в одной точке.
SELECT
    *
FROM
    rentalPointEquipment
    INNER JOIN equipment ON Equipment.id = idEquipment
WHERE
    availableQuantity > 0;

-- Задача 4. Вывести тарифы на оборудование
-- Условие
-- Показать тарифы аренды для каждого оборудования.
SELECT
    *
FROM
    equipmentRates
    INNER JOIN equipment ON equipment.id = idEquipment
    INNER JOIN rentalTypes ON rentalTypes.id = idRentalType;

-- Задача 5. Вывести заказы с данными клиента и статусом
-- Условие
-- Для каждого заказа показать:
-- номер заказа,
-- ФИО клиента,
-- статус заказа,
-- дату начала и окончания аренды,
-- сумму заказа,
-- сумму залога.
SELECT
    ro.id,
    u.firstName,
    u.lastName,
    ro.dateStart,
    ro.dateEnd,
    ro.amount,
    ro.depositAmount
FROM
    rentOrders AS ro
    INNER JOIN users AS u ON ro.idUser = u.id
    INNER JOIN orderStatus AS os ON ro.idStatus = os.id;

--     Задача 6. Показать состав каждого заказа
--     Условие
-- Вывести список позиций заказов.
-- Для каждой позиции показать:
-- номер заказа,
-- название оборудования,
-- количество,
-- цену за единицу,
-- итоговую стоимость позиции.
-- orderItems ,equipment 
SELECT
    orderItems.id,
    equipment.title,
    orderItems.pricePerUnit,
    orderItems.amount,
    orderItems.quantity
FROM
    equipment
    INNER JOIN orderItems ON equipment.id = orderItems.idEquipment;

-- Задача 7. Вывести полную информацию по заказам и платежам
-- Условие
-- Показать сводную информацию по каждому заказу:
-- номер заказа,
-- клиент,
-- статус заказа,
-- сумма заказа,
-- способ оплаты,
-- статус оплаты,
-- сумма платежа.
-- rentOrders users orderStatus payments paymentMethods  
SELECT
    rentOrders.id,
    users.firstName || ' ' || users.lastName AS clientName,
    orderStatus.title                        AS orderStatus,
    rentOrders.amount                        AS paymentAmount,
    paymentMethods.title                     AS paymentMethod,
    paymentStatuses.title                    AS paymentStatus,
    payments.amount                          AS orderAmount
FROM
    rentOrders
    INNER JOIN users ON rentOrders.idUser = users.id
    INNER JOIN orderStatus ON orderStatus.id = rentOrders.idStatus
    INNER JOIN payments ON rentOrders.id = payments.idOrder
    INNER JOIN paymentMethods ON paymentMethods.id = payments.idMethod
    INNER JOIN paymentStatuses ON payments.idStatus = paymentStatuses.id
    -- Задача 8. Найти оборудование без фотографий
    -- Условие
    -- Вывести оборудование, для которого не загружено ни одной фотографии.
SELECT
    *
FROM
    equipment
    INNER JOIN equipmentPhotos ON equipmentPhotos.idEquipment = equipment.id
WHERE
    equipmentPhotos.idImage IS NULL
    --  Задача 9. Найти оборудование без тарифов
    -- Условие
    -- Вывести оборудование, для которого не задано ни одного тарифа аренды.
SELECT
    *
FROM
    equipment
    INNER JOIN equipmentRates ON equipmentRates.idEquipment = equipment.id
WHERE
    equipmentRates.idRentalType = NULL
    -- Задача 10. Подсчитать количество оборудования в каждой точке аренды
    -- Условие
    -- Для каждой точки аренды вывести:
    -- название точки,
    -- суммарное количество единиц оборудования,
    -- суммарное количество доступных единиц.
SELECT
    rentalPoints.name,
    SUM(rpe.totalQuantity)     AS totalEquipmentCount,
    SUM(rpe.availableQuantity) AS totalAvailableCount
FROM
    rentalPointEquipment rpe
    INNER JOIN rentalPoints ON rentalPoints.id = rpe.idRentalPoint
GROUP BY
    rentalPoints.id,
    rentalPoints.name
    --     Задача 11. Подсчитать количество заказов у каждого клиента
    -- Условие
    -- Для каждого клиента вывести:
    -- ФИО,
    -- количество заказов.
SELECT
    u.lastName || ' ' || u.firstName AS clientName,
    COUNT(ro.id)                     AS ordersCount
FROM
    rentOrders AS ro
    INNER JOIN users AS u ON u.id = ro.idUser
GROUP BY
    u.id,
    u.lastName,
    u.firstName
    -- Задача 13. Подсчитать выручку по каждому виду оборудования
    -- Условие
    -- Для каждого оборудования вывести суммарную выручку, полученную по всем заказам.
SELECT
    equipment.title,
    SUM(orderItems.amount)
FROM
    equipment
    INNER JOIN orderItems ON equipment.id = orderItems.idEquipment
GROUP BY
    equipment.id;

-- Задача 14. Подсчитать выручку по дням
-- Условие
-- Вывести ежедневную выручку по заказам.
SELECT
    DATE (dateCreated) AS orderDate,
    SUM(amount)        AS dailyRevenue
FROM
    rentOrders
GROUP BY
    orderDate
    -- Задача 15. Найти клиентов, у которых есть заказы на сумму больше заданной
    -- Условие
    -- Вывести клиентов, у которых есть хотя бы один заказ на сумму более 2000.
    -- Решение
SELECT
    *
FROM
    Users
    INNER JOIN rentOrders ON rentOrders.idUser = Users.id
WHERE
    rentOrders.amount > 2000
    -- Задача 16. Найти точки аренды, где есть хотя бы один велосипед
    -- Условие
    -- Показать точки аренды, в которых есть оборудование из категории Bicycle.
    -- Решение
SELECT
    e.title AS "Equipment        Title",
    c.title AS 'Category  Title'
FROM
    equipment AS e
    INNER JOIN categories AS c ON c.id = e.idCategory
    INNER JOIN rentalPointEquipment ON rentalPointEquipment.idEquipment = e.id
WHERE
    c.title = "Bicycle"
    AND rentalPointEquipment.totalQuantity > 0
    -- Задача 17. Показать заказы, в которых более одной позиции
    -- Условие
    -- Найти заказы, в которых суммарное количество единиц оборудования больше 1.
    -- Решение
SELECT
    *
FROM
    rentOrders
    INNER JOIN orderItems ON orderItems.idOrder = rentOrders.id
WHERE
    orderItems.quantity > 1
    --  Задача 18. Найти клиентов без заказов
    -- Условие
    -- Вывести клиентов, которые зарегистрированы в системе, но ещё не сделали ни одного заказа.   
SELECT
    *
FROM
    rentOrders
    INNER JOIN users ON users.id = rentOrders.idUser
WHERE
    rentOrders.id = NULL
    -- Задача 19. Найти оборудование, доступное в нескольких точках аренды
    -- Условие
    -- Показать оборудование, которое есть более чем в одной точке аренды.
SELECT
    e.id,
    e.title                           AS EquipmentTitle,
    COUNT(DISTINCT rpe.idRentalPoint) AS rentalPointCount
FROM
    rentalPointEquipment rpe
    JOIN equipment e ON rpe.idEquipment = e.id
GROUP BY
    e.id,
    e.title
HAVING
    COUNT(DISTINCT rpe.idRentalPoint) > 1
    -- Задача 20. Найти максимальную стоимость аренды по каждому оборудованию
    -- Условие
    -- Для каждого оборудования вывести максимальную стоимость аренды среди всех тарифов.
SELECT
    e.id,
    e.title,
    MAX(er.price) AS MaxPrice
FROM
    equipment e
    INNER JOIN equipmentRates er ON e.id = er.idEquipment
GROUP BY
    e.id,
    e.title
    -- Задача 21. Вывести среднюю стоимость аренды по категориям
    -- Условие
    -- Для каждой категории оборудования определить среднюю стоимость аренды.
SELECT
    c.id,
    c.title,
    AVG(er.price) AS [Average price]
FROM
    Equipment e
    INNER JOIN equipmentRates er ON e.id = er.idEquipment
    INNER JOIN equipmentType et ON et.id = e.idType
    INNER JOIN categories c ON c.id =e.idCategory
    GROUP BY
    c.id
    -- Задача 22. Найти заказ с максимальной суммой
    -- Условие
    -- Вывести заказ или заказы с наибольшей суммой.
    SELECT
    ro.id,
    ro.amount,
    u.lastName || ' ' || u.firstName AS clientName
    FROM
    rentOrders ro
    INNER JOIN users u ON ro.idUser = u.id
    WHERE ro.amount =
(
     SELECT
     MAX(amount)
     FROM rentOrders
);
    -- Задача 23. Вывести заказы, по которым ещё нет платежа
    -- Условие
    -- Показать заказы, для которых отсутствует запись в таблице payments.
    SELECT
    *
    FROM
    rentOrders ro
    INNER JOIN payments p ON p.idOrder = ro.id
    INNER JOIN orderItems ot ON ot.idOrder = ro.id
    WHERE p.id = NULL

--     Задача 24. Вывести все оплаченные заказы
-- Условие
-- Показать только те заказы, по которым статус платежа — Paid.
-- Решение
SELECT
*
FROM
payments p 
INNER JOIN paymentStatuses ps ON ps.id = p.idStatus
INNER JOIN rentOrders ro ON ro.idStatus = p.idOrder
WHERE ps.title = "Paid"

-- Задача 25. Вывести полную информацию о доступном оборудовании с тарифами
-- Условие

-- Показать каталог оборудования, доступного для аренды.
-- Вывести:

-- название,
-- категорию,
-- бренд,
-- точку аренды,
-- доступное количество,
-- тип аренды,
-- цену.
SELECT
 e.title as EquipmentTitle,
 b.title as BrandTitle,
 rp.address,
 rpe.availableQuantity,
 er.price,
 c.title CategoryTitle,
 rt.title as RentType
FROM
Equipment e
INNER JOIN equipmentRates er ON er.idEquipment = e.id
INNER JOIN rentalPointEquipment rpe ON rpe.idEquipment = e.id
INNER JOIN categories c ON c.id = e.idCategory
INNER JOIN brands b ON b.id = e.idBrand
INNER JOIN equipmentType et ON et.id = e.idType
INNER JOIN rentalPoints rp ON rp.id = rpe.idRentalPoint
INNER JOIN rentalTypes rt ON rt.id = er.idRentalType
WHERE availableQuantity > 0
GROUP BY e.id


-- Задача 26. Найти точки аренды, где суммарно доступно больше 10 единиц оборудования
-- Условие
-- Вывести точки аренды, в которых общее количество доступного оборудования больше 10.
-- Решение
SELECT
    rp.id,
    rp.name,
    Sum(rpe.availableQuantity) AS totalAvailable
FROM rentalPoints rp
INNER JOIN rentalPointEquipment rpe ON rpe.idRentalPoint = rp.id
GROUP BY 
rp.id, rp.name
HAVING SUM(rpe.availableQuantity) > 10

-- Задача 27. Найти оборудование, которое ни разу не арендовали
-- Условие
-- Вывести список оборудования, которое отсутствует в таблице orderItems.
-- Решение
SELECT
e.id,
e.title,
oi.id as OI_ID
FROM
orderItems oi
LEFT JOIN equipment e ON e.id = oi.idEquipment
WHERE oi.id is NULL


-- Задача 28. Найти клиентов, которые оплатили все свои заказы
-- Условие
-- Вывести клиентов, у которых нет ни одного заказа без платежа со статусом Paid.
-- Решение
SELECT
    u.id,
    u.lastName || ' ' || u.firstName AS clientName
FROM
users u
WHERE NOT EXISTS(
    SELECT 1
    FROM rentOrders ro
    LEFT JOIN payments p ON p.idOrder = ro.id
    LEFT JOIN paymentStatuses ps ON ps.id = p.idStatus
    WHERE ro.idUser = u.id
    AND (p.id IS NULL OR ps.title <> 'Paid')
)
AND (
    SELECT 1
    FROM rentOrders ro
    WHERE ro.idUser = u.id
);

-- Задача 29. Для каждого заказа вывести количество разных позиций
-- Условие
-- Показать по каждому заказу, сколько разных видов оборудования в него входит.
-- Решение
SELECT
oi.idOrder,
e.title,
e.idCategory
FROM
Equipment e
INNER JOIN orderItems oi ON oi.idEquipment = e.id
INNER JOIN categories c ON c.id = e.idCategory
GROUP BY e.idCategory


-- Задача 30. Найти категорию с наибольшей суммарной выручкой
-- Условие

-- Определить категорию оборудования, которая принесла максимальную суммарную выручку.

-- Решение
SELECT
 e.title,
 MAX(oi.amount)
FROM
Equipment e
INNER JOIN categories c ON c.id = e.idCategory
INNER JOIN orderItems oi ON oi.idEquipment = e.id
WHERE 
GROUP BY c.description 