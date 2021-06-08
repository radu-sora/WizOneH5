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
    public partial class SchimbaParola : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Dami.AccesApp(this.Page);
            //Radu 06.01.2020
            if (Session["SchimbaParolaMesaj"] != null)
            {
                MessageBox.Show(Session["SchimbaParolaMesaj"].ToString(), MessageBox.icoWarning);
                Session["SchimbaParolaMesaj"] = null;
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (General.VarSession("UserId").ToString() == "-99" || General.VarSession("User").ToString() == "")
                {
                    MessageBox.Show("Utilizator invalid", MessageBox.icoWarning, "", "Default.aspx");
                }

                DataRow dr = General.IncarcaDR("SELECT F70103 FROM USERS WHERE F70104=@1", new string[] { General.VarSession("User").ToString() });

                if (txtPan1.Text == "" || txtPan2.Text == "" || txtPan3.Text == "")
                {
                    MessageBox.Show("Lipsesc date !", MessageBox.icoWarning, "");
                }
                else
                {
                    CriptDecript prc = new CriptDecript();
                    string parolaDinBaza = prc.EncryptString(Constante.cheieCriptare, dr["F70103"].ToString(), 2);

                    if (dr == null || parolaDinBaza != txtPan1.Text)
                    {
                        MessageBox.Show("Parola actuala eronata", MessageBox.icoWarning, "");
                    }
                    else
                    {
                        if (txtPan2.Text != txtPan3.Text)
                        {
                            MessageBox.Show("Parola nu coincide !", MessageBox.icoWarning, "");
                        }
                        else
                        {
                            if (General.VarSession("ParolaComplexa").ToString() == "1")
                            {
                                var ras = General.VerificaComplexitateParola(txtPan2.Text);
                                if (ras != "")
                                {
                                    MessageBox.Show(ras, MessageBox.icoWarning, "Parola invalida");
                                    return;
                                }
                            }

                            string idUser = General.Nz(HttpContext.Current.Session["UserId"], -99).ToString();
                            General.AddUserIstoric(idUser);

                            string parolaCriptata = prc.EncryptString(Constante.cheieCriptare, txtPan2.Text, 1);

                            string strSql = @"UPDATE USERS SET F70103=@1, F70113=0, F70122=GETDATE() WHERE F70102=@2";
                            if (Constante.tipBD == 2) strSql = @"UPDATE USERS SET F70103=@1, F70113=0, F70122=SYSDATE WHERE F70102=@2";
                            General.ExecutaNonQuery(strSql, new string[] { parolaCriptata, Session["UserId"].ToString() });
                            Session["SecApp"] = "OK";
                            MessageBox.Show("Proces realizat cu succes", MessageBox.icoWarning, "", "MainPage.aspx");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}