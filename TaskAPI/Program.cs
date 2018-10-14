using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskAPI
{
    /// <summary>
    /// main class program web host
    /// </summary>
    public class Program
    {
        /// <summary>
        /// main for console
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// middle ware
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {

                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>();
    }
}
