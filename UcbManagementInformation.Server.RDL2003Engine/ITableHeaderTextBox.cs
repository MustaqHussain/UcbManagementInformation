using System;
namespace UcbManagementInformation.Server.RDL2003Engine
{
    public interface ITableHeaderTextBox : IRdlRenderable
    {
        string Caption { get; }
        string Name { get; }
        void Render2010(System.Xml.XmlWriter xmlWriter);
        IReportingServicesStyle TextboxStyle { get; }
    }
}
