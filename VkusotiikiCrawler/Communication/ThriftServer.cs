using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Thrift.Server;
using Thrift.Transport;

namespace VkusotiikiCrawler
{
    public class ThriftServer
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public ThriftServer(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
            JSONManager manager = new JSONManager(VkusotiikiCrawler.JSON_FILE_PATH);
            if (VkusotiikiCrawler.Recipes.Count == 0)
            {
                VkusotiikiCrawler.Recipes = manager.ReadRecipes();
            }
        }

        public void Start()
        {
            try
            {
                CrawlerRecipesService service = new CrawlerRecipesService();
                ThriftRecipesService.Processor processor = new ThriftRecipesService.Processor(service);
                TcpListener tcpListener = new TcpListener(IPAddress.Parse(IpAddress), Port);
                TServerTransport serverTransport = new TServerSocket(tcpListener);
                TServer server = new TSimpleServer(processor, serverTransport);

                // Use this for a multithreaded server
                // server = new TThreadPoolServer(processor, serverTransport);

                Console.WriteLine("Starting the server...");
                server.Serve();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex.InnerException);
            }

            Console.WriteLine("Done.");
        }
    }
}
