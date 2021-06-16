using DevExpress.Web;
using ProceseSec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Revisal
{
    public partial class RapoarteRevisal : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                btnRegSal.Text = Dami.TraduCuvant("btnRegSal", "Registru salariati");
                //btnContrSal.Text = Dami.TraduCuvant("btnContrSal", "Contracte per salariat");
                btnRapSal.Text = Dami.TraduCuvant("btnRapSal", "Raport pe salariat");
                btnCont.Text = Dami.TraduCuvant("btnCont", "Continua");
                btnRen.Text = Dami.TraduCuvant("btnRen", "Renunta");
                #endregion

                string strSql = "";

                if (Constante.tipBD == 1)
                    strSql = @"SELECT CNP, NUME + ' ' + PRENUME + ' (' + CNP + ')' AS ""Salariat"" FROM SALARIATI ORDER BY ""Salariat""";
                else
                    strSql = @"SELECT CNP, NUME || ' ' || PRENUME || ' (' || CNP || ')' AS ""Salariat"" FROM SALARIATI ORDER BY ""Salariat""";

                cmbAng.DataSource = General.IncarcaDT(strSql, null);
                cmbAng.DataBind(); 
                                
                if (!IsPostBack)
                {
                    List<string> filePath = new List<string>();
                    string[] fileDrive = Directory.GetFiles(HostingEnvironment.MapPath("~/Temp/RapoarteRevisal/"), "*.xls");
                    foreach (string fileNames in fileDrive)
                        File.Delete(fileNames);
                }

            }
            catch (Exception ex)
            {
                //pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();                
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
 
            }
            catch (Exception ex)
            {
                //pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRegSal_Click(object sender, EventArgs e)
        {
            Hashtable Config = new Hashtable();

            string cnApp = Constante.cnnWeb;
            string tmp = cnApp.Split(new[] { "Password=" }, StringSplitOptions.None)[1];
            string pwd = tmp.Split(';')[0];

            tmp = cnApp.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
            string conn = tmp.Split(';')[0];
            tmp = cnApp.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
            string user = tmp.Split(';')[0];
            string DB = "";
            if (Constante.tipBD == 1)
            {
                tmp = cnApp.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                DB = tmp.Split(';')[0];
            }
            else
                DB = user;

            Config.Add("DATABASE", (Constante.tipBD == 2 ? "ORACLE" : "SQLSVR"));
            Config.Add("ORAUSER", DB);
            Config.Add("ORAPWD", pwd);
            Config.Add("ORALOGIN", user);

            string host = "", port = "";

            if (Constante.tipBD == 2)
            {
                host = conn.Split('/')[0];
                conn = conn.Split('/')[1];
            }
            Config.Add("ORACONN", conn);
            Config.Add("HOST_ADEV", host);
            Config.Add("PORT_ADEV", port);
            //Convert.ToInt64(Session["UserId"].ToString())
            bool rez = RapoarteRevisalDLL.Class1.RegistruSalariati(1, Config, HostingEnvironment.MapPath("~/Temp/"), 1);           
            string FileName = HostingEnvironment.MapPath("~/Temp/RapoarteRevisal/") + "RegistruSalariati.xls";

            byte[] fisier = File.ReadAllBytes(FileName);
            if (fisier != null)
            {
                MemoryStream stream = new MemoryStream(fisier);
                Response.Clear();
                MemoryStream ms = stream;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=RegistruSalariati.xls");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                File.Delete(FileName);
                try
                {
                    Response.End();
                }
                catch { };
            }            
        }

        //protected void btnContrSal_Click(object sender, EventArgs e)
        //{
        //    grAng.Visible = true;
        //    legAng.InnerText = "Lista angajati - Contracte per salariat";
        //}

        //protected void btnRapSal_Click(object sender, EventArgs e)
        //{
        //    grAng.Visible = true;            
        //    legAng.InnerText = "Lista angajati - Raport pe salariat";
        //}

        protected void btnCont_Click(object sender, EventArgs e)
        {
            string sql = "SELECT \"Nume\" , \"Valoare\"  FROM \"tblParametrii\" WHERE \"Nume\" = 'REVISAL_SAL'";
            DataTable dtParam = General.IncarcaDT(sql, null);
            string sal = "";
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                sal = dtParam.Rows[0][0].ToString();

            if (cmbAng.Value == null)
            {
                MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"));
                return;
            }


            if (sal.Length > 0)
            {
                //if (legAng.InnerText == "Lista angajati - Contracte per salariat")
                //{

                //}

                //if (legAng.InnerText == "Lista angajati - Raport pe salariat")
                {

                    Hashtable Config = new Hashtable();

                    string cnApp = Constante.cnnWeb;
                    string tmp = cnApp.Split(new[] { "Password=" }, StringSplitOptions.None)[1];
                    string pwd = tmp.Split(';')[0];

                    tmp = cnApp.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                    string conn = tmp.Split(';')[0];
                    tmp = cnApp.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                    string user = tmp.Split(';')[0];
                    string DB = "";
                    if (Constante.tipBD == 1)
                    {
                        tmp = cnApp.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                        DB = tmp.Split(';')[0];
                    }
                    else
                        DB = user;

                    Config.Add("DATABASE", (Constante.tipBD == 2 ? "ORACLE" : "SQLSVR"));
                    Config.Add("ORAUSER", DB);
                    Config.Add("ORAPWD", pwd);
                    Config.Add("ORALOGIN", user);

                    string host = "", port = "";

                    if (Constante.tipBD == 2)
                    {
                        host = conn.Split('/')[0];
                        conn = conn.Split('/')[1];
                    }
                    Config.Add("ORACONN", conn);
                    Config.Add("HOST_ADEV", host);
                    Config.Add("PORT_ADEV", port);
                    //Convert.ToInt64(Session["UserId"].ToString())
                    bool rez = RapoarteRevisalDLL.Class1.RaportSalariatWizOne(1, Config, HostingEnvironment.MapPath("~/Temp/"), 1, cmbAng.Value.ToString(), cmbAng.Text.Split('(')[0].TrimEnd());

                 
                    if (rez)
                    {
                        MessageBox.Show(Dami.TraduCuvant("Fisierul a fost generat cu success!"));

                        List<string> filePath = new List<string>();
                        string[] fileDrive = Directory.GetFiles(HostingEnvironment.MapPath("~/Temp/RapoarteRevisal/"), "*.xls");
                        foreach (string fileNames in fileDrive)
                            filePath.Add(fileNames);

                        byte[] fisier = File.ReadAllBytes(filePath[0]);
                        if (fisier != null)
                        {
                            MemoryStream stream = new MemoryStream(fisier);
                            Response.Clear();
                            MemoryStream ms = stream;
                            Response.ContentType = "application/vnd.ms-excel";
                            Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(filePath[0]));
                            Response.Buffer = true;
                            ms.WriteTo(Response.OutputStream);
                            File.Delete(filePath[0]);
                            try
                            {
                                Response.End();
                            }
                            catch { };
                        }                        
                    }
                    else
                        MessageBox.Show(Dami.TraduCuvant("Fisierul nu a fost generat!"));
                }
            }
            else
                MessageBox.Show(Dami.TraduCuvant("Nu este specificat parametrul REVISAL_SAL!"));
        }

        //protected void btnRen_Click(object sender, EventArgs e)
        //{
        //    grAng.Visible = false;
        //    legAng.InnerText = "Lista angajati";
        //}

    }

                        //<tr>
                        //    <td align = "right" >
                        //        < dx:ASPxButton ID = "btnContrSal" ClientInstanceName="btnContrSal" ClientIDMode="Static" runat="server" Text="Contracte per salariat" Width="180"   OnClick="btnContrSal_Click" oncontextMenu="ctx(this,event)">                    
                                    
                        //        </dx:ASPxButton>
                        //    </td>
                        //</tr> 


}