using Abot.Crawler;
using Abot.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class VkusotiikiCrawler
    {
        public const string JSON_FILE_PATH = @"../../Recipes/new/recipes.json";

        public static List<Recipe> Recipes;

        private string _URLPath;
        private PoliteWebCrawler _crawler;
        private RecipesFixer _recipesFixer;
        private IRecipeWebsite _recipeWebsite;

        public VkusotiikiCrawler(IRecipeWebsite recipeWebsite)
        {
            _recipeWebsite = recipeWebsite;
            _URLPath = recipeWebsite.GetURLPath();
            Recipes = new List<Recipe>();
            _recipesFixer = new RecipesFixer();
        }

        public void RunCrawler(int recipesCount)
        {
            while (Recipes.Count <= recipesCount)
            {
                RunCrawler();
            }

            _recipesFixer.FixRecipes(Recipes);
            _recipesFixer.TrimRecipes(Recipes);
        }

        public void RunCrawler()
        {
            int initialRecipesCount = Recipes.Count();
            JSONManager manager = new JSONManager(JSON_FILE_PATH);
            Recipes = manager.ReadRecipes();
            //InitiateCrawler();
            //StartCrawler();
            _recipesFixer.FixRecipes(Recipes);
            _recipesFixer.TrimRecipes(Recipes);
            if (initialRecipesCount != Recipes.Count())
            {
                manager.WriteRecipes(Recipes);
            }
        }

        public void StartCrawler()
        {
            //This is synchronous, it will not go to the next line until the crawl has completed
            CrawlResult result = _crawler.Crawl(new Uri(_URLPath));

            if (result.ErrorOccurred)
                Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
        }

        private void InitiateCrawler()
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            crawlConfig.CrawlTimeoutSeconds = 100;
            crawlConfig.MaxConcurrentThreads = 10;
            crawlConfig.MaxPagesToCrawl = 1000;
            //crawlConfig.UserAgentString = "abot v1.0 http://code.google.com/p/abot";

            //Will use the manually created crawlConfig object created above
            _crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);

            _crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            _crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            _crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            _crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
        }

        private void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        private void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser

            _recipeWebsite.GetRecipeDataFromHTML(htmlAgilityPackDocument, Recipes);
            //ReceptiteBg(htmlAgilityPackDocument);
            //ReceptiteGotvachBg(htmlAgilityPackDocument);

        }

        private void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        private void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
