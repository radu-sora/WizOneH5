using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.XtraRichEdit.Commands.Internal;
using Oracle.ManagedDataAccess.Client;
using ProceseSec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IdentityModel.Services;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WizOne.Module
{
    public class General
    {

        public class metaCereriRol
        {
            public int Id { get; set; }
            public int Rol { get; set; }
            public int IdStare { get; set; }
            public string Nume { get; set; }
            public DateTime DataInceput { get; set; }
            public int F10003 { get; set; }
            public DateTime DataSfarsit { get; set; }
            public int NrOre { get; set; }
            public int NrZile { get; set; }
            public int IdAbsenta { get; set; }
        }

        public class metaPontaj
        {
            public int F10003 { get; set; }
            public int Luna { get; set; }
            public int An { get; set; }
        }

        public static void MemoreazaEroarea(Exception ex, string varPagina = "", string varEvenimentul = "")
        {
            string varEroarea = ex.Message.ToString();
            if (ex.InnerException != null) varEroarea += ex.InnerException.ToString();

            StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/Temp/") + "wsInfoState.txt", true);
            //
            string mesaj = "";

            mesaj += "Data:    " + DateTime.Now.ToString() + "\r\n";
            if (!string.IsNullOrEmpty(varPagina))
                mesaj += "Pagina:    " + varPagina + "\r\n";
            if (!string.IsNullOrEmpty(varEvenimentul))
                mesaj += "Eveni:     " + varEvenimentul + "\r\n";
            if (!string.IsNullOrEmpty(varEroarea))
                mesaj += "Eroarea:   " + varEroarea + "\r\n";
            //
            sw.Write(mesaj + "-----------------------------------------------------" + "\r\n");
            //
            sw.Close();
            sw.Dispose();
        }

        public static void MemoreazaEroarea(string msg, string varPagina = "", string varEvenimentul = "")
        {
            string varEroarea = msg;

            StackTrace st = new StackTrace();
            if (varEvenimentul == "") varEvenimentul = st.GetFrame(0).GetMethod().Name;

            StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/Temp/") + "wsInfoState.txt", true);
            //
            string mesaj = "";

            mesaj += "Data:    " + DateTime.Now.ToString() + "\r\n";
            if (!string.IsNullOrEmpty(varPagina))
                mesaj += "Pagina:    " + varPagina + "\r\n";
            if (!string.IsNullOrEmpty(varEvenimentul))
                mesaj += "Eveni:     " + varEvenimentul + "\r\n";
            if (!string.IsNullOrEmpty(varEroarea))
                mesaj += "Eroarea:   " + varEroarea + "\r\n";
            //
            sw.Write(mesaj + "-----------------------------------------------------" + "\r\n");
            //
            sw.Close();
            sw.Dispose();
        }

        public static void CreazaLog(string msg, string strMetoda = "")
        {
            StackTrace st = new StackTrace();
            StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/Temp/") + "woLogNtf.txt", true);
            //
            string mesaj = "";

            mesaj += "Data:    " + DateTime.Now.ToString() + "\r\n";
            if (!string.IsNullOrEmpty(strMetoda)) mesaj += "Metoda:    " + strMetoda + "\r\n";
            if (!string.IsNullOrEmpty(msg)) mesaj += "Eroarea:   " + msg + "\r\n";
            //
            sw.Write(mesaj + "-----------------------------------------------------" + "\r\n");
            //
            sw.Close();
            sw.Dispose();
        }

        public static void CreazaLogCereri(string msg, string f10003, string dataInceput)
        {
            StackTrace st = new StackTrace();
            StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/Temp/") + "woLogCereri.txt", true);
            //
            string mesaj = "";

            mesaj += "Data:    " + DateTime.Now.ToString() + "\r\n";
            if (!string.IsNullOrEmpty(f10003)) mesaj += "Marca:    " + f10003 + "\r\n";
            if (!string.IsNullOrEmpty(dataInceput)) mesaj += "Data Inceput:    " + dataInceput + "\r\n";
            if (!string.IsNullOrEmpty(msg)) mesaj += "Select:   " + msg + "\r\n";
            //
            sw.Write(mesaj + "-----------------------------------------------------" + "\r\n");
            //
            sw.Close();
            sw.Dispose();
        }

        public static void CreazaLogFormuleCumulat(string msg, string strMetoda = "")
        {
            StackTrace st = new StackTrace();
            StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/Temp/") + "woLogFormuleCumulat.txt", true);
            //
            string mesaj = "";

            mesaj += "Data:    " + DateTime.Now.ToString() + "\r\n";
            if (!string.IsNullOrEmpty(strMetoda)) mesaj += "Metoda:    " + strMetoda + "\r\n";
            if (!string.IsNullOrEmpty(msg)) mesaj += "Eroarea:   " + msg + "\r\n";
            //
            sw.Write(mesaj + "-----------------------------------------------------" + "\r\n");
            //
            sw.Close();
            sw.Dispose();
        }

        public static string Strip(string txt)
        {
            string rez = txt;

            try 
	        {
                //if (!Regex.IsMatch(txt,@"^[a-zA-Z'.\s]{0,40}$")) rez = "";
                rez = txt.Trim(new Char[] { ' ','/','-','(',')','*','&','%','$','#' });
	        }
	        catch (Exception ex)
	        {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
	        }

            return rez;
        }

        public static DataTable IncarcaDT(string strSql, object[] lstParam = null, string primaryKey = "")
        {          
            DataTable dt = new DataTable();

            try
            {
                if (primaryKey != "")
                {
                    string[] lst = primaryKey.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lst.Length > 0)
                    {
                        DataColumn[] cols = new DataColumn[lst.Length];

                        for (int i = 0; i < lst.Length; i++)
                        {
                            cols[i] = dt.Columns[lst[i]];
                        }

                        dt.PrimaryKey = cols;               
                    }
                }

                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    da.SelectCommand = DamiSqlCommand(strSql, lstParam, 0);
                    da.SelectCommand.CommandTimeout = 600;       
                    da.Fill(dt);
                    da.Dispose();
                    da = null;
                }
                else
                {
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = DamiOleDbCommand(strSql, lstParam);
                    da.SelectCommand.CommandTimeout = 600;
                    da.Fill(dt);
                    da.Dispose();
                    da = null;
                }
            }
            catch (Exception ex)
            {
                if (strSql.IndexOf("Eval_") < 0)
                {
                    MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                    MemoreazaEroarea(strSql, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                }
            }
            return dt;
        }

        public static DataRow IncarcaDR(string strSql, object[] lstParam = null)
        {
            DataRow dr = null;

            try
            {
                DataTable dt = new DataTable();
                dt = IncarcaDT(strSql, lstParam);
                if (dt.Rows.Count > 0) dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dr;
        }

        public static object ExecutaScalar(string strSql, object[] lstParam = null)
        {
            object str = null;

            try
            {
                DataTable dt = IncarcaDT(strSql, lstParam);
                if (dt.Rows.Count > 0) str = dt.Rows[0][0];
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }


        public static bool ExecutaNonQuery(string sql, object[] lstParam = null)
        {
            try
            {
                if (Constante.tipBD == 1)
                {
                    SqlCommand cm = DamiSqlCommand(sql, lstParam, 1);
                    if (cm == null)
                        return false;
                    else
                        return true;
                }
                else
                {
                    OracleCommand cm = DamiOleDbCommand(sql, lstParam, 1);
                    if (cm == null)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return false;
            }
        }

        public static string SalveazaDate(DataTable dt, string tabela)
        {
            string ras = "";

            try
            {
                if (Constante.tipBD == 1)
                {
                    string strSql = @"SELECT TOP 0 * FROM ""{0}"" ";
                    strSql = string.Format(strSql, tabela);

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = General.DamiSqlCommand(strSql, null);
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    cb.ConflictOption = ConflictOption.OverwriteChanges;
                    da.Update(dt);
                    dt.AcceptChanges();

                    da.Dispose();
                    da = null;
                }
                else
                {
                    string strSql = @"SELECT * FROM ""{0}"" WHERE ROWNUM = 0";
                    strSql = string.Format(strSql, tabela);

                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = General.DamiOleDbCommand(strSql, null);
                    OracleCommandBuilder cb = new OracleCommandBuilder(da);
                    cb.ConflictOption = ConflictOption.OverwriteChanges;
                    da.Update(dt);
                    dt.AcceptChanges();

                    da.Dispose();
                    da = null;
                }
            }
            catch (Exception ex)
            {
                ras = ex.Message;
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        public static string TrimiteMail(List<string> lstTO, List<string> lstCC, List<string> lstBCC, string subiect, string corpMail)
        {
            string strErr = "";
            string strMsg = "";
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
                    strErr = "Nu exista date despre " + strMsg.Substring(2);
                    return strErr;
                }

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(smtpMailFrom);

                if (lstTO == null || lstTO.Count() == 0)
                    return "Nu exista destinatar !";
                else
                {
                    foreach (var mail in lstTO)
                    {
                        if (mail != "")
                        {
                            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                                mm.To.Add(new MailAddress("<" + mail + ">"));
                            else
                                mm.To.Add(new MailAddress(mail));
                        }
                    }
                }

                if (lstCC != null && lstCC.Count() > 0)
                {
                    foreach (var mail in lstCC)
                    {
                        if (mail != "")
                        {
                            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                                mm.CC.Add(new MailAddress("<" + mail + ">"));
                            else
                                mm.CC.Add(new MailAddress(mail));
                        }
                    }
                }

                if (lstBCC != null && lstBCC.Count() > 0)
                {
                    foreach (var mail in lstBCC)
                    {
                        if (mail != "")
                        {
                            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                                mm.Bcc.Add(new MailAddress("<" + mail + ">"));
                            else
                                mm.Bcc.Add(new MailAddress(mail));
                        }
                    }
                }

                mm.Subject = subiect;
                mm.Body = corpMail;
                mm.IsBodyHtml = true;

                smtp = new SmtpClient(smtpServer);
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

                strErr = "";

                smtp.Dispose();

            }
            catch (Exception ex)
            {
                strErr = ex.Message.ToString();
                if (ex.InnerException != null) strErr += ex.InnerException.ToString();

                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strErr;
        }

        public static string TrimiteMail(string lstTO, string lstCC, string lstBCC, string subiect, string corpMail)
        {
            string strErr = "";
            string strMsg = "";
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
                    strErr = "Nu exista date despre " + strMsg.Substring(2);
                    return strErr;
                }

                MailMessage mm = new MailMessage();

                mm.From = new MailAddress(smtpMailFrom);

                if (lstTO == "")
                    return "Nu exista destinatar !";
                else
                {
                    string[] mailTO = lstTO.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var mail in mailTO)
                    {
                        if (mail != "")
                        {
                            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                                mm.To.Add(new MailAddress("<" + mail + ">"));
                            else
                                mm.To.Add(new MailAddress(mail));
                        }
                    }
                }

                if (lstCC != "")
                {
                    string[] mailCC = lstCC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var mail in mailCC)
                    {
                        if (mail != "")
                        {
                            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                                mm.CC.Add(new MailAddress("<" + mail + ">"));
                            else
                                mm.CC.Add(new MailAddress(mail));
                        }
                    }
                }

                if (lstBCC != "")
                {
                    string[] mailBCC = lstBCC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var mail in mailBCC)
                    {
                        if (mail != "")
                        {
                            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                                mm.Bcc.Add(new MailAddress("<" + mail + ">"));
                            else
                                mm.Bcc.Add(new MailAddress(mail));
                        }
                    }
                }

                mm.Subject = subiect;
                mm.Body = corpMail;
                mm.IsBodyHtml = true;

                smtp = new SmtpClient(smtpServer);
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

                strErr = "";

                smtp.Dispose();

            }
            catch (Exception ex)
            {
                strErr = ex.Message.ToString();
                if (ex.InnerException != null) strErr += ex.InnerException.ToString();

                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strErr;
        }

        public static SqlCommand DamiSqlCommand(string strSql, object[] lstParam, int executa = 0)
        {
            SqlConnection conn = new SqlConnection(Constante.cnnWeb);

            while (true)
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1225)
                    {
                        MemoreazaEroarea(
                            "Number - " + ex.Number + Environment.NewLine +
                            "Message - " + ex.Message + Environment.NewLine +
                            "Errors - " + ex.Errors + Environment.NewLine +
                            "ErrorCode - " + ex.ErrorCode, "Rollback", new StackTrace().GetFrame(0).GetMethod().Name);
                        continue;
                    }

                    MemoreazaEroarea(
                        "Number - " + ex.Number + Environment.NewLine + 
                        "Message - " + ex.Message + Environment.NewLine + 
                        "Errors - " + ex.Errors + Environment.NewLine + 
                        "ErrorCode - " + ex.ErrorCode, "Rollback", new StackTrace().GetFrame(0).GetMethod().Name);
                }

                break;
            }


            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                if (HttpContext.Current != null && General.Nz(HttpContext.Current.Session["SecAuditSelect"], "0").ToString() == "1")
                {
                    string[] arr = Constante.campuriGDPR.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arr.Any(strSql.Contains))
                    {
                        ProceseSec.CriptDecript cls = new ProceseSec.CriptDecript();
                        ExecutaNonQuery($@"INSERT INTO tblLogSelect(IdUser, Zi, Sursa) VALUES(@1, {CurrentDate()}, @2)", new object[] { HttpContext.Current.Session["UserId"], cls.EncryptString("WizOne2016", strSql, 1) });
                    }
                }

                if (HttpContext.Current != null && General.Nz(HttpContext.Current.Session["SecCriptare"], "0").ToString() == "1")
                {
                    string[] arr = Constante.campuriGDPR.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arr.Any(strSql.Contains))
                    {
                        string[] arrSimplu = Constante.campuriGDPR_Strip.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < arrSimplu.Length; i++)
                        {
                            strSql = strSql.Replace("CRP." + arrSimplu[i], "CASE WHEN CRP." + arrSimplu[i] + " IS NOT NULL THEN dbo.nuf('" + arrSimplu[i] + "', CRP.F10003, CRP." + arrSimplu[i] + ") ELSE CRP." + arrSimplu[i] + " END ");
                        }
                    }
                }
            }catch (Exception){}


            SqlCommand cmd = new SqlCommand(strSql, conn, tran);
            cmd.CommandTimeout = 600;

            try
            {
                int x = 0;
                if (lstParam != null)
                {
                    foreach (object param in lstParam)
                    {
                        x += 1;
                        try
                        {
                            cmd.Parameters.AddWithValue("@" + x.ToString(), (param == null ? DBNull.Value : param));
                        }
                        catch (Exception ex)
                        {
                            MemoreazaEroarea(ex, "General", "DamiSqlCommand - Parametrii");
                        }
                    }
                }

                if (executa == 1) cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    tran.Rollback();
                }
                catch (Exception exRollback)
                {
                    MemoreazaEroarea(exRollback, "Rollback", new StackTrace().GetFrame(0).GetMethod().Name);
                }

                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                MemoreazaEroarea(strSql, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                var ert = strSql;
                cmd = null;
            }
            finally
            {
                conn.Close();
            }

            return cmd;
        }

        public static OracleCommand DamiOleDbCommand(string strSql, object[] lstParam, int executa = 0)
        {
            OracleConnection conn = new OracleConnection(Constante.cnnWeb);
            conn.Open();

            OracleCommand cmd = new OracleCommand(strSql, conn);

            strSql = strSql.Replace("@", ":param");

            try
            {
                int x = 0;
                bool areOut = false;
                if (lstParam != null)
                {
                    foreach (object param in lstParam)
                    {
                        x += 1;
                        try
                        {
                            if (General.Nz(param,"").ToString().Trim() == "")
                                cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("param" + x.ToString(), null));
                            else
                            {
                                if (param.GetType().Name == "Byte[]")
                                    cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("param" + x.ToString(), OracleDbType.Blob)).Value = param;
                                else
                                    cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("param" + x.ToString(), param));
                            }
                        }
                        catch (Exception ex)
                        {
                            MemoreazaEroarea(ex, "General", "DamiOleDbCommand - Parametrii");
                        }
                    }
                }
                cmd.CommandText = strSql;
                cmd.BindByName = true;
                
                if (executa == 1 || areOut == true) cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                conn.Close();
            }

            return cmd;
        }

        public static dynamic DamiOracleScalar(string strSql, object[] lstParam)
        {
            dynamic rez = null;

            OracleConnection conn = new OracleConnection(Constante.cnnWeb);
            conn.Open();

            strSql = strSql.Replace("GLOBAL.IDUSER", (HttpContext.Current.Session["UserId"] ?? "").ToString()).Replace("GLOBAL.MARCA", (HttpContext.Current.Session["User_Marca"] ?? "").ToString());

            OracleCommand cmd = new OracleCommand(strSql, conn);

            strSql = strSql.Replace("@", ":param");

            string paramOut = "";

            try
            {
                //primul parametru este intotdeauna cel care se intoarce
                int x = 0;
                if (lstParam != null)
                {
                    foreach (object param in lstParam)
                    {
                        x += 1;
                        if (param != null && param.ToString().Length > 0)
                        {
                            try
                            {
                                if (strSql.IndexOf("paramout_" + x.ToString()) >= 0)
                                {
                                    cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("paramout_" + x.ToString(), OracleDbType.Int32, ParameterDirection.ReturnValue));
                                    paramOut += "," + "paramout_" + x.ToString();
                                }
                                else
                                    cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("param" + x.ToString(), param.ToString()));
                            }
                            catch (Exception ex)
                            {
                                MemoreazaEroarea(ex, "General", "DamiOleDbCommand - Parametrii");
                            }
                        }
                    }
                }
                cmd.CommandText = strSql;
                cmd.BindByName = true;
                cmd.ExecuteNonQuery();

                if(paramOut != "")
                {
                    string[] arr = paramOut.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> lstOut = new List<string>(); 
                    for(int i = 0; i < arr.Length; i++)
                    {
                        lstOut.Add(General.Nz(cmd.Parameters[arr[i]].Value, "-99").ToString());
                    }

                    if (arr.Length > 0)
                    {
                        if (arr.Length == 1)
                            rez = General.Nz(cmd.Parameters[arr[0]].Value, "-99").ToString();
                        else
                            rez = lstOut;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                conn.Close();
            }

            return rez;
        }

        public static void ExecutaNonQueryOracle(string procNume, object[] lstParam)
        {
            OracleConnection conn = new OracleConnection(Constante.cnnWeb);
            conn.Open();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = procNume;
                cmd.CommandType = CommandType.StoredProcedure;
                if (lstParam != null)
                {
                    foreach (object param in lstParam)
                    {
                        try
                        {
                            string[] arr = param.ToString().Split('=');
                            cmd.Parameters.Add(arr[0], OracleDbType.Varchar2).Value = arr[1];
                        }
                        catch (Exception ex)
                        {
                            MemoreazaEroarea(ex, "General", "DamiOleDbCommand - Parametrii");
                        }
                    }
                }

                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                conn.Close();
            }
        }

        // For a projection with many columns (T must be a model class)
        public static List<T> RunSqlQuery<T>(string sql, params object[] paramList) where T : class, new()
        {
            var command = Constante.tipBD == 1 ?
                new SqlCommand(sql.Replace("@", "@p"), new SqlConnection(Constante.cnnWeb)) as DbCommand :
                new OracleCommand(sql.Replace('[', '"').Replace(']', '"').Replace("@", ":p"), new OracleConnection(Constante.cnnWeb))
                {
                    BindByName = true
                };
            var items = new List<T>();

            try
            {                            
                if (paramList != null)
                {
                    for (int param = 0; param < paramList.Length; param++)
                    {
                        var name = "p" + (param + 1);
                        var value = paramList[param] ?? DBNull.Value;
                        var parameter = Constante.tipBD == 1 ?
                            new SqlParameter(name, value) as DbParameter :
                            new OracleParameter(name, value.GetType() == typeof(bool) ? Convert.ToByte(value) : value); // TODO: FIX01 - A more generic solution must be found.

                        command.Parameters.Add(parameter);
                    }
                }

                command.CommandTimeout = 600;
                command.Connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        var properties = item.GetType().GetProperties();

                        foreach (var property in properties)
                        {
                            var value = reader[property.Name];

                            if (value == DBNull.Value)
                            {
                                if (property.PropertyType.IsValueType)
                                    value = Activator.CreateInstance(property.PropertyType);
                                else
                                    value = null;
                            }

                            property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                        }

                        items.Add(item);
                    }
                }                                                
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
                command.Dispose();
            }

            return items;
        }

        // For a projection with one column (T must be a primitive or a string)
        public static List<T> RunSqlColumn<T>(string sql, params object[] paramList)
        {
            var command = Constante.tipBD == 1 ?
                new SqlCommand(sql.Replace("@", "@p"), new SqlConnection(Constante.cnnWeb)) as DbCommand :
                new OracleCommand(sql.Replace('[', '"').Replace(']', '"').Replace("@", ":p"), new OracleConnection(Constante.cnnWeb))
                {
                    BindByName = true
                };
            var items = new List<T>();

            try
            {
                if (paramList != null)
                {
                    for (int param = 0; param < paramList.Length; param++)
                    {
                        var name = "p" + (param + 1);
                        var value = paramList[param] ?? DBNull.Value;
                        var parameter = Constante.tipBD == 1 ?
                            new SqlParameter(name, value) as DbParameter :
                            new OracleParameter(name, value.GetType() == typeof(bool) ? Convert.ToByte(value) : value); // TODO: FIX01

                        command.Parameters.Add(parameter);
                    }
                }

                command.CommandTimeout = 600;
                command.Connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var value = reader[0];

                        if (value == DBNull.Value)
                            value = default(T);

                        items.Add((T)Convert.ChangeType(value, typeof(T)));
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
                command.Dispose();
            }

            return items;
        }

        // For a projection with many columns and a single result (T must be a model class)
        public static T RunSqlSingle<T>(string sql, params object[] paramList) where T : class, new()
        {
            var command = Constante.tipBD == 1 ?
                new SqlCommand(sql.Replace("@", "@p"), new SqlConnection(Constante.cnnWeb)) as DbCommand :
                new OracleCommand(sql.Replace('[', '"').Replace(']', '"').Replace("@", ":p"), new OracleConnection(Constante.cnnWeb))
                {
                    BindByName = true
                };
            var item = null as T;

            try
            {
                if (paramList != null)
                {
                    for (int param = 0; param < paramList.Length; param++)
                    {
                        var name = "p" + (param + 1);
                        var value = paramList[param] ?? DBNull.Value;
                        var parameter = Constante.tipBD == 1 ?
                            new SqlParameter(name, value) as DbParameter :
                            new OracleParameter(name, value.GetType() == typeof(bool) ? Convert.ToByte(value) : value); // TODO: FIX01

                        command.Parameters.Add(parameter);
                    }
                }

                command.CommandTimeout = 600;
                command.Connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {                    
                    if (reader.Read())
                    {
                        item = new T();

                        var properties = item.GetType().GetProperties();

                        foreach (var property in properties)
                        {
                            var value = reader[property.Name];

                            if (value == DBNull.Value)
                            {
                                if (property.PropertyType.IsValueType)
                                    value = Activator.CreateInstance(property.PropertyType);
                                else
                                    value = null;
                            }

                            property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
                command.Dispose();
            }

            return item;
        }

        // For scalar and non-query (T must be a primitive or a string)
        public static T RunSqlScalar<T>(string sql, string pk, params object[] paramList)
        {
            var command = Constante.tipBD == 1 ?
                new SqlCommand(sql.Replace("@", "@p"), new SqlConnection(Constante.cnnWeb)) as DbCommand :
                new OracleCommand(sql.Replace('[', '"').Replace(']', '"').Replace("@", ":p"), new OracleConnection(Constante.cnnWeb))
                {
                    BindByName = true
                };
            var result = null as object;

            try
            {
                if (!string.IsNullOrEmpty(pk)) // Return the pk identity value
                {
                    command.CommandText = command.CommandText.Trim(new char[] { ' ', ';' }) +
                        (Constante.tipBD == 1 ? $" SELECT scope_identity()" : $@" RETURNING ""{pk.Trim()}"" INTO :pk");

                    if (Constante.tipBD != 1)
                        command.Parameters.Add(new OracleParameter("pk", OracleDbType.Int64, ParameterDirection.Output));
                }

                if (paramList != null)
                {
                    for (int param = 0; param < paramList.Length; param++)
                    {
                        var name = "p" + (param + 1);
                        var value = paramList[param] ?? DBNull.Value;
                        var parameter = Constante.tipBD == 1 ?
                            new SqlParameter(name, value) as DbParameter :
                            new OracleParameter(name, value.GetType() == typeof(bool) ? Convert.ToByte(value) : value); // TODO: FIX01

                        command.Parameters.Add(parameter);
                    }
                }

                command.CommandTimeout = 600;
                command.Connection.Open();

                if (command.CommandText.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    if ((result = command.ExecuteScalar()) != DBNull.Value)
                        result = (T)Convert.ChangeType(result, typeof(T));                    
                    else
                        result = default(T);
                }
                else
                    result = (T)Convert.ChangeType(command.ExecuteNonQuery(), typeof(T));

                if (!string.IsNullOrEmpty(pk) && Constante.tipBD != 1)
                    result = (T)Convert.ChangeType(command.Parameters["pk"].Value.ToString(), typeof(T));
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
                command.Dispose();
            }

            return result != null ? (T)result : default;
        }        

        public static void SetLimba()
        {
            try
            {
                CultureInfo lmb = new CultureInfo(VarSession("IdLimba").ToString());
                Thread.CurrentThread.CurrentCulture = lmb;
                Thread.CurrentThread.CurrentUICulture = lmb;
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static object Nz(object valoare, object inloc)
        {
            object rez = valoare;

            try
            {
                if (valoare != null && valoare != DBNull.Value && valoare.ToString() != "" && valoare.ToString().ToLower().Trim() != "null")
                    rez = valoare;
                else
                    rez = inloc;
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static string VerificaComplexitateParola(string password)
        {
            string ras = "";

            try
            {
                Regex upper = new Regex("[A-Z]");
                Regex lower = new Regex("[a-z]");
                Regex number = new Regex("[0-9]");
                Regex special = new Regex("[^a-zA-Z0-9]");

                if (password.Trim().Length < Convert.ToInt32(Dami.ValoareParam("Parola_LungimeMinima","100")))
                    ras += ", trebuie sa aiba minim " + Dami.ValoareParam("Parola_LungimeMinima", "100") + " caractere";

                if (Dami.ValoareParam("Parola_Cifre", "0") == "1" && number.Matches(password).Count < 1)
                    ras += ", trebuie sa contina cifre";

                if (Dami.ValoareParam("Parola_LitereMari", "0") == "1" && upper.Matches(password).Count < 1)
                    ras += ", trebuie sa contina litere mari";

                if (Dami.ValoareParam("Parola_LitereMici", "0") == "1" && lower.Matches(password).Count < 1)
                    ras += ", trebuie sa contina litere mici";

                if (Dami.ValoareParam("Parola_CaractereSpeciale", "0") == "1" && special.Matches(password).Count < 1)
                    ras += ", trebuie sa contina caractere speciale";
                
                if (Dami.ValoareParam("Parola_CaractIdentSuccesive", "0") == "1")
                {
                    for (int i = 0; i < password.Trim().Length - 1; i++)
                        if (password[i] == password[i + 1])
                        {
                            ras += ", contine caractere identice succesive";
                            break;
                        }
                }

                if (Dami.ValoareParam("Parola_ContineNumeUser", "0") == "1")
                {
                    String USR = General.Nz(HttpContext.Current.Session["User"], "").ToString();
                    if (USR.Length > 0 && password.ToUpper().Contains(USR.ToUpper()))
                        ras += ", contine numele utilizatorului";
                }

                CriptDecript prc = new CriptDecript();
                int nrMinParole = 0;
                try
                {
                    nrMinParole = Convert.ToInt32(Dami.ValoareParam("Parola_NrMinParole", "0"));
                }catch (Exception){}

                if (nrMinParole > 0)
                {
                    string strSql = $@"SELECT COUNT(*) FROM (
                        SELECT ROW_NUMBER() OVER(ORDER BY Y.""Data"" DESC) AS ""NrCrt"", Y.* FROM ""ParoleUtilizatorIstoric"" Y WHERE Y.""IdUser""=@1) X
                        WHERE X.""NrCrt"" <= @2 AND ""Parola""=@3";

                    string pwd = prc.EncryptString(Constante.cheieCriptare, password, 1);
                    if (Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, new object[] { HttpContext.Current.Session["UserId"] , nrMinParole, pwd}),0)) > 0)
                        ras += ", a mai fost utilizata";
                }

                if (Dami.ValoareParam("Parola_ContineUltimaParola", "0") == "1")
                {
                    string parola = General.Nz(General.ExecutaScalar("SELECT F70103 FROM USERS WHERE F70102=@1", new object[] { HttpContext.Current.Session["UserId"]  }), "").ToString();
                    if (parola != "")
                    {
                        string pwd = prc.EncryptString(Constante.cheieCriptare, parola, Constante.DECRYPT);
                        if (password.ToUpper().Contains(pwd.ToUpper()))
                            ras += ", face parte din ultima parola folosita";
                    }
                }


                try
                {
                    int xCaractere = Convert.ToInt32(Dami.ValoareParam("Parola_ContinePrimeleX", "0"));
                    if (xCaractere > 0)
                    {
                        string parola = General.Nz(General.ExecutaScalar("SELECT F70103 FROM USERS WHERE F70102=@1", new object[] { HttpContext.Current.Session["UserId"] }), "").ToString();
                        if (parola != "")
                        {
                            string pwd = prc.EncryptString(Constante.cheieCriptare, parola, Constante.DECRYPT);
                            if (password.Substring(0, xCaractere).ToLower() == pwd.Substring(0,xCaractere).ToLower())
                                ras += ", primele " + xCaractere + " caractere din parola veche se regasesc si in parola noua";
                        }
                    }
                }catch (Exception){}

                if (ras != "") ras = "parola " + ras.Substring(1);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        public static bool IsValidEmail(string email)
        {
            var boolRez = true;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                boolRez = false;
            }

            return boolRez;
        }

        public static string CreazaSelectFromRow(DataRow dr, string camp = "", bool gol = false)
        {
            string str = "";
            
            try
            {
                for(int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    var val = dr[i];
                    if (gol)
                    {
                        val = null;
                        if (dr.Table.Columns[i].ColumnName == "F10003")
                            continue;
                    }

                    switch (dr.Table.Columns[i].DataType.ToString())
                    {
                        case "System.String":
                            str += ", '" + Nz(val, "") + "' AS '" + dr.Table.Columns[i].ColumnName + "'";
                            break;
                        case "System.DateTime":
                            DateTime dt = Convert.ToDateTime(Nz(val, new DateTime(1900, 1, 1)));
                            str += ", " + ToDataUniv(dt) + " AS '" + dr.Table.Columns[i].ColumnName + "'";
                            break;
                        case "System.Double":
                        case "System.Decimal":
                            str += ", " + Nz(val, "null").ToString().Replace(",", ".") + " AS '" + dr.Table.Columns[i].ColumnName + "'";
                            break;
                        default:
                            str += ", " + Nz(val, "null") + " AS '" + dr.Table.Columns[i].ColumnName + "'";
                            break;
                    }
                }

                if (str != "")
                {
                    str = "SELECT " + str.Substring(1) + camp;
                    if (Constante.tipBD == 2) str += " FROM DUAL";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }

        public static string ToDataOrcl(object obj)
        {
            string rez = "";

            try
            {
                if (General.Nz(obj, "").ToString() == "")
                    rez = "01-01-1900";
                else
                {
                    DateTime dt = Convert.ToDateTime(obj);
                    rez = dt.Day.ToString().PadLeft(2, '0') + "-" + Dami.NumeLuna(dt.Month, 1, "EN") + "-" + dt.Year;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static string ToDataUniv(DateTime? dt, bool andTime = false)
        {
            string rez = "";

            try
            {
                if (dt != null)
                {
                    string an = dt.Value.Year.ToString();
                    string luna = dt.Value.Month.ToString().PadLeft(2, '0');
                    string zi = dt.Value.Day.ToString().PadLeft(2, '0');
                    string ora = dt.Value.Hour.ToString().PadLeft(2, '0');
                    string min = dt.Value.Minute.ToString().PadLeft(2, '0');
                    string sec = dt.Value.Second.ToString().PadLeft(2, '0');
                    string mask = "DD-MM-YYYY";

                    switch (Constante.tipBD)
                    {
                        case 1:
                            rez = an + "-" + luna + "-" + zi;
                            if (andTime) rez += " " + ora + ":" + min + ":" + sec;
                            rez = "'" + rez + "'";
                            if (!andTime) rez = "CONVERT(date," + rez + ")";
                            break;
                        case 2:
                            rez = zi.ToString().PadLeft(2, '0') + "-" + luna.ToString().PadLeft(2,'0') + "-" + an;
                            if (andTime)
                            {
                                mask = "DD-MM-YYYY HH24:MI:SS";
                                rez += " " + ora + ":" + min + ":" + sec;
                            }
                            rez = "TO_DATE('" + rez + "','" + mask + "')";
                            if (!andTime) rez = "TRUNC(" + rez + ")";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "ToDataUniv");
            }

            return rez;
        }

        public static string ToDataUniv(int anul, int luna, int zi = 1)
        {
            //tip = 99 - ultima zi din luna

            string rez = "";
            DateTime? dt = null;

            try
            {
                switch (Constante.tipBD)
                {
                    case 1:
                        if (zi == 99)
                            dt = new DateTime(anul, luna, DateTime.DaysInMonth(anul, luna));
                        else
                            dt = new DateTime(anul, luna, zi);

                        if (dt != null) rez = "'" + dt.Value.Year.ToString() + "-" + dt.Value.Month.ToString().PadLeft(2, '0') + "-" + dt.Value.Day.ToString().PadLeft(2, '0') + "'";
                        break;
                    case 2:
                        if (zi == 99)
                            dt = new DateTime(anul, luna, DateTime.DaysInMonth(anul, luna));
                        else
                            dt = new DateTime(anul, luna, zi);

                        if (dt != null) rez = "TO_DATE('" + dt.Value.Day.ToString().PadLeft(2, '0') + "-" + dt.Value.Month.ToString().PadLeft(2, '0') + "-" + dt.Value.Year.ToString() + "','DD-MM-YYYY')";
                        break;
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "ToDataUniv");
            }

            return rez;
        }

        public static string TruncateDate(string camp)
        {
            string rez = camp;

            try
            {
                if (camp != "")
                {
                    if (camp.IndexOf(".") >= 0)
                        camp = camp.Replace(".", ".\"");
                    else
                        camp = "\"" + camp;

                    camp = camp + "\"";

                    if (Constante.tipBD == 1)
                        rez = "CONVERT(date," + camp + ")";
                    else
                        rez = "TRUNC(" + camp + ")";
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "TruncateTime");
            }

            return rez;
        }

        public static string FunctiiData(string cmp, string tip)
        {
            string rez = cmp;

            try
            {
                switch(tip.ToUpper())
                {
                    case "A":
                        {
                            if (Constante.tipBD == 1)
                                rez = "YEAR(" + cmp + ")";
                            else
                                rez = "TO_NUMBER(TO_CHAR(" + cmp + ",'YYYY'))";
                        }
                        break;
                    case "L":
                        {
                            if (Constante.tipBD == 1)
                                rez = "MONTH(" + cmp + ")";
                            else
                                rez = "TO_NUMBER(TO_CHAR(" + cmp + ",'MM'))";
                        }
                        break;
                    case "Z":
                        {
                            if (Constante.tipBD == 1)
                                rez = "DAY(" + cmp + ")";
                            else
                                rez = "TO_NUMBER(TO_CHAR(" + cmp + ",'DD'))";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "TruncateTime");
            }

            return rez;
        }

        public static string CurrentDate(bool faraTimp = false)
        {
            string rez = "";

            try
            {
                if (Constante.tipBD == 1)
                {
                    rez = "GetDate()";
                    if (faraTimp)
                        rez = "CONVERT(DATE,GetDate())";
                }
                else
                {
                    rez = "SYSDATE";
                    if (faraTimp)
                        rez = "TRUNC(SYSDATE)";
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "CurrentDate");
            }

            return rez;
        }

        public static bool IsDate(object strData)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(strData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Boolean IsNumeric(object valoare)
        {
            try
            {
                if (valoare != null)
                {
                    double myNum = 0;
                    if (Double.TryParse(Convert.ToString(valoare), out myNum))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int SituatieZLOperatii(int F10003, DateTime zi, int tipOperatie, int nrZile)
        {
            //1  -  situatie ZL disponibile
            //2  -  adaugare
            //3  -  stergere


            //Este numai pt clientul Groupama
            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) != 34) return 0;

            //se modifica pt a lua ultimele 60 zile in urma de la data inceput cerere
            int zlDisp = 0;
            string strSql = "";
            string lst = "'1R','2R','3R','4R','5R','6R','7R','8R','9R','10R','11R','12R','13R','14R'";
            DateTime zi_1 = zi.AddMonths(-1);

            try
            {
                int norma = Convert.ToInt32(General.ExecutaScalar("SELECT F10043 FROM F100 WHERE F10003=" + F10003, null) ?? 0);
                string cmp = "*";

                if (tipOperatie == 1) cmp = "COALESCE(SUM(COALESCE(to_number(REPLACE(\"ValStr\",'R','')),0) - COALESCE(F30,0)),0) AS ziDisp";

                if (Constante.tipBD == 1)
                {
                    strSql = @"SELECT {4} FROM Ptj_Intrari WHERE F10003 = {0} AND ValStr IN ({1}) AND {2} <= Ziua AND Ziua <= {3}";
                }
                else
                {
                    strSql = @"SELECT {4} FROM ""Ptj_Intrari"" WHERE F10003 = {0} AND UPPER(LTRIM(RTRIM(""ValStr""))) IN ({1}) AND {2} <= ""Ziua"" AND ""Ziua"" <= {3}";
                }
                strSql = string.Format(strSql, F10003, lst, General.ToDataUniv(zi.AddDays(-60)), General.ToDataUniv(zi), cmp);

                DataTable dt = General.IncarcaDT(strSql, null);

                switch (tipOperatie)
                {
                    case 1:
                        if (dt.Rows.Count > 0) zlDisp = (Convert.ToInt32(dt.Rows[0][0] ?? 0) * 2) / norma;
                        break;
                    case 2:
                        int nrRec = (norma * nrZile) / 2;
                        if (nrRec != 0)
                        {
                            string strUp = "";

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                int dif = 0;
                                if (nrRec <= 0) break;

                                int val = Convert.ToInt32((dt.Rows[i]["ValStr"] ?? "").ToString().Replace("R", "")) - Convert.ToInt32(dt.Rows[i]["F30"] ?? 0);
                                if (nrRec >= val)
                                    dif = val;
                                else
                                    dif = nrRec;

                                if (dif != 0)
                                {
                                    strUp = @"UPDATE ""Ptj_Intrari"" SET F30=COALESCE(F30,0) + @1 WHERE F10003=@2 AND ""Ziua"" =@3";
                                    General.ExecutaNonQuery(strUp, new object[] { dif, dt.Rows[i]["F10003"], General.ToDataUniv(Convert.ToDateTime(dt.Rows[i]["Ziua"])) });
                                }

                                nrRec -= val;
                            }
                        }
                        break;
                    case 3:
                        int nrDel = (norma * nrZile) / 2;
                        if (nrDel != 0)
                        {
                            string strUp = "";

                            for (int i = 0; i < dt.Rows.Count; i++)     //order by descending
                            {
                                int? dif = 0;
                                if (nrDel <= 0) break;

                                int val = Convert.ToInt32(dt.Rows[i]["F30"] ?? 0);
                                if (nrDel >= val)
                                    dif = null;
                                else
                                    dif = val - nrDel;

                                if (dif != 0)
                                {
                                    strUp = @"UPDATE ""Ptj_Intrari"" SET F30=COALESCE(F30,0) + @1 WHERE F10003=@2 AND ""Ziua"" =@3";
                                    General.ExecutaNonQuery(strUp, new object[] { dif, dt.Rows[i]["F10003"], General.ToDataUniv(Convert.ToDateTime(dt.Rows[i]["Ziua"])) });
                                }

                                nrDel -= val;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "SituatieZLOperatii");
            }

            return zlDisp;
        }

        public static void TrimiteInPontaj(int idUser, int id, int idCuloare, int trimiteLa, decimal nrOre)
        {
            try
            {
                int nrMin = Convert.ToInt32(nrOre * 60);
                string tipData = "nvarchar(10)";
                if (Constante.tipBD == 2) tipData = "varchar2(10)";
                DataTable dt = General.IncarcaDT($@"SELECT A.*, CASE WHEN (B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL) THEN 1 ELSE 0 END AS ""EsteCuBifa"",
                                                    (CASE WHEN (CASE WHEN (B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL) THEN (CASE WHEN 1=@2 THEN C.""IdTipOre"" ELSE D.""IdTipOre"" END) ELSE B.""IdTipOre"" END)=0 THEN CAST(@3 AS {tipData}) ELSE '' END) {Dami.Operator()}  
                                                    (CASE WHEN (B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL) THEN (CASE WHEN 1=@2 THEN C.""DenumireScurta"" ELSE D.""DenumireScurta"" END) ELSE B.""DenumireScurta"" END) AS ""ValStr"",
                                                    E.F10002, E.F10004, E.F10005, E.F10006, E.F10007, E.F10043,
                                                    (CASE WHEN (CASE WHEN (B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL) THEN (CASE WHEN 1=@2 THEN C.""IdTipOre"" ELSE D.""IdTipOre"" END) ELSE B.""IdTipOre"" END)=0 THEN (CASE WHEN (B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL) THEN (CASE WHEN 1=@2 THEN C.""OreInVal"" ELSE D.""OreInVal"" END) ELSE B.""OreInVal"" END) ELSE '' END) AS ValPentruOre
                                                    FROM ""Ptj_Cereri"" A
                                                    INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                                    LEFT JOIN  ""Ptj_tblAbsente"" C ON B.""CompensareBanca"" = C.""Id""
                                                    LEFT JOIN  ""Ptj_tblAbsente"" D ON B.""CompensarePlata"" = D.""Id""
                                                    INNER JOIN F100 E ON A.F10003=E.F10003
                                                    WHERE A.""Id"" = @1 AND A.""IdStare"" = 3", new object[] { id, trimiteLa, nrOre.ToString().Replace(",",".") });

                if (dt.Rows.Count > 0)
                {
                    string strSql = "";
                    DataRow dr = dt.Rows[0];
                    DataTable dtAbs = General.IncarcaDT(SelectAbsentaInCereri(Convert.ToInt32(dr["F10003"]), Convert.ToDateTime(dr["DataInceput"]).Date, Convert.ToDateTime(dr["DataSfarsit"]).Date, 3, Convert.ToInt32(dr["IdAbsenta"])), null);

                    //construim sql-ul prin care adaugam cererea in pontaj
                    for (int i = 0; i < dtAbs.Rows.Count; i++)
                    {
                        DateTime zi = Convert.ToDateTime(dtAbs.Rows[i]["Zi"]);
                        int ziLib = Convert.ToInt32(dtAbs.Rows[i]["ZiLibera"]);
                        int ziLibLeg = Convert.ToInt32(dtAbs.Rows[i]["ZiLiberaLegala"]);
                        string valStr = (dr["ValStr"] ?? "").ToString();

                        if (Convert.ToInt32(General.Nz(dtAbs.Rows[i]["AreDrepturi"], 0)) == 1)
                        {
                            string sqlIns = "INSERT INTO \"Ptj_Intrari\"(F10003, \"Ziua\", \"ZiSapt\", \"ZiLibera\", \"ZiLiberaLegala\", \"IdContract\", \"Norma\", F10002, F10004, F10005, F10006, F10007, F06204, \"ValStr\", USER_NO, TIME" + ((dr["ValPentruOre"] ?? "").ToString() == "" ? "" : "," + (dr["ValPentruOre"] ?? "").ToString()) + ") \n" +
                                "SELECT " +
                                dr["F10003"] + ", " +
                                General.ToDataUniv(zi.Date) + ", " +
                                Dami.ZiSapt(zi.DayOfWeek.ToString()) + ", " +
                                ziLib + ", " +
                                ziLibLeg + ", " +
                                "(SELECT X.\"IdContract\" FROM \"F100Contracte\" X WHERE X.F10003 = " + dr["F10003"] + " AND X.\"DataInceput\" <= " + General.ToDataUniv(zi.Date) + " AND " + General.ToDataUniv(zi.Date) + " <= X.\"DataSfarsit\"), " +
                                dr["F10043"] + ", " +
                                dr["F10002"] + ", " +
                                dr["F10004"] + ", " +
                                dr["F10005"] + ", " +
                                dr["F10006"] + ", " +
                                dr["F10007"] + ", " +
                                "-1, " +
                                "'" + valStr + "', " +
                                idUser + ", " +
                                General.CurrentDate() +
                                ((dr["ValPentruOre"] ?? "").ToString() == "" ? "" : ", " + nrMin.ToString())
                                + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                            //trimte orele in Val indicat in tabela Ptj_tblAbsente - numai pt cererile de tip ore
                            string sqlUp = "";
                            string sqlIst = "";
                            string sqlValStr = "";
                            if (General.Nz(dr["ValPentruOre"], "").ToString() == "")
                            {
                                sqlUp = "UPDATE \"Ptj_Intrari\" SET \"ValStr\"='" + valStr + "', \"Val0\"=null, \"Val1\"=null, \"Val2\"=null, \"Val3\"=null, \"Val4\"=null, \"Val5\"=null, \"Val6\"=null, \"Val7\"=null, \"Val8\"=null, \"Val9\"=null, \"Val10\"=null, \"Val11\"=null, \"Val12\"=null, \"Val13\"=null, \"Val14\"=null, \"Val15\"=null, \"Val16\"=null, \"Val17\"=null, \"Val18\"=null, \"Val19\"=null, \"Val20\"=null" +
                                        " WHERE F10003 = " + dr["F10003"] + " AND \"Ziua\" = " + General.ToDataUniv(zi.Date);

                                sqlIst = $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", USER_NO, TIME, ""Observatii"") 
                                                VALUES({dr["F10003"]}, {ToDataUniv(zi.Date)}, '{valStr}', '{General.Nz(dtAbs.Rows[i]["ValStr"], "")}', {idUser}, {General.CurrentDate()}, {idUser}, {General.CurrentDate()}, 'Din Cereri')";
                            }
                            else
                            {
                                valStr = CalculValStr((int)dr["F10003"], zi.Date, "", (dr["ValPentruOre"] ?? "").ToString(), nrMin);
                                sqlUp = $@"UPDATE ""Ptj_Intrari"" SET {dr["ValPentruOre"]} = COALESCE({dr["ValPentruOre"]},0) + {nrMin} WHERE F10003= {dr["F10003"]} AND ""Ziua""= {General.ToDataUniv(zi.Date)}";
                                sqlValStr = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr"" = {valStr}  WHERE F10003= {dr["F10003"]} AND ""Ziua""= {General.ToDataUniv(zi.Date)}";
                                sqlIst = $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", USER_NO, TIME, ""Observatii"") 
                                            SELECT {dr["F10003"]}, {ToDataUniv(zi.Date)}, {valStr}, '{General.Nz(dtAbs.Rows[i]["ValStr"], "")}', {idUser}, {General.CurrentDate()}, {idUser}, {General.CurrentDate()}, 'Din Cereri' FROM ""Ptj_Intrari"" WHERE F10003={dr["F10003"]} AND ""Ziua""={General.ToDataUniv(zi.Date)}";

                            }

                            if (Constante.tipBD == 1)
                                strSql += "IF((SELECT COUNT(*) FROM \"Ptj_Intrari\" WHERE F10003 = " + dr["F10003"] + " AND \"Ziua\" = " + General.ToDataUniv(zi.Date) + ") = 0) \n"
                                        + sqlIns + "\n" +
                                        "ELSE \n" +
                                        sqlUp + "; \n" +
                                        sqlValStr + "; \n" +
                                        sqlIst + "; \n";
                            else
                                strSql += $@"
                                    BEGIN
                                        {sqlIst};
                                        BEGIN
                                            {sqlUp};

                                            IF (SQL%ROWCOUNT = 0) THEN 
                                              {sqlIns};
                                            END IF;
                                        END;
                                    END;";
                        }
                    }

                    if (strSql != "")
                        ExecutaNonQuery(strSql, null);
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "TrimiteInPontaj");
            }
        }


        //Florin 2019.12.23
        public static string CalculValStr(int f10003, DateTime ziua, string idAbsente, string valCamp, int valMinute)
        {
            string strCmp = "";

            try
            {
                //Florin 2020.03.20
                ////daca absenta este de tip zi (de exemplu este CO) nu mai trebuie calculat ValStr
                //int cnt = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""Ptj_tblAbsente"" WHERE ""IdTipOre""=1 AND ""Id"" IN (@1)", new object[] { idAbsente }) ?? 0);
                //if (cnt > 0) return strCmp;

                strCmp = Dami.ValoareParam("SintaxaValStr", "1");
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "CalculValStr");
            }

            return strCmp;
        }

        public static DataTable GetRoluriUser(int idUser)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = General.IncarcaDT(@"SELECT CAST(A.""Id"" AS INT) AS ""Id"", A.""Denumire"", A.""PoateInitializa"", A.""PoateSterge""
                                        FROM ""Ptj_tblRoluri"" A
                                        INNER JOIN ""Ptj_relGrupSuper"" B ON A.""Id"" = B.""IdRol""
                                        WHERE B.""IdSuper"" = @1
                                        UNION
                                        SELECT CAST(A.""Id"" AS INT) AS ""Id"", A.""Denumire"", A.""PoateInitializa"", A.""PoateSterge""
                                        FROM ""Ptj_tblRoluri"" A
                                        INNER JOIN ""Ptj_relGrupSuper"" B ON A.""Id"" = B.""IdRol""
                                        INNER JOIN ""relGrupAngajat"" C ON B.""IdGrup"" = C.""IdGrup""
                                        INNER JOIN ""F100Supervizori"" D ON C.F10003 = D.F10003 AND(B.""IdSuper"" * -1) = D.""IdSuper""
                                        WHERE D.""IdUser"" = @1", new object[] { idUser });
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "GetRoluriUser");
            }

            return dt;
        }

        //Florin 2020.01.21
        //public static string SelectAbsente(string f10003, DateTime data, int idAbs = -99)
        //{
        //    string strSql = "";

        //    try
        //    {
        //        if (string.IsNullOrEmpty(f10003)) return strSql;

        //        //Radu 27.02.2020 - se va lua data inceput concediu
        //        //string dt = "GetDate()";
        //        string dt = "";
        //        string idAuto = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) ";
        //        string filtru = "";

        //        if (Constante.tipBD == 2)
        //        {
        //            idAuto = "ROWNUM";
        //            //dt = "sysdate";
        //            dt = "TO_DATE('" + data.Day + "/" + data.Month + "/" + data.Year + "', 'dd/mm/yyyy')";
        //        }
        //        else
        //        {
        //            dt = "CONVERT(DATETIME, '" + data.Day + "/" + data.Month + "/" + data.Year + "', 103)";
        //        }

        //        if (idAbs != -99) filtru = @" WHERE Y.""Id""=" + idAbs;

        //        if (HttpContext.Current.Session["User_Marca"].ToString() != f10003)
        //        {
        //            strSql = @"SELECT {2} AS ""IdAuto"", Y.* FROM (
        //                    SELECT MIN(""IdCircuit"") AS ""IdCircuit"", X.""Id"", X.""Denumire"", X.""DenumireScurta"", X.""DenumireIstoricExtins"", X.""Prezenta"", X.""Culoare"", X.""IdGrupAbsenta"", X.""IdTipOre"", X.""Compensare"", X.""Explicatii"", X.""Planificare"" AS ""EstePlanificare"", X.""NrMax"", X.""NrMaxAn"", X.""WizSal_CodTranzac"", X.""WizSal_DataPlatii"", X.""WizSal_Cantitate"", X.""WizSal_Procent"", X.""WizSal_Suma1"", X.""WizSal_Suma2"", X.""WizSal_Vechime"", X.""LunaCalculInF300"", X.""Anulare"", X.""AnulareAltii"", X.""ArataInlocuitor"", X.""ArataAtasament"", X.""CompensareBanca"", X.""CompensarePlata"", X.""AdunaZileLibere"", X.""GrupOre"", X.""OreInVal"", X.""GrupOreDeVerificat"", X.""AbsenteCFPInCalculCO"", X.""AngajatulPoateAproba"", X.""VerificaCereriInlocuitor"", X.""NuTrimiteInPontaj"", X.""VerificareNrMaxOre"", X.""AbsentaTipOraFolosesteInterval"", X.""AbsentaTipOraPerioada""
        //                    FROM (
        //                    SELECT C.""IdAuto"" AS ""IdCircuit"",A.*
        //                    FROM ""Ptj_tblAbsente"" A
        //                    INNER JOIN ""Ptj_Circuit"" C ON a.""IdGrupAbsenta"" = c.""IdGrupAbsente""
        //                    INNER JOIN ""F100Supervizori"" B ON b.""IdSuper"" = -1 * c.""UserIntrod""
        //                    INNER JOIN ""relGrupAngajat"" D ON c.""IdGrupAngajat"" = d.""IdGrup"" AND b.F10003=d.F10003
        //                    INNER JOIN ""Ptj_relAngajatAbsenta"" E ON a.""Id"" = e.""IdAbsenta"" AND c.""IdGrupAngajat"" = e.""IdGrup""
        //                    INNER JOIN ""F100Contracte"" H ON H.F10003={0} AND CAST(H.""DataInceput"" as date) <= CAST({3} as date) AND CAST({3} as date) <= CAST(H.""DataSfarsit"" as date)
        //                    INNER JOIN ""Ptj_ContracteAbsente"" J ON J.""IdContract""=H.""IdContract"" AND J.""IdAbsenta""=A.""Id""
        //                    WHERE B.F10003 = {0} AND B.""IdUser"" = {1} 
        //                    UNION
        //                    SELECT C.""IdAuto"" AS ""IdCircuit"",A.*
        //                    FROM ""Ptj_tblAbsente"" A
        //                    INNER JOIN ""Ptj_Circuit"" C ON a.""IdGrupAbsenta"" = c.""IdGrupAbsente""
        //                    INNER JOIN ""relGrupAngajat"" D ON c.""IdGrupAngajat"" = d.""IdGrup""
        //                    INNER JOIN ""Ptj_relAngajatAbsenta"" E ON a.""Id"" = e.""IdAbsenta"" AND c.""IdGrupAngajat"" = e.""IdGrup""
        //                    INNER JOIN ""F100Contracte"" H ON H.F10003={0} AND CAST(H.""DataInceput"" as date) <= CAST({3} as date) AND CAST({3} as date) <= CAST(H.""DataSfarsit"" as date)
        //                    INNER JOIN ""Ptj_ContracteAbsente"" J ON J.""IdContract""=H.""IdContract"" AND J.""IdAbsenta""=A.""Id""
        //                    WHERE D.F10003 = {0} AND C.""UserIntrod"" = {1} 
        //                    ) X 
        //                    GROUP BY X.""Id"", X.""Denumire"", X.""DenumireScurta"", X.""DenumireIstoricExtins"", X.""Prezenta"", X.""Culoare"", X.""IdGrupAbsenta"", X.""IdTipOre"", X.""Compensare"", X.""Explicatii"", X.""Planificare"", X.""NrMax"", X.""NrMaxAn"", X.""WizSal_CodTranzac"", X.""WizSal_DataPlatii"", X.""WizSal_Cantitate"", X.""WizSal_Procent"", X.""WizSal_Suma1"", X.""WizSal_Suma2"", X.""WizSal_Vechime"", X.""LunaCalculInF300"", X.""Anulare"", X.""AnulareAltii"", X.""ArataInlocuitor"", X.""ArataAtasament"", X.""CompensareBanca"", X.""CompensarePlata"", X.""AdunaZileLibere"", X.""GrupOre"", X.""OreInVal"", X.""GrupOreDeVerificat"", X.""AbsenteCFPInCalculCO"", X.""AngajatulPoateAproba"", X.""VerificaCereriInlocuitor"", X.""NuTrimiteInPontaj"", X.""VerificareNrMaxOre"", X.""AbsentaTipOraFolosesteInterval"", X.""AbsentaTipOraPerioada""
        //                    ) Y {4} ORDER BY Y.""Denumire""";
        //        }
        //        else
        //        {
        //            strSql = @"SELECT {2} AS ""IdAuto"", Y.* FROM (
        //                    SELECT MIN(""IdCircuit"") AS ""IdCircuit"", X.""Id"", X.""Denumire"", X.""DenumireScurta"", X.""DenumireIstoricExtins"", X.""Prezenta"", X.""Culoare"", X.""IdGrupAbsenta"", X.""IdTipOre"", X.""Compensare"", X.""Explicatii"", X.""Planificare"" AS ""EstePlanificare"", X.""NrMax"", X.""NrMaxAn"", X.""WizSal_CodTranzac"", X.""WizSal_DataPlatii"", X.""WizSal_Cantitate"", X.""WizSal_Procent"", X.""WizSal_Suma1"", X.""WizSal_Suma2"", X.""WizSal_Vechime"", X.""LunaCalculInF300"", X.""Anulare"", X.""AnulareAltii"", X.""ArataInlocuitor"", X.""ArataAtasament"", X.""CompensareBanca"", X.""CompensarePlata"", X.""AdunaZileLibere"", X.""GrupOre"", X.""OreInVal"", X.""GrupOreDeVerificat"", X.""AbsenteCFPInCalculCO"", X.""AngajatulPoateAproba"", X.""VerificaCereriInlocuitor"", X.""NuTrimiteInPontaj"", X.""VerificareNrMaxOre"", X.""AbsentaTipOraFolosesteInterval"", X.""AbsentaTipOraPerioada""
        //                    FROM (
        //                    SELECT C.""IdAuto"" AS ""IdCircuit"",A.*
        //                    FROM ""Ptj_tblAbsente"" A
        //                    INNER JOIN ""Ptj_Circuit"" C ON a.""IdGrupAbsenta"" = c.""IdGrupAbsente""
        //                    INNER JOIN ""relGrupAngajat"" D ON c.""IdGrupAngajat"" = d.""IdGrup""
        //                    INNER JOIN ""Ptj_relAngajatAbsenta"" E ON a.""Id"" = e.""IdAbsenta"" AND c.""IdGrupAngajat"" = e.""IdGrup""
        //                    INNER JOIN ""F100Contracte"" H ON H.F10003={0} AND CAST(H.""DataInceput"" as date) <= CAST({3} as date) AND CAST({3} as date) <= CAST(H.""DataSfarsit"" as date)
        //                    INNER JOIN ""Ptj_ContracteAbsente"" J ON J.""IdContract""=H.""IdContract"" AND J.""IdAbsenta""=A.""Id""
        //                    WHERE D.F10003 = {0} AND C.""UserIntrod"" = 0 
        //                    UNION
        //                    SELECT C.""IdAuto"" AS ""IdCircuit"",A.*
        //                    FROM ""Ptj_tblAbsente"" A
        //                    INNER JOIN ""Ptj_Circuit"" C ON a.""IdGrupAbsenta"" = c.""IdGrupAbsente""
        //                    INNER JOIN ""relGrupAngajat"" D ON c.""IdGrupAngajat"" = d.""IdGrup""
        //                    INNER JOIN ""Ptj_relAngajatAbsenta"" E ON a.""Id"" = e.""IdAbsenta"" AND c.""IdGrupAngajat"" = e.""IdGrup""
        //                    INNER JOIN ""F100Contracte"" H ON H.F10003={0} AND CAST(H.""DataInceput"" as date) <= CAST({3} as date) AND CAST({3} as date) <= CAST(H.""DataSfarsit"" as date)
        //                    INNER JOIN ""Ptj_ContracteAbsente"" J ON J.""IdContract""=H.""IdContract"" AND J.""IdAbsenta""=A.""Id""
        //                    WHERE D.F10003 = {0} AND C.""UserIntrod"" = {1} 
        //                    ) X 
        //                    GROUP BY X.""Id"", X.""Denumire"", X.""DenumireScurta"", X.""DenumireIstoricExtins"", X.""Prezenta"", X.""Culoare"", X.""IdGrupAbsenta"", X.""IdTipOre"", X.""Compensare"", X.""Explicatii"", X.""Planificare"", X.""NrMax"", X.""NrMaxAn"", X.""WizSal_CodTranzac"", X.""WizSal_DataPlatii"", X.""WizSal_Cantitate"", X.""WizSal_Procent"", X.""WizSal_Suma1"", X.""WizSal_Suma2"", X.""WizSal_Vechime"", X.""LunaCalculInF300"", X.""Anulare"", X.""AnulareAltii"", X.""ArataInlocuitor"", X.""ArataAtasament"", X.""CompensareBanca"", X.""CompensarePlata"", X.""AdunaZileLibere"", X.""GrupOre"", X.""OreInVal"", X.""GrupOreDeVerificat"", X.""AbsenteCFPInCalculCO"", X.""AngajatulPoateAproba"", X.""VerificaCereriInlocuitor"", X.""NuTrimiteInPontaj"", X.""VerificareNrMaxOre"", X.""AbsentaTipOraFolosesteInterval"", X.""AbsentaTipOraPerioada""
        //                    ) Y {4} ORDER BY Y.""Denumire""";
        //        }

        //        strSql = string.Format(strSql, f10003, HttpContext.Current.Session["UserId"], idAuto, dt, filtru);
        //    }
        //    catch (Exception ex)
        //    {
        //        MemoreazaEroarea(ex.ToString(), "General", "SelectAbsente");
        //    }

        //    return strSql;
        //}


        //Florin 2020.05.25 - refacuta GitHub #444
        public static string SelectAbsente(string f10003, DateTime data, int idAbs = -99, int idRol = -99)
        {
            string strSql = "";

            try
            {
                if (string.IsNullOrEmpty(f10003)) return strSql;

                //Radu 27.02.2020 - se va lua data inceput concediu
                string idAuto = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) ";
                string filtru = "";
                if (idAbs != -99) filtru = @" WHERE Y.""Id""=" + idAbs;
                if (Constante.tipBD == 2)
                    idAuto = "ROWNUM";

                string filtruSuper = "";
                if (idRol != -99)
                    filtruSuper = $@" AND B.""IdSuper""={idRol}";

                strSql = $@"SELECT {idAuto} AS ""IdAuto"", Y.* FROM (
                        SELECT MIN(""IdCircuit"") AS ""IdCircuit"", X.""Id"", X.""Denumire"", X.""DenumireScurta"", X.""DenumireIstoricExtins"", X.""Prezenta"", X.""Culoare"", X.""IdGrupAbsenta"", X.""IdTipOre"", X.""Compensare"", X.""Explicatii"", X.""Planificare"" AS ""EstePlanificare"", X.""NrMax"", X.""NrMaxAn"", X.""WizSal_CodTranzac"", X.""WizSal_DataPlatii"", X.""WizSal_Cantitate"", X.""WizSal_Procent"", X.""WizSal_Suma1"", X.""WizSal_Suma2"", X.""WizSal_Vechime"", X.""LunaCalculInF300"", X.""Anulare"", X.""AnulareAltii"", X.""ArataInlocuitor"", X.""ArataAtasament"", X.""CompensareBanca"", X.""CompensarePlata"", X.""AdunaZileLibere"", X.""GrupOre"", X.""OreInVal"", X.""GrupOreDeVerificat"", X.""AbsenteCFPInCalculCO"", X.""AngajatulPoateAproba"", X.""VerificaCereriInlocuitor"", X.""NuTrimiteInPontaj"", X.""VerificareNrMaxOre"", X.""AbsentaTipOraFolosesteInterval"", X.""AbsentaTipOraPerioada""
                        FROM (
                        SELECT C.""IdAuto"" AS ""IdCircuit"",A.*
                        FROM ""Ptj_tblAbsente"" A
                        INNER JOIN ""Ptj_Circuit"" C ON a.""IdGrupAbsenta"" = c.""IdGrupAbsente""
                        INNER JOIN ""F100Supervizori"" B ON b.""IdSuper"" = -1 * c.""UserIntrod"" AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit"" {filtruSuper}
                        INNER JOIN ""relGrupAngajat"" D ON c.""IdGrupAngajat"" = d.""IdGrup"" AND b.F10003=d.F10003
                        INNER JOIN ""Ptj_relAngajatAbsenta"" E ON a.""Id"" = e.""IdAbsenta"" AND c.""IdGrupAngajat"" = e.""IdGrup""
                        INNER JOIN ""F100Contracte"" H ON H.F10003={f10003} AND CAST(H.""DataInceput"" as date) <= {General.ToDataUniv(data)} AND {General.ToDataUniv(data)} <= CAST(H.""DataSfarsit"" as date)
                        INNER JOIN ""Ptj_ContracteAbsente"" J ON J.""IdContract""=H.""IdContract"" AND J.""IdAbsenta""=A.""Id""
                        WHERE B.F10003 = {f10003} AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]} 
                        UNION
                        SELECT C.""IdAuto"" AS ""IdCircuit"",A.*
                        FROM ""Ptj_tblAbsente"" A
                        INNER JOIN ""Ptj_Circuit"" C ON a.""IdGrupAbsenta"" = c.""IdGrupAbsente""
                        INNER JOIN ""relGrupAngajat"" D ON c.""IdGrupAngajat"" = d.""IdGrup""
                        INNER JOIN ""Ptj_relAngajatAbsenta"" E ON a.""Id"" = e.""IdAbsenta"" AND c.""IdGrupAngajat"" = e.""IdGrup""
                        INNER JOIN ""F100Contracte"" H ON H.F10003={f10003} AND CAST(H.""DataInceput"" as date) <= {General.ToDataUniv(data)} AND {General.ToDataUniv(data)} <= CAST(H.""DataSfarsit"" as date)
                        INNER JOIN ""Ptj_ContracteAbsente"" J ON J.""IdContract""=H.""IdContract"" AND J.""IdAbsenta""=A.""Id""
                        WHERE D.F10003 = {f10003} AND C.""UserIntrod"" = {HttpContext.Current.Session["UserId"]} 
                        ) X 
                        GROUP BY X.""Id"", X.""Denumire"", X.""DenumireScurta"", X.""DenumireIstoricExtins"", X.""Prezenta"", X.""Culoare"", X.""IdGrupAbsenta"", X.""IdTipOre"", X.""Compensare"", X.""Explicatii"", X.""Planificare"", X.""NrMax"", X.""NrMaxAn"", X.""WizSal_CodTranzac"", X.""WizSal_DataPlatii"", X.""WizSal_Cantitate"", X.""WizSal_Procent"", X.""WizSal_Suma1"", X.""WizSal_Suma2"", X.""WizSal_Vechime"", X.""LunaCalculInF300"", X.""Anulare"", X.""AnulareAltii"", X.""ArataInlocuitor"", X.""ArataAtasament"", X.""CompensareBanca"", X.""CompensarePlata"", X.""AdunaZileLibere"", X.""GrupOre"", X.""OreInVal"", X.""GrupOreDeVerificat"", X.""AbsenteCFPInCalculCO"", X.""AngajatulPoateAproba"", X.""VerificaCereriInlocuitor"", X.""NuTrimiteInPontaj"", X.""VerificareNrMaxOre"", X.""AbsentaTipOraFolosesteInterval"", X.""AbsentaTipOraPerioada""
                        ) Y {filtru} ORDER BY Y.""Denumire""";
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "SelectAbsente");
            }

            return strSql;
        }


        public static void SelectCereriIstoric(int f10003, int idInloc, int idCircuit, int estePlanificare, out string sqlSelect, out int trimiteLaInlocuitor, int idCerere = -99, DateTime? dtInc = null)
        {
            string sqlIst = "";
            int trimite = 0;

            try
            {
                string op = "+";
                string tipData = "nvarchar";

                if (Constante.tipBD == 2)
                {
                    op = "||";
                    tipData = "varchar2";
                }

                string filtru = "";
                if (dtInc != null)
                    filtru = $@" AND ""DataInceput"" <= {General.ToDataUniv(dtInc)} AND {General.ToDataUniv(dtInc)} <= ""DataSfarsit"" ";

                //exceptand primul element, selectul de mai jos intoarce un string cu toti actorii de pe circuit
                string sqlCir = $@"SELECT CAST(COALESCE(""TrimiteLaInlocuitor"",0) AS {tipData}(10)) {op} ';' {op} COALESCE(CAST(""UserIntrod"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super1"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super2"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super3"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super4"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super5"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super6"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super7"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super8"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super9"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super10"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super11"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super12"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super13"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super14"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super15"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super16"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super17"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super18"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super19"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super20"" AS {tipData}(10)) {op} ';','') FROM ""Ptj_Circuit"" WHERE ""IdAuto""=" + idCircuit;
                string ids = (General.ExecutaScalar(sqlCir, null) ?? "").ToString();

                if (ids != "")
                {
                    string[] lstId = ids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    trimite = Convert.ToInt32(lstId[0]);
                    int idx = 0;
                    List<string> lstSql = new List<string>();
                    string strSql = "";

                    //cand CumulareAcelasiSupervizor este:
                    // 0 - (se rezolva punand particula ALL in UNION) se pun toti supervizorii chiar daca se repeta; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  3;   8;   3;   9;
                    // 1 - (se trateaza separat, dupa bucla for) daca uramtorul in circuit este acelasi user, se salveaza doar o singura data; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;   3;   9;
                    // 2 - (se rezolva scotand particula ALL din UNION) user-ul se salveaza doar o singura data indiferent de cate ori este pe circuit sau pe ce pozitie este;  ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;  9;
                    string paramCumul = Dami.ValoareParam("CumulareAcelasiSupervizor", "0");
                    string strUnion = "ALL";

                    for (int i = 1; i < lstId.Count(); i++)
                    {
                        string strTmp = "";

                        //daca Supervizorul este angajatul
                        if (lstId[i].ToString() == "0") strTmp += " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", F70102 AS \"IdUser\", " + lstId[i].ToString() + " AS \"IdSuper\", 0 AS \"Inlocuitor\" FROM USERS WHERE F10003=" + f10003;

                        //daca Supervizorul este id de utilizator
                        if (Convert.ToInt32(lstId[i].ToString()) > 0) strTmp += " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", " + lstId[i].ToString() + " AS \"IdUser\", -76 AS \"IdSuper\", 0 AS \"Inlocuitor\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                        //daca Supervizorul este din nommenclatorul tblSupervizori (este cu minus)
                        //se foloseste union pt a acoperi si cazul in care user-ul logat este deja un superviozr pt acest angajat;
                        if (Convert.ToInt32(lstId[i].ToString()) < 0)
                        {
                            strTmp = @" UNION {4} SELECT TOP 1 {3} AS ""Index"", IdUser, {1} AS IdSuper, 0 AS ""Inlocuitor"" FROM (
                                        SELECT TOP 1 IdUser FROM F100Supervizori WHERE F10003 = {0} AND IdSuper = (-1 * {1}) AND IdUser = {2} {5} 
                                        UNION ALL
                                        SELECT TOP 1 IdUser FROM F100Supervizori WHERE F10003 = {0} AND IdSuper = (-1 * {1}) {5} 
                                        ) x ";
                            if (Constante.tipBD == 2)
                            {
                                strTmp = @" UNION {4} SELECT {3} AS ""Index"", ""IdUser"", {1} AS ""IdSuper"", 0 AS ""Inlocuitor"" FROM (
                                    SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ""IdUser"" = {2} AND ROWNUM<=1 {5} 
                                    UNION ALL
                                    SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ROWNUM<=1 {5} 
                                    ) x  WHERE ROWNUM<=1";
                            }

                            strTmp = string.Format(strTmp, f10003, Convert.ToInt32(lstId[i]), HttpContext.Current.Session["UserId"], idx, strUnion, filtru);
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

                    //Florin - 2020.01.21
                    if (paramCumul == "1" || paramCumul == "2")
                    {
                        int x = 0;
                        int idUserTmp = -99;
                        string sqlNou = "";
                        int[] arr = new int[20];
                        
                        for (int i = 0; i < lstSql.Count; i++)
                        {
                            DataTable dtSql = General.IncarcaDT(lstSql[i].Substring((" UNION " + strUnion).Length), null);
                            if (dtSql.Rows.Count == 0) continue;
                            int idUsrCurent = Convert.ToInt32(General.Nz(dtSql.Rows[0]["IdUser"], 0));

                            if (i == 0 || (paramCumul == "1" && i > 0 && Convert.ToInt32(arr[x - 1]) != idUsrCurent) || (paramCumul == "2" && !arr.Contains(idUsrCurent)))
                            {
                                sqlNou += lstSql[i];
                                arr[x] = idUsrCurent;
                                x += 1;
                            }

                            idUserTmp = idUsrCurent;
                        }

                        if (sqlNou != "") strSql = sqlNou;
                    }

                    //daca este cerere de tip planificata se duce in starea 4 indiferent de pozitie
                    if (strSql != "")
                    {
                        string strPozitie = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                        if (Constante.tipBD == 2) strPozitie = "ROWNUM";

                        sqlIst = @"INSERT INTO ""Ptj_CereriIstoric""(""IdCerere"", ""IdCircuit"", ""IdSuper"", ""IdStare"", ""IdUser"", ""Pozitie"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""Inlocuitor"", ""IdUserInlocuitor"", ""Culoare"")
                                SELECT G.*, 
                                (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = ""IdStare"") AS ""Culoare""
                                FROM (
                                SELECT {7} AS ""IdCerere"", 
                                {0} AS ""IdCircuit"", ""IdSuper"", 
                                CASE WHEN ({6} = 1) AND (({3}) = 1) THEN 4 ELSE 
                                (CASE WHEN ""Total"" = 1 THEN 3 ELSE 
                                (CASE WHEN ""Total"" = ({3}) THEN 3 ELSE
                                (CASE WHEN (({3}) = 1) THEN 1 ELSE 2 END) END) END) END AS ""IdStare"", 
                                ""IdUser"", 
                                {3} AS Pozitie, 
                                CASE WHEN ({6} = 1) THEN (CASE WHEN ({3}) = 1 THEN 1 ELSE NULL END) ELSE (CASE WHEN ""IdUser"" = {1} THEN 1 ELSE NULL END) END AS ""Aprobat"", 
                                CASE WHEN ({6} = 1) THEN (CASE WHEN ({3}) = 1 THEN {5} ELSE NULL END) ELSE (CASE WHEN ""IdUser"" = {1} THEN {5} ELSE NULL END) END AS ""DataAprobare"", 
                                {1} AS USER_NO, {2} AS TIME, 0 AS ""Inlocuitor"", NULL AS ""IdUserInlocuitor"" 
                                FROM (SELECT W.*, COUNT(*) OVER (PARTITION BY 1) AS ""Total"" FROM ({4}) W) X) G";

                        string strCer = idCerere.ToString();
                        if (idCerere == -99) strCer = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"")";

                        sqlIst = string.Format(sqlIst, idCircuit, HttpContext.Current.Session["UserId"], General.CurrentDate(), strPozitie, strSql.Substring((" UNION " + strUnion).Length), General.CurrentDate(), estePlanificare, strCer);
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "SelectCereriIstoric");
            }

            sqlSelect = sqlIst;
            trimiteLaInlocuitor = trimite;
        }

        public static void CalcZile(DateTime? dtInc, DateTime? dtSf, string adunaZL, out int nr, out int nrViitor)
        {
            int tmpNr = 0;
            int tmpNrViitor = 0;
            bool este31 = false;

            nr = tmpNr;
            nrViitor = tmpNrViitor;

            try
            {
                if (dtInc == null || dtSf == null) return;

                DataTable dt = General.IncarcaDT($@"SELECT *, YEAR(DAY) AS AN, MONTH(DAY) AS LUNA, DAY(DAY) AS ZI FROM HOLIDAYS WHERE {General.ToDataUniv(Convert.ToDateTime(dtInc))} <= DAY AND DAY <= {General.ToDataUniv(Convert.ToDateTime(dtSf))} ", null);
                for (DateTime zi = Convert.ToDateTime(dtInc).Date; zi <= Convert.ToDateTime(dtSf); zi = zi.AddDays(1))
                {
                    if (adunaZL == "1")
                    {
                        nr += 1;
                    }
                    else
                    {
                        string ert = zi.ToShortDateString();
                        if (zi.DayOfWeek.ToString().ToLower() != "saturday" && zi.DayOfWeek.ToString().ToLower() != "sunday" && dt.Select("AN=" + zi.Year + " AND LUNA=" + zi.Month + " AND ZI=" + zi.Day).Count() == 0) nr += 1;
                    }

                    if (zi.Day == 31 && zi.Month == 12)
                    {
                        nrViitor = nr;
                        este31 = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "CalcZile");
            }

            tmpNr = nr;
            if (este31)
                tmpNrViitor = nr - nrViitor;
            else
                tmpNrViitor = nrViitor;
        }

        //Florin 2018.08.20 
        public static int CalcZile(int f10003, DateTime? dtInc, DateTime? dtSf, int idRol, int idAbsenta)
        {
            int nrZile = 0;

            try
            {
                string strSql = $@"SELECT SUM(""AreDrepturi"") FROM ({SelectAbsentaInCereri(f10003, dtInc, dtSf, -99, idAbsenta)}) X";
                nrZile = Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, null), 0));
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "CalcZile");
            }

            return nrZile;
        }

        public static string SelectAbsentaInCereri(int f10003, DateTime? dtInc, DateTime? dtSf, int idRol, int idAbsenta)
        {
            string strSql = "";

            try
            {
                //#311 si #344
                strSql = $@"SELECT P.""Zi"", P.""ZiSapt"", CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",
                    CASE WHEN P.""ZiSapt""=6 OR P.""ZiSapt""=7 OR D.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLibera"", 
                    CASE WHEN COALESCE(b.SL,0) <> 0 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) = 1 THEN 1 ELSE 
                    CASE WHEN COALESCE(b.ZL,0) <> 0 AND P.""ZiSapt"" < 6 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) <> 1 THEN 1 ELSE 
                    CASE WHEN COALESCE(b.S,0) <> 0 AND P.""ZiSapt"" = 6 THEN 1 ELSE 
                    CASE WHEN COALESCE(b.D,0) <> 0 AND P.""ZiSapt"" = 7 THEN 1 ELSE 0 
                    END
                    END
                    END
                    END AS ""AreDrepturi"", F.""ValStr""
                    FROM ""tblZile"" P
                    INNER JOIN ""Ptj_tblAbsente"" A ON 1=1
                    INNER JOIN ""Ptj_ContracteAbsente"" B ON A.""Id"" = B.""IdAbsenta""
                    LEFT JOIN HOLIDAYS D on P.""Zi""=D.DAY
                    LEFT JOIN ""Ptj_Intrari"" F ON F.F10003={f10003} AND F.""Ziua""=P.""Zi""
                    WHERE {General.ToDataUniv(dtInc)} <= CAST(P.""Zi"" AS date) AND CAST(P.""Zi"" AS date) <= {General.ToDataUniv(dtSf)} 
                    AND A.""Id"" = {idAbsenta}
                    AND COALESCE(A.""DenumireScurta"", '~') <> '~'
                    AND B.""IdContract"" = (SELECT MAX(""IdContract"") FROM ""F100Contracte"" WHERE F10003 = {f10003} AND ""DataInceput"" <= {General.ToDataUniv(dtInc)} AND {General.ToDataUniv(dtInc)} <= ""DataSfarsit"") ";
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "SelectAbsentaInCereri");
            }

            return strSql;
        }

        public static DataTable GetSituatieZileAbsente(int F10003, int an = 1900)
        {
            DataTable dt = new DataTable();

            try
            {
                if (an != 1900)
                    dt = General.IncarcaDT(@"SELECT A.""An"", A.""RamaseAnterior"" AS ""Ramase An Anterior"", A.""Cuvenite"" AS ""Cuvenite An Curent"", A.""Total"" AS ""Total An Curent"", A.""Aprobate"", A.""Ramase"", A.""Solicitate"", A.""Planificate"", A.""RamaseDePlanificat"" FROM ""SituatieZileAbsente"" A WHERE A.F10003=@1 AND A.""IdAbsenta""=(SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='IdAbsentaCO') AND An=@2", new object[] { F10003, an });
                else
                    dt = General.IncarcaDT(@"SELECT A.""An"", A.""RamaseAnterior"" AS ""Ramase An Anterior"", A.""Cuvenite"" AS ""Cuvenite An Curent"", A.""Total"" AS ""Total An Curent"", A.""Aprobate"", A.""Ramase"", A.""Solicitate"", A.""Planificate"", A.""RamaseDePlanificat"" FROM ""SituatieZileAbsente"" A WHERE A.F10003=@1 AND A.""IdAbsenta""=(SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='IdAbsentaCO')", new object[] { F10003 });
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "GetSituatieZileAbsente");
            }

            return dt;
        }

        public static DataTable GetSituatieZileAbsente(int F10003, int idAbs, int an = 1900)
        {
            DataTable dt = new DataTable();

            try
            {
                if (an != 1900)
                    dt = General.IncarcaDT(@"SELECT A.""An"" AS ""Anul"", A.""RamaseAnterior"" AS ""Ramase An Anterior"", A.""Cuvenite"" ""Cuvenite An Curent"", A.""Total"" AS AS ""Total An Curent"", A.""Aprobate"", A.""Ramase"" AS ""Ramase curent"", A.""Solicitate"", A.""Planificate"", A.""RamaseDePlanificat"" AS ""Ramase De Planificat"" FROM ""SituatieZileAbsente"" A WHERE A.F10003=@1 AND A.""IdAbsenta""=@2 AND An=@3", new object[] { F10003, idAbs, an });
                else
                    dt = General.IncarcaDT(@"SELECT A.""An"" AS ""Anul"", A.""RamaseAnterior"" AS ""Ramase An Anterior"", A.""Cuvenite"" ""Cuvenite An Curent"", A.""Total"" AS AS ""Total An Curent"", A.""Aprobate"", A.""Ramase"" AS ""Ramase curent"", A.""Solicitate"", A.""Planificate"", A.""RamaseDePlanificat"" AS ""Ramase De Planificat"" FROM ""SituatieZileAbsente"" A WHERE A.F10003=@1 AND A.""IdAbsenta""=@2", new object[] { F10003, idAbs });
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "GetSituatieZileAbsente");
            }

            return dt;
        }


        public static byte[] CreazaExcel(string strSelect)
        {
            byte[] ras = null;

            try
            {
                DataTable dt = General.IncarcaDT(strSelect, null);

                DevExpress.Spreadsheet.Workbook book = new DevExpress.Spreadsheet.Workbook();
                DevExpress.Spreadsheet.Worksheet ws2 = book.Worksheets["Sheet1"];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws2.Cells[0, i].Value = dt.Columns[i].ColumnName;
                }

                int row = 2;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string val = General.Nz(dt.Rows[i][j], "").ToString();
                        if (val != "" && val.Substring(0, 1) == "#")
                            ws2.Cells[row, j].FillColor = Culoare(dt.Rows[i][j].ToString());
                        else
                            ws2.Cells[row, j].Value = dt.Rows[i][j].ToString();
                    }
                    row++;
                }

                byte[] byteArray = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xls);
                ras = byteArray;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        public static Color Culoare(string culoare)
        {
            Color color = Color.FromArgb(255, 255, 255);
            ColorConverter cc = new ColorConverter();
            if (culoare.Length <= 0)
                return color;
            else
                return (Color)cc.ConvertFromString(culoare);
        }

        public static Color? CuloarePontaj(int tip)
        {
            Color? col = null;

            try
            {
                if (tip != 0)
                {
                    Color color = Color.FromArgb(255, 255, 255);
                    ColorConverter cc = new ColorConverter();
                    switch (tip)
                    {
                        case 1:
                            col = (Color)cc.ConvertFromString(Constante.CuloareDinCereri);
                            break;
                        case 2:
                            col = (Color)cc.ConvertFromString(Constante.CuloareDinInitializare);
                            break;
                        case 3:
                            col = (Color)cc.ConvertFromString(Constante.CuloareDinCalcul);
                            break;
                        case 4:
                            col = (Color)cc.ConvertFromString(Constante.CuloareModificatManual);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return col;

        }

        public static string MetodeCereri(int tipActiune, List<metaCereriRol> arr, int idUser, int userMarca, string motiv = "", bool esteHR = false)
        {
            string log = "";
            //string tmpLog = "A intrat in metoda - tipActiune: " + tipActiune;

            //actiune  1  - aprobare
            //actiune  2  - respingere

            try
            {
                if (arr.Count == 0) return "Eroare";

                bool HR = false;
                string strSelect = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    strSelect += " UNION SELECT " + arr[i].Id + " AS \"Id\", " + arr[i].Rol + " AS \"Rol\" " + (Constante.tipBD == 2 ? " FROM DUAL " : "");
                }

                string strDrepturi = $@"
                    (SELECT TOP 1 Valoare FROM Ptj_CereriDrepturi DR WHERE DR.IdAbs IN (A.IdAbsenta,-13) AND DR.IdStare IN (A.IdStare, -13) AND DR.IdRol IN (RL.Rol, -13) AND DR.IdActiune IN ({tipActiune}, -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Drepturi_Valoare,
                    (SELECT TOP 1 NrZile  FROM Ptj_CereriDrepturi DR WHERE DR.IdAbs IN (A.IdAbsenta,-13) AND DR.IdStare IN (A.IdStare, -13) AND DR.IdRol IN (RL.Rol, -13) AND DR.IdActiune IN ({tipActiune}, -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Drepturi_NrZile";

                if (Constante.tipBD == 2)
                    strDrepturi = $@"
                        (SELECT ""Valoare"" FROM ""Ptj_CereriDrepturi"" DR WHERE DR.""IdAbs"" IN (A.""IdAbsenta"",-13) AND DR.""IdStare"" IN (A.""IdStare"", -13) AND DR.""IdRol"" IN (RL.""Rol"", -13) AND DR.""IdActiune"" IN ({tipActiune}, -13) AND ROWNUM <=1) AS ""Drepturi_Valoare"",
                        (SELECT ""NrZile""  FROM ""Ptj_CereriDrepturi"" DR WHERE DR.""IdAbs"" IN (A.""IdAbsenta"",-13) AND DR.""IdStare"" IN (A.""IdStare"", -13) AND DR.""IdRol"" IN (RL.""Rol"", -13) AND DR.""IdActiune"" IN ({tipActiune}, -13) AND ROWNUM <=1) AS ""Drepturi_NrZile"" ";


                string strSql = $@"SELECT A.*, G.F10008 {Dami.Operator()} ' ' {Dami.Operator()} G.F10009 AS ""NumeComplet"",
                                CASE WHEN D.""IdCerere"" IS NOT NULL THEN D.""IdCerere"" ELSE B.""IdCerere"" END AS ""IdCerere"",
                                CASE WHEN D.""IdCerere"" IS NOT NULL THEN D.""Pozitie"" ELSE B.""Pozitie"" END AS ""PozitieIstoric"",
                                CASE WHEN D.""IdCerere"" IS NOT NULL THEN D.""Aprobat"" ELSE B.""Aprobat"" END AS ""Aprobat"",
                                CASE WHEN D.""IdCerere"" IS NOT NULL THEN D.""IdUser"" ELSE B.""IdUser"" END AS ""IdUser"",
                                CASE WHEN D.""IdAuto"" IS NOT NULL THEN D.""IdAuto"" ELSE B.""IdAuto"" END AS ""IdIst"",
                                COALESCE(F.""IdStare"",1) AS ""IdStareCumulat"", COALESCE(C.""IdTipOre"",0) AS ""IdTipOre"",
                                COALESCE(C.""Compensare"",0) AS ""Compensare"", 
                                COALESCE(C.""OreInVal"",'') AS ""OreInVal"", COALESCE(""NuTrimiteInPontaj"",0) AS ""NuTrimiteInPontaj"", E.""RespectaOrdinea"", RL.""Rol"", C.""Denumire"" AS ""DenumireAbsenta"", 
                                {strDrepturi}
                                FROM ({strSelect.Substring(6)}) RL
                                INNER JOIN ""Ptj_Cereri"" A ON RL.""Id"" = A.""Id""
                                LEFT JOIN ""Ptj_CereriIstoric"" B ON A.""Id""=B.""IdCerere"" AND B.""IdSuper"" = -1 * RL.""Rol"" AND B.""IdUser""={idUser}
                                LEFT JOIN ""Ptj_tblAbsente"" C ON A.""IdAbsenta""=C.""Id""
                                LEFT JOIN ""Ptj_Circuit"" E ON A.""IdCircuit""=E.""IdAuto""
                                LEFT JOIN ""Ptj_CereriIstoric"" D ON 
                                (D.""Pozitie"" = (A.""Pozitie"" + 1) OR (COALESCE(E.""RespectaOrdinea"",0)=0 AND D.""Pozitie"" > (A.""Pozitie"" + 1))) AND
                                A.""Id""=D.""IdCerere"" AND (D.""IdSuper"" = -1 * RL.""Rol""  OR RL.""Rol"" = 78) AND D.""IdUser"" IN (SELECT Y.F70102 FROM ""Ptj_Cereri"" X
                                                                                                                    INNER JOIN USERS Y ON X.F10003=Y.F10003
                                                                                                                    WHERE X.""Inlocuitor""=(SELECT G.F10003 FROM USERS G WHERE G.F70102={idUser}) AND {TruncateDate("X.DataInceput")} <= {CurrentDate(true)} AND {CurrentDate(true)} <= {TruncateDate("X.DataSfarsit")}
                                                                                                                    UNION
                                                                                                                    SELECT ""IdUser"" FROM ""tblDelegari"" WHERE COALESCE(""IdModul"",-99)=1 AND ""IdDelegat""={idUser} AND {TruncateDate("DataInceput")} <= {CurrentDate(true)} AND {CurrentDate(true)} <= {TruncateDate("DataSfarsit")})
                                LEFT JOIN ""Ptj_Cumulat"" F ON A.F10003=F.F10003 AND F.""An""={General.FunctiiData("A.\"DataInceput\"", "A")} AND F.""Luna""={General.FunctiiData("A.\"DataInceput\"", "L")}
                                LEFT JOIN F100 G ON A.F10003=G.F10003
                                WHERE 1=1 ";

                DataTable dt = General.IncarcaDT(strSql, null);

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    DataRow dr = dt.Rows[j];
                    if (Convert.ToInt32(General.Nz(dr["Rol"], -99)) == 77 || esteHR) HR = true;

                    #region Validare stare cerere
                    //Buacata de cod identica cu cea din Cereri -> btnAprobare si btnRespinge
                    //este nevoie de ea si aici pt cazul cand se aproba/respinge de pe mail

                    switch (Convert.ToInt32(General.Nz(dr["IdStare"], 0)))
                    {
                        case -1:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    #endregion

                    if (!HR)
                    {
                        if (Convert.ToInt32(General.Nz(dr["IdCerere"], -99)) == -99)
                        {
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("user-ul logat nu se regaseste pe circuit") + System.Environment.NewLine;
                            continue;
                        }
                    }

                    if (Convert.ToInt32(General.Nz(dr["Aprobat"], 0)) == 1)
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("nu aveti drepturi") + System.Environment.NewLine;
                        continue;
                    }

                    //verificam daca se respecta ordinea din circuit numai pt cei care nu sunt HR
                    if (!HR && Convert.ToInt32(General.Nz(dr["RespectaOrdinea"], 0)) == 1 && Convert.ToInt32(General.Nz(dr["PozitieIstoric"], 0)) != (Convert.ToInt32(General.Nz(dr["Pozitie"], 0)) + 1))
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("nu se respecta ordinea de pe circuit") + System.Environment.NewLine;
                        continue;
                    }

                    try
                    {
                        DateTime ziDrp = Dami.DataDrepturi(Convert.ToInt32(General.Nz(dr["Drepturi_Valoare"], -99)), Convert.ToInt32(General.Nz(dr["Drepturi_NrZile"], 0)), Convert.ToDateTime(dr["DataInceput"]), Convert.ToInt32(dr["F10003"]));
                        if (Convert.ToDateTime(dr["DataInceput"]).Date < ziDrp)
                        {
                            if (ziDrp.Year == 2111 && ziDrp.Month == 11 && ziDrp.Day == 11)
                                log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu aveti stabilite drepturi pentru a realiza aceasta operatie") + " " + System.Environment.NewLine;
                            else
                            {
                                if (ziDrp.Year == 2222 && ziDrp.Month == 12 && ziDrp.Day == 13)
                                    log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("pontajul a fost aprobat") + " " + System.Environment.NewLine;
                                else
                                    log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("data") + " " + (tipActiune == 1 ? Dami.TraduCuvant("aprobare") : Dami.TraduCuvant("respingere")) + " " + Dami.TraduCuvant("cerere este") + " " + ziDrp + System.Environment.NewLine;
                            }
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                        General.MemoreazaEroarea(ex, "Dami", "MetodeCereri - ParamCereri");
                    }

                    int idStare = 2;

                    if (HR)
                        idStare = 3;
                    else
                        if (Convert.ToInt32(dr["TotalSuperCircuit"]) == Convert.ToUInt32(dr["PozitieIstoric"])) idStare = 3;

                    //daca este ultimul pe circuit ne asiguram ca s-a ales tipul de compensare => banca sau plata
                    if (idStare == 3 && Convert.ToInt32(General.Nz(dr["Compensare"], 0)) != 0 && Convert.ToInt32(General.Nz(dr["TrimiteLa"], -99)) == -99)
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("nu s-a ales tipul de compensare");
                        continue;
                    }

                    if (tipActiune == 2) idStare = 0;

                    //comentariile si inlocuitorii se schimba in forma detaliata
                    string addCmp = "";
                    if (Convert.ToInt32(General.Nz(dr["IdUser"], -99)) != idUser)
                        addCmp = @" ,""Inlocuitor"" = 1, ""IdUserInlocuitor"" = " + idUser;

                    int pozitieIstoric = Convert.ToInt32(General.Nz(dr["PozitieIstoric"], -99));
                    int idIst = Convert.ToInt32(General.Nz(dr["IdIst"], -99));

                    if (HR)
                    {
                        DataRow drUlt = General.IncarcaDR(
                            $@"SELECT * FROM (
                            SELECT W.*, ROW_NUMBER() OVER(ORDER BY W.""Pozitie"" DESC) AS ""IdRow"" 
                            FROM ""Ptj_CereriIstoric"" W
                            WHERE ""IdCerere"" = @1) W
                            WHERE ""IdRow"" <= 1", new object[] { dr["Id"] });

                        if (drUlt != null && drUlt["IdCerere"] != null)
                        {
                            pozitieIstoric = Convert.ToInt32(General.Nz(drUlt["Pozitie"], -99));
                            idIst = Convert.ToInt32(General.Nz(drUlt["IdAuto"], -99));
                        }
                    }

                    string addMtv = @" ,""Comentarii"" = COALESCE(""Comentarii"",'') + @1 ";

                    string sqlCer = $@"UPDATE ""Ptj_Cereri"" SET 
                                        ""Pozitie""={pozitieIstoric}, 
                                        ""IdStare""={idStare}, 
                                        ""Culoare""=(SELECT COALESCE(""Culoare"",'#FFFFFFFF') FROM ""Ptj_tblStari"" WHERE ""Id""={idStare}),
                                        USER_NO={idUser},
                                        TIME={General.CurrentDate()}
                                        {addMtv}
                                        WHERE ""Id""={dr["Id"]}";

                    string sqlIst = $@"UPDATE ""Ptj_CereriIstoric"" SET 
                                        ""DataAprobare""={General.CurrentDate()}, 
                                        ""Aprobat""=1, 
                                        ""IdStare""={idStare}, 
                                        ""Culoare""=(SELECT COALESCE(""Culoare"",'#FFFFFFFF') FROM ""Ptj_tblStari"" WHERE ""Id""={idStare}),
                                        USER_NO={idUser},
                                        TIME={General.CurrentDate()} 
                                        {addCmp}
                                        WHERE ""IdAuto""={idIst}";


                    string msg = Notif.TrimiteNotificare("Absente.Lista", 2, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + dr["Id"], "", Convert.ToInt32(dr["Id"]), idUser, userMarca);
                    if (msg != "" && msg.Substring(0,1) == "2")
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant(msg.Substring(2));
                        continue;
                    }
                    else
                    {
                        try
                        {
                            //Florin 2018.04.04
                            string sqlDel = "";
                            if (tipActiune == 2)
                                sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={dr["Id"]} AND ""Tabela""='Ptj_Cereri' AND ""EsteCerere"" = 1; ";

                            string sqlFinal = @"BEGIN" + "\n\r" +
                                                    sqlCer + "; \n\r" +
                                                    sqlIst + "; \n\r" +
                                                    sqlDel + "\n\r" +
                                                    "END;";
                            bool ras = General.ExecutaNonQuery(sqlFinal, new object[] { motiv.Replace("\\", "\\\\").Replace("'", "''") });

                            if (!ras)
                            {
                                log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("a aparut o eroare") + System.Environment.NewLine;
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            General.ExecutaNonQuery("ROLLBACK TRAN", null);
                            General.MemoreazaEroarea(ex, "Dami", "MetodeCereri - Executa");
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + Convert.ToDateTime(dr["DataInceput"]).ToShortDateString() + " - " + Dami.TraduCuvant("a aparut o eroare") + System.Environment.NewLine;
                            continue;
                        }

                        //trimite in pontaj daca este finalizat
                        if (idStare == 3)
                        {
                            if ((Convert.ToInt32(General.Nz(dr["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(dr["IdTipOre"], 0)) == 0 && General.Nz(dr["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(dr["NuTrimiteInPontaj"], 0)) == 0)
                                General.TrimiteInPontaj(idUser, Convert.ToInt32(dr["Id"]), 5, Convert.ToInt32(General.Nz(dr["TrimiteLa"], 0)), Convert.ToDecimal(General.Nz(dr["NrOre"], 0)));

                            if (Convert.ToInt32(General.Nz(dr["IdTipOre"], 0)) == 1 && Dami.ValoareParam("PontajCCStergeDacaAbsentaDeTipZi") == "1")
                                General.ExecutaNonQuery($@"DELETE FROM ""Ptj_CC"" WHERE F10003={dr["F10003"]} AND {General.ToDataUniv(Convert.ToDateTime(dr["DataInceput"]))} <= ""Ziua"" AND ""Ziua"" <= {General.ToDataUniv(Convert.ToDateTime(dr["DataSfarsit"]))} ", null);

                            General.CalculFormule(dr["F10003"], null, Convert.ToDateTime(dr["DataInceput"]), Convert.ToDateTime(dr["DataSfarsit"]));
                        }

                        //completeaza soldul de ZL; Este numai pt clientul Groupama
                        if (tipActiune == 2) General.SituatieZLOperatii(Convert.ToInt32(dr["F10003"]), Convert.ToDateTime(dr["DataInceput"]), 2, Convert.ToInt32(General.Nz(dr["NrZile"], 1)));

                        //Florin 2020.09.16
                        string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(HttpContext.Current.Session["IdClient"], "1").ToString(), General.Nz(HttpContext.Current.Session["IdLimba"], "RO").ToString() };

                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                        {
                            NotifAsync.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + dr["Id"], "Ptj_Cereri", Convert.ToInt32(dr["Id"]), idUser, userMarca, arrParam);
                        });

                        //Florin 2021.05.28
                        DateTime dtInc = Convert.ToDateTime(dr["DataInceput"]);
                        DateTime dtSf = Convert.ToDateTime(dr["DataSfarsit"]);
                        if (tipActiune == 1)
                            log += Dami.TraduCuvant("Cererea de") + " " + dr["DenumireAbsenta"] + " " + Dami.TraduCuvant("solicitata de") + " " + dr["NumeComplet"] + " " + Dami.TraduCuvant("pentru perioada") + " " + dtInc.Day.ToString().PadLeft(2, '0') + " " + NumeLuna(dtInc.Month) + " " + dtInc.Year + " - " + dtSf.Day.ToString().PadLeft(2, '0') + " " + NumeLuna(dtSf.Month) + " " + dtSf.Year + " " + Dami.TraduCuvant("a fost aprobata") + System.Environment.NewLine;
                        else
                            log += Dami.TraduCuvant("Cererea de") + " " + dr["DenumireAbsenta"] + " " + Dami.TraduCuvant("solicitata de") + " " + dr["NumeComplet"] + " " + Dami.TraduCuvant("pentru perioada") + " " + dtInc.Day.ToString().PadLeft(2, '0') + " " + NumeLuna(dtInc.Month) + " " + dtInc.Year + " - " + dtSf.Day.ToString().PadLeft(2, '0') + " " + NumeLuna(dtSf.Month) + " " + dtSf.Year + " " + Dami.TraduCuvant("a fost respinsa") + System.Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                log = ex.Message;
                General.MemoreazaEroarea(ex, "General", "MetodeCereri");
            }

            return log;
        }




        //Radu 19.07.2018
        public static string MetodeCereriDiverse(int tipActiune, List<int> arr, int idUser, int userMarca, string motiv = "")
        {
            string log = "";

            //actiune  1  - aprobare
            //actiune  2  - respingere

            try
            {
                if (arr.Count == 0) return "Eroare";

                bool HR = false;
                string strSelect = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    strSelect += " UNION SELECT " + arr[i] + " AS Id ";
                }

                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                string strSql = $@"SELECT A.*, G.F10008 {op} ' ' {op} G.F10009 AS ""NumeComplet"", E.""RespectaOrdinea"", 
                                B.""IdCerere""  AS ""IdCerere"",
                                B.""Pozitie""  AS ""PozitieIstoric"",
                                B.""Aprobat""  AS ""Aprobat"",
                                B.""IdUser""  AS ""IdUser"",
                                B.""IdAuto""  AS ""IdIst""
                                FROM({strSelect.Substring(6)}) RL
                                INNER JOIN ""MP_Cereri"" A ON RL.Id = A.Id
                                LEFT JOIN ""MP_CereriIstoric"" B ON A.""Id""=B.""IdCerere"" 

                                AND ((B.""IdSuper"" >= 0 and B.""IdUser"" = {idUser}) or (B.""IdSuper"" < 0 and
				                {idUser} in (select sup.""IdUser"" from ""F100Supervizori"" sup where sup.F10003 = A.F10003 AND A.""Id""=B.""IdCerere"" and sup.""IdSuper"" = (-1) * B.""IdSuper"") ))
                                
                                LEFT JOIN ""MP_CereriCircuit"" E ON A.""IdCircuit""=E.""IdAuto""
                                LEFT JOIN F100 G ON A.F10003=G.F10003
                                WHERE 1=1 ";
                DataTable dt = General.IncarcaDT(strSql, null);

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    DataRow dr = dt.Rows[j];
                    if (Convert.ToInt32(General.Nz(dr["IdUser"], -99)) == 99999) HR = true;

                    #region Validare stare cerere
                    //Buacata de cod identica cu cea din Cereri -> btnAprobare si btnRespinge
                    //este nevoie de ea si aici pt cazul cand se aproba/respinge de pe mail

                    switch (Convert.ToInt32(General.Nz(dr["IdStare"], 0)))
                    {
                        case -1:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("este deja anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("este deja respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                            continue;
                        case 4:
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    #endregion

                    //Florin 2018.05.15
                    if (!HR)
                    {
                        if (Convert.ToInt32(General.Nz(dr["IdCerere"], -99)) == -99)
                        {
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("user-ul logat nu se regaseste pe circuit") + System.Environment.NewLine;
                            continue;
                        }
                    }

                    if (Convert.ToInt32(General.Nz(dr["Aprobat"], 0)) == 1)
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("nu aveti drepturi") + System.Environment.NewLine;
                        continue;
                    }

                    //verificam daca se respecta ordinea din circuit numai pt cei care nu sunt HR
                    if (!HR && Convert.ToInt32(General.Nz(dr["RespectaOrdinea"], 0)) == 1 && Convert.ToInt32(General.Nz(dr["PozitieIstoric"], 0)) != (Convert.ToInt32(General.Nz(dr["Pozitie"], 0)) + 1))
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("nu se respecta ordinea de pe circuit") + System.Environment.NewLine;
                        continue;
                    }

                    int idStare = 2;

                    if (HR)
                        idStare = 3;

                    if (tipActiune == 2) idStare = 0;


                    //comentariile si inlocuitorii se schimba in forma detaliata
                    string addCmp = "";
                    if (Convert.ToInt32(General.Nz(dr["IdUser"], -99)) != idUser)
                        addCmp = @" ,""Inlocuitor"" = 1, ""IdUserInlocuitor"" = " + idUser;

                    int pozitieIstoric = Convert.ToInt32(General.Nz(dr["PozitieIstoric"], -99));

                    if (pozitieIstoric == Convert.ToInt32(dr["TotalCircuit"].ToString()))
                        idStare = 3;

                    int idIst = Convert.ToInt32(General.Nz(dr["IdIst"], -99));

                    if (HR)
                    {
                        DataRow drUlt = General.IncarcaDR($"SELECT TOP 1 * FROM \"MP_CereriIstoric\" WHERE \"IdCerere\"={dr["Id"]} ORDER BY \"Pozitie\" DESC", null);
                        if (drUlt != null && drUlt["IdCerere"] != null)
                        {
                            pozitieIstoric = Convert.ToInt32(General.Nz(drUlt["Pozitie"], -99));
                            idIst = Convert.ToInt32(General.Nz(drUlt["IdAuto"], -99));
                        }
                    }

                    string sqlCer = $@"UPDATE ""MP_Cereri"" SET 
                                        ""Pozitie""={pozitieIstoric}, 
                                        ""IdStare""={idStare}, 
                                        ""Culoare""=(SELECT COALESCE(""Culoare"",'#FFFFFFFF') FROM ""Ptj_tblStari"" WHERE ""Id""={idStare}),
                                        USER_NO={idUser},
                                        TIME={General.CurrentDate()}
                                        
                                        WHERE ""Id""={dr["Id"]}";

                    string sqlIst = $@"UPDATE ""MP_CereriIstoric"" SET 
                                        ""DataAprobare""={General.CurrentDate()}, 
                                        ""Aprobat""=1, 
                                        ""IdStare""={idStare}, 
                                        ""Culoare""=(SELECT COALESCE(""Culoare"",'#FFFFFFFF') FROM ""Ptj_tblStari"" WHERE ""Id""={idStare}),
                                        USER_NO={idUser},
                                        TIME={General.CurrentDate()} 
                                        {addCmp}
                                        WHERE ""IdAuto""={idIst}";


                    string msg = Notif.TrimiteNotificare("CereriDiverse.Lista", 2, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""MP_Cereri"" Z WHERE ""Id""=" + dr["Id"], "", Convert.ToInt32(dr["Id"]), idUser, userMarca);
                    if (msg != "" && msg.Substring(0, 1) == "2")
                    {
                        log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant(msg.Substring(2));
                        continue;
                    }
                    else
                    {
                        try
                        {
                            //Florin 2018.04.04
                            string sqlDel = "";
                            if (tipActiune == 2)
                                sqlDel = $@"DELETE FROM ""tblFisiere"" WHERE ""Id""={dr["Id"]} AND ""Tabela""='MP_Cereri' ; ";

                            string sqlFinal = @"BEGIN" + "\n\r" +
                                                    sqlCer + "; \n\r" +
                                                    sqlIst + "; \n\r" +
                                                    sqlDel + "\n\r" +
                                                    "END;";
                            bool ras = General.ExecutaNonQuery(sqlFinal, new object[] { motiv.Replace("\\", "\\\\").Replace("'", "''") });

                            if (!ras)
                            {
                                log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("a aparut o eroare") + System.Environment.NewLine;
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            General.ExecutaNonQuery("ROLLBACK TRAN", null);
                            General.MemoreazaEroarea(ex, "Dami", "MetodeCereriDiverse - Executa");
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("a aparut o eroare") + System.Environment.NewLine;
                            continue;
                        }

                        Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Notificare, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""MP_Cereri"" Z WHERE ""Id""=" + dr["Id"], "MP_Cereri", Convert.ToInt32(dr["Id"]), idUser, userMarca);

                        if (tipActiune == 1)
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("a fost aprobata") + System.Environment.NewLine;
                        else
                            log += Dami.TraduCuvant("Cererea pt") + " " + dr["NumeComplet"] + "-" + dr["Descriere"].ToString() + " - " + Dami.TraduCuvant("a fost respinsa") + System.Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                log = ex.Message;
                General.MemoreazaEroarea(ex, "General", "MetodeCereriDiverse");
            }

            return log;

        }

        public static string Encrypt_QueryString(string str)
        {
            try
            {
                string EncrptKey = "2017;[pnuLIT)WizOne";
                byte[] byKey = { };
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
                byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "Criptare");
                return null;
            }
        }

        public static string Decrypt_QueryString(string str)
        {
            try
            {
                str = str.Replace(" ", "+");
                string DecryptKey = "2017;[pnuLIT)WizOne";
                byte[] byKey = { };
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
                byte[] inputByteArray = new byte[str.Length];

                byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(str);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "Criptare");
                return null;
            }
        }

        //Radu
        public static DataTable GetTipContract()
        {
            DataTable table = new DataTable();

            try
            {

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(0, "Altele");
                table.Rows.Add(9, "Cenzor/CA");
                table.Rows.Add(34, "CIM cu clauza de telemunca");
                table.Rows.Add(33, "CIM tineri dezavantajati (Legea 189 / 2018)");
                table.Rows.Add(30, "Consiliu Supraveghere");
                table.Rows.Add(31, "Consiliu Supraveghere straini");
                table.Rows.Add(97, "Ctr. Agent");
                table.Rows.Add(99, "Ctr./conventii cf. Cod Civil");
                table.Rows.Add(3, "Ctr. de munca la domiciliu");
                table.Rows.Add(2, "Ctr. de munca temporar");
                table.Rows.Add(4, "Ctr. de ucenicie la locul de munca");
                table.Rows.Add(1, "Ctr. individual de munca");
                table.Rows.Add(32, "Ctr. internship");     
                table.Rows.Add(8, "Ctr. mandat");
                table.Rows.Add(35, "Ctr.temporar cu clauza de telemunca");
                table.Rows.Add(98, "Drepturi de autor si drepturi conexe");

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return table;
        }

        public static DataTable ListaMP_GradInvaliditate()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "Valid");
                table.Rows.Add(2, "Invalid grad I");
                table.Rows.Add(3, "Invalid grad II");
               

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable ListaRezultatExamen()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "Apt");
                table.Rows.Add(2, "Apt conditionat");
                table.Rows.Add(3, "Inapt");
                table.Rows.Add(4, "Inapt temporar");

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetDurataContract()
        {
            string sql = @"SELECT * FROM F089 ORDER BY F08903";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F089", "F08902") + " ORDER BY F08903";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetPrelungireContract()
        {
            DataTable table = new DataTable();

            try
            {


                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "DA");
                table.Rows.Add(0, "NU");
         
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return table;
        }

        public static DataTable GetExceptieIncetare()
        {
            string sql = @"SELECT * FROM F094 ORDER BY F09403";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F094", "F09402") + " ORDER BY F09403";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetCASS()
        {
            string sql = @"SELECT * FROM F063 ORDER BY F06303";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F063", "F06302") + " ORDER BY F06303";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetTipAngajat()
        {
            string sql = @"SELECT * FROM F716 ORDER BY F71604";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F716", "F71602") + " ORDER BY F71604 ";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetTimpPartial(string tip)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Denumire", typeof(string));

            int j = 1, k = 7;
            if (Convert.ToInt32(tip) == 0)
            {
                j = 6;
                k = 8;
            }

            for (int i = j; i <= k; i++)
                table.Rows.Add(i, i.ToString());

            return table;
        }

        public static DataTable GetTipNorma(string param)
        {
            string sql = @"SELECT * FROM F092 WHERE F09202 = " + param + " ORDER BY F09203";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F092", "F09202") + " WHERE F09202 = " + param + " ORDER BY F09203";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetNorma()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Denumire", typeof(string));

            for (int i = 6; i <= 8; i++)
                table.Rows.Add(i, i.ToString());

            return table;
        }

        public static DataTable GetDurataTimpMunca(string param)
        {
            string cond = " WHERE F09105 IN (0, 1, 2)";
            if (param == "1")
                cond = " WHERE F09105 = 1";
            if (param == "2")
                cond = " WHERE F09105 = 2";
            //Florin #715
            if (param == "3")
                cond = " WHERE F09105 = 3";
            string sql = @"SELECT * FROM F091 " + cond + " ORDER BY F09103";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F091", "F09102") + cond + " ORDER BY F09103";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetRepartizareTimpMunca()
        {
            string sql = @"SELECT * FROM F093 ORDER BY F09303";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F093", "F09302") + " ORDER BY F09303";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetIntervalRepartizareTimpMunca()
        {
            string sql = @"SELECT * FROM F096 ORDER BY F09603";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F096", "F09602") + " ORDER BY F09603";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetCOR()
        {
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            DataTable table = ds.Tables[0];          
            return General.IncarcaDT("SELECT F72204, F72202 FROM F722 WHERE F72206 = " + (table.Rows[0]["F1001082"] as string ?? "(SELECT MAX(F72206) FROM F722)") + " ORDER BY F72204", null);     
        }

        public static DataTable GetFunctie()
        {
            string sql = @"SELECT * FROM F718 ORDER BY F71804";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F718", "F71802") + " ORDER BY F71804";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetMeserie()
        {
            string sql = @"SELECT * FROM F717 ORDER BY F71704";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F717", "F71702") + " ORDER BY F71704";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetMotivPlecare()
        {
            string sql = @"SELECT * FROM F721 ORDER BY F72104";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F721", "F72102") + " ORDER BY F72104";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetStructOrgModif(DateTime data)
        {
            DataTable table = new DataTable();

            string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
            string cmpData = "";

            if (Constante.tipBD == 2) cmp = "CAST(ROWNUM AS INT) ";

            if (data != null)
            {
                string dataRef = data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString();
                if (Constante.tipBD == 2)
                {
                    cmpData = " WHERE  B.F00310 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= B.F00311 AND "
                        + " C.F00411 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= C.F00412 AND "
                        + " D.F00513 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= D.F00514 AND "
                        + " E.F00622 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= E.F00623  ";
                }
                else
                {
                    cmpData = " WHERE B.F00310 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= B.F00311 AND "
                        + " C.F00411 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= C.F00412 AND "
                        + " D.F00513 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= D.F00514 AND "
                        + " E.F00622 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= E.F00623  ";
                }
            }
            string strSql = @"SELECT {0} as ""IdAuto"", a.F00204, b.F00305, c.F00406, d.F00507 ,e.F00608, F.F00709, G.F00810,
                                a.F00202, b.F00304 , c.F00405 , d.F00506, e.F00607, F.F00708 , G.F00809, COALESCE(E.F00615, 9999) AS CC
                                FROM F002 A
                                LEFT JOIN F003 B ON A.F00202 = B.F00303
                                LEFT JOIN F004 C ON B.F00304 = C.F00404
                                LEFT JOIN F005 D ON C.F00405 = D.F00505 
                                LEFT JOIN F006 E ON D.F00506 = E.F00606
                                LEFT JOIN F007 F ON E.F00607 = F.F00707
                                LEFT JOIN F008 G ON F.F00708 = G.F00808
                                {1}
                                ORDER BY E.F00607";

            strSql = string.Format(strSql, cmp, cmpData);

            table = General.IncarcaDT(strSql, null);

            return table;
        }

        public static DataTable GetStructOrgModifGen(string data)
        {
            return GetStructOrgModif(Convert.ToDateTime(data));
        }

        public static DataTable GetStructOrgAng(int marca)
        {
            DataTable table = new DataTable();

            string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";

            if (Constante.tipBD == 2) cmp = "CAST(ROWNUM AS INT) ";

            string strSql = $@"SELECT {cmp} as IdAuto, a.F00204, b.F00305, c.F00406, d.F00507 ,e.F00608, F.F00709, G.F00810,
                            a.F00202, b.F00304 , c.F00405 , d.F00506, e.F00607, F.F00708 , G.F00809
                            FROM F100 X
                            LEFT JOIN F1001 Y ON X.F10003=Y.F10003
                            LEFT JOIN F002 A ON X.F10002 = A.F00202 
                            LEFT JOIN F003 B ON X.F10004 = B.F00304 
                            LEFT JOIN F004 C ON X.F10005 = C.F00405 
                            LEFT JOIN F005 D ON X.F10006 = D.F00506 
                            LEFT JOIN F006 E ON X.F10007 = E.F00607
                            LEFT JOIN F007 F ON Y.F100958 = F.F00708
                            LEFT JOIN F008 G ON Y.F100959 = G.F00809
                            WHERE X.F10003={marca}";

            table = General.IncarcaDT(strSql, null);

            return table;
        }  

        public static DataTable GetCetatenie()
        {
            string sql = @"SELECT * FROM F732 ORDER BY F73204";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F732", "F73202") + " ORDER BY F73204";
            return General.IncarcaDT(sql, null);
        }
        
        public static DataTable GetTipAutMunca()
        {
            string sql = @"SELECT * FROM F088 ORDER BY F08803";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F088", "F08802") + " ORDER BY F08803";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetTipDoc(int idTara)
        {
            return General.IncarcaDT("select CAST(a.F08502 AS INT) AS \"Id\", a.F08503 as \"Denumire\" from F085 a join F086 b on a.F08502 = b.F08603 join F732 c on b.F08602 = c.F73202 join F733 d on c.F73202 = d.F73306 where d.F73302 = " + idTara.ToString() + " ORDER BY \"Denumire\"", null);
        }

        public static DataTable GetCategPermis()
        {
            string sql = @"SELECT * FROM F714 ORDER BY F71404";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F714", "F71402") + " ORDER BY F71404";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetMotivScutireImpozit()
        {
            string sql = @"SELECT * FROM F804 ORDER BY F80404";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F804", "F80403") + " ORDER BY F80404";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetMotivScutireCAS()
        {
            string sql = @"SELECT * FROM F802 ORDER BY F80204";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F802", "F80203") + " ORDER BY F80204";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetStudii()
        {
            string sql = @"SELECT * FROM F712 ORDER BY F71204";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F712", "F71202") + " ORDER BY F71204";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetTitluAcademic()
        {
            string sql = @"SELECT * FROM F713 ORDER BY F71304";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F713", "F71302") + " ORDER BY F71304";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable ListaMP_DeduceriSomaj()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));
                               
                table.Rows.Add(1, "Absolvent");
                table.Rows.Add(0, "Nu");
                table.Rows.Add(2, "Somer");

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetBanci()
        {
            string sql = @"SELECT CAST(F07503 AS INT) AS F07503, F07509 FROM F075 GROUP BY F07503, F07509 ORDER BY F07509";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetSucursale(int banca)
        {
            string sql = @"SELECT * FROM F075 WHERE F07503 = " + banca.ToString() + " ORDER BY F07505";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F075", "F07504") + " WHERE F07503 = " + banca.ToString() + " ORDER BY F07505";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetStareCivila()
        {
            string sql = @"SELECT * FROM F710 ORDER BY F71004";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F710", "F71002") + " ORDER BY F71004";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetCategAng_61()
        {
            string sql = @"SELECT * FROM F724 WHERE F72411 IN (0,1) ORDER BY F72404";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F724", "F72402") + " WHERE F72411 IN (0,1) ORDER BY F72404";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetCategAng_62()
        {
            string sql = @"SELECT * FROM F724 WHERE F72411 IN (0,2) ORDER BY F72404";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F724", "F72402") + " WHERE F72411 IN (0,2) ORDER BY F72404";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetCentriCost()
        {
            string sql = @"SELECT * FROM F062";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F062", "F06204");
            return General.IncarcaDT(sql, null);
        }

        public static DataTable ListaContacteF100()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(3, "Mail");
                table.Rows.Add(1, "Telefon 1");
                table.Rows.Add(2, "Telefon 2");
        

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetObiecteDinArie(string numeArie)
        {
            string strSql = "";
            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = @"select a.""IdCategorie"", CAST (a.""Id"" AS INT) as ""IdObiect"", b.""Denumire"" {1} '/' {1} a.""Denumire"" as ""NumeCompus"", a.""ValoareEstimata"", a.""Denumire"" as ""NumeObiect""
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = '{0}') ORDER BY ""NumeCompus""";

                strSql = string.Format(strSql, numeArie, op);
               
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return General.IncarcaDT(strSql, null);
        }

        public static DataTable GetF100NumeComplet()
        {
            string strSql = "";

            try
            {
                string op = "+";
                string dt = "GetDate()";
                if (Constante.tipBD == 2)
                {
                    op = "||";
                    dt = "SysDate";
                }
                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                strSql = @"SELECT CAST(A.F10003 AS int) AS F10003,  A.F10008 {0} ' ' {0} A.F10009 AS ""NumeComplet"", A.F10017 AS CNP,
                                B.F00204 AS ""Companie"", C.F00305 AS ""Subcompanie"", D.F00406 AS ""Filiala"", E.F00507 AS ""Sectie"", F.F00608 AS ""Departament"", 
                                A.F10002, A.F10004, A.F10005, A.F10006, A.F10007, A.F10022, A.F10023, A.F10022 AS ""DataAngajarii"", A.F10023 AS ""DataPlecarii"",
                                A.F10011 AS ""NrContract"", A.{2} AS ""SalariulBrut"", X.F71804 AS ""Functia"", A.F100992 AS ""DataFunctie"", A.F10025,
                                A.F100571 AS ""Regiune"", A.F100925, A.F100901, A.F100922, A.F100923, A.F100924,
                                CASE WHEN (A.F10025 = 0 OR A.F10025 = 999) THEN 1 ELSE 0 END AS ""AngajatActiv"", 
                                CASE WHEN (A.F10022 <= {1} AND {1} <= A.F10023) THEN CAST(1 AS int) ELSE CAST(0 AS int) END AS ""Stare"",
                                (SELECT MAX(G.F70102) FROM USERS G WHERE A.F10003=G.F10003) AS ""IdUser""
                                FROM F100 A
                                LEFT JOIN F002 B ON A.F10002 = B.F00202 
                                LEFT JOIN F003 C ON A.F10004 = C.F00304 
                                LEFT JOIN F004 D ON A.F10005 = D.F00405 
                                LEFT JOIN F005 E ON A.F10006 = E.F00506 
                                LEFT JOIN F006 F ON A.F10007 = F.F00607
                                LEFT JOIN USERS G ON A.F10003=G.F10003
                                LEFT JOIN F718 X ON A.F10071=X.F71802
                                ORDER BY A.F10008, A.F10009";

                strSql = string.Format(strSql, op, dt, salariu);
           }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return General.IncarcaDT(strSql, null);

        }

        public static DataTable GetF090()
        {
            string sql = @"SELECT * FROM F090";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F090", "F09002");
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetF733()
        {
            string sql = @"SELECT * FROM F733 ORDER BY F73304";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F733", "F73302") + " ORDER BY F73304";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetF737()
        {
            string sql = @"SELECT * FROM F737 ORDER BY F73703";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F737", "F73702") + " ORDER BY F73703";
            return General.IncarcaDT(sql, null);
        }


        public static DataTable ListaStariAngajat()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "-");
                table.Rows.Add(2, Dami.TraduCuvant("Activ"));
                table.Rows.Add(3, Dami.TraduCuvant("Activ suspendat"));
                table.Rows.Add(4, Dami.TraduCuvant("Inactiv"));
                table.Rows.Add(6, Dami.TraduCuvant("Candidat"));
                table.Rows.Add(7, Dami.TraduCuvant("Angajat in avans"));
                table.Rows.Add(8, Dami.TraduCuvant("Activ detasat"));

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        public static DataTable GetPersonalRestrans(int idUser, string strStare, int tip)
        {
            //tip
            //1 = lista obisnuita
            //2 - lista Benchmark

            DataTable q = null;

            try
            {
                string cmp = "";
                string left = "";
                string strFiltru = "";
                string strSql = "";
                string op = "+";
                string dt = "cast(getdate() as date) ";
                string filtruAng = "", condAng = "";
                string tipData = "INT";

                if (Constante.tipBD == 2)
                {
                    op = "||";
                    dt = "trunc(sysdate) ";
                    tipData = "NUMBER(9)";
                }

                strSql = @"SELECT * FROM (
                            SELECT CAST(A.F10003 AS {8}) AS ""Marca"",A.F10017 AS ""CNP"",A.F10008 {4} ' ' {4} A.F10009 AS ""NumeComplet"",
                            E.F00204 AS ""Companie"",F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00506 AS ""Sectie"",I.F00607 AS ""Departament"", A.F10022 AS ""DataAngajarii"",
                            CASE WHEN A.F10025 = 900 THEN '#ffff8000' ELSE CASE WHEN A.F10025 = 999 THEN '#dc96dc' ELSE (CASE WHEN A.F10025 = 0 THEN (CASE WHEN (A.F100925 <> 0 AND F100922 IS NOT NULL AND F100923 IS NOT NULL AND F100923 IS NOT NULL AND F100922 <= {5} AND {5} <= F100923 AND {5} <= F100924) THEN '#ffffffc8' ELSE CASE WHEN (A.F100915 <= {5} AND {5} <= A.F100916) THEN '#FF0066FF' ELSE '#ffc8ffc8' END END) ELSE '#ffffc8c8' END) END END AS ""Culoare"",
                            CASE WHEN A.F10025 = 900 THEN 'Candidat' ELSE CASE WHEN A.F10025 = 999 THEN 'Angajat in avans' ELSE (CASE WHEN A.F10025 = 0 THEN (CASE WHEN (A.F100925 <> 0 AND F100922 IS NOT NULL AND F100923 IS NOT NULL AND F100923 IS NOT NULL AND F100922 <= {5} AND {5} <= F100923 AND {5} <= F100924) THEN 'Activ suspendat' ELSE CASE WHEN (A.F100915 <= {5} AND {5} <= A.F100916) THEN 'Activ detasat' ELSE 'Activ' END END) ELSE 'Inactiv' END) END END AS ""Stare"",
                            (CASE WHEN COALESCE(A.F10083,'') = '' THEN '' ELSE CASE WHEN adr.F1001021 IS NULL OR adr.F1001021 = 0 THEN ' Str. ' ELSE (SELECT ""Denumire"" FROM ""tblTipStrada"" WHERE ""Id"" = adr.F1001021) {4} ' ' END {4} a.F10083 END) {4}
                            (CASE WHEN COALESCE(A.F10084,'') = '' THEN '' ELSE ' Nr. ' {4} a.F10084 END) {4}
                            (CASE WHEN COALESCE(A.F10085,'') = '' THEN '' ELSE ' Bloc. ' {4} a.F10085 END) {4}
                            (CASE WHEN COALESCE(A.F100892,'') = '' THEN '' ELSE ' Sc. ' {4} a.F100892 END) {4}
                            (CASE WHEN COALESCE(A.F100893,'') = '' THEN '' ELSE ' Et. ' {4} a.F100893 END) {4}

                            (CASE WHEN a.F100897 is null or a.F100897 = 0 THEN '' ELSE case when niv3.tip in (11, 19, 22, 23) then ' sat ' {4} niv3.denloc else
                            case when niv3.tip = 6 and upper(niv3.denloc) not like '%SECTOR%' then ' sector ' {4} niv3.DENLOC else ' ' {4} niv3.denloc end END end) {4}

                            (case when a.F100914 is null or a.F100914 = 0 THEN '' else case when niv2.tip in (1, 4) and UPPER(niv2.DENLOC) not like '%MUNICIPIU%' then ' municipiul ' {4} niv2.DENLOC else
                            case when niv2.tip = 2 and UPPER(niv2.DENLOC) not like '%ORAS%' then ' orasul ' {4} niv2.DENLOC else 
                            case when niv2.TIP = 3 and upper(niv2.DENLOC) not like '%COMUNA%' then ' comuna ' {4} niv2.DENLOC else ' ' {4} niv2.denloc end end end end ) {4}

                            (CASE WHEN a.F100921 is null or a.F100921 = 0 THEN '' ELSE case when upper(niv1.denloc) like '%JUDET%' or upper(niv1.denloc)  like '%MUNICIPIU%' then ' ' {4} niv1.DENLOC else
                             case when upper(niv1.denloc) like '%BUCURESTI%' then ' municipiul ' {4} niv1.DENLOC else ' judetul ' {4} niv1.denloc end end END) {4}
                            

                            (CASE WHEN COALESCE(A.F10087,'') = '' THEN '' ELSE ' Cod Postal. ' {4} a.F10087 END) AS ""AdresaCompleta""
                            {1} {6}
                            FROM F100 A
                            LEFT JOIN F002 E ON A.F10002 = E.F00202 
                            LEFT JOIN F003 F ON A.F10004 = F.F00304 
                            LEFT JOIN F004 G ON A.F10005 = G.F00405 
                            LEFT JOIN F005 H ON A.F10006 = H.F00506 
                            LEFT JOIN F006 I ON A.F10007 = I.F00607 
                            left join Localitati niv1 on niv1.SIRUTA = a.F100921
                            left join Localitati niv2 on niv2.SIRUTA = a.F100914
                            left join Localitati niv3 on niv3.SIRUTA = a.F100897
                            LEFT JOIN F1001 adr ON adr.F10003 = a.F10003
                            {2} {7}
                            WHERE 1=1  {3}
                            ) X WHERE 1=1 {0}
                            ORDER BY ""NumeComplet"" ";

                if (tip == 2)
                {
                    cmp = @",B.F71804 AS ""Functie"",D.F71304 AS ""TitluAcademic"",a.F10021 AS ""DataN"",A.F10022 AS ""DataAng"",a.F10023 AS ""DataPlec"",a.F100922 AS ""DataSuspendarii"",a.F10047 AS ""Sex"",a.F100993 AS ""DataPlecIntrerupereContract"",C.F71204 AS ""Studii"" ";
                    left = @"LEFT JOIN F718 B ON A.F10071=B.F71802
                            LEFT JOIN F712 C ON A.F10050=C.F71202
                            LEFT JOIN F713 D ON A.F10051=D.F71302";
                }


                if (strStare != "" && strStare != "-")
                {
                    strFiltru = " AND (";
                    string[] param = strStare.Split(';');
                    for (int i = 0; i < param.Length; i++)
                    {
                        strFiltru += " \"Stare\"='" + param[i] + "'";
                        if (i < param.Length - 1)
                            strFiltru += " OR ";
                    }
                    strFiltru += ")";

                    filtruAng = " inner join f100 e on b.F10003=e.F10003 ";
                    condAng = " AND (";
                    for (int i = 0; i < param.Length; i++)
                    {
                        if (param[i] == Dami.TraduCuvant("Activ") || param[i] == Dami.TraduCuvant("Activ detasat") || param[i] == Dami.TraduCuvant("Activ suspendat"))
                            condAng += " e.F10025 = 0 ";
                        if (param[i] == Dami.TraduCuvant("Inactiv"))
                            condAng += " e.F10025 <> 0 ";
                        if (param[i] == Dami.TraduCuvant("Candidat"))
                            condAng += " e.F10025 = 900 ";
                        if (param[i] == Dami.TraduCuvant("Angajat in avans"))
                            condAng += " e.F10025 = 999 ";
                        if (i < param.Length - 1)
                            condAng += " OR ";
                    }
                    condAng += ")";
                }

                string cmpSupl = "", legCmpSupl = "";
                DataTable dtParam = General.IncarcaDT("SELECT \"Nume\", \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'MP_CampuriSuplimentare' OR \"Nume\" = 'MP_LegaturaCampuriSuplimentare'", null);
                if (dtParam != null && dtParam.Rows.Count > 0)
                {
                    for (int i = 0; i < dtParam.Rows.Count; i++)
                    {
                        if (dtParam.Rows[i]["Nume"].ToString() == "MP_CampuriSuplimentare")
                            cmpSupl = dtParam.Rows[i]["Valoare"].ToString();
                        if (dtParam.Rows[i]["Nume"].ToString() == "MP_LegaturaCampuriSuplimentare")
                            legCmpSupl = dtParam.Rows[i]["Valoare"].ToString();
                    }
                }
                strSql = string.Format(strSql, strFiltru, cmp, left, " AND A.F10003 IN (" + DamiAngajati(idUser, filtruAng, condAng) + ")", op, dt, cmpSupl, legCmpSupl, tipData);

                //Radu 09.12.2019
                strSql = strSql.Replace("'Activ'", "'" + Dami.TraduCuvant("Activ") + "'").Replace("'Activ suspendat'", "'" + Dami.TraduCuvant("Activ suspendat") + "'").Replace("'Inactiv'", "'" 
                    + Dami.TraduCuvant("Inactiv") + "'").Replace("'Candidat'", "'" + Dami.TraduCuvant("Candidat") + "'").Replace("'Angajat in avans'", "'" + Dami.TraduCuvant("Angajat in avans") + "'").Replace("'Activ detasat'", "'" + Dami.TraduCuvant("Activ detasat") + "'");

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public static string DamiAngajati(int idUser, string filtru, string cond)
        {
            string strSql = "";

            try
            {
                strSql = @"SELECT CAST(F10003 AS INT) AS F10003 FROM USERS WHERE F70102 = {0}
                            UNION
                            SELECT B.F10003 FROM ""relGrupAngajat"" B
                            INNER JOIN ""MP_relGrupSuper"" C ON B.""IdGrup"" = C.""IdGrup""
                            {1}
                            WHERE C.""IdSuper"" = {0} {2}
                            UNION
                            SELECT CAST(B.F10003 AS INT) AS F10003 FROM ""relGrupAngajat"" B
                            INNER JOIN ""MP_relGrupSuper"" C ON B.""IdGrup"" = C.""IdGrup""
                            INNER JOIN ""F100Supervizori"" D ON B.F10003=D.F10003 AND C.""IdSuper"" = -1 * D.""IdSuper""
                            {1}
                            WHERE D.""IdUser"" = {0} {2}";

                strSql = string.Format(strSql, idUser, filtru, cond);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static object IncarcaFotografie(object sender, int id, string Tabela)
        {
            try
            {
                object fisier = null;
                string sql = "SELECT * FROM \"tblFisiere\" WHERE \"Tabela\" = '" + Tabela + "' AND \"Id\" = " + id;
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    fisier = dt.Rows[0]["Fisier"];
                }
                return fisier;                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static void IncarcaFisier(string fileName, object fis, string Tabela, object cheie)
        {
            try
            {
                if (cheie != null)
                {
                    string sql = "SELECT * FROM \"tblFisiere\"";
                    DataTable dt = new DataTable();

                    dt = General.IncarcaDT(sql, null);
                    dt.TableName = "tblFisiere";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                    DataRow dr = null;
                    if (dt.Select("Tabela = '" + Tabela + "' AND Id = " + cheie.ToString()).Count() == 0)
                    {
                        dr = dt.NewRow();
                        dr["Tabela"] = Tabela;
                        dr["Id"] = cheie.ToString();
                        dr["Fisier"] = fis;
                        dr["FisierNume"] = System.IO.Path.GetFileName(fileName);
                        dr["FisierExtensie"] = System.IO.Path.GetExtension(fileName);
                        dr["USER_NO"] = HttpContext.Current.Session["UserId"];
                        dr["TIME"] = DateTime.Now;
                        if (Constante.tipBD == 1)
                            dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                        else
                            dr["IdAuto"] = Dami.NextId("tblFisiere");
                        dr["EsteCerere"] = 0;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        dr = dt.Select("Tabela = '" + Tabela + "' AND Id = " + cheie.ToString()).FirstOrDefault();
                        dr["Fisier"] = fis;
                        dr["FisierNume"] = System.IO.Path.GetFileName(fileName);
                        dr["FisierExtensie"] = System.IO.Path.GetExtension(fileName);
                        dr["EsteCerere"] = 0;
                        dr["USER_NO"] = HttpContext.Current.Session["UserId"];
                        dr["TIME"] = DateTime.Now;
                    }

                    if (Constante.tipBD == 1)
                    {
                        SqlDataAdapter da = new SqlDataAdapter();
                        SqlCommandBuilder cb = new SqlCommandBuilder();
                        da = new SqlDataAdapter();
                        da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"tblFisiere\"", null);
                        cb = new SqlCommandBuilder(da);
                        da.Update(dt);
                        da.Dispose();
                        da = null;
                    }
                    else
                    {
                        OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                        oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"tblFisiere\" WHERE ROWNUM = 0", null);
                        OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                        oledbAdapter.Update(dt);
                        oledbAdapter.Dispose();
                        oledbAdapter = null;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public static void ArataFisier(string Tabela, object cheie)
        {
            try
            {
                if (cheie != null)
                {
                    string sql = "SELECT * FROM \"tblFisiere\"";
                    DataTable dt = new DataTable();

                    dt = General.IncarcaDT(sql, null);
                    dt.TableName = "tblFisiere";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                    DataRow dr = null;
                    if (dt.Select("Tabela = '" + Tabela + "' AND Id = " + cheie.ToString()).Count() > 0)
                    {
                        dr = dt.Select("Tabela = '" + Tabela + "' AND Id = " + cheie.ToString()).FirstOrDefault();
                        byte[] fis = (byte[])dr["Fisier"];
                        MemoryStream stream = new MemoryStream(fis);
                        string extensie = dr["FisierExtensie"].ToString();
                        HttpContext.Current.Response.Clear();
                        MemoryStream ms = stream;

                        if (extensie.EndsWith(".pdf"))
                            HttpContext.Current.Response.ContentType = "application/pdf";
                        else if (extensie.EndsWith(".xlsx"))
                            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        else if (extensie.EndsWith(".xls"))
                            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                        else if (extensie.EndsWith(".docx"))
                            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        else if (extensie.EndsWith(".doc"))
                            HttpContext.Current.Response.ContentType = "application/msword";
                        else if (extensie.EndsWith(".zip"))
                            HttpContext.Current.Response.ContentType = "application/zip";
                        else if (extensie.EndsWith(".gif"))
                            HttpContext.Current.Response.ContentType = "image/gif";
                        else if (extensie.EndsWith(".tiff"))
                            HttpContext.Current.Response.ContentType = "image/tiff";
                        else if (extensie.EndsWith(".bmp"))
                            HttpContext.Current.Response.ContentType = "image/bmp";
                        else if (extensie.EndsWith(".png"))
                            HttpContext.Current.Response.ContentType = "image/png";
                        else if (extensie.EndsWith(".htm"))
                            HttpContext.Current.Response.ContentType = "text/html";
                        else if (extensie.EndsWith(".html"))
                            HttpContext.Current.Response.ContentType = "text/html";
                        else if (extensie.EndsWith(".txt"))
                            HttpContext.Current.Response.ContentType = "text/plain";
                        else if (extensie.EndsWith(".xml"))
                            HttpContext.Current.Response.ContentType = "text/xml";
                        else if (extensie.EndsWith(".msg"))
                        {
                            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=test.msg");
                        }
                        else
                            HttpContext.Current.Response.ContentType = "image/jpeg";

                        HttpContext.Current.Response.BinaryWrite(fis);
                        HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.ASCII;
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + (dt.Rows[0]["FisierNume"] ?? "").ToString());
                        HttpContext.Current.Response.Buffer = true;
                        ms.WriteTo(HttpContext.Current.Response.OutputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string NumarLuniContract(decimal F10003, string F100985, DateTime F100933, DateTime F100934, int zileContractCurent)
        {
            string mesaj = "";

            try
            {

                string diff = "DATEDIFF(day, a.F09505, a.F09506)";
                string cond = "CONVERT(DATETIME, a.F09505, 103) != CONVERT(DATETIME, '" + F100933.Day.ToString().PadLeft(2, '0') + "/" + F100933.Month.ToString().PadLeft(2, '0') + "/" + F100933.Year.ToString()
                    + "', 103) or CONVERT(DATETIME, a.F09506, 103) != CONVERT(DATETIME, '" + F100934.Day.ToString().PadLeft(2, '0') + "/" + F100934.Month.ToString().PadLeft(2, '0') + "/" + F100934.Year.ToString() + "', 103)";
                if (Constante.tipBD == 2)
                {
                    diff = "TO_DATE(a.F09506, 'dd/mm/yyyy') -  TO_DATE(a.F09505, 'dd/mm/yyyy')";
                    cond = "TO_DATE(a.F09505, 'dd/mm/yyyy') != TO_DATE('" + F100933.Day.ToString().PadLeft(2, '0') + "/" + F100933.Month.ToString().PadLeft(2, '0') + "/" + F100933.Year.ToString()
                                        + "', 'dd/mm/yyyy') or TO_DATE(a.F09506, 'dd/mm/yyyy') != TO_DATE('" + F100934.Day.ToString().PadLeft(2, '0') + "/" + F100934.Month.ToString().PadLeft(2, '0') + "/" + F100934.Year.ToString() + "', 'dd/mm/yyyy')";

                }

                string sql = "SELECT F09505 as start, " + diff + " + 1 as days FROM F095 a WHERE a.F09503 = " + F10003 + " and a.F09504 = " + F100985 + " AND (" + cond + ") GROUP BY a.F09502, a.F09503, a.F09504, a.F09505, a.F09506";

                DataTable dt = General.IncarcaDT(sql, null);
                int daysInF095 = 0;
                DateTime dataAngajarii = F100933;

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                        daysInF095 += Convert.ToInt32(dt.Rows[i]["days"].ToString());
                    dataAngajarii = Convert.ToDateTime(dt.Rows[0]["start"].ToString());
                }

                DateTime endDate = dataAngajarii.AddDays(daysInF095 + zileContractCurent);

                int days = -99;
                int months = -99;
                int years = -99;

                DateDiff(endDate, dataAngajarii, out years, out months, out days);

                months = months + (years * 12) + Convert.ToInt32(days / 31);

                if (months > 36)
                {
                    mesaj = "0Contractul in derulare are " + months + " luni si " + days + " zile. Durata maxima a unui contract pe perioada determinata nu poate depasi 36 de luni!";
                }
                else
                {
                    DateTime dtAng = dataAngajarii.AddMonths(36);

                    days = -99;
                    months = -99;
                    years = -99;

                    DateDiff(dtAng, endDate, out years, out months, out days);
                    months = months + (years * 12);

                    mesaj = "1Mai puteti prelungi acest contract pe maxim " + months + " luni si " + days + " zile";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        public static void DateDiff(DateTime endDate, DateTime startDate, out int years, out int months, out int days)
        {
            days = endDate.Day - startDate.Day;
            months = endDate.Month - startDate.Month;
            years = endDate.Year - startDate.Year;

            if (days < 0)
            {
                days += DateTime.DaysInMonth(endDate.Year, endDate.Month);
                months--;
            }

            if (months < 0)
            {
                months += 12;
                years--;
            }
        }

        public static DateTime getDataNasterii(string cnp)
        {

            string luna = cnp.Substring(3, 2);
            string ziua = cnp.Substring(5, 2);

            int an = 0;
            int tempAn = Convert.ToInt16(cnp.Substring(1, 2));

            switch (cnp[0])
            {
                case '1':
                case '2':  
                    an = 1900 + tempAn;
                    break;
                case '3':
                case '4':
                    an = 1800 + tempAn;
                    break;
                case '5':
                case '6':
                    an = 2000 + tempAn;
                    break;
                case '7':
                case '8':
                    if (Convert.ToInt16(cnp.Substring(1, 2)) >= 30)
                        an = 1900 + tempAn; 
                    else
                        an = 2000 + tempAn;
                    break;
            }
            return new DateTime(Convert.ToInt16(an), Convert.ToInt16(luna), Convert.ToInt16(ziua));
        }

        public static DateTime DamiDataLucru()
        {
            DateTime dt = DateTime.Now;

            try
            {
                DataTable dtTemp = General.IncarcaDT("SELECT * FROM F010", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["F01011"] != null && dtTemp.Rows[0]["F01011"].ToString().Length > 0
                                                    && dtTemp.Rows[0]["F01012"] != null && dtTemp.Rows[0]["F01012"].ToString().Length > 0)
                    dt = new DateTime(Convert.ToInt32(dtTemp.Rows[0]["F01011"].ToString()), Convert.ToInt32(dtTemp.Rows[0]["F01012"].ToString()), 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        public static string NumeLuna(int NrLuna, int tip = 0, string limba = "RO")
        {
            string luna = "Ianuarie";

            try
            {
                if (limba == "EN")
                {
                    switch (NrLuna)
                    {
                        case 1:
                            luna = "January";
                            break;
                        case 2:
                            luna = "February";
                            break;
                        case 3:
                            luna = "March";
                            break;
                        case 4:
                            luna = "April";
                            break;
                        case 5:
                            luna = "May";
                            break;
                        case 6:
                            luna = "June";
                            break;
                        case 7:
                            luna = "July";
                            break;
                        case 8:
                            luna = "August";
                            break;
                        case 9:
                            luna = "September";
                            break;
                        case 10:
                            luna = "October";
                            break;
                        case 11:
                            luna = "November";
                            break;
                        case 12:
                            luna = "December";
                            break;
                        default:
                            luna = "January";
                            break;
                    }
                }

                if (limba == "RO")
                {
                    switch (NrLuna)
                    {
                        case 1:
                            luna = "Ianuarie";
                            break;
                        case 2:
                            luna = "Februarie";
                            break;
                        case 3:
                            luna = "Martie";
                            break;
                        case 4:
                            luna = "Aprilie";
                            break;
                        case 5:
                            luna = "Mai";
                            break;
                        case 6:
                            luna = "Iunie";
                            break;
                        case 7:
                            luna = "Iulie";
                            break;
                        case 8:
                            luna = "August";
                            break;
                        case 9:
                            luna = "Septembrie";
                            break;
                        case 10:
                            luna = "Octombrie";
                            break;
                        case 11:
                            luna = "Noiembrie";
                            break;
                        case 12:
                            luna = "Decembrie";
                            break;
                        default:
                            luna = "Ianuarie";
                            break;
                    }
                }

                switch (tip)
                {
                    case 0:
                        //NOP
                        break;
                    case 1:
                        luna = luna.Substring(0, 3);
                        break;
                }
            }
            catch (Exception)
            {
            }

            return luna;
        }

        public static string ToDataUnivPontaj(DateTime? dt, int tip = 1, int cuTimp = 0)
        {
            string rez = "";

            try
            {
                if (dt != null)
                {
                    string an = dt.Value.Year.ToString();
                    string luna = dt.Value.Month.ToString().PadLeft(2, Convert.ToChar("0"));
                    string zi = dt.Value.Day.ToString().PadLeft(2, Convert.ToChar("0"));
                    string ora = dt.Value.Hour.ToString().PadLeft(2, Convert.ToChar("0"));
                    string min = dt.Value.Minute.ToString().PadLeft(2, Convert.ToChar("0"));
                    string sec = dt.Value.Second.ToString().PadLeft(2, Convert.ToChar("0"));
                    string mask = "DD-MM-YYYY";

                    switch (Constante.tipBD)
                    {
                        case 1:
                            rez = an + "-" + luna + "-" + zi;
                            if (cuTimp == 1) rez += " " + ora + ":" + min + ":" + sec;
                            rez = "'" + rez + "'";
                            break;
                        case 2:
                            rez = zi.ToString().PadLeft(2, '0') + "-" + luna.ToString().PadLeft(2, '0') + "-" + an;
                            if (cuTimp == 1)
                            {
                                mask = "DD-MM-YYYY HH24:MI:SS";
                                rez += " " + ora + ":" + min + ":" + sec;
                            }
                            if (tip == 2) rez = "to_date('" + rez + "','" + mask + "')";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "ToDataUniv");
            }

            return rez;
        }

        public static DataTable ListaTipContract()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));
                table.Rows.Add(1, Dami.TraduCuvant("Contract zilnic"));
                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        //public static DataTable ListaAfisare()
        //{
        //    try
        //    {
        //        DataTable table = new DataTable();
        //        table.Columns.Add("Id", typeof(int));
        //        table.Columns.Add("Denumire", typeof(string));
        //        table.Rows.Add(1, "Trunchiere la ore");
        //        table.Rows.Add(2, "Cu minute");
        //        table.Rows.Add(3, "Cu zecimale");
        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //        return null;
        //    }
        //}

        //public static DataTable ListaRaportare()
        //{
        //    try
        //    {
        //        DataTable table = new DataTable();

        //        table.Columns.Add("Id", typeof(int));
        //        table.Columns.Add("Denumire", typeof(string));

        //        table.Rows.Add(1, "Pe inceput schimb");
        //        table.Rows.Add(2, "Pe sfarsit schimb");

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //        return null;
        //    }
        //}

        public static DataTable ListaVal_uri()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));
                for (int i = 0; i <= 20; i++)
                {
                    table.Rows.Add(i, "Val" + i.ToString());
                }

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable ListaFuri()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));
                for (int i = 1; i <= 60; i++)
                {
                    table.Rows.Add(i, "F" + i.ToString());
                }

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable ListaModVerif()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "intrare");
                table.Rows.Add(2, "iesire");
                table.Rows.Add(3, "intrare si iesire");
                table.Rows.Add(4, "intrare sau iesire");


                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable ListaTipSchimburi()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "un schimb");
                table.Rows.Add(2, "multiple schimburi");

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetPrograme()
        {
            string sql = @"SELECT * FROM ""Ptj_Programe"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Ptj_Programe", "Id");
            return General.IncarcaDT(sql, null);
        }

        //public static DataTable ListaTipPontare()
        //{
        //    try
        //    {
        //        DataTable table = new DataTable();

        //        table.Columns.Add("Id", typeof(int));
        //        table.Columns.Add("Denumire", typeof(string));

        //        table.Rows.Add(1, Dami.TraduCuvant("Pontare automata"));
        //        table.Rows.Add(2, Dami.TraduCuvant("Pontare automata la minim o citire card"));
        //        table.Rows.Add(3, Dami.TraduCuvant("Pontare doar prima intrare si ultima iesire"));
        //        table.Rows.Add(4, Dami.TraduCuvant("Pontare toate intrarile si iesirile"));
        //        table.Rows.Add(5, Dami.TraduCuvant("Pontare prima intrare, ultima iesire - pauze > x minute"));

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //        return null;
        //    }
        //}

        //public static DataTable ListaRotunjirePrgLucru()
        //{
        //    try
        //    {
        //        DataTable table = new DataTable();

        //        table.Columns.Add("Id", typeof(int));
        //        table.Columns.Add("Denumire", typeof(string));

        //        table.Rows.Add(1, "rotunjire la minute");
        //        table.Rows.Add(2, "rotunjire la ora");
        //        table.Rows.Add(3, "trunchiere la ora");
        //        table.Rows.Add(4, "rotunjire la 45 minute");
        //        table.Rows.Add(5, "rotunjire la 10 minute");
        //        table.Rows.Add(6, "rotunjire la 5 minute");

        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //        return null;
        //    }
        //}

        //public static DataTable GetPtj_AliasFOrdonat()
        //{
        //    try
        //    {
        //        return General.IncarcaDT("SELECT \"Coloana\" AS \"Denumire\", CASE WHEN \"Alias\" IS NULL THEN \"Coloana\" ELSE \"Alias\" END AS \"Alias\"  FROM \"Ptj_tblAdmin\" ORDER BY CASE WHEN \"Alias\" IS NULL THEN \"Coloana\" ELSE \"Alias\" END", null);
        //    }

        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //        return null;
        //    }
        //}

        public static DataTable ListaNumere(int valMin, int valMax)
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                for (int i = valMin; i <= valMax; i++)
                    table.Rows.Add(i, i.ToString());

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        public static byte[] ArataFisier(string Tabela, object cheie, out string numeFisier, out string extensie)
        {
            numeFisier = "";
            extensie = "";
            try
            {
                if (cheie != null)
                {
                    string sql = "SELECT * FROM \"tblFisiere\"";
                    DataTable dt = new DataTable();

                    dt = General.IncarcaDT(sql, null);
                    dt.TableName = "tblFisiere";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                    DataRow dr = null;
                    if (dt.Select("Tabela = '" + Tabela + "' AND Id = " + cheie.ToString()).Count() > 0)
                    {
                        dr = dt.Select("Tabela = '" + Tabela + "' AND Id = " + cheie.ToString()).FirstOrDefault();
                        byte[] fis = (byte[])dr["Fisier"];
                        MemoryStream stream = new MemoryStream(fis);
                        extensie = dr["FisierExtensie"].ToString();
                        numeFisier = (dt.Rows[0]["FisierNume"] ?? "").ToString();
                        return fis;
                    }

                }
                return null;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }

        }

        public static DataTable GetF005()
        {
            try
            {
                string sql = @"SELECT * FROM F005 ";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F005", "F00506");
                return General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetF006()
        {
            try
            {
                string sql = @"SELECT * FROM F006 ";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F006", "F00607");
                return General.IncarcaDT("SELECT F00607, F00608  FROM F006 ORDER BY F00608", null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetLocPrescriere()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(0, "---");
                table.Rows.Add(1, "Medic Familie");
                table.Rows.Add(2, "Spital");
                table.Rows.Add(3, "Ambulatoriu");
                table.Rows.Add(4, "CAS");

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetTipConcediu()
        {
            try
            {
                string sql = @"SELECT * FROM MARDEF ";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("MARDEF", "NO");
                return General.IncarcaDT("SELECT NO, NAME FROM MARDEF", null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static DataTable GetCoduriTransfer()
        {
            try
            {
                string sql = "select 0 AS F02104, '---' AS F02105" + (Constante.tipBD == 1 ? "" : " FROM dual ")
                            + " union       "
                            + "select CAST(F02104 AS INT) AS F02104, F02105 from f021 order by f02105";

                return General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static bool IsPropertyExist(dynamic settings, string name)
        {    
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);                     

            return settings.GetType().GetProperty(name) != null;
        }


        public static void SecuritatePersonal(DataList dtList, int idUser)
        {
            List<string> lista = new List<string>();
            //string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
            //                    SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
            //                    FROM ""Securitate"" A
            //                    INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
            //                    WHERE B.""IdUser"" = {0} AND A.""IdForm"" = 'Personal.Lista' 
            //                    UNION
            //                    SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
            //                    FROM ""Securitate"" A
            //                    WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Personal.Lista' ) X
            //                    GROUP BY X.""IdControl"", X.""IdColoana""";
            //strSql = string.Format(strSql, idUser.ToString());

            //DataTable dt = General.IncarcaDT(strSql, null);
            DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
            DataTable dt = new DataTable();
            if (dtSec != null && dtSec.Rows.Count > 0)
            {
                dt = dtSec.Select("IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat'") != null && dtSec.Select("IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat'").Count() > 0 ? dtSec.Select("IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat'").CopyToDataTable() : null;
            }
            else
                return;

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    lista.Add(dt.Rows[i]["IdControl"].ToString());
            }

            if (lista != null && lista.Count > 0)
            {
                foreach (string elem in lista)
                {
                    bool vizibil = true, blocat = false;
                    string[] param = elem.Split('_');
                    SecuritateCtrl(elem, idUser, out vizibil, out blocat);
                    dynamic ctl = dtList.Items[0].FindControl(param[0]);
                    if (ctl != null)
                    {
                        if (!IsPropertyExist(ctl, "ClientVisible"))
                        {
                            ctl.Visible = vizibil;
                        }
                        else
                        {
                            ctl.ClientVisible = vizibil;
                            if (IsPropertyExist(ctl, "ReadOnly"))
                                ctl.ReadOnly = blocat;
                            else
                                ctl.ClientEnabled = !blocat;
                            
                        }
                    }
                    //else
                    //{
                    if (param[0].Length >= 2 && param[0].Substring(0, 2) == "lg")
                    {
                        //HtmlGenericControl ctl1 = dtList.Items[0].FindControl(param[0]) as HtmlGenericControl;
                        dynamic ctl1 = dtList.Items[0].FindControl(param[0]);
                        if (ctl1 != null)
                        {
                            if (!IsPropertyExist(ctl1, "ClientVisible"))
                            {
                                ctl1.Visible = vizibil;
                            }
                            else
                            {
                                ctl1.ClientVisible = vizibil;
                            }
                            HtmlTable ctlTable = dtList.Items[0].FindControl(ctl1.ID + "Table") as HtmlTable;
                            if (ctlTable != null)
                                ctlTable.Visible = vizibil;
                        }
                     }
                    //}
                }
            }
        }

        public static void SecuritatePersonal(ASPxCallbackPanel pnl, int idUser)
        {
            List<string> lista = new List<string>();
            //string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
            //                    SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
            //                    FROM ""Securitate"" A
            //                    INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
            //                    WHERE B.""IdUser"" = {0} AND A.""IdForm"" = 'Personal.Lista' 
            //                    UNION
            //                    SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
            //                    FROM ""Securitate"" A
            //                    WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Personal.Lista' ) X
            //                    GROUP BY X.""IdControl"", X.""IdColoana""";
            //strSql = string.Format(strSql, idUser.ToString());

            //DataTable dt = General.IncarcaDT(strSql, null);
            DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
            DataTable dt = new DataTable();
            if (dtSec != null && dtSec.Rows.Count > 0)
            {
                dt = dtSec.Select("IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat'") != null && dtSec.Select("IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat'").Count() > 0 ? dtSec.Select("IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat'").CopyToDataTable() : null;
            }
            else
                return;

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    lista.Add(dt.Rows[i]["IdControl"].ToString());
            }

            if (lista != null && lista.Count > 0)
            {
                foreach (string elem in lista)
                {
                    bool vizibil = true, blocat = false;
                    string[] param = elem.Split('_');
                    SecuritateCtrl(elem, idUser, out vizibil, out blocat);
                    dynamic ctl = pnl.FindControl(param[0]);
                    if (ctl != null)
                    {
                        if (!IsPropertyExist(ctl, "ClientVisible"))
                        {
                            ctl.Visible = vizibil;
                        }
                        else
                        {
                            ctl.ClientVisible = vizibil;
                            if (IsPropertyExist(ctl, "ReadOnly"))
                                ctl.ReadOnly = blocat;
                            else
                                ctl.ClientEnabled = !blocat;
                        }
                    }
                    //else
                    //{
                        if ((param[0].Length >= 2 && param[0].Substring(0, 2) == "lg") || (param[0].Length >= 3 && param[0].Substring(0, 3) == "lbl"))
                        {
                            dynamic ctl1 = pnl.FindControl(param[0]);
                            if (ctl1 != null)
                            {
                                if (!IsPropertyExist(ctl1, "ClientVisible"))
                                {
                                    ctl1.Visible = vizibil;
                                }
                                else
                                {
                                    ctl1.ClientVisible = vizibil;
                                }
                                HtmlTable ctlTable = pnl.FindControl(ctl1.ID + "Table") as HtmlTable;
                                if (ctlTable != null)
                                    ctlTable.Visible = vizibil;
                            }
                        }
                    //}
                }
            }
        }

        public static void SecuritatePersonal(ListView dtList, int idUser)
        {
            List<string> lista = new List<string>();
            //string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
            //                    SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
            //                    FROM ""Securitate"" A
            //                    INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
            //                    WHERE B.""IdUser"" = {0} AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl"" like '%_I%'
            //                    UNION
            //                    SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
            //                    FROM ""Securitate"" A
            //                    WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl""  like '%_I%') X
            //                    GROUP BY X.""IdControl"", X.""IdColoana""";
            //strSql = string.Format(strSql, idUser.ToString());

            //DataTable dt = General.IncarcaDT(strSql, null);
            DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
            DataTable dt = new DataTable();
            if (dtSec != null && dtSec.Rows.Count > 0)
            {
                dt = dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl like '%_I%'") != null && dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl like '%_I%'").Count() > 0 ? dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl like '%_I%'").CopyToDataTable() : null;
            }
            else
                return;

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    lista.Add(dt.Rows[i]["IdControl"].ToString());
            }

            if (lista != null && lista.Count > 0)
            {
                foreach (string elem in lista)
                {
                    bool vizibil = true, blocat = false;
                    string[] param = elem.Split('_');
                    SecuritateCtrl(elem, idUser, out vizibil, out blocat);
                    dynamic ctl = dtList.Items[0].FindControl(param[0]);
                    if (ctl != null)
                    {
                        if (!IsPropertyExist(ctl, "ClientVisible"))
                        {
                            ctl.Visible = vizibil;
                        }
                        else
                        {
                            ctl.ClientVisible = vizibil;
                            if (IsPropertyExist(ctl, "ReadOnly"))
                                ctl.ReadOnly = blocat;
                            else
                                ctl.ClientEnabled = !blocat;
                        }
                    }
                }
            }
        }

        //Radu 25.02.2020
        public static void SecuritatePersonal(ASPxGridView grDate, string numeTab = "", int idUser = -99, bool editForm = false)
        {
            try
            {
                Control parent = grDate.Parent;
                string nume = parent.TemplateControl.ToString().Replace("ASP.", "").Replace("_ascx", "").Replace("_", ".");

                DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
                DataTable dt = new DataTable();
                if (dtSec != null && dtSec.Rows.Count > 0)
                {
                    if (editForm)
                        dt = dtSec.Select("IdForm = 'Personal.DateAngajat'") != null && dtSec.Select("IdForm = 'Personal.DateAngajat'").Count() > 0 ? dtSec.Select("IdForm = 'Personal.DateAngajat'").CopyToDataTable() : null;
                    else
                        dt = dtSec.Select("IdForm = '" + nume + "'") != null && dtSec.Select("IdForm = '" + nume + "'").Count() > 0 ? dtSec.Select("IdForm = '" + nume + "'").CopyToDataTable() : null;
                }
                else
                    return;

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            if (editForm)
                            {
                                string id = "";
                                if (dr["IdControl"].ToString().Split('_').Length >= 2)
                                    id = dr["IdControl"].ToString().Split('_')[2];
                                dynamic ctl = grDate.FindEditFormTemplateControl(id);
                                if (ctl != null)
                                {
                                    bool vizibil = true, blocat = false;
                                    SecuritateCtrl(numeTab, idUser, out vizibil, out blocat, true);
                                    if (!IsPropertyExist(ctl, "ClientVisible"))
                                    {
                                        ctl.Visible = vizibil;
                                    }
                                    else
                                    {
                                        ctl.ClientVisible = vizibil;
                                        if (IsPropertyExist(ctl, "ReadOnly"))
                                            ctl.ReadOnly = blocat;
                                        else
                                            ctl.ClientEnabled = !blocat;
                                    }
                                }
                            }
                            else
                            {
                                if (dr["IdColoana"].ToString() != "-")
                                {
                                    //ASPxGridView ctl = pag.FindControl(dr["IdControl"].ToString()) as ASPxGridView;
                                    ASPxGridView ctl = grDate;
                                    if (ctl != null)
                                    {
                                        GridViewDataColumn col = ctl.Columns[dr["IdColoana"].ToString()] as GridViewDataColumn;
                                        if (col != null)
                                        {
                                            col.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                            col.ReadOnly = (Convert.ToInt32(dr["Blocat"]) == 1 ? true : false);
                                        }
                                        else
                                        {
                                            //verificam daca sunt butoane in interiorul gridului
                                            GridViewCommandColumn column = ctl.Columns["butoaneGrid"] as GridViewCommandColumn;
                                            GridViewCommandColumnCustomButton button = column.CustomButtons[dr["IdColoana"].ToString()] as GridViewCommandColumnCustomButton;
                                            if (button != null)
                                            {
                                                if (Convert.ToInt32(dr["Vizibil"]) == 1)
                                                    button.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                                                else
                                                    button.Visibility = GridViewCustomButtonVisibility.Invisible;
                                            }
                                            else
                                            {
                                                //Florin 2018.08.16
                                                //atunci este buton BuiltIn al Devexpress-ului
                                                if (dr["IdColoana"].ToString().ToLower() == "btnedit")
                                                {
                                                    column.ShowEditButton = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                                }
                                                if (dr["IdColoana"].ToString().ToLower() == "btnsterge")
                                                {
                                                    column.ShowDeleteButton = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                                }
                                                if (dr["IdColoana"].ToString().ToLower() == "btnnew")
                                                {
                                                    column.ShowNewButtonInHeader = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                                }
                                            }


                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void SecuritateCtrl(string numeTab, int idUser, out bool vizibil, out bool blocat, bool editForm = false)
        {
            vizibil = true;
            blocat = false;
            try
            {

                //string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                //                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                //                FROM ""Securitate"" A
                //                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                //                WHERE B.""IdUser"" = {1} AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl"" = '{0}'
                //                UNION
                //                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                //                FROM ""Securitate"" A
                //                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl"" = '{0}') X
                //                GROUP BY X.""IdControl"", X.""IdColoana""";
                //strSql = string.Format(strSql, numeTab, idUser.ToString());

                //DataTable dt = General.IncarcaDT(strSql, null);
                DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
                DataTable dt = new DataTable();
                if (dtSec != null && dtSec.Rows.Count > 0)
                {
                    if (editForm)
                        dt = dtSec.Select("IdForm = 'Personal.DateAngajat' AND IdControl like 'grDate" + numeTab + "%'") != null && dtSec.Select("IdForm = 'Personal.DateAngajat' AND  IdControl like 'grDate" + numeTab + "%'").Count() > 0 ? dtSec.Select("IdForm = 'Personal.DateAngajat' AND  IdControl like 'grDate" + numeTab + "%'").CopyToDataTable() : null;
                    else
                        dt = dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl='" + numeTab + "'") != null && dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl='" + numeTab + "'").Count() > 0 ? dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl='" + numeTab + "'").CopyToDataTable() : null;
                }
                else
                    return;

                if (dt != null && dt.Rows.Count > 0)
                {
                    vizibil = (Convert.ToInt32(dt.Rows[0]["Vizibil"].ToString()) == 1 ? true : false);
                    blocat = (Convert.ToInt32(dt.Rows[0]["Blocat"].ToString()) == 1 ? true : false);
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static int GetnrInreg()
        {
            string sql = "";
            int id = 0;
            if (Constante.tipBD == 1)
            {
                sql = "SELECT NEXT VALUE FOR NRINREG_ADEV_SEQ;";
                DataTable dt = General.IncarcaDT(sql, null);

                id = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                sql = "select NRINREG_ADEV_SEQ.nextval from dual";
                DataTable dt = General.IncarcaDT(sql, null);
                id = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            return id;
        }

        public static void LogAdeverinta(Reports.AdeverintaMedic dlreport)
        {

            int nrInreg = 0;
            if (HttpContext.Current.Session["NrInregAdev"] != null)
                nrInreg = Convert.ToInt32(HttpContext.Current.Session["NrInregAdev"].ToString());

            string numeFis = HttpContext.Current.Session["User_NumeComplet"].ToString().Replace(' ', '_') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year
                    + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + ".docx";
            dlreport.ExportToDocx(HostingEnvironment.MapPath("~/Temp/") + numeFis);

            object file = File.ReadAllBytes(HostingEnvironment.MapPath("~/Temp/") + numeFis);

            string sql = "INSERT INTO \"tblAdeverinteIstoric\" (\"NrInreg\", \"Data\", \"Fisier\", \"FisierNume\", \"FisierExtensie\", USER_NO, TIME) VALUES ({0}, {1}, @1, '{2}', '{3}', {4}, {1})";
            sql = string.Format(sql, nrInreg, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), numeFis, ".docx", HttpContext.Current.Session["UserId"].ToString());
            General.ExecutaNonQuery(sql, new object[] { file });
            File.Delete(HostingEnvironment.MapPath("~/Temp/") + numeFis);
        }

        public static void LogAdeverinta(Reports.AdeverintaGenerala dlreport)
        {

            int nrInreg = 0;
            if (HttpContext.Current.Session["NrInregAdev"] != null)
                nrInreg = Convert.ToInt32(HttpContext.Current.Session["NrInregAdev"].ToString());

            string numeFis = HttpContext.Current.Session["User_NumeComplet"].ToString().Replace(' ', '_') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year
                    + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + ".docx";
            dlreport.ExportToDocx(HostingEnvironment.MapPath("~/Temp/") + numeFis);

            object file = File.ReadAllBytes(HostingEnvironment.MapPath("~/Temp/") + numeFis);

            string sql = "INSERT INTO \"tblAdeverinteIstoric\" (\"NrInreg\", \"Data\", \"Fisier\", \"FisierNume\", \"FisierExtensie\", USER_NO, TIME) VALUES ({0}, {1}, @1, '{2}', '{3}', {4}, {1})";
            sql = string.Format(sql, nrInreg, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), numeFis, ".docx", HttpContext.Current.Session["UserId"].ToString());
            General.ExecutaNonQuery(sql, new object[] { file });
            File.Delete(HostingEnvironment.MapPath("~/Temp/") + numeFis);
        }

        public static DataTable ListaLuniDesc(int lunaMax = 12)
        {
            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Denumire", typeof(string));

                for (int i = 1; i <= lunaMax; i++)
                {
                    dt.Rows.Add(i, NumeLuna(i));
                }

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        public static DataTable GetAngajati(int idUser, int an, int luna)
        {
            DataTable dt = new DataTable();
            string sql = "";
            try
            {
                string op = "+";
                if (Constante.tipBD == 2)
                    op = "||";

                sql = "SELECT CAST(A.F10003 AS INT) AS F10003, F10008 {0} ' ' {0} F10009 AS \"NumeComplet\", F00406 as \"Filiala\", F00507 AS \"Sectie\", F00608 AS \"Departament\", F71804 AS \"Functia\" FROM "
                            + " \"F100Supervizori\" R "
                            + " JOIN F100 A on R.F10003 = A.F10003"
                            + " JOIN \"BP_Circuit\" K ON (K.\"Super1\" = -1 * R.\"IdSuper\" OR K.\"Super2\" = -1 * R.\"IdSuper\" OR K.\"Super3\" = -1 * R.\"IdSuper\" OR K.\"Super4\" = -1 * R.\"IdSuper\" OR K.\"Super5\" = -1 * R.\"IdSuper\") "
                            + " LEFT JOIN F004 D on A.F10005 = D.F00405 "
                            + " LEFT JOIN F005 E on A.F10006 = E.F00506 "
                            + " LEFT JOIN F006 F on A.F10007 = F.F00607 "
                            + " LEFT JOIN USERS G on A.F10003 = G.F10003 "
                            + " LEFT JOIN F718 X on A.F10071 = X.F71802 "
                            + " WHERE R.\"IdUser\" = {1}  AND F10025 IN (0, 999) "
                            + " GROUP BY A.F10003, F10008 {0} ' ' {0} F10009, F00406, F00507, F00608, F71804"
                            + " UNION "
                            + "SELECT CAST(A.F10003 AS INT) AS F10003, F10008 {0} ' ' {0} F10009 AS \"NumeComplet\", F00406 as \"Filiala\", F00507 AS \"Sectie\", F00608 AS \"Departament\", F71804 AS \"Functia\" FROM "
                            + " F100 A "
                            + " JOIN \"BP_Circuit\" C  ON 1=1 "
                            + " LEFT JOIN F004 D on A.F10005 = D.F00405 "
                            + " LEFT JOIN F005 E on A.F10006 = E.F00506 "
                            + " LEFT JOIN F006 F on A.F10007 = F.F00607 "
                            + " LEFT JOIN USERS G on A.F10003 = G.F10003 "
                            + " LEFT JOIN F718 X on A.F10071 = X.F71802 "
                            + " WHERE C.\"Super1\" = {1}  AND F10025 IN (0, 999) ORDER BY \"NumeComplet\" ";

                sql = string.Format(sql, op, idUser);
                dt = General.IncarcaDT(sql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        public static DataTable ListaAvansLich()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(1, "Avans");
                table.Rows.Add(2, "Lichidare");

                return table;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        public static string AdaugaCerere(int idUser, int f10003, int an, int luna, int tip, decimal sumaNeta, int moneda, decimal curs, decimal totalNet, int avs, string exp)
        {

            int idUrm = -99;
            string strSql = "";

            DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);
            if (an < Convert.ToInt32(dt010.Rows[0][0].ToString()) || (an == Convert.ToInt32(dt010.Rows[0][0].ToString()) && luna < Convert.ToInt32(dt010.Rows[0][1].ToString())))
            {
                return "Nu se pot introduce bonusuri pentru o luna anterioara lunii de salarizare curente!";
            }

            if (an == Convert.ToInt32(dt010.Rows[0][0].ToString()) && luna == Convert.ToInt32(dt010.Rows[0][1].ToString()))
            {
                DataTable entParam = new DataTable();
                if (tip == 1)
                    entParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'ZiBlocareIntroducerePrimaAvans'", null);
                else
                    entParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'ZiBlocareIntroducerePrimaLichidare'", null);

                if (entParam != null && entParam.Rows.Count > 0 && entParam.Rows[0][0] != null && entParam.Rows[0][0].ToString().Length > 0)
                {
                    try
                    {
                        DataTable dtZiBloc = new DataTable();
                        if (tip == 1)
                        {
                            if (Constante.tipBD == 1)
                                dtZiBloc = General.IncarcaDT("SELECT CONVERT(VARCHAR, \"ZiBlocareAvans\", 103) FROM \"DataBlocareBonusuri\"", null);
                            else
                                dtZiBloc = General.IncarcaDT("SELECT TO_CHAR(\"ZiBlocareAvans\", 'dd/mm/yyyy') FROM \"DataBlocareBonusuri\"", null);
                        }
                        else
                        {
                            if (Constante.tipBD == 1)
                                dtZiBloc = General.IncarcaDT("SELECT CONVERT(VARCHAR, \"ZiBlocareLichidare\", 103) FROM \"DataBlocareBonusuri\"", null);
                            else
                                dtZiBloc = General.IncarcaDT("SELECT TO_CHAR(\"ZiBlocareLichidare\", 'dd/mm/yyyy') FROM \"DataBlocareBonusuri\"", null);
                        }
                        if (dtZiBloc != null && dtZiBloc.Rows.Count > 0 && dtZiBloc.Rows[0][0] != null && dtZiBloc.Rows[0][0].ToString().Length > 0)
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(dtZiBloc.Rows[0][0].ToString().Substring(6, 4)), Convert.ToInt32(dtZiBloc.Rows[0][0].ToString().Substring(3, 2)), Convert.ToInt32(dtZiBloc.Rows[0][0].ToString().Substring(0, 2)));
                            if (dt.Date < DateTime.Now.Date)
                            {
                                return "Operatie blocata pentru aceasta luna";
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            idUrm = Convert.ToInt32(Dami.NextId("BP_Prime"));
            DataTable entCir = General.IncarcaDT("SELECT * FROM \"BP_Circuit\" WHERE \"IdTip\" = " + tip + " AND -1 * \"Super1\" IN (SELECT \"IdSuper\" FROM \"F100Supervizori\" WHERE \"IdSuper\" <> 0 AND F10003 = " + f10003 + " AND \"IdUser\" = " + idUser + ")", null);
            if (entCir == null || entCir.Rows.Count <= 0)
            {
                return "Tipul de prima nu are circuit alocat.";
            }


            int idCircuit = -99;
            if (entCir != null) idCircuit = Convert.ToInt32(entCir.Rows[0]["IdAuto"].ToString());

            string culoare = "#FFFFFFFF";
            DataTable entStari = General.IncarcaDT("SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = 1", null);
            if (entStari != null && entStari.Rows.Count > 0 && entStari.Rows[0]["Culoare"] != null && entStari.Rows[0]["Culoare"].ToString().Length > 0)
                culoare = entStari.Rows[0]["Culoare"].ToString();


            int total = 0;
            int idStare = 2;
            int pozUser = 1;

            //aflam totalul de utilizatori din circuit
            for (int i = 1; i <= 20; i++)
            {
                if (entCir != null && entCir.Rows.Count > 0 && entCir.Rows[0]["Super" + i] != null && entCir.Rows[0]["Super" + i].ToString().Length > 0)
                {
                    string idSuper = entCir.Rows[0]["Super" + i].ToString();
                    if (idSuper != null && Convert.ToInt32(idSuper) != -99)
                    {
                        //ne asiguram ca exista user pentru supervizorul din circuit
                        if (Convert.ToInt32(idSuper) < 0)
                        {
                            int idSpr = Convert.ToInt32(idSuper);
                            strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + f10003.ToString() + " AND \"IdSuper\" = " + (-1 * idSpr).ToString();
                            DataTable dtTemp = General.IncarcaDT(strSql, null);
                            if (dtTemp == null || dtTemp.Rows.Count == 0 || dtTemp.Rows[0]["IdUser"] == null || dtTemp.Rows[0]["IdUser"].ToString().Length <= 0)
                            {
                                continue;
                            }
                        }

                        total++;
                    }
                }
            }


            //adaugam istoricul
            int poz = 0;
            int idUserCalc = -99;

            for (int i = 1; i <= 20; i++)
            {
                if (entCir != null && entCir.Rows.Count > 0 && entCir.Rows[0]["Super" + i] != null && entCir.Rows[0]["Super" + i].ToString().Length > 0)
                {
                    var valId = entCir.Rows[0]["Super" + i];
                    if (valId != null && Convert.ToInt32(valId.ToString()) != -99)
                    {
                        int usr = Convert.ToInt32(valId.ToString());
                        culoare = "#FFFFFFFF";
                        entStari = General.IncarcaDT("SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" =  " + idStare, null);
                        if (entStari != null && entStari.Rows.Count > 0 && entStari.Rows[0]["Culoare"] != null && entStari.Rows[0]["Culoare"].ToString().Length > 0)
                            culoare = entStari.Rows[0]["Culoare"].ToString();


                        //IdUser
                        if (Convert.ToInt32(valId) == 0)
                        {
                            DataTable entUtiliz = General.IncarcaDT("SELECT * FROM USERS WHERE F10003 = " + f10003, null);
                            if (entUtiliz != null && entUtiliz.Rows.Count > 0 && entUtiliz.Rows[0]["F70102"] != null && entUtiliz.Rows[0]["F70102"].ToString().Length > 0)
                            {
                                idUserCalc = Convert.ToInt32(entUtiliz.Rows[0]["F70102"].ToString());
                            }
                        }
                        if (Convert.ToInt32(valId) > 0) idUserCalc = Convert.ToInt32(valId);
                        if (Convert.ToInt32(valId) < 0)
                        {
                            int idSpr = Convert.ToInt32(valId);
                            //ne asiguram ca exista user pentru supervizorul din circuit
                            strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + f10003.ToString() + " AND \"IdSuper\" = " + (-1 * idSpr).ToString();
                            DataTable dtTemp = General.IncarcaDT(strSql, null);
                            if (dtTemp == null || dtTemp.Rows.Count == 0 || dtTemp.Rows[0]["IdUser"] == null || dtTemp.Rows[0]["IdUser"].ToString().Length <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["IdUser"].ToString());
                            }
                        }

                        poz += 1;

                        //starea
                        if (idUserCalc == idUser)
                        {
                            pozUser = poz;
                            if (poz == 1) idStare = 1;
                            if (poz == total) idStare = 3;


                        }


                        strSql = "INSERT INTO \"BP_Istoric\" (\"Id\", \"IdCircuit\", \"IdSuper\", \"Pozitie\", USER_NO, TIME, \"Inlocuitor\", \"IdUser\", \"Aprobat\", \"DataAprobare\", \"IdStare\", \"Culoare\")"
                                + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 0, {6}, {7}, {8}, {9}, {10})";
                        strSql = string.Format(strSql, idUrm, idCircuit, valId.ToString(), poz, idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idUserCalc,
                                (idUser == idUserCalc ? "1" : "NULL"), (idUser == idUserCalc ? (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") : "NULL"), (idUser == idUserCalc ? idStare.ToString() : "NULL"),
                                (idUser == idUserCalc ? "'" + culoare + "'" : "NULL"));
                        General.ExecutaNonQuery(strSql, null);
                    }
                }
            }

            culoare = "#FFFFFFFF";
            entStari = General.IncarcaDT("SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" =  " + idStare, null);
            if (entStari != null && entStari.Rows.Count > 0 && entStari.Rows[0]["Culoare"] != null && entStari.Rows[0]["Culoare"].ToString().Length > 0)
                culoare = entStari.Rows[0]["Culoare"].ToString();


            //adaugam headerul
            string sql = "INSERT INTO \"BP_Prime\" (F10003, \"An\", \"Luna\", \"IdTip\", \"SumaNeta\", \"IdMoneda\", \"Curs\", \"TotalNet\", \"AvansLichidare\", \"Explicatie\", USER_NO, TIME, \"UserIntrod\", \"IdCircuit\", \"IdStare\", \"Culoare\", \"TotalCircuit\", \"Pozitie\", \"Id\") "
                        + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11}, {10}, {12}, {13}, '{14}', {15}, {16}, {17}) ";
            sql = string.Format(sql, f10003, an, luna, tip, sumaNeta.ToString(new CultureInfo("en-US")), moneda, curs, totalNet.ToString(new CultureInfo("en-US")), avs, exp, idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idCircuit, idStare, culoare, total, pozUser, idUrm);
            General.ExecutaNonQuery(sql, null);




            Notif.TrimiteNotificare("BP.CrearePrime", 1, $@"SELECT Z.*, 2 AS ""Actiune"", {idStare} AS ""IdStareViitoare"" FROM ""BP_Prime"" Z WHERE ""Id""=" + idUrm, "", idUrm, idUser, f10003);
            return "";

        }

        public static bool VerificaCNP(string cnp)
        {
            //Un CNP este alcatuit astfel :
            //|S| |AA| |LL| |ZZ| |JJ| |ZZZ| |C|
            //|_| |__| |__| |__| |__| |___| |_|
            //C – Cifra de control
            //ZZZ - Numarul de ordine atribuit persoanelor
            //JJ - Codul judetului
            //ZZ - Ziua nasterii
            //LL – Luna nasterii
            //AA – Anul nasterii
            //S – Cifra sexului (M/F) pentru:
            //1/2 – cetateni romani nascuti intre 1 ian 1900 si 31 dec 1999
            //3/4 – cetateni romani nascuti intre 1 ian 1800 si 31 dec 1899
            //5/6 – cetateni romani nascuti intre 1 ian 2000 si 31 dec 2099
            //7/8 – rezidenti
            //Persoanele de cetatenie straina se identifica cu cifra “9″
            //Algoritmul de validare al unui cod CNP
            //Pas preliminar: Se testeaza daca codul respecta formatul unui cod CNP. Adica prima cifra sa fie cuprinsa in intervalul 1 – 6 sau sa fie 9 pentru straini. Urmatoarele sase cifre trebuie sa constituie o data calendaristica valida in formatul AALLZZ.
            //Pas 1: Se foloseste cheia de testare “279146358279″. Primele douasprezece cifre se inmultesc pe rand de la stanga spre dreapta cu cifra corespunzatoare din cheia de testare.
            //Pas 2: Cele douasprezece produse obtinute se aduna si suma obtinuta se imparte la 11.
            //    Daca restul impartirii la 11 este mai mic ca 10, atunci acesta va reprezenta cifra de control.
            //    Daca restul impartirii este 10 atunci cifra de control este 1.
            //Pentru un CNP valid cifra de control va trebui sa coincida cu cifra de pe pozitia treisprezece din CNP-ul initial.

            try
            {
                if (cnp.Length != 13)
                    return false;

                const string a = "279146358279";

                long j = 0;

                if (!long.TryParse(cnp, out j))
                    return false;

                long suma = 0;

                for (int i = 0; i < 12; i++)
                    suma += long.Parse(cnp.Substring(i, 1)) * long.Parse(a.Substring(i, 1));

                long rest = suma - 11 * (int)(suma / 11);

                rest = rest == 10 ? 1 : rest;

                if (long.Parse(cnp.Substring(12, 1)) != rest)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return false;
            }
        }

        public static string SelectOracle(string tabela, string coloana)
        {
            string sql = "", sql_tmp = "";

            sql_tmp = "select column_name from cols where table_name = '{0}' and column_name <> '{1}' and upper(column_name) not like 'SYS%' order by column_name";
            sql_tmp = string.Format(sql_tmp, tabela, coloana);
            DataTable dt = General.IncarcaDT(sql_tmp, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                sql += " SELECT ";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql += "\"" + dt.Rows[i]["column_name"].ToString() + "\"";
                    if (i < dt.Rows.Count - 1)
                        sql += ", ";
                    else
                        sql += ", CAST(\"" + coloana + "\" AS INT) AS \"" + coloana + "\" from \"" + tabela + "\"";
                }
            }

            return sql;
        }

        public static string SelectListaCampuriOracle(string tabela, string coloana)
        {
            string sql = "", sql_tmp = "";

            sql_tmp = "select column_name from cols where table_name = '{0}' and column_name <> '{1}' and upper(column_name) not like 'SYS%' order by column_name";
            sql_tmp = string.Format(sql_tmp, tabela, coloana);
            DataTable dt = General.IncarcaDT(sql_tmp, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql += "\"" + dt.Rows[i]["column_name"].ToString() + "\"";
                    if (i < dt.Rows.Count - 1)
                        sql += ", ";
                    else
                        sql += ", CAST(\"" + coloana + "\" AS INT) AS \"" + coloana + "\" ";
                }
            }

            return sql;
        }

        public static string SelectOraclePersonal(string marca)
        {
            string sql = " select * from cols where table_name = 'F100' and upper(column_name) not like 'SYS%' order by column_name";
            DataTable dt100 = IncarcaDT(sql, null);
            sql = " select * from cols where table_name ='F1001' and upper(column_name) != 'USER_NO' and upper(column_name) != 'TIME' and upper(column_name) != 'F10003' and upper(column_name) != 'F10001' and upper(column_name) != 'F10002' and upper(column_name) not like 'SYS%' order by column_name";
            DataTable dt1001 = IncarcaDT(sql, null);

            sql = "SELECT ";
            for (int i = 0; i < dt100.Rows.Count; i++)
            {
                if (dt100.Rows[i]["DATA_TYPE"].ToString() == "NUMBER" && dt100.Rows[i]["DATA_PRECISION"] != null && dt100.Rows[i]["DATA_PRECISION"].ToString().Length > 0 &&
                    Convert.ToInt32(dt100.Rows[i]["DATA_PRECISION"].ToString()) <= 10 && (dt100.Rows[i]["DATA_SCALE"] == null || dt100.Rows[i]["DATA_SCALE"].ToString().Length <= 0
                    || Convert.ToInt32(dt100.Rows[i]["DATA_SCALE"].ToString()) == 0))
                    sql += " CAST(F100.\"" + dt100.Rows[i]["COLUMN_NAME"].ToString() + "\" AS NUMBER(9)) AS \"" + dt100.Rows[i]["COLUMN_NAME"].ToString() + "\", ";
                else
                    sql += " F100.\"" + dt100.Rows[i]["COLUMN_NAME"].ToString() + "\", ";
            }
            for (int i = 0; i < dt1001.Rows.Count; i++)
            {
                if (dt1001.Rows[i]["DATA_TYPE"].ToString() == "NUMBER" && dt1001.Rows[i]["DATA_PRECISION"] != null && dt1001.Rows[i]["DATA_PRECISION"].ToString().Length > 0 &&
                    Convert.ToInt32(dt1001.Rows[i]["DATA_PRECISION"].ToString()) <= 10 && (dt1001.Rows[i]["DATA_SCALE"] == null || dt1001.Rows[i]["DATA_SCALE"].ToString().Length <= 0
                    || Convert.ToInt32(dt1001.Rows[i]["DATA_SCALE"].ToString()) == 0))
                    sql += " CAST(F1001.\"" + dt1001.Rows[i]["COLUMN_NAME"].ToString() + "\" AS NUMBER(9)) AS \"" + dt1001.Rows[i]["COLUMN_NAME"].ToString() + "\" ";
                else
                    sql += " F1001.\"" + dt1001.Rows[i]["COLUMN_NAME"].ToString() + "\" ";
                if (i < dt1001.Rows.Count - 1)
                    sql += ", ";
            }
            sql += " FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = " + marca;

            return sql;
        }

        public static DataTable GetSablon()
        {
            string op = "+";
            if (Constante.tipBD == 2) op = "||";

            string sql = "SELECT F09903 AS \"Id\", F09908 " + op + " ' ' " + op + " F09909 AS \"Denumire\" FROM F099";
            DataTable dt = General.IncarcaDT(sql, null);

            return dt;
        }

        public static DataTable GetLocatieInt()
        {
            string sql = @"SELECT * FROM LOCATII ORDER BY LOCATIE";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("LOCATII", "NUMAR") + " ORDER BY LOCATIE";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetNivelHay()
        {
            string sql = @"SELECT * FROM ""Org_tblNivelHay"" ORDER BY ""Id""";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Org_tblNivelHay", "Id") + " ORDER BY \"Id\"";
            return General.IncarcaDT(sql, null);
        }

        public static DataTable GetCategTarife(string data)
        {
            DataTable table = new DataTable();
            string cmpData = "";
            if (data != null && data.Length > 0)
            {
                DateTime dt = Convert.ToDateTime(data);
                string dataRef = dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString();
                if (Constante.tipBD == 2)
                {
                    cmpData = " AND  F01118 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F01119 ";
                }
                else
                {
                    cmpData = " AND F01118 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F01119 ";
                }
            }

            try
            {
                string sql = @"SELECT * FROM F011 WHERE F01105 = 1 " + cmpData + " ORDER BY F01107";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F011", "F01104") + " WHERE F01105 = 1 " + cmpData + " ORDER BY F01107";
                table = IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return table;
        }

        public static DataTable GetTarife(string categ, string data)
        {
            DataTable table = new DataTable();

            string cmpData = "";
            if (data != null && data.Length > 0)
            {
                DateTime dt = Convert.ToDateTime(data);
                string dataRef = dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString();
                if (Constante.tipBD == 2)
                {
                    cmpData = " AND  F01118 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F01119 ";
                }
                else
                {
                    cmpData = " AND F01118 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F01119 ";
                }
            }

            try
            {
                if (categ != null && categ.Length > 0)
                {
                    string sql = @"SELECT 0 AS F01105, '---' AS F01107 UNION SELECT F01105, F01107 FROM F011  WHERE F01104 = " + categ + cmpData + " ORDER BY F01107";
                    if (Constante.tipBD == 2)
                        sql = "SELECT 0 AS F01105, '---' AS F01107 FROM DUAL UNION " + General.SelectOracle("F011", "F01105") + " WHERE F01104 = " + categ + cmpData + " ORDER BY F01107";
                    table = IncarcaDT(sql, null);
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return table;
        }

        public static DataTable GetTarifeSp(string categ, string data)
        {
            DataTable table = new DataTable();

            try
            {

                string cmpData = "";
                if (data != null && data.Length > 0)
                {
                    DateTime dt = Convert.ToDateTime(data);
                    string dataRef = dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString();
                    if (Constante.tipBD == 2)
                    {
                        cmpData = " AND  F01118 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F01119 ";
                    }
                    else
                    {
                        cmpData = " AND F01118 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F01119 ";
                    }
                }

                string sql = @"SELECT 0 AS F01105, '---' AS F01107 UNION SELECT F01105, F01107 FROM F011  WHERE F01104 = (  select distinct f01104 from f025
                                left join f021 on f02510 = f02104
                                left join f011 on f02106 = f01104
                                where  f02504 = " + categ + cmpData + @") ORDER BY F01107";
                if (Constante.tipBD == 2)
                    sql = "SELECT 0 AS F01105, '---' AS F01107 FROM DUAL UNION " + General.SelectOracle("F011", "F01105") + " WHERE F01104 = (  select distinct f01104 from f025 "
                               + " left join f021 on f02510 = f02104 "
                               + " left join f011 on f02106 = f01104 "
                               + " where f02504 = " + categ + ") ORDER BY F01107";
                table = IncarcaDT(sql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return table;
        }


        public static DataTable GetSporuri(string param, string data)
        {
            string cmpData = "";
            if (data != null && data.Length > 0)
            {
                DateTime dt = Convert.ToDateTime(data);
                string dataRef = dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString();
                if (Constante.tipBD == 2)
                {
                    cmpData = " AND  F02524 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F02525 ";
                }
                else
                {
                    cmpData = " AND F02524 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F02525 ";
                }
            }

            string sql = @"SELECT 0 AS F02504, '---' AS F02505 UNION  SELECT F02504, F02505 FROM F025  WHERE " + (param == "0" ? " (F02526 IS NULL OR F02526 = 0) " : " F02526 = 1 ") + cmpData + " ORDER BY F02505" ;
            if (Constante.tipBD == 2)
                sql = " SELECT 0 AS F02504, '---' AS F02505 FROM DUAL UNION " +  General.SelectOracle("F025", "F02504") + " WHERE " + (param == "0" ? " (F02526 IS NULL OR F02526 = 0) " : " F02526 = 1 ") + cmpData + " ORDER BY F02505";
            return General.IncarcaDT(sql, null);
        }

        //end Radu

        public static string SelectInlocuitori(int f10003, DateTime? dtINc, DateTime? dtSf)
        {

            string strSql = "";
            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";


                if (f10003 == -55)
                {
                    return $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet""
                            FROM F100 A 
                            WHERE (A.F10025 = 0 OR A.F10025 = 999)";
                }

                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"", 
                                        A.F100901, C.F71804 AS ""Functia"",
                                        A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"", A.F10023 AS ""DataPlecarii"", A.F10011 AS ""NrContract"", 
                                        CASE WHEN A.F10022 <= {CurrentDate()} AND {CurrentDate()} <= A.F10023 THEN 1 ELSE 0 END AS ""Stare"", A.{salariu} AS ""SalariulBrut"", A.F10025, A.F100571 AS ""Regiune"",
                                        (SELECT MIN(Z.F70102) FROM USERS Z WHERE Z.F10003=A.F10003) AS ""IdUser""
                                        FROM ""InlocuitoriAngajat"" B
                                        INNER JOIN F100 A ON B.""F10003Inlocuitor"" = A.F10003
                                        LEFT JOIN F002 E ON A.F10002 = E.F00202
                                        LEFT JOIN F003 F ON A.F10004 = F.F00304 
                                        LEFT JOIN F004 G ON A.F10005 = G.F00405 
                                        LEFT JOIN F005 H ON A.F10006 = H.F00506 
                                        LEFT JOIN F006 I ON A.F10007 = I.F00607 
                                        LEFT JOIN F718  C ON A.F10071 = C.F71802
                                        WHERE B.F10003={f10003} AND A.F10003 <> {f10003} AND (A.F10025 = 0 OR A.F10025 = 999)";

                switch (Convert.ToInt32(HttpContext.Current.Session["IdClient"]))
                {
                    case 13:                    //Asirom
                        {

                            string strLst = $@"SELECT X.F10003 
                                                FROM ""Ptj_tblInlocuitori"" X
                                                INNER JOIN F100 A ON X.F10003=A.F10003
                                                INNER JOIN A.F10003=B.F10003
                                                INNER JOIN F100 C ON X.F10005=C.F10005 AND C.F10003={HttpContext.Current.Session["User_Marca"]}
                                                WHERE B.F1001004=4";

                            int nrLst = Convert.ToInt32(General.ExecutaScalar("SELECT COUNT(*) FROM (" + strLst + ") Z", null));
                            if (nrLst == 0)
                            {
                                strLst = $@"SELECT A.F10003 FROM F100 A 
                                            INNER JOIN F100 B ON A.F10005=B.F10005 AND B.F10003={HttpContext.Current.Session["User_Marca"]}
                                            INNER JOIN F005 C ON C.F00506 = A.F10006
                                            WHERE C.F00508='770'";
                                int nr770 = Convert.ToInt32(General.ExecutaScalar("SELECT COUNT(*) FROM (" + strLst + ") Z", null));
                                if (nr770 == 0)
                                {
                                    strLst = $@"SELECT A.F10003 FROM F100 A 
                                                INNER JOIN F100 B ON A.F10006=B.F10006 AND B.F10003={HttpContext.Current.Session["User_Marca"]}";
                                }
                            }

                            strSql += $@" AND A.F10003 IN ({strLst}) ORDER BY A.F10008, A.F10009";
                        }
                        break;
                    case 34:                    //Groupama
                        {
                            if (VarSession("User_Marca").ToString() != "-1")
                            {
                                if (VarSession("User_Dept").ToString().ToLower().Substring(0, 2) == "ag") strSql += $@" AND I.F00608 = '{VarSession("User_Dept")}' ORDER BY A.F10008, A.F10009";
                            }
                            else
                            {
                                if (dtINc == null || dtSf == null) return strSql;
                                string strLst = $@"SELECT A.F10003 FROM ""Ptj_Cereri"" WHERE {TruncateDate("A.DataInceput")} <= {ToDataUniv(dtSf)} AND {ToDataUniv(dtINc)} <= {TruncateDate("A.DataSfarsit")} AND A.""IdStare"" IN (1,2,3)";
                                strSql += $@" AND A.F10003 NOT IN ({strLst}) ORDER BY A.F10008, A.F10009";
                            }
                        }
                        break;
                    default:
                        {
                            strSql += " ORDER BY A.F10008, A.F10009";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;

        }

        public static void InitSessionVariables()
        {
            try
            {
                //Lista cu toate variabilele de sesiune din aplicatie
                HttpContext.Current.Session["SecApp"] = "NO";
                HttpContext.Current.Session["tblParam"] = "";
                HttpContext.Current.Session["tmpMeniu"] = "";
                HttpContext.Current.Session["Titlu"] = "";
                HttpContext.Current.Session["Sablon_Tabela"] = "";
                HttpContext.Current.Session["Sablon_CheiePrimara"] = "";
                HttpContext.Current.Session["Sablon_TipActiune"] = "";
                HttpContext.Current.Session["InformatiaCurenta"] = "";
                HttpContext.Current.Session["SecuritateCamp"] = "";
                HttpContext.Current.Session["SecuritateDate"] = "";
                HttpContext.Current.Session["PaginaWeb"] = "";
                HttpContext.Current.Session["TipNotificare"] = "";
                HttpContext.Current.Session["Date_Profile"] = "";
                HttpContext.Current.Session["ProfilId"] = "";
                HttpContext.Current.Session["Profil_Original"] = "";
                HttpContext.Current.Session["AbsDescValues"] = "";

                HttpContext.Current.Session["Cereri_Absente_Absente"] = "";
                HttpContext.Current.Session["Cereri_Absente_Angajati"] = "";


                HttpContext.Current.Session["formatDataSistem"] = CultureInfo.CurrentCulture;

                HttpContext.Current.Session["IdLimba"] = "RO";
                HttpContext.Current.Session["IdLimba_Veche"] = "RO";

                HttpContext.Current.Session["User"] = "";
                HttpContext.Current.Session["UserId"] = -99;
                HttpContext.Current.Session["User_NumeComplet"] = "";
                HttpContext.Current.Session["User_Marca"] = -99;
                HttpContext.Current.Session["User_IdDept"] = -99;
                HttpContext.Current.Session["User_Dept"] = "";
                HttpContext.Current.Session["User_CNP"] = "";

                HttpContext.Current.Session["EsteAdmin"] = 0;
                HttpContext.Current.Session["SchimbaParola"] = 0;
                HttpContext.Current.Session["EsteInGrup99"] = 0;
                HttpContext.Current.Session["ParolaComplexa"] = 0;

                HttpContext.Current.Session["PrimaIntrare"] = 0;

                HttpContext.Current.Session["IdMaxValue"] = 1;

                HttpContext.Current.Session["Ptj_IstoricVal"] = "";

                HttpContext.Current.Session["Filtru_PontajulEchipei"] = "";

                HttpContext.Current.Session["IdClient"] = "1";

                HttpContext.Current.Session["PontajulAreCC"] = "0";

                //nu se initializeaza; este pusa aici pentru documentare
                //HttpContext.Current.Session["NrIncercari"] = 0;

                HttpContext.Current.Session["SecAuditSelect"] = "0";
                HttpContext.Current.Session["SecCriptare"] = "0";

                //Florin 2019.06.04
                HttpContext.Current.Session["MP_NuPermiteCNPInvalid"] = "1";
                HttpContext.Current.Session["MP_AreContract"] = "0";
                HttpContext.Current.Session["MP_DataSfarsit36"] = "01/01/2100";

                //Florin 2019.06.21
                HttpContext.Current.Session["EsteTactil"] = "0";
                HttpContext.Current.Session["TimeOutSecunde"] = 0;

                HttpContext.Current.Session["NumeGriduri"] = "";
                HttpContext.Current.Session["IdGrid"] = "1";

                //Florin 2019.07.15
                HttpContext.Current.Session["Filtru_ActeAditionale"] = "{}";

                //Florin 2019.07.17
                HttpContext.Current.Session["Filtru_CereriAbs"] = "";

                //Florin 2019.07.19
                HttpContext.Current.Session["Ptj_DataBlocare"] = "22001231";

                //Florin 2019.10.16
                HttpContext.Current.Session["Json_Programe"] = "[]";

                //Florin 2020.01.03
                HttpContext.Current.Session["Eval_tblCategorieObiective"] = null;

                //Radu 15.05.2020
                HttpContext.Current.Session["TipInfoChiosc"] = 0;

                //Florin 2020.08.11
                HttpContext.Current.Session["ZileLibere"] = "";

                //Florin 2020.08.18
                HttpContext.Current.Session["FisiereDeSters"] = "";

                //Florin 2020.09.08
                HttpContext.Current.Session["TemplateIdObiectiv"] = 1;
                HttpContext.Current.Session["TemplateIdCompetenta"] = 1;
                HttpContext.Current.Session["QuizIntrebari_Id"] = 1;

                //Florin 2021.06.02  #909
                HttpContext.Current.Session["tmpMeniu2"] = "";
                HttpContext.Current.Session["tmpMeniu3"] = "";

                string ti = "nvarchar";
                if (Constante.tipBD == 2) ti = "varchar2";

                string strSql = @"SELECT ""Nume"", ""Valoare"", ""Explicatie"", ""IdModul"", ""Criptat"" FROM ""tblParametrii""
                                UNION
                                SELECT 'AnLucru', CAST(F01011 AS {0}(10)), '', 1, 0 FROM F010
                                UNION
                                SELECT 'LunaLucru', CAST(F01012 AS {0}(10)), '', 1, 0 FROM F010";
                strSql = string.Format(strSql, ti);

                HttpContext.Current.Session["tblParam"] = General.IncarcaDT(strSql, null);
                HttpContext.Current.Session["IdClient"] = Convert.ToInt32(Dami.ValoareParam("IdClient", "1"));
                HttpContext.Current.Session["PontajulAreCC"] = General.Nz(Dami.ValoareParam("PontajulAreCC"),"0");


                //Florin 2018.11.13
                if (HttpContext.Current != null && HttpContext.Current.Session["tblParam"] != null)
                {
                    DataTable dt = HttpContext.Current.Session["tblParam"] as DataTable;
                    if (dt != null)
                    {
                        DataRow[] arr1 = dt.Select("Nume='SecAuditSelect'");
                        if (arr1.Count() > 0)
                            HttpContext.Current.Session["SecAuditSelect"] = arr1[0];

                        DataRow[] arr2 = dt.Select("Nume='SecCriptare'");
                        if (arr2.Count() > 0)
                            HttpContext.Current.Session["SecCriptare"] = arr2[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static object VarSession(string numeVar)
        {
            object obj = null;

            try
            {
                if (General.Nz(HttpContext.Current.Session[numeVar],"").ToString() != "")
                    obj = HttpContext.Current.Session[numeVar];
                else
                    obj = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return obj;
        }

        public static string SqlCuSelectieToate(string select)
        {
            string strSql = select;
            string cond = Constante.tipBD == 2 ? " FROM DUAL " : "";
            try
            {
                strSql = $@"SELECT * FROM 
                            (
                            SELECT -13 AS ""Id"", 'Toate' AS ""Denumire"", 0 AS ""Ordin"" {cond}
                            UNION
                            {strSql}
                            ) Y 
                            ORDER BY ""Ordin"" ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql; 
        }

        public static string FiltruActivi(int an, int luna, int zi = 0)
        {
            string strSql = "";

            try
            {
                string dtInc = an + "-" + luna + "-01";
                string dtSf = an + "-" + luna + "-" + DateTime.DaysInMonth(an, luna);

                if (Constante.tipBD == 1)
                {
                    strSql = " AND F10022 <= '" + dtSf + "' AND '" + dtInc + "' <= F100993 ";
                    if (zi > 0 && zi <= 31)
                    {
                        string dt = an + "-" + luna + "-" + zi;
                        strSql = " AND F10022 <= '" + dt + "' AND '" + dt + "' <= F100993 ";
                    }
                }
                else
                {
                    dtInc = "01-" + luna.ToString().PadLeft(2, '0') + "-" + an.ToString().Substring(2);
                    dtSf = DateTime.DaysInMonth(an, luna) + "-" + luna.ToString().PadLeft(2,'0') + "-" + an.ToString();

                    strSql = " AND TRUNC(to_date('" + dtSf + "','DD-MM-RRRR') - F10022)>=0 AND TRUNC(COALESCE(A.F100993,TO_DATE('01-01-2101','DD-MM-YYYY')) - to_date('" + dtInc + "','DD-MM-RRRR'))>=0";
                    if (zi > 0 && zi <= 31)
                    {
                        string dt = zi.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + luna.ToString().PadLeft(2, '0') + "-" + an.ToString().Substring(2);
                        strSql = " AND TRUNC(to_date('" + dt + "','DD-MM-RRRR') - F10022)>=0 AND TRUNC(COALESCE(A.F100993,TO_DATE('01-01-2101','DD-MM-YYYY')) - to_date('" + dt + "','DD-MM-RRRR'))>=0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static List<string> ListaCuloareValoare()
        {
            try
            {
                List<string> list = new List<string>();
                //index
                list.Add("#FFDDEEFF");         // 0  -  cand este absenta
                list.Add("#FFFFFF00");         // 1  -  cand este > norma
                list.Add("#FFFF00FF");         // 2  -  cand este < norma
                list.Add("#FF00FFFF");         // 3  -  cand vine din ceasuri
                list.Add("#FFFFF0F0");         // 4  -  cand e modif de user
                list.Add("#FFF0FF0F");         // 5  -  cand vine din cereri
                list.Add("#FFF0DD0F");         // 6  -  cand este modificat de HR

                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static bool DrepturiAprobare(int actiune, int idRol)
        {
            bool rez = true;

            try
            {
                if (idRol == 4 || idRol == -99 || (idRol == 0 && actiune == 0) || (idRol == 1 && actiune == 0))
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie."), MessageBox.icoError, "Eroare !");
                    rez = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static string ActiuniExec(int actiune, int f10003, int idRol, int idStare, int an, int luna, string pagina, int userId, int userMarca, string motiv = "", string sqlPtj = "")
        {
            //    Actiune
            // 1   -  aprobat
            // 0   -  respins
            // 2   -  finalizat

            string mesaj = "";

            try
            {
                if ((idRol == 0 && (idStare == 1 || idStare == 4)) || 
                    (idRol == 1 && (idStare == 1 || idStare == 4)) ||
                    (idRol == 2 && (idStare == 1 || idStare == 2 || idStare == 4 || idStare == 6)) ||
                    (idRol == 3)
                    )
                {
                    //NOP
                }
                else
                {
                    return Dami.TraduCuvant("Marca") + " " + f10003 + ": " + Dami.TraduCuvant("nu aveti drepturi pentru aceasta operatie");
                }

                //Florin 2019.11.18
                if (idRol == 3 && idStare == 1 && actiune == 0) return Dami.TraduCuvant("nu puteti respinge un pontaj in stare initiat");

                //verificam data blocare pontaj
                DateTime dtBlc = new DateTime(2200, 1, 1);
                DataRow entBlc = General.IncarcaDR(@"SELECT * FROM ""DataBlocare"" ", null);

                if (entBlc != null)
                {
                    switch (idRol)
                    {
                        case 3:
                            //NOP
                            break;
                        case 2:
                            if (General.Nz(entBlc["DataManager"], "").ToString() != "")
                                dtBlc = Convert.ToDateTime(entBlc["DataManager"]);
                            break;
                        default:
                            if (General.Nz(entBlc["DataAngajat"], "").ToString() != "")
                                dtBlc = Convert.ToDateTime(entBlc["DataAngajat"]);
                            break;
                    }
                }

                if (dtBlc.Date < DateTime.Now.Date)
                {
                    return Dami.TraduCuvant("Marca") + " " + f10003 + ": " + Dami.TraduCuvant("data maxima aprobare") + " " + dtBlc.Day.ToString().PadLeft(2, '0') + "/" + dtBlc.Month.ToString().PadLeft(2, '0') + "/" + dtBlc.Year;
                }

                #region Validare start

                //Florin 2021.05.18
                string sqlVal = sqlPtj.Replace("FROM DUAL", "") + $@", {f10003} AS F10003, {General.ToDataUniv(an, luna)} AS ""ZiuaInc"", {idStare} AS ""IdStare"", {an} AS ""An"", {luna} AS ""Luna"", {actiune} AS ""Actiune"" " + (Constante.tipBD == 1 ? "" : " FROM DUAL");
                //string sablon = @"SELECT {0} AS F10003, {1} AS ""ZiuaInc"", {2} AS ""IdStare"", {3} AS ""An"", {4} AS ""Luna"", {5} AS ""Actiune"" " + (Constante.tipBD == 1 ? "" : " FROM DUAL");
                //string sqlVal = string.Format(sablon, f10003, General.ToDataUniv(an, luna), idStare, an, luna, actiune);

                string msg = Notif.TrimiteNotificare(pagina, (int)Constante.TipNotificare.Validare, sqlVal, "", -99, userId, userMarca);
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    return msg.Substring(2);
                }
                #endregion

                #region Schimbare stare in ptj cumulat

                DataTable dtCum = General.IncarcaDT(@"SELECT * FROM ""Ptj_Cumulat"" WHERE F10003=@1 AND ""An""=@2 AND ""Luna""=@3", new object[] { f10003, an, luna });
                DataRow drCum = dtCum.NewRow();

                if (dtCum == null || dtCum.Rows.Count == 0)
                {
                    drCum["F10003"] = f10003;
                    drCum["An"] = an;
                    drCum["Luna"] = luna;
                    dtCum.Rows.Add(drCum);
                }

                drCum = dtCum.Rows[0];

                drCum["TIME"] = DateTime.Now;
                drCum["USER_NO"] = userId;

                switch (idRol)
                {
                    case 0:                             //  angajat
                        if (actiune == 1) idStare = 2;
                        break;
                    case 1:                             //  introducere
                        if (actiune == 1) idStare = 2;
                        break;
                    case 2:                             //  manager
                        if (actiune == 1) idStare = 3;
                        if (actiune == 0 && idStare != 1) idStare = 4;
                        break;
                    case 3:                             //  HR
                        //if (actiune == 1) drCum["IdStare"] = 5;
                        if (actiune == 1) idStare = 5;
                        if (actiune == 0 && idStare != 1) idStare = 6;
                        if (actiune == 2) idStare = 7;
                        break;
                }

                drCum["IdStare"] = idStare;
                drCum["Comentarii"] = motiv;

                #endregion

                #region Istoric aprobare pontaj

                string sqlCum = @"SELECT TOP 0 * FROM ""Ptj_CumulatIstoric"" ";
                if (Constante.tipBD == 2)
                    sqlCum = @"SELECT * FROM ""Ptj_CumulatIstoric"" WHERE ROWNUM <= 0 ";
                DataTable dtIst = General.IncarcaDT(sqlCum, null);
                DataRow drIst = dtIst.NewRow();
                drIst["F10003"] = Convert.ToInt32(drCum["F10003"]);
                drIst["An"] = an;
                drIst["Luna"] = luna;
                drIst["IdStare"] = idStare;
                drIst["IdUser"] = userId;
                drIst["DataAprobare"] = DateTime.Now;
                drIst["USER_NO"] = userId;
                drIst["TIME"] = DateTime.Now;

                dtIst.Rows.Add(drIst);

                #endregion

                General.SalveazaDate(dtCum, "Ptj_Cumulat");
                General.SalveazaDate(dtIst, "Ptj_CumulatIstoric");

                #region  Notificare start

                //Florin 2021.05.18
                string sqlNtf = sqlPtj.Replace("FROM DUAL", "") + $@", {f10003} AS F10003, {General.ToDataUniv(an, luna)} AS ""ZiuaInc"", {idStare} AS ""IdStare"", {an} AS ""An"", {luna} AS ""Luna"", {actiune} AS ""Actiune"" " + (Constante.tipBD == 1 ? "" : " FROM DUAL");
                //string sqlNtf = string.Format(sablon, f10003, General.ToDataUniv(an, luna), idStare, an, luna, actiune);
                Notif.TrimiteNotificare(pagina, (int)Constante.TipNotificare.Notificare, sqlNtf, "Ptj_Cumulat",
                        Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT ""IdAuto"" FROM ""Ptj_Cumulat"" WHERE F10003 =@1 AND ""An"" =@2 AND ""Luna"" =@3", new object[] { f10003, an, luna }), -99)),
                        userId, userMarca);

                #endregion

                switch (actiune)
                {
                    case 0:                     // respins
                        mesaj = Dami.TraduCuvant("Marca") + " " + f10003 + ": " + Dami.TraduCuvant("pontajul a fost respins");
                        break;
                    case 1:                     // aprobat
                        mesaj = Dami.TraduCuvant("Marca") + " " + f10003 + ": " + Dami.TraduCuvant("pontajul a fost aprobat");
                        break;
                    case 2:                     // finalizat
                        mesaj = Dami.TraduCuvant("Marca") + " " + f10003 + ": " + Dami.TraduCuvant("pontajul a fost finalizat");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        public static string GetF10003Roluri(int idUser, int an, int luna, int alMeu, decimal F10003, string idRol, int zi = 0, int idDept = -99, int idAngajat = -99)
        {
            string str = "";

            try
            {
                if (alMeu == 1)
                {
                    str = " AND A.F10003 IN (" + F10003 + ")";
                }
                else
                {
                    string strSql = "";
                    string strFiltru = "";
                    if (idDept != -99) strFiltru = " AND A.F10007=" + idDept;
                    if (idAngajat != -99) strFiltru = " AND A.F10003=" + idAngajat;


                    strSql = $@"SELECT X.F10003 FROM ( 
                             SELECT B.F10003 FROM ""relGrupAngajat"" B
                             INNER JOIN ""Ptj_relGrupSuper"" C ON B.""IdGrup"" = C.""IdGrup""
                             WHERE C.""IdSuper"" = {idUser} AND C.""IdRol"" IN ({idRol})
                             GROUP BY B.F10003
                             UNION
                             SELECT B.F10003 FROM ""relGrupAngajat"" B
                             INNER JOIN ""Ptj_relGrupSuper"" C ON B.""IdGrup"" = C.""IdGrup""
                             INNER JOIN ""F100Supervizori"" D ON D.F10003 = B.F10003 AND D.""IdSuper"" = (-1 * C.""IdSuper"") AND D.""DataInceput"" <= {ToDataUniv(an, luna, 99)} AND {ToDataUniv(an, luna, 1)} <= D.""DataSfarsit""
                             WHERE D.""IdUser"" = {idUser} AND C.""IdRol"" IN ({idRol}) 
                             GROUP BY B.F10003) X 
                             INNER JOIN F100 A ON A.F10003=X.F10003 
                             WHERE A.F10025 <> 900 AND COALESCE({General.TruncateDate("A.F10022")},{General.CurrentDate()}) <> COALESCE({General.TruncateDate("A.F100993")},{General.CurrentDate()}) {strFiltru} {General.FiltruActivi(an, luna, zi)}
                             GROUP BY X.F10003";

                    str = " AND A.F10003 IN (" + strSql + ")";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }


        //public static string GetF10003Roluri(int idUser, int an, int luna, int alMeu, decimal F10003, string idRol, int zi = 0, int idDept = -99, int idAngajat = -99)
        //{
        //    string str = "";

        //    try
        //    {
        //        if (alMeu == 1)
        //        {
        //            str = " AND A.F10003 IN (" + F10003 + ")";
        //        }
        //        else
        //        {
        //            string strSql = "";
        //            string strFiltru = "";
        //            if (idDept != -99) strFiltru = " AND A.\"F10007\"=" + idDept;
        //            if (idAngajat != -99) strFiltru = " AND A.\"F10003\"=" + idAngajat;

        //            if (Constante.tipBD == 1)
        //            {
        //                strSql = " SELECT X.F10003 FROM ( " +
        //                        " SELECT B.f10003 FROM relGrupAngajat B" +
        //                        " INNER JOIN Ptj_relGrupSuper C ON B.IdGrup = C.IdGrup" +
        //                        " WHERE C.IdSuper =" + idUser + " AND C.IdRol=" + idRol +
        //                        " GROUP BY B.f10003" +
        //                        " UNION" +
        //                        " SELECT B.f10003 FROM relGrupAngajat B" +
        //                        " INNER JOIN Ptj_relGrupSuper C ON B.IdGrup = C.IdGrup" +
        //                        " INNER JOIN F100Supervizori D ON D.F10003 = B.F10003 AND D.IdSuper = (-1 * C.IdSuper) AND (100 * " + an + " + " + luna + " BETWEEN isnull(100 * year(D.DataInceput) + MONTH(D.DataInceput), 190000) AND isnull(100 * year(D.DataSfarsit) + MONTH(D.DataSfarsit), 210000)) " +
        //                        " WHERE D.IdUser =" + idUser + " AND C.IdRol=" + idRol +
        //                        " GROUP BY B.F10003) X " +
        //                        " WHERE X.F10003 IN (SELECT S.F10003 FROM F100 S WHERE S.F10025 <> 900 AND CONVERT(date,S.F10022) <> CONVERT(date,S.F100993) " + strFiltru.Replace("A.", "S.") + General.FiltruActivi(an, luna, zi) + ")" +
        //                        " GROUP BY X.F10003";
        //            }
        //            else
        //            {
        //                strSql = "SELECT X.F10003 FROM ( " +
        //                        " SELECT B.f10003 FROM \"relGrupAngajat\" B " +
        //                        " INNER JOIN \"Ptj_relGrupSuper\" C ON B.\"IdGrup\" = C.\"IdGrup\" " +
        //                        " WHERE C.\"IdSuper\" =" + idUser + " AND C.\"IdRol\"=" + idRol +
        //                        " GROUP BY B.F10003 " +
        //                        " UNION " +
        //                        " SELECT B.F10003 FROM \"relGrupAngajat\" B " +
        //                        " INNER JOIN \"Ptj_relGrupSuper\" C ON B.\"IdGrup\" = C.\"IdGrup\" " +
        //                        " INNER JOIN \"F100Supervizori\" D ON D.F10003 = B.F10003 AND D.\"IdSuper\" = (-1 * C.\"IdSuper\") AND D.\"DataInceput\" <= " + ToDataUniv(an, luna, 99) + " AND " + ToDataUniv(an, luna, 1) + " <= D.\"DataSfarsit\" " +
        //                        " WHERE D.\"IdUser\" =" + idUser + " AND C.\"IdRol\"=" + idRol +
        //                        " GROUP BY B.F10003) X  " +
        //                        " INNER JOIN F100 A ON A.F10003=X.F10003  " +
        //                        " WHERE 1=1 AND A.F10025 <> 900 AND TRUNC(A.F10022) <> TRUNC(COALESCE(A.F100993,TO_DATE('01-01-2101','DD-MM-YYYY'))) " + strFiltru + General.FiltruActivi(an, luna, zi) +
        //                        " GROUP BY X.F10003";

        //            }

        //            str = " AND A.F10003 IN (" + strSql + ")";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return str;
        //}

        ////Radu 04.02.2020
        //public static string GetF10003RoluriComasate(int idUser, int an, int luna, decimal F10003, List<int> lstRoluri, int zi = 0, int idDept = -99, int idAngajat = -99)
        //{
        //    string str = "";

        //    try
        //    {              
        //        string strSql = "";
        //        string strFiltru = "";
        //        if (idDept != -99) strFiltru = " AND A.\"F10007\"=" + idDept;
        //        if (idAngajat != -99) strFiltru = " AND A.\"F10003\"=" + idAngajat;

        //        foreach (int idRol in lstRoluri)
        //        {
        //            strSql += "UNION ";
        //            if (Constante.tipBD == 1)
        //            {
        //                strSql += " (SELECT X.F10003 FROM ( " +
        //                        " SELECT B.f10003 FROM relGrupAngajat B" +
        //                        " INNER JOIN Ptj_relGrupSuper C ON B.IdGrup = C.IdGrup" +
        //                        " WHERE C.IdSuper =" + idUser + " AND C.IdRol=" + idRol +
        //                        " GROUP BY B.f10003" +
        //                        " UNION" +
        //                        " SELECT B.f10003 FROM relGrupAngajat B" +
        //                        " INNER JOIN Ptj_relGrupSuper C ON B.IdGrup = C.IdGrup" +
        //                        " INNER JOIN F100Supervizori D ON D.F10003 = B.F10003 AND D.IdSuper = (-1 * C.IdSuper) AND (100 * " + an + " + " + luna + " BETWEEN isnull(100 * year(D.DataInceput) + MONTH(D.DataInceput), 190000) AND isnull(100 * year(D.DataSfarsit) + MONTH(D.DataSfarsit), 210000)) " +
        //                        " WHERE D.IdUser =" + idUser + " AND C.IdRol=" + idRol +
        //                        " GROUP BY B.F10003) X " +
        //                        " WHERE X.F10003 IN (SELECT S.F10003 FROM F100 S WHERE S.F10025 <> 900 AND CONVERT(date,S.F10022) <> CONVERT(date,S.F100993) " + strFiltru.Replace("A.", "S.") + General.FiltruActivi(an, luna, zi) + ")" +
        //                        " GROUP BY X.F10003)";
        //            }
        //            else
        //            {

        //                strSql += " (SELECT X.F10003 FROM ( " +
        //                        " SELECT B.f10003 FROM \"relGrupAngajat\" B " +
        //                        " INNER JOIN \"Ptj_relGrupSuper\" C ON B.\"IdGrup\" = C.\"IdGrup\" " +
        //                        " WHERE C.\"IdSuper\" =" + idUser + " AND C.\"IdRol\"=" + idRol +
        //                        " GROUP BY B.F10003 " +
        //                        " UNION " +
        //                        " SELECT B.F10003 FROM \"relGrupAngajat\" B " +
        //                        " INNER JOIN \"Ptj_relGrupSuper\" C ON B.\"IdGrup\" = C.\"IdGrup\" " +
        //                        " INNER JOIN \"F100Supervizori\" D ON D.F10003 = B.F10003 AND D.\"IdSuper\" = (-1 * C.\"IdSuper\") AND D.\"DataInceput\" <= " + ToDataUniv(an, luna, 99) + " AND " + ToDataUniv(an, luna, 1) + " <= D.\"DataSfarsit\" " +
        //                        " WHERE D.\"IdUser\" =" + idUser + " AND C.\"IdRol\"=" + idRol +
        //                        " GROUP BY B.F10003) X  " +
        //                        " INNER JOIN F100 A ON A.F10003=X.F10003  " +
        //                        " WHERE 1=1 AND A.F10025 <> 900 AND TRUNC(A.F10022) <> TRUNC(COALESCE(A.F100993,TO_DATE('01-01-2101','DD-MM-YYYY'))) " + strFiltru + General.FiltruActivi(an, luna, zi) +
        //                        " GROUP BY X.F10003)";
        //            }
        //        }
        //        if (strSql.Length > 5)
        //            str = " AND A.F10003 IN (" + strSql.Substring(5) + ")";

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return str;
        //}

        //Florin 2019.12.27
        public static string GetF10003Roluri(int idUser, int an, int luna, int alMeu, decimal F10003, int idRol, int zi = 0, string denDept = "", int idAngajat = -99)
        {
            string str = "";

            try
            {
                if (alMeu == 1)
                {
                    str = " AND A.F10003 IN (" + F10003 + ")";
                }
                else
                {
                    string strSql = "";
                    string strFiltru = "";
                    if (denDept != "") strFiltru = " AND B.\"F00608\" IN ('" + denDept.Replace("\\\\","','") + "')";
                    if (idAngajat != -99) strFiltru = " AND A.\"F10003\"=" + idAngajat;

                    if (Constante.tipBD == 1)
                    {
                        strSql = " SELECT X.F10003 FROM ( " +
                                " SELECT B.f10003 FROM relGrupAngajat B" +
                                " INNER JOIN Ptj_relGrupSuper C ON B.IdGrup = C.IdGrup" +
                                " WHERE C.IdSuper =" + idUser + " AND C.IdRol=" + idRol +
                                " GROUP BY B.f10003" +
                                " UNION" +
                                " SELECT B.f10003 FROM relGrupAngajat B" +
                                " INNER JOIN Ptj_relGrupSuper C ON B.IdGrup = C.IdGrup" +
                                " INNER JOIN F100Supervizori D ON D.F10003 = B.F10003 AND D.IdSuper = (-1 * C.IdSuper) AND (100 * " + an + " + " + luna + " BETWEEN isnull(100 * year(D.DataInceput) + MONTH(D.DataInceput), 190000) AND isnull(100 * year(D.DataSfarsit) + MONTH(D.DataSfarsit), 210000)) " +
                                " WHERE D.IdUser =" + idUser + " AND C.IdRol=" + idRol +
                                " GROUP BY B.F10003) X " +
                                " INNER JOIN F100 A ON A.F10003=X.F10003 " +
                                " INNER JOIN F006 B ON A.F10007=B.F00607 " +
                                " WHERE 1=1 AND CONVERT(date,A.F10022) <> CONVERT(date,A.F100993) " + strFiltru.Replace("A.", "S.") + General.FiltruActivi(an, luna, zi) +
                                " GROUP BY X.F10003";
                    }
                    else
                    {
                        strSql = "SELECT X.F10003 FROM ( " +
                                " SELECT B.f10003 FROM \"relGrupAngajat\" B " +
                                " INNER JOIN \"Ptj_relGrupSuper\" C ON B.\"IdGrup\" = C.\"IdGrup\" " +
                                " WHERE C.\"IdSuper\" =" + idUser + " AND C.\"IdRol\"=" + idRol +
                                " GROUP BY B.F10003 " +
                                " UNION " +
                                " SELECT B.F10003 FROM \"relGrupAngajat\" B " +
                                " INNER JOIN \"Ptj_relGrupSuper\" C ON B.\"IdGrup\" = C.\"IdGrup\" " +
                                " INNER JOIN \"F100Supervizori\" D ON D.F10003 = B.F10003 AND D.\"IdSuper\" = (-1 * C.\"IdSuper\") AND D.\"DataInceput\" <= " + ToDataUniv(an, luna, 99) + " AND " + ToDataUniv(an, luna, 1) + " <= D.\"DataSfarsit\" " +
                                " WHERE D.\"IdUser\" =" + idUser + " AND C.\"IdRol\"=" + idRol +
                                " GROUP BY B.F10003) X  " +
                                " INNER JOIN F100 A ON A.F10003=X.F10003 " +
                                " INNER JOIN F006 B ON A.F10007=B.F00607 " +
                                " WHERE 1=1 AND TRUNC(A.F10022) <> TRUNC(COALESCE(A.F100993,TO_DATE('01-01-2101','DD-MM-YYYY'))) " + strFiltru + General.FiltruActivi(an, luna, zi) +
                                " GROUP BY X.F10003";
                    }

                    str = " AND A.F10003 IN (" + strSql + ")";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }


        public static void PontajInitGeneral(int idUser, int an, int luna, string contracte = "", string dept = "", int f10003 = -99)
        {
            try
            {
                //Florin 2020.06.30
                string filtru = "";
                if (contracte != "")
                    filtru = $@" AND Y.""Denumire"" IN ('{contracte}')";

                //Florin 2021.03.23
                if (dept != "")
                    filtru = $@" AND G.F00608 IN ('{dept}')";

                //Florin 2021.03.23
                if (f10003 != -99)
                    filtru = $@" AND B.F10003 = {f10003}";

                string strInner = @"OUTER APPLY dbo.DamiNorma(B.F10003, A.""Zi"") dn
                                    OUTER APPLY    dbo.DamiCC(B.F10003, A.""Zi"") dc
                                    OUTER APPLY  dbo.DamiDept(B.F10003, A.""Zi"") dd";
                if (Dami.ValoareParam("TipCalculDate") == "2")
                {
                    strInner = @"LEFT JOIN DamiNorma_Table dn ON dn.F10003=B.F10003 AND dn.dt=A.Zi
                                LEFT JOIN DamiCC_Table     dc ON dc.F10003=B.F10003 AND dc.dt=A.Zi
								LEFT JOIN DamiDept_Table   dd ON dd.F10003=B.F10003 AND dd.dt=A.Zi";
                }

                //initializam Ptj_Intrari
                string strInt = $@"INSERT INTO ""Ptj_Intrari""(F10003, ""Ziua"", F06204, ""ZiSapt"", ""ZiLibera"", ""ZiLiberaLegala"", ""Norma"", ""IdContract"", F10002, F10004, F10005, F10006, F10007, USER_NO, TIME, ""F06204Default"", ""IdProgram"", F100958, F100959)
                                SELECT B.F10003, A.""Zi"", -1 AS F06204, A.""ZiSapt"", 
                                CASE WHEN A.""ZiSapt""=6 OR A.""ZiSapt""=7 OR C.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLibera"", 
                                CASE WHEN C.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",
                                dn.Norma AS ""Norma"", 
                                (SELECT MAX(P.""IdContract"") FROM ""F100Contracte"" P WHERE P.F10003 = B.F10003 AND {TruncateDate("P.DataInceput")} <= {TruncateDate("A.Zi")}  AND {TruncateDate("A.Zi")}  <= {TruncateDate("P.DataSfarsit")} ) AS ""IdContract"", 
                                G.F00603 AS F10002, G.F00604 AS F10004, G.F00605 AS F10005, G.F00606 AS F10006, G.F00607 as F10007,
                                {HttpContext.Current.Session["UserId"]} AS USER_NO, {CurrentDate()} AS TIME,

                                CASE WHEN (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = B.F10003 AND C.""DataInceput"" <= A.Zi AND  A.Zi <= C.""DataSfarsit"") IS NULL THEN 
                                CASE WHEN COALESCE(dc.CC, 9999) <> 9999 THEN dc.CC ELSE (SELECT C.F00615 FROM F006 C WHERE C.F00607 = dd.Dept) END 
                                ELSE (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = B.F10003 AND C.""DataInceput"" <=  A.Zi AND  A.Zi <= C.""DataSfarsit"") END AS ""F06204Default"",
                                CASE WHEN (CASE WHEN C.DAY is not null THEN 1 ELSE 0 END) = 1 AND Y.""TipSchimb8"" = 1 THEN  COALESCE(Y.""Program8"", Y.""Program0"", -99) ELSE
                                CASE (CASE WHEN datepart(dw,A.""Zi"") - 1 = 0 THEN 7 ELSE datepart(dw,A.Zi) - 1 END)
                                WHEN 1 THEN (CASE WHEN COALESCE(Y.""TipSchimb1"",1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 2 THEN (CASE WHEN COALESCE(Y.""TipSchimb2"",1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 3 THEN (CASE WHEN COALESCE(Y.""TipSchimb3"",1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 4 THEN (CASE WHEN COALESCE(Y.""TipSchimb4"",1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 5 THEN (CASE WHEN COALESCE(Y.""TipSchimb5"",1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 6 THEN (CASE WHEN COALESCE(Y.""TipSchimb6"",1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 7 THEN (CASE WHEN COALESCE(Y.""TipSchimb7"",1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"", -99) ELSE -99 END) 
                                END END AS ""IdProgram"",
                                COALESCE(sd.Subdept, (SELECT C.F100958 FROM F1001 C WHERE C.F10003=B.F10003)) AS F100958, 
                                COALESCE(br.Birou, (SELECT C.F100959 FROM F1001 C WHERE C.F10003=B.F10003)) AS F100959

                                FROM ""tblZile"" A
                                INNER JOIN F100 B ON 1=1 AND B.F10022 <= {TruncateDate("A.Zi")}  AND {TruncateDate("A.Zi")}  <= B.F10023
                                LEFT JOIN HOLIDAYS C on A.""Zi""=C.DAY
                                LEFT JOIN (SELECT X.F10003, X.""Ziua"", COUNT(*) AS CNT FROM ""Ptj_Intrari"" X WHERE {General.FunctiiData("X.\"Ziua\"", "A")}={an} AND {General.FunctiiData("X.\"Ziua\"", "L")}={luna} GROUP BY X.F10003, X.""Ziua"") D ON D.F10003=B.F10003 AND D.""Ziua"" = A.""Zi""
                                {strInner}                                
                                LEFT JOIN F006 G ON G.F00607 = dd.Dept
                                INNER JOIN ""Ptj_Contracte"" Y ON Y.""Id""=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" BB WHERE BB.F10003 = B.F10003 AND BB.""DataInceput"" <= A.Zi AND A.Zi <= BB.""DataSfarsit"")
                                OUTER APPLY dbo.DamiSubdept(B.F10003, A.Zi) sd
                                OUTER APPLY dbo.DamiBirou(B.F10003, A.Zi) br                                 
                                WHERE {General.FunctiiData("A.\"Zi\"", "A")}={an} AND {General.FunctiiData("A.\"Zi\"", "L")}={luna} AND COALESCE(D.CNT,0) = 0 {filtru};";
                if (Constante.tipBD == 2)
                    strInt = $@"INSERT INTO ""Ptj_Intrari""(F10003, ""Ziua"", F06204, ""ZiSapt"", ""ZiLibera"", ""ZiLiberaLegala"", ""Norma"", ""IdContract"", F10002, F10004, F10005, F10006, F10007, USER_NO, TIME, ""F06204Default"", ""IdProgram"", F100958, F100959)
                                SELECT B.F10003, A.""Zi"", -1 AS F06204, A.""ZiSapt"", 
                                CASE WHEN A.""ZiSapt""=6 OR A.""ZiSapt""=7 OR C.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLibera"", 
                                CASE WHEN C.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",
                                ""DamiNorma""(B.F10003, A.""Zi"") AS ""Norma"", 
                                (SELECT MAX(P.""IdContract"") FROM ""F100Contracte"" P WHERE P.F10003 = B.F10003 AND {TruncateDate("P.DataInceput")} <= {TruncateDate("A.Zi")}  AND {TruncateDate("A.Zi")}  <= {TruncateDate("P.DataSfarsit")} ) AS ""IdContract"", 
                                G.F00603 AS F10002, G.F00604 AS F10004, G.F00605 AS F10005, G.F00606 AS F10006, G.F00607 as F10007,
                                {HttpContext.Current.Session["UserId"]} AS USER_NO, {General.CurrentDate()} AS TIME,

                                CASE WHEN (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = B.F10003 AND C.""DataInceput"" <= A.""Zi"" AND A.""Zi"" <= C.""DataSfarsit"" and ROWNUM <= 1) IS NULL THEN
                                CASE WHEN COALESCE(""DamiCC""(B.F10003, A.""Zi""), 9999) <> 9999 THEN ""DamiCC""(B.F10003, A.""Zi"") ELSE (SELECT C.F00615 FROM F006 C WHERE C.F00607 = ""DamiDept""(B.F10003, A.""Zi"")) END 
                                ELSE (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = B.F10003 AND C.""DataInceput"" <= A.""Zi"" AND A.""Zi"" <= C.""DataSfarsit"" and ROWNUM <= 1) END AS ""F06204Default"",
                                CASE WHEN (CASE WHEN C.DAY is not null THEN 1 ELSE 0 END) = 1 AND Y.""TipSchimb8"" = 1 THEN  COALESCE(Y.""Program8"", Y.""Program0"", -99) ELSE
                                CASE (1 + TRUNC(A.""Zi"") - TRUNC(A.""Zi"", 'IW'))
                                WHEN 1 THEN (CASE WHEN COALESCE(Y.""TipSchimb1"",1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 2 THEN (CASE WHEN COALESCE(Y.""TipSchimb2"",1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 3 THEN (CASE WHEN COALESCE(Y.""TipSchimb3"",1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 4 THEN (CASE WHEN COALESCE(Y.""TipSchimb4"",1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 5 THEN (CASE WHEN COALESCE(Y.""TipSchimb5"",1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 6 THEN (CASE WHEN COALESCE(Y.""TipSchimb6"",1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 7 THEN (CASE WHEN COALESCE(Y.""TipSchimb7"",1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"", -99) ELSE -99 END) 
                                END END AS ""IdProgram"",
								COALESCE(""DamiSubdept""(B.F10003, A.""Ziua""), (SELECT C.F100958 FROM F1001 C WHERE C.F10003=B.F10003)) AS F100958,
                                COALESCE(""DamiBirou""(B.F10003, A.""Ziua""), (SELECT C.F100959 FROM F1001 C WHERE C.F10003=B.F10003)) AS F100958

                                FROM ""tblZile"" A
                                INNER JOIN F100 B ON 1=1 AND B.F10022  <= {TruncateDate("A.Zi")}  AND {TruncateDate("A.Zi")}  <= B.F10023
                                LEFT JOIN HOLIDAYS C on A.""Zi""=C.DAY
                                LEFT JOIN (SELECT X.F10003, X.""Ziua"", COUNT(*) AS CNT FROM ""Ptj_Intrari"" X WHERE {FunctiiData("X.\"Ziua\"", "A")}={an} AND {FunctiiData("X.\"Ziua\"", "L")}={luna} GROUP BY X.F10003, X.""Ziua"") D ON D.F10003=B.F10003 AND D.""Ziua"" = A.""Zi""                                                                   
                                LEFT JOIN F006 G ON G.F00607 = ""DamiDept""(B.F10003, A.""Zi"")
                                LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY CTR.F10003 order by CTR.""F10003"", CTR.""IdContract"" DESC) as ""NrCrt"", CTR.* FROM ""F100Contracte"" CTR) BC ON BC.F10003 = B.F10003 AND BC.""DataInceput"" <= A.""Zi"" AND A.""Zi"" <= BC.""DataSfarsit"" AND ""NrCrt"" = 1
                                INNER JOIN ""Ptj_Contracte"" Y ON Y.""Id"" = BC.""IdContract""                                
                                WHERE {General.FunctiiData("A.\"Zi\"", "A")}={an} AND {FunctiiData("A.\"Zi\"", "L")}={luna} AND COALESCE(D.CNT,0) = 0 {filtru};";
								
								
                //initializam Ptj_Cumulat
                string strCum = $@"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"", USER_NO, TIME)
                                SELECT F10003, {an}, {luna}, 1, F10008 {Dami.Operator()} ' ' {Dami.Operator()} F10009, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()} FROM F100
                                WHERE F10022 <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= F10023 
                                AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={an} AND ""Luna"" = {luna});";

                //initializam Ptj_CumulatIstoric
                string strIst = $@"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT F10003, {an}, {luna}, 1, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()} FROM F100 
                                WHERE F10022 <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= F10023 
                                AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={an} AND ""Luna"" = {luna} GROUP BY F10003);";

                if (contracte != "")
                {
                    //initializam Ptj_Cumulat
                    strCum = $@"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"", USER_NO, TIME)
                                SELECT A.F10003, {an}, {luna}, 1, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()} FROM F100 A
                                INNER JOIN ""F100Contracte"" B ON A.F10003=B.F10003 AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit""
                                INNER JOIN ""Ptj_Contracte"" C ON B.""IdContract""=C.""Id"" AND C.""Denumire"" IN ('{contracte}')
                                WHERE A.F10022 <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= A.F10023 
                                AND A.F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={an} AND ""Luna"" = {luna});";

                    //initializam Ptj_CumulatIstoric
                    strIst = $@"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT A.F10003, {an}, {luna}, 1, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()} FROM F100 A
                                INNER JOIN ""F100Contracte"" B ON A.F10003=B.F10003 AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit""
                                INNER JOIN ""Ptj_Contracte"" C ON B.""IdContract""=C.""Id"" AND C.""Denumire"" IN ('{contracte}')                                
                                WHERE A.F10022 <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= A.F10023 
                                AND A.F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={an} AND ""Luna"" = {luna} GROUP BY F10003);";
                }

                General.ExecutaNonQuery("BEGIN " + strInt + "\n\r" + strCum + "\n\r" + strIst + " END;", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string SelectDepartamente()
        {
            string strSql = "";

            try
            {
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";

                if (Constante.tipBD == 2) cmp = "ROWNUM";

                strSql = @"SELECT {0} as ""IdAuto"", a.F00204 AS ""Companie"",b.F00305 AS ""Subcompanie"",c.F00406 AS ""Filiala"",d.F00507 AS ""Sectie"",e.F00608 AS ""Dept"", F.F00709 AS ""Subdept"", G.F00810 AS ""Birou"",
                            a.F00202 AS ""IdCompanie"",b.F00304 AS ""IdSubcompanie"",c.F00405 AS ""IdFiliala"",d.F00506 AS ""IdSectie"",e.F00607 AS ""IdDept"", F.F00708 AS ""IdSubdept"", G.F00809 AS ""IdBirou"", e.F00607 
                            FROM F002 A
                            LEFT JOIN F003 B ON A.F00202 = B.F00303
                            LEFT JOIN F004 C ON B.F00304 = C.F00404
                            LEFT JOIN F005 D ON C.F00405 = D.F00505 
                            LEFT JOIN F006 E ON D.F00506 = E.F00606
                            LEFT JOIN F007 F ON E.F00607 = F.F00707
                            LEFT JOIN F008 G ON F.F00708 = G.F00808
                            ORDER BY E.F00607";

                strSql = string.Format(strSql, cmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static DataTable GetAbsentePeContract(int idAbs)
        {
            //tip
            //0 - absente de tip zi
            //1 - absente de tip ora

            DataTable dt = new DataTable();

            try
            {
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";

                if (Constante.tipBD == 2) cmp = "ROWNUM";

                string strSql = $@"select {0} as ""IdAuto"", x.* from ( 
                                select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 0 AS ""Tip"" 
                                from ""Ptj_tblAbsente"" a
                                inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                                inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                                WHERE A.""IdTipOre"" = 1
                                group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal""
                                UNION
                                select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 1 AS ""Tip"" 
                                from ""Ptj_tblAbsente"" a
                                inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                                inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                                WHERE A.""OreInVal"" IS NOT NULL
                                group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"") x 
                                where idcontract=1 and idrol=1 and idabsenta={idAbs}
                                ORDER BY x.""Tip"", x.""OreInVal"" ";

                strSql = string.Format(strSql, cmp);

                dt = IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        public static bool PontajInit(int idUser, int an, int luna, int idRol, bool cuNormaZL = false, bool cuCCCu = false, string denDept = "", int idAng = -99, int idSubcompanie = -99, int idFiliala = -99, int idSectie = -99, string denContract = "", bool cuNormaSD = false, bool cuNormaSL = false, bool cuCCFara = false, int stergePontariAngPlecati = 0, int cuInOut = 0)
        {
            bool ras = false;

            try
            {
                //Florin 2020.06.30 am comentat conditia de mai jos
                //if (cuNormaZL == false && cuNormaSD == false && cuNormaSL == false) return ras;

                string strZile = "";
                string usr = "";
                if (idAng == -99)
                    usr = General.GetF10003Roluri(idUser, an, luna, 0, -99, idRol, 0, denDept, idAng);
                else
                    usr = " AND A.F10003=" + idAng;

                string ziInc = General.ToDataUniv(an, luna, 1);
                string ziSf = General.ToDataUniv(an, luna, 99);

                if (Constante.tipBD == 1)
                {
                    #region SQL
                    string strFIN = "";

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " AS Ziua";
                    }

                    if (strZile.Length > 6) strZile = strZile.Substring(6);

                    if (stergePontariAngPlecati == 1)
                    {
                        //Florin 2019.12.05 - am adaugat Ptj_IstoricVal
                        string strDel = $@"
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Echipei - Initializare', {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                FROM Ptj_Intrari A
                                INNER JOIN (select f100.F10003, ISNULL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403 AND modif.data > F100.F10022
                                ) B 
                                ON A.F10003=B.F10003 AND A.Ziua> B.DATA_PLECARII AND {ziInc} <= A.Ziua AND A.Ziua <= {ziSf} AND CONVERT(date, B.DATA_PLECARII) <> '2100-01-01';
                                    
                                DELETE A
                                FROM Ptj_Intrari A
                                INNER JOIN (select f100.F10003, ISNULL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403 AND modif.data > F100.F10022
                                ) B 
                                ON A.F10003=B.F10003 AND A.Ziua> B.DATA_PLECARII AND {ziInc} <= A.Ziua AND A.Ziua <= {ziSf} AND CONVERT(date, B.DATA_PLECARII) <> '2100-01-01';";

                        strFIN += strDel + "\n\r";

                        strDel = $@"
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Echipei - Initializare', {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                FROM Ptj_Intrari A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE CONVERT(date,f10022) <= {ziSf} AND CONVERT(date,F10022) <> '2100-01-01') B ON A.F10003=B.F10003 AND A.Ziua< B.F10022  AND {ziInc} <= A.Ziua AND A.Ziua <= {ziSf};

                                DELETE A
                                FROM Ptj_Intrari A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE CONVERT(date,f10022) <= {ziSf} AND CONVERT(date,F10022) <> '2100-01-01') B ON A.F10003=B.F10003 AND A.Ziua< B.F10022  AND {ziInc} <= A.Ziua AND A.Ziua <= {ziSf};";

                        strFIN += strDel + "\n\r";
                    }

                    string strFiltruZile = "";
                    if (cuNormaZL) strFiltruZile += @"OR (CASE WHEN datepart(dw,X.Ziua) in (2,3,4,5,6) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSD) strFiltruZile += @"OR (CASE WHEN datepart(dw,X.Ziua) in (1,7) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSL) strFiltruZile += @"OR B.DAY is not null";

                    if (strFiltruZile != "") strFiltruZile = "AND (" + strFiltruZile.Substring(2) + ")";


                    string strFiltru = "";
                    //Florin 2019.12.27
                    //if (idContract != -99) strFiltru += " AND (SELECT MAX(IdContract) FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CONVERT(date,B.DataInceput) <= " + ziSf + " AND " + ziInc + " <= CONVERT(date,B.DataSfarsit)) = " + idContract.ToString();
                    if (denContract != "") strFiltru += " AND (SELECT MAX(W.Denumire) FROM F100Contracte B INNER JOIN Ptj_Contracte W ON W.Id=B.IdContract WHERE A.F10003 = B.F10003 AND CONVERT(date,B.DataInceput) <= " + ziSf + " AND " + ziInc + " <= CONVERT(date,B.DataSfarsit)) IN ('" + denContract.Replace(",","','") + "')";


                    if (idSubcompanie != -99) strFiltru += " AND A.F10004 = " + idSubcompanie.ToString();
                    if (idFiliala != -99) strFiltru += " AND A.F10005 = " + idFiliala.ToString();
                    if (idSectie != -99) strFiltru += " AND A.F10006 = " + idSectie.ToString();

                    //Florin 2019.12.27
                    //if (idDept != -99) strFiltru += " AND A.F10007 = " + idDept.ToString();
                    if (denDept != "") strFiltru += @" AND A.F10007 IN (SELECT F00607 FROM F006 WHERE F00608 IN ('" + denDept.Replace(",", "','") + "'))";


                    //Florin #773
                    string strLast = "";
                    if (cuInOut == 1)
                        strLast = @", ""FirstIn"", ""LastOut""";

                    string strIns = @"insert into ""Ptj_Intrari""(F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""Parinte"", ""Linia"", F06204, F10002, F10004, F10005, F10006, F10007, F100958, F100959, ""CuloareValoare"", ""Norma"", ""IdContract"", USER_NO, TIME, ""ZiLiberaLegala"", ""F06204Default"", ""IdProgram"", ""ValStr"", ""Val0"", ""In1"", ""Out1"" " + strLast + ") {0} {1} {2} {3} ";

                    //Florin 2020.06.30 am modificat 1 cu cuNormaZL
                    strIns = string.Format(strIns, DamiSelectPontajInit(idUser, an, luna, cuNormaZL, cuInOut), strFiltru, strFiltruZile, usr);

                    strFIN += strIns + ";";

                    if (cuCCCu || cuCCFara)
                    {
                        string strFiltruZileCC = "";
                        if (cuNormaZL) strFiltruZileCC += @"OR (CASE WHEN X.ZiSapt in (1,2,3,4,5) AND C.DAY is null THEN 1 ELSE 0 END) = 1";
                        if (cuNormaSD) strFiltruZileCC += @"OR (CASE WHEN X.ZiSapt in (6,7) AND C.DAY is null THEN 1 ELSE 0 END) = 1";
                        if (cuNormaSL) strFiltruZileCC += @"OR C.DAY is not null";
                        if (strFiltruZileCC != "") strFiltruZileCC = "AND (" + strFiltruZileCC.Substring(2) + ")";


                        //Florin 2018.10.23
                        string strInnerCC = @"OUTER APPLY dbo.DamiNorma(A.F10003, X.Zi) dn
                                            OUTER APPLY dbo.DamiCC(A.F10003, X.Zi) dc
                                            OUTER APPLY dbo.DamiDept(A.F10003, X.Zi) dd";
                        if (Dami.ValoareParam("TipCalculDate") == "2")
                        {
                            strInnerCC =
                                @"LEFT JOIN DamiNorma_Table dn ON dn.F10003=A.F10003 AND dn.dt=X.Zi
								LEFT JOIN DamiDept_Table dd ON dd.F10003=A.F10003 AND dd.dt=X.Zi
                                LEFT JOIN DamiCC_Table dc ON dc.F10003=A.F10003 AND dc.dt=X.Zi";
                        }

                        //Florin 2020.03.27 am transformat NrOre in minute - am adaugat * 60
                        string strInsCC = $@"INSERT INTO Ptj_CC(F10003, Ziua, F06204, NrOre1, IdStare, USER_NO, TIME)
                                            SELECT A.F10003, X.Zi, 
                                            CASE WHEN MAX(G.IdCentruCost) IS NOT NULL THEN MAX(G.IdCentruCost) ELSE CASE WHEN COALESCE(MAX(dc.CC), 9999) <> 9999 THEN MAX(dc.CC) ELSE MAX(H.F00615) END END AS F06204Default,
                                            dn.Norma * 60 AS NrOre1, 
                                            CASE WHEN COALESCE((SELECT COALESCE(Valoare,0) FROM tblParametrii WHERE Nume = 'PontajCCcuAprobare'),0) = 0 THEN 3 ELSE 1 END AS IdStare, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                            FROM (SELECT * FROM tblzile Z WHERE {General.ToDataUniv(an, luna, 1)} <= Z.Zi AND Z.Zi <= {General.ToDataUniv(an, luna, 99)}) X
                                            INNER JOIN F100 A ON 1=1 AND CONVERT(date, A.F10022) <= CONVERT(date, X.Zi) AND CONVERT(date, X.Zi) <= CONVERT(date, A.F10023)
                                            LEFT JOIN Ptj_CC B ON B.Ziua = X.Zi AND B.F10003=A.F10003
                                            LEFT JOIN HOLIDAYS C ON C.DAY = X.Zi
                                            {strInnerCC}
                                            LEFT JOIN F100CentreCost G ON G.F10003 = A.F10003 AND G.DataInceput <= X.Zi AND X.Zi <= G.DataSfarsit
                                            LEFT JOIN F006 H ON H.F00607 = dd.Dept
                                            WHERE B.F10003 IS NULL AND dc.CC IS NOT NULL 
                                            {usr} {strFiltru} {strFiltruZileCC}
                                            GROUP BY A.F10003, X.Zi, dn.Norma";

                        strFIN += strInsCC + ";" + "\n\r";
                    }


                    //Florin 2019.05.14
                    //string strInner = @"LEFT JOIN (SELECT F70403, MAX(DATEADD(d,-1,F70406)) DataPlecare FROM f704 WHERE F70404=4 GROUP BY F70403) ddp ON c.F10003 = ddp.F70403 
                    //                    OUTER APPLY dbo.DamiNorma(A.F10003, A.Ziua) dn";
                    ////Florin 2018.10.23
                    //Radu 02.02.2021 - s-a revenit la functia DamiDataPlecare
                    string strInner = @"OUTER APPLY dbo.DamiNorma(A.F10003, A.Ziua) dn
                                        OUTER APPLY dbo.DamiDataPlecare(A.F10003, A.Ziua) ddp";


                    if (Dami.ValoareParam("TipCalculDate") == "2")
                    {
                        strInner =
                            @"LEFT JOIN DamiNorma_Table dn ON dn.F10003=A.F10003 AND dn.dt=A.Ziua
                              LEFT JOIN DamiDataPlecare_Table ddp ON ddp.F10003=A.F10003 AND ddp.dt=A.Ziua";
                    }

                    //Florin 2020.06.30 - s-a adaugat conditia cu norma
                    //actualizam inregistrarile unde norma = null doar pt linia mama, nu si pt centrii de cost
                    //2015-12-07  s-a adaugat inner join f100 pt a elimina zilele in care angajatii s-au angajat sau au plecat in luna
                    if (cuNormaZL)
                    {
                        string strUp = @"UPDATE A SET 
                                        A.""ValStr"" = dn.Norma , 
                                        A.""Val0""   = dn.Norma * 60,
                                        In1  = ISNULL(In1,(SELECT DATETIMEFROMPARTS(YEAR(A.Ziua), MONTH(A.Ziua), DAY(A.Ziua), DATEPART(HOUR, OraInInitializare), DATEPART(MINUTE, OraInInitializare), 0, 0) FROM Ptj_Contracte WHERE Id = A.IdContract)),
                                        Out1 = ISNULL(Out1,(SELECT DATETIMEFROMPARTS(YEAR(A.Ziua), MONTH(A.Ziua), DAY(A.Ziua), DATEPART(HOUR, OraOutInitializare), DATEPART(MINUTE, OraOutInitializare), 0, 0) FROM Ptj_Contracte WHERE Id = A.IdContract))
                                        FROM ""Ptj_Intrari"" A
                                        INNER JOIN F100 C ON A.F10003=C.F10003 AND CONVERT(date, C.F10022) <= CONVERT(date, A.Ziua) AND CONVERT(date, A.Ziua) <= CONVERT(date, C.F10023)
                                        LEFT JOIN HOLIDAYS B ON A.Ziua=B.DAY
                                        {5}
                                        WHERE YEAR(A.""Ziua"")={0} AND MONTH(A.""Ziua"")={1} AND (A.""ValStr"" IS NULL OR RTRIM(A.""ValStr"") = '') AND F06204=-1 
                                        AND CONVERT(date, A.Ziua) <= COALESCE(CONVERT(date, ddp.DataPlecare),C.F10023)
                                        {2} {3} {4}";

                        strUp = string.Format(strUp, an, luna, usr, strFiltru, strFiltruZile.Replace("X.", "A."), strInner);
                        //strUp = strUp.Replace("X.", "A.");

                        strFIN += strUp + ";" + "\n\r";
                    }

                    //initializam Ptj_Cumulat
                    string strCum = @"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"")
                                SELECT F10003, {0}, {1}, 1, F10008 + ' ' + F10009 FROM F100 A WHERE F10022 <= {3} AND {2} <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={0} AND ""Luna"" = {1}) {4}";
                    strCum = string.Format(strCum, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), strFiltru);
                    strFIN += strCum + ";" + "\n\r";


                    //initializam Ptj_CumulatIstoric
                    string strIst = @"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT F10003, {0}, {1}, 1, {4}, {5}, {4}, {5} FROM F100 A WHERE F10022 <= {3} AND {2} <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={0} AND ""Luna"" = {1} GROUP BY F10003) {6}";
                    strIst = string.Format(strIst, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), idUser, "GetDate()", strFiltru);
                    strFIN += strIst + ";" + "\n\r";

                    ras = General.ExecutaNonQuery("BEGIN " + strFIN + " END;", null);

                    #endregion
                }
                else
                {

                    #region ORCL
                    string strFIN = "";

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " as Ziua from dual";
                    }

                    if (strZile.Length > 6) strZile = strZile.Substring(6);

                    if (stergePontariAngPlecati == 1)
                    {
                        //Florin 2019.12.05 - am adaugat Ptj_IstoricVal
                        string strDel = $@"
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Echipei - Initializare', {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                FROM ""Ptj_Intrari"" A
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (select f100.F10003, NVL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403 AND modif.data > F100.F10022
                                ) B 
                                ON A.F10003=B.F10003 AND A.""Ziua"" > B.DATA_PLECARII AND {ziInc} <= A.""Ziua"" AND A.""Ziua"" <= {ziSf} AND TRUNC(B.DATA_PLECARII) <> TO_DATE('01-01-2100','DD-MM-YYYY'));

                                DELETE FROM ""Ptj_Intrari"" 
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (select f100.F10003, NVL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403 AND modif.data > F100.F10022
                                ) B 
                                ON A.F10003=B.F10003 AND A.""Ziua"" > B.DATA_PLECARII AND {ziInc} <= A.""Ziua"" AND A.""Ziua"" <= {ziSf} AND TRUNC(B.DATA_PLECARII) <> TO_DATE('01-01-2100','DD-MM-YYYY'));";

                        strFIN += strDel;

                        strDel = $@"
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Echipei - Initializare', {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                FROM ""Ptj_Intrari"" A
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE TRUNC(f10022) <= {ziSf} AND TRUNC(F10022) <> TO_DATE('01-01-2100','DD-MM-YYYY')) B ON A.F10003=B.F10003 AND A.""Ziua"" < B.F10022  AND {ziInc} <= A.""Ziua"" AND A.""Ziua"" <= {ziSf});

                                DELETE FROM ""Ptj_Intrari"" 
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE TRUNC(f10022) <= {ziSf} AND TRUNC(F10022) <> TO_DATE('01-01-2100','DD-MM-YYYY')) B ON A.F10003=B.F10003 AND A.""Ziua"" < B.F10022  AND {ziInc} <= A.""Ziua"" AND A.""Ziua"" <= {ziSf});";

                        strFIN += strDel;
                    }

                    string strFiltruZile = "";
                    if (cuNormaZL) strFiltruZile += @"OR (CASE WHEN (1 + TRUNC (X.""Ziua"") - TRUNC (X.""Ziua"", 'IW')) in (1,2,3,4,5) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSD) strFiltruZile += @"OR (CASE WHEN (1 + TRUNC (X.""Ziua"") - TRUNC (X.""Ziua"", 'IW')) in (6,7) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSL) strFiltruZile += @"OR B.DAY is not null";


                    if (strFiltruZile != "") strFiltruZile = "AND (" + strFiltruZile.Substring(2) + ")";

                    string strFiltruUpdate = "";
                    if (cuNormaZL) strFiltruUpdate += @"OR (CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW')) in (1,2,3,4,5) AND (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)=0 THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSD) strFiltruUpdate += @"OR (CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW')) in (6,7) AND (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)=0 THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSL) strFiltruUpdate += @"OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0";
                    if (strFiltruUpdate != "") strFiltruUpdate = "AND (" + strFiltruUpdate.Substring(2) + ")";

                    string strFiltru = "";

                    //Florin 2019.12.27
                    //if (idContract != -99) strFiltru += @" AND (SELECT ""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= " + ziSf + " AND " + ziInc + " <= TRUNC(B.\"DataSfarsit\")) = " + idContract.ToString();
                    if (denContract != "") strFiltru += @" AND (SELECT MAX(W.""Denumire"") FROM ""F100Contracte"" BINNER JOIN Ptj_Contracte W ON W.Id=B.IdContract  WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= " + ziSf + " AND " + ziInc + " <= TRUNC(B.\"DataSfarsit\")) IN ('" + denContract.Replace(",", "','") + "')";

                    if (idSubcompanie != -99) strFiltru += " AND A.F10004 = " + idSubcompanie.ToString();
                    if (idFiliala != -99) strFiltru += " AND A.F10005 = " + idFiliala.ToString();
                    if (idSectie != -99) strFiltru += " AND A.F10006 = " + idSectie.ToString();

                    //Florin 2019.12.27
                    //if (idDept != -99) strFiltru += " AND A.F10007 = " + idDept.ToString();
                    if (denDept != "") strFiltru += @" AND A.""Dept"" IN ('" + denDept.Replace(",", "','") + "')";

                    string strIns = @"insert into ""Ptj_Intrari""(F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""Parinte"", ""Linia"", F06204, F10002, F10004, F10005, F10006, F10007, F100958, F100959, ""CuloareValoare"", ""Norma"", ""IdContract"", USER_NO, TIME, ""ZiLiberaLegala"", ""F06204Default"", ""IdProgram"", ""ValStr"", ""Val0"", ""In1"", ""Out1"")
                                 {0} {1} {2} {3} ";

                    //Florin 2020.06.30 am modificat 1 cu cuNormaZL
                    strIns = string.Format(strIns, DamiSelectPontajInit(idUser, an, luna, cuNormaZL, cuInOut), strFiltru, strFiltruZile, usr);

                    strFIN += strIns + ";";

                    if (cuCCCu || cuCCFara)
                    {
                        string strFiltruZileCC = "";
                        if (cuNormaZL) strFiltruZileCC += @"OR (CASE WHEN X.""ZiSapt"" in (1,2,3,4,5) AND C.DAY is null THEN 1 ELSE 0 END) = 1";
                        if (cuNormaSD) strFiltruZileCC += @"OR (CASE WHEN X.""ZiSapt"" in (6,7) AND C.DAY is null THEN 1 ELSE 0 END) = 1";
                        if (cuNormaSL) strFiltruZileCC += @"OR C.DAY is not null";
                        if (strFiltruZileCC != "") strFiltruZileCC = "AND (" + strFiltruZileCC.Substring(2) + ")";

                        //Florin 2020.03.27 am transformat NrOre in minute - am adaugat * 60
                        string strInsCC = $@"INSERT INTO ""Ptj_CC""(F10003, ""Ziua"", F06204, ""NrOre1"", ""IdStare"", USER_NO, TIME)
                                            SELECT A.F10003, X.""Zi"", 
                                            CASE WHEN MAX(G.""IdCentruCost"") IS NOT NULL THEN MAX(G.""IdCentruCost"") ELSE CASE WHEN COALESCE(MAX(""DamiCC""(A.F10003, X.""Zi"")), 9999) <> 9999 THEN MAX(""DamiCC""(A.F10003, X.""Zi"")) ELSE MAX(H.F00615) END END AS ""F06204Default"",
                                            ""DamiNorma""(A.F10003, X.""Zi"") * 60 AS ""NrOre1"", 
                                            CASE WHEN COALESCE((SELECT COALESCE(""Valoare"",0) FROM ""tblParametrii"" WHERE ""Nume"" = 'PontajCCcuAprobare'),0) = 0 THEN 3 ELSE 1 END AS ""IdStare"", {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                            FROM (SELECT Z.""Zi"", Z.""ZiSapt"" FROM ""tblzile"" Z WHERE {General.ToDataUniv(an, luna, 1)} <= Z.""Zi"" AND Z.""Zi"" <= {General.ToDataUniv(an, luna, 99)}) X
                                            INNER JOIN F100 A ON 1=1 AND TRUNC(A.F10022) <= TRUNC(X.""Zi"") AND TRUNC(X.""Zi"") <= TRUNC(A.F10023)
                                            LEFT JOIN ""Ptj_CC"" B ON B.""Ziua"" = X.""Zi"" AND B.F10003=A.F10003
                                            LEFT JOIN HOLIDAYS C ON C.DAY = X.""Zi""
                                            LEFT JOIN ""F100CentreCost"" G ON G.F10003 = A.F10003 AND G.""DataInceput"" <= X.""Zi"" AND X.""Zi"" <= G.""DataSfarsit""
                                            LEFT JOIN F006 H ON H.F00607 = ""DamiDept""(A.F10003, X.""Zi"")
                                            WHERE B.F10003 IS NULL
                                            {usr} {strFiltru} {strFiltruZileCC}
                                            GROUP BY A.F10003, X.""Zi"" ";

                        strFIN += strInsCC + ";" + "\n\r";
                    }


                    //Florin 2020.06.30 - s-a adaugat conditia cu norma
                    //actualizam inregistrarile unde norma = null
                    //2015-12-07  s-a adaugat inner join f100 pt a elimina zilele in care angajatii s-au angajat sau au plecat in luna
                    if (cuNormaZL)
                    {
                        string strUp = @"UPDATE ""Ptj_Intrari"" A SET 
                                        A.""ValStr"" = CASE WHEN ((SELECT COUNT(*) FROM F100 Z WHERE Z.F10003=A.F10003 AND TRUNC(F10022) <= TRUNC(""Ziua"") AND TRUNC(""Ziua"") <= TRUNC(F10023) ) = 1 AND TRUNC(A.""Ziua"") <= TRUNC(""DamiDataPlecare""(A.F10003, A.""Ziua""))) THEN CAST(nvl((""DamiNorma""(A.F10003, A.""Ziua"")),(select f10043 from f100 where f10003 = A.F10003)) as int) ELSE null END , 
                                        A.""Val0"" =   CASE WHEN ((SELECT COUNT(*) FROM F100 Z WHERE Z.F10003=A.F10003 AND TRUNC(F10022) <= TRUNC(""Ziua"") AND TRUNC(""Ziua"") <= TRUNC(F10023) ) = 1 AND TRUNC(A.""Ziua"") <= TRUNC(""DamiDataPlecare""(A.F10003, A.""Ziua""))) THEN CAST(nvl((""DamiNorma""(A.F10003, A.""Ziua"")),(select f10043 from f100 where f10003 = A.F10003)) * 60 as int) ELSE null END 
                                        WHERE TO_CHAR(A.""Ziua"",'yyyy')={0} AND TO_CHAR(A.""Ziua"",'mm')={1} AND (A.""ValStr"" IS NULL OR RTRIM(LTRIM(A.""ValStr"")) = '') AND A.F06204=-1 
                                        {2} {3} {4}";

                        strUp = string.Format(strUp, an, luna, usr, strFiltru, strFiltruUpdate.Replace("X.Ziua", "A.\"Ziua\""));
                        //strUp = strUp.Replace("X.ZIUA", "A.\"Ziua\"");

                        strFIN += strUp + ";";
                    }

                    //initializam Ptj_Cumulat
                    string strCum = @"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"")
                                SELECT F10003, {0}, {1}, 1, F10008 || ' ' || F10009 FROM F100 A WHERE F10022 <= TRUNC({3}) AND TRUNC({2}) <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={0} AND ""Luna"" = {1}) {4}";

                    strCum = string.Format(strCum, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), strFiltru);
                    strFIN += strCum + ";";

                    //initializam Ptj_CumulatIstoric
                    string strIst = @"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT F10003, {0}, {1}, 1, {4}, {5}, {4}, {5} FROM F100 A WHERE F10022 <= TRUNC({3}) AND TRUNC({2}) <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={0} AND ""Luna"" = {1} GROUP BY F10003) {6}";
                    strIst = string.Format(strIst, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), idUser, "SYSDATE", strFiltru);
                    strFIN += strIst + ";";

                    ras = General.ExecutaNonQuery("BEGIN " + strFIN + " END;", null);

                    #endregion

                }

                General.CalculFormuleCumulat($@"ent.""An"" = {an} AND ent.""Luna""={luna}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        private static string DamiSelectPontajInit(int idUser, int an, int luna, bool cuNorma = false, int cuInOut = 0)
        {
            //cuNorma
            //0 - PontajInitGlobal
            //1 - PontajInit


            string strSql = "";

            try
            {
                string strZile = "";
                string nrm = @" ,null AS ""ValStr"", null AS ""Val0""";
                string inOut = @" ,null AS ""In1"", null AS ""Out1""";

                string strInner =
                    @"OUTER APPLY dbo.DamiNorma(A.F10003, X.Ziua) dn
						OUTER APPLY dbo.DamiCC(A.F10003, X.Ziua) dc
						OUTER APPLY dbo.DamiDept(A.F10003, X.Ziua) dd
                        OUTER APPLY dbo.DamiDataPlecare(A.F10003, X.Ziua) ddp";
                if (Dami.ValoareParam("TipCalculDate") == "2")
                {
                    strInner =
                        @"LEFT JOIN DamiNorma_Table dn ON dn.F10003=A.F10003 AND dn.dt=X.Ziua
                            LEFT JOIN DamiCC_Table dc ON dc.F10003=A.F10003 AND dc.dt=X.Ziua
                            LEFT JOIN DamiDept_Table dd ON dd.F10003=A.F10003 AND dd.dt=X.Ziua
                            LEFT JOIN DamiDataPlecare_Table ddp ON ddp.F10003=A.F10003 AND ddp.dt=X.Ziua";
                }

                if (Constante.tipBD == 1)
                {
                    if (cuNorma)
                        nrm = @" ,CASE WHEN datepart(dw,X.Ziua)=1 OR datepart(dw,X.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 OR CONVERT(date, X.Ziua) > CONVERT(date, ddp.DataPlecare) THEN CONVERT(int,NULL) ELSE dn.Norma END AS ValStr
                                 ,CASE WHEN datepart(dw,X.Ziua)=1 OR datepart(dw,X.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 OR CONVERT(date, X.Ziua) > CONVERT(date, ddp.DataPlecare) THEN CONVERT(int,NULL) ELSE dn.Norma * 60 END AS Val0";
                    if (cuInOut == 1)
                    {
                        //Florin #773 - am adaugat FirstIn si LastOut
                        inOut = @",(SELECT DATETIMEFROMPARTS(YEAR(X.Ziua), MONTH(X.Ziua), DAY(X.Ziua), DATEPART(HOUR,OraInInitializare), DATEPART(MINUTE,OraInInitializare),0,0) FROM Ptj_Contracte WHERE Id=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"")) AS In1
                                  ,(SELECT DATETIMEFROMPARTS(YEAR(X.Ziua), MONTH(X.Ziua), DAY(X.Ziua), DATEPART(HOUR,OraOutInitializare), DATEPART(MINUTE,OraOutInitializare),0,0) FROM Ptj_Contracte WHERE Id=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"")) AS Out1
                                  ,(SELECT DATETIMEFROMPARTS(YEAR(X.Ziua), MONTH(X.Ziua), DAY(X.Ziua), DATEPART(HOUR,OraInInitializare), DATEPART(MINUTE,OraInInitializare),0,0) FROM Ptj_Contracte WHERE Id=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"")) AS FirstIn
                                  ,(SELECT DATETIMEFROMPARTS(YEAR(X.Ziua), MONTH(X.Ziua), DAY(X.Ziua), DATEPART(HOUR,OraOutInitializare), DATEPART(MINUTE,OraOutInitializare),0,0) FROM Ptj_Contracte WHERE Id=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"")) AS LastOut";
                    }

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " AS Ziua";
                    }


                    //Florin 201.12.02 - am adaugat IdProgram
                    //Radu 04.04.2017 - am modificat F06204Default
                    //Radu 03.02.2021 - am inlocuit DamiSubdept si DamiBirou
                    //OUTER APPLY dbo.DamiSubdept(A.F10003, X.Ziua) sd
                    //OUTER APPLY dbo.DamiBirou(A.F10003, X.Ziua) br

                    strSql = @" SELECT A.F10003, X.Ziua, CASE WHEN datepart(dw,X.Ziua) - 1 = 0 THEN 7 ELSE datepart(dw,X.Ziua) - 1 END AS ZiSapt,
                                CASE WHEN datepart(dw,X.Ziua)=1 OR datepart(dw,X.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 THEN 1 ELSE 0 END AS ZiLibera, 
                                0 as Parinte, 0 as Linia, -1 as F06204, 
                                G.F00603 AS F10002, G.F00604 AS F10004, G.F00605 AS F10005, G.F00606 AS F10006, G.F00607 as F10007, 
                                COALESCE(dd.Subdept, (SELECT C.F100958 FROM F1001 C WHERE C.F10003=A.F10003)) AS F100958, 
                                COALESCE(dd.Birou, (SELECT C.F100959 FROM F1001 C WHERE C.F10003=A.F10003)) AS F100959,
                                '#00FFFFFF' as CuloareValoare, 
                                dn.Norma AS Norma, 
                                (SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"") AS IdContract, 
                                {0} as USER_NO, getdate() as TIME,
                                CASE WHEN B.DAY is not null THEN 1 ELSE 0 END AS ZiLiberaLegala,

                                CASE WHEN (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.ZIUA AND X.ZIUA <= C.""DataSfarsit"") IS NULL THEN 
                                CASE WHEN COALESCE(dc.CC, 9999) <> 9999 THEN dc.CC ELSE (SELECT C.F00615 FROM F006 C WHERE C.F00607 = dd.Dept) END 
                                ELSE (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.ZIUA AND X.ZIUA <= C.""DataSfarsit"") END AS ""F06204Default"",
                                CASE WHEN (CASE WHEN B.DAY is not null THEN 1 ELSE 0 END) = 1 AND Y.""TipSchimb8"" = 1 THEN  COALESCE(Y.""Program8"", Y.""Program0"", -99) ELSE
                                CASE (CASE WHEN datepart(dw,X.""Ziua"") - 1 = 0 THEN 7 ELSE datepart(dw,X.Ziua) - 1 END)
                                WHEN 1 THEN (CASE WHEN COALESCE(Y.""TipSchimb1"",1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 2 THEN (CASE WHEN COALESCE(Y.""TipSchimb2"",1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 3 THEN (CASE WHEN COALESCE(Y.""TipSchimb3"",1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 4 THEN (CASE WHEN COALESCE(Y.""TipSchimb4"",1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 5 THEN (CASE WHEN COALESCE(Y.""TipSchimb5"",1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 6 THEN (CASE WHEN COALESCE(Y.""TipSchimb6"",1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 7 THEN (CASE WHEN COALESCE(Y.""TipSchimb7"",1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"", -99) ELSE -99 END) 
                                END END AS ""IdProgram""
                                {1}
                                {6}
                                FROM ({2}) x
                                inner join F100 A on 1=1 AND CONVERT(date, A.F10022) <= CONVERT(date, X.ZIUA) AND CONVERT(date, X.ZIUA) <= CONVERT(date, A.F10023)
                                left join HOLIDAYS B on X.Ziua=B.DAY
                                left join (select F10003, ""Ziua"", count(*) as CNT from ""Ptj_Intrari"" where YEAR(Ziua)={3} AND MONTH(Ziua)={4} AND F06204=-1 GROUP BY F10003, ""Ziua"") D on D.F10003=A.F10003 AND D.""Ziua"" = x.ZIUA
                                {5}
                                LEFT JOIN F006 G ON G.F00607 = dd.Dept
                                LEFT JOIN F007 H ON H.F00708 = dd.Subdept
                                LEFT JOIN F008 I ON I.F00809 = dd.Birou
                                LEFT JOIN ""Ptj_Contracte"" Y ON Y.""Id""=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"")
                                where isnull(D.CNT,0) = 0";

                }
                else
                {
                    if (cuNorma)
                        nrm = @" ,CASE WHEN (1 + TRUNC (X.""Ziua"") - TRUNC (X.""Ziua"", 'IW'))=6 OR (1 + TRUNC (X.""Ziua"") - TRUNC (X.""Ziua"", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.""Ziua"")<>0 OR TRUNC(X.""Ziua"") > TRUNC(""DamiDataPlecare""(A.F10003, X.""Ziua"")) THEN NULL ELSE ""DamiNorma""(A.F10003, X.""Ziua"") END AS ""ValStr""
                                 ,CASE WHEN (1 + TRUNC (X.""Ziua"") - TRUNC (X.""Ziua"", 'IW'))=6 OR (1 + TRUNC (X.""Ziua"") - TRUNC (X.""Ziua"", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.""Ziua"")<>0 OR TRUNC(X.""Ziua"") > TRUNC(""DamiDataPlecare""(A.F10003, X.""Ziua"")) THEN NULL ELSE ""DamiNorma""(A.F10003, X.""Ziua"") * 60 END AS ""Val0"" ";

                    if (cuInOut == 1)
                    {
                        inOut = @",(SELECT TO_DATE(TO_CHAR(X.""Ziua"", 'DD-MM-YYYY') || ' ' || TO_CHAR(""OraInInitializare"", 'HH24:MI:SS'), 'DD-MM-YYYY HH24:MI:SS') FROM ""Ptj_Contracte"" WHERE ""Id""=(SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.""Ziua"" AND X.""Ziua"" <= B.""DataSfarsit"")) AS ""In1""
                                  ,(SELECT TO_DATE(TO_CHAR(X.""Ziua"", 'DD-MM-YYYY') || ' ' || TO_CHAR(""OraOutInitializare"", 'HH24:MI:SS'), 'DD-MM-YYYY HH24:MI:SS') FROM ""Ptj_Contracte"" WHERE ""Id"" = (SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.""Ziua"" AND X.""Ziua"" <= B.""DataSfarsit"")) AS ""Out1"" ";
                    }
                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " AS \"Ziua\" FROM Dual";
                    }

                    //Radu 04.04.2017 - am modificat F06204Default
                    strSql = @"SELECT A.F10003, X.""Ziua"", (1 + TRUNC(X.""Ziua"") - TRUNC(X.""Ziua"", 'IW')) AS ""ZiSapt"",
                                CASE WHEN (1 + TRUNC(X.""Ziua"") - TRUNC(X.""Ziua"", 'IW'))=6 OR (1 + TRUNC(X.""Ziua"") - TRUNC(X.""Ziua"", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.""Ziua"")<>0 THEN 1 ELSE 0 END AS ""ZiLibera"",
                                0 as ""Parinte"", 0 as ""Linia"", -1 as F06204, 
                                (SELECT C.F00603 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.""Ziua"")) AS F10002,
                                (SELECT C.F00604 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.""Ziua"")) AS F10004,
                                (SELECT C.F00605 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.""Ziua"")) AS F10005,
                                (SELECT C.F00606 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.""Ziua"")) AS F10006,
                                ""DamiDept""(A.F10003, X.""Ziua"") AS F10007,
                                COALESCE(""DamiSubdept""(A.F10003, X.""Ziua""), (SELECT C.F100958 FROM F1001 C WHERE C.F10003=A.F10003)) AS F100958,
                                COALESCE(""DamiBirou""(A.F10003, X.""Ziua""), (SELECT C.F100959 FROM F1001 C WHERE C.F10003=A.F10003)) AS F100958,
                                '#00FFFFFF' as ""CuloareValoare"", 
                                ""DamiNorma""(A.F10003, X.""Ziua"") as ""Norma"", 
                                (SELECT ""IdContract"" FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.""Ziua"" AND X.""Ziua"" <= B.""DataSfarsit"" and ROWNUM <= 1) AS ""IdContract"", 
                                {0} as USER_NO, SYSDATE as TIME,
                                CASE WHEN B.DAY is not null THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",

                                CASE WHEN (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.""Ziua"" AND X.""Ziua"" <= C.""DataSfarsit"" and ROWNUM <= 1) IS NULL THEN
                                CASE WHEN COALESCE(""DamiCC""(A.F10003, X.""Ziua""), 9999) <> 9999 THEN ""DamiCC""(A.F10003, X.""Ziua"") ELSE (SELECT C.F00615 FROM F006 C WHERE C.F00607 = ""DamiDept""(A.F10003, X.""Ziua"")) END 
                                ELSE (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.""Ziua"" AND X.""Ziua"" <= C.""DataSfarsit"" and ROWNUM <= 1) END AS ""F06204Default"",
                                CASE WHEN (CASE WHEN B.DAY is not null THEN 1 ELSE 0 END) = 1 AND Y.""TipSchimb8"" = 1 THEN  COALESCE(Y.""Program8"", Y.""Program0"", -99) ELSE
                                CASE (1 + TRUNC(X.""Ziua"") - TRUNC(X.""Ziua"", 'IW'))
                                WHEN 1 THEN (CASE WHEN COALESCE(Y.""TipSchimb1"",1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 2 THEN (CASE WHEN COALESCE(Y.""TipSchimb2"",1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 3 THEN (CASE WHEN COALESCE(Y.""TipSchimb3"",1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 4 THEN (CASE WHEN COALESCE(Y.""TipSchimb4"",1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 5 THEN (CASE WHEN COALESCE(Y.""TipSchimb5"",1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 6 THEN (CASE WHEN COALESCE(Y.""TipSchimb6"",1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"", -99) ELSE -99 END) 
                                WHEN 7 THEN (CASE WHEN COALESCE(Y.""TipSchimb7"",1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"", -99) ELSE -99 END) 
                                END END AS ""IdProgram""
                                {1}
                                {6}
                                FROM ({2}) x
                                inner join F100 A on 1=1 AND TRUNC(A.F10022) <= TRUNC(X.""Ziua"") AND TRUNC(X.""Ziua"") <= TRUNC(A.F10023)
                                left join HOLIDAYS B on X.""Ziua""=B.DAY
                                left join (select F10003, ""Ziua"", count(*) as CNT from ""Ptj_Intrari"" WHERE TO_NUMBER(TO_CHAR(""Ziua"",'YYYY'))={3} AND TO_NUMBER(TO_CHAR(""Ziua"",'MM'))={4} AND F06204=-1 GROUP BY F10003, ""Ziua"") D on D.F10003=A.F10003 AND D.""Ziua"" = X.""Ziua""
                                LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY CTR.F10003 order by CTR.""F10003"", CTR.""IdContract"" DESC) as ""NrCrt"", CTR.* FROM ""F100Contracte"" CTR) BC ON BC.F10003 = A.F10003 AND BC.""DataInceput"" <= X.""Ziua"" AND X.""Ziua"" <= BC.""DataSfarsit"" AND ""NrCrt"" = 1
                                LEFT JOIN ""Ptj_Contracte"" Y ON Y.""Id"" = BC.""IdContract""
                                WHERE COALESCE(D.CNT,0) = 0 ";
                }

                if (strZile.Length > 6) strZile = strZile.Substring(6);

                strSql = string.Format(strSql, idUser, nrm, strZile, an, luna, strInner, inOut);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


        public static void AddUserIstoric(string idUser, int tip = 1)
        {//Radu 11.11.2019 - idUser se transmite ca parametru
            //tip
            //tip = 1 se salveaza istoric parola logare
            //tip = 2 se salveaza istoric parola fluturas

            try
            {                
                string strSql = $@"INSERT INTO ""ParoleUtilizatorIstoric""(""IdUser"", ""Parola"", ""Data"", USER_NO, TIME)
                        SELECT F70102, F70103, {General.CurrentDate()}, {idUser}, {General.CurrentDate()} FROM ""USERS"" WHERE F70102={idUser}";
                if (tip == 2)
                    strSql = $@"INSERT INTO ""ParoleFluturasIstoric""(""F10003"", ""Parola"", ""Data"", USER_NO, TIME)
                        SELECT F10003, ""Parola"", {General.CurrentDate()}, {idUser}, {General.CurrentDate()} FROM ""USERS"" WHERE F70102={idUser} AND F10003 IS NOT NULL AND F10003 <> -99";

                General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        public static string CreazaCod2FA()
        {
            string ras = "";

            try
            {
                string idUser = General.Nz(HttpContext.Current.Session["UserId"], -99).ToString();
                string mail = General.Nz(ExecutaScalar($@"SELECT ""Mail"" FROM USERS WHERE F70102={idUser}", null), "").ToString();
                if (mail == "")
                    return "Nu exista adresa de mail. Contactati departamentul HR";

                Random rnd = new Random();
                int cod = rnd.Next(100000, 999999);
                string strSql = $@"BEGIN
                                   DELETE FROM GDPR_2FA WHERE ""IdUser""={idUser};
                                   INSERT INTO GDPR_2FA(""IdUser"", ""Cod"", USER_NO, TIME) VALUES({idUser}, '{cod}', {idUser}, {General.CurrentDate()});
                                   END;";
                ExecutaNonQuery(strSql, null);

                TrimiteMail(mail, "", "", "Cod autentificare", "Buna Ziua," + Environment.NewLine + Environment.NewLine + 
                            "Codul dvs. de autentificare este " + cod);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }

            return ras;
        }


        public static void SetTheme()
        {
            try
            {
                string tema = (General.ExecutaScalar(@"SELECT ""Tema"" FROM ""tblConfigUsers"" WHERE F70102=@1", new string[] { HttpContext.Current.Session["UserId"].ToString() }) ?? Constante.DefaultTheme).ToString();

                if (tema.ToString() == "") tema = Constante.DefaultTheme;

                HttpCookie cookie = HttpContext.Current.Request.Cookies[Constante.CurrentThemeCookieKey];
                if (cookie == null)
                {
                    cookie = new HttpCookie(Constante.CurrentThemeCookieKey);
                }

                cookie.Value = tema.ToString();
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void InregistreazaLogarea(int succes, string usr, string motiv = "")
        {
            try
            {
                if (Dami.ValoareParam("SecAuditAuth", "0") == "1")
                {
                    string computerName = "";
                    try
                    {
                        string[] computer_name = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                        //String ecn = System.Environment.MachineName;
                        computerName = computer_name[0].ToString();
                        //computerName = ecn;
                    }
                    catch (Exception) { }

                    DataTable dt = General.IncarcaDT(@"SELECT TOP 0 * FROM ""WT_USERS"" ", null);
                    DataRow dr = dt.NewRow();
                    dr["USER_WIN"] = System.Web.HttpContext.Current.User.Identity.Name.ToString(); 
                    dr["COMPUTER_NAME"] = computerName;
                    dr["USER_WS"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;
                    dr["DATA"] = DateTime.Now;
                    dr["TABELA"] = "USERS";
                    dr["COD_OP"] = "S";
                    dr["NUME_CAMP"] = "F70102";
                    dr["COL_ID1"] = "F70102";
                    dr["VAL_ID1"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;
                    dr["VAL_OLD"] = usr;
                    //dr["VAL_NEW"] = "";
                    dr["LOGARE_REUSITA"] = succes;
                    dr["MOTIV"] = motiv;
                    dt.Rows.Add(dr);
                    General.SalveazaDate(dt, "WT_USERS");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public static void SchimbaInPlanificat(DateTime dtRef, int id, int modifStruc, int modifFunctie, int modifCOR, int modifSalariu)
        {
            try
            {
                //DateTime dtRef = Convert.ToDateTime(txtDtVig.Date).Date;

                DateTime dtLuc = DateTime.Now;
                DataRow drLuc = General.IncarcaDR("SELECT * FROM F010", null);
                if (drLuc.ToString() != "" && drLuc != null && drLuc["F01011"] != null && drLuc["F01012"] != null) dtLuc = new DateTime(Convert.ToInt32(drLuc["F01011"]), Convert.ToInt32(drLuc["F01012"]), 1);

                if (dtRef >= dtLuc || (dtRef.Year == dtLuc.Year && dtRef.Month == dtLuc.Month))
                {
                    string strSql = $@"SELECT A.* 
                        FROM ""Org_relPostAngajat"" A
                        INNER JOIN F100 B ON A.F10003=B.F10003
                        WHERE A.""IdPost""={id} AND B.F100925=0 AND (B.F10025=0 OR B.F10025 = 999)
                        AND CONVERT(date,A.""DataInceput"")<={General.ToDataUniv(dtRef)} AND {General.ToDataUniv(dtRef)} <= CONVERT(date,A.""DataSfarsit"")";

                    DataTable dt = General.IncarcaDT(strSql, null);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];

                        if (modifStruc == 1 || modifFunctie == 1 || modifCOR == 1 || modifSalariu == 1)
                        {
                            if (dr["DataReferinta"] != null && Convert.ToDateTime(dr["DataReferinta"]).Date == Convert.ToDateTime(dtRef.Date).Date)
                            {
                                //pastrez informatia deja introdusa si fac update doar pe ce s-a modificat
                                dr["Stare"] = 2;              //planificat
                                if (modifStruc == 1) dr["modifStructura"] = modifStruc;
                                if (modifFunctie == 1) dr["modifFunctie"] = modifFunctie;
                                if (modifCOR == 1) dr["modifCOR"] = modifCOR;
                                if (modifSalariu == 1) dr["modifSalariu"] = modifSalariu;
                            }
                            else
                            {
                                //introduc noua informatie
                                dr["Stare"] = 2;              //planificat
                                dr["DataReferinta"] = dtRef;
                                dr["modifStructura"] = modifStruc;
                                dr["modifFunctie"] = modifFunctie;
                                dr["modifCOR"] = modifCOR;
                                dr["modifSalariu"] = modifSalariu;
                            }
                        }
                        else
                        {
                            dr["Stare"] = 1;
                            dr["DataReferinta"] = null;
                            dr["modifStructura"] = 0;
                            dr["modifFunctie"] = 0;
                            dr["modifCOR"] = 0;
                            dr["modifSalariu"] = 0;
                        }
                    }

                    General.SalveazaDate(dt, "Org_relPostAngajat");

                    //Florin 2019.10.11
                    string sqlOrg = $@"SELECT TOP 1 * FROM Org_Posturi WHERE Id = " + id;
                    if (Constante.tipBD == 2)
                        sqlOrg = $@"SELECT Z.* FROM ""Org_Posturi"" Z WHERE ROWNUM <=1 AND ""Id"" = " + id;
                    Notif.TrimiteNotificare("PosturiNomen.PosturiLista", (int)Constante.TipNotificare.Notificare, sqlOrg, "Org_Posturi", -99, Convert.ToInt32(HttpContext.Current.Session["UserId"]), Convert.ToInt32(HttpContext.Current.Session["User_Marca"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void CalculCO(int an, int marca = -99, bool cuActualizareInF100 = true)
        {
            try
            {
                string dtInc = an.ToString() + "-01-01";
                string dtSf = an.ToString() + "-12-31";

                //Florin 2021.04.02 - am inlocuit peste tot unde aparea ultima zi din an cu ziua curenta
                //Radu 21.04.2021 - data se citeste cf. param.
                int param = Convert.ToInt32(Dami.ValoareParam("ModCalculZileCOCuveniteDataReferinta", "1"));
                string dtCalcul = "'" + dtSf + "'";
                switch (param)
                {
                    case 1:
                        dtCalcul = "'" + dtSf + "'";
                        break;
                    case 2:
                        dtCalcul = "'" + dtInc + "'";
                        break;
                    case 3:
                        dtCalcul = General.CurrentDate();
                        break;
                }


                string filtruIns = "";
                string f10003 = "-99";

                if (marca != -99)
                {
                    f10003 = marca.ToString();
                    filtruIns = " AND F10003=" + marca;
                }
                else
                    f10003 = "a.F10003";

                //Radu 21.04.2020
                //string strSql = SelectCalculCO(an, f10003, filtruIns);
                //string strSql = "select * from calculCO(" + f10003 + ", CONVERT(date,'" + an + "-12-31'), 1, (SELECT F10072 FROM f100 where f10003=" + f10003 + "))";
                string strSql = "select * from calculCO(" + f10003 + ", CONVERT(date," + dtCalcul + "), 1, (SELECT F10072 FROM f100 where f10003=" + f10003 + "))";
                General.ExecutaNonQuery(strSql, null);


                if (cuActualizareInF100)
                {
                    //Radu 21.04.2020
                    //string strUpd = $@"UPDATE A 
                    //    SET A.F100642 = B.""CuveniteAn"", A.F100995 = B.""Cuvenite"", A.F100996 = B.""SoldAnterior"" 
                    //    FROM F100 A
                    //    INNER JOIN ""Ptj_tblZileCO"" B ON A.F10003 = B.F10003 AND B.""An"" = {an}";
                    //if (Constante.tipBD == 2)
                    //    strUpd = $@"UPDATE F100 A
                    //                SET (A.F100642, A.F100995, A.F100996) =
                    //                  (SELECT B.""CuveniteAn"", B.""Cuvenite"", B.""SoldAnterior""
                    //                   FROM ""Ptj_tblZileCO"" B
                    //                   WHERE A.F10003 = B.F10003 AND B.""An"" = {an})
                    //                WHERE EXISTS(SELECT 1 FROM ""Ptj_tblZileCO"" B WHERE A.F10003 = B.F10003 AND B.""An"" = {an})";

                    //General.ExecutaNonQuery(strUpd, null);
                    if (marca != -99)
                    {
                        if (Constante.tipBD == 1)
                        {
                            //General.ExecutaNonQuery("DECLARE   @f10003 INT,  @zi datetime,  @mod int,     @grila int "
                            //                    + " SELECT TOP 1 @f10003 = " + f10003 + ", @zi = '" + an + "-12-31', @mod = 1, @grila = F10072 FROM F100 WHERE F10003 =  " + f10003
                            //                    + " EXEC CalculCOProc @f10003, @zi, @mod, @grila ", null);
                            General.ExecutaNonQuery("DECLARE   @f10003 INT,  @zi datetime,  @mod int,     @grila int "
                                + " SELECT TOP 1 @f10003 = " + f10003 + ", @zi = " + dtCalcul + ", @mod = 1, @grila = F10072 FROM F100 WHERE F10003 =  " + f10003
                                + " EXEC CalculCOProc @f10003, @zi, @mod, @grila ", null);
                        }
                        else
                        {
                            DataTable dtAng = General.IncarcaDT("SELECT F10072 FROM F100 WHERE F10003 = " + f10003);
                            //General.ExecutaNonQuery("exec \"CalculCOProc\" (" + f10003 + ", TO_DATE('31/12/" + an + "', 'dd/mm/yyyy'), 1, " + dtAng.Rows[0][0].ToString() + ");", null);
                            General.ExecutaNonQuery("exec \"CalculCOProc\" (" + f10003 + ", '" + dtCalcul + "', 1, " + dtAng.Rows[0][0].ToString() + ");", null);
                        }
                    }
                    else
                    {
                        string sql = "";
                        if (Constante.tipBD == 1)
                        {
                            sql = "SELECT F10003 FROM F100 WHERE CONVERT(DATE, F10022) <= '" + dtSf + "' AND '" + dtInc + "' <= CONVERT(DATE, F10023)";
                        }
                        else
                        {
                            dtInc = "01-01-" + an.ToString();
                            dtSf = "31-12-" + an.ToString();
                            sql = "SELECT F10003 FROM F100 WHERE TRUNC(F10022) <= to_date('" + dtSf + "','DD-MM-YYYY') AND to_date('" + dtInc + "','DD-MM-YYYY') <= TRUNC(F10023)";

                        }
                        DataTable dtAng = General.IncarcaDT(sql, null);
                        if (dtAng != null && dtAng.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtAng.Rows.Count; i++)
                            {
                                if (Constante.tipBD == 1)
                                {
                                    //General.ExecutaNonQuery("DECLARE   @f10003 INT,  @zi datetime,  @mod int,     @grila int "
                                    //                    + " SELECT TOP 1 @f10003 = " + dtAng.Rows[i][0].ToString() + ", @zi = '" + an + "-12-31', @mod = 1, @grila = F10072 FROM F100 WHERE F10003 =  " + dtAng.Rows[i][0].ToString()
                                    //                    + " EXEC CalculCOProc @f10003, @zi, @mod, @grila ", null);
                                    General.ExecutaNonQuery("DECLARE   @f10003 INT,  @zi datetime,  @mod int,     @grila int "
                                                        + " SELECT TOP 1 @f10003 = " + dtAng.Rows[i][0].ToString() + ", @zi = " + dtCalcul + ", @mod = 1, @grila = F10072 FROM F100 WHERE F10003 =  " + dtAng.Rows[i][0].ToString()
                                                        + " EXEC CalculCOProc @f10003, @zi, @mod, @grila ", null);
                                }
                                else
                                {
                                    DataTable dtAngajat = General.IncarcaDT("SELECT F10072 FROM F100 WHERE F10003 = " + dtAng.Rows[i][0].ToString());
                                    General.ExecutaNonQuery("exec \"CalculCOProc\" (" + dtAng.Rows[i][0].ToString() + ", TO_DATE('31/12/" + an + "', 'dd/mm/yyyy'), 1, " + dtAngajat.Rows[0][0].ToString() + ");", null);
                                    General.ExecutaNonQuery("exec \"CalculCOProc\" (" + dtAng.Rows[i][0].ToString() + ", " + dtCalcul + ", 1, " + dtAngajat.Rows[0][0].ToString() + ");", null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string SelectCalculCO(int an, string f10003 = "a.F10003", string filtruIns = "", DateTime? F10022 = null, string f10072 = "", string vechime = "", bool esteNou = false)
        {
            string strSql = "";

            try
            {
                string dtInc = an.ToString() + "-01-01";
                string dtSf = an.ToString() + "-12-31";

                //in cazul in care datele nu sunt inca salvate in baza de date, si valoarea campului F10022 se trimite in mod concret din interfata
                string strF10022 = "F10022";
                if (F10022 != null)
                    strF10022 = General.ToDataUniv(F10022);

                string paramVechime = Dami.ValoareParam("MP_VechimeCalculCO", "1");

                string strF10072 = "a.F10072";
                string strVechime = "";
                if (paramVechime == "1")
                    strVechime = "F100644";
                else
                    strVechime = "F100643";
                string strGrila = "case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end";
                if (Constante.tipBD == 2)
                    strGrila = "case when a.F100642 is null or a.F100642 = 0 then c.F02615 else TO_NUMBER(a.F100642) end";
                if (f10072 != "")
                {
                    strF10072 = f10072;
                    strGrila = " c.F02615 ";
                    strVechime = "'" + vechime + "'";
                }

                if (Constante.tipBD == 1)
                {
                    #region SQL
                    //daca nu exista inseram linie goala si apoi updatam
                    if (!esteNou)
                        strSql += "insert into Ptj_tblZileCO(F10003, An, USER_NO, TIME) " +
                        " select F10003, " + an + ", " + HttpContext.Current.Session["UserId"] + ", GetDate() from F100 where F10003 not in (select F10003 from Ptj_tblZileCO where An=" + an + ") " +
                        " and " + strF10022 + " <= '" + dtSf + "' and '" + dtInc + "' <= F10023" + filtruIns + ";";

                    strSql += "with xx as " +
                    " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
                    " (select a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
                    " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107='2100-01-01' then b.f11106 else b.f11107 end  " +
                    " and b.f11105 <= case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end) " +
                    " group by a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end, a.time) t " +
                    " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
                    " case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
                    " union all " +
                    " select f10003 Marca, datainceput de_la_data, datasfarsit la_data " +
                    " from ptj_cereri inner join Ptj_tblAbsente on Ptj_Cereri.IdAbsenta = Ptj_tblAbsente.id " +
                    " where ptj_cereri.IdStare=3 and isnull(Ptj_tblAbsente.AbsenteCFPInCalculCO,0) = 1  ), " +
                    "  yy as " +
                    " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
                    " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                    " group by a.marca, a.de_la_data, a.la_data), " +
                    " f111_2 as  " +
                    " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
                    " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                    " group by a.marca, a.de_la_data, a.la_data) " +
                    " update x set x.Cuvenite = (select y.ZileCuvenite from " +
                    " (select a.F10003, " +
                    " ROUND(( " + strGrila +                                                   //nr zile cuvenite conform grilei
                    " + (CASE WHEN ISNULL(a.F10027,0)>=2 THEN Convert(int,isnull((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile

                    " +  case when dateadd(year,18,f10021) >=CASE WHEN cast(" + strF10022 + " as date) < '" + dtInc + "'  THEN '" + dtInc + "'  ELSE cast(" + strF10022 + " as date) END   " +
                    "  then " +
                    " convert(int, round((datediff(day, " +
                    " (CASE WHEN cast(" + strF10022 + " as date) < '" + dtInc + "'  THEN '" + dtInc + "'  ELSE cast(" + strF10022 + " as date) END) " +
                    "  , (case when  dateadd(year, 18, f10021) <= CASE WHEN cast(f10023 as date) < '" + dtSf + "'  THEN cast(f10023 as date) ELSE '" + dtSf + "'  END " +
                    " then dateadd(year, 18, f10021)  else CASE WHEN cast(f10023 as date) < '" + dtSf + "'  THEN cast(f10023 as date) ELSE '" + dtSf + "'  END end) " +
                    " )+1 ) / convert(float, 365) * Convert(int, isnull((select Valoare from tblParametrii where Nume = 'NrZilePersoanaDizabilitatiSauMaiMica18Ani'), 3)),0)) " +
                    " else 0 end ) " +

                    " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
                    " (datediff(day,(CASE WHEN cast(" + strF10022 + " as date) < '" + dtInc + "' THEN '" + dtInc + "' ELSE cast(" + strF10022 + " as date) END) " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
                    " ,(CASE WHEN cast(f10023 as date) < '" + dtSf + "' THEN cast(f10023 as date) ELSE '" + dtSf + "' END))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
                    " - (SELECT COALESCE(SUM(datediff(d,CASE WHEN F11105 < CONVERT(date,'" + an + "-01-01') THEN CONVERT(date,'" + an + "-01-01') else F11105 END,CASE WHEN F11107 > CONVERT(date,'" + an + "-12-31') THEN CONVERT(date,'" + an + "-12-31') else F11107 END)) + 1,0) from f111_2 Z where f11103=" + f10003 + " and F11105 <= F11107 AND F11107 >= CONVERT(date,'" + an + "-01-01') AND F11105 <= CONVERT(date,'" + an + "-12-31') )" +            //AND (year(F11105)=" + an + " or year(F11107)=" + an + ")
                    " ) " +
                    " /CONVERT(float,365),0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
                    " from F100 a " +
                    " left join (select ISNULL(convert(int,substring(" + strVechime + ",1,2)),0) * 12 + ISNULL(convert(int,substring(" + strVechime + ",3,2)),0) + DATEDIFF (MONTH,  " +
                    " (select convert(nvarchar(4),F01011) + '-' + convert(nvarchar(4),F01012) + '-01' from F010),'" + dtSf + "' " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                    " ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma
                    " left join F026 c on convert(int," + strF10072 + ") = c.F02604 and (convert(int,c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int,c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                    " where " + strF10022 + " <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calculeaza totul pt angajatii activi in anul de referinta
                    " from Ptj_tblZileCO x " +
                    " where x.An=" + an + filtruIns + ";";

                    //la fel ca mai sus - fara ponderea cu nr de zile lucrate in an
                    strSql += "update x set x.CuveniteAn = (select y.ZileCuvenite from " +
                    " (select a.F10003, " +
                    " ( " + strGrila +                                                   //nr zile cuvenite conform grilei
                    " + (CASE WHEN ISNULL(a.F10027,0)>=2 THEN Convert(int,isnull((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile

                    " +  case when dateadd(year,18,f10021) >=CASE WHEN cast(" + strF10022 + " as date) < '" + dtInc + "'  THEN '" + dtInc + "'  ELSE cast(" + strF10022 + " as date) END   " +
                    "  then " +
                    " convert(int, round((datediff(day, " +
                    " (CASE WHEN cast(" + strF10022 + " as date) < '" + dtInc + "'  THEN '" + dtInc + "'  ELSE cast(" + strF10022 + " as date) END) " +
                    "  , (case when  dateadd(year, 18, f10021) <= CASE WHEN cast(f10023 as date) < '" + dtSf + "'  THEN cast(f10023 as date) ELSE '" + dtSf + "'  END " +
                    " then dateadd(year, 18, f10021)  else CASE WHEN cast(f10023 as date) < '" + dtSf + "'  THEN cast(f10023 as date) ELSE '" + dtSf + "'  END end) " +
                    " )+1 ) / convert(float, 365) * Convert(int, isnull((select Valoare from tblParametrii where Nume = 'NrZilePersoanaDizabilitatiSauMaiMica18Ani'), 3)),0)) " +
                    " else 0 end " +

                    " ) as ZileCuvenite " +
                    " from F100 a " +
                    " left join (select ISNULL(convert(int,substring(" + strVechime + ",1,2)),0) * 12 + ISNULL(convert(int,substring(" + strVechime + ",3,2)),0) + DATEDIFF (MONTH,  " +
                    " (select convert(nvarchar(4),F01011) + '-' + convert(nvarchar(4),F01012) + '-01' from F010),'" + dtSf + "' " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                    " ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma
                    " left join F026 c on convert(int," + strF10072 + ") = c.F02604 and (convert(int,c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int,c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                    " where " + strF10022 + " <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
                    " from Ptj_tblZileCO x " +
                    " where x.An=" + an + filtruIns + ";";
                    #endregion
                }
                else
                {
                    #region ORCL
                    dtInc = "01-01-" + an.ToString();
                    dtSf = "31-12-" + an.ToString();

                    //daca nu exista inseram linie goala si apoi updatam
                    if (!esteNou)
                        strSql += "insert into \"Ptj_tblZileCO\"(F10003, \"An\", USER_NO, TIME) " +
                        " select F10003, " + an + ", " + HttpContext.Current.Session["UserId"] + ", SYSDATE from F100 where F10003 not in (select F10003 from \"Ptj_tblZileCO\" where \"An\"=" + an + ") " +
                        " and " + strF10022 + " <= to_date('" + dtSf + "','DD-MM-YYYY') and to_date('" + dtInc + "','DD-MM-YYYY') <= F10023" + filtruIns + ";";

                    strSql += "update \"Ptj_tblZileCO\" x set x.\"Cuvenite\" = ( " +
                            " with xx as " +
                            " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107=to_date('01-01-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
                            " (select a.f11103, a.f11105, case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
                            " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107=to_date('01-01-2100','DD-MM-YYYY') then b.f11106 else b.f11107 end  " +
                            " and b.f11105 <= case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end) " +
                            " group by a.f11103, a.f11105, case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end, a.time) t " +
                            " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
                            " case when f111.f11107=to_date('01-01-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
                            " union all " +
                            " select f10003 Marca, \"DataInceput\" de_la_data, \"DataSfarsit\" la_data " +
                            " from \"Ptj_Cereri\" inner join \"Ptj_tblAbsente\" on \"Ptj_Cereri\".\"IdAbsenta\" = \"Ptj_tblAbsente\".\"Id\" " +
                            " where \"Ptj_Cereri\".\"IdStare\"=3 and COALESCE(\"Ptj_tblAbsente\".\"AbsenteCFPInCalculCO\",0) = 1  ), " +
                            "  yy as " +
                            " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
                            " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                            " group by a.marca, a.de_la_data, a.la_data), " +
                            " f111_2 as  " +
                            " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
                            " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                            " group by a.marca, a.de_la_data, a.la_data) " +
                            " select y.ZileCuvenite from " +
                    " (select a.F10003, " +
                    " ROUND(( " + strGrila +                                                   //nr zile cuvenite conform grilei
                    " + (CASE WHEN NVL(a.F10027,0)>=2 THEN to_number(nvl((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile


                    " + case when add_months(F10021, 216) >= CASE WHEN cast(f10022 as date) < to_date('01-01-" + an + "','DD-MM-YYYY') THEN to_date('01-01-" + an + "','DD-MM-YYYY') ELSE cast(f10022 as date) END " +
                    " then " +
                    " CAST(round(( " +
                    " TRUNC((CASE WHEN cast(f10022 as date) < to_date('01-01-" + an + "','DD-MM-YYYY') THEN to_date('01-01-" + an + "','DD-MM-YYYY') ELSE cast(f10022 as date) END)) " +
                    " - TRUNC((case when  add_months(F10021, 216) <= CASE WHEN cast(f10023 as date) < to_date('31-12-" + an + "','DD-MM-YYYY') THEN cast(f10023 as date) ELSE to_date('31-12-" + an + "','DD-MM-YYYY') END " +
                    " then add_months(F10021, 216)  else CASE WHEN cast(f10023 as date) < to_date('31-12-" + an + "','DD-MM-YYYY') THEN cast(f10023 as date) ELSE to_date('31-12-" + an + "','DD-MM-YYYY') END end)) " +
                    " + 1 ) / CAST(365 as number(10, 2)) * CAST(COALESCE((select \"Valoare\" from \"tblParametrii\" where \"Nume\" = 'NrZilePersoanaDizabilitatiSauMaiMica18Ani'), '3') AS number(10)),0) AS NUMBER(10)) " +
                    " else 0 end) " +


                    " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
                    " (least(trunc(f10023),to_date('31-12-" + an + "','DD-MM-YYYY') " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
                    " ) - greatest(trunc(" + strF10022 + "),to_date('01-01-" + an + "','DD-MM-YYYY'))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
                    " - nvl(b.cfp,0) " +                                                   //scadem zilele de concediu fara plata luate in anul de referinta
                    " - (select COALESCE(SUM(least(trunc(F11107),to_date('31-12-" + an + "','DD-MM-YYYY')) - greatest(trunc(f11105),to_date('01-01-" + an + "','DD-MM-YYYY')) + 1),0) from f111_2 Z where f11103=" + f10003 + " and F11105 <= F11107 AND F11107 >= to_date('01-01-" + an + "','DD-MM-YYYY') AND F11105 <= to_date('31-12-" + an + "','DD-MM-YYYY') ) " +        //AND (to_Char(F11105,'yyyy')='" + an + "' or to_Char(F11107,'yyyy')='" + an + "')
                    " ) " +
                    " /365,0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
                    " from F100 a " +
                    " left join (select nvl(to_number(substr(" + strVechime + ",1,2)),0) * 12 + nvl(to_number(substr(" + strVechime + ",3,2)),0) + trunc(MONTHS_BETWEEN (to_date('31-12-" + an + "','DD-MM-YYYY'), " +
                    " (select to_date('01/' ||  F01012 || '/' ||  F01011,'DD-MM-YYYY') from F010) " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                    " ) + 1 ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma + luna de lucru deschisa pt ca functia MONTHS_BETWEEN nu tine cont de ea
                    " left join F026 c on " + strF10072 + " = c.F02604 and (to_number(c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                    " left join ((select F10003, nvl(sum(least(trunc(\"DataSfarsit\"),to_date('31-12-" + an + "','DD-MM-YYYY')-1) - greatest(trunc(\"DataInceput\"),to_date('01-01-" + an + "','DD-MM-YYYY'))+1),0) as cfp from \"Ptj_Cereri\" where \"IdAbsenta\" in (SELECT \"Id\" from \"Ptj_tblAbsente\" where \"AbsenteCFPInCalculCO\"=1) and \"IdStare\"=3 AND (to_Char(\"DataInceput\",'YYYY') ='" + an + "' OR to_Char(\"DataSfarsit\",'YYYY') ='" + an + "') group by f10003)) b on a.F10003 = b.F10003 " +  //se calcuelaza nr de cfp avute in anul de referinta
                    " where " + strF10022 + " <= to_date('31-12-" + an + "','DD-MM-YYYY') and to_date('01-01-" + an + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
                    " where x.\"An\"=" + an + filtruIns + ";";

                    strSql += "update \"Ptj_tblZileCO\" x set x.\"CuveniteAn\" = (select y.ZileCuvenite from " +
                    " (select a.F10003, " +
                    " ( " + strGrila +                                                  //nr zile cuvenite conform grilei
                    " + (CASE WHEN NVL(a.F10027,0)>=2 THEN to_number(nvl((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile


                    " + case when add_months(F10021, 216) >= CASE WHEN cast(f10022 as date) < to_date('01-01-" + an + "','DD-MM-YYYY') THEN to_date('01-01-" + an + "','DD-MM-YYYY') ELSE cast(f10022 as date) END " +
                    " then " +
                    " CAST(round(( " +
                    " TRUNC((CASE WHEN cast(f10022 as date) < to_date('01-01-" + an + "','DD-MM-YYYY') THEN to_date('01-01-" + an + "','DD-MM-YYYY') ELSE cast(f10022 as date) END)) " +
                    " - TRUNC((case when  add_months(F10021, 216) <= CASE WHEN cast(f10023 as date) < to_date('31-12-" + an + "','DD-MM-YYYY') THEN cast(f10023 as date) ELSE to_date('31-12-" + an + "','DD-MM-YYYY') END " +
                    " then add_months(F10021, 216)  else CASE WHEN cast(f10023 as date) < to_date('31-12-" + an + "','DD-MM-YYYY') THEN cast(f10023 as date) ELSE to_date('31-12-" + an + "','DD-MM-YYYY') END end)) " +
                    " + 1 ) / CAST(365 as number(10, 2)) * CAST(COALESCE((select \"Valoare\" from \"tblParametrii\" where \"Nume\" = 'NrZilePersoanaDizabilitatiSauMaiMica18Ani'), '3') AS number(10)),0) AS NUMBER(10)) " +
                    " else 0 end " +


                    " ) as ZileCuvenite " +
                    " from F100 a " +
                    " left join (select nvl(to_number(substr(" + strVechime + ",1,2)),0) * 12 + nvl(to_number(substr(" + strVechime + ",3,2)),0) + trunc(MONTHS_BETWEEN (to_date('31-12-" + an + "','DD-MM-YYYY'), " +
                    " (select to_date('01/' || F01012 || '/' ||  F01011,'DD-MM-YYYY') from F010) " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                    " ) + 1 ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma + luna de lucru deschisa pt ca functia MONTHS_BETWEEN nu tine cont de ea
                    " left join F026 c on " + strF10072 + " = c.F02604 and (to_number(c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                    " where " + strF10022 + " <= to_date('31-12-" + an + "','DD-MM-YYYY') and to_date('01-01-" + an + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calculeaza totul pt angajatii activi in anul de referinta
                    " where x.\"An\"=" + an + filtruIns + ";";
                    #endregion
                }

                strSql = "BEGIN " + strSql + " END;";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static void ModificaFunctieAngajat(int f10003, int idFunc, DateTime dtInc, DateTime dtSf)
        {
            try
            {
                if (f10003 == -99) return;
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Org_relPostAngajat"" WHERE F10003={f10003} ORDER BY ""DataInceput"" DESC", null);
                int idPost = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT MAX(Id) FROM Org_Posturi WHERE IdFunctie=" + idFunc, null),-99));
                if (idPost == -99) return;

                if (dt == null || dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["IdPost"] = idPost;
                    dr["F10003"] = f10003;
                    dr["Stare"] = 1;
                    dr["DataReferinta"] = dtInc;
                    dr["DataInceput"] = dtInc;
                    dr["DataSfarsit"] = dtSf;
                    dr["USER_NO"] = HttpContext.Current.Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
                else
                {
                    DataRow drOld = dt.Rows[0];

                    DataRow dr = dt.NewRow();
                    dr["IdPost"] = idPost;
                    dr["F10003"] = f10003;
                    dr["Stare"] = 1;
                    dr["DataReferinta"] = dtInc;
                    dr["DataInceput"] = dtInc;
                    dr["DataSfarsit"] = drOld["DataSfarsit"];
                    dr["IdPostVechi"] = drOld["IdPost"];
                    dr["modifFunctie"] = 1;
                    dr["USER_NO"] = HttpContext.Current.Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    dt.Rows.Add(dr);

                    drOld["DataSfarsit"] = dtInc.AddDays(-1);
                }

                SalveazaDate(dt, "Org_relPostAngajat");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        public static string FromDual()
        {
            string rez = "";

            try
            {
                if (Constante.tipBD == 2)
                    rez = " FROM DUAL";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }

            return rez;
        }

        public static void SignOut()
        {
            try
            {
                WSFederationAuthenticationModule.FederatedSignOut(new Uri(WebConfigurationManager.AppSettings["ida:Issuer"]), new Uri(WebConfigurationManager.AppSettings["ADFSRealm"]));
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }
            catch (Exception)
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }
        }

        public static string FiltrulCuNull(string camp)
        {
            string str = "";

            try
            {
                if (camp != "")
                {
                    if (camp.IndexOf(".") >= 0)
                        camp = camp.Replace(".", ".\"");
                    else
                        camp = "\"" + camp;

                    camp = camp + "\"";
                    str = " AND " + camp + " IS NOT NULL ";
                    if (Constante.tipBD == 1)
                        str += $@" AND RTRIM(LTRIM({camp})) <> '' ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }

            return str;
        }

        public static string URLEncode(string expresie)
        {
            string rez = "";

            try
            {
                CriptDecript prc = new CriptDecript();

                string txt = prc.EncryptString(Constante.cheieCriptare, expresie, Constante.ENCRYPT);
                byte[] stream = Encoding.Unicode.GetBytes(txt);
                rez = HttpServerUtility.UrlTokenEncode(stream);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "URLEncode");
            }

            return rez;
        }

        public static string URLDecode(string expresie)
        {
            string rez = "";

            try
            {
                CriptDecript prc = new CriptDecript();

                byte[] stream = HttpServerUtility.UrlTokenDecode(expresie);
                string txt = Encoding.Unicode.GetString(stream);
                rez = prc.EncryptString(Constante.cheieCriptare, txt, Constante.DECRYPT);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "URLDecode");
            }

            return rez;
        }


        private static string TrimiteMailSys()
        {
            string strErr = "";
            string strMsg = "";
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
                    strErr = "Nu exista date despre " + strMsg.Substring(2);
                    return strErr;
                }

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(smtpMailFrom);
                
                string strSql = @"SELECT TOP 1 Mail FROM USERS A
                    LEFT JOIN F100 B ON A.F10003=B.F10003
                    INNER JOIN relGrupUser C ON A.F70102=C.IdUser
                    WHERE COALESCE(A.F70114,0)=0 AND COALESCE(Mail,'') <> '' AND C.IdGrup=0";
                if (Constante.tipBD == 2)
                    strSql = @"SELECT ""Mail"" FROM ""USERS"" A
                        LEFT JOIN F100 B ON A.F10003=B.F10003
                        INNER JOIN ""relGrupUser"" C ON A.F70102=C.""IdUser""
                        WHERE COALESCE(A.F70114,0)=0 AND COALESCE(""Mail"",'') <> '' AND C.""IdGrup""=0 AND ROWNUM<=1";
                DataTable dtMail = IncarcaDT(strSql, null);
                for (int i = 0; i < dtMail.Rows.Count; i++)
                {
                    mm.To.Add(new MailAddress(Nz(dtMail.Rows[i]["Mail"], "").ToString()));
                }
                mm.Bcc.Add("tehnic@WizRom.ro");

                mm.Subject = "WizOne - Important !";
                mm.Body = "Exista o neconcordanta privind securitatea datelor din baza. Va rugam sa luati de urgenta contactul cu administratorul aplicatiei.";
                mm.IsBodyHtml = true;

                smtp = new SmtpClient(smtpServer);
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

                strErr = "";

                smtp.Dispose();
            }
            catch (Exception)
            {
                strErr = "Mailul nu a fost trimis.";
            }

            return strErr;
        }

        public static void StergeInPontaj(int id, int idTipOre, string oreInVal, DateTime dtInc, DateTime dtSf, int f10003, int nrOre, int idUser)
        {
            try
            {

                //Florin 2019.12.05 - am adaugat Ptj_IstoricVal
                if (idTipOre == 0 && oreInVal != "")        //daca este de tip ore refacem varStr
                {
                    string sqlOre = $@"SELECT (CASE WHEN (CASE WHEN (B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL) 
                                    THEN(CASE WHEN 1 = COALESCE(A.""TrimiteLa"", 0) THEN C.""IdTipOre"" ELSE D.""IdTipOre"" END) ELSE B.""IdTipOre"" END)= 0 THEN(CASE WHEN(B.""CompensareBanca"" IS NOT NULL AND B.""CompensarePlata"" IS NOT NULL)
                                    THEN(CASE WHEN 1 = COALESCE(A.""TrimiteLa"", 0) THEN C.""OreInVal"" ELSE D.""OreInVal"" END) ELSE B.""OreInVal"" END) ELSE '' END) AS ValPentruOre
                                    FROM ""Ptj_Cereri"" A
                                    INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                    LEFT JOIN  ""Ptj_tblAbsente"" C ON B.""CompensareBanca"" = C.""Id""
                                    LEFT JOIN  ""Ptj_tblAbsente"" D ON B.""CompensarePlata"" = D.""Id""
                                    WHERE A.""Id"" = {id}";

                    string valPentruOre = General.Nz(General.ExecutaScalar(sqlOre, null), "").ToString();

                    for (DateTime zi = dtInc; zi <= dtSf; zi = zi.AddDays(1))
                    {
                        string valStr = General.CalculValStr(f10003, zi.Date, "", valPentruOre, 0);
                        string sqlStr =
                            $@"BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT F10003, ""Ziua"", {valStr}, ""ValStr"", {idUser}, {General.CurrentDate()}, 'Anulare cerere absenta', {idUser}, {General.CurrentDate()}
                                FROM ""Ptj_Intrari"" 
                                WHERE F10003={f10003} AND ""Ziua"" ={General.ToDataUniv(zi.Date)};

                                UPDATE ""Ptj_Intrari"" SET ""ValStr"" ={valStr}, ""{valPentruOre}"" = NULL WHERE  F10003={f10003} AND ""Ziua"" ={General.ToDataUniv(zi.Date)};
                            END;";
                        General.ExecutaNonQuery(sqlStr, null);
                    }
                }
                else
                {
                    string sqlStr =
                        $@"BEGIN
                            INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                            SELECT F10003, ""Ziua"", NULL, ""ValStr"", {idUser}, {General.CurrentDate()}, 'Anulare cerere absenta', {idUser}, {General.CurrentDate()}
                            FROM ""Ptj_Intrari"" 
                            WHERE F10003 = (SELECT F10003 FROM ""Ptj_Cereri"" WHERE ""Id"" = {id})
                            AND(SELECT ""DataInceput"" FROM ""Ptj_Cereri"" WHERE ""Id"" = {id}) <= ""Ziua""
                            AND ""Ziua"" <= (SELECT ""DataSfarsit"" FROM ""Ptj_Cereri"" WHERE ""Id"" = {id});

                            UPDATE ""Ptj_Intrari"" SET ""ValStr"" = NULL 
                            WHERE F10003 = (SELECT F10003 FROM ""Ptj_Cereri"" WHERE ""Id"" = {id})
                            AND (SELECT ""DataInceput"" FROM ""Ptj_Cereri"" WHERE ""Id"" = {id}) <= ""Ziua""
                            AND ""Ziua"" <= (SELECT ""DataSfarsit"" FROM ""Ptj_Cereri"" WHERE ""Id"" = {id});
                        END;";
                    General.ExecutaNonQuery(sqlStr, null);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "StergeInPontaj");
            }
        }


        public static void CalcSalariu(int tipVenit, object venit, int f10003, out decimal venitCalculat, out string text, DataTable dt = null, int valTichete = 0)
        {
            decimal tmpVB = 0;
            string rezultat = "";

            try
            {
                //tipVenit = 1     VB -> SN
                //tipVenit = 2     SN -> VB

                int i = 0;              //daca ajunge la 100 oprim iteratia ca sa nu devina bucla infinita

                decimal varCas = 10.5m;
                decimal varCass = 5.5m;
                //decimal varSom = 0.5m;
                decimal varSom = 0m;
                decimal varNr = 0;
                decimal scutit = 0;
                decimal tipAng = 1;
                decimal varDed = 250;
                decimal varImp = 16;
                decimal salMediu = 0;

                try
                {
                    //DataTable dt = GetVariabileVB(f10003);
                    if (dt == null)
                        dt = GetVariabileVB(f10003);

                    varCass = Convert.ToDecimal(General.Nz(dt.Rows[0]["CASS"],0));
                    //varSom = lst[1];    nu se mai foloseste
                    varCas = Convert.ToDecimal(General.Nz(dt.Rows[0]["CAS"], 0));
                    varNr = Convert.ToDecimal(General.Nz(dt.Rows[0]["NrDed"], 0));
                    scutit = Convert.ToDecimal(General.Nz(dt.Rows[0]["Scutit"], 0));
                    tipAng = Convert.ToDecimal(General.Nz(dt.Rows[0]["TipAng"], 0));
                    salMediu = Convert.ToDecimal(General.Nz(dt.Rows[0]["SalMediu"], 0));
                    varImp = Convert.ToDecimal(General.Nz(dt.Rows[0]["ProcImp"], 0));
                }
                catch (Exception ex)
                {
                    General.MemoreazaEroarea(ex, "General", "GetVariabileVB");
                }

                if (scutit == 1) varImp = 0;
                if (tipAng == 2) varSom = 0;         //daca este pensionar nu plateste somaj

                if (tipVenit == 1)           //VB -> SN
                {
                    varDed = DamiValDeducere(varNr, Convert.ToDecimal(venit ?? 0) + valTichete);
                    CalcSN(Convert.ToDecimal(venit ?? 0), varCas, varCass, varSom, varImp, varDed, salMediu, valTichete, out tmpVB, out rezultat);
                }
                else                    //SN -> VB
                {
                    decimal SN = Convert.ToDecimal(venit ?? 1);
                    varDed = DamiValDeducere(varNr, SN + valTichete);
                    tmpVB = Math.Round((SN - (1.5m * varImp / 100 * varDed)) / (1 - varImp / 100 - (varImp / 100 * varDed / 2000) - ((1 - varImp / 100) * (varCas + varCass + varSom) / 100)));
                    decimal tmpSN = 0;

                    while (tmpSN != SN)
                    {
                        if (i > 100)
                        {
                            break;
                        }
                        else
                        {
                            i += 1;

                            varDed = DamiValDeducere(varNr, tmpVB + valTichete);
                            CalcSN(tmpVB, varCas, varCass, varSom, varImp, varDed, salMediu, valTichete, out tmpSN, out rezultat);
                            if (tmpSN != SN)
                            {
                                if (tmpSN > SN)
                                    tmpVB -= 1;
                                else
                                    tmpVB += 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "CalcSalariu");
            }

            venitCalculat = tmpVB;
            text = rezultat;
        }

        public static DataTable GetVariabileVB(int f10003)
        {
            DataTable dt = new DataTable();

            try
            {
                string strSql = 
                    $@"SELECT 
                    (SELECT COALESCE(F01324,0) AS ""Valoare"" FROM F013 WHERE F01304=(SELECT F80003 FROM F800 WHERE UPPER(F80002)='ASSB')) AS CASS,
                    (SELECT COALESCE(F01324,0) AS ""Valoare"" FROM F013 WHERE F01304=(SELECT F80003 FROM F800 WHERE UPPER(F80002)='CAS_ANG')) AS CAS,
                    (SELECT COUNT(*) AS ""Valoare"" FROM F110 WHERE F11003=@1 AND F11016=1) AS ""NrDed"",
                    (SELECT COALESCE(F10026,0) AS ""Valoare"" FROM F100 WHERE F10003=@1) AS ""Scutit"",
                    (SELECT COALESCE(F10010,0) AS ""Valoare"" FROM F100 WHERE F10003=@1) AS ""TipAng"",
                    (SELECT F80003 FROM F800 WHERE UPPER(F80002)='SAL_MED') AS ""SalMediu"",
                    (SELECT F80003 FROM F800 WHERE UPPER(F80002)='IMP_ASIG') AS ""ProcImp"" " + General.FromDual();

                dt = IncarcaDT(strSql, new object[] { f10003});
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "GetVariablieVB");
            }

            return dt;
        }

        private static decimal DamiValDeducere(decimal nrPersIntretinere, decimal VB)
        {
            decimal? varDed = 0;

            try
            {
                varDed = Convert.ToDecimal(General.Nz(General.ExecutaScalar(
                            $@"SELECT 
                            CASE WHEN 0={nrPersIntretinere} THEN F73008 ELSE
                            CASE WHEN 1={nrPersIntretinere} THEN F73009 ELSE
                            CASE WHEN 2={nrPersIntretinere} THEN F73010 ELSE
                            CASE WHEN 3={nrPersIntretinere} THEN F73011 ELSE F73012 END END END END 
                            FROM F730 WHERE F73004 <= {Convert.ToInt32(VB)} AND {Convert.ToInt32(VB)} <= F73006", null),0));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "DamiValDeducere");
            }

            return Convert.ToDecimal(varDed ?? 250);
        }


        private static void CalcSN(decimal VB, decimal varCas, decimal varCass, decimal varSom, decimal varImp, decimal varDed, decimal salMediu, int valTichete, out decimal SN, out string rezultat)
        {
            decimal tmpSN = 0m;
            string tmpRezultat = "";

            try
            {
                //teorie:
                //SN = VB - IMP - TAXE
                //TAXE = CAS + CASS + SOM
                //DED = sumafixa * (1-(VB-1000)/2000)
                //IMP = varImp/100 * (VB - TAXE - DED)

                //unde:
                //CAS = round(VB * 10,5/100)
                //CASS = round(VB * 5,5/100)
                //SOM = round(VB * 0,5/100)

                //sumafixa: (este tabel)
                //250 fara pers. in intretinere
                //350 1 pers
                //450 2 pers


                decimal cas = 0;
                decimal cass = 0;
                decimal som = 0;

                //in calculul CAS-ului, daca VB este mai mare decat salariul mediu * 5 ori atunci se plafoneaza la salariul mediu * 5 ori
                //if (VB > (salMediu * 5))
                //    cas = MathExt.Round((salMediu * 5) * varCas / 100, WizOne.Module.MidpointRounding.AwayFromZero);
                //else
                cas = MathExt.Round(VB * varCas / 100, MidpointRounding.AwayFromZero);

                if (0 < cas && cas <= 1) cas = 1;   //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0

                cass = VB * varCass / 100;
                som = VB * varSom / 100;

                if (0 < cass && cass <= 1)          //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0
                    cass = 1;
                else
                    cass = MathExt.Round(VB * varCass / 100, MidpointRounding.AwayFromZero);

                if (0 < som && som <= 1)            //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0
                    som = 1;
                else
                    som = MathExt.Round(VB * varSom / 100, MidpointRounding.AwayFromZero);

                decimal taxe = cas + cass + som;

                decimal ded = varDed;

                //Florin 2019.12.19 - nu se mai foloseste, s-a modificat, doar daca F73013 <> 0 se mai foloseste
                //if (1001 <= VB && VB <= 3000)
                //{
                //    //ded = Math.Round((varDed * (1 - (VB - 1000) / 2000)), 0);
                //    ded = (varDed * (1 - (VB - 1000) / 2000));
                //    ded = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(ded / 10)) * 10);
                //}

                decimal imp = MathExt.Round(varImp / 100 * ((VB + valTichete) - taxe - ded), MidpointRounding.AwayFromZero);
                if (imp < 0) imp = 0;

                tmpSN = MathExt.Round((VB - imp - taxe), MidpointRounding.AwayFromZero);
                tmpRezultat = "CAS=" + cas + ";CASS=" + cass + ";Deducere=" + ded + ";Impozit=" + imp;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "CalcSN");
            }

            SN = tmpSN;
            rezultat = tmpRezultat;

        }

        public static void SintaxaValStr()
        {
            try
            {
                string tipAfisare = Dami.ValoareParam("TipAfisareOre", "1");
                string masca = "";

                string substr = "SUBSTRING";
                string conversie = "nvarchar";
                int poz = 2;
                if (Constante.tipBD == 2)
                {
                    substr = "SUBSTR";
                    conversie = "varchar2";
                    poz = 3;
                }

                switch (tipAfisare)
                {
                    case "1":
                        masca = $"'CASE WHEN COALESCE(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\",0)=0 THEN '''' ELSE COALESCE(''/'' {Dami.Operator()} CAST(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\"/60 AS {conversie}(10)) {Dami.Operator()} ''' {Dami.Operator()} COALESCE(\"DenumireScurta\",'') {Dami.Operator()} ''','''') END'";
                        if (Constante.tipBD == 2)
                            masca = $"'CASE WHEN COALESCE(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\",0)=0 THEN '''' ELSE COALESCE(''/'' {Dami.Operator()} CAST(TRUNC(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\"/60) AS {conversie}(10)) {Dami.Operator()} ''' {Dami.Operator()} COALESCE(\"DenumireScurta\",'') {Dami.Operator()} ''','''') END'";
                        break;
                    case "2":
                        masca = $"'CASE WHEN COALESCE(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\",0)=0 THEN '''' ELSE COALESCE(''/'' {Dami.Operator()} CAST(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\" / 60 + CAST(CAST((\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\" % 60) as decimal(18,2)) / 100 AS DECIMAL(18,2)) AS {conversie}(10)) {Dami.Operator()} ''' {Dami.Operator()} COALESCE(\"DenumireScurta\",'') {Dami.Operator()} ''','''') END'";
                        if (Constante.tipBD == 2)
                            masca = $"'CASE WHEN COALESCE(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\",0)=0 THEN '''' ELSE COALESCE(''/'' {Dami.Operator()} CAST(TRUNC(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\" / 60) AS {conversie}(10)) {Dami.Operator()} ''.'' {Dami.Operator()} REPLACE(CAST(CAST(MOD(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\" , 60) AS number(18,2))/100 AS {conversie}(10)),'','','''') {Dami.Operator()} ''' {Dami.Operator()} COALESCE(\"DenumireScurta\",'') {Dami.Operator()} ''','''') END'";
                        break;
                    case "3":
                        masca = $"'CASE WHEN COALESCE(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\",0)=0 THEN '''' ELSE COALESCE(''/'' {Dami.Operator()} CAST(CAST(ROUND(CAST(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\" AS decimal) / 60,2) as decimal(18,2)) AS {conversie}(10)) {Dami.Operator()} ''' {Dami.Operator()} COALESCE(\"DenumireScurta\",'') {Dami.Operator()} ''','''') END'";
                        if (Constante.tipBD == 2)
                            masca = $"'CASE WHEN COALESCE(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\",0)=0 THEN '''' ELSE COALESCE(''/'' {Dami.Operator()} CAST(CAST(TRUNC(CAST(\"' {Dami.Operator()} \"OreInVal\" {Dami.Operator()} '\" AS number) / 60,2) as number(18,2)) AS {conversie}(10)) {Dami.Operator()} ''' {Dami.Operator()} COALESCE(\"DenumireScurta\",'') {Dami.Operator()} ''','''') END'";
                        break;
                }

                string sqlVal = $@"SELECT CONVERT(nvarchar(max),(SELECT '{Dami.Operator()}' {Dami.Operator()} {masca} FROM ""Ptj_tblAbsente"" WHERE ""OreInVal"" IS NOT NULL GROUP BY ""OreInVal"", ""DenumireScurta"" ORDER BY CAST(REPLACE(""OreInVal"", 'Val','') AS int) For XML PATH (''))) ";
                if (Constante.tipBD == 2)
                    sqlVal = $@"SELECT LISTAGG('{Dami.Operator()}' {Dami.Operator()} {masca}) WITHIN GROUP (ORDER BY CAST(REPLACE(""OreInVal"", 'Val','') AS int)) FROM (SELECT ""OreInVal"", ""DenumireScurta"" FROM ""Ptj_tblAbsente"" WHERE ""OreInVal"" IS NOT NULL GROUP BY ""OreInVal"", ""DenumireScurta"")";

                string strVal = (General.ExecutaScalar(sqlVal, null) ?? "").ToString();

                if (strVal != "")
                {
                    strVal = substr + "(" + strVal.Substring(poz - 1) + ",2,500)";
                    strVal = strVal.Replace("'", "''");
                    strVal = $@"CASE WHEN (SELECT COUNT(*) FROM ""Ptj_tblAbsente"" BS WHERE BS.""DenumireScurta""=""ValStr"" {FiltrulCuNull("BS.DenumireScurta").Replace("'","''")}) = 0 THEN {strVal} ELSE ""ValStr"" END ";
                    string strSql = $@"
                        IF ((SELECT COUNT(*) FROM ""tblParametrii"" WHERE ""Nume""='SintaxaValStr') = 0)
                        INSERT INTO ""tblParametrii""(""Nume"", ""Valoare"", ""Explicatie"", ""IdModul"", USER_NO, TIME) VALUES('SintaxaValStr', '{strVal}', 'Este formula prin care se creaza ValStr in pontaj', 2, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()})
                        ELSE
                        UPDATE ""tblParametrii"" SET ""Valoare""='{strVal}' WHERE ""Nume""='SintaxaValStr'";
                    if (Constante.tipBD == 2)
                        strSql = $@"
                            DECLARE cnt number;
                            BEGIN
                                SELECT COUNT(*) into cnt FROM ""tblParametrii"" WHERE ""Nume""='SintaxaValStr';
                                IF(cnt = 0) THEN
                                    INSERT INTO ""tblParametrii""(""Nume"", ""Valoare"", ""Explicatie"", ""IdModul"", USER_NO, TIME) VALUES('SintaxaValStr', '{strVal}', 'Este formula prin care se creaza ValStr in pontaj', 2, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()});
                                ELSE
                                    UPDATE ""tblParametrii"" SET ""Valoare""='{strVal}' WHERE ""Nume""='SintaxaValStr';
                                END IF;
                            END; ";
                    General.ExecutaNonQuery(strSql, null);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", "SintaxaValStr");
            }
        }

        public static string VerificareDepasireNorma(int f10003, DateTime dtInc, int? nrMinute, int tip)
        {
            //tip
            //tip - 1  vine din cererei - unde trebuie sa luam in caclul si valorile care deja exista in pontaj
            //tip - 2  vine din pontaj  - valorile sunt deja in pontaj

            string msg = "";

            try
            {
                //calculam norma
                string strSql = "SELECT Norma FROM DamiNorma(" + f10003 + "," + General.ToDataUniv(dtInc) + ")";
                if (Constante.tipBD == 2) strSql = "SELECT \"DamiNorma\"(" + f10003 + ", " + General.ToDataUniv(dtInc) + ") FROM DUAL";
                int norma = Convert.ToInt32(General.ExecutaScalar(strSql, null));

                int sumaPtj = 0;
                if (tip == 1)
                {
                    //absentele din pontaj care intra in suma de ore
                    string sqlOre = @"SELECT ' + COALESCE(' + OreInVal + ',0)'  FROM Ptj_tblAbsente WHERE COALESCE(VerificareNrMaxOre,0) = 1 FOR XML PATH ('')";
                    if (Constante.tipBD == 2) sqlOre = @"SELECT LISTAGG('COALESCE(' || ""OreInVal"" || ')', ' + ') WITHIN GROUP (ORDER BY ""OreInVal"") FROM ""Ptj_tblAbsente"" WHERE COALESCE(VerificareNrMaxOre,0) = 1";
                    string strVal = (General.ExecutaScalar(sqlOre, null) ?? "").ToString();
                    if (Constante.tipBD == 1) strVal = strVal.Substring(3);
                    if (strVal != "") sumaPtj = Convert.ToInt32(General.ExecutaScalar($@"SELECT COALESCE(SUM({strVal}), 0) FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(dtInc.Date)}", null));
                }

                //suma de ore din Cereri
                int sumaCere = Convert.ToInt32(General.ExecutaScalar($@"SELECT COALESCE(SUM(COALESCE(""NrOre"",0)),0) FROM ""Ptj_Cereri"" WHERE F10003={f10003} AND ""DataInceput"" = {General.ToDataUniv(dtInc.Date)} AND ""IdStare"" IN (1,2)", null));
                if (((sumaCere * 60) + sumaPtj + nrMinute) > (norma * 60))
                {
                    msg = "Totalul de ore depaseste norma pe aceasta zi";
                    return msg;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", "VerificareDepasireNorma");
            }

            return msg;
        }

        public static void ExecValStr(int f10003, DateTime ziua)
        {
            try
            {
                ExecutaNonQuery($@"UPDATE ""Ptj_Intrari"" SET ""ValStr""={Dami.ValoareParam("SintaxaValStr", "")} WHERE F10003=@1 AND ""Ziua""=@2", new object[] { f10003, ziua });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", "ExecValStr");
            }
        }

        public static void ExecValStr(string filtru)
        {
            try
            {
                ExecutaNonQuery($@"UPDATE ""Ptj_Intrari"" SET ""ValStr""={Dami.ValoareParam("SintaxaValStr", "")} WHERE {filtru}", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", "ExecValStr");
            }
        }


        public static void CalculFormulePeZi(string filtru)
        {
            try
            {
                string strSql = "";
                DataTable dt = IncarcaDT(@"SELECT * FROM ""Ptj_tblFormule"" WHERE ""Pagina"" = 'Pontaj.PontajPeAng' AND ""Control"" = 'grDate' " + FiltrulCuNull("FormulaSql") + FiltrulCuNull("Coloana") + @" ORDER BY ""Ordine"" ", null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    if (row["FormulaSql"].ToString().ToLower().IndexOf("delete") >= 0 || row["FormulaSql"].ToString().ToLower().IndexOf("insert") >= 0 || row["FormulaSql"].ToString().ToLower().IndexOf("update") >= 0 || row["FormulaSql"].ToString().ToLower().IndexOf("drop") >= 0)
                    {
                        TrimiteMailSys();
                        continue;
                    }

                    if (Constante.tipBD == 1)
                        strSql += @"UPDATE ent
                        SET ent.{0}=({1})
                        FROM Ptj_Intrari ent
                        WHERE {2};" + Environment.NewLine;
                    else
                        strSql += @"UPDATE ""Ptj_Intrari"" ent 
                        SET ent.""{0}""=({1}) 
                        WHERE {2};" + Environment.NewLine;

                    strSql = string.Format(strSql, row["Coloana"].ToString(), row["FormulaSql"].ToString(), filtru);
                }

                if (strSql != "")
                {
                    string sqlCum = "BEGIN" + "\n\r" + strSql + "\n\r" + "END;";
                    ExecutaNonQuery(sqlCum, null);
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "Calcul", "CalculFormulePeZi");
            }
        }

        public static void CalculFormuleCumulat(string filtru)
        {
            string sqlCum = "";

            try
            {
                string strSql = "";
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE 1=1 {General.FiltrulCuNull("CampSelect")} ORDER BY ""Ordine"" ", null);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    if (General.Nz(row["Coloana"], "").ToString() != "" && General.Nz(row["CampSelect"], "").ToString() != "")
                    {
                        if (row["Coloana"].ToString().ToLower().IndexOf("delete") >= 0 || row["Coloana"].ToString().ToLower().IndexOf("insert") >= 0 || row["Coloana"].ToString().ToLower().IndexOf("update") >= 0 || row["Coloana"].ToString().ToLower().IndexOf("drop") >= 0)
                        {
                            TrimiteMailSys();
                            continue;
                        }

                        if (Constante.tipBD == 1)
                            strSql += $@"UPDATE ent 
                            SET ent.{row["Coloana"]} = ({row["CampSelect"]}) 
                            FROM ""Ptj_Cumulat"" ent
                            WHERE {filtru};" + Environment.NewLine;
                        else
                            strSql += $@"UPDATE ""Ptj_Cumulat"" ent 
                            SET ent.{row["Coloana"]} = ({row["CampSelect"]}) 
                            WHERE {filtru};" + Environment.NewLine;
                    }
                }

                if (strSql != "")
                {
                    sqlCum = "BEGIN" + "\n\r" + strSql + "\n\r" + "END;";
                    General.ExecutaNonQuery(sqlCum, null);

                    if (Dami.ValoareParam("LogFormuleCumulat") == "1") General.CreazaLogFormuleCumulat(sqlCum, "PontajDetaliat");
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "Calcul", "CalculFormuleCumulat");
                MemoreazaEroarea(sqlCum, "Calcul", "CalculFormuleCumulat");
            }
        }

        public static void CalculFormule(object marcaInc, object marcaSf = null, DateTime? ziuaInc = null, DateTime? ziuaSf = null)
        {
            try
            {
                string filtru = "";
                string filtruPeZi = "";
                string filtruCumulat = "";
                if (marcaInc != null && marcaSf == null) filtru += " AND ent.F10003=" + marcaInc;
                if (marcaInc != null && marcaSf != null) filtru += " AND " + marcaInc + " <= ent.F10003 AND ent.F10003 <= " + marcaSf;
                if (ziuaInc != null && ziuaSf == null) filtruPeZi += " AND " + General.TruncateDate("Ziua") + "=" + General.ToDataUniv(ziuaInc);
                if (ziuaInc != null && ziuaSf != null) filtruPeZi += " AND " + General.ToDataUniv(ziuaInc) + " <= " + General.TruncateDate("Ziua") + " AND " + General.TruncateDate("Ziua") + " <= " + General.ToDataUniv(ziuaSf);
                if (ziuaInc != null && ziuaSf == null) filtruCumulat += " AND ent.\"An\"=" + Convert.ToDateTime(ziuaInc).Year + " AND ent.\"Luna\"=" + Convert.ToDateTime(ziuaInc).Month;
                if (ziuaInc != null && ziuaSf != null) filtruCumulat += $@" AND {Convert.ToDateTime(ziuaInc).Year * 100 + Convert.ToDateTime(ziuaInc).Month} <= (ent.""An"" * 100 + ent.""Luna"") AND (ent.""An"" * 100 + ent.""Luna"") <= {Convert.ToDateTime(ziuaSf).Year * 100 + Convert.ToDateTime(ziuaSf).Month}";

                CalculFormulePeZi(" 1=1 " + filtru + filtruPeZi);
                CalculFormuleCumulat(" 1=1 " + filtru + filtruCumulat);
                ExecValStr("1=1 " + filtru.Replace("ent.","") + filtruPeZi);
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "Calcul", "CalculFormule");
            }
        }

        //Radu 03.03.2020
        public static void TransferTranzactii(string marca, string cod, DateTime dataInceput, DateTime dataSfarsit, DateTime dataIncetare)
        {
            DateTime date1 = DamiDataLucru();
            DateTime date2 = DamiDataLucru().AddMonths(1).AddDays(-1);
            DateTime szdI1 = DamiDataLucru(), szdI2 = date2, datasftranz = new DateTime(2100, 1, 1);

            try
            {
                if (dataInceput <= date2 && dataSfarsit >= date1 && dataIncetare >= date1)  // sunt in luna curenta
                {
                    int datachange = 0;
                    if (dataInceput > date1)
                    {
                        szdI1 = dataInceput;
                        datachange = 1;
                    }
                    if (dataIncetare <= date2)
                    {
                        dataIncetare = dataIncetare.AddDays(-1);
                        szdI2 = dataIncetare;
                        datachange = 1;
                    }
                    else if (dataSfarsit < date2)
                    {
                        szdI2 = dataSfarsit;
                        datachange = 1;
                    }

                    string sql = "SELECT 1, F30038 FROM F300 WHERE F30010 = {0} AND F30037 = {1} AND F30003 = {2}";
                    sql = string.Format(sql, cod, General.ToDataUniv(szdI1), marca);
                    DataTable dtVerif = IncarcaDT(sql, null);
                    int continua = 0;
                    if (dtVerif != null && dtVerif.Rows.Count > 0)
                    {
                        continua = Convert.ToInt32(dtVerif.Rows[0][0].ToString());
                        datasftranz = Convert.ToDateTime(dtVerif.Rows[0][1].ToString());
                    }

                    if (continua != 1)      // nu exista tranzactia
                    {

                        //calendarul pt luna curenta
                        DataTable dtCalendar = IncarcaDT("SELECT * FROM F069 WHERE F06904 = (SELECT F01011 FROM F010) AND F06905 = (SELECT F01012 FROM F010)", null);

                        if (dtCalendar == null || dtCalendar.Rows.Count <= 0) return;

                        int nZileLucratoare = Convert.ToInt32(dtCalendar.Rows[0]["F06907"].ToString());

                        if (datachange == 1)
                        {
                            // sarbatori legale
                            List<int> arlHolidays = new List<int>();

                            // incarc sarbatorile legale:
                            int nI = 0;
                            sql = "SELECT * FROM HOLIDAYS WHERE MONTH(DAY) = {0} AND YEAR(DAY) = {1}";
                            if (Constante.tipBD == 2)
                                sql = "SELECT * FROM HOLIDAYS WHERE EXTRACT(MONTH FROM DAY) = {0} AND EXTRACT(YEAR FROM DAY) = {1}";
                            sql = string.Format(sql, DamiDataLucru().Month, DamiDataLucru().Year);
                            DataTable dtHolidays = IncarcaDT(sql, null);
                            if (dtHolidays != null && dtHolidays.Rows.Count > 0)
                                for (int i = 0; i < dtHolidays.Rows.Count; i++)
                                    arlHolidays.Add(Convert.ToDateTime(dtHolidays.Rows[i]["DAY"].ToString()).Day);


                            for (nI = 1; nI < szdI1.Day; nI++)
                            {
                                DateTime odtTmp = new DateTime(DamiDataLucru().Year, DamiDataLucru().Month, nI);
                                if (!arlHolidays.Contains(odtTmp.Day) && odtTmp.DayOfWeek != DayOfWeek.Saturday && odtTmp.DayOfWeek != DayOfWeek.Sunday)
                                    nZileLucratoare--;
                            }

                            for (nI = szdI2.Day + 1; nI <= date2.Day; nI++)
                            {
                                DateTime odtTmp = new DateTime(DamiDataLucru().Year, DamiDataLucru().Month, nI);
                                if (!arlHolidays.Contains(odtTmp.Day) && odtTmp.DayOfWeek != DayOfWeek.Saturday && odtTmp.DayOfWeek != DayOfWeek.Sunday)
                                    nZileLucratoare--;
                            }
                        }


                        sql = "INSERT INTO F300 (F30001, F30002, F30003, F30004, F30005, F30006, F30007, F30010, F30011, F30013, F30035, F30036, F30037, F30038, F30050, F300603, F300611, USER_NO, TIME,"
                             + " F30012, F30014, F30015, F30021, F30022, F30023, F30039, F30040, F30041, F30044, F30045, F30046, F30051, F30053, F300612, F300613, F300614, F30054, F30042)"
                             + " SELECT 300, F10002, F10003, F10004, F10005, F10006, F10007, {0},  1, {1}, {2}, {2}, "
                             + " {3}, {4}, CASE WHEN F10053 IS NULL OR F10053=0 THEN F00615 ELSE F10053 END, "
                             + " {2}, 1, {5}, {6}, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'Tranzactie din Suspendari'"
                             + " FROM F100, F006 WHERE F10003 = {7} AND F10007=F00607";
                        sql = string.Format(sql, cod, nZileLucratoare, ToDataUniv(date1), ToDataUniv(szdI1), ToDataUniv(szdI2), HttpContext.Current.Session["UserId"].ToString(),
                                    (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), marca);
                        ExecutaNonQuery(sql, null);
                    }
                    else if (datasftranz != szdI2)
                    {
                        //calendarul pt luna curenta
                        DataTable dtCalendar = IncarcaDT("SELECT * FROM F069 WHERE F06904 = (SELECT F01011 FROM F010) AND F06905 = (SELECT F01012 FROM F010)", null);

                        if (dtCalendar == null || dtCalendar.Rows.Count <= 0) return;

                        int nZileLucratoare = Convert.ToInt32(dtCalendar.Rows[0]["F06907"].ToString());

                        if (datachange == 1)
                        {
                            // sarbatori legale
                            List<int> arlHolidays = new List<int>();

                            // incarc sarbatorile legale:
                            int nI = 0;
                            sql = "SELECT * FROM HOLIDAYS WHERE MONTH(DAY) = {0} AND YEAR(DAY) = {1}";
                            if (Constante.tipBD == 2)
                                sql = "SELECT * FROM HOLIDAYS WHERE EXTRACT(MONTH FROM DAY) = {0} AND EXTRACT(YEAR FROM DAY) = {1}";
                            sql = string.Format(sql, DamiDataLucru().Month, DamiDataLucru().Year);
                            DataTable dtHolidays = IncarcaDT(sql, null);
                            if (dtHolidays != null && dtHolidays.Rows.Count > 0)
                                for (int i = 0; i < dtHolidays.Rows.Count; i++)
                                    arlHolidays.Add(Convert.ToDateTime(dtHolidays.Rows[i]["DAY"].ToString()).Day);

                            for (nI = 1; nI < szdI1.Day; nI++)
                            {
                                DateTime odtTmp = new DateTime(DamiDataLucru().Year, DamiDataLucru().Month, nI);
                                if (!arlHolidays.Contains(odtTmp.Day) && odtTmp.DayOfWeek != DayOfWeek.Saturday && odtTmp.DayOfWeek != DayOfWeek.Sunday)
                                    nZileLucratoare--;
                            }

                            for (nI = szdI2.Day + 1; nI <= date2.Day; nI++)
                            {
                                DateTime odtTmp = new DateTime(DamiDataLucru().Year, DamiDataLucru().Month, nI);
                                if (!arlHolidays.Contains(odtTmp.Day) && odtTmp.DayOfWeek != DayOfWeek.Saturday && odtTmp.DayOfWeek != DayOfWeek.Sunday)
                                    nZileLucratoare--;
                            }

                        }

                        sql = "UPDATE F300 SET F30013 = {0}, F30038 = {1}, USER_NO = {2}, TIME = {3}"
                            + " WHERE F30010 = {4} AND F30037 = {5} AND F30003 = {6}";
                        sql = string.Format(sql, nZileLucratoare, ToDataUniv(szdI2), HttpContext.Current.Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), cod, ToDataUniv(szdI1), marca);
                        ExecutaNonQuery(sql, null);

                        sql = "UPDATE F111 SET F11112 = 2 WHERE F11105 = {0} AND F11106 = {1} AND F11103 = {2} AND F11112 = 1";
                        sql = string.Format(sql, ToDataUniv(dataInceput), ToDataUniv(dataSfarsit), marca);
                        ExecutaNonQuery(sql, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "TransferTranzactii");
            }
        }

        public static void TransferPontaj(string marca, DateTime dataInceput, DateTime dataSfarsit, DateTime dataIncetare, string denScurta, DateTime dtIncetareVeche)
        {
            try
            {
                DateTime dtSf = (dataIncetare == new DateTime(2100, 1, 1) ? dataSfarsit : dataIncetare.AddDays(-1));

                string strSql = "";
                int idAbs = -99;
                DataTable dtAbsNomen = General.IncarcaDT("SELECT * FROM \"Ptj_tblAbsente\" WHERE \"DenumireScurta\" = '" + denScurta + "'", null);
                if (dtAbsNomen != null && dtAbsNomen.Rows.Count > 0)
                    idAbs = Convert.ToInt32(dtAbsNomen.Rows[0]["Id"].ToString());
                else
                    return;

                string sql = "DELETE FROM  \"Ptj_IstoricVal\" WHERE F10003 = " + marca + " AND  \"Ziua\" BETWEEN " + General.ToDataUniv(dataInceput.Date) + " AND " + General.ToDataUniv(dtSf.Date);
                ExecutaNonQuery(sql, null);

                //#311 si #344
                string sqlIst = $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", USER_NO, TIME, ""Observatii"") 
                                        SELECT {marca}, ""Zi"", ""DenumireScurta"", (SELECT ""ValStr"" FROM ""Ptj_Intrari"" WHERE F10003 = {marca} AND ""Ziua"" = ""Zi""), 
                                {HttpContext.Current.Session["UserId"].ToString() }, {General.CurrentDate()}, {HttpContext.Current.Session["UserId"].ToString() }, {General.CurrentDate()}, 'Transfer din Suspendari'
                                    FROM 
                                    (select case when (SELECT count(*) FROM ""Ptj_Intrari"" WHERE f10003 = {marca} and ""Ziua"" = a.""Zi"") = 0 then 0 else 1 end as prezenta, 
                                    b.* from  ""tblZile"" a
                                    left join 
                                    (SELECT P.""Zi"",
                                    CASE WHEN COALESCE(b.SL,0) <> 0 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) = 1 THEN 1 ELSE
                                    CASE WHEN COALESCE(b.ZL,0) <> 0 AND P.""ZiSapt"" < 6 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) <> 1 THEN 1 ELSE 
                                    CASE WHEN COALESCE(b.S,0) <> 0 AND P.""ZiSapt"" = 6 THEN 1 ELSE 
                                    CASE WHEN COALESCE(b.D,0) <> 0 AND P.""ZiSapt"" = 7 THEN 1 ELSE 0 
                                    END
                                    END
                                    END
                                    END AS ""AreDrepturi"", ""DenumireScurta""
                                    FROM ""tblZile"" P
                                    INNER JOIN ""Ptj_tblAbsente"" A ON 1=1
                                    INNER JOIN ""Ptj_ContracteAbsente"" B ON A.""Id"" = B.""IdAbsenta""
                                    LEFT JOIN HOLIDAYS D on P.""Zi""=D.DAY
                                    WHERE { General.ToDataUniv(dataInceput.Date)} <= CAST(P.""Zi"" AS date) AND CAST(P.""Zi"" AS date) <=  {General.ToDataUniv(dtSf.Date)}
                                    AND A.""Id"" = {idAbs}
                                    AND COALESCE(A.""DenumireScurta"", '~') <> '~'
                                    AND B.""IdContract"" = (SELECT MAX(""IdContract"") FROM ""F100Contracte"" WHERE F10003 = {marca} AND ""DataInceput"" <= { General.ToDataUniv(dataInceput.Date)} AND {General.ToDataUniv(dtSf.Date)} <= ""DataSfarsit"") 
                                    ) b
                                    on a.""Zi"" = b.""Zi""

                                    where a.""Zi"" between  { General.ToDataUniv(dataInceput.Date)} AND    {General.ToDataUniv(dtSf.Date)} and aredrepturi = 1) x   ";

                ExecutaNonQuery(sqlIst, null);

                string campuri = "";
                for (int i = 0; i <= 20; i++)
                    campuri += ", \"Val" + i.ToString() + "\" = NULL";
                for (int i = 1; i <= 60; i++)
                    campuri += ", F" + i.ToString() + " = NULL";
                //for (int i = 1; i <= 20; i++)
                //    campuri += ", \"In" + i.ToString() + "\" = NULL, \"Out" + i.ToString() + "\" = NULL";


                //#311 si #344
                strSql = $@"MERGE INTO ""Ptj_intrari"" USING 
                            (Select  case when ""AreDrepturi"" = 1 then ""DenumireScurta"" else null end  as ""Denumire"", x.* from                                
                            (select {marca} as F10003, case when (SELECT count(*) FROM ""Ptj_Intrari"" WHERE f10003 = {marca} and ""Ziua"" = a.""Zi"") = 0 then 0 else 1 end as prezenta, 
                            b.* from  ""tblZile"" a
                            left join 

                            (SELECT P.""Zi"", P.""ZiSapt"", CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",
                            CASE WHEN P.""ZiSapt""=6 OR P.""ZiSapt""=7 OR D.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLibera"", 
                            CASE WHEN COALESCE(b.SL,0) <> 0 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) = 1 THEN 1 ELSE
                            CASE WHEN COALESCE(b.ZL,0) <> 0 AND P.""ZiSapt"" < 6 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) <> 1 THEN 1 ELSE 
                            CASE WHEN COALESCE(b.S,0) <> 0 AND P.""ZiSapt"" = 6 THEN 1 ELSE 
                            CASE WHEN COALESCE(b.D,0) <> 0 AND P.""ZiSapt"" = 7 THEN 1 ELSE 0 
                            END
                            END
                            END
                            END AS ""AreDrepturi"", ""DenumireScurta""
                            FROM ""tblZile"" P
                            INNER JOIN ""Ptj_tblAbsente"" A ON 1=1
                            INNER JOIN ""Ptj_ContracteAbsente"" B ON A.""Id"" = B.""IdAbsenta""
                            LEFT JOIN HOLIDAYS D on P.""Zi""=D.DAY
                            WHERE {General.ToDataUniv(dataInceput.Date)} <= CAST(P.""Zi"" AS date) AND CAST(P.""Zi"" AS date) <= {General.ToDataUniv(dtSf.Date)}
                            AND A.""Id"" = {idAbs}
                            AND COALESCE(A.""DenumireScurta"", '~') <> '~'
                            AND B.""IdContract"" = (SELECT MAX(""IdContract"") FROM ""F100Contracte"" WHERE F10003 =  {marca} AND ""DataInceput"" <= {General.ToDataUniv(dataInceput.Date)} AND {General.ToDataUniv(dtSf.Date)} <= ""DataSfarsit"") 
                            ) b
                            on a.""Zi"" = b.""Zi""

                            where a.""Zi"" between  {General.ToDataUniv(dataInceput.Date)} AND    {General.ToDataUniv(dtSf.Date)} ) x

                            ) Tmp 
                            ON (""Ptj_Intrari"".""Ziua"" = ""Zi"" AND ""Ptj_Intrari"".F10003 = Tmp.F10003 and prezenta = 1) 
                            WHEN MATCHED THEN UPDATE SET ""ValStr"" = ""Denumire"" {campuri} , USER_NO ={HttpContext.Current.Session["UserId"].ToString()}, TIME = {General.CurrentDate()}
                            WHEN NOT MATCHED THEN INSERT (F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""ZiLiberaLegala"", ""IdContract"", ""Norma"", F10002, F10004, F10005, F10006, F10007, F06204, ""ValStr"", USER_NO, TIME)
                             VALUES ({marca}, ""Zi"", ""ZiSapt"" ,""ZiLibera"" , ""ZiLiberaLegala"", 
                            (SELECT X.""IdContract"" FROM ""F100Contracte"" X WHERE X.F10003 = {marca} AND X.""DataInceput"" <= ""Zi"" AND ""Zi"" <= X.""DataSfarsit""), 
                            (SELECT F10043 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10002 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10004 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10005 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10006 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10007 FROM F100 WHERE F10003 = {marca}), 
                             -1,  ""Denumire"", {HttpContext.Current.Session["UserId"].ToString() }, {General.CurrentDate()});";
                ExecutaNonQuery(strSql, null);


                if (dataIncetare.Date != new DateTime(2100, 1, 1) && dataIncetare.Date <= dataSfarsit.Date)
                {//stergerea pontarilor adaugate in plus
                    sql = "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = NULL WHERE F10003 = " + marca + " AND  \"Ziua\" BETWEEN " + General.ToDataUniv(dataIncetare.Date) + " AND " + General.ToDataUniv(dataSfarsit.Date);
                    ExecutaNonQuery(sql, null);
                    sql = "DELETE FROM  \"Ptj_IstoricVal\" WHERE F10003 = " + marca + " AND  \"Ziua\" BETWEEN " + General.ToDataUniv(dataIncetare.Date) + " AND " + General.ToDataUniv(dataSfarsit.Date);
                    ExecutaNonQuery(sql, null);
                }

                if (dtIncetareVeche.Date != new DateTime(2100, 1, 1) && dataIncetare.Date < dtIncetareVeche.Date)
                {//stergerea pontarilor adaugate in plus
                    sql = "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = NULL WHERE F10003 = " + marca + " AND  \"Ziua\" BETWEEN " + General.ToDataUniv(dataIncetare.Date) + " AND " + General.ToDataUniv(dtIncetareVeche.Date);
                    ExecutaNonQuery(sql, null);
                    sql = "DELETE FROM  \"Ptj_IstoricVal\" WHERE F10003 = " + marca + " AND  \"Ziua\" BETWEEN " + General.ToDataUniv(dataIncetare.Date) + " AND " + General.ToDataUniv(dtIncetareVeche.Date);
                    ExecutaNonQuery(sql, null);
                }

                //inserare in Ptj_cereri
                int nrZile = 0;
                DataTable dtAbs = General.IncarcaDT(SelectAbsentaInCereri(Convert.ToInt32(marca), dataInceput.Date, dtSf.Date, 3, idAbs), null);
                for (int i = 0; i < dtAbs.Rows.Count; i++)
                    if (Convert.ToInt32(General.Nz(dtAbs.Rows[i]["AreDrepturi"], 0)) == 1)
                        nrZile++;

                ExecutaNonQuery("DELETE FROM \"Ptj_Cereri\" WHERE F10003 = " + marca + " AND \"IdAbsenta\" = " + idAbs + " AND \"DataInceput\" = " + General.ToDataUniv(dataInceput.Date), null);

                //Radu 01.02.2021 - citire idCerere din secventa
                int idCerere = Dami.NextId("Ptj_Cereri");
                string sqlIdCerere = idCerere.ToString();
                if (idCerere == -99)
                    sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
                string sqlInsert = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""Observatii"", ""IdStare"", USER_NO, TIME) "
                                + @"VALUES (" +
                                sqlIdCerere + ", " +
                                marca + ", " +
                                idAbs + ", " +
                                General.ToDataUniv(dataInceput.Date) + ", " +
                                General.ToDataUniv(dtSf.Date) + ", " +
                                nrZile.ToString() + ", " +
                                "'Transfer din Suspendari', " +
                                "3, " + HttpContext.Current.Session["UserId"].ToString() + ", " + General.CurrentDate() + ")";
                ExecutaNonQuery(sqlInsert, null);
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "General", "TransferPontaj");
            }
        }
        //end Radu


        public static void ExecutaProcedura(string numeProcedura, int idUser, string comentariu = "")
        {
            try
            {
                using (var conn = new SqlConnection(Constante.cnnWeb))
                using (var command = new SqlCommand(numeProcedura, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.AddWithValue("@idUser", idUser);
                    if (comentariu != "")
                        command.Parameters.AddWithValue("@comentariu", comentariu);

                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MemoreazaEroarea(ex.ToString(), "Calcul", "CalculFormule");
            }
        }

        public static void BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e, string numeTabela, Dictionary<string, string> dic)
        {
            try
            {
                int idAuto = 100000000;
                ASPxGridView grDate = sender as ASPxGridView;

                grDate.CancelEdit();
                DataSet ds = HttpContext.Current.Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables[numeTabela];
                if (dt == null) return;

                //daca avem linii noi
                for (int i = 0; i < e.InsertValues.Count; i++)
                {
                    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;

                    bool modif = false;

                    DataRow dr = dt.NewRow();

                    foreach (DictionaryEntry de in upd.NewValues)
                    {
                        string numeCol = de.Key.ToString();
                        dynamic newValue = upd.NewValues[numeCol];
                        if (newValue == null)
                        {
                            dr[numeCol] = DBNull.Value;
                        }
                        else
                        {
                            modif = true;
                            switch (dr.Table.Columns[numeCol].DataType.ToString())
                            {
                                case "System.DateTime":
                                    if (Convert.ToDateTime(newValue).Year == 100)
                                        dr[numeCol] = ChangeToCurrentYear(newValue);
                                    else
                                        dr[numeCol] = newValue;
                                    break;
                                default:
                                    dr[numeCol] = newValue;
                                    break;
                            }
                        }
                    }

                    foreach (KeyValuePair<string, string> l in dic)
                    {
                        dr[l.Key] = l.Value;
                    }

                    dr["USER_NO"] = HttpContext.Current.Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    if (dt.Columns["IdAuto"] != null)
                    {
                        idAuto += 1;
                        dr["IdAuto"] = idAuto;
                    }

                    if (!modif) continue;
                    dt.Rows.Add(dr);
                }

                //daca avem linii modificate
                for (int i = 0; i < e.UpdateValues.Count; i++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                    bool modif = false;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    foreach (DictionaryEntry de in upd.NewValues)
                    {
                        string numeCol = de.Key.ToString();
                        dynamic oldValue = upd.OldValues[numeCol];
                        dynamic newValue = upd.NewValues[numeCol];
                        if (oldValue != null && upd.OldValues[numeCol].GetType() == typeof(System.DBNull))
                            oldValue = null;

                        if (newValue == oldValue) continue;

                        if (newValue == null)
                        {
                            dr[numeCol] = DBNull.Value;
                        }
                        else
                        {
                            modif = true;
                            switch (dr.Table.Columns[numeCol].DataType.ToString())
                            {
                                case "System.DateTime":
                                    if (Convert.ToDateTime(newValue).Year == 100)
                                        dr[numeCol] = ChangeToCurrentYear(newValue);
                                    else
                                        dr[numeCol] = newValue;
                                    break;
                                default:
                                    dr[numeCol] = newValue;
                                    break;
                            }
                        }
                    }

                    foreach (KeyValuePair<string, string> l in dic)
                    {
                        dr[l.Key] = l.Value;
                    }

                    dr["USER_NO"] = HttpContext.Current.Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    if (dt.Columns["IdAuto"] != null)
                    {
                        idAuto += 1;
                        dr["IdAuto"] = idAuto;
                    }

                    if (!modif) continue;
                }


                //daca avem linii sterse
                for (int i = 0; i < e.DeleteValues.Count; i++)
                {
                    ASPxDataDeleteValues upd = e.DeleteValues[i] as ASPxDataDeleteValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    dr.Delete();
                }

                e.Handled = true;
                HttpContext.Current.Session["InformatiaCurenta"] = ds;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "BatchUpdate", new StackTrace().GetFrame(0).GetMethod().Name);
                e.Handled = true;
            }
        }

        public static DateTime ChangeToCurrentYear(DateTime val)
        {
            DateTime dt = val;

            try
            {
                dt = new DateTime(2100, 1, 1, val.Hour, val.Minute, 0);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "ChangeToCurrentYear", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }


        public static void CalculDateCategorieAsigurat(int marca, DateTime dtStart, DateTime dtEstim, DateTime dtSfarsit, out DateTime dtIntrare, out DateTime dtIesire)
        {
            dtIntrare = new DateTime(2100, 1, 1);
            dtIesire = new DateTime(2100, 1, 1);

            if (dtSfarsit.Date != new DateTime(2100, 1, 1))
            {
                dtIntrare = dtSfarsit;
                dtIesire = new DateTime(2100, 1, 1);
            }
            else
            {
                DataTable dtSusp = IncarcaDT("SELECT * FROM F111 WHERE F11103 = " + marca + " AND F11107 IS NOT NULL AND F11107 <> "
                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + " AND " 
                    + " F11104 NOT IN (SELECT F09002 FROM F090 WHERE F09004 = 'Art52Alin1LiteraC' OR F09004 = 'Art52Alin3') ORDER BY F11107 DESC ", null);
                DataTable dtAng = IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + marca, null);
                dtIntrare = Convert.ToDateTime(dtAng.Rows[0]["F10022"].ToString());
                dtIesire = dtStart.AddDays(-1);
                DateTime data = dtStart;
                for (int i = 0; i < dtSusp.Rows.Count; i++)
                {
                    if (Convert.ToDateTime(dtSusp.Rows[i]["F11107"].ToString()).Date == data.Date)
                        data = Convert.ToDateTime(dtSusp.Rows[i]["F11105"].ToString());
                    else
                    {
                        dtIntrare = Convert.ToDateTime(dtSusp.Rows[i]["F11107"].ToString());
                        if (i > 0)
                            dtIesire = Convert.ToDateTime(dtSusp.Rows[i - 1]["F11105"].ToString()).AddDays(-1);
                        break;
                    }
                }

            }

        }

        public static bool EstePontajulInitializat(DateTime dt, string contracte = "")
        {
            bool ras = false;

            try
            {
                //Florin 2021.03.24 - am adaugat filtrul  -  AND F10023 >= {General.ToDataUniv(dt)}
                string sqlCnt = $@"
                    SELECT
                    (SELECT COUNT(*) FROM F100 WHERE F10025 IN (0,999) AND F10023 >= {General.ToDataUniv(dt)}) AS ""NrAng"",
                    (SELECT COUNT(DISTINCT F10003) FROM ""Ptj_Intrari"" WHERE {General.ToDataUniv(dt.Year, dt.Month)} <= ""Ziua"" AND ""Ziua"" <=  {General.ToDataUniv(dt.Year, dt.Month, 99)}) AS ""NrPtj"" {General.FromDual()}";

                if (contracte != "")
                {
                    sqlCnt = $@"
                    SELECT
                    (SELECT COUNT(*) FROM F100 A
                    INNER JOIN ""F100Contracte"" B ON A.F10003=B.F10003 AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit""
                    INNER JOIN ""Ptj_Contracte"" C ON B.""IdContract""=C.""Id"" AND C.""Denumire"" IN ('{contracte.Replace("\\\\", "','")}')
                    WHERE A.F10025 IN (0,999)) AS ""NrAng"",
                    (SELECT COUNT(DISTINCT A.F10003) FROM ""Ptj_Intrari"" A
                    INNER JOIN ""Ptj_Contracte"" C ON A.""IdContract"" = C.""Id"" AND C.""Denumire"" IN ('{contracte.Replace("\\\\", "', '")}')
                    WHERE {General.ToDataUniv(dt.Year, dt.Month)} <= A.""Ziua"" AND A.""Ziua"" <=  {General.ToDataUniv(dt.Year, dt.Month, 99)}) AS ""NrPtj"" {General.FromDual()}";
                }

                DataRow drCnt = General.IncarcaDR(sqlCnt);
                if (drCnt != null)
                {
                    decimal nrAng = Convert.ToDecimal(General.Nz(drCnt["NrAng"], 0));
                    decimal nrPtj = Convert.ToDecimal(General.Nz(drCnt["NrPtj"], 0));

                    if (nrAng != 0)
                    {
                        decimal rez = (nrPtj / nrAng) * 100;
                        if (rez > 51)
                            ras = true;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "BatchUpdate", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        public static void LoadFile(string numeFisier, object fisier, string tabela, int idAuto)
        {
            try
            {
                if (Dami.ValoareParam("SalvareFisierInDisc") == "1")
                {
                    numeFisier = General.CreazaFisierInDisc(numeFisier, fisier, tabela);
                    fisier = null;
                }
                General.IncarcaFisier(numeFisier, fisier, tabela, idAuto);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "BatchUpdate", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string CreazaFisierInDisc(string numeFisier, object fisier, string tabela)
        {
            string tmpNume = numeFisier;

            try
            {
                int i = 1;
                string director = tabela.Replace("Admin_", "").Replace("F100", "").Replace("tbl","");
                string cale = HostingEnvironment.MapPath("~/FisiereApp/" + director + "/");
                
                while(File.Exists(cale + tmpNume))
                {
                    tmpNume = Path.GetFileNameWithoutExtension(cale + tmpNume) + "_" + i + Path.GetExtension(cale + tmpNume);
                    i++;
                }

                using (FileStream stream = new FileStream(cale + tmpNume, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    stream.Write((byte[])fisier, 0, ((byte[])fisier).Length);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "CreazaFisierInDisc", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return tmpNume;
        }



        public static DateTime FindDataConsemnare(int marca)
        {
            string sqlDatesF100 = "";
            string sqlDatesF704_1 = "";
            string sqlDatesF704_2 = "";
            string sqlHistory = "";
            string sz_input = "", dataselect = "", dataperdet = "";

            List<DateTime> listDates = new List<DateTime>();

            DateTime dataazi = DateTime.Now;
            string azi = "";

            if (Constante.tipBD == 1)
                sz_input = "select CONVERT(VARCHAR, DATEADD(dd,CASE WHEN DATEDIFF(dd,0,getdate())%7 > 3 THEN 7-DATEDIFF(dd,0,getdate())%7 ELSE 1 END,getdate()), 103) FROM F010";
            else
                sz_input = "select TO_CHAR(SYSDATE + (CASE WHEN MOD(to_number(to_char(sysdate, 'j')),7) > 3 THEN 7 -  MOD(to_number(to_char(sysdate, 'j')),7) ELSE 1 END), 'DD/MM/YYYY') as ladata from F010";

            DataTable dtDataAzi = IncarcaDT(sz_input, null);
            if (dtDataAzi == null || dtDataAzi.Rows.Count <= 0)
                return dataazi;

            azi = dtDataAzi.Rows[0][0].ToString();
            if (azi.Length == 10)
                dataazi = new DateTime(Convert.ToInt32(azi.Substring(6, 4)), Convert.ToInt32(azi.Substring(3, 2)), Convert.ToInt32(azi.Substring(0, 2)), 0, 0, 0);

            string dataAng = "CONVERT(VARCHAR, F10022, 103)";
            string dataSal = "CONVERT(VARCHAR, F100991, 103)";
            string dataCOR = "CONVERT(VARCHAR, F100956, 103)";
            string dataNorma = "CONVERT(VARCHAR, F100955, 103)";
            string dataSpor = "CONVERT(VARCHAR, F100957, 103)";
            string dataIncet = "CONVERT(VARCHAR, F100993, 103)";
            string dataContr = "CONVERT(VARCHAR, F100986, 103)";
            string dataPerDet = "CONVERT(VARCHAR, F100933, 103)";
            string dataModifTCtr = "CONVERT(VARCHAR, F1001137, 103)";
            string dataModifDCtr = "CONVERT(VARCHAR, F1001138, 103)";
            string dataConsemn = "CONVERT(VARCHAR, F1001109, 103) ";
            string dataModif = " CONVERT(VARCHAR, F70406, 103)";
            string dataTerm = "CONVERT(VARCHAR, MAX(F09506), 103)";

            if (Constante.tipBD == 2)
            {
                dataAng = "TO_CHAR(F10022, 'dd/mm/yyyy')";
                dataSal = "TO_CHAR(F100991, 'dd/mm/yyyy')";
                dataCOR = "TO_CHAR(F100956, 'dd/mm/yyyy')";
                dataNorma = "TO_CHAR(F100955, 'dd/mm/yyyy')";
                dataSpor = "TO_CHAR(F100957, 'dd/mm/yyyy')";
                dataIncet = "TO_CHAR(F100993, 'dd/mm/yyyy')";
                dataContr = "TO_CHAR(F100986, 'dd/mm/yyyy')";
                dataPerDet = "TO_CHAR(F100933, 'dd/mm/yyyy')";
                dataModifTCtr = "TO_CHAR(F1001137, 'dd/mm/yyyy')";
                dataModifDCtr = "TO_CHAR(F1001138, 'dd/mm/yyyy')";
                dataConsemn = "TO_CHAR(F1001109, 'dd/mm/yyyy') ";
                dataModif = "TO_CHAR(F70406, 'dd/mm/yyyy')";
                dataTerm = "TO_CHAR(MAX(F09506), 'dd/mm/yyyy')";
            }


            sz_input = "SELECT " + dataAng + " AS DataAng, " + dataSal + " AS DataSal,  " + dataCOR + " AS DataCor, ";
            sz_input += dataNorma + " AS DataNorma, " + dataSpor + " AS DataSpor, " + dataIncet + "  AS DataIncet, " + dataContr + " AS DataContr, ";
            sz_input += "CASE WHEN F100933 IS NULL THEN '01/01/2100' ELSE " + dataPerDet + " END AS DataPerDet, " + dataModifTCtr + " AS DataModifTCtr,  " + dataModifDCtr + " AS DataModifDCtr, " + dataConsemn + " AS DataConsemnare ";
            sz_input += " FROM F100, F1001 WHERE F100.F10003=F1001.F10003 AND F100.F10003 = {0}";
            sqlDatesF100 = string.Format(sz_input, marca);

            sz_input = "SELECT " + dataModif + ", F70404, F70407, F70420 FROM F704 WHERE F70420=0 AND (F70404=1 OR F70404=3 OR F70404=6 OR F70404=11 OR F70404=15 OR F70404=35 OR F70404=36) AND F70403 = {0} AND F70406 <= {1}";
            sqlDatesF704_1 = string.Format(sz_input, marca, (Constante.tipBD == 1 ? "CONVERT(DATETIME,'" + azi + "',103)" : "TO_DATE('" + azi + "', 'dd/mm/yyyy')"));
            sz_input = "SELECT " + dataModif + ", F70404, F70420 FROM F100 LEFT JOIN F704 ON F10003=F70403 WHERE (F70420 = 0 OR (F70420=1 AND F100993=F70406)) AND F70404 = 4 AND F70403 = {0}";
            sqlDatesF704_2 = string.Format(sz_input, marca);
            sz_input = "SELECT " + dataTerm + " AS DataTerm FROM F095, F100 WHERE F09503 = {0} AND F09506 <> {1}  AND F09504 = F100985";
            sqlHistory = string.Format(sz_input, marca, (Constante.tipBD == 1 ? " CONVERT(DATETIME,'01/01/2100', 103)" : " TO_DATE('01/01/2100', 'dd/mm/yyyy')"));

            DateTime datas;
            DataTable dtData100 = IncarcaDT(sqlDatesF100, null);

            if (dtData100 == null || dtData100.Rows.Count <= 0)
                return dataazi;

            for (int i = 0; i < dtData100.Columns.Count; i++)
            {
                dataselect = dtData100.Rows[0][i].ToString();

                if (dataselect.Length == 10)
                {
                    if (i == 7)
                        dataperdet = dataselect;
                    datas = new DateTime(Convert.ToInt32(dataselect.Substring(6, 4)), Convert.ToInt32(dataselect.Substring(3, 2)), Convert.ToInt32(dataselect.Substring(0, 2)), 0, 0, 0);

                    if (dataselect != "01/01/2100" && dataselect != "01/01/1900" && datas <= dataazi)
                    {
                        listDates.Add(datas);
                    }
                }
            }


            //Radu 15.04.2013 - adaugare data terminarii ultimului contract pe perioada determinata, in cazul in care actualul e pe perioada nedeterminata               
            if (dataperdet == "01/01/2100" || dataperdet == "01/01/1900")
            {//daca actualul contract e pe perioada nedeterminata
                DataTable dtIst = IncarcaDT(sqlHistory, null);
                if (dtIst == null || dtIst.Rows.Count <= 0)
                    return dataazi;

                dataselect = dtIst.Rows[0][0].ToString();
                if (dataselect.Length == 10)
                {
                    datas = new DateTime(Convert.ToInt32(dataselect.Substring(6, 4)), Convert.ToInt32(dataselect.Substring(3, 2)), Convert.ToInt32(dataselect.Substring(0, 2)), 0, 0, 0);

                    if (dataselect != "01/01/2100" && dataselect != "01/01/1900" && datas <= dataazi)
                    {
                        listDates.Add(datas);
                    }
                }
            }


            //F704_1
            DataTable dtF704_1 = IncarcaDT(sqlDatesF704_1, null);
            if (dtF704_1 == null || dtF704_1.Rows.Count <= 0)
                return dataazi;

            for (int i = 0; i < dtF704_1.Rows.Count; i++)
            {
                dataselect = dtF704_1.Rows[i][0].ToString();
                if (dataselect.Length == 10)
                {
                    datas = new DateTime(Convert.ToInt32(dataselect.Substring(6, 4)), Convert.ToInt32(dataselect.Substring(3, 2)), Convert.ToInt32(dataselect.Substring(0, 2)), 0, 0, 0);

                    if (dataselect != "01/01/2100" && dataselect != "01/01/1900" && datas <= dataazi)
                    {
                        listDates.Add(datas);
                    }
                }
            }

            //F704_2
            DataTable dtF704_2 = IncarcaDT(sqlDatesF704_2, null);
            if (dtF704_2 == null || dtF704_2.Rows.Count <= 0)
                return dataazi;

            for (int i = 0; i < dtF704_2.Rows.Count; i++)
            {
                dataselect = dtF704_2.Rows[i][0].ToString();
                if (dataselect.Length == 10)
                {
                    datas = new DateTime(Convert.ToInt32(dataselect.Substring(6, 4)), Convert.ToInt32(dataselect.Substring(3, 2)), Convert.ToInt32(dataselect.Substring(0, 2)), 0, 0, 0);

                    if (dataselect != "01/01/2100" && dataselect != "01/01/1900" && datas <= dataazi)
                    {
                        listDates.Add(datas);
                    }
                }
            }


            //Aleg elementul cel mai mare
            DateTime dataConsemnare = dataazi;
            if (listDates.Count > 0)
            {
                dataConsemnare = listDates.ElementAt(0);
                for (int i = 0; i < listDates.Count; i++)
                {
                    DateTime element = listDates.ElementAt(i);
                    if (element > dataConsemnare)
                    {
                        dataConsemnare = element;
                    }
                }
            }
            return dataConsemnare;

        } 

        public static void SalveazaPost(object f10003, object idPost, DateTime dtModif)
        {
            try
            {
                if (f10003 == null || idPost == null) return;

                string sqlPost =
                    $@"BEGIN
                        IF ((SELECT COUNT(*) FROM ""Org_relPostAngajat"" WHERE F10003=@1 AND {General.TruncateDate("DataInceput")} = {General.ToDataUniv(dtModif)} AND {General.ToDataUniv(dtModif)} <= {General.TruncateDate("DataSfarsit")}) <> 0)
                            BEGIN
                                IF (@2 <> -99)
                                    BEGIN
                                        IF ((SELECT COUNT(*) FROM ""Org_relPostAngajat"" WHERE F10003=@1 AND ""IdPost""=@2 AND {General.TruncateDate("DataSfarsit")}={General.ToDataUniv(dtModif.AddDays(-1))}) = 1)
                                            BEGIN
                                                DELETE ""Org_relPostAngajat"" WHERE F10003=@1 AND {General.TruncateDate("DataInceput")}  = {General.ToDataUniv(dtModif)} AND {General.ToDataUniv(dtModif)} <= {General.TruncateDate("DataSfarsit")};
                                                UPDATE ""Org_relPostAngajat"" SET ""DataSfarsit""={General.ToDataUniv(2100, 1, 1)} WHERE F10003=@1 AND ""IdPost""=@2 AND {General.TruncateDate("DataSfarsit")}={General.ToDataUniv(dtModif.AddDays(-1))};
                                            END;
                                        ELSE
                                            UPDATE ""Org_relPostAngajat"" SET ""IdPost"" = @2 WHERE F10003=@1 AND {General.TruncateDate("DataInceput")}  = {General.ToDataUniv(dtModif)} AND {General.ToDataUniv(dtModif)} <= {General.TruncateDate("DataSfarsit")}
                                    END;                                
                                ELSE
                                    DELETE ""Org_relPostAngajat"" WHERE F10003=@1 AND {General.TruncateDate("DataInceput")}  = {General.ToDataUniv(dtModif)} AND {General.ToDataUniv(dtModif)} <= {General.TruncateDate("DataSfarsit")};
                            END;
                        ELSE
                            BEGIN
			                    DECLARE @Id int
			                    SELECT @Id=IdAuto FROM ""Org_relPostAngajat"" WHERE  F10003=@1 AND {General.TruncateDate("DataInceput")}  <= {General.ToDataUniv(dtModif)} AND {General.ToDataUniv(dtModif)} <= {General.TruncateDate("DataSfarsit")};

                                IF (@2 <> -99)
                                    BEGIN
                                        INSERT INTO ""Org_relPostAngajat""(""IdPost"", F10003, ""DataInceput"", ""DataSfarsit"", ""IdPostVechi"", USER_NO, TIME) VALUES(@2, @1, {General.ToDataUniv(dtModif)}, 
                                        COALESCE((SELECT ""DataSfarsit"" FROM ""Org_relPostAngajat"" WHERE IdAuto=@Id), {General.ToDataUniv(2100, 1, 1)}), 
                                        (SELECT ""IdPost"" FROM ""Org_relPostAngajat"" WHERE IdAuto=@Id), @3, {General.CurrentDate()});
                                    END;                            
                                UPDATE ""Org_relPostAngajat"" SET ""DataSfarsit""={General.ToDataUniv(dtModif.AddDays(-1))} WHERE IdAuto=@Id;
                            END;                     
                    END;";
                General.ExecutaNonQuery(sqlPost, new object[] { f10003, idPost, HttpContext.Current.Session["UserId"] });
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "SalveazaPost", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //public static void AdaugaBeneficiile(ref DataSet ds, object f10003, DataRow drPost)
        //{
        //    try
        //    {
        //        string sqlFinal = "SELECT * FROM \"Admin_Beneficii\" WHERE \"Marca\" = " + f10003.ToString();
        //        DataTable dtBen = new DataTable();
        //        if (ds.Tables.Contains("Admin_Beneficii"))
        //        {
        //            dtBen = ds.Tables["Admin_Beneficii"];
        //        }
        //        else
        //        {
        //            dtBen = General.IncarcaDT($@"SELECT * FROM ""Admin_Beneficii"" WHERE ""Marca"" = @1", new object[] { f10003 });
        //            dtBen.TableName = "Admin_Beneficii";
        //            dtBen.PrimaryKey = new DataColumn[] { dtBen.Columns["IdAuto"] };
        //            ds.Tables.Add(dtBen);
        //        }

        //        //stergem toate beneficiile existente
        //        for (int i = 0; i < dtBen.Rows.Count; i++)
        //        {
        //            dtBen.Rows[i].Delete();
        //        }

        //        for (int i = 1; i <= 10; i++)
        //        {
        //            int idBen = Convert.ToInt32(General.Nz(drPost["IdBeneficiu" + i], -99));
        //            if (idBen != -99 && dtBen.Select("IdObiect=" + idBen).Count() <= 0)
        //            {
        //                DataRow drBen = ds.Tables["Admin_Beneficii"].NewRow();
        //                drBen["Marca"] = f10003;
        //                drBen["IdObiect"] = idBen;
        //                drBen["DataPrimire"] = DateTime.Now;
        //                drBen["DataExpirare"] = new DateTime(2100, 1, 1);
        //                drBen["TIME"] = DateTime.Now;
        //                drBen["USER_NO"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;

        //                if (Constante.tipBD == 1)
        //                    drBen["IdAuto"] = Convert.ToInt32(General.Nz(dtBen.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
        //                else
        //                    drBen["IdAuto"] = Dami.NextId("Admin_Beneficii");
        //                if (Convert.ToInt32(drBen["IdAuto"].ToString()) < 1000000)
        //                    drBen["IdAuto"] = Convert.ToInt32(drBen["IdAuto"].ToString()) + 1000000;

        //                dtBen.Rows.Add(drBen);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "AdaugaBeneficiile", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        public static void AdaugaBeneficiile(ref DataSet ds, object f10003)
        {
            try
            {
                string sqlFinal = "SELECT * FROM \"Admin_Beneficii\" WHERE \"Marca\" = " + f10003.ToString();
                DataTable dt = new DataTable();
                if (ds.Tables.Contains("Admin_Beneficii"))
                {
                    dt = ds.Tables["Admin_Beneficii"];
                }
                else
                {
                    dt = General.IncarcaDT($@"SELECT * FROM ""Admin_Beneficii"" WHERE ""Marca"" = @1", new object[] { f10003 });
                    dt.TableName = "Admin_Beneficii";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }

                //stergem inregistrarile
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted && General.Nz(dt.Rows[i]["VineDinPosturi"], 0).ToString() == "1" && dt.Rows[i]["Fisier"] == DBNull.Value)
                        dt.Rows[i].Delete();
                }

                DataTable dtOrg = General.IncarcaDT(
                    $@"SELECT A.*, B.""Denumire"" AS ""Caracteristica"" 
                    FROM ""Org_PosturiBeneficii"" A
                    LEFT JOIN ""Admin_Obiecte"" B ON A.""IdObiect"" = B.""Id""
                    WHERE ""IdPost"" = @1", new object[] { HttpContext.Current.Session["MP_IdPost"] });

                for (int i = 0; i < dtOrg.Rows.Count; i++)
                {
                    if (dt.Select("IdObiect=" + dtOrg.Rows[i]["IdObiect"]).Count() == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Marca"] = f10003;
                        dr["IdObiect"] = dtOrg.Rows[i]["IdObiect"];
                        dr["Caracteristica"] = dtOrg.Rows[i]["Caracteristica"];
                        dr["VineDinPosturi"] = 1;
                        dr["TIME"] = DateTime.Now;
                        dr["USER_NO"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;

                        dt.Rows.Add(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "AdaugaBeneficiile", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void AdaugaDosar(ref DataSet ds, object f10003)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ds.Tables.Contains("Atasamente"))
                {
                    dt = ds.Tables["Atasamente"];
                }
                else
                {
                    dt = General.IncarcaDT(@"SELECT * FROM ""Atasamente"" WHERE IdEmpl=@1", new object[] { f10003 });
                    dt.TableName = "Atasamente";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }

                //stergem inregistrarile
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted && General.Nz(dt.Rows[i]["VineDinPosturi"], 0).ToString() == "1" && dt.Rows[i]["Attach"] == DBNull.Value)
                        dt.Rows[i].Delete();
                }

                DataTable dtOrg = General.IncarcaDT($@"SELECT * FROM ""Org_PosturiDosar"" WHERE ""IdPost""=@1", new object[] { HttpContext.Current.Session["MP_IdPost"] });

                for (int i = 0; i < dtOrg.Rows.Count; i++)
                {
                    if (dt.Select("IdCategory=" + dtOrg.Rows[i]["IdObiect"]).Count() == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["IdEmpl"] = f10003;
                        dr["IdCategory"] = dtOrg.Rows[i]["IdObiect"];
                        dr["VineDinPosturi"] = 1;
                        dr["TIME"] = DateTime.Now;
                        dr["USER_NO"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;

                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "AdaugaDosar", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void AdaugaEchipamente(ref DataSet ds, object f10003)
        {
            try
            {
                string sqlFinal = "SELECT * FROM \"Admin_Echipamente\" WHERE \"Marca\" = " + f10003.ToString();
                DataTable dt = new DataTable();
                if (ds.Tables.Contains("Admin_Echipamente"))
                {
                    dt = ds.Tables["Admin_Echipamente"];
                }
                else
                {
                    dt = General.IncarcaDT($@"SELECT * FROM ""Admin_Echipamente"" WHERE ""Marca"" = @1", new object[] { f10003 });
                    dt.TableName = "Admin_Echipamente";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }

                //stergem inregistrarile
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted && General.Nz(dt.Rows[i]["VineDinPosturi"], 0).ToString() == "1" && dt.Rows[i]["Fisier"] == DBNull.Value)
                        dt.Rows[i].Delete();
                }

                DataTable dtOrg = General.IncarcaDT(
                    $@"SELECT A.*, B.""Denumire"" AS ""Caracteristica"" 
                    FROM ""Org_PosturiEchipamente"" A
                    LEFT JOIN ""Admin_Obiecte"" B ON A.""IdObiect"" = B.""Id""
                    WHERE ""IdPost"" = @1", new object[] { HttpContext.Current.Session["MP_IdPost"] });

                for (int i = 0; i < dtOrg.Rows.Count; i++)
                {
                    if (dt.Select("IdObiect=" + dtOrg.Rows[i]["IdObiect"]).Count() == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Marca"] = f10003;
                        dr["IdObiect"] = dtOrg.Rows[i]["IdObiect"];
                        dr["Caracteristica"] = dtOrg.Rows[i]["Caracteristica"];
                        dr["VineDinPosturi"] = 1;
                        dr["TIME"] = DateTime.Now;
                        dr["USER_NO"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;

                        dt.Rows.Add(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "AdaugaEchipamente", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void AflaIdPost()
        {
            try
            {
                if (HttpContext.Current.Session["MP_IdPost"] == null)
                {
                    string sqlIdPost = $@"SELECT ""IdPost"" FROM ""Org_relPostAngajat"" WHERE F10003=@1 AND {General.TruncateDate("DataInceput")} <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= {General.TruncateDate("DataSfarsit")}";
                    HttpContext.Current.Session["MP_IdPost"] = General.ExecutaScalar(sqlIdPost, new object[] { HttpContext.Current.Session["Marca"] });
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "AflaIdPost", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void CreeazaAtributePost(int id, object f10003, object idPost, DateTime dtModif)
        {
            try
            {
                //Florin 2021.03.02 #710 - la ramura cu functie am adaugat sa se salveaza si PostId; este necesar pt functia de salvare post din F704
                General.ExecutaNonQuery(
                    $@"
                    BEGIN
                        IF((SELECT COUNT(*) FROM Avs_Cereri WHERE Id={id} AND FunctieId IS NOT NULL) > 0)
                            BEGIN
                                INSERT INTO Avs_Cereri(Id, F10003, IdAtribut, IdCircuit, Pozitie, TotalCircuit, Culoare, IdStare, Explicatii, DataModif, USER_NO, TIME, UserIntrod, GenerareDoc, IdParinte, FunctieId, FunctieNume, PostId)
                                SELECT NEXT VALUE FOR Avs_Cereri_SEQ, F10003, 2, IdCircuit, Pozitie, TotalCircuit, Culoare, IdStare, Explicatii, DataModif, USER_NO, GetDate(), UserIntrod, GenerareDoc, {id}, FunctieId, FunctieNume, {idPost} FROM Avs_Cereri WHERE Id={id};
                                
                                INSERT INTO Avs_CereriIstoric(Id, IdCircuit, IdPost, IdUser, IdStare, Pozitie, CUloare, Aprobat, DataAprobare, Inlocuitor, IdUserInlocuitor, IdRol, IdSuper, USER_NO, TIME)
                                SELECT CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'Avs_Cereri_SEQ')), IdCircuit, IdPost, IdUser, IdStare, Pozitie, CUloare, Aprobat, DataAprobare, Inlocuitor, IdUserInlocuitor, IdRol, IdSuper, USER_NO, GetDate() FROM Avs_CereriIstoric WHERE Id={id};
                            END;

                        IF((SELECT COUNT(*) FROM Avs_Cereri WHERE Id={id} AND CORCod IS NOT NULL) > 0)
                            BEGIN
                                INSERT INTO Avs_Cereri(Id, F10003, IdAtribut, IdCircuit, Pozitie, TotalCircuit, Culoare, IdStare, Explicatii, DataModif, USER_NO, TIME, UserIntrod, GenerareDoc, IdParinte, CORCod, CORNume)
                                SELECT NEXT VALUE FOR Avs_Cereri_SEQ, F10003, 3, IdCircuit, Pozitie, TotalCircuit, Culoare, IdStare, Explicatii, DataModif, USER_NO, GetDate(), UserIntrod, GenerareDoc, {id}, CORCod, CORNume FROM Avs_Cereri WHERE Id={id}

                                INSERT INTO Avs_CereriIstoric(Id, IdCircuit, IdPost, IdUser, IdStare, Pozitie, CUloare, Aprobat, DataAprobare, Inlocuitor, IdUserInlocuitor, IdRol, IdSuper, USER_NO, TIME)
                                SELECT CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'Avs_Cereri_SEQ')), IdCircuit, IdPost, IdUser, IdStare, Pozitie, CUloare, Aprobat, DataAprobare, Inlocuitor, IdUserInlocuitor, IdRol, IdSuper, USER_NO, GetDate() FROM Avs_CereriIstoric WHERE Id={id};
                            END;

                        IF((SELECT COUNT(*) FROM Avs_Cereri WHERE Id={id} AND DeptId IS NOT NULL) > 0)
                            BEGIN
                                INSERT INTO Avs_Cereri(Id, F10003, IdAtribut, IdCircuit, Pozitie, TotalCircuit, Culoare, IdStare, Explicatii, DataModif, USER_NO, TIME, UserIntrod, GenerareDoc, IdParinte, SubcompanieId, SubcompanieNume, FilialaId, FilialaNume, SectieId, SectieNume, DeptId, DeptNume)
                                SELECT NEXT VALUE FOR Avs_Cereri_SEQ, F10003, 5, IdCircuit, Pozitie, TotalCircuit, Culoare, IdStare, Explicatii, DataModif, USER_NO, GetDate(), UserIntrod, GenerareDoc, {id}, SubcompanieId, SubcompanieNume, FilialaId, FilialaNume, SectieId, SectieNume, DeptId, DeptNume FROM Avs_Cereri WHERE Id={id}

                                INSERT INTO Avs_CereriIstoric(Id, IdCircuit, IdPost, IdUser, IdStare, Pozitie, CUloare, Aprobat, DataAprobare, Inlocuitor, IdUserInlocuitor, IdRol, IdSuper, USER_NO, TIME)
                                SELECT CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'Avs_Cereri_SEQ')), IdCircuit, IdPost, IdUser, IdStare, Pozitie, CUloare, Aprobat, DataAprobare, Inlocuitor, IdUserInlocuitor, IdRol, IdSuper, USER_NO, GetDate() FROM Avs_CereriIstoric WHERE Id={id};
                            END;                   
                    END;");


                //Florin 2021.03.02 #710
                //SalveazaPost(f10003, idPost, dtModif);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "CreeazaAtributePost", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}