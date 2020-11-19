using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class Form1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Florin 2020.11.13
                DataTable dtTbl = General.IncarcaDT(@"SELECT * FROM ""Eval_ConfigTipTabela"" WHERE ""IdQuiz""=47 ", new object[] { Session["IdEvalQuiz"] });
                Session["Eval_ConfigTipTabela"] = dtTbl;
                DataTable dtZero = General.IncarcaDT(@"SELECT * FROM ""Eval_ConfigTipTabela"" WHERE 1=2");


                DataTable dtFiltru = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow[] arr = dtFiltru.Select("IdQuiz = 47 AND IdLInie = 1008");
                if (arr.Count() > 0)
                    dtFiltru = arr.CopyToDataTable();
                else
                    dtFiltru = dtZero;

                grDateTabela.DataSource = dtFiltru;
                grDateTabela.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow dr = dt.NewRow();

                dr["IdQuiz"] = 47;
                dr["IdLinie"] = 1008;
                dr["Coloana"] = e.NewValues["Coloana"];
                dr["Lungime"] = e.NewValues["Lungime"];
                dr["Alias"] = e.NewValues["Alias"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDateTabela.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDateTabela.DataSource = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                dr["Coloana"] = e.NewValues["Coloana"];
                dr["Lungime"] = e.NewValues["Lungime"];
                dr["Alias"] = e.NewValues["Alias"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDateTabela.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDateTabela.DataSource = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDateTabela.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["IdLinie"] = 1008;
                e.NewValues["IdQuiz"] = 47;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}