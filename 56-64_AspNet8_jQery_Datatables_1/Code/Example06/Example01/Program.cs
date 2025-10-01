using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

//Program.cs
namespace Example06
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DataTables.AspNet registration 
            DataTablesAspNetRegistration(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        //DataTables.AspNet registration
        public static void DataTablesAspNetRegistration(WebApplicationBuilder? builder)
        {
            if (builder == null) { throw new Exception("builder == null"); };

            var options = new DataTables.AspNet.AspNetCore.Options()
               .EnableRequestAdditionalParameters()
               .EnableResponseAdditionalParameters();

            // DataTables.AspNet registration 
            builder.Services.RegisterDataTables(options);
        }
    }
}
