using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.BP
{
    public partial class Aprobare : System.Web.UI.Page
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
                Session["PaginaWeb"] = "BP.Aprobare";
                //Session["Absente_Cereri_Date"] = null;

                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnTranf.Text = Dami.TraduCuvant("btnTranf", "Transfera");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                lblAnul.InnerText = Dami.TraduCuvant("Anul");
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblLuna.InnerText = Dami.TraduCuvant("Luna");
                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblNivel.InnerText = Dami.TraduCuvant("Nivel aprobare");
                lblUser.InnerText = Dami.TraduCuvant("Utilizator");

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                cmbAnul.DataSource = General.ListaNumere(2010, DateTime.Now.Year + 5);
                cmbAnul.DataBind();

                DataTable dt = new DataTable();

                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Denumire", typeof(string));

                dt.Rows.Add(1, "Nivelul meu");
                dt.Rows.Add(2, "Toate (inclusiv nivelurile inferioare)");

                cmbNivel.DataSource = dt;
                cmbNivel.DataBind();

                cmbLuna.DataSource = General.ListaLuniDesc();
                cmbLuna.DataBind();

                DataTable dtStari = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                DataTable dtUser = General.IncarcaDT($@"SELECT F70102, F70104 FROM USERS ", null);
                GridViewDataComboBoxColumn colUser = (grDate.Columns["USER_NO"] as GridViewDataComboBoxColumn);
                colUser.PropertiesComboBox.DataSource = dtUser;

                cmbUser.DataSource = General.IncarcaDT("SELECT DISTINCT A.USER_NO AS F70102, F70104 FROM \"BP_Prime\" A JOIN USERS ON A.USER_NO = F70102", null);
                cmbUser.DataBind();

                DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

                if (!IsPostBack)
                {
                    //cmbLuna.Value = Convert.ToInt32(Dami.ValoareParam("LunaLucru"));
                    //cmbAnul.Value = Convert.ToInt32(Dami.ValoareParam("AnLucru"));

                    cmbLuna.Value = Convert.ToInt32(dt010.Rows[0][1].ToString());
                    cmbAnul.Value = Convert.ToInt32(dt010.Rows[0][0].ToString());
                    Session["AprobareBP_Anul"] = Convert.ToInt32(cmbAnul.Value);
                    Session["AprobareBP_Luna"] = Convert.ToInt32(cmbLuna.Value);

                }
                else
                {
                    cmbLuna.Value = Convert.ToInt32(Session["AprobareBP_Luna"].ToString());
                    cmbAnul.Value = Convert.ToInt32(Session["AprobareBP_Anul"].ToString());
                }

                cmbAng.DataSource = General.GetAngajati(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbAnul.Value.ToString()), Convert.ToInt32(cmbLuna.Value.ToString()));
                cmbAng.DataBind();


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

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

    
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");

   
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");


                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

    
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                   
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
                string explicatii = "", sume = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Explicatie", "SumaNeta", "SumaBruta", "Luna", "An", "Tip" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    int tip = arr[7].ToString() == "Avans" ? 1 : 2;

                    if (Convert.ToInt32(arr[6].ToString()) < Convert.ToInt32(dt010.Rows[0][0].ToString())
                        || (Convert.ToInt32(arr[6].ToString()) == Convert.ToInt32(dt010.Rows[0][0].ToString()) && Convert.ToInt32(arr[5].ToString()) < Convert.ToInt32(dt010.Rows[0][1].ToString())))
                    {
                        grDate.JSProperties["cpAlertMessage"] = "Nu se pot introduce bonusuri pentru o luna anterioara lunii de salarizare curente!";
                        return;
                    }

                    if (Convert.ToInt32(arr[6].ToString()) == Convert.ToInt32(dt010.Rows[0][0].ToString()) && Convert.ToInt32(arr[5].ToString()) == Convert.ToInt32(dt010.Rows[0][1].ToString()))
                    {
                        DataTable entParam = new DataTable();

                        string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                        string sql = "SELECT COUNT(*) FROM \"F100Supervizori\" WHERE \"IdUser\" = {0} AND \"IdSuper\" IN ({1})";
                        sql = string.Format(sql, Session["UserId"].ToString(), idHR);
                        DataTable dtHR = General.IncarcaDT(sql, null);
                        string HR = "";
                        if (dtHR != null && dtHR.Rows.Count > 0 && dtHR.Rows[0][0] != null && dtHR.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtHR.Rows[0][0].ToString()) > 0)
                            HR = "HR";

                        if (tip == 1)
                            entParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'ZiBlocareIntroducerePrimaAvans" + HR + "'", null);
                        else
                            entParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'ZiBlocareIntroducerePrimaLichidare" + HR + "'", null);

                        if (entParam != null && entParam.Rows.Count > 0 && entParam.Rows[0][0] != null && entParam.Rows[0][0].ToString().Length > 0)
                        {
                            try
                            {
                                DataTable dtZiBloc = new DataTable();
                                if (tip == 1)
                                {
                                    if (Constante.tipBD == 1)
                                        dtZiBloc = General.IncarcaDT("SELECT CONVERT(VARCHAR, \"ZiBlocareAvans" + HR + "\", 103) FROM \"DataBlocareBonusuri\"", null);
                                    else
                                        dtZiBloc = General.IncarcaDT("SELECT TO_CHAR(\"ZiBlocareAvans" + HR + "\", 'dd/mm/yyyy') FROM \"DataBlocareBonusuri\"", null);
                                }
                                else
                                {
                                    if (Constante.tipBD == 1)
                                        dtZiBloc = General.IncarcaDT("SELECT CONVERT(VARCHAR, \"ZiBlocareLichidare" + HR + "\", 103) FROM \"DataBlocareBonusuri\"", null);
                                    else
                                        dtZiBloc = General.IncarcaDT("SELECT TO_CHAR(\"ZiBlocareLichidare" + HR + "\", 'dd/mm/yyyy') FROM \"DataBlocareBonusuri\"", null);
                                }
                                if (dtZiBloc != null && dtZiBloc.Rows.Count > 0 && dtZiBloc.Rows[0][0] != null && dtZiBloc.Rows[0][0].ToString().Length > 0)
                                {
                                    DateTime dt = new DateTime(Convert.ToInt32(dtZiBloc.Rows[0][0].ToString().Substring(6, 4)), Convert.ToInt32(dtZiBloc.Rows[0][0].ToString().Substring(3, 2)), Convert.ToInt32(dtZiBloc.Rows[0][0].ToString().Substring(0, 2)));
                                    if (dt.Date < DateTime.Now.Date)
                                    {
                                        grDate.JSProperties["cpAlertMessage"] = "Operatie blocata pentru aceasta luna";
                                        return;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }

                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += Dami.TraduCuvant("Cererea ") + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += Dami.TraduCuvant("Cererea ") + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += Dami.TraduCuvant("Cererea ") + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += Dami.TraduCuvant("Cererea ") + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(Convert.ToInt32(General.Nz(arr[0], 0)));
                    //ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                    explicatii += (arr[2] == null || arr[2].ToString().Trim() == "" ? "_" : arr[2].ToString()) + "#$";
                    sume += (arr[3] == null || arr[3].ToString().Trim() == "" ? "0" : arr[3].ToString()) + ";" + (arr[4] == null || arr[4].ToString().Trim() == "" ? "0" : arr[4].ToString()) + "#$";
                }

                if (ids.Count != 0) msg += MetodeCereri(1, ids, explicatii, sume);

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

        protected void btnTransf_Click(object sender, EventArgs e)
        {
            List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Explicatie", "SumaNeta", "SumaBruta", "Luna", "An", "Tip" });
            if (lst == null || lst.Count() == 0 || lst[0] == null) return;

            int x = 0;

            for (int i = 0; i < lst.Count(); i++)
            {
                object[] arr = lst[i] as object[];
                int idStare = Convert.ToInt32(arr[1].ToString());
                int id = Convert.ToInt32(arr[0].ToString());
                if (idStare == 3)
                {
                    decimal suma = 0;
                    int idTranz = 0;

                    DataTable entCer = General.IncarcaDT("SELECT * FROM \"BP_Prime\" WHERE \"Id\" = " + id, null);
                    DataTable entTip = General.IncarcaDT("SELECT a.*, " + (Constante.tipBD == 1 ? "CONVERT(VARCHAR, \"WizSal_DataPlatii\", 103)" : "TO_CHAR(\"WizSal_DataPlatii\", 'dd/mm/yyyy')") + " AS \"DataPlatii\" FROM \"BP_tblTipPrima\" a WHERE \"Id\" = "
                        + (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["IdTip"] != null && entCer.Rows[0]["IdTip"].ToString().Length > 0 ? entCer.Rows[0]["IdTip"].ToString() : "-99"), null);


                    int WizSal_CodTranzac_Net = 0, WizSal_CodTranzac_Brut = 0;
                    if (entTip != null && entTip.Rows.Count > 0 && entTip.Rows[0]["WizSal_CodTranzac_Net"] != null && entTip.Rows[0]["WizSal_CodTranzac_Net"].ToString().Length > 0)
                        WizSal_CodTranzac_Net = Convert.ToInt32(entTip.Rows[0]["WizSal_CodTranzac_Net"].ToString());
                    if (entTip != null && entTip.Rows.Count > 0 && entTip.Rows[0]["WizSal_CodTranzac_Brut"] != null && entTip.Rows[0]["WizSal_CodTranzac_Brut"].ToString().Length > 0)
                        WizSal_CodTranzac_Brut = Convert.ToInt32(entTip.Rows[0]["WizSal_CodTranzac_Brut"].ToString());

                    decimal totalNet = 0, totalBrut = 0;                
                    if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["TotalNet"] != null && entCer.Rows[0]["TotalNet"].ToString().Length > 0)
                        totalNet = Convert.ToDecimal(entCer.Rows[0]["TotalNet"].ToString());
                    if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["TotalBrut"] != null && entCer.Rows[0]["TotalBrut"].ToString().Length > 0)
                        totalBrut = Convert.ToDecimal(entCer.Rows[0]["TotalBrut"].ToString());

                    if (totalNet != 0)
                    {
                        suma = totalNet;
                        idTranz = WizSal_CodTranzac_Net;
                    }

                    if (totalBrut != 0)
                    {
                        suma = totalBrut;
                        idTranz = WizSal_CodTranzac_Brut;
                    }

                    DataTable entTipDT = General.IncarcaDT("SELECT b.\"WizSal_LunaCalcul\" FROM \"BP_Prime\" a JOIN \"BP_tblTipPrima\" b ON a.\"IdTip\" = b.\"Id\" WHERE a.\"Id\" = " + id, null);


                    int tipData = 1;
                    if (entTipDT != null && entTipDT.Rows.Count > 0 && entTipDT.Rows[0]["WizSal_LunaCalcul"] != null && entTipDT.Rows[0]["WizSal_LunaCalcul"].ToString().Length > 0)
                        tipData = Convert.ToInt32(entTipDT.Rows[0]["WizSal_LunaCalcul"].ToString());


                    int an = Convert.ToInt32(entCer.Rows[0]["An"].ToString());
                    int luna = Convert.ToInt32(entCer.Rows[0]["Luna"].ToString());

                    DateTime dt = new DateTime(1900, 1, 1);
                    if (entTip != null && entTip.Rows.Count > 0 && entTip.Rows[0]["DataPlatii"] != null && entTip.Rows[0]["DataPlatii"].ToString().Length > 0)
                    {
                        string data = entTip.Rows[0]["DataPlatii"].ToString();
                        dt = new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2)));
                    }

                    int cant = 0;
                    decimal pro = 0;
                    if (entTip != null && entTip.Rows.Count > 0 && entTip.Rows[0]["WizSal_Cantitate"] != null && entTip.Rows[0]["WizSal_Cantitate"].ToString().Length > 0)
                        cant = Convert.ToInt32(entTip.Rows[0]["WizSal_Cantitate"].ToString());
                    if (entTip != null && entTip.Rows.Count > 0 && entTip.Rows[0]["WizSal_Procent"] != null && entTip.Rows[0]["WizSal_Procent"].ToString().Length > 0)
                        pro = Convert.ToDecimal(entTip.Rows[0]["WizSal_Procent"].ToString());

                    int f10003 = 0;
                    if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["F10003"] != null && entCer.Rows[0]["F10003"].ToString().Length > 0)
                        f10003 = Convert.ToInt32(entCer.Rows[0]["F10003"].ToString());

                    if (f10003 != 0 && idTranz != 0 && suma != 0)
                    {
                        CreazaTranzacF300(
                            Convert.ToInt32(Session["UserId"].ToString()),
                            f10003,
                            id,
                            idTranz,
                            dt,
                            cant,
                            pro,
                            suma,
                            new DateTime(an, luna, 1),
                            new DateTime(an, luna, 1),
                            tipData,
                            "Prime");
                    }
                    else
                    {
                        string err = "";
                        if (f10003 == 0) err += ", angajat";
                        if (idTranz == 0) err += ", cod tranzactie";
                        if (suma == 0) err += ", suma";
                        if (err != "") General.MemoreazaEroarea(err, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                    x++;
                }
            }
            grDate.JSProperties["cpAlertMessage"] = (x == 1 ? "S-a efectuat 1 transfer!" : "S-au efectuat " + x + " transferuri!");
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
                int f10003 = Convert.ToInt32(cmbAng.Value ?? -99);
                int an = Convert.ToInt32(cmbAnul.Value ?? -99);
                int luna = Convert.ToInt32(cmbLuna.Value ?? -99);
                int tipViz = Convert.ToInt32(cmbNivel.Value ?? 1);
                int user = Convert.ToInt32(cmbUser.Value ?? -99);

                dt = GetCereriAprobare(Convert.ToInt32(Session["UserId"].ToString()), f10003, an, luna, FiltruTipStari(checkComboBoxStare.Value != null ? checkComboBoxStare.Value.ToString() : ""), tipViz, user);
                grDate.KeyFieldName = "Id";
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

                                if (idStare == 3)
                                {
                                    string descModul = "Prime";
                                    string sql = "DELETE FROM F300 WHERE F30042 = 'WIZONE" + descModul.PadLeft(30, Convert.ToChar("_")) + obj[0].ToString() + "'";
                                    General.ExecutaNonQuery(sql, null);
                                }

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
                        case "btnTransf":
                            btnTransf_Click(null, null);
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

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
    
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
                string explicatii = "", sume = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Explicatie", "SumaNeta", "SumaBruta" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += Dami.TraduCuvant("Cererea ") + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += Dami.TraduCuvant("Cererea ")  + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += Dami.TraduCuvant("Cererea ")  + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += Dami.TraduCuvant("Cererea ")  + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(Convert.ToInt32(General.Nz(arr[0], 0)));
                    //ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                    explicatii += (arr[2] == null || arr[2].ToString().Trim() == "" ? "_" : arr[2].ToString()) + "#$";
                    sume += (arr[3] == null || arr[3].ToString().Trim() == "" ? "0" : arr[3].ToString()) + ";" + (arr[4] == null || arr[4].ToString().Trim() == "" ? "0" : arr[4].ToString()) + "#$";
                }

                if (ids.Count != 0) msg += MetodeCereri(2, ids, explicatii, sume);
                
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






        public DataTable GetCereriAprobare(int idUser, int f10003, int an, int luna, string filtruStari, int tipViz, int user)
        {
            DataTable dt = new DataTable();
            string sql = "";
            try
            {
                string[] arr = filtruStari.Split(Convert.ToChar(";"));
                List<int> lst = new List<int>();

                //Radu 28.11.2019
                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");

                string op = "+", cmp = "ISNULL";
                if (Constante.tipBD == 2)
                {
                    op = "||";
                    cmp = "NVL";
                }

                string filtru = "";

                //if (tipViz == 1)                
                //filtru = " AND X.\"Pozitie\" = {0}(A.\"Pozitie\", 0) ";                
                //else
                //filtru = " AND ({0}(A.\"Pozitie\", 0) <= X.\"Pozitie\" OR A.\"IdStare\" = 3 OR A.\"IdStare\" = 0) ";                  
                //filtru = string.Format(filtru, cmp);

                string filtruGen = "";
                if (f10003 != -99) filtruGen += " AND A.F10003 = " + f10003;
                if (an != -99) filtruGen += " AND A.\"An\" = " + an;
                if (luna != -99) filtruGen += " AND A.\"Luna\" = " + luna;
                if (user != -99) filtruGen += " AND A.USER_NO = " + user;
                if (filtruStari.Length > 0) filtruGen += " AND a.\"IdStare\" IN (" + FiltruTipStari(checkComboBoxStare.Value.ToString()).Replace(";", ",").Substring(0, FiltruTipStari(checkComboBoxStare.Value.ToString()).Length - 1) + ")";


                string filtruViz = " AND H.\"Pozitie\" = A.\"Pozitie\" + 1  AND (A.\"IdStare\" = 1 OR A.\"IdStare\" = 2) ";
                if (tipViz != 1)
                    filtruViz = "   AND (A.\"Pozitie\" <= H.\"Pozitie\" or A.\"IdStare\" = 0 OR A.\"IdStare\" = 3 ) ";

                sql = " SELECT DISTINCT A.\"Id\", A.\"An\", A.\"Luna\", CASE  WHEN A.\"Luna\" = 1 THEN 'Ianuarie' WHEN A.\"Luna\" = 2 THEN 'Februarie' WHEN A.\"Luna\" = 3 THEN 'Martie' "
                    + " WHEN A.\"Luna\" = 4 THEN 'Aprilie' WHEN A.\"Luna\" = 5 THEN 'Mai' WHEN A.\"Luna\" = 6 THEN 'Iunie' WHEN A.\"Luna\" = 7 THEN 'Iulie' WHEN A.\"Luna\" = 8 THEN 'August' "
                    + " WHEN A.\"Luna\" = 9 THEN 'Septembrie' WHEN A.\"Luna\" = 10 THEN 'Octombrie' WHEN A.\"Luna\" = 11 THEN 'Noiembrie' WHEN A.\"Luna\" = 12 THEN 'Decembrie' END AS \"LunaDesc\", "
                    + " F10008 {0} ' ' {0} F10009 AS \"Angajat\", A.F10003, \"SumaBruta\", \"SumaNeta\", \"TotalBrut\", \"TotalNet\", C.\"Denumire\" as \"Moneda\", D.\"Denumire\" as \"Grup\", E.\"Denumire\" as \"Tip\", F.\"Denumire\" as \"Categorie\", \"Curs\", "
                    + " \"Explicatie\", \"Recurenta\", CASE WHEN \"AvansLichidare\" = 1 THEN 'Avans' WHEN \"AvansLichidare\" = 2 THEN 'Lichidare' ELSE '' END AS \"Avans\", {4}(A.\"IdStare\", 0) AS \"IdStare\", A.\"Culoare\", CASE WHEN A.\"Id\" IS NOT NULL THEN 1 ELSE 0 END AS \"PoateModifica\", A.USER_NO FROM "
                    + " \"BP_Prime\" A "
                    + " JOIN F100 B ON A.F10003 = B.F10003 "
                    + " LEFT JOIN \"tblMonede\" C ON A.\"IdMoneda\" = C.\"Id\" "
                    + " LEFT JOIN \"BP_tblGrupuri\" D ON A.\"IdGrup\" = D.\"Id\" "
                    + " LEFT JOIN \"BP_tblPrime\" E ON A.\"IdTip\" = E.\"Id\" "
                    + " LEFT JOIN \"BP_tblCategorii\" F ON A.\"IdCategorie\" = F.\"Id\" "
                    + " JOIN \"BP_Istoric\" H ON A.\"Id\" = H.\"Id\"  AND  ((H.\"IdUser\" = {1} {5}) OR {1} in (select sup.\"IdUser\" FROM \"F100Supervizori\" sup where sup.\"IdSuper\" in ({6})))"
                    + " WHERE 1=1 {2} {3} ";

                sql = string.Format(sql, op, idUser, filtru, filtruGen, cmp, filtruViz, idHR);
                dt = General.IncarcaDT(sql, null);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        private string MetodeCereri(int tipActiune, List<int> ids, string explicatii, string sume)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere

            string msg = "";
            string msgValid = "";

            bool HR = false;

            //Radu 28.11.2019
            string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
            string sql = "SELECT COUNT(*) FROM \"F100Supervizori\" WHERE \"IdUser\" = {0} AND \"IdSuper\" IN ({1})";
            sql = string.Format(sql, Session["UserId"].ToString(), idHR);
            DataTable dtHR = General.IncarcaDT(sql, null);       
            if (dtHR != null && dtHR.Rows.Count > 0 && dtHR.Rows[0][0] != null && dtHR.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtHR.Rows[0][0].ToString()) > 0)
                HR = true;

            try
            {
                if (ids.Count == 0) return "Nu exista cereri pentru aceasta actiune !";

                int nr = 0;
                string[] arrExp = explicatii.Split(new string[] { "#$" }, StringSplitOptions.None);
                string[] arrSume = sume.Split(new string[] { "#$" }, StringSplitOptions.None);
                int idUser = Convert.ToInt32(Session["UserId"] ?? -99);

                string strSelect = "";
                for (int i = 0; i < ids.Count; i++)
                {
                    strSelect += " UNION SELECT " + ids[i] + " AS Id ";
                }

                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                string strSql = $@"SELECT A.*, G.F10008 {op} ' ' {op} G.F10009 AS ""NumeComplet"" ,
                                B.""Id""  AS ""IdCerere"",
                                B.""Pozitie""  AS ""PozitieIstoric"",
                                B.""Aprobat""  AS ""Aprobat"",
                                B.""IdUser""  AS ""IdUser"",
                                B.""IdAuto""  AS ""IdIst""
                                FROM({strSelect.Substring(6)}) RL
                                INNER JOIN ""BP_Prime"" A ON RL.Id = A.Id
                                LEFT JOIN ""BP_Istoric"" B ON A.""Id""=B.""Id"" AND B.""Pozitie"" = A.""Pozitie"" + 1

                                
                                LEFT JOIN ""BP_Circuit"" E ON A.""IdCircuit""=E.""IdAuto""
                                LEFT JOIN F100 G ON A.F10003=G.F10003
                                WHERE 1=1  
                                AND ((B.""IdSuper"" >= 0 and B.""IdUser"" = {idUser}) or (B.""IdSuper"" < 0 and
				                {idUser} in (select sup.""IdUser"" from ""F100Supervizori"" sup where sup.F10003 = A.F10003 AND A.""Id""=B.""Id"" and sup.""IdSuper"" = (-1) * B.""IdSuper"") ) OR
                                {idUser} in (select sup.""IdUser"" FROM ""F100Supervizori"" sup where sup.""IdSuper"" in ({idHR})) )  ";
                DataTable dtCer = General.IncarcaDT(strSql, null);


                //for (int i = 0; i <= ids.Count - 1; i++)
                for (int i = 0; i < dtCer.Rows.Count; i++)
                {
                    DataRow dr = dtCer.Rows[i];
                    int id = -99;

                    //try
                    //{
                    //    id = ids[i];
                    //}
                    //catch (Exception)
                    //{
                    //}

                    id = Convert.ToInt32(General.Nz(dr["IdCerere"], -99));

                    if (id != -99)
                    {


                        if (!HR)
                        {
                            if (Convert.ToInt32(General.Nz(dr["IdCerere"], -99)) == -99)
                            {
                                return Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["IdCerere"].ToString() + " - " + Dami.TraduCuvant("utilizatorul logat nu se regaseste pe circuit") + System.Environment.NewLine;

                            }
                        }


                        if (Convert.ToInt32(General.Nz(dr["Aprobat"], 0)) == 1)
                        {
                            return Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["IdCerere"].ToString() + " - " + Dami.TraduCuvant("nu aveti drepturi") + System.Environment.NewLine;
                        }

                        DataTable entCer = General.IncarcaDT("SELECT * FROM \"BP_Prime\" WHERE \"Id\" = " + id, null);
                        //DataTable entIst = General.IncarcaDT("SELECT * FROM \"BP_Istoric\" WHERE \"Id\" = " + id + " AND \"IdUser\" = " + Session["UserId"].ToString() + " AND \"Aprobat\" IS NULL ", null); 
                        DataTable entIst = General.IncarcaDT("SELECT * FROM \"BP_Istoric\" WHERE \"Id\" = " + id + " AND  \"Aprobat\" IS NULL ORDER BY \"Pozitie\"", null);
                        DataTable entTip = General.IncarcaDT("SELECT a.*, " + (Constante.tipBD == 1 ? "CONVERT(VARCHAR, \"WizSal_DataPlatii\", 103)" : "TO_CHAR(\"WizSal_DataPlatii\", 'dd/mm/yyyy')") + " AS \"DataPlatii\" FROM \"BP_tblTipPrima\" a WHERE \"Id\" = " 
                            + (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["IdTip"] != null && entCer.Rows[0]["IdTip"].ToString().Length > 0 ? entCer.Rows[0]["IdTip"].ToString() : "-99"), null);
                        

                        if (entIst == null || entIst.Rows.Count <= 0) continue;

                        int idStare = 2;

                        int totalCircuit = 0, pozitie = -1;
                        decimal totalNet = 0, totalBrut = 0;
                        if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["TotalCircuit"] != null && entCer.Rows[0]["TotalCircuit"].ToString().Length > 0)
                            totalCircuit = Convert.ToInt32(entCer.Rows[0]["TotalCircuit"].ToString());
                        if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["TotalNet"] != null && entCer.Rows[0]["TotalNet"].ToString().Length > 0)
                            totalNet = Convert.ToDecimal(entCer.Rows[0]["TotalNet"].ToString());
                        if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["TotalBrut"] != null && entCer.Rows[0]["TotalBrut"].ToString().Length > 0)
                            totalBrut = Convert.ToDecimal(entCer.Rows[0]["TotalBrut"].ToString());
                        if (entIst != null && entIst.Rows.Count > 0 && entIst.Rows[0]["Pozitie"] != null && entIst.Rows[0]["Pozitie"].ToString().Length > 0)
                            pozitie = Convert.ToInt32(entIst.Rows[0]["Pozitie"].ToString());

                        if (HR)
                            idStare = 3;
                        else
                            if (idStare == 2 && totalCircuit == pozitie) idStare = 3;


                        if (tipActiune == 2) idStare = 0;

                        string culoare = "";
                        DataTable entStari = General.IncarcaDT("SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare, null);
                        culoare = "#FFFFFFFF";
                        if (entStari != null && entStari.Rows.Count > 0 && entStari.Rows[0]["Culoare"] != null && entStari.Rows[0]["Culoare"].ToString().Length > 0)
                            culoare = entStari.Rows[0]["Culoare"].ToString();

               

                        string explicatie = "";
                        double sumaNeta = 0, sumaBruta = 0;
                        if (arrExp[i].Trim() != "" && arrExp[i].Trim() != "_") explicatie = arrExp[i];
                        if (arrSume[i].Trim() != "")
                        {
                            try
                            {
                                string[] arrS = arrSume[i].Split(new string[] { ";" }, StringSplitOptions.None);
                                if (arrS.Length >= 1 && arrS[0].Trim() != "" && arrS[0].Trim() != "0") sumaNeta = Convert.ToInt32(Convert.ToDecimal(arrS[0]));
                                if (arrS.Length >= 2 && arrS[1].Trim() != "" && arrS[1].Trim() != "0") sumaBruta = Convert.ToInt32(Convert.ToDecimal(arrS[1]));
                            }
                            catch (Exception) { }
                        }

        

                        int idUserInloc = 0;
                        if (entIst != null && entIst.Rows.Count > 0 && entIst.Rows[0]["IdUser"] != null && entIst.Rows[0]["IdUser"].ToString().Length > 0)
                            if (Convert.ToInt32(entIst.Rows[0]["IdUser"].ToString())  != Convert.ToInt32(Session["UserId"].ToString()))
                            {
                                idUserInloc = Convert.ToInt32(Session["UserId"].ToString());
                            }

                        int f10003 = 0;
                        if (entCer != null && entCer.Rows.Count > 0 && entCer.Rows[0]["F10003"] != null && entCer.Rows[0]["F10003"].ToString().Length > 0)
                            f10003 = Convert.ToInt32(entCer.Rows[0]["F10003"].ToString());

                        #region Validare start

                        msg = Notif.TrimiteNotificare("BP.Aprobare", 2, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""BP_Prime"" Z WHERE ""Id""=" + id, "", id, Convert.ToInt32(Session["UserId"].ToString()), f10003);

                        if (msg != "" && msg.Substring(0, 1) == "2")
                        {
                            msg += Dami.TraduCuvant(Dami.TraduCuvant(msg.Substring(2)));
                            continue;
                        }

                        #endregion

                        sql = "UPDATE \"BP_Prime\" SET \"Pozitie\" = {0}, \"IdStare\" = {1}, \"Culoare\" = '{2}', \"Explicatie\" = '{3}', \"SumaNeta\" = {4}, \"SumaBruta\" = {5} WHERE \"Id\" = {6} ";
                        sql = string.Format(sql, pozitie, idStare, culoare, explicatie, sumaNeta.ToString(new CultureInfo("en-US")), sumaBruta.ToString(new CultureInfo("en-US")), id);
                        General.ExecutaNonQuery(sql, null);

                        sql = "UPDATE X  SET \"DataAprobare\" = {0}, \"Aprobat\" = 1, \"IdStare\" = {1}, \"Culoare\" = '{2}', USER_NO = {3}, TIME = {0}, \"IdUserInlocuitor\" = {4} FROM \"BP_Istoric\" X WHERE \"Id\" = {5}  "
                            + " AND  ((IdSuper >= 0 and IdUser = {3}) or (IdSuper < 0 and {3} in (select sup.IdUser from F100Supervizori sup where sup.F10003 = (select A.F10003 from bp_prime a where A.Id = x.Id) and sup.IdSuper = (-1) * x.IdSuper) )) "
                            + " AND \"Aprobat\" IS NULL  ";
                        sql = string.Format(sql, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idStare, culoare, Session["UserId"].ToString(), (idUserInloc == 0 ? "NULL" : idUserInloc.ToString()), id);
                        General.ExecutaNonQuery(sql, null);


                        //Radu 19.08.2019 - transferul a fost mutat in functia speciala
                        if (tipActiune == 2)
                        {
                            string descModul = "Prime";
                            sql = "DELETE FROM F300 WHERE F30042 = 'WIZONE" + descModul.PadLeft(30, Convert.ToChar("_")) + id.ToString() + "'";
                            General.ExecutaNonQuery(sql, null);
                        }


                        #region  Notificare strat
                        string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };

                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                        {
                            NotifAsync.TrimiteNotificare("BP.Aprobare", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""BP_Prime"" Z WHERE ""Id""=" + id, "", id, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), arrParam);
                        });
                        //Notif.TrimiteNotificare("BP.Aprobare", 1, $@"SELECT *, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""BP_Prime"" WHERE ""Id""=" + id, "", id, Convert.ToInt32(Session["UserId"].ToString()), f10003);
                        #endregion

                        nr++;
                        
                    }
                    
                }

                if (nr > 0)
                {
                    if (tipActiune == 1)
                        msg = "S-au aprobat " + nr.ToString() + " cereri din " + ids.Count + " !";
                    else
                        msg = "S-au respins " + nr.ToString() + " cereri din " + ids.Count + " !";

                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
                }
                else
                {
                    if (msgValid != "")
                        msg = msgValid;
                    else
                        msg = "Nu exista prime pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;
        }


        public void CreazaTranzacF300(int idUser,
                                int f10003,
                                int id,
                                int idTranz,
                                DateTime dtPlatii,
                                int cantitate,
                                decimal procent,
                                decimal suma,
                                DateTime dtStart,
                                DateTime dtEnd,
                                int tipData,
                                string descModul)
        {
            try
            {

                int? idCom = null;
                int? idSub = null;
                int? idFil = null;
                int? idSec = null;
                int? idDep = null;
                int? idCC = null;

                DataTable dt100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + f10003, null);
                if (dt100 != null && dt100.Rows.Count > 0)
                {
                    idCom = Convert.ToInt32(dt100.Rows[0]["F10002"].ToString());
                    idSub = Convert.ToInt32(dt100.Rows[0]["F10004"].ToString());
                    idFil = Convert.ToInt32(dt100.Rows[0]["F10005"].ToString());
                    idSec = Convert.ToInt32(dt100.Rows[0]["F10006"].ToString());
                    idDep = Convert.ToInt32(dt100.Rows[0]["F10007"].ToString());
                    idCC = Convert.ToInt32(dt100.Rows[0]["F10053"].ToString());
                }



                int anLucru = DateTime.Now.Year;
                int lunaLucru = DateTime.Now.Month;

                DataTable dt010 = General.IncarcaDT("SELECT * FROM F010", null);
                if (dt010 != null && dt010.Rows.Count > 0)
                {
                    anLucru = Convert.ToInt32(dt010.Rows[0]["F01011"].ToString());
                    lunaLucru = Convert.ToInt32(dt010.Rows[0]["F01012"].ToString());
                }

                DateTime dtLucru = new DateTime(anLucru, lunaLucru, 1);
                //dtStart = new DateTime(anLucru, lunaLucru, 1);

                DateTime dt = new DateTime(2100, 1, 1);
                try
                {
                    switch (tipData)
                    {
                        case 1:                         //data inceput cerere
                            dt = dtStart;
                            break;
                        case 2:                         //data aprobarii
                            dt = DateTime.Now.Date;
                            break;
                        case 3:                         //luna de lucru
                            dt = new DateTime(anLucru, lunaLucru, dtPlatii.Day);
                            break;
                    }
                }
                catch (Exception)
                { }

                dtEnd = dt;

                string sql = "DELETE FROM F300 WHERE F30042 = 'WIZONE" + descModul.PadLeft(30, Convert.ToChar("_")) + id.ToString() + "'";
                General.ExecutaNonQuery(sql, null);

                sql = "INSERT INTO F300 (F30001, F30002,F30003, F30004, F30005, F30006, F30007, F30010, F30012, F30013, F30014, F30015, F30021, F30022, F30023, F30025, F30035, " 
                            + " F30011, F30036, F30037, F30038, F30039, F30040, F30041, F30042, F30043, F30044, F30045, F30046, F30050, F30051,  F30053, USER_NO, TIME) "
                            + "VALUES (300, {0}, {1}, {2}, {3}, {4}, {5}, {6}, 0, {7}, {8}, {9}, 0, 0, 0, '', {10}, {11}, {12}, {13}, {14}, 0, 0, 0, '{15}', 0, 0, 0, 0, {16}, 0, 0, 75, {17})";
                sql = string.Format(sql, idCom, f10003, idSub, idFil, idSec, idDep, idTranz, cantitate, procent.ToString(new CultureInfo("en-US")), suma.ToString(new CultureInfo("en-US")),
                                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString() + "', 103)" 
                                : "TO_DATE('" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString() + "', 'dd/mm/yyyy')"),
                                Convert.ToInt16((12 * dt.Year) + dt.Month - (12 * anLucru) - lunaLucru + 1).ToString(),
                                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString() + "', 103)"
                                : "TO_DATE('" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString() + "', 'dd/mm/yyyy')"),
                                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtLucru.Day.ToString().PadLeft(2, '0') + "/" + dtLucru.Month.ToString().PadLeft(2, '0') + "/" + dtLucru.Year.ToString() + "', 103)"
                                : "TO_DATE('" + dtLucru.Day.ToString().PadLeft(2, '0') + "/" + dtLucru.Month.ToString().PadLeft(2, '0') + "/" + dtLucru.Year.ToString() + "', 'dd/mm/yyyy')"),
                                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)"
                                : "TO_DATE('" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 'dd/mm/yyyy')"),
                                "WIZONE" + descModul.PadLeft(30, Convert.ToChar("_")) + id.ToString(), idCC, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));
                General.ExecutaNonQuery(sql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
        protected void cmbAnul_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["AprobareBP_Anul"] = Convert.ToInt32(cmbAnul.Value);
        }

        protected void cmbLuna_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["AprobareBP_Luna"] = Convert.ToInt32(cmbLuna.Value);
        }


    }
}