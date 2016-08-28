using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace Stubbornium.Sample.Utils
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(LogTraffic);
            app.UseFileServer();
        }

        private static async Task LogTraffic(IOwinContext context, Func<Task> next)
        {
            Console.WriteLine($"-> {context.Request.Method} {context.Request.Path}");

            try
            {
                await next();
            }
            catch (Exception e)
            {
                Console.WriteLine($"!! {e.Message}");
            }
            finally
            {
                Console.WriteLine($"<- {context.Response.StatusCode}");
            }
        }
    }
}