//EmployeesAdvancedFilterVM.cs
namespace Example09.Models.Home
{
    public class EmployeesAdvancedFilterVM
    {
        //model
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? City { get; set; } = null;
        public string? Country { get; set; } = null;
        public bool IsSubmit { get; set; } = false;
        public bool IsReset { get; set; } = false;

        //view model
    }
}
