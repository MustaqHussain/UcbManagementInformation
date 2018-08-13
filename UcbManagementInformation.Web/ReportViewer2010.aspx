<%@ Page Language="C#" AutoEventWireup="true" Culture="en-GB" CodeBehind="ReportViewer2010.aspx.cs" Inherits="UcbManagementInformation.Web.ReportViewer2010" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"><style type="text/css">html,body,form {height:100%}</style>
    
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
   <asp:scriptmanager runat="server" AsyncPostBackTimeout="600"></asp:scriptmanager>
   
    <rsweb:ReportViewer id="OperationalReportViewer" runat="server" processingmode="Remote"
             AsyncRendering="true" Width="100%" Height="100%" ><serverreport />
    </rsweb:ReportViewer>
        
            
      
     
    
    </form>
</body>
</html>
