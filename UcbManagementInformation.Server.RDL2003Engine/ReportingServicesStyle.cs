/*----------------------------------------------------------------------
Name:			ReportingServicesStyle

Description:	This object encapsulates the elements of a Style complex 
				type (in RDL) so many objects may safely share this 
				class for managing Style type usage.
				
				Hence, you can add or remove data members from this class
				and all other objects using this class will be protected
				from these changes. It should be possible to use this
				class to produce any Style in an RDL file.

History:
--------
05 Apr 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
21 Jul 2005		1.03 LL		TIR0200 - Set Text Align to allow Column Heading 
							to be aligned to user's preference
27 Jul 2005		1.04 LL		TIR0243 Set Border Lines for Report
							Also changed the font size to 10 for Table Group TextBox.
17 Aug 2005		1.05 LL		TIR0267 - Changed font and background colour and font size							
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// This object encapsulates the elements of a Style complex type 
	/// (in RDL) so many objects may safely share this class for managing 
	/// Style type usage.
	/// </summary>
	public class ReportingServicesStyle : IRdlRenderable, UcbManagementInformation.Server.RDL2003Engine.IReportingServicesStyle
	{
		#region Private data members
		
		// Textboxes that use this Style.
		public enum TextBoxStyle 
		{
			TabularReport10pt,
			TabularReport14pt,
			TableHeaderTextBox,
			TableHeaderTextBoxRightAligned,
			TableGroupTextBox,
			TableDetailsTextBox,
			MatrixReport,
			MatrixReportCorner,
			MatrixDynamic,
			MatrixRow,
            BoldBordered,
            PlainBordered
		}
		
		// The members of the Style complex type
		private string _backgroundColour	= "";
		private string _borderStyle			= "";
		private string _borderColour		= "";
		private string _paddingLeft			= "";
		private string _paddingRight		= "";
		private string _paddingTop			= "";
		private string _paddingBottom		= "";
		private string _fontSize			= "";
		private string _fontWeight			= "";
		private string _verticalAlign		= "";
		private string _color				= "";
		private string _textDecoration		= "";
		private string _format				= "";
		private string _textAlign			= "";

		#endregion


		#region Constructors and constructor helper methods

        //public ReportingServicesStyle(string backgroundColour, string borderStyle, string borderColour,
        //    string paddingLeft, string paddingRight, string paddingTop, string paddingBottom,
        //    string fontSize, string fontWeight, string verticalAlign, string colour, 
        //    string textDecoration, string format)
        //{
        //    _backgroundColour	= backgroundColour;
        //    _borderStyle		= borderStyle;
        //    _borderColour		= borderColour;
        //    _paddingLeft		= paddingLeft;
        //    _paddingRight		= paddingRight;
        //    _paddingTop			= paddingTop;
        //    _paddingBottom		= paddingBottom;
        //    _fontSize			= fontSize;
        //    _fontWeight			= fontWeight;
        //    _verticalAlign		= verticalAlign;
        //    _color				= colour;
        //    _textDecoration		= textDecoration;
        //    _format				= format;
        //}
	
		/// <summary>
		/// Creates a new ReportingServicesStyle for textboxes.
		/// </summary>
		/// <param name="style">Which textbox type this is a style for (use the 
		/// TextBoxStyle enum.</param>
		public ReportingServicesStyle(TextBoxStyle style) : this(style, null)
		{
		}

		//21/07/05 LL - TIR0200 added overloaded ReportingServicesStyle constructor 
		//for text alignment
		/// <summary>
		/// Creates a new ReportingServicesStyle for textboxes with Text Alignment.
		/// </summary>
		/// <param name="style">Which textbox type this is a style for (use the 
		/// TextBoxStyle enum.</param>
		/// <param name="textAlign">Indicate Text Alignment.</param>
		public ReportingServicesStyle(TextBoxStyle style, string textAlign)
		{
			//set Text Alignment
			if(textAlign != null)
				_textAlign = textAlign;
			else
				//set default value
				_textAlign = "";
	
			// Initialise the properties correctly.
			switch (style)
			{
				case TextBoxStyle.TabularReport10pt:
					SetupTabularReport10ptTextBox();
					break;
				case TextBoxStyle.TabularReport14pt:
					SetupTabularReport14ptTextBox();
					break;
				case TextBoxStyle.TableHeaderTextBox:
					SetupTableHeaderTextBox();
					break;
				case TextBoxStyle.TableGroupTextBox:
					SetupTableGroupTextBox();
					break;
				case TextBoxStyle.TableDetailsTextBox:
					SetupTableDetailsTextBox();
					break;
				case TextBoxStyle.MatrixReport:
					SetupMatrixReportTextBox();
					break;
				case TextBoxStyle.MatrixReportCorner:
					SetupMatrixReportCornerTextBox();
					break;
				case TextBoxStyle.MatrixDynamic:
					SetupMatrixDynamicTextBox();
					break;
				case TextBoxStyle.MatrixRow:
					SetupMatrixRow();
					break;
                case TextBoxStyle.BoldBordered:
                    SetupBoldBordered();
                    break;
                case TextBoxStyle.PlainBordered:
                    SetupPlainBordered();
                    break;
			}
		}
       

        private void SetupBoldBordered()
        {
            _borderStyle = "Solid";
            _borderColour = "Black";
            _fontSize = "10pt";
            _fontWeight = "Bold";
        }

        private void SetupPlainBordered()
        {
            _borderStyle = "Solid";
            _borderColour = "Black";
            _fontSize = "10pt";
        }
        
        private void SetupTabularReport10ptTextBox()
		{
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "10pt";
			_fontWeight       = "Bold"; 
			_verticalAlign    =	"Bottom";
            _color = "Black";
			_textDecoration   = "Underline";
		}


		/// <summary>
		/// Setup a TabularReportTextBox
		/// </summary>
		private void SetupTabularReport14ptTextBox()
		{
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "14pt";
			_fontWeight       = "Bold"; 
			_verticalAlign    =	"Bottom";
            _color = "Black";
			_textDecoration   = "Underline";
		}


		/// <summary>
		/// Setup a TableHeaderTextBox
		/// </summary>
		private void SetupTableHeaderTextBox()
		{
            _backgroundColour = "#005db3";
			_borderStyle      = "Solid";
			_borderColour     = "#4297d7";
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "12pt";
			_fontWeight       = "Bold";
			_verticalAlign    =	"Bottom";
            _color = "#eaf5f7";	
			_textDecoration   = "Underline";
		}

		/// <summary>
		/// Setup a TableGroupTextBox
		/// </summary>
		private void SetupTableGroupTextBox()
		{
            _backgroundColour = "#eeeeee";
			_borderStyle	  = "Solid";
			_borderColour	  = "#aaaaaa";
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "10pt";
			_fontWeight       = "Bold"; 
			_verticalAlign    =	"Bottom";
            _color = "Black";
		}

		/// <summary>
		/// Setup a TableDetailsTextBox
		/// </summary>
		public void SetupTableDetailsTextBox()
		{
			_borderStyle	  = "Solid";
			_borderColour	  = "#cccccc";
			_backgroundColour = @"=Iif(RowNumber(""Table1"") Mod 2, ""White"", ""#d7d7f5"")";
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "10pt";
			_fontWeight       = "Normal"; 
			_verticalAlign    =	"Bottom"; 
			_color			  =	"Black";
		}

		/// <summary>
		/// Setup a MatrixReportTextBox
		/// </summary>
		public void SetupMatrixReportTextBox()
		{
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "14pt";
			_fontWeight       = "Bold"; 
			_verticalAlign    =	"Bottom";
            _color = "#513184";
			_textDecoration   = "Underline";
		}

		/// <summary>
		/// Setup a MatrixReportCornerTextbox
		/// </summary>
		public void SetupMatrixReportCornerTextBox()
		{
			_paddingLeft   = "2pt";
			_paddingRight  = "2pt";
			_paddingTop    = "2pt";
			_paddingBottom = "2pt";
		}


		/// <summary>
		/// Setup a MatrixDynamicTextBox
		/// </summary>
		public void SetupMatrixDynamicTextBox()
		{
			_borderStyle	  = "Solid";
			_borderColour	  = "Black";
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
			_fontSize         = "12pt";
			_fontWeight       = "Bold";
            _backgroundColour = "#ded7ea";
            _color = "#513184";
		}

		/// <summary>
		/// Setup a MatrixRow
		/// </summary>
		public void SetupMatrixRow()
		{
            _backgroundColour = "#d2f1ef";
			_borderStyle	  = "Solid";
			_borderColour	  = "Black";
			_paddingLeft	  = "2pt";
			_paddingRight	  = "2pt";
			_paddingTop 	  = "2pt";
			_paddingBottom	  = "2pt";
		}

		#endregion


		/// <summary>
		/// Renders this style in RDL using the supplied XmlWriter.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter used to write the RDL.</param>
		public void Render(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Style");

			xmlWriter.WriteStartElement("BorderStyle");
				RdlRender.AddLine( xmlWriter, "Top", _borderStyle);
				RdlRender.AddLine( xmlWriter, "Bottom", _borderStyle);
				RdlRender.AddLine( xmlWriter, "Left", _borderStyle);
				RdlRender.AddLine( xmlWriter, "Right", _borderStyle);
			xmlWriter.WriteEndElement(); // BorderStyle
					
			xmlWriter.WriteStartElement("BorderColor");
				RdlRender.AddLine( xmlWriter, "Top", _borderColour);
				RdlRender.AddLine( xmlWriter, "Bottom", _borderColour);
				RdlRender.AddLine( xmlWriter, "Left", _borderColour);
				RdlRender.AddLine( xmlWriter, "Right", _borderColour);				
			xmlWriter.WriteEndElement(); // BorderColour

			RdlRender.AddLine( xmlWriter, "BackgroundColor", _backgroundColour);
			RdlRender.AddLine(xmlWriter, "PaddingLeft", _paddingLeft);
			RdlRender.AddLine(xmlWriter, "PaddingRight", _paddingRight);
			RdlRender.AddLine(xmlWriter, "PaddingTop", _paddingTop);
			RdlRender.AddLine(xmlWriter, "PaddingBottom", _paddingBottom);
			RdlRender.AddLine(xmlWriter, "FontSize", _fontSize);
			RdlRender.AddLine(xmlWriter, "FontWeight", _fontWeight);
			//21/07/05 LL - TIR0200 Set Text Alignment
			RdlRender.AddLine(xmlWriter, "TextAlign", _textAlign);
			RdlRender.AddLine(xmlWriter, "VerticalAlign", _verticalAlign);
			RdlRender.AddLine(xmlWriter, "Color", _color);
			RdlRender.AddLine(xmlWriter, "TextDecoration", _textDecoration);
			RdlRender.AddLine(xmlWriter, "Format", _format);

			// end Style tag
			xmlWriter.WriteEndElement();
		}

        /// <summary>
        /// Renders this style in RDL using the supplied XmlWriter.
        /// </summary>
        /// <param name="xmlWriter">The XmlWriter used to write the RDL.</param>
        public void Render2010TextRun(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Style");

            //xmlWriter.WriteStartElement("BorderStyle");
            //RdlRender.AddLine(xmlWriter, "Top", _borderStyle);
            //RdlRender.AddLine(xmlWriter, "Bottom", _borderStyle);
            //RdlRender.AddLine(xmlWriter, "Left", _borderStyle);
            //RdlRender.AddLine(xmlWriter, "Right", _borderStyle);
            //xmlWriter.WriteEndElement(); // BorderStyle

            //xmlWriter.WriteStartElement("BorderColor");
            //RdlRender.AddLine(xmlWriter, "Top", _borderColour);
            //RdlRender.AddLine(xmlWriter, "Bottom", _borderColour);
            //RdlRender.AddLine(xmlWriter, "Left", _borderColour);
            //RdlRender.AddLine(xmlWriter, "Right", _borderColour);
            //xmlWriter.WriteEndElement(); // BorderColour

            //RdlRender.AddLine(xmlWriter, "BackgroundColor", _backgroundColour);
            //RdlRender.AddLine(xmlWriter, "PaddingLeft", _paddingLeft);
            //RdlRender.AddLine(xmlWriter, "PaddingRight", _paddingRight);
            //RdlRender.AddLine(xmlWriter, "PaddingTop", _paddingTop);
            //RdlRender.AddLine(xmlWriter, "PaddingBottom", _paddingBottom);
            RdlRender.AddLine(xmlWriter, "FontSize", _fontSize);
            RdlRender.AddLine(xmlWriter, "FontWeight", _fontWeight);
            //21/07/05 LL - TIR0200 Set Text Alignment
            //RdlRender.AddLine(xmlWriter, "TextAlign", _textAlign);
            //RdlRender.AddLine(xmlWriter, "VerticalAlign", _verticalAlign);
            RdlRender.AddLine(xmlWriter, "Color", _color);
            RdlRender.AddLine(xmlWriter, "TextDecoration", _textDecoration);
            RdlRender.AddLine(xmlWriter, "Format", _format);

            // end Style tag
            xmlWriter.WriteEndElement();//Style
        }
        public void Render2010TextBox(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Style");

            xmlWriter.WriteStartElement("TopBorder");
            RdlRender.AddLine(xmlWriter, "Color", _borderColour);
            RdlRender.AddLine(xmlWriter, "Style", _borderStyle);
            xmlWriter.WriteEndElement(); // TopBorder
            xmlWriter.WriteStartElement("BottomBorder");
            RdlRender.AddLine(xmlWriter, "Color", _borderColour);
            RdlRender.AddLine(xmlWriter, "Style", _borderStyle);
            xmlWriter.WriteEndElement(); // BottomBorder
            xmlWriter.WriteStartElement("LeftBorder");
            RdlRender.AddLine(xmlWriter, "Color", _borderColour);
            RdlRender.AddLine(xmlWriter, "Style", _borderStyle);
            xmlWriter.WriteEndElement(); // LeftBorder
            xmlWriter.WriteStartElement("RightBorder");
            RdlRender.AddLine(xmlWriter, "Color", _borderColour);
            RdlRender.AddLine(xmlWriter, "Style", _borderStyle);
            xmlWriter.WriteEndElement(); // RightBorder
            
            RdlRender.AddLine(xmlWriter, "BackgroundColor", _backgroundColour);
            RdlRender.AddLine(xmlWriter, "PaddingLeft", _paddingLeft);
            RdlRender.AddLine(xmlWriter, "PaddingRight", _paddingRight);
            RdlRender.AddLine(xmlWriter, "PaddingTop", _paddingTop);
            RdlRender.AddLine(xmlWriter, "PaddingBottom", _paddingBottom);
            //????? RdlRender.AddLine(xmlWriter, "TextAlign", _textAlign);
            RdlRender.AddLine(xmlWriter, "VerticalAlign", _verticalAlign);
            RdlRender.AddLine(xmlWriter, "Format", _format);

            // end Style tag
            xmlWriter.WriteEndElement();//Style
        }

		#region Properties

		//TODO: Code Review Issue 11/05/05: Missing XML summaries
		// Done 24/05/2005

		/// <summary>
		/// The background colour property of this style.
		/// </summary>
		public string BackgroundColor
		{
			get { return _backgroundColour; }
		}

		/// <summary>
		/// The border style property of this style.
		/// </summary>
		public string BorderStyle
		{
			get { return _borderStyle; }
		}

		/// <summary>
		/// The border colour property of this style.
		/// </summary>
		public string BorderColour
		{
			get { return _borderColour; }
		}

		/// <summary>
		/// The left padding property of this style.
		/// </summary>
		public string PaddingLeft
		{
			get { return _paddingLeft; }
		}

		/// <summary>
		/// The right padding property of this style.
		/// </summary>
		public string PaddingRight
		{
			get { return _paddingRight; }
		}

		/// <summary>
		/// The top padding property of this style.
		/// </summary>
		public string PaddingTop
		{
			get { return _paddingTop; }
		}

		/// <summary>
		/// The bottom padding property of this style.
		/// </summary>
		public string PaddingBottom
		{
			get { return _paddingBottom; }
		}

		/// <summary>
		/// The font size used in this style.
		/// </summary>
		public string FontSize
		{
			get { return _fontSize; }
		}

		/// <summary>
		/// The font weight used in this style.
		/// </summary>
		public string FontWeight
		{
			get { return _fontWeight; }
		}

		/// <summary>
		/// The vertical alignment of this style.
		/// </summary>
		public string VerticalAlign
		{
			get { return _verticalAlign; }
		}

		/// <summary>
		/// The foreground colour property of this style.
		/// </summary>
		public string Color
		{
			get { return _color; }
		}

		/// <summary>
		/// The text decoration property of this style.
		/// </summary>
		public string TextDecoration
		{
			get { return _textDecoration; }
		}

		/// <summary>
		/// The format property of this style.
		/// </summary>
		public string Format
		{
			get { return _format; }
			set { _format = value; }
		}

		/// <summary>
		/// The text alignment property of this style.
		/// </summary>
		public string TextAlign
		{
			get { return _textAlign; }
			//21/07/05 LL - TIR0200 Allow Text Alignment to be set
			set { _textAlign = value; }
		}

		#endregion

        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
