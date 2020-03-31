using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WizOne.Module
{
    public static class MessageBox
    {
        internal static string icoWarning = "warning";
        internal static string icoError = "error";
        internal static string icoSuccess = "success";
        internal static string icoInfo = "info";

        internal static void Show(object obj, string icoana = "info", string titlu = "", string paginaRedirect = "")
        {
            try
            {
                string mesaj = "";
                var ert = obj.GetType();

                if (obj.GetType().Name == "ArgumentException")
                {
                    Exception ex = obj as Exception;
                    mesaj = ex.Message.ToString();
                    if (ex.InnerException != null) mesaj += ex.InnerException.ToString();
                }
                else
                {
                    mesaj = obj.ToString();
                }

                if (mesaj == null || mesaj == "") return;

                string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
                if (t != null && t != "") mesaj = t;

                string pagRed = "";
                if (paginaRedirect != "") pagRed = @"},function(){window.location = '" + paginaRedirect + "';";


                titlu = Dami.TraduCuvant(titlu);
                mesaj = Dami.TraduCuvant(mesaj);


                mesaj = mesaj.Replace(System.Environment.NewLine, "\\n");
                string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
                    " swal({ " +
                    "     title: \"" + titlu + "\", " +
                    "     text: \"" + mesaj + "\", " +
                    "     type: \"" + icoana + "\" " +
                    pagRed +
                    " });</script>";

                Page pagina = HttpContext.Current.Handler as Page;
                pagina.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
            }
            catch (Exception exM)
            {
                General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



    }
}