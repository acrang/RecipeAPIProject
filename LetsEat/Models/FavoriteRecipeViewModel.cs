using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEat.Models
{
    public class FavoriteRecipeViewModel
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string RecipeUrl { get; set; }
        public string Ingredients { get; set; }
        public string Thumbnail { get; set; }
        public string Category { get; set; }
        public byte? Rating { get; set; }

    }
}
