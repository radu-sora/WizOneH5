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
    public partial class QuizCircuit : System.Web.UI.UserControl
    {
        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }
        List<metaDate> lstCircuit = new List<metaDate>();

        protected void Page_Load(object sender, EventArgs e)
        {
            grDateCircuit.DataBind();
        }

        #region Circuit
        protected void grDateCircuit_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                DataRow row = ds.Tables["Eval_Circuit"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCircuit.CancelEdit();
                Session["InformatiaCurentaEvalQuiz"] = ds;
                grDateCircuit.DataSource = ds.Tables["Eval_Circuit"];
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCircuit_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                object[] row = new object[ds.Tables["Eval_Circuit"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Eval_Circuit"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDQUIZ":
                                row[x] = Session["IdEvalQuiz"];
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

                ds.Tables["Eval_Circuit"].Rows.Add(row);
                e.Cancel = true;
                grDateCircuit.CancelEdit();
                grDateCircuit.DataSource = ds.Tables["Eval_Circuit"];
                grDateCircuit.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaEvalQuiz"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCircuit_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                DataRow row = ds.Tables["Eval_Circuit"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Eval_Circuit"].Columns)
                {
                    if (grDateCircuit.Columns[col.ColumnName] != null)
                        if (!col.AutoIncrement && grDateCircuit.Columns[col.ColumnName].Visible)
                        {
                            var edc = e.NewValues[col.ColumnName];
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                        }

                }

                e.Cancel = true;
                grDateCircuit.CancelEdit();
                Session["InformatiaCurentaEvalQuiz"] = ds;
                grDateCircuit.DataSource = ds.Tables["Eval_Circuit"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCircuit_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["IdAuto"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void grDateCircuit_DataBinding(object sender, EventArgs e)
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
        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                string strSQL = string.Empty;
                strSQL = "select * from \"Eval_Circuit\" where \"IdQuiz\" ={0}";
                strSQL = string.Format(strSQL, Session["IdEvalQuiz"].ToString());
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                if (ds.Tables.Contains("Eval_Circuit"))
                {
                    dt = ds.Tables["Eval_Circuit"];
                }
                else
                {
                    dt = General.IncarcaDT(strSQL, null);
                    dt.TableName = "Eval_Circuit";
                    ds.Tables.Add(dt);
                }

                grDateCircuit.KeyFieldName = "IdAuto";
                grDateCircuit.DataSource = dt;

                List<metaDate> dtSuperCircuit = Session["Eval_QuizSuperCircuit"] as List<metaDate>;
                if (dtSuperCircuit != null)
                {
                    GridViewDataComboBoxColumn colSetAngajati1 = (grDateCircuit.Columns["Super1"] as GridViewDataComboBoxColumn);
                    colSetAngajati1.PropertiesComboBox.DataSource = dtSuperCircuit;

                    GridViewDataComboBoxColumn colSetAngajati2 = (grDateCircuit.Columns["Super2"] as GridViewDataComboBoxColumn);
                    colSetAngajati2.PropertiesComboBox.DataSource = dtSuperCircuit;

                    GridViewDataComboBoxColumn colSetAngajati3 = (grDateCircuit.Columns["Super3"] as GridViewDataComboBoxColumn);
                    colSetAngajati3.PropertiesComboBox.DataSource = dtSuperCircuit;

                    GridViewDataComboBoxColumn colSetAngajati4 = (grDateCircuit.Columns["Super4"] as GridViewDataComboBoxColumn);
                    colSetAngajati4.PropertiesComboBox.DataSource = dtSuperCircuit;

                    GridViewDataComboBoxColumn colSetAngajati5 = (grDateCircuit.Columns["Super5"] as GridViewDataComboBoxColumn);
                    colSetAngajati5.PropertiesComboBox.DataSource = dtSuperCircuit;
                }
                else
                {

                    strSQL = @"select -99 as ""Id"", ' ' as ""Denumire"" {0}
                                union all
                                select 0 as ""Id"", 'Angajat' as ""Denumire"" {0}
                                union all
                                select(-1) * ""Id"" as ""Id"", ""Alias"" as ""Denumire"" from ""tblSupervizori""
                                union all
                                select F70102 as ""Id"", F70104 as ""Denumire"" from USERS ";
                    if (Constante.tipBD == 2)
                        strSQL = string.Format(strSQL, " from dual");
                    else
                        strSQL = string.Format(strSQL, "");

                    DataTable dtSupervizori = new DataTable();
                    dtSupervizori = General.IncarcaDT(strSQL, null);
                    lstCircuit = new List<metaDate>();
                    if (dtSupervizori != null && dtSupervizori.Rows.Count != 0)
                    {
                        foreach (DataRow rwSetAngajat in dtSupervizori.Rows)
                        {
                            metaDate clsSetAng = new metaDate();
                            clsSetAng.Id = Convert.ToInt32(rwSetAngajat["Id"].ToString());
                            clsSetAng.Denumire = rwSetAngajat["Denumire"].ToString();
                            lstCircuit.Add(clsSetAng);
                        }
                    }
                    Session["Eval_QuizSuperCircuit"] = lstCircuit;
                    GridViewDataComboBoxColumn colSetAngajati1 = (grDateCircuit.Columns["Super1"] as GridViewDataComboBoxColumn);
                    colSetAngajati1.PropertiesComboBox.DataSource = lstCircuit;

                    GridViewDataComboBoxColumn colSetAngajati2 = (grDateCircuit.Columns["Super2"] as GridViewDataComboBoxColumn);
                    colSetAngajati2.PropertiesComboBox.DataSource = lstCircuit;

                    GridViewDataComboBoxColumn colSetAngajati3 = (grDateCircuit.Columns["Super3"] as GridViewDataComboBoxColumn);
                    colSetAngajati3.PropertiesComboBox.DataSource = lstCircuit;

                    GridViewDataComboBoxColumn colSetAngajati4 = (grDateCircuit.Columns["Super4"] as GridViewDataComboBoxColumn);
                    colSetAngajati4.PropertiesComboBox.DataSource = lstCircuit;

                    GridViewDataComboBoxColumn colSetAngajati5 = (grDateCircuit.Columns["Super5"] as GridViewDataComboBoxColumn);
                    colSetAngajati5.PropertiesComboBox.DataSource = lstCircuit;
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion

    }
}