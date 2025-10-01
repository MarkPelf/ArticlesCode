using LiquidTechnologies.XmlObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
#pragma warning disable CS1591 // Undocumented items

namespace XsdExample_Ver4.XSD1
{
    #region Using Liquid XML Objects to read/write data
    /// <summary>
    /// The following code is intended to demonstrate the basics of using the 
    /// Liquid XML Objects code you have just generated.
    /// 
    /// The creation of the sample code is an option within the XML Data Binder
    /// Wizard and can be turned off.
    /// </summary>
    internal class SampleUsage
    {
        #region Reading
        /// <summary>
        /// Demonstrates how to read XML data into a generated object.
        /// In this case the first element in your schema was selected.
        /// </summary>
        /// <remarks>
        /// LxSerializer.Deserialize has a number of other overloads
        /// allowing data to be read from a file/Stream/TextReader/XmlReader 
        /// </remarks>
        /// <exception cref="LxSerializationException">LxSerializer.Deserialize will throw if the XML data contains errors</exception>
        public void SimpleXmlReader()
        {
            #region Read <ns:SmallCompany xmlns:ns='https://markpelf.com/SmallCompany.xsd'>
            {
                string sampleXml = @"<ns:SmallCompany xmlns:ns='https://markpelf.com/SmallCompany.xsd'>
                                        <!-- Place your own XML code here -->
                                   </ns:SmallCompany>";

                // A LxSerializer is required to de-serialize the XML data into a 
                // generated Liquid XML Objects class.
                LxSerializer<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm> serializer = new LxSerializer<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm>();

                using (TextReader textReader = new StringReader(sampleXml))
                {
                    // The settings in LxReaderSettings govern the behaviour of the De-serialization
                    // in this example we will provide our own error handler callback function.
                    LxReaderSettings readerSettings = new LxReaderSettings();
                    readerSettings.ErrorHandler = XmlReader_ErrorHandler;

                    // reads XML data from a TextReader, which it uses to 
                    // create and populate a XsdExample_Ver4.XSD1.Tns.SmallCompanyElm
                    XsdExample_Ver4.XSD1.Tns.SmallCompanyElm smallCompanyElm = serializer.Deserialize(textReader, readerSettings);

                    // TODO use the smallCompanyElm object ...
                }
            }
            #endregion


        }
        #endregion

        #region Writing
        /// <summary>
        /// Demonstrates how to serialize a generated Liquid XML object to XML.
        /// In this case the first element in your schema was selected.
        /// </summary>
        /// <remarks>
        /// LxSerializer.Serialize has a number of other overloads
        /// allowing data to be written to a file/Stream/TextWriter/XmlWriter
        /// </remarks>
        /// <exception cref="LxSerializationException">LxSerializer.Serialize will throw if the object being serialized contains errors</exception>
        public void SimpleXmlWriter()
        {
            #region Write <ns:SmallCompany xmlns:ns='https://markpelf.com/SmallCompany.xsd'>
            {
                // A LxSerializer is required to serialize the XML data into a 
                // generated Liquid XML Objects class.
                LxSerializer<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm> serializer = new LxSerializer<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm>();

                XsdExample_Ver4.XSD1.Tns.SmallCompanyElm smallCompanyElm = new XsdExample_Ver4.XSD1.Tns.SmallCompanyElm();
                // TODO populate the object
                // smallCompanyElm.xyz = value;

                using (StringWriter writer = new StringWriter())
                {
                    LxWriterSettings writerSettings = new LxWriterSettings();
                    writerSettings.ErrorHandler = XmlWriter_ErrorHandler;

                    serializer.Serialize(writer, smallCompanyElm, writerSettings);

                    Console.Write(writer.ToString());
                }
            }
            #endregion


        }
        #endregion

