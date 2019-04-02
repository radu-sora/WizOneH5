using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Recrutare
{
    public partial class DateGenerale : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtCan = General.IncarcaDT("SELECT * FROM Rec_tblCanale", null);
                cmbCanal.DataSource = dtCan;
                cmbCanal.DataBind();

                DataTable dtJud = General.IncarcaDT("SELECT JudetID AS Id, DENJUD AS Denumire FROM JUDETE ORDER BY DENJUD", null);
                cmbJud.DataSource = dtJud;
                cmbJud.DataBind();


                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                frmGen.DataSource = ds.Tables["Rec_Candidati"];
                frmGen.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Rec_Candidati"];

                DataRow dr = dt.NewRow();
                if (dt.Rows.Count > 0)
                    dr = dt.Rows[0];
                else
                    dr["Id"] = Session["Sablon_CheiePrimara"];

                dr["CanalRecrutare"] = General.Nz(cmbCanal.Value, DBNull.Value);
                dr["Nume"] = General.Nz(txtNume.Value,DBNull.Value);
                dr["Prenume"] = General.Nz(txtPrenume.Value, DBNull.Value);
                dr["DataNastere"] = General.Nz(txtDataNastere.Value, DBNull.Value);
                dr["CNP"] = General.Nz(txtCNP.Value, DBNull.Value);
                dr["AdresaCompleta"] = General.Nz(txtAdr.Value, DBNull.Value);
                dr["Judet"] = General.Nz(cmbJud.Text, DBNull.Value);
                dr["IdJudet"] = General.Nz(cmbJud.Value, DBNull.Value);
                dr["Localitate"] = General.Nz(txtLoc.Value, DBNull.Value);
                dr["Mail"] = General.Nz(txtMail.Value, DBNull.Value);
                dr["Telefon"] = General.Nz(txtTel.Value, DBNull.Value);
                dr["Linkedin"] = General.Nz(txtLinkedin.Value, DBNull.Value);
                dr["Facebook"] = General.Nz(txtFacebook.Value, DBNull.Value);
                dr["Twitter"] = General.Nz(txtTwitter.Value, DBNull.Value);
                dr["Instagram"] = General.Nz(txtInstagram.Value, DBNull.Value);
                dr["Permis"] = General.Nz(chkPermis.Value, DBNull.Value);
                dr["Deplasari"] = General.Nz(chkDeplasare.Value, DBNull.Value);
                dr["Relocare"] = General.Nz(chkRelocare.Value, DBNull.Value);
                dr["TIME"] = DateTime.Now;
                dr["USER_NO"] = Session["UserId"];

                if (dt.Rows.Count == 0)
                    dt.Rows.Add(dr);

                Session["InformatiaCurenta"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}