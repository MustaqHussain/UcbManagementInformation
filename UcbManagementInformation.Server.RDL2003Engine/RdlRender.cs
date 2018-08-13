/*----------------------------------------------------------------------
Name:			RdlTextbox

Description:	Object offering a collection of methods for
				rendering RDL items to an XmlWriter.

History:
--------
25 Feb 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.IO;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Object offering a collection of methods for rendering RDL 
	/// items to an XmlWriter.
	/// </summary>
	public class RdlRender
	{
		/// <summary>
		/// Private to prevenit instantiation.
		/// </summary>
		private RdlRender()
		{

		}
        /// <summary>
        /// Adds a Textbox rendered in RDL to an XmlWriter. The parameters allow all 
        /// information that a textbox may accept to be passed. If an empty string is
        /// passed then no element tags will added to the RDL for that element. Refer to 
        /// Reporting Services documentation to see which elements are required.
        /// </summary>
        /// <param name="xmlWriter">The XmlWriter object to render to.</param>

        public static void AddTextbox2010(XmlWriter xmlWriter, string nameAttribute,
            string valueElement, IReportingServicesStyle textboxStyle, string textAlign,
            string top, string left, string width, string canGrow, string height,
            string zIndex, string visibility, string toolTip, string label,
            string linkToChild, string bookmark, string repeatWith, string canShrink,
            string hideDuplicates,string hyperlink=null)
        {
            //TODO: Code Review Issue 11/05/05: give comment and/or indent the code as in XML 
            //to increase readability
            // Done 24/05/2005
            xmlWriter.WriteStartElement("Textbox");
            xmlWriter.WriteAttributeString("Name", nameAttribute);
            //AddLine(xmlWriter, "KeepTogether", keepTogether);
            xmlWriter.WriteStartElement("Paragraphs");
            xmlWriter.WriteStartElement("Paragraph");
            xmlWriter.WriteStartElement("TextRuns");
            xmlWriter.WriteStartElement("TextRun");
            xmlWriter.WriteStartElement("Value");
            xmlWriter.WriteString(valueElement);
            xmlWriter.WriteEndElement();//Value
            if (hyperlink !=null)
            {
                xmlWriter.WriteStartElement("ActionInfo");
                xmlWriter.WriteStartElement("Actions");
                xmlWriter.WriteStartElement("Action");
                AddLine(xmlWriter, "Hyperlink", hyperlink);
                xmlWriter.WriteEndElement();//Action</Action>
                xmlWriter.WriteEndElement();//Actions</Actions>
                xmlWriter.WriteEndElement();//ActionInfo
            }
            textboxStyle.Render2010TextRun(xmlWriter);
            xmlWriter.WriteEndElement();//TextRun
            xmlWriter.WriteEndElement();//TextRuns
            xmlWriter.WriteEndElement();//Paragraph
            xmlWriter.WriteEndElement();//Paragraphs
            textboxStyle.Render2010TextBox(xmlWriter);


            AddLine(xmlWriter, "CanGrow", canGrow);
            AddLine(xmlWriter, "Top", top);
            AddLine(xmlWriter, "Left", left);
            AddLine(xmlWriter, "Height", height);
            AddLine(xmlWriter, "Width", width);
            AddLine(xmlWriter, "ZIndex", zIndex);
            AddLine(xmlWriter, "Visibility", visibility);
            AddLine(xmlWriter, "ToolTip", toolTip);
            AddLine(xmlWriter, "DocumentMapLabel", label);
            AddLine(xmlWriter, "LinkToChild", linkToChild);
            AddLine(xmlWriter, "Bookmark", bookmark);
            AddLine(xmlWriter, "RepeatWith", repeatWith);
            AddLine(xmlWriter, "CanShrink", canShrink);
            AddLine(xmlWriter, "HideDuplicates", hideDuplicates);

            xmlWriter.WriteEndElement();//TextBox

            
        }

		/// <summary>
		/// Adds a Textbox rendered in RDL to an XmlWriter. The parameters allow all 
		/// information that a textbox may accept to be passed. If an empty string is
		/// passed then no element tags will added to the RDL for that element. Refer to 
		/// Reporting Services documentation to see which elements are required.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter object to render to.</param>
		
		public static void AddTextbox( XmlWriter xmlWriter, string nameAttribute,
			string valueElement, IReportingServicesStyle textboxStyle, string textAlign, 
			string top, string left, string width, string canGrow, string height, 
			string zIndex, string visibility, string toolTip, string label,
			string linkToChild, string bookmark, string repeatWith, string canShrink,
			string hideDuplicates)
		{
			//TODO: Code Review Issue 11/05/05: give comment and/or indent the code as in XML 
			//to increase readability
			// Done 24/05/2005
			xmlWriter.WriteStartElement("Textbox");
			xmlWriter.WriteAttributeString("Name", nameAttribute);
			
				xmlWriter.WriteStartElement("Value");
					xmlWriter.WriteString(valueElement);
				xmlWriter.WriteEndElement();
			
				textboxStyle.Render(xmlWriter);

				AddLine( xmlWriter, "Top", top);
				AddLine( xmlWriter, "Left", left);
				AddLine( xmlWriter, "Height", height);
				AddLine( xmlWriter, "Width", width);
				AddLine( xmlWriter, "CanGrow", canGrow);
				AddLine( xmlWriter, "ZIndex", zIndex);
				AddLine( xmlWriter, "Visibility", visibility);
				AddLine( xmlWriter, "ToolTip", toolTip);
				AddLine( xmlWriter, "Label", label);
				AddLine( xmlWriter, "LinkToChild", linkToChild);
				AddLine( xmlWriter, "Bookmark", bookmark);
				AddLine( xmlWriter, "RepeatWith", repeatWith);
				AddLine( xmlWriter, "CanShrink", canShrink);
				AddLine( xmlWriter, "HideDuplicates", hideDuplicates);


			xmlWriter.WriteEndElement();
		}
		
		/// <summary>
		/// Adds a value enclosed by open and close tags.
		/// </summary>
		/// <param name="xmlWriter">XmlWriter to be written to.</param>
		/// <param name="element">The element tag name.</param>
		/// <param name="elementValue">The element value.</param>
		public static void AddLine( XmlWriter xmlWriter, string element, string elementValue)
		{
			//TODO: Code Review Issue 11/05/05: null value should be checked first.
			// Done 24/05/2005
			if (! (elementValue == null || elementValue == "") ) 
			{
				xmlWriter.WriteStartElement(element);
				xmlWriter.WriteString(elementValue);
				xmlWriter.WriteEndElement();
			}
		}

		//TODO: Code Review Issue 11/05/05: Missing XML summary
		// Done 24/05/2005
		/// <summary>
		/// Adds a Image rendered in RDL to an XmlWriter instance. The parameters allow all
		/// elements belonging to an Image type to be populated, although an empty string
		/// can be supplied to optional elements. Refer to Reporting Services to determine
		/// which elements are optional.
		/// </summary>
		/// <param name="xmlWriter"></param>
		/// <param name="name"></param>
		/// <param name="height"></param>
		/// <param name="width"></param>
		/// <param name="left"></param>
		/// <param name="source"></param>
		/// <param name="imageValue"></param>
		/// <param name="sizing"></param>
		public static void AddImage( XmlWriter xmlWriter, string name, 
			string height, string width, string left, string source, string imageValue,
			string sizing)
		{
			xmlWriter.WriteStartElement("Image");
			xmlWriter.WriteAttributeString("Name", name);
			AddLine( xmlWriter, "Height", height);
			AddLine( xmlWriter, "Width", width);
			AddLine( xmlWriter, "Left", left);
			AddLine( xmlWriter, "Source", source);
			AddLine( xmlWriter, "Value", imageValue);
			AddLine( xmlWriter, "Sizing", sizing);
			xmlWriter.WriteEndElement();
		}

        /// <summary>
        /// Adds a Image rendered in RDL to an XmlWriter instance. The parameters allow all
        /// elements belonging to an Image type to be populated, although an empty string
        /// can be supplied to optional elements. Refer to Reporting Services to determine
        /// which elements are optional.
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="name"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="left"></param>
        /// <param name="source"></param>
        /// <param name="imageValue"></param>
        /// <param name="sizing"></param>
        public static void AddImage2010(XmlWriter xmlWriter, string name,
            string height, string width, string left, string source, string imageValue,
            string sizing)
        {
            xmlWriter.WriteStartElement("Image");
            xmlWriter.WriteAttributeString("Name", name);
            AddLine(xmlWriter, "Height", height);
            AddLine(xmlWriter, "Width", width);
            AddLine(xmlWriter, "Left", left);
            AddLine(xmlWriter, "Source", source);
            AddLine(xmlWriter, "Value", imageValue);
            AddLine(xmlWriter, "Sizing", sizing);
            AddLine(xmlWriter, "ZIndex", "2");
            xmlWriter.WriteEndElement();
        }
		/// <summary>
		/// Creates an XmlDocument object given a MemoryStream object. This MemoryStream
		/// must contain well-formed XML, so it should have been generated by an XmlWriter.
		/// </summary>
		/// <param name="memoryStream">The MemoryStream that contains the XML data.</param>
		/// <returns>An XMLDocument object containing a report rendered in RDL.</returns>
		public static XmlDocument CreateXmlDocument(MemoryStream memoryStream)
		{
			// Holds the raw bytes within the MemoryStream
			Byte[] TestByteArray = new Byte[memoryStream.Length];

			// The XmlDocument to be created
			XmlDocument XmlDoc = new XmlDocument();

			// Holds the XML data to be loaded into the XmlDocument
			XmlTextReader TestXmlReader;

			// A temporary string to hold the XML data
			string XmlString = "";
			
			XmlParserContext context = new XmlParserContext(null, null, null, XmlSpace.Preserve);
			
			/* The MemoryStream is read into TestByteArray so can be interated through.
			 * This data is put into a string, which is taken by an XmlReader. This Reader
			 * is in turn fed into the XmlDocument.
			*/
			
			memoryStream.Seek(0, SeekOrigin.Begin);
			TestByteArray = memoryStream.ToArray();
			
			foreach (Byte CurrentByte in TestByteArray)
				XmlString += ((char)CurrentByte).ToString();
			
			// The MemoryStream always begins with 3 "garbage" characters, so these are not
			// included in the Reader.
			TestXmlReader = new XmlTextReader(XmlString.Substring(3), XmlNodeType.Element,
				context);
			
			XmlDoc.Load(TestXmlReader);
			
			return XmlDoc;
		}

	}
}
