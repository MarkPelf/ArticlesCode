///////////////////////////////////////////////////////////////////////////
//           Liquid XML Objects GENERATED CODE - DO NOT MODIFY           //
//            https://www.liquid-technologies.com/xml-objects            //
//=======================================================================//
// Dependencies                                                          //
//     Nuget : LiquidTechnologies.XmlObjects.Runtime                     //
//           : MUST BE VERSION 20.7.14                                   //
//=======================================================================//
// Online Help                                                           //
//     https://www.liquid-technologies.com/xml-objects-quick-start-guide //
//=======================================================================//
// Licensing Information                                                 //
//     https://www.liquid-technologies.com/eula                          //
///////////////////////////////////////////////////////////////////////////
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Numerics;
using LiquidTechnologies.XmlObjects;
using LiquidTechnologies.XmlObjects.Attribution;

// ------------------------------------------------------
// |                      Settings                      |
// ------------------------------------------------------
// GenerateCommonBaseClass                  = False
// GenerateUnprocessedNodeHandlers          = False
// RaiseChangeEvents                        = False
// CollectionNaming                         = Pluralize
// Language                                 = CS
// OutputNamespace                          = XsdExample_Ver4.XSD1
// WriteDefaultValuesForOptionalAttributes  = True
// WriteDefaultValuesForOptionalElements    = True
// MixedContentHandling                     = TreatAsAny
// GenerationModel                          = Simple
//                                            *WARNING* this simplified model that is very easy to work with
//                                            but may cause the XML to be produced without regard for element
//                                            cardinality or order. Where very high compliance with the XML Schema
//                                            standard is required use GenerationModelType.Conformant
// XSD Schema Files
//    C:\TmpXSD\XsdExample_Ver4\Example01\XSD1\SmallCompany.xsd


namespace XsdExample_Ver4.XSD1
{
    #region Global Settings
    /// <summary>Contains library level properties, and ensures the version of the runtime used matches the version used to generate it.</summary>
    [LxRuntimeRequirements("20.7.14.13112", "Free Community Edition", "M9WDQLWDQNEEVNLX", LiquidTechnologies.XmlObjects.LicenseTermsType.CommunityEdition)]
    public partial class LxRuntimeRequirementsWritten
    {
    }

    #endregion

}

namespace XsdExample_Ver4.XSD1.Xs
{
    #region Complex Types
    /// <summary>A class representing the root XSD complexType anyType@http://www.w3.org/2001/XMLSchema</summary>
    [LxSimpleComplexTypeDefinition("anyType", "http://www.w3.org/2001/XMLSchema")]
    public partial class AnyTypeCt : XElement
    {
        /// <summary>Constructor : create a <see cref="AnyTypeCt" /> element &lt;anyType xmlns='http://www.w3.org/2001/XMLSchema'&gt;</summary>
        public AnyTypeCt()  : base(XName.Get("anyType", "http://www.w3.org/2001/XMLSchema")) { }

    }

    #endregion

}

namespace XsdExample_Ver4.XSD1.Tns
{
    #region Elements
    /// <summary>A class representing the root XSD element SmallCompany@https://markpelf.com/SmallCompany.xsd</summary>
    [LxSimpleElementDefinition("SmallCompany", "https://markpelf.com/SmallCompany.xsd", ElementScopeType.GlobalElement)]
    public partial class SmallCompanyElm
    {
        /// <summary>A <see cref="System.String" />, Required : should not be set to null</summary>
        [LxElementValue(0, "CompanyName", "https://markpelf.com/SmallCompany.xsd", LxValueType.Value, XsdType.XsdString, MinOccurs = 1, MaxOccurs = 1)]
        public System.String CompanyName { get; set; } = "";

        /// <summary>A collection of <see cref="XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.EmployeeElm" /></summary>
        [LxElementRef(1, MinOccurs = 0, MaxOccurs = LxConstants.Unbounded)]
        public List<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.EmployeeElm> Employees { get; } = new List<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.EmployeeElm>();

        /// <summary>A collection of <see cref="XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.InfoDataElm" /></summary>
        [LxElementRef(2, MinOccurs = 0, MaxOccurs = LxConstants.Unbounded)]
        public List<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.InfoDataElm> InfoData { get; } = new List<XsdExample_Ver4.XSD1.Tns.SmallCompanyElm.InfoDataElm>();

        /// <summary>Represent the inline xs:element Employee@https://markpelf.com/SmallCompany.xsd.</summary>
        [LxSimpleElementDefinition("Employee", "https://markpelf.com/SmallCompany.xsd", ElementScopeType.InlineElement)]
        public partial class EmployeeElm
        {
            /// <summary>A <see cref="System.String" />, Required : should not be set to null</summary>
            [LxElementValue(0, "Name_String_NO", "https://markpelf.com/SmallCompany.xsd", LxValueType.Value, XsdType.XsdString, MinOccurs = 1, MaxOccurs = 1)]
            public System.String Name_String_NO { get; set; } = "";

            /// <summary>A <see cref="System.String" />, Optional : null when not set</summary>
            [LxElementValue(1, "City_String_O", "https://markpelf.com/SmallCompany.xsd", LxValueType.Value, XsdType.XsdString, MinOccurs = 0, MaxOccurs = 1)]
            public System.String? City_String_O { get; set; }

        }

        /// <summary>Represent the inline xs:element InfoData@https://markpelf.com/SmallCompany.xsd.</summary>
        [LxSimpleElementDefinition("InfoData", "https://markpelf.com/SmallCompany.xsd", ElementScopeType.InlineElement)]
        public partial class InfoDataElm
        {
            /// <summary>A <see cref="System.Int32" />, Required</summary>
            [LxElementValue(0, "Id_Int_NO", "https://markpelf.com/SmallCompany.xsd", LxValueType.Value, XsdType.XsdInt, MinOccurs = 1, MaxOccurs = 1)]
            public System.Int32 Id_Int_NO { get; set; }

            /// <summary>A nullable <see cref="System.Int32" />, Optional : null when not set</summary>
            [LxElementValue(1, "Quantity_Int_O", "https://markpelf.com/SmallCompany.xsd", LxValueType.Value, XsdType.XsdInt, MinOccurs = 0, MaxOccurs = 1)]
            public System.Int32? Quantity_Int_O { get; set; }

        }

    }

    #endregion

}

