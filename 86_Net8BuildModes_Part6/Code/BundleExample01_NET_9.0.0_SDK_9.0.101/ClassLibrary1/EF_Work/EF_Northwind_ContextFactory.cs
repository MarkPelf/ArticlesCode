using ClassLibraryA.EF_Northwind;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryA.EF_Work
{
    internal class EF_Northwind_ContextFactory : IDesignTimeDbContextFactory<NorthwindContext>
    {
        static EF_Northwind_ContextFactory()
        {          
            connectionString = 
                "Data Source=.;User Id=sa;Password=dbadmin1!;Initial Catalog=Northwind;Encrypt=False";
            //Console.WriteLine("ConnectionString:" + connectionString);
        }

        static string? connectionString = null;

        public NorthwindContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NorthwindContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new NorthwindContext(optionsBuilder.Options);
        }
    }
}
