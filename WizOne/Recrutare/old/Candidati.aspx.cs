using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Recrutare
{
    public partial class Candidati : System.Web.UI.Page
    {

 
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                lblNume.InnerText = Dami.TraduCuvant("Nume");
                lblPrenume.InnerText = Dami.TraduCuvant("Prenume");
                lblCanal.InnerText = Dami.TraduCuvant("Canal de recrutare");
                lblJud.InnerText = Dami.TraduCuvant("Judet");
                lblLoc.InnerText = Dami.TraduCuvant("Localitate");
                lblMail.InnerText = Dami.TraduCuvant("Mail");
                lblAdr.InnerText = Dami.TraduCuvant("Adresa completa");
                lblObs.InnerText = Dami.TraduCuvant("Observatii");

                #endregion

                if (!IsPostBack)
                {
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                    DataTable dtCan = GeneralRec.IncarcaDT("SELECT * FROM Rec_tblCanale", null);
                    cmbCanal.DataSource = dtCan;
                    cmbCanal.DataBind();

                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            //NOP
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                DataTable dt = GeneralRec.IncarcaDT(@"SELECT * FROM ""Rec_Candidati"" WHERE Id=@1 ", new object[] { Session["Sablon_CheiePrimara"] });
                                if (dt.Rows.Count > 0)
                                {
                                    DataRow dr = dt.Rows[0];
                                    cmbCanal.Value = Convert.ToInt32(General.Nz(dr["CanalRecrutare"], "1"));
                                    txtNume.Text = General.Nz(dr["Nume"],"").ToString();
                                    txtPrenume.Text = General.Nz(dr["Prenume"], "").ToString();
                                    txtJud.Text = General.Nz(dr["Judet"], "").ToString();
                                    txtLoc.Text = General.Nz(dr["Localitate"], "").ToString();
                                    txtMail.Text = General.Nz(dr["Mail"], "").ToString();
                                    txtAdr.Text = General.Nz(dr["AdresaCompleta"], "").ToString();
                                    txtObs.Text = General.Nz(dr["Observatii"], "").ToString();
                                }
                            }
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strErr = "";

                if (cmbCanal.Value == null) strErr += ", " + Dami.TraduCuvant("canal de recrutare");
                if (txtNume.Value == null) strErr += ", " + Dami.TraduCuvant("nume");
                if (txtPrenume.Value == null) strErr += ", " + Dami.TraduCuvant("prenume");
                if (txtLoc.Text == "") strErr += ", " + Dami.TraduCuvant("localitate");
                if (txtJud.Text == "") strErr += ", " + Dami.TraduCuvant("judet");
                if (txtMail.Text == "") strErr += ", " + Dami.TraduCuvant("mail");
                if (txtAdr.Text == "") strErr += ", " + Dami.TraduCuvant("adresa");

                if (strErr != "")
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    return;
                }


                int id = -99;
                if (Session["Sablon_TipActiune"].ToString() == "Edit") id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = GeneralRec.IncarcaDT("SELECT * FROM Rec_Candidati WHERE Id=@1", new object[] { id.ToString() });

                DataRow dr = dt.NewRow();

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        //NOP
                        break;
                    case "Edit":
                            dr = dt.Rows[0];
                        break;
                }

                dr["Nume"] = txtNume.Text;
                dr["Prenume"] = txtPrenume.Text;
                dr["CanalRecrutare"] = cmbCanal.Value;
                dr["Localitate"] = txtLoc.Text;
                dr["Judet"] = txtJud.Text;
                dr["Mail"] = txtMail.Text;
                dr["AdresaCompleta"] = txtAdr.Text;
                dr["Observatii"] = txtObs.Text;

                dr["TIME"] = DateTime.Now;
                dr["USER_NO"] = Session["UserId"];

                if (Session["Sablon_TipActiune"].ToString() == "New" || Session["Sablon_TipActiune"].ToString() == "Clone") dt.Rows.Add(dr);

                //string msg = Notif.TrimiteNotificare("tbl.Stiri", (int)Constante.TipNotificare.Validare, General.CreazaSelectFromRow(dr), "", id, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                //if (msg != "" && msg == Constante.MesajeValidari.MesajDeEroare.ToString())
                //{
                //    return;
                //}
                //else
                //{
                //    General.SalveazaDate(dt, "Stiri");
                //    Notif.TrimiteNotificare("tbl.Stiri", (int)Constante.TipNotificare.Notificare, @"SELECT * FROM ""Stiri"" WHERE ""Id""=" + id, "Stiri", id, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                //    if (msg == "") Iesire();
                //}

                GeneralRec.SalveazaDate(dt, "Rec_Candidati");
                Iesire();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void Iesire()
        {
            try
            {
                HttpContext.Current.Session["Sablon_Tabela"] = "Rec_Candidati";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}