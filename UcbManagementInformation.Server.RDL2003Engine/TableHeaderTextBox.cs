/*----------------------------------------------------------------------
Name:			TableHeaderTextBox

Description:	Generates a table header text box rendered in RDL for use 
				in a TableHeader object.

History:
--------
24 Feb 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
21 Jul 2005		1.03 LL		TIR0200 - Added overloaded constructor to allow text alignment
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using System.Collections.Generic;

namespace UcbManagementInformation.Server.RDL2003Engine
{

    
	/// <summary>
	/// Generates a table header text box  for use in a TableHeader
	/// object.
	/// </summary>
    public class TableHeaderTextBox : ITableHeaderTextBox
	{

		#region Private data members

		// TableHeader name and caption
        private string _name;

        public string Name
        {
            get { return _name; }
            //set { _name = value; }
        }
        private string _caption;

        public string Caption
        {
            get { return _caption; }
            //set { _caption = value; }
        }

		// Style of this textbox
        private IReportingServicesStyle _textboxStyle;

        public IReportingServicesStyle TextboxStyle
        {
            get { return _textboxStyle; }
            //set { _textboxStyle = value; }
        }

		#endregion

		#region Constructors
		
		/// <summary>
		/// Generates a table header text box  for use in a TableHeader
		/// object.
		/// </summary>
		/// <param name="name">Name of this textbox.</param>
		/// <param name="caption">Caption of this textbox.</param>
		public TableHeaderTextBox(string name, string caption) : this(name, caption, false)
		{
		}

		/// <summary>
		/// Generates a table header text box to allow text alignment in a TableHeader
		/// object.
		/// </summary>
		/// <param name="name">Name of this textbox.</param>
		/// <param name="caption">Caption of this textbox.</param>
		/// <param name="isRightAligned">Indicate the textbox alignment. True: right-aligned.</param>
		public TableHeaderTextBox(string name, string caption, bool isRightAligned)
		{
			_name    = name;
			_caption = caption;

			if(!isRightAligned)
			{
                Dictionary<string, object> styleHolder = new Dictionary<string, object>();
            
                styleHolder.Add("style",ReportingServicesStyle.TextBoxStyle.TableHeaderTextBox);

                _textboxStyle = SimpleServiceLocator.Instance.Get<IReportingServicesStyle>("ReportingServicesStyle", styleHolder);
                 // new ReportingServicesStyle(
					//ReportingServicesStyle.TextBoxStyle.TableHeaderTextBox);
			}
			else
			{
                Dictionary<string, object> styleHolder = new Dictionary<string, object>();
            
                styleHolder.Add("style",ReportingServicesStyle.TextBoxStyle.TableHeaderTextBox);
                styleHolder.Add("rightAlign","Right");

                _textboxStyle = SimpleServiceLocator.Instance.Get<IReportingServicesStyle>("ReportingServicesStyleRight", styleHolder);//new ReportingServicesStyle(
					//ReportingServicesStyle.TextBoxStyle.TableHeaderTextBox, "Right");
			}
		}

		#endregion

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL TableCell type to an XmlWriter object.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("TableCell");
			xmlWriter.WriteStartElement("ReportItems");
			RdlRender.AddTextbox( xmlWriter, "Header" + _name, _caption, _textboxStyle, 
				"Center", "0in", "0in", ".5in", "true", "", "", "", "", "", "", "", "", "", "");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

        /// <summary>
        /// Makes object render itself as an RDL TableCell type to an XmlWriter object.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TablixCell");
            xmlWriter.WriteStartElement("CellContents");
            RdlRender.AddTextbox2010(xmlWriter, "Header" + _name, _caption, _textboxStyle,
                "Center", "0in", "0in", ".5in", "true", "", "", "", "", "", "", "", "", "", "");
            xmlWriter.WriteEndElement();//CellContents
            xmlWriter.WriteEndElement();//TablixCell
        }
	}
}
