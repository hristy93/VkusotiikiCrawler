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
            "кестен", "шам-фъстъ", "соя", "пшени", "500", "горчица",  "целина" , "типов"};
        public static readonly string[] MEAT = new string[]  { "кайма", "телешк", "овч", "агнешк", "свинск", "суджук",
            "филе", "заеш", "месо", "кайма", "кренвирш", "кюфте", "говежд", "скарид", "овнешк", "пиле", "пуйка", "патешк",
            "надениц", "пушен", "роле", "шпек", "колбас", "еленск", "шунка", "гъши", "гъск", "бекон", "агнешк", "кървавиц",
            "салам", "пастърма" };
        public static readonly string[] FISH = new string[] { "риба", "скумрия", "шаран", "рибн" , "рибен", "сьомга",
            "пъстърв", "ципур", "щука", "риба тон", "треска", "барабун", "султанк", "атерина", "сребърка", "зарган",
            "костур", "калкан", "карагьоз", "каракуда", "лаврак", "лефер", "моруна", "попче", "паламуд", "писия",
            "сафрид", "сардин", "цаца", "хамсия" };
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
                string ingredientName = String.Empty;
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
                foreach (var allergy in OTHER_ALERGIES.Concat(MEAT)
                    .Concat(FISH).Concat(DAIRY))
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
