using System;
using System.Web;

namespace Wizrom.Reports.Code
{
    public class ReportSessionModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.PostAcquireRequestState += (s, e) => 
            {
                try
                {
                    var context = (s as HttpApplication).Context;

                    if (context.Session != null)
                        ReportProxy.ClearSessions();
                }
                catch (Exception)
                {
                    // Log error here
                }                
            };
        }

        public void Dispose()
        {            
        }
    }
}
