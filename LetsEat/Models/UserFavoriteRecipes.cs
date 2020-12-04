using System;
using System.Collections.Generic;

namespace LetsEat.Models
{
    public partial class UserFavoriteRecipes
    {
        public string UserId { get; set; }
        public int RecipeId { get; set; }
        public string Category { get; set; }
        public byte Rating { get; set; }

        public UserFavoriteRecipes() { }

        public UserFavoriteRecipes(string UserId, int RecipeId)
        {
            this.UserId = UserId;
            this.RecipeId = RecipeId;
        }
        public virtual FavoriteRecipes Recipe { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