        #region Reading when the root element is not known at design time
        /// <summary>
        /// Typically when you read an XML document you know what the root element 
        /// should be, however in some instances the root element may not be known at 
        /// design time. This sample demonstrates how to deal with this.
        /// </summary>
        /// <exception cref="LxException">LxSerializer.Deserialize will throw if the generated 
        /// Liquid XML Objects library does not contain an object capable of having the XML data 
        /// de-serialized into it.</exception>
        public void ReadingXmlOfUnknownType()
        {
            string sampleXml = @"<ns:SmallCompany xmlns:ns='https://markpelf.com/SmallCompany.xsd'>
                                    <!-- Place your own XML code here -->
                               </ns:SmallCompany>";

            // Note we use the un-templated version
            LxSerializer serializer = new LxSerializer();

            using (TextReader textReader = new StringReader(sampleXml))
            {
                // reads XML data from a TextReader, which it uses to 
                // create and populate a XsdExample_Ver4.XSD1.Tns.SmallCompanyElm
                XmlQualifiedName rootElementName;
                Object rootObject = serializer.Deserialize(textReader, out rootElementName);

                if (rootObject is XsdExample_Ver4.XSD1.Tns.SmallCompanyElm)
                {
                    XsdExample_Ver4.XSD1.Tns.SmallCompanyElm smallCompanyElm = (XsdExample_Ver4.XSD1.Tns.SmallCompanyElm)rootObject;
                    //if (smallCompanyElm is SomeClassType myClass)
                    //{
                    //    myClass.MyProperty = 5;
                    //}
                    //else if (smallCompanyElm is SomeOtherClassType myOtherClass)
                    //{
                    //    myOtherClass.MyOtherProperty = "New value";
                    //}
                    //else
                    //{
                    //    throw new NotSupportedException();
                    //}
                }
                else
                {
                    throw new NotImplementedException($"No handler provided for the root element {rootElementName}");
                }
            }
        }
        #endregion

        #region XML Reader Error Handler
        /// <summary>
        /// This method is called when an error or warning is reported during
        /// the de-serialization process. 
        /// The method must throw an Exception in order to stop the de-serialization 
        /// process. If it returns, the error/warning is ignored.
        /// </summary>
        /// <param name="msg">The error message describing the issue</param>
        /// <param name="severity">The severity of the error</param>
        /// <param name="errorCode">The error code providing detail about the issue</param>
        /// <param name="location">The location of the issue in the source XML document</param>
        /// <param name="targetObject">The object the data is being de-serialized into (a Liquid XML Objects generated class)</param>
        private void XmlReader_ErrorHandler(string msg, LxErrorSeverity severity, LxErrorCode errorCode, TextLocation? location, object targetObject)
        {
            Console.WriteLine($"{severity}:{errorCode}:{location} {msg}");

            // throwing an exception will stop de-serialization
            if (severity == LxErrorSeverity.Error)
                throw new LxSerializationException(msg, severity, errorCode, location, targetObject);

            // returning will cause the warning/error to be ignored and serialization will continue
        }
        #endregion

        #region XML Writer Error Handler
        /// <summary>
        /// This method is called when an error or warning is reported during
        /// the serialization process. 
        /// The method must throw an Exception in order to stop the serialization 
        /// process. If it returns, the error/warning is ignored.
        /// </summary>
        /// <param name="msg">The error message describing the issue</param>
        /// <param name="severity">The severity of the error</param>
        /// <param name="errorCode">The error code providing detail about the issue</param>
        /// <param name="targetObject">The object the data is being de-serialized into (a Liquid XML Objects generated class)</param>
        private void XmlWriter_ErrorHandler(string msg, LxErrorSeverity severity, LxErrorCode errorCode, object? targetObject)
        {
            Console.WriteLine($"{severity}:{errorCode} {msg}");

            // throwing an exception will stop Serialization
            if (severity == LxErrorSeverity.Error)
                throw new LxSerializationException(msg, severity, errorCode, null, targetObject);

            // returning will cause the warning/error to be ignored and serialization will continue
        }
        #endregion


    
    }
    #endregion


}

#pragma warning restore CS1591