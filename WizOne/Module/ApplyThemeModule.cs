using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;


namespace WizOne.Module
{
    internal class ApplyThemeModule
    {

        public ApplyThemeModule()
        {
        }

        public String ModuleName
        {
            get { return "ApplyThemeModule"; }
        }


        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += (new EventHandler(this.context_PreRequestHandlerExecute));
        }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            Page page = HttpContext.Current.CurrentHandler as Page;
            if (page != null)
            {
                page.PreInit += new EventHandler(page_PreInit);
            }
        }

        void page_PreInit(object sender, EventArgs e)
        {
            Page page = sender as Page;
            if (page != null)
            {
                string tema = (General.ExecutaScalar(@"SELECT ""Tema"" FROM ""tblConfigUsers"" WHERE F70102=@1", new string[] { General.VarSession("UserId").ToString() }) ?? Constante.DefaultTheme).ToString();

                if (tema.ToString() == "") tema = Constante.DefaultTheme;

                HttpCookie cookie = page.Request.Cookies[Constante.CurrentThemeCookieKey];
                if (cookie == null)
                {
                    cookie = new HttpCookie(Constante.CurrentThemeCookieKey);
                }

                cookie.Value = tema.ToString();
                page.Response.Cookies.Add(cookie);

                HttpCookie c = page.Request.Cookies[Constante.CurrentThemeCookieKey];
                page.Theme = c == null ? "BlackGlass" : c.Value;

            }
        }

        public void Dispose()
        {
        }

    }
}