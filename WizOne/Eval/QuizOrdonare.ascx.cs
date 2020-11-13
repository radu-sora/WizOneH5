using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class QuizOrdonare : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateOrdonare_BatchUpdate(object sender, DevExpress.Web.ASPxTreeList.ASPxTreeListBatchUpdateEventArgs e)
        {
            try
            {
                grDateOrdonare.CancelEdit();

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                DataTable dt = ds.Tables["Eval_QuizIntrebari"];

                foreach (var l in e.UpdateValues)
                {
                    DataRow row = dt.Rows.Find(l.NewValues["Id"]);
                    if (row == null) continue;

                    row["OrdineAfisare"] = l.NewValues["OrdineAfisare"];
                    row["Descriere"] = l.NewValues["Descriere"];
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable table = new DataTable();
                DataTable tableIntrebari = new DataTable();
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                table = ds.Tables[0];

                if (ds.Tables.Contains("Eval_QuizIntrebari"))
                {
                    tableIntrebari = ds.Tables["Eval_QuizIntrebari"];
                }
                else
                {
                    string strSQL = " select * from \"Eval_QuizIntrebari\" where \"IdQuiz\" ={0}";
                    strSQL = string.Format(strSQL, Session["IdEvalQuiz"].ToString());
                    tableIntrebari = General.IncarcaDT(strSQL, null);
                }
                tableIntrebari.PrimaryKey = new DataColumn[] { tableIntrebari.Columns["Id"] };

                grDateOrdonare.DataSource = tableIntrebari;
                grDateOrdonare.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateOrdonare_CustomCallback(object sender, DevExpress.Web.ASPxTreeList.TreeListCustomCallbackEventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}