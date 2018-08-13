using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Net;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Web.Server;
namespace UcbManagementInformation.Web
{
    public partial class ReportViewer2010 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string reportPath = "/" + Session["report"];
                // Get report server and location information
                string ReportServer = ConfigurationManager.AppSettings["ReportServer"];
                string ReportLocation = ConfigurationManager.AppSettings["ReportLocation"];

                this.OperationalReportViewer.ServerReport.ReportServerUrl = new Uri(ReportServer);
                this.OperationalReportViewer.ServerReport.ReportServerCredentials = new ReportCredentials();
                this.OperationalReportViewer.ServerReport.ReportPath = reportPath;
            

                string staffGuid = ReportWizardService.GetUserGuid();
                try
                {
                    
                    List<ReportParameter> ParameterList = new List<ReportParameter>();
                    ParameterList.Add(new ReportParameter("ReportUserName",User.Identity.Name));
                    ParameterList.Add(new ReportParameter("ReportUserCode", staffGuid));
                    
                    foreach (string pKey in this.Request.QueryString.AllKeys)
                    {
                        if (pKey != "report")
                        {
                            string pValue = this.Request.QueryString[pKey];
                            ParameterList.Add(new ReportParameter(pKey, pValue));
                        }
                    }
                    this.OperationalReportViewer.ServerReport.SetParameters(ParameterList);
                }
                catch
                {
                    //If ReportUser parameter doesn't exist ignore it.
                }
                ScriptManager MyScriptManager = ScriptManager.GetCurrent(this);
                MyScriptManager.AsyncPostBackTimeout = 600;
            }
        }

        

        public class ReportCredentials : IReportServerCredentials
        {

            #region IReportServerCredentials Members

            public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
            {
                authCookie = null;
                userName = null;
                password = null;
                authority = null;

                // Not using form credentials
                return false;
            }

            public System.Security.Principal.WindowsIdentity ImpersonationUser
            {
                get { return null; }
            }

            public ICredentials NetworkCredentials
            {
                get
                {
                    //set the credentials
                    string UserReportID = ConfigurationManager.AppSettings["userReportID"];
                    string UserReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
                    string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

                    return new NetworkCredential(UserReportID, UserReportPassword, UserReportDomain);
                }
            }

            #endregion
        }
        
    }
}