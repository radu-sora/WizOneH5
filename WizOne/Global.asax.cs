using Microsoft.AspNet.FriendlyUrls;
using Microsoft.AspNet.FriendlyUrls.Resolvers;
using Microsoft.Web.Administration;
using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
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

        protected void Application_PreSendRequestHeaders()
        {
            try
            {
                //Florin 2021.05.31 - pct 14
                if (Request.IsSecureConnection)
                {
                    Response.Cookies.Add(
                        new HttpCookie("key", "value")
                        {
                            Secure = true,
                        });
                }
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
                //ServerManager serverManager = new ServerManager();
                //ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
                //foreach (ApplicationPool applicationPool in applicationPoolCollection)
                //{
                //    applicationPool.ProcessModel.IdleTimeout = TimeSpan.FromMinutes(5);
                //}

                RouteTable.Routes.EnableFriendlyUrls(new FriendlyUrlSettings()
                {
                    AutoRedirectMode = RedirectMode.Permanent
                }, new WebFormsFriendlyUrlResolver(), new GenericHandlerFriendlyUrlResolver()/*, new FriendlyUrlResolver(".axd")*/);

                string str = Constante.cnnWeb;
                if (str.ToUpper().IndexOf("INITIAL CATALOG") >=0)
                {
                    Constante.tipBD = 1;
                    int poz = str.ToUpper().IndexOf("INITIAL CATALOG", StringComparison.OrdinalIgnoreCase);     
                    int pozEnd = str.IndexOf(";", poz);
                    if (pozEnd > 0)
                        //Constante.BD = str.Substring(poz, pozEnd - poz).Replace(" ", "").Replace("INITIALCATALOG=", "");  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
                        Constante.BD = Regex.Replace(str.Substring(poz, pozEnd - poz).Replace(" ", ""), "INITIALCATALOG=", "", RegexOptions.IgnoreCase);
                    else
                        //Constante.BD = str.Substring(poz).Replace(" ", "").Replace("INITIALCATALOG=", "");    //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
                        Constante.BD = Regex.Replace(str.Substring(poz).Replace(" ", ""), "INITIALCATALOG=", "", RegexOptions.IgnoreCase);
                }
                else
                {
                    Constante.tipBD = 2;
                    int poz = str.ToUpper().IndexOf("USER ID", StringComparison.OrdinalIgnoreCase);
                    int pozEnd = str.IndexOf(";", poz);
                    if (pozEnd > 0)
                        //Constante.BD = str.Substring(poz, pozEnd - poz).Replace(" ","").Replace("USERID=", "");   //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
                        Constante.BD = Regex.Replace(str.Substring(poz, pozEnd - poz).Replace(" ", ""), "USERID=", "", RegexOptions.IgnoreCase);
                    else
                        //Constante.BD = str.Substring(poz).Replace(" ", "").Replace("USERID=", "");    //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
                        Constante.BD = Regex.Replace(str.Substring(poz).Replace(" ", ""), "USERID=", "", RegexOptions.IgnoreCase);
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

                //Radu 23.04.2020
                Constante.idTerminal = Convert.ToString(General.Nz(ConfigurationManager.AppSettings["IdTerminal"], false));
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
                //Florin 2021.05.31 #909 pct 18
                if (Request.IsSecureConnection)
                    HttpContext.Current.Response.AddHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");            
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Global.asax");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpUnhandledException) General.MemoreazaEroarea(ex, "AppError");
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