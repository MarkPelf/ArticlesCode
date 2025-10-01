using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

//Program.cs
namespace Example05
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

            var binder = new DataTables.AspNet.AspNetCore.ModelBinder();
            binder.ParseAdditionalParameters = Parser;

            // DataTables.AspNet registration 
            builder.Services.RegisterDataTables(options, binder);

            //inner method
            IDictionary<string, object> Parser(ModelBindingContext modelBindingContext)
            {
                //for string
                string? countryFixedValue = modelBindingContext.ValueProvider.GetValue("countryFixed").FirstValue;
                //for int
                int? testNumberValue = null;
                {
                    string? tmp1 = modelBindingContext.ValueProvider.GetValue("testNumber").FirstValue;
                    if(tmp1 != null)
                    {
                        testNumberValue = System.Convert.ToInt32(tmp1);
                    }
                }

                var myDic = new Dictionary<string, object>();

                if(countryFixedValue!=null)
                {
                    myDic.Add("countryFixed", countryFixedValue);
                }

                if (testNumberValue != null)
                {
                    myDic.Add("testNumber", testNumberValue);
                }

                return myDic;
            }
        }
    }
}
