using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Absente
{
    public partial class Lista : System.Web.UI.Page
    {

        bool esteHr = false;

        internal class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "Absente.Lista";
                Session["Absente_Cereri_Date"] = null;

                DataTable dtAbs = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ", null);
                GridViewDataComboBoxColumn colAbs = (grDate.Columns["IdAbsenta"] as GridViewDataComboBoxColumn);
                colAbs.PropertiesComboBox.DataSource = dtAbs;

                DataTable dtStari = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnSolNoua.Text = Dami.TraduCuvant("btnSolNoua", "Solicitare noua");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnIstoricExtins.Text = Dami.TraduCuvant("btnIstoricExtins", "Istoric Extins");

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");
                btnDivide.Image.ToolTip = Dami.TraduCuvant("btnDivide", "Divide");
                btnCerere.Image.ToolTip = Dami.TraduCuvant("btnCerere", "Arata Cerere");
                btnAtasament.Image.ToolTip = Dami.TraduCuvant("btnAtasament", "Arata Atasament");
                btnPlanif.Image.ToolTip = Dami.TraduCuvant("btnPlanif", "Transforma in solicitat");

                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblDtInc.InnerText = Dami.TraduCuvant("Data Inceput");
                lblDtSf.InnerText = Dami.TraduCuvant("Data Sfarsit");

                lblViz.InnerText = Dami.TraduCuvant("Vizualizare");
                lblRol.InnerText = Dami.TraduCuvant("Roluri");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

                        //var ert = c.GetType();
                        //if (c.GetType() == typeof(GridViewDataColumn))
                        //{
                        //    GridViewDataColumn col = c as GridViewDataColumn;
                        //    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        //}
                    }
                    catch (Exception) { }
                }

                //Radu 27.11.2019             
                ASPxListBox nestedListBox = cmbStare.FindControl("listBoxStare") as ASPxListBox;
                foreach (ListEditItem item in nestedListBox.Items)                
                    item.Text = Dami.TraduCuvant(item.Text);
                ASPxButton btnInchide = cmbStare.FindControl("btnInchide") as ASPxButton;
                if (btnInchide != null)
                    btnInchide.Text = Dami.TraduCuvant("btnInchide", "Inchide");

                #endregion

                if (Request["pp"] != null)
                    txtTitlu.Text = "Prima Pagina - Cereri";
                else
                    txtTitlu.Text = General.VarSession("Titlu").ToString();
				
                string cmp = "";
                if (Constante.tipBD == 2)
                    cmp = "FROM DUAL";

                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                string sqlHr = $@"SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                DataTable dtHr = General.IncarcaDT(sqlHr, null);

                if (dtHr != null && dtHr.Rows.Count > 0) esteHr = true;

                if (!IsPostBack)
                {
                    DataTable dt = General.IncarcaDT($@"SELECT ""Rol"" AS ""Id"", ""RolDenumire"" AS ""Denumire"" FROM (
                            SELECT ""Rol"", ""RolDenumire"", 1 AS ""Ordin"" FROM ({Dami.SelectCereri()}) X GROUP BY ""Rol"", ""RolDenumire""
                            UNION 
                            SELECT -1 AS ""Rol"", '{Dami.TraduCuvant("Toate")}' AS ""RolDenumire"", 0 AS ""Ordin""  {cmp} ) Y WHERE Y.""Rol"" IS NOT NULL ORDER BY ""Ordin"" ", null);

                    //Radu 10.02.2020
                    chkAnulare.Checked = true;

                    //Florin 2018.10.15
                    switch(dt.Rows.Count)
                    {
                        case 0:
                            lblRol.Visible = false;
                            cmbRol.Visible = false;
                            break;
                        case 1:
                            lblRol.Visible = false;
                            cmbRol.Visible = false;
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
                    if (dtViz == null || dtViz.Rows.Count == 0 || (dtViz.Rows.Count == 1 && General.Nz(dtViz.Rows[0]["IdSuper"], "0").ToString() == "0"))
                    {
                        cmbViz.Items.Add(Dami.TraduCuvant("Toate cererile", "Toate cererile"), 2);
                    }
                    else
                    {
                        cmbViz.Items.Add(Dami.TraduCuvant("De aprobat", "De aprobat"), 1);
                        cmbViz.Items.Add(Dami.TraduCuvant("Toate cererile", "Toate cererile"), 2);
                    }

                    cmbViz.SelectedIndex = 0;

                    if (esteHr)
                    {
                        ListEditItem itm = new ListEditItem();
                        itm.Text = Dami.TraduCuvant("Toti angajatii", "Toti angajatii");
                        itm.Value = 3;
                        cmbViz.Items.Add(itm);
                    }

                    //Florin2019.07.17
                    NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_CereriAbs"] ?? "").ToString());
                    if (lst.Count > 0)
                    {
                        if (General.Nz(lst["Viz"], "").ToString() != "") cmbViz.SelectedIndex = Convert.ToInt32(lst["Viz"])-1;
                        if (General.Nz(lst["Rol"], "").ToString() != "") cmbRol.Value = Convert.ToInt32(lst["Rol"]);
                        if (General.Nz(lst["Stare"], "").ToString() != "") cmbStare.Text = lst["Stare"].ToString();
                        if (General.Nz(lst["DtInc"], "").ToString() != "") txtDtInc.Value = Convert.ToDateTime(lst["DtInc"]);
                        if (General.Nz(lst["DtSf"], "").ToString() != "") txtDtSf.Value = Convert.ToDateTime(lst["DtSf"]);

                        Session["Filtru_CereriAbs"] = "";
                    }

                    grDate.DataBind();
                }

                if (General.Nz(cmbViz.Value,"").ToString() == "3")
                    cmbRol.Value = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
                if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);
                
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
                //Florin 2019.07.17
                #region Salvam Filtrul

                string req = "";
                if (cmbViz.Value != null) req += "&Viz=" + cmbViz.Value;
                if (cmbRol.Value != null) req += "&Rol=" + cmbRol.Value;
                if (cmbStare.Value != null) req += "&Stare=" + cmbStare.Value;
                if (txtDtInc.Value != null) req += "&DtInc=" + txtDtInc.Value;
                if (txtDtSf.Value != null) req += "&DtSf=" + txtDtSf.Value;

                Session["Filtru_CereriAbs"] = req;

                #endregion

                Session["grDate_Filtru"] = "Absente.Lista;" + grDate.FilterExpression;
                Session["Sablon_CheiePrimara"] = -99;
                Session["Sablon_TipActiune"] = "New";
                Response.Redirect("~/Absente/Cereri.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Actiune", "NumeAngajat", "DataInceput", "Rol" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

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

                if (ids.Count != 0) msg += General.MetodeCereri(1, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), "",  Convert.ToInt32(General.Nz(cmbRol.Value,0)));
                grDate.JSProperties["cpAlertMessage"] = msg;
                grDate.DataBind();
                grDate.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRespinge_Click(object sender, EventArgs e)
        {
            try
            {
                RespingeCerere("");            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                string filtru = "";
                if (General.Nz(cmbViz.Value,1).ToString() == "1") filtru=" AND A.\"Actiune\"=1 ";
                if (cmbRol.SelectedIndex != -1 && cmbRol.SelectedIndex != 0) filtru += @" AND A.""Rol""= " + General.Nz(cmbRol.Value, 0);
                if (cmbStare.Value != null) filtru += @" AND A.""IdStare"" IN (" + DamiStari() + ")";
                if (txtDtSf.Date != null) filtru += @" AND A.""DataInceput"" <= " + General.ToDataUniv(txtDtSf.Date);
                if (txtDtInc.Date != null) filtru += " AND " + General.ToDataUniv(txtDtInc.Date) + @" <= A.""DataSfarsit"" ";

                string sqlFinal = Dami.SelectCereri(Convert.ToInt32(cmbViz.Value ?? -99)) + $@" {filtru} ORDER BY A.TIME DESC";
                dt = General.IncarcaDT(sqlFinal, null);
                grDate.KeyFieldName = "Id; Rol";
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    switch (arr[0])
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

                                int idStare = Convert.ToInt32(obj[3] ?? -1);
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
                                    if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 34)
                                    {
                                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D1" && General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D2")
                                        {
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Puteti anula numai cerereile cu tip de absenta Delegatie");
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

                                //daca este hr nu se aplica regulile
                                if (Convert.ToInt32(obj[10] ?? 0) != 77)
                                {
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
                                }

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
                                Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "Ptj_Cereri", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                                grDate.DataBind();
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
                                    General.SelectCereriIstoric(Convert.ToInt32(obj[1]), Convert.ToInt32(General.Nz(obj[3], -1)), Convert.ToInt32(drAbs["IdCircuit"]), 0, out sqlIst, out trimiteLaInlocuitor, (int)obj[0]);


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

                                        grDate.DataBind();

                                        string strPopUp = "Proces realizat cu succes";
                                        if (msg != "")
                                            strPopUp += " " + Dami.TraduCuvant("cu urmatorul avertisment") + Environment.NewLine + msg;
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(strPopUp);
                                    }
                                }
                                #endregion
                            }
                            break;
                        case "btnRespinge":
                            RespingeCerere(arr[1].Trim());
                            break;
                        case "btnAproba":
                            btnAproba_Click(null, null);
                            break;
                        case "colHide":
                            grDate.Columns[arr[1]].Visible = false;
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

                string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
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
                nr = General.CalcZile(Convert.ToInt32(obj[1] ?? 0), dtIncOri, dtSfOri, Convert.ToInt32(cmbRol.Value ?? 0), Convert.ToInt32(obj[2] ?? 0));

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
                nr = General.CalcZile(Convert.ToInt32(obj[1] ?? 0), dtIncDes, dtSfDes, Convert.ToInt32(cmbRol.Value ?? 0), Convert.ToInt32(obj[2] ?? 0));

                //Radu 10.02.2020 - daca chkAnulare este bifata, IdStare = -1
                string idStare = @"""IdStare""";
                if (chkAnulare.Checked)
                    idStare = "-1";

                string sqlDes = $@"INSERT INTO ""Ptj_Cereri""(F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", USER_NO, TIME, ""Id"", ""TrimiteLa"", ""IdCerereDivizata"", ""Comentarii"", ""NrOre"")
                                OUTPUT Inserted.Id                                
                                SELECT F10003, ""IdAbsenta"", {General.ToDataUniv(dtIncDes)} AS""DataInceput"", {General.ToDataUniv(dtSfDes)} AS ""DataSfarsit"", {nr} AS ""NrZile"", {nrViitor} AS ""NrZileViitor"", ""Observatii"", {idStare}, ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", {Session["UserId"]}, {General.CurrentDate()}, {sqlIdCerere} AS ""Id"", ""TrimiteLa"", {obj[0]} AS ""IdCerereDivizata"", ""Comentarii"", ""NrOre"" FROM ""Ptj_Cereri"" WHERE ""Id""={obj[0]}";

                string sqlGen = "BEGIN TRAN " + "\n\r" +
                                sqlOri + "; " + "\n\r" +
                                sqlDes + "; " + "\n\r" +
                                "COMMIT TRAN";

                DataTable dtCer = General.IncarcaDT(sqlGen, null);

                if (dtCer.Rows.Count > 0)
                {
                    string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""(IdCerere, IdCircuit, IdSuper, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor, IdUserInlocuitor)
                                SELECT {Convert.ToInt32(dtCer.Rows[0]["Id"])} AS IdCerere, IdCircuit, IdSuper, {idStare}, IdUser, Pozitie, Culoare, Aprobat, DataAprobare, {Session["UserId"]} AS USER_NO, {General.CurrentDate()} AS TIME, Inlocuitor, IdUserInlocuitor FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere"" = {obj[0]}";

                    General.ExecutaNonQuery(sqlIst, null);
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

                object cps = null;
                object inl = null;
                object cb = null;

                ASPxComboBox cmbCps = grDate.FindEditFormTemplateControl("cmbCps") as ASPxComboBox;
                if (cmbCps != null && cmbCps.SelectedIndex != -1 && cmbCps.Value != null) cps = cmbCps.Value;

                ASPxComboBox cmbInl = grDate.FindEditFormTemplateControl("cmbInl") as ASPxComboBox;
                if (cmbInl != null && cmbInl.SelectedIndex != -1 && cmbInl.Value != null) inl = cmbInl.Value;

                ASPxCheckBox chk = grDate.FindEditFormTemplateControl("chkBifa") as ASPxCheckBox;
                if (chk != null && chk.Value != null) cb = chk.Value;

                General.ExecutaNonQuery($@"UPDATE ""Ptj_Cereri"" SET ""Observatii""=@1, ""Comentarii""=@2, ""TrimiteLa""=@3, ""Inlocuitor""=@4, ""CampBifa""=@6 WHERE ""Id""=@5", 
                    new object[] { e.NewValues["Observatii"], e.NewValues["Comentarii"], cps, inl, id, cb });

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
                #region Salvam Filtrul

                string req = "";
                if (cmbViz.Value != null) req += "&Viz=" + cmbViz.Value;
                if (cmbRol.Value != null) req += "&Rol=" + cmbRol.Value;
                if (cmbStare.Value != null) req += "&Stare=" + cmbStare.Value;
                if (txtDtInc.Value != null) req += "&DtInc=" + txtDtInc.Value;
                if (txtDtSf.Value != null) req += "&DtSf=" + txtDtSf.Value;

                Session["Filtru_CereriAbs"] = req;

                #endregion

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
                    Response.Redirect("~/Absente/IstoricExtins.aspx", false);
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

        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Comentarii")
                {
                    if (Dami.ValoareParam("InlocuitorEditabilInAprobare", "0") == "1")
                    {
                        e.Editor.Visible = true;
                        e.Editor.Caption = "Comentarii";
                    }
                    else
                    {
                        e.Editor.Visible = false;
                        e.Editor.Caption = "";
                    }
                }
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

        protected void oObservatiiMemo_Init(object sender, EventArgs e)
        {
            try
            {
                ASPxMemo txt = sender as ASPxMemo;
                GridViewDataColumn col = grDate.Columns["Observatii"] as GridViewDataColumn;
                txt.Enabled = !col.ReadOnly;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void comentariiMemo_Init(object sender, EventArgs e)
        {
            try
            {
                ASPxMemo txt = sender as ASPxMemo;
                GridViewDataColumn col = grDate.Columns["Comentarii"] as GridViewDataColumn;
                txt.Enabled = !col.ReadOnly;
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
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Compensare", "CompensareBanca", "CompensarePlata", "CompensareBancaDenumire", "CompensarePlataDenumire", "Inlocuitor", "TrimiteLa", "AdaugaAtasament", "IdStare" }) as object[];
               
                ASPxMemo txtObs = grDate.FindEditFormTemplateControl("txtObs") as ASPxMemo;
                ASPxMemo txtCom = grDate.FindEditFormTemplateControl("txtCom") as ASPxMemo;

                ASPxLabel lbl = grDate.FindEditFormTemplateControl("lblInl") as ASPxLabel;
                ASPxComboBox cmbInl = grDate.FindEditFormTemplateControl("cmbInl") as ASPxComboBox;

                if (Dami.ValoareParam("InlocuitorEditabilInAprobare", "0") == "1")
                {
                    DataTable dtInl = General.IncarcaDT(General.SelectInlocuitori(-55, new DateTime(1900, 1, 1), new DateTime(2200, 1, 1)), null);
                    cmbInl.DataSource = dtInl;
                    if (Convert.ToInt32(General.Nz(obj[5], -1)) == -1)
                        cmbInl.Value = null;
                    else
                        cmbInl.Value = Convert.ToInt32(General.Nz(obj[5],-1));

                    lbl.Visible = true;
                    cmbInl.Visible = true;
                }
                else
                {
                    lbl.Visible = false;
                    cmbInl.Visible = false;
                }

                ASPxLabel lblCps = grDate.FindEditFormTemplateControl("lblCps") as ASPxLabel;
                ASPxComboBox cmbCps = grDate.FindEditFormTemplateControl("cmbCps") as ASPxComboBox;

                if (obj[0] != null && (int)obj[0] == 1)
                {
                    lblCps.Visible = true;
                    cmbCps.Visible = true;

                    // id = -13 este banca
                    // id = -14 este plata
                    List<metaDate> lst = new List<metaDate>();
                    metaDate ent = new metaDate();
                    ent.Id = Convert.ToInt32(General.Nz(obj[1],(int)Constante.IdCompensareDefault.LaBanca));
                    ent.Denumire = General.Nz(obj[3], Dami.TraduCuvant("Banca")).ToString();
                    lst.Add(ent);

                    metaDate ent2 = new metaDate();
                    ent2.Id = Convert.ToInt32(General.Nz(obj[2], (int)Constante.IdCompensareDefault.LaPlata));
                    ent2.Denumire = General.Nz(obj[4], Dami.TraduCuvant("Plata")).ToString();
                    lst.Add(ent2);

                    cmbCps.DataSource = lst;
                    cmbCps.Value = obj[6];
                }
                else
                {
                    lblCps.Visible = false;
                    cmbCps.Visible = false;
                }

                ASPxUploadControl btn = grDate.FindEditFormTemplateControl("btnDocUpload") as ASPxUploadControl;
                if (btn != null)
                {
                    if (General.Nz(obj[7], "0").ToString() == "1")
                        btn.Visible = true;
                    else
                        btn.Visible = false;
                }

                //daca starea cererii este aprobata si rolul este cel de hr afisam campul bifa
                string rolCmp = Dami.ValoareParam("CampBifa_IDuriRoluri");
                ASPxCheckBox chk = grDate.FindEditFormTemplateControl("chkBifa") as ASPxCheckBox;
                chk.Text = Dami.TraduCuvant("Camp bifa");
                if (chk != null)
                {
                    if (General.Nz(obj[8], "0").ToString() == "3" && (esteHr || rolCmp.IndexOf((cmbRol.Value ?? "qwerty").ToString()) >= 0))
                        chk.Visible = true;
                    else
                        chk.Visible = false;
                }

                //daca are rol de HR il lasam sa completeze observatii
                string rolHr = Dami.ValoareParam("Cereri_IDuriRoluriHR");
                if (rolHr.IndexOf((cmbRol.Value ?? "").ToString()) >= 0 || General.Nz(cmbViz.Value,"").ToString() == "3")
                {
                    txtObs.ReadOnly = false;
                    txtObs.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void RespingeCerere(string motiv)
        {
            try
            {
                List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Actiune", "NumeAngajat", "DataInceput", "Rol" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

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
                General.MemoreazaEroarea("Vine din Absente Lista");
                if (ids.Count != 0) msg += General.MetodeCereri(2, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), motiv, Convert.ToInt32(General.Nz(cmbRol.Value, 0)));
                grDate.JSProperties["cpAlertMessage"] = msg;
                grDate.DataBind();
                grDate.Selection.UnselectAll();
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

        private string DamiStari()
        {
            string val = "";

            try
            {
                if (General.Nz(cmbStare.Value,"").ToString() != "")
                {
                    val = cmbStare.Value.ToString().Replace(Dami.TraduCuvant("Solicitat"), "1").Replace(Dami.TraduCuvant("In Curs"), "2").Replace(Dami.TraduCuvant("Aprobat"), "3").Replace(Dami.TraduCuvant("Respins"), "0").Replace(Dami.TraduCuvant("Anulat"), "-1").Replace(Dami.TraduCuvant("Planificat"), "4");
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

    }
}