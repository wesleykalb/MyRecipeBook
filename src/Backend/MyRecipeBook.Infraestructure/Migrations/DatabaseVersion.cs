﻿namespace MyRecipeBook.Infraestructure.Migrations
{
    public abstract class DatabaseVersion
    {
        public const int TABLE_USER = 1;
        public const int TABLE_RECIPES = 2;
        public const int IMAGE_FOR_RECIPES = 3;
        public const int TABLE_REFRESH_TOKEN = 4;
    }
}
