create table Brands
(
	BrandId INT PRIMARY KEY,
	BrandName NVARCHAR(30) NOT NULL
)
create table Models
(
	ModelId INT PRIMARY KEY,
	ModelName NVARCHAR(30) NOT NULL
)
GO
create table Shoes
(
ShoeId INT PRIMARY KEY,
FirstIntroducedOn DATE NOT NULL,
Active BIT NOT NULL DEFAULT 1,
Picture NVARCHAR(150) NOT NULL,
BrandId INT NOT NULL REFERENCES Brands(BrandId),
ModelId INT NOT NULL REFERENCES Models(ModelId)
)
GO
create table Stocks
(
StockId INT PRIMARY KEY,
Size NVARCHAR(15) NOT NULL,
Price MONEY NOT NULL,
StockQuantity INT NOT NULL,
ShoeId INT NOT NULL REFERENCES Shoes (ShoeId)
)
GO