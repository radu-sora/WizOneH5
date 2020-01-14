using System;
using System.Web;
using System.Web.SessionState;
using WizOne.Generatoare.Reports.Code;

namespace WizOne.Generatoare.Reports.Pages
{
    /// <summary>
    /// Report print http handler
    /// </summary>
    public class Print : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var session = ReportProxy.GetSession() as dynamic;

                ReportProxy.Print(session.ReportId, session.ParamList);
                ReportProxy.RemoveSession();
            }
            catch (Exception ex)
            {
                // Log error here
                // For now, just display the error message
                context.Response.ContentType = "text/plain";
                context.Response.Write(ex.Message);
            }
            finally
            {
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}