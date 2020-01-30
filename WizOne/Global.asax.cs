using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Routing;
using WizOne.Module;
using Wizrom.Reports.Code;

namespace WizOne
{
    public class Global : HttpApplication
    {
        static Global()
        {
            try
            {
                Dami.InitCnn();
                ReportProxy.Register("Generatoare", Constante.cnnWeb, Constante.cnnRap);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Global.asax");
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {                
                RouteTable.Routes.EnableFriendlyUrls(new FriendlyUrlSettings()
                {
                    AutoRedirectMode = RedirectMode.Permanent
                });                                                               

                string str = Constante.cnnWeb;
                if (str.ToUpper().IndexOf("INITIAL CATALOG") >=0)
                {
                    Constante.tipBD = 1;
                    int poz = str.ToUpper().IndexOf("INITIAL CATALOG");
                    int pozEnd = str.IndexOf(";", poz);
                    if (pozEnd > 0)
                        Constante.BD = str.ToUpper().Substring(poz, pozEnd - poz).Replace(" ", "").Replace("INITIALCATALOG=", "");
                    else
                        Constante.BD = str.Substring(poz);
                }
                else
                {
                    Constante.tipBD = 2;
                    int poz = str.ToUpper().IndexOf("USER ID");
                    int pozEnd = str.IndexOf(";", poz);
                    if (pozEnd > 0)
                        Constante.BD = str.ToUpper().Substring(poz, pozEnd - poz).Replace(" ","").Replace("USERID=", "");
                    else
                        Constante.BD = str.Substring(poz);
                }

                //determinam versiunea
                string cale = Server.MapPath("~/Fisiere/Config");
                if (File.Exists(cale))
                {
                    string txt = File.ReadAllText(cale);
                    Constante.versiune = "WizOne vers. " + txt;
                }

                DevExpress.Web.ASPxWebControl.CallbackError += new EventHandler(Application_Error);

                Constante.esteTactil = Convert.ToBoolean(General.Nz(ConfigurationManager.AppSettings["EsteTactil"], false));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Global.asax");
            }            
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            try
            {
                //General.InitSessionVariables();                
                //string ti = "nvarchar";
                //if (Constante.tipBD == 2) ti = "varchar2";

                //string strSql = @"SELECT ""Nume"", ""Valoare"", ""Explicatie"", ""IdModul"", ""Criptat"" FROM ""tblParametrii""
                //                UNION
                //                SELECT 'AnLucru', CAST(F01011 AS {0}(10)), '', 1, 0 FROM F010
                //                UNION
                //                SELECT 'LunaLucru', CAST(F01012 AS {0}(10)), '', 1, 0 FROM F010";
                //strSql = string.Format(strSql, ti);

                ////Session["tblParam"] = General.IncarcaDT(strSql, new string[] { Session["UserId"].ToString() });
                //Session["tblParam"] = General.IncarcaDT(strSql, null);
                //Session["IdClient"] = Convert.ToInt32(Dami.ValoareParam("IdClient","1"));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Global.asax");
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Cache.SetExpires(DateTime.Now);
                //Response.Cache.SetNoStore();
                //Response.Cache.SetMaxAge(new TimeSpan(0, 0, 30));

                //Response.AddHeader("Cache-Control", "max-age=0,no-cache,no-store,must-revalidate");
                //Response.AddHeader("Pragma", "no-cache");
                //Response.AddHeader("Expires", "Tue, 01 Jan 1970 00:00:00 GMT");                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Global.asax");
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpUnhandledException) General.MemoreazaEroarea(ex, "AppError");          
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Session.RemoveAll();
            //Session.Abandon();
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Request.Cookies[Constante.CurrentThemeCookieKey] != null)
                DevExpress.Web.ASPxWebControl.GlobalTheme = HttpUtility.UrlDecode(Request.Cookies[Constante.CurrentThemeCookieKey].Value);
            else
                DevExpress.Web.ASPxWebControl.GlobalTheme = Constante.DefaultTheme;
        } 
    }
}