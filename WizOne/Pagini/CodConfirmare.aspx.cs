using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.IO;
using System.Data;
using System.Diagnostics;
using ProceseSec;

namespace WizOne.Pagini
{
    public partial class CodConfirmare : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Dami.AccesApp();
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (General.VarSession("UserId").ToString() == "-99" || General.VarSession("User").ToString() == "")
                    MessageBox.Show("Utilizator invalid", MessageBox.icoWarning, "", "Default.aspx");
                

                if (txtCod.Text != "")
                {
                    string strSql = $@"SELECT 
                        CASE WHEN DATEADD(s, COALESCE((SELECT COALESCE(Valoare,60) FROM tblParametrii WHERE Nume='2FA_Timp'),60), TIME) < GetDate() THEN 0 ELSE 1 END AS Este 
                        FROM GDPR_2FA WHERE IdUser={Session["UserId"]} AND Cod='{txtCod.Text}'";
                    if (Constante.tipBD == 2)
                        strSql = $@"SELECT 
                        CASE WHEN (TIME + numToDSInterval(TO_NUMBER(COALESCE((SELECT COALESCE(""Valoare"",'60') FROM ""tblParametrii"" WHERE ""Nume"" ='2FA_Timp'),'60')), 'second')) < SYSDATE THEN 0 ELSE 1 END AS ""Este""
                        FROM GDPR_2FA WHERE ""IdUser"" ={Session["UserId"]} AND ""Cod"" = '{txtCod.Text}'";

                    int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, null),0));

                    if (cnt == 0)
                        MessageBox.Show("Cod eronat", MessageBox.icoError, "");
                    else
                        Response.Redirect("~/Pagini/MainPage", false);
                }
                else
                    MessageBox.Show("Lipsesc date !", MessageBox.icoWarning, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}