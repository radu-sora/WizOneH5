using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class Profile : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);
                Dami.AccesAdmin();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                if (General.VarSession("EsteAdmin").ToString() == "1")
                {
                    btnEdit.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                    btnDelete.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                    btnNew.Visible = true;
                }
                else
                {
                    btnEdit.Visibility = GridViewCustomButtonVisibility.Invisible;
                    btnDelete.Visibility = GridViewCustomButtonVisibility.Invisible;
                    btnNew.Visible = false;
                }

                if (Request.UrlReferrer.ToString().IndexOf("PontajDetaliat") >= 0)
                    Session["TipPontajDetaliat"] = General.Nz(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["tip"], "").ToString();

                if (!IsPostBack)
                    IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", "window.parent.popGen.SetContentUrl('../Pagini/ProfileDetaliu.aspx?tip=New&id=-99'); window.parent.popGen.SetHeaderText('Profile - Detaliu');", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    {
                        MessageBox.Show("Insuficienti parametrii", MessageBox.icoError, "Eroare !");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
                                General.ExecutaNonQuery(@"BEGIN
                                                  DELETE FROM ""tblProfileLinii"" WHERE ""Id""=@1;
                                                  DELETE FROM ""tblProfile"" WHERE ""Id""=@1;
                                                  END;", new string[] { arr[1] });
                                IncarcaGrid();
                            }
                            break;
                    }

                    Session["Profil_DataGrid"] = "";
                    Session.Remove("Profil_DataGrid");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string filtru = "";
                if (General.Nz(Session["TipPontajDetaliat"], "").ToString() != "")
                    filtru = @" AND ""Grid"" = '" + Session["TipPontajDetaliat"] + "'";
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""tblProfile"" WHERE ""Pagina""=@1 {filtru} ORDER BY ""Denumire"" ", new string[] { General.Nz(Session["PaginaWeb"], "").ToString().Replace("\\", "."), Session["UserId"].ToString() });
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void grDate_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
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
    }
}