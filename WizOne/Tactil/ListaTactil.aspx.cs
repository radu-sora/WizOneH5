using DevExpress.Web;
using DevExpress.XtraReports.UI;
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

namespace WizOne.Tactil
{
    public partial class ListaTactil : System.Web.UI.Page
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
                Session["PaginaWeb"] = "Tactil.ListaTactil";
                Session["Absente_Cereri_Date"] = null;

                DataTable dtAbs = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ", null);
                GridViewDataComboBoxColumn colAbs = (grDate.Columns["IdAbsenta"] as GridViewDataComboBoxColumn);
                colAbs.PropertiesComboBox.DataSource = dtAbs;

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;


                grDate.JSProperties["cpParamMotiv"] = Dami.ValoareParam("AfisareMotivLaRespingereCerere","0");

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();

                //string sFont = Dami.ValoareParam("GridFontSize", "0");
                //if (sFont != "0" && General.IsNumeric(sFont)) grDate.Font.Size = FontUnit.Point(Convert.ToInt32(sFont));

                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY13", "CloseDeferedWindow();", true);
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
                if (Constante.esteTactil)
                    Dami.AccesTactil();
                else
                    Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                 
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


                if (!IsPostBack)
                {
                    DataTable dt = General.IncarcaDT($@"SELECT ""Rol"" AS ""Id"", ""RolDenumire"" AS ""Denumire"" FROM (
                            SELECT ""Rol"", ""RolDenumire"", 1 AS ""Ordin"" FROM ({Dami.SelectCereri()}) X GROUP BY ""Rol"", ""RolDenumire""
                            UNION 
                            SELECT -1 AS ""Rol"", 'Toate' AS ""RolDenumire"", 0 AS ""Ordin"" {General.FromDual()}) Y ORDER BY ""Ordin"" ", null);


                    string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                    string sqlHr = $@"SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                    DataTable dtHr = General.IncarcaDT(sqlHr, null);

                    //determinam daca este angajat sau manager pentru a selecta in cmbViz
                    DataTable dtViz = General.IncarcaDT(@"SELECT * FROM ""F100Supervizori"" WHERE ""IdUser""=@1", new object[] { Session["UserId"] });
    

                    bool esteHr = false;
                    if (dtHr != null && dtHr.Rows.Count > 0) esteHr = true;
                    if (esteHr)
                    {
                        ListEditItem itm = new ListEditItem();
                        itm.Text = "Toti angajatii";
                        itm.Value = 3;
                   
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
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                }
                General.MemoreazaEroarea("Vine din Tactil");
                if (ids.Count != 0) msg += General.MetodeCereri(1, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                //MessageBox.Show(msg, MessageBox.icoWarning, "");
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
                //List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
                //string msg = "";
                //List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "Actiune", "NumeAngajat", "DataInceput", "Rol" });
                //if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                //for (int i = 0; i < lst.Count(); i++)
                //{
                //    object[] arr = lst[i] as object[];
                //    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                //    {
                //        case -1:
                //            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                //            continue;
                //        case 0:
                //            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                //            continue;
                //        case 3:
                //            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                //            continue;
                //        case 4:
                //            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                //            continue;
                //    }

                //    ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                //}

                //if (ids.Count != 0) msg += General.MetodeCereri(2, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                ////MessageBox.Show(msg, MessageBox.icoWarning, "");
                //grDate.JSProperties["cpAlertMessage"] = msg;
                //grDate.DataBind();

                //grDate.Selection.UnselectAll();

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
       

                string sqlFinal = Dami.SelectCereri(-99) + $@" {filtru} ORDER BY A.TIME DESC";
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Insuficienti parametrii");
                        //MessageBox.Show("Insuficienti parametrii", MessageBox.icoError, "Eroare !");
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
                                    //MessageBox.Show(Dami.TraduCuvant("Nu exista linie selectata"), MessageBox.icoWarning, "");
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
                                    //MessageBox.Show(Dami.TraduCuvant("Nu puteti anula o cerere respinsa"), MessageBox.icoWarning, "");
                                    return;
                                }

                                DataRow drAbs = General.IncarcaDR(General.SelectAbsente(obj[1].ToString(), Convert.ToDateTime(obj[4]).Date, Convert.ToInt32(obj[2])), null);
                                if (drAbs != null)
                                {
                                    if (Convert.ToInt32(General.Nz(Session["IdClient"], "-99")) == Convert.ToInt32(IdClienti.Clienti.Groupama))
                                    {
                                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D1" && General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() != "D2")
                                        {
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Puteti anula numai cerereile cu tip de absenta Delegatie");
                                            //MessageBox.Show(Dami.TraduCuvant("Puteti anula numai cerereile cu tip de absenta Delegatie"), MessageBox.icoWarning, "");
                                            return;
                                        }

                                        if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "CO" && Convert.ToInt32(drAbs["CampBifa1"]) == 1)
                                        {
                                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima");
                                            //MessageBox.Show(Dami.TraduCuvant("Nu puteti anula o cerere pentru care s-a cerut prima"), MessageBox.icoWarning, "");
                                            return;
                                        }
                                    }

                                    if ((obj[1] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["Anulare"], 0)) == 0 && idStare != 4)
                                    {
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta");
                                        //MessageBox.Show(Dami.TraduCuvant("Angajatul nu are drepturi pentru a anula acest tip de absenta"), MessageBox.icoWarning, "");
                                        return;
                                    }

