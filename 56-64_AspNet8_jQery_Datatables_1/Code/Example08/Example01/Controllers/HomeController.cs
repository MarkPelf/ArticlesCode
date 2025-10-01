using Example08.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Example08.MockDatabase;
using System.Numerics;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Linq.Dynamic.Core;
using Example08.Models.Home;

//HomeController.cs
namespace Example08.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Employees()
        {
            return View();
        }

        //this is target of AJAX call and provides data for
        //the table, based on selected input parameters
        public IActionResult EmployeesDT(DataTables.AspNet.Core.IDataTablesRequest request)
        {
            // There is dependency in this method on names of fields
            // and implied mapping. I see it almost impossible to avoid.
            // At least, in this method, we avoided dependency on the order
            // of table fields, in case order needs to be changed
            //Here are our mapped table columns:
            //Column0 selectCheckbox
            //Column0 id -> Employee.Id
            //Column1 givenName -> Employee.FirstName
            //Column2 familyName -> Employee.LastName
            //Column3 town -> Employee.City
            //Column4 country -> Employee.Country
            //Column5 email -> Employee.Email
            //Column6 phoneNo -> Employee.Phone

            try
            {
                IQueryable<Employee> employees = MockDatabase.MockDatabase.Instance.EmployeesTable.AsQueryable();

                int totalRecordsCount = employees.Count();

                var iQueryableOfAnonymous = employees.Select(p => new
                {
                    selectCheckbox = System.String.Empty,
                    id = p.Id,
                    givenName = p.FirstName,
                    familyName = p.LastName,
                    town = p.City,
                    country = p.Country,
                    email = p.Email,
                    phoneNo = p.Phone,
                });

                iQueryableOfAnonymous = FilterRowsPerRequestParameters(iQueryableOfAnonymous, request);

                int filteredRecordsCount = iQueryableOfAnonymous.Count();

                iQueryableOfAnonymous = SortRowsPerRequestParamters(iQueryableOfAnonymous, request);

                iQueryableOfAnonymous = iQueryableOfAnonymous.Skip(request.Start).Take(request.Length);

                var dataPage = iQueryableOfAnonymous.ToList();

                var response = DataTablesResponse.Create(request, totalRecordsCount, filteredRecordsCount, dataPage);

                return new DataTablesJsonResult(response, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                var response = DataTablesResponse.Create(request, "Error processing AJAX call on server side");
                return new DataTablesJsonResult(response, false);
            }
        }

        private IQueryable<T> SortRowsPerRequestParamters<T>(
            IQueryable<T> iQueryableOfAnonymous, DataTables.AspNet.Core.IDataTablesRequest request)
        {
            /*
            * So, in "IQueryable<T> iQueryableOfAnonymous" I have the source data that I need to 
            * sort. In "DataTables.AspNet.Core.IDataTablesRequest request" I have full specification 
            * of sorting required, including the specification of columns
            * that need to be sorted by marked with "IsSortable" and "Sort.Order". 
            * I use Reflection to check if names of entity T properties match column names in
            * DataTables.AspNet.Core.IDataTablesRequest request and then create sort Linq query using
            * System.Linq.Dynamic.Core library, and return Linq query as a result.
            * It might look complicated, but advantage is that this method is generic and can 
            * be applied many times on different entities T. 
            */

            if (request != null && request.Columns != null && request.Columns.Any())
            {
                //this will work if type T contains properties that have names column.Name
                var sortingColumns = request.Columns.Where(p => p.IsSortable && p.Sort != null).OrderBy(p => p.Sort.Order).ToList();

                Type objType = typeof(T);
                var ListOfAllPropertiesInT = objType.GetProperties().Select(p => p.Name).ToList();

                if (sortingColumns != null && sortingColumns.Count > 0)
                {
                    //we plan to build dynamic Linq expression in this string
                    string dynamicLinqOrder = string.Empty;
                    bool isFirstString = true;

                    for (int i = 0; i < sortingColumns.Count; i++)
                    {
                        var column = sortingColumns[i];

                        //check if that property exists in T, 
                        //otherwise we will create syntax error in dynamic linq
                        if (ListOfAllPropertiesInT.Contains(column.Name))
                        {
                            if (isFirstString)
                            {
                                isFirstString = false;
                            }
                            else
                            {
                                dynamicLinqOrder += ", ";
                            }

                            dynamicLinqOrder += column.Name;
                            if (column.Sort.Direction == SortDirection.Descending)
                            {
                                dynamicLinqOrder += " desc";
                            }
                        }
                    }

                    if (dynamicLinqOrder.Length > 0)
                    {
                        //using System.Linq.Dynamic.Core
                        iQueryableOfAnonymous = iQueryableOfAnonymous.OrderBy(dynamicLinqOrder);
                    }
                };
            }

            return iQueryableOfAnonymous;
        }

        private IQueryable<T> FilterRowsPerRequestParameters<T>(
            IQueryable<T> iQueryableOfAnonymous, DataTables.AspNet.Core.IDataTablesRequest request)
        {
            /*
             * So, in "IQueryable<T> iQueryableOfAnonymous" I have the source data that I need to 
             * filter. In "DataTables.AspNet.Core.IDataTablesRequest request" I have full specification 
             * of request, including the search value "request.Search.Value" and specification of columns
             * that need to be searched for marked by "IsSearchable". 
             * I use Reflection to check if names of entity T properties match column names in
             * DataTables.AspNet.Core.IDataTablesRequest request and then create filter Linq query using
             * System.Linq.Dynamic.Core library, and return Linq query as a result.
             * It might look complicated, but advantage is that this method is generic and can 
             * be applied many times on different entities T. 
             */
            //this will work if type T contains properties that have names column.Name
            if (request != null && request.Search != null && !System.String.IsNullOrEmpty(request.Search.Value))
            {
                string pattern = request.Search.Value?.Trim() ?? System.String.Empty;

                var searchingColumns = request.Columns.Where(p => p.IsSearchable).ToList();
                var config = new ParsingConfig { ResolveTypesBySimpleName = true };

                Type objType = typeof(T);
                var ListOfAllPropertiesInT = objType.GetProperties().Select(p => p.Name).ToList();

                if (searchingColumns.Count > 0)
                {
                    //we plan to build dynamic Linq expression in this string
                    string dynamicLinqSearch = string.Empty;
                    bool isFirstString = true;

                    for (int i = 0; i < searchingColumns.Count; i++)
                    {
                        var column = searchingColumns[i];

                        //check if that property exists in T, 
                        //otherwise we will create syntax error in dynamic linq
                        if (ListOfAllPropertiesInT.Contains(column.Name))
                        {
                            if (isFirstString)
                            {
                                isFirstString = false;
                            }
                            else
                            {
                                dynamicLinqSearch += " or ";
                            }

                            dynamicLinqSearch += $"""{column.Name}.Contains("{pattern}")""";
                        }
                    }

                    if (dynamicLinqSearch.Length > 0)
                    {
                        //using System.Linq.Dynamic.Core
                        iQueryableOfAnonymous = iQueryableOfAnonymous.Where(config, dynamicLinqSearch);
                    }
                }
            }

            return iQueryableOfAnonymous;
        }

        public IActionResult SelectedEmployees(SelectedEmployeesVM model)
        {
            model.SelectedIds = model.SelectedIds?.Trim();
            if (model.SelectedIds != null)
            {
                string[] strings = model.SelectedIds.Split(',');
                model.SelectedIdsList = new List<string>(strings);
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
