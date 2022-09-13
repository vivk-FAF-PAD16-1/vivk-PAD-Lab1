using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Gateway
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Run(Handler);
        }
        
        private async Task Handler(HttpContext context)
        {
            await context.Response.WriteAsync("Hello!");
        }
    }
}