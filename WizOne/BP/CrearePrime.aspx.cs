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
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;

namespace WizOne.BP
{
    public partial class CrearePrime : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");


                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                IncarcaDate();

                if (!IsPostBack)
                {
                    Session["BP_RONNet"] = null;
                }
                else
                {
                    if (IsCallback)
                    {
                        cmbAng.DataSource = null;
                        cmbAng.Items.Clear();
                        cmbAng.DataSource = Session["BP_Angajati"];
                        cmbAng.DataBind();

                        cmbAngFiltru.DataSource = null;
                        cmbAngFiltru.Items.Clear();
                        cmbAngFiltru.DataSource = Session["BP_Angajati"];
                        cmbAngFiltru.DataBind();
                    }
                    txtRONNet.Text = (Session["BP_RONNet"] ?? "").ToString();
                }
            }
            catch (Exception ex)
            {
            
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void Iesire()
        {
            try
            {
                Response.Redirect("~/Pagini/MainPage", false);
            }
            catch (Exception ex)
            {
              
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        private void IncarcaDate()
        {

            try
            {
                cmbAn.DataSource = General.ListaNumere(2010, DateTime.Now.Year + 5);
                cmbAn.DataBind();
                cmbLuna.DataSource = General.ListaLuniDesc();
                cmbLuna.DataBind();
                cmbAnFil.DataSource = General.ListaNumere(2010, DateTime.Now.Year + 5);
                cmbAnFil.DataBind();
                cmbLunaFil.DataSource = General.ListaLuniDesc();
                cmbLunaFil.DataBind();

                cmbTip.DataSource = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"BP_tblPrime\"", null);
                cmbTip.DataBind();

                cmbMoneda.DataSource = General.IncarcaDT("SELECT \"Id\", \"Abreviere\" FROM \"tblMonede\" ORDER BY \"Id\"", null);
                cmbMoneda.DataBind();


                cmbAvsLch.DataSource = General.ListaAvansLich();
                cmbAvsLch.DataBind();


                int an = Convert.ToInt32(cmbAn.Value ?? DateTime.Now.Year);
                int luna = Convert.ToInt32(cmbLuna.Value ?? DateTime.Now.Month);


                DataTable dtAng = General.GetAngajati(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbAn.Value ?? DateTime.Now.Year), Convert.ToInt32(cmbLuna.Value ?? DateTime.Now.Month));
                cmbAng.DataSource = dtAng;
                cmbAngFiltru.DataSource = dtAng;
                Session["BP_Angajati"] = dtAng;
                cmbAng.DataBind();

                cmbAngFiltru.DataBind();


                cmbAng.Focus();


                if (!IsPostBack)
                {
                    DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

                    try
                    {
                        cmbLuna.Value = Convert.ToInt32(dt010.Rows[0][1].ToString());
                        cmbAn.Value = Convert.ToInt32(dt010.Rows[0][0].ToString());
                        cmbLunaFil.Value = Convert.ToInt32(dt010.Rows[0][1].ToString());
                        cmbAnFil.Value = Convert.ToInt32(dt010.Rows[0][0].ToString());
                    }
                    catch (Exception) { }

                    txtCurs.Value = 1;

                    try
                    {
                        cmbMoneda.SelectedIndex = 0;
                        cmbAvsLch.SelectedIndex = 1;
                    }
                    catch (Exception) { }
                    cmbAng.SelectedIndex = -1;
                    cmbAngFiltru.SelectedIndex = -1;
                }
                else
                {
                    DataTable dt = Session["BP_Prime"] as DataTable;
                    grDate.KeyFieldName = "Id";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }



            }
            catch (Exception ex)
            {
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametri insuficienti");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
                                #region Sterge
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "Angajat", "Tip" }) as object[];
                                if (obj == null || obj.Count() == 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata");
                                    return;
                                }

                                int idStare = Convert.ToInt32(obj[1] ?? -1);

                                if (idStare == -1)                //nu anulezi o cerere deja anulata
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Cererea este deja anulata");
                                    return;
                                }

                                if (idStare == 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti anula o cerere respinsa");
                                    return;
                                }
                                try
                                {
                                    string sqlIst = $@"INSERT INTO ""BP_Istoric""
                                                    (""Id"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                                    SELECT ""Id"", ""IdCircuit"", {Session["UserId"]}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""BP_Prime"" WHERE ""Id""={obj[0]};";

                                    string sqlCer = $@"UPDATE ""BP_Prime"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={obj[0]};";


                                    string sqlGen = "BEGIN " + "\n\r" +
                                                            sqlIst + "\n\r" +
                                                            sqlCer + "\n\r" +
                                                            "END;";
                                    General.ExecutaNonQuery(sqlGen, null);
                                }
                                catch (Exception)
                                {
                                }


                                grDate.DataBind();
                                #endregion 
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter.Split(';')[0];
                txtRONNet.Text = (Session["BP_RONNet"] ?? "").ToString();
                switch (tip)
                {
                    case "1":
                        
                        IncarcaDate();
                        break;
                    case "2":
                        txtRONNet.Text = (Convert.ToInt32(txtSumaNeta.Text) * Convert.ToDecimal(txtCurs.Text)).ToString();
                        Session["BP_RONNet"] = txtRONNet.Text;
                        txtExpl.Focus();
                        //IncarcaDate();
                        break;
                    case "4":
                        SalveazaDate();
                        break;
                    case "5":

                        IncarcaGrid();
                        break;
                    case "6":
                        StergeFiltre();
                        break;
                }


 
            }
            catch (Exception ex)
            {
         
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SalveazaDate()
        {
            try
            {
                //string strErr = "";
                //int idAtr = -99;


                string msg = General.AdaugaCerere( Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbAn.Value ?? -99), Convert.ToInt32(cmbLuna.Value ?? -99), Convert.ToInt32(cmbTip.Value ?? -99), 
                            Convert.ToDecimal(txtSumaNeta.Text), Convert.ToInt32(cmbMoneda.Value ?? -99), Convert.ToDecimal(txtCurs.Text.Length <= 0 ? "0" : txtCurs.Text), Convert.ToDecimal((Session["BP_RONNet"] ?? "").ToString()), Convert.ToInt32(cmbAvsLch.Value ?? -99), txtExpl.Text);

                if (msg == "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes!");
                    cmbAng.Value = null;
                    cmbTip.Value = null;
                    txtSumaNeta.Text = "";
                    txtRONNet.Text = "";
                    txtExpl.Text = "";
                }
                else
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
            }
            catch (Exception ex)
            {
              
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        



        private void IncarcaGrid()
        {
            DataTable dt = null;

            try
            {
                string strSql = "";
                string filtru = "";

                string op = "+";
                if (Constante.tipBD == 2)
                {
                    op = "||";
                }

                if (cmbAngFiltru.SelectedIndex >= 0) filtru += " AND a.F10003 = " + cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString();

                if (cmbAnFil.SelectedIndex >= 0) filtru += " AND a.\"An\" = " + cmbAnFil.Items[cmbAnFil.SelectedIndex].Value.ToString();

                if (cmbLunaFil.SelectedIndex >= 0) filtru += " AND a.\"Luna\" = " + cmbLunaFil.Items[cmbLunaFil.SelectedIndex].Value.ToString();

                if (checkComboBoxStare.Value != null) filtru += " AND a.\"IdStare\" IN (" + FiltruTipStari(checkComboBoxStare.Value.ToString()).Replace(";", ",").Substring(0, FiltruTipStari(checkComboBoxStare.Value.ToString()).Length - 1) + ")";

                strSql = "SELECT a.\"Id\", a.\"An\", b.F10008 {0} ' ' {0} b.F10009 AS \"Angajat\", CASE  WHEN A.\"Luna\" = 1 THEN 'Ianuarie' WHEN A.\"Luna\" = 2 THEN 'Februarie' WHEN A.\"Luna\" = 3 THEN 'Martie' "
                    + " WHEN A.\"Luna\" = 4 THEN 'Aprilie' WHEN A.\"Luna\" = 5 THEN 'Mai' WHEN A.\"Luna\" = 6 THEN 'Iunie' WHEN A.\"Luna\" = 7 THEN 'Iulie' WHEN A.\"Luna\" = 8 THEN 'August' "
                    + " WHEN A.\"Luna\" = 9 THEN 'Septembrie' WHEN A.\"Luna\" = 10 THEN 'Octombrie' WHEN A.\"Luna\" = 11 THEN 'Noiembrie' WHEN A.\"Luna\" = 12 THEN 'Decembrie' END AS \"LunaDesc\", a.F10003, a.\"SumaNeta\", "
                    + " \"Abreviere\" AS \"Moneda\", a.\"Curs\", a.\"TotalNet\", e.\"Denumire\" AS\"Tip\", CASE WHEN \"AvansLichidare\" = 1 THEN 'Avans' WHEN \"AvansLichidare\" = 2 THEN 'Lichidare' ELSE '' END AS \"Avans\", a.\"Explicatie\", a.\"IdStare\" "
                    + " FROM \"BP_Prime\" A "
                    + " JOIN \"BP_Istoric\" X ON A.\"Id\" = X.\"Id\" "
                    + " JOIN F100 B ON A.F10003 = B.F10003 "
                    + " LEFT JOIN \"tblMonede\" C ON A.\"IdMoneda\" = C.\"Id\" "
                    + " LEFT JOIN \"BP_tblGrupuri\" D ON A.\"IdGrup\" = D.\"Id\" "
                    + " LEFT JOIN \"BP_tblPrime\" E ON A.\"IdTip\" = E.\"Id\" "
                    + " LEFT JOIN \"BP_tblCategorii\" F ON A.\"IdCategorie\" = F.\"Id\" "
                    + " WHERE x.\"IdUser\" = {1} {2} "
                    + " GROUP BY a.\"Id\", a.\"An\", b.F10008 {0} ' ' {0} b.F10009,  a.F10003, a.\"SumaNeta\",  \"Abreviere\", a.\"Curs\", a.\"TotalNet\", e.\"Denumire\", a.\"Explicatie\", a.\"IdStare\", A.\"Luna\", \"AvansLichidare\"";
                strSql = string.Format(strSql, op, Session["USerId"].ToString(), filtru);

                
                dt = General.IncarcaDT(strSql, null);
                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["BP_Prime"] = dt;
            }
            catch (Exception)
            {
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string FiltruTipStari(string stari)
        {
            string val = "";

            try
            {
                string[] param = stari.Split(';');
                foreach (string elem in param)
                {
                    switch (elem.ToLower())
                    {
                        case "solicitat":
                            val += "1;";
                            break;
                        case "in curs":
                            val += "2;";
                            break;
                        case "aprobat":
                            val += "3;";
                            break;
                        case "respins":
                            val += "0;";
                            break;
                        case "anulat":
                            val += "-1;";
                            break;
                        case "planificat":
                            val += "4;";
                            break;
                        case "prezent":
                            val += "5;";
                            break;
                        case "absent":
                            val += "6;";
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
                    DataRow[] lst = dt.Select("Id=" + idStare);
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {                
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void StergeFiltre()
        {
            cmbAngFiltru.SelectedIndex = -1;
            cmbAngFiltru.Value = null;
            cmbAnFil.Value = null;
            cmbLunaFil.Value = null;
            checkComboBoxStare.Value = null;

            //Session["Avs_MarcaFiltru"] = null;
            //Session["Avs_AtributFiltru"] = null;

        }
        


     

     





    }


}