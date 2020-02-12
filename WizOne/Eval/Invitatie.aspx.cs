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

namespace WizOne.Eval
{
    public partial class Invitatie : System.Web.UI.Page
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
                //Florin 2020.02.12
                //string sqlUsr = $@"SELECT F70102, CASE WHEN ""NumeComplet"" IS NULL THEN F70104 ELSE ""NumeComplet"" END AS ""NumeComplet"", F10003 FROM USERS";
                string sqlUsr = 
                    $@"SELECT F70102, CASE WHEN ""NumeComplet"" IS NULL THEN F70104 ELSE ""NumeComplet"" END AS ""NumeComplet"", F10003 FROM USERS WHERE F10003 IS NULL
                    UNION
                    SELECT A.F70102, CASE WHEN A.""NumeComplet"" IS NULL THEN A.F70104 ELSE A.""NumeComplet"" END AS ""NumeComplet"", A.F10003 
                    FROM USERS A 
                    INNER JOIN F100 B ON A.F10003=B.F10003
                    WHERE A.F10003 IS NOT NULL AND B.F10023 >= CAST({General.CurrentDate()} AS DATE) AND B.F10025 <> 900
                    ORDER BY ""NumeComplet"" ";
                DataTable dtUsr = General.IncarcaDT(sqlUsr, null);
                cmbUsr.DataSource = dtUsr;
                cmbUsr.DataBind();

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                if (Dami.ValoareParam("Eval_AprobareInvitatie") == "1")
                {
                    btnAproba.Visible = false;
                    btnRespinge.Visible = false;
                }
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
                Session["PaginaWeb"] = "Eval.Invitatie";

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnTrimite.Text = Dami.TraduCuvant("btnTrimite", "Trimite Invitatie");

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                
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

