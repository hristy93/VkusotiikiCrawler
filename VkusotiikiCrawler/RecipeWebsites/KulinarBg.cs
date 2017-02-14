using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    class KulinarBg : IRecipeWebsite
    {
        private const string URL_PATH = "http://kulinar.bg/%D0%91%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%9A%D1%83%D1%85%D0%BD%D1%8F-%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8_l.rl_rn.1.html";

        public void GetRecipeDataFromHTML(HtmlDocument htmlAgilityPackDocument, List<Recipe> recipes)
        {
            List<string> recipeIngredients = new List<string>();
            var recipeTitleElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='recipeHead']")?.First();
            string recipeName = recipeTitleElement?.InnerText;

            if (recipeName != null)
            {
                var recipeIngredientsElements = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='item mb20 fs14 color3']");
                if (recipeIngredientsElements != null)
                {
                    foreach (var item in recipeIngredientsElements)
                    {
                        var unit = item.ChildNodes[1].InnerText;
                        var quantity = item.ChildNodes[2].InnerText;
                        if (!String.IsNullOrWhiteSpace(quantity) || !String.IsNullOrWhiteSpace(unit))
                        {
                            recipeIngredients.Add(unit + " " + quantity); 
                        }
                    }
                }

                string recipeInstructions = String.Empty;
                var recipeInstructionsElements = htmlAgilityPackDocument.DocumentNode.SelectNodes("//li[@class='item mb15 relative fs14 articleText']");
                if (recipeInstructionsElements != null)
                {
                    foreach (var item in recipeInstructionsElements)
                    {
                        if (item.ChildNodes.Count <= 3)
                        {
                            recipeInstructions += item.ChildNodes[2].InnerText;
                        }
                        else
                        {
                            recipeInstructions += item.ChildNodes[3].InnerText;
                        }
                    }
                }

                var recipeDurationElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//span[@class='fs24 bold']")?.First();
                string recipeDuration = recipeDurationElement?.InnerText.Trim();

                var recipeServingsElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//input[@class='fs24 openSans bold alignCenter colorDef']")?.First();
                string recipeServings = recipeServingsElement?.GetAttributeValue("value", "1").Trim();

                if (recipes.Where(s => recipeName.Contains(s.Name)).Count() == 0 &&
                        !Recipe.FORBIDDEN_TITLES.Any(s => recipeName.ToLower().Contains(s)))
                {
                    Recipe newRecipe = new Recipe();
                    newRecipe.Name = recipeName;
                    newRecipe.Description = recipeInstructions;
                    foreach (var item in recipeIngredients)
                    {
                        newRecipe.Ingredients.Add(new Ingredient(item));
                    }

                    if (String.IsNullOrWhiteSpace(recipeDuration))
                    {
                        recipeDuration = "40 мин.";
                    }

                    newRecipe.Duration = recipeDuration;
                    newRecipe.Servings = Int32.Parse(recipeServings);
                    newRecipe.FixRecipeProblems();
                    recipes.Add(newRecipe);
                }
                else
                {
                    Console.WriteLine($"Recipe with name {recipeName} is alredy there!");
                }
            }

        }

        public string GetURLPath() => URL_PATH;
    }
}
