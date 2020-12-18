using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class Competente : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();
                txtTitlu.Text = General.VarSession("Titlu").ToString();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");


                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");
                #endregion

                int IdCategorie = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                if (!IsPostBack)
                {
                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                //incarcare header
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_CategCompetente"" WHERE ""IdCategorie""=@1", new object[] { IdCategorie });
                                if (dtHead.Rows.Count != 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["IdCategorie"].ToString();
                                    txtCodCategorie.Text = Convert.ToString(General.Nz(dtHead.Rows[0]["CodCategorie"], ""));
                                    txtDenCategorie.Text = Convert.ToString(General.Nz(dtHead.Rows[0]["DenCategorie"], ""));
                                    txtSec.Text = Convert.ToString(General.Nz(dtHead.Rows[0]["Sectiune"],""));
                                    txtSub.Text = Convert.ToString(General.Nz(dtHead.Rows[0]["Subsectiune"], ""));
                                    cmbCal.Value = Convert.ToInt32(General.Nz(dtHead.Rows[0]["IdCalificativ"],-1));
                                }
                            }
                            break;
                    }

                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_CategCompetenteDet"" WHERE ""IdCategorie""=@1", new object[] { IdCategorie });
                    Session["InformatiaCurenta"] = dt;
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "IdCompetenta";
                    grDate.DataBind();

                    DataTable dtCal = General.IncarcaDT(@"SELECT * FROM ""Eval_tblTipValori"" ");
                    cmbCal.DataSource = dtCal;
                    cmbCal.DataBind();
                }
                else
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_CategCompetente";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_CategCompetente"" WHERE ""IdCategorie"" = @1", new object[] { id });

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""IdCategorie"",0)) FROM ""Eval_CategCompetente"" "), 0)) + 1;
                            DataRow drHead = dtHead.NewRow();
                            drHead["IdCategorie"] = id;
                            drHead["CodCategorie"] = txtCodCategorie.Text;
                            drHead["DenCategorie"] = txtDenCategorie.Text;
                            drHead["Sectiune"] = txtSec.Text;
                            drHead["Subsectiune"] = txtSub.Text;
                            drHead["IdCalificativ"] = cmbCal.Value ?? DBNull.Value;
                            drHead["TIME"] = DateTime.Now;
                            drHead["USER_NO"] = Session["UserId"];
                            dtHead.Rows.Add(drHead);

                            foreach (DataRow dr in dt.Rows)
                            {
                                dr["IdCategorie"] = id;
                            }
                        }
                        break;
                    case "Edit":
                        dtHead.Rows[0]["CodCategorie"] = txtCodCategorie.Text;
                        dtHead.Rows[0]["DenCategorie"] = txtDenCategorie.Text;
                        dtHead.Rows[0]["Sectiune"] = txtSec.Text;
                        dtHead.Rows[0]["Subsectiune"] = txtSub.Text;
                        dtHead.Rows[0]["IdCalificativ"] = cmbCal.Value ?? DBNull.Value;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                                dr["IdCategorie"] = id;
                        }
                        break;
                }

                General.SalveazaDate(dtHead, "Eval_CategCompetente");
                General.SalveazaDate(dt, "Eval_CategCompetenteDet");

                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_CategCompetente";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_AutoFilterCellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.Items.Clear();
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

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow rw = dt.NewRow();

                rw["CodCompetenta"] = e.NewValues["CodCompetenta"];
                rw["DenCompetenta"] = e.NewValues["DenCompetenta"];
                rw["Pondere"] = e.NewValues["Pondere"] ?? DBNull.Value; //Radu 29.10.2019
                rw["TIME"] = DateTime.Now;
                rw["USER_NO"] = Session["UserId"];

                dt.Rows.Add(rw);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
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

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow row = dt.Rows.Find(keys);
                DataColumn[] colPrimaryKey = dt.PrimaryKey;
                foreach (DataColumn col in dt.Columns)
                {
                    if ((!col.AutoIncrement && (cmp.IndexOf(col.ColumnName.ToUpper() + ",") < 0)) && colPrimaryKey.Where(p => p.ColumnName == col.ColumnName).Count() == 0)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
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
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
                e.NewValues["IdCompetenta"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}