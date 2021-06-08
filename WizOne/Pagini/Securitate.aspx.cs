using DevExpress.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;

namespace WizOne.Pagini
{
    public partial class Securitate : System.Web.UI.Page
    {


        string cmp = "USER_NO,TIME,IDAUTO,";
        string idFrm = "-";
        string idCtl = "-";
        string idCol = "-";


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);
                Dami.AccesAdmin();



                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNewSec.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnSaveSec.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                grSec.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grSec.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");

                #endregion

                //idFrm = Request["IdForm"] ?? "-";
                idFrm = General.Nz(Session["PaginaWeb"],"-").ToString().Replace("\\",".");
                idCtl = Request["IdControl"] ?? "-";
                idCtl = idCtl.Replace(" ", "");     //Radu 22.08.2018
                idCol = Request["IdColoana"] ?? "-";
                idFrm = idFrm.Replace(".aspx", "");
                if (idFrm.ToLower().IndexOf("sablon") >= 0)
                    idFrm = "tbl." + Session["NomenTableName"];


                DataTable dtCmb = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""tblGrupUsers"" ", null);
                GridViewDataComboBoxColumn colGr = (grSec.Columns["IdGrup"] as GridViewDataComboBoxColumn);
                colGr.PropertiesComboBox.DataSource = dtCmb;

                if (!IsPostBack)
                {
                    string gridKey = "";

                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Securitate"" WHERE ""IdForm""=@1 AND ""IdControl""=@2 AND ""IdColoana""=@3", new string[] { idFrm, idCtl, idCol });

                    //determinam campurile care formeaza cheia primara
                    string strSql = @"SELECT COLUMN_NAME
                                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                                    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
                                    AND TABLE_NAME = 'Securitate'  ORDER BY ORDINAL_POSITION ";

                    if (Constante.tipBD == 2)
                    {
                        strSql = @"SELECT B.column_name
                                    FROM all_constraints A
                                    INNER JOIN all_cons_columns B ON A.constraint_name = B.constraint_name AND A.owner = B.owner
                                    WHERE A.owner = '" + Constante.BD + "' AND B.table_name = 'Securitate' AND A.constraint_type = 'P'";
                    }
                    DataTable dtKey = General.IncarcaDT(strSql, null);
                    if (dtKey.Rows.Count > 0)
                    {
                        DataColumn[] keys = new DataColumn[dtKey.Rows.Count];
                        for (int i = 0; i < dtKey.Rows.Count; i++)
                        {
                            keys[i] = dt.Columns[dtKey.Rows[i][0].ToString()];
                            gridKey += ";" + dtKey.Rows[i][0].ToString();
                        }

                        dt.PrimaryKey = keys;
                    }

                    Session["SecuritateDate"] = dt;

                    grSec.DataSource = Session["SecuritateDate"];
                    grSec.KeyFieldName = "IdForm;IdControl;IdColoana;IdGrup";
                    if (gridKey != "") grSec.KeyFieldName = gridKey.Substring(1);
                    grSec.DataBind();
                }
                else
                {
                    foreach (var c in grSec.Columns)
                    {
                        if (c.GetType().ToString() == "DevExpress.Web.GridViewDataColumn")
                        {
                            string m = (string)HttpContext.GetGlobalResourceObject("General", ((GridViewDataColumn)c).FieldName.Replace(" ", "").Replace("!", "").Replace("/", ""), new CultureInfo(Session["IdLimba"].ToString()));
                            if (m != null && m != "") ((GridViewDataColumn)c).Caption = m;
                        }
                    }

                    grSec.DataSource = Session["SecuritateDate"];
                    grSec.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grSec_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["SecuritateDate"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grSec.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grSec_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["SecuritateDate"] as DataTable;
                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToLower())
                        {
                            case "idform":
                                row[x] = idFrm;
                                break;
                            case "idcontrol":
                                row[x] = idCtl;
                                break;
                            case "idcoloana":
                                row[x] = idCol;
                                break;
                            case "idgrup":
                                row[x] = e.NewValues[col.ColumnName] ?? -1;
                                break;
                            case "vizibil":
                                row[x] = e.NewValues[col.ColumnName] ?? 1;
                                break;
                            case "blocat":
                                row[x] = e.NewValues[col.ColumnName] ?? 0;
                                break;
                            case "obligatoriu":
                                row[x] = 0;
                                break;
                            case "vizibilincolumnchooser":
                                row[x] = 1;
                                break;
                            case "idauto":
                                //row[x] = dt.AsEnumerable().Max(p => p.Field<int>("IdAuto")) + 1;
                                break;
                            case "user_no":
                                row[x] = Session["UserId"];
                                break;
                            case "time":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = col;
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grSec.CancelEdit();
                Session["SecuritateDate"] = dt;
                grSec.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grSec_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["SecuritateDate"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                foreach(DictionaryEntry col in e.NewValues)
                {
                    if (cmp.IndexOf(col.Key.ToString().ToUpper() + ",") < 0)
                    {
                        var edc = col.Key.ToString();
                        var rty = col.Value ?? DBNull.Value;
                        row[col.Key.ToString()] = col.Value ?? DBNull.Value;
                    }
                }

                e.Cancel = true;
                grSec.CancelEdit();
                Session["SecuritateDate"] = dt;
                grSec.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grSec_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataTable dt = Session["SecuritateDate"] as DataTable;

                e.NewValues["IdGrup"] = Convert.ChangeType(-1, dt.Columns["IdGrup"].DataType);
                e.NewValues["Vizibil"] = Convert.ChangeType(1, dt.Columns["Vizibil"].DataType);
                e.NewValues["Blocat"] = Convert.ChangeType(0, dt.Columns["Blocat"].DataType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSaveSec_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["SecuritateDate"] as DataTable;
                //SqlDataAdapter da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""Securitate"" ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;
                General.SalveazaDate(dt, "Securitate");

                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", "window.parent.popSec.Hide();", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", "window.parent.popGen.Hide();", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grSec_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    cmb.Items.Clear();
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.ValueType = chk.PropertiesCheckEdit.ValueType;
                        cmb.Items.Add(string.Empty, null);
                        cmb.Items.Add(Dami.TraduCuvant("BIfat"), chk.PropertiesCheckEdit.ValueChecked);
                        cmb.Items.Add(Dami.TraduCuvant("Nebifat"), chk.PropertiesCheckEdit.ValueUnchecked);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}