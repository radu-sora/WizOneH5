using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;

namespace WizOne.Pagini
{
    public partial class Notificari : System.Web.UI.Page
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
                btnClone.Image.ToolTip = Dami.TraduCuvant("btnClone", "Duplicare");
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

                //if (!IsPostBack)
                //{
                    IncarcaGrid();
                //}
                //else
                //{
                //    grDate.DataSource = Session["InformatiaCurenta"];
                //    grDate.DataBind();
                //}
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
                //string idFrm = Request["IdForm"] ?? "-";
                //idFrm = idFrm.Replace(".aspx", "");
                //if (idFrm.ToLower().IndexOf("sablonlista") >= 0)
                //    idFrm = "tbl." + Session["Sablon_Tabela"];
                //else
                //{
                //    if (idFrm.ToLower().IndexOf("sablon") >= 0) idFrm = "tbl." + Session["NomenTableName"];
                //}


                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", "window.parent.popGen.Hide(); window.parent.popGen.SetContentUrl('Pagini/NotificariDetaliu.aspx?tip=1&id=-99'); window.parent.popGen.Show();", true);
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", "window.parent.popGen.SetContentUrl('Pagini/NotificariDetaliu.aspx?tip=1&id=-99&IdForm='" + idFrm + "); popGen.SetHeaderText('Notificari - Detaliu');", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", "window.parent.popGen.SetContentUrl('../Pagini/NotificariDetaliu.aspx?tip=New&id=-99'); popGen.SetHeaderText('Notificari - Detaliu');", true);
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
                        //case "btnEdit":
                        //case "btnClone":
                        //    {

                        //        //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY33", "window.parent.popGen.SetContentUrl('NotificariDetaliu.aspx?tip=" + arr[0].Replace("btn","") + "&id=" + arr[1] + "&IdForm=" + Request["IdForm"] + "'); popGen.SetHeaderText('Notificari - Detaliu');", true);
                        //        Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY33", "window.parent.popGen.SetContentUrl('NotificariDetaliu.aspx?tip=" + arr[0].Replace("btn", "") + "&id=" + arr[1] + "&IdForm=" + Request["IdForm"] + "'); popGen.SetHeaderText('Notificari - Detaliu');", false);
                        //        Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY33", "alert('Acasa');", true);
                        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "MessageBox", "alert('qwe');", true);
                        //    }
                        //    break;
                        case "btnDelete":
                            {
                                General.ExecutaNonQuery(@"BEGIN
                                                  DELETE FROM ""Ntf_Conditii"" WHERE ""Id""=@1;
                                                  DELETE FROM ""Ntf_Mailuri"" WHERE ""Id""=@1;
                                                  DELETE FROM ""Ntf_Setari"" WHERE ""Id""=@1;
                                                  END;", new string[] { arr[1] });
                                IncarcaGrid();
                            }
                            break;
                    }

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
                //string idFrm = Request["IdForm"] ?? "-";
                //idFrm = idFrm.Replace(".aspx", "");
                //if (idFrm.ToLower().IndexOf("sablonlista") >= 0)
                //    idFrm = "tbl." + Session["Sablon_Tabela"];
                //else
                //{
                //    if (idFrm.ToLower().IndexOf("sablon") >= 0) idFrm = "tbl." + Session["NomenTableName"];
                //}

                //DataTable dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Activ"" FROM ""Ntf_Setari"" WHERE REPLACE(""Pagina"", '.','/')=@1 AND ""TipNotificare""=1 ", new string[] { idFrm });

                string pag = Session["PaginaWeb"].ToString().Replace("\\", ".");
                if (Session["PaginaWeb"].ToString().ToLower().IndexOf("sablon") >= 0) pag = "tbl." + Session["Sablon_Tabela"].ToString();
                Session["TipNotificare"] = "1";
                if ((Request["tip"] ?? "") != "") Session["TipNotificare"] = Request["tip"].ToString();

                DataTable dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Activ"" FROM ""Ntf_Setari"" WHERE ""Pagina""=@1 AND ""TipNotificare""=@2 ", new string[] { pag, Session["TipNotificare"].ToString() });
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

    }
}