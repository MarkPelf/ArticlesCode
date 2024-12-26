using ClassLibraryA.EF_Northwind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryA.EF_Work
{
    internal class EF_Test1
    {
        public static string? BasicDbTest()
        {
            string? result = String.Empty;

            using NorthwindContext ctx =
                new EF_Northwind_ContextFactory().CreateDbContext(new string[0]);

            result+= "\nTable Customers ==================================";
            var tableCustomers = ctx.Customers.Where(p => p.Country == "Germany");
            foreach (var customer in tableCustomers)
            {
                result += "\n";
                result += "Customer Name: " + customer.ContactName;
            }

            return result;
        }


    }
}
