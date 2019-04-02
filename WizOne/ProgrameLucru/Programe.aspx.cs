using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;

namespace WizOne.ProgrameLucru
{
    public partial class Programe : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
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

                txtTitlu.Text = (Session["Titlu"] ?? "").ToString();

                grDate.DataBind();


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void IncarcaGrid()
        {
            try
            {
                string sqlFinal = "SELECT * FROM \"Ptj_Programe\" ORDER BY \"Id\"";
                DataTable dt = new DataTable();

                dt = General.IncarcaDT(sqlFinal, null);

                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;     

                DataTable dtCtr = General.ListaTipPontare();
                GridViewDataComboBoxColumn colCtr = (grDate.Columns["TipPontare"] as GridViewDataComboBoxColumn);
                colCtr.PropertiesComboBox.DataSource = dtCtr;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
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
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnEdit":
                            {
                                string url = "~/ProgrameLucru/ProgrameDetaliu.aspx";
                                if (url != "")
                                {
                                    Session["InformatiaCurentaPrograme"] = null;
                                    Session["ProgramNou"] = null;
                                    Session["IdProgram"] = arr[1];
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                }
                            }
                            break;
                        case "btnSterge":
                            ProgramSterge(Convert.ToInt32(arr[1].ToString()));
                            IncarcaGrid();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "~/ProgrameLucru/ProgrameDetaliu.aspx";
                if (url != "")
                {
                    Session["IdProgram"] = ProgramUrmatorulId().ToString();
                    Session["ProgramNou"] = "1";
                    Session["InformatiaCurentaPrograme"] = null;
                    if (Page.IsCallback)
                        ASPxWebControl.RedirectOnCallback(url);
                    else
                        Response.Redirect(url, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        public void ProgramSterge(int id)
        {
            try
            {
                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgrameAlteOre\" WHERE \"IdProgram\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgramePauza\" WHERE \"IdProgram\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgrameTrepte\" WHERE \"IdProgram\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_Programe\" WHERE \"Id\" = " + id, null);


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        public int ProgramUrmatorulId()
        {
            try
            {
                int val = 0;
                string sql = "SELECT MAX(\"Id\") FROM \"Ptj_Programe\"";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows. Count > 0)
                {
                    val = Convert.ToInt32(dt.Rows[0][0].ToString());                    
                }
                return (val + 1);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
                return 1;
            }
        }



    }
}