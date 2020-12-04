CREATE DATABASE LetsEat;
USE DATABASE LetsEat;

CREATE TABLE FavoriteRecipes(
    ID int PRIMARY KEY IDENTITY(1,1),
	RecipeURL nvarchar(max),
	Title nvarchar(250) NOT NULL,
	Ingredients nvarchar(max),
	Thumbnail nvarchar(max),
	Rating tinyint,
	Category nvarchar(100)
);
CREATE TABLE UserFavoriteRecipes(
	UserID nvarchar(450) NOT NULL,
	RecipeID int NOT NULL,
	Category nvarchar(150),
	Rating tinyint,
	CONSTRAINT PK_UserFavoriteRecipes PRIMARY KEY NONCLUSTERED (UserID, RecipeID),
	CONSTRAINT FK_UserID FOREIGN KEY (UserID) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_RecipeID FOREIGN KEY (RecipeID) REFERENCES FavoriteRecipes(ID)
);