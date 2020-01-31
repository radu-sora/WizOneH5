using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.CereriDiverse
{
    public partial class Lista : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "CereriDiverse.Lista";
                Session["Absente_Cereri_Date"] = null;

                DataTable dtTip = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"" FROM ""MP_tblTipCerere"" ", null);
                cmbTip.DataSource = dtTip;
                cmbTip.DataBind();

                DataTable dtStari = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                cmbStare.DataSource = dtStari;
                cmbStare.DataBind();

                txtDataInc.Value = new DateTime(DateTime.Now.Year, 1, 1);
                txtDataSf.Value = new DateTime(DateTime.Now.Year, 12, 31);
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

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");
                btnCerere.Image.ToolTip = Dami.TraduCuvant("btnCerere", "Arata Cerere");
                btnAtasament.Image.ToolTip = Dami.TraduCuvant("btnAtasament", "Arata Atasament");

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

                #endregion

                if (Request["pp"] != null)
                    txtTitlu.Text = "Prima Pagina - Cereri";
                else
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    DataTable dt = General.IncarcaDT($@"SELECT ""Rol"" AS ""Id"", ""RolDenumire"" AS ""Denumire"" FROM (
                            SELECT ""Rol"", ""RolDenumire"", 1 AS ""Ordin"" FROM ({Dami.SelectCereriDiverse()}) X GROUP BY ""Rol"", ""RolDenumire""
                            UNION 
                            SELECT -1 AS ""Rol"", '{Dami.TraduCuvant("Toate")}' AS ""RolDenumire"", 0 AS ""Ordin"" {General.FromDual()}) Y ORDER BY ""Ordin"" ", null);

                    if (dt.Rows.Count == 0 || dt.Rows.Count == 1 || dt.Rows.Count == 2)
                    {
                        lblRol.Visible = false;
                        cmbRol.Visible = false;
                    }
                    else
                    {
                        cmbRol.DataSource = dt;
                        cmbRol.DataBind();
                        cmbRol.SelectedIndex = 0;
                    }

                    string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                    string sqlHr = $@"SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                    DataTable dtHr = General.IncarcaDT(sqlHr, null);

                    //determinam daca este angajat sau manager pentru a selecta in cmbViz
                    DataTable dtViz = General.IncarcaDT(@"SELECT * FROM ""F100Supervizori"" WHERE ""IdUser""=@1", new object[] { Session["UserId"] });
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

                    bool esteHr = false;
                    if (dtHr != null && dtHr.Rows.Count > 0) esteHr = true;
                    if (esteHr)
                    {
                        ListEditItem itm = new ListEditItem();
                        itm.Text = Dami.TraduCuvant("Toti angajatii", "Toti angajatii");
                        itm.Value = 3;
                        cmbViz.Items.Add(itm);
                    }

                    grDate.DataBind();
                }
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
                Session["grDate_Filtru"] = "CereriDiverse.Lista;" + grDate.FilterExpression;
                Session["Sablon_CheiePrimara"] = -99;
                Session["Sablon_TipActiune"] = "New";
                Response.Redirect("~/CereriDiverse/Cereri.aspx", false);
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
                List<int> ids = new List<int>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "NumeAngajat", "TipCerere", "Descriere", "Raspuns" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(Convert.ToInt32(General.Nz(arr[0], 0)));
                    //ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                }

                if (ids.Count != 0) msg += General.MetodeCereriDiverse(1, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), "");

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
                //if (General.Nz(cmbViz.Value,1).ToString() == "1") filtru = " AND A.\"Actiune\"=1 ";
                if (cmbTip.SelectedIndex != -1) filtru += @" AND A.""IdTipCerere""= " + General.Nz(cmbTip.Value, 0);
                //if (cmbRol.SelectedIndex != -1 && cmbRol.SelectedIndex != 0) filtru += @" AND A.""Rol""= " + General.Nz(cmbRol.Value, 0);

                if (txtDataInc.Value != null) filtru += @" AND A.""TIME"" >= " + General.ToDataUniv(Convert.ToDateTime(txtDataInc.Value));
                if (txtDataSf.Value != null) filtru += @" AND A.""TIME"" <= " + General.ToDataUniv(Convert.ToDateTime(txtDataSf.Value));
                if (cmbStare.Value != null) filtru += @" AND A.""IdStare"" IN (" + DamiStari() + ")";

                string sqlFinal = Dami.SelectCereriDiverse(Convert.ToInt32(cmbViz.Value ?? -99)) + $@" {filtru} ORDER BY A.TIME DESC";
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
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "NumeAngajat", "TipCerere", "Descriere", "Raspuns" }) as object[];
                                if (obj == null || obj.Count() == 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata");
                                    //MessageBox.Show(Dami.TraduCuvant("Nu exista linie selectata"), MessageBox.icoWarning, "");
                                    return;
                                }

                                int idStare = Convert.ToInt32(obj[1] ?? -1);
                                //int idAbs = Convert.ToInt32(obj[2] ?? -1);

                                if (idStare == -1)                //nu anulezi o cerere deja anulata
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Cererea este deja anulata");
                                    return;
                                }

                                if (idStare == 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti anula o cerere respinsa");
                                    //MessageBox.Show(Dami.TraduCuvant("Nu puteti anula o cerere respinsa"), MessageBox.icoWarning, "");
                                    return;
                                }

                                //DataRow drAbs = General.IncarcaDR(General.SelectAbsente(obj[1].ToString(), Convert.ToInt32(obj[2])), null);
                                //if (drAbs != null)
                                //{
                                //    if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 34)
                                //    {
                                //        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D1" && General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D2")
                                //        {
                                //            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Puteti anula numai cerereile cu tip de absenta Delegatie");
                                //            //MessageBox.Show(Dami.TraduCuvant("Puteti anula numai cerereile cu tip de absenta Delegatie"), MessageBox.icoWarning, "");
                                //            return;
                                //        }

                                //        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "CO" && Convert.ToInt32(drAbs["CampBifa1"]) == 1)
                                //        {
                                //            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima");
                                //            //MessageBox.Show(Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima"), MessageBox.icoWarning, "");
                                //            return;
                                //        }
                                //    }

                                //    if ((obj[1] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["Anulare"], 0)) == 0 && idStare != 4)
                                //    {
                                //        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta");
                                //        //MessageBox.Show(Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta"), MessageBox.icoWarning, "");
                                //        return;
                                //    }

                                //    if ((obj[1] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["AnulareAltii"], 0)) == 0)
                                //    {
                                //        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta");
                                //        //MessageBox.Show(Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta"), MessageBox.icoWarning, "");
                                //        return;
                                //    }
                                //}

                                //daca este hr nu se aplica regulile
                                //if (Convert.ToInt32(obj[10] ?? 0) != 77)
                                //{
                                //    int selRol = 0;
                                //    if (cmbRol.SelectedItem != null) selRol = Convert.ToInt32(General.Nz(cmbRol.SelectedItem.Value, 0));
                                //    DateTime ziDrp = Dami.DataDrepturi(Convert.ToInt32(General.Nz(obj[8], -99)), Convert.ToInt32(General.Nz(obj[9], 0)), Convert.ToDateTime(obj[4]), Convert.ToInt32(obj[1]), selRol);
                                //    if (Convert.ToDateTime(obj[4]).Date < ziDrp)
                                //    {
                                //        if (ziDrp.Year == 2111 && ziDrp.Month == 11 && ziDrp.Day == 11)
                                //            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti stabilite drepturi pentru a realiza aceasta operatie");
                                //        else
                                //        {
                                //            if (ziDrp.Year == 2222 && ziDrp.Month == 12 && ziDrp.Day == 13)
                                //                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pontajul a fost aprobat");
                                //            else
                                //                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat") + " " + ziDrp.Date.ToShortDateString();
                                //        }
                                //        return;
                                //    }
                                //}

                                //string msg = Notif.TrimiteNotificare("Absente.Lista", 2, $@"SELECT *, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" WHERE ""Id""=" + obj[0], "", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                                //if (msg != "" && msg.Substring(0, 1) == "2")
                                //{
                                //    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg.Substring(2));
                                //    return;
                                //}
                                //else
                                //{
                                    try
                                    {
                                        ////introducem o linie de anulare in Ptj_CereriIstoric
                                        //string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""
                                        //            (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                        //            SELECT ""Id"", ""IdCircuit"", {Session["UserId"]}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""Ptj_Cereri"" WHERE ""Id""={obj[0]};";

                                        //string sqlCer = $@"UPDATE ""Ptj_Cereri"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={obj[0]};";

                                        ////Florin 2018.04.04
                                        //string sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={obj[0]} AND ""Tabela""='Ptj_Cereri' AND ""EsteCerere"" = 1; ";

                                        //string sqlGen = "BEGIN " + "\n\r" +
                                        //                        sqlIst + "\n\r" +
                                        //                        sqlCer + "\n\r" +
                                        //                        sqlDel + "\n\r" +
                                        //                        "END;";
                                        //General.ExecutaNonQuery(sqlGen, null);

                                        //introducem o linie de anulare in MP_CereriIstoric
                                        string sqlIst = $@"INSERT INTO ""MP_CereriIstoric""
                                                        (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"", ""IdTipCerere"")
                                                        SELECT ""Id"", ""IdCircuit"", {Session["UserId"]}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1), ""IdTipCerere"" FROM ""MP_Cereri"" WHERE ""Id""={obj[0]};";

                                        string sqlCer = $@"UPDATE ""MP_Cereri"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={obj[0]};";

                                        //Florin 2018.04.04
                                        string sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={obj[0]} AND ""Tabela""='MP_Cereri'; ";

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
                                //}

                                //stergem din pontaj
                                //int idTipOre = 0;
                                //string oreInVal = "";
                                //if (drAbs != null)
                                //{
                                //    idTipOre = Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0));
                                //    oreInVal = General.Nz(drAbs["OreInVal"], "").ToString();
                                //}

                                //Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT *, 2 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" WHERE ""Id""=" + obj[0], "Ptj_Cereri", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                                grDate.DataBind();
                                #endregion 
                            }
                            break;
                        case "btnRespinge":
                            RespingeCerere(arr[1].Trim());
                            break;
                        case "btnAproba":
                            btnAproba_Click(null, null);
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

        private DataRow GetDataAnulare(int idUser, int f10003, int tipOperatie, int idCircuit = -99)
        {
            DataRow ras = null;

            try
            {
                DataTable entRol = General.GetRoluriUser(idUser);

                int idRol = 1;
                if (General.VarSession("User_Marca").ToString() == f10003.ToString())
                {
                    idRol = 1;
                }
                else
                {
                    if (entRol.Select("Id = 3").Count() > 0)
                    {
                        idRol = 3;
                    }
                    else
                    {
                        if (entRol.Select("Id = 2").Count() > 0) idRol = 2;
                    }
                }

                ras = General.GetParamCereri(tipOperatie, idRol, idCircuit);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var id = e.Keys["Id"];

                object cps = null;
                object inl = null;

                ASPxComboBox cmbCps = grDate.FindEditFormTemplateControl("cmbCps") as ASPxComboBox;
                if (cmbCps != null && cmbCps.SelectedIndex != -1 && cmbCps.Value != null) cps = cmbCps.Value;

                ASPxComboBox cmbInl = grDate.FindEditFormTemplateControl("cmbInl") as ASPxComboBox;
                if (cmbInl != null && cmbInl.SelectedIndex != -1 && cmbInl.Value != null) inl = cmbInl.Value;

                General.ExecutaNonQuery($@"UPDATE ""MP_Cereri"" SET ""Raspuns""=@1 WHERE ""Id""=@2",    
                    new object[] { e.NewValues["Raspuns"], id });   //Radu 13.08.2018


                if (Session["CereriDiverse_Fisier"] != null)
                {
                    string sql = "SELECT COUNT(*) FROM \"tblFisiere\" WHERE \"Tabela\" = 'MP_Cereri' AND \"Id\" = {0} AND \"EsteCerere\" = 0";
                    sql = string.Format(sql, id);
                    DataTable dt = General.IncarcaDT(sql, null);
                    if (dt == null || dt.Rows.Count == 0 || dt.Rows[0][0] == null || dt.Rows[0][0].ToString().Length <= 0 || Convert.ToInt32(dt.Rows[0][0].ToString()) == 0)
                    {
                        sql = "INSERT INTO \"tblFisiere\" (\"Tabela\", \"Id\", \"Fisier\", \"FisierNume\", \"FisierExtensie\", \"EsteCerere\") VALUES ('MP_Cereri', {0}, @1, '{1}', '.{2}', 0)";
                        sql = string.Format(sql, id, Session["CereriDiverse_FisierNume"].ToString(), Session["CereriDiverse_FisierExtensie"].ToString());
                    }
                    else
                    {
                        sql = "UPDATE \"tblFisiere\" SET \"Fisier\" = @1, \"FisierNume\" = '{0}', \"FisierExtensie\" = '.{1}' WHERE \"Tabela\" = 'MP_Cereri' AND \"Id\" = {2} AND \"EsteCerere\" = 0";
                        sql = string.Format(sql, Session["CereriDiverse_FisierNume"].ToString(), Session["CereriDiverse_FisierExtensie"].ToString(), id);
                    }
                    object file = Session["CereriDiverse_Fisier"] as object;
                    General.ExecutaNonQuery(sql, new object[] { file });
                    Session["CereriDiverse_Fisier"] = null;
                    Session["CereriDiverse_FisierNume"] = null;
                    Session["CereriDiverse_FisierExtensie"] = null;
                }

                ////List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id" });

                ////object[] lst = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id" }) as object[];
                ////if (lst == null || lst.Count() == 0) return;

                //if (Dami.ValoareParam("InlocuitorEditabilInAprobare", "0") == "1")
                //{
                //    string sqlStr = $@"UPDATE ""Ptj_Cereri"" SET ""Observatii""=@1, ""TrimiteLa""=@2, ""Comentarii""=@3, ""Inlocuitor""=@5 WHERE ""Id""=@4";
                //    General.ExecutaNonQuery(sqlStr, new object[] { e.NewValues["Observatii"], e.NewValues["TrimiteLa"], e.NewValues["Comentarii"], id, e.NewValues["Inlocuitor"] });
                //}
                //else
                //{
                //    string sqlStr = $@"UPDATE ""Ptj_Cereri"" SET ""Observatii""=@1, ""TrimiteLa""=@2, ""Comentarii""=@3 WHERE ""Id""=@4";
                //    General.ExecutaNonQuery(sqlStr, new object[] { e.NewValues["Observatii"], e.NewValues["TrimiteLa"], e.NewValues["Comentarii"], id });
                //}

                e.Cancel = true;
                grDate.CancelEdit();

                //MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoSuccess, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            try
            {
                if (e.ButtonType == ColumnCommandButtonType.Edit)
                {
                    object row = ((ASPxGridView)sender).GetRow(e.VisibleIndex);
                    if (row == null) return;
                    int id = Convert.ToInt32(((DataRowView)row)["IdStare"]);
                    if (id == 1 || id == 2 || id == 4)
                        e.Enabled = true;
                    else
                        e.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void oRaspunsMemo_Init(object sender, EventArgs e)
        {
            try
            {
                ASPxMemo txt = sender as ASPxMemo;
                GridViewDataColumn col = grDate.Columns["Raspuns"] as GridViewDataColumn;
                //txt.Enabled = !col.ReadOnly;      //Radu 13.08.2018
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
                List<int> ids = new List<int>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "NumeAngajat", "TipCerere", "Descriere", "Raspuns" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += Dami.TraduCuvant("Cererea pt") + " " + arr[2] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(Convert.ToInt32(General.Nz(arr[0], 0)));
                    //ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                }

                if (ids.Count != 0) msg += General.MetodeCereriDiverse(2, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), motiv);
                
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

        private string DamiStari()
        {
            string val = "";

            try
            {
                if (General.Nz(cmbStare.Value, "").ToString() != "")
                {
                    val = cmbStare.Value.ToString().ToLower().Replace("solicitat", "1").Replace("in curs", "2").Replace("aprobat", "3").Replace("respins", "0").Replace("anulat", "-1").Replace("planificat", "4");
                }
            }
            catch (Exception)
            {
            }

            return val;
        }
		
		protected void binImg_ValueChanged(object sender, EventArgs e)
        {
            ASPxBinaryImage binImg = sender as ASPxBinaryImage;
            Session["CereriDiverse_FisierNume"] = binImg.GetUploadedFileName();
            Session["CereriDiverse_Fisier"] = binImg.Value;
            Session["CereriDiverse_FisierExtensie"] = binImg.GetUploadedFileName().Split('.')[binImg.GetUploadedFileName().Split('.').Count() - 1];

        }

    }
}