/*----------------------------------------------------------------------
Name:			TableDetailsTextBox

Description:	Generates a table details text box rendered in RDL for 
				use in a TabularReport object.

History:
--------
02 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a table details text box rendered in RDL for 
	/// use in a TabularReport object.
	/// </summary>
	public class TableDetailsTextBox : IRdlRenderable
	{
		#region Private data members

		// Name of this textbox
		private string _name;

		// Is this field empty?
		private bool   _isEmpty;

		// Data type of the data item in this textbox
		private string _dataType;

        private string _hyperlink;

		// Style of this textbox
		private ReportingServicesStyle _textboxStyle;

		#endregion

		#region Constructors

		/// <summary>
		/// Generates a table details text box rendered in RDL for 
		/// use in a TabularReport object.
		/// </summary>
		/// <param name="name">Name of this textbox.</param>
		/// <param name="isEmpty">Denotes if this field is empty.</param>
		/// <param name="dataType">Data type of this data item.</param>
		public TableDetailsTextBox(string name, bool isEmpty, string dataType,string hyperlink=null)
		{
			_name    = name;
			_isEmpty = isEmpty;
			_dataType = dataType;
            _hyperlink = hyperlink;
			_textboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.TableDetailsTextBox);
		}

		#endregion

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL TableCell to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			string TextboxValue = _isEmpty ? "" : "=Fields!" + _name + ".Value";
			string Alignment = RdlFormatter.FormatAlignment(_dataType);
			_textboxStyle.Format = RdlFormatter.FormatData(_dataType);

			xmlWriter.WriteStartElement("TableCell");
			xmlWriter.WriteStartElement("ReportItems");
			RdlRender.AddTextbox( xmlWriter, "Details" + _name, TextboxValue, _textboxStyle,
				Alignment, "0in", "0in", ".5in", "true", "", "", "", "", "", "", "", "", "", "");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

        /// <summary>
        /// Makes object render itself as an RDL TableCell to an XmlWriter instance.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            string TextboxValue = _isEmpty ? "" : "=Fields!" + _name + ".Value";
            string Alignment = RdlFormatter.FormatAlignment(_dataType);
            _textboxStyle.Format = RdlFormatter.FormatData(_dataType);

            xmlWriter.WriteStartElement("TablixCell");
            xmlWriter.WriteStartElement("CellContents");
            RdlRender.AddTextbox2010(xmlWriter, "Details" + _name, TextboxValue, _textboxStyle,
                Alignment, "0in", "0in", ".5in", "true", "", "", "", "", "", "", "", "", "", "",_hyperlink);
            xmlWriter.WriteEndElement();//CellContents
            xmlWriter.WriteEndElement();//TablixCell
        }
	}
}
