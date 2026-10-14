/* ============================================================
   DE KIEM TRA GIUA KY - QUAN LY MON AN
   MSSV: BIT240049
   Database: BIT240049_MID
   Chay toan bo script nay trong SSMS (Ctrl + A, roi F5)
   ============================================================ */

USE master;
GO

IF DB_ID(N'BIT240049_MID') IS NOT NULL
BEGIN
    ALTER DATABASE BIT240049_MID SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BIT240049_MID;
END
GO

CREATE DATABASE BIT240049_MID;
GO

USE BIT240049_MID;
GO

/* ------------------------------------------------------------
   1. Bang loai mon an
   ------------------------------------------------------------ */
CREATE TABLE dbo.BIT240049_DishCategories
(
    BIT240049_Id          INT IDENTITY(1,1) NOT NULL,
    BIT240049_Name        NVARCHAR(100)     NOT NULL,
    BIT240049_Description NVARCHAR(500)     NULL,
    CONSTRAINT PK_BIT240049_DishCategories PRIMARY KEY (BIT240049_Id),
    CONSTRAINT CK_BIT240049_DishCategories_Name CHECK (LEN(LTRIM(RTRIM(BIT240049_Name))) > 0)
);
GO

/* ------------------------------------------------------------
   2. Bang mon an
   ------------------------------------------------------------ */
CREATE TABLE dbo.BIT240049_Dishes
(
    BIT240049_Id              INT IDENTITY(1,1) NOT NULL,
    BIT240049_Name            NVARCHAR(150)     NOT NULL,
    BIT240049_Price           DECIMAL(18,2)     NOT NULL,
    BIT240049_PreparationTime INT               NOT NULL,
    BIT240049_IsAvailable     BIT               NOT NULL CONSTRAINT DF_BIT240049_Dishes_IsAvailable DEFAULT (1),
    BIT240049_Description     NVARCHAR(1000)    NULL,
    BIT240049_DishCategoryId  INT               NOT NULL,
    CONSTRAINT PK_BIT240049_Dishes PRIMARY KEY (BIT240049_Id),
    CONSTRAINT FK_BIT240049_Dishes_DishCategories
        FOREIGN KEY (BIT240049_DishCategoryId)
        REFERENCES dbo.BIT240049_DishCategories (BIT240049_Id)
        ON DELETE NO ACTION,                                  -- khong cho xoa loai dang co mon
    CONSTRAINT CK_BIT240049_Dishes_Name CHECK (LEN(LTRIM(RTRIM(BIT240049_Name))) > 0),
    CONSTRAINT CK_BIT240049_Dishes_Price CHECK (BIT240049_Price > 0),
    CONSTRAINT CK_BIT240049_Dishes_PreparationTime CHECK (BIT240049_PreparationTime > 0),
    -- ten mon khong trung trong cung mot loai
    CONSTRAINT UQ_BIT240049_Dishes_Category_Name UNIQUE (BIT240049_DishCategoryId, BIT240049_Name)
);
GO

CREATE INDEX IX_BIT240049_Dishes_DishCategoryId ON dbo.BIT240049_Dishes (BIT240049_DishCategoryId);
GO

/* ------------------------------------------------------------
   3. Bang anh mon an
   ------------------------------------------------------------ */
CREATE TABLE dbo.BIT240049_DishImages
(
    BIT240049_Id          INT IDENTITY(1,1) NOT NULL,
    BIT240049_ImageUrl    NVARCHAR(500)     NOT NULL,
    BIT240049_IsThumbnail BIT               NOT NULL CONSTRAINT DF_BIT240049_DishImages_IsThumbnail DEFAULT (0),
    BIT240049_DishId      INT               NOT NULL,
    CONSTRAINT PK_BIT240049_DishImages PRIMARY KEY (BIT240049_Id),
    CONSTRAINT FK_BIT240049_DishImages_Dishes
        FOREIGN KEY (BIT240049_DishId)
        REFERENCES dbo.BIT240049_Dishes (BIT240049_Id)
        ON DELETE CASCADE,
    CONSTRAINT CK_BIT240049_DishImages_ImageUrl CHECK (LEN(LTRIM(RTRIM(BIT240049_ImageUrl))) > 0)
);
GO

CREATE INDEX IX_BIT240049_DishImages_DishId ON dbo.BIT240049_DishImages (BIT240049_DishId);
GO

-- Moi mon toi da 1 anh dai dien (IsThumbnail = 1)
CREATE UNIQUE INDEX UX_BIT240049_DishImages_OneThumbnail
    ON dbo.BIT240049_DishImages (BIT240049_DishId)
    WHERE BIT240049_IsThumbnail = 1;
GO

/* ------------------------------------------------------------
   4. Du lieu mau: 3 loai, 6 mon (>= 5), moi mon dung 1 loai
   ------------------------------------------------------------ */
INSERT INTO dbo.BIT240049_DishCategories (BIT240049_Name, BIT240049_Description) VALUES
(N'Main',    N'Main dishes / Món chính'),
(N'Drink',   N'Drinks / Đồ uống'),
(N'Dessert', N'Desserts / Tráng miệng');
GO

INSERT INTO dbo.BIT240049_Dishes
    (BIT240049_Name, BIT240049_Price, BIT240049_PreparationTime, BIT240049_IsAvailable, BIT240049_Description, BIT240049_DishCategoryId)
VALUES
(N'Phở bò',    50000, 10, 1, N'Phở bò truyền thống',         1),   -- Quick
(N'Bún chả',   60000, 20, 1, N'Bún chả nướng than hoa',      1),   -- Regular
(N'Trà đá',     5000,  2, 1, N'Trà đá giải nhiệt',           2),   -- Quick, chua co anh
(N'Cà phê sữa',25000,  8, 1, N'Cà phê sữa đá',               2),   -- Quick
(N'Chè thái',  30000, 15, 0, N'Chè thái nhiều topping',      3),   -- Quick, ngung ban
(N'Bánh flan', 20000, 25, 1, N'Bánh flan caramel',           3);   -- Regular
GO

-- Anh mau: mon 1 co 2 anh (1 anh dai dien), mon 2 co 1 anh dai dien,
-- mon 4 co 2 anh (chua chon anh dai dien), mon 3, 5, 6 chua co anh
INSERT INTO dbo.BIT240049_DishImages (BIT240049_ImageUrl, BIT240049_IsThumbnail, BIT240049_DishId) VALUES
(N'https://picsum.photos/seed/pho1/600/400',  1, 1),
(N'https://picsum.photos/seed/pho2/600/400',  0, 1),
(N'https://picsum.photos/seed/buncha1/600/400', 1, 2),
(N'https://picsum.photos/seed/cafe1/600/400', 0, 4),
(N'https://picsum.photos/seed/cafe2/600/400', 0, 4);
GO

/* ------------------------------------------------------------
   5. Kiem tra nhanh
   ------------------------------------------------------------ */
SELECT * FROM dbo.BIT240049_DishCategories;
SELECT * FROM dbo.BIT240049_Dishes;
SELECT * FROM dbo.BIT240049_DishImages;
GO
