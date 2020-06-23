using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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
    public class NotifAsync
    {
        static string baseUrl = "";
        static string idClient = "1";
        static string idLimba = "RO";

        internal class metaAdreseMail
        {
            public string Mail { get; set; }
            public string Destinatie { get; set; }
            public int IncludeLinkAprobare { get; set; }
        }

        internal static string TrimiteNotificare(string numePagina, int tipNotificare, string strSelect, string tblAtasamente_Tabela, int tblAtasamente_Id, int userId, int userMarca, string[] arrParam)
        {
            string rez = "";

            try
            {
                if (arrParam.Length >= 1 && arrParam[0] != "") baseUrl = arrParam[0];
                if (arrParam.Length >= 2 && arrParam[1] != "") idClient = arrParam[1];
                if (arrParam.Length >= 3 && arrParam[2] != "") idLimba = arrParam[2];

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
                        DataTable dtFiltru = General.IncarcaDT(strFiltru, null);
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
                                                    lstOne.Add(new metaAdreseMail { Mail = lstAdrese[j].Mail, Destinatie = "TO", IncludeLinkAprobare = 0 });

                                                    string corpMail = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel.Rows[0], userId, userMarca, numePagina, tblAtasamente_Id, lstAdrese[j].Mail, lstAdrese[j].IncludeLinkAprobare);
                                                    TrimiteMail(lstOne, subiect, corpMail, Convert.ToInt32(dtReg.Rows[i]["TrimitePeMail"]), numeAtt, CreazaHTML(corpAtt), Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteXLS"], 0)), selectXls, numeExcel, idClient);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog("Nu exista adrese de mail", "VerificaDate");
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

        private static void CreazaSelect(int id, string strSelect, string numePagina, int userId, int userMarca, out string ntf_Campuri, out string ntf_Conditii)
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
                                                        strCond += " AND CONVERT(date,(DATEADD(d," + General.Nz(dr["NrZile1"],0) + ",(" + arr1[0] + ")))) <= CONVERT(date,(" + col + ")) AND CONVERT(date,(" + col + ")) <= CONVERT(date,DATEADD(d," + General.Nz(dr["NrZile2"], 0) + ",(" + arr2[0] + ")))";
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

        private static List<metaAdreseMail> CreazaAdreseMail(int id, DataRow dtSel)
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

        private static string InlocuiesteCampuri(string text, DataRow drSel, int userId, int userMarca, string numePagina = "", int id = -99, string lstAdr = "", int inlocLinkAprobare = 0)
        {
            string str = text;

            try
            {
                string strSelect = "";
                string strOriginal = "";

                for (int i = 0; i < drSel.Table.Columns.Count; i++)
                {
                    str = str.Replace("#$" + drSel.Table.Columns[i] + "$#", General.Nz(drSel[drSel.Table.Columns[i]], "").ToString());
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
                            string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/1/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + idClient.PadLeft(8, '0') + "/" + numePagina;

                            string rsp = General.Encrypt_QueryString(arg);
                            string hostUrl = baseUrl + VirtualPathUtility.ToAbsolute("~/");
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
                            string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/2/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + idClient.PadLeft(8, '0') + "/" + numePagina;

                            string rsp = General.Encrypt_QueryString(arg);
                            string hostUrl = baseUrl + VirtualPathUtility.ToAbsolute("~/");
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
                if (str.ToLower().IndexOf("#$select") >= 0)
                {
                    int start = str.ToLower().IndexOf("#$select");
                    strSelect = str.Substring(start, str.Substring(start).LastIndexOf("$#")).Replace("#$", "");
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

//        private static void TrimiteMail(string mail, string subiect, string corpMail, int trimiteAtt, string numeAtt, string corpAtt, int trimiteXls, string selectXls, string numeExcel)
//        {
//            try
//            {
//                string folosesteCred = Dami.ValoareParam("TrimiteMailCuCredentiale");
//                string cuSSL = Dami.ValoareParam("TrimiteMailCuSSL", "false");

//                string smtpMailFrom = Dami.ValoareParam("SmtpMailFrom");
//                string smtpServer = Dami.ValoareParam("SmtpServer");
//                string smtpPort = Dami.ValoareParam("SmtpPort");
//                string smtpMail = Dami.ValoareParam("SmtpMail");
//                string smtpParola = Dami.ValoareParam("SmtpParola");


//                string strMsg = "";
//                if (smtpMailFrom == "") strMsg += ", mail from";

//                if (smtpServer == "") strMsg += ", serverul de smtp";
//                if (smtpPort == "") strMsg += ", smtp port";
//                if (folosesteCred == "1" || folosesteCred == "2")
//                {
//                    if (smtpMail == "") strMsg += ", smtp mail";
//                    if (smtpParola == "") strMsg += ", smtp parola";
//                }

//                if (strMsg != "")
//                {
//                    General.MemoreazaEroarea("Nu exista date despre " + strMsg.Substring(2), "Notif", "TrimiteMail");
//                    return;
//                }

//                MailMessage mm = new MailMessage();
//                mm.From = new MailAddress(smtpMailFrom);

//                if (mail.Trim() == "")
//                {
//                    General.MemoreazaEroarea(TraduCuvant("Nu exista destinatar"), "Notif", "TrimiteMail");
//                    return;
//                }
//                else
//                {
//                    if (Convert.ToInt32(idClient) == 14)
//                        mm.To.Add(new MailAddress("<" + mail + ">"));
//                    else
//                        mm.To.Add(new MailAddress(mail));
//                }

//                #region OLD
//                //if (lstAdr == null || lstAdr.Count() == 0)
//                //{
//                //    General.MemoreazaEroarea(Dami.TraduCuvant("Nu exista destinatar"), "Dami", "TrimiteMail");
//                //    return;
//                //}
//                //else
//                //{
//                //    foreach (var mail in lstAdr)
//                //    {
//                //        switch(mail.Destinatie.ToUpper())
//                //        {
//                //            case "TO":
//                //                {
//                //                    if (Session["IdClient"] == 14)
//                //                        mm.To.Add(new MailAddress("<" + mail.Mail + ">"));
//                //                    else
//                //                        mm.To.Add(new MailAddress(mail.Mail));
//                //                }
//                //                break;
//                //            case "CC":
//                //                {
//                //                    if (Session["IdClient"] == 14)
//                //                        mm.CC.Add(new MailAddress("<" + mail.Mail + ">"));
//                //                    else
//                //                        mm.CC.Add(new MailAddress(mail.Mail));
//                //                }
//                //                break;
//                //            case "BCC":
//                //                {
//                //                    if (Session["IdClient"] == 14)
//                //                        mm.Bcc.Add(new MailAddress("<" + mail.Mail + ">"));
//                //                    else
//                //                        mm.Bcc.Add(new MailAddress(mail.Mail));
//                //                }
//                //                break;
//                //        }
//                //    }
//                //}
//#endregion

//                mm.Subject = subiect;
//                mm.Body = corpMail;
//                mm.IsBodyHtml = true;
                
//                //
//                if (trimiteAtt == 1)
//                {
//                    byte[] arrByte = Encoding.UTF8.GetBytes(CreazaHTML(corpAtt));
//                    MemoryStream stream = new MemoryStream(arrByte);
//                    mm.Attachments.Add(new Attachment(stream, numeAtt, "text/html"));
//                }

//                //
//                if (trimiteXls == 1)
//                {
//                    if (selectXls != "")
//                    {
//                        MemoryStream stream = new MemoryStream(General.CreazaExcel(selectXls));
//                        mm.Attachments.Add(new Attachment(stream, numeExcel, "application/vnd.ms-excel"));
//                    }
//                    else
//                    {
//                        if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog("Sursa de date pentru excel nu este setata", "TrimiteMail");
//                    }
//                }

//                //
//                SmtpClient smtp = new SmtpClient(smtpServer);
//                smtp.Port = Convert.ToInt32(smtpPort);
//                smtp.Host = smtpServer;

//                if (folosesteCred == "1" || folosesteCred == "2")
//                {
//                    NetworkCredential basicCred = new NetworkCredential(smtpMail, smtpParola);
//                    smtp.UseDefaultCredentials = false;
//                    smtp.Credentials = basicCred;
//                }
//                else
//                {
//                    smtp.UseDefaultCredentials = true;
//                }

//                smtp.EnableSsl = cuSSL == "1" ? true : false;

//                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

//                smtp.Send(mm);
//                smtp.Dispose();
//            }
//            catch (Exception ex)
//            {
//                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
//            }
//        }

        private static void TrimiteMail(List<metaAdreseMail> lstAdr, string subiect, string corpMail, int trimiteAtt, string numeAtt, string corpAtt, int trimiteXls, string selectXls, string numeExcel, int idClient)
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
                    General.MemoreazaEroarea(TraduCuvant("Nu exista destinatar"), "Dami", "TrimiteMail");
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

                //
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

        private static string CreazaHTML(string corpAtt)
        {
            string str = corpAtt;

            try
            {
                var virtualDir = VirtualPathUtility.ToAbsolute("~/");

                int poz = corpAtt.IndexOf("UploadFiles/Images");
                if (poz >= 0)
                {
                    int poz1 = corpAtt.Substring(0, poz).LastIndexOf('"');
                    if (poz - poz1 > 0)
                    {
                        string txtReplace = corpAtt.Substring(poz1, poz - poz1) + "UploadFiles/Images";
                        corpAtt = corpAtt.Replace(txtReplace, "\"" + baseUrl + virtualDir + "UploadFiles/Images");
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

        internal static string TraduCuvant(string nume, string cuvant = "")
        {
            string rez = cuvant;

            try
            {
                rez = (string)HttpContext.GetGlobalResourceObject("General", nume.ToString().Replace(" ", "").Replace("!", "").Replace("/", "").Replace("-", "").Replace("+", "").Replace(".", ""), new CultureInfo(idLimba));

                if (rez == null || rez == "")
                {
                    if (cuvant != "")
                        rez = cuvant;
                    else
                        rez = nume;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }
    }
}