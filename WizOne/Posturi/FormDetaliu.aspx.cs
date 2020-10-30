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
    public partial class FormDetaliu : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];                 

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare");
                btnPrint.Text = Dami.TraduCuvant("btnPrint", "Imprima");
                #endregion

                AfiseazaCtl();
                if (Session["FormDetaliu_Id"] != null)
                    CompletareCampuri();
                else
                    btnSave.Visible = false;
     
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
                Dictionary<string, string> lstCtrl = new Dictionary<string, string>();
                lstCtrl = Session["FormDetaliu_Lista"] as Dictionary<string, string>;
                string sql = "UPDATE \"Org_DateGenerale\" SET ";
                string lstCamp = "";

                string strSql = " SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Org_DateGenerale' ";
                if (Constante.tipBD == 2)
                    strSql = " SELECT COLUMN_NAME, DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = 'Org_DateGenerale' ";
                DataTable dtTip = General.IncarcaDT(strSql, null);

                foreach (string key in lstCtrl.Keys)
                {
                    dynamic ctl = (dynamic)div.FindControl(lstCtrl[key]);
                    if (ctl != null)
                    {
                        DataRow[] dr = dtTip.Select("COLUMN_NAME = '" + key + "'");

                        string val = "NULL";
                        switch (dr[0]["DATA_TYPE"].ToString().ToLower())
                        {
                            case "int":
                            case "numeric":
                            case "number":
                            case "decimal":
                                    val = (ctl.Value ?? "NULL");
                                break;
                            case "nvarchar":
                            case "varchar":
                            case "varchar2":
                            case "nvarchar2":                                
                                    val = ctl.Value == null ? "NULL" : "'" + ctl.Value + "'";
                                break;
                            case "date":
                            case "datetime":
                                if (ctl.Value == null)
                                    val = "NULL";
                                else
                                {
                                    if (Constante.tipBD == 1)
                                        val = "CONVERT(DATETIME, '" + ctl.Value + "', 103)";
                                    else
                                        val = "TO_DATE('" + ctl.Value + "', 'dd/mm/yyyy HH24:mi:ss')";
                                }
                                break;
                        }
                        lstCamp += ", \"" + key + "\" = " + val;
                    }
                }

                if (Session["FormDetaliu_Id"].ToString() == "21")
                    lstCamp += ", F10003 = " + (Session["User_Marca"] ?? "-99");
                if (lstCamp.Length > 0)
                {
                    sql += lstCamp.Substring(1) + " WHERE \"Id\" = " + Session["FormDetaliu_Id"].ToString();
                }
                General.ExecutaNonQuery(sql, null);
                MessageBox.Show("Proces terminat cu succes!", MessageBox.icoSuccess);

                Response.Redirect("~/Posturi/FormLista.aspx", false);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            int idRap = Convert.ToInt32(Dami.ValoareParam("IdRaportFormular" + Session["FormDetaliu_IdFormular"].ToString(), "-99"));
            int id = Convert.ToInt32(Session["FormDetaliu_Id"].ToString());     

            var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(idRap);
            var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(idRap, reportSettings.ToolbarType, reportSettings.ExportOptions, new { Id = id.ToString() });

            Response.Redirect(reportUrl, false);
            //Response.RedirectLocation = System.Web.VirtualPathUtility.ToAbsolute(reportUrl);
        }

        private void AfiseazaCtl()
        {
            try
            {
                div.Controls.Clear();        

                Dictionary<string, string> lstCtrl = new Dictionary<string, string>();
                bool modif = true;

                bool poateModifica = false;
                if (Session["FormDetaliu_PoateModifica"] != null)
                    poateModifica = Convert.ToInt32(Session["FormDetaliu_PoateModifica"].ToString()) == 1 ? true : false;

                bool esteNou = false;
                if (Session["FormDetaliu_EsteNou"] != null)
                    esteNou = Convert.ToInt32(Session["FormDetaliu_EsteNou"].ToString()) == 1 ? true : false;

                int idStare = 0;
                if (Session["FormDetaliu_IdStare"] != null)
                    idStare = Convert.ToInt32(Session["FormDetaliu_IdStare"].ToString());

                if (!esteNou && (!poateModifica || (idStare != 1 && idStare != 2)))          //are doar drepturi de vizualizare
                {
                    modif = false;
                }


                HtmlTable table = new HtmlTable();
                table.CellPadding = 3;
                table.CellSpacing = 3;

                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell;

                cell = new HtmlTableCell();

                Label lbl1 = new Label();
                lbl1.Text = "Formular";
                lbl1.Style.Add("margin", "15px 15px !important");
                cell.Controls.Add(lbl1);

                ASPxTextBox txt1 = new ASPxTextBox();
                txt1.ID = "txtForm";
                txt1.ClientIDMode = ClientIDMode.Static;
                txt1.ClientInstanceName = "txtForm";
                txt1.Width = Unit.Pixel(250);
                txt1.ReadOnly = modif ? false : true;
                txt1.Style.Add("margin", "15px 15px !important");
                txt1.Text = (Session["FormDetaliu_NumeFormular"] ?? "").ToString();
                txt1.ReadOnly = true;
                cell.ColSpan = 2;
                cell.Controls.Add(txt1);
                row.Cells.Add(cell);

                if (Session["FormDetaliu_IdFormular"].ToString() != "20")
                {
                    cell = new HtmlTableCell();
                    Label lbl2 = new Label();
                    lbl2.Text = "Data vigoare";
                    lbl2.Style.Add("margin", "15px 15px !important");
                    cell.Controls.Add(lbl2);

                    ASPxDateEdit dte1 = new ASPxDateEdit();
                    dte1.ID = "deDataVig";
                    dte1.ClientIDMode = ClientIDMode.Static;
                    dte1.ClientInstanceName = "deDataVig";
                    dte1.Width = Unit.Pixel(100);
                    dte1.DisplayFormatString = "dd/MM/yyyy";
                    dte1.EditFormat = EditFormat.Custom;
                    dte1.EditFormatString = "dd/MM/yyyy";
                    dte1.ReadOnly = modif ? false : true;
                    dte1.Style.Add("margin", "15px 15px !important");
                    dte1.Value = Session["FormDetaliu_DataVigoare"] ?? DateTime.Now;
                    dte1.ReadOnly = true;
                    cell.Controls.Add(dte1);
                    row.Cells.Add(cell);
                }               

                table.Rows.Add(row);

                row = new HtmlTableRow();

                int rand = 0, poz = 0;

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Org_FormCreate"" WHERE ""IdFormular""=@1 ORDER BY ""Rand"", ""Pozitie""", new object[] { General.Nz(Session["FormDetaliu_IdFormular"], "-99") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    if (rand != Convert.ToInt32(dt.Rows[i]["Rand"].ToString()))
                    {
                        rand = Convert.ToInt32(dt.Rows[i]["Rand"].ToString());
                        poz = 0;
                        if (rand != 1)
                        {
                            table.Rows.Add(row);
                            row = new HtmlTableRow();
                        }
                    }
                    poz++;           
                    
                    cell = new HtmlTableCell();

                    Label lbl = new Label();
                    lbl.Text = Dami.TraduCuvant(dr["NumeEticheta"].ToString());
                    lbl.Style.Add("margin", "15px 15px !important");
                    cell.Controls.Add(lbl);
                    

                    string ctlId = "ctlDinamic_" + rand + "_" + poz;
                    string numeGrup = "ctlDinamic_" + rand;
                    switch (General.Nz(dr["TipControl"], "").ToString())
                    {
                        case "1":                   //TextBox
                            ASPxTextBox txt = new ASPxTextBox();
                            txt.ID = ctlId;
                            txt.ClientIDMode = ClientIDMode.Static;
                            txt.ClientInstanceName = ctlId;
                            txt.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()));
                            txt.ReadOnly = modif ? false : true;
                            txt.Style.Add("margin", "15px 15px !important");
                            cell.ColSpan = 1;
                            if (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) > 150)
                            {
                                int nr = (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) / 150) + (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) % 150 > 0 ? 1 : 0);
                                cell.ColSpan = nr;
                            }
                            cell.Controls.Add(txt);
                            if (cell.ColSpan > 1)
                            {
                                row.Cells.Add(cell);
                                cell = new HtmlTableCell();
                            }
                            break;
                        case "2":                   //DateEdit
                            ASPxDateEdit dte = new ASPxDateEdit();
                            dte.ID = ctlId;
                            dte.ClientIDMode = ClientIDMode.Static;
                            dte.ClientInstanceName = ctlId;
                            dte.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dr["Latime"], "100").ToString()));
                            dte.DisplayFormatString = "dd/MM/yyyy";
                            dte.EditFormat = EditFormat.Custom;
                            dte.EditFormatString = "dd/MM/yyyy";
                            dte.ReadOnly = modif ? false : true;
                            dte.Style.Add("margin", "15px 15px !important");
                            cell.ColSpan = 1;
                            if (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) > 150)
                            {
                                int nr = (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) / 150) + (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) % 150 > 0 ? 1 : 0);
                                cell.ColSpan = nr;
                            }
                            cell.Controls.Add(dte);
                            if (cell.ColSpan > 1)
                            {
                                row.Cells.Add(cell);
                                cell = new HtmlTableCell();
                            }
                            break;
                        case "3":                   //ComboBox
                            ASPxComboBox cmb = new ASPxComboBox();
                            cmb.ID = ctlId;
                            cmb.ClientIDMode = ClientIDMode.Static;
                            cmb.ClientInstanceName = ctlId;
                            cmb.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()));
                            cmb.ReadOnly = modif ? false : true;
                            cmb.Style.Add("margin", "15px 15px !important");
                            try
                            {
                                if (General.Nz(dr["Sursa"], "").ToString() != "")
                                {
                                    string sel = dr["Sursa"].ToString();                                   
                                    DataTable dtCmb = General.IncarcaDT(sel, null);
                                    cmb.ValueField = dtCmb.Columns[0].ColumnName;
                                    cmb.TextField = dtCmb.Columns[1].ColumnName;
                                    cmb.DataSource = dtCmb;
                                    cmb.DataBind();
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                            }
                            cell.ColSpan = 1;
                            if (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) > 150)
                            {
                                int nr = (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) / 150) + (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) % 150 > 0 ? 1 : 0);
                                cell.ColSpan = nr;
                            }
                            cell.Controls.Add(cmb);
                            if (cell.ColSpan > 1)
                            {
                                row.Cells.Add(cell);
                                cell = new HtmlTableCell();
                            }
                            break;
                        case "4":                   //RadioButton
                            ASPxRadioButton rb = new ASPxRadioButton();
                            rb.ID = ctlId;
                            rb.ClientIDMode = ClientIDMode.Static;
                            rb.ClientInstanceName = ctlId;
                            rb.Checked = false;
                            rb.ReadOnly = modif ? false : true;
                            rb.Style.Add("margin", "15px 15px !important");
                            rb.GroupName = numeGrup;
                            cell.Controls.Add(rb);
                            break;
                        case "5":                   //CheckBox
                            ASPxCheckBox chk = new ASPxCheckBox();
                            chk.ID = ctlId;
                            chk.ClientIDMode = ClientIDMode.Static;
                            chk.ClientInstanceName = ctlId;
                            chk.Checked = false;
                            chk.AllowGrayed = false;
                            chk.ReadOnly = modif ? false : true;
                            chk.Style.Add("margin", "15px 15px !important");
                            cell.Controls.Add(chk);
                            break;  
                    }
                    if (!lstCtrl.ContainsKey(dr["ColoanaBD"].ToString()))
                        lstCtrl.Add(dr["ColoanaBD"].ToString(), ctlId);
                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);

                div.Controls.Add(table);
                Session["FormDetaliu_Lista"] = lstCtrl;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void CompletareCampuri()
        {
            string sql = "SELECT * FROM \"Org_DateGenerale\" WHERE \"Id\" = " + Session["FormDetaliu_Id"].ToString();
            DataTable dt = General.IncarcaDT(sql, null);

            Dictionary<string, string> lstCtrl = new Dictionary<string, string>();
            lstCtrl = Session["FormDetaliu_Lista"] as Dictionary<string, string>;

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (lstCtrl.ContainsKey(dt.Columns[i].ColumnName))
                    {
                        dynamic ctl = (dynamic)div.FindControl(lstCtrl[dt.Columns[i].ColumnName]);
                        if (ctl != null)
                        {
                            ctl.Value = dt.Rows[0][dt.Columns[i].ColumnName].ToString();
                        }
                    }
                }
            }
       
        }
    }
}