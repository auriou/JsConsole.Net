using System;
using System.IO;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace JsConsole.Services
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "web";
            app.Use((ctx, next) =>
                    {
                        var output = ctx.Get<TextWriter>("host.TraceOutput");
                        output.WriteLine("{0} {1}: {2}", ctx.Request.Scheme, ctx.Request.Method, ctx.Request.Path);
                        return next();
                    });
            //pour construction du hub avec parametre
            //GlobalHost.DependencyResolver.Register(typeof(VoteHub), () => new VoteHub(new QuestionRepository()));
            
            app.UseCors(CorsOptions.AllowAll);
     

            app.Map("/signalr", map =>
                map.UseCors(CorsOptions.AllowAll)
                    .RunSignalR(new HubConfiguration
                                {
                                    EnableDetailedErrors = true,
                                    EnableJSONP = true
                                }));
            app.UseFileServer(new FileServerOptions()
                              {
                                  EnableDirectoryBrowsing = true,
                                  FileSystem = new PhysicalFileSystem(path)
                              });
            //app.UseNancy(options =>
            //    options.PerformPassThrough = context =>
            //        context.Response.StatusCode == HttpStatusCode.NotFound);

            //app.UseDirectoryBrowser(new DirectoryBrowserOptions() { FileSystem = new PhysicalFileSystem(_path) });
            //app.UseStaticFiles(new StaticFileOptions() { ContentTypeProvider = new CustomContentTypeProvider() });

            //app.Run(context =>
            //        {
            //            context.Response.ContentType = "text/plain";
            //            return context.Response.WriteAsync("Hello World!");
            //        });
        }
    }
}