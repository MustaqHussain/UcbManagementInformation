/*----------------------------------------------------------------------
Name:			MatrixDynamicRowSubTotal

Description:	Generates a dynamic row subtotal rendered in RDL 
				for use in a MatrixReport object.

History:
--------
07 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a dynamic row subtotal rendered in RDL for use
	/// in a MatrixReport object.
	/// </summary>
    public class MatrixDynamicRowSubTotal : IRdlRenderable
	{
		#region Private data members

		// Name of this subtotal
		private string _name;

		// Style of a textbox
		private ReportingServicesStyle _textboxStyle;
		
		#endregion

		/// <summary>
		/// Constructs a new MatrixDynamicRowSubtotal
		/// </summary>
		/// <param name="name">Name of this subtotal.</param>
		public MatrixDynamicRowSubTotal(string name)
		{
			_name = name;
			_textboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.MatrixDynamic);
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as an RDL Subtotal to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter to render the object to.</param>
		public void Render(XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: Indent blocks of code as in XML to increase readability.
			// Done 24/05/2005
			xmlWriter.WriteStartElement("Subtotal");
				xmlWriter.WriteStartElement("ReportItems");
					RdlRender.AddTextbox(xmlWriter, _name, "Total", _textboxStyle, "", "", "", "", 
						"true", "", "", "", "", "", "", "", "", "", "");
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}


        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
