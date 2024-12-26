//ClassLibraryA, ClassA.cs =========================================
using ClassLibraryA.EF_Work;

namespace ClassLibraryA
{
    public static class ClassA
    {
        public static string? BasicCall()
        {
            string? result = "Hello from ClassLibraryA.ClassA";
            return result;
        }

        public static string? ExceptionTest() 
        {
            string? result = null;
            try
            {
                throw new Exception("Exception_from_ClassLibraryA.ClassA");
            }
            catch(Exception ex) 
            {
                result= ex.ToString();
            }
            return result;
        }

        public static string? BasicDbTest()
        {
            string? result = EF_Test1.BasicDbTest();
            return result;
        }
    }
}
