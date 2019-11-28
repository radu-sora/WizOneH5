using DevExpress.Web;
using ProceseSec;
using System;
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
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                btnRegSal.Text = Dami.TraduCuvant("btnRegSal", "Registru salariati");
                btnContrSal.Text = Dami.TraduCuvant("btnContrSal", "Contracte per salariat");
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
            //string fisier = "RegistruSalariati.xls";
            //byte[] fis = RegistruSalariati(fisier);  

            //if (fis != null)
            //{
            //    MessageBox.Show(Dami.TraduCuvant("Fisierul a fost generat cu success!"));

            //    MemoryStream stream = new MemoryStream(fis);
            //    Response.Clear();
            //    MemoryStream ms = stream;
            //    Response.ContentType = "application/vnd.ms-excel";
            //    Response.AddHeader("content-disposition", "attachment;filename=" + fisier);
            //    Response.Buffer = true;
            //    ms.WriteTo(Response.OutputStream);
            //    Response.End();          
            //}
            //else
            //    MessageBox.Show(Dami.TraduCuvant("Fisierul nu a fost generat!"));             
        }

        protected void btnContrSal_Click(object sender, EventArgs e)
        {
            grAng.Visible = true;
            legAng.InnerText = "Lista angajati - Contracte per salariat";
        }

        protected void btnRapSal_Click(object sender, EventArgs e)
        {
            grAng.Visible = true;
            legAng.InnerText = "Lista angajati - Raport pe salariat";
        }

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
                if (legAng.InnerText == "Lista angajati - Contracte per salariat")
                {
                    //string fisier = "ContracteSalariat" + cmbAng.Text.Replace(' ', '_') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0')
                    //    + DateTime.Now.Year.ToString() + ".xls";
                    //byte[] fis = ContracteSalariat(sal, cmbAng.Value.ToString(), cmbAng.Text.ToString(), fisier);                   

                    //if (fis != null)
                    //{
                    //    MessageBox.Show((Dami.TraduCuvant("Fisierul a fost generat cu success!"));

                    //    MemoryStream stream = new MemoryStream(fis);
                    //    Response.Clear();
                    //    MemoryStream ms = stream;
                    //    Response.ContentType = "application/vnd.ms-excel";
                    //    Response.AddHeader("content-disposition", "attachment;filename=" + fisier);
                    //    Response.Buffer = true;
                    //    ms.WriteTo(Response.OutputStream);
                    //    Response.End();
                    //}
                    //else
                    //    MessageBox.Show(Dami.TraduCuvant("Fisierul nu a fost generat!"));


                }

                if (legAng.InnerText == "Lista angajati - Raport pe salariat")
                {
                    //string fisier = "RaportSalariat" + cmbAng.Text.Replace(' ', '_') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0')
                    //    + DateTime.Now.Year.ToString() + ".xls";
                    //byte[] fis = RaportSalariat(sal, cmbAng.Value.ToString(), cmbAng.Text.ToString(), fisier);                     
                    //if (fis != null)
                    //{
                    //    MessageBox.Show(Dami.TraduCuvant("Fisierul a fost generat cu success!"));

                    //    MemoryStream stream = new MemoryStream(fis);
                    //    Response.Clear();
                    //    MemoryStream ms = stream;
                    //    Response.ContentType = "application/vnd.ms-excel";
                    //    Response.AddHeader("content-disposition", "attachment;filename=" + fisier);
                    //    Response.Buffer = true;
                    //    ms.WriteTo(Response.OutputStream);
                    //    Response.End();
                    //}
                    //else
                    //    MessageBox.Show(Dami.TraduCuvant("Fisierul nu a fost generat!"));
                }
            }
            else
                MessageBox.Show(Dami.TraduCuvant("Nu este specificat parametrul REVISAL_SAL!"));
        }

        protected void btnRen_Click(object sender, EventArgs e)
        {
            grAng.Visible = false;
            legAng.InnerText = "Lista angajati";
        } 

    }
}