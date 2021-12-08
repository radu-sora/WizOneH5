using ProceseSec;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;


namespace WizOne.Module
{
    public class Notif
    {
        public class metaAdreseMail
        {
            public string Mail { get; set; }
            public string Destinatie { get; set; }
            public int IncludeLinkAprobare { get; set; }
        }

        internal static string TrimiteNotificare(string numePagina, int tipNotificare, string strSelect, string tblAtasamente_Tabela, int tblAtasamente_Id, int userId, int userMarca)
        {
            string rez = "";

            try
            {
                string strSql = @"SELECT A.* FROM ""Ntf_Setari"" A WHERE A.""Pagina""=@1 AND A.""TipNotificare""=@2 AND A.""Activ""=1 AND 
                            (A.""ValidTip""=0 OR
                            (A.""ValidTip""=1 AND (SELECT COUNT(*) FROM ""relGrupUser"" X WHERE X.""IdUser""=@3 AND X.""IdGrup""=A.""ValidVal"")>0) OR 
                             A.""ValidTip""=2 AND A.""ValidVal""=@3)";

                DataTable dtReg = General.IncarcaDT(strSql, new object[] { numePagina, tipNotificare, userId });
                if (dtReg.Rows.Count == 0)
                {
                    if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strSql.Replace("@1","'" + numePagina + "'").Replace("@2","1").Replace("@3", userId.ToString()), "SelectReguli");
                }
                else
                {
                    for (int i = 0; i < dtReg.Rows.Count; i++)
                    {
                        string ntf_Campuri = "";
                        string ntf_Conditii = "";
                
                        CreazaSelect(Convert.ToInt32(dtReg.Rows[i]["Id"]), strSelect, numePagina, userId, userMarca, out ntf_Campuri, out ntf_Conditii);
                        if (ntf_Campuri == "" || ntf_Conditii == "") continue;
                        string strFiltru = "SELECT * FROM (" + strSelect + ") ent WHERE 1 = 1 " + ntf_Conditii;
                        strFiltru = strFiltru.Replace("GLOBAL.MARCA", userMarca.ToString()).Replace("GLOBAL.IDUSER", userId.ToString());
                        DataTable dtFiltru = new DataTable();
                        try
                        {
                            dtFiltru = General.IncarcaDT(strFiltru, null);
                        }
                        catch (Exception ex)
                        {
                            General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
                        }
                        if (dtFiltru.Rows.Count != 0)
                        {
                            string strSele = "SELECT " + ntf_Campuri + " FROM (" + strSelect + ") ent WHERE 1 = 1 " + ntf_Conditii;
                            strSele = strSele.Replace("GLOBAL.MARCA", userMarca.ToString()).Replace("GLOBAL.IDUSER", userId.ToString());
                            DataTable dtSel = General.IncarcaDT(strSele, null);
                            switch (tipNotificare)
                            {
                                case 1:                                 //Notificare
                                    {
                                        string corpAtt = "";
                                        string numeAtt = "";
                                        string selectXls = "";
                                        string numeExcel = "";

                                        if (Convert.ToInt32(General.Nz(dtReg.Rows[i]["SalveazaInDisc"],0)) == 1 || Convert.ToInt32(General.Nz(dtReg.Rows[i]["SalveazaInBaza"],0)) == 1 || Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimitePeMail"],0)) == 1)
                                        {
                                            corpAtt = InlocuiesteCampuri((dtReg.Rows[i]["ContinutAtasament"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca);
                                            numeAtt = InlocuiesteCampuri(General.Nz(dtReg.Rows[i]["NumeAtasament"], "Atasament_" + DateTime.Now.Ticks.ToString()).ToString(), dtSel.Rows[0], userId, userMarca) + ".html";

                                            if (Convert.ToInt32(dtReg.Rows[i]["SalveazaInBaza"]) == 1)
                                            {
                                                SalveazaInBaza(numeAtt, corpAtt, tblAtasamente_Tabela, tblAtasamente_Id, userId);
                                            }

                                            if (Convert.ToInt32(dtReg.Rows[i]["SalveazaInDisc"]) == 1)
                                            {
                                                if (!Directory.Exists(HostingEnvironment.MapPath("~/FisiereNotif/"))) Directory.CreateDirectory(HostingEnvironment.MapPath("~/FisiereNotif/"));
                                                
                                                StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/FisiereNotif/") + numeAtt, true);
                                                //
                                                sw.Write(CreazaHTML(corpAtt));
                                                //
                                                sw.Close();
                                                sw.Dispose();
                                            }

                                            if (Convert.ToInt32(dtReg.Rows[i]["TrimitePeMail"]) == 1)
                                            {
                                                //NOP
                                                //se face in procedura de mai jos  TrimiteMail
                                            }
                                        }

                                        if (Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteXLS"], 0)) == 1 || (dtReg.Rows[i]["TrimiteXLS"] ?? "").ToString() != "")
                                        {
                                            numeExcel = InlocuiesteCampuri(General.Nz(dtReg.Rows[i]["NumeExcel"], "Atasament_" + DateTime.Now.Ticks.ToString()).ToString(), dtSel.Rows[0], userId, userMarca) + ".xls";
                                            selectXls = InlocuiesteCampuri((dtReg.Rows[i]["SelectXLS"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca);
                                        }

                                        List<metaAdreseMail> lstAdrese = CreazaAdreseMail(Convert.ToInt32(dtReg.Rows[i]["Id"]), dtSel.Rows[0]);

                                        if (lstAdrese.Count > 0)
                                        {
                                            string subiect = InlocuiesteCampuri((dtReg.Rows[i]["Subiect"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca);
                                            int idClient = Convert.ToInt32(HttpContext.Current.Session["IdClient"]);

                                            //Florin 2018.05.09
                                            if (lstAdrese.Where(p => p.IncludeLinkAprobare == 1).Count() == 0)
                                            {
                                                string corpMail = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca, numePagina, tblAtasamente_Id);
                                                TrimiteMail(lstAdrese, subiect, corpMail, Convert.ToInt32(dtReg.Rows[i]["TrimitePeMail"]), numeAtt, CreazaHTML(corpAtt), Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteXLS"], 0)), selectXls, numeExcel, idClient);
                                            }
                                            else
                                            {
                                                for (int j = 0; j < lstAdrese.Count(); j++)
                                                {
                                                    List<metaAdreseMail> lstOne = new List<metaAdreseMail>();
                                                    lstOne.Add(new metaAdreseMail { Mail=lstAdrese[j].Mail, Destinatie="TO", IncludeLinkAprobare=0 });

                                                    string corpMail = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca, numePagina, tblAtasamente_Id, lstAdrese[j].Mail, lstAdrese[j].IncludeLinkAprobare);
                                                    TrimiteMail(lstOne, subiect, corpMail, Convert.ToInt32(dtReg.Rows[i]["TrimitePeMail"]), numeAtt, CreazaHTML(corpAtt), Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteXLS"], 0)), selectXls, numeExcel, idClient);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog("Nu exista adrese de mail", "VerificaDate");
                                        }

                                        //Radu 07.05.2021
                                        if (Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteICS"], 0)) > 0)
                                        {
                                            DateTime dtStart = new DateTime(2100, 1, 1);     
                                            DateTime dtSf = new DateTime(2100, 1, 1);
                                            try
                                            {
                                                DataTable dtUser = General.IncarcaDT("SELECT * FROM USERS WHERE F10003 = " + userMarca, null);                                                
                                                if (dtUser != null && dtUser.Rows.Count > 0 && dtUser.Rows[0]["Mail"] != null && dtUser.Rows[0]["Mail"].ToString().Length > 0)
                                                {
                                                    string tipData = "";
                                                    bool esteCamp = false;

                                                    dtStart = Convert.ToDateTime(DamiValTabela(numePagina, "grDate", 1, dtReg.Rows[i]["DataInceputICS"].ToString().Replace("#$", "").Replace("$#", ""), dtFiltru, userId, userMarca, out tipData, out esteCamp));
                                                    dtSf = Convert.ToDateTime(DamiValTabela(numePagina, "grDate", 1, dtReg.Rows[i]["DataSfarsitICS"].ToString().Replace("#$", "").Replace("#$", ""), dtFiltru, userId, userMarca, out tipData, out esteCamp));
                                                    string subiectICS = General.Nz(dtReg.Rows[i]["SubiectICS"], "").ToString();
                                                    string corpICS = General.Nz(dtReg.Rows[i]["CorpICS"], "").ToString();  

                                                    //Radu 15.02.2019
                                                    DateTime? oraStart = null;
                                                    DateTime? oraSfarsit = null;
                                                    if (dtReg.Rows[i]["OraInceputICS"] != null && dtReg.Rows[i]["OraInceputICS"].ToString().Length > 0 && dtReg.Rows[i]["OraSfarsitICS"] != null && dtReg.Rows[i]["OraSfarsitICS"].ToString().Length > 0)
                                                    {
                                                        if (dtReg.Rows[i]["OraInceputICS"].ToString().Contains(":"))
                                                            oraStart = new DateTime(2100, 1, 1, Convert.ToInt32(dtReg.Rows[i]["OraInceputICS"].ToString().Substring(0, 2)), Convert.ToInt32(dtReg.Rows[i]["OraInceputICS"].ToString().Substring(3, 2)), 0);
                                                        else
                                                        {
                                                            string ora = DamiValTabela(numePagina, "grDate", 1, dtReg.Rows[i]["OraInceputICS"].ToString().Replace("#$", "").Replace("$#", ""), dtFiltru, userId, userMarca, out tipData, out esteCamp).ToString();
                                                            oraStart = new DateTime(2100, 1, 1, Convert.ToInt32(ora.Substring(0, 2)), Convert.ToInt32(ora.Substring(3, 2)), 0);
                                                        }
                                                        if (dtReg.Rows[i]["OraSfarsitICS"].ToString().Contains(":"))
                                                            oraSfarsit = new DateTime(2100, 1, 1, Convert.ToInt32(dtReg.Rows[i]["OraSfarsitICS"].ToString().Substring(0, 2)), Convert.ToInt32(dtReg.Rows[i]["OraSfarsitICS"].ToString().Substring(3, 2)), 0);
                                                        else
                                                        {
                                                            string ora = DamiValTabela(numePagina, "grDate", 1, dtReg.Rows[i]["OraSfarsitICS"].ToString().Replace("#$", "").Replace("$#", ""), dtFiltru, userId, userMarca, out tipData, out esteCamp).ToString();
                                                            oraSfarsit = new DateTime(2100, 1, 1, Convert.ToInt32(ora.Substring(0, 2)), Convert.ToInt32(ora.Substring(3, 2)), 0);
                                                        }
                                                    }

                                                    CreazaCalendarOutlook2(dtUser.Rows[0]["Mail"].ToString(), dtStart, dtSf, subiectICS, corpICS, oraStart == null ? "" : oraStart.Value.Hour.ToString().PadLeft(2, '0') +
                                                                          ":" + oraStart.Value.Minute.ToString().PadLeft(2, '0'), oraSfarsit == null ? "" : oraSfarsit.Value.Hour.ToString().PadLeft(2, '0') +
                                                                           ":" + oraSfarsit.Value.Minute.ToString().PadLeft(2, '0'));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
                                            }
                                        }

                                    }
                                    break;
                                case 2:                                 //Validare
                                    {
                                        string corpMsg = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca);
                                        if (corpMsg != "")
                                        {
                                            if ((dtReg.Rows[i]["mesaj"] ?? "avertisment").ToString() == "avertisment")
                                            {
                                                rez = corpMsg;  //Radu 15.04.2019
                                                string msg = "<script type='text/javascript'> " +
                                                            " swal({  title: 'Validare',    " +
                                                            "         text: '" + corpMsg + "', " +
                                                            "         type: '" + MessageBox.icoWarning + "', " +
                                                            "         showCancelButton: false, " +
                                                            "         confirmButtonText: 'OK', " +
                                                            "         closeOnConfirm: false },  " +
                                                            "         function(isConfirm){  " +
                                                            "         if (isConfirm) { document.getElementById('btnBack').click(); } }); " +
                                                            "         </script>";

                                                Page pagina = HttpContext.Current.Handler as Page;
                                                pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", msg);
                                            }
                                            else
                                            {
                                                rez += "2;" + corpMsg;
                                                string subiect = InlocuiesteCampuri((dtReg.Rows[i]["Subiect"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca);
                                                MessageBox.Show(corpMsg, MessageBox.icoError, subiect);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog("Conditie neindeplinita, vezi selectul", "CreazaSelectul");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static void CreazaSelect(int id, string strSelect, string numePagina, int userId, int userMarca, out string ntf_Campuri, out string ntf_Conditii)
        {
            string strCamp = "";
            string strCond = "";

            try
            {
                //cream filtrul
                
                DataTable dtCond = General.IncarcaDT(@"SELECT * FROM ""Ntf_Conditii"" WHERE ""Id""=@1", new string[] { id.ToString() });
                DataTable dtCmp = General.IncarcaDT(@"SELECT * FROM ""Ntf_tblCampuri"" WHERE ""Pagina""=@1", new string[] { numePagina });
                foreach (DataRow dr in dtCond.Rows)
                {
                    if ((dr["Coloana"] ?? "").ToString() != "" && (dr["Operator"] ?? "").ToString() != "")
                    {
                        DataRow[] lstTip = dtCmp.Select("Alias='" + dr["Coloana"] + "'");
                        if (lstTip.Count() == 0) continue;
                        string col = (lstTip[0]["CampSelect"] ?? "").ToString();
                        if (col == "") continue;
                        string tipData = "string";
                        if ((lstTip[0]["TipData"] ?? "").ToString() != "") tipData = (lstTip[0]["TipData"] ?? "").ToString();

                        if (dr["Operator"].ToString() != "fara valoare" && dr["Operator"].ToString() != "cu valoare" && (dr["Valoare1"] ?? "").ToString() == "") continue;

                        if (tipData == "string")
                        {
                            switch (dr["Operator"].ToString())
                            {
                                case "incepe cu":
                                    {
                                        string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                        if (Constante.tipBD == 1)
                                        {
                                            if (arr[1] == "3")
                                                strCond += " AND ((" + col + ") LIKE (" + arr[0] + ") + '%')";
                                            else
                                                strCond += " AND ((" + col + ") LIKE '" + arr[0] + "' + '%')";
                                        }
                                        else
                                        {
                                            if (arr[1] == "3")
                                                strCond += " AND ((" + col + ") LIKE (" + arr[0] + ") || '%')";
                                            else
                                                strCond += " AND ((" + col + ") LIKE '" + arr[0] + "' || '%')";
                                        }
                                    }
                                    break;
                                case "contine":
                                    {
                                        string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                        if (Constante.tipBD == 1)
                                        {
                                            if (arr[1] == "3")
                                                strCond += " AND ((" + col + ") LIKE '%' + (" + arr[0] + ") + '%')";
                                            else
                                                strCond += " AND ((" + col + ") LIKE '%' + '" + arr[0] + "' + '%')";
                                        }
                                        else
                                        {
                                            if (arr[1] == "3")
                                                strCond += " AND ((" + col + ") LIKE '%' || (" + arr[0] + ") || '%')";
                                            else
                                                strCond += " AND ((" + col + ") LIKE '%' || '" + arr[0] + "' || '%')";

                                        }
                                    }
                                    break;
                                case "se termina cu":
                                    {
                                        string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                        if (Constante.tipBD == 1)
                                        {
                                            if (arr[1] == "3")
                                                strCond += " AND ((" + col + ") LIKE '%' + (" + arr[0] + "))";
                                            else
                                                strCond += " AND ((" + col + ") LIKE '%' + '" + arr[0] + "')";
                                        }
                                        else
                                        {
                                            if (arr[1] == "3")
                                                strCond += " AND ((" + col + ") LIKE '%' || (" + arr[0] + "))";
                                            else
                                                strCond += " AND ((" + col + ") LIKE '%' || '" + arr[0] + "')";
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (dr["Operator"].ToString())
                            {
                                case "fara valoare":
                                    strCond += " AND (" + col + ") IS NULL";
                                    break;
                                case "cu valoare":
                                    strCond += " AND (" + col + ") IS NOT NULL";
                                    break;
                                case "in":
                                    {
                                        string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                        strCond += " AND ((" + col + ") IN (" + arr[0] + "))";
                                    }
                                    break;
                                case "not in":
                                    {
                                        string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                        strCond += " AND ((" + col + ") NOT IN (" + arr[0] + "))";
                                    }
                                    break;
                                case "intre":
                                    {
                                        if ((dr["Valoare1"] ?? "").ToString() != "" && (dr["Valoare2"] ?? "").ToString() != "")
                                        {
                                            string[] arr1 = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                            string[] arr2 = AflaValoarea(dr["Valoare2"].ToString(), dtCmp);
                                            if (tipData == "int")
                                                strCond += " AND ((" + arr1[0] + ") <= (" + col + ") AND (" + col + ") <= (" + arr2[0] + "))";
                                            else
                                            {
                                                if (tipData == "datetime")
                                                {
                                                    if (Constante.tipBD == 1)
                                                        strCond += " AND CONVERT(date,(DATEADD(d," + General.Nz(dr["NrZile1"], 0) + ",(" + arr1[0] + ")))) <= CONVERT(date,(" + col + ")) AND CONVERT(date,(" + col + ")) <= CONVERT(date,DATEADD(d," + General.Nz(dr["NrZile2"], 0) + ",(" + arr2[0] + ")))";
                                                    else
                                                        strCond += " AND ((TRUNC(" + arr1[0] + ") + " + General.Nz(dr["NrZile1"], 0) + ") <= (" + col + ") AND (" + col + ") <= (TRUNC(" + arr2[0] + ") + " + General.Nz(dr["NrZile2"], 0) + "))";
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "<>":
                                case ">":
                                case ">=":
                                case "<":
                                case "<=":
                                case "=":
                                    {
                                        string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                        strCond += " AND ((" + col + ")" + dr["Operator"] + "(" + arr[0] + "))";
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strCond, "CreazaFiltrul");

                //cream campurile             
                for (int i = 0; i < dtCmp.Rows.Count; i++)
                {
                    if ((dtCmp.Rows[i]["CampSelect"] ?? "").ToString() != "" && (dtCmp.Rows[i]["Alias"] ?? "").ToString() != "")
                        strCamp += ", (" + dtCmp.Rows[i]["CampSelect"] + ") AS \"" + dtCmp.Rows[i]["Alias"] + "\"";
                }

                if (strCamp != "") strCamp = strCamp.Substring(1);
                if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strCamp, "CreazaCampurile");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            ntf_Campuri = strCamp;
            ntf_Conditii = strCond;
        }

        private static string[] AflaValoarea(string camp, DataTable dtCmp, int nrZile = 0)
        {
            //1 - este valoare
            //2 - este cuvant rezervat
            //3 - este din tabela

            string[] str = { camp, "1" };

            try
            {
                //verificam daca este cuvant rezervat al aplicatiei
                string arr = "ieri, astazi, maine, prima zi din saptamana, ultima zi din saptamana, prima zi din luna, ultima zi din luna, prima zi din an, ultima zi din an";
                if (arr.IndexOf(camp.ToLower()) >= 0)
                {
                    DateTime dt = DateTime.Now;

                    switch (camp.ToLower())
                    {
                        case "ieri":
                            dt = DateTime.Now.AddDays(-1);
                            break;
                        case "astazi":
                            dt = DateTime.Now;
                            break;
                        case "maine":
                            dt = DateTime.Now.AddDays(1);
                            break;
                        case "prima zi din saptamana":
                            {
                                int ziSapt = Dami.ZiSapt(DateTime.Now.DayOfWeek.ToString());
                                dt = DateTime.Now.AddDays(1 - ziSapt);
                            }
                            break;
                        case "ultima zi din saptamana":
                            {
                                int ziSapt = Dami.ZiSapt(DateTime.Now.DayOfWeek.ToString());
                                dt = DateTime.Now.AddDays(7 - ziSapt);
                            }
                            break;
                        case "prima zi din luna":
                            dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            break;
                        case "ultima zi din luna":
                            {
                                DateTime tmp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                dt = tmp.AddMonths(1).AddDays(-1);
                            }
                            break;
                        case "prima zi din an":
                            dt = new DateTime(DateTime.Now.Year, 1, 1);
                            break;
                        case "ultima zi din an":
                            dt = new DateTime(DateTime.Now.Year, 12, 31);
                            break;
                    }

                    dt = dt.AddDays(nrZile);

                    if (Constante.tipBD == 1)
                        str[0] = "'" + dt.Year + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Day.ToString().PadLeft(2, '0') + "'";
                    else
                        str[0] = "to_date('" + dt.Day.ToString().PadLeft(2, '0') + "-" + Dami.NumeLuna(dt.Month, 1, "EN").ToUpper() + "-" + dt.Year.ToString() + "','DD-MM-YYYY')";

                    str[1] = "2";
                }
                else
                {
                    //verificam daca este camp din tabela Ntf_tblCampuri
                    DataRow[] lst = dtCmp.Select("Alias='" + camp + "'");
                    if (lst.Count() > 0 && lst[0]["CampSelect"] != null && lst[0]["CampSelect"].ToString() != "")
                    {
                        str[0] = (lst[0]["CampSelect"] ?? "").ToString();
                        str[1] = "3";
                    }
                    else
                    {
                        //daca nu este nici cuvant rezervat, nu se gaseste nici in tabela de campuri atunci este valoare
                        //NOP; s-a facut la inceput metodei prin asignarea string str = camp
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }

        public static List<metaAdreseMail> CreazaAdreseMail(int id, DataRow dtSel)
        {
            List<metaAdreseMail> lst = new List<metaAdreseMail>();

            try
            {
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ntf_Mailuri"" WHERE ""Id""=@1 {General.FiltrulCuNull("MailTip")} {General.FiltrulCuNull("MailAdresaId")} ", new string[] { id.ToString() });

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        switch (dt.Rows[i]["MailTip"].ToString())
                        {
                            case "mail":
                                lst.Add(new metaAdreseMail { Mail = dt.Rows[i]["MailAdresaId"].ToString(), Destinatie = dt.Rows[i]["MailDestinatie"].ToString(), IncludeLinkAprobare = Convert.ToInt32(General.Nz(dt.Rows[i]["IncludeLinkAprobare"],0)) });
                                break;
                            case "angajat":
                                {
                                    string mail = General.ExecutaScalar(@"SELECT CASE WHEN (A.F100894 IS NULL OR A.F100894 = '') THEN B.""Mail"" ELSE A.F100894 END AS ""Mail"" FROM F100 A
                                                                    LEFT JOIN USERS B ON A.F10003=B.F10003
                                                                    WHERE A.F10003=@1", new string[] { dt.Rows[i]["MailAdresaId"].ToString() }).ToString();
                                    lst.Add(new metaAdreseMail { Mail = mail, Destinatie = dt.Rows[i]["MailDestinatie"].ToString(), IncludeLinkAprobare = Convert.ToInt32(General.Nz(dt.Rows[i]["IncludeLinkAprobare"], 0)) });
                                }
                                break;
                            case "user":
                                {
                                    string mail = General.ExecutaScalar(@"SELECT CASE WHEN (A.""Mail"" IS NULL OR A.""Mail"" = '') THEN B.F100894 ELSE A.""Mail"" END AS ""Mail"" FROM USERS A
                                                                    LEFT JOIN F100 B ON A.F10003=B.F10003
                                                                    WHERE A.F70102=@1", new string[] { dt.Rows[i]["MailAdresaId"].ToString() }).ToString();
                                    lst.Add(new metaAdreseMail { Mail = mail, Destinatie = dt.Rows[i]["MailDestinatie"].ToString(), IncludeLinkAprobare = Convert.ToInt32(General.Nz(dt.Rows[i]["IncludeLinkAprobare"], 0)) });
                                }
                                break;
                            case "grup angajati":
                                {
                                    DataTable dtMail = General.IncarcaDT(@"SELECT CASE WHEN (A.F100894 IS NULL OR A.F100894 = '') THEN C.""Mail"" ELSE A.F100894 END AS ""Mail"" 
                                                                    FROM F100 A
                                                                    INNER JOIN ""relGrupAngajat"" B ON A.F10003=B.F10003
                                                                    LEFT JOIN USERS C ON B.F10003=C.F10003
                                                                    WHERE B.""IdGrup""=@1 
                                                                    AND (CASE WHEN (A.F100894 IS NULL OR A.F100894 = '') THEN C.""Mail"" ELSE A.F100894 END) IS NOT NULL 
                                                                    AND (CASE WHEN (A.F100894 IS NULL OR A.F100894 = '') THEN C.""Mail"" ELSE A.F100894 END) != '' ", new string[] { dt.Rows[i]["MailAdresaId"].ToString() });
                                    for(int x = 0; x < dtMail.Rows.Count; x++)
                                    {
                                        lst.Add(new metaAdreseMail { Mail = dtMail.Rows[x]["Mail"].ToString(), Destinatie = dt.Rows[i]["MailDestinatie"].ToString(), IncludeLinkAprobare = Convert.ToInt32(General.Nz(dt.Rows[i]["IncludeLinkAprobare"], 0)) });
                                    }
                                }
                                break;
                            case "grup user":
                                {
                                    DataTable dtMail = General.IncarcaDT(@"SELECT CASE WHEN (A.""Mail"" IS NULL OR A.""Mail"" = '') THEN C.F100894 ELSE A.""Mail"" END AS ""Mail"" 
                                                                    FROM USERS A
                                                                    INNER JOIN ""relGrupUser"" B ON A.F70102=B.""IdUser""
                                                                    LEFT JOIN F100 C ON A.F10003=C.F10003
                                                                    WHERE B.""IdGrup""=@1 
                                                                    AND (CASE WHEN (A.""Mail"" IS NULL OR A.""Mail"" = '') THEN C.F100894 ELSE A.""Mail"" END) IS NOT NULL 
                                                                    AND (CASE WHEN (A.""Mail"" IS NULL OR A.""Mail"" = '') THEN C.F100894 ELSE A.""Mail"" END) != ''", new string[] { dt.Rows[i]["MailAdresaId"].ToString() });
                                    for (int x = 0; x < dtMail.Rows.Count; x++)
                                    {
                                        lst.Add(new metaAdreseMail { Mail = dtMail.Rows[x]["Mail"].ToString(), Destinatie = dt.Rows[i]["MailDestinatie"].ToString(), IncludeLinkAprobare = Convert.ToInt32(General.Nz(dt.Rows[i]["IncludeLinkAprobare"], 0)) });
                                    }
                                }
                                break;
                            case "coloana tabel":
                                {
                                    string str = (dtSel[dt.Rows[i]["MailAdresaText"].ToString()] ?? "").ToString();
                                    string[] lstMail = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    for(int x = 0; x < lstMail.Count(); x++)
                                    {
                                        lst.Add(new metaAdreseMail { Mail = lstMail[x], Destinatie = dt.Rows[i]["MailDestinatie"].ToString(), IncludeLinkAprobare = Convert.ToInt32(General.Nz(dt.Rows[i]["IncludeLinkAprobare"], 0)) });
                                    }
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                }

                if (Dami.ValoareParam("LogNotificari") == "1")
                {
                    string msg = "";
                    var ert = lst.ToString();
                    for (int y = 0; y < lst.Count; y++)
                    {
                        msg += lst[y].Destinatie.PadRight(6, ' ') + " - " + lst[y].Mail + "\n\r";
                    }

                    General.CreazaLog(msg, "CreazaAdreseMail");
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lst;

        }

        public static string InlocuiesteCampuri(string text, DataRow drSel, int userId, int userMarca, string numePagina = "", int id = -99, string lstAdr = "", int inlocLinkAprobare = 0)
        {
            string str = text;

            try
            {
                string strSelect = "";
                string strOriginal = "";

                for (int i = 0; i < drSel.Table.Columns.Count; i++)
                {
                    str = str.Replace("#$" + drSel.Table.Columns[i] + "$#", General.Nz(drSel[drSel.Table.Columns[i]],"").ToString());
                }

                int z = 1;
                if (str.IndexOf("Link1") >= 0)
                {
                    do
                    {
                        int pozFirst = str.Substring(0, str.IndexOf("Link1")).LastIndexOf("#$");
                        string cuv = str.Substring(pozFirst, str.Substring(pozFirst).IndexOf("$#"));
                        string cuvOriginal = cuv;

                        if (cuv != "") cuv = cuv.Replace("Link1 ", "").Replace("Link1", "").Replace("#$", "").Replace("$#", "").Trim();

                        if ((numePagina.IndexOf("Absente.Lista") >= 0 || numePagina.IndexOf("Pontaj.PontajEchipa") >= 0 || numePagina.IndexOf("Pontaj.PontajDetaliat") >= 0) && id != -99 && lstAdr != "" && inlocLinkAprobare == 1)
                        {
                            string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/1/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + HttpContext.Current.Session["IdClient"].ToString().PadLeft(8, '0') + "/" + numePagina;

                            string rsp = General.Encrypt_QueryString(arg);
                            string hostUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + VirtualPathUtility.ToAbsolute("~/");
                            string lnk = "<a href='" + hostUrl + "/Raspuns.aspx?arg=" + rsp + "' target='_blank'>" + cuv + "</a>";
                            str = str.Replace(cuvOriginal + "$#", lnk).ToString();
                        }
                        else
                            str = str.Replace("#$Link1 " + cuv + "$#", "").ToString();

                        z++;
                        if (z == 10) break;
                    } while (str.IndexOf("Link1") >= 0);
                }

                z = 1;
                if (str.IndexOf("Link2") >= 0)
                {
                    do
                    {
                        int pozFirst = str.Substring(0, str.IndexOf("Link2")).LastIndexOf("#$");
                        string cuv = str.Substring(pozFirst, str.Substring(pozFirst).IndexOf("$#"));
                        string cuvOriginal = cuv;

                        if (cuv != "") cuv = cuv.Replace("Link2 ", "").Replace("Link2", "").Replace("#$", "").Replace("$#", "").Trim();

                        if ((numePagina.IndexOf("Absente.Lista") >= 0 || numePagina.IndexOf("Pontaj.PontajEchipa") >= 0 || numePagina.IndexOf("Pontaj.PontajDetaliat") >= 0) && id != -99 && lstAdr != "" && inlocLinkAprobare == 1)
                        {
                            string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/2/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + HttpContext.Current.Session["IdClient"].ToString().PadLeft(8, '0') + "/" + numePagina;

                            string rsp = General.Encrypt_QueryString(arg);
                            string hostUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + VirtualPathUtility.ToAbsolute("~/");
                            string lnk = "<a href='" + hostUrl + "/Raspuns.aspx?arg=" + rsp + "' target='_blank'>" + cuv + "</a>";
                            str = str.Replace(cuvOriginal + "$#", lnk).ToString();
                        }
                        else
                            str = str.Replace("#$Link2 " + cuv + "$#", "").ToString();

                        z++;
                        if (z == 10) break;
                    } while (str.IndexOf("Link2") >= 0);
                }

                //cautam daca avem de inserat tabel
                while (str.ToLower().IndexOf("#$select") >= 0)
                {
                    int start = str.ToLower().IndexOf("#$select");
                    strSelect = str.Substring(start, str.Substring(start).IndexOf("$#")).Replace("#$", "");
                    strOriginal = strSelect;
                    strSelect = WebUtility.HtmlDecode(strSelect);
                    strSelect = strSelect.Replace("GLOBAL.MARCA", userMarca.ToString()).Replace("GLOBAL.IDUSER", userId.ToString());

                    DataTable dtTbl = General.IncarcaDT(WebUtility.HtmlDecode(strSelect), null);
                    string tbl = "";
                    tbl += @"<table style=""border: solid 1px #ccc; width:100%;"">" + Environment.NewLine;


                    //adaugam capul de tabel
                    tbl += @"<thead style=""background-color:lightblue;"">" + Environment.NewLine;
                    tbl += "<tr>" + Environment.NewLine;
                    for (int x = 0; x < dtTbl.Columns.Count; x++)
                    {
                        tbl += "<td>" + dtTbl.Columns[x].ColumnName + "</td>" + Environment.NewLine;
                    }
                    tbl += "</tr>" + Environment.NewLine;
                    tbl += @"</thead>" + Environment.NewLine;


                    //adaugam corpul tabelului
                    tbl += @"<tbody>" + Environment.NewLine;
                    for (int x = 0; x < dtTbl.Rows.Count; x++)
                    {
                        tbl += "<tr>" + Environment.NewLine;
                        for (int y = 0; y < dtTbl.Columns.Count; y++)
                        {
                            tbl += @"<td style=""border: solid 1px #ccc;"">" + dtTbl.Rows[x][dtTbl.Columns[y]] + "</td>" + Environment.NewLine;
                        }
                        tbl += "</tr>" + Environment.NewLine;
                    }
                    tbl += @"</tbody>" + Environment.NewLine;


                    tbl += "</table>" + Environment.NewLine;

                    str = str.Replace("#$" + strOriginal + "$#", tbl);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }

        public static void TrimiteMail(List<metaAdreseMail> lstAdr, string subiect, string corpMail, int trimiteAtt, string numeAtt, string corpAtt, int trimiteXls, string selectXls, string numeExcel, int idClient, MemoryStream mem  = null) //Radu 29.06.2020 - am adaugat mem
        {        

            try
            {
                string folosesteCred = Dami.ValoareParam("TrimiteMailCuCredentiale");
                string cuSSL = Dami.ValoareParam("TrimiteMailCuSSL", "false");

                string smtpMailFrom = Dami.ValoareParam("SmtpMailFrom");
                string smtpServer = Dami.ValoareParam("SmtpServer");
                string smtpPort = Dami.ValoareParam("SmtpPort");
                string smtpMail = Dami.ValoareParam("SmtpMail");
                string smtpParola = Dami.ValoareParam("SmtpParola");


                string strMsg = "";
                if (smtpMailFrom == "") strMsg += ", mail from";

                if (smtpServer == "") strMsg += ", serverul de smtp";
                if (smtpPort == "") strMsg += ", smtp port";
                if (folosesteCred == "1" || folosesteCred == "2")
                {
                    if (smtpMail == "") strMsg += ", smtp mail";
                    if (smtpParola == "") strMsg += ", smtp parola";
                }

                if (strMsg != "")
                {
                    General.MemoreazaEroarea("Nu exista date despre " + strMsg.Substring(2), "Notif", "TrimiteMail");
                    return;
                }

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(smtpMailFrom);

                if (lstAdr == null || lstAdr.Count() == 0)
                {
                    General.MemoreazaEroarea(Dami.TraduCuvant("Nu exista destinatar"), "Dami", "TrimiteMail");
                    return;
                }
                else
                {
                    foreach (var mail in lstAdr)
                    {
                        string mailPt = mail.Mail;
                        if (idClient == Convert.ToInt32(IdClienti.Clienti.Honeywell))
                            mailPt = "<" + mail.Mail + ">";

                        switch (mail.Destinatie.ToUpper())
                        {
                            case "TO":
                                mm.To.Add(new MailAddress(mail.Mail));
                                break;
                            case "CC":
                                mm.CC.Add(new MailAddress(mail.Mail));
                                break;
                            case "BCC":
                                mm.Bcc.Add(new MailAddress(mail.Mail));
                                break;
                        }
                    }
                }
                

                mm.Subject = subiect;
                mm.Body = corpMail;
                mm.IsBodyHtml = true;

                //
                if (trimiteAtt == 1)
                {
                    byte[] arrByte = Encoding.UTF8.GetBytes(corpAtt);
                    MemoryStream stream = new MemoryStream(arrByte);                   
                    mm.Attachments.Add(new Attachment(stream, numeAtt, "text/html"));
             
                }

                //
                if (trimiteXls == 1)
                {
                    if (selectXls != "")
                    {
                        MemoryStream stream = new MemoryStream(General.CreazaExcel(selectXls));
                        mm.Attachments.Add(new Attachment(stream, numeExcel, "application/vnd.ms-excel"));
                    }
                    else
                    {
                        if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog("Sursa de date pentru excel nu este setata", "TrimiteMail");
                    }
                }

                //Radu 29.06.2020
                if (mem != null)
                {
                    mm.Attachments.Add(new Attachment(mem, numeAtt, "application/pdf"));
                }
                //

                //Radu 02.12.2021 - SmtpServer si SmtpParola sunt criptate pt. Asirom
                if (idClient == (int)IdClienti.Clienti.Asirom)
                {
                    CriptDecript prc = new CriptDecript();
                    smtpServer = prc.EncryptString("WizOne2016", smtpServer, 2);
                    smtpParola = prc.EncryptString("WizOne2016", smtpParola, 2);
                }
                SmtpClient smtp = new SmtpClient(smtpServer);
                smtp.Port = Convert.ToInt32(smtpPort);
                smtp.Host = smtpServer;

                if (folosesteCred == "1" || folosesteCred == "2")
                {
                    NetworkCredential basicCred = new NetworkCredential(smtpMail, smtpParola);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = basicCred;
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                smtp.EnableSsl = cuSSL == "1" ? true : false;

                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                smtp.Send(mm);
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);             
            }

        }

        private static void SalveazaInBaza(string nume, string corp, string tblAtasamente_Tabela, int tblAtasamente_Id, int userId)
        {
            try
            {
                string txt = CreazaHTML(corp);

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=1", new string[] { tblAtasamente_Tabela, tblAtasamente_Id.ToString() });
                if (dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["Tabela"] = tblAtasamente_Tabela;
                    dr["Id"] = tblAtasamente_Id;
                    dr["EsteCerere"] = 1;
                    dr["Fisier"] = System.Text.Encoding.ASCII.GetBytes(txt);
                    dr["FisierNume"] = nume;
                    dr["FisierExtensie"] = ".html";
                    dr["USER_NO"] = userId;
                    dr["TIME"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
                else
                {
                    dt.Rows[0]["Fisier"] = System.Text.Encoding.ASCII.GetBytes(txt);
                    dt.Rows[0]["FisierNume"] = nume;
                    dt.Rows[0]["FisierExtensie"] = ".html";
                    dt.Rows[0]["USER_NO"] = userId;
                    dt.Rows[0]["TIME"] = DateTime.Now;
                }

                General.SalveazaDate(dt, "tblFisiere");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string CreazaHTML(string corpAtt)
        {
            string str = corpAtt;

            try
            {
                int poz = corpAtt.IndexOf("UploadFiles/Images");
                if (poz >= 0)
                {
                    string hostUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                    var virtualDir = VirtualPathUtility.ToAbsolute("~/");

                    int poz1 = corpAtt.Substring(0, poz).LastIndexOf('"');
                    if (poz - poz1 > 0)
                    {
                        string txtReplace = corpAtt.Substring(poz1, poz - poz1) + "UploadFiles/Images";
                        corpAtt = corpAtt.Replace(txtReplace, "\"" + hostUrl + virtualDir + "UploadFiles/Images");
                    }
                }

                string txt = @"<!DOCTYPE html>
                                <html>
                                <head>
                                <title>WizOne - Fisiere</title>
                                <meta charset=""utf - 8"" />
                                </head>
                                <body>
                                {0}
                                </body>
                                </html>";
                str = string.Format(txt, corpAtt);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }

        //Radu 07.05.2021
        private static void CreazaCalendarOutlook2(string mail, DateTime ziInc, DateTime ziSf, string subiect, string corp, string oraInc, string oraSf)
        {
            string log = "1";
            try
            {
                DateTime dtInc = ziInc;
                DateTime dtSf = ziSf.AddDays(1);
                log += "2";
                string strInc = dtInc.Year + dtInc.Month.ToString().PadLeft(2, '0') + dtInc.Day.ToString().PadLeft(2, '0');
                string strSf = dtSf.Year + dtSf.Month.ToString().PadLeft(2, '0') + dtSf.Day.ToString().PadLeft(2, '0');
                log += "3";
                string attCorp = corp;
                log += "4";

                //Radu 14.02.2019       
                string tip = "";
                if (oraInc.Length > 0 && oraSf.Length > 0 && oraSf != "00:00")
                    tip = "DATE-TIME";
                else
                    tip = "DATE";
                string oraStart = "", oraSfarsit = "";
                if (tip == "DATE-TIME")
                {
                    oraStart = "T" + (oraInc.Length > 4 ? oraInc.Substring(0, 2) + oraInc.Substring(3, 2) + "00" : "000000");
                    oraSfarsit = "T" + (oraSf.Length > 4 ? oraSf.Substring(0, 2) + oraSf.Substring(3, 2) + "00" : "235959");
                    strSf = ziSf.Year + ziSf.Month.ToString().PadLeft(2, '0') + ziSf.Day.ToString().PadLeft(2, '0');
                }

                String[] contents = {
                        "BEGIN:VCALENDAR",
                        "BEGIN:VEVENT",
                        "STATUS:BUSY",      //Radu 12.02.2019
                        "DTSTART; VALUE = " + tip + ":" + strInc + (tip == "DATE-TIME" ? oraStart : ""),        //Radu 14.02.2019
                        "DTEND; VALUE = " + tip + ":" + strSf + (tip == "DATE-TIME" ? oraSfarsit : ""),         //Radu 14.02.2019
                        "DESCRIPTION;ENCODING=QUOTED-PRINTABLE:" + attCorp.Replace("\r","=0D=0A"),
                        "SUMMARY:" + subiect,
                        "PRIORITY:3",
                        "END:VEVENT",
                        "END:VCALENDAR" };
                log += "5";
                string mom = DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                string numeAtt = "Appt_" + mom + ".ics";
                log += "6";
                if (!Directory.Exists(HostingEnvironment.MapPath("~/Temp/Calendar"))) Directory.CreateDirectory(HostingEnvironment.MapPath("~/Temp/Calendar"));
                log += "7";
                File.WriteAllLines(HostingEnvironment.MapPath("~/Temp/Calendar/" + numeAtt), contents);
                log += "8";
                TrimiteMail(mail, subiect, corp.Replace("\r", "<br>"), numeAtt);
                log += "9";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }
           
        }


        private static object DamiValTabela(string numePagina, string numeGrid, int tipNotif, string camp, DataTable entVal, int idUser, int f10003, out string tipData, out bool esteCamp)
        {
            object val = null;
            tipData = "string";
            esteCamp = true;

            try
            {
                //verificam daca nu cumva acest camp este din alta tabela
                DataTable q = General.IncarcaDT("SELECT * FROM Ntf_tblCampuri WHERE Pagina = " + numePagina + " AND GRID = " + numeGrid + " AND ALIAS = " + camp, null);     
                if (q != null && q.Rows.Count > 0)
                {                   

                    try
                    {
                        if (q.Rows[0]["CampSelect"] != null && q.Rows[0]["CampSelect"].ToString().Length > 0)
                        {
                            //in cazul in care este direct select
                            string strSql = q.Rows[0]["CampSelect"].ToString();
                            for (int i = 0; i < entVal.Columns.Count; i++)
                            {
                                strSql = strSql.Replace("ent." + q.Columns[i].ColumnName, q.Rows[0][q.Columns[i].ColumnName].ToString());
                            }
                         
                            strSql = strSql.Replace("GLOBAL.IDUSER", idUser.ToString()).Replace("GLOBAL.MARCA", f10003.ToString());

                            DataTable rez = General.IncarcaDT(strSql, null);

                            if (rez != null && rez.Rows.Count > 0 && rez.Rows[0][0] != null && rez.Rows[0][0].ToString().Length > 0) val = rez.Rows[0][0].ToString();
                        }
                        else
                        {
                            if (q.Rows[0]["Tabela"] != null && q.Rows[0]["Tabela"].ToString().Length > 0 && q.Rows[0]["CampLegatura"] != null && q.Rows[0]["CampLegatura"].ToString().Length > 0 && q.Rows[0]["CampEntitate"] != null && q.Rows[0]["CampEntitate"].ToString().Length > 0)
                            {
                                //in cazul in care am si restul de campuri completate inseamna ca este camp din alta tabela
                                object filt = null;                     
                                if (entVal.Columns.Contains(q.Rows[0]["CampEntitate"].ToString()))
                                    filt = entVal.Rows[0][q.Rows[0]["CampEntitate"].ToString()];
                                else
                                    filt = q.Rows[0]["CampEntitate"].ToString();

                                string strSql = "";

                                if (Constante.tipBD == 1)
                                    strSql = "SELECT CONVERT(nvarchar(4000)," + q.Rows[0]["Camp"].ToString() + ") AS cmp FROM " + q.Rows[0]["Tabela"].ToString() + " WHERE " + q.Rows[0]["CampLegatura"].ToString() + "=" + filt.ToString();
                                else
                                {
                                    if (q.Rows[0]["TipData"].ToString().ToLower() == "datetime")
                                        strSql = "SELECT to_char(" + q.Rows[0]["Camp"].ToString() + ",'dd-mm-yyyy') AS cmp FROM \"" + q.Rows[0]["Tabela"].ToString() + "\" WHERE \"" + q.Rows[0]["CampLegatura"].ToString() + "\"='" + filt.ToString() + "'";
                                    else
                                        strSql = "SELECT to_char(" + q.Rows[0]["Camp"].ToString() + ") AS cmp FROM \"" + q.Rows[0]["Tabela"].ToString() + "\" WHERE \"" + q.Rows[0]["CampLegatura"].ToString() + "\"='" + filt.ToString() + "'";
                                }

                                strSql = strSql.Replace("GLOBAL.IDUSER", idUser.ToString()).Replace("GLOBAL.MARCA", f10003.ToString());

                                DataTable rez = General.IncarcaDT(strSql, null);

                                if (rez != null && rez.Rows.Count > 0 && rez.Rows[0][0] != null && rez.Rows[0][0].ToString().Length > 0) val = rez.Rows[0][0].ToString();
                            }
                            else
                            {
                                //este camp din entitate
                                if (entVal.Columns.Contains(q.Rows[0]["Camp"].ToString()))
                                {
                                    if (q.Rows[0]["TipData"].ToString().ToLower() == "datetime")
                                        if (entVal.Rows[0][q.Rows[0]["Camp"].ToString()] != null && entVal.Rows[0][q.Rows[0]["Camp"].ToString()].ToString().Length > 0)
                                        {
                                            try
                                            {
                                                val = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(entVal.Rows[0][q.Rows[0]["Camp"].ToString()].ToString()));
                                            }
                                            catch (Exception) { }
                                        }
                                        else
                                        {
                                            val = "";
                                        }
                                    else
                                        val = entVal.Rows[0][q.Rows[0]["Camp"].ToString()].ToString();
                                }
                                else
                                {
                                    switch (q.Rows[0]["Camp"].ToString().ToUpper())
                                    {
                                        case "GLOBAL.MARCA":
                                            val = f10003.ToString();
                                            break;
                                        case "GLOBAL.IDUSER":
                                            val = idUser.ToString();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    tipData = q.Rows[0]["TipData"].ToString();
                }
                else
                {
                    esteCamp = false;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return val;
        }

        private static string TrimiteMail(string mailTO, string subiect, string corpMail, string numeAtt)
        {
            string strErr = "";
            SmtpClient smtp = new SmtpClient();

            try
            {

                string folosesteCred = Dami.ValoareParam("TrimiteMailCuCredentiale");
                string cuSSL = Dami.ValoareParam("TrimiteMailCuSSL", "false");

                string smtpMailFrom = Dami.ValoareParam("SmtpMailFrom");
                string smtpServer = Dami.ValoareParam("SmtpServer");
                string smtpPort = Dami.ValoareParam("SmtpPort");
                string smtpMail = Dami.ValoareParam("SmtpMail");
                string smtpParola = Dami.ValoareParam("SmtpParola");

                string strMsg = "";
                if (smtpMailFrom == "") strMsg += ", mail from";

                if (smtpServer == "") strMsg += ", serverul de smtp";
                if (smtpPort == "") strMsg += ", smtp port";
                if (folosesteCred == "1" || folosesteCred == "2")
                {
                    if (smtpMail == "") strMsg += ", smtp mail";
                    if (smtpParola == "") strMsg += ", smtp parola";
                }

                if (strMsg != "")
                {
                    General.MemoreazaEroarea("Nu exista date despre " + strMsg.Substring(2), "Notif", "TrimiteMail");
                    return "Nu exista date despre " + strMsg.Substring(2);
                }

                MailMessage mm = new MailMessage();
                mm.To.Add(new MailAddress(mailTO));
                mm.From = new MailAddress(smtpMailFrom);
                mm.Subject = subiect;
                mm.Body = corpMail;
                mm.IsBodyHtml = true;
                smtp = new SmtpClient(smtpServer);
                smtp.Port = Convert.ToInt32(smtpPort);
                smtp.Host = smtpServer;

                if (cuSSL == "1")
                    smtp.EnableSsl = true;
                if (folosesteCred == "1")
                {
                    NetworkCredential basicCred = new NetworkCredential(smtpMail, smtpParola);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = basicCred;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                Attachment mailAttachment = new Attachment(HostingEnvironment.MapPath("~/Temp/Calendar/" + numeAtt));
                mm.Attachments.Add(mailAttachment);
                smtp.Send(mm);

                strErr = "";
    

                smtp.Dispose();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strErr;
        }

    }
}