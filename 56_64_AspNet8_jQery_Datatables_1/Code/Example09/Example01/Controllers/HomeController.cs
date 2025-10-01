using Example09.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Example09.MockDatabase;
using System.Numerics;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Linq.Dynamic.Core;
using Example09.Models.Home;
using Microsoft.VisualBasic;
using System.Text.Json;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

//HomeController.cs ======================================
namespace Example09.Controllers
{
    public class HomeController : Controller
    {
        public const string EMPLOYEES_ADVANCED_FILTER_STATE = "EMPLOYEES_ADVANCED_FILTER_STATE";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Employees(EmployeesVM model)
        {
            try
            {
                ISession? CurrentSession = this.HttpContext?.Session;

                {
                    //we get advanced-filter state from session if there is one
                    string? jsonUserEmployeesAdvancedFilterState = CurrentSession?.GetString(EMPLOYEES_ADVANCED_FILTER_STATE);

                    if (!string.IsNullOrEmpty(jsonUserEmployeesAdvancedFilterState))
                    {
                        EmployeesAdvancedFilterVM? AdvancedFilterState =
                            JsonSerializer.Deserialize<EmployeesAdvancedFilterVM>(jsonUserEmployeesAdvancedFilterState);

                        if (AdvancedFilterState != null)
                        {
                            string filterState =
                                "Given Name: " + AdvancedFilterState.FirstName +
                                "; Family Name: " + AdvancedFilterState.LastName +
                                "; Town: " + AdvancedFilterState.City+
                                "; Country: " + AdvancedFilterState.Country ;
                            model.AdvancedFilterState = filterState;    
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return View(model);
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

                //here we get the count that needs to be presented by the UI
                int totalRecordsCount = employees.Count();

                employees = FilterRowsPerSavedAdvancedFilterState(employees);

                var iQueryableOfAnonymous = employees.Select(p => new
                {
                    id = p.Id,
                    givenName = p.FirstName,
                    familyName = p.LastName,
                    town = p.City,
                    country = p.Country,
                    email = p.Email,
                    phoneNo = p.Phone,
                });

                //here we get the count that needs to be presented by the UI
                int filteredRecordsCount = iQueryableOfAnonymous.Count();

                iQueryableOfAnonymous = SortRowsPerRequestParamters(iQueryableOfAnonymous, request);

                iQueryableOfAnonymous = iQueryableOfAnonymous.Skip(request.Start).Take(request.Length);

                //here we materialize the query
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

        private IQueryable<Example09.MockDatabase.Employee> FilterRowsPerSavedAdvancedFilterState(
           IQueryable<Example09.MockDatabase.Employee> iQueryableOfEmployee)
        {
            try
            {
                ISession? CurrentSession = this.HttpContext?.Session;

                {
                    //we get advanced-filter state from session if there is one
                    string? jsonUserEmployeesAdvancedFilterState = CurrentSession?.GetString(EMPLOYEES_ADVANCED_FILTER_STATE);

                    if (!string.IsNullOrEmpty(jsonUserEmployeesAdvancedFilterState))
                    {
                        EmployeesAdvancedFilterVM? advancedFilter = 
                            JsonSerializer.Deserialize<EmployeesAdvancedFilterVM>(jsonUserEmployeesAdvancedFilterState);

                        if (advancedFilter != null)
                        {
                            //FirstName
                            advancedFilter.FirstName = advancedFilter.FirstName?.Trim();
                            if (!string.IsNullOrEmpty(advancedFilter.FirstName))
                            {
                                /*
                                 * In EF we would go for DbFunctions.Like
                                if (advancedFilter.FirstName.Contains('*') || advancedFilter.FirstName.Contains('?'))
                                {
                                    string pattern = advancedFilter.FirstName.Replace('*', '%').Replace('?', '_');
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(vk => DbFunctions.Like(vk.FirstName, pattern));
                                }
                                else
                                {
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.FirstName != null && vk.FirstName.Equals(advancedFilter.FirstName));
                                }
                                */

                                //in pure Linq going for Regex
                                if (advancedFilter.FirstName.Contains('*') || advancedFilter.FirstName.Contains('?'))
                                {
                                    string pattern = advancedFilter.FirstName.Replace("*", ".*").Replace("?", ".{1}");
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.FirstName != null && Regex.IsMatch(vk.FirstName, pattern));
                                }
                                else
                                {
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.FirstName != null && vk.FirstName.Equals(advancedFilter.FirstName));
                                }
                            }

                            //LastName
                            advancedFilter.LastName = advancedFilter.LastName?.Trim();
                            if (!string.IsNullOrEmpty(advancedFilter.LastName))
                            {
                                //in pure Linq going for Regex
                                if (advancedFilter.LastName.Contains('*') || advancedFilter.LastName.Contains('?'))
                                {
                                    string pattern = advancedFilter.LastName.Replace("*", ".*").Replace("?", ".{1}");
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.LastName != null && Regex.IsMatch(vk.LastName, pattern));
                                }
                                else
                                {
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.LastName != null && vk.LastName.Equals(advancedFilter.LastName));
                                }
                            }

                            //City
                            advancedFilter.City = advancedFilter.City?.Trim();
                            if (!string.IsNullOrEmpty(advancedFilter.City))
                            {
                                //in pure Linq going for Regex
                                if (advancedFilter.City.Contains('*') || advancedFilter.City.Contains('?'))
                                {
                                    string pattern = advancedFilter.City.Replace("*", ".*").Replace("?", ".{1}");
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.City != null && Regex.IsMatch(vk.City, pattern));
                                }
                                else
                                {
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.City != null && vk.City.Equals(advancedFilter.City));
                                }
                            }

                            //Country
                            advancedFilter.Country = advancedFilter.Country?.Trim();
                            if (!string.IsNullOrEmpty(advancedFilter.Country))
                            {
                                //in pure Linq going for Regex
                                if (advancedFilter.Country.Contains('*') || advancedFilter.Country.Contains('?'))
                                {
                                    string pattern = advancedFilter.Country.Replace("*", ".*").Replace("?", ".{1}");
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.Country != null && Regex.IsMatch(vk.Country, pattern));
                                }
                                else
                                {
                                    iQueryableOfEmployee = iQueryableOfEmployee.Where(
                                        vk => vk.Country != null && vk.Country.Equals(advancedFilter.Country));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return iQueryableOfEmployee;
        }

        public IActionResult EmployeesAdvancedFilter(EmployeesAdvancedFilterVM model)
        {           
            try
            {
                ISession ? CurrentSession=this.HttpContext?.Session;

                if (model.IsSubmit)
                {
                    //we have submit
                    //we save the advanced-filter state to session
                    if (CurrentSession != null && model != null)
                    {
                        model.FirstName=model.FirstName?.Trim();
                        model.LastName = model.LastName?.Trim();
                        model.City = model.City?.Trim();
                        model.Country = model.Country?.Trim();
                        string jsonUserEmployeesAdvancedFilterState = JsonSerializer.Serialize(model);
                        CurrentSession.SetString(EMPLOYEES_ADVANCED_FILTER_STATE, jsonUserEmployeesAdvancedFilterState);
                    }

                    return RedirectToAction("Employees", "Home");
                }
                else if (model.IsReset)
                {
                    //we have reset
                    //we clear advanced-filter state in session
                    CurrentSession?.Remove(EMPLOYEES_ADVANCED_FILTER_STATE);
                }

                //go for presentation
                {
                    //we get advanced-filter state from session if there is one
                    string? jsonUserEmployeesAdvancedFilterState = CurrentSession?.GetString(EMPLOYEES_ADVANCED_FILTER_STATE);

                    if (!string.IsNullOrEmpty(jsonUserEmployeesAdvancedFilterState)) 
                    {
                        model = JsonSerializer.Deserialize<EmployeesAdvancedFilterVM>(jsonUserEmployeesAdvancedFilterState)
                            ?? new EmployeesAdvancedFilterVM();
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
