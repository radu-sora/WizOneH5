using DevExpress.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Absente
{
    public partial class Lista : Page
    {
        bool esteHr = false;
        
        private bool IsMobileDevice
        {
            get
            {
                var userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
                var devices = new string[] { "iPhone", "iPad", "Android", "Windows Phone" }; // Add more devices

                return devices.Any(d => userAgent.Contains(d));
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {                
                (grDate.Columns["IdAbsenta"] as GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ", null);                                
                (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                (grDate.Columns["NumeInlocuitor"] as GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = General.IncarcaDT(General.SelectInlocuitori(-55, new DateTime(1900, 1, 1), new DateTime(2200, 1, 1)), null);
                (grDate.Columns["TrimiteLa"] as GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = General.IncarcaDT(
                    "SELECT Id, Denumire FROM Ptj_tblAbsente WHERE Id IN (SELECT CompensareBanca FROM Ptj_tblAbsente WHERE Compensare > 0 UNION SELECT CompensarePlata FROM Ptj_tblAbsente WHERE Compensare > 0) " +
                    "UNION " +
                   $"SELECT -13 AS Id, '{Dami.TraduCuvant("Banca")}' AS Denumire " +
                    "UNION " +
                   $"SELECT -14 AS Id, '{Dami.TraduCuvant("Plata")}' AS Denumire");                    

                grDate.JSProperties["cpParamMotiv"] = Dami.ValoareParam("AfisareMotivLaRespingereCerere","0");

                DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NrRanduriPePaginaPTJ'", null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                    grDate.SettingsPager.PageSize = Convert.ToInt32(dtParam.Rows[0][0].ToString());

                txtDtInc.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                txtDtSf.Date = new DateTime(2100,1, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);
                
                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                string sqlHr = $@"SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                DataTable dtHr = General.IncarcaDT(sqlHr, null);

                if (dtHr != null && dtHr.Rows.Count > 0) esteHr = true;

                if (!IsPostBack)
                {
                    if (Session["PaginaWeb"] as string != "Absente.Lista")
                    {
                        Session["PaginaWeb"] = "Absente.Lista";
                        Session["Filtru_CereriAbs"] = "{}";
                    }

                    #region Traducere
                    string ctlPost = Request.Params["__EVENTTARGET"];
                    if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                    btnSolNoua.Text = Dami.TraduCuvant("btnSolNoua", "Solicitare noua");
                    btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                    btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                    btnAnulare.Text = Dami.TraduCuvant("btnAnulare", "Anulare");
                    btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                    btnIstoricExtins.Text = Dami.TraduCuvant("btnIstoricExtins", "Istoric Extins");

                    btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modificare");
                    btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                    btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");
                    btnDivide.Image.ToolTip = Dami.TraduCuvant("btnDivide", "Divide");
                    btnCerere.Image.ToolTip = Dami.TraduCuvant("btnCerere", "Arata Cerere");
                    btnAtasament.Image.ToolTip = Dami.TraduCuvant("btnAtasament", "Arata Atasament");
                    btnPlanif.Image.ToolTip = Dami.TraduCuvant("btnPlanif", "Transforma in solicitat");

                    lblStare.Text = Dami.TraduCuvant("Stare");
                    lblDtInc.Text = Dami.TraduCuvant("Data Inceput");
                    lblDtSf.Text = Dami.TraduCuvant("Data Sfarsit");
                    lblAng.Text = Dami.TraduCuvant("Angajat");

                    lblViz.Text = Dami.TraduCuvant("Vizualizare");
                    lblRol.Text = Dami.TraduCuvant("Roluri");

                    btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");

                    foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                        col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                    cmbStare.SettingsAdaptivity.ModalDropDownCaption = Dami.TraduCuvant(cmbStare.SettingsAdaptivity.ModalDropDownCaption);

                    //Radu 27.11.2019                
                    var lstStare = cmbStare.FindControl("lstStare") as ASPxListBox;

                    lstStare.SelectAllText = Dami.TraduCuvant(lstStare.SelectAllText);
                    foreach (ListEditItem item in lstStare.Items)
                        item.Text = Dami.TraduCuvant(item.Text);

                    (cmbStare.FindControl("btnInchide") as ASPxButton).Text = Dami.TraduCuvant("btnInchide", "Inchide");

                    popUpDivide.HeaderText = Dami.TraduCuvant("Alege data de divizare");
                    btnOKDivide.Text = Dami.TraduCuvant("btnOKDivide", "Divide");
                    chkAnulare.Text = Dami.TraduCuvant("chkAnulare", "Anulare concediu incepand cu ziua urmatoare acestei date");

                    if (Request["pp"] != null)
                        txtTitlu.Text = Dami.TraduCuvant("Prima Pagina - Cereri");
                    else
                        txtTitlu.Text = General.VarSession("Titlu").ToString();
                    #endregion

                    string cmp = Constante.tipBD == 2 ? "FROM DUAL" : "";                    
                    DataTable dt = General.IncarcaDT($@"SELECT ""Rol"" AS ""Id"", ""RolDenumire"" AS ""Denumire"" FROM (
                            SELECT ""Rol"", ""RolDenumire"", 1 AS ""Ordin"" FROM ({Dami.SelectCereri()}) X GROUP BY ""Rol"", ""RolDenumire""
                            UNION 
                            SELECT -1 AS ""Rol"", '{Dami.TraduCuvant("Toate")}' AS ""RolDenumire"", 0 AS ""Ordin""  {cmp} ) Y WHERE Y.""Rol"" IS NOT NULL ORDER BY ""Ordin"" ", null);

                    //Radu 10.02.2020
                    chkAnulare.Checked = true;

                    //Florin 2018.10.15
                    switch (dt.Rows.Count)
                    {
                        case 0:
                            divRol.ClientVisible = false;
                            //lblRol.ClientVisible = false;
                            //cmbRol.ClientVisible = false;
                            break;
                        case 1:
                            divRol.ClientVisible = false;
                            //lblRol.ClientVisible = false;
                            //cmbRol.ClientVisible = false;
                            cmbRol.SelectedIndex = 0;
                            break;
                        case 2:
                            //lblRol.Visible = false;
                            //cmbRol.Visible = false;
                            //break;
                        default:
                            cmbRol.DataSource = dt;
                            cmbRol.DataBind();
                            cmbRol.SelectedIndex = 0;
                            break;
                    }

                    //determinam daca este angajat sau manager pentru a selecta in cmbViz
                    DataTable dtViz = General.IncarcaDT(@"SELECT * FROM ""F100Supervizori"" WHERE ""IdUser""=@1", new object[] { Session["UserId"].ToString() });
                    bool esteMng = !(dtViz == null || dtViz.Rows.Count == 0 || (dtViz.Rows.Count == 1 && General.Nz(dtViz.Rows[0]["IdSuper"], "0").ToString() == "0"));
                    string idViz = Dami.ValoareParam("Cereri_IDuriRoluriVizualizare", "-99");
                    bool esteSuper = idViz != "" && Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idViz}) GROUP BY ""IdUser"" "), 0)) > 0;

                    if (esteMng)
                        cmbViz.Items.Add(Dami.TraduCuvant("De aprobat", "De aprobat"), 1);

                    if (esteHr)
                        cmbViz.Items.Add(Dami.TraduCuvant("Toti angajatii - Rol HR", "Toti angajatii - Rol HR"), 3);
                    
                    if (esteSuper)
                        cmbViz.Items.Add(Dami.TraduCuvant("Toti angajatii - Rol Vizualizare", "Toti angajatii - Rol Vizualizare"), 4);

                    cmbViz.Items.Add(Dami.TraduCuvant("Toate cererile", "Toate cererile"), 2);
                    cmbViz.SelectedIndex = 0;                   

                    var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;

                    if ((filter as JObject).HasValues)
                    {
                        cmbViz.Value = (int)filter.viz;
                        cmbRol.Value = (int?)filter.rol;

                        var stari = filter.stare.ToObject<int[]>() as int[];

                        foreach (ListEditItem item in lstStare.Items)
                        {
                            item.Selected = stari.Contains((int)item.Value);

                            if (item.Selected)
                                cmbStare.Text += item.Text + ",";
                        }
                        cmbStare.Text = cmbStare.Text.TrimEnd(',');

                        txtDtInc.Value = (DateTime)filter.dtInc;
                        txtDtSf.Value = (DateTime)filter.dtSf;
                    }
                    else
                    {
                        Session["Filtru_CereriAbs"] = JsonConvert.SerializeObject(new
                        {
                            viz = (int)cmbViz.Value,
                            rol = divRol.ClientVisible ? (int)cmbRol.Value : null as int?,
                            stare = new int[0],
                            dtInc = txtDtInc.Value,
                            dtSf = txtDtSf.Value,
                            ang = null as int?
                        });                     
                    }

                    IncarcaCmbAng();                    

                    if (IsMobileDevice)
                        grDate.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;

                    if (!esteMng && !esteHr)
                    {
                        btnAproba.Visible = false;
                        btnRespinge.Visible = false;
                        grDate.Columns["F10003"].Visible = false;
                        grDate.Columns["NumeAngajat"].Visible = false;
                        grDate.Columns["EID"].Visible = false;
                    }

                    IncarcaGrid();
                    if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);
                }
                else if (grDate.IsCallback) {
                    IncarcaGrid();
                }                
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
                Session["grDate_Filtru"] = "Absente.Lista;" + grDate.FilterExpression;
                Session["Sablon_CheiePrimara"] = -99;
                Session["Sablon_TipActiune"] = "New";
                Response.Redirect("~/Absente/Cereri", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }                

        private string CreeazaSelect(bool forList = true)
        {
            string strSql = "";

            try
            {
                var viz = -99;
                var filtru = "";
                var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;

                if ((filter as JObject).HasValues)
                {
                    viz = filter.viz;
                    if (viz == 1) filtru = " AND A.\"Actiune\"=1 ";
                    if (((int?)filter.rol ?? 0) > 0) filtru += @" AND A.""Rol""= " + (int)filter.rol;
                    var stari = string.Join(",", filter.stare.ToObject<int[]>());
                    if (stari.Length > 0) filtru += @" AND A.""IdStare"" IN (" + stari + ")";
                    filtru += @" AND A.""DataInceput"" <= " + General.ToDataUniv((DateTime)filter.dtSf);
                    filtru += " AND " + General.ToDataUniv((DateTime)filter.dtInc) + @" <= A.""DataSfarsit"" ";
                    if (forList && filter.ang != null) filtru += @" AND B.F10003= " + (int)filter.ang;
                }                                

                strSql = Dami.SelectCereri(viz) + filtru;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }       

        private void IncarcaCmbAng()
        {
            string strSql = $@"SELECT DISTINCT X.F10003, X.""NumeAngajat"" FROM ({CreeazaSelect(false)}) X ORDER BY X.""NumeAngajat"" ";
            DataTable dt = General.IncarcaDT(strSql);
            var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;
            var selAng = (int?)filter.ang ?? -99;

            cmbAng.DataSource = dt;
            cmbAng.DataBind();
            
            if (dt.Select("F10003=" + selAng).Count() > 0) //TODO: BUG - On first page load the cmbAng selected value is not restored after returning from the request page.
                cmbAng.Value = selAng;
        }

        private void IncarcaGrid()
        {
 
            try
            {
                var strSql = CreeazaSelect() + " ORDER BY A.TIME DESC";
                var dtTemp = General.IncarcaDT(strSql, null);
                var dt = new DataTable();            
                dt.Columns.Add("CampBifa", typeof(bool));   
                dt.Load(dtTemp.CreateDataReader(), LoadOption.OverwriteChanges);
                dt.Columns["Observatii"].ReadOnly = false;
                dt.Columns["Comentarii"].ReadOnly = false;
                dt.Columns["NumeInlocuitor"].ReadOnly = false;
                dt.Columns["TrimiteLa"].ReadOnly = false;

                grDate.KeyFieldName = "Id; Rol";
                grDate.DataSource = dt;
                grDate.DataBind();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string AnulareCerere(int idCerere)
        {
            var dt = General.IncarcaDT(Dami.SelectCereri() + $"AND A.\"Id\"={idCerere}", null);

            if (dt.Rows.Count == 1)
                return AnulareCerere(idCerere, dt.Rows[0]["F10003"] as int?, dt.Rows[0]["IdAbsenta"] as int?, dt.Rows[0]["IdStare"] as int?, dt.Rows[0]["DataInceput"] as DateTime?, dt.Rows[0]["DataSfarsit"] as DateTime?, dt.Rows[0]["NrZile"] as int?, dt.Rows[0]["NrOre"] as int?, dt.Rows[0]["Anulare_Valoare"] as int?, dt.Rows[0]["Anulare_NrZile"] as int?, dt.Rows[0]["Rol"] as int?);
            else
                return Dami.TraduCuvant("Nu exista cerere selectata");
        }

        public string AnulareCerere(int idCerere, int? idAngajat, int? idAbsenta, int? idStare, DateTime? dataInceput, DateTime? dataSfarsit, int? nrZile, int? nrOre, int? anulareValoare, int? anulareNrZile, int? rol)
        {
            return AnulareCerere(idCerere, idAngajat ?? -99, idAbsenta ?? -1, idStare ?? -1, dataInceput ?? DateTime.Now.AddYears(100), dataSfarsit ?? DateTime.Now.AddYears(100), nrZile ?? 0, nrOre ?? 0, anulareValoare ?? -99, anulareNrZile ?? 0, rol ?? 0);
        }

        public string AnulareCerere(int idCerere, int idAngajat, int idAbsenta, int idStare, DateTime dataInceput, DateTime dataSfarsit, int nrZile, int nrOre, int anulareValoare, int anulareNrZile, int rol)
        {
            if (idStare == -1) //nu anulezi o cerere deja anulata
                return Dami.TraduCuvant("Cererea este deja anulata");

            if (idStare == 0)
                return Dami.TraduCuvant("Nu puteti anula o cerere respinsa");

            DataRow drAbs = General.IncarcaDR(General.SelectAbsente(idAngajat.ToString(), dataInceput.Date, idAbsenta), null);

            if (drAbs != null)
            {
                if (Convert.ToInt32(Session["IdClient"]) == 34)
                {
                    if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D1" && General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D2")
                        return Dami.TraduCuvant("Puteti anula numai cererile cu tip de absenta Delegatie");

                    if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "CO" && Convert.ToInt32(drAbs["CampBifa1"]) == 1)
                        return Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima");
                }

                if (idAngajat.ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["Anulare"], 0)) == 0 && idStare != 4)
                    return Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta");

                if (idAngajat.ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["AnulareAltii"], 0)) == 0)
                    return Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta");
            }

            //Florin 2020.06.23 - am scos conditia de mai jos
            ////daca este hr nu se aplica regulile
            //if (Convert.ToInt32(obj[10] ?? 0) != 77)
            //{
            var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;
            var selRol = (int?)filter.rol ?? 0;
            //int selRol = Convert.ToInt32(General.Nz(cmbRol?.SelectedItem?.Value, 0));
            DateTime ziDrp = Dami.DataDrepturi(anulareValoare, anulareNrZile, dataInceput, idAngajat, selRol);

            if (dataInceput.Date < ziDrp)
            {
                if (ziDrp.Year == 2111 && ziDrp.Month == 11 && ziDrp.Day == 11)
                    return Dami.TraduCuvant("Nu aveti stabilite drepturi pentru a realiza aceasta operatie");
                else
                {
                    if (ziDrp.Year == 2222 && ziDrp.Month == 12 && ziDrp.Day == 13)
                        return Dami.TraduCuvant("Pontajul a fost aprobat");
                    else
                        return Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat") + " " + ziDrp.Date.ToShortDateString();
                }
            }
            //}

            string msg = Notif.TrimiteNotificare("Absente.Lista", 2, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + idCerere, "", idCerere, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

            if (msg != "" && msg.Substring(0, 1) == "2")
                return Dami.TraduCuvant(msg.Substring(2));
            else
            {
                try
                {
                    string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""
                                                    (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                                    SELECT ""Id"", ""IdCircuit"", {-1 * rol}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""Ptj_Cereri"" WHERE ""Id""={idCerere};";
                    string sqlCer = $@"UPDATE ""Ptj_Cereri"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={idCerere};";
                    string sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={idCerere} AND ""Tabela""='Ptj_Cereri' AND ""EsteCerere"" = 1; ";

                    string sqlGen = "BEGIN " + "\n\r" +
                                            sqlIst + "\n\r" +
                                            sqlCer + "\n\r" +
                                            sqlDel + "\n\r" +
                                            "END;";
                    General.ExecutaNonQuery(sqlGen, null);
                }
                catch { }
            }

            //stergem din pontaj
            int idTipOre = 0;
            string oreInVal = "";

            if (drAbs != null)
            {
                idTipOre = Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0));
                oreInVal = General.Nz(drAbs["OreInVal"], "").ToString();
            }

            if (idStare == 3)
                General.StergeInPontaj(idCerere, idTipOre, oreInVal, dataInceput, dataSfarsit, idAngajat, nrOre, Convert.ToInt32(General.Nz(Session["UserId"], -99)));

            DataTable dtPtj = General.IncarcaDT($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003=@1 AND @2 <= ""Ziua"" AND ""Ziua"" <= @3", new object[] { idAngajat, dataInceput, dataSfarsit });

            if (dtPtj != null && dtPtj.Rows.Count > 0)
            {
                for (int i = 0; i < dtPtj.Rows.Count; i++)
                {
                    Calcul.AlocaContract(Convert.ToInt32(dtPtj.Rows[i]["F10003"].ToString()), Convert.ToDateTime(dtPtj.Rows[i]["Ziua"]));
                    Calcul.CalculInOut(dtPtj.Rows[i], true, true);
                }
            }

            General.CalculFormule(idAngajat, null, dataInceput, dataSfarsit);
            General.SituatieZLOperatii(idAngajat, dataInceput, 3, nrZile);
            Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + idCerere, "Ptj_Cereri", idCerere, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

            return null;
        }

        protected void AnulareCereri()
        {
            List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
            string msg = "";
            List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Actiune", "NumeAngajat", "DataInceput", "Rol", "IdAbsenta", "F10003", "Anulare_Valoare", "Anulare_NrZile", "DataSfarsit", "NrOre", "NrZile" });
            if (lst == null || lst.Count() == 0 || lst[0] == null) return;

            var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;
            var selRol = (int?)filter.rol ?? 0;

            for (int i = 0; i < lst.Count(); i++)
            {
                object[] arr = lst[i] as object[];
                switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                {
                    case -1:
                        msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                        continue;
                    case 0:
                        msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                        continue;
                }

                DataRow drAbs = General.IncarcaDR(General.SelectAbsente(arr[7].ToString(), Convert.ToDateTime(arr[4]).Date, Convert.ToInt32(arr[6])), null);
                if (drAbs != null)
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 34)
                    {
                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D1" && General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D2")
                        {
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Puteti anula numai cerereile cu tip de absenta Delegatie") + System.Environment.NewLine;
                            continue;
                        }

                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "CO" && Convert.ToInt32(drAbs["CampBifa1"]) == 1)
                        {
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima") + System.Environment.NewLine;
                            continue;
                        }
                    }

                    if ((arr[7] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["Anulare"], 0)) == 0 && Convert.ToInt32(General.Nz(arr[1], 0)) != 4)
                    {
                        msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta") + System.Environment.NewLine;
                        continue;
                    }

                    if ((arr[7] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["AnulareAltii"], 0)) == 0)
                    {
                        msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta") + System.Environment.NewLine;
                        continue;
                    }
                }

                //int selRol = 0;
                //if (cmbRol.SelectedItem != null) selRol = Convert.ToInt32(General.Nz(cmbRol.SelectedItem.Value, 0));
                DateTime ziDrp = Dami.DataDrepturi(Convert.ToInt32(General.Nz(arr[8], -99)), Convert.ToInt32(General.Nz(arr[9], 0)), Convert.ToDateTime(arr[4]), Convert.ToInt32(arr[7]), selRol);
                if (Convert.ToDateTime(arr[4]).Date < ziDrp)
                {
                    if (ziDrp.Year == 2111 && ziDrp.Month == 11 && ziDrp.Day == 11)
                    {
                        msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu aveti stabilite drepturi pentru a realiza aceasta operatie") + System.Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        if (ziDrp.Year == 2222 && ziDrp.Month == 12 && ziDrp.Day == 13)
                        {
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Pontajul a fost aprobat") + System.Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat") + " " + ziDrp.Date.ToShortDateString() + System.Environment.NewLine;
                            continue;
                        }
                    }
                }


                ids.Add(new Module.General.metaCereriRol
                {
                    Id = Convert.ToInt32(General.Nz(arr[0], 0)),
                    Rol = Convert.ToInt32(General.Nz(arr[5], 0)),
                    IdStare = Convert.ToInt32(General.Nz(arr[1], 0)),
                    Nume = General.Nz(arr[3], "").ToString(),
                    DataInceput = Convert.ToDateTime(General.Nz(arr[4], new DateTime(2100, 1, 1))),
                    F10003 = Convert.ToInt32(General.Nz(arr[7], 0)),
                    DataSfarsit = Convert.ToDateTime(General.Nz(arr[10], new DateTime(2100, 1, 1))),
                    NrOre = Convert.ToInt32(General.Nz(arr[11], 0)),
                    NrZile = Convert.ToInt32(General.Nz(arr[12], 0)),
                    IdAbsenta = Convert.ToInt32(General.Nz(arr[6], 0))
                });
            }


            for (int i = 0; i < ids.Count; i++)
            {
                string msgNtf = Notif.TrimiteNotificare("Absente.Lista", 2, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + ids[i].Id, "", Convert.ToInt32(ids[i].Id), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msgNtf != "" && msgNtf.Substring(0, 1) == "2")
                {
                    msg += Dami.TraduCuvant("Cererea pt") + " " + ids[i].Nume + "-" + Convert.ToDateTime(ids[i].DataInceput).ToShortDateString() + " - " + "-" + Dami.TraduCuvant(msgNtf.Substring(2)) + System.Environment.NewLine;
                    continue;
                }
                else
                {
                    try
                    {
                        string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""
                                                    (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                                    SELECT ""Id"", ""IdCircuit"", {-1 * Convert.ToInt32(General.Nz(ids[i].Rol, 0))}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""Ptj_Cereri"" WHERE ""Id""={ids[i].Id};";
                        string sqlCer = $@"UPDATE ""Ptj_Cereri"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={ids[i].Id};";
                        string sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={ids[i].Id} AND ""Tabela""='Ptj_Cereri' AND ""EsteCerere"" = 1; ";

                        string sqlGen = "BEGIN " + "\n\r" +
                                                sqlIst + "\n\r" +
                                                sqlCer + "\n\r" +
                                                sqlDel + "\n\r" +
                                                "END;";
                        General.ExecutaNonQuery(sqlGen, null);
                    }
                    catch (Exception)
                    {
                    }
                }

                //stergem din pontaj
                int idTipOre = 0;
                string oreInVal = "";
                DataRow drAbs = General.IncarcaDR(General.SelectAbsente(ids[i].F10003.ToString(), ids[i].DataInceput.Date, ids[i].IdAbsenta), null);
                if (drAbs != null)
                {
                    idTipOre = Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0));
                    oreInVal = General.Nz(drAbs["OreInVal"], "").ToString();
                }

                if (ids[i].IdStare == 3)
                    General.StergeInPontaj(Convert.ToInt32(ids[i].Id), idTipOre, oreInVal, ids[i].DataInceput, ids[i].DataSfarsit, ids[i].F10003, ids[i].NrOre, Convert.ToInt32(General.Nz(Session["UserId"], -99)));

                DataTable dtPtj = General.IncarcaDT($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003=@1 AND @2 <= ""Ziua"" AND ""Ziua"" <= @3", new object[] { ids[i].F10003, ids[i].DataInceput, ids[i].DataSfarsit });
                if (dtPtj != null && dtPtj.Rows.Count > 0)
                {
                    for (int k = 0; k < dtPtj.Rows.Count; k++)
                    {
                        Calcul.AlocaContract(Convert.ToInt32(dtPtj.Rows[k]["F10003"].ToString()), Convert.ToDateTime(dtPtj.Rows[k]["Ziua"]));
                        Calcul.CalculInOut(dtPtj.Rows[k], true, true);
                    }
                }

                General.CalculFormule(ids[i].F10003, null, ids[i].DataInceput, ids[i].DataSfarsit);
                General.SituatieZLOperatii(Convert.ToInt32(General.Nz(ids[i].F10003, -99)), ids[i].DataInceput, 3, ids[i].NrZile);
                Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + ids[i].Id, "Ptj_Cereri", Convert.ToInt32(ids[i].Id), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));


            }

            grDate.JSProperties["cpAlertMessage"] = msg;
            //grDate.DataBind();
            //grDate.Selection.UnselectAll();
        }

        protected void AprobareCereri()
        {
            try
            {
                List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Actiune", "NumeAngajat", "DataInceput", "Rol" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;
                var selViz = (int)filter.viz;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                }

                bool esteHR = selViz == 3;
                //if (Convert.ToInt32(General.Nz(cmbViz.Value, 1)) == 3) esteHR = true;
                if (ids.Count != 0) msg += General.MetodeCereri(1, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), "", esteHR);
                grDate.JSProperties["cpAlertMessage"] = msg;
                if (msg.Contains("a fost aprobata"))
                    grDate.JSProperties["cpSuccessMessage"] = "1";
                //grDate.DataBind();
                //grDate.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void RespingeCerere(string motiv)
        {
            try
            {
                List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Actiune", "NumeAngajat", "DataInceput", "Rol" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;
                var selViz = (int)filter.viz;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                }

                bool esteHR = selViz == 3;
                //if (Convert.ToInt32(General.Nz(cmbViz.Value, 1)) == 3) esteHR = true;
                if (ids.Count != 0) msg += General.MetodeCereri(2, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), motiv, esteHR);
                grDate.JSProperties["cpAlertMessage"] = msg;
                //grDate.DataBind();
                //grDate.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void cmbAng_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter.Length > 0)
                {                    
                    Session["Filtru_CereriAbs"] = e.Parameter;
                    IncarcaCmbAng();
                }
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
                if (e.Parameters.Length > 0)
                {
                    var parameters = e.Parameters.Split(';');

                    if (parameters.Length != 2 || parameters[0].Length == 0)
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    var command = parameters[0];
                    var param = parameters[1];

                    switch (command)
                    {
                        case "btnDelete":
                            {
                                #region Sterge
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "F10003", "IdAbsenta", "IdStare", "DataInceput", "NrZile", "DataSfarsit", "NrOre", "Anulare_Valoare", "Anulare_NrZile", "Rol" }) as object[];
                                if (obj == null || obj.Count() == 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata");
                                    return;
                                }

                                grDate.JSProperties["cpAlertMessage"] = AnulareCerere((int)obj[0], obj[1] as int?, obj[2] as int?, obj[3] as int?, obj[4] as DateTime?, obj[6] as DateTime?, obj[5] as int?, obj[7] as int?, obj[8] as int?, obj[9] as int?, obj[10] as int?);

                                /*int idStare = Convert.ToInt32(obj[3] ?? -1);
                                int idAbs = Convert.ToInt32(obj[2] ?? -1);

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

                                DataRow drAbs = General.IncarcaDR(General.SelectAbsente(obj[1].ToString(), Convert.ToDateTime(obj[4]).Date, Convert.ToInt32(obj[2])), null);
                                if (drAbs != null)
                                {
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Puteti anula numai cererile cu tip de absenta Delegatie");
                                    {
                                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D1" && General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D2")
                                        {
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Puteti anula numai cererile cu tip de absenta Delegatie");
                                            return;
                                        }

                                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "CO" && Convert.ToInt32(drAbs["CampBifa1"]) == 1)
                                        {
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima");
                                            return;
                                        }
                                    }

                                    if ((obj[1] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["Anulare"], 0)) == 0 && idStare != 4)
                                    {
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta");
                                        return;
                                    }

                                    if ((obj[1] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["AnulareAltii"], 0)) == 0)
                                    {
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta");
                                        return;
                                    }
                                }

                                //Florin 2020.06.23 - am scos conditia de mai jos
                                ////daca este hr nu se aplica regulile
                                //if (Convert.ToInt32(obj[10] ?? 0) != 77)
                                //{
                                int selRol = 0;
                                    if (cmbRol.SelectedItem != null) selRol = Convert.ToInt32(General.Nz(cmbRol.SelectedItem.Value, 0));
                                    DateTime ziDrp = Dami.DataDrepturi(Convert.ToInt32(General.Nz(obj[8], -99)), Convert.ToInt32(General.Nz(obj[9], 0)), Convert.ToDateTime(obj[4]), Convert.ToInt32(obj[1]), selRol);
                                    if (Convert.ToDateTime(obj[4]).Date < ziDrp)
                                    {
                                        if (ziDrp.Year == 2111 && ziDrp.Month == 11 && ziDrp.Day == 11)
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti stabilite drepturi pentru a realiza aceasta operatie");
                                        else
										{
											if (ziDrp.Year == 2222 && ziDrp.Month == 12 && ziDrp.Day == 13)
												grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pontajul a fost aprobat");                                            											
											else
												grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat") + " " + ziDrp.Date.ToShortDateString();
                                        }
                                        return;
                                    }
                                //}

                                string msg = Notif.TrimiteNotificare("Absente.Lista", 2, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                                if (msg != "" && msg.Substring(0, 1) == "2")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg.Substring(2));
                                    return;
                                }
                                else
                                {
                                    try
                                    {
                                        string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""
                                                    (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                                    SELECT ""Id"", ""IdCircuit"", {-1 * Convert.ToInt32(General.Nz(obj[10],0))}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""Ptj_Cereri"" WHERE ""Id""={obj[0]};";
                                        string sqlCer = $@"UPDATE ""Ptj_Cereri"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={obj[0]};";
                                        string sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={obj[0]} AND ""Tabela""='Ptj_Cereri' AND ""EsteCerere"" = 1; ";

                                        string sqlGen = "BEGIN " + "\n\r" +
                                                                sqlIst + "\n\r" +
                                                                sqlCer + "\n\r" +
                                                                sqlDel + "\n\r" +
                                                                "END;";
                                        General.ExecutaNonQuery(sqlGen, null);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                //stergem din pontaj
                                int idTipOre = 0;
                                string oreInVal = "";
                                if (drAbs != null)
                                {
                                    idTipOre = Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0));
                                    oreInVal = General.Nz(drAbs["OreInVal"], "").ToString();
                                }

                                if (idStare == 3)
                                    General.StergeInPontaj(Convert.ToInt32(obj[0]), idTipOre, oreInVal, Convert.ToDateTime(obj[4]), Convert.ToDateTime(obj[6]), Convert.ToInt32(obj[1]), Convert.ToInt32(General.Nz(obj[7], 0)), Convert.ToInt32(General.Nz(Session["UserId"],-99)));

                                DataTable dtPtj = General.IncarcaDT($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003=@1 AND @2 <= ""Ziua"" AND ""Ziua"" <= @3", new object[] { obj[1], obj[4], obj[6] });
                                if (dtPtj != null && dtPtj.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtPtj.Rows.Count; i++)
                                    {
                                        Calcul.AlocaContract(Convert.ToInt32(dtPtj.Rows[i]["F10003"].ToString()), Convert.ToDateTime(dtPtj.Rows[i]["Ziua"]));
                                        Calcul.CalculInOut(dtPtj.Rows[i], true, true);
                                    }
                                }

                                General.CalculFormule(obj[1], null, Convert.ToDateTime(obj[4]), Convert.ToDateTime(obj[6]));
                                General.SituatieZLOperatii(Convert.ToInt32(General.Nz(obj[1],-99)), Convert.ToDateTime(General.Nz(obj[4],new DateTime(2100,1,1))), 3, Convert.ToInt32(General.Nz(obj[5],0)));
                                Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "Ptj_Cereri", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));*/
                                
                                //grDate.DataBind();
                                #endregion 
                            }
                            break;
                        case "btnPlanif":
                            {
                                #region Planificare
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "F10003", "IdAbsenta", "Inlocuitor", "IdStare", "NrOre", "DataInceput", "DataSfarsit" }) as object[];
                                if (obj == null || obj.Count() == 0 || obj[0] == null || obj[1] == null || obj[2] == null)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata");
                                    return;
                                }

                                if (Convert.ToInt32(General.Nz(obj[4], -99)) != 4)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu este cerere planificata");
                                    return;
                                }

                                DataRow drAbs = General.IncarcaDR(General.SelectAbsente(obj[1].ToString(), Convert.ToDateTime(obj[6]).Date, Convert.ToInt32(obj[2])), null);
                                if (drAbs != null)
                                {
                                    int idCircuit = Convert.ToInt32(General.Nz(drAbs["IdCircuit"], -99));

                                    //stergem istoricul existent
                                    string sqlDel = @"DELETE FROM ""Ptj_CereriIstoric"" WHERE ""IdStare""<>4 AND ""IdCerere""=" + obj[0];

                                    //actualizam linia cu starea planificat
                                    string sqlPlf = @"UPDATE ""Ptj_CereriIstoric"" SET ""Pozitie""=0 WHERE ""IdStare""=4 AND ""IdCerere""=" + obj[0];

                                    //insetul pt introducerea istoricului
                                    string sqlIst;
                                    int trimiteLaInlocuitor;
                                    General.SelectCereriIstoric(Convert.ToInt32(obj[1]), Convert.ToInt32(General.Nz(obj[3], -1)), Convert.ToInt32(drAbs["IdCircuit"]), 0, out sqlIst, out trimiteLaInlocuitor, (int)obj[0], Convert.ToDateTime(obj[6]));


                                    //update-ul pt actualizarea datelor cererii
                                    string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""Pozitie""<>0 AND ""IdCerere""=" + obj[0] + ")";
                                    string sqlIdStare = $@"(SELECT TOP 1 ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Pozitie""<>0 AND ""Aprobat""=1 AND ""IdCerere""={obj[0]} ORDER BY ""IdAuto"" DESC)";
                                    string sqlPozitie = $@"(SELECT TOP 1 ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Pozitie""<>0 AND ""Aprobat""=1 AND ""IdCerere""={obj[0]} ORDER BY ""IdAuto"" DESC)";
                                    string sqlCuloare = $@"(SELECT TOP 1 ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Pozitie""<>0 AND ""Aprobat""=1 AND ""IdCerere""={obj[0]} ORDER BY ""IdAuto"" DESC)";

                                    string sqlUp = @"UPDATE ""Ptj_Cereri"" SET ""TotalSuperCircuit""={0}, ""IdStare""={1}, ""Pozitie""={2}, ""Culoare""={3} 
                                                    OUTPUT inserted.IdStare
                                                    WHERE ""Id""= " + obj[0];
                                    sqlUp = string.Format(sqlUp, sqlTotal, sqlIdStare, sqlPozitie, sqlCuloare);

                                    //Florin 2019.07.29
                                    //s-a adaugat si validare
                                    int idStare = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT TOP 1 ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Pozitie""<>0 AND ""Aprobat""=1 AND ""IdStare""<>4 AND ""IdCerere""={obj[0]} ORDER BY ""IdAuto"" DESC", null), 1));

                                    string msg = Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Validare, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                                    if (msg != "" && msg.Substring(0, 1) == "2")
                                    {
                                        MessageBox.Show(Dami.TraduCuvant(msg.Substring(2)));
                                        return;
                                    }
                                    else
                                    {
                                        string strGen = "BEGIN TRAN " +
                                            sqlDel + "; " +
                                            sqlPlf + "; " +
                                            sqlIst + "; " +
                                            sqlUp + "; " +
                                            "COMMIT TRAN";

                                        DataTable dtCer = General.IncarcaDT(strGen, null);

                                        //trimite in pontaj daca este finalizat
                                        if (Convert.ToInt32(dtCer.Rows[0]["IdStare"]) == 3)
                                        {
                                            if ((Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 0 && General.Nz(drAbs["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(drAbs["NuTrimiteInPontaj"], 0)) == 0)
                                                General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(General.Nz(obj[0], 1)), 5, trimiteLaInlocuitor, Convert.ToDecimal(General.Nz(obj[5], 0)));

                                            General.CalculFormule(obj[1], null, Convert.ToDateTime(obj[6]), Convert.ToDateTime(obj[7]));
                                        }

                                        Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "Ptj_Cereri", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                                        //grDate.DataBind();

                                        string strPopUp = "Proces realizat cu succes";
                                        if (msg != "")
                                            strPopUp += " " + Dami.TraduCuvant("cu urmatorul avertisment") + Environment.NewLine + msg;
                                        else
                                            grDate.JSProperties["cpSuccessMessage"] = "1";
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(strPopUp);
                                    }
                                }
                                #endregion
                            }
                            break;
                        case "btnRespinge":
                            RespingeCerere(param.Trim());                            
                            break;
                        case "btnAnulare":
                            AnulareCereri();                            
                            break;
                        case "btnAproba":                           
                            AprobareCereri();                            
                            break;
                        case "colHide":
                            grDate.Columns[param].Visible = false;
                            break;
                        case "btnFiltru":
                            Session["Filtru_CereriAbs"] = param;
                            break;
                    }

                    if (command != "colHide")
                    {
                        IncarcaGrid();
                        grDate.Selection.UnselectAll();
                        if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
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
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnOKDivide_Click(object sender, EventArgs e)
        {
            try
            {
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "F10003", "IdAbsenta", "Inlocuitor", "DataInceput", "DataSfarsit", "IdStare", "NrOre" }) as object[];
                if (obj == null || obj.Count() == 0 || obj[0] == null || obj[1] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista linie selectata"), MessageBox.icoWarning, "");
                    return;
                }


                DateTime dtSplit = Convert.ToDateTime(txtDataDivide.Value);

                //Radu 08.02.2021
                if (dtSplit == null || dtSplit.Date < new DateTime(1900, 1, 1))
                    return;

                //Radu 01.02.2021 - citire idCerere din secventa
                int idCerere = Dami.NextId("Ptj_Cereri");
                string sqlIdCerere = idCerere.ToString();
                if (idCerere == -99)
                    sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
                DateTime dtIncOri = Convert.ToDateTime(obj[4]);
                DateTime dtSfOri = Convert.ToDateTime(obj[5]);

                DateTime dtIncDes = Convert.ToDateTime(obj[4]);
                DateTime dtSfDes = Convert.ToDateTime(obj[5]);


                if (dtIncOri.Date == dtSplit.Date)
                {
                    dtSfOri = dtSplit;
                    dtIncDes = dtSplit.Date.AddDays(1);
                }
                else
                {
                    if (dtSfOri.Date == dtSplit.Date)
                    {
                        dtSfOri = dtSplit.Date.AddDays(-1);
                        dtIncDes = dtSplit.Date;
                    }
                    else
                    {
                        if (dtIncOri.Date != dtSplit.Date && dtSfOri.Date != dtSplit.Date)
                        {
                            dtSfOri = dtSplit.Date;
                            dtIncDes = dtSplit.Date.AddDays(1);
                        }
                    }
                }


                int nr = 0;
                int nrViitor = 0;
                nr = General.CalcZile(Convert.ToInt32(obj[1] ?? 0), dtIncOri, dtSfOri, Convert.ToInt32(obj[2] ?? 0));

                //Radu 10.02.2020 - daca chkAnulare este bifata, IdStare = -1
                string idStare = @"""IdStare""";
                if (chkAnulare.Checked)
                    idStare = "-1";

                string msg = "";
                if (chkAnulare.Checked)
                {  
                    msg = Notif.TrimiteNotificare("Absente.Lista", 2, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                } 
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg.Substring(2));
                    return;
                }

                string sqlOri = $@"UPDATE ""Ptj_Cereri"" SET 
                                    ""DataSfarsit"" = {General.ToDataUniv(dtSfOri)},
                                    ""IdCerereDivizata""={obj[0]},
                                    ""NrZile""={nr},
                                    ""NrZileViitor""={nrViitor},
                                    USER_NO={Session["UserId"]},
                                    TIME={General.CurrentDate()}
                                    WHERE ""Id""={obj[0]}";

                nr = 0;
                nrViitor = 0;
                nr = General.CalcZile(Convert.ToInt32(obj[1] ?? 0), dtIncDes, dtSfDes, Convert.ToInt32(obj[2] ?? 0));



                string sqlDes = $@"INSERT INTO ""Ptj_Cereri""(F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", USER_NO, TIME, ""Id"", ""TrimiteLa"", ""IdCerereDivizata"", ""Comentarii"", ""NrOre"")
                                OUTPUT Inserted.Id                                
                                SELECT F10003, ""IdAbsenta"", {General.ToDataUniv(dtIncDes)} AS""DataInceput"", {General.ToDataUniv(dtSfDes)} AS ""DataSfarsit"", {nr} AS ""NrZile"", {nrViitor} AS ""NrZileViitor"", ""Observatii"", {idStare} AS ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", {Session["UserId"]} AS USER_NO, {General.CurrentDate()} AS TIME, {sqlIdCerere} AS ""Id"", ""TrimiteLa"", {obj[0]} AS ""IdCerereDivizata"", ""Comentarii"", ""NrOre"" FROM ""Ptj_Cereri"" WHERE ""Id""={obj[0]}";

                string sqlGen = "BEGIN TRAN " + "\n\r" +
                                sqlOri + "; " + "\n\r" +
                                sqlDes + "; " + "\n\r" +
                                "COMMIT TRAN";

                DataTable dtCer = General.IncarcaDT(sqlGen, null);

                if (dtCer.Rows.Count > 0)
                {  
                    string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""(IdCerere, IdCircuit, IdSuper, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor, IdUserInlocuitor)
                                SELECT {Convert.ToInt32(dtCer.Rows[0]["Id"])} AS IdCerere, IdCircuit, IdSuper, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare, {Session["UserId"]} AS USER_NO, {General.CurrentDate()} AS TIME, Inlocuitor, IdUserInlocuitor FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere"" = {obj[0]}";
                    General.ExecutaNonQuery(sqlIst, null);

                    //Radu 08.02.2021
                    if (chkAnulare.Checked)
                    {
                        string sqlIstAnulare = $@"INSERT INTO ""Ptj_CereriIstoric""
                                                    (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                                    SELECT TOP 1 {Convert.ToInt32(dtCer.Rows[0]["Id"])}, ""IdCircuit"", ""IdSuper"", -1, ""IdUser"", 22, 1, {General.CurrentDate()}, {Session["UserId"]}, 
                                                    {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""={obj[0]};";
                        General.ExecutaNonQuery(sqlIstAnulare, null);
                    }

                 
                }

                //Radu 13.02.2020 - daca chkAnulare este bifata, trebuie sterse valorile din Pontaj
                if (chkAnulare.Checked)
                {
                    int idTipOre = 0;
                    string oreInVal = "";
                    DataRow drAbs = General.IncarcaDR(General.SelectAbsente(obj[1].ToString(), Convert.ToDateTime(obj[4]).Date, Convert.ToInt32(obj[2])), null);
                    if (drAbs != null)
                    {
                        idTipOre = Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0));
                        oreInVal = General.Nz(drAbs["OreInVal"], "").ToString();
                    }

                    General.StergeInPontaj(Convert.ToInt32(dtCer.Rows[0]["Id"]), idTipOre, oreInVal, dtIncDes, dtSfDes, Convert.ToInt32(obj[1]), Convert.ToInt32(General.Nz(obj[7], 0)), Convert.ToInt32(General.Nz(Session["UserId"], -99)));
                    General.CalculFormule(obj[1], null, dtIncDes, dtSfDes);
                }

                //stergem cererea veche din baza de date
                General.ExecutaNonQuery(@"DELETE ""tblFisiere"" WHERE ""Tabela""='Ptj_Cereri' AND ""Id""=@1", new object[] { obj[0] });
                Notif.TrimiteNotificare("Absente.Lista", 1, $@"SELECT Z.*, 6 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""IdCerereDivizata""=" + obj[0], "Ptj_Cereri", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                grDate.DataBind();
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
                var id = e.Keys["Id"];

                object cps = (grDate.FindEditFormTemplateControl("SolicitareTemplate").Controls[0] as ASPxComboBox).SelectedItem?.GetFieldValue("Id");
                object inl = (grDate.FindEditFormTemplateControl("InlocuitorTemplate").Controls[0] as ASPxComboBox).SelectedItem?.GetFieldValue("F10003");                
                
                General.ExecutaNonQuery($@"UPDATE ""Ptj_Cereri"" SET ""Observatii""=@1, ""Comentarii""=@2, ""TrimiteLa""=@3, ""Inlocuitor""=@4, ""CampBifa""=@6 WHERE ""Id""=@5", 
                    new object[] { e.NewValues["Observatii"], e.NewValues["Comentarii"], cps, inl, id, ((bool?)e.NewValues["CampBifa"] ?? false) ? 1 : 0 });

                e.Cancel = true;
                grDate.CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnIstoricExtins_Click(object sender, EventArgs e)
        {
            try
            {
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "F10003", "NumeAngajat" }) as object[];
                if (obj == null || obj.Count() == 0 || obj[0] == null || obj[1] == null)
                {
                    Session["IstoricExtins_Angajat_Marca"] = Session["User_Marca"];
                    Session["IstoricExtins_Angajat_Nume"] = Session["User_NumeComplet"];
                }
                else
                {
                    Session["IstoricExtins_Angajat_Marca"] = obj[0];
                    Session["IstoricExtins_Angajat_Nume"] = obj[1];
                }

                if (General.Nz(Session["IstoricExtins_Angajat_Marca"], "").ToString() != "" && General.Nz(Session["IstoricExtins_Angajat_Marca"], "").ToString() != "-99")
                {
                    Session["IstoricExtins_VineDin"] = 1;
                    Session["grDate_Filtru"] = "Absente.Lista;" + grDate.FilterExpression;
                    Response.Redirect("~/Absente/IstoricExtins", false);
                }
                else
                    MessageBox.Show("Nu exista angajat selectat", MessageBox.icoWarning, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        
        protected void grDate_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        {
            try
            {
                if (e.VisibleIndex == -1) return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }        

        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                var editForm = ((e.EditForm as DevExpress.Web.Rendering.GridHtmlEditFormPopupContainer).NamingContainer as ASPxPopupControl);
                var obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "NumeAngajat", "DataInceput", "Compensare", "CompensareBanca", "CompensarePlata", "AdaugaAtasament", "IdStare" }) as object[];

                if (grDate.Columns["NumeAngajat"].Visible)
                    editForm.HeaderText = $"{Dami.TraduCuvant("Modificare cerere")} {obj[0]}, {Dami.TraduCuvant("data")} {obj[1]:dd/MM/yyyy}";
                else
                    editForm.HeaderText = Dami.TraduCuvant("Modificare cerere");

                string rolHr = Dami.ValoareParam("Cereri_IDuriRoluriHR");
                string rolCmp = Dami.ValoareParam("CampBifa_IDuriRoluri");
                var filter = JObject.Parse(Session["Filtru_CereriAbs"] as string) as dynamic;
                var selRol = (int?)filter.rol ?? -99;
                var selViz = (int)filter.viz;

                //daca are rol de HR il lasam sa completeze observatii
                //if (!(rolHr.IndexOf((cmbRol.Value ?? "").ToString()) >= 0 || General.Nz(cmbViz.Value, "").ToString() == "3"))
                if (!(rolHr.IndexOf(selRol.ToString()) >= 0 || selViz == 3))
                {
                    var obsMemo = grDate.FindEditFormTemplateControl("ObservatiiTemplate").Controls[0] as ASPxMemo;

                    obsMemo.ReadOnly = true;
                    obsMemo.Enabled = false;
                }

                grDate.FindEditFormTemplateControl("InlocuitorEditContainer").Visible = Dami.ValoareParam("InlocuitorEditabilInAprobare", "0") == "1";

                if (obj[2] != null && (int)obj[2] == 1)
                {
                    var solicitareCombo = grDate.FindEditFormTemplateControl("SolicitareTemplate").Controls[0] as ASPxComboBox;

                    solicitareCombo.DataSource = (solicitareCombo.DataSource as DataTable).Select($"Id IN ({General.Nz(obj[3], (int)Constante.IdCompensareDefault.LaBanca)}, {General.Nz(obj[4], (int)Constante.IdCompensareDefault.LaPlata)})").CopyToDataTable();
                }
                else
                    grDate.FindEditFormTemplateControl("SolicitareEditContainer").Visible = false;

                grDate.FindEditFormTemplateControl("UploadEditContainer").Visible = General.Nz(obj[5], "0").ToString() == "1";
                grDate.FindEditFormTemplateControl("CampBifaEditContainer").Visible = //daca starea cererii este aprobata si rolul este cel de hr afisam campul bifa                    
                    General.Nz(obj[6], "0").ToString() == "3" && (esteHr || rolCmp.IndexOf(selRol.ToString()) >= 0);
                    //General.Nz(obj[6], "0").ToString() == "3" && (esteHr || rolCmp.IndexOf((cmbRol.Value ?? "qwerty").ToString()) >= 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
      
        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare" }) as object[];

                string sqlFis = $@"BEGIN
                            if ((SELECT COUNT(*) FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0) = 0)
                            INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {(Constante.tipBD == 1 ? "" : " FROM DUAL")}
                            ELSE
                            UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                            UPDATE ""Ptj_Cereri"" SET ""AreAtas""=1 WHERE ""Id""=@2;
                            END;";
                
                General.ExecutaNonQuery(sqlFis, new object[] { "Ptj_Cereri", obj[0], e.UploadedFile.FileBytes, e.UploadedFile.FileName, e.UploadedFile.ContentType, Session["UserId"] });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Radu 02.06.2020
        protected void grDate_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "NumarOre")
            {
                decimal nrOre = 0;
                string ore = "{0}:{1}";
                string nr = (e.GetListSourceFieldValue("NrOre") ?? "").ToString();

                if (nr.Length > 0)
                {
                    nrOre = Convert.ToDecimal(nr);

                    int x = Convert.ToInt32(Math.Truncate(nrOre));
                    decimal y = nrOre - Math.Truncate(nrOre);

                    ore = string.Format(ore, x.ToString().PadLeft(2, '0'), Convert.ToInt32(y * 60).ToString().PadLeft(2, '0'));

                    e.Value = ore;
                }
            }
        }        
    }
}