using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class Recipe
    {

        public static readonly string[] USER_IDS = new string[]
        {
            "170f7163-6da9-4536-b8d6-cf588abc8c1e",
            "8ff53edb-4442-4880-a9f8-89bc7218b692",
            "bc6e69b9-6162-4e69-8028-900abfb9030e",
            "f121925d-d296-4818-870f-5f453389c4a8",
            "fffae408-45a8-42e0-a361-2c5c0735645f",
        };

        public static readonly Dictionary<string, string> InstructionFixes = new Dictionary<string, string>()
            {
                { @"\.\s?", ". " },
                { @"!\s?",  "! " },
                { @"([. ]*)?&nbsp;([. ]*)?", ". " },
                { @"([! ]*)?&nbsp;([! ]*)?", "! " },
                { @"([ ]*)?&ndash;([ ]*)?", " - "},
                { "(([ ]*)?(\\r\\n)+([ ]*)?)+", " " },
                { "([ ]*)?&ldquo;([ ]*)?", " \""},
                { "([ ]*)?&rdquo;([ ]*)?", "\" "},
                { "([ ]*)&bdquo;([ ]*)?", " \""},
                { "([ ]*)&deg;([ ]*)?", ""},
                { "([ ]*)&quot;([ ]*)?", " \" "}
            };

        public readonly static string[] FORBIDDEN_TITLES = new string[] { "италиан", "турск", "индий", "паста", "спагети", "палачинк",
        "англий", "арабск", "макарон", "спагети", "грузинск", "мъфин", "джинджифил", "сандвич", "пудинг", "торта", "кекс", "топено",
        "сладолед", "немск", "африка", "рикота", "талиатели", "бутер", "болонезе", "ризото", "бургер", "пай", "пица", "фъстъч",
        "крутон", "шербет", "борш", "равиоли", "шницел", "китайск", "тако", "чоризо", "бишкот", "средиземноморск", "холандск", "коктейл",
        "мезе", "дип", "песто", "япон", "чийзкейк", "разядк", "фъдж", "тарт", "трюфел", "манго", "гуакамоле", "лазаня", "испан", "сироп",
        "глазура", "пастет", "карпачо", "ролца", "руло", "бисквити", "джейми", "пунш", "мармалад", "тайланд", "целувк", " тост", "руски",
        "скарид", "авокадо", "дюнер", "годжи", "шведск", "ирланд", "индия", "фрикасе", "суши", "ананас", "гофрет", "халва", "сорбе", "кокос",
        "киш", "канелон", "бонбон", "баклав", "аспержи", "филипин", "брускет", "хапки", "ориенталск", "кебап", "синьо", "мус", "тарталет",
        "брауни", "моцарела", "пармезан", "брауни", "омлет", "поничк", "ратату", "персийск", "гризини", "хавайск", "киноа", "трева",
        "суфле", "гръцк", "шейк", "смути", "кейк", "портокал", "банан", "марината", "парфе", "алжирск", "кроасан", "еклер", "тоскан",
        "сръбск", "крилца", "маршмелоу", "герман", "сельодка", "щрудел", "гулаш", "унгарск", "касерола", "мексик", "кордон-бльо", "булгур",
        "джалеби", "бразил", "тирамису", "америка", "кренвирш", "паеля", "рьощ", "тортийа", "тортия", "плескавиц", "прошуто" };

        private static Random _random = StaticRandom.Instance;

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("duration")]
        public string Duration { get; set; } = "0";

        [JsonProperty("ingredients")]
        public List<Ingredient> Ingredients { get; set; }

        [JsonProperty("difficulty")]
        public int Difficulty { get; set; } = 1;

        [JsonProperty("servings")]
        public int Servings { get; set; } = 1;

        [JsonProperty("user")]
        public string User { get; set; } = "";

        [JsonProperty("category")]
        public int Category { get; set; } = 1;

        [JsonProperty("dish")]
        public int Dish { get; set; } = 1;

        [JsonProperty("region")]
        public int Region { get; set; } = 1;

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
        }

        public void FixRecipeProblems()
        {
            FixInstructions();
            TrimTitle();
            FindAlergies();
            FindDifficulty();
            FixIngredientsNames();
            FixUserIds();
        }

        private void FixUserIds()
        {
            if (String.IsNullOrEmpty(User) || User == "1")
            {
                int randomUserIdIndex = _random.Next(0, USER_IDS.Count());
                string userId = USER_IDS[randomUserIdIndex];
                User = userId; 
            }
        }

        private void FixIngredientsNames()
        {
            foreach (var ingredient in Ingredients)
            {
                ingredient.Name = ingredient.Name.Trim();
            }
        }

        private void FindDifficulty()
        {
            int ingredientsCount = Ingredients.Count;
            if (ingredientsCount <= 5)
            {
                Difficulty = 1;
            }
            else if (ingredientsCount > 5 && ingredientsCount <= 7)
            {
                Difficulty = 2;
            }
            else if (ingredientsCount > 7 && ingredientsCount <= 9)
            {
                Difficulty = 3;
            }
            else if (ingredientsCount > 9 && ingredientsCount <= 11)
            {
                Difficulty = 4;
            }
            else if (ingredientsCount > 11)
            {
                Difficulty = 5;
            }
        }

        private void TrimTitle()
        {
            if (Name != null)
            {
                string pattern1 = @"(\r\n)*";
                string pattern2 = @"(\d)?( )*(\d)*$";
                string replacement = "";
                Regex rgx1 = new Regex(pattern1);
                Regex rgx2 = new Regex(pattern2);
                Name = rgx1.Replace(Name, replacement);
                Name = rgx2.Replace(Name, replacement);
                Name = Name.TrimStart(' ');
                Name = Name.TrimEnd(' ');
            }
        }

        private void FindAlergies()
        {
            foreach (var ingredient in Ingredients)
            {
                foreach (var item in ingredient.Name.Split(' '))
                {
                    foreach (var allergy in Ingredient.OTHER_ALERGIES.
                        Concat(Ingredient.MEAT).Concat(Ingredient.DAIRY))
                    {
                        if (item.StartsWith(allergy))
                        {
                            ingredient.IsAllergic = true;
                            break;
                        }
                    }
                }
            }
        }

        private void FixInstructions()
        {
            string fracPattern = @"([ ]*)?&frac(?<num>(\d)*);([ ]*)?";
            Regex fracRegex = new Regex(fracPattern);
            MatchCollection fracMaches = null;

            if (fracRegex.IsMatch(Description))
            {
                fracMaches = fracRegex.Matches(Description);

                foreach (Match match in fracMaches)
                {
                    var fracNumbers = match.Groups["num"].Value;
                    string fracResult = " " + fracNumbers[0] + "/" + fracNumbers[1] + " ";
                    Description = Regex.Replace(Description, match.ToString(), fracResult);
                }
            }


            foreach (var item in InstructionFixes)
            {
                Regex regex = new Regex(item.Key);
                Description = regex.Replace(Description, item.Value);
            }
        }
    }
}
