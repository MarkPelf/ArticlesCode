using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using System.Reflection;
using System.Xml.Schema;
using System.Xml;

namespace XsdExample03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("XsdExample01-Start------------");

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

                Processing1(_logger,
                    xmlFile11_FullPath, xsdFile1_FullPath,
                    xmlFile21_FullPath, xsdFile2_FullPath);

                Console.WriteLine("XsdExample01-End------------");
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

        private static void Processing1(
            Microsoft.Extensions.Logging.ILogger? _logger,
            string xmlFile11_FullPath,
            string xsdFile1_FullPath,
            string xmlFile21_FullPath,
            string xsdFile2_FullPath)
        {
            try
            {
                _logger?.LogInformation("XsdExample01-Start------------");

                _logger?.LogInformation($"xmlFile11_FullPath: {xmlFile11_FullPath}");
                _logger?.LogInformation($"xsdFile1_FullPath: {xsdFile1_FullPath}");
                _logger?.LogInformation($"xmlFile21_FullPath: {xmlFile21_FullPath}");
                _logger?.LogInformation($"xsdFile2_FullPath: {xsdFile2_FullPath}");

                ValidateXmlForXsd(_logger, xmlFile11_FullPath, xsdFile1_FullPath);
                ValidateXmlForXsd(_logger, xmlFile21_FullPath, xsdFile2_FullPath);

                _logger?.LogInformation("XsdExample01-End------------");
            }
            catch (Exception ex)
            {
                string methodName =
                    $"Type: {System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}, " +
                    $"Method: Processing1; ";
                _logger?.LogError(ex, methodName);
            }
        }

        public static bool ValidateXmlForXsd(
            Microsoft.Extensions.Logging.ILogger? _logger, 
            string xmlPath, string xsdPath)
        {
            bool isValid=false;

            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);

            xml.Schemas.Add(null, xsdPath);

            try
            {
                xml.Validate(null);
                isValid = true;
            }
            catch (XmlSchemaValidationException ex)
            {
                string text1 = $"Validation FAILED for: xmlPath={xmlPath}" +
                    $" and xsdPath={xsdPath} .Validation Error: " +
                    ex.Message;
                _logger?.LogInformation(text1);
                isValid = false;
            }

            if (isValid)
            {
                string text1 = $"Validation SUCCESS for: xmlPath={xmlPath}" +
                    $" and  xsdPath={xsdPath}";
                _logger?.LogInformation(text1);
            }
            return isValid;
        }
    }
}
