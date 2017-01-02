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
        public int User { get; set; } = 1;

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
                    //int matchIndex = match.Index + match.Length;
                    //string fracNumbers = Description.Substring(matchIndex, );
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
