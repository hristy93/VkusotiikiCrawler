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
            // Пълнени пуешки бутчета в гърне
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

            crawler.RunCrawler();
            //crawler.RunCrawler(RECIPES_COUNT_LIMIT);
            //GetJsonRpcRequests(ipAddress, port);
        }
 

        private static void GetJsonRpcRequests(string ipAddress, int port)
        {
            var server = new ThriftServer(ipAddress, port);
            server.Start();
        }
    }
}
