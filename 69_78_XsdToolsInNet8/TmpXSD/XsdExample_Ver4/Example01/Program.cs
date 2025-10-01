using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using System.Reflection;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using LiquidTechnologies.XmlObjects;

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

                ProcessVer4_Process1(xmlFile11_FullPath, _logger);
                ProcessVer4_Process2(xmlFile21_FullPath, _logger);

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

        public static void ProcessVer4_Process1(
            string? filePath,
            Microsoft.Extensions.Logging.ILogger? logger)
        {
            try
            {
                logger?.LogInformation(
                    "+++ProcessVer4_Process1-Start++++++++++++++++++");
                logger?.LogInformation("filePath:" + filePath);

                LxSerializer<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm> serializer = 
                    new LxSerializer<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm>();
                TextReader textReader = File.OpenText(filePath ?? String.Empty);
                LxReaderSettings readerSettings = new LxReaderSettings();

                XsdExample_Ver4.XSD1.Tns.SmallCompanyElm? xmlObject =
                    serializer.Deserialize(textReader, readerSettings);

                if (xmlObject != null)
                {
                    logger?.LogInformation("CompanyName:" + xmlObject.CompanyName);

                    foreach(XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.EmployeeElm item in xmlObject.Employees)
                    {
                        logger?.LogInformation("------------" );
                        logger?.LogInformation("Name_String_NO:" + item.Name_String_NO);
                        logger?.LogInformation("City_String_O:" + (item.City_String_O ?? "null") );
                    }

                    foreach (XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.InfoDataElm item in xmlObject.InfoData)
                    {
                        logger?.LogInformation("------------");
                        logger?.LogInformation("Id_Int_NO:" + item.Id_Int_NO.ToString());
                        
                        logger?.LogInformation("Quantity_Int_O:" + (item.Quantity_Int_O?.ToString() ?? "null"));
                    }
                }
                else
                {
                    logger?.LogError("xmlObject == null");
                }

                logger?.LogInformation(
                    "+++ProcessVer4_Process1-End++++++++++++++++++");
            }
            catch (Exception ex)
            {
                string methodName =
                    $"Type: {System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}, " +
                    $"Method: ProcessVer4_Process1; ";
                logger?.LogError(ex, methodName);
            }
        }

        public static void ProcessVer4_Process2(
            string? filePath,
            Microsoft.Extensions.Logging.ILogger? logger)
        {
            try
            {
                logger?.LogInformation(
                    "+++ProcessVer4_Process2-Start++++++++++++++++++");
                logger?.LogInformation("filePath:" + filePath);

                LxSerializer<XsdExample_Ver4.XSD2.Tns.BigCompanyElm> serializer =
                    new LxSerializer<XsdExample_Ver4.XSD2.Tns.BigCompanyElm>();
                TextReader textReader = File.OpenText(filePath ?? String.Empty);
                LxReaderSettings readerSettings = new LxReaderSettings();

                XsdExample_Ver4.XSD2.Tns.BigCompanyElm? xmlObject =
                    serializer.Deserialize(textReader, readerSettings);

                if (xmlObject != null)
                {
                    logger?.LogInformation("CompanyName:" + xmlObject.CompanyName);

                    foreach (XsdExample_Ver4.XSD2.Tns.BigCompanyElm.EmployeeElm item in xmlObject.Employees)
                    {
                        logger?.LogInformation("------------");
                        logger?.LogInformation("Name_String_NO:" + item.Name_String_NO);
                        logger?.LogInformation("City_String_O:" + (item.City_String_O ?? "null"));
                    }

                    foreach (XsdExample_Ver4.XSD2.Tns.BigCompanyElm.InfoDataElm item in xmlObject.InfoData)
                    {
                        logger?.LogInformation("------------");
                        logger?.LogInformation("Data1_Int_NO_R:" + item.Data1_Int_NO_R.ToString());
                        logger?.LogInformation("Data2_Int_NO_NR:" + 
                            (item.Data2_Int_NO_NR.IsNil ? "nill" : item.Data2_Int_NO_NR.Value));

                        logger?.LogInformation("Data3_Int_O_R:" + (item.Data3_Int_O_R?.ToString() ?? "null"));

                        logger?.LogInformation("Data4_Int_O_NR:" + 
                            (item.Data4_Int_O_NR==null ? "null" :
                             (item.Data4_Int_O_NR.IsNil ? "nil" : item.Data4_Int_O_NR.Value)));
                    }
                }
                else
                {
                    logger?.LogError("xmlObject == null");
                }

                logger?.LogInformation(
                    "+++ProcessVer4_Process2-End++++++++++++++++++");
            }
            catch (Exception ex)
            {
                string methodName =
                    $"Type: {System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}, " +
                    $"Method: ProcessVer4_Process2; ";
                logger?.LogError(ex, methodName);
            }
        }
    }
}