                                    if ((obj[1] ?? -99).ToString() == General.VarSession("User_Marca").ToString() && Convert.ToInt32(General.Nz(drAbs["AnulareAltii"], 0)) == 0)
                                    {
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta");
                                        //MessageBox.Show(Dami.TraduCuvant("Persoanele din circuit nu au dreptul de a anula acest tip de absenta"), MessageBox.icoWarning, "");
                                        return;
                                    }
                                }

                                //daca este hr nu se aplica regulile
                                if (Convert.ToInt32(obj[10] ?? 0) != 77)
                                {
                                    DateTime ziDrp = Dami.DataDrepturi(Convert.ToInt32(General.Nz(obj[8], -99)), Convert.ToInt32(General.Nz(obj[9], 0)), Convert.ToDateTime(obj[4]), Convert.ToInt32(obj[1]));
                                    if (Convert.ToDateTime(obj[4]).Date < ziDrp)
                                    {
                                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat") + " " + ziDrp.Date.ToShortDateString();
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
                                        //introducem o linie de anulare in Ptj_CereriIstoric
                                        //Florin 2018.09.20
                                        //s-a schimbat IdSuper din Session["UserId"] in Convert.ToInt32(obj[10] ?? 0)
                                        string sqlIst = $@"INSERT INTO ""Ptj_CereriIstoric""
                                                    (""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                                    SELECT ""Id"", ""IdCircuit"", {-1 * Convert.ToInt32(General.Nz(obj[10], 0))}, -1, {Session["UserId"]}, 22, 1, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()}, 0, null, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1) FROM ""Ptj_Cereri"" WHERE ""Id""={obj[0]};";

                                        string sqlCer = $@"UPDATE ""Ptj_Cereri"" SET ""IdStare"" =-1, ""Culoare"" =(SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" =-1) WHERE ""Id"" ={obj[0]};";

                                        //Florin 2018.04.04
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
                                    General.StergeInPontaj(Convert.ToInt32(obj[0]), idTipOre, oreInVal, Convert.ToDateTime(obj[4]), Convert.ToDateTime(obj[6]), Convert.ToInt32(obj[1]), Convert.ToInt32(General.Nz(obj[7], 0)), Convert.ToInt32(General.Nz(Session["UserId"], -99)));

                                General.CalculFormuleCumulat($@"ent.F10003 = {obj[1]} AND ent.""An""={Convert.ToDateTime(obj[4]).Year} AND ent.""Luna""={Convert.ToDateTime(obj[4]).Month}");
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
                                    //MessageBox.Show(Dami.TraduCuvant("Nu exista linie selectata"), MessageBox.icoWarning, "");
                                    return;
                                }

                                if (Convert.ToInt32(General.Nz(obj[4], -99)) != 4)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu este cerere planificata");
                                    //MessageBox.Show(Dami.TraduCuvant("Nu este cerere planificata"), MessageBox.icoWarning, Dami.TraduCuvant("Operatie nepermisa") + " !");
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

                                    string strGen = "BEGIN TRAN " +
                                        sqlDel + "; " +
                                        sqlPlf + "; " +
                                        sqlIst + "; " +
                                        sqlUp + "; " +
                                        "COMMIT TRAN";

                                    DataTable dtCer = General.IncarcaDT(strGen, null);
                                    //General.ExecutaNonQuery(strGen,null);

                                    //trimite in pontaj daca este finalizat
                                    if (Convert.ToInt32(dtCer.Rows[0]["IdStare"]) == 3)
                                    {
                                        if ((Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 0 && General.Nz(drAbs["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(drAbs["NuTrimiteInPontaj"], 0)) == 0)
                                            General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(General.Nz(obj[0],1)), 5, trimiteLaInlocuitor, Convert.ToDecimal(General.Nz(obj[5],0)));

                                        General.CalculFormule(obj[1], null, Convert.ToDateTime(obj[6]), Convert.ToDateTime(obj[7]));
                                    }

                                    Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 2 AS ""Actiune"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + obj[0], "Ptj_Cereri", Convert.ToInt32(obj[0]), Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                                    grDate.DataBind();
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
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
                        case "btnPrint":
                            Session["IdCerereTactil"] = Convert.ToInt32(arr[1]);

                            DataTable dt = General.IncarcaDT($@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""='Ptj_Cereri' AND ""Id""={(Session["IdCerereTactil"] ?? "0")} AND ""EsteCerere""=1", null);
                            if (dt == null || dt.Rows.Count <= 0)
                            {
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista date de afisat!");
                            }
                            else
                            {

                                Reports.PrintCereri dlreport = new Reports.PrintCereri();
                                dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                                dlreport.Margins.Top = 10;
                                dlreport.Margins.Bottom = 10;
                                dlreport.Margins.Left = 50;
                                dlreport.Margins.Right = 50;
                                dlreport.PrintingSystem.ShowMarginsWarning = false;
                                dlreport.ShowPrintMarginsWarning = false;

                                //Florin 2020.05.22
                                string numeImprimanta = Dami.ValoareParam("TactilImprimantaFluturas").Trim();
                                if (numeImprimanta == "")
                                    numeImprimanta = Dami.ValoareParam("TactilImprimanta").Trim();
                                if (numeImprimanta != "")
                                    dlreport.PrinterName = numeImprimanta;

                                dlreport.CreateDocument();
                                ReportPrintTool pt = new ReportPrintTool(dlreport);
                                pt.Print();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "F10003", "IdAbsenta", "Inlocuitor", "DataInceput", "DataSfarsit", "IdStare" }) as object[];
                if (obj == null || obj.Count() == 0 || obj[0] == null || obj[1] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista linie selectata"), MessageBox.icoWarning, "");
                    return;
                }

                //if (Convert.ToInt32(obj[6]) == -1 || Convert.ToInt32(obj[6]) == 0)
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Starea cererii nu permite divizarea"), MessageBox.icoWarning, "");
                //    Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY13", "popUpDivide.Hide();", true);
                //    return;
                //}

                //TimeSpan? ts = Convert.ToDateTime(obj[5]) - Convert.ToDateTime(obj[4]);
                //if (ts.Value.TotalDays < 1)
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Intervalul ales trebuie sa fie de minim 2 zile."), MessageBox.icoWarning, "");
                //    return;
                //}

                //if (txtDataDivide.Value == null)
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Lipseste data cu care se va divide cererea"), MessageBox.icoWarning, "");
                //    return;
                //}

                //if (!(Convert.ToDateTime(obj[4]) <= Convert.ToDateTime(txtDataDivide.Value) && Convert.ToDateTime(txtDataDivide.Value) <= Convert.ToDateTime(obj[5])))
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Data nu este in intervalul din cerere"), MessageBox.icoWarning, "");
                //    return;
                //}

                DateTime dtSplit = new DateTime(2100, 1, 1);

                int adunaZL = 0;
                DataRow drAbs = General.IncarcaDR(General.SelectAbsente(obj[1].ToString(), Convert.ToDateTime(obj[4]).Date, Convert.ToInt32(obj[2])), null);
                if (drAbs != null) adunaZL = Convert.ToInt32(General.Nz(drAbs["AdunaZileLibere"], 0));

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
                General.CalcZile(dtIncOri, dtSfOri, adunaZL.ToString(), out nr, out nrViitor);

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
                General.CalcZile(dtIncDes, dtSfDes, adunaZL.ToString(), out nr, out nrViitor);

                string sqlDes = $@"INSERT INTO ""Ptj_Cereri""(F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", USER_NO, TIME, ""Id"", ""TrimiteLa"", ""IdCerereDivizata"", ""Comentarii"", ""NrOre"")
                                OUTPUT Inserted.Id                                
                                SELECT F10003, ""IdAbsenta"", {General.ToDataUniv(dtIncDes)} AS""DataInceput"", {General.ToDataUniv(dtSfDes)} AS ""DataSfarsit"", {nr} AS ""NrZile"", {nrViitor} AS ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", {Session["UserId"]}, {General.CurrentDate()}, {sqlIdCerere} AS ""Id"", ""TrimiteLa"", {obj[0]} AS ""IdCerereDivizata"", ""Comentarii"", ""NrOre"" FROM ""Ptj_Cereri"" WHERE ""Id""={obj[0]}";

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
                }

                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY13", "CloseDeferedWindow();", true);
                //popUpDivide.ShowOnPageLoad = false;
                
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

                ASPxComboBox cmbCps = grDate.FindEditFormTemplateControl("cmbCps") as ASPxComboBox;
                if (cmbCps != null && cmbCps.SelectedIndex != -1 && cmbCps.Value != null) cps = cmbCps.Value;

                ASPxComboBox cmbInl = grDate.FindEditFormTemplateControl("cmbInl") as ASPxComboBox;
                if (cmbInl != null && cmbInl.SelectedIndex != -1 && cmbInl.Value != null) inl = cmbInl.Value;

                General.ExecutaNonQuery($@"UPDATE ""Ptj_Cereri"" SET ""Observatii""=@1, ""Comentarii""=@2, ""TrimiteLa""=@3, ""Inlocuitor""=@4 WHERE ""Id""=@5", 
                    new object[] { e.NewValues["Observatii"], e.NewValues["Comentarii"], cps, inl, id });

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

                Session["IstoricExtins_VineDin"] = 1;
                Session["grDate_Filtru"] = "Absente.Lista;" + grDate.FilterExpression;
                Response.Redirect("~/Absente/IstoricExtins", false);
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

                    //e.Editor.Visible = false;
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

                //if (e.ButtonID == "btnDelete")
                //{
                //    bool esteRol = false;
                //    bool esteStare = false;

                //    object row = ((ASPxGridView)sender).GetRow(e.VisibleIndex);
                //    if (row == null) return;
                //    string id = ((DataRowView)row)["IdStare"].ToString();
                //    string rol = ((DataRowView)row)["Rol"].ToString();

                //    string strStari = Dami.ValoareParam("AnulareCerere_Stari");
                //    string strRoluri = Dami.ValoareParam("AnulareCerere_Roluri");

                //    if (strStari != "")
                //    {
                //        string[] arr = strStari.Split(',');
                //        object ent = arr.FirstOrDefault(p => p == id);
                //        if (ent != null) esteStare = true;
                //    }
                //    if (strRoluri != "")
                //    {
                //        string[] arr = strRoluri.Split(',');
                //        object ent = arr.FirstOrDefault(p => p == rol);
                //        if (ent != null) esteRol = true;
                //    }
                    
                //    //if ((id == "1" || id == "2") && rol != "0")
                //    if (esteRol && esteStare)
                //        e.Enabled = true;
                //    else
                //        e.Enabled = false;
                //}
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
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Compensare", "CompensareBanca", "CompensarePlata", "CompensareBancaDenumire", "CompensarePlataDenumire", "Inlocuitor", "TrimiteLa" }) as object[];


                ASPxLabel lbl = grDate.FindEditFormTemplateControl("lblInl") as ASPxLabel;
                ASPxComboBox cmb = grDate.FindEditFormTemplateControl("cmbInl") as ASPxComboBox;

                if (Dami.ValoareParam("InlocuitorEditabilInAprobare", "0") == "1")
                {
                    DataTable dtInl = General.IncarcaDT(General.SelectInlocuitori(-55, new DateTime(1900, 1, 1), new DateTime(2200, 1, 1)), null);
                    cmb.DataSource = dtInl;
                    if (Convert.ToInt32(General.Nz(obj[5], -1)) == -1)
                        cmb.Value = null;
                    else
                        cmb.Value = Convert.ToInt32(General.Nz(obj[5],-1));

                    lbl.Visible = true;
                    cmb.Visible = true;
                }
                else
                {
                    lbl.Visible = false;
                    cmb.Visible = false;
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
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
                }
                General.MemoreazaEroarea("Vine din Tactil");
                if (ids.Count != 0) msg += General.MetodeCereri(2, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), motiv);
                //MessageBox.Show(msg, MessageBox.icoWarning, "");
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Tactil/Main", false);    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                //Radu 24.04.2020
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                if (tip == "0")
                    Response.Redirect("../DefaultTactil", false);
                else if (tip == "1" || tip == "2")
                    Response.Redirect("../DefaultTactilFaraCard", false);
                else if (tip == "3")
                    Response.Redirect("../DefaultTactilExtra", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}