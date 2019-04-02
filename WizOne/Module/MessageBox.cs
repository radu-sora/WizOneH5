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
        public static string icoWarning = "warning";
        public static string icoError = "error";
        public static string icoSuccess = "success";
        public static string icoInfo = "info";


        //public static void Show(this Page pagina, string mesaj, string icoana = "info", string titlu = "", string paginaRedirect = "")
        //{
        //    try
        //    {
        //        if (mesaj == null || mesaj == "") return;
        //        string edc = Session["IdLimba"];

        //        string m = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (m != null && m != "") mesaj = m;

        //        if (titlu != "")
        //        {
        //            string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", titlu.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //            if (t != null && t != "") titlu = t;
        //        }

        //        string pag = "";
        //        if (paginaRedirect != "") pag = @"},function(){window.location = '" + paginaRedirect + "';";

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        //            " swal({ " +
        //            "     title: \"" + titlu + "\", " +
        //            "     text: \"" + mesaj + "\", " +
        //            "     type: \"" + icoana + "\" " +
        //            pag +
        //            " });</script>";

        //        //pagina introDetaliu -> dockManager_ClientLayout -> MessageBox.Show
        //        pagina.ClientScript.RegisterStartupScript(pagina.Page.GetType(), "MessageBox", txt);
        //        //ScriptManager.RegisterClientScriptBlock(pagina.Page, pagina.Page.GetType(), "MessageBox", txt, true);
        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //public static void Show(this Page pagina, Exception ex, string icoana = "info", string titlu = "")
        //{
        //    try
        //    {
        //        string mesaj = ex.Message.ToString();
        //        if (ex.InnerException != null) mesaj += ex.InnerException.ToString();

        //        if (mesaj == null || mesaj == "") return;

        //        string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (t != null && t != "") titlu = t;

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        //            " swal({ " +
        //            "     title: \"" + titlu + "\", " +
        //            "     text: \"" + mesaj + "\", " +
        //            "     type: \"" + icoana + "\" " +
        //            "      " +
        //            " });</script>";


        //        pagina.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        ////public static void Show(this Page pagina, string mesaj, string icoana = "info", string titlu = "", int edc = 1)
        ////{
        ////    try
        ////    {
        ////        if (mesaj == null || mesaj == "") return;

        ////        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        ////        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        ////            " swal({ " +
        ////            "     title: \"" + titlu + "\", " +
        ////            "     text: \"" + mesaj + "\", " +
        ////            "     type: \"" + icoana + "\" " +
        ////            "      " +
        ////            " });</script>";


        ////        pagina.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
        ////    }
        ////    catch (Exception exM)
        ////    {
        ////        General.MemoreazaEroarea(exM, "MessageBox");
        ////    }
        ////}

        //public static void Show(this MasterPage pagina, string mesaj, string icoana = "info", string titlu = "")
        //{
        //    try
        //    {
        //        if (mesaj == null || mesaj == "") return;

        //        string m = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (m != null && m != "") mesaj = m;

        //        string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (t != null && t != "") titlu = t;

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        //            " swal({ " +
        //            "     title: \"" + titlu + "\", " +
        //            "     text: \"" + mesaj + "\", " +
        //            "     type: \"" + icoana + "\" " +
        //            "      " +
        //            " });</script>";

        //        pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);

        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //public static void Show(this MasterPage pagina, Exception ex, string icoana = "info", string titlu = "")
        //{
        //    try
        //    {
        //        string mesaj = ex.Message.ToString();
        //        if (ex.InnerException != null) mesaj += ex.InnerException.ToString();

        //        if (mesaj == null || mesaj == "") return;

        //        string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (t != null && t != "") titlu = t;

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        //            " swal({ " +
        //            "     title: \"" + titlu + "\", " +
        //            "     text: \"" + mesaj + "\", " +
        //            "     type: \"" + icoana + "\" " +
        //            "      " +
        //            " });</script>";


        //        pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //public static void ShowProba(this MasterPage pagina, string mesaj, string icoana = "info", string titlu = "")
        //{
        //    try
        //    {

        //        if (mesaj == null || mesaj == "") return;

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "swal({ " +
        //                    " title: 'Sunteti sigur?', " +
        //                    " text: 'Meniul va fi sters si nu va putea fi recuperat!', " +
        //                    " type: 'warning', " +
        //                    " showCancelButton: true, " +
        //                    " confirmButtonColor: '#DD6B55', " +
        //                    " confirmButtonText: 'Da, sterge!', " +
        //                    " cancelButtonText: 'Nu, m-am razgandit!', " +
        //                    " closeOnConfirm: false, " +
        //                    " closeOnCancel: false " +
        //                    " }, " +
        //                    " function(isConfirm) " +
        //                    " { " +
        //                    "     if (isConfirm) " +
        //                    "     { " +
        //                    "         swal('Sters!', 'Meniul a fost sters', 'success'); " +
        //                    "     }" +
        //                    "     else " +
        //                    "     {" +
        //                    "         swal('Cancel', 'S-a renuntat la operatie', 'error'); " +
        //                    "     }" +
        //                    " });";


        //        pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        //window.location = "http://www.yoururl.com";

        //swal({
        //  title: "Are you sure?",
        //  text: "You will not be able to recover this imaginary file!",
        //  type: "warning",
        //  showCancelButton: true,
        //  confirmButtonColor: "#DD6B55",
        //  confirmButtonText: "Yes, delete it!",
        //  closeOnConfirm: false
        //},
        //function(){
        //  swal("Deleted!", "Your imaginary file has been deleted.", "success");
        //});




        //swal({
        //            title: "Are you sure?",
        //  text: "You will not be able to recover this imaginary file!",
        //  type: "warning",
        //  showCancelButton: true,
        //  confirmButtonColor: "#DD6B55",
        //  confirmButtonText: "Yes, delete it!",
        //  cancelButtonText: "No, cancel plx!",
        //  closeOnConfirm: false,
        //  closeOnCancel: false
        //},
        //function(isConfirm)
        //        {
        //            if (isConfirm)
        //            {
        //                swal("Deleted!", "Your imaginary file has been deleted.", "success");
        //            }
        //            else
        //            {
        //                swal("Cancelled", "Your imaginary file is safe :)", "error");
        //            }
        //        });


        //public static void Show(string mesaj, string icoana = "info", string titlu = "", string paginaRedirect = "")
        //{
        //    try
        //    {
        //        if (mesaj == null || mesaj == "") return;

        //        string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (t != null && t != "") titlu = t;

        //        string pagRed = "";
        //        if (paginaRedirect != "") pagRed = @"},function(){window.location = '" + paginaRedirect + "';";

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        //            " swal({ " +
        //            "     title: \"" + titlu + "\", " +
        //            "     text: \"" + mesaj + "\", " +
        //            "     type: \"" + icoana + "\" " +
        //            pagRed +
        //            " });</script>";

        //        Page pagina = HttpContext.Current.Handler as Page;
        //        pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //public static void Show(Exception ex, string icoana = "info", string titlu = "", string paginaRedirect = "")
        //{
        //    try
        //    {
        //        string mesaj = ex.Message.ToString();
        //        if (ex.InnerException != null) mesaj += ex.InnerException.ToString();

        //        if (mesaj == null || mesaj == "") return;

        //        string t = (string)HttpContext.GetGlobalResourceObject("Mesaje", mesaj.Replace(" ", "").Replace("!", "").Replace("/", ""));
        //        if (t != null && t != "") titlu = t;

        //        string pagRed = "";
        //        if (paginaRedirect != "") pagRed = @"},function(){window.location = '" + paginaRedirect + "';";

        //        mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
        //        string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
        //            " swal({ " +
        //            "     title: \"" + titlu + "\", " +
        //            "     text: \"" + mesaj + "\", " +
        //            "     type: \"" + icoana + "\" " +
        //            pagRed +
        //            " });</script>";

        //        Page pagina = HttpContext.Current.Handler as Page;
        //        pagina.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);
        //    }
        //    catch (Exception exM)
        //    {
        //        General.MemoreazaEroarea(exM, "MessageBox", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        public static void Show(object obj, string icoana = "info", string titlu = "", string paginaRedirect = "")
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