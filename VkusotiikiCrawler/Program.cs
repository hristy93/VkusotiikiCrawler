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
        private const int RECIPES_COUNT_LIMIT = 600;

        static void Main(string[] args)
        {
            IRecipeWebsite recipeWebsite = new KulinarBg();
            VkusotiikiCrawler crawler = new VkusotiikiCrawler(recipeWebsite);
            string ipAddress = "127.0.0.1";
            //string ipAddress = "192.168.0.108";
            int port = 3333;
            if (args.Count() != 0)
            {
                ipAddress = args[0];
                port = Int32.Parse(args[1]);
            }

            // starts the crawler or one run or until some recipe count limit is reached
            crawler.RunCrawler();
            //crawler.RunCrawler(RECIPES_COUNT_LIMIT);

            // starts a Apache Thrit server for listening on a IP Address and a port number
            //new ThriftServer(ipAddress, port).Start();
        }
    }
}
