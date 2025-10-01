using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using System.Reflection;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;

namespace XsdExample04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("XsdExample-Start------------");

            // Build configuration 
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var filePathAssembly = assembly.Location;
            var basePath = Path.GetDirectoryName(filePathAssembly);
            var applicationName = Path.GetFileNameWithoutExtension(assembly.Location);
            var applicationLogName = string.Format("{0}-.Log", applicationName);
            var logFullPath = Path.Combine(basePath ?? String.Empty, applicationLogName);
            Console.WriteLine($"logFullPath: {logFullPath}");

            Microsoft.Extensions.Logging.ILogger? _logger = CreateSerilog(logFullPath);

            try
            {
                var _configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath ?? String.Empty)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

                // Read settings 
                var xmlFile11 = _configuration?["AppSettings:xmlFile11"];
                var xsdFile1 = _configuration?["AppSettings:xsdFile1"];
                var xmlFile21 = _configuration?["AppSettings:xmlFile21"];
                var xsdFile2 = _configuration?["AppSettings:xsdFile2"];

                var xmlFile11_FullPath = Path.Combine(basePath ?? String.Empty, "XmlFiles", xmlFile11 ?? String.Empty);
                var xsdFile1_FullPath = Path.Combine(basePath ?? String.Empty, "XmlFiles", xsdFile1 ?? String.Empty);
                var xmlFile21_FullPath = Path.Combine(basePath ?? String.Empty, "XmlFiles", xmlFile21 ?? String.Empty);
                var xsdFile2_FullPath = Path.Combine(basePath ?? String.Empty, "XmlFiles", xsdFile2 ?? String.Empty);

                ProcessVer2_Process1(xmlFile11_FullPath, _logger);
                ProcessVer2_Process2(xmlFile21_FullPath, _logger);

                Console.WriteLine("XsdExample-End------------");
            }
            catch (Exception ex)
            {
                string methodName =
                    $"Type: {System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}, " +
                    $"Method: Main; ";
                _logger?.LogError(ex, methodName);
            }
        }

        private static Microsoft.Extensions.Logging.ILogger? CreateSerilog(string filePath)
        {
            Microsoft.Extensions.Logging.ILogger? _instanceSerilog = null;
            //here we directly use Serilog for logging to file
            Serilog.Core.Logger? _serilog =
                new LoggerConfiguration()
            .WriteTo.File(
            filePath,
            Serilog.Events.LogEventLevel.Information,
            //"{Timestamp:yyyy-MM-dd-HH:mm:ss.fff zzz} {CorrelationId} {Level:u3} " +
            "{Username} {Message:lj}{Exception}{NewLine}",
            null,
            1000000,
            null,
            false,
            false,
            null,
            RollingInterval.Day
            )
            .CreateLogger();

            if (_serilog != null)
            {
                _instanceSerilog = new SerilogLoggerFactory(_serilog)
                    .CreateLogger<Program>(); // creates an instance of ILogger<IMyService>
            }

            return _instanceSerilog;
        }        

        public static void ProcessVer2_Process1(
            string? filePath,
            Microsoft.Extensions.Logging.ILogger? logger)
        {
            try
            {
                logger?.LogInformation(
                    "+++ProcessVer2_Process1-Start++++++++++++++++++");
                logger?.LogInformation("filePath:" + filePath);

                XmlSerializer ser = new XmlSerializer(typeof(Example2SmallCompany.SmallCompany));
                TextReader textReader = File.OpenText(filePath ?? String.Empty);
                Example2SmallCompany.SmallCompany? xmlObject = 
                    ser.Deserialize(textReader) as Example2SmallCompany.SmallCompany;

                if (xmlObject != null)
                {
                    logger?.LogInformation("CompanyName:" + xmlObject.CompanyName);

                    foreach(Example2SmallCompany.SmallCompanyEmployee item in xmlObject.Employee)
                    {
                        logger?.LogInformation("------------" );
                        logger?.LogInformation("Name_String_NO:" + item.Name_String_NO);
                        logger?.LogInformation("City_String_O:" + (item.City_String_O ?? "null") );
                    }

                    foreach (Example2SmallCompany.SmallCompanyInfoData item in xmlObject.InfoData)
                    {
                        logger?.LogInformation("------------");
                        logger?.LogInformation("Id_Int_NO:" + item.Id_Int_NO.ToString());

                        logger?.LogInformation("Quantity_Int_OValue:" + item.Quantity_Int_OValue.ToString());
                        logger?.LogInformation("Quantity_Int_OValueSpecified:" + item.Quantity_Int_OValueSpecified.ToString());
                        logger?.LogInformation("Quantity_Int_O:" + (item.Quantity_Int_O?.ToString() ?? "null"));
                    }
                }
                else
                {
                    logger?.LogError("xmlObject == null");
                }

                logger?.LogInformation(
                    "+++ProcessVer2_Process1-End++++++++++++++++++");
            }
            catch (Exception ex)
            {
                string methodName =
                    $"Type: {System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}, " +
                    $"Method: ProcessVer2_Process1; ";
                logger?.LogError(ex, methodName);
            }
        }

        public static void ProcessVer2_Process2(
            string? filePath,
            Microsoft.Extensions.Logging.ILogger? logger)
        {
            try
            {
                logger?.LogInformation(
                    "+++ProcessVer2_Process2-Start++++++++++++++++++");
                logger?.LogInformation("filePath:" + filePath);

                XmlSerializer ser = new XmlSerializer(typeof(Example2BigCompany.BigCompany));
                TextReader textReader = File.OpenText(filePath ?? String.Empty);
                Example2BigCompany.BigCompany? xmlObject = 
                    ser.Deserialize(textReader) as Example2BigCompany.BigCompany;

                if (xmlObject != null)
                {
                    logger?.LogInformation("CompanyName:" + xmlObject.CompanyName);

                    foreach (Example2BigCompany.BigCompanyEmployee item in xmlObject.Employee)
                    {
                        logger?.LogInformation("------------");
                        logger?.LogInformation("Name_String_NO:" + item.Name_String_NO);
                        logger?.LogInformation("City_String_O:" + (item.City_String_O ?? "null"));
                    }

                    foreach (Example2BigCompany.BigCompanyInfoData item in xmlObject.InfoData)
                    {
                        logger?.LogInformation("------------");
                        logger?.LogInformation("Data1_Int_NO_R:" + item.Data1_Int_NO_R.ToString());
                        logger?.LogInformation("Data2_Int_NO_NR:" + (item.Data2_Int_NO_NR?.ToString() ?? "null"));

                        logger?.LogInformation("Data3_Int_O_RValue:" + item.Data3_Int_O_RValue.ToString());
                        logger?.LogInformation("Data3_Int_O_RValueSpecified:" + item.Data3_Int_O_RValueSpecified.ToString());
                        logger?.LogInformation("Data3_Int_O_R:" + (item.Data3_Int_O_R?.ToString() ?? "null"));

                        logger?.LogInformation("Data4_Int_O_NRValue:" + (item.Data4_Int_O_NRValue?.ToString() ?? "null"));
                        logger?.LogInformation("Data4_Int_O_NRValueSpecified:" + item.Data4_Int_O_NRValueSpecified.ToString());
                        logger?.LogInformation("Data4_Int_O_NR:" + (item.Data4_Int_O_NR?.ToString() ?? "null"));
                    }
                }
                else
                {
                    logger?.LogError("xmlObject == null");
                }

                logger?.LogInformation(
                    "+++ProcessVer2_Process2-End++++++++++++++++++");
            }
            catch (Exception ex)
            {
                string methodName =
                    $"Type: {System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}, " +
                    $"Method: ProcessVer2_Process2; ";
                logger?.LogError(ex, methodName);
            }
        }
    }
}
