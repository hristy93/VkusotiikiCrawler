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
        public string GetRecipeData()
        {
            var recipesToJson = JsonConvert.SerializeObject(VkusotiikiCrawler.Recipes);
            return recipesToJson;
        }
    }
}