                if (Request["pp"] != null)
                    txtTitlu.Text = "Prima Pagina - Evaluare 360";
                else
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                int esteHr = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR})", null), 0));
                int manager = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper""=1", null), 0));

                if (esteHr > 0 || (manager > 0 && Convert.ToInt32(General.Nz(Session["IdClient"], -99)) != (int)IdClienti.Clienti.Pelifilip))
                {
                    btnAproba.Visible = true;
                    btnRespinge.Visible = true;
                    pnlStare.Visible = true;

                    cmbPar.DataSource = cmbUsr.DataSource;
                    cmbPar.DataBind();
                    cmbPar.Value = Convert.ToInt32(Session["UserId"]);
                }
                else
                {
                    string sqlUsr = $@"SELECT F70102, CASE WHEN ""NumeComplet"" IS NULL THEN F70104 ELSE ""NumeComplet"" END AS ""NumeComplet"", F10003 FROM USERS WHERE F70102=@1";
                    DataTable dtUsr = General.IncarcaDT(sqlUsr, new object[] { Session["UserId"] });
                    cmbPar.DataSource = dtUsr;
                    cmbPar.DataBind();
                    cmbPar.SelectedIndex = 0;
                }


                if (!IsPostBack)
                {
                    IncarcaGrid();
                }
                else
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnTrimite_Click(object sender, EventArgs e)
        {
            //rbTip.Value
            //1 - Invitatie
            //2 - Autoinvitatie


            try
            {
                if (btnAproba.Visible == false)
                {
                    if (cmbUsr.Value == null)
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date");
                        return;
                    }
                }
                else
                {
                    if (cmbUsr.Value == null || cmbPar.Value == null)
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date");
                        return;
                    }
                }

                //verificam sa nu participe la propria evaluare; asta se face din momentul initializarii
                if (btnAproba.Visible == false)
                {
                    if (General.Nz(Session["UserId"], -99).ToString() == General.Nz(cmbUsr.Value, -98).ToString())
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Participantul si user-ul este una si aceeasi persoana");
                        return;
                    }
                }
                else
                {
                    if (General.Nz(cmbPar.Value, -99).ToString() == General.Nz(cmbUsr.Value, -98).ToString())
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Participantul si user-ul este una si aceeasi persoana");
                        return;
                    }
                }

                int idUsr = -99;
                int f10003 = -99;
                int idQuiz = -99;

                if (btnAproba.Visible == false)
                {
                    if ((rbTip.Value ?? 1).ToString() == "1")
                    {
                        idUsr = Convert.ToInt32(cmbUsr.Value ?? -99);
                        f10003 = Convert.ToInt32(Session["User_Marca"] ?? -99);
                    }
                    else
                    {
                        idUsr = Convert.ToInt32(Session["UserId"] ?? -99);
                        f10003 = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT F10003 FROM USERS WHERE F70102=" + Convert.ToInt32(cmbUsr.Value ?? -99), null), -99));
                    }
                }
                else
                {
                    if ((rbTip.Value ?? 1).ToString() == "1")
                    {
                        idUsr = Convert.ToInt32(cmbUsr.Value ?? -99);
                        f10003 = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT F10003 FROM USERS WHERE F70102=" + Convert.ToInt32(cmbPar.Value ?? -99), null), -99));
                    }
                    else
                    {
                        idUsr = Convert.ToInt32(cmbPar.Value ?? -99);
                        f10003 = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT F10003 FROM USERS WHERE F70102=" + Convert.ToInt32(cmbUsr.Value ?? -99), null), -99));
                    }
                }

                int idUsrQuiz = Convert.ToInt32(Session["UserId"] ?? -99);
                if ((rbTip.Value ?? 1).ToString() == "2")
                    idUsrQuiz = Convert.ToInt32(cmbUsr.Value ?? -99);


                string sqlSel = @"SELECT TOP 1 A.Id
                    FROM Eval_Quiz A
                    INNER JOIN Eval_relGrupAngajatQuiz B ON A.Id=B.IdQuiz
                    INNER JOIN Eval_SetAngajatiDetail C ON B.IdGrup=C.IdSetAng
                    INNER JOIN USERS D ON C.Id=D.F10003
                    WHERE A.CategorieQuiz = @1 AND D.F70102 = @2 AND COALESCE(A.""Activ"",0)=1";

                if (Constante.tipBD == 2)
                    sqlSel = @"SELECT A.""Id""
                    FROM ""Eval_Quiz"" A
                    INNER JOIN ""Eval_relGrupAngajatQuiz"" B ON A.""Id""=B.""IdQuiz""
                    INNER JOIN ""Eval_SetAngajatiDetail"" C ON B.""IdGrup""=C.""IdSetAng""
                    INNER JOIN USERS D ON C.Id=D.F10003
                    WHERE A.""CategorieQuiz"" = @1 AND D.F70102 = @2 AND COALESCE(A.""Activ"",0)=1 AND ROWNUM <= 1";

                DataTable dtQuiz = General.IncarcaDT(sqlSel, new object[] { cmbTip.Value, idUsrQuiz });

                if (dtQuiz != null && dtQuiz.Rows.Count > 0) idQuiz = Convert.ToInt32(General.Nz(dtQuiz.Rows[0]["Id"],-99));

                //verificam ca sesiunea de evaluare pe chestionarul selectat sa fie deschisa
                int esteDeschis = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""Eval_Raspuns"" WHERE F10003=@1 AND ""IdQuiz""=@2", new object[] { f10003, idQuiz }));
                if (esteDeschis == 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Sesiunea de evaluare pentru acest chestionar nu este deschisa");
                    return;
                }


                if (f10003 == -99)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Persoana selectata nu are marca");
                    return;
                }

                int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM ""Eval_Invitatie360"" WHERE ""IdUser""={idUsr} AND ""F10003""={f10003} AND ""IdQuiz""={idQuiz} AND ""IdStare""<>-1", null),0));
                if (cnt != 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Invitatie existenta");
                    return;
                }


                string strSql = "";
                int idStare = 1;

                if (Dami.ValoareParam("Eval_AprobareInvitatie") == "1")
                {
                    strSql += $@"INSERT INTO ""Eval_RaspunsIstoric""(""IdQuiz"", F10003, ""IdSuper"", ""IdUser"", ""Pozitie"") VALUES({idQuiz}, {f10003}, ({DamiRol()}), {idUsr}, (SELECT COALESCE(MAX(COALESCE(""Pozitie"",0)),0) + 1 FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {idQuiz} AND F10003 = {f10003}));" + System.Environment.NewLine;
                    idStare = 3;

                    strSql += $@"INSERT INTO ""Eval_Invitatie360""(""IdUser"", ""F10003"", ""IdQuiz"", ""IdStare"", ""IdTip"", USER_NO, TIME) VALUES({idUsr}, {f10003}, {idQuiz}, {idStare}, {rbTip.Value}, {Session["UserId"]}, GetDate()); " + System.Environment.NewLine;
                }
                else
                {
                    if ((rbTip.Value ?? "").ToString() == "1")
                    {
                        strSql += $@"INSERT INTO ""Eval_RaspunsIstoric""(""IdQuiz"", F10003, ""IdSuper"", ""IdUser"", ""Pozitie"") VALUES({idQuiz}, {f10003}, ({DamiRol()}), {idUsr}, (SELECT COALESCE(MAX(COALESCE(""Pozitie"",0)),0) + 1 FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {idQuiz} AND F10003 = {f10003}));" + System.Environment.NewLine;
                        idStare = 3;
                    }
                    strSql += $@"INSERT INTO ""Eval_Invitatie360""(""IdUser"", ""F10003"", ""IdQuiz"", ""IdStare"", ""IdTip"", USER_NO, TIME) VALUES({idUsr}, {f10003}, {idQuiz}, {idStare}, {rbTip.Value}, {Session["UserId"]}, GetDate()); " + System.Environment.NewLine;
                }


                bool ras = General.ExecutaNonQuery("BEGIN " + strSql + " END;", null);
                if (ras)
                {
                    cmbUsr.SelectedIndex = -1;

                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
                    IncarcaGrid();

                    Notif.TrimiteNotificare("Eval.Invitatie", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, -99 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Eval_Invitatie360"" Z WHERE ""IdUser""={idUsr} AND F10003={f10003} AND ""IdQuiz""={idQuiz}", "Eval_Invitatie360", 1, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                }
                else
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Eroare la procesare");
                }

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
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string CreazaSelect(int f10003, int idUser, int tipQuiz=-99)
        {
            string strSql = "";

            try
            {
                strSql = $@"SELECT CASE WHEN B.F10003 IS NULL THEN B.F70104 ELSE E.F10008 {Dami.Operator()} ' ' {Dami.Operator()} E.F10009 END AS ""User"", 
                        C.F10008 {Dami.Operator()} ' ' {Dami.Operator()} C.F10009 AS ""Evaluat"", D.""Denumire"" AS ""Quiz"", 
                        CASE WHEN A.""IdTip""=1 AND A.F10003={Session["User_Marca"]} THEN 'Invitat de catre mine' ELSE
                        CASE WHEN A.""IdTip""=1 AND A.""IdUser""={Session["UserId"]} THEN 'Eu sunt invitat' ELSE
                        CASE WHEN A.""IdTip""=2 AND A.F10003={Session["User_Marca"]} THEN 'Coleg autoinvitat' ELSE
                        CASE WHEN A.""IdTip""=2 AND A.""IdUser""={Session["UserId"]} THEN 'Eu dau feedback autoinvitat' ELSE '' END END END END AS ""Tip"",
                        A.*
                        FROM ""Eval_Invitatie360"" A
                        INNER JOIN USERS B ON A.""IdUser""=B.F70102
                        LEFT JOIN F100 E ON B.F10003=E.F10003
                        INNER JOIN F100 C ON A.F10003=C.F10003
                        INNER JOIN ""Eval_Quiz"" D ON A.""IdQuiz""=D.""Id""
                        WHERE (A.""IdUser""={idUser} OR A.F10003={f10003})";

                if (tipQuiz != -99)
                    strSql += @" AND D.""CategorieQuiz"" =" + tipQuiz;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                int f10003 = Convert.ToInt32(General.Nz(Session["User_Marca"],-99));
                int idUser = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                int tipQuiz = 2;

                if (General.Nz(cmbTip.Value, "").ToString() != "") tipQuiz = Convert.ToInt32(General.Nz(cmbTip.Value,-99));

                if (General.Nz(cmbPar.Value, "").ToString() != "")
                {
                    ListEditItem li = cmbPar.SelectedItem;
                    string usr_f10003 = General.Nz(li.GetFieldValue("F10003"), -99).ToString();
                    idUser = Convert.ToInt32(General.Nz(cmbPar.Value,-99));
                    f10003 = Convert.ToInt32(General.Nz(usr_f10003,-99));
                }

                dt = General.IncarcaDT(CreazaSelect(f10003, idUser, tipQuiz) + " ORDER BY A.TIME DESC", null);
                grDate.KeyFieldName = "IdUser; F10003; IdQuiz";

                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdUser"], dt.Columns["F10003"], dt.Columns["IdQuiz"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();

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
                switch (e.Parameters)
                {
                    case "btnDelete":
                        {
                            #region Sterge

                            object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdUser", "F10003", "IdQuiz", "IdStare" }) as object[];
                            if (obj == null || obj.Count() == 0)
                            {
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata");
                                return;
                            }

                            int idStare = Convert.ToInt32(obj[3] ?? -1);

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

                            string cmp = "";
                            string poz = "";
                            int aprobat = 0;

                            DataTable dt = General.IncarcaDT($@"SELECT ""Pozitie"", ""Aprobat"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""={obj[2]} AND F10003={obj[1]} AND ""IdUser""={obj[0]}", null);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                poz = General.Nz(dt.Rows[0]["Pozitie"],"").ToString();
                                aprobat = Convert.ToInt32(General.Nz(dt.Rows[0]["Aprobat"],0));
                            }

                            if (aprobat == 1)
                            {
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Aceasta invitatie nu mai poate fi stearsa deoarece a fost deja finalizata");
                                return;
                            }

                            if (poz != "")
                            {
                                cmp = $@"UPDATE ""Eval_RaspunsLinii"" SET ""Super{poz}""=null, ""Super{poz}_1""=null, ""Super{poz}_2""=null, ""Super{poz}_3""=null, 
                                    ""Super{poz}_4""=null, ""Super{poz}_5""=null, ""Super{poz}_6""=null 
                                    WHERE F10003={obj[1]} AND ""IdQuiz""={obj[2]};";

                                cmp += $@"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdQuiz""={obj[2]} AND F10003={obj[1]} AND ""Pozitie""={poz}; ";
                            }

                            string strSql = $@"BEGIN
                                            DELETE ""Eval_Invitatie360"" WHERE ""IdUser""={obj[0]} AND F10003={obj[1]} AND ""IdQuiz""={obj[2]};
                                            {cmp}
                                            DELETE ""Eval_RaspunsIstoric"" WHERE ""IdUser""={obj[0]} AND F10003={obj[1]} AND ""IdQuiz""={obj[2]};
                                            END";

                            General.ExecutaNonQuery(strSql, null);

                            IncarcaGrid();

                            #endregion 
                        }
                        break;
                    case "btnRespinge":
                        Metode(0);
                        break;
                    case "btnAproba":
                        Metode(3);
                        break;
                    case "btnTrimite":
                        btnTrimite_Click(null, null);
                        break;
                    case "btnFiltru":
                        btnFiltru_Click(null, null);
                        break;
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

        private void Metode(int tip)
        {
            //tip
            //0 - respins
            //3 - aprobat
            try
            {
                List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "IdUser", "F10003", "IdQuiz", "IdStare", "Evaluat" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[3], 0)))
                    {
                        case -1:
                            msg += "Cererea pt " + arr[4] + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += "Cererea pt " + arr[4] + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            msg += "Cererea pt " + arr[4] + " - " + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
                            continue;
                        default:
                            {
                                string sqlUpd = $@"UPDATE ""Eval_Invitatie360"" SET ""IdStare""={tip} WHERE ""IdUser""={arr[0]} AND F10003={arr[1]} AND ""IdQuiz""={arr[2]};";
                                string sqlIns = "";
                                if (tip == 3) sqlIns = $@"INSERT INTO ""Eval_RaspunsIstoric""(""IdQuiz"", F10003, ""IdSuper"", ""IdUser"", ""Pozitie"") VALUES({arr[2]}, {arr[1]}, ({DamiRol()}), {arr[0]}, (SELECT COALESCE(MAX(COALESCE(""Pozitie"",0)),0) + 1 FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {arr[2]} AND F10003 = {arr[1]}));";
                                string sqlGen = "BEGIN " + "\n\r" +
                                           sqlUpd + "\n\r" +
                                           sqlIns + "\n\r" +
                                           "END;";
                                General.ExecutaNonQuery(sqlGen, null);
                                string txt = "respinsa";
                                if (tip == 3) txt = "aprobata";
                                msg += "Cererea pt " + arr[4] + " - " + Dami.TraduCuvant("a fost " + txt) + System.Environment.NewLine;

                                Notif.TrimiteNotificare("Eval.Invitatie", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, {tip} AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Eval_Invitatie360"" Z WHERE ""IdUser""={arr[0]} AND F10003={arr[1]} AND ""IdQuiz""={arr[2]}", "Eval_Invitatie360", 1, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                            }
                            break;
                    }
                }

                grDate.JSProperties["cpAlertMessage"] = msg;
                IncarcaGrid();

                grDate.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string DamiRol()
        {
            string strSql = "";
            //1 - manager
            //2 - subordonat
            //3 - coleg

            try
            {
                string top = "";
                string rowNum = "";
                if (Constante.tipBD == 1)
                    top = "TOP 1";
                else
                    rowNum = " WHERE ROWNUM<=1 ";

                string idSuper = Dami.ValoareParam("Eval_IdSuperManager", "1");
                strSql = $@"
                    SELECT {top} X.IdRol FROM
                    (
                    SELECT 'Manager' AS Rol, 1 AS IdRol FROM F100Supervizori WHERE F10003={Session["User_Marca"]} AND IdSuper={idSuper} AND IdUser={cmbUsr.Value ?? -99}
                    UNION
                    SELECT 'Subordonat' AS Rol, 2 AS IdRol FROM F100Supervizori WHERE F10003=(SELECT F10003 FROM USERS WHERE F70102 = {cmbUsr.Value ?? -99}) AND IdSuper={idSuper} AND IdUser={Session["UserId"]}
                    UNION
                    SELECT 'Coleg' AS Rol, 3 AS IdRol {General.FromDual()}
                    ) X {rowNum} ORDER BY X.IdRol";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

    }
}