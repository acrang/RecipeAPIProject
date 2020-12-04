using LetsEat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEat.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecipeDAL _dal;

        public HomeController(RecipeDAL dal)
        {
            _dal = dal;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchResults(string DishName, string Ingredients, string ExcludedIngredients, bool OnlyImages, int Page)
        {
            TempData["DishName"] = DishName;
            TempData["Ingredients"] = Ingredients;
            TempData["QueryDescription"] = BuildQueryDescription(DishName, Ingredients, ExcludedIngredients);
            TempData["OnlyImages"] = OnlyImages;
            TempData["Page"] = Page;
            string query = BuildSearchQuery(DishName, Ingredients, ExcludedIngredients, OnlyImages, Page);
            TempData["Query"] = query;
            Rootobject ro;

            ro = await _dal.FindRecipesAsync(query);

            Result[] results = ro.results;

            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public string BuildSearchQuery(string DishName, string Ingredients, string ExcludedIngredients, bool OnlyImages, int Page)
        {
            string query = "?";

            if (DishName != null)
            {
                query += $"q={DishName}&";
            }

            //add ingredients modifiers
            if (Ingredients != null && ExcludedIngredients == null)
            {
                query += $"i={Ingredients}&";
            }
            else if (ExcludedIngredients != null && Ingredients == null)
            {
                query += $"i={BuildExcludingString(ExcludedIngredients)}&";
            }
            else if (ExcludedIngredients != null && Ingredients != null)
            {
                query += $"i={Ingredients},{BuildExcludingString(ExcludedIngredients)}&";
            }

            if (OnlyImages == true)
            {
                query += $"oi=1&";
            }

            query += $"p={Page}";

            return query;
        }

        public string BuildExcludingString(string excluding)
        {
            string[] ingredients = excluding.Split(",");
            string output = "";

            for (int i = 0; i < ingredients.Length; i++)
            {
                ingredients[i] = $"-{ingredients[i].Trim()}";
            }

            output = string.Join(",", ingredients);
            return output;
        }
        public string BuildQueryDescription(string DishName, string Ingredients, string ExcludedIngredients)
        {
            string queryDescription = "";

            if (DishName != null)
            {
                queryDescription += $"Recipes matching: {DishName}";
            }

            if (DishName == null && Ingredients != null)
            {
                queryDescription += $"Recipes containing: '{Ingredients}'";
            }

            if (Ingredients != null && DishName != null)
            {
                queryDescription += $" containing: '{Ingredients}'";
            }

            if (ExcludedIngredients != null)
            {
                queryDescription += $"excluding: '{ExcludedIngredients}'";
            }

            return queryDescription;
        }
    }
}
