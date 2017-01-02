using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace VkusotiikiCrawler
{
    class Program
    {
        //private const string JSON_FILE_PATH = @"../../Recipes/recipes.json";
        private const int RECIPES_COUNT_LIMIT = 600;

        //static readonly char[] TRIM_CHRACTERS = new char[3] {' ', '\r', '\n' };
        //const string URL_PATH = "http://www.receptite.com/%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8-%D0%BE%D1%82/%D0%B1%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%BA%D1%83%D1%85%D0%BD%D1%8F";
        //const string URL_PATH = "http://kulinar.bg/%D0%91%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%9A%D1%83%D1%85%D0%BD%D1%8F-%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8_l.rl_rn.1.html";
        //const string URL_PATH = "http://recepti.gotvach.bg/r-56561-%D0%9B%D0%B5%D1%81%D0%BD%D0%BE_%D0%BF%D0%BE%D1%81%D1%82%D0%BD%D0%BE_%D0%BF%D1%80%D0%B5%D0%B4%D1%8F%D1%81%D1%82%D0%B8%D0%B5";
        //private static PoliteWebCrawler Crawler { get; set; }
        //public static List<Recipe> Recipes { get; set; }

        static void Main(string[] args)
        {
            // Пълнени пуешки бутчета в гърне
            IRecipeWebsite recipeWebsite = new KulinarBg();
            VkusotiikiCrawler crawler = new VkusotiikiCrawler(recipeWebsite);
            crawler.RunCrawler();
            //string ipAddress = "127.0.0.1";
            string ipAddress = "192.168.0.108";
            int port = 9090;
            if (args.Count() != 0)
            {
                ipAddress = args[0];
                port = Int32.Parse(args[1]);
            }
            //GetJsonRpcRequests(ipAddress, port);
            crawler.RunCrawler();
            //crawler.RunCrawler(RECIPES_COUNT_LIMIT);
            //TestSmth();
        }

        private static void TestSmth()
        {
            //Recipe recipe = new Recipe();
            //recipe.FixRecipeProblems();
            //string str = "Еклери";
            //bool valid = Recipe.FORBIDDEN_TITLES.Any(s => str.ToLower().Contains(s));
        }

        private static void GetJsonRpcRequests(string ipAddress, int port)
        {
            //CrawlerRecipesService service = new CrawlerRecipesService();
            //DatabaseCommunicator communicator = new DatabaseCommunicator(ipAddress, port, service);
            //communicator.ConnectToDatabase();
            var server = new ThriftServer(ipAddress, port);
            server.Start();
        }
    }
}
