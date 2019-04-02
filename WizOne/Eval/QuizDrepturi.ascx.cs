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
    public partial class QuizDrepturi : System.Web.UI.UserControl
    {
        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        List<metaDate> lstDrepturi = new List<metaDate>();

        protected void Page_Load(object sender, EventArgs e)
        {
            grDateDrepturi.DataBind();
        }

        #region Drepturi
        protected void grDateDrepturi_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                DataRow row = ds.Tables["Eval_Drepturi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateDrepturi.CancelEdit();
                Session["InformatiaCurentaEvalQuiz"] = ds;
                grDateDrepturi.DataSource = ds.Tables["Eval_Drepturi"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDrepturi_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                object[] row = new object[ds.Tables["Eval_Drepturi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Eval_Drepturi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDQUIZ":
                                row[x] = Session["IdEvalQuiz"];
                                break;
                            case "IDAUTO":
                                row[x] = Dami.NextId("Eval_Drepturi");
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                                break;
                        }
                    }

                    x++;
                }

                ds.Tables["Eval_Drepturi"].Rows.Add(row);
                e.Cancel = true;
                grDateDrepturi.CancelEdit();
                grDateDrepturi.DataSource = ds.Tables["Eval_Drepturi"];
                grDateDrepturi.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaEvalQuiz"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDrepturi_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                DataRow row = ds.Tables["Eval_Drepturi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Eval_Drepturi"].Columns)
                {
                    if (!col.AutoIncrement && grDateDrepturi.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateDrepturi.CancelEdit();
                Session["InformatiaCurentaEvalQuiz"] = ds;
                grDateDrepturi.DataSource = ds.Tables["Eval_Drepturi"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDrepturi_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                DataTable dt = ds.Tables["Eval_Drepturi"];
                if (dt.Columns["IdAuto"] != null)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        e.NewValues["IdAuto"] = Dami.NextId("Eval_Drepturi");
                    }
                    else
                        e.NewValues["IdAuto"] = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void grDateDrepturi_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGridDrepturi();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGridDrepturi()
        {
            try
            {
                DataTable dt = new DataTable();
                string strSQL = string.Empty;
                strSQL = "select * from \"Eval_Drepturi\" where \"IdQuiz\" ={0}";
                strSQL = string.Format(strSQL, Session["IdEvalQuiz"].ToString());
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                if (ds.Tables.Contains("Eval_Drepturi"))
                {
                    dt = ds.Tables["Eval_Drepturi"];
                }
                else
                {
                    dt = General.IncarcaDT(strSQL, null);
                    dt.TableName = "Eval_Drepturi";
                    ds.Tables.Add(dt);
                }

                grDateDrepturi.KeyFieldName = "IdAuto";
                grDateDrepturi.DataSource = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion


    }
}