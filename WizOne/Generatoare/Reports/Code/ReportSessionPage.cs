using System;
using System.Web.UI;

namespace Wizrom.Reports.Code
{
    public class ReportSessionPage : Page
    {
        private bool _close;

        protected dynamic ReportSession
        {
            get; private set;
        }

        protected override void OnInit(EventArgs e)
        {
            _close = (Request.Form["close"] == "true");

            try
            {
                if (_close)
                    ReportProxy.RecycleSession();
                else
                    ReportSession = ReportProxy.GetSession();
            }
            catch (Exception)
            {
                // Log error here
                if (!_close)
                {
                    _close = true;

                    if (!IsPostBack) // Close the page
                        Response.Redirect(Request.UrlReferrer?.LocalPath ?? "~/");
                    else // Or, page loaded, show the error
                        throw;
                }
            }

            if (!_close)
                base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!_close)
                base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!_close)
                base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!_close)
                base.Render(writer);
        }
    }
}