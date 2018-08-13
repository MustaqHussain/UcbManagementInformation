using System;
namespace UcbManagementInformation.Server.RDL2003Engine
{
    public interface IReportingServicesStyle
    {
        string BackgroundColor { get; }
        string BorderColour { get; }
        string BorderStyle { get; }
        string Color { get; }
        string FontSize { get; }
        string FontWeight { get; }
        string Format { get; set; }
        string PaddingBottom { get; }
        string PaddingLeft { get; }
        string PaddingRight { get; }
        string PaddingTop { get; }
        void Render(System.Xml.XmlWriter xmlWriter);
        void Render2010(System.Xml.XmlWriter xmlWriter);
        void Render2010TextBox(System.Xml.XmlWriter xmlWriter);
        void Render2010TextRun(System.Xml.XmlWriter xmlWriter);
        void SetupMatrixDynamicTextBox();
        void SetupMatrixReportCornerTextBox();
        void SetupMatrixReportTextBox();
        void SetupMatrixRow();
        void SetupTableDetailsTextBox();
        string TextAlign { get; set; }
        string TextDecoration { get; }
        string VerticalAlign { get; }
    }
}
