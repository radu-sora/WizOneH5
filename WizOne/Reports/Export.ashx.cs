using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.SessionState;
using WizOne.Module;

namespace WizOne.Reports
{
    /// <summary>
    /// Report export http handler
    /// Request example:
    /// GET: https://servername/appname/reports/export?phone=value1&serial=value2
    /// Response example (200):
    /// Empty response
    /// Response example (400 & 500):
    /// {
    ///     ErrorMessage: 'validation or server error'
    /// } 
    /// </summary>
    public class Export : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.TrySkipIisCustomErrors = true;
            context.Response.ContentType = "application/json";

            try
            {
                var phone = context.Request.QueryString["phone"];
                var serial = 0;

                int.TryParse(context.Request.QueryString["serial"], out serial);

                if (phone != null && serial > 0)                
                    TrimitereWhatsApp.SendAsync(phone, serial);                                                       
                else
                {
                    context.Response.StatusCode = 400; // Bad request
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        ErrorMessage = "Invalid parameters."
                    }));
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "ReportExportHttpHandler", new StackTrace().GetFrame(0).GetMethod().Name);
                
                context.Response.StatusCode = 500; // Internal server error
                context.Response.Write(JsonConvert.SerializeObject(new 
                {
                    ErrorMessage = "Export service error. Check server log."
                }));
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