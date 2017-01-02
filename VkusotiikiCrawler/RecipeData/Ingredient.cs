using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class Ingredient
    {
        public static readonly string[] OTHER_ALERGIES = new string[] {"ядки", "орех", "лешни", "сусам", "фъстъ", "кашу", "бадем",
            "кестен", "шам-фъстъ", "соя", "пшени", "500", "горчица",  "целина" };
        public static readonly string[] MEAT = new string[] { "риба", "скумри", "шаран", "рибн", "кайма", "телешк", "овч", "агнешк",
        "свинско", "суджук", "филе", "заеш", "месо", "кайма", "кренвирш", "кюфте", "говежд", "скарид", "сьомг", "пъстърв", "ципур",
        "риба тон", "треска", "овнешк"};
        public static readonly string[] DAIRY = new string[] { "мляко", "млечн", "майонеза", "мед", "яйц", "сирене", "кашкавал", "масло",
        "ивара", "катък", "сметана", "кефир", "чедър", "пармезан"};

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("unstructured_data")]
        public string UnstructuredData { get; set; }

        [JsonProperty("is_allergic")]
        public bool IsAllergic { get; set; } = false;

        public Ingredient()
        {

        }

        public Ingredient(string ingredients)
        {
            var splittedIngredients = ingredients.Split(' ');
            if (splittedIngredients.Count() == 3)
            {
                Quantity = splittedIngredients[0];
                Unit = splittedIngredients[1];
                Name = splittedIngredients[2];
            }
            else
            {
                Quantity = splittedIngredients[0];
                Unit = splittedIngredients[1];
                string ingredientName = "";
                for (int i = 2; i < splittedIngredients.Count(); i++)
                {
                    ingredientName += splittedIngredients[i] + " ";
                }
                Name = ingredientName;
            }

            UnstructuredData = ingredients;
            FindAlergies();
            FixIngredientName();
        }

        private void FindAlergies()
        {
            foreach (var item in Name.Split(' '))
            {
                foreach (var allergy in OTHER_ALERGIES.Concat(MEAT).Concat(DAIRY))
                {
                    if (item.StartsWith(allergy))
                    {
                        IsAllergic = true;
                        break;
                    }
                }
            }
        }

        private void FixIngredientName() => Name = Name.Trim();
    }
}
