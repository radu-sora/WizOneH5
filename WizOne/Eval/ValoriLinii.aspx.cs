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
    public partial class ValoriLinii : System.Web.UI.Page
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

                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
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
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_tblTipValori"" WHERE ""Id""=@1 ", new object[] { id });
                                if (dtHead.Rows.Count != 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["Id"].ToString();
                                    txtValoare.Text = dtHead.Rows[0]["Denumire"].ToString();
                                }
                            }
                            break;
                    }

                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_tblTipValoriLinii"" WHERE ""Id""=@1", new object[] { id });
                    Session["InformatiaCurenta"] = dt;
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataBind();
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
                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_tblTipValori";
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
                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_tblTipValori"" WHERE ""Id""=@1", new object[] { id });

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Id"",0)) FROM ""Eval_tblTipValori"" "), 0)) + 1;
                            DataRow drHead = dtHead.NewRow();
                            drHead["Id"] = id;
                            drHead["Denumire"] = txtValoare.Text;
                            drHead["TIME"] = DateTime.Now;
                            drHead["USER_NO"] = Session["UserId"];
                            dtHead.Rows.Add(drHead);

                            foreach (DataRow dr in dt.Rows)
                            {
                                dr["Id"] = id;
                            }
                        }
                        break;
                    case "Edit":
                        dtHead.Rows[0]["Denumire"] = txtValoare.Text;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                                dr["Id"] = id;
                        }
                        break;
                }

                General.SalveazaDate(dtHead, "Eval_tblTipValori");
                General.SalveazaDate(dt, "Eval_tblTipValoriLinii");

                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_tblTipValori";
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
            catch(Exception ex)
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
                DataRow row = dt.NewRow();
                
                row["Valoare"] = e.NewValues["Valoare"];
                row["Nota"] = e.NewValues["Nota"];
                row["TIME"] = DateTime.Now;
                row["USER_NO"] = Session["UserId"];

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, MessageBox.icoError, "");
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
                MessageBox.Show(ex, MessageBox.icoError, "Atentie! ");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
                e.NewValues["Id"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}