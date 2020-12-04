using System;
using System.Collections.Generic;

namespace LetsEat.Models
{
    public partial class FavoriteRecipes
    {
        public FavoriteRecipes()
        {
            UserFavoriteRecipes = new HashSet<UserFavoriteRecipes>();
        }

        public int Id { get; set; }
        public string RecipeUrl { get; set; }
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Thumbnail { get; set; }
        public byte? Rating { get; set; }
        public string Category { get; set; }

        public FavoriteRecipes(string RecipeUrl, string Title, string Ingredients, string Thumbnail, byte Rating, string Category) 
        {
            this.RecipeUrl = RecipeUrl;
            this.Title = Title;
            this.Ingredients = Ingredients;
            this.Thumbnail = Thumbnail;
            this.Rating = Rating;
            this.Category = Category;
        }

        public virtual ICollection<UserFavoriteRecipes> UserFavoriteRecipes { get; set; }
    }
}
