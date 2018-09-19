using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Walldisplay;

namespace Walldisplay
{
    class WebStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {

            var httpConfiguration = new HttpConfiguration();

            httpConfiguration.Routes.MapHttpRoute(
                name: "user statistics", 
                routeTemplate: "userstatistics", 
                defaults: new { controller = "Web", groupId = RouteParameter.Optional });

            httpConfiguration.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;
            httpConfiguration.Formatters.Remove(httpConfiguration.Formatters.XmlFormatter);

            appBuilder.UseWebApi(httpConfiguration);

            //            appBuilder.UseDefaultFiles("/index.html");

            var fileSystem = new EmbeddedResourceFileSystem(typeof(Program).Assembly, "Walldisplay.web");

            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = fileSystem
            };
            options.StaticFileOptions.FileSystem = fileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[] { "main.html" };
            appBuilder.UseFileServer(options);
        }
    }
}
