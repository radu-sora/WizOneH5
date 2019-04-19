using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Globalization;

namespace WizOne.CereriDiverse
{
    public partial class Cereri : System.Web.UI.Page
    {
        public class metaCereriDate
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                lblTip.InnerText = Dami.TraduCuvant("Tip Cerere");
                lblDesc.InnerText = Dami.TraduCuvant("Descriere");

                btnDocUpload.ToolTip = Dami.TraduCuvant("incarca document");
                btnDocSterge.ToolTip = Dami.TraduCuvant("sterge document");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                DataTable dtTip = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"" FROM ""MP_tblTipCerere""", null);
                cmbTip.DataSource = dtTip;
                cmbTip.DataBind();
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
                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void Iesire()
        {
            try
            {
                Response.Redirect("~/CereriDiverse/Lista.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter;
                DataTable dtAbs = new DataTable();

                switch (tip)
                {
                    case "4":               //btnSave
                        {
                            SalveazaDate(2);
                        }
                        break;
                    case "5":               //btnDocSterge
                        {
                            metaCereriDate itm = new metaCereriDate();
                            if (Session["CereriDiverse_Upload"] != null) itm = Session["CereriDiverse_Upload"] as metaCereriDate;

                            itm.UploadedFile = null;
                            itm.UploadedFileName = null;
                            itm.UploadedFileExtension = null;

                            Session["CereriDiverse_Upload"] = itm;

                            lblDoc.InnerHtml = "&nbsp;";
                        }
                        break;
                }
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
                metaCereriDate itm = new metaCereriDate();
                if (Session["CereriDiverse_Upload"] != null) itm = Session["CereriDiverse_Upload"] as metaCereriDate;

                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["CereriDiverse_Upload"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SalveazaDate(int tip=1)
        {
            try
            {
                string strErr = "";

                if (cmbTip.Value == null) strErr += ", " + Dami.TraduCuvant("tip cerere");
                if (txtDesc.Value == null) strErr += ", " + Dami.TraduCuvant("cerere");

                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);

                    return;
                }

                string sqlIst = SelectCereriIstoric(Convert.ToInt32(Session["UserId"]), -1);

                string sqlPre = @"INSERT INTO ""MP_Cereri""(""Id"", F10003, ""IdTipCerere"", ""Descriere"", ""Raspuns"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalCircuit"", ""Pozitie"", USER_NO, TIME) 
                                OUTPUT Inserted.Id, Inserted.IdStare ";

                string sqlCer = CreazaSelectCuValori();

                string strGen = "BEGIN TRAN " +
                                sqlIst + "; " +
                                sqlPre +
                                sqlCer + (Constante.tipBD == 1 ? "" : " FROM DUAL") + "; " +
                                "COMMIT TRAN";

                //Radu 18.07.2018
                string mesaj = AdaugaCerere(Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(cmbTip.Value), Convert.ToInt32(Session["User_Marca"] ?? -99), (txtDesc.Value == null ? "NULL" : "'" + txtDesc.Value.ToString() + "'"), sqlCer);



                //string msg = Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", " +  General.CurrentDate() + " AS TIME," + Session["UserId"] + " AS USER_NO , 1 AS \"Actiune\", 1 AS \"IdStareViitoare\", '' AS \"Comentarii\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                //if (msg != "" && msg.Substring(0,1) == "2")
                //{
                //    if (tip == 1)
                //        MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
                //    else
                //        pnlCtl.JSProperties["cpAlertMessage"] = msg.Substring(2);

                //    return;
                //}
                //else
                //{
                //    int idCer = 1;
                //    DataTable dtCer = new DataTable();

                //    try
                //    {
                //        dtCer = General.IncarcaDT(strGen, null);
                //    }
                //    catch (Exception ex)
                //    {
                //        General.ExecutaNonQuery("ROLLBACK TRAN", null);
                //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                //        return;
                //    }


                //    if (dtCer.Rows.Count > 0) idCer = Convert.ToInt32(dtCer.Rows[0]["Id"]);


                //    #region Adaugare atasament

                //    if (Session["CereriDiverse_Upload"] != null)
                //    {
                //        metaCereriDate itm = Session["CereriDiverse_Upload"] as metaCereriDate;
                //        if (itm.UploadedFile != null)
                //        {
                //            string sqlFis = $@"INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                //            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                //            General.ExecutaNonQuery(sqlFis, new object[] { "MP_Cereri", idCer, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
                //        }
                //    }

                //    #endregion

                //    Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""MP_Cereri"" WHERE ""Id""=" + idCer, "MP_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                //}

                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
                ASPxPanel.RedirectOnCallback("~/CereriDiverse/Lista.aspx");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string CreazaSelectCuValori()
        {
            string sqlCer = "";

            try
            {
                string idCircuit = General.Nz(cmbTip.Value, -99).ToString();
                string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""MP_Cereri"") ";
                string sqlTotal = @"(SELECT COUNT(*) FROM ""MP_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
                string sqlIdStare = @"(SELECT ""IdStare"" FROM ""MP_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""=" + sqlIdCerere + ")";
                string sqlPozitie = @"(SELECT ""Pozitie"" FROM ""MP_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""=" + sqlIdCerere + ")";
                string sqlCuloare = @"(SELECT ""Culoare"" FROM ""MP_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""=" + sqlIdCerere + ")";

                sqlCer = @"SELECT " +
                                sqlIdCerere + " AS \"Id\", " +
                                Session["User_Marca"] + " AS \"F10003\", " +
                                cmbTip.Value + " AS \"IdTipCerere\", " +
                                (txtDesc.Value == null ? "NULL" : "'" + txtDesc.Value.ToString() + "'") + " AS \"Descriere\", " +
                                "NULL AS \"Raspuns\", " +
                                (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + " AS \"IdStare\", " +
                                (idCircuit) + " AS \"IdCircuit\", " +
                                Session["UserId"] + " AS \"UserIntrod\", " +
                                (sqlCuloare == null ? "NULL" : sqlCuloare) + " AS \"Culoare\", " +
                                "NULL AS \"Inlocuitor\", " +
                                (sqlTotal == null ? "NULL" : sqlTotal) + " AS \"TotalCircuit\", " +
                                (sqlPozitie == null ? "NULL" : sqlPozitie) + " AS \"Pozitie\", " +
                                Session["UserId"] + " AS \"UserIntrod\", " + 
                                General.CurrentDate() + " AS TIME ";
                if (Constante.tipBD == 2) sqlCer += " FROM DUAL";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlCer;
        }

        private string SelectCereriIstoric(int f10003, int idInloc)
        {
            string sqlIst = "";

            try
            {
                string op = "+";
                string tipData = "nvarchar";

                if (Constante.tipBD == 2)
                {
                    op = "||";
                    tipData = "varchar2";
                }

                //exceptand primul element, selectul de mai jos intoarce un string cu toti actorii de pe circuit
                string sqlCir = $@"SELECT CAST(COALESCE(0,0) AS {tipData}(10)) {op} ';' {op} COALESCE(CAST({Session["UserId"]} AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super1"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super2"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super3"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super4"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super5"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super6"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super7"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super8"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super9"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super10"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super11"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super12"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super13"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super14"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super15"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super16"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super17"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super18"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super19"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super20"" AS {tipData}(10)) {op} ';','') FROM ""MP_CereriCircuit"" WHERE ""IdTipCerere""=" + General.Nz(cmbTip.Value, -99);
                string ids = (General.ExecutaScalar(sqlCir, null) ?? "").ToString();

                if (ids != "")
                {
                    string[] lstId = ids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    int idx = 0;
                    List<string> lstSql = new List<string>();
                    string strSql = "";

                    //cand CumulareAcelasiSupervizor este:
                    // 0 - (se rezolva punand particula ALL in UNION) se pun toti supervizorii chiar daca se repeta; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  3;   8;   3;   9;
                    // 1 - (se trateaza separat, dupa bucla for) daca uramtorul in circuit este acelasi user, se salveaza doar o singura data; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;   3;   9;
                    // 2 - (se rezolva scotand particula ALL din UNION) user-ul se salveaza doar o singura data indiferent de cate ori este pe circuit sau pe ce pozitie este;  ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;  9;
                    string paramCumul = Dami.ValoareParam("CumulareAcelasiSupervizor", "0");
                    string strUnion = "ALL";
                    if (paramCumul == "2") strUnion = "";

                    for (int i = 1; i < lstId.Count(); i++)
                    {
                        string strTmp = "";

                        //daca Supervizorul este angajatul          
                        if (lstId[i].ToString() == "0") strTmp += " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", F70102 AS \"IdUser\", " + lstId[i].ToString() + " AS \"IdSuper\", 0 AS \"Inlocuitor\" FROM USERS WHERE F10003=" + f10003;

                        //daca Supervizorul este id de utilizator                 
                        if (Convert.ToInt32(lstId[i].ToString()) > 0) strTmp += " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", " + lstId[i].ToString() + " AS \"IdUser\", 76 AS \"IdSuper\", 0 AS \"Inlocuitor\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                        //daca Supervizorul este din nommenclatorul tblSupervizori (este cu minus)
                        //se foloseste union pt a acoperi si cazul in care user-ul logat este deja un superviozr pt acest angajat;
                        if (Convert.ToInt32(lstId[i].ToString()) < 0)
                        {
                            strTmp = @" UNION {4} SELECT TOP 1 {3} AS ""Index"", ""IdUser"", {1} AS ""IdSuper"", 0 AS ""Inlocuitor"" FROM (
                                        SELECT TOP 1 ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ""IdUser"" = {2}
                                        UNION ALL
                                        SELECT TOP 1 ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1})
                                        ) x ";
                            if (Constante.tipBD == 2)
                            {
                                strTmp = @" UNION {4} SELECT {3} AS ""Index"", ""IdUser"", {1} AS ""IdSuper"", 0 AS ""Inlocuitor"" FROM (
                                    SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ""IdUser"" = {2} AND ROWNUM<=1
                                    UNION ALL
                                    SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ROWNUM<=1
                                    ) x  WHERE ROWNUM<=1";
                            }

                            strTmp = string.Format(strTmp, f10003, Convert.ToInt32(lstId[i]), HttpContext.Current.Session["UserId"], idx, strUnion);
                        }

                        idx++;
                        strSql += strTmp;
                        lstSql.Add(strTmp);

                        //inseram inlocuitorul pe a doua pozitie din circuit
                        if (idx == 1 && Convert.ToInt32(lstId[0] ?? "0") == 1 && idInloc != -1)
                        {
                            string strInloc = " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", F70102 AS \"IdUser\", -78 AS \"IdSuper\", 1 AS \"Inlocuitor\" FROM USERS WHERE F10003=" + idInloc;

                            idx++;
                            strSql += strInloc;
                            lstSql.Add(strInloc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlIst;
        }

        public string AdaugaCerere(int idUser, int idTip, int f10003, string cerere, string sqlCer)
        {
            int idUrm = -99;

            try
            {

                string msg = Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", " + General.CurrentDate() + " AS TIME," + Session["UserId"] + " AS USER_NO , 1 AS \"Actiune\", 1 AS \"IdStareViitoare\", '' AS \"Comentarii\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    if (idTip == 1)
                        MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = msg.Substring(2);

                    return "";
                }
                else
                {

                    idUrm = Convert.ToInt32(Dami.NextId("MP_Cereri"));

                    string sql = "SELECT * FROM \"MP_CereriCircuit\" WHERE \"IdTipCerere\" = " + idTip;
                    DataTable dtCircuit = General.IncarcaDT(sql, null);


                    if (dtCircuit == null || dtCircuit.Rows.Count <= 0) return "Atributul nu are circuit alocat.";

                    int idCircuit = -99;
                    idCircuit = Convert.ToInt32(dtCircuit.Rows[0]["IdAuto"].ToString());

                    sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = 1";
                    DataTable dtStari = General.IncarcaDT(sql, null);
                    string culoare = "#FFFFFFFF";
                    if (dtStari != null && dtStari.Rows.Count > 0 && dtStari.Rows[0]["Culoare"] != null && dtStari.Rows[0]["Culoare"].ToString().Length > 0)
                        culoare = dtStari.Rows[0]["Culoare"].ToString();

                    //adaugam headerul

                    int total = 0;
                    int idStare = -1;
                    int idStareCereri = 2;
                    int pozUser = 1;

                    //aflam totalul de users din circuit
                    for (int i = 1; i <= 20; i++)
                    {
                        var idSuper = (dtCircuit.Rows[0]["Super" + i.ToString()] == DBNull.Value ? null : dtCircuit.Rows[0]["Super" + i.ToString()]);
                        if (idSuper != null && Convert.ToInt32(idSuper) != -99)
                        {
                            //ne asiguram ca exista user pentru supervizorul din circuit
                            if (Convert.ToInt32(idSuper) < 0)
                            {
                                int idSpr = Convert.ToInt32(idSuper);
                                sql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = {0} AND \"IdSuper\" = {1}";
                                sql = string.Format(sql, f10003, (-1 * idSpr).ToString());
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == null || dtUser.Rows[0]["IdUser"].ToString().Length <= 0)
                                {
                                    continue;
                                }
                            }

                            total++;
                        }

                    }


                    //adaugam istoricul
                    int poz = 0;
                    int idUserCalc = -99;

                    for (int i = 1; i <= 20; i++)
                    {

                        var valId = (dtCircuit.Rows[0]["Super" + i.ToString()] == DBNull.Value ? null : dtCircuit.Rows[0]["Super" + i.ToString()]);
                        if (valId != null && Convert.ToInt32(valId) != -99)
                        {
                            //poz++;

                            //IdUser
                            if (Convert.ToInt32(valId) == 0)
                            {
                                //idUserCalc = idUser;
                                sql = "SELECT * FROM USERS WHERE F10003 = " + f10003;
                                DataTable dtUtiliz = General.IncarcaDT(sql, null);
                                if (dtUtiliz != null && dtUtiliz.Rows.Count > 0 && dtUtiliz.Rows[0]["F70102"] != null && dtUtiliz.Rows[0]["F70102"].ToString().Length > 0)
                                {
                                    idUserCalc = Convert.ToInt32(dtUtiliz.Rows[0]["F70102"].ToString());
                                }
                            }
                            if (Convert.ToInt32(valId) > 0) idUserCalc = Convert.ToInt32(valId);
                            if (Convert.ToInt32(valId) < 0)
                            {
                                int idSpr = Convert.ToInt32(valId);
                                //ne asiguram ca exista user pentru supervizorul din circuit
                                sql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = {0} AND \"IdSuper\" = {1}";
                                sql = string.Format(sql, f10003, (-1 * idSpr).ToString());
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == null || dtUser.Rows[0]["IdUser"].ToString().Length <= 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    idUserCalc = Convert.ToInt32(dtUser.Rows[0]["IdUser"].ToString());
                                }
                            }

                            poz += 1;

                            int usr = Convert.ToInt32(valId);
                            string sqlIst = "INSERT INTO \"MP_CereriIstoric\" (\"IdCerere\", \"IdCircuit\", \"IdSuper\", \"Pozitie\", \"Inlocuitor\", \"IdUser\", \"Aprobat\", \"DataAprobare\", \"IdStare\", \"Culoare\", USER_NO, TIME, \"IdTipCerere\") "
                                + " VALUES ({0}, {1}, {2}, {3}, 0, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";

                            string aprobat = "NULL";
                            string dataAprobare = "NULL";
                            idStare = -1;
                            if (idUserCalc == idUser)
                            {
                                pozUser = poz;
                                if (poz == 1) idStare = 1;
                                if (poz == total) idStare = 3;

                                idStareCereri = idStare;

                                aprobat = "1";
                                dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare;
                                DataTable dtStareIst = General.IncarcaDT(sql, null);
                                culoare = "#FFFFFFFF";
                                if (dtStareIst != null && dtStareIst.Rows.Count > 0 && dtStareIst.Rows[0]["Culoare"] != null)
                                    culoare = dtStareIst.Rows[0]["Culoare"].ToString();
                            }

                            sqlIst = string.Format(sqlIst, idUrm, idCircuit, valId.ToString(), poz, idUserCalc, aprobat, dataAprobare, (idStare < 0 ? "NULL": idStare.ToString()), (idStare < 0 ? "NULL" : "'" + culoare + "'"), idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idTip);


                            General.ExecutaNonQuery(sqlIst, null);
                        }

                    }

                    sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStareCereri;
                    DataTable dtCuloare = General.IncarcaDT(sql, null);
                    culoare = "#FFFFFFFF";
                    if (dtCuloare != null && dtCuloare.Rows.Count > 0 && dtCuloare.Rows[0]["Culoare"] != null)
                        culoare = dtCuloare.Rows[0]["Culoare"].ToString();

                    string sqlCereri = "INSERT INTO \"MP_Cereri\" (\"Id\", F10003, \"IdTipCerere\", \"Descriere\", \"Raspuns\", \"IdCircuit\", \"Pozitie\", \"UserIntrod\", \"IdStare\", \"Culoare\", \"TotalCircuit\", USER_NO, TIME) "
                        + " VALUES ({0}, {1}, {2}, {3}, NULL, {4}, {5}, {6}, {8}, '{9}', {10}, {6}, {7})";
                    sqlCereri = string.Format(sqlCereri, idUrm, f10003, idTip, cerere, idCircuit, pozUser, idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idStareCereri, culoare, total);

                    General.ExecutaNonQuery(sqlCereri, null);

                    #region  Atasamente start

                    //adauga atasamentul
                    if (Session["CereriDiverse_Upload"] != null)
                    {
                        metaCereriDate itm = Session["CereriDiverse_Upload"] as metaCereriDate;
                        if (itm.UploadedFile != null)
                        {
                            string sqlFis = $@"INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                            //General.ExecutaNonQuery(sqlFis, new object[] { "MP_Cereri", idUrm, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
                            General.ExecutaNonQuery(sqlFis, new object[] { "MP_Cereri", idUrm, itm.UploadedFile, itm.UploadedFileName, "." + itm.UploadedFileName.ToString().Split('.')[itm.UploadedFileName.ToString().Split('.').Length - 1], Session["UserId"] });
                        }
                    }


                    Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""MP_Cereri"" WHERE ""Id""=" + idUrm, "MP_Cereri", idUrm, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                    #endregion
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return "Proces finalizat cu succes.";
        }


    }
}