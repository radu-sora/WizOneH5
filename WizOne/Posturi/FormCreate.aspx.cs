using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Posturi
{
    public partial class FormCreate : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];

                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDate.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDate.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDate.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");    

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare");
                #endregion

                grDate.SettingsDataSecurity.AllowEdit = true;
                grDate.SettingsDataSecurity.AllowInsert = true;
                grDate.SettingsDataSecurity.AllowDelete = true;                

                DataTable dtform = General.IncarcaDT("SELECT * FROM \"Org_tblFormulare\"", null);
                cmbForm.DataSource = dtform;
                cmbForm.DataBind();

            

                if (!IsPostBack)
                {
                    cmbForm.SelectedIndex = -1;
                    Session["FormCreate_Save"] = 1;
                }
                else
                {
                    DataTable dt = Session["FormCreate_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                }


                DataTable dtTipCtrl = new DataTable();

                dtTipCtrl.Columns.Add("Id", typeof(int));
                dtTipCtrl.Columns.Add("Denumire", typeof(string));

                dtTipCtrl.Rows.Add(1, "TextBox");
                dtTipCtrl.Rows.Add(2, "DateEdit");
                dtTipCtrl.Rows.Add(3, "ComboBox");
                dtTipCtrl.Rows.Add(4, "RadioButton");
                dtTipCtrl.Rows.Add(5, "CheckBox");

                GridViewDataComboBoxColumn colTipCtrl = (grDate.Columns["TipControl"] as GridViewDataComboBoxColumn);
                colTipCtrl.PropertiesComboBox.DataSource = dtTipCtrl;
                string sql = "";
                if (Constante.tipBD == 1)
                    sql = " SELECT COLUMN_NAME AS Id, COLUMN_NAME AS Denumire FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Org_DateGenerale' ORDER BY Denumire";
                else
                    sql = "SELECT COLUMN_NAME AS \"Id\", COLUMN_NAME AS \"Denumire\" FROM user_tab_columns  WHERE  TABLE_NAME = 'Org_DateGenerale' ORDER BY \"Denumire\"";

                DataTable dtColBD = General.IncarcaDT(sql, null);
                GridViewDataComboBoxColumn colBD = (grDate.Columns["ColoanaBD"] as GridViewDataComboBoxColumn);
                colBD.PropertiesComboBox.DataSource = dtColBD;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }      



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter.Split(';')[0];
                switch(tip)
                {
                    case "5":
                        IncarcaGrid();
                        break;    
                }


 
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


                if (cmbForm.SelectedIndex < 0)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat niciun formular!");
                    return;
                }  

                string sql = "SELECT * FROM \"Org_FormCreate\" WHERE  \"IdFormular\"= " + cmbForm.Items[cmbForm.SelectedIndex].Value.ToString() + " ORDER BY \"Rand\", \"Pozitie\"";
                DataTable dtForm = General.IncarcaDT(sql, null);


                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dtForm;
                grDate.DataBind();
                Session["FormCreate_Grid"] = dtForm;
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

 

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

        }

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["FormCreate_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;

                if (e.NewValues["Rand"] == null || e.NewValues["Pozitie"] == null || e.NewValues["TipControl"] == null || e.NewValues["NumeEticheta"] == null || e.NewValues["ColoanaBD"] == null)                
                {
                    MessageBox.Show("Lipsesc date!", MessageBox.icoError);
                    e.Cancel = true;
                    return;
                }

                if (e.NewValues["TipControl"].ToString() == "3" && e.NewValues["Sursa"] == null)
                {
                    MessageBox.Show("Trebuie sa precizati sursa de date!", MessageBox.icoError);
                    e.Cancel = true;
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDFORMULAR":
                                row[x] = Convert.ToInt32(cmbForm.Items[cmbForm.SelectedIndex].Value.ToString());
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("Org_FormCreate");
                                break;                           
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["FormCreate_Grid"] = dt;
                Session["FormCreate_Save"] = null;
            }
            catch (Exception ex)
            {
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

                DataTable dt = Session["FormCreate_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                if (e.NewValues["Rand"] == null || e.NewValues["Pozitie"] == null || e.NewValues["TipControl"] == null || e.NewValues["NumeEticheta"] == null || e.NewValues["ColoanaBD"] == null)
                {
                    MessageBox.Show("Lipsesc date!", MessageBox.icoError);
                    e.Cancel = true;
                    return;
                }

                if (e.NewValues["TipControl"].ToString() == "3" && e.NewValues["Sursa"] == null)
                {
                    MessageBox.Show("Trebuie sa precizati sursa de date!", MessageBox.icoError);
                    e.Cancel = true;
                    return;
                }


                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["FormCreate_Grid"] = dt;
                grDate.DataSource = dt;
                Session["FormCreate_Save"] = null;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["FormCreate_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["FormCreate_Grid"] = dt;
                grDate.DataSource = dt;
                Session["FormCreate_Save"] = null;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["FormCreate_Grid"] as DataTable;
                General.SalveazaDate(dt, "Org_FormCreate");
                Session["FormCreate_Save"] = 1;
                MessageBox.Show("Proces terminat cu succes!", MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPreviz_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["FormCreate_Save"] == null)
                {
                    MessageBox.Show("Nu ati salvat modificarile!", MessageBox.icoError);
                    return;
                }

                string url = "~/Posturi/FormDetaliu.aspx";
                Session["FormDetaliu_Id"] = null;
                Session["FormDetaliu_IdFormular"] = Convert.ToInt32(cmbForm.Items[cmbForm.SelectedIndex].Value.ToString());
                Session["FormDetaliu_IdStare"] = 0;
                Session["FormDetaliu_PoateModifica"] = 1;
                Session["FormDetaliu_EsteNou"] = 1;

                Session["FormDetaliu_NumeFormular"] = cmbForm.Text;
                Session["FormDetaliu_DataVigoare"] = DateTime.Now;

                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}