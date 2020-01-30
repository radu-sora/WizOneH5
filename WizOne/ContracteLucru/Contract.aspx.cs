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

namespace WizOne.ContracteLucru
{
    public partial class Contract : System.Web.UI.Page
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

                //if (!IsPostBack)
                //{
                //    IncarcaGrid();
                //    //btnNew.Attributes.Add("onclick", "window.open('Contracte.aspx', null,'height=200,width=300,left='+(window.outerWidth / 2 + window.screenX - 150)+', top=' + (window.outerHeight / 2 + window.screenY - 100));");
                //}
                //else
                //{
                //    grDate.DataSource = Session["InformatiaCurentaContracte"];
                //    grDate.KeyFieldName = "Id";
                //    grDate.DataBind();
                //}


                grDate.DataBind();

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }


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
                string sqlFinal = "SELECT * FROM \"Ptj_Contracte\" ORDER BY \"Id\"";
                DataTable dt = new DataTable();

                dt = General.IncarcaDT(sqlFinal, null);

                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;                
                Session["InformatiaCurentaContracte"] = dt;

                DataTable dtCtr = General.ListaTipContract();
                GridViewDataComboBoxColumn colCtr = (grDate.Columns["TipContract"] as GridViewDataComboBoxColumn);
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
                                string url = "~/ContracteLucru/ContractDetaliu.aspx";
                                if (url != "")
                                {
                                    Session["InformatiaCurentaContracte"] = null;
                                    Session["ContractNou"] = null;
                                    Session["IdContract"] = arr[1];
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                }
                            }
                            break;
                        case "btnDuplica":
                            DuplicareCtr(Convert.ToInt32(arr[1]));
                            grDate.DataBind();
                            break;
                        case "btnSterge":
                            ContractSterge(Convert.ToInt32(arr[1].ToString()));
                            grDate.DataBind();
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
                //if (e.DataColumn.FieldName == "Stare")
                //{
                //    object row = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                //    string culoare = row.ToString();

                //    e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(culoare);
                    
                //}
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
                string url = "~/ContracteLucru/ContractDetaliu.aspx";
                if (url != "")
                {
                    Session["IdContract"] = ContractUrmatorulId();
                    Session["ContractNou"] = "1";
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

        public void ContractSterge(int id)
        {
            try
            {
                General.ExecutaNonQuery("DELETE FROM \"Ptj_ContracteAbsente\" WHERE \"IdContract\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_ContracteCiclice\" WHERE \"IdContract\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_ContracteSchimburi\" WHERE \"IdContract\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_Contracte\" WHERE \"Id\" = " + id, null);


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        public int ContractUrmatorulId()
        {
            try
            {
                int val = 0;
                string sql = "SELECT MAX(\"Id\") FROM \"Ptj_Contracte\"";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)                
                    val = Convert.ToInt32(dt.Rows[0][0].ToString());    
                return (val + 1);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
                return 1;
            }
        }

        private void DuplicareCtr(int id)
        {    

            int idUrm = ContractUrmatorulId();

            string[] listaTabele = { "Ptj_Contracte", "Ptj_ContracteAbsente", "Ptj_ContracteCiclice", "Ptj_ContracteSchimburi"};

            foreach (string tabela in listaTabele)
            {
                DataTable dtCtr = General.IncarcaDT("SELECT * FROM \"" + tabela + "\" WHERE " + (tabela == "Ptj_Contracte" ? "\"Id\" = " : "\"IdContract\" = ") + id, null);

                string sqlCtr = "";
                if (dtCtr != null && dtCtr.Rows.Count > 0)
                {
                    sqlCtr = "INSERT INTO \"" + tabela + "\" (";
                    for (int i = 0; i < dtCtr.Columns.Count; i++)
                    {
                        if (dtCtr.Columns[i].ColumnName != "IdAuto")
                        {
                            sqlCtr += "\"" + dtCtr.Columns[i].ColumnName + "\"";
                            if (i < dtCtr.Columns.Count - 1)
                                sqlCtr += ", ";
                        }
                        else
                            if (sqlCtr.Length > 2 && i == dtCtr.Columns.Count - 1)
                                sqlCtr = sqlCtr.Substring(0, sqlCtr.Length - 2);
                    }
                    sqlCtr += ") VALUES (";
                    for (int i = 0; i < dtCtr.Columns.Count; i++)
                    {
                        if (dtCtr.Columns[i].ColumnName != "IdAuto")
                        {
                            switch (dtCtr.Columns[i].ColumnName)
                            {
                                case "Id":
                                case "IdContract":
                                    sqlCtr += idUrm;
                                    break;
                                case "Denumire":
                                    sqlCtr += "'" + dtCtr.Rows[0]["Denumire"].ToString() + " - Copie'";
                                    break;
                                case "USER_NO":
                                    sqlCtr += Convert.ToInt32(Session["UserId"].ToString());
                                    break;
                                case "TIME":
                                    sqlCtr += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                    break;
                                default:
                                    if (dtCtr.Rows[0][dtCtr.Columns[i].ColumnName] == null || dtCtr.Rows[0][dtCtr.Columns[i].ColumnName].ToString().Length <= 0)
                                        sqlCtr += "NULL";
                                    else
                                    {
                                        switch (dtCtr.Columns[i].DataType.ToString())
                                        {
                                            case "System.String":
                                                sqlCtr += "'" + dtCtr.Rows[0][dtCtr.Columns[i].ColumnName].ToString() + "'";
                                                break;
                                            case "System.DateTime":
                                                DateTime dt = Convert.ToDateTime(General.Nz(dtCtr.Rows[0][dtCtr.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                                                sqlCtr += General.ToDataUniv(dt);
                                                break;
                                            default:
                                                sqlCtr += dtCtr.Rows[0][dtCtr.Columns[i].ColumnName].ToString();
                                                break;
                                        }
                                    }
                                    break;
                            }
                            if (i < dtCtr.Columns.Count - 1)
                                sqlCtr += ", ";
                        }
                        else
                        {
                            if (sqlCtr.Length > 2 && i == dtCtr.Columns.Count - 1)
                                sqlCtr = sqlCtr.Substring(0, sqlCtr.Length - 2);
                        }          
                    }

                    sqlCtr += ")";
                }
                if (sqlCtr.Length > 0)
                    General.IncarcaDT(sqlCtr, null);
            }
        }


    }
}