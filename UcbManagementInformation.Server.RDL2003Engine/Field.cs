/*----------------------------------------------------------------------
Name:			Field

Description:	Object representing a field of a DataSet, able to render
				itself in RDL.

History:
--------
02 Mar 2005		1.00 KSB	Created.
10 May 2005		1.01 LL		Code Review
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05
using System;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Object representing a field of a DataSet, able to render itself in RDL.
	/// </summary>
    public class Field : IRdlRenderable
	{
		// Name of this field
		private string _name;

		// Caption of this field
		private string _caption;

		/// <summary>
		/// Constructs a new field.
		/// </summary>
		/// <param name="name">Name of this field.</param>
		/// <param name="caption">Name of this field.</param>
		public Field(string name, string caption)
		{
			_name = name;
			_caption = caption;
		}

		//TODO: Code Review Issue 11/05/05: Give a more specific summary rather than
		//using 'object' word.
		// Done 24/05/2005
		/// <summary>
		/// Makes this field render itself as an RDL Field to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Field");
			xmlWriter.WriteAttributeString("Name", _name);
			RdlRender.AddLine( xmlWriter, "DataField", _caption);
			xmlWriter.WriteEndElement();
		}

        public void Render2010(XmlWriter xmlWriter)
        {
            Render(xmlWriter);
        }
    }
}
