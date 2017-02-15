using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class CrawlerRecipesService : ThriftRecipesService.Iface, ICrawlerRecipesService
    {
        private List<Recipe> _recipes;

        public void GetRecipes(List<Recipe> recipes) => _recipes = recipes;

        public string GetRecipeData()
        {
            var recipesToJson = JsonConvert.SerializeObject(_recipes);
            return recipesToJson;
        }
    }
}
