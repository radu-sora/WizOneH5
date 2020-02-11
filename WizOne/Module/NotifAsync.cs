using ProceseSec;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
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

        public class metaAdreseMail
        {
            public string Mail { get; set; }
            public string Destinatie { get; set; }
            public int IncludeLinkAprobare { get; set; }
        }


        public static string TrimiteNotificare(string numePagina, int tipNotificare, string strSelect, string tblAtasamente_Tabela, int tblAtasamente_Id, int userId, int userMarca, string[] arrParam)
        {
            string rez = "";

            try
            {
                if (arrParam.Length >= 1 && arrParam[0] != "") baseUrl = arrParam[0];
                if (arrParam.Length >= 2 && arrParam[1] != "") idClient = arrParam[1];
                if (arrParam.Length >= 3 && arrParam[2] != "") idLimba = arrParam[2];

                //string numePagina = Dami.PaginaWeb();
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

                        //string strSel = CreazaSelect(Convert.ToInt32(dtReg.Rows[i]["Id"]), strSelect, numePagina, userId, userMarca);
                        //if (strSql == "") continue;


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
                                            corpAtt = InlocuiesteCampuri((dtReg.Rows[i]["ContinutAtasament"] ?? "").ToString(), dtSel, userId, userMarca);
                                            numeAtt = InlocuiesteCampuri(General.Nz(dtReg.Rows[i]["NumeAtasament"], "Atasament_" + DateTime.Now.Ticks.ToString()).ToString(), dtSel, userId, userMarca) + ".html";

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
                                            numeExcel = InlocuiesteCampuri(General.Nz(dtReg.Rows[i]["NumeExcel"], "Atasament_" + DateTime.Now.Ticks.ToString()).ToString(), dtSel, userId, userMarca) + ".xls";
                                            selectXls = InlocuiesteCampuri((dtReg.Rows[i]["SelectXLS"] ?? "").ToString(),dtSel, userId, userMarca);
                                        }

                                        List<metaAdreseMail> lstAdrese = CreazaAdreseMail(Convert.ToInt32(dtReg.Rows[i]["Id"]), dtSel);
                                        //string lstAdr = "";
                                        //for(int j = 0; j < lstAdrese.Count(); j++)
                                        //{
                                        //    if (lstAdrese[j].Destinatie.ToUpper() == "TO")
                                        //    {
                                        //        //lstAdr += ";" + lstAdrese[j].Mail;
                                        //        lstAdr = lstAdrese[j].Mail;
                                        //        break;
                                        //    }
                                        //}
                                        //string corpMail = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel, numePagina, tblAtasamente_Id, lstAdr);

                                        if (lstAdrese.Count > 0)
                                        {
                                            string subiect = InlocuiesteCampuri((dtReg.Rows[i]["Subiect"] ?? "").ToString(), dtSel, userId, userMarca);

                                            //Florin 2018.05.09
                                            if (lstAdrese.Where(p => p.IncludeLinkAprobare == 1).Count() == 0)
                                            {
                                                string corpMail = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel, userId, userMarca, numePagina, tblAtasamente_Id);
                                                TrimiteMail(lstAdrese, subiect, corpMail, Convert.ToInt32(dtReg.Rows[i]["TrimitePeMail"]), numeAtt, corpAtt, Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteXLS"], 0)), selectXls, numeExcel);
                                            }
                                            else
                                            {
                                                for (int j = 0; j < lstAdrese.Count(); j++)
                                                {
                                                    string corpMail = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel, userId, userMarca, numePagina, tblAtasamente_Id, lstAdrese[j].Mail, lstAdrese[j].IncludeLinkAprobare);
                                                    TrimiteMail(lstAdrese[j].Mail, subiect, corpMail, Convert.ToInt32(dtReg.Rows[i]["TrimitePeMail"]), numeAtt, corpAtt, Convert.ToInt32(General.Nz(dtReg.Rows[i]["TrimiteXLS"], 0)), selectXls, numeExcel);
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
                                        string corpMsg = InlocuiesteCampuri((dtReg.Rows[i]["ContinutMail"] ?? "").ToString(), dtSel, userId, userMarca);
                                        if (corpMsg != "")
                                        {
                                            
                                            if ((dtReg.Rows[i]["mesaj"] ?? "avertisment").ToString() == "avertisment")
                                            {
                                                rez = Constante.MesajeValidari.Avertisment.ToString();
                                                //MessageBox.ShowProba2(corpMsg, MessageBox.icoWarning, (dtReg.Rows[i]["Subiect"] ?? "").ToString());
                                                //string asd = @"<script type='text/javascript'> swal({   title: 'Are you sure ? ',   text: 'You will not be able to recover this imaginary file!',   type: 'warning',   showCancelButton: true,   confirmButtonColor: '#DD6B55',   confirmButtonText: 'Yes, delete it!',   closeOnConfirm: false }, function(isConfirm){ if (isConfirm) { <%= Iesire() %> } });</script>";

                                                //string msg = "<script type='text/javascript'> " +
                                                //            " swal({  title: 'Validare',    " +
                                                //            "         text: '" + corpMsg + "', " +
                                                //            "         type: '" + tipIco + "', " +
                                                //            "         showCancelButton: true, " +
                                                //            "         confirmButtonColor: '#DD6B55',    " +
                                                //            "         confirmButtonText: 'Continuati operatia!', " +
                                                //            "         cancelButtonText: 'Renuntare!',    " +
                                                //            "         closeOnConfirm: false },  " +
                                                //            "         function(isConfirm){  " +
                                                //            "         if (isConfirm) { document.getElementById('btnBack').click(); } }); " +
                                                //            "         </script>";

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

                                                //string txt = "<script type=\"text/javascript\" language=\"javascript\"> " +
                                                //    " swal({ " +
                                                //    "     title: \"" + (dtReg.Rows[i]["Subiect"] ?? "").ToString() + "\", " +
                                                //    "     text: \"" + corpMsg + "\", " +
                                                //    "     type: \"" + MessageBox.icoError + "\" " +
                                                //    " });</script>";
                                                //Page pagina = HttpContext.Current.Handler as Page;
                                                //pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);

                                                string subiect = InlocuiesteCampuri((dtReg.Rows[i]["Subiect"] ?? "").ToString(), dtSel, userId, userMarca);
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
            string strSql = "";

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


                        switch (dr["Operator"].ToString())
                        {
                            case "fara valoare":
                                strCond += " AND ((" + col + ") IS NULL OR (" + col + ") = '' OR (" + col + ") = 0)";
                                break;
                            case "cu valoare":
                                strCond += " AND ((" + col + ") IS NOT NULL AND (" + col + ") <> '' AND (" + col + ") <> 0)";
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
                                                    strCond += " AND CONVERT(date,(DATEADD(d," + (dr["NrZile1"] ?? 0).ToString() + ",(" + arr1[0] + "))) <= CONVERT(date,(" + col + ")) AND CONVERT(date,(" + col + ")) <= CONVERT(date,DATEADD(d," + (dr["NrZile2"] ?? 0).ToString() + ",(" + arr2[0] + "))))";
                                                else
                                                    strCond += " AND ((TRUNC(" + arr1[0] + ") + " + (dr["NrZile1"] ?? 0).ToString() + ") <= (" + col + ") AND (" + col + ") <= (TRUNC(" + arr2[0] + ") + " + (dr["NrZile2"] ?? 0).ToString() + "))";
                                            }
                                        }
                                    }
                                }
                                break;
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
                            case "<>":
                            case ">":
                            case ">=":
                            case "<":
                            case "<=":
                            case "=":
                                {
                                    string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
                                    if (arr[1] == "2")
                                        strCond += " AND ((" + col + ")" + dr["Operator"] + "('" + arr[0] + "'))";
                                    else
                                        strCond += " AND ((" + col + ")" + dr["Operator"] + "(" + arr[0] + "))";
                                }
                                break;
                        }
                    }
                }
                if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strCond, "CreazaFiltrul");


                //cream campurile

                for (int i = 0; i < dtCmp.Rows.Count; i++)
                {
                    //Florin 2019.10.11
                    //if ((dtCmp.Rows[i]["CampSelect"] ?? "").ToString() != "" && (dtCmp.Rows[i]["Alias"] ?? "").ToString() != "")
                    //    strCamp += ", (" + dtCmp.Rows[i]["CampSelect"] + ") AS '" + dtCmp.Rows[i]["Alias"] + "'";
                    if ((dtCmp.Rows[i]["CampSelect"] ?? "").ToString() != "" && (dtCmp.Rows[i]["Alias"] ?? "").ToString() != "")
                        strCamp += ", (" + dtCmp.Rows[i]["CampSelect"] + ") AS \"" + dtCmp.Rows[i]["Alias"] + "\"";
                }

                if (strCamp != "") strCamp = strCamp.Substring(1);
                if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strCamp, "CreazaCampurile");


                //cream selectul
                if (strCamp != "")
                {
                    strSql = "SELECT " + strCamp + " FROM (" + strSelect + ") ent WHERE 1 = 1 " + strCond;
                    strSql = strSql.Replace("GLOBAL.MARCA", userMarca.ToString()).Replace("GLOBAL.IDUSER", userId.ToString());
                    if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strSql, "CreazaSelect");
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            //return strSql;

            ntf_Campuri = strCamp;
            ntf_Conditii = strCond;
        }


        //private static string CreazaSelect(int id, string strSelect, string numePagina, int userId, int userMarca)
        //{
        //    string strSql = "";

        //    try
        //    {
        //        //cream filtrul
        //        string strCond = "";
        //        DataTable dtCond = General.IncarcaDT(@"SELECT * FROM ""Ntf_Conditii"" WHERE ""Id""=@1", new string[] { id.ToString() });
        //        DataTable dtCmp = General.IncarcaDT(@"SELECT * FROM ""Ntf_tblCampuri"" WHERE ""Pagina""=@1", new string[] { numePagina });
        //        foreach (DataRow dr in dtCond.Rows)
        //        {
        //            if ((dr["Coloana"] ?? "").ToString() != "" && (dr["Operator"] ?? "").ToString() != "")
        //            {
        //                DataRow[] lstTip = dtCmp.Select("Alias='" + dr["Coloana"] + "'");
        //                if (lstTip.Count() == 0) continue;
        //                string col = (lstTip[0]["CampSelect"] ?? "").ToString();
        //                if (col == "") continue;
        //                string tipData = "string";
        //                if ((lstTip[0]["TipData"] ?? "").ToString() != "") tipData = (lstTip[0]["TipData"] ?? "").ToString();

        //                if (dr["Operator"].ToString() != "fara valoare" && dr["Operator"].ToString() != "cu valoare" && (dr["Valoare1"] ?? "").ToString() == "") continue;


        //                switch (dr["Operator"].ToString())
        //                {
        //                    case "fara valoare":
        //                        strCond += " AND ((" + col + ") IS NULL OR (" + col + ") = '' OR (" + col + ") = 0)";
        //                        break;
        //                    case "cu valoare":
        //                        strCond += " AND ((" + col + ") IS NOT NULL AND (" + col + ") <> '' AND (" + col + ") <> 0)";
        //                        break;
        //                    case "in":
        //                        {
        //                            string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                            strCond += " AND ((" + col + ") IN (" + arr[0] + "))";
        //                        }
        //                        break;
        //                    case "not in":
        //                        {
        //                            string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                            strCond += " AND ((" + col + ") NOT IN (" + arr[0] + "))";
        //                        }
        //                        break;
        //                    case "intre":
        //                        {
        //                            if ((dr["Valoare1"] ?? "").ToString() != "" && (dr["Valoare2"] ?? "").ToString() != "")
        //                            {
        //                                string[] arr1 = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                                string[] arr2 = AflaValoarea(dr["Valoare2"].ToString(), dtCmp);
        //                                if (tipData == "int")
        //                                    strCond += " AND ((" + arr1[0] + ") <= (" + col + ") AND (" + col + ") <= (" + arr2[0] + "))";
        //                                else
        //                                {
        //                                    if (tipData == "datetime")
        //                                    {
        //                                        if (Constante.tipBD == 1)
        //                                            strCond += " AND CONVERT(date,(DATEADD(d," + (dr["NrZile1"] ?? "0").ToString() + ",(" + arr1[0] + "))) <= CONVERT(date,(" + col + ")) AND CONVERT(date,(" + col + ")) <= CONVERT(date,DATEADD(d," + (dr["NrZile2"] ?? "0").ToString() + ",(" + arr2[0] + "))))";
        //                                        else
        //                                            strCond += " AND ((TRUNC(" + arr1[0] + ") + " + (dr["NrZile1"] ?? "0").ToString() + ") <= (" + col + ") AND (" + col + ") <= (TRUNC(" + arr2[0] + ") + " + (dr["NrZile2"] ?? "0").ToString() + "))";
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    case "incepe cu":
        //                        {
        //                            string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                            if (arr[1] == "3")
        //                                strCond += " AND ((" + col + ") LIKE (" + arr[0] + ") + '%')";
        //                            else
        //                                strCond += " AND ((" + col + ") LIKE '" + arr[0] + "' + '%')";
        //                        }
        //                        break;
        //                    case "contine":
        //                        {
        //                            string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                            if (arr[1] == "3")
        //                                strCond += " AND ((" + col + ") LIKE '%' + (" + arr[0] + ") + '%')";
        //                            else
        //                                strCond += " AND ((" + col + ") LIKE '%' + '" + arr[0] + "' + '%')";
        //                        }
        //                        break;
        //                    case "se termina cu":
        //                        {
        //                            string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                            if (arr[1] == "3")
        //                                strCond += " AND ((" + col + ") LIKE '%' + (" + arr[0] + "))";
        //                            else
        //                                strCond += " AND ((" + col + ") LIKE '%' + '" + arr[0] + "')";
        //                        }
        //                        break;
        //                    case "<>":
        //                    case ">":
        //                    case ">=":
        //                    case "<":
        //                    case "<=":
        //                    case "=":
        //                        {
        //                            string[] arr = AflaValoarea(dr["Valoare1"].ToString(), dtCmp);
        //                            if (arr[1] == "2")
        //                                strCond += " AND ((" + col + ")" + dr["Operator"] + "('" + arr[0] + "'))";
        //                            else
        //                                strCond += " AND ((" + col + ")" + dr["Operator"] + "(" + arr[0] + "))";
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //        if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strCond, "CreazaFiltrul");


        //        //cream campurile
        //        string strCamp = "";
        //        for (int i = 0; i < dtCmp.Rows.Count; i++)
        //        {
        //            if ((dtCmp.Rows[i]["CampSelect"] ?? "").ToString() != "" && (dtCmp.Rows[i]["Alias"] ?? "").ToString() != "")
        //                strCamp += ", (" + dtCmp.Rows[i]["CampSelect"] + ") AS '" + dtCmp.Rows[i]["Alias"] + "'";
        //        }

        //        if (strCamp != "") strCamp = strCamp.Substring(1);
        //        if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strCamp, "CreazaCampurile");


        //        //cream selectul
        //        if (strCamp != "")
        //        {
        //            strSql = "SELECT " + strCamp + " FROM (" + strSelect + ") ent WHERE 1 = 1 " + strCond;
        //            strSql = strSql.Replace("GLOBAL.MARCA", userMarca.ToString()).Replace("GLOBAL.IDUSER",userId.ToString());
        //            if (Dami.ValoareParam("LogNotificari") == "1") General.CreazaLog(strSql, "CreazaSelect");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return strSql;
        //}

        private static string[] AflaValoarea(string camp, DataTable dtCmp)
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

                    if (Constante.tipBD == 1)
                        str[0] = dt.Year + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Day.ToString().PadLeft(2, '0');
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

        private static List<metaAdreseMail> CreazaAdreseMail(int id, DataTable dtSel)
        {
            List<metaAdreseMail> lst = new List<metaAdreseMail>();

            try
            {
                //Florin 2019.10.11
                //DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ntf_Mailuri"" WHERE ""Id""=@1 AND ""MailTip"" IS NOT NULL AND ""MailTip"" <> '' AND ""MailAdresaId"" IS NOT NULL AND ""MailAdresaId"" <> ''  ", new string[] { id.ToString() });
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
                                    string str = (dtSel.Rows[0][dt.Rows[i]["MailAdresaText"].ToString()] ?? "").ToString();
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

        //2020.02.06

        private static string InlocuiesteCampuri(string text, DataTable dtSel, int userId, int userMarca, string numePagina = "", int id = -99, string lstAdr = "", int inlocLinkAprobare = 0)
        {
            string str = text;

            try
            {
                string strSelect = "";
                string strOriginal = "";

                for (int i = 0; i < dtSel.Columns.Count; i++)
                {
                    str = str.Replace("#$" + dtSel.Columns[i] + "$#", (dtSel.Rows[0][dtSel.Columns[i]] ?? "").ToString());
                }

                if (str.IndexOf("#$Link1") >= 0)
                {
                    string cuv = str.Substring(str.IndexOf("#$Link1"), str.Substring(str.IndexOf("#$Link1")).IndexOf("$#"));
                    if (cuv != "") cuv = cuv.Replace("#$Link1", "").Replace("$#", "").Trim();

                    if ((numePagina.IndexOf("Absente.Lista") >= 0 || numePagina.IndexOf("Pontaj.PontajEchipa") >= 0 || numePagina.IndexOf("Pontaj.PontajDetaliat") >= 0) && id != -99 && lstAdr != "" && inlocLinkAprobare == 1)
                    {
                        string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/1/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + idClient.PadLeft(8, '0') + "/" + numePagina;

                        string rsp = General.Encrypt_QueryString(arg);
                        string hostUrl = baseUrl + VirtualPathUtility.ToAbsolute("~/");
                        string lnk = "<a href='" + hostUrl + "/Raspuns.aspx?arg=" + rsp + "' target='_blank'>" + cuv + "</a>";
                        str = str.Replace("#$Link1 " + cuv + "$#", lnk).ToString();
                    }
                    else
                        str = str.Replace("#$Link1 " + cuv + "$#", "").ToString();
                }


                if (str.IndexOf("#$Link2") >= 0)
                {
                    string cuv = str.Substring(str.IndexOf("#$Link2"), str.Substring(str.IndexOf("#$Link2")).IndexOf("$#"));
                    if (cuv != "") cuv = cuv.Replace("#$Link2", "").Replace("$#", "").Trim();

                    if ((numePagina.IndexOf("Absente.Lista") >= 0 || numePagina.IndexOf("Pontaj.PontajEchipa") >= 0 || numePagina.IndexOf("Pontaj.PontajDetaliat") >= 0) && id != -99 && lstAdr != "" && inlocLinkAprobare == 1)
                    {
                        string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/2/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + idClient.PadLeft(8, '0') + "/" + numePagina;

                        string rsp = General.Encrypt_QueryString(arg);
                        string hostUrl = baseUrl + VirtualPathUtility.ToAbsolute("~/");
                        string lnk = "<a href='" + hostUrl + "/Raspuns.aspx?arg=" + rsp + "' target='_blank'>" + cuv + "</a>";
                        str = str.Replace("#$Link2 " + cuv + "$#", lnk).ToString();
                    }
                    else
                        str = str.Replace("#$Link2 " + cuv + "$#", "").ToString();
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


        //private static string InlocuiesteCampuri(string text, DataTable dtSel, int userId, int userMarca, string numePagina = "", int id = -99, string lstAdr = "", int inlocLinkAprobare = 0)
        //{
        //    string str = text;

        //    try
        //    {
        //        string strSelect = "";
        //        string strOriginal = "";

        //        //cautam daca avem de inserat tabel
        //        if (str.ToLower().IndexOf("#$select") >= 0)
        //        {
        //            int start = str.ToLower().IndexOf("#$select");
        //            strSelect = str.Substring(start, str.Substring(start).IndexOf("$#")).Replace("#$", "");
        //            strOriginal = strSelect;
        //            strSelect = WebUtility.HtmlDecode(strSelect);
        //            strSelect = strSelect.Replace("GLOBAL.MARCA", userMarca.ToString()).Replace("GLOBAL.IDUSER", userId.ToString());
        //        }

        //        for (int i = 0; i < dtSel.Columns.Count; i++)
        //        {
        //            str = str.Replace("#$" + dtSel.Columns[i] + "$#", (dtSel.Rows[0][dtSel.Columns[i]] ?? "").ToString());
        //            strSelect = strSelect.Replace("#$" + dtSel.Columns[i] + "$#", (dtSel.Rows[0][dtSel.Columns[i]] ?? "").ToString());
        //        }

        //        if (str.IndexOf("#$Link") >= 0)
        //        {
        //            string cuv = str.Substring(str.IndexOf("#$Link"), str.Substring(str.IndexOf("#$Link")).IndexOf("$#"));
        //            if (cuv != "") cuv = cuv.Replace("#$Link", "").Replace("$#", "").Trim();

        //            if ((numePagina.IndexOf("Absente.Lista") >= 0 || numePagina.IndexOf("Pontaj.PontajEchipa") >= 0 || numePagina.IndexOf("Pontaj.PontajDetaliat") >= 0) && id != -99 && lstAdr != "" && inlocLinkAprobare == 1)
        //            {
        //                string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/1/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + idClient.PadLeft(8, '0') + "/" + numePagina;

        //                string rsp = General.Encrypt_QueryString(arg);
        //                string hostUrl = baseUrl + VirtualPathUtility.ToAbsolute("~/");
        //                string lnk = "<a href='" + hostUrl + "/Raspuns.aspx?arg=" + rsp + "' target='_blank'>" + cuv + "</a>";
        //                str = str.Replace("#$Link " + cuv + "$#", lnk).ToString();
        //            }
        //            else
        //                str = str.Replace("#$Link " + cuv + "$#", "").ToString();
        //        }


        //        if (str.IndexOf("Link Respinge") >= 0)
        //        {
        //            if ((numePagina.IndexOf("Absente.Lista") >= 0 || numePagina.IndexOf("Pontaj.PontajEchipa") >= 0 || numePagina.IndexOf("Pontaj.PontajDetaliat") >= 0) && id != -99 && lstAdr != "" && inlocLinkAprobare == 1)
        //            {
        //                string arg = DateTime.Now.Second.ToString().PadLeft(2, '0') + "/Wiz/" + lstAdr + "/" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "/2/One/" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "/" + id.ToString().PadLeft(8, '0') + "/" + idClient.PadLeft(8, '0') + "/" + numePagina;

        //                string rsp = General.Encrypt_QueryString(arg);
        //                string hostUrl = baseUrl + VirtualPathUtility.ToAbsolute("~/");
        //                string lnk = "<a href='" + hostUrl + "/Raspuns.aspx?arg=" + rsp + "' target='_blank'>" + TraduCuvant("Respinge") + "</a>";
        //                str = str.Replace("#$Link Respinge$#", lnk).ToString();
        //            }
        //            else
        //                str = str.Replace("#$Link Respinge$#", "").ToString();
        //        }


        //        //cautam daca avem de inserat tabel
        //        if (str.ToLower().IndexOf("#$select") >= 0)
        //        {
        //            DataTable dtTbl = General.IncarcaDT(WebUtility.HtmlDecode(strSelect), null);
        //            string tbl = "";
        //            tbl += @"<table style=""border: solid 1px #ccc; width:100%;"">" + Environment.NewLine;


        //            //adaugam capul de tabel
        //            tbl += @"<thead style=""background-color:lightblue;"">" + Environment.NewLine;
        //            tbl += "<tr>" + Environment.NewLine;
        //            for (int x = 0; x < dtTbl.Columns.Count; x++)
        //            {
        //                tbl += "<td>" + dtTbl.Columns[x].ColumnName + "</td>" + Environment.NewLine;
        //            }
        //            tbl += "</tr>" + Environment.NewLine;
        //            tbl += @"</thead>" + Environment.NewLine;


        //            //adaugam corpul tabelului
        //            tbl += @"<tbody>" + Environment.NewLine;
        //            for (int x = 0; x < dtTbl.Rows.Count; x++)
        //            {
        //                tbl += "<tr>" + Environment.NewLine;
        //                for (int y = 0; y < dtTbl.Columns.Count; y++)
        //                {
        //                    tbl += @"<td style=""border: solid 1px #ccc;"">" + dtTbl.Rows[x][dtTbl.Columns[y]] + "</td>" + Environment.NewLine;
        //                }
        //                tbl += "</tr>" + Environment.NewLine;
        //            }
        //            tbl += @"</tbody>" + Environment.NewLine;


        //            tbl += "</table>" + Environment.NewLine;

        //            str = str.Replace("#$" + strOriginal + "$#", tbl);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return str;
        //}


        private static void TrimiteMail(string mail, string subiect, string corpMail, int trimiteAtt, string numeAtt, string corpAtt, int trimiteXls, string selectXls, string numeExcel)
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

                if (mail.Trim() == "")
                {
                    General.MemoreazaEroarea(TraduCuvant("Nu exista destinatar"), "Notif", "TrimiteMail");
                    return;
                }
                else
                {
                    if (Convert.ToInt32(idClient) == 14)
                        mm.To.Add(new MailAddress("<" + mail + ">"));
                    else
                        mm.To.Add(new MailAddress(mail));
                }

                #region OLD
                //if (lstAdr == null || lstAdr.Count() == 0)
                //{
                //    General.MemoreazaEroarea(Dami.TraduCuvant("Nu exista destinatar"), "Dami", "TrimiteMail");
                //    return;
                //}
                //else
                //{
                //    foreach (var mail in lstAdr)
                //    {
                //        switch(mail.Destinatie.ToUpper())
                //        {
                //            case "TO":
                //                {
                //                    if (Session["IdClient"] == 14)
                //                        mm.To.Add(new MailAddress("<" + mail.Mail + ">"));
                //                    else
                //                        mm.To.Add(new MailAddress(mail.Mail));
                //                }
                //                break;
                //            case "CC":
                //                {
                //                    if (Session["IdClient"] == 14)
                //                        mm.CC.Add(new MailAddress("<" + mail.Mail + ">"));
                //                    else
                //                        mm.CC.Add(new MailAddress(mail.Mail));
                //                }
                //                break;
                //            case "BCC":
                //                {
                //                    if (Session["IdClient"] == 14)
                //                        mm.Bcc.Add(new MailAddress("<" + mail.Mail + ">"));
                //                    else
                //                        mm.Bcc.Add(new MailAddress(mail.Mail));
                //                }
                //                break;
                //        }
                //    }
                //}
#endregion

                mm.Subject = subiect;
                mm.Body = corpMail;
                mm.IsBodyHtml = true;
                
                //
                if (trimiteAtt == 1)
                {
                    byte[] arrByte = Encoding.UTF8.GetBytes(CreazaHTML(corpAtt));
                    MemoryStream stream = new MemoryStream(arrByte);
                    mm.Attachments.Add(new Attachment(stream, numeAtt, "text/html"));
                }

                //
                if (trimiteXls == 1)
                {
                    if (selectXls != "")
                    {
                        DateTime ora = DateTime.Now;
                        //string numeXLS = "SituatieConcedii_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xls";

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


        private static void TrimiteMail(List<metaAdreseMail> lstAdr, string subiect, string corpMail, int trimiteAtt, string numeAtt, string corpAtt, int trimiteXls, string selectXls, string numeExcel)
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
                        switch (mail.Destinatie.ToUpper())
                        {
                            case "TO":
                                {
                                    if (Convert.ToInt32(idClient) == 14)
                                        mm.To.Add(new MailAddress("<" + mail.Mail + ">"));
                                    else
                                        mm.To.Add(new MailAddress(mail.Mail));
                                }
                                break;
                            case "CC":
                                {
                                    if (Convert.ToInt32(idClient) == 14)
                                        mm.CC.Add(new MailAddress("<" + mail.Mail + ">"));
                                    else
                                        mm.CC.Add(new MailAddress(mail.Mail));
                                }
                                break;
                            case "BCC":
                                {
                                    if (Convert.ToInt32(idClient) == 14)
                                        mm.Bcc.Add(new MailAddress("<" + mail.Mail + ">"));
                                    else
                                        mm.Bcc.Add(new MailAddress(mail.Mail));
                                }
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
                    byte[] arrByte = Encoding.UTF8.GetBytes(CreazaHTML(corpAtt));
                    MemoryStream stream = new MemoryStream(arrByte);
                    mm.Attachments.Add(new Attachment(stream, numeAtt, "text/html"));
                }

                //
                if (trimiteXls == 1)
                {
                    if (selectXls != "")
                    {
                        DateTime ora = DateTime.Now;
                        //string numeXLS = "SituatieConcedii_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xls";

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

                //DataTable dt = General.IncarcaDT(@"SELECT * FROM ""tblAtasamente"" WHERE ""Tabela""=@1 AND ""Id""=@2", new string[] { tblAtasamente_Tabela, tblAtasamente_Id.ToString() });
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
                    //dr["IdAuto"] = 1;
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

                //SqlDataAdapter da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""tblFisiere"" ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;
                General.SalveazaDate(dt, "tblFisiere");

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Notif", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //public void CreeazaXLS(string strSelect)
        //{
        //    try
        //    {
        //        DataTable dt = General.IncarcaDT(strSelect, null);


        //        //IEnumerable<metaIstoricExtinsLunar> q = null;

        //        //q = GetIstoricExtinsLunar(idUser, -99, dataStart, dataSfarsit, structura, f10003);

        //        Microsoft.Office.Interop.Excel.Workbook book = new Microsoft.Office.Interop.Excel.Workbook();

        //        //File.Create(cale).Close();

        //        //book.LoadDocument(cale);

        //        //book.Worksheets.Remove(book.Worksheets["Sheet1"]);
        //        //book.Worksheets.Insert(0, "Sheet1");

        //        Microsoft.Office.Interop.Excel.Worksheet ws2 = book.Worksheets["Sheet1"];

        //        //IEnumerable<metaIstoricExtinsLunar> lst = q.ToList();

        //        int i = 1;
        //        int nr = 0;
        //        DateTime dt = dataStart;

        //        int nrZile = (dataSfarsit - dataStart).Days;
        //        nrZile++;

        //        Color color = Color.FromArgb(255, 255, 255);
        //        foreach (var el in lst)
        //        {
        //            bool cont = false;
        //            if ((el.F10003 == f10003 || ((el.Ziua1 == null || el.Ziua1.Trim() == "#FFFF0000") && (el.Ziua2 == null || el.Ziua2.Trim() == "#FFFF0000") && (el.Ziua3 == null || el.Ziua3.Trim() == "#FFFF0000") && (el.Ziua4 == null || el.Ziua4.Trim() == "#FFFF0000") && (el.Ziua5 == null || el.Ziua5.Trim() == "#FFFF0000") &&
        //                (el.Ziua6 == null || el.Ziua6.Trim() == "#FFFF0000") && (el.Ziua7 == null || el.Ziua7.Trim() == "#FFFF0000") && (el.Ziua8 == null || el.Ziua8.Trim() == "#FFFF0000") && (el.Ziua9 == null || el.Ziua9.Trim() == "#FFFF0000") && (el.Ziua10 == null || el.Ziua10.Trim() == "#FFFF0000") &&
        //                (el.Ziua11 == null || el.Ziua11.Trim() == "#FFFF0000") && (el.Ziua12 == null || el.Ziua12.Trim() == "#FFFF0000") && (el.Ziua13 == null || el.Ziua13.Trim() == "#FFFF0000") && (el.Ziua14 == null || el.Ziua14.Trim() == "#FFFF0000") && (el.Ziua15 == null || el.Ziua15.Trim() == "#FFFF0000") &&
        //                (el.Ziua16 == null || el.Ziua16.Trim() == "#FFFF0000") && (el.Ziua17 == null || el.Ziua17.Trim() == "#FFFF0000") && (el.Ziua18 == null || el.Ziua18.Trim() == "#FFFF0000") && (el.Ziua19 == null || el.Ziua19.Trim() == "#FFFF0000") && (el.Ziua20 == null || el.Ziua20.Trim() == "#FFFF0000") &&
        //                (el.Ziua21 == null || el.Ziua21.Trim() == "#FFFF0000") && (el.Ziua22 == null || el.Ziua22.Trim() == "#FFFF0000") && (el.Ziua23 == null || el.Ziua23.Trim() == "#FFFF0000") && (el.Ziua24 == null || el.Ziua24.Trim() == "#FFFF0000") && (el.Ziua25 == null || el.Ziua25.Trim() == "#FFFF0000") &&
        //                (el.Ziua26 == null || el.Ziua26.Trim() == "#FFFF0000") && (el.Ziua27 == null || el.Ziua27.Trim() == "#FFFF0000") && (el.Ziua28 == null || el.Ziua28.Trim() == "#FFFF0000") && (el.Ziua29 == null || el.Ziua29.Trim() == "#FFFF0000") && (el.Ziua30 == null || el.Ziua30.Trim() == "#FFFF0000") && (el.Ziua31 == null || el.Ziua31.Trim() == "#FFFF0000"))))
        //            {
        //                cont = true;
        //                if (i > 1)
        //                    continue;
        //            }
        //            nr++;
        //            ws2.Cells["A" + i].Value = (i == 1 ? "Angajat" : el.NumeComplet);

        //            if (i == 1 && !cont)
        //                ws2.Cells["A" + (i + 1).ToString()].Value = el.NumeComplet;

        //            if (nrZile >= 1)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["B" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua1 != null) ws2.Cells["B" + (i + 1).ToString()].FillColor = Culoare(el.Ziua1);
        //                }
        //                else
        //                    if (el.Ziua1 != null) ws2.Cells["B" + i].FillColor = Culoare(el.Ziua1);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 2)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["C" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua2 != null) ws2.Cells["C" + (i + 1).ToString()].FillColor = Culoare(el.Ziua2);
        //                }
        //                else
        //                    if (el.Ziua2 != null) ws2.Cells["C" + i].FillColor = Culoare(el.Ziua2);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 3)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["D" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua3 != null) ws2.Cells["D" + (i + 1).ToString()].FillColor = Culoare(el.Ziua3);
        //                }
        //                else
        //                    if (el.Ziua3 != null) ws2.Cells["D" + i].FillColor = Culoare(el.Ziua3);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 4)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["E" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua4 != null) ws2.Cells["E" + (i + 1).ToString()].FillColor = Culoare(el.Ziua4);
        //                }
        //                else
        //                    if (el.Ziua4 != null) ws2.Cells["E" + i].FillColor = Culoare(el.Ziua4);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 5)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["F" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua5 != null) ws2.Cells["F" + (i + 1).ToString()].FillColor = Culoare(el.Ziua5);
        //                }
        //                else
        //                    if (el.Ziua5 != null) ws2.Cells["F" + i].FillColor = Culoare(el.Ziua5);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 6)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["G" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua6 != null) ws2.Cells["G" + (i + 1).ToString()].FillColor = Culoare(el.Ziua6);
        //                }
        //                else
        //                    if (el.Ziua6 != null) ws2.Cells["G" + i].FillColor = Culoare(el.Ziua6);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 7)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["H" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua7 != null) ws2.Cells["H" + (i + 1).ToString()].FillColor = Culoare(el.Ziua7);
        //                }
        //                else
        //                    if (el.Ziua7 != null) ws2.Cells["H" + i].FillColor = Culoare(el.Ziua7);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 8)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["I" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua8 != null) ws2.Cells["I" + (i + 1).ToString()].FillColor = Culoare(el.Ziua8);
        //                }
        //                else
        //                    if (el.Ziua8 != null) ws2.Cells["I" + i].FillColor = Culoare(el.Ziua8);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 9)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["J" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua9 != null) ws2.Cells["J" + (i + 1).ToString()].FillColor = Culoare(el.Ziua9);
        //                }
        //                else
        //                    if (el.Ziua9 != null) ws2.Cells["J" + i].FillColor = Culoare(el.Ziua9);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 10)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["K" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua10 != null) ws2.Cells["K" + (i + 1).ToString()].FillColor = Culoare(el.Ziua10);
        //                }
        //                else
        //                    if (el.Ziua10 != null) ws2.Cells["K" + i].FillColor = Culoare(el.Ziua10);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 11)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["L" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua11 != null) ws2.Cells["L" + (i + 1).ToString()].FillColor = Culoare(el.Ziua11);
        //                }
        //                else
        //                    if (el.Ziua11 != null) ws2.Cells["L" + i].FillColor = Culoare(el.Ziua11);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 12)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["M" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua12 != null) ws2.Cells["M" + (i + 1).ToString()].FillColor = Culoare(el.Ziua12);
        //                }
        //                else
        //                    if (el.Ziua12 != null) ws2.Cells["M" + i].FillColor = Culoare(el.Ziua12);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 13)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["N" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua13 != null) ws2.Cells["N" + (i + 1).ToString()].FillColor = Culoare(el.Ziua13);
        //                }
        //                else
        //                    if (el.Ziua13 != null) ws2.Cells["N" + i].FillColor = Culoare(el.Ziua13);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 14)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["O" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua14 != null) ws2.Cells["O" + (i + 1).ToString()].FillColor = Culoare(el.Ziua14);
        //                }
        //                else
        //                    if (el.Ziua14 != null) ws2.Cells["O" + i].FillColor = Culoare(el.Ziua14);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 15)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["P" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua15 != null) ws2.Cells["P" + (i + 1).ToString()].FillColor = Culoare(el.Ziua15);
        //                }
        //                else
        //                    if (el.Ziua15 != null) ws2.Cells["P" + i].FillColor = Culoare(el.Ziua15);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 16)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["Q" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua16 != null) ws2.Cells["Q" + (i + 1).ToString()].FillColor = Culoare(el.Ziua16);
        //                }
        //                else
        //                    if (el.Ziua16 != null) ws2.Cells["Q" + i].FillColor = Culoare(el.Ziua16);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 17)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["R" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua17 != null) ws2.Cells["R" + (i + 1).ToString()].FillColor = Culoare(el.Ziua17);
        //                }
        //                else
        //                    if (el.Ziua17 != null) ws2.Cells["R" + i].FillColor = Culoare(el.Ziua17);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 18)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["S" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua18 != null) ws2.Cells["S" + (i + 1).ToString()].FillColor = Culoare(el.Ziua18);
        //                }
        //                else
        //                    if (el.Ziua18 != null) ws2.Cells["S" + i].FillColor = Culoare(el.Ziua18);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 19)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["T" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua19 != null) ws2.Cells["T" + (i + 1).ToString()].FillColor = Culoare(el.Ziua19);
        //                }
        //                else
        //                    if (el.Ziua19 != null) ws2.Cells["T" + i].FillColor = Culoare(el.Ziua19);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 20)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["U" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua20 != null) ws2.Cells["U" + (i + 1).ToString()].FillColor = Culoare(el.Ziua20);
        //                }
        //                else
        //                    if (el.Ziua20 != null) ws2.Cells["U" + i].FillColor = Culoare(el.Ziua20);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 21)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["V" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua21 != null) ws2.Cells["V" + (i + 1).ToString()].FillColor = Culoare(el.Ziua21);
        //                }
        //                else
        //                    if (el.Ziua21 != null) ws2.Cells["V" + i].FillColor = Culoare(el.Ziua21);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 22)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["W" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua22 != null) ws2.Cells["W" + (i + 1).ToString()].FillColor = Culoare(el.Ziua22);
        //                }
        //                else
        //                    if (el.Ziua22 != null) ws2.Cells["W" + i].FillColor = Culoare(el.Ziua22);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 23)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["X" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua23 != null) ws2.Cells["X" + (i + 1).ToString()].FillColor = Culoare(el.Ziua23);
        //                }
        //                else
        //                    if (el.Ziua23 != null) ws2.Cells["X" + i].FillColor = Culoare(el.Ziua23);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 24)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["Y" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua24 != null) ws2.Cells["Y" + (i + 1).ToString()].FillColor = Culoare(el.Ziua24);
        //                }
        //                else
        //                    if (el.Ziua24 != null) ws2.Cells["Y" + i].FillColor = Culoare(el.Ziua24);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 25)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["Z" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua25 != null) ws2.Cells["Z" + (i + 1).ToString()].FillColor = Culoare(el.Ziua25);
        //                }
        //                else
        //                    if (el.Ziua25 != null) ws2.Cells["Z" + i].FillColor = Culoare(el.Ziua25);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 26)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["AA" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua26 != null) ws2.Cells["AA" + (i + 1).ToString()].FillColor = Culoare(el.Ziua26);
        //                }
        //                else
        //                    if (el.Ziua26 != null) ws2.Cells["AA" + i].FillColor = Culoare(el.Ziua26);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 27)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["AB" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua27 != null) ws2.Cells["AB" + (i + 1).ToString()].FillColor = Culoare(el.Ziua27);
        //                }
        //                else
        //                    if (el.Ziua27 != null) ws2.Cells["AB" + i].FillColor = Culoare(el.Ziua27);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 28)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["AC" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua28 != null) ws2.Cells["AC" + (i + 1).ToString()].FillColor = Culoare(el.Ziua28);
        //                }
        //                else
        //                    if (el.Ziua28 != null) ws2.Cells["AC" + i].FillColor = Culoare(el.Ziua28);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 29)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["AD" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua29 != null) ws2.Cells["AD" + (i + 1).ToString()].FillColor = Culoare(el.Ziua29);
        //                }
        //                else
        //                    if (el.Ziua29 != null) ws2.Cells["AD" + i].FillColor = Culoare(el.Ziua29);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile >= 30)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["AE" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua30 != null) ws2.Cells["AE" + (i + 1).ToString()].FillColor = Culoare(el.Ziua30);
        //                }
        //                else
        //                    if (el.Ziua30 != null) ws2.Cells["AE" + i].FillColor = Culoare(el.Ziua30);
        //            }
        //            dt = dt.AddDays(1);
        //            if (nrZile == 31)
        //            {
        //                if (i == 1)
        //                {
        //                    ws2.Cells["AF" + i].Value = dt.Day.ToString().PadLeft(2, '0') + "." + dt.Month.ToString().PadLeft(2, '0');
        //                    if (el.Ziua31 != null) ws2.Cells["AF" + (i + 1).ToString()].FillColor = Culoare(el.Ziua31);
        //                }
        //                else
        //                    if (el.Ziua31 != null) ws2.Cells["AF" + i].FillColor = Culoare(el.Ziua31);
        //            }
        //            if (i == 1) i += 2;
        //            else i++;
        //        }

        //        ws2.Columns.AutoFit(0, 31);


        //        srvGeneral ctx = new srvGeneral();
        //        IEnumerable<metaLegendaCulori> r = ctx.LegendaCulori(2);
        //        IEnumerable<metaLegendaCulori> lstCulori = r.ToList();

        //        i = 1;
        //        string col1 = "", col2 = "";

        //        col1 = (nrZile <= 5 ? "I" : (nrZile > 5 && nrZile <= 10 ? "N" : (nrZile > 10 && nrZile <= 15 ? "S" : (nrZile > 15 && nrZile <= 20 ? "X" : (nrZile > 20 && nrZile <= 25 ? "AC" : "AI")))));
        //        col2 = (nrZile <= 5 ? "J" : (nrZile > 5 && nrZile <= 10 ? "O" : (nrZile > 10 && nrZile <= 15 ? "T" : (nrZile > 15 && nrZile <= 20 ? "Y" : (nrZile > 20 && nrZile <= 25 ? "AD" : "AJ")))));

        //        foreach (var el in lstCulori)
        //        {
        //            if (i == 1)
        //            {
        //                ws2.Cells[col1 + i.ToString()].Value = "Legenda";
        //                ws2.Cells[col1 + (i + 1).ToString()].Value = el.Descriere;
        //            }
        //            else
        //                ws2.Cells[col1 + i.ToString()].Value = el.Descriere;

        //            if (i == 1)
        //                ws2.Cells[col2 + (i + 1).ToString()].FillColor = Culoare(el.Culoare);
        //            else
        //                ws2.Cells[col2 + i.ToString()].FillColor = Culoare(el.Culoare);

        //            if (i == 1) i += 2;
        //            else i++;
        //        }

        //        ws2.Columns.AutoFit(nrZile + 3, nrZile + 3);
        //        ws2.Columns[nrZile + 4].Width = 60;


        //        book.Worksheets.ActiveWorksheet = book.Worksheets["Sheet1"];
        //        book.SaveDocument(cale);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}


        private static string CreazaHTML(string corpAtt)
        {
            string str = corpAtt;

            try
            {
                //string ert = baseUrl + Environment.NewLine;


                var virtualDir = VirtualPathUtility.ToAbsolute("~/");
                ////ert += virtualDir + Environment.NewLine;

                ////if (virtualDir != "" && virtualDir.Substring(virtualDir.Length - 1, 1) == "/")
                ////    virtualDir = virtualDir.Substring(0, virtualDir.Length - 1);
                ////ert += virtualDir + Environment.NewLine;

                ////ert += corpAtt.IndexOf("../UploadFiles/Images") + Environment.NewLine;
                ////ert += virtualDir + "../UploadFiles/Images" + Environment.NewLine;
                ////ert += baseUrl + virtualDir + "../UploadFiles/Images" + Environment.NewLine;
                ////ert += corpAtt + Environment.NewLine;



                //if (corpAtt.IndexOf("../UploadFiles/Images") >= 0)
                //    corpAtt = corpAtt.Replace("../UploadFiles/Images", baseUrl + virtualDir + "/UploadFiles/Images");
                //else
                //{
                //    if (corpAtt.IndexOf("/UploadFiles/Images") >= 0)
                //        corpAtt = corpAtt.Replace("/UploadFiles/Images", baseUrl + virtualDir + "/UploadFiles/Images");
                //    else
                //    {
                //        if (corpAtt.IndexOf("UploadFiles/Images") >= 0)
                //            corpAtt = corpAtt.Replace("UploadFiles/Images", baseUrl + virtualDir + "/UploadFiles/Images");
                //    }

                //}


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

        public static string TraduCuvant(string nume, string cuvant = "")
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