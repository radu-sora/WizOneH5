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
using System.Web.Hosting;

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
                string sqlValid = " SELECT ";
                string lstCamp = "", lstCampValid = "";

                string strSql = " SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'Org_DateGenerale' ";
                if (Constante.tipBD == 2)
                    strSql = " SELECT COLUMN_NAME, DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = 'Org_DateGenerale' ";
                DataTable dtTip = General.IncarcaDT(strSql, null);

                foreach (string key in lstCtrl.Keys)
                {
                    if (key.Length > 5 && key.Substring(0, 5) == "Label")
                        continue;
                    dynamic ctl = (dynamic)div.FindControl(lstCtrl[key]);                    
                    if (ctl != null)
                    {                        
                        if (key == "StructOrg")
                        {
                            ListEditItem itm = ctl.SelectedItem;
                            if (itm != null)
                            {
                                lstCamp += ", \"Companie\" = " + itm.GetFieldValue("IdCompanie") + ", \"Subcompanie\" = " + itm.GetFieldValue("IdSubcompanie") + ", \"Filiala\" = " + itm.GetFieldValue("IdFiliala") 
                                    + ", \"Sectie\" = " + itm.GetFieldValue("IdSectie") + ", \"Dept\" = " + itm.GetFieldValue("IdDept");
                                lstCampValid += ", " + itm.GetFieldValue("IdCompanie") + " AS \"Companie\", " + itm.GetFieldValue("IdSubcompanie") + " AS \"Subcompanie\", " + itm.GetFieldValue("IdFiliala") 
                                    + " AS \"Filiala\", " + itm.GetFieldValue("IdSectie") + " AS \"Sectie\", " + itm.GetFieldValue("IdDept") + " AS \"Dept\"";
                            }
                        }
                        else
                        {
                            DataRow[] dr = dtTip.Select("COLUMN_NAME = '" + key + "'");
                            string val = "NULL";
                            switch (dr[0]["DATA_TYPE"].ToString().ToLower())
                            {
                                case "int":
                                case "numeric":
                                case "number":
                                case "decimal":
                                    if (((object)ctl).GetType() == typeof(ASPxCheckBox) || ((object)ctl).GetType() == typeof(ASPxRadioButton))
                                        val = (ctl.Value == null ? "NULL" : (ctl.Value ? "1" : "0"));
                                    else
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
                            lstCampValid += ", " + val + " AS \"" + key + "\"";
                        }
                    }
                }

                if (Session["FormDetaliu_IdFormular"].ToString() == "21")
                {
                    lstCamp += ", F10003 = " + (Session["User_Marca"] ?? "-99");
                    lstCampValid = ", " + (Session["User_Marca"] ?? "-99") + " AS F10003";
                }
                if (lstCamp.Length > 0)
                {
                    sql += lstCamp.Substring(1) + " WHERE \"Id\" = " + Session["FormDetaliu_Id"].ToString();
                    sqlValid += lstCampValid.Substring(1);
                }

                //validari
                string msg = Notif.TrimiteNotificare("Posturi.FormLista", (int)Constante.TipNotificare.Validare, sqlValid + ", " + Session["FormDetaliu_IdFormular"].ToString() + " AS \"IdFormular\", 1 AS \"Actiune\", 1 AS \"IdStareViitoare\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
                    return;
                }

                General.ExecutaNonQuery(sql, null);
                MessageBox.Show("Proces terminat cu succes!", MessageBox.icoSuccess);

                int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);
                int idUser = Convert.ToInt32(Session["UserId"] ?? -99);
                string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
                int pozitie = 0;
                if (Session["FormDetaliu_Pozitie"] != null)
                    pozitie = Convert.ToInt32(Session["FormDetaliu_Pozitie"].ToString());
                if (((Session["FormDetaliu_IdFormular"].ToString() == "21" || Session["FormDetaliu_IdFormular"].ToString() == "22") && pozitie == 0) || Session["FormDetaliu_IdFormular"].ToString() == "20")
                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        NotifAsync.TrimiteNotificare("Posturi.FormLista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Org_DateGenerale"" Z WHERE ""Id""=" + Session["FormDetaliu_Id"].ToString(), "Org_DateGenerale", Convert.ToInt32(Session["FormDetaliu_Id"].ToString()), idUser, marcaUser, arrParam);
                    });


                Response.Redirect("~/Posturi/FormLista.aspx", false);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            DataTable dt = General.IncarcaDT("SELECT \"IdRaport\" FROM \"Org_tblFormulare\" WHERE \"Id\" = " + Session["FormDetaliu_IdFormular"].ToString(), null);
            int idRap = Convert.ToInt32(General.Nz(dt.Rows[0][0] , "-99"));
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
                bool modifGen = true;

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
                    modifGen = false;
                }

                int pozitie = 0;
                if (Session["FormDetaliu_Pozitie"] != null)
                    pozitie = Convert.ToInt32(Session["FormDetaliu_Pozitie"].ToString());

                int idRol = 0;
                if (Session["FormDetaliu_IdRol"] != null)
                    idRol = Convert.ToInt32(Session["FormDetaliu_IdRol"].ToString());

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
                txt1.ReadOnly = modifGen ? false : true;
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
                    dte1.ReadOnly = modifGen ? false : true;
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
                    bool modif = modifGen;
                    if (dt.Rows[i]["PozitieCircuit"] != DBNull.Value)
                    {
                        if (Convert.ToInt32(dt.Rows[i]["PozitieCircuit"].ToString()) != idRol)  
                            modif = false;
                    }

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
                        case "7":               //StructOrg
                            ctlId = "StructOrg";
                            ASPxComboBox cmbStruct = new ASPxComboBox();
                            cmbStruct.ID = ctlId;
                            cmbStruct.ClientIDMode = ClientIDMode.Static;
                            cmbStruct.ClientInstanceName = ctlId;
                            cmbStruct.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()));
                            cmbStruct.ReadOnly = modif ? false : true;
                            cmbStruct.Style.Add("margin", "15px 15px !important");

                            DataTable dtCmbStruct = GetDepartamente();
                        
                            ListBoxColumn col = new ListBoxColumn();                            
                            col.FieldName = "Companie";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);
                            col = new ListBoxColumn();
                            col.FieldName = "IdCompanie";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);

                            col = new ListBoxColumn();
                            col.FieldName = "Subcompanie";
                            col.Caption = "Divizie";
                            cmbStruct.Columns.Add(col);
                            col = new ListBoxColumn();
                            col.FieldName = "IdSubcompanie";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);

                            col = new ListBoxColumn();
                            col.FieldName = "Filiala";
                            col.Caption = "Directie";
                            cmbStruct.Columns.Add(col);
                            col = new ListBoxColumn();
                            col.FieldName = "IdFiliala";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);

                            col = new ListBoxColumn();
                            col.FieldName = "Sectie";
                            col.Caption = "Dept";
                            cmbStruct.Columns.Add(col);
                            col = new ListBoxColumn();
                            col.FieldName = "IdSectie";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);

                            col = new ListBoxColumn();
                            col.FieldName = "Dept";
                            col.Caption = "Birou";
                            cmbStruct.Columns.Add(col);
                            col = new ListBoxColumn();
                            col.FieldName = "IdDept";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);

                            col = new ListBoxColumn();
                            col.FieldName = "IdAuto";
                            col.Visible = false;
                            cmbStruct.Columns.Add(col);

                            cmbStruct.ValueField = "IdDept";
                            cmbStruct.DataSource = dtCmbStruct;
                            cmbStruct.DataBind();

                            cell.ColSpan = 1;
                            if (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) > 150)
                            {
                                int nr = (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) / 150) + (Convert.ToInt32(General.Nz(dr["Latime"], "150").ToString()) % 150 > 0 ? 1 : 0);
                                cell.ColSpan = nr;
                            }
                            cell.Controls.Add(cmbStruct);
                            if (cell.ColSpan > 1)
                            {
                                row.Cells.Add(cell);
                                cell = new HtmlTableCell();
                            }
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
                            if (ctl.GetType() == typeof(ASPxRadioButton))
                            {
                                ctl.Checked = dt.Rows[0][dt.Columns[i].ColumnName] == DBNull.Value ? false : (Convert.ToInt32(dt.Rows[0][dt.Columns[i].ColumnName].ToString()) == 1 ? true : false);
                                string[] param = ctl.GroupName.Split('_');
                                string numeGrup = param[0] + "_" + param[1];
                                int j = 2;
                                dynamic rb = (dynamic)div.FindControl(numeGrup + "_" + j);
                                while (rb != null)
                                {
                                    rb.Checked = !ctl.Checked;
                                    j++;
                                    rb = (dynamic)div.FindControl(numeGrup + "_" + j);
                                }
                            }
                            else
                            {
                                if (ctl.GetType() == typeof(ASPxDateEdit))
                                    ctl.Value = dt.Rows[0][dt.Columns[i].ColumnName] == DBNull.Value ? new DateTime(2100, 1, 1) : Convert.ToDateTime(dt.Rows[0][dt.Columns[i].ColumnName].ToString());
                                if (ctl.GetType() == typeof(ASPxCheckBox))
                                    ctl.Checked = dt.Rows[0][dt.Columns[i].ColumnName] == DBNull.Value ? false : Convert.ToInt32(dt.Rows[0][dt.Columns[i].ColumnName].ToString()) == 1 ? true : false;
                                if (ctl.GetType() == typeof(ASPxTextBox) || ctl.GetType() == typeof(ASPxComboBox))
                                    ctl.Value = dt.Rows[0][dt.Columns[i].ColumnName] == DBNull.Value ? "" : dt.Rows[0][dt.Columns[i].ColumnName].ToString();
                            }
                        }
                    }
                }
                dynamic ctlStruct = (dynamic)div.FindControl("StructOrg");
                if (ctlStruct != null)
                {
                    ctlStruct.Value = dt.Rows[0]["Dept"] == DBNull.Value ? "" : dt.Rows[0]["Dept"].ToString();
                }


            }
       
        }


        public DataTable GetDepartamente()
        {
            DataTable q = null;

            try
            {
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                string op = "+";

                if (Constante.tipBD == 2)
                {
                    cmp = "ROWNUM";
                    op = "||";
                }

                string strSql = @"SELECT {0} as ""IdAuto"", a.F00204 AS ""Companie"",b.F00305 AS ""Subcompanie"",c.F00406 AS ""Filiala"",d.F00507 AS ""Sectie"",e.F00608 AS ""Dept"", F.F00709 AS ""Subdept"", G.F00810 AS ""Birou"",
                                a.F00202 AS ""IdCompanie"",b.F00304 AS ""IdSubcompanie"",c.F00405 AS ""IdFiliala"",d.F00506 AS ""IdSectie"",e.F00607 AS ""IdDept"", F.F00708 AS ""IdSubdept"", G.F00809 AS ""IdBirou"", e.F00607,
                                a.F00204 {1} ' / ' {1} b.F00305  {1} ' / ' {1} c.F00406 {1} ' / ' {1} d.F00507 {1} ' / ' {1} e.F00608 AS DenumireCompleta
                                FROM F002 A
                                INNER JOIN F003 B ON A.F00202 = B.F00303
                                INNER JOIN F004 C ON B.F00304 = C.F00404
                                INNER JOIN F005 D ON C.F00405 = D.F00505 
                                INNER JOIN F006 E ON D.F00506 = E.F00606
                                LEFT JOIN F007 F ON E.F00607 = F.F00707
                                LEFT JOIN F008 G ON F.F00708 = G.F00808
                                ORDER BY E.F00607";

                strSql = string.Format(strSql, cmp, op);

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }
    }
}