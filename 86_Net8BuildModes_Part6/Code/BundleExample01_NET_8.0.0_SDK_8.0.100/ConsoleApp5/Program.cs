namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World from ConsoleApp2\n");

            Console.WriteLine(TestAssemblyLocationAPI());

            Console.WriteLine(ClassLibraryA.ClassA.BasicCall());
            Console.WriteLine(ClassLibraryA.ClassA.ExceptionTest());

            //deliberately commented to test Trim mode
            //Console.WriteLine(ClassLibraryA.ClassA.BasicDbTest());

            Console.ReadLine();
        }

        static string? TestAssemblyLocationAPI()
        {
            string? result = String.Empty;

            string ? locationAssembly =
                    System.Reflection.Assembly.GetExecutingAssembly().Location;
            //result += "\n";
            result +=$"locationAssembly:{locationAssembly}";


            string? BaseDirectory = System.AppContext.BaseDirectory;
            result += "\n";
            result += $"BaseDirectory:{BaseDirectory}";

            string? ProcessPath = System.Environment.ProcessPath;
            result += "\n";
            result += $"ProcessPath:{ProcessPath}";
            result += "\n";

            return result;
        }
    }


}
