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
    public partial class QuizAngajatGrup : System.Web.UI.UserControl
    {
        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }
        List<metaDate> lstGrupuriAngajati = new List<metaDate>();
        protected void Page_Load(object sender, EventArgs e)
        {
            grDate.DataBind();
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                DataRow row = ds.Tables["Eval_relGrupAngajatQuiz"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurentaEvalQuiz"] = ds;
                grDate.DataSource = ds.Tables["Eval_relGrupAngajatQuiz"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                object[] row = new object[ds.Tables["Eval_relGrupAngajatQuiz"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Eval_relGrupAngajatQuiz"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDQUIZ":
                                row[x] = Session["IdEvalQuiz"];
                                break;
                            //case "IDAUTO":
                            //    row[x] = Dami.NextId("Eval_relGrupAngajatQuiz");
                            //    break;
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

                ds.Tables["Eval_relGrupAngajatQuiz"].Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = ds.Tables["Eval_relGrupAngajatQuiz"];
                grDate.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaEvalQuiz"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                DataRow row = ds.Tables["Eval_relGrupAngajatQuiz"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Eval_relGrupAngajatQuiz"].Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurentaEvalQuiz"] = ds;
                grDate.DataSource = ds.Tables["Eval_relGrupAngajatQuiz"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                //DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                //DataTable dt = ds.Tables["Eval_relGrupAngajatQuiz"];
                //if (dt.Columns["IdAuto"] != null)
                //{
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        e.NewValues["IdAuto"] = Dami.NextId("Eval_relGrupAngajatQuiz");
                //    }
                //    else
                        e.NewValues["IdAuto"] = 1;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #region Methods
        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                string strSQL = string.Empty;
                strSQL = "select * from \"Eval_relGrupAngajatQuiz\" where \"IdQuiz\" ={0}";
                strSQL = string.Format(strSQL, Session["IdEvalQuiz"].ToString());
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                if(ds.Tables.Contains("Eval_relGrupAngajatQuiz"))
                {
                    dt = ds.Tables["Eval_relGrupAngajatQuiz"];
                }
                else
                {
                    dt = General.IncarcaDT(strSQL, null);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    dt.TableName = "Eval_relGrupAngajatQuiz";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }

                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;

                DataTable dtSet = Session["Eval_QuizSetAngajati"] as DataTable;
                if (dtSet == null || dtSet.Rows.Count == 0)
                {
                    dtSet = General.IncarcaDT(@"SELECT ""IdSetAng"" AS ""Id"", ""DenSet"" AS ""Denumire"" FROM ""Eval_SetAngajati"" ORDER BY ""DenSet"" ", null);
                    Session["Eval_QuizSetAngajati"] = dtSet;
                }

                GridViewDataComboBoxColumn colSetAngajati = (grDate.Columns["IdGrup"] as GridViewDataComboBoxColumn);
                colSetAngajati.PropertiesComboBox.DataSource = dtSet;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion

    }
}