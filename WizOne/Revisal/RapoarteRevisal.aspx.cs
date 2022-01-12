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
                btnRegSal.Text = Dami.TraduCuvant("btnRegSal", "Genereaza registru");
                btnRapSal.Text = Dami.TraduCuvant("btnRapSal", "Genereaza registru salariat");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                for (int i = 0; i < Menu.Items.Count; i++)
                    Menu.Items[i].Text = Dami.TraduCuvant(Menu.Items[i].Text);
                lblRol.InnerText = Dami.TraduCuvant("Supervizor");

                foreach (dynamic c in grDateReg.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                foreach (dynamic c in grDateRap.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }
                #endregion



                string strSql = "";             


                DataTable dtSuper = new DataTable();
                dtSuper.Columns.Add("Rol", typeof(int));
                dtSuper.Columns.Add("RolDenumire", typeof(string));

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null, "F10003;Rol");            

                DataView view = new DataView(dtAng);
                DataTable dtRol = view.ToTable(true, "Rol", "RolDenumire");
                string lstIdSuper = Dami.ValoareParam("Adev_IdSuper", "");
                if (lstIdSuper.Length > 0)
                    dtSuper = dtRol.Select("Rol IN (" + lstIdSuper + ")").CopyToDataTable();
                else
                    dtSuper = dtRol;

                cmbRol.DataSource = dtSuper;
                cmbRol.DataBind();                

                if (!IsPostBack)
                {
                    List<string> filePath = new List<string>();
                    string[] fileDrive = Directory.GetFiles(HostingEnvironment.MapPath("~/Temp/RapoarteRevisal/"), "*.xls");
                    foreach (string fileNames in fileDrive)
                        File.Delete(fileNames);

                    btnRapSal.ClientVisible = false;
                    grDateRap.ClientVisible = false;
                    //btnRapSal.Style["display"] = "none";
                    //grDateRap.Style["display"] = "none";
                 
                    cmbRol.SelectedIndex = 0;

                    IncarcaGridRap((cmbRol.Value ?? -99).ToString());  
                    IncarcaGridReg((cmbRol.Value ?? -99).ToString());

                }
                else
                {
                    DataTable dtRap = Session["Revisal_Rap"] as DataTable;
                    grDateRap.DataSource = dtRap;
                    grDateRap.KeyFieldName = "Id";
                    grDateRap.DataBind();

                    DataTable dtReg = Session["Revisal_Reg"] as DataTable;
                    grDateReg.DataSource = dtReg;
                    grDateReg.KeyFieldName = "Id";
                    grDateReg.DataBind();
                }

                grDateRap.SettingsPager.PageSize = 50;
                grDateReg.SettingsPager.PageSize = 50;

            }
            catch (Exception ex)
            {
                //pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();                
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGridRap(string rol)
        {
            string strSql = "SELECT a.Marca, b.NUME + ' ' + b.PRENUME as NumeComplet, a.CNP, b.NATIONALIT as Nationalitate, b.ADRESA as Adresa, a.Radiat, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as Id FROM CONTRACTE a LEFT JOIN SALARIATI B ON a.CNP=b.CNP " +
                "WHERE a.CNP IN (SELECT F10017 FROM F100 WHERE F10003 IN (SELECT F10003 FROM F100Supervizori WHERE IdUser = " + Session["UserId"].ToString() + " AND IdSuper = " + rol + ")) order by NumeComplet";
            DataTable dtRap = General.IncarcaDT(strSql, null);
            grDateRap.DataSource = dtRap;
            grDateRap.KeyFieldName = "Id";
            grDateRap.DataBind();
            Session["Revisal_Rap"] = dtRap;

        }

        private void IncarcaGridReg(string rol)
        {
            string strSql = "SELECT a.Marca, b.NUME + ' ' + b.PRENUME as NumeComplet, a.CNP, b.NATIONALIT as Nationalitate, b.ADRESA as Adresa, a.TipDurata, "
                + "(SELECT F72204 FROM F722 WHERE F72202 = a.Cor and F72206=a.CorVers) AS COR, a.StareCur as Stare, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as Id FROM CONTRACTE a LEFT JOIN SALARIATI B ON a.CNP=b.CNP "
                + " WHERE a.CNP IN (SELECT F10017 FROM F100 WHERE F10003 IN (SELECT F10003 FROM F100Supervizori WHERE IdUser = " + Session["UserId"].ToString() + " AND IdSuper = " + rol + ")) order by NumeComplet";
            DataTable dtReg = General.IncarcaDT(strSql, null);
            grDateReg.DataSource = dtReg;
            grDateReg.KeyFieldName = "Id";
            grDateReg.DataBind();
            Session["Revisal_Reg"] = dtReg;
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

            tmp = cnApp.Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
            string conn = tmp.Split(';')[0];
            tmp = cnApp.Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
            string user = tmp.Split(';')[0];
            string DB = "";
            if (Constante.tipBD == 1)
            {
                tmp = cnApp.Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
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

        protected void btnRapSal_Click(object sender, EventArgs e)
        {
            string sql = "SELECT \"Nume\" , \"Valoare\"  FROM \"tblParametrii\" WHERE \"Nume\" = 'REVISAL_SAL'";
            DataTable dtParam = General.IncarcaDT(sql, null);
            string sal = "";
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                sal = dtParam.Rows[0][0].ToString();

            //if (cmbAng.Value == null)
            //{
            //    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"));
            //    return;
            //}

            List<object> lst = grDateRap.GetSelectedFieldValues(new string[] { "CNP", "NumeComplet" });
            if (lst == null || lst.Count() == 0 || lst[0] == null)
            {

                MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"));
                return;
            }

            object[] arr = lst[0] as object[];


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

                    tmp = cnApp.Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
                    string conn = tmp.Split(';')[0];
                    tmp = cnApp.Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
                    string user = tmp.Split(';')[0];
                    string DB = "";
                    if (Constante.tipBD == 1)
                    {
                        tmp = cnApp.Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];  //#1079 - Radu 12.01.2022 - am eliminat ToUpper()
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
                   
                    bool rez = RapoarteRevisalDLL.Class1.RaportSalariatWizOne(1, Config, HostingEnvironment.MapPath("~/Temp/"), 1, arr[0].ToString(), arr[1].ToString());

                 
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


        private string SelectAngajati(string filtru = "")
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" , ""Rol"", ""RolDenumire""
                        FROM (
                        SELECT A.F10003, 0 AS ""Rol"",  COALESCE((SELECT COALESCE(""Alias"", ""Denumire"") FROM ""tblSupervizori"" WHERE ""Id""=0),'Angajat') AS ""RolDenumire""
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003, B.""IdSuper"" AS ""Rol"", CASE WHEN D.""Alias"" IS NOT NULL AND D.""Alias"" <> '' THEN D.""Alias"" ELSE D.""Denumire"" END AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        LEFT JOIN ""tblSupervizori"" D ON D.""Id"" = B.""IdSuper""
                        WHERE B.""IdUser""= {Session["UserId"]}) B                        
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607 {filtru}";

            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void grDateReg_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {      
            string[] arr = e.Parameters.Split(';');
            switch (arr[0])
            {
                case "cmbRol":
                    IncarcaGridReg(arr[1]);
                    break;
            }
        }

        protected void grDateRap_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string[] arr = e.Parameters.Split(';');
            switch (arr[0])
            {
                case "cmbRol":
                    IncarcaGridRap(arr[1]);
                    break;
            }
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