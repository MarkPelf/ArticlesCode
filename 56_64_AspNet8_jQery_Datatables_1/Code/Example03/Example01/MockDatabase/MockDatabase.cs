using System.Text.Json;

namespace Example03.MockDatabase
{
    public class MockDatabase
    {
        private List<Employee>? _employeeTable;
        public List<Employee> EmployeesTable
        {
            get
            {
                //going for singleton pattern
                if( _employeeTable == null )
                {
                    Employee[]? employeeArray = JsonSerializer.Deserialize<Employee[]>(MockEmployeeData.Data);
                    if( employeeArray != null )
                    {
                        _employeeTable = new List<Employee>();
                        _employeeTable.AddRange( employeeArray );
                    }
                }

                if (_employeeTable == null) { _employeeTable = new List<Employee>(); };
                return _employeeTable;
            }
        }

        private static MockDatabase? _instance;
        public static MockDatabase Instance { 
            get 
            {
                //going for singleton pattern
                if (_instance==null)
                {
                    _instance = new MockDatabase();
                }
                return _instance; 
            } 
        }
    }
}
