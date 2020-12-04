using LetsEat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LetsEat.Controllers
{
    [Authorize]
    public class LetsEatController : Controller
    {

        private readonly LetsEatContext _db; 

        public LetsEatController(LetsEatContext db)
        {
            _db = db;
        }

        public IActionResult ShowAllFavorites(string sortOrder)
        {
            ViewData["CategorySortParm"] = sortOrder == "Category" ? "Category_desc" : "Category";
            ViewData["RatingSortParm"] = sortOrder == "Rating" ? "Rating_desc" : "Rating";
            var user = FindUser();
            var recipes = from all in _db.FavoriteRecipes
                            join Recipe in _db.UserFavoriteRecipes on all.Id equals Recipe.RecipeId
                            where Recipe.UserId.Equals(user)
                            select all;

            List<FavoriteRecipes> RecipeList = recipes.ToList();
            // create list of viewmodel objects to populate
            List<FavoriteRecipeViewModel> OutputList = new List<FavoriteRecipeViewModel>();
            foreach (var item in RecipeList)
            {
                FavoriteRecipeViewModel vm = new FavoriteRecipeViewModel();
                // find user favorite rating and category
                UserFavoriteRecipes ur = new UserFavoriteRecipes();

                ur = _db.UserFavoriteRecipes.Find(FindUser(), item.Id);
                if (!string.IsNullOrWhiteSpace(ur.Category))
                {
                    vm.Category = ur.Category;
                }
                vm.Rating = ur.Rating;
                vm.Title = item.Title;
                vm.RecipeID = ur.RecipeId;
                vm.RecipeUrl = item.RecipeUrl;
                vm.Ingredients = item.Ingredients;
                vm.Thumbnail = item.Thumbnail;
                OutputList.Add(vm);
            }

            switch (sortOrder)
            {
                case "Category":
                    OutputList = OutputList.OrderBy(x => x.Category).ToList();
                    break;
                case "Category_desc":
                    OutputList = OutputList.OrderByDescending(x => x.Category).ToList();
                    break;
                case "Rating":
                    OutputList = OutputList.OrderBy(x => x.Rating).ToList();
                    break;
                case "Rating_desc":
                    OutputList = OutputList.OrderByDescending(x => x.Rating).ToList();
                    break;
                default:
                    break;
            }
            return View(OutputList);
        }
        public async Task<IActionResult> EditFavorite(int id)
        {
            FavoriteRecipeViewModel vm = new FavoriteRecipeViewModel();
            FavoriteRecipes fr = _db.FavoriteRecipes.Find(id);
            vm.Title = fr.Title;
            vm.Ingredients = fr.Ingredients;
            vm.Thumbnail = fr.Thumbnail;
            vm.RecipeUrl = fr.RecipeUrl;
            // find user favorite rating and category
            var ur = _db.UserFavoriteRecipes.Find(FindUser(), fr.Id);
            vm.Category = ur.Category;
            vm.Rating = ur.Rating;
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditFavorite(FavoriteRecipes fr, string Category, byte Rating)
        {
            // create primary key from values
            string currentUserID = FindUser();
            int currentRecipeID = fr.Id;
            var ur = _db.UserFavoriteRecipes.Find(currentUserID, currentRecipeID);
            // adding values to selected record
            ur.Category = Category;
            ur.Rating = Rating;
            // necessary due to values being stored in main recipe table visible to all users
            fr.Category = "";
            fr.Rating = null;

            _db.UserFavoriteRecipes.Update(ur);
            _db.FavoriteRecipes.Update(fr);
            await _db.SaveChangesAsync();

            return RedirectToAction("ShowAllFavorites");
        }
        [HttpGet]
        public async Task<IActionResult> GetRecipe(Result r)
        {
            TempData["Name"] = r.title;

            return View(r);
        }
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(string title, string recipeUrl, string ingredients, string thumbnail)
        {
            FavoriteRecipes r = new FavoriteRecipes();
            r.Title = title;
            r.RecipeUrl = recipeUrl;
            r.Ingredients = ingredients;
            r.Thumbnail = thumbnail;

            // retrieve current ID for favorite recipe if it exists
            int favoriteID = (from RecipeURL in _db.FavoriteRecipes
                             where RecipeURL.RecipeUrl.Equals(recipeUrl)
                             select RecipeURL.Id).FirstOrDefault();

            // retrieve current ID for favorite recipe if it exists
            int existsInUserFavorites = 0;
            existsInUserFavorites = (from Recipe in _db.UserFavoriteRecipes
                             where Recipe.RecipeId.Equals(favoriteID)
                             select Recipe.RecipeId).FirstOrDefault();

            // adds currently selected recipe to FavoriteRecipes and newly added RecipeID and UserID to UserFavoriteRecipes
            if (ModelState.IsValid && favoriteID == 0 && existsInUserFavorites == 0)

            {
                await _db.FavoriteRecipes.AddAsync(r);
                await _db.SaveChangesAsync();

                var user = FindUser();
                UserFavoriteRecipes f = new UserFavoriteRecipes(user, r.Id);

                await _db.UserFavoriteRecipes.AddAsync(f);
                await _db.SaveChangesAsync();

                return RedirectToAction("ShowAllFavorites");
            }

            // if the currently selected recipe exists in database it will only add that recipeID and current UserID to UserFavoriteRecipes table
            if (favoriteID != 0 && existsInUserFavorites == 0)
            {
                var user = FindUser();
                UserFavoriteRecipes f = new UserFavoriteRecipes(user, favoriteID);

                await _db.UserFavoriteRecipes.AddAsync(f);
                await _db.SaveChangesAsync();

                return RedirectToAction("ShowAllFavorites");
            }
            return RedirectToAction("ShowAllFavorites");
        }

        [HttpGet]
        public IActionResult UpdateFavorite(int Id)
        {
            FavoriteRecipes r = _db.FavoriteRecipes.Find(Id);
            return View(r);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFavorite(FavoriteRecipes r)
        {
            if (ModelState.IsValid)
            {
                _db.FavoriteRecipes.Update(r);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("ShowAllFavorites");
        }
        [HttpGet]
        public IActionResult DeleteFavorite(int Id)
        {
            string user = FindUser();
            FavoriteRecipes r = _db.FavoriteRecipes.Find(Id);
            // create primary key from values
            int currentRecipeID = r.Id;
            var ur = _db.UserFavoriteRecipes.Find(user, currentRecipeID);
            FavoriteRecipeViewModel vm = new FavoriteRecipeViewModel();

            vm.Title = r.Title;
            vm.Ingredients = r.Ingredients;
            vm.RecipeUrl = r.RecipeUrl;
            vm.Category = ur.Category;
            vm.Rating = ur.Rating;
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFavorite(FavoriteRecipes r)
        {
            var recipe = _db.UserFavoriteRecipes.Where(x => x.RecipeId == r.Id).First();
            _db.UserFavoriteRecipes.Remove(recipe);

            string recipeID = r.Id.ToString();
            int recipeIDint = int.Parse(recipeID);
            //setup to check other users with this recipe as favorite before deleting
            var count = _db.UserFavoriteRecipes.Where(x => x.RecipeId == r.Id);
            if(count.Count() == 1)
            {
                var recipeObject = _db.FavoriteRecipes.Where(x => x.Id == r.Id).First();
                _db.FavoriteRecipes.Remove(recipeObject);
            }            
            
            await _db.SaveChangesAsync();

            return RedirectToAction("ShowAllFavorites");
        }
        public IActionResult RandomFavorite()
        {
            var user = FindUser();
            var recipes = from r in _db.FavoriteRecipes
                          where _db.UserFavoriteRecipes.Any(x => x.UserId == user && x.RecipeId == r.Id)
                          select r;
            var randomRecipe = recipes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            
            if (randomRecipe != null)
            {     
                // create primary key from values
                int currentRecipeID = randomRecipe.Id;
                var ur = _db.UserFavoriteRecipes.Find(user, currentRecipeID);
                FavoriteRecipeViewModel vm = new FavoriteRecipeViewModel();
    
                vm.Title = randomRecipe.Title;
                vm.Ingredients = randomRecipe.Ingredients;
                vm.RecipeUrl = randomRecipe.RecipeUrl;
                vm.Category = ur.Category;
                vm.Rating = ur.Rating;
                return View(vm);
            } 
            return View("ShowAllFavorites");
        }
        public string FindUser()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            return userId;
        }
    }
}
